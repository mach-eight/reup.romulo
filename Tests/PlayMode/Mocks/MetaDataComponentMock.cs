using Newtonsoft.Json.Linq;
using ReupVirtualTwin.modelInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.PlayMode.Mocks
{
    class MetaDataComponentMock : MonoBehaviour, IObjectMetaDataGetterSetter
    {
        public JObject _objectMetaData;
        public JObject objectMetaData { get => _objectMetaData; set => _objectMetaData = value; }
    }
}
