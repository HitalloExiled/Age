namespace Age.Scene;

[Flags]
public enum NodeFlags
{
    None,
    IgnoreUpdates         = 1 << 0,
    IgnoreChildrenUpdates = 1 << 1,
}
