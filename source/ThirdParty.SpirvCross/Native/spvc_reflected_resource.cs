namespace ThirdParty.SpirvCross.Native;

public unsafe struct spvc_reflected_resource
{
    public spvc_variable_id id;
	public spvc_type_id base_type_id;
	public spvc_type_id type_id;
	public byte* name;
}
