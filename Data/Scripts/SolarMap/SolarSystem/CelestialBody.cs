using System.Collections.Generic;
using VRageMath;

namespace SolarMap.SolarSystem
{
	public class CelestialBody
	{

		public long Id { get; set; }

		public string Atmosphere { get; set; }
		public float Gravity { get; set; }
		public string Oxygen { get; set; }
		public float Radius { get; set; }
		public string Name { get; set; }
		public string Resources { get; set; }
		public List<string> ResourcesList { get; set; }
		public Vector3 PositionXZ { get; set; }
		public Vector3 Position { get; set; }
		public CelestialType Type { get; set; }

		public List<CelestialBody> Neighbors { get; set; } = new List<CelestialBody>();
		public List<CelestialBody> Moons { get; set; } = new List<CelestialBody>();

		// Properties used in map.
		public Vector2 MapPosition { get; set; }
		public Vector2 OrbitSize { get; set; } = Vector2.One;
		public Vector2 OrbitPosition { get; set; }
		public Vector2 PlanetSize { get; set; }
		public Vector2 LblTitlePos { get; set; }
		public Vector2 LblDistancePos { get; set; }
		public Vector2 MapOffset { get; set; }
		public Vector2 MapPositionBefore { get; set; }


	}
}
