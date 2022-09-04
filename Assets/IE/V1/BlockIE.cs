/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System.Collections.Generic;
using System.Data.Common;
using System.Collections.Specialized;
using System.IO;
using System.Collections;
using System;
using UnityEngine;
using Assets;

namespace Assets.IE.V1
{
    public class BlockIE
    {
        public enum Flag
        {
            Anchor,
            Rotation,
            Monster,
            Prop,
            MaterialType,
            BlockType,
            Length,
            Height,
            COUNT
        }
        static readonly int SIZE = Mathf.CeilToInt((float)Flag.COUNT / 8.0f) + 5 * 1 + 3 * 2 + 2 * 4;

        public byte StructureID;
        public byte Layer;
        public Def.StairState Stair;
        private BitArray _Flags;
        public BitArray Flags
        {
            get
            {
                return _Flags;
            }
        }

        public bool Anchor
        {
            get
            {
                return _Flags.Get((int)Flag.Anchor);
            }
            set
            {
                _Flags.Set((int)Flag.Anchor, value);
            }
        }
        private Def.RotationState m_Rotation;
        public Def.RotationState BlockRotation
        {
            get
            {
                return m_Rotation;
            }
            set
            {
                m_Rotation = value;
                bool flagSet = m_Rotation != Def.RotationState.COUNT;
                _Flags.Set((int)Flag.Rotation, flagSet);
            }
        }
        private ushort m_Monster;
        public int MonsterID
        {
            get
            {
                return m_Monster;
            }
            set
            {
                m_Monster = (ushort)value;
                bool flagSet = !(m_Monster == ushort.MaxValue);
                _Flags.Set((int)Flag.Monster, flagSet);
            }
        }
        private ushort m_Prop;
        public int PropID
        {
            get
            {
                return m_Prop;
            }
            set
            {
                m_Prop = (ushort)value;
                bool flagSet = !(m_Prop == ushort.MaxValue);
                _Flags.Set((int)Flag.Prop, flagSet);
            }
        }
        private ushort m_MaterialTypeID;
        public int MaterialTypeID
        {
            get
            {
                return m_MaterialTypeID;
            }
            set
            {
                m_MaterialTypeID = (ushort)value;
                bool flagSet = !(m_MaterialTypeID == 0);
                _Flags.Set((int)Flag.MaterialType, flagSet);
            }
        }
        private Def.BlockType m_BlockType;
        public Def.BlockType blockType
        {
            get
            {
                return m_BlockType;
            }
            set
            {
                m_BlockType = value;
                bool flagSet = !(m_BlockType == Def.BlockType.COUNT);
                _Flags.Set((int)Flag.BlockType, flagSet);
            }
        }
        private float m_Length;
        public float Length
        {
            get
            {
                return m_Length;
            }
            set
            {
                m_Length = value;
                bool flagSet = !(m_Length < 0.5);
                _Flags.Set((int)Flag.Length, flagSet);
            }
        }
        private float m_Height;
        public float Height
        {
            get
            {
                return m_Height;
            }
            set
            {
                m_Height = value;
                bool flagSet = !(float.IsInfinity(m_Height) || float.IsNaN(m_Height));
                _Flags.Set((int)Flag.Height, flagSet);
            }
        }

        public BlockIE()
        {
            _Flags = new BitArray((int)Flag.COUNT);
            SetDefault();
        }

        public V2.BlockIE Upgrade()
        {
            var matFamily = BlockMaterial.MaterialFamilies[m_MaterialTypeID].FamilyInfo.FamilyName;

            var ie = new V2.BlockIE()
            {
                StructureID = StructureID,
                Layer = Layer,
                Stair = Stair,
                Anchor = Anchor,
                BlockRotation = BlockRotation,
                MonsterID = MonsterID,
                PropID = PropID,
                MaterialFamily = matFamily,
                blockType = blockType,
                Length = Length,
                Height = Height
            };
            for (int i = 0; i < (int)Flag.COUNT; ++i)
                ie.Flags.Set(i, Flags[i]);
            return ie;
        }

        public void SetDefault()
        {
            _Flags.SetAll(false);
            Layer = 0;
            Stair = Def.StairState.NONE;
            m_Monster = 0;
            m_Prop = 0;
            m_MaterialTypeID = 0;
            m_BlockType = Def.BlockType.NORMAL;
            m_Length = 0.5f;
            m_Height = 0.0f;
        }

        //public void Apply(CBlockEdit block)
        //{
        //    int locked = 0;
        //    block.SetStairState(Stair == Definitions.StairState.COUNT ? Definitions.StairState.NONE : Stair);
        //    block.SetAnchor(Anchor);
        //    if (_Flags[(int)Flag.Rotation] && m_Rotation != Definitions.RotationState.COUNT)
        //    {
        //        block.SetRotation(m_Rotation);
        //        ++locked;
        //    }

        //    if (_Flags[(int)Flag.Monster] && m_Monster > 0)
        //    {
        //        if (block.GetMonster() != null)
        //            block.GetMonster().ReceiveDamage(Def.DamageType.UNAVOIDABLE, block.GetMonster().GetTotalHealth());
        //        //var monster = Monsters.MonsterSprites[m_Monster];
        //        var mon = new GameObject($"Monster_{MonsterScript.MonsterID++}");
        //        block.SetMonster(Monsters.AddMonsterComponent(mon, m_Monster));
        //        //block.Monster = mon.AddComponent<MonsterScript>();

        //        mon.transform.Translate(block.GetPilar().transform.position, Space.World);
        //        mon.transform.Translate(new Vector3(0.0f, block.GetHeight() + block.GetMicroHeight(), 0.0f), Space.World);
        //        block.GetMonster().InitMonster();
        //        //block.Monster.SetMonster(m_Monster);
        //        block.GetMonster().enabled = true;
        //        block.GetMonster().SpriteSR.enabled = false;
        //        //block.Monster.SpriteBC.enabled = false;
        //        block.GetMonster().SpriteCC.enabled = false;
        //        block.GetMonster().ShadowSR.enabled = false;
        //        //block.Monster.Struc = block.Pilar.Struc;
        //        var facing = block.GetMonster().Facing;
        //        float nChance = UnityEngine.Random.value; //(float)Manager.Mgr.BuildRNG.NextDouble();
        //        if (nChance >= 0.5f)
        //            facing.Horizontal = SpriteHorizontal.RIGHT;
        //        nChance = UnityEngine.Random.value; //(float)Manager.Mgr.BuildRNG.NextDouble();
        //        if (nChance >= 0.5f)
        //            facing.Vertical = SpriteVertical.UP;
        //        block.GetMonster().Facing = facing;
        //        ++locked;
        //    }

        //    if (_Flags[(int)Flag.Prop] && m_Prop > 0)
        //    {
        //        if (block.GetProp() != null)
        //            block.GetProp().ReceiveDamage(Def.DamageType.UNAVOIDABLE, block.GetProp().GetTotalHealth());
        //        var propFamily = Props.PropFamilies[m_Prop];
        //        if (propFamily.Props.Length == 0)
        //            throw new Exception($"This prop {propFamily.FamilyName}, does not have any available prop.");

        //        var propID = UnityEngine.Random.Range(0, propFamily.Props.Length); //Manager.Mgr.BuildRNG.Next(0, propFamily.Props.Length);
        //        var prop = new GameObject($"Prop_{PropScript.PropID++}");
        //        block.SetProp(prop.AddComponent<PropScript>());
        //        block.GetProp().SetProp(m_Prop, propID);
        //        block.GetProp().Block = block;
        //        block.GetProp().enabled = true;
        //        block.GetProp().SpriteSR.enabled = false;
        //        block.GetProp().SpriteBC.enabled = false;
        //        if (block.GetProp().ShadowSR != null)
        //            block.GetProp().ShadowSR.enabled = false;
        //        if (block.GetProp().PropLight != null)
        //            block.GetProp().PropLight.enabled = false;
        //        float nChance = UnityEngine.Random.value; //(float)Manager.Mgr.BuildRNG.NextDouble();
        //        var facing = block.GetProp().Facing;
        //        if (nChance >= 0.5f)
        //            facing.Horizontal = SpriteHorizontal.RIGHT;
        //        block.GetProp().Facing = facing;
        //        ++locked;
        //    }

        //    if (_Flags[(int)Flag.BlockType] && m_BlockType != Definitions.BlockType.COUNT)
        //    {
        //        block.SetBlockType(m_BlockType);
        //        ++locked;
        //    }

        //    if (_Flags[(int)Flag.MaterialType] && m_MaterialTypeID > 0)
        //    {
        //        block.SetMaterialFamily(BlockMaterial.MaterialFamilies[m_MaterialTypeID]);
        //        //block.MaterialTypeID = m_MaterialTypeID;
        //        ++locked;
        //    }

        //    var maxLength = Mathf.Abs(BlockMeshDef.MidMesh.VertexHeight[block.GetBlockType() == Definitions.BlockType.WIDE ? 1 : 0].y);
        //    if (_Flags[(int)Flag.Length] && m_Length >= 0.5f && m_Length <= maxLength)
        //    {
        //        block.SetLength(m_Length);
        //        ++locked;
        //    }

        //    if (_Flags[(int)Flag.Height] && !float.IsInfinity(m_Height) && !float.IsNaN(m_Height))
        //    {
        //        block.SetHeight(m_Height);
        //        ++locked;
        //    }
        //    // Theres something or everything locked
        //    if (locked == 7)
        //    {
        //        block.SetLockState(Definitions.LockState.Locked);
        //    }
        //    else if (locked > 0)
        //    {
        //        block.SetLockState(Definitions.LockState.SemiLocked);
        //    }
        //    else
        //    {
        //        block.SetLockState(Definitions.LockState.Unlocked);
        //    }
        //}

        public static bool operator ==(BlockIE left, BlockIE right)
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

            for (int i = 0; i < (int)Flag.COUNT; ++i)
            {
                if (left._Flags[i] != right._Flags[i])
                    return false;
            }

            if (left._Flags[(int)Flag.Rotation]
                && left.m_Rotation != right.m_Rotation)
                return false;

            if (left._Flags[(int)Flag.Monster]
                && left.MonsterID != right.MonsterID)
                return false;

            if (left._Flags[(int)Flag.Prop]
                && left.PropID != right.PropID)
                return false;

            if (left._Flags[(int)Flag.MaterialType]
                && left.MaterialTypeID != right.MaterialTypeID)
                return false;

            if (left._Flags[(int)Flag.BlockType]
                && left.blockType != right.blockType)
                return false;

            if (left._Flags[(int)Flag.Length]
                && left.Length != right.Length)
                return false;

            if (left._Flags[(int)Flag.Height]
                && left.Height != right.Height)
                return false;

            return true;
        }

        public static bool operator !=(BlockIE left, BlockIE right)
        {
            return !(left == right);
        }

        public bool ReadBlock(ref FileStream fr)
        {
            if ((fr.Length - fr.Position) < SIZE)
                return false;

            var tempArray = new byte[SIZE];
            fr.Read(tempArray, 0, tempArray.Length);

            StructureID = tempArray[0];
            Layer = tempArray[1];
            Stair = (Def.StairState)tempArray[2];
            int lastIdx = 3;
            _Flags.Length = (int)Flag.COUNT;
            var tempBytes = new byte[Mathf.CeilToInt((float)Flag.COUNT / 8.0f)];
            for (int i = 0; i < tempBytes.Length; ++i)
            {
                tempBytes[i] = tempArray[lastIdx];
                ++lastIdx;
            }
            _Flags = new BitArray(tempBytes);
            m_Rotation = (Def.RotationState)tempArray[lastIdx];
            ++lastIdx;

            m_Monster = BitConverter.ToUInt16(tempArray, lastIdx);
            lastIdx += 2;

            m_Prop = BitConverter.ToUInt16(tempArray, lastIdx);
            lastIdx += 2;

            m_MaterialTypeID = BitConverter.ToUInt16(tempArray, lastIdx);
            lastIdx += 2;

            m_BlockType = (Def.BlockType)tempArray[lastIdx];
            ++lastIdx;

            m_Length = BitConverter.ToSingle(tempArray, lastIdx);
            lastIdx += 4;

            m_Height = BitConverter.ToSingle(tempArray, lastIdx);
            lastIdx += 4;

            if (lastIdx != tempArray.Length)
                throw new Exception("Not everything has been read, BlockDefIE");

            return true;
        }

        //public bool WriteBlock(ref FileStream fw)
        //{
        //    var tempArray = new byte[SIZE];

        //    if (Layer == 0)
        //        throw new Exception("Trying to save an invalid block!");

        //    tempArray[0] = StructureID;
        //    tempArray[1] = Layer;
        //    tempArray[2] = (byte)Stair;
        //    _Flags.CopyTo(tempArray, 3);
        //    int lastIdx = 3 + Mathf.CeilToInt((float)Flag.COUNT / 8.0f);
        //    tempArray[lastIdx] = (byte)m_Rotation;
        //    ++lastIdx;

        //    // MonsterID
        //    var tempBytes = BitConverter.GetBytes(m_Monster);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx] = tempBytes[i];
        //        ++lastIdx;
        //    }

        //    // PropID
        //    tempBytes = BitConverter.GetBytes(m_Prop);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx] = tempBytes[i];
        //        ++lastIdx;
        //    }

        //    // MaterialType
        //    tempBytes = BitConverter.GetBytes(m_MaterialTypeID);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx] = tempBytes[i];
        //        ++lastIdx;
        //    }

        //    tempArray[lastIdx] = (byte)m_BlockType;
        //    ++lastIdx;

        //    // Length
        //    tempBytes = BitConverter.GetBytes(m_Length);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx] = tempBytes[i];
        //        ++lastIdx;
        //    }

        //    // Height
        //    tempBytes = BitConverter.GetBytes(m_Height);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx] = tempBytes[i];
        //        ++lastIdx;
        //    }

        //    if (lastIdx != tempArray.Length)
        //        throw new Exception("Not everything has been written, BlockDefIE");

        //    fw.Write(tempArray, 0, tempArray.Length);

        //    return true;
        //}

        public override bool Equals(object obj)
        {
            if (!(obj is BlockIE))
                return false;
            var iE = obj as BlockIE;
            return iE == this;
        }

        public override int GetHashCode()
        {
            var hashCode = 1038191710;
            hashCode = hashCode * -1521134295 + StructureID.GetHashCode();
            hashCode = hashCode * -1521134295 + Layer.GetHashCode();
            hashCode = hashCode * -1521134295 + Stair.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<BitArray>.Default.GetHashCode(_Flags);
            hashCode = hashCode * -1521134295 + m_Rotation.GetHashCode();
            hashCode = hashCode * -1521134295 + m_Monster.GetHashCode();
            hashCode = hashCode * -1521134295 + m_Prop.GetHashCode();
            hashCode = hashCode * -1521134295 + m_MaterialTypeID.GetHashCode();
            hashCode = hashCode * -1521134295 + m_BlockType.GetHashCode();
            hashCode = hashCode * -1521134295 + m_Length.GetHashCode();
            hashCode = hashCode * -1521134295 + m_Height.GetHashCode();
            return hashCode;
        }
    }
}
