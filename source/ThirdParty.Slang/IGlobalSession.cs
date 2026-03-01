namespace ThirdParty.Slang;

internal unsafe struct IGlobalSession
{
    internal struct VTable
    {
        public ISlangUnknown.VTable SlangUnknown;

        public delegate* unmanaged<void*, Internal.SessionDesc*, ISession**, SlangResult> CreateSession;
        public delegate* unmanaged<void*, byte*, SlangProfileID>                          FindProfile;
        public void* SetDownstreamCompilerPath;
        public void* SetDownstreamCompilerPrelude;
        public void* GetDownstreamCompilerPrelude;
        public void* GetBuildTagString;
        public void* SetDefaultDownstreamCompiler;
        public void* GetDefaultDownstreamCompiler;
        public void* SetLanguagePrelude;
        public void* GetLanguagePrelude;
        public void* CreateCompileRequest;
        public void* AddBuiltins;
        public void* SetSharedLibraryLoader;
        public void* GetSharedLibraryLoader;
        public void* CheckCompileTargetSupport;
        public void* CheckPassThroughSupport;
        public void* CompileCoreModule;
        public void* LoadCoreModule;
        public void* SaveCoreModule;
        public void* FindCapability;
        public void* SetDownstreamCompilerForTransition;
        public void* GetDownstreamCompilerForTransition;
        public void* GetCompilerElapsedTime;
        public void* SetSpirvCoreGrammar;
        public void* ParseCommandLineArguments;
        public void* GetSessionDescDigest;
        public void* CompileBuiltinModule;
        public void* LoadBuiltinModule;
        public void* SaveBuiltinModule;
    }

    internal VTable* Vtbl;
}
