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
	public class Map : MonoBehaviour
	{
		public const int Width = 32000;
		public const int Height = 32000;
		public const int StrucWidth = 16;
		public const int StrucHeight = 16;
		public const int HorizontalStrucs = Width / StrucWidth;
		public const int VerticalStrucs = Height / StrucHeight;
		public const int MinStrucID = 0;
		public const int MaxStrucID = (HorizontalStrucs * VerticalStrucs) - 1;
		public const int MinPilarID = 0;
		public const int MaxPilarID = (Width * Height) - 1;

		public static bool IsPilarIDValid(int pilarID)
		{
			return pilarID >= MinPilarID && pilarID <= MaxPilarID;
		}
		public static bool IsPilarPosValid(Vector2Int pos)
		{
			return pos.x >= MinPilarID && pos.x <= Width && pos.y >= MinPilarID && pos.y <= Height;
		}
		public static bool IsPilarPosValid(Vector2 pos)
		{
			return IsPilarPosValid(GameUtils.TransformPosition(pos));
		}
		public static int StrucIDFromPilarPos(Vector2Int pos)
		{
			if(!IsPilarPosValid(pos))
			{
				Debug.LogWarning($"Invalid PilarPos '{pos}' Map.StrucIDFromPilarPos");
				return 0;
			}
			var strucPos = new Vector2Int(
				Mathf.FloorToInt((float)pos.x / StrucWidth),
				Mathf.FloorToInt((float)pos.y / StrucHeight));
			return strucPos.y * HorizontalStrucs + strucPos.x;
		}
		public static int StrucIDFromPilarID(int pilarID)
		{
			if (!IsPilarIDValid(pilarID))
			{
				Debug.LogWarning($"Invalid PilarID '{pilarID}' Map.StrucIDFromPilarID");
				return 0;
			}
			var pos = new Vector2Int(pilarID % Width, Mathf.FloorToInt(pilarID / Height));
			return StrucIDFromPilarPos(pos);
		}
		List<int> GetStrucIDRect(RectInt rect)
		{
			var strucs = new List<int>(
				Mathf.CeilToInt((float)rect.width / CStruc.Width) * 2 +
				Mathf.CeilToInt((float)rect.height / CStruc.Height) * 2);

			int struc;
			int maxX = rect.x + rect.width - 1;
			int maxY = rect.y + rect.height - 1;
			for (int y = rect.y; y <= maxY;)
			{
				for (int x = rect.x; x <= maxX;)
				{
					struc = StrucIDFromPilarPos(new Vector2Int(x, y));
					if (!strucs.Contains(struc))
						strucs.Add(struc);

					if (x == maxX)
						break;

					x += CStruc.Width / 2;
					if (x > maxX)
						x = maxX;
				}

				if (y == maxY)
					break;

				y += CStruc.Height / 2;
				if (y > maxY)
					y = maxY;
			}

			return strucs;
		}

		public const float ShutdownDistance = 100f; // Distance where AI are disabled

		static Map m_Current;
		public static Map GetCurrent() => m_Current;

		Dictionary<int, CStruc> m_Structures;
		IE.V3.MapIE m_MapIE;

		private void Awake()
		{
			m_Structures = new Dictionary<int, CStruc>(
				(Width / CStruc.Width) * (Height / CStruc.Height));
		}

		public static void Init(List<CStrucEdit> strucs)
		{
			m_Current = new GameObject("Map").AddComponent<Map>();

			// !! CStrucEdit prepare
			/*
			 * We need to follow this order to create a correct map
			 * Remove layer 0 blocks
			 * Apply voids
			 * Apply stairs
			 * Merge the structure pilars
			 * Apply wides as one super structure (inter-struc wides)
			 * Apply ants
			 * Apply decos
			 * Apply props
			 * Apply monsters
			 * ...
			 */

			// Remove layer 0 blocks, apply voids, apply stairs
			for (int i = 0; i < strucs.Count; ++i)
			{
				var struc = strucs[i];
				var pilars = struc.GetPilars();
				for (int j = 0; j < pilars.Length; ++j)
				{
					var pilar = pilars[j];
					if (pilar == null)
						continue;
					var blocks = pilar.GetBlocks();
					for (int k = 0; k < blocks.Count; ++k)
					{
						var block = blocks[k] as CBlockEdit;
						if (block.GetLayer() == 0)
						{
							block.DestroyBlock(false);
							continue;
						}
						var layer = struc.GetLayers()[block.GetLayer() - 1];
						if (layer.IsLinkedLayer)
						{
							layer = struc.GetLayers()[block.GetLinkedTo() - 1];
						}
						switch (block.GetVoidState())
						{
							case Def.BlockVoid.SEMIVOID:
								var rng = (ushort)(UnityEngine.Random.value * 10000f);
								if (rng > layer.SemiVoidChance)
									break;
								block.DestroyBlock(false);
								--j;
								continue;
							case Def.BlockVoid.FULLVOID:
								block.DestroyBlock(false);
								--j;
								continue;
						}
						switch (block.GetStairState())
						{
							case Def.StairState.POSSIBLE:
								{
									ushort rng = (ushort)(UnityEngine.Random.value * 10000f);
									if (rng > layer.StairBlockChance)
										continue;

									block.SetStairState(Def.StairState.ALWAYS);
								}
								break;
							case Def.StairState.STAIR_OR_RAMP:
								{
									ushort rng = (ushort)(UnityEngine.Random.value * 10000f);
									if (rng > layer.StairBlockChance)
										continue;
									ushort rampRNG = (ushort)(UnityEngine.Random.value * 10000f);
									if (rampRNG >= layer.RampBlockChance)
									{
										block.SetStairState(Def.StairState.ALWAYS);
									}
									else
									{
										block.SetStairState(Def.StairState.RAMP_ALWAYS);
									}
								}
								break;
							case Def.StairState.RAMP_POSSIBLE:
								{
									ushort rng = (ushort)(UnityEngine.Random.value * 10000f);
									if (rng > layer.StairBlockChance)
										continue;
									block.SetStairState(Def.StairState.RAMP_ALWAYS);
								}
								break;
						}
						if(block.GetProp() != null)
						{
							block.GetProp().enabled = true;
						}
						if(block.GetMonster() != null)
						{
							block.GetMonster().enabled = true;
						}
					}
				}
			}

			// Structure merge
			// - find colliding structures
			// - manage merge, only one of them has a pilar in the colliding positions
			var cpyStrucs = new List<CStrucEdit>(strucs);
			while(cpyStrucs.Count > 0)
			{
				var testStruc = cpyStrucs[0];
				for(int i = 1; i < cpyStrucs.Count; ++i)
				{
					var overlapping = cpyStrucs[i];
					if (!testStruc.GetBounds().Overlaps(overlapping.GetBounds()))
						continue;
					Debug.LogWarning("Overlapping not handled!");
				}
				cpyStrucs.RemoveAt(0);
			}

			// !! CStrucEdit conversion to CStruc

			// What CStrucEdit belong to each CStruc
			var strucIDict = new Dictionary<int, List<CStrucEdit>>(strucs.Count);

			RectInt ComputeRect(CStrucEdit struc)
			{
				int minX = int.MaxValue, minY = int.MaxValue,
					maxX = int.MinValue, maxY = int.MinValue;

				for(int i = 0; i < struc.GetPilars().Length; ++i)
				{
					var pilar = struc.GetPilars()[i];
					if (pilar == null || (pilar != null &&
							(pilar.GetBlocks().Count == 0 ||
							(pilar.GetBlocks().Count == 1 && pilar.GetBlocks()[0].GetLayer() == 0))))
						continue;

					var pos = struc.VPosFromPilarID(i);
					if (pos.x < minX)
						minX = pos.x;
					if (pos.x > maxX)
						maxX = pos.x;
					if (pos.y < minY)
						minY = pos.y;
					if (pos.y > maxY)
						maxY = pos.y;
				}
				
				var strucPos = GameUtils.TransformPosition(
					new Vector2(struc.transform.position.x, struc.transform.position.z));
				return new RectInt(strucPos + new Vector2Int(minX, minY),
					new Vector2Int((maxX - minX) + 1, (maxY - minY) + 1));
			}

			for(int i = 0; i < strucs.Count; ++i)
			{
				var struc = strucs[i];
				var strucRect = ComputeRect(struc);
				var strucIDs = m_Current.GetStrucIDRect(strucRect);
				for(int j = 0; j < strucIDs.Count; ++j)
				{
					var strucID = strucIDs[j];
					if(!strucIDict.ContainsKey(strucID))
					{
						strucIDict.Add(strucID, new List<CStrucEdit>() { struc });
						m_Current.m_Structures.Add(strucID, CStruc.CreateFromMap(strucID));
					}
					else
					{
						if (!strucIDict[strucID].Contains(struc))
							strucIDict[strucID].Add(struc);
					}
				}
			}
			
			for (int i = 0; i < strucIDict.Count; ++i)
			{
				var pair = strucIDict.ElementAt(i);
				var struc = m_Current.m_Structures[pair.Key];
				
				struc.AssignPilars(pair.Value);
			}
			for(int i = 0; i < strucs.Count; ++i)
			{
				strucs[i].DestroyStruc(true);
			}
		}

		void _GetStrucsRadius(Vector2Int center, int radius, ref List<CStruc> strucs)
		{
			const float mult = 0.70710678118654f;
			const float diagAdv = ((CStruc.Width / 2) * mult + (CStruc.Height / 2) * mult) * 0.5f;

			CStruc struc;
			// left to right
			int maxX = center.x + radius;
			for (int x = center.x - radius; x <= maxX;)
			{
				struc = GetStrucAt(new Vector2Int(x, center.y));
				if (struc != null && !strucs.Contains(struc))
					strucs.Add(struc);

				if (x == maxX)
					break;

				x += CStruc.Width / 2;
				if (x > maxX)
					x = maxX;
			}
			// bottom to top
			int maxY = center.y + radius;
			for (int y = center.y - radius; y <= maxY;)
			{
				struc = GetStrucAt(new Vector2Int(center.x, y));
				if (struc != null && !strucs.Contains(struc))
					strucs.Add(struc);

				if (y == maxY)
					break;

				y += CStruc.Height / 2;
				if (y > maxY)
					y = maxY;
			}
			// Bottom left to top right
			float angularRadius = mult * radius;
			maxX = Mathf.CeilToInt(center.x + angularRadius);
			maxY = Mathf.CeilToInt(center.y + angularRadius);
			int advance = Mathf.FloorToInt(diagAdv);
			for (int x = Mathf.CeilToInt(center.x - angularRadius),
					y = Mathf.CeilToInt(center.y - angularRadius);
					x <= maxX && y <= maxY;)
			{
				struc = GetStrucAt(new Vector2Int(x, y));
				if (struc != null && !strucs.Contains(struc))
					strucs.Add(struc);

				if (x == maxX || y == maxY)
					break;

				x += advance;
				y += advance;
				if (x > maxX || y > maxY)
				{
					x = maxX;
					y = maxY;
				}
			}
			// top left to bottom right
			maxY = Mathf.CeilToInt(center.y - angularRadius);
			for (int x = Mathf.CeilToInt(center.x - angularRadius),
				y = Mathf.CeilToInt(center.y + angularRadius);
				x <= maxX && y >= maxY;)
			{
				struc = GetStrucAt(new Vector2Int(x, y));
				if (struc != null && !strucs.Contains(struc))
					strucs.Add(struc);

				if (x == maxX || y == maxY)
					break;

				x += advance;
				y -= advance;
				if (x > maxX || y < maxY)
				{
					x = maxX;
					y = maxY;
				}
			}
		}
		void _GetStrucsRect(RectInt rect, ref List<CStruc> strucs)
		{
			CStruc struc;

			int maxY = rect.y + rect.height - 1;
			int maxX = rect.x + rect.width - 1;
			for (int y = rect.y; y <= maxY;)
			{
				for (int x = rect.x; x <= maxX;)
				{
					struc = GetStrucAt(new Vector2Int(x, y));
					if (struc != null && !strucs.Contains(struc))
						strucs.Add(struc);

					if (x == maxX)
						break;

					x += CStruc.Width / 2;
					if (x > maxX)
						x = maxX;
				}

				if (y == maxY)
					break;

				y += CStruc.Height / 2;
				if (y > maxY)
					y = maxY;
			}
		}
		public CStruc GetStrucFromStrucID(int strucID)
		{
			if (m_Structures.ContainsKey(strucID))
				return m_Structures[strucID];
			return null;
		}
		public CStruc GetStrucFromPilarID(int pilarID)
		{
			return GetStrucFromStrucID(StrucIDFromPilarID(pilarID));
		}
		public CStruc GetStrucAt(Vector2Int pos)
		{
			if (pos.x < 0 || pos.x >= Width || pos.y < 0 || pos.y > Height)
				return null;
			return GetStrucFromStrucID(StrucIDFromPilarPos(pos));
		}
		public CStruc GetStrucAt(Vector2 pos)
		{
			return GetStrucAt(GameUtils.TransformPosition(pos));
		}
		public void GetStrucsRange(Vector2Int center, int radius, ref List<CStruc> strucs)
		{
			_GetStrucsRadius(center, radius, ref strucs);
		}
		public void GetStrucsRange(Vector2 center, float radius, ref List<CStruc> strucs)
		{
			_GetStrucsRadius(GameUtils.TransformPosition(center), Mathf.FloorToInt(radius), ref strucs);
		}
		public List<CStruc> GetStrucsRange(Vector2Int center, int radius)
		{
			var strucs = new List<CStruc>(
				Mathf.CeilToInt(radius / CStruc.Width) * 2 +
				Mathf.CeilToInt(radius / CStruc.Height) * 2);

			_GetStrucsRadius(center, radius, ref strucs);

			return strucs;
		}
		public List<CStruc> GetStrucsRange(Vector2 center, float radius)
		{
			return GetStrucsRange(GameUtils.TransformPosition(center), Mathf.FloorToInt(radius));
		}
		public void GetStrucsRect(RectInt rect, ref List<CStruc> strucs)
		{
			_GetStrucsRect(rect, ref strucs);
		}
		public void GetStrucsRect(Rect rect, ref List<CStruc> strucs)
		{
			_GetStrucsRect(
				new RectInt(GameUtils.TransformPosition(rect.position), 
				new Vector2Int(Mathf.FloorToInt(rect.size.x), Mathf.FloorToInt(rect.size.y))),
				ref strucs);
		}
		public List<CStruc> GetStrucsRect(RectInt rect)
		{
			var strucs = new List<CStruc>(
				Mathf.CeilToInt((float)rect.width / CStruc.Width) * 2 +
				Mathf.CeilToInt((float)rect.height / CStruc.Height) * 2);

			_GetStrucsRect(rect, ref strucs);

			return strucs;
		}
		public List<CStruc> GetStrucsRect(Rect rect)
		{
			return GetStrucsRect(new RectInt(GameUtils.TransformPosition(rect.position),
				new Vector2Int(Mathf.FloorToInt(rect.size.x), Mathf.FloorToInt(rect.size.y))));
		}
		public CPilar GetPilarAt(Vector2Int pos)
		{
			var struc = GetStrucAt(pos);
			if (struc == null)
				return null;
			var bounds = struc.GetBounds();
			var pilarStrucPos = pos - bounds.position;
			var id = struc.PilarIDFromVPos(pilarStrucPos);
			return struc.GetPilars()[id];
		}
		public CPilar GetPilarAt(Vector2 pos)
		{
			return GetPilarAt(GameUtils.TransformPosition(pos));
		}
		public CBlock GetBlockAt(Vector3 pos)
		{
			var pilar = GetPilarAt(new Vector2(pos.x, pos.z));
			if (pilar != null)
				return pilar.GetClosestBlock(pos.y) as CBlock;
			return null;
		}
		public void DestroyMap()
		{
			if (m_Current == this)
				m_Current = null;
			for(int i = 0; i < m_Structures.Count; ++i)
			{
				var struc = m_Structures.ElementAt(i).Value;
				struc.DestroyStruc(false);
			}
			GameUtils.DeleteGameobject(gameObject);
		}
		public void ForEachStruc(Action<CStruc> fn)
		{
			for(int i = 0; i < m_Structures.Count; ++i)
			{
				var pair = m_Structures.ElementAt(i);
				fn(pair.Value);
			}
		}
	}
}
