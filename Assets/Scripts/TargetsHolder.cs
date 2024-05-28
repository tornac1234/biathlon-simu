using System.Collections.Generic;
using UnityEngine;

public class TargetsHolder : MonoBehaviour
{
    public static TargetsHolder Main;
    private Dictionary<ShootType, List<Target>> TargetsByType;

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

    public void AddMark(Vector3 positionInWall, ShootType shootType)
    {
        if (TargetsByType.TryGetValue(shootType, out List<Target> targets))
        {
            foreach (Target target in targets)
            {
                if (target.TryMark(positionInWall))
                {
                    break;
                }
            }
        }
    }

    public enum ShootType
    {
        Standing,
        Prone
    }
}
