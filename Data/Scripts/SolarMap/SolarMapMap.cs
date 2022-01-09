using System.Collections.Generic;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using VRageMath;
using Sandbox.Game.GameSystems.TextSurfaceScripts;
using VRage.Game.GUI.TextPanel;
using SolarMap.SessionComponents;
using SolarMap.Panels;
using SolarMap.Managers;

namespace SolarMap
{

    [MyTextSurfaceScript("SolarMapMap", "Solar Map (Map)")]
    public class SolarMapMap : SolarMapTSSBase
    {

        public SolarMapMap(IMyTextSurface surface, IMyCubeBlock block, Vector2 size) : base(surface, block, size)
        {
            MapPanel = new MapPanel(this);
        }

        public MapPanel MapPanel { get; private set; }

        public override void Run()
        {

            // LCD dimension error. 
            if (Surface.SurfaceSize.Y != 512)
            {
                DrawError("Solar map cannot be displayed on 5:3 text panel.");
                return;
            }

            SolarSystem.Grid.Update();
            MapPanel.SortCelestialLists();
            ColorManager.UpdateColors();
            DrawSprites();

        }

        public override void Dispose()
        {
            base.Dispose();
        }

        private void DrawSprites()
        {

            Sprites.Clear();

            // Add paint. Must be added in the correct order. 
            using (Frame = Surface.DrawFrame())
            {
                Frame.AddRange(MapPanel.GetAllSprites());
            }

        }

        private void DrawError(string text)
        {
            using (Frame = Surface.DrawFrame())
            {
                Frame.Add(new MySprite(SpriteType.TEXT, text, new Vector2(512 / 2, 307.2f / 2 * 1.6f), null, Surface.ScriptForegroundColor, null, rotation: 0.6f));
            }
        }

    }

}