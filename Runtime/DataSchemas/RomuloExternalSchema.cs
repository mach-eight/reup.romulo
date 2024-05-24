using Newtonsoft.Json.Linq;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.dataSchemas
{
    public class RomuloExternalSchema
    {
        public static readonly JObject changeObjectMaterialPayloadSchema = new JObject
        {
            { "type", DataValidator.objectType },
            { "properties", new JObject
                {
                    { "material_url", DataValidator.stringSchema },
                    { "object_ids", DataValidator.CreateArraySchema(new JObject[] { DataValidator.stringSchema }) },
                }
            }
        };
        //public static JSchema BoolPayloadWebMessageSchema = new JSchema
        //{
        //    SchemaVersion = new System.Uri("http://json-schema.org/draft-07/schema#"),
        //    Type = JSchemaType.Object,
        //    Properties =
        //    {
        //        { "type", new JSchema { Enum = {
        //            WebMessageType.setEditMode,
        //        } } },
        //        { "payload", new JSchema { Type = JSchemaType.Boolean } }
        //    },
        //    Required = { "type", }
        //};
        //public static JSchema StringPayloadWebMessageSchema = new JSchema
        //{
        //    SchemaVersion = new System.Uri("http://json-schema.org/draft-07/schema#"),
        //    Type = JSchemaType.Object,
        //    Properties =
        //    {
        //        { "type", new JSchema { Enum = {
        //            WebMessageType.setSelectedObjects,
        //            WebMessageType.deleteObjects,
        //            WebMessageType.loadObject,
        //            WebMessageType.changeObjectColor,
        //            WebMessageType.changeObjectsMaterial,
        //        } } },
        //        { "payload", new JSchema { Type = JSchemaType.String } }
        //    },
        //    Required = { "type", }
        //};
        //public static JSchema OnlyTypeWebMessageSchema = new JSchema
        //{
        //    SchemaVersion = new System.Uri("http://json-schema.org/draft-07/schema#"),
        //    Type = JSchemaType.Object,
        //    Properties =
        //    {
        //        { "type", new JSchema { Enum = {
        //                WebMessageType.activatePositionTransform,
        //                WebMessageType.activateRotationTransform,
        //                WebMessageType.deactivateTransformMode,
        //                WebMessageType.requestModelInfo,
        //            }
        //        }}
        //    },
        //    Required = { "type", }
        //};
        //public static JSchema RequestChangeMaterialSchema = new JSchema
        //{
        //    SchemaVersion = new System.Uri("http://json-schema.org/draft-07/schema#"),
        //    Type = JSchemaType.Object,
        //    Properties =
        //    {
        //        { "type", new JSchema { Enum = { WebMessageType.changeObjectsMaterial } } },
        //        { "payload", RomuloInternalSchema.materialChangeInfo }
        //    }
        //};
        //public static JSchema IncomingMessageSchema = new JSchema
        //{
        //    OneOf =
        //    {
        //        BoolPayloadWebMessageSchema,
        //        OnlyTypeWebMessageSchema,
        //        StringPayloadWebMessageSchema,
        //        RequestChangeMaterialSchema
        //    }
        //};
    }
}
