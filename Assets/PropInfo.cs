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
	[CreateAssetMenu(fileName = "PropInfo", menuName = "Assets/Prop", order = 5)]
	public class PropInfo : ScriptableObject
	{
		public bool UpdateInfo;

		const int MaxColorDistance = 40;
		const int MaxColors = 50;
		public void OnUpdateInfo()
		{
			var _name = (FamilyName == null || (FamilyName != null && FamilyName.Length == 0)) ? 
				"Prop_" + name : FamilyName;
			Debug.Log("Updating " + _name + "...");

			if(PropSprite == null)
			{
				Debug.LogError("Trying to update '" + _name + "', but it does not have the required sprite.");
				return;
			}

			Texture = PropSprite.texture;
			if (Texture.width != Texture.height)
			{
				Debug.LogWarning("Prop '" + _name + "', has a non square texture!");
			}
			if (!Mathf.IsPowerOfTwo(Texture.width))
			{
				Debug.LogWarning("Prop '" + _name + "', has a non power of two texture!");
			}

			// Read pixels
			var topPixel = new Vector2Int(-1, -1);
			var bottomPixel = new Vector2Int(-1, -1);
			var LeftPixel = new Vector2Int(-1, -1);
			var RightPixel = new Vector2Int(-1, -1);

			var height25 = Texture.height / 4;

			var damageColors = new List<Color32>(MaxColors);
			var damageColorProbs = new List<float>(damageColors.Capacity);

			int colorCount = 0;
			var texColors = Texture.GetPixels32();
			for (int y = 0; y < Texture.height; ++y)
			{
				int hOffset = y * Texture.width;
				for (int x = 0; x < Texture.width; ++x)
				{
					var color = texColors[hOffset + x];
					if (color.a == 0)
						continue;
					color.a = 255;
					++colorCount;
					// Damage check
					int damageIdx = -1;
					for (int i = 0; i < damageColors.Count; ++i)
					{
						var dColor = damageColors[i];
						if (Vector3Int.Distance(new Vector3Int(dColor.r, dColor.g, dColor.b), new Vector3Int(color.r, color.g, color.b)) <= MaxColorDistance)
						{
							damageIdx = i;
							break;
						}
					}
					if (damageIdx == -1)
					{
						damageColors.Add(color);
						damageColorProbs.Add(1f);
					}
					else
					{
						damageColorProbs[damageIdx] = damageColorProbs[damageIdx] + 1f;
					}

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


			for (int i = 0; i < damageColorProbs.Count; ++i)
			{
				damageColorProbs[i] = damageColorProbs[i] / colorCount;
			}
			GameUtils.Sort(damageColorProbs, (int prev, int cur) => { var tempColor = damageColors[cur]; damageColors[cur] = damageColors[prev]; damageColors[prev] = tempColor; });

			int firstPixel25 = -1, lastPixel25 = -1;

			var prevPixel = texColors[height25 * Texture.width];
			var curPixel = texColors[height25 * Texture.width + 1];

			for (int j = 1; j < (Texture.width - 1); ++j)
			{
				var nextPixel = texColors[height25 * Texture.width + j + 1];
				if (firstPixel25 < 0)
				{
					if (prevPixel.a == 0 && curPixel.a != 0)
						firstPixel25 = j;
				}

				if (curPixel.a != 0 && nextPixel.a == 0)
					lastPixel25 = j;

				prevPixel = curPixel;
				curPixel = nextPixel;
			}
			VisibleRect = new RectInt(LeftPixel.x, bottomPixel.y, RightPixel.x - LeftPixel.x, topPixel.y - bottomPixel.y);
			LastPixel = bottomPixel;
			BoxSize = new Vector2(VisibleRect.width, VisibleRect.height) / PropSprite.pixelsPerUnit;

			var visCenter = new Vector2(VisibleRect.width * 0.5f + VisibleRect.x, VisibleRect.height * 0.5f + VisibleRect.y);
			var texCenter = new Vector2(Texture.width * 0.5f, Texture.height * 0.5f);
			BoxCenterOffset = visCenter - texCenter;
			BoxCenterOffset /= PropSprite.pixelsPerUnit;
			BoxWidth = (lastPixel25 - firstPixel25) / (float)Texture.width;

			DamageColors = damageColors;
			DamageColorProbs = damageColorProbs;

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
		public float SpriteScale;
		public bool HasShadow = true;
		public Sprite PropSprite;
		public string FamilyName;

		[Header("Lighting")]
		public bool HasLighting = false;
		public float LightHeight = 1f;
		public float LightRange = 1f;
		public float LightIntensity = 1f;
		public Color32 LightColor = new Color32(255, 255, 255, 255);

		public float BaseHealth;
		[Header("Resistances")]
		[Range(-100, 100)] public int PhysicalResistance = 50;
		[Range(-100, 100)] public int ElementalResistance = 50;
		[Range(-100, 100)] public int UltimateResistance = 50;
		[Range(-100, 100)] public int SoulResistance = 50;
		[Range(-100, 100)] public int PoisonResistance = 50;

		[HideInInspector] [SerializeField] Vector2Int LastPixel;
		[HideInInspector] [SerializeField] RectInt VisibleRect;
		[HideInInspector] [SerializeField] Vector2 BoxCenterOffset;
		[HideInInspector] [SerializeField] Vector2 BoxSize;
		[HideInInspector] [SerializeField] float BoxWidth;
		[HideInInspector] [SerializeField] Vector2 Pivot;
		[HideInInspector] [SerializeField] Texture2D Texture;
		[HideInInspector] [SerializeField] List<Color32> DamageColors;
		[HideInInspector] [SerializeField] List<float> DamageColorProbs;
		[NonSerialized] public PropFamily Family;
		public Vector2Int GetLastPixel() => LastPixel;
		public RectInt GetVisibleRect() => VisibleRect;
		public Vector2 GetBoxCenterOffset() => BoxCenterOffset;
		public Vector2 GetBoxSize() => BoxSize;
		public float GetBoxWidth() => BoxWidth;
		public Vector2 GetPivot() => Pivot;
		public void _SetPivot(Vector2 pivot) => Pivot = pivot;
		public Texture2D GetTexture() => Texture;
		public List<Color32> GetDamageColors() => DamageColors;
		public List<float> GetDamageColorProbs() => DamageColorProbs;
	}
}