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

namespace Assets.IE.V2
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
        static readonly int SIZE = Mathf.CeilToInt((float)Flag.COUNT / 8.0f) + 6 * 1 + 2 * 2 + 2 * 4;

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
        private byte m_MaterialFamilyLength;
        private byte[] m_MaterialFamilyName;
        public string MaterialFamily
        {
            get
            {
                char[] name = new char[m_MaterialFamilyLength];
                for (int i = 0; i < name.Length; ++i)
                    name[i] = (char)m_MaterialFamilyName[i];
                
                return new string(name);
            }
            set
            {
                m_MaterialFamilyLength = (byte)value.Length;
                m_MaterialFamilyName = new byte[m_MaterialFamilyLength];
                for (int i = 0; i < m_MaterialFamilyLength; ++i)
                    m_MaterialFamilyName[i] = (byte)value[i];
                bool flagSet = !(m_MaterialFamilyLength == 0);
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

        public V3.BlockIE Upgrade()
        {
            var flags = new List<bool>((int)Flag.COUNT);
            for (int i = 0; i < (int)Flag.COUNT; ++i)
                flags.Add(Flags[i]);
            for (int i = (int)Flag.COUNT; i < V3.BlockIE.FlagCount; ++i)
                flags.Add(false);
            string monsterFamily = "";
            if (MonsterID > 0)
                monsterFamily = Monsters.MonsterFamilies[MonsterID].Name;
            string propFamily = "";
            if (PropID > 0)
                propFamily = Props.PropFamilies[PropID].FamilyName;

            var oPos = GameUtils.PosFromID(StructureID, 8, 8);// new Vector2Int(StructureID % 8, Mathf.FloorToInt(StructureID / 8f));
            var nPos = new Vector2Int(oPos.x + 12, oPos.y + 12);
            var nID = nPos.y * Def.MaxStrucSide + nPos.x;

            var ie = new V3.BlockIE()
            {
                StructureID = (ushort)nID,
                Layer = Layer,
                Stair = Stair,
                Flags = flags,
                Rotation = BlockRotation,
                MonsterFamily = monsterFamily,
                PropFamily = propFamily,
                MaterialFamily = MaterialFamily,
                BlockVoid = Def.BlockVoid.NORMAL,
                BlockType = blockType,
                Length = Length,
                Height = Height,
                MicroHeight = 0f
            };

            return ie;
        }

        public void SetDefault()
        {
            _Flags.SetAll(false);
            Layer = 0;
            Stair = Def.StairState.NONE;
            m_Monster = 0;
            m_Prop = 0;
            m_MaterialFamilyLength = 0;
            m_MaterialFamilyName = null;
            m_BlockType = Def.BlockType.NORMAL;
            m_Length = 0.5f;
            m_Height = 0.0f;
        }

        //public void Apply(BlockComponent block)
        //{
        //    int locked = 0;
        //    block.Stair = Stair == StairType.COUNT ? StairType.NONE : Stair;
        //    block.Anchor = Anchor;
        //    if (_Flags[(int)Flag.Rotation] && m_Rotation != BlockRotation.COUNT)
        //    {
        //        block.Rotation = m_Rotation;
        //        ++locked;
        //    }

        //    if (_Flags[(int)Flag.Monster] && m_Monster > 0)
        //    {
        //        if (block.Monster != null)
        //            block.Monster.ReceiveDamage(Def.DamageType.UNAVOIDABLE, block.Monster.GetTotalHealth());
        //        //var monster = Monsters.MonsterInfos[m_Monster];
        //        var mon = new GameObject($"Monster_{MonsterScript.MonsterID++}");
        //        block.Monster = Monsters.AddMonsterComponent(mon, m_Monster);
        //        //block.Monster = mon.AddComponent<MonsterScript>();

        //        mon.transform.Translate(block.Pilar.transform.position, Space.World);
        //        mon.transform.Translate(new Vector3(0.0f, block.Height + block.MicroHeight, 0.0f), Space.World);
        //        block.Monster = Monsters.AddMonsterComponent(mon, m_Monster);
        //        block.Monster.InitMonster();
        //        //block.Monster.SetMonster(m_Monster);
        //        block.Monster.enabled = true;
        //        block.Monster.SpriteSR.enabled = false;
        //        //block.Monster.SpriteBC.enabled = false;
        //        block.Monster.SpriteCC.enabled = false;
        //        block.Monster.ShadowSR.enabled = false;
        //        //block.Monster.Struc = block.Pilar.Struc;
        //        var facing = block.Monster.Facing;
        //        float nChance = UnityEngine.Random.value; //(float)Manager.Mgr.BuildRNG.NextDouble();
        //        if (nChance >= 0.5f)
        //            facing.Horizontal = SpriteHorizontal.RIGHT;
        //        nChance = UnityEngine.Random.value; //(float)Manager.Mgr.BuildRNG.NextDouble();
        //        if (nChance >= 0.5f)
        //            facing.Vertical = SpriteVertical.UP;
        //        block.Monster.Facing = facing;
        //        ++locked;
        //    }

        //    if (_Flags[(int)Flag.Prop] && m_Prop > 0)
        //    {
        //        if (block.Prop != null)
        //            block.Prop.ReceiveDamage(Def.DamageType.UNAVOIDABLE, block.Prop.GetTotalHealth());
        //        var propFamily = Props.PropFamilies[m_Prop];
        //        if (propFamily.Props.Length == 0)
        //            throw new Exception($"This prop {propFamily.FamilyName}, does not have any available prop.");

        //        var propID = UnityEngine.Random.Range(0, propFamily.Props.Length); //Manager.Mgr.BuildRNG.Next(0, propFamily.Props.Length);
        //        var prop = new GameObject($"Prop_{PropScript.PropID++}");
        //        block.Prop = prop.AddComponent<PropScript>();
        //        block.Prop.SetProp(m_Prop, propID);
        //        block.Prop.Block = block;
        //        block.Prop.enabled = true;
        //        block.Prop.SpriteSR.enabled = false;
        //        block.Prop.SpriteBC.enabled = false;
        //        if (block.Prop.ShadowSR != null)
        //            block.Prop.ShadowSR.enabled = false;
        //        if (block.Prop.PropLight != null)
        //            block.Prop.PropLight.enabled = false;
        //        float nChance = UnityEngine.Random.value; //(float)Manager.Mgr.BuildRNG.NextDouble();
        //        var facing = block.Prop.Facing;
        //        if (nChance >= 0.5f)
        //            facing.Horizontal = SpriteHorizontal.RIGHT;
        //        block.Prop.Facing = facing;
        //        ++locked;
        //    }

        //    if (_Flags[(int)Flag.BlockType] && m_BlockType != BlockType.COUNT)
        //    {
        //        block.blockType = m_BlockType;
        //        ++locked;
        //    }

        //    if (_Flags[(int)Flag.MaterialType] && m_MaterialFamilyLength > 0)
        //    {
        //        block.MaterialFmly = BlockMaterial.MaterialFamilies[BlockMaterial.FamilyDict[MaterialFamily]];
        //        ++locked;
        //    }

        //    var maxLength = Mathf.Abs(BlockMeshDef.MidMesh.VertexHeight[block.blockType == BlockType.WIDE ? 1 : 0].y);
        //    if (_Flags[(int)Flag.Length] && m_Length >= 0.5f && m_Length <= maxLength)
        //    {
        //        block.Length = m_Length;
        //        ++locked;
        //    }

        //    if (_Flags[(int)Flag.Height] && !float.IsInfinity(m_Height) && !float.IsNaN(m_Height))
        //    {
        //        block.Height = m_Height;
        //        ++locked;
        //    }
        //    // Theres something or everything locked
        //    if (locked == 7)
        //    {
        //        block.Locked = BlockLock.Locked;
        //    }
        //    else if (locked > 0)
        //    {
        //        block.Locked = BlockLock.SemiLocked;
        //    }
        //    else
        //    {
        //        block.Locked = BlockLock.Unlocked;
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

            if (left._Flags[(int)Flag.MaterialType])
            {
                if (left.m_MaterialFamilyLength != right.m_MaterialFamilyLength)
                    return false;
                for(int i = 0; i < left.m_MaterialFamilyLength; ++i)
                {
                    if (left.m_MaterialFamilyName[i] != right.m_MaterialFamilyName[i])
                        return false;
                }
            }

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
                tempBytes[i] = tempArray[lastIdx++];
            }
            _Flags = new BitArray(tempBytes);
            m_Rotation = (Def.RotationState)tempArray[lastIdx++];

            m_Monster = BitConverter.ToUInt16(tempArray, lastIdx);
            lastIdx += 2;

            m_Prop = BitConverter.ToUInt16(tempArray, lastIdx);
            lastIdx += 2;

            m_MaterialFamilyLength = tempArray[lastIdx++];
            m_MaterialFamilyName = new byte[m_MaterialFamilyLength];

            m_BlockType = (Def.BlockType)tempArray[lastIdx++];

            m_Length = BitConverter.ToSingle(tempArray, lastIdx);
            lastIdx += 4;

            m_Height = BitConverter.ToSingle(tempArray, lastIdx);
            lastIdx += 4;

            tempArray = new byte[m_MaterialFamilyLength];
            lastIdx = 0;
            fr.Read(tempArray, 0, tempArray.Length);
            for(int i = 0; i < m_MaterialFamilyLength; ++i)
            {
                m_MaterialFamilyName[i] = tempArray[lastIdx++];
            }

            if (lastIdx != tempArray.Length)
                throw new Exception("Not everything has been read, BlockDefIE");

            return true;
        }

        //public bool WriteBlock(ref FileStream fw)
        //{
        //    var tempArray = new byte[SIZE + m_MaterialFamilyLength];

        //    if (Layer == 0)
        //        throw new Exception("Trying to save an invalid block!");

        //    tempArray[0] = StructureID;
        //    tempArray[1] = Layer;
        //    tempArray[2] = (byte)Stair;
        //    _Flags.CopyTo(tempArray, 3);
        //    int lastIdx = 3 + Mathf.CeilToInt((float)Flag.COUNT / 8.0f);
        //    tempArray[lastIdx++] = (byte)m_Rotation;

        //    // MonsterID
        //    var tempBytes = BitConverter.GetBytes(m_Monster);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    // PropID
        //    tempBytes = BitConverter.GetBytes(m_Prop);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    // MaterialType
        //    tempArray[lastIdx++] = m_MaterialFamilyLength;

        //    // BlockType
        //    tempArray[lastIdx++] = (byte)m_BlockType;

        //    // Length
        //    tempBytes = BitConverter.GetBytes(m_Length);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    // Height
        //    tempBytes = BitConverter.GetBytes(m_Height);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    for (int i = 0; i < m_MaterialFamilyLength; ++i)
        //        tempArray[lastIdx++] = m_MaterialFamilyName[i];

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
            hashCode = hashCode * -1521134295 + m_MaterialFamilyLength.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(m_MaterialFamilyName);
            hashCode = hashCode * -1521134295 + m_BlockType.GetHashCode();
            hashCode = hashCode * -1521134295 + m_Length.GetHashCode();
            hashCode = hashCode * -1521134295 + m_Height.GetHashCode();
            return hashCode;
        }
    }
}
