using System.Runtime.InteropServices;
using ThirdParty.SpirvCross.Enums;
using ThirdParty.SpirvCross.Native;

namespace ThirdParty.SpirvCross;

public unsafe class Resources
{
    private readonly Context        context;
    private readonly spvc_resources handler;

    internal Resources(nint handler, Context context)
    {
        this.handler = handler;
        this.context = context;
    }

    public ReflectedResource[] GetResourceListForType(ResorceType resorceType)
    {
        spvc_reflected_resource* resourceList;
        ulong                    resourceSize;

        this.context.CheckResult(PInvoke.spvc_resources_get_resource_list_for_type(this.handler, (spvc_resource_type)resorceType, &resourceList, &resourceSize));

        var resources = new ReflectedResource[resourceSize];

        for (var i = 0ul; i < resourceSize; i++)
        {
            resources[i] = new()
            {
                BaseTypeId = resourceList[i].base_type_id,
                Id         = resourceList[i].id,
                Name       = Marshal.PtrToStringAnsi((nint)resourceList[i].name)!,
                TypeId     = resourceList[i].type_id,
            };
        }

        return resources;
    }
}
