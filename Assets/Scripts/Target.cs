using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private Vector3 Center;
    private float Radius;
    private Vector3 TouchedPosition;

    public TargetsHolder.ShootType TargetType;
    public List<Vector3> ShotPositions = new List<Vector3>();
    public GameObject TouchedCover;
    public GameObject SmallTarget;
    public bool Touched;

    public void Start()
    {
        Center = transform.position;
        Center.x = 50f;
        ResetTarget();
    }

    public float TouchedDistance()
    {
        return (TouchedPosition - Center).magnitude;
    }

    public void ResetTarget()
    {
        SetTouched(false);
        bool proneTarget = TargetType == TargetsHolder.ShootType.Prone;
        SmallTarget.SetActive(proneTarget);
        Radius = (proneTarget ? SmallTarget.transform : transform).lossyScale.z / 2;
        TouchedPosition = default;
    }

    public bool TryMark(Vector3 position)
    {
        Vector3 projectedPosition = new Vector3(Center.x, position.y, position.z);
        if ((projectedPosition - Center).magnitude <= Radius)
        {
            ShotPositions.Add(projectedPosition);
            TouchedPosition = projectedPosition;
            SetTouched(true);
            return true;
        }
        return false;
    }

    public void SetTouched(bool touched)
    {
        if (!Touched && touched)
        {
            Debug.Log("TOUCHED TARGET");
            // TODO: Trigger some sort of callback for UI
        }

        Touched = touched;
        TouchedCover.SetActive(touched);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Center, Radius);
    }
}
