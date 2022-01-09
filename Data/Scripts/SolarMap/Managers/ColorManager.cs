using Sandbox.ModAPI;
using VRageMath;

namespace SolarMap.Managers
{
	public class ColorManager
	{

		private IMyTextSurface surface;
		private Vector3 hsv = Vector3.Zero; // Hue, Saturation, Value
		public Color PanelBackground = new Color(0, 0, 0, 50);
		public Color TitleBackground = new Color(0, 0, 0, 150);

		public ColorManager(IMyTextSurface surface)
		{
			this.surface = surface;
		}

		public Color Fill { get; private set; }
		public Color Border { get; private set; }
		public Color Grid { get; private set; }
		public Color Text { get; private set; }
		private bool BgIsDark => hsv.Z < 0.125 || (hsv.Y > 0.5 && hsv.Z < 0.25);
		private bool BgIsRed => (hsv.X < 0.1 || hsv.X > 0.9) && hsv.Y > 0.65 && hsv.Z > 0.125;
		private bool BgIsBlue => hsv.X > 0.5 && hsv.X < 0.75 && hsv.Y > 0.25 && hsv.Z > 0.25;

		public void UpdateColors()
		{
			UpdateGeneralColors();
			UpdateGridArrowColor();
		}

		/// <summary>
		/// Updates background and foreground colors of planets, orbits, and text. 
		/// </summary>
		private void UpdateGeneralColors()
		{

			Fill = surface.ScriptBackgroundColor;
			Border = new Color(surface.ScriptForegroundColor, 0.5f);
			Text = surface.ScriptForegroundColor;

		}

		/// <summary>
		/// Updates the color of the grid arrow/dot. 
		/// </summary>
		private void UpdateGridArrowColor()
		{

			hsv = surface.ScriptBackgroundColor.ColorToHSV();

			if (BgIsBlue)
			{
				Grid = Color.Red;
			}
			else if (BgIsRed)
			{
				Grid = Color.LimeGreen;
			}
			else
			{
				Grid = Color.Black;
				if (BgIsDark)
				{
					Grid = Color.Red;
				}
			}

		}
	}
}
