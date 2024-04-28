using UnityEditor;
using ReupVirtualTwin.models;

[CustomEditor(typeof(RegisteredIdentifier))]
public class RegisteredIdentifierEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RegisteredIdentifier identifier = (RegisteredIdentifier)target;
        if (string.IsNullOrEmpty(identifier.manualId))
        {
            identifier.manualId = identifier.uniqueId;
        }
    }
}
