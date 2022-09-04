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
    public class LayerIE
    {
        private enum Flag
        {
            IsLinkedLayer,
            RandomBlockRotation,
            RandomMicroHeight,
            RandomBlockLength,
            RandomProps,
            SpawnMonsters,
            SpawnZoneMonsters,
            LayerMonstersRespawn,
            HasEffects,
            BlockFloat,
            COUNT
        }
        static readonly int STATIC_SIZE = Mathf.CeilToInt((float)Flag.COUNT / 8.0f) +
            4 * 1 + 3 * 2 + 13 * 4;

        public byte Layer;
        private BitArray Flags;

        public bool EnableRandomRotation
        {
            get
            {
                return Flags[(int)Flag.RandomBlockRotation];
            }
            set
            {
                if (IsLinkedLayer)
                    return;
                Flags.Set((int)Flag.RandomBlockRotation, value);
            }
        }
        public Def.RotationState DefaultBlockRotation;

        public List<int> SubMaterials
        {
            get
            {
                var list = new List<int>(m_SubMaterialNum);
                for (int i = 0; i < m_SubMaterialNum; ++i)
                    list.Add(m_SubMaterials[i]);
                return list;
            }
            set
            {
                m_SubMaterialNum = (byte)value.Count;
                m_SubMaterials = new ushort[m_SubMaterialNum];
                for (int i = 0; i < m_SubMaterialNum; ++i)
                    m_SubMaterials[i] = (ushort)value[i];
            }
        }
        public List<float> SubMaterialChances
        {
            get
            {
                return new List<float>(m_SubMaterialChances);
            }
            set
            {
                m_SubMaterialNum = (byte)value.Count;
                m_SubMaterialChances = value.ToArray();
            }
        }

        public float BlockHeight;

        public bool EnableRandomMicroHeight
        {
            get
            {
                return Flags[(int)Flag.RandomMicroHeight];
            }
            set
            {
                if (IsLinkedLayer)
                    return;
                Flags.Set((int)Flag.RandomMicroHeight, value);
            }
        }
        public float BlockMicroHeightMin;
        public float BlockMicroHeightMax;

        public bool EnableRandomBlockLenght
        {
            get
            {
                return Flags[(int)Flag.RandomBlockLength];
            }
            set
            {
                if (IsLinkedLayer)
                    return;
                Flags.Set((int)Flag.RandomBlockLength, value);
            }
        }
        public float BlockLenghtMin;
        public float BlockLenghtMax;

        public bool EnableRandomProps
        {
            get
            {
                return Flags[(int)Flag.RandomProps];
            }
            set
            {
                if (IsLinkedLayer)
                    return;
                Flags.Set((int)Flag.RandomProps, value);
            }
        }
        public float PropChance;
        public List<int> Props
        {
            get
            {
                var list = new List<int>(m_PropNum);
                for (int i = 0; i < m_PropNum; ++i)
                    list.Add(m_Props[i]);
                return list;
            }
            set
            {
                m_PropNum = (ushort)value.Count;
                m_Props = new ushort[m_PropNum];
                for (int i = 0; i < m_PropNum; ++i)
                    m_Props[i] = (ushort)value[i];
            }
        }
        public List<float> PropChances
        {
            get
            {
                return new List<float>(m_PropChances);
            }
            set
            {
                m_PropNum = (ushort)value.Count;
                m_PropChances = value.ToArray();
            }
        }
        public float PropNoSpawnRadius;

        public bool SpawnMonsters
        {
            get
            {
                return Flags[(int)Flag.SpawnMonsters];
            }
            set
            {
                if (IsLinkedLayer)
                    return;
                Flags.Set((int)Flag.SpawnMonsters, value);
            }
        }
        public bool SpawnZoneMonsters
        {
            get
            {
                return Flags[(int)Flag.SpawnZoneMonsters];
            }
            set
            {
                if (IsLinkedLayer)
                    return;
                Flags.Set((int)Flag.SpawnZoneMonsters, value);
            }
        }
        public bool LayerMonstersRespawn
        {
            get
            {
                return Flags[(int)Flag.LayerMonstersRespawn];
            }
            set
            {
                if (IsLinkedLayer)
                    return;
                Flags.Set((int)Flag.LayerMonstersRespawn, value);
            }
        }
        public List<int> Monsters
        {
            get
            {
                var list = new List<int>(m_MonsterNum);
                for (int i = 0; i < m_MonsterNum; ++i)
                    list.Add(m_Monsters[i]);
                return list;
            }
            set
            {
                m_MonsterNum = (ushort)value.Count;
                m_Monsters = new ushort[m_MonsterNum];
                for (int i = 0; i < m_MonsterNum; ++i)
                    m_Monsters[i] = (ushort)value[i];
            }
        }
        public List<float> MonsterChances
        {
            get
            {
                return new List<float>(m_MonsterChances);
            }
            set
            {
                m_MonsterNum = (ushort)value.Count;
                m_MonsterChances = value.ToArray();
            }
        }
        public float MonsterChance;

        public bool HasEffect
        {
            get
            {
                return Flags[(int)Flag.HasEffects];
            }
            set
            {
                if (IsLinkedLayer)
                    return;
                Flags.Set((int)Flag.HasEffects, value);
            }
        }
        public List<int> Effects
        {
            get
            {
                var list = new List<int>(m_EffectNum);
                for (int i = 0; i < m_EffectNum; ++i)
                    list.Add(m_Effects[i]);
                return list;
            }
            set
            {
                m_EffectNum = (ushort)value.Count;
                m_Effects = new ushort[m_EffectNum];
                for (int i = 0; i < m_EffectNum; ++i)
                    m_Effects[i] = (ushort)value[i];
            }
        }

        public bool EnableBlockFloat
        {
            get
            {
                return Flags[(int)Flag.BlockFloat];
            }
            set
            {
                if (IsLinkedLayer)
                    return;
                Flags.Set((int)Flag.BlockFloat, value);
            }
        }
        public float BlockFloatMin;
        public float BlockFloatMax;
        public float BlockFloatSpeed;

        public float RandomWideBlockChance;

        public float RandomStairBlockChance;

        public bool IsLinkedLayer
        {
            get
            {
                return Flags[(int)Flag.IsLinkedLayer];
            }
            set
            {
                if (value)
                    Flags.SetAll(false);
                Flags.Set((int)Flag.IsLinkedLayer, value);
            }
        }
        public List<int> LinkedLayers
        {
            get
            {
                var list = new List<int>(m_LayerNum);
                for (int i = 0; i < m_LayerNum; ++i)
                    list.Add(m_LinkedLayers[i]);
                return list;
            }
            set
            {
                m_LayerNum = (byte)value.Count;
                m_LinkedLayers = new byte[m_LayerNum];
                for (int i = 0; i < m_LayerNum; ++i)
                    m_LinkedLayers[i] = (byte)value[i];
            }
        }
        public List<float> LinkedLayerChances
        {
            get
            {
                return new List<float>(m_LinkedLayerChances);
            }
            set
            {
                m_LayerNum = (byte)value.Count;
                m_LinkedLayerChances = value.ToArray();
            }
        }

        public byte m_SubMaterialNum;
        public ushort m_PropNum;
        public ushort m_MonsterNum;
        public ushort m_EffectNum;
        public byte m_LayerNum;

        private ushort[] m_SubMaterials;
        private float[] m_SubMaterialChances;

        private ushort[] m_Props;
        private float[] m_PropChances;

        private ushort[] m_Monsters;
        private float[] m_MonsterChances;

        private ushort[] m_Effects;

        private byte[] m_LinkedLayers;
        private float[] m_LinkedLayerChances;

        public LayerIE()
        {
            Flags = new BitArray((int)Flag.COUNT);
            Flags.SetAll(false);

            Layer = 0;
            m_SubMaterialNum = 0;
            m_SubMaterials = null;
            m_SubMaterialChances = null;

            DefaultBlockRotation = (byte)Def.RotationState.Default;

            BlockHeight = 0.0f;
            BlockMicroHeightMin = 0.0f;
            BlockMicroHeightMax = 0.0f;

            BlockLenghtMin = 1.0f;
            BlockLenghtMax = 1.0f;

            PropChance = 0.0f;
            m_PropNum = 0;
            m_Props = null;
            m_PropChances = null;
            PropNoSpawnRadius = 0.0f;

            MonsterChance = 0.0f;
            m_MonsterNum = 0;
            m_Monsters = null;
            m_MonsterChances = null;

            m_EffectNum = 0;
            m_Effects = null;

            BlockFloatMin = 0.0f;
            BlockFloatMax = 0.0f;
            BlockFloatSpeed = 0.0f;

            RandomWideBlockChance = 0.0f;
            RandomStairBlockChance = 0.0f;

            m_LayerNum = 0;
            m_LinkedLayers = null;
            m_LinkedLayerChances = null;

        }

        //public LayerInfo ToLayerInfo()
        //{
        //    var info = new LayerInfo
        //    {
        //        Layer = Layer,
        //        MaterialTypes = new List<int>(m_SubMaterialNum),
        //        MaterialTypeChances = new List<float>(m_SubMaterialNum),
        //        EnableRandomRotation = EnableRandomRotation,
        //        DefaultRotation = DefaultBlockRotation,
        //        BlockHeight = BlockHeight,
        //        EnableRandomMicroHeight = EnableRandomMicroHeight,
        //        BlockMicroHeightMin = BlockMicroHeightMin,
        //        BlockMicroHeightMax = BlockMicroHeightMax,
        //        EnableRandomBlockLength = EnableRandomBlockLenght,
        //        BlockLengthMin = BlockLenghtMin,
        //        BlockLengthMax = BlockLenghtMax,
        //        EnableRandomProps = EnableRandomProps,
        //        PropChance = PropChance,
        //        Props = new List<int>(m_PropNum),
        //        PropChances = new List<float>(m_PropNum),
        //        PropNoSpawnRadius = PropNoSpawnRadius,
        //        SpawnMonsters = SpawnMonsters,
        //        SpawnZoneMonsters = SpawnZoneMonsters,
        //        LayerMonstersRespawn = LayerMonstersRespawn,
        //        MonsterChance = MonsterChance,
        //        Monsters = new List<int>(m_MonsterNum),
        //        MonsterChances = new List<float>(m_MonsterNum),
        //        HasEffects = HasEffect,
        //        Effects = new List<int>(m_EffectNum),
        //        EnableBlockFloat = EnableBlockFloat,
        //        BlockFloatMin = BlockFloatMin,
        //        BlockFloatMax = BlockFloatMax,
        //        BlockFloatSpeed = BlockFloatSpeed,
        //        RandomWideBlockChance = RandomWideBlockChance,
        //        RandomStairBlockChance = RandomStairBlockChance,
        //        IsLinkedLayer = IsLinkedLayer,
        //        LinkedLayers = new List<int>(m_LayerNum),
        //        LinkedLayerChances = new List<float>(m_LayerNum),
        //        //IE = this
        //    };

        //    for (int i = 0; i < m_SubMaterialNum; ++i)
        //    {
        //        info.MaterialTypes.Add(m_SubMaterials[i]);
        //        info.MaterialTypeChances.Add(m_SubMaterialChances[i]);
        //    }

        //    for (int i = 0; i < m_PropNum; ++i)
        //    {
        //        info.Props.Add(m_Props[i]);
        //        info.PropChances.Add(m_PropChances[i]);
        //    }

        //    for (int i = 0; i < m_MonsterNum; ++i)
        //    {
        //        info.Monsters.Add(m_Monsters[i]);
        //        info.MonsterChances.Add(m_MonsterChances[i]);
        //    }

        //    for (int i = 0; i < m_EffectNum; ++i)
        //    {
        //        info.Effects.Add(m_Effects[i]);
        //    }

        //    for (int i = 0; i < m_LayerNum; ++i)
        //    {
        //        info.LinkedLayers.Add(m_LinkedLayers[i]);
        //        info.LinkedLayerChances.Add(m_LinkedLayerChances[i]);
        //    }

        //    return info;
        //}

        //public LayerInfo ToLayerEInfo()
        //{
        //    throw new Exception("Not implemented.");
        //}

        public V2.LayerIE Upgrade()
        {
            var materialFamilies = new List<string>(m_SubMaterialNum);
            for (int i = 0; i < m_SubMaterialNum; ++i)
            {
                var name = BlockMaterial.MaterialFamilies[m_SubMaterials[i]].FamilyInfo.FamilyName;
                materialFamilies.Add(name);
            }
            var ie = new V2.LayerIE
            {
                Layer = Layer,
                EnableRandomRotation = EnableRandomRotation,
                DefaultBlockRotation = DefaultBlockRotation,
                MaterialFamilyChances = SubMaterialChances,
                MaterialFamilies = materialFamilies,
                BlockHeight = BlockHeight,
                EnableRandomMicroHeight = EnableRandomMicroHeight,
                BlockMicroHeightMin = BlockMicroHeightMin,
                BlockMicroHeightMax = BlockMicroHeightMax,
                EnableRandomBlockLenght = EnableRandomBlockLenght,
                BlockLenghtMin = BlockLenghtMin,
                BlockLenghtMax = BlockLenghtMax,
                EnableRandomProps = EnableRandomProps,
                PropChance = PropChance,
                Props = Props,
                PropChances = PropChances,
                PropNoSpawnRadius = PropNoSpawnRadius,
                SpawnMonsters = SpawnMonsters,
                SpawnZoneMonsters = SpawnZoneMonsters,
                LayerMonstersRespawn = LayerMonstersRespawn,
                Monsters = Monsters,
                MonsterChances = MonsterChances,
                MonsterChance = MonsterChance,
                HasEffect = HasEffect,
                Effects = Effects,
                EnableBlockFloat = EnableBlockFloat,
                BlockFloatMin = BlockFloatMin,
                BlockFloatMax = BlockFloatMax,
                BlockFloatSpeed = BlockFloatSpeed,
                RandomWideBlockChance = RandomWideBlockChance,
                RandomStairBlockChance = RandomStairBlockChance,
                IsLinkedLayer = IsLinkedLayer,
                LinkedLayers = LinkedLayers,
                LinkedLayerChances = LinkedLayerChances
            };

            return ie;
        }

        public static bool operator ==(LayerIE left, LayerIE right)
        {
            if (left is null && !(right is null))
                return false;
            if (!(left is null) && right is null)
                return false;
            if (left is null && right is null)
                return true;

            if (left.Layer != right.Layer)
                return false;

            if (left.IsLinkedLayer)
            {
                if (!right.IsLinkedLayer)
                    return false;

                if (left.m_LayerNum != right.m_LayerNum)
                    return false;

                for (int i = 0; i < left.m_LayerNum; ++i)
                {
                    if (left.m_LinkedLayers[i] != right.m_LinkedLayers[i])
                        return false;

                    if (left.m_LinkedLayerChances[i] != right.m_LinkedLayerChances[i])
                        return false;
                }

                return true;
            }

            for (int i = (int)Flag.IsLinkedLayer + 1; i < (int)Flag.COUNT; ++i)
            {
                if (left.Flags[i] != right.Flags[i])
                    return false;
            }

            if (left.m_SubMaterialNum != right.m_SubMaterialNum)
                return false;

            for (int i = 0; i < left.m_SubMaterialNum; ++i)
            {
                if (left.m_SubMaterials[i] != right.m_SubMaterials[i])
                    return false;

                if (left.m_SubMaterialChances[i] != right.m_SubMaterialChances[i])
                    return false;
            }

            if (!left.EnableRandomRotation
                && left.DefaultBlockRotation != right.DefaultBlockRotation)
                return false;

            if (left.BlockHeight != right.BlockHeight)
                return false;

            if (left.EnableRandomMicroHeight
                && (left.BlockMicroHeightMin != right.BlockMicroHeightMin
                || left.BlockMicroHeightMax != right.BlockMicroHeightMax))
                return false;

            if (left.EnableRandomBlockLenght
                && (left.BlockLenghtMin != right.BlockLenghtMin
                || left.BlockLenghtMax != right.BlockLenghtMax))
                return false;

            if (left.EnableRandomProps)
            {
                if (left.PropChance != right.PropChance)
                    return false;

                if (left.m_PropNum != right.m_PropNum)
                    return false;

                for (int i = 0; i < left.m_PropNum; ++i)
                {
                    if (left.m_Props[i] != right.m_Props[i])
                        return false;

                    if (left.m_PropChances[i] != right.m_PropChances[i])
                        return false;
                }

                if (left.PropNoSpawnRadius != right.PropNoSpawnRadius)
                    return false;
            }

            if (left.SpawnMonsters)
            {
                if (left.MonsterChance != right.MonsterChance)
                    return false;

                if (left.m_MonsterNum != right.m_MonsterNum)
                    return false;

                for (int i = 0; i < left.m_MonsterNum; ++i)
                {
                    if (left.m_Monsters[i] != right.m_Monsters[i])
                        return false;

                    if (left.m_MonsterChances[i] != right.m_MonsterChances[i])
                        return false;
                }
            }

            if (left.HasEffect)
            {
                if (left.m_EffectNum != right.m_EffectNum)
                    return false;

                for (int i = 0; i < left.m_EffectNum; ++i)
                {
                    if (left.m_Effects[i] != right.m_Effects[i])
                        return false;
                }
            }

            if (left.EnableBlockFloat)
            {
                if (left.BlockFloatMin != right.BlockFloatMin)
                    return false;

                if (left.BlockFloatMax != right.BlockFloatMax)
                    return false;

                if (left.BlockFloatSpeed != right.BlockFloatSpeed)
                    return false;
            }

            if (left.RandomWideBlockChance != right.RandomWideBlockChance)
                return false;

            if (left.RandomStairBlockChance != right.RandomStairBlockChance)
                return false;

            return true;
        }

        public static bool operator !=(LayerIE left, LayerIE right)
        {
            return !(left == right);
        }

        public bool ReadLayer(ref FileStream fr)
        {
            if ((fr.Length - fr.Position) < STATIC_SIZE)
                return false;
            var tempArray = new byte[STATIC_SIZE];
            fr.Read(tempArray, 0, tempArray.Length);

            Layer = tempArray[0];
            int lastIdx = 1;
            var tempBytes = new byte[Mathf.CeilToInt((float)Flag.COUNT / 8.0f)];
            for (int i = 0; i < tempBytes.Length; ++i)
            {
                tempBytes[i] = tempArray[lastIdx];
                ++lastIdx;
            }

            Flags = new BitArray(tempBytes);

            m_SubMaterialNum = tempArray[lastIdx++];
            m_SubMaterials = new ushort[m_SubMaterialNum];
            m_SubMaterialChances = new float[m_SubMaterialNum];
            DefaultBlockRotation = (Def.RotationState)tempArray[lastIdx++];

            BlockHeight = BitConverter.ToSingle(tempArray, lastIdx);
            lastIdx += 4;

            BlockMicroHeightMin = BitConverter.ToSingle(tempArray, lastIdx);
            lastIdx += 4;

            BlockMicroHeightMax = BitConverter.ToSingle(tempArray, lastIdx);
            lastIdx += 4;

            BlockLenghtMin = BitConverter.ToSingle(tempArray, lastIdx);
            lastIdx += 4;

            BlockLenghtMax = BitConverter.ToSingle(tempArray, lastIdx);
            lastIdx += 4;

            PropChance = BitConverter.ToSingle(tempArray, lastIdx);
            lastIdx += 4;

            m_PropNum = BitConverter.ToUInt16(tempArray, lastIdx);
            lastIdx += 2;
            m_Props = new ushort[m_PropNum];
            m_PropChances = new float[m_PropNum];

            PropNoSpawnRadius = BitConverter.ToSingle(tempArray, lastIdx);
            lastIdx += 4;

            MonsterChance = BitConverter.ToSingle(tempArray, lastIdx);
            lastIdx += 4;

            m_MonsterNum = BitConverter.ToUInt16(tempArray, lastIdx);
            lastIdx += 2;
            m_Monsters = new ushort[m_MonsterNum];
            m_MonsterChances = new float[m_MonsterNum];

            m_EffectNum = BitConverter.ToUInt16(tempArray, lastIdx);
            lastIdx += 2;
            m_Effects = new ushort[m_EffectNum];

            BlockFloatMin = BitConverter.ToSingle(tempArray, lastIdx);
            lastIdx += 4;

            BlockFloatMax = BitConverter.ToSingle(tempArray, lastIdx);
            lastIdx += 4;

            BlockFloatSpeed = BitConverter.ToSingle(tempArray, lastIdx);
            lastIdx += 4;

            RandomWideBlockChance = BitConverter.ToSingle(tempArray, lastIdx);
            lastIdx += 4;

            RandomStairBlockChance = BitConverter.ToSingle(tempArray, lastIdx);
            lastIdx += 4;

            m_LayerNum = tempArray[lastIdx++];
            if (lastIdx != tempArray.Length)
                throw new Exception("Not everything has been read, LayerIE");

            m_LinkedLayers = new byte[m_LayerNum];
            m_LinkedLayerChances = new float[m_LayerNum];

            if (IsLinkedLayer)
            {
                tempArray = new byte[m_LayerNum * 1 + m_LayerNum * 4];
                fr.Read(tempArray, 0, tempArray.Length);

                for (int i = 0; i < m_LayerNum; ++i)
                {
                    m_LinkedLayers[i] = tempArray[i];
                }
                lastIdx = m_LayerNum;
                for (int i = 0; i < m_LayerNum; ++i)
                {
                    m_LinkedLayerChances[i] = BitConverter.ToSingle(tempArray, lastIdx);
                    lastIdx += 4;
                }
                if (lastIdx != tempArray.Length)
                    throw new Exception("Not everything has been read, LayerIE");
            }
            else
            {
                tempArray = new byte[m_SubMaterialNum * (2 + 4)
                    + m_PropNum * (2 + 4) + m_MonsterNum * (2 + 4) + m_EffectNum * 2];
                fr.Read(tempArray, 0, tempArray.Length);

                lastIdx = 0;
                // SubMaterials
                for (int i = 0; i < m_SubMaterialNum; ++i)
                {
                    m_SubMaterials[i] = BitConverter.ToUInt16(tempArray, lastIdx);
                    lastIdx += 2;
                }
                for (int i = 0; i < m_SubMaterialNum; ++i)
                {
                    m_SubMaterialChances[i] = BitConverter.ToSingle(tempArray, lastIdx);
                    lastIdx += 4;
                }
                // Props
                for (int i = 0; i < m_PropNum; ++i)
                {
                    m_Props[i] = BitConverter.ToUInt16(tempArray, lastIdx);
                    lastIdx += 2;
                }
                for (int i = 0; i < m_PropNum; ++i)
                {
                    m_PropChances[i] = BitConverter.ToSingle(tempArray, lastIdx);
                    lastIdx += 4;
                }
                // Monsters
                for (int i = 0; i < m_MonsterNum; ++i)
                {
                    m_Monsters[i] = BitConverter.ToUInt16(tempArray, lastIdx);
                    lastIdx += 2;
                }
                for (int i = 0; i < m_MonsterNum; ++i)
                {
                    m_MonsterChances[i] = BitConverter.ToSingle(tempArray, lastIdx);
                    lastIdx += 4;
                }
                // Effects
                for (int i = 0; i < m_EffectNum; ++i)
                {
                    m_Effects[i] = BitConverter.ToUInt16(tempArray, lastIdx);
                    lastIdx += 2;
                }
                if (lastIdx != tempArray.Length)
                    throw new Exception("Not everything has been read, LayerIE");
            }

            return true;
        }

        //public bool WriteLayer(ref FileStream fw)
        //{
        //    var tempArray = new byte[STATIC_SIZE];

        //    tempArray[0] = Layer;
        //    int lastIdx = 1;
        //    Flags.CopyTo(tempArray, lastIdx);
        //    lastIdx += Mathf.CeilToInt((float)Flag.COUNT / 8.0f);

        //    tempArray[lastIdx++] = m_SubMaterialNum;
        //    tempArray[lastIdx++] = (byte)DefaultBlockRotation;

        //    var tempBytes = BitConverter.GetBytes(BlockHeight);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    tempBytes = BitConverter.GetBytes(BlockMicroHeightMin);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    tempBytes = BitConverter.GetBytes(BlockMicroHeightMax);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    tempBytes = BitConverter.GetBytes(BlockLenghtMin);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    tempBytes = BitConverter.GetBytes(BlockLenghtMax);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    tempBytes = BitConverter.GetBytes(PropChance);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    tempBytes = BitConverter.GetBytes(m_PropNum);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    tempBytes = BitConverter.GetBytes(PropNoSpawnRadius);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    tempBytes = BitConverter.GetBytes(MonsterChance);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    tempBytes = BitConverter.GetBytes(m_MonsterNum);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    tempBytes = BitConverter.GetBytes(m_EffectNum);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    tempBytes = BitConverter.GetBytes(BlockFloatMin);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    tempBytes = BitConverter.GetBytes(BlockFloatMax);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    tempBytes = BitConverter.GetBytes(BlockFloatSpeed);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    tempBytes = BitConverter.GetBytes(RandomWideBlockChance);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    tempBytes = BitConverter.GetBytes(RandomStairBlockChance);
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //    {
        //        tempArray[lastIdx++] = tempBytes[i];
        //    }

        //    tempArray[lastIdx++] = m_LayerNum;
        //    if (lastIdx != tempArray.Length)
        //        throw new Exception("Not everything has been written, LayerIE");
        //    fw.Write(tempArray, 0, tempArray.Length);

        //    if (IsLinkedLayer)
        //    {
        //        tempArray = new byte[(4 + 1) * m_LayerNum];
        //        lastIdx = 0;
        //        for (int i = 0; i < m_LayerNum; ++i)
        //        {
        //            tempArray[lastIdx++] = m_LinkedLayers[i];
        //        }
        //        for (int i = 0; i < m_LayerNum; ++i)
        //        {
        //            tempBytes = BitConverter.GetBytes(m_LinkedLayerChances[i]);
        //            for (int j = 0; j < tempBytes.Length; ++j)
        //            {
        //                tempArray[lastIdx++] = tempBytes[j];
        //            }
        //        }
        //        if (lastIdx != tempArray.Length)
        //            throw new Exception("Not everything has been written, LayerIE");
        //        fw.Write(tempArray, 0, tempArray.Length);
        //    }
        //    else
        //    {
        //        tempArray = new byte[(2 + 4) * m_SubMaterialNum
        //            + (2 + 4) * m_PropNum + (2 + 4) * m_MonsterNum
        //            + (2) * m_EffectNum];

        //        lastIdx = 0;
        //        // Sub material
        //        for (int i = 0; i < m_SubMaterialNum; ++i)
        //        {
        //            tempBytes = BitConverter.GetBytes(m_SubMaterials[i]);
        //            for (int j = 0; j < tempBytes.Length; ++j)
        //            {
        //                tempArray[lastIdx++] = tempBytes[j];
        //            }
        //        }
        //        for (int i = 0; i < m_SubMaterialNum; ++i)
        //        {
        //            tempBytes = BitConverter.GetBytes(m_SubMaterialChances[i]);
        //            for (int j = 0; j < tempBytes.Length; ++j)
        //            {
        //                tempArray[lastIdx++] = tempBytes[j];
        //            }
        //        }
        //        // Props
        //        for (int i = 0; i < m_PropNum; ++i)
        //        {
        //            tempBytes = BitConverter.GetBytes(m_Props[i]);
        //            for (int j = 0; j < tempBytes.Length; ++j)
        //            {
        //                tempArray[lastIdx++] = tempBytes[j];
        //            }
        //        }
        //        for (int i = 0; i < m_PropNum; ++i)
        //        {
        //            tempBytes = BitConverter.GetBytes(m_PropChances[i]);
        //            for (int j = 0; j < tempBytes.Length; ++j)
        //            {
        //                tempArray[lastIdx++] = tempBytes[j];
        //            }
        //        }
        //        // Monsters
        //        for (int i = 0; i < m_MonsterNum; ++i)
        //        {
        //            tempBytes = BitConverter.GetBytes(m_Monsters[i]);
        //            for (int j = 0; j < tempBytes.Length; ++j)
        //            {
        //                tempArray[lastIdx++] = tempBytes[j];
        //            }
        //        }
        //        for (int i = 0; i < m_MonsterNum; ++i)
        //        {
        //            tempBytes = BitConverter.GetBytes(m_MonsterChances[i]);
        //            for (int j = 0; j < tempBytes.Length; ++j)
        //            {
        //                tempArray[lastIdx++] = tempBytes[j];
        //            }
        //        }
        //        // Effects
        //        for (int i = 0; i < m_EffectNum; ++i)
        //        {
        //            tempBytes = BitConverter.GetBytes(m_Effects[i]);
        //            for (int j = 0; j < tempBytes.Length; ++j)
        //            {
        //                tempArray[lastIdx++] = tempBytes[j];
        //            }
        //        }
        //        if (lastIdx != tempArray.Length)
        //            throw new Exception("Not everything has been written, LayerIE");
        //        fw.Write(tempArray, 0, tempArray.Length);
        //    }

        //    return true;
        //}

        public override bool Equals(object obj)
        {
            if (!(obj is LayerIE))
                return false;
            var iE = obj as LayerIE;
            return iE == this;
        }

        public override int GetHashCode()
        {
            var hashCode = 719741704;
            hashCode = hashCode * -1521134295 + EqualityComparer<BitArray>.Default.GetHashCode(Flags);
            hashCode = hashCode * -1521134295 + Layer.GetHashCode();
            hashCode = hashCode * -1521134295 + m_SubMaterialNum.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ushort[]>.Default.GetHashCode(m_SubMaterials);
            hashCode = hashCode * -1521134295 + EqualityComparer<float[]>.Default.GetHashCode(m_SubMaterialChances);
            hashCode = hashCode * -1521134295 + DefaultBlockRotation.GetHashCode();
            hashCode = hashCode * -1521134295 + BlockHeight.GetHashCode();
            hashCode = hashCode * -1521134295 + BlockMicroHeightMin.GetHashCode();
            hashCode = hashCode * -1521134295 + BlockMicroHeightMax.GetHashCode();
            hashCode = hashCode * -1521134295 + BlockLenghtMin.GetHashCode();
            hashCode = hashCode * -1521134295 + BlockLenghtMax.GetHashCode();
            hashCode = hashCode * -1521134295 + PropChance.GetHashCode();
            hashCode = hashCode * -1521134295 + m_PropNum.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ushort[]>.Default.GetHashCode(m_Props);
            hashCode = hashCode * -1521134295 + EqualityComparer<float[]>.Default.GetHashCode(m_PropChances);
            hashCode = hashCode * -1521134295 + PropNoSpawnRadius.GetHashCode();
            hashCode = hashCode * -1521134295 + MonsterChance.GetHashCode();
            hashCode = hashCode * -1521134295 + m_MonsterNum.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ushort[]>.Default.GetHashCode(m_Monsters);
            hashCode = hashCode * -1521134295 + EqualityComparer<float[]>.Default.GetHashCode(m_MonsterChances);
            hashCode = hashCode * -1521134295 + m_EffectNum.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ushort[]>.Default.GetHashCode(m_Effects);
            hashCode = hashCode * -1521134295 + BlockFloatMin.GetHashCode();
            hashCode = hashCode * -1521134295 + BlockFloatMax.GetHashCode();
            hashCode = hashCode * -1521134295 + BlockFloatSpeed.GetHashCode();
            hashCode = hashCode * -1521134295 + RandomWideBlockChance.GetHashCode();
            hashCode = hashCode * -1521134295 + RandomStairBlockChance.GetHashCode();
            hashCode = hashCode * -1521134295 + m_LayerNum.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(m_LinkedLayers);
            hashCode = hashCode * -1521134295 + EqualityComparer<float[]>.Default.GetHashCode(m_LinkedLayerChances);
            return hashCode;
        }
    }
}
