using Sandbox.ModAPI;
using SolarMap.Managers;
using SolarMap.SessionComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.GUI.TextPanel;

namespace SolarMap.Panels
{
	public abstract class Panel
	{

		public Panel(SolarMapTSSBase solarMap)
		{
			ColorManager = solarMap.ColorManager;
			SolarSystem = solarMap.SolarSystem;
		}

		public ColorManager ColorManager { get; private set; }
		public SolarSystemSessionComponent SolarSystem { get; private set; }

	}
}
