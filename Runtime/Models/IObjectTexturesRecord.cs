using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.models
{
    public interface IObjectTexturesRecord
    {
        public void UpdateRecords(GameObject obj);
        public void CleanRecords(GameObject obj);
    }
}
