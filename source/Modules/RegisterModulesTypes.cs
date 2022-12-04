namespace Age.Modules;

internal interface IRegisterModulesTypes
{
    enum ModuleInitializationLevel
    {
        MODULE_INITIALIZATION_LEVEL_CORE, // = GDEXTENSION_INITIALIZATION_CORE,
        MODULE_INITIALIZATION_LEVEL_SERVERS, // = GDEXTENSION_INITIALIZATION_SERVERS,
        MODULE_INITIALIZATION_LEVEL_SCENE, // = GDEXTENSION_INITIALIZATION_SCENE,
        MODULE_INITIALIZATION_LEVEL_EDITOR, // = GDEXTENSION_INITIALIZATION_EDITOR
    };

    void InitializeModules(ModuleInitializationLevel level);
    void UninitializeModules(ModuleInitializationLevel level);
}
