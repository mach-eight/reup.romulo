using ReupVirtualTwin.helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.behaviours
{
    public class FollowCharacter : MonoBehaviour
    {
        GameObject _character;
        void Start()
        {
            _character = ObjectFinder.FindCharacter();
        }

        void Update()
        {
            Vector3 lookDirection = _character.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}
