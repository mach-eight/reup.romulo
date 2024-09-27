using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.enums;
using System;
using ReupVirtualTwin.modelInterfaces;

namespace ReupVirtualTwin.models
{

    public class SpacesRecord : MonoBehaviour, ISpacesRecord
    {

        List<ISpaceJumpPoint> _jumpPoints;
        public List<ISpaceJumpPoint> jumpPoints
        {
            get
            {
                if (_jumpPoints == null)
                {
                    UpdateSpaces();
                }
                return _jumpPoints;
            }
            set => _jumpPoints = value;
        }

        public void UpdateSpaces()
        {
            if (_jumpPoints == null)
            {
                _jumpPoints = new List<ISpaceJumpPoint>() { };
            }
            if (!SpaceTagIsDefined())
            {
                return;
            }
            GameObject[] spaces = GameObject.FindGameObjectsWithTag(TagsEnum.spaceSelector);
            _jumpPoints.Clear();
            foreach (GameObject room in spaces)
            {
                SpaceJumpPoint roomSelector = room.GetComponent<SpaceJumpPoint>();
                _jumpPoints.Add(roomSelector);
            }
        }

        public bool SpaceTagIsDefined()
        {
            return Array.Exists(
                UnityEditorInternal.InternalEditorUtility.tags,
                element => element == TagsEnum.spaceSelector
            );
        }

        public void GoToSpace(string SpaceJumpPointId)
        {
            throw new NotImplementedException();
        }
    }
}
