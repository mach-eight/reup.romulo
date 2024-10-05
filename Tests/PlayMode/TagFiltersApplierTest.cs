using UnityEngine;
using NUnit.Framework;

using ReupVirtualTwin.controllers;
using ReupVirtualTwin.controllerInterfaces;
using System.Collections.Generic;
using ReupVirtualTwinTests.utils;

namespace ReupVirtualTwinTests.controllers
{
    public class TagFiltersApplierTest : MonoBehaviour
    {
        GameObject building;

        [TearDown]
        public void TearDown()
        {
            Destroy(building);
        }
        [Test]
        public void ShouldGetParent()
        {
            building = StubObjectTreeCreator.CreateMockBuilding();
            ITagFilter filterForParent = new TagFilter(StubObjectTreeCreator.parentTags[0]);
            List<ITagFilter> filterList = new List<ITagFilter>() { filterForParent };
            List<GameObject> filteredObjects = TagFiltersApplier.ApplyExclusiveFiltersToTree(building, filterList);
            Assert.AreEqual(1, filteredObjects.Count);
            Assert.AreEqual(building, filteredObjects[0]);
        }
        [Test]
        public void ShouldGetEmptyGameObjectList()
        {
            building = StubObjectTreeCreator.CreateMockBuilding();
            ITagFilter filterparent = new TagFilter(StubObjectTreeCreator.parentTags[0]);
            ITagFilter filterParentInverse = new TagFilter(StubObjectTreeCreator.parentTags[0]);
            filterParentInverse.invertFilter = true;
            List<ITagFilter> filterList = new List<ITagFilter>() { filterparent, filterParentInverse };
            List<GameObject> filteredObjects = TagFiltersApplier.ApplyExclusiveFiltersToTree(building, filterList);
            Assert.AreEqual(0, filteredObjects.Count);
        }

        [Test]
        public void ShouldGetChildrenObjects()
        {
            building = StubObjectTreeCreator.CreateMockBuilding();
            List<ITagFilter> filterList = new List<ITagFilter>() { new TagFilter(StubObjectTreeCreator.commonChildrenTag) };
            List<GameObject> filteredObjects = TagFiltersApplier.ApplyExclusiveFiltersToTree(building, filterList);
            Assert.AreEqual(2, filteredObjects.Count);
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(0).gameObject));
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(1).gameObject));
        }

        [Test]
        public void ShouldReturn_ObjectsWithTagX()
        {
            building = StubObjectTreeWithTagAtDifferentLevelsCreator.CreateMockObjectWithArbitraryTagAtSecondAndThirdLevel();
            ITagFilter filter = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagX);
            List<ITagFilter> filterList = new List<ITagFilter>() { filter };
            List<GameObject> filteredObjects = TagFiltersApplier.ApplyExclusiveFiltersToTree(building, filterList);
            Assert.AreEqual(2, filteredObjects.Count);
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(0).GetChild(0).gameObject));
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(1).gameObject));
        }

        [Test]
        public void ShouldReturn_ObjectsWithoutTagX_and_WithoutChildrenWithTagX()
        {
            building = StubObjectTreeWithTagAtDifferentLevelsCreator.CreateMockObjectWithArbitraryTagAtSecondAndThirdLevel();
            ITagFilter filter = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagX);
            filter.invertFilter = true;
            List<ITagFilter> filterList = new List<ITagFilter>() { filter };
            List<GameObject> filteredObjects = TagFiltersApplier.ApplyExclusiveFiltersToTree(building, filterList);
            Assert.AreEqual(2, filteredObjects.Count);
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(0).GetChild(1).gameObject));
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(2).gameObject));
        }

        [Test]
        public void ShouldReturn_ObjectsWithTagXAndY()
        {
            building = StubObjectTreeWithTagAtDifferentLevelsCreator.CreateMockObjectWithArbitraryTagAtSecondAndThirdLevel();
            ITagFilter filterX = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagX);
            ITagFilter filterY = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagY);
            List<ITagFilter> filterList = new List<ITagFilter>() { filterX, filterY };
            List<GameObject> filteredObjects = TagFiltersApplier.ApplyExclusiveFiltersToTree(building, filterList);
            Assert.AreEqual(1, filteredObjects.Count);
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(1).gameObject));
        }

        [Test]
        public void ShouldReturn_ObjectsWithTagXAndNotY()
        {
            building = StubObjectTreeWithTagAtDifferentLevelsCreator.CreateMockObjectWithArbitraryTagAtSecondAndThirdLevel();
            ITagFilter filterX = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagX);
            ITagFilter filterY = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagY);
            filterY.invertFilter = true;
            List<ITagFilter> filterList = new List<ITagFilter>() { filterX, filterY };
            List<GameObject> filteredObjects = TagFiltersApplier.ApplyExclusiveFiltersToTree(building, filterList);
            Assert.AreEqual(1, filteredObjects.Count);
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(0).GetChild(0).gameObject));
        }

        [Test]
        public void ShouldReturn_ObjectsWithoutTagXAndWithTagY()
        {
            building = StubObjectTreeWithTagAtDifferentLevelsCreator.CreateMockObjectWithArbitraryTagAtSecondAndThirdLevel();
            ITagFilter filterX = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagX);
            filterX.invertFilter = true;
            ITagFilter filterY = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagY);
            List<ITagFilter> filterList = new List<ITagFilter>() { filterX, filterY };
            List<GameObject> filteredObjects = TagFiltersApplier.ApplyExclusiveFiltersToTree(building, filterList);
            Assert.AreEqual(1, filteredObjects.Count);
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(0).GetChild(1).gameObject));
        }

        [Test]
        public void ShouldReturn_ObjectsWithoutTagXNorY()
        {
            building = StubObjectTreeWithTagAtDifferentLevelsCreator.CreateMockObjectWithArbitraryTagAtSecondAndThirdLevel();
            ITagFilter filterX = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagX);
            filterX.invertFilter = true;
            ITagFilter filterY = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagY);
            filterY.invertFilter = true;
            List<ITagFilter> filterList = new List<ITagFilter>() { filterX, filterY };
            List<GameObject> filteredObjects = TagFiltersApplier.ApplyExclusiveFiltersToTree(building, filterList);
            Assert.AreEqual(1, filteredObjects.Count);
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(2).gameObject));
        }

        [Test]
        public void ShouldReturn_ObjectsWithTagZ()
        {
            building = StubObjectTreeWithTagAtDifferentLevelsCreator.CreateMockObjectWithArbitraryTagAtSecondAndThirdLevel();
            ITagFilter filter = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagZ);
            List<ITagFilter> filterList = new List<ITagFilter>() { filter };
            List<GameObject> filteredObjects = TagFiltersApplier.ApplyExclusiveFiltersToTree(building, filterList);
            Assert.AreEqual(1, filteredObjects.Count);
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(0).gameObject));
        }

        [Test]
        public void ShouldReturn_ObjectsWithoutTagZ()
        {
            building = StubObjectTreeWithTagAtDifferentLevelsCreator.CreateMockObjectWithArbitraryTagAtSecondAndThirdLevel();
            ITagFilter filter = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagZ);
            filter.invertFilter = true;
            List<ITagFilter> filterList = new List<ITagFilter>() { filter };
            List<GameObject> filteredObjects = TagFiltersApplier.ApplyExclusiveFiltersToTree(building, filterList);
            Assert.AreEqual(2, filteredObjects.Count);
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(1).gameObject));
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(2).gameObject));
        }

        [Test]
        public void ShouldReturn_ObjectsWithSubString_Tag_X()
        {
            building = StubObjectTreeWithTagAtDifferentLevelsCreator.CreateMockObjectWithArbitraryTagAtSecondAndThirdLevel();
            ITagFilter filterX = new SubstringTagFilter("ag X");
            List<ITagFilter> filterList = new List<ITagFilter>() { filterX };
            List<GameObject> filteredObjects = TagFiltersApplier.ApplyExclusiveFiltersToTree(building, filterList);
            Assert.AreEqual(2, filteredObjects.Count);
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(0).GetChild(0).gameObject));
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(1).gameObject));
        }

        [Test]
        public void ShouldReturn_ObjectsWithout_Substring_Tag_X_AndWithTagY()
        {
            building = StubObjectTreeWithTagAtDifferentLevelsCreator.CreateMockObjectWithArbitraryTagAtSecondAndThirdLevel();
            ITagFilter filterX = new SubstringTagFilter("ag X");
            filterX.invertFilter = true;
            ITagFilter filterY = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagY);
            List<ITagFilter> filterList = new List<ITagFilter>() { filterX, filterY };
            List<GameObject> filteredObjects = TagFiltersApplier.ApplyExclusiveFiltersToTree(building, filterList);
            Assert.AreEqual(1, filteredObjects.Count);
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(0).GetChild(1).gameObject));
        }

        [Test]
        public void ShouldReturnObjects_WithTagZ_And_WithoutNotPresentTag()
        {
            building = StubObjectTreeWithTagAtDifferentLevelsCreator.CreateMockObjectWithArbitraryTagAtSecondAndThirdLevel();
            ITagFilter filterZ = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagZ);
            ITagFilter filterNotPresentTag = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.notPesentTag);
            filterNotPresentTag.invertFilter = true;
            List<ITagFilter> filterList = new List<ITagFilter>() { filterZ, filterNotPresentTag };
            List<GameObject> filteredObjects = TagFiltersApplier.ApplyExclusiveFiltersToTree(building, filterList);
            Assert.AreEqual(1, filteredObjects.Count);
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(0).gameObject));
        }

        [Test]
        public void ShouldNotApplyDeactivatedFilters()
        {
            building = StubObjectTreeWithTagAtDifferentLevelsCreator.CreateMockObjectWithArbitraryTagAtSecondAndThirdLevel();
            ITagFilter filterX = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagX);
            ITagFilter filterY = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagY);
            filterY.filterIsActive = false;
            List<ITagFilter> filterList = new List<ITagFilter>() { filterX, filterY };
            List<GameObject> filteredObjects = TagFiltersApplier.ApplyExclusiveFiltersToTree(building, filterList);
            Assert.AreEqual(2, filteredObjects.Count);
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(0).GetChild(0).gameObject));
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(1).gameObject));
        }

        [Test]
        public void ShouldApplyInclusiveFiltersForTagXOrTagY()
        {
            building = StubObjectTreeWithTagAtDifferentLevelsCreator.CreateMockObjectWithArbitraryTagAtSecondAndThirdLevel();
            ITagFilter filterX = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagX);
            ITagFilter filterY = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagY);
            List<ITagFilter> filterList = new List<ITagFilter>() { filterX, filterY };
            List<GameObject> filteredObjects = TagFiltersApplier.ApplyInclusiveFiltersToTree(building, filterList);
            Assert.AreEqual(3, filteredObjects.Count);
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(0).GetChild(0).gameObject));
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(0).GetChild(1).gameObject));
            Assert.IsTrue(filteredObjects.Contains(building.transform.GetChild(1).gameObject));
        }

        [Test]
        public void IfNoActiveFiltersShouldReturnEmptyList()
        {
            building = StubObjectTreeWithTagAtDifferentLevelsCreator.CreateMockObjectWithArbitraryTagAtSecondAndThirdLevel();
            ITagFilter filterX = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagX);
            ITagFilter filterY = new TagFilter(StubObjectTreeWithTagAtDifferentLevelsCreator.tagY);
            filterX.filterIsActive = false;
            filterY.filterIsActive = false;
            List<ITagFilter> filterList = new List<ITagFilter>() { filterX, filterY };
            List<GameObject> InclusiveFilteredObjects = TagFiltersApplier.ApplyInclusiveFiltersToTree(building, filterList);
            Assert.AreEqual(0, InclusiveFilteredObjects.Count);
            List<GameObject> ExclusiveFilteredObjects = TagFiltersApplier.ApplyExclusiveFiltersToTree(building, filterList);
            Assert.AreEqual(0, ExclusiveFilteredObjects.Count);
        }

    }
}
