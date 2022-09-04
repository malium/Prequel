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
    public class StructureIE
    {
        const int Version = 2;

        private enum Flag
        {
            GenerateBridges,

            COUNT
        }
        private static readonly int STATIC_SIZE = Mathf.CeilToInt((float)Flag.COUNT / 8.0f)
            + 8 * 1 + 2 * 2 + 1 * 4;

        private BitArray _Flags;
        public BitArray Flags { get => _Flags; }

        private bool m_IsFromFile;
        string m_FilePath;

        public bool IsFromFile { get => m_IsFromFile; }
        public bool _FromFile { set => m_IsFromFile = value; }
        public string GetFilePath() => m_FilePath;
        public void _FilePath(string fileName) => m_FilePath = fileName;

        public char[] MagicNum;
        public byte StructureVersion;
        public ushort StructureID;
        public bool GenerateBridges
        {
            get
            {
                return _Flags[(int)Flag.GenerateBridges];
            }
            set
            {
                _Flags.Set((int)Flag.GenerateBridges, value);
            }
        }

        public List<LayerIE> Layers
        {
            get
            {
                if (m_Layers == null)
                    return new List<LayerIE>();
                return new List<LayerIE>(m_Layers);
            }
            set
            {
                m_Layers = value.ToArray();
                m_LayerNum = (byte)m_Layers.Length;
            }
        }
        public List<BlockIE> Blocks
        {
            get
            {
                if (m_Blocks == null)
                    return new List<BlockIE>();
                return new List<BlockIE>(m_Blocks);
            }
            set
            {
                m_Blocks = value.ToArray();
                m_BlockNum = (ushort)m_Blocks.Length;
            }
        }
        public List<int> Zones
        {
            get
            {
                var zones = new List<int>(m_ZoneNum);
                for (int i = 0; i < m_ZoneNum; ++i)
                    zones.Add(m_Zones[i]);
                return zones;
            }
            set
            {
                m_ZoneNum = (byte)value.Count;
                m_Zones = new ushort[m_ZoneNum];
                for (int i = 0; i < m_ZoneNum; ++i)
                    m_Zones[i] = (ushort)value[i];
            }
        }

        public string Name
        {
            get
            {
                var chArr = new char[m_NameLength];
                for (int i = 0; i < m_NameLength; ++i)
                    chArr[i] = (char)m_Name[i];
                return new string(chArr);
            }
            set
            {
                var chArr = value.ToCharArray();
                m_NameLength = (byte)chArr.Length;
                m_Name = new byte[m_NameLength];
                for (int i = 0; i < m_NameLength; ++i)
                    m_Name[i] = (byte)chArr[i];
            }
        }

        private Texture2D m_ScreenShot;
        public Texture2D ScreenShot
        {
            get
            {
                return m_ScreenShot;
            }
            set
            {
                m_ScreenShot = value;
                m_ScreenShotData = m_ScreenShot.EncodeToPNG();
                m_ScreenShotSize = (uint)m_ScreenShotData.Length;
            }
        }

        public byte m_LayerNum;
        public ushort m_BlockNum;
        public byte m_ZoneNum;
        public byte m_NameLength;
        private uint m_ScreenShotSize;

        private ushort[] m_Zones;
        private LayerIE[] m_Layers;
        private BlockIE[] m_Blocks;
        private byte[] m_Name;
        private byte[] m_ScreenShotData;

        public StructureIE()
        {
            m_IsFromFile = false;

            _Flags = new BitArray((int)Flag.COUNT);

            MagicNum = new char[4] { 'S', 'T', 'R', 'C' };
            StructureVersion = Version;
            StructureID = 0;

            GenerateBridges = true;

            m_LayerNum = 0;
            m_Layers = null;

            m_BlockNum = 0;
            m_Blocks = null;

            m_NameLength = 0;
            m_Name = null;
        }

        //public void ToStructure(ref StructureComponent struc, bool copyID = true)
        //{
        //    if (copyID)
        //        struc.IDXIE = StructureID;

        //    bool inEditMode = struc.transform.position == Vector3.zero;
        //    Vector2Int strucPos = GameUtils.PosFromID(
        //        GameUtils.MapIDFromPosition(new Vector2(struc.transform.position.x + 0.1f, struc.transform.position.z + 0.1f)),
        //        Manager.MapWidth, Manager.MapHeight);


        //    for (int y = 0; y < StructureComponent.Height; ++y)
        //    {
        //        var yOffset = y * StructureComponent.Width;
        //        for (int x = 0; x < StructureComponent.Width; ++x)
        //        {
        //            var strucID = yOffset + x;

        //            //var strucID = GameUtils.IDFromPos(new Vector2Int(x, y));
        //            var pilar = new GameObject("InvalidPilar").AddComponent<PilarComponent>();
        //            struc.Pilars[strucID] = pilar;
        //            int mapID = -1;
        //            if (!inEditMode)
        //            {
        //                mapID = GameUtils.IDFromPos(strucPos + new Vector2Int(x, y),
        //                    Manager.MapWidth, Manager.MapHeight);
        //            }
        //            pilar.Init(struc, strucID, mapID);
        //            pilar.AddBlock();
        //        }
        //    }

        //    for (int i = 0; i < m_BlockNum; ++i)
        //    {
        //        var blockIE = m_Blocks[i];
        //        var strucID = blockIE.StructureID;
        //        var pilar = struc.Pilars[strucID];
        //        BlockComponent block = null;
        //        for (int j = 0; j < pilar.Blocks.Count; ++j)
        //        {
        //            var curBlock = pilar.Blocks[j];
        //            if (curBlock.Layer != 0)
        //                continue;
        //            block = curBlock;
        //        }
        //        if (block == null)
        //        {
        //            pilar.AddBlock();
        //            block = pilar.Blocks[pilar.Blocks.Count - 1];
        //        }
        //        block.Layer = blockIE.Layer;
        //        block.IDXIE = -(i + 2);
        //    }

        //    for (int i = 0; i < m_LayerNum; ++i)
        //    {
        //        var layer = m_Layers[i];
        //        struc.SetLayer(layer.Layer, layer.ToLayerInfo());
        //    }
        //    List<KeyValuePair<int, BlockComponent>> wideBlocks = new List<KeyValuePair<int, BlockComponent>>();

        //    for (int i = 0; i < struc.Pilars.Length; ++i)
        //    {
        //        var pilar = struc.Pilars[i];
        //        for (int j = 0; j < pilar.Blocks.Count; ++j)
        //        {
        //            var block = pilar.Blocks[j];
        //            if (block.IDXIE < -1)
        //            {
        //                if(m_Blocks[(-block.IDXIE) - 2].blockType == BlockType.WIDE)
        //                {
        //                    wideBlocks.Add(new KeyValuePair<int, BlockComponent>((-block.IDXIE) - 2, block));
        //                    break;
        //                }
        //                m_Blocks[(-block.IDXIE) - 2].Apply(block);
        //            }
        //        }
        //    }
        //    for(int i = 0; i < wideBlocks.Count; ++i)
        //    {
        //        var pair = wideBlocks[i];

        //        BlockComponent[] hidden = StructureComponent.GetNearBlocks(struc, pair.Value.Pilar.StructureID, m_Blocks[pair.Key].Height, m_Blocks[pair.Key].Layer);
        //        if(hidden[0] == null)
        //        {
        //            hidden = StructureComponent.GetNearBlocks(struc, pair.Value.Pilar.StructureID, m_Blocks[pair.Key].Height, -1);
        //            if(hidden[0] == null)
        //                throw new Exception("Couldn't find near blocks to convert them into hidden blocks, while loading a structure.");
        //        }
        //        pair.Value.SetWIDE(hidden);
        //        m_Blocks[pair.Key].Apply(pair.Value);
        //    }
        //    for (int i = 0; i < struc.Pilars.Length; ++i)
        //    {
        //        var pilar = struc.Pilars[i];
        //        for (int j = 0; j < pilar.Blocks.Count; ++j)
        //        {
        //            var block = pilar.Blocks[j];
        //            if (block.IDXIE < -1)
        //            {
        //                block.IDXIE = (-block.IDXIE) - 2;
        //                //if (block.blockType == BlockType.WIDE)
        //                //{
        //                //    var nearBlocks = StructureComponent.GetNearBlocks(struc, pilar.StructureID, block.Height, block.Layer);
        //                //    if (nearBlocks[0] == null)
        //                //    {
        //                //        throw new Exception("Couldn't find near blocks to convert them into removed blocks, while loading a structure.");
        //                //    }
        //                //    block.SetWIDE(nearBlocks);
        //                //    //for(int k = 0; k < nearBlocks.Length; ++k)
        //                //    //{
        //                //    //    var b = nearBlocks[k];

        //                //    //    b.MicroHeight = block.MicroHeight;

        //                //    //    b.TopMR.enabled = false;
        //                //    //    b.TopBC.enabled = false;
        //                //    //    b.MidMR.enabled = false;
        //                //    //    b.MidBC.enabled = false;

        //                //    //    b.LayerSR.enabled = false;

        //                //    //    b.AnchorSR.enabled = false;
        //                //    //    b.StairSR.enabled = false;
        //                //    //    b.LockSR.enabled = false;

        //                //    //    b.Removed = true;
        //                //    //}
        //                //}
        //            }
        //        }
        //    }
        //}

        //public StructureComponent ToStructure()
        //{
        //    var strucGO = new GameObject("InvalidStruc");
        //    var struc = strucGO.AddComponent<StructureComponent>();
        //    ToStructure(ref struc);
        //    return struc;
        //}

        public static bool operator ==(StructureIE left, StructureIE right)
        {
            if (left is null && !(right is null))
                return false;
            if (!(left is null) && right is null)
                return false;
            if (left is null && right is null)
                return true;

            for (int i = 0; i < left._Flags.Length; ++i)
            {
                if (left._Flags[i] != right._Flags[i])
                    return false;
            }

            if (left.StructureVersion != right.StructureVersion)
                return false;

            if (left.StructureID != right.StructureID)
                return false;

            if (left.m_LayerNum != right.m_LayerNum)
                return false;

            for (int i = 0; i < left.m_LayerNum; ++i)
            {
                if (left.m_Layers[i] != right.m_Layers[i])
                    return false;
            }

            if (left.m_BlockNum != right.m_BlockNum)
                return false;

            for (int i = 0; i < left.m_BlockNum; ++i)
            {
                if (left.m_Blocks[i] != right.m_Blocks[i])
                    return false;
            }

            if (left.m_NameLength != right.m_NameLength)
                return false;

            for (int i = 0; i < left.m_NameLength; ++i)
            {
                if (left.m_Name[i] != right.m_Name[i])
                    return false;
            }

            if (left.m_ZoneNum != right.m_ZoneNum)
                return false;

            for (int i = 0; i < left.m_ZoneNum; ++i)
            {
                if (left.m_Zones[i] != right.m_Zones[i])
                    return false;
            }

            return true;
        }

        public static bool operator !=(StructureIE left, StructureIE right)
        {
            return !(left == right);
        }

        //public void SaveStructure(string path)
        //{
        //    var fw = File.OpenWrite(path);

        //    m_IsFromFile = true;

        //    var tempArray = new byte[STATIC_SIZE];
        //    int lastIdx = 0;
        //    // Magic num write
        //    for (int i = 0; i < MagicNum.Length; ++i)   // 0 - 3
        //        tempArray[lastIdx++] = (byte)MagicNum[i];

        //    // Version Write
        //    tempArray[lastIdx++] = StructureVersion; // 4

        //    // ID Write
        //    var tempBytes = BitConverter.GetBytes(StructureID); //5 - 6
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //        tempArray[lastIdx++] = tempBytes[i];

        //    // Flags
        //    Flags.CopyTo(tempArray, lastIdx);
        //    lastIdx += Mathf.CeilToInt((float)Flag.COUNT / 8.0f); // 7

        //    // m_LayerNum write
        //    tempArray[lastIdx++] = m_LayerNum; // 8

        //    // m_BlockNum write
        //    tempBytes = BitConverter.GetBytes(m_BlockNum); //9 - 10
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //        tempArray[lastIdx++] = tempBytes[i];

        //    // m_ZoneNum write
        //    tempArray[lastIdx++] = m_ZoneNum; // 11

        //    // m_NameLength
        //    tempArray[lastIdx++] = m_NameLength; // 12

        //    // m_ScreenShotSize write
        //    tempBytes = BitConverter.GetBytes(m_ScreenShotSize); // 13 - 16
        //    for (int i = 0; i < tempBytes.Length; ++i)
        //        tempArray[lastIdx++] = tempBytes[i];

        //    if (lastIdx != tempArray.Length)
        //        throw new Exception("Not everything has been written, StructureIE");

        //    // Write To File
        //    fw.Write(tempArray, 0, tempArray.Length);

        //    // m_Zones write
        //    tempArray = new byte[m_ZoneNum * 2];
        //    lastIdx = 0;
        //    for (int i = 0; i < m_ZoneNum; ++i)
        //    {
        //        tempBytes = BitConverter.GetBytes(m_Zones[i]);
        //        for (int j = 0; j < tempBytes.Length; ++j)
        //            tempArray[lastIdx++] = tempBytes[j];
        //    }

        //    if (lastIdx != tempArray.Length)
        //        throw new Exception("Not everything has been written, StructureIE");

        //    // Write to File
        //    fw.Write(tempArray, 0, tempArray.Length);

        //    // Layer write
        //    for (int i = 0; i < m_LayerNum; ++i)
        //        m_Layers[i].WriteLayer(ref fw);

        //    // Block write
        //    for (int i = 0; i < m_BlockNum; ++i)
        //        m_Blocks[i].WriteBlock(ref fw);

        //    // Name write
        //    fw.Write(m_Name, 0, m_NameLength);

        //    // Screenshot write
        //    fw.Write(m_ScreenShotData, 0, (int)m_ScreenShotSize);

        //    fw.Close();
        //}

        public static StructureIE FromFile(string path)
        {
            StructureIE sie = new StructureIE
            {
                m_IsFromFile = true,
                m_FilePath = path
            };
            var fr = File.OpenRead(path);
            if (!fr.CanRead || fr.Length < STATIC_SIZE)
            {
                fr.Close();
                return null;
            }

            var tempArray = new byte[STATIC_SIZE];
            fr.Read(tempArray, 0, tempArray.Length);

            int lastIdx = 0;
            // Magic num read
            for (int i = 0; i < sie.MagicNum.Length; ++i)
            {
                if (tempArray[lastIdx++] != (byte)sie.MagicNum[i])
                {
                    fr.Close();
                    return null; // Magic num was wrong
                }
            }

            var version = tempArray[lastIdx++];
            if(version > Version)
            {
                fr.Close();
                throw new Exception("Trying to read a Structure from a file, which is saved with a greater StructureVersion.");
            }
            if(version == 1)
            {
                fr.Close();
                return V1.StructureIE.LoadAndUpgrade(path);
            }

            var id = BitConverter.ToUInt16(tempArray, lastIdx);
            if (id == ushort.MaxValue)
            {
                fr.Close();
                return null; // Invalid id
            }
            sie.StructureID = id;
            lastIdx += 2;

            // Read flags
            var tempBytes = new byte[Mathf.CeilToInt((float)Flag.COUNT / 8.0f)];
            for (int i = 0; i < tempBytes.Length; ++i)
            {
                tempBytes[i] = tempArray[lastIdx];
                ++lastIdx;
            }
            sie._Flags = new BitArray(tempBytes);

            // Read layer num
            var layerNum = tempArray[lastIdx++];
            if (layerNum > 8)
            {
                fr.Close();
                return null; // layer num is invalid
            }
            sie.m_LayerNum = layerNum;

            // Read block num
            var blockNum = BitConverter.ToUInt16(tempArray, lastIdx);
            lastIdx += 2;
            if (blockNum == ushort.MaxValue)
            {
                fr.Close();
                return null; // block num was invalid
            }
            sie.m_BlockNum = blockNum;

            // Read zone Num
            var zoneNum = tempArray[lastIdx++];
            sie.m_ZoneNum = zoneNum;

            // Read Name length
            sie.m_NameLength = tempArray[lastIdx++];

            // Read screenshot data size
            sie.m_ScreenShotSize = BitConverter.ToUInt32(tempArray, lastIdx);
            lastIdx += 4;

            if (lastIdx != tempArray.Length)
                throw new Exception("Not everything has been read, StructureIE");

            // Read zones
            sie.m_Zones = new ushort[sie.m_ZoneNum];
            for (int i = 0; i < zoneNum; ++i)
            {
                sie.m_Zones[i] = BitConverter.ToUInt16(tempArray, lastIdx);
                lastIdx += 2;
            }

            // Read each layer
            sie.m_Layers = new LayerIE[sie.m_LayerNum];
            //if (oldVersion)
            //{
            //    for (int i = 0; i < layerNum; ++i)
            //    {
            //        var tempLayer = new V1.LayerIE();
            //        bool isOK = tempLayer.ReadLayer(ref fr);
            //        if(!isOK)
            //        {
            //            fr.Close();
            //            return null;
            //        }
            //        sie.m_Layers[i] = new LayerIE();
            //        sie.m_Layers[i].FromOld(tempLayer);
            //    }
            //}
            //else
            //{
                for (int i = 0; i < layerNum; ++i)
                {
                    sie.m_Layers[i] = new LayerIE();
                    bool isOK = sie.m_Layers[i].ReadLayer(ref fr);
                    if (!isOK)
                    {
                        fr.Close();
                        return null;
                    }
                }
            //}

            // Read each block
            sie.m_Blocks = new BlockIE[sie.m_BlockNum];
            //if (oldVersion)
            //{
            //    for(int i = 0; i < blockNum; ++i)
            //    {
            //        var tempBlock = new V1.BlockIE();
            //        bool isOK = tempBlock.ReadBlock(ref fr);
            //        if (!isOK)
            //        {
            //            fr.Close();
            //            return null;
            //        }
            //        sie.m_Blocks[i] = new BlockIE();
            //        sie.m_Blocks[i].FromOld(tempBlock);
            //    }
            //}
            //else
            //{
                for (int i = 0; i < sie.m_BlockNum; ++i)
                {
                    sie.m_Blocks[i] = new BlockIE();
                    bool isOK = sie.m_Blocks[i].ReadBlock(ref fr);
                    if (!isOK)
                    {
                        fr.Close();
                        return null;
                    }
                }
            //}

            // Read name
            sie.m_Name = new byte[sie.m_NameLength];
            fr.Read(sie.m_Name, 0, sie.m_NameLength);

            // Read screen shot
            sie.m_ScreenShotData = new byte[sie.m_ScreenShotSize];
            fr.Read(sie.m_ScreenShotData, 0, (int)sie.m_ScreenShotSize);
            sie.m_ScreenShot = new Texture2D(0, 0);
            sie.m_ScreenShot.LoadImage(sie.m_ScreenShotData, false);

            fr.Close();

            return sie;
        }

        public static V3.StructureIE LoadAndUpgrade(string path)
        {
            var sie = FromFile(path);
            return sie.Upgrade();
        }

        public V3.StructureIE Upgrade()
        {
            var ie = new V3.StructureIE()
            {
                StructureID = StructureID,
            };
            ie._FromFile(m_IsFromFile);
            ie._FilePath(m_FilePath);
            ie.SetName(Name);
            ie.SetWidth(Def.DefaultStrucSide);
            ie.SetHeight(Def.DefaultStrucSide);
            for (int i = 0; i < m_LayerNum; ++i)
                ie.SetLayer(m_Layers[i].Layer, m_Layers[i].Upgrade());
            for (int i = 0; i < m_BlockNum; ++i)
                ie.AddBlock(m_Blocks[i].Upgrade());
            ie.SetScreenshot(ScreenShot);

            return ie;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is StructureIE))
                return false;
            var iE = obj as StructureIE;
            return iE == this;
        }

        public override int GetHashCode()
        {
            var hashCode = -727961520;
            hashCode = hashCode * -1521134295 + EqualityComparer<BitArray>.Default.GetHashCode(_Flags);
            hashCode = hashCode * -1521134295 + EqualityComparer<char[]>.Default.GetHashCode(MagicNum);
            hashCode = hashCode * -1521134295 + StructureVersion.GetHashCode();
            hashCode = hashCode * -1521134295 + StructureID.GetHashCode();
            hashCode = hashCode * -1521134295 + GenerateBridges.GetHashCode();
            hashCode = hashCode * -1521134295 + m_LayerNum.GetHashCode();
            hashCode = hashCode * -1521134295 + m_BlockNum.GetHashCode();
            hashCode = hashCode * -1521134295 + m_ZoneNum.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ushort[]>.Default.GetHashCode(m_Zones);
            hashCode = hashCode * -1521134295 + EqualityComparer<LayerIE[]>.Default.GetHashCode(m_Layers);
            hashCode = hashCode * -1521134295 + EqualityComparer<BlockIE[]>.Default.GetHashCode(m_Blocks);
            return hashCode;
        }
    }
}
