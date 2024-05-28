using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public TargetsHolder.ShootType TargetType;
    public Vector3 Center;
    public float Radius;
    public List<Vector3> ShotPositions = new List<Vector3>();

    public void Start()
    {
        Center = transform.position;
        Center.x = 50f;
        Radius = transform.localScale.z;
        SetTouched(false);
    }

    public bool TryMark(Vector3 position)
    {
        Vector3 projectedPosition = new Vector3(Center.x, position.y, position.z);
        if ((projectedPosition - Center).magnitude <= Radius)
        {
            // TODO: Set target color
            SetTouched(true);
            ShotPositions.Add(projectedPosition);
            return true;
        }
        return false;
    }

    public void SetTouched(bool touched)
    {
        transform.localRotation = Quaternion.Euler(0f, 0f, touched ? -90f : 90f);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Center, Radius);
    }
}
