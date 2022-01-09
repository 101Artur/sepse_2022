using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;
using VRageMath;
using SolarMap.SessionComponents;
using SolarMap.SolarSystem;
using SolarMap.Managers;
using System;

namespace SolarMap.Panels
{
	public class InfoPanel : Panel
	{

		private readonly List<MySprite> sprites = new List<MySprite>();
		private Vector2I panelLayout = Vector2I.One;
		private Vector2 size = new Vector2(512 - 6, 20);
		private Vector2 position = Vector2.Zero;
		private Vector2 margin = new Vector2(3, 1);
		private int tick;

		/// <summary>
		/// The panel upon which planet information is drawn. 
		/// </summary>
		public InfoPanel(SolarMapTSSBase solarMap) : base(solarMap) { }

		/// <summary>
		/// Paints the information unto the panel. 
		/// </summary>
		public List<MySprite> GetAllSprites(bool hideMap = false)
		{

			// Create a panel layout and sort them panel roughly every ~5 seconds. 
			if (tick % 27 == 0)
			{
				SolarSystem.CelestialInfo.Sort(SortByDistanceFromGrid);
			}
			tick++;

			sprites.Clear();

			// Panel background.
			if (!hideMap)
			{
				sprites.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(95, 256), new Vector2(183, 505), ColorManager.PanelBackground));
			}

			int cbCount = SolarSystem.CelestialInfo.Count > 4 ? 5 : SolarSystem.CelestialInfo.Count;
			panelLayout = hideMap ? GetPanelLayout(SolarSystem.CelestialInfo.Count) : new Vector2I(1, cbCount);

			// Position multiplier.
			int xPosMult = 0;
			int yPosMult = -1;
			float ySizeMult = 1.75f - (panelLayout.Y + 1) * (0.125f + (panelLayout.X - 1) * 0.01f);

			// Panel content.
			for (int i = 0; i < SolarSystem.CelestialInfo.Count; i++)
			{

				if (panelLayout.Y - 1 == yPosMult)
				{
					yPosMult = 0;
					xPosMult++;
				}
				else
				{
					yPosMult++;
				}

				CelestialBody celestialBody = SolarSystem.CelestialInfo[i];

				// General.
				float marginX = hideMap ? margin.X : 0;
				float sizeX = hideMap ? size.X / panelLayout.X : (size.X / 3) + margin.X + 1;
				position.X = margin.X + (sizeX * xPosMult);
				position.Y = margin.X + (size.X / panelLayout.Y * yPosMult);

				// Title: Background.
				Vector2 titleBgPosition = new Vector2(position.X + (sizeX / 2), position.Y + (size.Y * ySizeMult / 2));
				Vector2 titleBgSize = new Vector2(sizeX - marginX, size.Y * ySizeMult);
				sprites.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", titleBgPosition, titleBgSize, ColorManager.TitleBackground));

				// Title: Text.
				Vector2 titleTxtPosition = new Vector2(margin.X + position.X, margin.Y * ySizeMult + position.Y);
				float distance = Vector3.Distance(celestialBody.Position, SolarSystem.Grid.Position) / 1000;
				sprites.Add(new MySprite(SpriteType.TEXT, celestialBody.Name + " (" + distance.ToString("F1") + " km" + ")", titleTxtPosition, null, ColorManager.Text, null, TextAlignment.LEFT, 0.65f * ySizeMult));


				// Body: Text.
				Vector2 bodyTxtPosition = new Vector2(margin.X + position.X, size.Y * ySizeMult + position.Y);
				string txt = "Radius: " + celestialBody.Radius.ToString("F2") + " km\n" +
							"Gravity: " + celestialBody.Gravity.ToString("F2") + " G\n" +
							"Atmosphere: " + celestialBody.Atmosphere + "\n" +
							"Oxygen: " + celestialBody.Oxygen + "\n" +
							"Resources: " + celestialBody.Resources + "\n";
				sprites.Add(new MySprite(SpriteType.TEXT, txt, bodyTxtPosition, null, ColorManager.Text, null, TextAlignment.LEFT, 0.65f * ySizeMult));

				/*
				// Create square icon variant. 
				int yTick = 0;
				int xTick = 0;
				int iconSize = 22;
				for (int j = 0; j < celestialBody.ResourcesList.Count; j++)
				{

					string text = celestialBody.ResourcesList[j];

					Vector2 bodyTxtPosition = new Vector2(
						(iconSize * 0.25f) + (margin.X + position.X) + ((3 + iconSize) * xTick),
						(iconSize * 0.65f) + (size.Y * ySizeMult + position.Y) + ((3 + iconSize) * yTick)
					);
					sprites.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", bodyTxtPosition, new Vector2I(iconSize), ColorManager.Border));
					sprites.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", bodyTxtPosition, new Vector2I(iconSize - 2), ColorManager.Fill));

					// Text test.
					sprites.Add(new MySprite(SpriteType.TEXT, text, new Vector2(bodyTxtPosition.X - 1, bodyTxtPosition.Y - 6), null, ColorManager.Text, null, TextAlignment.CENTER, 0.4f * ySizeMult));

					xTick++;
					if (xTick != 0 && xTick % 7 == 0)
					{
						xTick = 0;
						yTick++;
					}

				}
				*/

				if (!hideMap && i == 4)
				{
					break;
				}

			}

			return sprites;

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
			else if (num > 16)
				x = 4;
			else if (num > 8)
				x = 3;
			else
				x = 2;

			/*/ Y
			if (num < 3)
				y = (int)Math.Round((0.42f * num) + 0.75f);
			else if (num > 15)
				y = (int)Math.Round((0.24f * num) + 0.7f);
			else if (num > 9)
				y = (int)Math.Round((0.3f * num) + 0.8f);
			else
				y = 3;*/

			/*/ Y
			if (num>15)
				y = (int)Math.Round(num/4f) + 0.7f;
			if (num>8)
				y = (int)Math.Round(num/3f) + 0.7f;
			if (num>2)
				y = (int)Math.Round(num/2f) + 0.7f;
			else
				y = 1;*/

			// Y
				y = (int)Math.Round((num+0f)/x);

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