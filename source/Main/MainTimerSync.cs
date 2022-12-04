using System.Diagnostics;
using Age.Core.Config;

namespace Age.Main;

internal record struct MainFrameTime
{
    public double ProcessStep           { get; set; }
    public int    PhysicsSteps          { get; set; }
    public double InterpolationFraction { get; set; }

    public void ClampProcessStep(double minProcessStep, double maxProcessStep)
    {
        if (this.ProcessStep < minProcessStep)
        {
            this.ProcessStep = minProcessStep;
        }
        else if (this.ProcessStep > maxProcessStep)
        {
            this.ProcessStep = maxProcessStep;
        }
    }
}

internal class MainTimerSync
{
    private const int CONTROL_STEPS = 12;
    private readonly int[] accumulatedPhysicsSteps = new int[12];
    private readonly int[] typicalPhysicsSteps     = new int[12];

    private long   CurrentCpuTicksUsec { get; set; }
    private int    FixedFps            { get; set; }

    private long   lastCpuTicksUsec = 0;
    private double timeAccumulated  = 0;
    private double timeDeficit      = 0;

    private static void WARN_PRINT_ONCE(string message) => throw new NotImplementedException();

    private MainFrameTime AdvanceCore(double physicsStep, int physicsTicksPerSecond, double processStep)
    {
        this.timeAccumulated += processStep;

        var ret = new MainFrameTime()
        {
            ProcessStep  = processStep,
            PhysicsSteps = (int)Math.Floor(this.timeAccumulated * physicsTicksPerSecond),
        };

        var minTypicalSteps = this.typicalPhysicsSteps[0];
        var maxTypicalSteps = minTypicalSteps + 1;

        var updateTypical = false;

        for (var i = 0; i < CONTROL_STEPS; i++)
        {
            var stepsLeftToMatchTypical = this.typicalPhysicsSteps[i + 1] - this.accumulatedPhysicsSteps[i];

            if (stepsLeftToMatchTypical > maxTypicalSteps || stepsLeftToMatchTypical + 1 < minTypicalSteps)
            {
                updateTypical = true;

                break;
            }

            if (stepsLeftToMatchTypical > minTypicalSteps)
            {
                minTypicalSteps = stepsLeftToMatchTypical;
            }
            if (stepsLeftToMatchTypical + 1 < maxTypicalSteps)
            {
                maxTypicalSteps = stepsLeftToMatchTypical + 1;
            }
        }

        if (Debugger.IsAttached && maxTypicalSteps < 0)
        {
            WARN_PRINT_ONCE($"`{nameof(maxTypicalSteps)}` is negative. This could hint at an engine bug or system timer misconfiguration.");
        }

        if (ret.PhysicsSteps < minTypicalSteps)
        {
            var maxPossibleSteps = (int)Math.Floor(this.timeAccumulated * physicsTicksPerSecond + Engine.Singleton.PhysicsJitterFix);

            if (maxPossibleSteps < minTypicalSteps)
            {
                ret.PhysicsSteps = maxPossibleSteps;
                updateTypical = true;
            }
            else
            {
                ret.PhysicsSteps = minTypicalSteps;
            }
        }
        else if (ret.PhysicsSteps > maxTypicalSteps)
        {
            var minPossibleSteps = (int)Math.Floor(this.timeAccumulated * physicsTicksPerSecond - Engine.Singleton.PhysicsJitterFix);

            if (minPossibleSteps > maxTypicalSteps)
            {
                ret.PhysicsSteps = minPossibleSteps;
                updateTypical = true;
            }
            else
            {
                ret.PhysicsSteps = maxTypicalSteps;
            }
        }

        if (ret.PhysicsSteps < 0)
        {
            ret.PhysicsSteps = 0;
        }

        this.timeAccumulated -= ret.PhysicsSteps * physicsStep;

        // keep track of accumulated step counts
        for (var i = CONTROL_STEPS - 2; i >= 0; --i)
        {
            this.accumulatedPhysicsSteps[i + 1] = this.accumulatedPhysicsSteps[i] + ret.PhysicsSteps;
        }
        this.accumulatedPhysicsSteps[0] = ret.PhysicsSteps;

        if (updateTypical)
        {
            for (var i = CONTROL_STEPS - 1; i >= 0; --i)
            {
                if (this.typicalPhysicsSteps[i] > this.accumulatedPhysicsSteps[i])
                {
                    this.typicalPhysicsSteps[i] = this.accumulatedPhysicsSteps[i];
                }
                else if (this.typicalPhysicsSteps[i] < this.accumulatedPhysicsSteps[i] - 1)
                {
                    this.typicalPhysicsSteps[i] = this.accumulatedPhysicsSteps[i] - 1;
                }
            }
        }

        return ret;
    }

    private (int Steps, int Min, int Max) GetAveragePhysicsSteps()
    {
        var min = this.typicalPhysicsSteps[0];
        var max = min + 1;

        for (var i = 1; i < CONTROL_STEPS; ++i)
        {
            var typicalLower = this.typicalPhysicsSteps[i];
            var currentMin   = typicalLower / (i + 1);

            if (currentMin > max)
            {
                return (i, min, max);
            }
            else if (currentMin > min)
            {
                min = currentMin;
            }

            var currentMax = (typicalLower + 1) / (i + 1);

            if (currentMax < min)
            {
                return (i, min, max);
            }
            else if (currentMax < max)
            {
                max = currentMax;
            }
        }

        return (CONTROL_STEPS, min, max);
    }

    private double GetCpuProcessStep()
    {
        var cpuTicksElapsed = this.CurrentCpuTicksUsec - this.lastCpuTicksUsec;
        this.lastCpuTicksUsec = this.CurrentCpuTicksUsec;

        return cpuTicksElapsed / 1000000.0;
    }

    public MainFrameTime Advance(double physicsStep, int physicsTicksPerSecond)
    {
        var cpuProcessStep = this.GetCpuProcessStep();

        return this.AdvanceChecked(physicsStep, physicsTicksPerSecond, cpuProcessStep);
    }

    public MainFrameTime AdvanceChecked(double physicsStep, int physicsTicksPerSecond, double processStep)
    {
        if (this.FixedFps != -1)
        {
            processStep = 1.0 / this.FixedFps;
        }

        var minOutputStep = Math.Max(processStep / 8, 1e-6);

        processStep += this.timeDeficit;

        var ret = this.AdvanceCore(physicsStep, physicsTicksPerSecond, processStep);

        var processMinusAccumulated = ret.ProcessStep - this.timeAccumulated;

		var (consistentSteps, minAveragePhysicsSteps, maxAveragePhysicsSteps) = this.GetAveragePhysicsSteps();

		if (consistentSteps > 3)
        {
			ret.ClampProcessStep(minAveragePhysicsSteps * physicsStep, maxAveragePhysicsSteps * physicsStep);
		}

        var maxClockDeviation = Engine.Singleton.PhysicsJitterFix * physicsStep;

        ret.ClampProcessStep(processStep - maxClockDeviation, processStep + maxClockDeviation);
        ret.ClampProcessStep(processMinusAccumulated, processMinusAccumulated + physicsStep);

        if (ret.ProcessStep < minOutputStep)
        {
            ret.ProcessStep = minOutputStep;
        }

        this.timeAccumulated = ret.ProcessStep - processMinusAccumulated;

        if (Debugger.IsAttached && this.timeAccumulated < -1e-7)
        {
		    WARN_PRINT_ONCE($"Intermediate value of `{nameof(this.timeAccumulated)}` is negative. This could hint at an engine bug or system timer misconfiguration.");
	    }

        if (this.timeAccumulated > physicsStep)
        {
            var extraPhysicsSteps = (int)Math.Floor(this.timeAccumulated * physicsTicksPerSecond);

            this.timeAccumulated -= extraPhysicsSteps * physicsStep;
            ret.PhysicsSteps += extraPhysicsSteps;
        }

        if (Debugger.IsAttached)
        {
            if (this.timeAccumulated < -1e-7)
            {
                WARN_PRINT_ONCE($"Final value of `{this.timeAccumulated}` is negative. It should always be between 0 and `p_physics_step`. This hints at an engine bug.");
            }

            if (this.timeAccumulated > physicsStep + 1e-7)
            {
                WARN_PRINT_ONCE($"Final value of `{this.timeAccumulated}` is larger than `p_physics_step`. It should always be between 0 and `p_physics_step`. This hints at an engine bug.");
            }
        }

        this.timeDeficit = processStep - ret.ProcessStep;

        ret.InterpolationFraction = this.timeAccumulated / physicsStep;

        return ret;
    }
}
