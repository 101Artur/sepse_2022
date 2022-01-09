using Sandbox.Definitions;
using Sandbox.Game.GUI;
using VRage.Game;
using VRage.Game.Components;
using VRage.Utils;

namespace BalancedDeformation
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public class Core : MySessionComponentBase
    {

        public const float General_Deformation_Ratio = 0.25f;
        public const float HA_Deformation_Ratio = 0.25f;
        public const float LA_Deformation_Ratio = 0.75f;
        public const float General_Damage_Multiplier = 1f;
        public const float small_grid_HA_Damage_Multiplier = 0.5f;
        public const float large_grid_HA_Damage_Multiplier = 1.0f;

        public override bool UpdatedBeforeInit()
        {
            foreach (MyDefinitionBase def in MyDefinitionManager.Static.GetAllDefinitions())
            {
                MyCubeBlockDefinition blockDef = def as MyCubeBlockDefinition;

                if (blockDef == null) continue;

                blockDef.DeformationRatio = General_Deformation_Ratio;
		blockDef.GeneralDamageMultiplier = General_Damage_Multiplier;

                if (blockDef.Id.SubtypeName.Contains("Armor"))
                {
                    	blockDef.DeformationRatio = LA_Deformation_Ratio;
			if (blockDef.Id.SubtypeName.Contains("Heavy")) 
				{
				blockDef.GeneralDamageMultiplier = large_grid_HA_Damage_Multiplier; blockDef.DeformationRatio = HA_Deformation_Ratio;
				}
		    	if (blockDef.Id.SubtypeName.Contains("Heavy") && blockDef.Id.SubtypeName.Contains("Small")) 
				{
				blockDef.GeneralDamageMultiplier = small_grid_HA_Damage_Multiplier; blockDef.DeformationRatio = HA_Deformation_Ratio;
				}
                }
            }

            return true;
        }
    }
}

namespace sepse.speed9999
{
    using Sandbox.Common;
    using Sandbox.Definitions;
    using Sandbox.Game.Gui;
    using Sandbox.Game.World;
    using VRage.Game.Components;
    using VRageMath;

    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class SessionComponentLogic : MySessionComponentBase
    {
		//public static ModUI instance;
		bool init = false;
		bool isServer = false;
		bool isDedicated = false;
		bool initmenu = false;
        //Vector3 velocidad=0f;

        public override void LoadData()
        {
            MyDefinitionManager.Static.EnvironmentDefinition.SmallShipMaxSpeed = 9999;
            MyDefinitionManager.Static.EnvironmentDefinition.LargeShipMaxSpeed = 9999;

            base.LoadData();

            //MyStatControlledEntitySpeed.MaxValue=1000f;

        }

 		/*public override void Draw()
		{
            MyDefinitionManager.Static.EnvironmentDefinition.SmallShipMaxSpeed = 500;
            MyDefinitionManager.Static.EnvironmentDefinition.LargeShipMaxSpeed = 500;
			//Update();
		}
		public void Init()
		{
			if (init)
				return;//script already initialized, abort.
			instance = this;
			init = true;

			isServer = MyAPIGateway.Session.OnlineMode == MyOnlineModeEnum.OFFLINE || MyAPIGateway.Multiplayer.IsServer;
			isDedicated = (MyAPIGateway.Utilities.IsDedicated && isServer);
			if (isDedicated) return;
		}

		private void Update()
		{

			if (!init)
			{

                if (MyAPIGateway.Session == null)
					return;
				if (MyAPIGateway.Multiplayer == null && MyAPIGateway.Session.OnlineMode != MyOnlineModeEnum.OFFLINE)
					return;
				Init();
			}

			if (MyAPIGateway.Session?.Camera == null) return;

			if (FontUI != null)
				FontUI.Update();


			if (MyAPIGateway.Session == null)
			{
				//unload();
			}
			if (isDedicated) return;




      if (v != Vector3.Zero && this.m_velocityTexture != null)
      {
        Vector2 vector2_1 = new Vector2((float) right.Dot(v), -(float) up.Dot(v));
        float transitionAlpha = Math.Min(MyMath.Clamp((float) ((double) v.Length() / ((double) MyGridPhysics.ShipMaxLinearVelocity() + 7.0) / 0.0500000007450581), 0.0f, 1f), alpha);
        Vector2 vector2_2 = this.m_screenPosition + this.m_sizeOnScreen * 0.5f - this.m_velocitySizeOnScreen * 0.5f;
        float num5 = vector2_1.Length();
        float num6 = MyMath.Clamp(1f - (float) Math.Exp(-(double) num5 * 0.00999999977648258), 0.0f, 1f);
        vector2_1 *= 1f / num5 * num6;
        Vector2 position = vector2_2 + vector2_1 * (this.m_sizeOnScreen / 2f);
        Rectangle? sourceRectangle = new Rectangle?();
        destination = new RectangleF(position, this.m_velocitySizeOnScreen);
        MyRenderProxy.DrawSpriteExt(this.m_velocityTexture.Path, ref destination, sourceRectangle, MyGuiControlBase.ApplyColorMaskModifiers((Vector4) Color.White, true, transitionAlpha), ref Vector2.UnitX, ref this.m_origin, false, true);
      }






		}

    }

    public static class Velocidad
    {
        public static Vector3 velocidad=new Vector3();
    }
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation, Priority = 9)]
    public class SessionComponentLogic1 : MySessionComponentBase
    {
		public override void UpdateBeforeSimulation()
		{
        IMyTerminalBlock controlledEntity = MyAPIGateway.Session.LocalHumanPlayer.Controller.ControlledEntity as IMyTerminalBlock;
        //IMyControllableEntity controlledEntity = MySession.Static.ControlledEntity;
        if (controlledEntity == null)
            return;
        MyPhysicsComponentBase physicsComponentBase = controlledEntity.CubeGrid?.Physics;
        Velocidad.velocidad = (physicsComponentBase != null ? physicsComponentBase.LinearVelocity : Vector3.Zero);
        if (physicsComponentBase!=null) physicsComponentBase.LinearVelocity=Vector3.Normalize(Velocidad.velocidad)*MyDefinitionManager.Static.EnvironmentDefinition.LargeShipMaxSpeed;
        MyHud.GravityIndicator.Show();
		}
    }
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation, Priority = 11)]
    public class SessionComponentLogic2 : MySessionComponentBase
    {
		public override void UpdateBeforeSimulation()
		{
        IMyTerminalBlock controlledEntity = MyAPIGateway.Session.LocalHumanPlayer.Controller.ControlledEntity as IMyTerminalBlock;
        //IMyControllableEntity controlledEntity = MySession.Static.ControlledEntity;
        if (controlledEntity == null)
            return;
        MyPhysicsComponentBase physicsComponentBase = controlledEntity.CubeGrid?.Physics;
        if (physicsComponentBase!=null) physicsComponentBase.LinearVelocity=Velocidad.velocidad;
		}*/
    }
}

