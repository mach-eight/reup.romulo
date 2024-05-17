using Newtonsoft.Json.Schema;

namespace ReupVirtualTwin.dataSchemas
{
    public class InternalSchema
    {
        public static JSchema materialChangeInfo = JSchema.Parse(@"
        {
            '$schema': 'http://json-schema.org/draft-07/schema#',
            'type': 'object',
            'properties': {
            'material_url': {'type': 'string'},
            'object_ids': {'type': 'array', 'items': {'type': 'string'}}
            },
            'required': ['material_url', 'object_ids']
        }");
    }
}
