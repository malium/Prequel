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

namespace Assets.World
{
	[Serializable]
	public class Perlin
	{
		[SerializeField] public int Seed;
		[SerializeField] public Vector2 Offset;
		[SerializeField] public float Contrast;
		[SerializeField] public float Frequency;
		[SerializeField] public Vector2Int Size;
		public float[] Samples;
		void NormalizeArray(float[] arr)
		{
			float min = float.PositiveInfinity;
			float max = float.NegativeInfinity;

			for (int i = 0; i < arr.Length; ++i)
			{
				float value = arr[i];
				if (value < min) min = value;
				if (value > max) max = value;
			}
			float range = max - min;
			for (int i = 0; i < arr.Length; ++i)
			{
				float value = arr[i];
				arr[i] = (value - min) / range;
			}
		}
		public void Generate()
		{
			var noise = new ProceduralNoiseProject.PerlinNoise(Seed, Frequency)
			{
				Offset = Offset
			};
			Samples = new float[Size.x * Size.y];

			float xMult = 1f / (Size.x - 1f);
			float yMult = 1f / (Size.y - 1f);
			for (int i = 0; i < Samples.Length; ++i)
			{
				var pos = GameUtils.PosFromIDUnsafe(i, Size.x, Size.y);
				float fx = pos.x * xMult;
				float fy = pos.y * yMult;

				Samples[i] = noise.Sample2D(fx, fy);
			}
			NormalizeArray(Samples);

			var contrastValue = (100f + Contrast) / 100f;
			contrastValue *= contrastValue;
			//var colors = m_PerlinTexture.GetPixels32();
			//var color = new Color32(0, 0, 0, 255);
			float inv255 = 1f / 255f;
			for (int i = 0; i < Samples.Length; ++i)
			{
				var colorValue = (int)((((Samples[i] - 0.5f) * contrastValue) + 0.5f) * 255f);
				var color = (byte)Mathf.Clamp(colorValue, 0, 255);
				Samples[i] = color * inv255;
				//colors[i] = color;
			}
			//m_PerlinTexture.SetPixels32(colors);
			//m_PerlinTexture.Apply();
		}
		public void Seamlessify(float seamlessPCT = 0.6666666f)
		{
			if (seamlessPCT < 0.5f || seamlessPCT > 1f)
				throw new Exception("Trying to Seamlessify a perlin with an invalid seamlessPCT value " + seamlessPCT.ToString());
			var invPct = 1f - seamlessPCT;
			var invSizeX = 1f / Size.x;
			var invSizeY = 1f / Size.y;
			var invRange = 1f / invPct;

			var sPos = new Vector2Int((int)(Size.x * seamlessPCT), (int)(Size.y * seamlessPCT));
			for (int y = 0; y < sPos.y; ++y)
			{
				for (int x = sPos.x; x < Size.x; ++x)
				{
					var coord = new Vector2(
						x * invSizeX,
						y * invSizeY);
					var id = GameUtils.IDFromPosUnsafe(new Vector2Int(x, y), Size.x, Size.y);
					var invPos = new Vector2Int((Size.x - 1) - x, y);
					int invID = GameUtils.IDFromPosUnsafe(invPos, Size.x, Size.y);
					Samples[id] = Mathf.Lerp(Samples[id], Samples[invID], (coord.x - seamlessPCT) * invRange);
				}
			}
			for (int y = sPos.y; y < Size.y; ++y)
			{
				for (int x = 0; x < sPos.x; ++x)
				{
					var coord = new Vector2(
						x * invSizeX,
						y * invSizeY);
					var id = GameUtils.IDFromPosUnsafe(new Vector2Int(x, y), Size.x, Size.y);
					var invPos = new Vector2Int(x, (Size.x - 1) - y);
					int invID = GameUtils.IDFromPosUnsafe(invPos, Size.x, Size.y);
					Samples[id] = Mathf.Lerp(Samples[id], Samples[invID], (coord.y - seamlessPCT) * invRange);
				}
			}
			for (int y = sPos.y; y < Size.y; ++y)
			{
				for (int x = sPos.x; x < Size.x; ++x)
				{
					var coord = new Vector2(
						x * invSizeX,
						y * invSizeY);
					var id = GameUtils.IDFromPosUnsafe(new Vector2Int(x, y), Size.x, Size.y);
					var invPos = new Vector2Int(x, (Size.y - 1) - y);
					int invID = GameUtils.IDFromPosUnsafe(invPos, Size.x, Size.y);
					Samples[id] = Mathf.Lerp(Samples[id], Samples[invID], (coord.y - seamlessPCT) * invRange);
					invPos = new Vector2Int((Size.x - 1) - x, y);
					invID = GameUtils.IDFromPosUnsafe(invPos, Size.y, Size.y);
					Samples[id] = Mathf.Lerp(Samples[id], Samples[invID], (coord.x - seamlessPCT) * invRange);
				}
			}
		}
	}
}
