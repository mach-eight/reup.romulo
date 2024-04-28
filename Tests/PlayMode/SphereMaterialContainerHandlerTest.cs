using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using ReupVirtualTwin.enums;
using UnityEditor;
using ReupVirtualTwin.models;
using ReupVirtualTwin.helpers;

public class SphereMaterialContainerHandlerTest
{
    private GameObject objectPoolPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/ScriptHolders/ObjectPool.prefab");
    private GameObject materialContainerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/MaterialContainerTest.prefab");
    private GameObject materialSpherePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/MaterialSelector/materialSphere.prefab");
    private GameObject triggerObject;
    private MaterialSelectionTrigger trigger;
    private GameObject objectPool;
    private GameObject extensionSceneTriggers;
    private IObjectPool pool;
    private Camera camera;
    private SpheresMaterialContainerHandler spheresMaterialContainerHandler;

    [SetUp]
    public void SetUp()
    {
        objectPool = (GameObject)PrefabUtility.InstantiatePrefab(objectPoolPrefab);

        extensionSceneTriggers = new GameObject();
        extensionSceneTriggers.tag = TagsEnum.extensionsTriggers;

        spheresMaterialContainerHandler = extensionSceneTriggers.AddComponent<SpheresMaterialContainerHandler>();
        spheresMaterialContainerHandler.materialsContainerPrefab = materialContainerPrefab;
        spheresMaterialContainerHandler.materialsSpherePrefab = materialSpherePrefab;

        triggerObject = new GameObject();
        triggerObject.AddComponent<MaterialSelectionTrigger>();
        trigger = triggerObject.GetComponent<MaterialSelectionTrigger>();

        pool = objectPool.GetComponent<IObjectPool>();
        pool.AddPrefabType(materialContainerPrefab);

        camera = new GameObject().AddComponent<Camera>();
        camera.tag = TagsEnum.mainCamera;
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(objectPool);
        Object.Destroy(extensionSceneTriggers);
        Object.Destroy(triggerObject);
        Object.Destroy(camera);
    }

    [UnityTest]
    public IEnumerator CreateContainerShouldSuccess()
    {

        var materials = new List<Material>()
        {
            new Material(Shader.Find("Standard")),
            new Material(Shader.Find("Standard")),
            new Material(Shader.Find("Standard")),
        };
        trigger.selectableMaterials = materials;

        // Check there is no container
        Assert.AreEqual(0, camera.transform.childCount);

        // Create cointainer
        var container = spheresMaterialContainerHandler.CreateContainer(trigger);

        // Check container is a child of camera
        Assert.AreEqual(camera.gameObject, container.transform.parent.gameObject);

        // Check the material spheres are there
        Assert.AreEqual(materials.Count, container.transform.childCount);

        // Check the materials of the spherematerials
        for (int i = 0; i < materials.Count; i++)
        {
            var sphereMaterial = container.transform.GetChild(i);
            Assert.AreEqual(materials[i], sphereMaterial.GetComponent<Renderer>().sharedMaterial);
        }

        yield return null;
    }
}
