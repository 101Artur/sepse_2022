using Sandbox.ModAPI;
using SolarMap.SessionComponents;
using SolarMap.SolarSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace SolarMap.Panels
{
	public class MapPanel : Panel
	{

		private const float OPACITY = 0.1f;
		private readonly List<MySprite> sprites = new List<MySprite>();
		private readonly List<CelestialBody> celestialBodies;
		private readonly List<CelestialBody> celestialOrbits;
		readonly bool isWideScreen;
		private Vector2 screenLimit = new Vector2(512);
		private Vector2 wideScreen = Vector2.Zero;

		/// <summary>
		/// The panel upon which the map is drawn.
		/// </summary>
		public MapPanel(SolarMapTSSBase solarMap) : base(solarMap)
		{

			isWideScreen = solarMap.Surface.SurfaceSize.X > 512;
			wideScreen = isWideScreen ? new Vector2(256, 0) : Vector2.Zero;
			celestialBodies = SolarSystem.CelestialMap.Where(cb => cb.Type == CelestialType.Planet).ToList();

			// Create map properties for celestial bodies.
			foreach (CelestialBody celestialBody in celestialBodies)
			{

				celestialBody.MapPosition = SolarSystem.WorldToMapCoordinatesAbs(celestialBody.PositionXZ, screenLimit, wideScreen);
				celestialBody.OrbitSize = new Vector2(Vector2.Distance(celestialBody.MapPosition, screenLimit)) * 2;
				celestialBody.OrbitPosition = isWideScreen ? screenLimit + wideScreen : screenLimit;
				celestialBody.PlanetSize = screenLimit * celestialBody.Radius * 0.001f - 0.01f;
				celestialBody.LblTitlePos = new Vector2(celestialBody.MapPosition.X, celestialBody.MapPosition.Y - screenLimit.Y * 0.1f);
				celestialBody.LblDistancePos = new Vector2(celestialBody.MapPosition.X, celestialBody.MapPosition.Y - screenLimit.Y * 0.065f);

				// Check to see if there are any other planets that are too close to this planet.
				foreach (CelestialBody potentialNeighbor in celestialBodies)
				{

					if (potentialNeighbor == celestialBody)
						continue;

					// This planet is within 1000 km.
					if (Vector3.Distance(celestialBody.PositionXZ, potentialNeighbor.PositionXZ) / 1000 < 1000)
						celestialBody.Neighbors.Add(potentialNeighbor);

				}

			}

			celestialOrbits = new List<CelestialBody>(celestialBodies);

		}

		public void SortCelestialLists()
		{

			celestialBodies.Sort(SortByDistanceFromScreen);
			celestialBodies.Sort(SortByDistanceFromGrid);
			celestialOrbits.Sort(SortByDistanceFromGrid);
			celestialOrbits.Sort(SortByDistanceFromScreenReverse);

		}

		private int SortByDistanceFromGrid(CelestialBody a, CelestialBody b)
		{

			// Doesn't have any neighbors.
			if (a.Neighbors.Count == 0)
				return 0;

			if (a.Neighbors.Contains(b))
			{

				float distanceA = Vector3.Distance(a.Position, SolarSystem.Grid.Position);
				float distanceB = Vector3.Distance(b.Position, SolarSystem.Grid.Position);

				return distanceA > distanceB ? -1 : 1;

			}

			return 0;

		}

		/// <summary>
		/// Used to sort celestial bodies by distance on the X and Z axes so that sprites will stack nicely. 
		/// </summary>
		private int SortByDistanceFromScreen(CelestialBody a, CelestialBody b)
		{

			float distanceA = Vector2.Distance(a.MapPosition, a.OrbitPosition);
			float distanceB = Vector2.Distance(b.MapPosition, b.OrbitPosition);
			return distanceA < distanceB ? -1 : 1;

		}

		private int SortByDistanceFromScreenReverse(CelestialBody a, CelestialBody b)
		{

			float distanceA = Vector2.Distance(a.MapPosition, a.OrbitPosition);
			float distanceB = Vector2.Distance(b.MapPosition, b.OrbitPosition);
			return distanceA > distanceB ? -1 : 1;

		}

		public List<MySprite> GetAllSprites()
		{

			sprites.Clear();

			AddOrbits(sprites);
			AddPlanets(sprites);
			AddGrid(sprites);

			return sprites;

		}

		/// <summary>
		/// Paints planetary orbits. 
		/// </summary>
		private void AddOrbits(List<MySprite> sprites)
		{

			foreach (CelestialBody planet in celestialOrbits)
			{

				Vector2 orbitSize = new Vector2(Vector2.Distance(planet.MapPosition, screenLimit + wideScreen)) * 2;

				Color colorBorder = ColorManager.Border;
				float opacity = 0.1f;

				// Check to see if the grid is closer to this planet or one of its neighbors.
				bool isGridCloserToNeighbor = false;
				foreach (CelestialBody neighbor in planet.Neighbors)
				{
					if (Vector3.Distance(planet.Position, SolarSystem.Grid.Position) > Vector3.Distance(neighbor.Position, SolarSystem.Grid.Position))
						isGridCloserToNeighbor = true;
				}

				// Grid is closer to one of its neighbors. 
				if (isGridCloserToNeighbor)
					colorBorder = ColorExtensions.Alpha(ColorManager.Border, opacity);

				// Border, then fill.
				sprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", screenLimit + wideScreen, orbitSize + 3, colorBorder));
				sprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", screenLimit + wideScreen, orbitSize, ColorManager.Fill));

			}

		}

		/// <summary>
		/// Paints planets. 
		/// </summary>
		private void AddPlanets(List<MySprite> sprites)
		{

			foreach (CelestialBody planet in celestialBodies)
			{

				float planetDistanceFromGrid = Vector3.Distance(planet.Position, SolarSystem.Grid.Position) / 1000; // km
				Color colorText = ColorManager.Text;
				Color colorBorder = ColorManager.Border;

				// Check to see if the grid is closer to this planet or one of its neighbors. 
				bool isGridCloserToNeighbor = IsGridCloserToNeighbor(planet, planetDistanceFromGrid);

				// Grid is closer to one of its neighbors. 
				if (isGridCloserToNeighbor)
				{
					colorText = ColorExtensions.Alpha(ColorManager.Text, OPACITY);
					colorBorder = ColorExtensions.Alpha(ColorManager.Border, OPACITY);
				}

				// Text.
				sprites.Add(new MySprite(SpriteType.TEXT, planet.Name, planet.LblTitlePos, null, colorText, null, rotation: 0.7f));
				sprites.Add(new MySprite(SpriteType.TEXT, planetDistanceFromGrid.ToString("F1") + " km", planet.LblDistancePos, null, colorText, null, rotation: 0.55f));

				// Border, then fill.
				sprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", planet.MapPosition, planet.PlanetSize + 3, colorBorder));
				sprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", planet.MapPosition, planet.PlanetSize, ColorManager.Fill));


			}

		}

		/// <summary>
		/// Paints the grid. 
		/// </summary>
		private void AddGrid(List<MySprite> sprites)
		{

			Vector2 gridSize = new Vector2(7);
			Vector3 gridPositionXZ = Vector3.Abs(new Vector3(SolarSystem.Grid.Position.X, 0, SolarSystem.Grid.Position.Z));
			Vector2 gridMapPosition = SolarSystem.WorldToMapCoordinatesAbs(gridPositionXZ, screenLimit, wideScreen);

			/*
			//return new MySprite(SpriteType.TEXT, "", new Vector2(250, 250), null, ColorManager.Text, null, rotation: 0.7f);
			if (SolarSystem.Grid != null && SolarSystem.Grid.IsMoveable)
			{
				float azimuth, elevation;
				Vector3.GetAzimuthAndElevation(SolarSystem.Grid.Main.WorldMatrix.Forward, out azimuth, out elevation);
				sprites.Add(new MySprite(SpriteType.TEXTURE, "AH_BoreSight", gridPosition, mapSizeLimit * 0.05f + 3, ColorManager.Grid, null, rotation: -azimuth + MathHelper.PiOver2)); //   MathHelper.PiOver2
				return;
			}
			*/

			// Limit
			Vector2 max = isWideScreen ? new Vector2(screenLimit.X * 2, screenLimit.Y) : screenLimit;
			gridMapPosition = Vector2.Clamp(gridMapPosition, Vector2.Zero + (gridSize / 2), max - (gridSize / 2));

			sprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", gridMapPosition, gridSize, ColorManager.Grid));

		}

		private bool IsGridCloserToNeighbor(CelestialBody planet, float planetDistanceFromGrid)
		{
			foreach (CelestialBody neighbor in planet.Neighbors)
			{
				if (planetDistanceFromGrid > Vector3.Distance(neighbor.Position, SolarSystem.Grid.Position) / 1000)
					return true;
			}
			return false;
		}

	}
}