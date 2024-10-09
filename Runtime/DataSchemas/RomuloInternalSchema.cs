using Newtonsoft.Json.Schema;

namespace ReupVirtualTwin.dataSchemas
{
    public static class RomuloInternalSchema
    {
        public static readonly JSchema materialSchema = JSchema.Parse(@"{
            ""type"": ""object"",
            ""properties"": {
                ""id"": { ""type"": ""integer"" },
                ""texture"": { ""type"": ""string"" },
                ""widthMillimeters"": { ""type"": ""integer"" },
                ""heightMillimeters"": { ""type"": ""integer"" }
            },
            ""required"": [""id"", ""texture"", ""widthMillimeters"", ""heightMillimeters""]
        }");

        public static readonly JSchema materialChangeInfoSchema = JSchema.Parse(@"{
            ""type"": ""object"",
            ""properties"": {
                ""material"": " + materialSchema.ToString() + @",
                ""objectIds"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } }
            },
            ""required"": [""objectIds"", ""material""]
        }");

        public static readonly JSchema sceneStateAppearanceSchema = JSchema.Parse(@"{
            ""type"": ""object"",
            ""properties"": {
                ""color"": { ""type"": ""string"" },
                ""materialId"": { ""type"": ""string"" }
            }
        }");

        public static readonly JSchema sceneStateSchema = JSchema.Parse(@"{
            ""type"": ""object"",
            ""properties"": {
                ""id"": { ""type"": ""string"" },
                ""appearance"": " + sceneStateAppearanceSchema.ToString() + @",
                ""children"": {
                    ""type"": ""array"",
                    ""items"": { ""$ref"": ""#"" }
                }
            },
            ""required"": [""id""]
        }");

        public static readonly JSchema objectWithChangedMaterialSceneSchema = JSchema.Parse(@"{
            ""type"": ""object"",
            ""properties"": {
                ""objectId"": { ""type"": ""string"" },
                ""material"": " + materialSchema.ToString() + @",
                ""color"": { ""type"": ""null"" }
            },
            ""required"": [""objectId"", ""material"", ""color""]
        }");

        public static readonly JSchema objectWithChangedColorSceneSchema = JSchema.Parse(@"{
            ""type"": ""object"",
            ""properties"": {
                ""objectId"": { ""type"": ""string"" },
                ""material"": { ""type"": ""null"" },
                ""color"": { ""type"": ""string"" }
            },
            ""required"": [""objectId"", ""material"", ""color""]
        }");

        public static readonly JSchema objectWithNoChangesSceneSchema = JSchema.Parse(@"{
            ""type"": ""object"",
            ""properties"": {
                ""objectId"": { ""type"": ""string"" },
                ""material"": { ""type"": ""null"" },
                ""color"": { ""type"": ""null"" }
            },
            ""required"": [""objectId"", ""material"", ""color""]
        }");

        public static readonly JSchema spaceJumpInfoEventPayload = JSchema.Parse(@"{
            ""type"": ""object"",
            ""properties"": {
                ""spaceId"": { ""type"": ""string"" },
                ""requestId"": { ""type"": ""string"" }
            },
            ""required"": [""spaceId"", ""requestId""]
        }");

    }
}