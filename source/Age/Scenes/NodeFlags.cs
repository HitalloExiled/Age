namespace Age.Scenes;

[Flags]
public enum NodeFlags
{
    None,
    ChildrenUpdatesSuspended = 1 << 0,
    Sealed                   = 1 << 2,
    UpdatesSuspended         = 1 << 1,
}
