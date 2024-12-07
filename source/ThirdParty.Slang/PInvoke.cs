using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

internal static unsafe partial class PInvoke
{
    #region Session
    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangSessionHandle spCreateSession(byte* chars);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangCompileRequestHandle spCreateCompileRequest(SlangSessionHandle session);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spDestroySession(SlangSessionHandle inSession);
    #endregion

    #region CompileRequest
    [LibraryImport(PLATFORM_PATH)]
    internal static partial int spAddCodeGenTarget(SlangCompileRequestHandle request, SlangCompileTarget target);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int spAddEntryPoint(SlangCompileRequestHandle request, int translationUnitIndex, byte* name, SlangStage stage);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spAddSearchPath(SlangCompileRequestHandle request, byte* path);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int spAddTranslationUnit(SlangCompileRequestHandle request, SlangSourceLanguage language, byte* inName);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spAddTranslationUnitSourceFile(SlangCompileRequestHandle request, int translationUnitIndex, byte* path);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spAddTranslationUnitSourceString(SlangCompileRequestHandle request, int translationUnitIndex, byte* path, byte* source);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangResult spCompile(SlangCompileRequestHandle request);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spDestroyCompileRequest(SlangCompileRequestHandle request);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangProfileID spFindProfile(SlangCompileRequestHandle session, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int spGetDependencyFileCount(SlangCompileRequestHandle request);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial char* spGetDependencyFilePath(SlangCompileRequestHandle request, int index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spGetDiagnosticOutput(SlangCompileRequestHandle request);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void* spGetEntryPointCode(SlangCompileRequestHandle request, int entryPointIndex, size_t* outSize);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionHandle spGetReflection(SlangCompileRequestHandle request);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spSetCodeGenTarget(SlangCompileRequestHandle request, SlangCompileTarget target);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spSetDebugInfoLevel(SlangCompileRequestHandle request, SlangDebugInfoLevel level);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spSetOptimizationLevel(SlangCompileRequestHandle request, SlangOptimizationLevel level);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spSetTargetFlags(SlangCompileRequestHandle request, int targetIndex, SlangTargetFlags flags);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spSetTargetProfile(SlangCompileRequestHandle request, int targetIndex, SlangProfileID profile);
    #endregion

    #region Reflection
    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionEntryPointHandle spReflection_findEntryPointByName(SlangReflectionHandle inProgram, char* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionEntryPointHandle spReflection_getEntryPointByIndex(SlangReflectionHandle inProgram, SlangUInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangUInt spReflection_getEntryPointCount(SlangReflectionHandle inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangUInt spReflection_getGlobalConstantBufferBinding(SlangReflectionHandle inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflection_getGlobalConstantBufferSize(SlangReflectionHandle inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionVariableLayoutHandle spReflection_getGlobalParamsVarLayout(SlangReflectionHandle inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeParameterHandle spReflection_FindTypeParameter(SlangReflectionHandle inProgram, char* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionParameterHandle spReflection_GetParameterByIndex(SlangReflectionHandle inProgram, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflection_GetParameterCount(SlangReflectionHandle inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeParameterHandle spReflection_GetTypeParameterByIndex(SlangReflectionHandle reflection, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int spReflection_GetTypeParameterCount(SlangReflectionHandle reflection);
    #endregion

    #region SlangReflectionEntryPoint
    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionFunctionHandle spReflectionEntryPoint_getFunction(SlangReflectionEntryPointHandle inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial char* spReflectionEntryPoint_getName(SlangReflectionEntryPointHandle inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial char* spReflectionEntryPoint_getNameOverride(SlangReflectionEntryPointHandle inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionVariableLayoutHandle spReflectionEntryPoint_getParameterByIndex(SlangReflectionEntryPointHandle inEntryPoint, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionEntryPoint_getParameterCount(SlangReflectionEntryPointHandle inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangStage spReflectionEntryPoint_getStage(SlangReflectionEntryPointHandle inEntryPoint);
    #endregion

    #region ReflectionParameter
    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionParameter_GetBindingIndex(SlangReflectionParameterHandle inVarLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionParameter_GetBindingSpace(SlangReflectionParameterHandle inVarLayout);
    #endregion
}
