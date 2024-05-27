using Newtonsoft.Json.Linq;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.dataSchemas
{
    public class RomuloExternalSchema
    {
        public static readonly JObject changeObjectMaterialPayloadSchema = new JObject
        {
            { "type", DataValidator.objectType },
            { "properties", new JObject
                {
                    { "material_url", DataValidator.stringSchema },
                    { "object_ids", DataValidator.CreateArraySchema(new JObject[] { DataValidator.stringSchema }) },
                }
            }
        };
    }
}
