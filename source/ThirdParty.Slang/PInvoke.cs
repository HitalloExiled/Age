using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

internal static unsafe partial class PInvoke
{
#if WINDOWS
    private const string PLATFORM_PATH = "slang";
#elif LINUX
    private const string PLATFORM_PATH = "libslang";
#endif

    #region SlangGlobalSession
    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangResult slang_createGlobalSession(SlangInt apiVersion, IGlobalSession** outGlobalSession);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void slang_shutdown();

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* slang_getLastInternalErrorMessage();
    #endregion

    #region SlangSession
    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangSession> spCreateSession(byte* chars);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangCompileRequest> spCreateCompileRequest(Handle<SlangSession> session);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangProfileID spFindProfile(Handle<SlangSession> session, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spDestroySession(Handle<SlangSession> inSession);
    #endregion

    #region SlangCompileRequest
    [LibraryImport(PLATFORM_PATH)]
    internal static partial int spAddCodeGenTarget(Handle<SlangCompileRequest> request, SlangCompileTarget target);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int spAddEntryPoint(Handle<SlangCompileRequest> request, int translationUnitIndex, byte* name, SlangStage stage);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spAddSearchPath(Handle<SlangCompileRequest> request, byte* path);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int spAddTranslationUnit(Handle<SlangCompileRequest> request, SlangSourceLanguage language, byte* inName);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spAddTranslationUnitSourceFile(Handle<SlangCompileRequest> request, int translationUnitIndex, byte* path);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spAddTranslationUnitSourceString(Handle<SlangCompileRequest> request, int translationUnitIndex, byte* path, byte* source);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangResult spCompile(Handle<SlangCompileRequest> request);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spDestroyCompileRequest(Handle<SlangCompileRequest> request);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int spGetDependencyFileCount(Handle<SlangCompileRequest> request);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spGetDependencyFilePath(Handle<SlangCompileRequest> request, int index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spGetDiagnosticOutput(Handle<SlangCompileRequest> request);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void* spGetEntryPointCode(Handle<SlangCompileRequest> request, int entryPointIndex, size_t* outSize);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflection> spGetReflection(Handle<SlangCompileRequest> request);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spSetCodeGenTarget(Handle<SlangCompileRequest> request, SlangCompileTarget target);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spSetDebugInfoLevel(Handle<SlangCompileRequest> request, SlangDebugInfoLevel level);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spSetOptimizationLevel(Handle<SlangCompileRequest> request, SlangOptimizationLevel level);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spSetTargetFlags(Handle<SlangCompileRequest> request, int targetIndex, SlangTargetFlags flags);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spSetTargetProfile(Handle<SlangCompileRequest> request, int targetIndex, SlangProfileID profile);
    #endregion

    #region SlangReflection
    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionEntryPoint> spReflection_findEntryPointByName(Handle<SlangReflection> inProgram, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionFunction> spReflection_FindFunctionByName(Handle<SlangReflection> reflection, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionFunction> spReflection_FindFunctionByNameInType(Handle<SlangReflection> reflection, Handle<SlangReflectionType> reflType, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionType> spReflection_FindTypeByName(Handle<SlangReflection> reflection, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionTypeParameter> spReflection_FindTypeParameter(Handle<SlangReflection> inProgram, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariable> spReflection_FindVarByNameInType(Handle<SlangReflection> reflection, Handle<SlangReflectionType> reflType, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionEntryPoint> spReflection_getEntryPointByIndex(Handle<SlangReflection> inProgram, SlangUInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangUInt spReflection_getEntryPointCount(Handle<SlangReflection> inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangUInt spReflection_getGlobalConstantBufferBinding(Handle<SlangReflection> inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflection_getGlobalConstantBufferSize(Handle<SlangReflection> inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionTypeLayout> spReflection_getGlobalParamsTypeLayout(Handle<SlangReflection> reflection);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariableLayout> spReflection_getGlobalParamsVarLayout(Handle<SlangReflection> inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflection_getHashedString(Handle<SlangReflection> reflection, SlangUInt index, size_t* outCount);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangUInt spReflection_getHashedStringCount(Handle<SlangReflection> reflection);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionParameter> spReflection_GetParameterByIndex(Handle<SlangReflection> inProgram, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflection_GetParameterCount(Handle<SlangReflection> inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangSession> spReflection_GetSession(Handle<SlangReflection> reflection);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionType> spReflection_getTypeFromDecl(Handle<SlangReflectionDecl> decl);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionTypeLayout> spReflection_GetTypeLayout(Handle<SlangReflection> reflection, Handle<SlangReflectionType> inType, SlangLayoutRules rules);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionTypeParameter> spReflection_GetTypeParameterByIndex(Handle<SlangReflection> reflection, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflection_GetTypeParameterCount(Handle<SlangReflection> reflection);

    [LibraryImport(PLATFORM_PATH)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool spReflection_isSubType(Handle<SlangReflection> reflection, Handle<SlangReflectionType> subType, Handle<SlangReflectionType> superType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionGeneric> spReflection_specializeGeneric(Handle<SlangReflection> inProgramLayout, Handle<SlangReflectionGeneric> generic, SlangInt argCount, Handle<SlangReflectionGenericArgType> argTypes, Handle<SlangReflectionGenericArg> args, ISlangBlob* outDiagnostics);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionType> spReflection_specializeType(Handle<SlangReflection> inProgramLayout, Handle<SlangReflectionType> inType, SlangInt specializationArgCount, Handle<SlangReflectionType> specializationArgs, ISlangBlob* outDiagnostics);
    #endregion

    #region SlangReflectionEntryPoint
    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spReflectionEntryPoint_getComputeThreadGroupSize(Handle<SlangReflectionEntryPoint> inEntryPoint, SlangUInt axisCount, SlangUInt* outSizeAlongAxis);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spReflectionEntryPoint_getComputeWaveSize(Handle<SlangReflectionEntryPoint> inEntryPoint, SlangUInt* outWaveSize);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionFunction> spReflectionEntryPoint_getFunction(Handle<SlangReflectionEntryPoint> inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionEntryPoint_getName(Handle<SlangReflectionEntryPoint> inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionEntryPoint_getNameOverride(Handle<SlangReflectionEntryPoint> inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariableLayout> spReflectionEntryPoint_getParameterByIndex(Handle<SlangReflectionEntryPoint> inEntryPoint, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionEntryPoint_getParameterCount(Handle<SlangReflectionEntryPoint> inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariableLayout> spReflectionEntryPoint_getResultVarLayout(Handle<SlangReflectionEntryPoint> inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangStage spReflectionEntryPoint_getStage(Handle<SlangReflectionEntryPoint> inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariableLayout> spReflectionEntryPoint_getVarLayout(Handle<SlangReflectionEntryPoint> inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int spReflectionEntryPoint_hasDefaultConstantBuffer(Handle<SlangReflectionEntryPoint> inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int spReflectionEntryPoint_usesAnySampleRateInput(Handle<SlangReflectionEntryPoint> inEntryPoint);
    #endregion

    #region ReflectionParameter
    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionParameter_GetBindingIndex(Handle<SlangReflectionParameter> inVarLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionParameter_GetBindingSpace(Handle<SlangReflectionParameter> inVarLayout);
    #endregion

    #region SlangReflectionTypeParameter
    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionType> spReflectionTypeParameter_GetConstraintByIndex(Handle<SlangReflectionTypeParameter> inTypeParam, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionTypeParameter_GetConstraintCount(Handle<SlangReflectionTypeParameter> inTypeParam);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangResult spReflectionType_GetFullName(Handle<SlangReflectionType> inType, ISlangBlob* outNameBlob);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionTypeParameter_GetIndex(Handle<SlangReflectionTypeParameter> inTypeParam);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionTypeParameter_GetName(Handle<SlangReflectionTypeParameter> inTypeParam);
    #endregion

    #region SlangReflectionType
    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionType> spReflectionType_applySpecializations(Handle<SlangReflectionType> inType, Handle<SlangReflectionGeneric> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionUserAttribute> spReflectionType_FindUserAttributeByName(Handle<SlangReflectionType> inType, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionType_GetColumnCount(Handle<SlangReflectionType> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial ulong spReflectionType_GetElementCount(Handle<SlangReflectionType> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionType> spReflectionType_GetElementType(Handle<SlangReflectionType> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariable> spReflectionType_GetFieldByIndex(Handle<SlangReflectionType> inType, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionType_GetFieldCount(Handle<SlangReflectionType> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionGeneric> spReflectionType_GetGenericContainer(Handle<SlangReflectionType> type);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangTypeKind spReflectionType_GetKind(Handle<SlangReflectionType> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionType_GetName(Handle<SlangReflectionType> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangResourceAccess spReflectionType_GetResourceAccess(Handle<SlangReflectionType> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionType> spReflectionType_GetResourceResultType(Handle<SlangReflectionType> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangResourceShape spReflectionType_GetResourceShape(Handle<SlangReflectionType> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionType_GetRowCount(Handle<SlangReflectionType> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangScalarType spReflectionType_GetScalarType(Handle<SlangReflectionType> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionType_getSpecializedTypeArgCount(Handle<SlangReflectionType> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionType> spReflectionType_getSpecializedTypeArgType(Handle<SlangReflectionType> inType, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionUserAttribute> spReflectionType_GetUserAttribute(Handle<SlangReflectionType> inType, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionType_GetUserAttributeCount(Handle<SlangReflectionType> inType);
    #endregion

    #region ReflectionTypeLayout

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getBindingRangeCount(Handle<SlangReflectionTypeLayout> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangBindingType spReflectionTypeLayout_getBindingRangeType(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getFieldBindingRangeOffset(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt fieldIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_findFieldIndexByName(Handle<SlangReflectionTypeLayout> inTypeLayout, byte* nameBegin, byte* nameEnd);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int32_t spReflectionTypeLayout_getAlignment(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangParameterCategory category);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getBindingRangeBindingCount(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getBindingRangeDescriptorRangeCount(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getBindingRangeDescriptorSetIndex(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getBindingRangeFirstDescriptorRangeIndex(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangImageFormat spReflectionTypeLayout_getBindingRangeImageFormat(Handle<SlangReflectionTypeLayout> typeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionTypeLayout> spReflectionTypeLayout_getBindingRangeLeafTypeLayout(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariable> spReflectionTypeLayout_getBindingRangeLeafVariable(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangParameterCategory spReflectionTypeLayout_GetCategoryByIndex(Handle<SlangReflectionTypeLayout> inTypeLayout, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionTypeLayout_GetCategoryCount(Handle<SlangReflectionTypeLayout> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariableLayout> spReflectionTypeLayout_getContainerVarLayout(Handle<SlangReflectionTypeLayout> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getDescriptorSetCount(Handle<SlangReflectionTypeLayout> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangParameterCategory spReflectionTypeLayout_getDescriptorSetDescriptorRangeCategory(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt setIndex, SlangInt rangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getDescriptorSetDescriptorRangeCount(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt setIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getDescriptorSetDescriptorRangeDescriptorCount(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt setIndex, SlangInt rangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getDescriptorSetDescriptorRangeIndexOffset(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt setIndex, SlangInt rangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangBindingType spReflectionTypeLayout_getDescriptorSetDescriptorRangeType(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt setIndex, SlangInt rangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getDescriptorSetSpaceOffset(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt setIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflectionTypeLayout_GetElementStride(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangParameterCategory category);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionTypeLayout> spReflectionTypeLayout_GetElementTypeLayout(Handle<SlangReflectionTypeLayout> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariableLayout> spReflectionTypeLayout_GetElementVarLayout(Handle<SlangReflectionTypeLayout> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariableLayout> spReflectionTypeLayout_GetExplicitCounter(Handle<SlangReflectionTypeLayout> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getExplicitCounterBindingRangeOffset(Handle<SlangReflectionTypeLayout> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariableLayout> spReflectionTypeLayout_GetFieldByIndex(Handle<SlangReflectionTypeLayout> inTypeLayout, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint32_t spReflectionTypeLayout_GetFieldCount(Handle<SlangReflectionTypeLayout> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int spReflectionTypeLayout_getGenericParamIndex(Handle<SlangReflectionTypeLayout> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangTypeKind spReflectionTypeLayout_getKind(Handle<SlangReflectionTypeLayout> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangMatrixLayoutMode spReflectionTypeLayout_GetMatrixLayoutMode(Handle<SlangReflectionTypeLayout> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangParameterCategory spReflectionTypeLayout_GetParameterCategory(Handle<SlangReflectionTypeLayout> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionTypeLayout> spReflectionTypeLayout_getPendingDataTypeLayout(Handle<SlangReflectionTypeLayout> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflectionTypeLayout_GetSize(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangParameterCategory category);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariableLayout> spReflectionTypeLayout_getSpecializedTypePendingDataVarLayout(Handle<SlangReflectionTypeLayout> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflectionTypeLayout_GetStride(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangParameterCategory category);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeBindingRangeIndex(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt subObjectRangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeCount(Handle<SlangReflectionTypeLayout> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeDescriptorRangeBindingCount(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt subObjectRangeIndex, SlangInt bindingRangeIndexInSubObject);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangBindingType spReflectionTypeLayout_getSubObjectRangeDescriptorRangeBindingType(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt subObjectRangeIndex, SlangInt bindingRangeIndexInSubObject);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeDescriptorRangeCount(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt subObjectRangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeDescriptorRangeIndexOffset(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt subObjectRangeIndex, SlangInt bindingRangeIndexInSubObject);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeDescriptorRangeSpaceOffset(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt subObjectRangeIndex, SlangInt bindingRangeIndexInSubObject);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeObjectCount(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariableLayout> spReflectionTypeLayout_getSubObjectRangeOffset(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt subObjectRangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeSpaceOffset(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt subObjectRangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionTypeLayout> spReflectionTypeLayout_getSubObjectRangeTypeLayout(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionType> spReflectionTypeLayout_GetType(Handle<SlangReflectionTypeLayout> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_isBindingRangeSpecializable(Handle<SlangReflectionTypeLayout> inTypeLayout, SlangInt index);
    #endregion

    #region SlangReflectionUserAttribute
    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionUserAttribute_GetArgumentCount(Handle<SlangReflectionUserAttribute> attrib);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangResult spReflectionUserAttribute_GetArgumentValueFloat(Handle<SlangReflectionUserAttribute> attrib, uint index, float* rs);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangResult spReflectionUserAttribute_GetArgumentValueInt(Handle<SlangReflectionUserAttribute> attrib, uint index, int* rs);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionUserAttribute_GetArgumentValueString(Handle<SlangReflectionUserAttribute> attrib, uint index, size_t* bufLen);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionUserAttribute_GetName(Handle<SlangReflectionUserAttribute> attrib);
    #endregion

    #region SlangReflectionVariable
    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariable> spReflectionVariable_applySpecializations(Handle<SlangReflectionVariable> var, Handle<SlangReflectionGeneric> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionModifier> spReflectionVariable_FindModifier(Handle<SlangReflectionVariable> inVar, SlangModifierID modifierID);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionUserAttribute> spReflectionVariable_FindUserAttributeByName(Handle<SlangReflectionVariable> inVar, Handle<SlangSession> session, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionGeneric> spReflectionVariable_GetGenericContainer(Handle<SlangReflectionVariable> var);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionVariable_GetName(Handle<SlangReflectionVariable> inVar);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionType> spReflectionVariable_GetType(Handle<SlangReflectionVariable> inVar);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionUserAttribute> spReflectionVariable_GetUserAttribute(Handle<SlangReflectionVariable> inVar, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionVariable_GetUserAttributeCount(Handle<SlangReflectionVariable> inVar);

    [LibraryImport(PLATFORM_PATH)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool spReflectionVariable_HasDefaultValue(Handle<SlangReflectionVariable> inVar);
    #endregion

    #region SlangReflectionVariableLayout
    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflectionVariableLayout_GetOffset(Handle<SlangReflectionVariableLayout> inVarLayout, SlangParameterCategory category);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariableLayout> spReflectionVariableLayout_getPendingDataLayout(Handle<SlangReflectionVariableLayout> inVarLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionVariableLayout_GetSemanticName(Handle<SlangReflectionVariableLayout> inVarLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflectionVariableLayout_GetSemanticIndex(Handle<SlangReflectionVariableLayout> inVarLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflectionVariableLayout_GetSpace(Handle<SlangReflectionVariableLayout> inVarLayout, SlangParameterCategory category);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangStage spReflectionVariableLayout_getStage(Handle<SlangReflectionVariableLayout> inVarLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionTypeLayout> spReflectionVariableLayout_GetTypeLayout(Handle<SlangReflectionVariableLayout> inVarLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariable> spReflectionVariableLayout_GetVariable(Handle<SlangReflectionVariableLayout> inVarLayout);
    #endregion

    #region SlangReflectionGeneric
    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionGeneric> spReflectionGeneric_applySpecializations(Handle<SlangReflectionGeneric> currGeneric, Handle<SlangReflectionGeneric> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionDecl> spReflectionGeneric_asDecl(Handle<SlangReflectionGeneric> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int64_t spReflectionGeneric_GetConcreteIntVal(Handle<SlangReflectionGeneric> generic, Handle<SlangReflectionVariable> valueParam);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionType> spReflectionGeneric_GetConcreteType(Handle<SlangReflectionGeneric> generic, Handle<SlangReflectionVariable> typeParam);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionDecl> spReflectionGeneric_GetInnerDecl(Handle<SlangReflectionGeneric> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangDeclKind spReflectionGeneric_GetInnerKind(Handle<SlangReflectionGeneric> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionGeneric_GetName(Handle<SlangReflectionGeneric> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionGeneric> spReflectionGeneric_GetOuterGenericContainer(Handle<SlangReflectionGeneric> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariable> spReflectionGeneric_GetTypeParameter(Handle<SlangReflectionGeneric> generic, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionGeneric_GetTypeParameterConstraintCount(Handle<SlangReflectionGeneric> generic, Handle<SlangReflectionVariable> typeParam);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionType> spReflectionGeneric_GetTypeParameterConstraintType(Handle<SlangReflectionGeneric> generic, Handle<SlangReflectionVariable> typeParam, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionGeneric_GetTypeParameterCount(Handle<SlangReflectionGeneric> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariable> spReflectionGeneric_GetValueParameter(Handle<SlangReflectionGeneric> generic, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionGeneric_GetValueParameterCount(Handle<SlangReflectionGeneric> generic);
    #endregion

    #region SlangReflectionFunction
    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionFunction> spReflectionFunction_applySpecializations(Handle<SlangReflectionFunction> func, Handle<SlangReflectionGeneric> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionDecl> spReflectionFunction_asDecl(Handle<SlangReflectionFunction> inFunc);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionModifier> spReflectionFunction_FindModifier(Handle<SlangReflectionFunction> inFunc, SlangModifierID modifierID);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionUserAttribute> spReflectionFunction_FindUserAttributeByName(Handle<SlangReflectionFunction> inFunc, Handle<SlangSession> session, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionGeneric> spReflectionFunction_GetGenericContainer(Handle<SlangReflectionFunction> func);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionFunction_GetName(Handle<SlangReflectionFunction> inFunc);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionFunction> spReflectionFunction_getOverload(Handle<SlangReflectionFunction> func, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionFunction_getOverloadCount(Handle<SlangReflectionFunction> func);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionVariable> spReflectionFunction_GetParameter(Handle<SlangReflectionFunction> inFunc, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionFunction_GetParameterCount(Handle<SlangReflectionFunction> inFunc);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionType> spReflectionFunction_GetResultType(Handle<SlangReflectionFunction> inFunc);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionUserAttribute> spReflectionFunction_GetUserAttribute(Handle<SlangReflectionFunction> inFunc, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionFunction_GetUserAttributeCount(Handle<SlangReflectionFunction> inFunc);

    [LibraryImport(PLATFORM_PATH)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool spReflectionFunction_isOverloaded(Handle<SlangReflectionFunction> func);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<SlangReflectionFunction> spReflectionFunction_specializeWithArgTypes(Handle<SlangReflectionFunction> func, SlangInt argTypeCount, Handle<SlangReflectionType>* argTypes);
    #endregion
}
