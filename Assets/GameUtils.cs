/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

namespace Assets
{
	public static class GameUtils
	{
		const float Angle90 = 90f * Mathf.Deg2Rad;
		const float Angle180 = 180f * Mathf.Deg2Rad;
		const float Angle270 = 270f * Mathf.Deg2Rad;
		public const float Gravity = -9.81f;
		//        public static float RadToDeg(float rad) { return rad * (180.0f / Mathf.PI); }

		//        public static float AngleFromAtan(Vector2 v)
		//        {
		//            if(v.x == 0.0f)
		//            {
		//                return (v.y > 0.0f) ? 90.0f : (v.y == 0.0f) ? 0.0f : 270.0f;
		//            }
		//            else if(v.y == 0.0f)
		//            {
		//                return (v.x >= 0.0f) ? 0.0f : 180.0f;
		//            }
		//            float rtn = RadToDeg(Mathf.Atan2(v.y, v.x));
		//            if ((v.x < 0.0f && v.y < 0.0f) || (v.x < 0.0f))
		//                rtn = 180.0f + rtn;
		//            else if (v.y < 0.0f)
		//                rtn = 270.0f + (90.0f + rtn);
		//            return rtn;
		//        }
		public static bool RayTestEntityToEntity(AI.CLivingEntity caster, AI.CLivingEntity target, float maxDistance = 100f)
		{
			var casterHead = caster.transform.position + new Vector3(0f, caster.GetHeight() * 0.9f, 0f);
			var targetHead = target.transform.position + new Vector3(0f, target.GetHeight() * 0.9f, 0f);
			var headHeadDir = (targetHead - casterHead).normalized;
			var headRay = new Ray(casterHead, headHeadDir);
			caster.GetCollider().enabled = false;
			int layer = 1 << Def.RCLayerBlock | 1 << Def.RCLayerBridge | 1 << Def.RCLayerLE;
			bool headTest = Physics.Raycast(headRay, out RaycastHit headHit, maxDistance, layer);
			if (headTest)
			{
				if (headHit.collider.gameObject.layer == Def.RCLayerLE)
				{
					if (headHit.collider.gameObject == target.gameObject)
					{
						//Debug.Log("Head Hit!");
						caster.GetCollider().enabled = true;
						return true;
					}
				}
				//Debug.Log("Head collided with " + headHit.collider.gameObject.name);
			}
			//Debug.DrawLine(casterHead, targetHead, Color.red);
			var targetMid = target.transform.position + new Vector3(0f, target.GetHeight() * 0.5f, 0f);
			var headMidDir = (targetMid - casterHead).normalized;
			var midRay = new Ray(casterHead, headMidDir);
			bool midTest = Physics.Raycast(midRay, out RaycastHit midHit, maxDistance, layer);
			if (midTest)
			{
				if (midHit.collider.gameObject.layer == Def.RCLayerLE)
				{
					if (midHit.collider.gameObject == target.gameObject)
					{
						//Debug.Log("Mid Hit!");
						caster.GetCollider().enabled = true;
						//Debug.Break();
						return true;
					}
				}
				//Debug.Log("Mid collided with " + headHit.collider.gameObject.name);
			}
			caster.GetCollider().enabled = true;
			//Debug.DrawLine(casterHead, targetMid, Color.green);
			//Debug.Break();
			return false;
		}
		public static float _AngleBetween2D(Vector2 a, Vector2 b)
		{
			float upperPart = a.x * b.x + a.y * b.y;
			float bottomPart = Mathf.Sqrt((a.x * a.x + a.y * a.y) * (b.x * b.x + b.y * b.y));
			return Mathf.Acos(upperPart / bottomPart);
		}
		public static float AngleBetween2D(Vector2 a, Vector2 b)
		{
			float cA = b.y * a.x - b.x * a.y;
			float cB = b.x * a.x + b.y * a.y;
			return Mathf.Atan2(cA, cB);
		}
		public static float AngleFromDir2D(Vector2 dir)
		{
			float sin = Mathf.Asin(-dir.x);
			float cos = Mathf.Acos(dir.y);
			if (sin < 0f)
			{
				if (cos > Angle90)
					return Angle180 - cos + Angle180;
				else if (cos == Angle90)
					return Angle270;
				else if (cos < Angle90)
					return Angle90 - cos + Angle270;
			}
			return cos;
		}
		public static Vector2 DirFromAngle2D(float angle)
		{
			return new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));
		}
		public static Vector2Int TransformPosition(Vector2 wPos)
		{
			//const float blockSep = 1f + Def.BlockSeparation;
			//const float invSep = 1f / blockSep;
			//var pos = wPos * invSep;
			//var vPos = new Vector2Int((int)pos.x, (int)pos.y);
			//return vPos;
			const float blockSep = 1f + Def.BlockSeparation;
			const float invSep = 1f / blockSep;
			var pos = wPos * invSep;
			var pox0 = Mathf.FloorToInt(pos.x);
			//var pox1 = Mathf.RoundToInt(pos.x);
			//var pox2 = Mathf.CeilToInt(pos.x);
			//var pox3 = (int)pos.x;
			var poy0 = Mathf.FloorToInt(pos.y);
			//var poy1 = Mathf.RoundToInt(pos.y);
			//var poy2 = Mathf.CeilToInt(pos.y);
			//var poy3 = (int)pos.y;
			var vPos = new Vector2Int(pox0, poy0);
			return vPos;
		}
		public static Vector2 TransformPosition(Vector2Int vPos)
		{
			const float blockSep = 1f + Def.BlockSeparation;
			var wPos = new Vector2(vPos.x * blockSep, vPos.y * blockSep);
			return wPos;
		}
		public static int Mod(int a, int b) => ((a %= b) < 0) ? a + b : a;
		public static float Mod(float a, float b) => ((a %= b) < 0f) ? a + b : a;
		public static string CleanUniqueName(string uniqueName)
		{
			if (uniqueName.Length == 0)
				return uniqueName;
			var name = uniqueName;

			int last = 1;
			while (name[name.Length - last] != '_') ++last;

			//while (!char.IsLetter(name[name.Length - last]) && last < name.Length)
			//	++last;

			name = name.Substring(0, (name.Length - last)/* + 1*/);

			return name;
		}
		public static string GenerateUniqueName(string baseName, int zeroCount, Func<string, bool> check, bool baseNameValid = false)
		{
			if(baseNameValid && check(baseName))
			{
				return baseName;
			}
			int nameID = 0;
			string testName = baseName + '_' + (nameID++).ToString().PadLeft(zeroCount, '0');
			while(!check(testName))
				testName = baseName + '_' + (nameID++).ToString().PadLeft(zeroCount, '0');
			return testName;
		}
		public static bool IsStairPossible(Def.StairState state)
		{
			switch (state)
			{
				case Def.StairState.POSSIBLE:
				case Def.StairState.STAIR_OR_RAMP:
				case Def.StairState.RAMP_POSSIBLE:
					return true;
			}
			return false;
		}
		public static bool IsValidBiomeLayer(Def.BiomeLayerType biomeLayer)
		{
			switch(biomeLayer)
			{
				case Def.BiomeLayerType.OTHER:
				case Def.BiomeLayerType.FULLVOID:
				case Def.BiomeLayerType.COUNT:
					return false;
				default:
					return true;
			}
		}
		static void SortMerge<T>(List<T> origList, int begin, int middle, int end, List<T> cpyList, Action<int, int> onChangeIdx) where T : IComparable
		{
			int i = begin, j = middle;
			for(int k = begin; k < end; ++k)
			{

				if(i < middle && (j >= end || cpyList[i].CompareTo(cpyList[j]) <= 0))//  cpyList[i] <= cpyList[j]
				{
					origList[k] = cpyList[i];
					onChangeIdx(i, k);
					++i;
				}
				else
				{
					origList[k] = cpyList[j];
					onChangeIdx(k, j);
					++j;
				}
			}
		}
		static void SortSplitMerge<T>(List<T> origList, int begin, int end, List<T> cpyList, Action<int, int> onChangeIdx) where T : IComparable
		{
			if ((end - begin) < 2)
				return;

			int middle = (end + begin) / 2;

			SortSplitMerge(origList, begin, middle, cpyList, onChangeIdx);
			SortSplitMerge(origList, middle, end, cpyList, onChangeIdx);
			SortMerge(origList, begin, middle, end, cpyList, onChangeIdx);
		}
		public static void Sort<T>(List<T> list, Action<int, int> onChangeIdx) where T : IComparable
		{
			if (list == null || (list != null && list.Count < 2))
				return;

			var cpyList = new List<T>(list);
			SortSplitMerge(list, 0, list.Count, cpyList, onChangeIdx);
		}

		//        //public static int MapIDFromPosition(Vector2 position, int mapWidth = Manager.MapWidth, int mapHeight = Manager.MapHeight, float blockSeparation = StructureComponent.Separation)
		//        //{
		//        //    var blockSep = 1.0f + blockSeparation;
		//        //    if (position.x >= (mapWidth * blockSep) || position.y >= (mapHeight * blockSep))
		//        //        return -1;
		//        //    float sep = 1.0f / blockSep;
		//        //    var nPos = position * sep;
		//        //    var pos = new Vector2Int(Mathf.FloorToInt(nPos.x), Mathf.FloorToInt(nPos.y));
		//        //    return IDFromPos(pos, mapWidth, mapHeight);
		//        //}

		//        //public static Vector2 PositionFromMapID(int mapID, int mapWidth = Manager.MapWidth, int mapHeight = Manager.MapHeight, float blockSeparation = StructureComponent.Separation)
		//        //{
		//        //    if (mapID > (mapHeight * mapWidth))
		//        //        return Vector2.zero;
		//        //    var blockSep = 1.0f + blockSeparation;
		//        //    var pos = PosFromID(mapID, mapWidth, mapHeight);
		//        //    var mapPos = new Vector2(pos.x * blockSep, pos.y * blockSep);
		//        //    return mapPos;
		//        //}
		static List<CBlock> JumpableBlocks;
		public static CBlock GetJumpableBlock(Vector3 oddPos, Collider[] colliders, int count)
		{
			if (count == 0 || colliders == null || colliders.Length == 0)
				return null;

			if (JumpableBlocks == null || JumpableBlocks.Capacity < count)
				JumpableBlocks = new List<CBlock>(count);
			JumpableBlocks.Clear();

			bool isJumpable(CBlock block)
			{
				if (block.GetStackedAbove() != null)
					return false;

				float addition = 0f;
				if(block.GetBlockType() == Def.BlockType.STAIRS)
				{
					switch (block.GetRotation())
					{
						case Def.RotationState.Default:
							if (oddPos.x < block.transform.position.x)
								addition = 0.5f;
							break;
						case Def.RotationState.Right:
							if (oddPos.z < block.transform.position.z)
								addition = 0.5f;
							break;
						case Def.RotationState.Half:
							if (oddPos.x > (block.transform.position.x + 1f + Def.BlockSeparation))
								addition = 0.5f;
							break;
						case Def.RotationState.Left:
							if (oddPos.z > (block.transform.position.z + 1f + Def.BlockSeparation))
								addition = 0.5f;
							break;
					}
				}
				float heightDiff = (block.transform.position.y + addition) - oddPos.y;
				if(heightDiff > 0.5f && heightDiff < 0.75f)
				{
					return true;
				}
				return false;
			}

			for (int i = 0; i < count; ++i)
			{
				var col = colliders[i];
				if (col.gameObject.TryGetComponent(out CBlock block))
				{
					if (isJumpable(block))
						JumpableBlocks.Add(block);
				}
				else if(col.transform.parent.TryGetComponent(out block))
				{
					if (isJumpable(block))
						JumpableBlocks.Add(block);
				}
			}
			if(JumpableBlocks.Count == 0)
				return null;
			return JumpableBlocks[0];
		}
		public static bool IsNearlyEqual(float a, float b, float tolerance = 0.001f)
		{
			return Mathf.Abs(a - b) <= tolerance;
		}
		public static bool IsNearlyEqual(Vector2 a, Vector2 b, float tolerance = 0.001f)
		{
			return IsNearlyEqual(a.x, b.x, tolerance)
				&& IsNearlyEqual(a.y, b.y, tolerance);
		}
		public static bool IsNearlyEqual(Vector3 a, Vector3 b, float tolerance = 0.001f)
		{
			return IsNearlyEqual(a.x, b.x, tolerance)
				&& IsNearlyEqual(a.y, b.y, tolerance)
				&& IsNearlyEqual(a.z, b.z, tolerance);
		}

		//        public static void FixSideAnts(CBlockEdit block, AntTopDirection direction)
		//        {
		//            AntComponent ant0, ant1;
		//            switch (direction)
		//            {
		//                case AntTopDirection.SOUTH_EAST:
		//                    ant0 = block.GetAnts()[(int)Def.DecoPosition.EAST];
		//                    if (ant0 != null)
		//                        ant0.Renderer.flipX = !ant0.Renderer.flipX;
		//                    break;
		//                case AntTopDirection.NORTH_WEST:
		//                    ant0 = block.GetAnts()[(int)Def.DecoPosition.NORTH];
		//                    if (ant0 != null)
		//                        ant0.Renderer.flipX = !ant0.Renderer.flipX;
		//                    break;
		//                case AntTopDirection.NORTH_EAST:
		//                    ant0 = block.GetAnts()[(int)Def.DecoPosition.NORTH];
		//                    if (ant0 != null)
		//                        ant0.Renderer.flipX = !ant0.Renderer.flipX;
		//                    ant1 = block.GetAnts()[(int)Def.DecoPosition.EAST];
		//                    if (ant1 != null)
		//                        ant1.Renderer.flipX = !ant1.Renderer.flipX;
		//                    break;
		//                case AntTopDirection.WEST_NORTH:
		//                    ant0 = block.GetAnts()[(int)Def.DecoPosition.WEST];
		//                    if (ant0 != null)
		//                        ant0.Renderer.flipX = !ant0.Renderer.flipX;
		//                    break;
		//                case AntTopDirection.WEST_SOUTH:
		//                    ant0 = block.GetAnts()[(int)Def.DecoPosition.WEST];
		//                    if (ant0 != null)
		//                        ant0.Renderer.flipX = !ant0.Renderer.flipX;
		//                    ant1 = block.GetAnts()[(int)Def.DecoPosition.SOUTH];
		//                    if (ant1 != null)
		//                        ant1.Renderer.flipX = !ant1.Renderer.flipX;
		//                    break;
		//                case AntTopDirection.EAST_SOUTH:
		//                    ant0 = block.GetAnts()[(int)Def.DecoPosition.SOUTH];
		//                    if (ant0 != null)
		//                        ant0.Renderer.flipX = !ant0.Renderer.flipX;
		//                    break;
		//            }
		//        }

		//        //public static bool TargetReached(Vector2 distance, Vector2 origDir, Vector2 newDir)
		//        //{
		//        //    if (distance.x > 1.0f || distance.y > 1.0f)
		//        //        return false;

		//        //    if (IsNearlyEqual(Vector2.Dot(origDir, newDir), -1.0f))
		//        //        return true;
		//        //    //if(origDir.x != 0.0f && newDir.x != 0.0f)
		//        //    //{
		//        //    //    if (Mathf.Sign(origDir.x) != Mathf.Sign(newDir.x))
		//        //    //        return true;
		//        //    //}

		//        //    //if (origDir.y != 0.0f && newDir.y != 0.0f)
		//        //    //{
		//        //    //    if (Mathf.Sign(origDir.y) != Mathf.Sign(newDir.y))
		//        //    //        return true;
		//        //    //}

		//        //    return false;
		//        //}

		//        //public static void UpdateChances(ref List<float> chances)
		//        //{
		//        //    if (chances.Count == 0)
		//        //        return;
		//        //    if(chances.Count == 1)
		//        //    {
		//        //        chances[0] = 1.0f;
		//        //        return;
		//        //    }

		//        //    float amount = 0.0f;
		//        //    for (int i = 0; i < chances.Count; ++i)
		//        //        amount += chances[i];

		//        //    if (IsNearlyEqual(amount, 1.0f))
		//        //        return;
		//        //    amount = 1.0f / amount;
		//        //    for (int i = 0; i < chances.Count; ++i)
		//        //    {
		//        //        chances[i] *= amount;
		//        //    }
		//        //}

		//public static void UpdateChances2(ref List<IDChance> chances)
		//{
		//    if (chances.Count == 0)
		//        return;
		//    if (chances.Count == 1)
		//    {
		//        var c = chances[0];
		//        c.Chance = 10000;
		//        chances[0] = c;
		//        return;
		//    }

		//    int amount = 0;
		//    for (int i = 0; i < chances.Count; ++i)
		//        amount += chances[i].Chance;

		//    if (amount == 10000)
		//        return;
		//    int remaining = 10000 - amount;
		//    int sign = remaining >= 0 ? 1 : -1;
		//    var times = remaining / chances.Count;
		//    for (int j = 0; j < chances.Count; ++j)
		//    {
		//        var c = chances[j];
		//        if (c.Chance < (times * sign))
		//        {
		//            remaining -= (sign * c.Chance);
		//            c.Chance = 0;
		//        }
		//        else
		//        {
		//            c.Chance = (ushort)(c.Chance + times);
		//            remaining -= times;
		//        }
		//        chances[j] = c;
		//    }
		//    while (remaining != 0)
		//    {
		//        int idx = UnityEngine.Random.Range(0, chances.Count);
		//        var c = chances[idx];
		//        if (c.Chance == 0 && sign == -1)
		//            continue;
		//        c.Chance = (ushort)(c.Chance + sign);
		//        chances[idx] = c;
		//        remaining -= sign;
		//    }
		//}
		//public static void UpdateChances2(ref List<NameChance> chances)
		//{
		//    if (chances.Count == 0)
		//        return;
		//    if (chances.Count == 1)
		//    {
		//        var c = chances[0];
		//        c.Chance = 10000;
		//        chances[0] = c;
		//        return;
		//    }

		//    int amount = 0;
		//    for (int i = 0; i < chances.Count; ++i)
		//        amount += chances[i].Chance;

		//    if (amount == 10000)
		//        return;
		//    int remaining = 10000 - amount;
		//    int sign = remaining >= 0 ? 1 : -1;
		//    var times = remaining / chances.Count;
		//    for (int j = 0; j < chances.Count; ++j)
		//    {
		//        var c = chances[j];
		//        if (c.Chance < times)
		//        {
		//            remaining -= (sign * c.Chance);
		//            c.Chance = 0;
		//        }
		//        else
		//        {
		//            c.Chance = (ushort)(c.Chance + times);
		//            remaining -= times;
		//        }
		//        chances[j] = c;
		//    }
		//    while (remaining != 0)
		//    {
		//        int idx = UnityEngine.Random.Range(0, chances.Count);
		//        var c = chances[idx];
		//        if (c.Chance == 0 && sign == -1)
		//            continue;
		//        c.Chance = (ushort)(c.Chance + sign);
		//        chances[idx] = c;
		//        remaining -= sign;
		//    }
		//}
		public static void UpdateChances2(ref List<IDChance> chances, System.Random rng, int protectedIdx = -1)
		{
			if (chances.Count == 0)
				return;
			if (chances.Count == 1)
			{
				var c = chances[0];
				c.Chance = 10000;
				chances[0] = c;
				return;
			}

			int SumChance(List<IDChance> list)
			{
				int sum = 0;
				for (int i = 0; i < list.Count; ++i)
					sum += list[i].Chance;
				return sum;
			}

			int amount = SumChance(chances);

			if (amount == 10000)
				return;

			int nAmount = 0;
			for (int i = 0; i < chances.Count; ++i)
			{
				if (i == protectedIdx)
				{
					nAmount += chances[i].Chance;
					continue;
				}
				var prob = (float)chances[i].Chance;
				var nProb = 10000f * prob / (float)amount;
				var nChance = (ushort)Mathf.FloorToInt(nProb);
				chances[i] = new IDChance()
				{
					ID = chances[i].ID,
					Chance = nChance
				};
				nAmount += nChance;
			}
			int remaining = 10000 - nAmount;
			while (remaining > 0)
			{
				if (protectedIdx >= 0 && chances[protectedIdx].Chance == remaining)
					break;
				int idx = rng.Next(0, chances.Count); // UnityEngine.Random.Range(0, chances.Count);
				if (idx == protectedIdx)
					continue;
				chances[idx] = new IDChance()
				{
					ID = chances[idx].ID,
					Chance = (ushort)(chances[idx].Chance + 1)
				};
				--remaining;
			}
			while (remaining < 0)
			{
				int idx = rng.Next(0, chances.Count); //UnityEngine.Random.Range(0, chances.Count);
				if (idx == protectedIdx || chances[idx].Chance == 0)
					continue;
				chances[idx] = new IDChance()
				{
					ID = chances[idx].ID,
					Chance = (ushort)(chances[idx].Chance - 1)
				};
				++remaining;
			}
		}
		public static void UpdateChances2(ref List<IDChance> chances, int protectedIdx = -1)
		{
			if (chances.Count == 0)
				return;
			if (chances.Count == 1)
			{
				var c = chances[0];
				c.Chance = 10000;
				chances[0] = c;
				return;
			}

			int SumChance(List<IDChance> list)
			{
				int sum = 0;
				for (int i = 0; i < list.Count; ++i)
					sum += list[i].Chance;
				return sum;
			}

			int amount = SumChance(chances);

			if (amount == 10000)
				return;

			//int remaining = 10000 - amount;
			int nAmount = 0;
			for (int i = 0; i < chances.Count; ++i)
			{
				if (i == protectedIdx)
				{
					nAmount += chances[i].Chance;
					continue;
				}
				var prob = (float)chances[i].Chance;
				var nProb = 10000f * prob / (float)amount;
				var nChance = (ushort)Mathf.FloorToInt(nProb);
				chances[i] = new IDChance()
				{
					ID = chances[i].ID,
					Chance = nChance
				};
				nAmount += nChance;
			}
			int remaining = 10000 - nAmount;
			while (remaining > 0)
			{
				if (protectedIdx >= 0 && chances[protectedIdx].Chance == remaining)
					break;
				int idx = UnityEngine.Random.Range(0, chances.Count);
				if (idx == protectedIdx)
					continue;
				chances[idx] = new IDChance()
				{
					ID = chances[idx].ID,
					Chance = (ushort)(chances[idx].Chance + 1)
				};
				--remaining;
			}
			while (remaining < 0)
			{
				int idx = UnityEngine.Random.Range(0, chances.Count);
				if (idx == protectedIdx || chances[idx].Chance == 0)
					continue;
				chances[idx] = new IDChance()
				{
					ID = chances[idx].ID,
					Chance = (ushort)(chances[idx].Chance - 1)
				};
				++remaining;
			}

			//int sign = remaining >= 0 ? 1 : -1;
			//var times = remaining / (protectedIdx >= 0 ? chances.Count - 1 : chances.Count);
			//for (int j = 0; j < chances.Count; ++j)
			//{
			//	if (j == protectedIdx)
			//		continue;
			//	var c = chances[j];
			//	if (c.Chance < (times * sign))
			//	{
			//		remaining -= (sign * c.Chance);
			//		c.Chance = 0;
			//	}
			//	else
			//	{
			//		c.Chance = (ushort)(c.Chance + times);
			//		remaining -= times;
			//	}
			//	chances[j] = c;
			//}
			//while (remaining != 0)
			//{
			//	int idx = UnityEngine.Random.Range(0, chances.Count);
			//	if (idx == protectedIdx)
			//		continue;
			//	var c = chances[idx];
			//	if (c.Chance == 0 && sign == -1)
			//		continue;
			//	c.Chance = (ushort)(c.Chance + sign);
			//	chances[idx] = c;
			//	remaining -= sign;
			//}
		}
		public static void UpdateChances2(ref List<NameChance> chances, int protectedIdx = -1)
		{
			if (chances.Count == 0)
				return;
			if (chances.Count == 1)
			{
				var c = chances[0];
				c.Chance = 10000;
				chances[0] = c;
				return;
			}

			int amount = 0;
			for (int i = 0; i < chances.Count; ++i)
				amount += chances[i].Chance;

			if (amount == 10000)
				return;
			int remaining = 10000 - amount;
			int sign = remaining >= 0 ? 1 : -1;
			var times = remaining / (protectedIdx >= 0 ? chances.Count - 1 : chances.Count);
			for (int j = 0; j < chances.Count; ++j)
			{
				if (j == protectedIdx)
					continue;
				var c = chances[j];
				if (c.Chance < times)
				{
					remaining -= (sign * c.Chance);
					c.Chance = 0;
				}
				else
				{
					c.Chance = (ushort)(c.Chance + times);
					remaining -= times;
				}
				chances[j] = c;
			}
			while (remaining != 0)
			{
				int idx = UnityEngine.Random.Range(0, chances.Count);
				if (idx == protectedIdx)
					continue;
				var c = chances[idx];
				if (c.Chance == 0 && sign == -1)
					continue;
				c.Chance = (ushort)(c.Chance + sign);
				chances[idx] = c;
				remaining -= sign;
			}
		}
		public static bool LinearCheck(float origin, float span, float target, float targetSpan)
		{
			float lowDist = Mathf.Abs(target - origin);
			float highDist = Mathf.Abs((target + targetSpan) - origin);
			float highOppositeDist = Mathf.Abs((target - targetSpan) - origin);
			float midDist = Mathf.Abs((target + targetSpan * 0.5f) - origin);
			float midOppositeDist = Mathf.Abs((target - targetSpan * 0.5f) - origin);
			if (lowDist < span || highDist < span || midDist < span || highOppositeDist < span || midOppositeDist < span)
				return true;
			return false;
		}
		public static int GetRandomFromProbs(List<float> probs)
		{
			var rng = UnityEngine.Random.value;
			float accum = 0f;
			for(int i = 0; i < probs.Count; ++i)
			{
				float nextChance = accum + probs[i];
				if (rng >= accum && rng <= nextChance)
					return i;
				accum = nextChance;
			}
			Debug.LogWarning($"Couldnt find a suitable random item, chance'{rng}', count'{probs.Count}' !GameUtils.GetRandomFromProbs");
			return Mathf.FloorToInt(rng * (probs.Count - 1));
		}
		public static bool SphericalCheck(Vector3 origin, float radius, Vector3 target, float targetHeight, float targetRadius)
		{
			bool xCheck = LinearCheck(origin.x, radius, target.x, targetRadius);
			bool yCheck = LinearCheck(origin.y, radius, target.y, targetHeight);
			bool zCheck = LinearCheck(origin.z, radius, target.z, targetRadius);
			return xCheck && yCheck && zCheck;
		}

		public static long MapIDFromPos(Vector2Int pos)
		{
			if(pos.x >= Def.MapEnd || pos.x < Def.MapStart || pos.y >= Def.MapEnd || pos.y < Def.MapStart)
			{
				Debug.LogWarning($"Invalid position({pos.x},{pos.y}) to convert to MapID.");
				return 0;
			}
			var x = (long)pos.x + (long)Def.MapEnd;
			var y = (long)pos.y + (long)Def.MapEnd;

			return y * Def.MapSideSize + x;
		}

		public static Vector2Int PosFromMapID(long mapID)
		{
			const float invMap = 1f / Def.MapSideSize;
			if(mapID < 0 || mapID >= Def.MaxMapID)
			{
				Debug.LogWarning($"Invalid MapID {mapID}, to convert to position.");
				return Vector2Int.zero;
			}
			var x = (mapID % Def.MapSideSize) - Def.MapEnd;
			var y = (long)Mathf.Floor(mapID * invMap) - Def.MapEnd;
			if(x >= int.MaxValue || x <= int.MinValue || y >= int.MaxValue || y <= int.MinValue)
			{
				Debug.LogWarning($"Conversion error from {mapID} to position ({x},{y}).");
				return Vector2Int.zero;
			}
			return new Vector2Int((int)x, (int)y);
		}
		public static bool IDFromPosSafe(Vector2Int pos, int maxX, int maxY, out int id)
		{
			if (pos.x > maxX || pos.y > maxY || pos.x < 0 || pos.y < 0)
			{
				id = -1;
				return false;
			}
			id = pos.y * maxX + pos.x;
			return true;
		}
		public static int IDFromPos(Vector2Int pos, int maxX, int maxY)
		{
			if (pos.x > maxX || pos.y > maxY || pos.x < 0 || pos.y < 0)
			{
				Debug.LogWarning($"CellID from position error, pos:'{pos}', max:'x={maxX},y={maxY}'");
				return 0;
			}

			int cellID = pos.y * maxX + pos.x;

			return cellID;
		}
		public static int IDFromPosUnsafe(Vector2Int pos, int maxX, int maxY)
		{
			return pos.y * maxX + pos.x;
		}
		public static Vector2Int PosFromID(int id, int maxX, int maxY)
		{
			Vector2Int pos = new Vector2Int(id % maxX, Mathf.FloorToInt(id / (float)maxY));

			if (pos.x > maxX || pos.y > maxY || pos.x < 0 || pos.y < 0)
			{
				Debug.LogWarning($"Position from CellID overflow, pos:'{pos}', max:'x={maxX},y={maxY}'");
				return Vector2Int.zero;
			}

			return pos;
		}
		public static Vector2Int PosFromIDUnsafe(int id, int maxX, int maxY)
		{
			return new Vector2Int(id % maxX, Mathf.FloorToInt(id / (float)maxY));
		}
		public static int DistanceMod(int a, int b, int mod)
		{
			int distNorm = Mathf.Abs(a - b);
			int distModA = Mathf.Abs((a + mod) - b);
			int distModB = Mathf.Abs(a - (b + mod));

			return Mathf.Min(distNorm, distModA, distModB);
		}
		public static float DistanceMod(float a, float b, float mod)
		{
			float distNorm = Mathf.Abs(a - b);
			float distModA = Mathf.Abs((a + mod) - b);
			float distModB = Mathf.Abs(a - (b + mod));

			return Mathf.Min(distNorm, distModA, distModB);
		}
		public static float DistanceMod(Vector2 a, Vector2 b, Vector2 mod)
		{
			float distModXXXX = Vector2.Distance(new Vector2(a.x, a.y), new Vector2(b.x, b.y));
			float distModXXXB = Vector2.Distance(new Vector2(a.x, a.y), new Vector2(b.x, b.y + mod.y));
			float distModXXBX = Vector2.Distance(new Vector2(a.x, a.y), new Vector2(b.x + mod.x, b.y));
			float distModXXBB = Vector2.Distance(new Vector2(a.x, a.y), new Vector2(b.x + mod.x, b.y + mod.y));

			float distModAXXX = Vector2.Distance(new Vector2(a.x + mod.x, a.y), new Vector2(b.x, b.y));
			float distModXAXX = Vector2.Distance(new Vector2(a.x, a.y + mod.y), new Vector2(b.x, b.y));
			float distModAAXX = Vector2.Distance(new Vector2(a.x + mod.x, a.y + mod.y), new Vector2(b.x, b.y));

			return Mathf.Min(distModXXXX, distModXXXB, distModXXBX, distModXXBB, distModAXXX, distModXAXX, distModAAXX);
		}
		public static float DistanceMod(Vector2Int a, Vector2Int b, Vector2Int mod)
		{
			float distModXXXX = Vector2Int.Distance(new Vector2Int(a.x, a.y), new Vector2Int(b.x, b.y));
			float distModXXXB = Vector2Int.Distance(new Vector2Int(a.x, a.y), new Vector2Int(b.x, b.y + mod.y));
			float distModXXBX = Vector2Int.Distance(new Vector2Int(a.x, a.y), new Vector2Int(b.x + mod.x, b.y));
			float distModXXBB = Vector2Int.Distance(new Vector2Int(a.x, a.y), new Vector2Int(b.x + mod.x, b.y + mod.y));

			float distModAXXX = Vector2Int.Distance(new Vector2Int(a.x + mod.x, a.y), new Vector2Int(b.x, b.y));
			float distModXAXX = Vector2Int.Distance(new Vector2Int(a.x, a.y + mod.y), new Vector2Int(b.x, b.y));
			float distModAAXX = Vector2Int.Distance(new Vector2Int(a.x + mod.x, a.y + mod.y), new Vector2Int(b.x, b.y));

			return Mathf.Min(distModXXXX, distModXXXB, distModXXBX, distModXXBB, distModAXXX, distModXAXX, distModAAXX);
		}
		public static float SinMovement1D(float min, float max, float speed, float time)
		{
			float A = (max - min) * 0.5f;
			return A * Mathf.Sin(time / speed) + A;
		}

		//        public static float BridgeYPosition(float bridgeLength, float distanceFromInit, float initialHeight, float finalHeight, out float linearHeight)
		//        {
		//            if (distanceFromInit > bridgeLength)
		//                throw new Exception();
		//            // distance from 0 to 1
		//            float distEncoded = distanceFromInit / bridgeLength;

		//            // Lerp
		//            linearHeight = initialHeight * (1.0f - distEncoded) + distEncoded * finalHeight;

		//            // arc from pi to 2pi, total angle pi
		//            float angleEncoded = distEncoded * Mathf.PI; // 0 - PI
		//            // we want the lower part of the sine
		//            float angle = Mathf.Sin(Mathf.PI + angleEncoded);

		//            if (bridgeLength > 3.0f)
		//                return linearHeight + angle;
		//            else
		//                return linearHeight;
		//        }

		public static float LinearMovement1D(float origin, float target, float deltaSpeed)
		{
			float dist = target - origin;
			float dir = dist > 0.0f ? 1.0f : dist < 0.0f ? -1.0f : 0.0f;
			float destination = origin + (dir * deltaSpeed);
			float movDist = destination - origin;
			if (Mathf.Abs(dist) < Mathf.Abs(movDist))
				destination = target;
			return destination - origin;
		}

		public static Vector2 LinearMovement2D(Vector2 origin, Vector2 target, float deltaSpeed)
		{
			var dir = (target - origin).normalized;
			Vector2 destination = new Vector2(origin.x + (dir.x * deltaSpeed), origin.y + (dir.y * deltaSpeed));
			var dist = Vector2.Distance(origin, target);
			var movDist = Vector2.Distance(origin, destination);
			if (Mathf.Abs(dist) < Mathf.Abs(movDist))
				destination = target;
			return destination - origin;
		}

		public static float GetStairYOffset(Vector3 stairPilarPosition, Def.RotationState stairRotation, 
			Vector3 currentPosition)
		{
			float stairOffset = 0.0f;
			var offset = currentPosition - stairPilarPosition;
			switch (stairRotation)
			{
				case Def.RotationState.Default:
					if (offset.x < 0.5f)
						stairOffset = 0.5f;
					break;
				case Def.RotationState.Left:
					if (offset.z > 0.5f)
						stairOffset = 0.5f;
					break;
				case Def.RotationState.Right:
					if (offset.z < 0.5f)
						stairOffset = 0.5f;
					break;
				case Def.RotationState.Half:
					if (offset.x > 0.5f)
						stairOffset = 0.5f;
					break;
			}
			return stairOffset;
		}

		public static bool CanGoThere(float currentY, Vector2 NextPosition, float maxJump, bool avoidVoid)
		{
			return false;
//            var nextMapID = MapIDFromPosition(NextPosition, Manager.MapWidth, Manager.MapHeight, Def.BlockSeparation);
//            var nextPilar = Manager.Mgr.Pilars[nextMapID];
//            if(nextPilar.Bridge != null)
//            {
//                var nextMapPos = PositionFromMapID(nextMapID);
//                var inPilarOffset = NextPosition - nextMapPos;
//                var bridgeHeight = nextPilar.Bridge.GetHeight(inPilarOffset);
//                if (bridgeHeight < (currentY + maxJump))
//                    return true;

//                //var bridgeHeightDiff = Mathf.Abs(currentY - bridgeHeight);
//                //if(bridgeHeightDiff <= maxJump)
//                //{
//                //    return true;
//                //}
//            }
//            IBlock nextBlock = null;
//            {
//                float dist = float.PositiveInfinity;
//                for (int i = 0; i < nextPilar.Blocks.Count; ++i)
//                {
//                    var block = nextPilar.Blocks[i];
//                    if (avoidVoid && block.Layer == 0)
//                        continue;
//                    var diff = Mathf.Abs(block.transform.position.y - currentY);
//                    if (diff < dist)
//                    {
//                        nextBlock = block;
//                        dist = diff;
//                    }
//                }
//                if (nextBlock == null)
//                {
//                    return !avoidVoid;
//                }
//            }
//            float nextY = nextBlock.transform.position.y;
//            if(nextBlock.GetBlockType() == Def.BlockType.STAIRS)
//            {
//                nextY += GetStairYOffset(nextBlock.GetPilar().transform.position,
//                    nextBlock.GetRotation(), new Vector3(NextPosition.x, 0.0f, NextPosition.y));
//            }

//            if ((currentY + maxJump) > nextY)
//                return true;
//            return false;
//            //if (currentY > nextY)
//            //    return true;
//            //float height = Mathf.Abs(currentY - nextY);
//            ////Debug.Log(height);
//            //if (height > maxJump)
//            //    return false;
//            //return true;
		}

		public static float GetNearGround(Vector3 position, out IBlock groundBlock, out BridgeComponent groundBridge, float heightDiff = 0.6f)
		{
			groundBlock = null;
			groundBridge = null;
			return 0f;
//            var posXZ = new Vector2(position.x, position.z);
//            var mapID = MapIDFromPosition(posXZ);
//            float ground = 0.0f;
//            groundBlock = null;
//            groundBridge = null;
//            if (mapID < 0)
//            {
//                return ground;
//            }
//            var mapPos = PositionFromMapID(mapID);
//            var currentPilar = Manager.Mgr.Pilars[mapID];
//            var inPilarOffset = posXZ - mapPos;

//            if(currentPilar != null)
//            {
//                if (currentPilar.Bridge != null)
//                {
//                    var bridgeHeight = currentPilar.Bridge.GetHeight(inPilarOffset);
//                    var bridgeHeightOffset = Mathf.Abs(position.y - bridgeHeight);
//                    if (bridgeHeightOffset <= heightDiff)
//                    {
//                        ground = bridgeHeight;
//                        groundBridge = currentPilar.Bridge;
//                        return ground;
//                    }
//                }

//                IBlock block = null;
//                IBlock possibleBlock = null;
//                float blockDiff = float.MaxValue;
//                float height = float.MinValue;
//                for(int i = 0; i < currentPilar.Blocks.Count; ++i)
//                {
//                    possibleBlock = currentPilar.Blocks[i];
//                    var blockGround = possibleBlock.GetHeight() + possibleBlock.GetMicroHeight();
//                    if(possibleBlock.GetBlockType() == Def.BlockType.STAIRS)// && !possibleBlock.Removed)
//                    {
//                        blockGround += GetStairYOffset(currentPilar.transform.position, possibleBlock.GetRotation(), position);
//                    }
//                    var blockHeightDiff = Mathf.Abs(position.y - blockGround);
//                    if (blockHeightDiff > heightDiff)
//                        continue;
//                    if(blockHeightDiff < blockDiff || blockGround > height)
//                    {
//                        block = possibleBlock;
//                        blockDiff = blockHeightDiff;
//                        height = blockGround;
//                    }
//                }
//                if(block == null && currentPilar.Blocks.Count > 0)
//                {
//                    block = currentPilar.Blocks[currentPilar.Blocks.Count - 1];
//                    height = block.GetHeight() + block.GetMicroHeight();
//                    if (block.GetBlockType() == Def.BlockType.STAIRS)// && !block.Removed)
//                    {
//                        height += GetStairYOffset(currentPilar.transform.position, block.GetRotation(), position);
//                    }
//                }
//                if(block != null)
//                {
//                    ground = height;
//                    groundBlock = block;
//                }
//            }

//            return ground;
		}

//        //public static float GetGround(Vector3 position, out BlockComponent groundBlock)
//        //{
//        //    var mapID = MapIDFromPosition(new Vector2(position.x, position.z));
//        //    float ground = 0.0f;
//        //    groundBlock = null;
//        //    if (mapID < 0)
//        //    {
//        //        return ground;
//        //    }
//        //    var currentPilar = Manager.Mgr.Pilars[mapID];
//        //    if (currentPilar != null && currentPilar.Blocks.Count > 0)
//        //    {
//        //        BlockComponent block = null;
//        //        BlockComponent possibleBlock = null;
//        //        float blockDiff = 20000.0f;
//        //        float height = -20000.0f;
//        //        for (int i = 0; i < currentPilar.Blocks.Count; ++i)
//        //        {
//        //            var pBlock = currentPilar.Blocks[i];
//        //            possibleBlock = pBlock;
//        //            var blockGround = pBlock.Height + pBlock.MicroHeight;

//        //            var diff = blockGround - position.y;
//        //            if (diff > 0.6f)
//        //                continue;

//        //            var aDiff = Mathf.Abs(diff);

//        //            if(aDiff < blockDiff || blockGround > height)
//        //            {
//        //                block = pBlock;
//        //                blockDiff = aDiff;
//        //                height = blockGround;
//        //            }
//        //        }
//        //        if (block == null)
//        //        {
//        //            Debug.LogWarning("A non valid block was found while moving");
//        //            block = possibleBlock;
//        //            //throw new Exception("Something went wrong");
//        //        }

//        //        float stairOffset = 0.0f;
//        //        if (block.blockType == BlockType.STAIRS && !block.Removed)
//        //        {
//        //            stairOffset = GetStairYOffset(block.Pilar.transform.position, block.Rotation, position);
//        //        }
//        //        groundBlock = block;
//        //        ground = block.Height + block.MicroHeight + stairOffset;
//        //    }


//        //    return ground;
//        //}

//        //public static bool PFMovement(Vector3 actualPos, Vector2 TargetPos, float xzSpeed, float yUpSpeed,
//        //    float yFallSpeed, float radius, float maxJump, float deltaTime, bool avoidVoid, out Vector3 movement, out BlockComponent currentBlock,
//        //    out BridgeComponent currentBridge)
//        //{
//        //    movement = Vector3.zero;
//        //    bool onGround = true;
//        //    var movementXZ = LinearMovement2D(new Vector2(actualPos.x, actualPos.z), TargetPos, xzSpeed * deltaTime);
//        //    float movementY = 0.0f;
//        //    float ground = 0.0f;
//        //    var mapID = MapIDFromPosition(new Vector2(actualPos.x, actualPos.z));
//        //    currentBlock = null;
//        //    currentBridge = null;

//        //    if(mapID < 0)
//        //    {
//        //        return onGround;
//        //    }

//        //    var horizontalRadius = radius;
//        //    var verticalRadius = radius;
//        //    if (radius > 0.5f)
//        //        verticalRadius = radius * 0.5f;

//        //    var halfRadius = radius * 0.5f;
//        //    var halfHorizontalRadius = horizontalRadius * 0.5f;
//        //    var halfVerticalRadius = verticalRadius * 0.5f;
//        //    var baseGround = GetNearGround(actualPos, out currentBlock, out currentBridge, maxJump);
//        //    //var baseGround = GetGround(actualPos, out currentBlock);

//        //    Vector2[] checkPositions = new Vector2[]
//        //    {
//        //        new Vector2(actualPos.x + halfVerticalRadius, actualPos.z),
//        //        new Vector2(actualPos.x - halfVerticalRadius, actualPos.z),
//        //        new Vector2(actualPos.x, actualPos.z + halfHorizontalRadius),
//        //        new Vector2(actualPos.x, actualPos.z - halfHorizontalRadius),
//        //        //new Vector2(actualPos.x + radius, actualPos.z + radius),
//        //        //new Vector2(actualPos.x - radius, actualPos.z + radius),
//        //        //new Vector2(actualPos.x + radius, actualPos.z - radius),
//        //        //new Vector2(actualPos.x - radius, actualPos.z - radius),
//        //    };
//        //    bool nearGroundFound = false;
//        //    for(int i = 0; i < checkPositions.Length; ++i)
//        //    {
//        //        //var curGround = GetGround(new Vector3(checkPositions[i].x, actualPos.y, checkPositions[i].y), out BlockComponent b);
//        //        var curGround = GetNearGround(new Vector3(checkPositions[i].x, actualPos.y, checkPositions[i].y), out BlockComponent b, out BridgeComponent bc, maxJump);
//        //        if (ground < curGround && (curGround - baseGround) <= maxJump)
//        //        {
//        //            ground = curGround;
//        //            nearGroundFound = true;
//        //        }
//        //    }
//        //    if (nearGroundFound)
//        //        ground = (baseGround + ground) * 0.5f;
//        //    else
//        //        ground = baseGround;

//        //    // Below ground ?
//        //    if (actualPos.y < ground)
//        //    {
//        //        movementY = LinearMovement1D(actualPos.y, ground, yUpSpeed * deltaTime);
//        //        onGround = false;
//        //    }
//        //    // Above ground ?
//        //    else if (actualPos.y > ground)
//        //    {
//        //        // Y movement (Gravity)
//        //        movementY = LinearMovement1D(actualPos.y, ground, yFallSpeed * deltaTime);
//        //        onGround = false;
//        //    }
//        //    // Can we go to the next position?
//        //    var nextPos = new Vector2(actualPos.x, actualPos.z) + (movementXZ);
//        //    var nextMapID = MapIDFromPosition(nextPos);
//        //    if (mapID != nextMapID)
//        //    {
//        //        bool canMove = CanGoThere(ground, nextPos, maxJump, avoidVoid);
//        //        if (!canMove)
//        //        {
//        //            nextPos.Set(actualPos.x + movementXZ.x, actualPos.z);
//        //            canMove = CanGoThere(ground, nextPos, maxJump, avoidVoid);
//        //            if (!canMove)
//        //            {
//        //                nextPos.Set(actualPos.x, actualPos.z + movementXZ.y);
//        //                canMove = CanGoThere(ground, nextPos, maxJump, avoidVoid);
//        //                if (!canMove)
//        //                {
//        //                    movementXZ = Vector2.zero;
//        //                    nextPos.Set(actualPos.x, actualPos.z);
//        //                    //TargetPos.Set(transform.position.x, transform.position.z);
//        //                }
//        //                else
//        //                {
//        //                    movementXZ.Set(0.0f, movementXZ.y);
//        //                }
//        //            }
//        //            else
//        //            {
//        //                movementXZ.Set(movementXZ.x, 0.0f);
//        //            }
//        //        }
//        //    }
//        //    // Height check
//        //    if(currentBridge == null)
//        //    {
//        //        var currentMapPosition = PosFromID(mapID, Manager.MapWidth, Manager.MapHeight);
//        //        int[] nearMapIDs = new int[4]
//        //        {
//        //            IDFromPos(new Vector2Int(currentMapPosition.x + 1, currentMapPosition.y), Manager.MapWidth, Manager.MapHeight), // bot
//        //            IDFromPos(new Vector2Int(currentMapPosition.x, currentMapPosition.y + 1), Manager.MapWidth, Manager.MapHeight), // left
//        //            IDFromPos(new Vector2Int(currentMapPosition.x - 1, currentMapPosition.y), Manager.MapWidth, Manager.MapHeight), // top
//        //            IDFromPos(new Vector2Int(currentMapPosition.x, currentMapPosition.y - 1), Manager.MapWidth, Manager.MapHeight), // right
//        //        };
//        //        Vector2Int[] dirCheck = new Vector2Int[4]
//        //        {
//        //            new Vector2Int(1, 0),
//        //            new Vector2Int(0, 1),
//        //            new Vector2Int(-1, 0),
//        //            new Vector2Int(0, -1)
//        //        };
//        //        for(int i = 0; i < nearMapIDs.Length; ++i)
//        //        {
//        //            var pilar = Manager.Mgr.Pilars[nearMapIDs[i]];
//        //            if (pilar == null || pilar.Blocks.Count == 0)
//        //                continue;
//        //            var block = pilar.Blocks[pilar.Blocks.Count - 1];
//        //            if (block.Layer == 0)
//        //                continue;
//        //            var height = block.Height + block.MicroHeight;
//        //            if (height < (ground + maxJump))
//        //                continue;

//        //            var nextPilarPos = pilar.transform.position;

//        //            float top = nextPilarPos.x - 0.1f;
//        //            float bottom = top + 1.2f;
//        //            float left = nextPilarPos.z - 0.1f;
//        //            float right = left + 1.2f;

//        //            // Cylinder collision
//        //            //  -   X Collision
//        //            if((movementXZ.x > 0.0f && dirCheck[i].x > 0) || (movementXZ.x < 0.0f && dirCheck[i].x < 0))
//        //            {
//        //                var rad = verticalRadius * dirCheck[i].x;
//        //                if ((nextPos.x >= top && nextPos.x <= bottom) || ((nextPos.x + rad) >= top && (nextPos.x + rad) <= bottom))
//        //                {
//        //                    movementXZ.x = 0.0f;
//        //                }
//        //                //if((actualPos.x > top || (actualPos.x + radius) > top || (actualPos.x - radius) > top)
//        //                //    && (actualPos.x < bottom || (actualPos.x + radius) < bottom || (actualPos.x - radius) < bottom))
//        //                //{
//        //                //    movementXZ.x = 0.0f;
//        //                //}
//        //            }
//        //            //  -   Z Collision
//        //            if ((movementXZ.y > 0.0f && dirCheck[i].y > 0) || (movementXZ.y < 0.0f && dirCheck[i].y < 0))
//        //            {
//        //                var rad = horizontalRadius * dirCheck[i].y;
//        //                if ((nextPos.y >= left && nextPos.y <= right) || ((nextPos.y + rad) >= left && (nextPos.y + rad) <= right))
//        //                {
//        //                    movementXZ.y = 0.0f;
//        //                }

//        //                //if ((actualPos.z > left || (actualPos.z + radius) > left || (actualPos.z - radius) > left)
//        //                //    && (actualPos.z < right || (actualPos.z + radius) < right || (actualPos.z - radius) < right))
//        //                //{
//        //                //    movementXZ.y = 0.0f;
//        //                //}
//        //            }

//        //            //// X inside
//        //            //if (actualPos.x > top && actualPos.x < bottom && dirCheck[i].y == 0)
//        //            //{
//        //            //    if(movementXZ.x > 0.0f && dirCheck[i].x > 0 || movementXZ.x < 0.0f && dirCheck[i].x < 0)
//        //            //        movementXZ.x = 0.0f;
//        //            //}
//        //            //// Z inside
//        //            //if (actualPos.z > left && actualPos.z < right && dirCheck[i].x == 0)
//        //            //{
//        //            //    if (movementXZ.y > 0.0f && dirCheck[i].y > 0 || movementXZ.y < 0.0f && dirCheck[i].y < 0)
//        //            //        movementXZ.y = 0.0f;
//        //            //}
//        //        }
//        //    }
//        //    movement = new Vector3(movementXZ.x, movementY, movementXZ.y);

//        //    return onGround;
//        //}

		public static void DeleteGameobject(GameObject go, bool instant = false)
		{
			if (instant)
			{
				GameObject.DestroyImmediate(go);
			}
			else
			{
				GameObject.Destroy(go);
			}
		}
		public static int ContainsAtChance(List<IDChance> list, int id)
		{
			if (list == null)
				return -1;
			for (int i = 0; i < list.Count; ++i)
				if (list[i].ID == id && list[i].Chance > 0)
					return i;
			return -1;
		}
		public static void ApplyFnBlockAbove(IBlock block, Action<IBlock> fn)
		{
			var topBlock = block.GetStackedAbove(); //block.GetStackedBlocks()[1] as CBlockEdit;
			while (topBlock != null)
			{
				fn(topBlock);
				topBlock = topBlock.GetStackedAbove(); //topBlock.GetStackedBlocks()[1] as CBlockEdit;
			}
		}
		public static void ApplyFnBlockBelow(IBlock block, Action<IBlock> fn)
		{
			var botBlock = block.GetStackedBelow(); //block.GetStackedBlocks()[0] as CBlockEdit;
			while (botBlock != null)
			{
				fn(botBlock);
				botBlock = botBlock.GetStackedBelow(); //botBlock.GetStackedBlocks()[0] as CBlockEdit;
			}
		}

		public static void UpdateMicroheightAbove(IBlock block)
		{
			ApplyFnBlockAbove(block, (IBlock b) => b.SetMicroHeight(block.GetMicroHeight()));
		}
		public static GameObject FindChild(GameObject obj, string childName)
		{
			var cName = childName.ToLower();
			GameObject childGO = null;
			for (int i = 0; i < obj.transform.childCount; ++i)
			{
				var child = obj.transform.GetChild(i).gameObject;
				if (child.name.ToLower() == cName)
				{
					childGO = child;
					break;
				}
			}
			return childGO;
		}
		public static GameObject FindParent(GameObject obj, string parentName)
		{
			Transform parent = obj.transform.parent;
			var parentLower = parentName.ToLower();
			while (parent != null)
			{
				if (parent.name.ToLower() == parentLower)
				{
					return parent.gameObject;
				}
				parent = parent.transform.parent;
			}
			if (parent == null)
				return null;
			return parent.gameObject;
		}
		//        public static void SortRayCast(ref RaycastHit[] tempHits, ref RaycastHit[] sortedHits, int rayAmount)
		//        {
		//            for (int i = 0; i < rayAmount; ++i)
		//            {
		//                float minDist = float.MaxValue;
		//                int minIdx = int.MaxValue;
		//                for (int j = 0; j < rayAmount; ++j)
		//                {
		//                    if (tempHits[j].distance < minDist)
		//                    {
		//                        minIdx = j;
		//                        minDist = tempHits[j].distance;
		//                    }
		//                }
		//                sortedHits[i] = tempHits[minIdx];
		//                tempHits[minIdx].distance = float.MaxValue;
		//            }
		//        }
		public enum ImageFilterMode : int
		{
			Nearest = 0,
			Biliner = 1,
			Average = 2
		}
		public static Texture2D ResizeTexture(Texture2D pSource, ImageFilterMode pFilterMode, float pScale, bool readable = false)
		{
			//*** Variables
			int i = 0;

			//*** Get All the source pixels
			var aSourceColor = pSource.GetPixels32(0);
			var vSourceSize = new Vector2(pSource.width, pSource.height);

			//*** Calculate New Size
			var xWidth = Mathf.RoundToInt((float)pSource.width * pScale);
			var xHeight = Mathf.RoundToInt((float)pSource.height * pScale);

			//*** Make New
			var oNewTex = new Texture2D((int)xWidth, (int)xHeight, pSource.format, false);

			//*** Make destination array
			var xLength = (int)xWidth * (int)xHeight;
			var aColor = new Color32[xLength];

			var vPixelSize = new Vector2(vSourceSize.x / xWidth, vSourceSize.y / xHeight);

			//*** Loop through destination pixels and process
			var vCenter = new Vector2();
			void NearestFilter()
			{
				//*** Nearest neighbour (testing)
				vCenter.x = Mathf.Round(vCenter.x);
				vCenter.y = Mathf.Round(vCenter.y);

				//*** Calculate source index
				int xSourceIndex = (int)((vCenter.y * vSourceSize.x) + vCenter.x);

				//*** Copy Pixel
				aColor[i] = aSourceColor[xSourceIndex];
			}
			void BilinearFilter()
			{
				//*** Get Ratios
				float xRatioX = vCenter.x - Mathf.Floor(vCenter.x);
				float xRatioY = vCenter.y - Mathf.Floor(vCenter.y);

				//*** Get Pixel index's
				int xIndexTL = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
				int xIndexTR = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));
				int xIndexBL = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
				int xIndexBR = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));

				//*** Calculate Color
				aColor[i] = Color32.Lerp(
					Color32.Lerp(aSourceColor[xIndexTL], aSourceColor[xIndexTR], xRatioX),
					Color32.Lerp(aSourceColor[xIndexBL], aSourceColor[xIndexBR], xRatioX),
					xRatioY
				);
			}
			void AverageFilter()
			{
				//*** Calculate grid around point
				int xXFrom = (int)Mathf.Max(Mathf.Floor(vCenter.x - (vPixelSize.x * 0.5f)), 0);
				int xXTo = (int)Mathf.Min(Mathf.Ceil(vCenter.x + (vPixelSize.x * 0.5f)), vSourceSize.x);
				int xYFrom = (int)Mathf.Max(Mathf.Floor(vCenter.y - (vPixelSize.y * 0.5f)), 0);
				int xYTo = (int)Mathf.Min(Mathf.Ceil(vCenter.y + (vPixelSize.y * 0.5f)), vSourceSize.y);

				//*** Loop and accumulate
				//Vector4 oColorTotal = new Vector4();
				var oColorTemp = new Color();
				float xGridCount = 0;
				for (int iy = xYFrom; iy < xYTo; iy++)
				{
					for (int ix = xXFrom; ix < xXTo; ix++)
					{

						//*** Get Color
						oColorTemp += aSourceColor[(int)((iy * vSourceSize.x) + ix)];

						//*** Sum
						xGridCount++;
					}
				}

				//*** Average Color
				var fColor = (oColorTemp / xGridCount).linear;
				aColor[i] = new Color32((byte)(fColor.r * 255f), (byte)(fColor.g * 255f), (byte)(fColor.b * 255f), (byte)(fColor.a * 255f));
			}

			Action filter = () => { };
			switch (pFilterMode)
			{
				case ImageFilterMode.Nearest:
					filter = NearestFilter;
					break;
				case ImageFilterMode.Biliner:
					filter = BilinearFilter;
					break;
				case ImageFilterMode.Average:
					filter = AverageFilter;
					break;
			}

			for (i = 0; i < xLength; i++)
			{
				//*** Figure out x&y
				var xX = (float)i % xWidth;
				var xY = Mathf.Floor((float)i / xWidth);

				//*** Calculate Center
				vCenter.x = (xX / xWidth) * vSourceSize.x;
				vCenter.y = (xY / xHeight) * vSourceSize.y;

				filter();
			}

			//*** Set Pixels
			oNewTex.SetPixels32(aColor);
			oNewTex.Apply(false, !readable);

			//*** Return
			return oNewTex;
		}

		public static string RemoveWhitespaces(string input)
		{
			var len = input.Length;
			var src = input.ToCharArray();
			int dstIdx = 0;
			for (int i = 0; i < len; i++)
			{
				var ch = src[i];
				switch (ch)
				{
					case '\u0020':
					case '\u00A0':
					case '\u1680':
					case '\u2000':
					case '\u2001':
					case '\u2002':
					case '\u2003':
					case '\u2004':
					case '\u2005':
					case '\u2006':
					case '\u2007':
					case '\u2008':
					case '\u2009':
					case '\u200A':
					case '\u202F':
					case '\u205F':
					case '\u3000':
					case '\u2028':
					case '\u2029':
					case '\u0009':
					case '\u000A':
					case '\u000B':
					case '\u000C':
					case '\u000D':
					case '\u0085':
						continue;
					default:
						src[dstIdx++] = ch;
						break;
				}
			}
			return new string(src, 0, dstIdx);
		}
		
		public static void CopyMesh(Mesh from, ref Mesh to)
		{
			var vertices = new Vector3[from.vertices.Length];
			from.vertices.CopyTo(vertices, 0);
			to.vertices = vertices;

			var uvs = new Vector2[from.uv.Length];
			from.uv.CopyTo(uvs, 0);
			to.uv = uvs;

			var normals = new Vector3[from.normals.Length];
			from.normals.CopyTo(normals, 0);
			to.normals = normals;

			var tangents = new Vector4[from.tangents.Length];
			from.tangents.CopyTo(tangents, 0);
			to.tangents = tangents;

			var triangles = new int[from.triangles.Length];
			from.triangles.CopyTo(triangles, 0);
			to.triangles = triangles;
		}
		public static Color32 UnpackColor(uint packed)
		{
			return new Color32(
				(byte)((packed >> 24) & 0xFF),
				(byte)((packed >> 16) & 0xFF),
				(byte)((packed >> 8) & 0xFF),
				(byte)(packed & 0xFF));
		}
		public static bool WriteXMLString(XmlWriter writer, string elementName, string value)
		{
			if(writer == null)
			{
				Debug.LogWarning("Trying to write to a null XmlWriter");
				return false;
			}
			if(elementName.Length == 0)
			{
				Debug.LogWarning("Trying to write an unnamed element to XML");
				return false;
			}

			try
			{
				writer.WriteStartElement(elementName);

				writer.WriteString(value);

				if (value.Length > 0)
					writer.WriteEndElement();
				else
					writer.WriteFullEndElement();
			}
			catch(XmlException e)
			{
				Debug.LogWarning("XML Exception: " + e.Message);
				return false;
			}
			return true;
		}
		public static bool ReadXMLString(XmlReader reader, string elementName, out string value)
		{
			value = "";

			if(reader == null)
			{
				Debug.LogWarning("Trying to read from a null XmlReader.");
				return false;
			}
			if(elementName.Length == 0)
			{
				Debug.LogWarning("Trying to read an unnamed element from XML");
				return false;
			}

			try
			{
				reader.ReadStartElement(elementName);
				value = reader.ReadContentAsString();
				reader.ReadEndElement();
			}
			catch(XmlException e)
			{
				Debug.LogWarning("XML Exception: " + e.Message);
				return false;
			}
			return true;
		}
		public static bool ReadXMLInt(XmlReader reader, string elementName, out int value)
		{
			value = 0;

			if (reader == null)
			{
				Debug.LogWarning("Trying to read from a null XmlReader.");
				return false;
			}
			if (elementName.Length == 0)
			{
				Debug.LogWarning("Trying to read an unnamed element from XML");
				return false;
			}

			try
			{
				reader.ReadStartElement(elementName);
				int.TryParse(reader.ReadContentAsString(), out value);
				reader.ReadEndElement();
			}
			catch (XmlException e)
			{
				Debug.LogWarning("XML Exception: " + e.Message);
				return false;
			}
			return true;
		}
		public static bool ReadXMLFloat(XmlReader reader, string elementName, out float value)
		{
			value = 0f;

			if (reader == null)
			{
				Debug.LogWarning("Trying to read from a null XmlReader.");
				return false;
			}
			if (elementName.Length == 0)
			{
				Debug.LogWarning("Trying to read an unnamed element from XML");
				return false;
			}

			try
			{
				reader.ReadStartElement(elementName);
				float.TryParse(reader.ReadContentAsString(), out value);
				reader.ReadEndElement();
			}
			catch (XmlException e)
			{
				Debug.LogWarning("XML Exception: " + e.Message);
				return false;
			}
			return true;
		}
		public static bool ReadXMLBool(XmlReader reader, string elementName, out bool value)
		{
			value = false;

			if (reader == null)
			{
				Debug.LogWarning("Trying to read from a null XmlReader.");
				return false;
			}
			if (elementName.Length == 0)
			{
				Debug.LogWarning("Trying to read an unnamed element from XML");
				return false;
			}

			try
			{
				reader.ReadStartElement(elementName);
				bool.TryParse(reader.ReadContentAsString(), out value);
				reader.ReadEndElement();
			}
			catch (XmlException e)
			{
				Debug.LogWarning("XML Exception: " + e.Message);
				return false;
			}
			return true;
		}
		public static bool ReadXMLEnum<TEnum>(XmlReader reader, string elementName, out TEnum value) where TEnum : struct
		{
			value = new TEnum();

			if (reader == null)
			{
				Debug.LogWarning("Trying to read from a null XmlReader.");
				return false;
			}
			if (elementName.Length == 0)
			{
				Debug.LogWarning("Trying to read an unnamed element from XML");
				return false;
			}

			try
			{
				reader.ReadStartElement(elementName);
				Enum.TryParse(reader.ReadContentAsString(), out value);
				reader.ReadEndElement();
			}
			catch (XmlException e)
			{
				Debug.LogWarning("XML Exception: " + e.Message);
				return false;
			}
			return true;
		}
	}

	//    public enum DialogResponse
	//    {
	//        Wait,
	//        No,
	//        Yes
	//    }

	//    public class GUIUtils
	//    {
	//        static float Width = 2000.0f;
	//        static float Height = 2000.0f;

	//        public static DialogResponse CenteredDialog(string question)
	//        {
	//            var rect = Manager.Mgr.m_Canvas.pixelRect;
	//            var pos = new Vector2(rect.width * 0.5f - Width * 0.5f, rect.height * 0.5f - Height * 0.5f);
	//            GUI.Box(new Rect(pos.x, pos.y, Width, Height), "");
	//            const float Separation = 50.0f;
	//            float lastHeight = 10.0f;
	//            float lastWidth = 0.0f;
	//            var content = new GUIContent(question);
	//            var style = GUI.skin.label;
	//            style.alignment = TextAnchor.MiddleLeft;
	//            var size = style.CalcSize(content);
	//            GUI.Label(new Rect(pos.x + Separation, pos.y + lastHeight, size.x, size.y), content);
	//            lastWidth = Mathf.Max(lastWidth, size.x);
	//            lastHeight += 25.0f;


	//            lastHeight += 5.0f;
	//            bool yes = GUI.Button(new Rect(pos.x + Width * 0.5f - 50.0f, pos.y + lastHeight, 50.0f, 25.0f), "Yes");
	//            bool no = GUI.Button(new Rect(pos.x + Width * 0.5f, pos.y + lastHeight, 50.0f, 25.0f), "No");
	//            lastHeight += 25.0f;

	//            Width = lastWidth + 2.0f * Separation;
	//            Height = lastHeight;
	//            return yes ? DialogResponse.Yes : no ? DialogResponse.No : DialogResponse.Wait;
	//        }
	//    }

	public static class SpriteUtils
	{
		public static Type GetSpriteBackend(SpriteBackendType type)
		{
			switch (type)
			{
				case SpriteBackendType.SPRITE:
					return typeof(SpriteBackendSprite);
				case SpriteBackendType.MIX:
					return typeof(SpriteBackendMix);
				case SpriteBackendType.SQUAD:
					return typeof(SpriteBackendSQuad);
				case SpriteBackendType.SQUAD_DS:
					return typeof(SpriteBackendSQuadDS);
				case SpriteBackendType.DQUAD:
					return typeof(SpriteBackendDQuad);
				default:
					return null;
			}
		}

		public static ISpriteBackend AddSprite(GameObject obj, SpriteBackendType type, Sprite sprite)
		{
			var backend = GetSpriteBackend(type);
			if(backend == null)
			{
				Debug.LogWarning("Trying to add a SpriteBackend component to a GO, but invalid SpriteBackendType was provided '" + type.ToString() + '\'');
				return null;
			}
			var cmp = obj.AddComponent(backend) as ISpriteBackend;
			cmp.SetSprite(sprite);
			return cmp;
		}

		public static void InitSprite(SpriteRenderer sr, Sprite sprite, Material material)
		{
			sr.drawMode = SpriteDrawMode.Simple;
			sr.material = material;
			sr.sprite = sprite;
			sr.allowOcclusionWhenDynamic = true;
			sr.receiveShadows = false;
			sr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
			sr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			sr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
			sr.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
			sr.enabled = true;
		}

		public static void InitSpriteLit(SpriteRenderer sr, Sprite sprite)
		{
			sr.drawMode = SpriteDrawMode.Simple;
			sr.material = new Material(Materials.GetMaterial(Def.Materials.SpriteLit))
			{
				mainTexture = sprite.texture,
				color = sr.color
			};
			sr.sprite = sprite;
			sr.allowOcclusionWhenDynamic = true;
			sr.receiveShadows = true;
			sr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
			sr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			sr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
			sr.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
			sr.enabled = true;
		}
	}
}
