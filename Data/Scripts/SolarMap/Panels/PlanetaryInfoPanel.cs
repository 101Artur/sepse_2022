using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;
using VRageMath;
using SolarMap.SessionComponents;
using SolarMap.SolarSystem;
using SolarMap.Managers;
using System;
using System.Linq;

namespace SolarMap.Panels
{
	public class PlanetaryInfoPanel : Panel
	{

		private readonly List<MySprite> sprites = new List<MySprite>();
		private Vector2I panelLayout = Vector2I.One;
		private Vector2 size = new Vector2(512 - 14, 20);
		private Vector2 position = Vector2.Zero;
		private Vector2 margin = new Vector2(7, 2);
		private Vector2 surfaceSize = new Vector2(512);
		private int tick;
		private readonly List<CelestialBody> planets;
		private readonly List<CelestialBody> celestialBodies = new List<CelestialBody>();
		private CelestialBody currentPlanet;

		/// <summary>
		/// The panel upon which planet information is drawn. 
		/// </summary>
		public PlanetaryInfoPanel(SolarMapTSSBase solarMap) : base(solarMap)
		{

			surfaceSize = solarMap.Surface.SurfaceSize;
			planets = SolarSystem.CelestialMap.Where(cb => cb.Type == CelestialType.Planet).ToList();

		}

		/// <summary>
		/// Paints the information unto the panel. 
		/// </summary>
		public List<MySprite> GetAllSprites(bool hideMap = false)
		{

			sprites.Clear();
			Sort();
			UpdateCurrentPlanet();
			AddInfo(sprites, hideMap);
			return sprites;

		}

		private void Sort()
		{

			// Sort planets roughly every ~5 seconds. 
			if (tick % 27 == 0)
				planets.Sort(SortByDistanceFromGrid);
			tick++;

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
		private void AddInfo(List<MySprite> sprites, bool hideMap)
		{

			if (currentPlanet == null)
				return;

			celestialBodies.Clear();
			celestialBodies.AddRange(currentPlanet.Moons);
			celestialBodies.Add(currentPlanet);
			celestialBodies.Sort(SortByDistanceFromGrid);

			Vector2 mainPanelSize = new Vector2(surfaceSize.Y / 3, surfaceSize.Y);
			Vector2 mainPanelPosition = mainPanelSize / 2;
			Vector2 margin = new Vector2(6);
			Vector2 textMargin = new Vector2(margin.X * 2, margin.Y);
			Vector2 infoPanelSize = new Vector2(mainPanelSize.X - margin.X, (mainPanelSize.Y - margin.Y) / 5);
			Vector2 titlePanelSize = new Vector2(infoPanelSize.X - (margin.X / 2), infoPanelSize.Y / 5);

			// Shrink the mainPanelSize for the Planetery Map
			mainPanelSize = new Vector2(mainPanelSize.X, (mainPanelSize.Y / 5) * celestialBodies.Count);
			mainPanelPosition = mainPanelSize / 2;

			// Background: Panel.
			sprites.Add(new MySprite { 
				Type = SpriteType.TEXTURE, 
				Data = "SquareSimple", 
				Position = mainPanelPosition, 
				Size = mainPanelSize, 
				Color = ColorManager.PanelBackground, 
				FontId = null, 
				Alignment = TextAlignment.CENTER, 
				RotationOrScale = 0 
			});

			// Panel content.
			for (int i = 0; i < celestialBodies.Count; i++)
			{

				CelestialBody celestialBody = celestialBodies[i];
				float distance = Vector3.Distance(celestialBody.Position, SolarSystem.Grid.Position) / 1000;

				// Background: Title.
				sprites.Add(new MySprite {
					Type = SpriteType.TEXTURE,
					Data = "SquareSimple",
					Position = (new Vector2(margin.X * 2, margin.Y) / 2) + new Vector2(titlePanelSize.X / 2, (titlePanelSize.Y / 2) + (infoPanelSize.Y * i)),
					Size = titlePanelSize,
					Color = ColorManager.TitleBackground,
					FontId = null,
					Alignment = TextAlignment.CENTER,
					RotationOrScale = 0
				});

				// Text: Title.
				sprites.Add(new MySprite{
					Type = SpriteType.TEXT, 
					Data = celestialBody.Name + " (" + distance.ToString("F1") + " km" + ")", 
					Position = textMargin + new Vector2(0, infoPanelSize.Y * i), 
					Size = null, 
					Color = ColorManager.Text, 
					FontId = null, 
					Alignment = TextAlignment.LEFT, 
					RotationOrScale = 0.5f
				});

				// Text: Body.
				sprites.Add(new MySprite { 
					Type = SpriteType.TEXT, 
					Data = "Radius: " + celestialBody.Radius.ToString("F2") + " km\n" +
							"Gravity: " + celestialBody.Gravity.ToString("F2") + " G\n" +
							"Atmosphere: " + celestialBody.Atmosphere + "\n" +
							"Oxygen: " + celestialBody.Oxygen + "\n" +
							"Resources: " + celestialBody.Resources + "\n", 
					Position = textMargin + new Vector2(0, 20 + (infoPanelSize.Y * i)), 
					Size = null, 
					Color = ColorManager.Text, 
					FontId = null,
					Alignment = TextAlignment.LEFT,
					RotationOrScale = 0.4f 
				});

			}

		}

		/// <summary>
		/// Returns the amount of columns and rows that the information panel will have. 
		/// </summary>
		private Vector2I GetPanelLayout(int num)
		{

			int x, y;

			// X
			if (num < 3)
				x = 1;
			else if (num > 8)
				x = 3;
			else
				x = 2;

			// Y
			if (num < 9)
				y = (int)Math.Round((0.42f * num) + 0.75f);
			else if (num > 9)
				y = (int)Math.Round((0.3f * num) + 0.8f);
			else
				y = 3;

			return new Vector2I(x, y);

		}

		/// <summary>
		/// Sorts by distance. 
		/// </summary>
		private int SortByDistanceFromGrid(CelestialBody a, CelestialBody b)
		{

			float distanceA = Vector3.Distance(a.PositionXZ, SolarSystem.Grid.Position);
			float distanceB = Vector3.Distance(b.PositionXZ, SolarSystem.Grid.Position);

			if (distanceA < distanceB)
				return -1;
			else if (distanceB < distanceA)
				return 1;
			return 0;

		}

	}
}