using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using ReupVirtualTwinTests.utils;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.models;
using UnityEditor;

namespace ReupVirtualTwinTests.managers
{
    public class SpacesRecordTest : MonoBehaviour
    {
        GameObject platformPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/Platform2.prefab");
        ReupSceneInstantiator.SceneObjects sceneObjects;
        ISpacesRecord spacesRecord;
        List<GameObject> spaceSelectors;
        Transform character;
        float lowPlatformHeight = -3;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateSceneWithBuildingFromPrefab(platformPrefab, SetLowPlatform);
            spacesRecord = sceneObjects.spacesRecord;
            character = sceneObjects.character;
            yield return null;
        }
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            SpaceSelectorFabric.DestroySpaceSelectors(spaceSelectors);
            spaceSelectors?.Clear();
            yield return null;
        }

        void SetLowPlatform(GameObject platform)
        {
            platform.transform.position = new Vector3(0, lowPlatformHeight, 0);
        }

        [UnityTest]
        public IEnumerator SpacesListShouldBeAnEmptyList_if_noSpacesExist()
        {
            Assert.AreEqual(0, spacesRecord.jumpPoints.Count);
            yield return null;
        }

        [UnityTest]
        public IEnumerator SpacesListShouldContainAllSpaceJumpPointsInTheScene()
        {
            spaceSelectors = SpaceSelectorFabric.CreateBulk(5);
            yield return null;
            Assert.AreEqual(5, spacesRecord.jumpPoints.Count);
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(spaceSelectors[i].GetComponent<SpaceJumpPoint>(), spacesRecord.jumpPoints[i]);
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator CharacterShouldMoveToSpaceJumpPoint()
        {
            string spaceJumpPointId = "space-jump-point-1";
            float characterYPosition = character.position.y;
            Vector3 spaceJumpPointPosition = new Vector3(1, 2, 3);
            spaceSelectors = new List<GameObject>(){
                SpaceSelectorFabric.Create(new SpaceSelectorFabric.SpaceSelectorConfig
                {
                    id = spaceJumpPointId,
                    position = spaceJumpPointPosition,
                })
            };
            yield return null;
            spacesRecord.GoToSpace(spaceJumpPointId);
            yield return new WaitForSeconds(1);
            Vector3 desiredPositionAfterJump = new Vector3(spaceJumpPointPosition.x, characterYPosition, spaceJumpPointPosition.z);
            Assert.AreEqual(desiredPositionAfterJump, character.position);
        }
    }
}