/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
	public class Manager
	{
		public static Manager Mgr;

        public GameObject UIAnchor;
		public bool HideInfo;
		public bool IsPaused = false;
        public bool DebugStats = true;
        public bool DebugMovement = false;
        public bool DebugSpells = false;
        public bool DebugAI = true;
        public bool DebugOddAttack = false;
        public bool DebugOddPosition = false;
        public Def.GameInputControl GameInputControl = Def.GameInputControl.Mouse;
        public bool InvertCameraMovement = true;

		public Sprite SpriteShadow;
		
		void CreateShadow()
		{
            var SpriteShadowTex = new Texture2D(128, 128)
            {
                name = "Sprite Shadow"
            };
            var center = new Vector2Int(SpriteShadowTex.width / 2, SpriteShadowTex.height / 2);
            var maxDist = Mathf.Abs(Vector2Int.Distance(center, new Vector2Int(32, 32)));
            var colors = new Color32[SpriteShadowTex.width * SpriteShadowTex.height];
            for (int y = 0; y < SpriteShadowTex.height; ++y)
            {
                int yOffset = y * SpriteShadowTex.width;
                for (int x = 0; x < SpriteShadowTex.width; ++x)
                {
                    var dist = Vector2Int.Distance(center, new Vector2Int(x, y));
                    dist = Mathf.Abs(dist);
                    float alpha;
                    if (dist > maxDist)
                    {
                        alpha = 0.0f;
                    }
                    else
                    {
                        dist /= maxDist;
                        alpha = 1.0f - dist;
                        alpha *= alpha;
                    }
                    if (alpha > 0.5f)
                        alpha = 0.5f;
                    colors[x + yOffset] = new Color32(0, 0, 0, (byte)Mathf.FloorToInt(alpha * 255f));
                }
            }
            SpriteShadowTex.SetPixels32(colors);
            SpriteShadowTex.Apply(true, true);
            SpriteShadow = Sprite.Create(SpriteShadowTex,
                new Rect(0f, 0f, SpriteShadowTex.width, SpriteShadowTex.height),
                new Vector2(0f, 0f), 100f, 0, SpriteMeshType.FullRect);
        }
		public static void Prepare()
		{
			Mgr = new Manager();
            Mgr.CreateShadow();
		}
	}
}
