using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using Bogus;
using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwinTests.utils
{
    public static class TagFactory
    {
        static Faker<Tag> faker;
        public static Tag Create()
        {
            Faker<Tag> faker = GetFaker();
            return Create(faker);
        }

        public static List<Tag> CreateBulk(int count)
        {
            Faker<Tag> faker = GetFaker();
            return CreateBulk(count, faker);
        }

        public static List<Tag> CreateBulk(int count, Faker<Tag> faker){
            return Enumerable.Range(0, count).Select(_ => Create(faker)).ToList();
        }

        static Tag Create(Faker<Tag> faker)
        {
            return faker.Generate();
        }

        static Faker<Tag> GetFaker()
        {
            if (faker != null)
            {
                return faker;
            }
            faker = CreateFaker();
            return faker;
        }
        public static Faker<Tag> CreateFaker(){
            var tagId = 0;
            return new Faker<Tag>().StrictMode(true)
                .RuleFor(t => t.id, f => tagId++)
                .RuleFor(t => t.name, f => f.Name.FirstName())
                .RuleFor(t => t.description, f => f.Lorem.Sentence())
                .RuleFor(t => t.priority, f => f.Random.Int(0, 100));
        }

    }

}