using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Canon))]
public class CanonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Canon canon = (Canon)target;
        EditorGUILayout.Vector3Field("Orientation", canon.GetOrientation());
        if (GUILayout.Button("Move"))
        {
            canon.SetOrientation(canon.MovingTransform.position);
        }
    }
}
