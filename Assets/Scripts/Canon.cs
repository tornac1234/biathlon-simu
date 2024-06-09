using UnityEngine;

public class Canon : MonoBehaviour
{
    private const float FixedToMovingDistance = 0.359f;
    public Transform MuzzleTransform;
    public GameObject BulletPrefab;

    public Transform GlobalFixedTransform;
    public Transform LocalFixedTransform;
    public Transform MovingTransform;
    public Transform InitialMovingTransform;

    public float BorderWidth;
    public float BorderHeight;
    private Border MovingBorder;

    public bool Aiming;
    public TargetsHolder.ShootType ShootType;
    public Bullet.TrajectoryType TrajectoryType;
    public float MuzzleVelocity = 360f;
    public float Temperature = 20; // °C
    private float T => World.CelsiusToKelvin(Temperature);

    private GameObject CurrentBullet;

    public int Shots;

    private void Start()
    {
        MakeBorder();
        GameUI.SetSummaryVisible(false);
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            ResetCamera();
            Shots = 0;
            return;
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Aiming = false;
            return;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ResetCamera();
            Aiming = true;
            return;
        }

        if (!Aiming)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            CreateBullet();
        }

        float factor = 0.1f;
        float zMovement = -Input.GetAxis("Mouse X") * factor * Time.deltaTime;
        float yMovement = Input.GetAxis("Mouse Y") * factor * Time.deltaTime;

        Vector3 position = MovingTransform.position;
        Vector3 newPosition = position + new Vector3(0, yMovement, zMovement);

        MovingTransform.position = MovingBorder.ClampPosition(MovingTransform.position, newPosition);
        SetOrientation(MovingTransform.position);
    }

    public void CreateBullet()
    {
        Shots += 1;
        if (CurrentBullet)
        {
            Destroy(CurrentBullet);
        }

        CurrentBullet = Instantiate(BulletPrefab, MuzzleTransform.position, Quaternion.Euler(GetOrientation()));
        Bullet bullet = CurrentBullet.GetComponent<Bullet>();
        bullet.Shoot(MuzzleVelocity, T, TrajectoryType, ShootType);

        if (Shots == 5)
        {
            Aiming = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameUI.SetSummaryVisible(true);
            GameUI.UpdateSummary(ShootType);
        }
    }

    public void ResetCamera()
    {
        SetOrientation(InitialMovingTransform.position);
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

        // Draw axes
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(fixedPos, fixedPos + Vector3.forward);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(fixedPos, fixedPos + Vector3.right);
        Gizmos.color = new Color(0, 255, 255);
        Gizmos.DrawLine(fixedPos, fixedPos + Vector3.up);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(fixedPos, fixedPos + LocalFixedTransform.up);
        /*Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(fixedPos, FixedToMovingDistance);*/

        MakeBorder();
        // Draw borders
        Gizmos.color = Color.red;
        Gizmos.DrawLine(MovingBorder.BottomLeft, MovingBorder.BottomRight);
        Gizmos.DrawLine(MovingBorder.TopLeft, MovingBorder.TopRight);
        Gizmos.DrawLine(MovingBorder.BottomLeft, MovingBorder.TopLeft);
        Gizmos.DrawLine(MovingBorder.BottomRight, MovingBorder.TopRight);
    }

    public void MakeBorder()
    {
        MovingBorder = new Border(InitialMovingTransform.position, BorderWidth, BorderHeight);
    }

    public struct Border
    {
        public Vector3 BottomLeft;
        public Vector3 TopLeft;
        public Vector3 TopRight;
        public Vector3 BottomRight;

        public Border(Vector3 center, float width, float height)
        {
            BottomLeft = center + (new Vector3(0, -height, -width) / 2);
            TopLeft = center + (new Vector3(0, height, -width) / 2);
            TopRight = center + (new Vector3(0, height, width) / 2);
            BottomRight = center + (new Vector3(0, -height, width) / 2);
        }

        public Vector3 ClampPosition(Vector3 previousPosition, Vector3 newPosition)
        {
            if (newPosition.y < BottomLeft.y)
            {
                Vector3 clamped = World.Intersection(previousPosition, newPosition, 1, BottomLeft.y);
                clamped.y = BottomLeft.y;
                return ClampPosition(previousPosition, clamped);
            }
            
            if (newPosition.y > TopRight.y)
            {
                Vector3 clamped = World.Intersection(previousPosition, newPosition, 1, TopRight.y);
                clamped.y = TopRight.y;
                return ClampPosition(previousPosition, clamped);
            }
            
            if (newPosition.z < BottomLeft.z)
            {
                Vector3 clamped = World.Intersection(previousPosition, newPosition, 2, BottomLeft.z);
                clamped.z = BottomLeft.z;
                return ClampPosition(previousPosition, clamped);
            }
            
            if (newPosition.z > TopRight.z)
            {
                Vector3 clamped = World.Intersection(previousPosition, newPosition, 2, TopRight.z);
                clamped.z = TopRight.z;
                return ClampPosition(previousPosition, clamped);
            }
            
            return newPosition;
        }
    }
}
