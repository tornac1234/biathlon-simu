using System.IO;
using System.Text;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class WorldTest
    {
        [Test]
        public void GenerateSheet()
        {
            float airSpeed = World.KmphToMps(20);
            float diameter = 0.09f;
            
            StringBuilder csv = new StringBuilder("Temperature;Dynamic viscosity;Air volumetric mass;Reynolds\n");
            for (float temperature = -40; temperature <= 30; temperature += 10)
            {
                float T = World.CelsiusToKelvin(temperature);
                csv.AppendLine($"{temperature};{World.CalculateDynamicViscosity(T)};{World.CalculateAirVolumicMass(T)};{World.CalculateReynoldsNumber(T, airSpeed, diameter)}");
            }

            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "export.csv"), csv.ToString());
            Assert.IsTrue(true);
        }

        [Test]
        public void TestIntersection()
        {
            Vector3 origin = new Vector3(0, 0, 0);
            Vector3 end = new Vector3(53, 15, 5);
            Vector3 intersection = World.Intersection(origin, end, 0, 50);
            Assert.IsTrue(intersection == end, intersection.ToString());
        }
    }
}
