/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    [CreateAssetMenu(fileName = "MonsterFamily", menuName = "Assets/MonsterFamily", order = 4)]
    public class MonsterFamily : ScriptableObject
    {
        //[InspectorButton("OnUpdateInfo")]
        public bool UpdateInfo;

        public void OnUpdateInfo()
        {
            var _name = (Name == null || (Name != null && Name.Length == 0)) ? "Monster_" + name : Name;
            Debug.Log("Updating " + _name + "...");

            if(Frames == null || (Frames != null && Frames.Length < Def.MonsterFrameCount))
            {
                Debug.LogError("Trying to update '" + _name + "', but it does not have the required sprites.");
                return;
            }

            void SetVisibility(int curSpriteIdx)
            {
                var topPixel = new Vector2Int(-1, -1);
                var bottomPixel = new Vector2Int(-1, -1);
                var LeftPixel = new Vector2Int(-1, -1);
                var RightPixel = new Vector2Int(-1, -1);
                var smon = Frames[curSpriteIdx];

                var colors = smon.texture.GetPixels32();

                for (int y = 0; y < smon.texture.height; ++y)
                {
                    int yOffset = smon.texture.width * y;
                    for (int x = 0; x < smon.texture.width; ++x)
                    {
                        var color = colors[yOffset + x];
                        if (color.a == 0)
                            continue;

                        var pixel = new Vector2Int(x, y);
                        if (topPixel.y < 0 || y > topPixel.y)
                        {
                            topPixel = pixel;
                        }
                        if (bottomPixel.y < 0 || y < bottomPixel.y)
                        {
                            bottomPixel = pixel;
                        }
                        if (LeftPixel.x < 0 || x < LeftPixel.x)
                        {
                            LeftPixel = pixel;
                        }
                        if (RightPixel.x < 0 || x > RightPixel.x)
                        {
                            RightPixel = pixel;
                        }
                    }
                }

                VisibleRect[curSpriteIdx] = new RectInt(LeftPixel.x, bottomPixel.y, RightPixel.x - LeftPixel.x, topPixel.y - bottomPixel.y);
                LastPixel[curSpriteIdx] = bottomPixel;
                BoxSize[curSpriteIdx] = new Vector2(VisibleRect[curSpriteIdx].width, VisibleRect[curSpriteIdx].height) / smon.pixelsPerUnit;

                var visCenter = new Vector2(VisibleRect[curSpriteIdx].width * 0.5f + VisibleRect[curSpriteIdx].x, VisibleRect[curSpriteIdx].height * 0.5f + VisibleRect[curSpriteIdx].y);
                var texCenter = new Vector2(smon.texture.width * 0.5f, smon.texture.height * 0.5f);
                BoxCenterOffset[curSpriteIdx] = visCenter - texCenter;
                BoxCenterOffset[curSpriteIdx] /= smon.pixelsPerUnit;
            }

            var sortedSprites = new Sprite[Def.MonsterFrameCount];
            LastPixel = new Vector2Int[Def.MonsterFrameCount];
            VisibleRect = new RectInt[Def.MonsterFrameCount];
            BoxCenterOffset = new Vector2[Def.MonsterFrameCount];
            BoxSize = new Vector2[Def.MonsterFrameCount];
            Textures = new Texture2D[Def.MonsterFrameCount];
            Pivots = new Vector2[Def.MonsterFrameCount];
            for (int i = 0; i < Frames.Length; ++i)
            {
                var split = Frames[i].name.ToLower().Split('_');
                var facingFrame = split[1];
				Def.MonsterFrame monFrame;
				switch (facingFrame)
				{
					case "b1":
						monFrame = Def.MonsterFrame.BACK_1;
						break;
					case "b2":
						monFrame = Def.MonsterFrame.BACK_2;
						break;
					case "f1":
						monFrame = Def.MonsterFrame.FACE_1;
						break;
					case "f2":
						monFrame = Def.MonsterFrame.FACE_2;
						break;
                    case "fa":
                        monFrame = Def.MonsterFrame.FACE_ATTACK;
                        break;
                    case "ba":
                        monFrame = Def.MonsterFrame.BACK_ATTACK;
                        break;
					default:
						Debug.LogError("Monster '" + _name + "', has an invalid facing frame format.");
						return;
				}
				sortedSprites[(int)monFrame] = Frames[i];
                Textures[(int)monFrame] = Frames[i].texture;
                if(Textures[(int)monFrame].width != Textures[(int)monFrame].height)
                {
                    Debug.LogWarning("Monster '" + _name + "', has a non square texture!");
                }
                if(!Mathf.IsPowerOfTwo(Textures[(int)monFrame].width))
                {
                    Debug.LogWarning("Monster '" + _name + "', has a non power of two texture!");
                }
                Pivots[(int)monFrame] = Frames[i].pivot;
                SetVisibility((int)monFrame);
            }

            Frames = sortedSprites;


            Debug.Log(_name + " Updated!");
        }

        private void OnValidate()
        {
            if(UpdateInfo)
            {
                OnUpdateInfo();
                UpdateInfo = false;
            }
        }

        [Header("Base info")]
        public string Name;
        public float SpriteScale;
        public Sprite[] Frames;
        
        [Space]
        [Header("Combat Stats")]
        public float BaseHealth = 100f;
        public float BaseSpeed = 1f;
        public float SightRange = 4f;
        public float SightAngle = 90f;
        public float HearingRange = 8f;
        public float AttackRange = 3f;
        public float AttackRate = 1f;
        public float AttackDamage = 10f;
        public Def.DamageType DamageType = Def.DamageType.CUT;
        [SerializeReference]
        public MonsterFamily[] FriendlyFamilies;
        public Def.MonsterAIType AIType;
        
        [Space]
        [Header("Resistances")]
        [Range(-100, 100)]
        public int PhysicalResistance = 50;
        [Range(-100, 100)]
        public int ElementalResistance = 50;
        [Range(-100, 100)]
        public int UltimateResistance = 50;
        [Range(-100, 100)]
        public int SoulResistance = 50;
        [Range(-100, 100)]
        public int PoisonResistance = 50;


        [HideInInspector]
        public Vector2Int[] LastPixel;
        [HideInInspector]
        public RectInt[] VisibleRect;
        [HideInInspector]
        public Vector2[] BoxCenterOffset;
        [HideInInspector]
        public Vector2[] BoxSize;
        [HideInInspector]
        public Texture2D[] Textures;
        [HideInInspector]
        public Vector2[] Pivots;
        [HideInInspector]
        [NonSerialized]
        public Type Class;
        [HideInInspector]
        [NonSerialized]
        public AI.MonsterInfo Info;
		[NonSerialized]
		public List<string> FamilyTags;
    }
}