using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

internal static unsafe partial class PInvoke
{
    #region SlangSession
    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangSessionHandle spCreateSession(byte* chars);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangCompileRequestHandle spCreateCompileRequest(SlangSessionHandle session);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spDestroySession(SlangSessionHandle inSession);
    #endregion

    #region SlangCompileRequest
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
    internal static partial byte* spGetDependencyFilePath(SlangCompileRequestHandle request, int index);

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

    #region SlangReflection
    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionEntryPointHandle spReflection_findEntryPointByName(SlangReflectionHandle inProgram, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionFunctionHandle spReflection_FindFunctionByName(SlangReflectionHandle reflection, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionFunctionHandle spReflection_FindFunctionByNameInType(SlangReflectionHandle reflection, SlangReflectionTypeHandle reflType, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeHandle spReflection_FindTypeByName(SlangReflectionHandle reflection, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeParameterHandle spReflection_FindTypeParameter(SlangReflectionHandle inProgram, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionVariableHandle spReflection_FindVarByNameInType(SlangReflectionHandle reflection, SlangReflectionTypeHandle reflType, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionEntryPointHandle spReflection_getEntryPointByIndex(SlangReflectionHandle inProgram, SlangUInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangUInt spReflection_getEntryPointCount(SlangReflectionHandle inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangUInt spReflection_getGlobalConstantBufferBinding(SlangReflectionHandle inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflection_getGlobalConstantBufferSize(SlangReflectionHandle inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeLayoutHandle spReflection_getGlobalParamsTypeLayout(SlangReflectionHandle reflection);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionVariableLayoutHandle spReflection_getGlobalParamsVarLayout(SlangReflectionHandle inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflection_getHashedString(SlangReflectionHandle reflection, SlangUInt index, size_t* outCount);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangUInt spReflection_getHashedStringCount(SlangReflectionHandle reflection);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionParameterHandle spReflection_GetParameterByIndex(SlangReflectionHandle inProgram, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflection_GetParameterCount(SlangReflectionHandle inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangSessionHandle spReflection_GetSession(SlangReflectionHandle reflection);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeHandle spReflection_getTypeFromDecl(SlangReflectionDeclHandle decl);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeLayoutHandle spReflection_GetTypeLayout(SlangReflectionHandle reflection, SlangReflectionTypeHandle inType, SlangLayoutRules rules);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeParameterHandle spReflection_GetTypeParameterByIndex(SlangReflectionHandle reflection, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflection_GetTypeParameterCount(SlangReflectionHandle reflection);

    [LibraryImport(PLATFORM_PATH)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool spReflection_isSubType(SlangReflectionHandle reflection, SlangReflectionTypeHandle subType, SlangReflectionTypeHandle superType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionGenericHandle spReflection_specializeGeneric(SlangReflectionHandle inProgramLayout, SlangReflectionGenericHandle generic, SlangInt argCount, SlangReflectionGenericArgTypeHandle argTypes, SlangReflectionGenericArgHandle args, ISlangBlob* outDiagnostics);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeHandle spReflection_specializeType(SlangReflectionHandle inProgramLayout, SlangReflectionTypeHandle inType, SlangInt specializationArgCount, SlangReflectionTypeHandle specializationArgs, ISlangBlob* outDiagnostics);
    #endregion

    #region SlangReflectionEntryPoint
    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionFunctionHandle spReflectionEntryPoint_getFunction(SlangReflectionEntryPointHandle inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionEntryPoint_getName(SlangReflectionEntryPointHandle inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionEntryPoint_getNameOverride(SlangReflectionEntryPointHandle inEntryPoint);

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

    #region SlangReflectionTypeParameter
    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeHandle spReflectionTypeParameter_GetConstraintByIndex(SlangReflectionTypeParameterHandle inTypeParam, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionTypeParameter_GetConstraintCount(SlangReflectionTypeParameterHandle inTypeParam);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionTypeParameter_GetIndex(SlangReflectionTypeParameterHandle inTypeParam);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionTypeParameter_GetName(SlangReflectionTypeParameterHandle inTypeParam);
    #endregion

    #region SlangReflectionType
    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionUserAttributeHandle spReflectionType_FindUserAttributeByName(SlangReflectionTypeHandle inType, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionType_GetColumnCount(SlangReflectionTypeHandle inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial ulong spReflectionType_GetElementCount(SlangReflectionTypeHandle inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeHandle spReflectionType_GetElementType(SlangReflectionTypeHandle inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionVariableLayoutHandle spReflectionType_GetFieldByIndex(SlangReflectionTypeHandle inType, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionType_GetFieldCount(SlangReflectionTypeHandle inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionGenericHandle spReflectionType_GetGenericContainer(SlangReflectionTypeHandle type);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangTypeKind spReflectionType_GetKind(SlangReflectionTypeHandle inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionType_GetName(SlangReflectionTypeHandle inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangResourceAccess spReflectionType_GetResourceAccess(SlangReflectionTypeHandle inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeHandle spReflectionType_GetResourceResultType(SlangReflectionTypeHandle inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangResourceShape spReflectionType_GetResourceShape(SlangReflectionTypeHandle inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionType_GetRowCount(SlangReflectionTypeHandle inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangScalarType spReflectionType_GetScalarType(SlangReflectionTypeHandle inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionType_getSpecializedTypeArgCount(SlangReflectionTypeHandle inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeHandle spReflectionType_getSpecializedTypeArgType(SlangReflectionTypeHandle inType, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionUserAttributeHandle spReflectionType_GetUserAttribute(SlangReflectionTypeHandle inType, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionType_GetUserAttributeCount(SlangReflectionTypeHandle inType);
    #endregion

    #region ReflectionTypeLayout
    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_findFieldIndexByName(SlangReflectionTypeLayoutHandle inTypeLayout, byte* nameBegin, byte* nameEnd);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int32_t spReflectionTypeLayout_getAlignment(SlangReflectionTypeLayoutHandle inTypeLayout, SlangParameterCategory category);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getBindingRangeBindingCount(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getBindingRangeDescriptorRangeCount(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getBindingRangeDescriptorSetIndex(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getBindingRangeFirstDescriptorRangeIndex(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangImageFormat spReflectionTypeLayout_getBindingRangeImageFormat(SlangReflectionTypeLayoutHandle typeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeLayoutHandle spReflectionTypeLayout_getBindingRangeLeafTypeLayout(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionVariableHandle spReflectionTypeLayout_getBindingRangeLeafVariable(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionTypeLayout_GetCategoryCount(SlangReflectionTypeLayoutHandle inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionVariableLayoutHandle spReflectionTypeLayout_getContainerVarLayout(SlangReflectionTypeLayoutHandle inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangParameterCategory spReflectionTypeLayout_getDescriptorSetDescriptorRangeCategory(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt setIndex, SlangInt rangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getDescriptorSetDescriptorRangeCount(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt setIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getDescriptorSetDescriptorRangeDescriptorCount(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt setIndex, SlangInt rangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getDescriptorSetDescriptorRangeIndexOffset(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt setIndex, SlangInt rangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangBindingType spReflectionTypeLayout_getDescriptorSetDescriptorRangeType(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt setIndex, SlangInt rangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getDescriptorSetSpaceOffset(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt setIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflectionTypeLayout_GetElementStride(SlangReflectionTypeLayoutHandle inTypeLayout, SlangParameterCategory category);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeLayoutHandle spReflectionTypeLayout_GetElementTypeLayout(SlangReflectionTypeLayoutHandle inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionVariableLayoutHandle spReflectionTypeLayout_GetElementVarLayout(SlangReflectionTypeLayoutHandle inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionVariableLayoutHandle spReflectionTypeLayout_GetExplicitCounter(SlangReflectionTypeLayoutHandle inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getFieldBindingRangeOffset(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt fieldIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionVariableLayoutHandle spReflectionTypeLayout_GetFieldByIndex(SlangReflectionTypeLayoutHandle inTypeLayout, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint32_t spReflectionTypeLayout_GetFieldCount(SlangReflectionTypeLayoutHandle inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int spReflectionTypeLayout_getGenericParamIndex(SlangReflectionTypeLayoutHandle inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangTypeKind spReflectionTypeLayout_getKind(SlangReflectionTypeLayoutHandle inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeLayoutHandle spReflectionTypeLayout_getPendingDataTypeLayout(SlangReflectionTypeLayoutHandle inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflectionTypeLayout_GetSize(SlangReflectionTypeLayoutHandle inTypeLayout, SlangParameterCategory category);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionVariableLayoutHandle spReflectionTypeLayout_getSpecializedTypePendingDataVarLayout(SlangReflectionTypeLayoutHandle inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflectionTypeLayout_GetStride(SlangReflectionTypeLayoutHandle inTypeLayout, SlangParameterCategory category);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeBindingRangeIndex(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt subObjectRangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeCount(SlangReflectionTypeLayoutHandle inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeDescriptorRangeBindingCount(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt subObjectRangeIndex, SlangInt bindingRangeIndexInSubObject);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangBindingType spReflectionTypeLayout_getSubObjectRangeDescriptorRangeBindingType(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt subObjectRangeIndex, SlangInt bindingRangeIndexInSubObject);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeDescriptorRangeCount(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt subObjectRangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeDescriptorRangeIndexOffset(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt subObjectRangeIndex, SlangInt bindingRangeIndexInSubObject);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeDescriptorRangeSpaceOffset(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt subObjectRangeIndex, SlangInt bindingRangeIndexInSubObject);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeObjectCount(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionVariableLayoutHandle spReflectionTypeLayout_getSubObjectRangeOffset(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt subObjectRangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeSpaceOffset(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt subObjectRangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeLayoutHandle spReflectionTypeLayout_getSubObjectRangeTypeLayout(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeHandle spReflectionTypeLayout_GetType(SlangReflectionTypeLayoutHandle inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_isBindingRangeSpecializable(SlangReflectionTypeLayoutHandle inTypeLayout, SlangInt index);
    #endregion

    #region SlangReflectionUserAttribute
    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionUserAttribute_GetArgumentCount(SlangReflectionUserAttributeHandle attrib);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangResult spReflectionUserAttribute_GetArgumentValueFloat(SlangReflectionUserAttributeHandle attrib, uint index, float* rs);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangResult spReflectionUserAttribute_GetArgumentValueInt(SlangReflectionUserAttributeHandle attrib, uint index, int* rs);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionUserAttribute_GetArgumentValueString(SlangReflectionUserAttributeHandle attrib, uint index, size_t* bufLen);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionUserAttribute_GetName(SlangReflectionUserAttributeHandle attrib);
    #endregion

    #region SlangReflectionVariable
    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionVariableHandle spReflectionVariable_applySpecializations(SlangReflectionVariableHandle var, SlangReflectionGenericHandle generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionModifierHandle spReflectionVariable_FindModifier(SlangReflectionVariableHandle inVar, SlangModifierID modifierID);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionUserAttributeHandle spReflectionVariable_FindUserAttributeByName(SlangReflectionVariableHandle inVar, SlangSessionHandle session, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionGenericHandle spReflectionVariable_GetGenericContainer(SlangReflectionVariableHandle var);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionVariable_GetName(SlangReflectionVariableHandle inVar);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionTypeHandle spReflectionVariable_GetType(SlangReflectionVariableHandle inVar);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionUserAttributeHandle spReflectionVariable_GetUserAttribute(SlangReflectionVariableHandle inVar, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionVariable_GetUserAttributeCount(SlangReflectionVariableHandle inVar);

    [LibraryImport(PLATFORM_PATH)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool spReflectionVariable_HasDefaultValue(SlangReflectionVariableHandle inVar);
    #endregion
}
