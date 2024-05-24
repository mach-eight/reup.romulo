using Newtonsoft.Json.Linq;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.dataSchemas
{
    public static class RomuloInternalSchema
    {
        public static readonly JObject materialChangeInfo = new()
        {
            { "type", DataValidator.objectType },
            { "properties", new JObject
                {
                    { "material_url", DataValidator.stringSchema },
                    { "object_ids",  DataValidator.CreateArraySchema(new JObject[] { DataValidator.stringSchema })}
                }
            },
        };
    }
}
