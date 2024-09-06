using Newtonsoft.Json.Schema;

namespace ReupVirtualTwin.dataSchemas
{
    public static class BasicTypesSchemas
    {
        public static readonly JSchema boolSchema = JSchema.Parse(@"{ ""type"": ""boolean"" }");

        public static readonly JSchema intSchema = JSchema.Parse(@"{ ""type"": ""integer"" }");

        public static readonly JSchema stringSchema = JSchema.Parse(@"{ ""type"": ""string"" }");

        public static readonly JSchema nullSchema = JSchema.Parse(@"{ ""type"": ""null"" }");
    }

}
