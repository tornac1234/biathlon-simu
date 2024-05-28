using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Target))]
public class TargetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Target targetComponent = (Target)target;
        EditorGUILayout.Vector3Field("Global position", targetComponent.transform.position);
        EditorGUILayout.Vector3Field("Global scale", targetComponent.transform.lossyScale);

        bool touched = GUILayout.Toggle(targetComponent.transform.localRotation.z < 0, "Touched");
        targetComponent.SetTouched(touched);
    }
}
