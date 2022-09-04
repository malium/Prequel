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
	public class NoiseTest : MonoBehaviour
	{
		public Texture2D NoiseTexture;
		public int Size = 128;
		public int Seed = 0;
		public float Frequency = 1f;
		public Vector2 Offset = Vector2.zero;
		[Range(-100f, 100f)]
		public float Contrast = 0f;
		public bool Seamless = true;
		public float SeamlessPCT = 0.75f;
		int m_Size = 128;
		int m_Seed = 0;
		float m_Frequency = 1f;
		Vector2 m_Offset = Vector2.zero;
		float m_Contrast = 0f;
		bool m_Seamless = true;
		float m_SeamlessPCT = 0.75f;
		public UnityEngine.UI.RawImage Image;

		private void OnValidate()
		{
			if (!gameObject.activeSelf)
				return;
			if (Size < 1)
				Size = 1;

			if (m_Samples == null ||
				m_SeamlessPCT != SeamlessPCT ||
				m_Seamless != Seamless ||
				m_Seed != Seed ||
				m_Size != Size ||
				m_Frequency != Frequency ||
				m_Offset != Offset ||
				m_Contrast != Contrast)
				GenerateNoise();
		}
		float[] m_Samples;
		float[] m_TempSamples;
		void NormalizeArray(float[] arr)
		{
			float min = float.PositiveInfinity;
			float max = float.NegativeInfinity;

			for(int i = 0; i < arr.Length; ++i)
			{
				float value = arr[i];
				if (value < min) min = value;
				if (value > max) max = value;
			}
			float range = max - min;
			for(int i = 0; i < arr.Length; ++i)
			{
				float value = arr[i];
				arr[i] = (value - min) / range;
			}
		}
		void Seamlessify()
		{
			var invPct = 1f - m_SeamlessPCT;
			var invSize = 1f / m_Size;
			var invRange = 1f / invPct;
			//TimeSpan accum = new TimeSpan();
			//const int amount = 1;
			//for (int i = 0; i < amount; ++i)
			//{
			//	var start = DateTime.Now;

				//for (int j = 0; j < m_Samples.Length; ++j)
				//{
				//	var pos = GameUtils.PosFromIDUnsafe(j, m_Size, m_Size);
				//	var coord = new Vector2(
				//		pos.x * invSize,
				//		pos.y * invSize);
				//	if (coord.x < pct && coord.y < pct)
				//		continue;

				//	//Debug.Log("Pos: " + pos.ToString() + " InvPos: " + invPos.ToString());
				//	if (coord.x >= pct)
				//	{
				//		var invPos = new Vector2Int((m_Size - 1) - pos.x, pos.y);
				//		int invID = GameUtils.IDFromPosUnsafe(invPos, m_Size, m_Size);
				//		m_Samples[j] = Mathf.Lerp(m_Samples[j], m_Samples[invID], (coord.x - pct) * invRange);
				//	}
				//	if (coord.y >= pct)
				//	{
				//		var invPos = new Vector2Int(pos.x, (m_Size - 1) - pos.y);
				//		int invID = GameUtils.IDFromPosUnsafe(invPos, m_Size, m_Size);
				//		m_Samples[j] = Mathf.Lerp(m_Samples[j], m_Samples[invID], (coord.y - pct) * invRange);
				//	}
				//}
				var sPos = (int)(m_Size * m_SeamlessPCT);
				var xPos = sPos;
				for (int y = 0; y < sPos; ++y)
				{
					for (int x = sPos; x < m_Size; ++x)
					{
						var coord = new Vector2(
							x * invSize,
							y * invSize);
						var id = GameUtils.IDFromPosUnsafe(new Vector2Int(x, y), m_Size, m_Size);
						var invPos = new Vector2Int((m_Size - 1) - x, y);
						int invID = GameUtils.IDFromPosUnsafe(invPos, m_Size, m_Size);
						m_Samples[id] = Mathf.Lerp(m_Samples[id], m_Samples[invID], (coord.x - m_SeamlessPCT) * invRange);
					}
				}
				for (int y = sPos; y < m_Size; ++y)
				{
					for (int x = 0; x < sPos; ++x)
					{
						var coord = new Vector2(
							x * invSize,
							y * invSize);
						var id = GameUtils.IDFromPosUnsafe(new Vector2Int(x, y), m_Size, m_Size);
						var invPos = new Vector2Int(x, (m_Size - 1) - y);
						int invID = GameUtils.IDFromPosUnsafe(invPos, m_Size, m_Size);
						m_Samples[id] = Mathf.Lerp(m_Samples[id], m_Samples[invID], (coord.y - m_SeamlessPCT) * invRange);
					}
				}
				for (int y = sPos; y < m_Size; ++y)
				{
					for (int x = sPos; x < m_Size; ++x)
					{
						var coord = new Vector2(
							x * invSize,
							y * invSize);
						var id = GameUtils.IDFromPosUnsafe(new Vector2Int(x, y), m_Size, m_Size);
						var invPos = new Vector2Int(x, (m_Size - 1) - y);
						int invID = GameUtils.IDFromPosUnsafe(invPos, m_Size, m_Size);
						m_Samples[id] = Mathf.Lerp(m_Samples[id], m_Samples[invID], (coord.y - m_SeamlessPCT) * invRange);
						invPos = new Vector2Int((m_Size - 1) - x, y);
						invID = GameUtils.IDFromPosUnsafe(invPos, m_Size, m_Size);
						m_Samples[id] = Mathf.Lerp(m_Samples[id], m_Samples[invID], (coord.x - m_SeamlessPCT) * invRange);
					}
				}
				//for (int y = 0; y < m_Size; ++y)
				//{
				//	if (y >= sPos)
				//		xPos = 0;
				//	for (int x = xPos; x < m_Size; ++x)
				//	{
				//		var coord = new Vector2(
				//			x * invSize,
				//			y * invSize);
				//		var id = GameUtils.IDFromPosUnsafe(new Vector2Int(x, y), m_Size, m_Size);
				//		if (coord.x >= pct)
				//		{
				//			var invPos = new Vector2Int((m_Size - 1) - x, y);
				//			int invID = GameUtils.IDFromPosUnsafe(invPos, m_Size, m_Size);
				//			m_Samples[id] = Mathf.Lerp(m_Samples[id], m_Samples[invID], (coord.x - pct) * invRange);
				//		}
				//		if (coord.y >= pct)
				//		{
				//			var invPos = new Vector2Int(x, (m_Size - 1) - y);
				//			int invID = GameUtils.IDFromPosUnsafe(invPos, m_Size, m_Size);
				//			m_Samples[id] = Mathf.Lerp(m_Samples[id], m_Samples[invID], (coord.y - pct) * invRange);
				//		}
				//	}
				//}


				//var end = DateTime.Now;
				//var diff = end - start;
				//accum += diff;
			//}
			//var avg = new TimeSpan(accum.Ticks / amount);
			//Debug.Log("SeaTook " + avg.TotalMilliseconds.ToString() + "ms");
		}
		public void GenerateNoise()
		{
			bool m_SampleResize = m_Size != Size;
			bool m_NeedsResampling = m_SampleResize || m_SeamlessPCT != SeamlessPCT || m_Seamless != Seamless || m_Seed != Seed || m_Frequency != Frequency || m_Offset != Offset;
			m_Size = Size;
			m_Seed = Seed;
			m_Frequency = Frequency;
			m_Offset = Offset;
			m_Contrast = Contrast;
			m_Seamless = Seamless;
			m_SeamlessPCT = SeamlessPCT;

			var start = DateTime.Now;

			NoiseTexture = new Texture2D(m_Size, m_Size, TextureFormat.RGB24, false, true);
			var noise = new ProceduralNoiseProject.PerlinNoise(m_Seed, m_Frequency)
			{
				Offset = m_Offset
			};
			if(m_Samples == null || m_SampleResize)
			{
				m_Samples = new float[m_Size * m_Size];
				m_TempSamples = new float[m_Samples.Length];
			}
			if(m_NeedsResampling)
			{
				//Sample the 2D noise and add it into a array.
				float mult = 1f / (m_Size - 1f);
				for(int i = 0; i < m_Samples.Length; ++i)
				{
					var pos = GameUtils.PosFromID(i, m_Size, m_Size);
					float fx = pos.x * mult;
					float fy = pos.y * mult;

					m_Samples[i] = noise.Sample2D(fx, fy);
				}
				//Some of the noises range from -1-1 so normalize the data to 0-1 to make it easier to see.
				NormalizeArray(m_Samples);
				if(m_Seamless)
					Seamlessify();
			}

			var contrastValue = (100f + m_Contrast) / 100f;
			contrastValue *= contrastValue;
			var colors = NoiseTexture.GetPixels32();
			for(int i = 0; i < colors.Length; ++i)
			{
				var color = colors[i];
				var colorValue = (int)((((m_Samples[i] - 0.5f) * contrastValue) + 0.5f) * 255f);
				color.r = color.g = color.b = (byte)Mathf.Clamp(colorValue, 0, 255);
				colors[i] = color;
			}

			var end = DateTime.Now;

			Debug.Log("Took " + (end - start).TotalMilliseconds.ToString() + "ms");

			NoiseTexture.SetPixels32(colors);
			NoiseTexture.Apply();
			if (Image != null)
			{
				Image.texture = NoiseTexture;
				Image.GetComponent<RectTransform>().sizeDelta = new Vector2(m_Size, m_Size);
			}
		}
	}
}
