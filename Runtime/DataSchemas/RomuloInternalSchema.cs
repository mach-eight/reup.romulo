using Newtonsoft.Json.Linq;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.dataSchemas
{
    public static class RomuloInternalSchema
    {
        public static JObject materialChangeInfo { get; private set; }
        public static JObject sceneStateSchema { get; private set; }
        public static JObject sceneStateAppearanceSchema { get; private set; }
        public static JObject objectWithChangedMaterialSceneSchema { get; private set; }
        public static JObject objectWithChangedColorSceneSchema { get; private set; }
        public static JObject objectWithNoChangesSceneSchema { get; private set; }
        public static JObject materialSchema { get; private set; }

        static RomuloInternalSchema()
        {
            materialChangeInfo = new()
            {
                { "type", DataValidator.objectType },
                { "properties", new JObject
                    {
                        { "materialId", DataValidator.intSchema },
                        { "materialUrl", DataValidator.stringSchema },
                        { "objectIds",  DataValidator.CreateArraySchema(DataValidator.stringSchema)},
                    }
                },
                { "required", new JArray { "materialUrl", "objectIds", "materialId" } }
            };

            sceneStateAppearanceSchema = new()
            {
                { "type", DataValidator.objectType },
                { "properties", new JObject
                    {
                        { "color", DataValidator.stringSchema },
                        { "materialId", DataValidator.stringSchema}
                    }
                }
            };

            sceneStateSchema = new()
            {
                { "type", DataValidator.objectType },
                { "name", "sceneStateSchema" },
                { "properties", new JObject
                    {
                        { "id", DataValidator.stringSchema },
                        { "appearance", sceneStateAppearanceSchema },
                        { "children", DataValidator.CreateArraySchema(DataValidator.CreateRefSchema("sceneStateSchema"))
                        },
                    }
                },
                { "required", new JArray { "id" } }
            };

            materialSchema = new()
            {
                { "type", DataValidator.objectType },
                { "properties", new JObject
                    {
                        { "id", DataValidator.intSchema },
                        { "texture", DataValidator.stringSchema },
                        { "widthMilimeters", DataValidator.intSchema },
                        { "heightMilimeters", DataValidator.intSchema },
                    }
                },
                { "required", new JArray { "id", "texture", "widthMilimeters", "heightMilimeters" } }
            };

            objectWithChangedMaterialSceneSchema = new()
            {
                { "type", DataValidator.objectType },
                { "properties", new JObject
                    {
                        { "objectId", DataValidator.stringSchema},
                        { "material", materialSchema },
                        { "color", DataValidator.nullSchema },
                    }
                },
                { "required", new JArray { "objectId", "material", "color" } }
            };

            objectWithChangedColorSceneSchema = new()
            {
                { "type", DataValidator.objectType },
                { "properties", new JObject
                    {
                        { "objectId", DataValidator.stringSchema},
                        { "material", DataValidator.nullSchema },
                        { "color", DataValidator.stringSchema },
                    }
                },
                { "required", new JArray { "objectId", "material", "color" } }
            };

            objectWithNoChangesSceneSchema = new()
            {
                { "type", DataValidator.objectType },
                { "properties", new JObject
                    {
                        { "objectId", DataValidator.stringSchema},
                        { "material", DataValidator.nullSchema },
                        { "color", DataValidator.nullSchema },
                    }
                },
                { "required", new JArray { "objectId", "material", "color" } }
            };

        }
    }
}