using System.Runtime.InteropServices;

namespace ThirdParty.Slang;

internal static unsafe partial class PInvoke
{
#if WINDOWS
    private const string PLATFORM_PATH = "slang-compiler.dll";
#elif LINUX
    private const string PLATFORM_PATH = "libslang-compiler.so";
#endif

    [LibraryImport(PLATFORM_PATH)]
    internal static partial IBlob* slang_createBlob(void* data, size_t size);

    #region SlangGlobalSession
    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangResult slang_createGlobalSession(SlangInt apiVersion, IGlobalSession** outGlobalSession);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void slang_shutdown();

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* slang_getLastInternalErrorMessage();
    #endregion

    #region SlangReflection
    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflection_getBindlessSpaceIndex(Handle<ShaderReflection> reflection);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<EntryPointReflection> spReflection_findEntryPointByName(Handle<ShaderReflection> inProgram, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<FunctionReflection> spReflection_FindFunctionByName(Handle<ShaderReflection> reflection, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<FunctionReflection> spReflection_FindFunctionByNameInType(Handle<ShaderReflection> reflection, Handle<TypeReflection> reflType, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeReflection> spReflection_FindTypeByName(Handle<ShaderReflection> reflection, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeParameterReflection> spReflection_FindTypeParameter(Handle<ShaderReflection> inProgram, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableReflection> spReflection_FindVarByNameInType(Handle<ShaderReflection> reflection, Handle<TypeReflection> reflType, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<EntryPointReflection> spReflection_getEntryPointByIndex(Handle<ShaderReflection> inProgram, SlangUInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangUInt spReflection_getEntryPointCount(Handle<ShaderReflection> inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangUInt spReflection_getGlobalConstantBufferBinding(Handle<ShaderReflection> inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflection_getGlobalConstantBufferSize(Handle<ShaderReflection> inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeLayoutReflection> spReflection_getGlobalParamsTypeLayout(Handle<ShaderReflection> reflection);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableLayoutReflection> spReflection_getGlobalParamsVarLayout(Handle<ShaderReflection> inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflection_getHashedString(Handle<ShaderReflection> reflection, SlangUInt index, size_t* outCount);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangUInt spReflection_getHashedStringCount(Handle<ShaderReflection> reflection);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableLayoutReflection> spReflection_GetParameterByIndex(Handle<ShaderReflection> inProgram, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflection_GetParameterCount(Handle<ShaderReflection> inProgram);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeReflection> spReflection_getTypeFromDecl(SlangReflectionDecl decl);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeLayoutReflection> spReflection_GetTypeLayout(Handle<ShaderReflection> reflection, Handle<TypeReflection> inType, SlangLayoutRules rules);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeParameterReflection> spReflection_GetTypeParameterByIndex(Handle<ShaderReflection> reflection, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflection_GetTypeParameterCount(Handle<ShaderReflection> reflection);

    [LibraryImport(PLATFORM_PATH)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool spReflection_isSubType(Handle<ShaderReflection> reflection, Handle<TypeReflection> subType, Handle<TypeReflection> superType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<GenericReflection> spReflection_specializeGeneric(Handle<ShaderReflection> inProgramLayout, Handle<GenericReflection> generic, SlangInt argCount, SlangReflectionGenericArgType* argTypes, SlangReflectionGenericArg* args, IBlob** outDiagnostics);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeReflection> spReflection_specializeType(Handle<ShaderReflection> inProgramLayout, Handle<TypeReflection> inType, SlangInt specializationArgCount, Handle<TypeReflection> specializationArgs, IBlob** outDiagnostics);
    #endregion

    #region SlangReflectionEntryPoint
    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spReflectionEntryPoint_getComputeThreadGroupSize(Handle<EntryPointReflection> inEntryPoint, SlangUInt axisCount, SlangUInt* outSizeAlongAxis);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial void spReflectionEntryPoint_getComputeWaveSize(Handle<EntryPointReflection> inEntryPoint, SlangUInt* outWaveSize);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<FunctionReflection> spReflectionEntryPoint_getFunction(Handle<EntryPointReflection> inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionEntryPoint_getName(Handle<EntryPointReflection> inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionEntryPoint_getNameOverride(Handle<EntryPointReflection> inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableLayoutReflection> spReflectionEntryPoint_getParameterByIndex(Handle<EntryPointReflection> inEntryPoint, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionEntryPoint_getParameterCount(Handle<EntryPointReflection> inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableLayoutReflection> spReflectionEntryPoint_getResultVarLayout(Handle<EntryPointReflection> inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangStage spReflectionEntryPoint_getStage(Handle<EntryPointReflection> inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableLayoutReflection> spReflectionEntryPoint_getVarLayout(Handle<EntryPointReflection> inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int spReflectionEntryPoint_hasDefaultConstantBuffer(Handle<EntryPointReflection> inEntryPoint);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int spReflectionEntryPoint_usesAnySampleRateInput(Handle<EntryPointReflection> inEntryPoint);
    #endregion

    #region ReflectionParameter
    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionParameter_GetBindingIndex(Handle<VariableLayoutReflection> inVarLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionParameter_GetBindingSpace(Handle<VariableLayoutReflection> inVarLayout);
    #endregion

    #region SlangReflectionTypeParameter
    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeReflection> spReflectionTypeParameter_GetConstraintByIndex(Handle<TypeParameterReflection> inTypeParam, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionTypeParameter_GetConstraintCount(Handle<TypeParameterReflection> inTypeParam);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangResult spReflectionType_GetFullName(Handle<TypeReflection> inType, IBlob** outNameBlob);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionTypeParameter_GetIndex(Handle<TypeParameterReflection> inTypeParam);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionTypeParameter_GetName(Handle<TypeParameterReflection> inTypeParam);
    #endregion

    #region TypeReflection
    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeReflection> spReflectionType_applySpecializations(Handle<TypeReflection> inType, Handle<GenericReflection> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<Attribute> spReflectionType_FindUserAttributeByName(Handle<TypeReflection> inType, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionType_GetColumnCount(Handle<TypeReflection> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial ulong spReflectionType_GetElementCount(Handle<TypeReflection> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeReflection> spReflectionType_GetElementType(Handle<TypeReflection> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableReflection> spReflectionType_GetFieldByIndex(Handle<TypeReflection> inType, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionType_GetFieldCount(Handle<TypeReflection> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<GenericReflection> spReflectionType_GetGenericContainer(Handle<TypeReflection> type);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangTypeKind spReflectionType_GetKind(Handle<TypeReflection> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionType_GetName(Handle<TypeReflection> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangResourceAccess spReflectionType_GetResourceAccess(Handle<TypeReflection> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeReflection> spReflectionType_GetResourceResultType(Handle<TypeReflection> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangResourceShape spReflectionType_GetResourceShape(Handle<TypeReflection> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionType_GetRowCount(Handle<TypeReflection> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangScalarType spReflectionType_GetScalarType(Handle<TypeReflection> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionType_getSpecializedTypeArgCount(Handle<TypeReflection> inType);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeReflection> spReflectionType_getSpecializedTypeArgType(Handle<TypeReflection> inType, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<Attribute> spReflectionType_GetUserAttribute(Handle<TypeReflection> inType, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionType_GetUserAttributeCount(Handle<TypeReflection> inType);
    #endregion

    #region ReflectionTypeLayout

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getBindingRangeCount(Handle<TypeLayoutReflection> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangBindingType spReflectionTypeLayout_getBindingRangeType(Handle<TypeLayoutReflection> inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getFieldBindingRangeOffset(Handle<TypeLayoutReflection> inTypeLayout, SlangInt fieldIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_findFieldIndexByName(Handle<TypeLayoutReflection> inTypeLayout, byte* nameBegin, byte* nameEnd);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int32_t spReflectionTypeLayout_getAlignment(Handle<TypeLayoutReflection> inTypeLayout, SlangParameterCategory category);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getBindingRangeBindingCount(Handle<TypeLayoutReflection> inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getBindingRangeDescriptorRangeCount(Handle<TypeLayoutReflection> inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getBindingRangeDescriptorSetIndex(Handle<TypeLayoutReflection> inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getBindingRangeFirstDescriptorRangeIndex(Handle<TypeLayoutReflection> inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangImageFormat spReflectionTypeLayout_getBindingRangeImageFormat(Handle<TypeLayoutReflection> typeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeLayoutReflection> spReflectionTypeLayout_getBindingRangeLeafTypeLayout(Handle<TypeLayoutReflection> inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableReflection> spReflectionTypeLayout_getBindingRangeLeafVariable(Handle<TypeLayoutReflection> inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangParameterCategory spReflectionTypeLayout_GetCategoryByIndex(Handle<TypeLayoutReflection> inTypeLayout, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionTypeLayout_GetCategoryCount(Handle<TypeLayoutReflection> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableLayoutReflection> spReflectionTypeLayout_getContainerVarLayout(Handle<TypeLayoutReflection> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getDescriptorSetCount(Handle<TypeLayoutReflection> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangParameterCategory spReflectionTypeLayout_getDescriptorSetDescriptorRangeCategory(Handle<TypeLayoutReflection> inTypeLayout, SlangInt setIndex, SlangInt rangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getDescriptorSetDescriptorRangeCount(Handle<TypeLayoutReflection> inTypeLayout, SlangInt setIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getDescriptorSetDescriptorRangeDescriptorCount(Handle<TypeLayoutReflection> inTypeLayout, SlangInt setIndex, SlangInt rangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getDescriptorSetDescriptorRangeIndexOffset(Handle<TypeLayoutReflection> inTypeLayout, SlangInt setIndex, SlangInt rangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangBindingType spReflectionTypeLayout_getDescriptorSetDescriptorRangeType(Handle<TypeLayoutReflection> inTypeLayout, SlangInt setIndex, SlangInt rangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getDescriptorSetSpaceOffset(Handle<TypeLayoutReflection> inTypeLayout, SlangInt setIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflectionTypeLayout_GetElementStride(Handle<TypeLayoutReflection> inTypeLayout, SlangParameterCategory category);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeLayoutReflection> spReflectionTypeLayout_GetElementTypeLayout(Handle<TypeLayoutReflection> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableLayoutReflection> spReflectionTypeLayout_GetElementVarLayout(Handle<TypeLayoutReflection> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableLayoutReflection> spReflectionTypeLayout_GetExplicitCounter(Handle<TypeLayoutReflection> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getExplicitCounterBindingRangeOffset(Handle<TypeLayoutReflection> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableLayoutReflection> spReflectionTypeLayout_GetFieldByIndex(Handle<TypeLayoutReflection> inTypeLayout, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint32_t spReflectionTypeLayout_GetFieldCount(Handle<TypeLayoutReflection> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int spReflectionTypeLayout_getGenericParamIndex(Handle<TypeLayoutReflection> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangTypeKind spReflectionTypeLayout_getKind(Handle<TypeLayoutReflection> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangMatrixLayoutMode spReflectionTypeLayout_GetMatrixLayoutMode(Handle<TypeLayoutReflection> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangParameterCategory spReflectionTypeLayout_GetParameterCategory(Handle<TypeLayoutReflection> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeLayoutReflection> spReflectionTypeLayout_getPendingDataTypeLayout(Handle<TypeLayoutReflection> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflectionTypeLayout_GetSize(Handle<TypeLayoutReflection> inTypeLayout, SlangParameterCategory category);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableLayoutReflection> spReflectionTypeLayout_getSpecializedTypePendingDataVarLayout(Handle<TypeLayoutReflection> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflectionTypeLayout_GetStride(Handle<TypeLayoutReflection> inTypeLayout, SlangParameterCategory category);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeBindingRangeIndex(Handle<TypeLayoutReflection> inTypeLayout, SlangInt subObjectRangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeCount(Handle<TypeLayoutReflection> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeDescriptorRangeBindingCount(Handle<TypeLayoutReflection> inTypeLayout, SlangInt subObjectRangeIndex, SlangInt bindingRangeIndexInSubObject);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangBindingType spReflectionTypeLayout_getSubObjectRangeDescriptorRangeBindingType(Handle<TypeLayoutReflection> inTypeLayout, SlangInt subObjectRangeIndex, SlangInt bindingRangeIndexInSubObject);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeDescriptorRangeCount(Handle<TypeLayoutReflection> inTypeLayout, SlangInt subObjectRangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeDescriptorRangeIndexOffset(Handle<TypeLayoutReflection> inTypeLayout, SlangInt subObjectRangeIndex, SlangInt bindingRangeIndexInSubObject);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeDescriptorRangeSpaceOffset(Handle<TypeLayoutReflection> inTypeLayout, SlangInt subObjectRangeIndex, SlangInt bindingRangeIndexInSubObject);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeObjectCount(Handle<TypeLayoutReflection> inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableLayoutReflection> spReflectionTypeLayout_getSubObjectRangeOffset(Handle<TypeLayoutReflection> inTypeLayout, SlangInt subObjectRangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_getSubObjectRangeSpaceOffset(Handle<TypeLayoutReflection> inTypeLayout, SlangInt subObjectRangeIndex);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeLayoutReflection> spReflectionTypeLayout_getSubObjectRangeTypeLayout(Handle<TypeLayoutReflection> inTypeLayout, SlangInt index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeReflection> spReflectionTypeLayout_GetType(Handle<TypeLayoutReflection> inTypeLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangInt spReflectionTypeLayout_isBindingRangeSpecializable(Handle<TypeLayoutReflection> inTypeLayout, SlangInt index);
    #endregion

    #region SlangReflectionUserAttribute
    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionUserAttribute_GetArgumentCount(Handle<Attribute> attrib);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangResult spReflectionUserAttribute_GetArgumentValueFloat(Handle<Attribute> attrib, uint index, float* rs);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangResult spReflectionUserAttribute_GetArgumentValueInt(Handle<Attribute> attrib, uint index, int* rs);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionUserAttribute_GetArgumentValueString(Handle<Attribute> attrib, uint index, size_t* bufLen);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionUserAttribute_GetName(Handle<Attribute> attrib);
    #endregion

    #region SlangReflectionVariable
    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableReflection> spReflectionVariable_applySpecializations(Handle<VariableReflection> var, Handle<GenericReflection> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionModifier spReflectionVariable_FindModifier(Handle<VariableReflection> inVar, SlangModifierID modifierID);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<Attribute> spReflectionVariable_FindUserAttributeByName(Handle<VariableReflection> inVar, IGlobalSession* session, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<GenericReflection> spReflectionVariable_GetGenericContainer(Handle<VariableReflection> var);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionVariable_GetName(Handle<VariableReflection> inVar);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeReflection> spReflectionVariable_GetType(Handle<VariableReflection> inVar);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<Attribute> spReflectionVariable_GetUserAttribute(Handle<VariableReflection> inVar, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionVariable_GetUserAttributeCount(Handle<VariableReflection> inVar);

    [LibraryImport(PLATFORM_PATH)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool spReflectionVariable_HasDefaultValue(Handle<VariableReflection> inVar);
    #endregion

    #region VariableLayoutReflection
    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflectionVariableLayout_GetOffset(Handle<VariableLayoutReflection> inVarLayout, SlangParameterCategory category);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableLayoutReflection> spReflectionVariableLayout_getPendingDataLayout(Handle<VariableLayoutReflection> inVarLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionVariableLayout_GetSemanticName(Handle<VariableLayoutReflection> inVarLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflectionVariableLayout_GetSemanticIndex(Handle<VariableLayoutReflection> inVarLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial size_t spReflectionVariableLayout_GetSpace(Handle<VariableLayoutReflection> inVarLayout, SlangParameterCategory category);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangStage spReflectionVariableLayout_getStage(Handle<VariableLayoutReflection> inVarLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeLayoutReflection> spReflectionVariableLayout_GetTypeLayout(Handle<VariableLayoutReflection> inVarLayout);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableReflection> spReflectionVariableLayout_GetVariable(Handle<VariableLayoutReflection> inVarLayout);
    #endregion

    #region SlangReflectionGeneric
    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<GenericReflection> spReflectionGeneric_applySpecializations(Handle<GenericReflection> currGeneric, Handle<GenericReflection> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionDecl spReflectionGeneric_asDecl(Handle<GenericReflection> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial int64_t spReflectionGeneric_GetConcreteIntVal(Handle<GenericReflection> generic, Handle<VariableReflection> valueParam);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeReflection> spReflectionGeneric_GetConcreteType(Handle<GenericReflection> generic, Handle<VariableReflection> typeParam);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionDecl spReflectionGeneric_GetInnerDecl(Handle<GenericReflection> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangDeclKind spReflectionGeneric_GetInnerKind(Handle<GenericReflection> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionGeneric_GetName(Handle<GenericReflection> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<GenericReflection> spReflectionGeneric_GetOuterGenericContainer(Handle<GenericReflection> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableReflection> spReflectionGeneric_GetTypeParameter(Handle<GenericReflection> generic, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionGeneric_GetTypeParameterConstraintCount(Handle<GenericReflection> generic, Handle<VariableReflection> typeParam);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeReflection> spReflectionGeneric_GetTypeParameterConstraintType(Handle<GenericReflection> generic, Handle<VariableReflection> typeParam, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionGeneric_GetTypeParameterCount(Handle<GenericReflection> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableReflection> spReflectionGeneric_GetValueParameter(Handle<GenericReflection> generic, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionGeneric_GetValueParameterCount(Handle<GenericReflection> generic);
    #endregion

    #region SlangReflectionFunction
    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<FunctionReflection> spReflectionFunction_applySpecializations(Handle<FunctionReflection> func, Handle<GenericReflection> generic);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionDecl spReflectionFunction_asDecl(Handle<FunctionReflection> inFunc);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial SlangReflectionModifier spReflectionFunction_FindModifier(Handle<FunctionReflection> inFunc, SlangModifierID modifierID);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<Attribute> spReflectionFunction_FindUserAttributeByName(Handle<FunctionReflection> inFunc, IGlobalSession* session, byte* name);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<GenericReflection> spReflectionFunction_GetGenericContainer(Handle<FunctionReflection> func);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial byte* spReflectionFunction_GetName(Handle<FunctionReflection> inFunc);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<FunctionReflection> spReflectionFunction_getOverload(Handle<FunctionReflection> func, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionFunction_getOverloadCount(Handle<FunctionReflection> func);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<VariableReflection> spReflectionFunction_GetParameter(Handle<FunctionReflection> inFunc, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionFunction_GetParameterCount(Handle<FunctionReflection> inFunc);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<TypeReflection> spReflectionFunction_GetResultType(Handle<FunctionReflection> inFunc);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<Attribute> spReflectionFunction_GetUserAttribute(Handle<FunctionReflection> inFunc, uint index);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial uint spReflectionFunction_GetUserAttributeCount(Handle<FunctionReflection> inFunc);

    [LibraryImport(PLATFORM_PATH)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool spReflectionFunction_isOverloaded(Handle<FunctionReflection> func);

    [LibraryImport(PLATFORM_PATH)]
    internal static partial Handle<FunctionReflection> spReflectionFunction_specializeWithArgTypes(Handle<FunctionReflection> func, SlangInt argTypeCount, Handle<TypeReflection>* argTypes);
    #endregion
}
