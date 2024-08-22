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
                    { "material_id", DataValidator.intSchema },
                    { "material_url", DataValidator.stringSchema },
                    { "object_ids", DataValidator.CreateArraySchema(DataValidator.stringSchema) },
                }
            },
            { "required", new JArray { "material_url", "object_ids", "material_id" } },
        };

        public static readonly JObject requestSceneStatePayloadSchema = new JObject
        {
            { "type", DataValidator.objectType },
            { "properties", new JObject
                {
                    { "request_timestamp", DataValidator.intSchema },
                }
            },
            { "required", new JArray { "request_timestamp" } },
        };

        public static readonly JObject requestLoadScenePayloadSchema = new JObject()
        {
            {"type", DataValidator.objectType },
            {"properties", new JObject
                {
                    { "request_timestamp", DataValidator.intSchema },
                    { "objects", DataValidator.CreateArraySchema(RomuloInternalSchema.objectSceneSchema) },
                }
            },
            { "required", new JArray { "request_timestamp", "objects" } },
        };
    }
}
