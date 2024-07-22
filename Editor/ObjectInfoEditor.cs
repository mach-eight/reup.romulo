using UnityEditor;
using ReupVirtualTwin.models;

[CustomEditor(typeof(ObjectInfo))]
public class ObjectInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ObjectInfo identifier = (ObjectInfo)target;
        if (string.IsNullOrEmpty(identifier.manualId))
        {
            identifier.manualId = identifier.uniqueId;
        }
    }
}
