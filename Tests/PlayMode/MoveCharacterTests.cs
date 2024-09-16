using NUnit.Framework;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwinTests.utils;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace ReupVirtualTwinTests.behaviours
{
    public class MoveCharacterTests
    {
        InputTestFixture input;
        ReupSceneInstantiator.SceneObjects sceneObjects;
        Keyboard keyboard;
        Transform characterTransform;

        float timeInSecsForEachAction = 0.25f;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateScene();
            input = sceneObjects.input;
            keyboard = InputSystem.AddDevice<Keyboard>();
            characterTransform = sceneObjects.character.transform;
            yield return null;
        }
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }

        [Test]
        public void WalkSpeedIsDefined()
        {
            Assert.AreEqual(3.5f, CharacterMovementKeyboard.WALK_SPEED_M_PER_SECOND);
        }

        [UnityTest]
        public IEnumerator WKeyShouldMoveCharacterForward()
        {
            Assert.AreEqual(Vector3.zero, characterTransform.position);
            input.Press(keyboard.wKey);
            yield return new WaitForSeconds(timeInSecsForEachAction);
            input.Release(keyboard.wKey);
            Assert.GreaterOrEqual(characterTransform.position.z, CharacterMovementKeyboard.WALK_SPEED_M_PER_SECOND * timeInSecsForEachAction);
            Assert.Zero(characterTransform.position.x);
            Assert.Zero(characterTransform.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator AKeyShouldMoveCharacterToTheLeft()
        {
            Assert.AreEqual(Vector3.zero, characterTransform.position);
            input.Press(keyboard.aKey);
            yield return new WaitForSeconds(timeInSecsForEachAction);
            input.Release(keyboard.aKey);
            Assert.LessOrEqual(characterTransform.position.x, -1 * CharacterMovementKeyboard.WALK_SPEED_M_PER_SECOND * timeInSecsForEachAction);
            Assert.Zero(characterTransform.position.z);
            Assert.Zero(characterTransform.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator DKeyShouldMoveCharacterToTheRight()
        {
            Assert.AreEqual(Vector3.zero, characterTransform.position);
            input.Press(keyboard.dKey);
            yield return new WaitForSeconds(timeInSecsForEachAction);
            input.Release(keyboard.dKey);
            Assert.GreaterOrEqual(characterTransform.position.x, CharacterMovementKeyboard.WALK_SPEED_M_PER_SECOND * timeInSecsForEachAction);
            Assert.Zero(characterTransform.position.z);
            Assert.Zero(characterTransform.position.y);
            yield return null;
        }

        [UnityTest]
        public IEnumerator SKeyShouldMoveCharacterBackwards()
        {
            Assert.AreEqual(Vector3.zero, characterTransform.position);
            input.Press(keyboard.sKey);
            yield return new WaitForSeconds(timeInSecsForEachAction);
            input.Release(keyboard.sKey);
            Assert.LessOrEqual(characterTransform.position.z, -1 * CharacterMovementKeyboard.WALK_SPEED_M_PER_SECOND * timeInSecsForEachAction);
            Assert.Zero(characterTransform.position.x);
            Assert.Zero(characterTransform.position.y);
            yield return null;
        }


    }
}
