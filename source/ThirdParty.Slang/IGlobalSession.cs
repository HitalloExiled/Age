namespace ThirdParty.Slang;

internal unsafe struct IGlobalSession
{
    internal struct VTable
    {
        internal ISlangUnknown.VTable SlangUnknown;

        internal delegate* unmanaged<void*, SessionDesc*, ISession**, SlangResult> CreateSession;
        internal delegate* unmanaged<void*, byte*, SlangProfileID>                          FindProfile;
        internal void* SetDownstreamCompilerPath;
        internal void* SetDownstreamCompilerPrelude;
        internal void* GetDownstreamCompilerPrelude;
        internal void* GetBuildTagString;
        internal void* SetDefaultDownstreamCompiler;
        internal void* GetDefaultDownstreamCompiler;
        internal void* SetLanguagePrelude;
        internal void* GetLanguagePrelude;
        internal void* CreateCompileRequest;
        internal void* AddBuiltins;
        internal void* SetSharedLibraryLoader;
        internal void* GetSharedLibraryLoader;
        internal void* CheckCompileTargetSupport;
        internal void* CheckPassThroughSupport;
        internal void* CompileCoreModule;
        internal void* LoadCoreModule;
        internal void* SaveCoreModule;
        internal void* FindCapability;
        internal void* SetDownstreamCompilerForTransition;
        internal void* GetDownstreamCompilerForTransition;
        internal void* GetCompilerElapsedTime;
        internal void* SetSpirvCoreGrammar;
        internal void* ParseCommandLineArguments;
        internal void* GetSessionDescDigest;
        internal void* CompileBuiltinModule;
        internal void* LoadBuiltinModule;
        internal void* SaveBuiltinModule;
    }

    internal VTable* Vtbl;
}
