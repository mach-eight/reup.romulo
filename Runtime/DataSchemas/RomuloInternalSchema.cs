using Newtonsoft.Json.Schema;

namespace ReupVirtualTwin.dataSchemas
{
    public class RomuloInternalSchema
    {
        public static JSchema materialChangeInfo = new JSchema
        {
            SchemaVersion = new System.Uri("http://json-schema.org/draft-07/schema#"),
            Type = JSchemaType.Object,
            Properties =
            {
                { "material_url", new JSchema { Type = JSchemaType.String } },
                { "object_ids", new JSchema { Type = JSchemaType.Array, Items = { new JSchema { Type = JSchemaType.String } } } },
            },
            Required = { "material_url", "object_ids" },
        };
    }
}
