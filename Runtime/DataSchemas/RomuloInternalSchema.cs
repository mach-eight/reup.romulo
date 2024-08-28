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
                        { "material_id", DataValidator.intSchema },
                        { "material_url", DataValidator.stringSchema },
                        { "object_ids",  DataValidator.CreateArraySchema(DataValidator.stringSchema)},
                    }
                },
                { "required", new JArray { "material_url", "object_ids", "material_id" } }
            };

            sceneStateAppearanceSchema = new()
            {
                { "type", DataValidator.objectType },
                { "properties", new JObject
                    {
                        { "color", DataValidator.stringSchema },
                        { "material_id", DataValidator.stringSchema}
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

            objectWithChangedMaterialSceneSchema = new()
            {
                { "type", DataValidator.objectType },
                { "properties", new JObject
                    {
                        { "object_id", DataValidator.stringSchema},
                        { "material", materialSchema },
                        { "color", DataValidator.nullSchema },
                    }
                },
                { "required", new JArray { "object_id", "material", "color" } }
            };

            objectWithChangedColorSceneSchema = new()
            {
                { "type", DataValidator.objectType },
                { "properties", new JObject
                    {
                        { "object_id", DataValidator.stringSchema},
                        { "material", DataValidator.nullSchema },
                        { "color", DataValidator.stringSchema },
                    }
                },
                { "required", new JArray { "object_id", "material", "color" } }
            };

            objectWithNoChangesSceneSchema = new()
            {
                { "type", DataValidator.objectType },
                { "properties", new JObject
                    {
                        { "object_id", DataValidator.stringSchema},
                        { "material", DataValidator.nullSchema },
                        { "color", DataValidator.nullSchema },
                    }
                },
                { "required", new JArray { "object_id", "material", "color" } }
            };

            materialSchema = new()
            {
                { "type", DataValidator.objectType },
                { "properties", new JObject
                    {
                        { "id", DataValidator.intSchema },
                        { "texture", DataValidator.stringSchema },
                        { "width_mm", DataValidator.intSchema },
                        { "height_mm", DataValidator.intSchema },
                    }
                },
                { "required", new JArray { "id", "texture", "width_mm", "height_mm" } }
            };
        }
    }
}