using System.Collections.Generic;
using UnityEngine;

public class TargetsHolder : MonoBehaviour
{
    public static TargetsHolder Main;
    private Dictionary<ShootType, List<Target>> TargetsByType;

    public GameObject HitMarkerPrefab;

    public void Start()
    {
        Main = this;
        
        TargetsByType = new Dictionary<ShootType, List<Target>>()
        {
            { ShootType.Standing, new List<Target>() },
            { ShootType.Prone, new List<Target>() }
        };

        foreach (Target target in GetComponentsInChildren<Target>())
        {
            TargetsByType[target.TargetType].Add(target);
        }
    }

    public List<Target> GetTargets(ShootType type)
    {
        return TargetsByType[type];
    }

    public void AddMark(Vector3 positionInWall, ShootType shootType)
    {
        bool touched = false;
        if (TargetsByType.TryGetValue(shootType, out List<Target> targets))
        {
            foreach (Target target in targets)
            {
                if (target.TryMark(positionInWall))
                {
                    touched = true;
                    break;
                }
            }
        }

        positionInWall -= new Vector3(touched ? 0.01368f : 0.0202f, 0, 0);

        GameObject hitMarker = Instantiate(HitMarkerPrefab);
        hitMarker.transform.position = positionInWall;
    }

    public enum ShootType
    {
        Standing,
        Prone
    }
}
