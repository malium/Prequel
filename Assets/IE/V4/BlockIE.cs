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

namespace Assets.IE.V4
{
	[System.Serializable]
	public class BlockIE
	{
		public enum Flag
		{
			Anchor,
			Rotation,
			Prop,
			MaterialFamily,
			Length,
			COUNT
		}
		public const int FlagCount = (int)Flag.COUNT;

		public ushort StructureID;
		public byte Layer;

		public Def.StairState Stair;
		public List<bool> Flags;
		public Def.RotationState Rotation;
		public Def.BlockVoid BlockVoid;
		public string PropFamily;
		public string MaterialFamily;
		public Def.BlockType BlockType;
		public byte VLength;
		public short VHeight;
		public short[] StackedIdx;

		public void SetFlag(Flag flag, bool enable)
		{
			Flags[(int)flag] = enable;
		}
		public bool GetFlag(Flag flag)
		{
			return Flags[(int)flag];
		}
		public BlockIE()
		{
			StructureID = ushort.MaxValue;
			StackedIdx = new short[2];
			SetDefault();
		}
		public BlockIE(BlockIE copy)
		{
			StructureID = copy.StructureID;
			Flags = new List<bool>(copy.Flags);
			Layer = copy.Layer;
			Stair = copy.Stair;
			PropFamily = copy.PropFamily;
			MaterialFamily = copy.MaterialFamily;
			BlockVoid = copy.BlockVoid;
			BlockType = copy.BlockType;
			VLength = copy.VLength;
			VHeight = copy.VHeight;
			StackedIdx = new short[2];
			copy.StackedIdx.CopyTo(StackedIdx, 0);
		}
		public float GetLength()
		{
			var len = (VLength + 1) * 0.5f;
			if (len == 3.5f)
				len = 3.4f;
			return len;
		}
		public void SetLength(float length)
		{
			if (length == 3.4f)
				length = 3.5f;
			length *= 2f;
			VLength = (byte)(((int)length) - 1);
		}
		public float GetHeight() => VHeight * 0.5f;
		public void SetHeight(float height) => VHeight = (short)(height * 2f);
		public void SetDefault()
		{
			Flags = new List<bool>(Enumerable.Repeat(false, FlagCount));
			Layer = 0;
			Stair = Def.StairState.NONE;
			PropFamily = "";
			MaterialFamily = "";
			BlockVoid = Def.BlockVoid.NORMAL;
			BlockType = Def.BlockType.NORMAL;
			VLength = 0;
			VHeight = 0;
			StackedIdx[0] = -1;
			StackedIdx[1] = -1;
		}
		public void Apply(CBlockEdit block)
		{
			int locked = 0;
			block.SetStairState(
				Stair == Def.StairState.COUNT ? 
				Def.StairState.NONE : Stair);

			block.SetAnchor(Flags[(int)Flag.Anchor]);

			if(BlockType != Def.BlockType.COUNT)
			{
				block.SetBlockType(BlockType);
			}

			if(Flags[(int)Flag.Rotation])
			{
				if(Rotation != Def.RotationState.COUNT)
					block.SetRotation(Rotation);
				++locked;
			}

			if(BlockVoid != Def.BlockVoid.COUNT)
			{
				block.SetVoidState(BlockVoid);
			}

			// Height
			{
				block.SetHeight(GetHeight());
				//block._CheckStackedLinks();
			}

			if(Flags[(int)Flag.MaterialFamily])
			{
				if(MaterialFamily.Length > 0)
					block.SetMaterialFamily(BlockMaterial.MaterialFamilies[BlockMaterial.FamilyDict[MaterialFamily]]);
				++locked;
			}

			if(Flags[(int)Flag.Length] && VLength >= 0 && VLength <= 6)
			{
				block.SetLength(GetLength());
				block._CheckStackedLinks();
				++locked;
			}

			if(Flags[(int)Flag.Prop])
			{
				if (PropFamily.Length > 0)
				{
					var familyID = Props.FamilyDict[PropFamily];
					var propFamily = Props.PropFamilies[familyID];
					if (propFamily.Props.Count > 0)
					{
						var propID = UnityEngine.Random.Range(0, propFamily.Props.Count);
						var prop = new GameObject().AddComponent<AI.CProp>();
						prop.SetProp(familyID, propID);
						//prop.SetBlock(block);
						prop.GetLE().SetCurrentBlock(block);
						block.SetProp(prop);
						block.GetPilar().GetStruc().GetLES().Add(prop.GetLE());
						prop.GetSprite().Flip(UnityEngine.Random.value > 0.5f, false);
					}
					else
					{
						Debug.Log("Trying to spawn a prop from a family with no childs.");
					}
				}
				++locked;
			}

			if (locked == (FlagCount - 1)) // All except anchor
			{
				block.SetLockState(Def.LockState.Locked);
			}
			else if (locked > 0)
			{
				block.SetLockState(Def.LockState.SemiLocked);
			}
			else
			{
				block.SetLockState(Def.LockState.Unlocked);
			}
		}
		public override bool Equals(object obj)
		{
			return obj is BlockIE iE
				&& this == iE;
		}
		public override int GetHashCode()
		{
			int hashCode = -1812799824;
			hashCode = hashCode * -1521134295 + StructureID.GetHashCode();
			hashCode = hashCode * -1521134295 + Layer.GetHashCode();
			hashCode = hashCode * -1521134295 + Stair.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<List<bool>>.Default.GetHashCode(Flags);
			hashCode = hashCode * -1521134295 + Rotation.GetHashCode();
			hashCode = hashCode * -1521134295 + BlockVoid.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PropFamily);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MaterialFamily);
			hashCode = hashCode * -1521134295 + BlockType.GetHashCode();
			hashCode = hashCode * -1521134295 + VLength.GetHashCode();
			hashCode = hashCode * -1521134295 + VHeight.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<short[]>.Default.GetHashCode(StackedIdx);
			return hashCode;
		}
		public static bool operator==(BlockIE left, BlockIE right)
		{
			if (left is null && !(right is null))
				return false;
			if (!(left is null) && right is null)
				return false;
			if (left is null && right is null)
				return true;

			if (left.StructureID != right.StructureID)
				return false;

			if (left.Layer != right.Layer)
				return false;

			if (left.Stair != right.Stair)
				return false;

			for(int i = 0; i < FlagCount; ++i)
			{
				if (left.Flags[i] != right.Flags[i])
					return false;
			}

			if (left.Flags[(int)Flag.Rotation]
				&& left.Rotation != right.Rotation)
				return false;

			if (left.Flags[(int)Flag.Prop]
				&& left.PropFamily != right.PropFamily)
				return false;

			if (left.Flags[(int)Flag.MaterialFamily]
				&& left.MaterialFamily != right.MaterialFamily)
				return false;

			if (left.BlockVoid != right.BlockVoid)
				return false;

			if (left.BlockType != right.BlockType)
				return false;

			if (left.Flags[(int)Flag.Length]
				&& left.VLength != right.VLength)
				return false;

			if (left.VHeight != right.VHeight)
				return false;

			if (left.StackedIdx[0] != right.StackedIdx[0] ||
				left.StackedIdx[1] != right.StackedIdx[1])
				return false;

			return true;
		}
		public static bool operator!=(BlockIE left, BlockIE right)
		{
			return !(left == right);
		}
	}
}
