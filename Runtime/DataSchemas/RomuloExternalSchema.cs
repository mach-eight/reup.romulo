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
                    { "width_mm", DataValidator.intSchema },
                    { "height_mm", DataValidator.intSchema },
                    { "object_ids", DataValidator.CreateArraySchema(DataValidator.stringSchema) },
                }
            },
            { "required", new JArray { "material_url", "object_ids", "material_id", "width_mm", "height_mm" } },
        };

        public static readonly JObject requestSceneStatePayloadSchema = new JObject
        {
            { "type", DataValidator.objectType },
            { "properties", new JObject
                {
                    { "scene_name", DataValidator.stringSchema },
                }
            },
            { "required", new JArray { "scene_name" } },
        };

        public static readonly JObject requestLoadScenePayloadSchema = new JObject()
        {
            {"type", DataValidator.objectType },
            {"properties", new JObject
                {
                    { "scene_name", DataValidator.stringSchema },
                    { "scene_id", DataValidator.intSchema },
                { "objects", DataValidator.CreateArraySchema(RomuloInternalSchema.objectSceneSchema) },
                }
            },
            { "required", new JArray { "scene_name", "scene_id", "objects" } },
        };
    }
}
