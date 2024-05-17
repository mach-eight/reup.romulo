using Newtonsoft.Json.Schema;
using ReupVirtualTwin.enums;

namespace ReupVirtualTwin.dataSchemas
{
    public class RomuloExternalSchema
    {
        public static JSchema BoolPayloadWebMessageSchema = new JSchema
        {
            Type = JSchemaType.Object,
            Properties =
            {
                { "type", new JSchema { Enum = {
                    WebMessageType.setEditMode,
                } } },
                { "payload", new JSchema { Type = JSchemaType.Boolean } }
            },
            Required = { "type", }
        };
        public static JSchema StringPayloadWebMessageSchema = new JSchema
        {
            Type = JSchemaType.Object,
            Properties =
            {
                { "type", new JSchema { Enum = {
                    WebMessageType.setSelectedObjects,
                    WebMessageType.deleteObjects,
                    WebMessageType.loadObject,
                    WebMessageType.changeObjectColor,
                    WebMessageType.changeObjectsMaterial,
                } } },
                { "payload", new JSchema { Type = JSchemaType.String } }
            },
            Required = { "type", }
        };
        public static JSchema OnlyTypeWebMessageSchema = new JSchema
        {
            Type = JSchemaType.Object,
            Properties =
            {
                { "type", new JSchema { Enum = {
                        WebMessageType.activatePositionTransform,
                        WebMessageType.activateRotationTransform,
                        WebMessageType.deactivateTransformMode,
                        WebMessageType.requestModelInfo,
                    }
                }}
            },
            Required = { "type", }
        };
        public static JSchema RequestChangeMaterialSchema = new JSchema
        {
            Type = JSchemaType.Object,
            Properties =
            {
                { "type", new JSchema { Enum = { WebMessageType.changeObjectsMaterial } } },
                { "payload", RomuloInternalSchema.materialChangeInfo }
            }
        };
        public static JSchema IncomingMessageSchema = new JSchema
        {
            OneOf =
            {
                BoolPayloadWebMessageSchema,
                OnlyTypeWebMessageSchema,
                StringPayloadWebMessageSchema,
                RequestChangeMaterialSchema
            }
        };
    }
}
