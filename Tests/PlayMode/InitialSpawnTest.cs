using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using ReupVirtualTwinTests.instantiators;

namespace ReupVirtualTwinTests.behaviours
{
    public class InitialSpawnTest : MonoBehaviour
    {
        GameObject platformPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/Platform2.prefab");
        ReupSceneInstantiator.SceneObjects sceneObjects;
        Transform characterTransform;
        float lowPlatformHeight = -3;
        float HighPlatformHeight = -0.5f;
        float platformWidth = 0.1f;

        void SetHighPlatform(GameObject platform)
        {
            platform.transform.position = new Vector3(0, HighPlatformHeight, 0);
        }
        void SetLowPlatform(GameObject platform)
        {
            platform.transform.position = new Vector3(0, lowPlatformHeight, 0);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }

        [UnityTest]
        public IEnumerator CharacterShouldSpawnAtDesiredHeight_when_placedOverDesiredHeight()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateSceneWithBuildingFromPrefab(platformPrefab, SetLowPlatform);
            yield return null;
            characterTransform = sceneObjects.character.transform;
            float initialHeight = characterTransform.position.y;
            float expectedHeight = lowPlatformHeight + sceneObjects.heightMediator.initialCharacterHeight + platformWidth / 2;
            Assert.AreEqual(expectedHeight, initialHeight);
            yield return null;
        }

        [UnityTest]
        public IEnumerator CharacterShouldSpawnAtDesiredHeight_when_placedUnderDesiredHeight()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateSceneWithBuildingFromPrefab(platformPrefab, SetHighPlatform);
            yield return null;
            characterTransform = sceneObjects.character.transform;
            float initialHeight = characterTransform.position.y;
            float expectedHeight = HighPlatformHeight + sceneObjects.heightMediator.initialCharacterHeight + platformWidth / 2;
            Assert.AreEqual(expectedHeight, initialHeight);
            yield return null;
        }


    }
}
