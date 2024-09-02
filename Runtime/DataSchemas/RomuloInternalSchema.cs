using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using ReupVirtualTwin.helpers;
using System;

namespace ReupVirtualTwin.dataSchemas
{
    public static class RomuloInternalSchema
    {
        public static JSchema boolSchema { get; private set; }
        public static JSchema intSchema { get; private set; }
        public static JSchema stringSchema { get; private set; }
        public static JSchema nullSchema { get; private set; }
        public static JSchema materialChangeInfoSchema { get; private set; }
        public static JSchema sceneStateSchema { get; private set; }
        public static JSchema sceneStateAppearanceSchema { get; private set; }
        public static JSchema objectWithChangedMaterialSceneSchema { get; private set; }
        public static JSchema objectWithChangedColorSceneSchema { get; private set; }
        public static JSchema objectWithNoChangesSceneSchema { get; private set; }
        public static JSchema materialSchema { get; private set; }

        static RomuloInternalSchema()
        {
            stringSchema = JSchema.Parse(@"{ ""type"": ""string"" }");

            intSchema = JSchema.Parse(@"{ ""type"": ""integer"" }");

            boolSchema = JSchema.Parse(@"{ ""type"": ""boolean"" }");

            nullSchema = JSchema.Parse(@"{ ""type"": ""null"" }");

            materialSchema = JSchema.Parse(@"{
                ""type"": ""object"",
                ""properties"": {
                    ""id"": { ""type"": ""string"" },
                    ""texture"": { ""type"": ""string"" },
                    ""widthMilimeters"": { ""type"": ""integer"" },
                    ""heightMilimeters"": { ""type"": ""integer"" }
                },
                ""required"": [""id"", ""texture"", ""widthMilimeters"", ""heightMilimeters""]
            }");

            materialChangeInfoSchema = JSchema.Parse(@"{
                ""type"": ""object"",
                ""properties"": {
                    ""material"": " + materialSchema.ToString() + @",
                    ""objectIds"": { ""type"": ""array"", ""items"": { ""type"": ""string"" } }
                },
                ""required"": [""objectIds"", ""material""]
            }");

            sceneStateAppearanceSchema = JSchema.Parse(@"{
                ""type"": ""object"",
                ""properties"": {
                    ""color"": { ""type"": ""string"" },
                    ""materialId"": { ""type"": ""string"" }
                }
            }");

            sceneStateSchema = JSchema.Parse(@"{
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

            objectWithChangedMaterialSceneSchema = JSchema.Parse(@"{
                ""type"": ""object"",
                ""properties"": {
                    ""objectId"": { ""type"": ""string"" },
                    ""material"": " + materialSchema.ToString() + @",
                    ""color"": { ""type"": ""null"" }
                },
                ""required"": [""objectId"", ""material"", ""color""]
            }");

            objectWithChangedColorSceneSchema = JSchema.Parse(@"{
                ""type"": ""object"",
                ""properties"": {
                    ""objectId"": { ""type"": ""string"" },
                    ""material"": { ""type"": ""null"" },
                    ""color"": { ""type"": ""string"" }
                },
                ""required"": [""objectId"", ""material"", ""color""]
            }");

            objectWithNoChangesSceneSchema = JSchema.Parse(@"{
                ""type"": ""object"",
                ""properties"": {
                    ""objectId"": { ""type"": ""string"" },
                    ""material"": { ""type"": ""null"" },
                    ""color"": { ""type"": ""null"" }
                },
                ""required"": [""objectId"", ""material"", ""color""]
            }");

        }
    }
}