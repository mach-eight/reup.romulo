using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.dataSchemas
{
    public class RomuloExternalSchema
    {
        public static readonly JSchema changeObjectMaterialPayloadSchema = RomuloInternalSchema.materialChangeInfoSchema;

        public static readonly JSchema requestSceneStatePayloadSchema = JSchema.Parse(@"{
            ""type"": ""object"",
            ""properties"": {
                ""requestTimestamp"": { ""type"": ""integer"" }
            },
            ""required"": [""requestTimestamp""]
        }");

        public static readonly JSchema RequestLoadScenePayloadSchema = JSchema.Parse(@"{
            ""type"": ""object"",
            ""properties"": {
                ""requestTimestamp"": { ""type"": ""integer"" },
                ""objects"": {
                    ""type"": ""array"",
                    ""items"": {
                        ""oneOf"": [
                            " + RomuloInternalSchema.objectWithChangedMaterialSceneSchema.ToString() + @",
                            " + RomuloInternalSchema.objectWithChangedColorSceneSchema.ToString() + @",
                            " + RomuloInternalSchema.objectWithNoChangesSceneSchema.ToString() + @"
                        ]
                    }
                }
            },
            ""required"": [""requestTimestamp"", ""objects""]
        }");

        public static readonly JSchema incomingMessageSchema = JSchema.Parse(@"{
            ""type"": ""object"",
            ""properties"": {
                ""type"": { ""type"": ""string"" },
                ""payload"": {
                    ""anyOf"": [
                        " + changeObjectMaterialPayloadSchema.ToString() + @",
                        " + requestSceneStatePayloadSchema.ToString() + @",
                        " + RequestLoadScenePayloadSchema.ToString() + @",
                        " + RomuloInternalSchema.boolSchema.ToString() + @",
                        " + RomuloInternalSchema.stringSchema.ToString() + @"
                    ]
                }
            },
            ""required"": [""type""]
        }");
    }
}
