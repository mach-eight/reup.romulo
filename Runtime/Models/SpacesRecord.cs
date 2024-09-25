using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.enums;
using System;

namespace ReupVirtualTwin.models
{

    public class SpacesRecord : MonoBehaviour
    {
        public List<SpaceJumpPoint> jumpPoints;

        public void UpdateSpaces()
        {
            if (!SpaceTagIsDefined())
            {
                jumpPoints = new List<SpaceJumpPoint>() { };
                return;
            }
            GameObject[] spaces = GameObject.FindGameObjectsWithTag(TagsEnum.spaceSelector);
            jumpPoints.Clear();
            foreach (GameObject room in spaces)
            {
                SpaceJumpPoint roomSelector = room.GetComponent<SpaceJumpPoint>();
                jumpPoints.Add(roomSelector);
            }
        }
        public bool SpaceTagIsDefined()
        {
            return Array.Exists(
                   UnityEditorInternal.InternalEditorUtility.tags,
                   element => element == TagsEnum.spaceSelector
               );
        }
    }
}
