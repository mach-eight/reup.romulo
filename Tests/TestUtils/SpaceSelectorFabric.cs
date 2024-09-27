using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using ReupVirtualTwin.models;
using Bogus;

namespace ReupVirtualTwinTests.utils
{
    public static class SpaceSelectorFabric

    {
        static GameObject spaceSelectorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/Quickstart/SpaceSelectors/SpaceSelector.prefab");

        public class SpaceSelectorConfig
        {
            public string name;
            // public string id;
        }

        public static GameObject Create()
        {
            SpaceSelectorConfig fullConfig = GetFullConfig();
            return CreateFromConfig(fullConfig);
        }
        public static GameObject Create(SpaceSelectorConfig config)
        {
            SpaceSelectorConfig fullConfig = GetFullConfig(config);
            return CreateFromConfig(fullConfig);
        }

        static GameObject CreateFromConfig(SpaceSelectorConfig config)
        {
            GameObject spaceSelector = GameObject.Instantiate(spaceSelectorPrefab);
            spaceSelector.name = config.name;
            SpaceJumpPoint jumpPointScript = spaceSelector.GetComponent<SpaceJumpPoint>();
            jumpPointScript.spaceName = config.name;
            return spaceSelector;
        }

        public static List<GameObject> CreateBulk(int count)
        {
            return Enumerable.Range(0, count).Select(_ => Create()).ToList();
        }

        static SpaceSelectorConfig GetFullConfig()
        {
            SpaceSelectorConfig config = new SpaceSelectorConfig();
            return GetFullConfig(config);
        }
        static SpaceSelectorConfig GetFullConfig(SpaceSelectorConfig config)
        {
            if (config.name == null)
            {
                config.name = new Faker().Company.CompanyName();
            }
            // if (config.id == null)
            // {
            //     config.id = new Faker().Random.Guid().ToString();
            // }
            return config;
        }
    }

}