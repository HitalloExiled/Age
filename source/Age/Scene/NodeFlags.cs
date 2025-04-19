namespace Age.Scene;

[Flags]
public enum NodeFlags
{
    None,
    IgnoreChildrenUpdates = 1 << 0,
    IgnoreUpdates         = 1 << 1,
    Immutable             = 1 << 2,
}
