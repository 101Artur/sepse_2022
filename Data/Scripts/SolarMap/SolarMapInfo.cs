﻿using System.Collections.Generic;
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

    [MyTextSurfaceScript("SolarMapInfo", "Solar Map (Info)")]
    public class SolarMapInfo : SolarMapTSSBase
    {

        public SolarMapInfo(IMyTextSurface surface, IMyCubeBlock block, Vector2 size) : base(surface, block, size)
        {
            InfoPanel = new InfoPanel(this);
        }

        public InfoPanel InfoPanel { get; private set; }

        public override void Run()
        {

            // LCD dimension error. 
            if (Surface.SurfaceSize.Y != 512)
            {
                DrawError("Solar map cannot be displayed on 5:3 text panel.");
                return;
            }

            SolarSystem.Grid.Update();
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
                Frame.AddRange(InfoPanel.GetAllSprites(true));
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