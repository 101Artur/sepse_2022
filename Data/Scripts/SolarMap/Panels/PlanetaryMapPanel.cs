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
	public class PlanetaryMapPanel : Panel
	{

		private readonly List<MySprite> sprites = new List<MySprite>();
		private readonly List<CelestialBody> planets;
		private Vector2 screenLimit = new Vector2(512);
		private Vector2 centerOfScreen = new Vector2(512 / 2);
		CelestialBody currentPlanet;
		bool isWideScreen;
		Vector2 widescreen = Vector2.Zero;
		Vector2 push;

		/// <summary>
		/// The panel upon which the map is drawn.
		/// </summary>
		public PlanetaryMapPanel(SolarMapTSSBase solarMap) : base(solarMap)
		{

			isWideScreen = solarMap.Surface.SurfaceSize.X > 512;
			widescreen = isWideScreen ? new Vector2(256, 0) : Vector2.Zero;
			push = isWideScreen ? new Vector2(screenLimit.X / 2, 0) : new Vector2(screenLimit.X / 10, 0);
			planets = SolarSystem.CelestialMap.Where(cb => cb.Type == CelestialType.Planet).ToList();
			
			// Create map properties for planets.
			foreach (CelestialBody planet in planets)
			{

				planet.PlanetSize = GetCelestialBodySize(planet); // Resize the planet. 
				planet.MapPosition = SolarSystem.WorldToMapCoordinates(planet.PositionXZ + 1, screenLimit, widescreen, push); // Get the map coordinates.

				planet.MapOffset = planet.MapPosition - centerOfScreen; // Set how far off from the center the planet is.
				planet.MapPosition -= planet.MapOffset; // Adjust the planet to be positioned in the center of the map. 
				planet.MapPosition += push;

				planet.LblTitlePos = new Vector2(planet.MapPosition.X, planet.MapPosition.Y - 22f); // Title text position.
				planet.LblDistancePos = new Vector2(planet.MapPosition.X, planet.MapPosition.Y + 4f); // Distance text position.

				// Distance determines what is and what isn't a moon.
				int kmDistanceFromPlanet = 500;

				foreach (CelestialBody moon in SolarSystem.CelestialMap.Where(cb => cb.Type == CelestialType.Moon).ToList())
				{

					// This moon is not within 1000 km.
					if (!(Vector3.Distance(planet.PositionXZ, moon.PositionXZ) / 1000 < kmDistanceFromPlanet))
					{
						continue;
					}

					moon.PlanetSize = GetCelestialBodySize(moon); // Resize the planet. 
					moon.MapPosition = SolarSystem.WorldToMapCoordinates(moon.PositionXZ, screenLimit, widescreen, push); // Get the map coordinates.

					moon.MapOffset = (moon.MapPosition - planet.MapOffset) - (planet.MapPosition - push); // Set how far off from the planet the moon is (Very small value!).
					moon.MapOffset *= planet.PlanetSize / 6; // Increase the value of the offset, so that the moon will be farther away from the planet;
					moon.MapPosition = (planet.MapPosition - push); // Set the moon to be in the same position as the planet.
					moon.MapPosition += moon.MapOffset; // Insert the new offset value. 
					moon.MapPosition += push;
					moon.OrbitPosition = planet.MapPosition;

					moon.OrbitSize = new Vector2(Vector2.Distance(planet.MapPosition, moon.MapPosition) * 2);

					moon.LblTitlePos = new Vector2(moon.MapPosition.X, moon.MapPosition.Y - moon.PlanetSize.Y - 28);
					moon.LblDistancePos = new Vector2(moon.MapPosition.X, moon.MapPosition.Y - moon.PlanetSize.Y - 10);

					planet.Moons.Add(moon);

				}

				// Sort moons by distance from planet.
				planet.Moons.Sort((a, b) => {

					float distanceA = Vector2.Distance(a.MapPosition, planet.MapPosition);
					float distanceB = Vector2.Distance(b.MapPosition, planet.MapPosition);
					return distanceA > distanceB ? -1 : 1;

				});

			}

		}

		/// <summary>
		/// Returns a size vector that has been desized.
		/// </summary>
		private Vector2 GetCelestialBodySize(CelestialBody cb)
		{
			float screenWidth = isWideScreen ? 3 : 2;
			return (screenLimit * screenWidth) * (cb.Radius / 500);
		}

		public List<MySprite> GetAllSprites()
		{

			sprites.Clear();

			UpdateCurrentPlanet();
			AddOrbits(sprites);
			AddPlanetAndMoons(sprites);
			AddGrid(sprites);

			return sprites;

		}

		private void UpdateCurrentPlanet()
		{

			currentPlanet = planets.FirstOrDefault();
			Vector3 celestialBodyPosition = currentPlanet != null ? currentPlanet.Position : Vector3.Zero;

			foreach (CelestialBody celestialBody in planets)
			{
				if (Vector3.Distance(celestialBody.Position, SolarSystem.Grid.Position) < Vector3.Distance(celestialBodyPosition, SolarSystem.Grid.Position))
				{
					currentPlanet = celestialBody;
					celestialBodyPosition = celestialBody.Position;
				}
			}

		}

		/// <summary>
		/// Paints planetary orbits. 
		/// </summary>
		private void AddOrbits(List<MySprite> sprites)
		{

			if (currentPlanet == null)
				return;

			foreach (CelestialBody moon in currentPlanet.Moons)
			{

				// Border, then fill.
				sprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", moon.OrbitPosition, moon.OrbitSize + 3, ColorManager.Border));
				sprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", moon.OrbitPosition, moon.OrbitSize, ColorManager.Fill));

			}

		}

		/// <summary>
		/// Paints planets. 
		/// </summary>
		private void AddPlanetAndMoons(List<MySprite> sprites)
		{

			if (currentPlanet == null)
				return;

			Color fill = new Color(ColorManager.Border, 0.1f);
			float planetDistanceFromGrid = Vector3.Distance(currentPlanet.Position, SolarSystem.Grid.Position) / 1000; // km

			// Border, then fill.
			sprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", currentPlanet.MapPosition, currentPlanet.PlanetSize + 3, ColorManager.Border));
			sprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", currentPlanet.MapPosition, currentPlanet.PlanetSize, ColorManager.Fill));
			sprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", currentPlanet.MapPosition, currentPlanet.PlanetSize - 3, fill));

			// Text.
			sprites.Add(new MySprite(SpriteType.TEXT, currentPlanet.Name, currentPlanet.LblTitlePos, null, ColorManager.Text, null, rotation: 0.85f));
			sprites.Add(new MySprite(SpriteType.TEXT, planetDistanceFromGrid.ToString("F1") + " km", currentPlanet.LblDistancePos, null, ColorManager.Text, null, rotation: 0.70f));

			// Moons. 
			foreach (CelestialBody moon in currentPlanet.Moons)
			{

				float moonDistanceFromGrid = Vector3.Distance(moon.Position, SolarSystem.Grid.Position) / 1000; // km

				// Text.
				sprites.Add(new MySprite(SpriteType.TEXT, moon.Name, moon.LblTitlePos, null, ColorManager.Text, null, rotation: 0.7f));
				sprites.Add(new MySprite(SpriteType.TEXT, moonDistanceFromGrid.ToString("F1") + " km", moon.LblDistancePos, null, ColorManager.Text, null, rotation: 0.55f));

				// Border, then fill.
				sprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", moon.MapPosition, moon.PlanetSize + 3, ColorManager.Border));
				sprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", moon.MapPosition, moon.PlanetSize, ColorManager.Fill));
				sprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", moon.MapPosition, moon.PlanetSize - 3, fill));

			}

		}

		/// <summary>
		/// Paints the grid. 
		/// </summary>
		private void AddGrid(List<MySprite> sprites)
		{

			if (currentPlanet == null)
				return;

			Vector2 gridSize = new Vector2(7);
			Vector3 gridPositionXZ = Vector3.Abs(new Vector3(SolarSystem.Grid.Position.X, 0, SolarSystem.Grid.Position.Z));
			Vector2 gridMapPosition = SolarSystem.WorldToMapCoordinates(gridPositionXZ, screenLimit, widescreen, push);

			/*
				
				DO NOT DELETE: REFERENCES HOW THE MOON IS POSITIONED ON THE MAP!

				moon.MapOffset = (moon.MapPosition - planet.MapOffset) - planet.MapPosition; // Set how far off from the planet the moon is (Very small value!).
				moon.MapOffset *= planet.PlanetSize / 8; // Increase the value of the offset, so that the moon will be farther away from the planet;
				moon.MapPosition = planet.MapPosition; // Set the moon to be in the same position as the planet.
				moon.MapPosition += moon.MapOffset; // Insert the new offset value. 
			
			*/

			Vector2 gridMapOffset = (gridMapPosition - currentPlanet.MapOffset) - (currentPlanet.MapPosition - push);
			gridMapOffset *= currentPlanet.PlanetSize / 6;
			gridMapPosition = (currentPlanet.MapPosition - push);
			gridMapPosition += gridMapOffset;
			gridMapPosition += push;

			// Limit
			Vector2 max = isWideScreen ? new Vector2(screenLimit.X * 2, screenLimit.Y) : screenLimit;
			gridMapPosition = Vector2.Clamp(gridMapPosition, Vector2.Zero + (gridSize / 2), max - (gridSize / 2));

			// Text
			//sprites.Add(new MySprite(SpriteType.TEXT, SolarSystem.Grid.Main.CubeGrid.DisplayName.ToString(), new Vector2(gridMapPosition.X, gridMapPosition.Y - 25), null, ColorManager.Grid, null, rotation: 0.55f));

			if (SolarSystem.Grid != null && SolarSystem.Grid.IsMoveable)
			{
				float azimuth, elevation;
				Vector3.GetAzimuthAndElevation(SolarSystem.Grid.Main.WorldMatrix.Forward, out azimuth, out elevation);
				sprites.Add(new MySprite(SpriteType.TEXTURE, "AH_BoreSight", gridMapPosition, screenLimit * 0.05f + 3, ColorManager.Grid, null, rotation: -azimuth + MathHelper.PiOver2));
				return;
			}

			sprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", gridMapPosition, gridSize, ColorManager.Grid));

		}

	}
}