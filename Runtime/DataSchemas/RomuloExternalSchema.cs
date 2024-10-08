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

        public static readonly JSchema slideToSpacePayloadSchema = RomuloInternalSchema.spaceJumpInfoEventPayload;
        public static readonly JSchema showOrHideObjectsPayloadSchema = JSchema.Parse(@"{
            ""type"": ""object"",
            ""properties"": {
                ""objectIds"": {
                    ""type"": ""array"",
                    ""items"": { ""type"": ""string"" },
                },
                ""requestId"": { ""type"": ""string"" }
            },
            ""required"": [""objectIds"", ""requestId""]
        }");

        public static readonly JSchema simpleRequestIdPayloadSchema = JSchema.Parse(@"{
            ""type"": ""object"",
            ""properties"": {
                ""requestId"": { ""type"": ""string"" }
            },
            ""required"": [""requestId""]
        }");

        public static readonly JSchema activateViewModeSchema = JSchema.Parse(@"{
            ""type"": ""object"",
            ""properties"": {
                ""viewMode"": { ""enum"": [""FPV"", ""DHV""] },
                ""requestId"": { ""type"": ""string"" }
            },
            ""required"": [""requestId"", ""viewMode""]
        }");
    }

}
