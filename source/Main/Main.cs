#define TOOLS_ENABLED
#define VULKAN_ENABLED
#define GLES3_ENABLED

using System.Globalization;
using System.Runtime.CompilerServices;
using Age.Core.Config;
using Age.Core.Error;
using Age.Core.Math;
using Age.Core.OS;
using Age.Servers;
using Age.Servers.Rendering;
using static Age.Core.String.PrintString;
using static Age.Modules.IRegisterModulesTypes;
using static Age.Core.Config.Macros;
using RealT = System.Single;

namespace Age.Main;

#pragma warning disable IDE0044,IDE0059,IDE0052,CS0219,CS0414,IDE0051,CS0649,IDE0060 // TODO - Remove

internal class Main
{
    private static readonly ProjectSettings globals       = new();
    private static readonly MainTimerSync   mainTimerSync = new();

    private static bool                            autoQuit             = false;
    private static CameraServer                    cameraServer         = null!;
    private static bool                            cmdlineTool          = false;
    private static bool                            disableRenderLoop    = false;
    private static bool                            disableVsync         = false;
    private static int                             displayDriverIdx     = -1;
    private static DisplayServer?                  displayServer        = null;
    private static bool                            editor               = false;
    private static Engine                          engine               = null!;
    private static int                             fixedFps             = 0;
    private static bool                            forceRedrawRequested = false;
    private static bool                            foundProject         = false;
    private static uint                            frame                = 0;
    private static int                             frameDelay;
    private static uint                            frames               = 0;
    private static uint                            hidePrintFpsAttempts = 3;
    private static bool                            initAlwaysOnTop;
    private static Vector2<int>                    initCustomPos;
    private static bool                            initFullscreen;
    private static bool                            initMaximized;
    private static int                             initScreen;
    private static bool                            initUseCustomPos;
    private static bool                            initWindowed;
    private static int                             iterating            = 0;
    private static uint                            lastTicks            = 0;
    private static Performance                     performance          = null!;
    private static long                            physicsProcessMax    = 0;
    private static bool                            printFps             = false;
    private static long                            processMax           = 0;
    private static bool                            profileGpu;
    private static bool                            projectManager       = false;
    private static string?                         renderingDriver      = null;
    private static string?                         renderingMethod      = null;
    private static RenderingServer                 renderingServer      = null!;
    private static DisplayServer.WindowFlagsBit    windowFlags          = DisplayServer.WindowFlagsBit.NONE;
    private static DisplayServer.WindowMode        windowMode           = DisplayServer.WindowMode.NONE;
    private static DisplayServer.ScreenOrientation windowOrientation    = DisplayServer.ScreenOrientation.NONE;
    private static Vector2<int>                    windowSize;
    private static DisplayServer.VSyncMode         windowVsyncMode      = DisplayServer.VSyncMode.NONE;
    private static bool startSuccess;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void ERR_PRINT(string message) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void MAIN_PRINT(string message) => PrintLine(message);

    private static ErrorType Setup2()
    {
        // print_line(String(VERSION_NAME) + " v" + get_full_version_string() + " - " + String(VERSION_WEBSITE));

        engine.StartupBenchmarkBeginMeasure("servers");

        // TODO - main\main.cpp[1952:1961]

        RegisterServerTypes();
        InitializeModules(ModuleInitializationLevel.MODULE_INITIALIZATION_LEVEL_SERVERS);

        // TODO - main\main.cpp[1965:1986]

        var displayDriver = DisplayServer.ServerCreateFunctions[displayDriverIdx].Name;

        var windowPosition = new Vector2<int>();
        var position = initCustomPos;

        if (initUseCustomPos)
        {
            windowPosition = position;
        }

        var err = ErrorType.OK;

        displayServer = DisplayServer.Create(
            displayDriverIdx,
            renderingDriver!,
            windowMode,
            windowVsyncMode,
            windowFlags,
            out windowPosition,
            windowSize,
            out err
        );

        if (err != ErrorType.OK || displayServer == null)
        {
            for (var i = 0; i < DisplayServer.ServerCreateFunctions.Count - 1; i++)
            {
                if (i == displayDriverIdx)
                {
                    continue; // Don't try the same twice.
                }

                displayServer = DisplayServer.Create(
                    i,
                    renderingDriver!,
                    windowMode,
                    windowVsyncMode,
                    windowFlags,
                    out windowPosition,
                    windowSize,
                    out err
                );

                if (err == ErrorType.OK && displayServer != null)
                {
                    break;
                }
            }
        }

        if (err != ErrorType.OK || displayServer == null)
        {
            ERR_PRINT("Unable to create DisplayServer, all display drivers failed.");
            return err;
        }

        if (displayServer.HasFeature(DisplayServer.Feature.FEATURE_ORIENTATION))
        {
            displayServer.ScreenSetOrientation(windowOrientation);
        }

        if (GLOBAL_GET<bool>("debug/settings/stdout/print_fps") || printFps)
        {
            switch (windowVsyncMode)
            {
                case DisplayServer.VSyncMode.VSYNC_DISABLED:
                    PrintLine("Requested V-Sync mode: Disabled");
                    break;
                case DisplayServer.VSyncMode.VSYNC_ENABLED:
                    PrintLine("Requested V-Sync mode: Enabled - FPS will likely be capped to the monitor refresh rate.");
                    break;
                case DisplayServer.VSyncMode.VSYNC_ADAPTIVE:
                    PrintLine("Requested V-Sync mode: Adaptive");
                    break;
                case DisplayServer.VSyncMode.VSYNC_MAILBOX:
                    PrintLine("Requested V-Sync mode: Mailbox");
                    break;
            }
        }

        GLOBAL_DEF_RST_NOVAL("input_devices/pen_tablet/driver", "");
		GLOBAL_DEF_RST_NOVAL("input_devices/pen_tablet/driver.windows", "");
		ProjectSettings.Singleton.Set("input_devices/pen_tablet/driver.windows", new PropertyInfo(Variant.STRING, "input_devices/pen_tablet/driver.windows", PropertyHint.PROPERTY_HINT_ENUM, "wintab,winink"));

        // TODO - main\main.cpp[2053:2071]

        renderingServer = new RenderingServerDefault(OS.Singleton.RenderThreadMode == RenderThreadMode.RENDER_SEPARATE_THREAD);

        renderingServer.Init();

        renderingServer.RenderLoopEnabled = !disableRenderLoop;

        if (profileGpu || (!editor && GLOBAL_GET<bool>("debug/settings/stdout/print_gpu_profile")))
        {
            renderingServer.PrintGpuProfile = true;
        }

        // TODO - main\main.cpp[2081:2106]

        if (initUseCustomPos)
        {
            displayServer.WindowSetPosition(initCustomPos);
        }

        // TODO - main\main.cpp[2114:2118]

        RegisterCoreSingletons();

        MAIN_PRINT("Main: Setup Logo");

        #if !TOOLS_ENABLED && (WEB_ENABLED || ANDROID_ENABLED)
        var show_logo = false;
        #else
	    var show_logo = true;
        #endif

        if (initScreen != -1)
        {
            DisplayServer.Singleton.WindowSetCurrentScreen(initScreen);
        }
        if (initWindowed)
        {
            //do none..
        }
        else if (initMaximized)
        {
            DisplayServer.Singleton.WindowSetMode(DisplayServer.WindowMode.WINDOW_MODE_MAXIMIZED);
        }
        else if (initFullscreen)
        {
            DisplayServer.Singleton.WindowSetMode(DisplayServer.WindowMode.WINDOW_MODE_FULLSCREEN);
        }
        if (initAlwaysOnTop)
        {
            DisplayServer.Singleton.WindowSetFlag(DisplayServer.WindowFlags.WINDOW_FLAG_ALWAYS_ON_TOP, true);
        }

        // TODO - main\main.cpp[2144:2312]

        engine.StartupBenchmarkEndMeasure();

        MAIN_PRINT("Main: Load Scene Types");

        engine.StartupBenchmarkBeginMeasure("scene");

        RegisterSceneTypes();
        RegisterDriverTypes();

        InitializeModules(ModuleInitializationLevel.MODULE_INITIALIZATION_LEVEL_SCENE);

        // TODO - main\main.cpp[2324:2334]

        MAIN_PRINT("Main: Load Modules");

        RegisterPlatformApis();

        // Theme needs modules to be initialized so that sub-resources can be loaded.
        InitializeThemeDb();
        RegisterSceneSingletons();

        GLOBAL_DEF_BASIC("display/mouse_cursor/custom_image", "");
        GLOBAL_DEF_BASIC("display/mouse_cursor/custom_image_hotspot", new Vector2<RealT>());
        GLOBAL_DEF_BASIC("display/mouse_cursor/tooltip_position_offset", new Vector2<RealT>(10, 10));
        ProjectSettings.Singleton.Set(
            "display/mouse_cursor/custom_image",
            new PropertyInfo(Variant.STRING,
                "display/mouse_cursor/custom_image",
                PropertyHint.PROPERTY_HINT_FILE,
                "*.png,*.webp"
            )
        );

        // TODO - main\main.cpp[2352:2359]

        cameraServer = CameraServer.Create();

        // TODO - main\main.cpp[2363:2366]

        RegisterServerSingletons();

        // TODO - main\main.cpp[2369:2386]

        startSuccess = true;

        // TODO - main\main.cpp[2390:2393]

        MAIN_PRINT("Main: Done");

        engine.StartupBenchmarkEndMeasure(); // scene

        return ErrorType.OK;
    }

    private static void InitializeModules(ModuleInitializationLevel level)
    {
        // TODO
    }

    private static void InitializeThemeDb()                                => throw new NotImplementedException();
    private static void RegisterCoreSingletons()                           => throw new NotImplementedException();
    private static void RegisterDriverTypes()                              => throw new NotImplementedException();
    private static void RegisterPlatformApis()                             => throw new NotImplementedException();
    private static void RegisterSceneSingletons()                          => throw new NotImplementedException();
    private static void RegisterSceneTypes()                               => throw new NotImplementedException();
    private static void RegisterServerSingletons()                         => throw new NotImplementedException();
    private static void RegisterServerTypes()
    {
        // TODO
    }

    private static void UnregisterCoreDriverTypes()
    {
        // TODO
    }

    private static void UnregisterCoreExtensions()
    {
        // TODO
    }

    private static void UnregisterCoreTypes()
    {
        // TODO
    }

    public static void Cleanup() => throw new NotImplementedException();

    public static bool Iteration()
    {
        iterating++;

        var ticks = (uint)DateTime.Now.Ticks;

        Engine.Singleton.FrameTicks = ticks;

        var ticksElapsed = ticks - lastTicks;

        var physicsStep = 1.0 / Engine.Singleton.PhysicsTicksPerSecond;
        var timeScale   = Engine.Singleton.TimeScale;

        var advance = mainTimerSync.Advance(physicsStep, Engine.Singleton.PhysicsTicksPerSecond);

        var processStep = advance.ProcessStep;
        var scaledStep  = advance.ProcessStep * timeScale;

        Engine.Singleton.ProcessStep                  = processStep;
        Engine.Singleton.PhysicsInterpolationFraction = advance.InterpolationFraction;

        // var physicsProcessTicks = 0uL;
        var processTicks = 0L;

        frame += ticksElapsed;

        lastTicks = ticks;

        var maxPhysicsSteps = Engine.Singleton.MaxPhysicsStepsPerFrame;

        if (fixedFps == -1 && advance.PhysicsSteps > maxPhysicsSteps)
        {
            processStep -= (advance.PhysicsSteps - maxPhysicsSteps) * physicsStep;
            advance.PhysicsSteps = maxPhysicsSteps;
        }

        var exit = false;

        // TODO - main\main.cpp[3146:3189]

        var processBegin = DateTime.Now.Ticks;

        if (OS.Singleton.MainLoop.Process(processStep * timeScale))
        {
            exit = true;
        }

        // TODO - main\main.cpp[3146:3196]

        RenderingServer.Singleton.Sync();

        if (DisplayServer.Singleton.CanAnyWindowDraw && RenderingServer.Singleton.IsRenderLoopEnabled)
        {
            if (!forceRedrawRequested && OS.Singleton.IsInLowProcessorUsageMode)
            {
                if (RenderingServer.Singleton.HasChanged)
                {
                    RenderingServer.Singleton.Draw(true, scaledStep);
                    Engine.Singleton.FramesDrawn++;
                }
            }
            else
            {
                RenderingServer.Singleton.Draw(true, scaledStep);
                Engine.Singleton.FramesDrawn++;
                forceRedrawRequested = false;
            }
        }

        ticks = (uint)DateTime.Now.Ticks;

        processTicks    = DateTime.Now.Ticks - processBegin;
        processMax      = Math.Max(processTicks, processMax);
        var frameTime   = DateTime.Now.Ticks - ticks;

        // TODO - main\main.cpp[3218:3226]

        frames++;

        Engine.Singleton.ProcessFrames++;

        if (frame > 1000000)
        {
            if (hidePrintFpsAttempts == 0)
            {
                if (editor || projectManager)
                {
                    if (printFps)
                    {
                        PrintLine($"Editor FPS: {frames} ({(1000.0 / frames).ToString("0.00", CultureInfo.InvariantCulture)} mspf)");
                    }
                }
                else if (printFps || ProjectSettings.Singleton.Get<bool>("debug/settings/stdout/print_fps"))
                {
                    PrintLine($"Project FPS: {frames} ({(1000.0 / frames).ToString("0.00", CultureInfo.InvariantCulture)} mspf)");
                }
            }
            else
            {
                hidePrintFpsAttempts--;
            }

            Engine.Singleton.Fps = frames;

            performance.ProcessTime        = processMax / TimeSpan.TicksPerSecond;
            performance.PhysicsProcessTime = physicsProcessMax / TimeSpan.TicksPerSecond;

            processMax        = 0;
            physicsProcessMax = 0;

            frame %= 1000000;
            frames = 0;
        }

        iterating--;

        // TODO - main\main.cpp[3258:3267]

        if (fixedFps != -1)
        {
            return exit;
        }

        OS.Singleton.AddFrameDelay(DisplayServer.Singleton.WindowCanDraw());

        // TODO - main\main.cpp[3275:3288]

        return exit || autoQuit;
    }

    public static ErrorType Setup(string[] args, bool secondPhase)
    {
        // TODO - Review non GLOBAL_DEF

        OS.Singleton.Initialize();

        engine = new Engine();

        MAIN_PRINT("Main: Initialize CORE");

        engine.StartupBegin();
        engine.StartupBenchmarkBeginMeasure("core");

        RegisterCoreTypes();
        RegisterCoreDriverTypes();

        MAIN_PRINT("Main: Initialize Globals");

        // TODO - main\main.cpp[658:660]

        RegisterCoreSettings(); //here globals are present

        // TODO - main\main.cpp[664]

        performance = new();
        // TODO - main\main.cpp[666]
        engine.AddSingleton(performance);


        GLOBAL_DEF("application/run/flush_stdout_on_print", false);
        GLOBAL_DEF("application/run/flush_stdout_on_print.debug", true);

        // TODO - main\main.cpp[674:698]

        var displayDriver   = "";
        var audioDriver     = "";
        var projectPath     = ".";
        var upwards         = false;
        var debugUri        = "";
        var skipBreakpoints = false;
        var mainPack        = default(string);
        var quietStdout     = false;
        var rtm             = -1;

        var remotefs     = default(string);
        var remotefsPass = default(string);

        var breakpoints  = default(string[]);
        var useCustomRes = true;
        var forceRes     = false;

        var defaultRenderer       = "";
        var defaultRendererMobile = "";
        var rendererHints         = "";

        var exitCode = ErrorType.ERR_INVALID_PARAMETER;

        // TODO - main\main.cpp[721:1360]

        ProjectSettings.Singleton.Set("memory/limits/multithreaded_server/rid_pool_prealloc",
			new PropertyInfo(
                Variant.INT,
				"memory/limits/multithreaded_server/rid_pool_prealloc",
				PropertyHint.PROPERTY_HINT_RANGE,
				"0,500,1"
            )
        );
        GLOBAL_DEF("network/limits/debugger/max_chars_per_second", 32768);
        ProjectSettings.Singleton.Set(
            "network/limits/debugger/max_chars_per_second",
            new PropertyInfo(
                Variant.INT,
                "network/limits/debugger/max_chars_per_second",
                PropertyHint.PROPERTY_HINT_RANGE,
                "0, 4096, 1, or_greater"
            )
        );
        GLOBAL_DEF("network/limits/debugger/max_queued_messages", 2048);
        ProjectSettings.Singleton.Set(
            "network/limits/debugger/max_queued_messages",
            new PropertyInfo(
                Variant.INT,
                "network/limits/debugger/max_queued_messages",
                PropertyHint.PROPERTY_HINT_RANGE,
                "0, 8192, 1, or_greater"
            )
        );
        GLOBAL_DEF("network/limits/debugger/max_errors_per_second", 400);
        ProjectSettings.Singleton.Set(
            "network/limits/debugger/max_errors_per_second",
            new PropertyInfo(
                Variant.INT,
                "network/limits/debugger/max_errors_per_second",
                PropertyHint.PROPERTY_HINT_RANGE,
                "0, 200, 1, or_greater"
            )
        );
        GLOBAL_DEF("network/limits/debugger/max_warnings_per_second", 400);
        ProjectSettings.Singleton.Set(
            "network/limits/debugger/max_warnings_per_second",
            new PropertyInfo(
                Variant.INT,
                "network/limits/debugger/max_warnings_per_second",
                PropertyHint.PROPERTY_HINT_RANGE,
                "0, 200, 1, or_greater"
            )
        );

        // TODO - main\main.cpp[1392:1408]

        if (!projectManager && !editor)
        {
            projectManager = !foundProject && !cmdlineTool;
        }

        if (projectManager)
        {
            engine.ProjectManagerHint = true;
        }

        GLOBAL_DEF("debug/file_logging/enable_file_logging", false);
        GLOBAL_DEF("debug/file_logging/enable_file_logging.pc", true);
        GLOBAL_DEF("debug/file_logging/log_path", "user://logs/age.log");
        GLOBAL_DEF("debug/file_logging/max_log_files", 5);
        ProjectSettings.Singleton.Set(
            "debug/file_logging/max_log_files",
			new PropertyInfo(Variant.INT,
                "debug/file_logging/max_log_files",
				PropertyHint.PROPERTY_HINT_RANGE,
				"0,20,1,or_greater"
            )
        );

        // TODO - main\main.cpp[1433:1476]

        var driverHints = "";

        #if VULKAN_ENABLED
        driverHints = "vulkan";
        #endif

        var defaultDriver = driverHints.Split(',').FirstOrDefault() ?? "";

        GLOBAL_DEF("rendering/rendering_device/driver", defaultDriver);
        GLOBAL_DEF("rendering/rendering_device/driver.windows", defaultDriver);

        ProjectSettings.Singleton.Set(
            "rendering/rendering_device/driver.windows",
			new PropertyInfo(
                Variant.STRING,
                "rendering/rendering_device/driver.windows",
                PropertyHint.PROPERTY_HINT_ENUM,
                driverHints
            )
        );
		GLOBAL_DEF("rendering/rendering_device/driver.linuxbsd", defaultDriver);
		ProjectSettings.Singleton.Set(
            "rendering/rendering_device/driver.linuxbsd",
			new PropertyInfo(
                Variant.STRING,
                "rendering/rendering_device/driver.linuxbsd",
                PropertyHint.PROPERTY_HINT_ENUM,
                driverHints
            )
        );
		GLOBAL_DEF("rendering/rendering_device/driver.android", defaultDriver);
		ProjectSettings.Singleton.Set(
            "rendering/rendering_device/driver.android",
			new PropertyInfo(
                Variant.STRING,
                "rendering/rendering_device/driver.android",
                PropertyHint.PROPERTY_HINT_ENUM,
                driverHints
            )
        );
		GLOBAL_DEF("rendering/rendering_device/driver.ios", defaultDriver);
		ProjectSettings.Singleton.Set(
            "rendering/rendering_device/driver.ios",
            new PropertyInfo(
                Variant.STRING,
                "rendering/rendering_device/driver.ios",
                PropertyHint.PROPERTY_HINT_ENUM,
                driverHints
            )
        );
		GLOBAL_DEF("rendering/rendering_device/driver.macos", defaultDriver);
		ProjectSettings.Singleton.Set(
            "rendering/rendering_device/driver.macos",
            new PropertyInfo(
                Variant.STRING,
                "rendering/rendering_device/driver.macos",
                PropertyHint.PROPERTY_HINT_ENUM,
                driverHints
            )
        );

        driverHints = "";

        #if GLES3_ENABLED
        driverHints = "opengl3";
        #endif

        defaultDriver = driverHints.Split(',').FirstOrDefault() ?? "";

		GLOBAL_DEF("rendering/gl_compatibility/driver", defaultDriver);
		GLOBAL_DEF("rendering/gl_compatibility/driver.windows", defaultDriver);
		ProjectSettings.Singleton.Set(
            "rendering/gl_compatibility/driver.windows",
			new PropertyInfo(
                Variant.STRING,
                "rendering/gl_compatibility/driver.windows",
                PropertyHint.PROPERTY_HINT_ENUM,
                driverHints
            )
        );
		GLOBAL_DEF("rendering/gl_compatibility/driver.linuxbsd", defaultDriver);
		ProjectSettings.Singleton.Set(
            "rendering/gl_compatibility/driver.linuxbsd",
			new PropertyInfo(
                Variant.STRING,
                "rendering/gl_compatibility/driver.linuxbsd",
                PropertyHint.PROPERTY_HINT_ENUM,
                driverHints
            )
        );
		GLOBAL_DEF("rendering/gl_compatibility/driver.web", defaultDriver);
		ProjectSettings.Singleton.Set(
            "rendering/gl_compatibility/driver.web",
			new PropertyInfo(
                Variant.STRING,
                "rendering/gl_compatibility/driver.web",
                PropertyHint.PROPERTY_HINT_ENUM,
                driverHints
            )
        );
		GLOBAL_DEF("rendering/gl_compatibility/driver.android", defaultDriver);
		ProjectSettings.Singleton.Set(
            "rendering/gl_compatibility/driver.android",
			new PropertyInfo(
                Variant.STRING,
                "rendering/gl_compatibility/driver.android",
                PropertyHint.PROPERTY_HINT_ENUM,
                driverHints
            )
        );
		GLOBAL_DEF("rendering/gl_compatibility/driver.ios", defaultDriver);
		ProjectSettings.Singleton.Set(
            "rendering/gl_compatibility/driver.ios",
			new PropertyInfo(
                Variant.STRING,
                "rendering/gl_compatibility/driver.ios",
                PropertyHint.PROPERTY_HINT_ENUM,
                driverHints
            )
        );
		GLOBAL_DEF("rendering/gl_compatibility/driver.macos", defaultDriver);
		ProjectSettings.Singleton.Set(
            "rendering/gl_compatibility/driver.macos",
			new PropertyInfo(
                Variant.STRING,
                "rendering/gl_compatibility/driver.macos",
                PropertyHint.PROPERTY_HINT_ENUM,
                driverHints
            )
        );

        #if VULKAN_ENABLED
        rendererHints = "forward_plus,mobile";
        defaultRendererMobile = "mobile";
        #endif

        #if GLES3_ENABLED
        if (!string.IsNullOrEmpty(rendererHints))
        {
            rendererHints += ",";
        }

        rendererHints += "gl_compatibility";
        defaultRendererMobile ??= "gl_compatibility";

        if (renderingDriver == null && renderingMethod == null && projectManager)
        {
            renderingDriver       = "opengl3";
            renderingMethod       = "gl_compatibility";
            defaultRendererMobile = "gl_compatibility";
        }
        #endif

        if (string.IsNullOrEmpty(rendererHints))
        {
            ERR_PRINT("No renderers available.");
        }

        if (!string.IsNullOrEmpty(renderingMethod))
        {
		    if (renderingMethod is not "forward_plus" and not "mobile" and not "gl_compatibility")
            {
                OS.Singleton.Print($"Unknown renderer name '{renderingMethod}', aborting. Valid options are: {rendererHints}\n");
			    goto error;
		    }
	    }

        if (!string.IsNullOrEmpty(renderingDriver))
        {
            var found = DisplayServer.ServerCreateFunctions.SelectMany(x => x.GetRenderingDriversFunction()).Any(x => renderingDriver == x);

            if (!found)
            {
                OS.Singleton.Print($"Unknown rendering driver '{renderingDriver}', aborting.\nValid options are ");

                foreach (var driver in DisplayServer.ServerCreateFunctions.SelectMany(x => x.GetRenderingDriversFunction()))
                {
                    OS.Singleton.Print($"'{driver}', ");
                }

                OS.Singleton.Print(".\n");

                goto error;
            }

            if (string.IsNullOrEmpty(renderingMethod))
            {
                if (renderingDriver == "opengl3")
                {
                    renderingMethod = "gl_compatibility";
                }
                else
                {
                    renderingMethod = "forward_plus";
                }
            }

            var validCombination = false;
		    var availableDrivers = new List<string>();
            #if VULKAN_ENABLED
            if (renderingMethod is "forward_plus" or "mobile")
            {
                availableDrivers.Add("vulkan");
            }
            #endif

            #if GLES3_ENABLED
            if (renderingMethod == "gl_compatibility")
            {
                availableDrivers.Add("opengl3");
            }
            #endif

            if (availableDrivers.Count == 0)
            {
                OS.Singleton.Print($"Unknown renderer name '{renderingMethod}', aborting.\n");

                goto error;
            }

            for (var i = 0; i < availableDrivers.Count; i++)
            {
                if (renderingDriver == availableDrivers[i])
                {
                    validCombination = true;
                    break;
                }
            }

            if (!validCombination)
            {
                OS.Singleton.Print($"Invalid renderer/driver combination '{renderingMethod}' and '{renderingDriver}', aborting. {renderingMethod} only supports the following drivers ");

                for (var d = 0; d < availableDrivers.Count; d++)
                {
                    OS.Singleton.Print($"'{availableDrivers[d]}', ");
                }

                OS.Singleton.Print(".\n");

                goto error;
            }
        }

        defaultRenderer = rendererHints.Split(',').FirstOrDefault() ?? "";

        GLOBAL_DEF_RST_BASIC("rendering/renderer/rendering_method", defaultRenderer);
        GLOBAL_DEF_RST_BASIC("rendering/renderer/rendering_method.mobile", defaultRendererMobile);
        GLOBAL_DEF_RST_BASIC("rendering/renderer/rendering_method.web", "gl_compatibility");

        ProjectSettings.Singleton.Set(
            "rendering/renderer/rendering_method",
			new PropertyInfo(
                Variant.STRING,
                "rendering/renderer/rendering_method",
                PropertyHint.PROPERTY_HINT_ENUM,
                rendererHints
            )
        );

        if (string.IsNullOrEmpty(renderingMethod))
        {
            renderingMethod = ProjectSettings.Singleton.Get<string>("rendering/renderer/rendering_method");
	    }

        if (string.IsNullOrEmpty(renderingDriver))
        {
            if (renderingMethod == "gl_compatibility")
            {
                renderingDriver = ProjectSettings.Singleton.Get<string>("rendering/gl_compatibility/driver")!;
            }
            else
            {
                renderingDriver = ProjectSettings.Singleton.Get<string>("rendering/rendering_device/driver")!;
            }
        }

        OS.Singleton.CurrentRenderingDriverName = renderingDriver;
	    OS.Singleton.CurrentRenderingMethod     = renderingMethod;

        renderingDriver = renderingDriver.ToLower();

        if (useCustomRes)
        {
            if (!forceRes)
            {
                windowSize.X = ProjectSettings.Singleton.Get<int>("display/window/size/viewport_width");
                windowSize.Y = ProjectSettings.Singleton.Get<int>("display/window/size/viewport_height");

                if (globals.Has("display/window/size/window_width_override") && ProjectSettings.Singleton.Has("display/window/size/window_height_override"))
                {
                    var desiredWidth = globals.Get<int>("display/window/size/window_width_override");

                    if (desiredWidth > 0)
                    {
                        windowSize.X = desiredWidth;
                    }

                    var desiredHeight = globals.Get<int>("display/window/size/window_height_override");

                    if (desiredHeight > 0)
                    {
                        windowSize.Y = desiredHeight;
                    }
                }
            }

            if (!ProjectSettings.Singleton.Get<bool>("display/window/size/resizable"))
            {
                windowFlags |= DisplayServer.WindowFlagsBit.WINDOW_FLAG_RESIZE_DISABLED_BIT;
            }
            if (ProjectSettings.Singleton.Get<bool>("display/window/size/borderless"))
            {
                windowFlags |= DisplayServer.WindowFlagsBit.WINDOW_FLAG_BORDERLESS_BIT;
            }
            if (ProjectSettings.Singleton.Get<bool>("display/window/size/always_on_top"))
            {
                windowFlags |= DisplayServer.WindowFlagsBit.WINDOW_FLAG_ALWAYS_ON_TOP_BIT;
            }
            if (ProjectSettings.Singleton.Get<bool>("display/window/size/transparent"))
            {
                windowFlags |= DisplayServer.WindowFlagsBit.WINDOW_FLAG_TRANSPARENT_BIT;
            }
            if (ProjectSettings.Singleton.Get<bool>("display/window/size/extend_to_title"))
            {
                windowFlags |= DisplayServer.WindowFlagsBit.WINDOW_FLAG_EXTEND_TO_TITLE_BIT;
            }
            if (ProjectSettings.Singleton.Get<bool>("display/window/size/no_focus"))
            {
                windowFlags |= DisplayServer.WindowFlagsBit.WINDOW_FLAG_NO_FOCUS_BIT;
            }

            windowMode = ProjectSettings.Singleton.Get<DisplayServer.WindowMode>("display/window/size/mode");
        }

        GLOBAL_DEF("internationalization/locale/include_text_server_data", false);

        GLOBAL_DEF("display/window/dpi/allow_hidpi",                OS.Singleton.AllowHidpi = true);
        GLOBAL_DEF("display/window/per_pixel_transparency/allowed", OS.Singleton.AllowLayered = false);

        if (editor || projectManager)
        {
            // The editor and project manager always detect and use hiDPI if needed
            OS.Singleton.AllowHidpi = true;
        }

        if (rtm == -1)
        {
            GLOBAL_DEF("rendering/driver/threads/thread_model", rtm = (int)RenderThreadMode.RENDER_THREAD_SAFE);
        }

        if (rtm >= 0 && (int)rtm < 3)
        {
            if (editor)
            {
                rtm = (int)RenderThreadMode.RENDER_THREAD_SAFE;
            }

            OS.Singleton.RenderThreadMode = (RenderThreadMode)rtm;
        }

        // Make sure that headless is the last one, which it is assumed to be by design.
        //DEV_ASSERT(String("headless") == DisplayServer::get_create_function_name(DisplayServer::get_create_function_count() - 1));

        for (var i = 0; i < DisplayServer.ServerCreateFunctions.Count; i++)
        {
            var name = DisplayServer.ServerCreateFunctions[i].Name;

            if (displayDriver == name)
            {
                displayDriverIdx = i;
                break;
            }
        }

        if (displayDriverIdx < 0)
        {
            displayDriverIdx = 0;
        }

        OS.Singleton.DisplayDriverId = displayDriverIdx;

        // TODO - main\main.cpp[1763:1787]

        GLOBAL_DEF_BASIC("display/window/handheld/orientation", windowOrientation = DisplayServer.ScreenOrientation.SCREEN_LANDSCAPE);
        GLOBAL_DEF("display/window/vsync/vsync_mode",     windowVsyncMode = DisplayServer.VSyncMode.VSYNC_ENABLED);

        if (disableVsync)
        {
            windowVsyncMode = DisplayServer.VSyncMode.VSYNC_DISABLED;
        }

        GLOBAL_DEF_BASIC("physics/common/physics_ticks_per_second", engine.PhysicsTicksPerSecond = 60);

        ProjectSettings.Singleton.Set("physics/common/physics_ticks_per_second",
            new PropertyInfo(
                Variant.INT,
                "physics/common/physics_ticks_per_second",
                PropertyHint.PROPERTY_HINT_RANGE,
                "1,1000,1"
            )
        );

        GLOBAL_DEF("physics/common/max_physics_steps_per_frame", engine.MaxPhysicsStepsPerFrame = 8);
        ProjectSettings.Singleton.Set(
            "physics/common/max_physics_steps_per_frame",
            new PropertyInfo(
                Variant.INT,
                "physics/common/max_physics_steps_per_frame",
                PropertyHint.PROPERTY_HINT_RANGE,
                "1,100,1"
            )
        );

        GLOBAL_DEF("physics/common/physics_jitter_fix", engine.PhysicsJitterFix = 0.5);
        GLOBAL_DEF("application/run/max_fps", engine.MaxFps = 0);
        ProjectSettings.Singleton.Set(
            "application/run/max_fps",
            new PropertyInfo(
                Variant.INT,
                "application/run/max_fps",
                PropertyHint.PROPERTY_HINT_RANGE,
                "0,1000,1"
            )
        );

        GLOBAL_DEF("debug/settings/stdout/print_fps", false);
        GLOBAL_DEF("debug/settings/stdout/print_gpu_profile", false);
        GLOBAL_DEF("debug/settings/stdout/verbose_stdout", false);

        if (!OS.Singleton.VerboseStdout)
        {
            OS.Singleton.VerboseStdout = ProjectSettings.Singleton.Get<bool>("debug/settings/stdout/verbose_stdout");
        }

        if (frameDelay == 0)
        {
            GLOBAL_DEF("application/run/frame_delay_msec", frameDelay = 0);
            ProjectSettings.Singleton.Set(
                "application/run/frame_delay_msec",
                new PropertyInfo(
                    Variant.INT,
                    "application/run/frame_delay_msec",
                    PropertyHint.PROPERTY_HINT_RANGE,
                    "0,100,1,or_greater"
                )
            ); // No negative numbers
        }

        GLOBAL_DEF("application/run/low_processor_mode", OS.Singleton.LowProcessorUsageMode = false);
        GLOBAL_DEF("application/run/low_processor_mode_sleep_usec", OS.Singleton.LowProcessorUsageModeSleepUsec = 6900); // Roughly 144 FPS
        ProjectSettings.Singleton.Set(
            "application/run/low_processor_mode_sleep_usec",
            new PropertyInfo(
                Variant.INT,
                "application/run/low_processor_mode_sleep_usec",
                PropertyHint.PROPERTY_HINT_RANGE,
                "0,33200,1,or_greater"
            )
        ); // No negative numbers

        GLOBAL_DEF("display/window/ios/allow_high_refresh_rate", true);
        GLOBAL_DEF("display/window/ios/hide_home_indicator", true);
        GLOBAL_DEF("display/window/ios/hide_status_bar", true);
        GLOBAL_DEF("display/window/ios/suppress_ui_gesture", true);
        GLOBAL_DEF("input_devices/pointing/ios/touch_delay", 0.15);
        ProjectSettings.Singleton.Set("input_devices/pointing/ios/touch_delay",
            new PropertyInfo(
                Variant.FLOAT,
                "input_devices/pointing/ios/touch_delay",
                PropertyHint.PROPERTY_HINT_RANGE, "0,1,0.001"
            )
        );

        // XR project settings.
        GLOBAL_DEF_RST_BASIC("xr/openxr/enabled", false);
        GLOBAL_DEF_BASIC("xr/openxr/default_action_map", "res://openxr_action_map.tres");
        ProjectSettings.Singleton.Set(
            "xr/openxr/default_action_map",
            new PropertyInfo(Variant.STRING, "xr/openxr/default_action_map", PropertyHint.PROPERTY_HINT_FILE, "*.tres")
        );

        GLOBAL_DEF_BASIC("xr/openxr/form_factor", "0");
        ProjectSettings.Singleton.Set(
            "xr/openxr/form_factor",
            new PropertyInfo(Variant.INT, "xr/openxr/form_factor", PropertyHint.PROPERTY_HINT_ENUM, "Head Mounted,Handheld")
        );

        GLOBAL_DEF_BASIC("xr/openxr/view_configuration", "1");
        ProjectSettings.Singleton.Set(
            "xr/openxr/view_configuration",
            new PropertyInfo(Variant.INT, "xr/openxr/view_configuration", PropertyHint.PROPERTY_HINT_ENUM, "Mono,Stereo")
        ); // "Mono,Stereo,Quad,Observer"

        GLOBAL_DEF_BASIC("xr/openxr/reference_space", "1");
        ProjectSettings.Singleton.Set(
            "xr/openxr/reference_space",
            new PropertyInfo(Variant.INT, "xr/openxr/reference_space", PropertyHint.PROPERTY_HINT_ENUM, "Local,Stage")
        );

        GLOBAL_DEF_BASIC("xr/openxr/submit_depth_buffer", false);

        engine.FrameDelay = frameDelay;

        // TODO - message_queue = memnew(MessageQueue);

        engine.StartupBenchmarkEndMeasure(); // core

        if (secondPhase)
        {
            return Setup2();
        }

        return ErrorType.OK;

        error:

        // Todo - main\main.cpp[1889:1928]

        UnregisterCoreDriverTypes();
        UnregisterCoreExtensions();
        UnregisterCoreTypes();

        // Todo - main\main.cpp[1937:1939]

        OS.Singleton.FinalizeCore();

        // Todo - main\main.cpp[1941]

        return exitCode;
    }

    private static void RegisterCoreSettings()
    {
        GLOBAL_DEF("network/limits/tcp/connect_timeout_seconds", 30);
        ProjectSettings.Singleton.Set("network/limits/tcp/connect_timeout_seconds", new PropertyInfo(Variant.INT, "network/limits/tcp/connect_timeout_seconds", PropertyHint.PROPERTY_HINT_RANGE, "1,1800,1"));
        GLOBAL_DEF_RST("network/limits/packet_peer_stream/max_buffer_po2", (16));
        ProjectSettings.Singleton.Set("network/limits/packet_peer_stream/max_buffer_po2", new PropertyInfo(Variant.INT, "network/limits/packet_peer_stream/max_buffer_po2", PropertyHint.PROPERTY_HINT_RANGE, "0,64,1,or_greater"));
        GLOBAL_DEF("network/tls/certificate_bundle_override", "");
        ProjectSettings.Singleton.Set("network/tls/certificate_bundle_override", new PropertyInfo(Variant.STRING, "network/tls/certificate_bundle_override", PropertyHint.PROPERTY_HINT_FILE, "*.crt"));

        var workerThreads               = GLOBAL_DEF("threading/worker_pool/max_threads", -1);
        var lowPriorityUseSystemThreads = GLOBAL_DEF("threading/worker_pool/use_system_threads_for_low_priority_tasks", true);
        var lowPropertyRatio            = GLOBAL_DEF("threading/worker_pool/low_priority_thread_ratio", 0.3f);

        // Todo - core\register_core_types.cpp[307:310] WorkerThreadPool possibly managed by dotnet
    }

    private static void RegisterCoreDriverTypes()
    {
        // Todo
    }

    private static void RegisterCoreTypes()
    {
        // Todo
    }

    public static bool Start() => throw new NotImplementedException();
}
