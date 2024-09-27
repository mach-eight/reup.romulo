using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using ReupVirtualTwinTests.utils;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.models;
using UnityEditor;
using ReupVirtualTwinTests.mocks;
using ReupVirtualTwin.managerInterfaces;
using Newtonsoft.Json.Linq;

namespace ReupVirtualTwinTests.managers
{
    public class SpacesRecordTest : MonoBehaviour
    {
        GameObject platformPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/Platform2.prefab");
        ReupSceneInstantiator.SceneObjects sceneObjects;
        SpacesRecord spacesRecord;
        List<GameObject> spaceSelectors;
        Transform character;
        float lowPlatformHeight = -3;
        MediatorSpy mediator;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateSceneWithBuildingFromPrefab(platformPrefab, SetLowPlatform);
            spacesRecord = sceneObjects.spacesRecord;
            character = sceneObjects.character;
            mediator = new MediatorSpy();
            spacesRecord.mediator = mediator;
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
            Vector3 spaceJumpPointPosition = new Vector3(0.2f, 2, 0.3f);
            spaceSelectors = new List<GameObject>(){
                SpaceSelectorFabric.Create(new SpaceSelectorFabric.SpaceSelectorConfig
                {
                    id = spaceJumpPointId,
                    position = spaceJumpPointPosition,
                })
            };
            yield return null;
            spacesRecord.GoToSpace(spaceJumpPointId, "request-1");
            yield return new WaitForSeconds(1);
            Vector3 desiredPositionAfterJump = new Vector3(spaceJumpPointPosition.x, characterYPosition, spaceJumpPointPosition.z);
            AssertUtils.AssertVectorsAreClose(desiredPositionAfterJump, character.position, 0.02);
        }

        [UnityTest]
        public IEnumerator ShouldNotifyMediatorWhenSpacePointIsReached()
        {
            string spaceJumpPointId = "space-jump-point-1";
            string requestId = "request-1";
            spaceSelectors = new List<GameObject>(){
                SpaceSelectorFabric.Create(new SpaceSelectorFabric.SpaceSelectorConfig
                {
                    id = spaceJumpPointId,
                })
            };
            yield return null;
            spacesRecord.GoToSpace(spaceJumpPointId, requestId);
            yield return null;
            Assert.AreEqual(1, mediator.receivedEvents.Count);
            Assert.AreEqual(ReupVirtualTwin.enums.ReupEvent.spaceJumpPointReached, mediator.receivedEvents[0]);
            JObject expectedPayload = new JObject
            {
                { "spaceId", spaceJumpPointId },
                { "requestId", requestId },
            };
            Assert.AreEqual(expectedPayload, mediator.receivedPayloads[0]);
        }

    }
}