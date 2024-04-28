using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.managerInterfaces;

public class FollowCharacterTest
{
    private GameObject character;
    private GameObject materialPicker;
    private Rigidbody rb;
    private ICharacterPositionManager posManager;

    private Vector3 originalCharacterPosition = new Vector3(1, 1, 1);

    [SetUp]
    public void SetUp()
    {
        character = new GameObject();
        character.tag = TagsEnum.character;
        character.transform.position = originalCharacterPosition;
        rb = character.AddComponent<Rigidbody>();
        rb.useGravity = false;
        posManager = character.AddComponent<CharacterPositionManager>();

        materialPicker = new GameObject();
        materialPicker.AddComponent<FollowCharacter>();
        materialPicker.transform.position = Vector3.zero;
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.Destroy(character);
        UnityEngine.Object.Destroy(materialPicker);
    }

    [UnityTest]
    public IEnumerator MaterialPickerShouldPointToCharacter()
    {
        //check original character's position
        Assert.AreEqual(originalCharacterPosition, character.transform.position);

        //Calculate a normalized vector from material picker to character
        var v = Vector3.Normalize(character.transform.position - materialPicker.transform.position);
        Assert.IsTrue(materialPicker.transform.forward == v);

        var secondPosition = new Vector3(10, 10, 10);
        character.transform.position = secondPosition;
        Assert.IsTrue(character.transform.position == secondPosition);

        v = Vector3.Normalize(character.transform.position - materialPicker.transform.position);
        Assert.IsTrue(materialPicker.transform.forward == v);

        yield return null;
    }
}
