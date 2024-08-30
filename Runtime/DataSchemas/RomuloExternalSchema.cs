using Newtonsoft.Json.Linq;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.dataSchemas
{
    public class RomuloExternalSchema
    {
        public static readonly JObject changeObjectMaterialPayloadSchema = RomuloInternalSchema.materialChangeInfo;

        public static readonly JObject requestSceneStatePayloadSchema = new JObject
        {
            { "type", DataValidator.objectType },
            { "properties", new JObject
                {
                    { "requestTimestamp", DataValidator.intSchema },
                }
            },
            { "required", new JArray { "requestTimestamp" } },
        };

        public static readonly JObject requestLoadScenePayloadSchema = new JObject()
        {
            {"type", DataValidator.objectType },
            {"properties", new JObject
                {
                    { "requestTimestamp", DataValidator.intSchema },
                    { "objects", DataValidator.CreateArraySchema
                        (
                            RomuloInternalSchema.objectWithNoChangesSceneSchema,
                            RomuloInternalSchema.objectWithChangedColorSceneSchema,
                            RomuloInternalSchema.objectWithChangedMaterialSceneSchema
                        )
                    },
                }
            },
            { "required", new JArray { "requestTimestamp", "objects" } },
        };
    }
}
