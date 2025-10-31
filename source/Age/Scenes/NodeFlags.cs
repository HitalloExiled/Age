namespace Age.Scenes;

[Flags]
public enum NodeFlags : byte
{
    None,
    ChildrenUpdatesSuspended = 1 << 0,
    UpdatesSuspended         = 1 << 1,
    Sealed                   = 1 << 2,
    Slotted                  = 1 << 3,
}
