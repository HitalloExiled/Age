namespace Age.Platforms.Linux.LibXKBCommon;

internal unsafe struct xkb_rule_names
{
    public byte* rules;
    public byte* model;
    public byte* layout;
    public byte* variant;
    public byte* options;
};
