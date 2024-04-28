using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.models;

namespace ReupVirtualTwin.models
{

    public class SpacesRecord : MonoBehaviour
    {
        public List<SpaceJumpPoint> jumpPoints;

        public void UpdateSpaces()
        {
            GameObject[] spaces = GameObject.FindGameObjectsWithTag(TagsEnum.spaceSelector);
            jumpPoints.Clear();
            foreach (GameObject room in spaces)
            {
                var roomSelector = room.GetComponent<SpaceJumpPoint>();
                jumpPoints.Add(roomSelector);
            }
        }
    }
}
