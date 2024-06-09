using UnityEditor;

[CustomEditor(typeof(Target))]
public class TargetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Target targetComponent = (Target)target;
        EditorGUILayout.Vector3Field("Global position", targetComponent.transform.position);
        EditorGUILayout.Vector3Field("Global scale", targetComponent.transform.lossyScale);

        targetComponent.SetTouched(targetComponent.Touched);
        targetComponent.SmallTarget.SetActive(targetComponent.TargetType == TargetsHolder.ShootType.Prone);
    }
}
