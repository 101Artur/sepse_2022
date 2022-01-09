using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using SolarMap.Managers;
using SolarMap.SolarSystem;
using System;
using System.Collections.Generic;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRageMath;

namespace SolarMap.SessionComponents
{
	[MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
	public class SolarSystemSessionComponent : MySessionComponentBase
	{

		private readonly OreManager oreManager;
		Vector2 largestRealPositions = Vector2.Zero;
		Vector2 smallestRealPositions = Vector2.Zero;
		Vector2 realPositionsContainer = Vector2.Zero;

		public SolarSystemSessionComponent(IMyCubeGrid grid)
		{

			Grid = new Grid(grid);
			oreManager = new OreManager();
			CelestialMap = GetCelestialBodies();
			CelestialInfo = new List<CelestialBody>(CelestialMap); // Copy.

			// Setup map specfic properties. 
			foreach (CelestialBody celestialBody in CelestialMap)
			{

				// Find the farthest vector points from origo in the solar system.
				float x = Math.Abs(celestialBody.PositionXZ.X);
				float y = Math.Abs(celestialBody.PositionXZ.Z);
				largestRealPositions.X = x > largestRealPositions.X ? x : largestRealPositions.X;
				largestRealPositions.Y = y > largestRealPositions.Y ? y : largestRealPositions.Y;
				smallestRealPositions.X = x < smallestRealPositions.X ? x : smallestRealPositions.X;
				smallestRealPositions.Y = y < smallestRealPositions.Y ? y : smallestRealPositions.Y;

			}

			realPositionsContainer = new Vector2(largestRealPositions.X - smallestRealPositions.X, largestRealPositions.Y - smallestRealPositions.Y);

		}

		public List<MyPlanet> Planets { get; private set; }

		/// <summary>
		/// A list containing celestial bodies. 
		/// </summary>
		public List<CelestialBody> CelestialMap { get; private set; } = new List<CelestialBody>();

		/// <summary>
		/// Contains a copy of CelestialMap. This duplicate is used to avoid having to sort back and forth. 
		/// </summary>
		public List<CelestialBody> CelestialInfo { get; private set; } = new List<CelestialBody>();

		/// <summary>
		/// Access to the grid. 
		/// </summary>
		public Grid Grid { get; private set; }

		/// <summary>
		/// Simple check to see if the solar system is indeed ready to be read. 
		/// </summary>
		public bool IsSolarSystemReady => CelestialMap != null;

		/// <summary>
		/// Simple check to see if the solar system is indeed ready to be read. 
		/// </summary>
		public bool HasCelestialBodies => CelestialMap.Count > 0;

		/// <summary>
		/// Retrieve Vector2 equivalent in percent to be multipled to orbital position later.
		/// </summary>
		public Vector2 WorldToMapCoordinatesAbs(Vector3 position, Vector2 surfaceSize, Vector2 widescreen)
		{

			Vector2 realPositionAbs = new Vector2(Math.Abs(position.X), Math.Abs(position.Z));
			Vector2 realPositionPercent = realPositionAbs / realPositionsContainer;
			Vector2 mapPosition = surfaceSize * realPositionPercent;
			mapPosition *= 0.4f; // Desize the planet positions on the map to create a margin.
			mapPosition = surfaceSize - mapPosition; // Flip values so that Earth starts from the bottom right.
			return widescreen + mapPosition - new Vector2(surfaceSize.X / 9, surfaceSize.Y / 4);

		}

		/// <summary>
		/// Retrieve Vector2 equivalent in percent to be multipled to orbital position later.
		/// </summary>
		public Vector2 WorldToMapCoordinates(Vector3 position, Vector2 surfaceSize, Vector2 widescreen, Vector2 push)
		{

			Vector2 realPosition = new Vector2(position.X, position.Z);
			Vector2 realPositionPercent = realPosition / realPositionsContainer;
			Vector2 mapPosition = surfaceSize * realPositionPercent;
			mapPosition *= 0.4f; // Desize the planet positions on the map to create a margin.
			mapPosition = surfaceSize - mapPosition; // Flip values so that Earth starts from the bottom right.
			return mapPosition; 

		}

		/// <summary>
		/// Retrieves a list of celestial bodies converted from MyPlanet.
		/// </summary>
		private List<CelestialBody> GetCelestialBodies()
		{

			List<CelestialBody> celestialBodies = new List<CelestialBody>();
			HashSet<IMyEntity> myPlanets = new HashSet<IMyEntity>();

			// Fetch entities via MyAPIGateWay.
			MyAPIGateway.Entities.GetEntities(myPlanets, entity => entity.InScene && entity is MyPlanet);

			foreach (MyPlanet myPlanet in myPlanets)
			{

				string resources = oreManager.GetResourcesAsString(myPlanet);
				//List<string> resourceList = oreManager.GetResources(myPlanet);

				celestialBodies.Add(new CelestialBody()
				{
					Id = myPlanet.EntityId,
					Name = myPlanet.Generator.Id.SubtypeName.Contains("EarthLike") ? "Earth" : myPlanet.Generator.Id.SubtypeName,
					Type = myPlanet.MinimumRadius > 17500 ? CelestialType.Planet : CelestialType.Moon,
					Position = myPlanet.WorldMatrix.Translation,
					PositionXZ = Vector3.Abs(new Vector3(myPlanet.WorldMatrix.Translation.X, 0, myPlanet.WorldMatrix.Translation.Z)),
					Radius = myPlanet.MinimumRadius / 1000, // Kilometer
					Gravity = myPlanet.Generator.SurfaceGravity,
					Atmosphere = GetAtmosphereValue(myPlanet),
					Oxygen = GetOxygenValue(myPlanet),
					Resources = resources,
					//ResourcesList = resourceList
				});

			}

			return celestialBodies;

		}

		private string GetAtmosphereValue(MyPlanet myPlanet)
		{
			if (myPlanet.HasAtmosphere)
			{
				return myPlanet.Generator.Atmosphere.Density > 0.5 ? myPlanet.Generator.Atmosphere.Density > 1 ? "Very High " : "High" : "Low";
			}
			return "None";
		}

		private string GetOxygenValue(MyPlanet myPlanet)
		{
			if (myPlanet.HasAtmosphere && myPlanet.Generator.Atmosphere.Breathable)
			{
				return myPlanet.Generator.Atmosphere.OxygenDensity < 0.8 ? myPlanet.Generator.Atmosphere.OxygenDensity < 0.2 ? "Very Low" : "Low" : "High";
			}
			return "None";
		}

		/// <summary>
		/// Used to sort celestial bodies by distance on the X and Z axes so that sprites will stack nicely. 
		/// </summary>
		private int SortByDistance(CelestialBody a, CelestialBody b)
		{

			float distanceA = Vector2.Distance(new Vector2(a.PositionXZ.X, a.PositionXZ.Z), Vector2.Zero);
			float distanceB = Vector2.Distance(new Vector2(b.PositionXZ.X, b.PositionXZ.Z), Vector2.Zero);

			if (distanceA > distanceB)
				return -1;
			else if (distanceB > distanceA)
				return 1;
			return 0;

		}

		/// <summary>
		/// Used to sort by celestial type. 
		/// </summary>
		private int SortByType(CelestialBody a, CelestialBody b)
		{
			if (a.Type == CelestialType.Moon && b.Type == CelestialType.Planet)
				return -1;
			else if (b.Type == CelestialType.Moon && a.Type == CelestialType.Planet)
				return 1;
			return 0;
		}

		/// <summary>
		/// Dictionary containing arbitrary names that default planets can be renamed to. 
		/// </summary>
		private Dictionary<string, string> SubTypeNameToClassification = new Dictionary<string, string>()
		{
			{ "EarthLike", "Verdant" },
			{ "Moon", "Lifeless" },
			{ "Triton", "Abandoned" },
			{ "Mars", "Barren" },
			{ "Europa", "Glacial" },
			{ "Alien", "Lush" },
			{ "Titan", "Acrid" },
			{ "Pertam", "Dry" }
		};

	}
}
