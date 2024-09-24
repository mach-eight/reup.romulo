using NUnit.Framework;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwinTests.utils;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEditor;


namespace ReupVirtualTwinTests.behaviours
{
    public class ChangeHeightTest
    {
        GameObject platformPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/Platform2.prefab");
        InputTestFixture input;
        ReupSceneInstantiator.SceneObjects sceneObjects;
        Transform characterTransform;
        Keyboard keyboard;

        float timeInSecsForHoldingButton = 0.25f;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateSceneWithBuildingFromPrefab(platformPrefab, SetPlatform);
            input = sceneObjects.input;
            keyboard = InputSystem.AddDevice<Keyboard>();
            characterTransform = sceneObjects.character.transform;
            yield return null;
        }

        void SetPlatform(GameObject platform)
        {
            platform.transform.position = platform.transform.position + new Vector3(0, -3, 0);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }

        private void AssertCloseTo(float expected, float actual, float tolerance)
        {
            Assert.LessOrEqual(Mathf.Abs(expected - actual), tolerance);
        }

        [UnityTest]
        public IEnumerator QKeyShouldDecreaseCharacterHeight()
        {
            float initialHeight = characterTransform.position.y;
            input.Press(keyboard.qKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.qKey);
            AssertCloseTo(
                initialHeight - ChangeHeight.CHANGE_SPEED_M_PER_SECOND * timeInSecsForHoldingButton,
                characterTransform.position.y,
                0.1f
            );
            yield return null;
        }

        [UnityTest]
        public IEnumerator EKeyShouldIncreaseCharacterHeight()
        {
            float initialHeight = characterTransform.position.y;
            input.Press(keyboard.eKey);
            yield return new WaitForSeconds(timeInSecsForHoldingButton);
            input.Release(keyboard.eKey);
            AssertCloseTo(
                initialHeight + ChangeHeight.CHANGE_SPEED_M_PER_SECOND * timeInSecsForHoldingButton,
                characterTransform.position.y,
                0.1f
            );
            yield return null;
        }

    }
}

