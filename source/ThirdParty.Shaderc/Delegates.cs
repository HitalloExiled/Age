namespace ThirdParty.Shaderc;

/// <summary>
/// <para>An includer callback type for mapping an #include request to an include result.</para>
/// <para>The user_data parameter specifies the client context.</para>
/// <para>The requested_source parameter specifies the name of the source being requested.</para>
/// <para>The type parameter specifies the kind of inclusion request being made.</para>
/// <para>The requesting_source parameter specifies the name of the source containing the #include request.</para>
/// <para>The includer owns the result object and its contents, and both must remain valid until the release callback is called on the result object.</para>
/// </summary>
public unsafe delegate shaderc_include_result* IncludeResolveFn(void* userData, byte* requestedSource, int type, byte* requestingSource, size_t includeDepth);

/// <summary>
/// An includer callback type for destroying an include result.
/// </summary>
public unsafe delegate void IncludeResultReleaseFn(void* userData, shaderc_include_result* includeResult);
