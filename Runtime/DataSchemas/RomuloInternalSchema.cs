using Newtonsoft.Json.Linq;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.dataSchemas
{
    public static class RomuloInternalSchema
    {
        public static readonly JObject materialChangeInfo = new()
        {
            { "material_url", DataValidator.stringSchema },
            { "object_ids",  DataValidator.CreateArraySchema(new JObject[] { DataValidator.stringSchema })}
        };
    }
}
