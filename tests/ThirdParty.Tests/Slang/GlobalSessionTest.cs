using Age.Core.Collections;
using ThirdParty.Slang;

namespace ThirdParty.Tests.Slang;

public class GlobalSessionTest
{
    [Fact]
    public void CreateGlobalSession()
    {
        using var globalSession = new GlobalSession(0);

        using NativeStringRefArray       searchPath = [Path.GetFullPath(Path.Join(Path.RootLocation, "Slang/Shaders"))];
        using NativeRefArray<TargetDesc> targets =
        [
            new()
            {
                Format  = CompileTarget.Spirv,
                Profile = globalSession.FindProfile("spirv_1_0"),
            }
        ];

        var sessionDesc = new SessionDesc
        {
            SearchPaths     = searchPath,
            SearchPathCount = searchPath.Length,
            Targets         = targets,
            TargetCount     = targets.Length,
        };

        using var session = globalSession.CreateSession(sessionDesc);

        using var module = session.LoadModule("shader-a");

        Assert.NotNull(module);

        using var entryPoint = module.FindEntryPointByName("main");

        Assert.NotNull(entryPoint);

        using var composedProgram = session.CreateCompositeComponentType([entryPoint, module]);

        using var program   = composedProgram.Link();
        using var spirvCode = program.GetEntryPointCode(0, 0);

        Assert.True(spirvCode.Length > 0);

        var reflection = program.GetLayout();

        Assert.NotNull(reflection);
    }
}
