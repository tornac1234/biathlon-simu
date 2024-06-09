using UnityEngine;

public static class World
{
    public const float GRAVITY_INTENSITY = 9.81f; // N/kg
    public const float DEFAULT_TEMPERATURE = 273.15f; // K
    public static readonly float B = 1.4810f * Mathf.Pow(10, -6); // Ns K^(-0.5) m²
    public const float c = 119.4f; // K
    public const float DRAG_FACTOR = 0.42f;

    /// <summary>
    /// https://fr.wikipedia.org/wiki/Trajectoire_d%27un_projectile#Masse_volumique
    /// </summary>
    /// <param name="temperature">In Kelvins</param>
    public static float CalculateAirVolumicMass(float temperature)
    {
        return 1.292f * DEFAULT_TEMPERATURE / temperature;
    }

    /// <summary>
    /// https://fr.wikipedia.org/wiki/Trajectoire_d%27un_projectile#Viscosit%C3%A9_dynamique
    /// </summary>
    /// <param name="temperature">In Kelvins</param>
    public static float CalculateDynamicViscosity(float temperature)
    {
        return B * Mathf.Sqrt(temperature) / (1 + c / temperature);
    }

    public static float CalculateReynoldsNumber(float temperature, float airSpeed, float diameter)
    {
        return CalculateAirVolumicMass(temperature) * airSpeed * diameter / CalculateDynamicViscosity(temperature);
    }

    public static float CelsiusToKelvin(float celsius)
    {
        return celsius + DEFAULT_TEMPERATURE;
    }

    public static float KmphToMps(float kmph)
    {
        return kmph / 3.6f;
    }

    public static float CalculateDragForceFactor(float fluidVolumicMass, float surface)
    {
        return 0.5f * fluidVolumicMass * surface * DRAG_FACTOR;
    }

    /// <remarks>
    /// La poussée d'Archimède est orientée dans le sens opposé à la force de gravitation (donc vers le haut)
    /// </remarks>
    public static float CalculateArchimedesThrust(float fluidVolumicMass, float volume)
    {
        return fluidVolumicMass * volume * GRAVITY_INTENSITY;
    }

    /// <summary>
    /// Calcule la surface de référence, on considère dans le cas du projectile que c'est une section droite.
    /// </summary>
    public static float CalculateSurface(float radius)
    {
        return Mathf.PI * Mathf.Pow(radius, 2);
    }

    /// <summary>
    /// Calcule le volume du projectile, approximé à un cylindre.
    /// </summary>
    public static float CalculateVolume(float radius, float height)
    {
        return height * Mathf.PI * Mathf.Pow(radius, 2);
    }

    /// <summary>
    /// Dichotomy algorithm adapted with 3d vectors, based on the only coordinate being looked up.
    /// </summary>
    /// <param name="coordinateIndex">Coordinate index in the vector (0: x, 1: y, 2: z)</param>
    public static Vector3 Intersection(Vector3 origin, Vector3 end, int coordinateIndex, float limit, float tolerance = 1E-6f)
    {
        Vector3 cursor = origin;
        while (Mathf.Abs(cursor[coordinateIndex] - limit) > tolerance)
        {
            cursor = (origin + end) / 2;

            float cursorCoordinate = cursor[coordinateIndex] - limit;
            float originCoordinate = origin[coordinateIndex] - limit;

            if (cursorCoordinate * originCoordinate > 0)
            {
                origin = cursor;
            }
            else if (cursorCoordinate * originCoordinate < 0)
            {
                end = cursor;
            }
            else
            {
                return cursor;
            }
        }

        return cursor;
    }
}