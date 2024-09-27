using Newtonsoft.Json.Schema;

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

        public static readonly JSchema requestLoadScenePayloadSchema = JSchema.Parse(@"{
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

        public static readonly JSchema slideToSpacePayloadSchema = JSchema.Parse(@"{
            ""type"": ""object"",
            ""properties"": {
                ""requestId"": { ""type"": ""string"" },
                ""spaceId"": { ""type"": ""string"" },
            },
            ""required"": [""requestId"", ""spaceId""]
        }");

    }
}
