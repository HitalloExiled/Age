namespace Age.Scenes;

[Flags]
public enum DirtState : byte
{
    None     = 0,
    Subtree  = 1 << 0,
    Commands = 1 << 1,
}
