using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Mass = 2.55f * Mathf.Pow(10, -3);

    private TrajectoryType Trajectory;
    public TargetsHolder.ShootType ShootType;
    private Vector3 InitialPosition;
    private Vector3 InitialVelocityFactors;

    private Vector3 PreviousPosition;
    private Vector3 PreviousSpeed;
    private Vector3 PreviousAcceleration;
    private bool Moving;
    private float t;

    private float Temperature;
    private float FluidVolumicMass;
    private float Surface;
    private float Volume;
    private float ArchimedesThrust;
    private float DragForceFactor;

    public void Start()
    {
        UI.Main.TimePassingToggle.onValueChanged.AddListener((toggled) => Moving = toggled);
        Moving = UI.Main.TimePassingToggle.isOn;
    }

    public void Shoot(float initialVelocity, float temperature, TrajectoryType trajectory, TargetsHolder.ShootType shootType)
    {
        Trajectory = trajectory;
        ShootType = shootType;
     
        InitialPosition = transform.position;

        // when normally aligned with axis X, bullet's rotation is: 0°, 0°, -90°
        Vector3 initialRotation = transform.rotation.eulerAngles + new Vector3(0, 0, 90f);
        Vector3 rotInRad = initialRotation * Mathf.Deg2Rad;
        rotInRad.y = -rotInRad.y; // Axis is not in the right orientation in Unity

        // rot.z : rotation around lateral axis
        // rot.y : rotation around vertical axis

        float initialXRotation = Mathf.Cos(rotInRad.y) * Mathf.Cos(rotInRad.z);
        float initialYRotation = Mathf.Sin(rotInRad.z);
        float initialZRotation = Mathf.Sin(rotInRad.y) * Mathf.Cos(rotInRad.z);

        InitialVelocityFactors = new Vector3(initialXRotation, initialYRotation, initialZRotation) * initialVelocity;

        UI.SetTRText($"X0, Y0, Z0 = {InitialPosition.x}, {InitialPosition.y}, {InitialPosition.z}\nVx0, Vy0, Vz0 = {InitialVelocityFactors.x}, {InitialVelocityFactors.y}, {InitialVelocityFactors.z}\nϴx0, ϴy0, ϴz0 = {rotInRad.x}, {rotInRad.y}, {rotInRad.z}");

        // Caching data to avoid too much calculation
        Temperature = temperature;
        FluidVolumicMass = World.CalculateAirVolumicMass(temperature);
        Surface = World.CalculateSurface(temperature);
        Volume = World.CalculateVolume(transform.lossyScale.z / 2, transform.lossyScale.y / 2);
        ArchimedesThrust = World.CalculateArchimedesThrust(FluidVolumicMass, Volume);
        DragForceFactor = World.CalculateDragForceFactor(FluidVolumicMass, Surface);

        // Preparing the initial values
        PreviousPosition = transform.position;
        PreviousSpeed = InitialVelocityFactors;
        PreviousAcceleration = CalculateAcceleration();
    }

    public void Update()
    {
        if (!Moving)
        {
            return;
        }

        float deltaTime = Time.deltaTime * UI.GetTimeScale();
        t += deltaTime;

        switch (Trajectory)
        {
            case TrajectoryType.FreeFall:
                FreeFallMovement();
                break;
            case TrajectoryType.Fluid:
                FluidMovement(deltaTime);
                break;
        }

        if (transform.position.x > 50)
        {
            Moving = false;
            MarkWall();
        }

        UI.SetTLText($"T = {t}\nX = {transform.position.x}\nY = {transform.position.y}\nZ = {transform.position.z}");
    }

    public void FreeFallMovement()
    {
        float x = InitialVelocityFactors.x * t + InitialPosition.x;
        float y = - (World.GRAVITY_INTENSITY / 2) * Mathf.Pow(t, 2) + InitialVelocityFactors.y * t + InitialPosition.y;
        float z = InitialVelocityFactors.z * t + InitialPosition.z;

        SetPos(x, y, z);
    }

    public void FluidMovement(float deltaTime)
    {
        PreviousAcceleration = CalculateAcceleration();
        Vector3 speed = PreviousSpeed + PreviousAcceleration * deltaTime;

        Vector3 position = PreviousPosition + speed * deltaTime;
        SetPos(position.x, position.y, position.z);
        
        PreviousSpeed = speed;
    }

    public void MarkWall()
    {
        float factor = Mathf.InverseLerp(PreviousPosition.x, transform.position.x, 50);

        Vector3 markPosition = Vector3.Lerp(PreviousPosition, transform.position, factor);

        TargetsHolder.Main.AddMark(markPosition, ShootType);
        Destroy(gameObject);
    }

    public void SetPos(float x, float y, float z)
    {
        PreviousPosition = transform.position;
        transform.position = new Vector3(x, y, z);
        // TODO: Set rotation according to velocity (tangent)
    }

    public Vector3 CalculateAcceleration()
    {
        float accelX = -DragForceFactor * PreviousSpeed.magnitude * PreviousSpeed.x / Mass;
        float accelY = (-DragForceFactor * PreviousSpeed.magnitude * PreviousSpeed.y / Mass) - World.GRAVITY_INTENSITY + ArchimedesThrust / Mass;
        float accelZ = -DragForceFactor * PreviousSpeed.magnitude * PreviousSpeed.z / Mass;

        return new Vector3(accelX, accelY, accelZ);
    }

    public enum TrajectoryType
    {
        FreeFall,
        Fluid
    }
}
