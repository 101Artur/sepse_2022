using Sandbox.Game.GameSystems.TextSurfaceScripts;
using Sandbox.ModAPI;
using SolarMap.Managers;
using SolarMap.SessionComponents;
using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI;
using VRageMath;

namespace SolarMap
{
	public abstract class SolarMapTSSBase : MyTextSurfaceScriptBase
	{

		public SolarMapTSSBase(IMyTextSurface surface, IMyCubeBlock block, Vector2 size) : base(surface, block, size)
		{
			Surface = surface;
			SolarSystem = new SolarSystemSessionComponent(block.CubeGrid);
			Sprites = new List<MySprite>();
			ColorManager = new ColorManager(surface);
		}

		public new IMyTextSurface Surface { get; set; }
		public List<MySprite> Sprites { get; private set; }
		public MySpriteDrawFrame Frame { get; set; }
		public SolarSystemSessionComponent SolarSystem { get; private set; }
		public ColorManager ColorManager { get; private set; }

		public override ScriptUpdate NeedsUpdate => ScriptUpdate.Update10;

	}
}
