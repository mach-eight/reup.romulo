using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using ReupVirtualTwin.models;
using Bogus;

namespace ReupVirtualTwinTests.utils
{
    public static class SpaceSelectorFactory

    {
        static GameObject spaceSelectorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/Quickstart/SpaceSelectors/SpaceSelector.prefab");

        public class SpaceSelectorConfig
        {
            public string name;
            public string id;
            public Vector3 position;
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
            spaceSelector.transform.position = config.position;
            if (config.id != null)
            {
                jumpPointScript.id = config.id;
            }
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
            if (config.position == null)
            {
                config.position = new Vector3(0, 0, 0);
            }
            return config;
        }
        public static void DestroySpaceSelectors(List<GameObject> spaceSelectors)
        {
            if (spaceSelectors == null)
            {
                return;
            }
            foreach (GameObject spaceSelector in spaceSelectors)
            {
                GameObject.Destroy(spaceSelector);
            }
        }
    }

}