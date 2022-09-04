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

namespace Assets.IE.V3
{
	[System.Serializable]
	public class BlockIE
	{
		public enum Flag
		{
			Anchor,
			Rotation,
			Monster,
			Prop,
			MaterialFamily,
			BlockType,
			Length,
			Height,
			Void,
			COUNT
		}
		public const int FlagCount = (int)Flag.COUNT;

		public ushort StructureID;
		public byte Layer;
		public Def.StairState Stair;
		public List<bool> Flags;
		public Def.RotationState Rotation;
		public Def.BlockVoid BlockVoid;
		public string MonsterFamily;
		public string PropFamily;
		public string MaterialFamily;
		public Def.BlockType BlockType;
		public float Length;
		public float Height;
		public float MicroHeight; // Unused

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
			SetDefault();
		}
		public BlockIE(BlockIE copy)
		{
			StructureID = copy.StructureID;
			Flags = new List<bool>(copy.Flags);
			Layer = copy.Layer;
			Stair = copy.Stair;
			MonsterFamily = copy.MonsterFamily;
			PropFamily = copy.PropFamily;
			MaterialFamily = copy.MaterialFamily;
			BlockVoid = copy.BlockVoid;
			BlockType = copy.BlockType;
			Length = copy.Length;
			Height = copy.Height;
			MicroHeight = copy.MicroHeight;
		}
		public void SetDefault()
		{
			Flags = new List<bool>(Enumerable.Repeat(false, FlagCount));
			Layer = 0;
			Stair = Def.StairState.NONE;
			MonsterFamily = "";
			PropFamily = "";
			MaterialFamily = "";
			BlockVoid = Def.BlockVoid.NORMAL;
			BlockType = Def.BlockType.NORMAL;
			Length = 0.5f;
			Height = 0f;
			MicroHeight = 0f;
		}

		public V4.BlockIE Upgrade(StructureIE struc)
		{
			var ie = new V4.BlockIE()
			{
				StructureID = StructureID,
				Layer = Layer,
				Stair = Stair,
				Rotation = Rotation,
				BlockVoid = BlockVoid,
				PropFamily = PropFamily,
				MaterialFamily = MaterialFamily,
				BlockType = BlockType,
			};
			ie.SetLength(Length);

			ie.SetFlag(V4.BlockIE.Flag.Anchor, GetFlag(Flag.Anchor));
			ie.SetFlag(V4.BlockIE.Flag.Rotation, GetFlag(Flag.Rotation));
			ie.SetFlag(V4.BlockIE.Flag.Prop, GetFlag(Flag.Prop));
			ie.SetFlag(V4.BlockIE.Flag.MaterialFamily, GetFlag(Flag.MaterialFamily));
			ie.SetFlag(V4.BlockIE.Flag.Length, GetFlag(Flag.Length));

			if (GetFlag(Flag.Height))
			{
				ie.SetHeight(Height);
			}
			else
			{
				var blocksIE = new List<BlockIE>();
				for(int i = 0; i < struc.GetBlocks().Length; ++i)
				{
					var block = struc.GetBlocks()[i];
					if (block == null || block.StructureID != StructureID)
						continue;

					if (block == this)
						break;
				
					if(block.GetFlag(Flag.Height))
					{
						blocksIE.Clear();
					}
					blocksIE.Add(block);
				}

				float height = 0f;
				for(int i = 0; i < blocksIE.Count; ++i)
				{
					var block = blocksIE[i];
					if(block.GetFlag(Flag.Height))
					{
						height = block.Height;
						continue;
					}
					if(block.GetFlag(Flag.Length))
					{
						height += (block.Length == 3.4f ? 3.5f : block.Length);
					}
					else
					{
						height += 0.5f;
					}
				}
				ie.SetHeight(height);
			}

			return ie;
		}

		public void Apply(CBlockEdit block)
		{
			int locked = 0;
			block.SetStairState(
				Stair == Def.StairState.COUNT ? 
				Def.StairState.NONE : Stair);

			block.SetAnchor(Flags[(int)Flag.Anchor]);

			if(/*Flags[(int)Flag.BlockType] && */BlockType != Def.BlockType.COUNT)
			{
				block.SetBlockType(BlockType);
				//++locked;
			}

			if(Flags[(int)Flag.Rotation] && Rotation != Def.RotationState.COUNT)
			{
				block.SetRotation(Rotation);
				++locked;
			}

			if(/*Flags[(int)Flag.Void] && */BlockVoid != Def.BlockVoid.COUNT)
			{
				block.SetVoidState(BlockVoid);
				//++locked;
			}

			if(Flags[(int)Flag.Height] && Mathf.Abs(Height) < 1000f)
			{
				var diff = Height - block.GetHeight();
				bool increase = diff >= 0f;
				Action fn;
				if(increase)
				{
					fn = () => block.IncreaseHeight();
				}
				else
				{
					diff = -diff;
					fn = () => block.DecreaseHeight();
				}

				for(float step = 0f; step < diff; step += 0.5f)
				{
					fn();
					//block.SetHeight(block.GetHeight() + 0.5f * mult);
					//if (increase)
					//	block.IncreaseHeightCheck();
					//else
					//	block.DecreseHeightCheck();
				}
				//block.SetHeight(Height);
				//block.SetMicroHeight(MicroHeight);
				++locked;
			}

			if(Flags[(int)Flag.MaterialFamily] && MaterialFamily.Length > 0)
			{
				block.SetMaterialFamily(BlockMaterial.MaterialFamilies[BlockMaterial.FamilyDict[MaterialFamily]]);
				++locked;
			}

			float maxLength;
			if (block.GetBlockType() == Def.BlockType.WIDE)
				maxLength = Blocks.MaxWideHeight;
			else
				maxLength = Blocks.MaxNormalHeight;
			if(Flags[(int)Flag.Length] && Length > 0.5f && Length <= maxLength)
			{
				var diff = (Length == 3.4f ? 3.5f : Length) - (block.GetLength() == 3.4f ? 3.5f : block.GetLength());
				bool increase = diff >= 0f;
				Action fn;
				if(increase)
				{
					fn = () => block.IncreaseLength();
				}
				else
				{
					diff = -diff;
					fn = () => block.DecreaseLength();
				}
				for (float step = 0f; step < diff; step += 0.5f)
				{
					fn();
					//float amount = 0.5f;
					//if ((block.GetLength() == 3.4f && !increase) || (block.GetLength() == 3f && increase))
					//	amount = 0.4f;

					//block.SetLength(block.GetLength() + amount * mult);
					//if (increase)
					//	block.IncreaseLengthCheck();
					//else
					//	block.DecreseLengthCheck();
				}
				//block.SetLength(Length);
				++locked;
			}

			if (Flags[(int)Flag.Monster] && MonsterFamily.Length > 0)
			{
				var mon = block.GetMonster();
				if (mon != null)
					mon.GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, mon.GetLE().GetCurrentHealth());
					//mon.ReceiveDamage(Def.DamageType.UNAVOIDABLE, mon.GetTotalHealth());

				mon = new GameObject().AddComponent<AI.CMonster>();
				mon.SetMonster(Monsters.MonsterFamilies[Monsters.FamilyDict[MonsterFamily]]);
				mon.transform.position = block.GetPilar().transform.position +
						new Vector3(UnityEngine.Random.value, block.GetHeight() + block.GetMicroHeight(), UnityEngine.Random.value);
				mon.enabled = true;
				mon.GetSprite().SetEnabled(Manager.Mgr.HideInfo);
				mon.GetShadow().SetEnabled(Manager.Mgr.HideInfo);
				mon.GetLE().GetCollider().enabled = false;
				mon.GetME().Impulse(new Vector2(UnityEngine.Random.value * 2f - 1f, UnityEngine.Random.value * 2f - 1f), 0f);
				//mon.GetME().SetTarget(mon.transform.position,
				//		new Vector3(UnityEngine.Random.value * 2f - 1f, 0f, UnityEngine.Random.value * 2f - 1f));
				block.SetMonster(mon);
				//var monGO = new GameObject($"Monster_{MonsterScript.MonsterID++}");
				//mon = Monsters.AddMonsterComponent(monGO, Monsters.FamilyDict[MonsterFamily]);
				//mon.InitMonster();
				//mon.transform.position = block.GetPilar().transform.position +
				//		new Vector3(UnityEngine.Random.value, block.GetHeight() + block.GetMicroHeight(), UnityEngine.Random.value);
				////mon.transform.position = block.GetPilar().transform.position;
				////mon.transform.Translate(0f, block.GetHeight() + block.GetMicroHeight(), 0f, Space.World);
				//mon.enabled = false;
				////mon.SpriteSR.enabled = Manager.Mgr.HideInfo;
				////mon.ShadowSR.enabled = Manager.Mgr.HideInfo;
				//mon.SpriteCC.enabled = false;
				//var facing = mon.Facing;
				//facing.Horizontal = (SpriteHorizontal)UnityEngine.Random.Range(0, (int)SpriteHorizontal.COUNT);
				//facing.Vertical = (SpriteVertical)UnityEngine.Random.Range(0, (int)SpriteVertical.COUNT);
				//mon.Facing = facing;

				++locked;
			}

			if(Flags[(int)Flag.Prop] && PropFamily.Length > 0)
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
					//var prop = new GameObject($"Prop_{PropScript.PropID++}").AddComponent<PropScript>();
					//block.SetProp(prop);
					//prop.Block = block;
					//prop.SetProp(familyID, propID);
					//prop.enabled = false;
					////prop.SpriteSR.enabled = false;
					//prop.SpriteBC.enabled = false;
					////if (prop.ShadowSR != null)
					////	prop.ShadowSR.enabled = false;
					////if (prop.PropLight != null)
					////	prop.PropLight.enabled = false;
					//var facing = prop.Facing;
					//facing.Horizontal = (SpriteHorizontal)UnityEngine.Random.Range(0, (int)SpriteHorizontal.COUNT);
					//prop.Facing = facing;
				}
				else
				{
					Debug.Log("Trying to spawn a prop from a family with no childs.");
				}
			}

			if (locked == (FlagCount - 2))
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
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MonsterFamily);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PropFamily);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MaterialFamily);
			hashCode = hashCode * -1521134295 + BlockType.GetHashCode();
			hashCode = hashCode * -1521134295 + Length.GetHashCode();
			hashCode = hashCode * -1521134295 + Height.GetHashCode();
			hashCode = hashCode * -1521134295 + MicroHeight.GetHashCode();
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

			if (left.Flags[(int)Flag.Monster]
				&& left.MonsterFamily != right.MonsterFamily)
				return false;

			if (left.Flags[(int)Flag.Prop]
				&& left.PropFamily != right.PropFamily)
				return false;

			if (left.Flags[(int)Flag.MaterialFamily]
				&& left.MaterialFamily != right.MaterialFamily)
				return false;

			if (left.Flags[(int)Flag.Void]
				&& left.BlockVoid != right.BlockVoid)
				return false;

			if (left.Flags[(int)Flag.BlockType]
				&& left.BlockType != right.BlockType)
				return false;

			if (left.Flags[(int)Flag.Length]
				&& left.Length != right.Length)
				return false;

			if (left.Flags[(int)Flag.Height]
				&& left.Height != right.Height)
				return false;

			if (left.Flags[(int)Flag.Height]
				&& left.MicroHeight != right.MicroHeight)
				return false;

			return true;
		}
		
		public static bool operator!=(BlockIE left, BlockIE right)
		{
			return !(left == right);
		}
	}
}
