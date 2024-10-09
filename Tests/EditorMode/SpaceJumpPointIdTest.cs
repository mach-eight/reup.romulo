using UnityEngine;
using NUnit.Framework;
using UnityEditor;
using ReupVirtualTwin.models;

namespace ReupVirtualTwinTests.editor
{

    public class SpaceJumpPointIdTest
    {
        static GameObject spaceSelectorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/Quickstart/SpaceSelectors/SpaceSelector.prefab");
        GameObject spaceSelector;

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(spaceSelector);
        }

        [Test]
        public void SpaceJumpPointId_shouldNotBeWhiteSpaces()
        {
            spaceSelector = PrefabUtility.InstantiatePrefab(spaceSelectorPrefab) as GameObject;
            SpaceJumpPoint jumpPointScript = spaceSelector.GetComponent<SpaceJumpPoint>();
            jumpPointScript.id = "    ";
            jumpPointScript.Awake();
            Assert.IsFalse(string.IsNullOrEmpty(jumpPointScript.id.Trim()));

        }
    }

}