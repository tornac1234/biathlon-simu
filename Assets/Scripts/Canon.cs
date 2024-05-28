using UnityEngine;

public class Canon : MonoBehaviour
{
    public Transform MuzzleTransform;
    public GameObject BulletPrefab;

    public Transform GlobalFixedTransform;
    public Transform LocalFixedTransform;
    public Transform MovingTransform;

    private const float FixedToMovingDistance = 0.359f;

    public TargetsHolder.ShootType ShootType;
    public Bullet.TrajectoryType TrajectoryType;
    public float MuzzleVelocity = 360f;
    public float Temperature = 20; // °C
    private float T => World.CelsiusToKelvin(Temperature);

    private GameObject CurrentBullet;

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            CreateBullet();
        }
    }

    public void CreateBullet()
    {
        if (CurrentBullet)
        {
            Destroy(CurrentBullet);
        }

        CurrentBullet = Instantiate(BulletPrefab, MuzzleTransform.position, Quaternion.Euler(GetOrientation()));
        Bullet bullet = CurrentBullet.GetComponent<Bullet>();
        bullet.Shoot(MuzzleVelocity, T, TrajectoryType, ShootType);
    }

    public Vector3 GetOrientation()
    {
        return GetOrientation(MovingTransform.position);
    }

    public Vector3 GetOrientation(Vector3 movingPosition)
    {
        Vector3 dir = movingPosition - LocalFixedTransform.position;
        return new Vector3(0, -Mathf.Atan(dir.z / dir.x), Mathf.Asin(dir.y / dir.magnitude) - Mathf.PI / 2) * Mathf.Rad2Deg;
    }

    public void SetOrientation(Vector3 movingPosition)
    {
        Vector3 orientation = GetOrientation(movingPosition);
        Quaternion rotation = Quaternion.Euler(orientation);

        MovingTransform.localPosition = new Vector3(0f, -FixedToMovingDistance, 0f);
        transform.rotation = rotation;
        Vector3 fixedTransformTranslation = GlobalFixedTransform.position - LocalFixedTransform.position;
        transform.position += fixedTransformTranslation;
    }

    public void OnDrawGizmos()
    {
        Vector3 fixedPos = LocalFixedTransform.position;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(fixedPos, fixedPos + Vector3.forward);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(fixedPos, fixedPos + Vector3.right);
        Gizmos.color = new Color(0, 255, 255);
        Gizmos.DrawLine(fixedPos, fixedPos + Vector3.up);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(fixedPos, fixedPos + LocalFixedTransform.up);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(fixedPos, FixedToMovingDistance);
    }
}
