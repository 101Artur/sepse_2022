using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRageMath;
using VRage.Collections;

namespace SolarMap.SolarSystem
{
	public class Grid
	{

		private readonly MyCubeGrid grid;
		private readonly List<MyShipController> shipControllers;
		private MyShipController main;
		private int tick = 0;

		public Grid(IMyCubeGrid grid)
		{
			this.grid = grid as MyCubeGrid;
			shipControllers = new List<MyShipController>();
		}

		/// <summary>
		/// Where the grid is positioned in the world. 
		/// </summary>
		public Vector3D Position { get; private set; }

		/// <summary>
		/// Contains the selected IMyShipController.
		/// </summary>
		public MyShipController Main {
			get {
				return HasController ? main : null;
			}
		}

		/// <summary>
		/// Returns true if the main controller is not null and is working. 
		/// </summary>
		public bool HasController => main != null && main.IsWorking;

		/// <summary>
		/// Returns true if there is a valid controller and the grid is not static. 
		/// </summary>
		public bool IsMoveable => HasController && !grid.IsStatic;

		/// <summary>
		/// Updates controllers list, main controller, and the position of the grid. 
		/// </summary>
		public void Update()
		{

			// Update blocks.
			GetControllers();

			// Update controller.
			SelectController();

			// Update position.
			Position = grid.WorldMatrix.Translation;

			tick++;

		}

		/// <summary>
		/// Attempts to retrieve grid controllers. 
		/// </summary>
		private void GetControllers()
		{

			if (tick % 100 != 0)
				return;

			shipControllers.Clear();

			foreach (MyCubeBlock block in grid.GetFatBlocks())
			{

				if (!(block is MyShipController && block is IMyShipController))
					continue;

				// Using ingame interface to gain access to CanControlShip
				IMyShipController shipController = block as IMyShipController;

				if (!shipController.IsFunctional && !shipController.CanControlShip)
					continue;

				shipControllers.Add(block as MyShipController);

			}

		}

		/// <summary>
		/// Sets the main controller. 
		/// </summary>
		private void SelectController()
		{

			if (tick % 10 != 0)
				return;

			foreach (MyShipController controller in shipControllers)
			{

				if (controller.IsMainCockpit)
				{
					main = controller;
					return;
				}

				main = controller;

			}

		}

	}
}
