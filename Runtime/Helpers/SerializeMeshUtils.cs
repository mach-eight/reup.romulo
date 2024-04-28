using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class SerializeMeshUtils
    {
        public static SerializeMesh GetSerializeMesh(GameObject obj)
        {
            var serializeMesh = obj.GetComponent<SerializeMesh>();
            if (serializeMesh == null)
            {
                serializeMesh = obj.AddComponent<SerializeMesh>();
                serializeMesh.Serialize();
            }
            return serializeMesh;
        }
        public static void SerializeTree(GameObject obj)
        {
            var meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter != null) GetSerializeMesh(obj);
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);
                SerializeTree(child.gameObject);
            }
        }
    }

}
