/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/


//using System.Collections.Generic;
//using System.Data.Common;
//using System.Collections.Specialized;
//using System.IO;
//using System.Collections;
//using System;
//using UnityEngine;

//namespace Assets
//{
//    public class ZoneIE
//    {
//        const int STATIC_SIZE = 8 * 1 + 1 * 2;

//        public string Name
//        {
//            get
//            {
//                var chr = new char[m_NameLength];
//                for (int i = 0; i < m_NameLength; ++i)
//                    chr[i] = (char)m_Name[i];
//                return new string(chr);
//            }
//            set
//            {
//                var str = value;
//                if (str.Length > 255)
//                    str = str.Substring(0, 255);
//                m_NameLength = (byte)str.Length;
//                m_Name = new byte[m_NameLength];
//                for (int i = 0; i < m_NameLength; ++i)
//                {
//                    if (str[i] > 'ÿ')
//                        m_Name[i] = (byte)'~';
//                    else
//                        m_Name[i] = (byte)str[i];
//                }
//            }
//        }
//        public List<int> Strucs
//        {
//            get
//            {
//                List<int> temp = new List<int>(m_StructureNum);
//                for (int i = 0; i < m_StructureNum; ++i)
//                    temp.Add(m_Structures[i]);
//                return temp;
//            }
//            set
//            {
//                m_StructureNum = (byte)value.Count;
//                m_Structures = new ushort[m_StructureNum];
//                for (int i = 0; i < m_StructureNum; ++i)
//                    m_Structures[i] = (ushort)value[i];
//            }
//        }
//        public List<int> Monsters
//        {
//            get
//            {
//                List<int> temp = new List<int>(m_MonsterNum);
//                for (int i = 0; i < m_MonsterNum; ++i)
//                    temp.Add(m_Monsters[i]);
//                return temp;
//            }
//            set
//            {
//                m_MonsterNum = (byte)value.Count;
//                m_Monsters = new ushort[m_MonsterNum];
//                for (int i = 0; i < m_StructureNum; ++i)
//                    m_Monsters[i] = (ushort)value[i];
//            }
//        }
//        public List<int> MonsterChances
//        {
//            get
//            {
//                List<int> temp = new List<int>(m_MonsterNum);
//                for (int i = 0; i < m_MonsterNum; ++i)
//                    temp.Add(m_MonsterChances[i]);
//                return temp;
//            }
//            set
//            {
//                m_MonsterNum = (byte)value.Count;
//                m_MonsterChances = new ushort[m_MonsterNum];
//                for (int i = 0; i < m_StructureNum; ++i)
//                    m_MonsterChances[i] = (ushort)value[i];
//            }
//        }

//        public char[] MagicNum;
//        public ushort ZoneID;
//        public byte ZoneVersion;
//        public byte m_NameLength;
//        public byte m_StructureNum;
//        public byte m_MonsterNum;

//        private byte[] m_Name;
//        private ushort[] m_Structures;
//        private ushort[] m_Monsters;
//        private ushort[] m_MonsterChances;

//        public ZoneIE()
//        {
//            MagicNum = new char[] { 'Z', 'O', 'N', 'E' };
//            ZoneID = ushort.MaxValue;
//            ZoneVersion = 1;
//            m_NameLength = 0;
//            m_StructureNum = 0;
//            m_MonsterNum = 0;
//        }

//        public void SaveZone(string path)
//        {
//            var fw = File.OpenWrite(path);

//            if (!fw.CanWrite)
//                return;

//            var tempArray = new byte[STATIC_SIZE];
//            int lastIdx = 0;
//            for (int i = 0; i < MagicNum.Length; ++i)
//                tempArray[lastIdx++] = (byte)MagicNum[i];

//            var tempBytes = BitConverter.GetBytes(ZoneID);
//            for (int i = 0; i < tempBytes.Length; ++i)
//                tempArray[lastIdx++] = tempBytes[i];

//            tempArray[lastIdx++] = ZoneVersion;
//            tempArray[lastIdx++] = m_NameLength;
//            tempArray[lastIdx++] = m_StructureNum;
//            tempArray[lastIdx++] = m_MonsterNum;

//            fw.Write(tempArray, 0, tempArray.Length);

//            tempArray = new byte[m_NameLength * 1
//                + m_StructureNum * 2 + m_MonsterNum * (4 + 2)];

//            for (int i = 0; i < m_NameLength; ++i)
//                tempArray[lastIdx++] = (byte)m_Name[i];

//            for(int i = 0; i < m_StructureNum; ++i)
//            {
//                tempBytes = BitConverter.GetBytes(m_Structures[i]);
//                for (int j = 0; j < tempBytes.Length; ++j)
//                    tempArray[lastIdx++] = tempBytes[j];
//            }

//            for (int i = 0; i < m_MonsterNum; ++i)
//            {
//                tempBytes = BitConverter.GetBytes(m_Monsters[i]);
//                for (int j = 0; j < tempBytes.Length; ++j)
//                    tempArray[lastIdx++] = tempBytes[j];
//            }

//            for (int i = 0; i < m_MonsterNum; ++i)
//            {
//                tempBytes = BitConverter.GetBytes(m_MonsterChances[i]);
//                for (int j = 0; j < tempBytes.Length; ++j)
//                    tempArray[lastIdx++] = tempBytes[j];
//            }
//            fw.Write(tempArray, 0, tempArray.Length);
//            fw.Close();
//        }

//        public static ZoneIE FromFile(string path)
//        {
//            ZoneIE zone = new ZoneIE();
//            var fr = File.OpenRead(path);
//            if(!fr.CanRead || fr.Length < STATIC_SIZE)
//            {
//                fr.Close();
//                return null;
//            }

//            var tempArray = new byte[STATIC_SIZE];
//            fr.Read(tempArray, 0, tempArray.Length);

//            int lastIdx = 0;
//            for(int i = 0; i < zone.MagicNum.Length; ++i)
//            {
//                if((char)tempArray[lastIdx++] != zone.MagicNum[i])
//                {
//                    fr.Close();
//                    return null; // Magic num error
//                }
//            }

//            var id = BitConverter.ToUInt16(tempArray, lastIdx);
//            if(id == ushort.MaxValue)
//            {
//                fr.Close();
//                return null;
//            }
//            zone.ZoneID = id;
//            lastIdx += 2;

//            var version = tempArray[lastIdx++];
//            if(version != zone.ZoneVersion)
//            {
//                fr.Close();
//                return null;
//            }

//            zone.m_NameLength = tempArray[lastIdx++];
//            zone.m_StructureNum = tempArray[lastIdx++];
//            zone.m_MonsterNum = tempArray[lastIdx++];

//            tempArray = new byte[zone.m_NameLength * 1
//                + zone.m_StructureNum * 2 + zone.m_MonsterNum * (4 + 2)];

//            zone.m_Name = new byte[zone.m_NameLength];
//            zone.m_Structures = new ushort[zone.m_StructureNum];
//            zone.m_Monsters = new ushort[zone.m_MonsterNum];
//            zone.m_MonsterChances = new ushort[zone.m_MonsterNum];

//            for(int i = 0; i < zone.m_NameLength; ++i)
//            {
//                zone.m_Name[i] = tempArray[lastIdx++];
//            }
//            for(int i = 0; i < zone.m_StructureNum; ++i)
//            {
//                zone.m_Structures[i] = BitConverter.ToUInt16(tempArray, lastIdx);
//                lastIdx += 2;
//            }
//            for (int i = 0; i < zone.m_MonsterNum; ++i)
//            {
//                zone.m_Monsters[i] = BitConverter.ToUInt16(tempArray, lastIdx);
//                lastIdx += 2;
//            }
//            for (int i = 0; i < zone.m_MonsterNum; ++i)
//            {
//                zone.m_MonsterChances[i] = BitConverter.ToUInt16(tempArray, lastIdx);
//                lastIdx += 2;
//            }
//            fr.Close();

//            return zone;
//        }

//        public override bool Equals(object obj)
//        {
//            var iE = obj as ZoneIE;
//            return iE == this;
//        }

//        public override int GetHashCode()
//        {
//            var hashCode = -1009486901;
//            hashCode = hashCode * -1521134295 + EqualityComparer<char[]>.Default.GetHashCode(MagicNum);
//            hashCode = hashCode * -1521134295 + ZoneID.GetHashCode();
//            hashCode = hashCode * -1521134295 + ZoneVersion.GetHashCode();
//            hashCode = hashCode * -1521134295 + m_NameLength.GetHashCode();
//            hashCode = hashCode * -1521134295 + m_StructureNum.GetHashCode();
//            hashCode = hashCode * -1521134295 + m_MonsterNum.GetHashCode();
//            hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(m_Name);
//            hashCode = hashCode * -1521134295 + EqualityComparer<ushort[]>.Default.GetHashCode(m_Structures);
//            hashCode = hashCode * -1521134295 + EqualityComparer<ushort[]>.Default.GetHashCode(m_Monsters);
//            hashCode = hashCode * -1521134295 + EqualityComparer<ushort[]>.Default.GetHashCode(m_MonsterChances);
//            return hashCode;
//        }

//        public static bool operator==(ZoneIE left, ZoneIE right)
//        {
//            if (left is null && !(right is null))
//                return false;
//            if (!(left is null) && right is null)
//                return false;
//            if (left is null && right is null)
//                return true;

//            if (left.ZoneID != right.ZoneID)
//                return false;

//            if (left.ZoneVersion != right.ZoneVersion)
//                return false;

//            if (left.m_NameLength != right.m_NameLength)
//                return false;

//            if (left.m_StructureNum != right.m_StructureNum)
//                return false;

//            if (left.m_MonsterNum != right.m_MonsterNum)
//                return false;

//            for(int i = 0; i < left.m_NameLength; ++i)
//            {
//                if (left.m_Name[i] != right.m_Name[i])
//                    return false;
//            }

//            for(int i = 0; i < left.m_StructureNum; ++i)
//            {
//                if (left.m_Structures[i] != right.m_Structures[i])
//                    return false;
//            }

//            for (int i = 0; i < left.m_MonsterNum; ++i)
//            {
//                if (left.m_Monsters[i] != right.m_Monsters[i])
//                    return false;
//            }

//            for (int i = 0; i < left.m_MonsterNum; ++i)
//            {
//                if (left.m_MonsterChances[i] != right.m_MonsterChances[i])
//                    return false;
//            }

//            return true;
//        }
//        public static bool operator!=(ZoneIE left, ZoneIE right)
//        {
//            return !(left == right);
//        }
//    }

//    public class SubZoneIE
//    {

//        public string Name;
//        public List<int> Strucs;
//        public List<int> Monsters;
//        public List<int> MonsterChances;
//    }
    
//    public class StructureIE
//    {
//        private enum Flag
//        {
//            GenerateBridges,

//            COUNT
//        }
//        private static readonly int STATIC_SIZE = Mathf.CeilToInt((float)Flag.COUNT / 8.0f)
//            + 8 * 1 + 2 * 2 + 1 * 4;

//        private BitArray Flags;

//        private bool m_IsFromFile;
//        public bool IsFromFile
//        {
//            get
//            {
//                return m_IsFromFile;
//            }
//        }

//        public char[] MagicNum;
//        public byte StructureVersion;
//        public ushort StructureID;
//        public bool GenerateBridges
//        {
//            get
//            {
//                return Flags[(int)Flag.GenerateBridges];
//            }
//            set
//            {
//                Flags.Set((int)Flag.GenerateBridges, value);
//            }
//        }

//        public List<LayerIE> Layers
//        {
//            get
//            {
//                if (m_Layers == null)
//                    return new List<LayerIE>();
//                return new List<LayerIE>(m_Layers);
//            }
//            set
//            {
//                m_Layers = value.ToArray();
//                m_LayerNum = (byte)m_Layers.Length;
//            }
//        }
//        public List<BlockIE> Blocks
//        {
//            get
//            {
//                if (m_Blocks == null)
//                    return new List<BlockIE>();
//                return new List<BlockIE>(m_Blocks);
//            }
//            set
//            {
//                m_Blocks = value.ToArray();
//                m_BlockNum = (ushort)m_Blocks.Length;
//            }
//        }
//        public List<int> Zones
//        {
//            get
//            {
//                var zones = new List<int>(m_ZoneNum);
//                for (int i = 0; i < m_ZoneNum; ++i)
//                    zones.Add(m_Zones[i]);
//                return zones;
//            }
//            set
//            {
//                m_ZoneNum = (byte)value.Count;
//                m_Zones = new ushort[m_ZoneNum];
//                for (int i = 0; i < m_ZoneNum; ++i)
//                    m_Zones[i] = (ushort)value[i];
//            }
//        }

//        public string Name
//        {
//            get
//            {
//                var chArr = new char[m_NameLength];
//                for (int i = 0; i < m_NameLength; ++i)
//                    chArr[i] = (char)m_Name[i];
//                return new string(chArr);
//            }
//            set
//            {
//                var chArr = value.ToCharArray();
//                m_NameLength = (byte)chArr.Length;
//                m_Name = new byte[m_NameLength];
//                for (int i = 0; i < m_NameLength; ++i)
//                    m_Name[i] = (byte)chArr[i];
//            }
//        }

//        private Texture2D m_ScreenShot;
//        public Texture2D ScreenShot
//        {
//            get
//            {
//                return m_ScreenShot;
//            }
//            set
//            {
//                m_ScreenShot = value;
//                m_ScreenShotData = m_ScreenShot.EncodeToPNG();
//                m_ScreenShotSize = (uint)m_ScreenShotData.Length;
//            }
//        }

//        public byte m_LayerNum;
//        public ushort m_BlockNum;
//        public byte m_ZoneNum;
//        public byte m_NameLength;
//        private uint m_ScreenShotSize;

//        private ushort[] m_Zones;
//        private LayerIE[] m_Layers;
//        private BlockIE[] m_Blocks;
//        private byte[] m_Name;
//        private byte[] m_ScreenShotData;

//        public StructureIE()
//        {
//            m_IsFromFile = false;

//            Flags = new BitArray((int)Flag.COUNT);

//            MagicNum = new char[4] { 'S', 'T', 'R', 'C' };
//            StructureVersion = 1;
//            StructureID = 0;

//            GenerateBridges = true;

//            m_LayerNum = 0;
//            m_Layers = null;

//            m_BlockNum = 0;
//            m_Blocks = null;

//            m_NameLength = 0;
//            m_Name = null;
//        }

//        public void ToStructure(ref StructureComponent struc, bool copyID = true)
//        {
//            if(copyID)
//                struc.IDXIE = StructureID;

//            for (int y = 0; y < StructureComponent.Height; ++y)
//            {
//                var yOffset = y * StructureComponent.Width;
//                for (int x = 0; x < StructureComponent.Width; ++x)
//                {
//                    var idx = yOffset + x;

//                    var strucID = GameUtils.IDFromPos(new Vector2Int(x, y));
//                    var pilarGO = new GameObject("InvalidPilar");
//                    var pilar = pilarGO.AddComponent<PilarComponent>();
//                    struc.Pilars[idx] = pilar;
//                    pilar.Init(struc, strucID);
//                    pilar.AddBlock();
//                }
//            }

//            for (int i = 0; i < m_BlockNum; ++i)
//            {
//                var blockIE = m_Blocks[i];
//                var strucID = blockIE.StructureID;
//                var pilar = struc.Pilars[strucID];
//                BlockComponent block = null;
//                for (int j = 0; j < pilar.Blocks.Count; ++j)
//                {
//                    var curBlock = pilar.Blocks[j];
//                    if (curBlock.Layer != 0)
//                        continue;
//                    block = curBlock;
//                }
//                if (block == null)
//                {
//                    pilar.AddBlock();
//                    block = pilar.Blocks[pilar.Blocks.Count - 1];
//                }
//                block.Layer = blockIE.Layer;
//                block.IDXIE = -(i + 2);
//            }

//            for (int i = 0; i < m_LayerNum; ++i)
//            {
//                var layer = m_Layers[i];
//                struc.SetLayer(layer.Layer, layer.ToLayerInfo());
//            }
//            for (int i = 0; i < struc.Pilars.Length; ++i)
//            {
//                var pilar = struc.Pilars[i];
//                for (int j = 0; j < pilar.Blocks.Count; ++j)
//                {
//                    var block = pilar.Blocks[j];
//                    if (block.IDXIE < -1)
//                    {
//                        m_Blocks[(-block.IDXIE) - 2].Apply(block);
//                    }
//                }
//            }
//            for (int i = 0; i < struc.Pilars.Length; ++i)
//            {
//                var pilar = struc.Pilars[i];
//                for (int j = 0; j < pilar.Blocks.Count; ++j)
//                {
//                    var block = pilar.Blocks[j];
//                    if (block.IDXIE < -1)
//                    {
//                        block.IDXIE = (-block.IDXIE) - 2;
//                        if(block.blockType == BlockType.WIDE)
//                        {
//                            var nearBlocks = StructureComponent.GetNearBlocks(struc, pilar.StructureID, block.Height, block.Layer);
//                            if (nearBlocks[0] == null)
//                            {
//                                throw new Exception("Couldn't find near blocks to convert them into removed blocks, while loading a structure.");
//                            }
//                            block.SetWIDE(nearBlocks);
//                            //for(int k = 0; k < nearBlocks.Length; ++k)
//                            //{
//                            //    var b = nearBlocks[k];

//                            //    b.MicroHeight = block.MicroHeight;

//                            //    b.TopMR.enabled = false;
//                            //    b.TopBC.enabled = false;
//                            //    b.MidMR.enabled = false;
//                            //    b.MidBC.enabled = false;

//                            //    b.LayerSR.enabled = false;

//                            //    b.AnchorSR.enabled = false;
//                            //    b.StairSR.enabled = false;
//                            //    b.LockSR.enabled = false;

//                            //    b.Removed = true;
//                            //}
//                        }
//                    }
//                }
//            }
//        }

//        public StructureComponent ToStructure()
//        {
//            var strucGO = new GameObject("InvalidStruc");
//            var struc = strucGO.AddComponent<StructureComponent>();
//            ToStructure(ref struc);
//            return struc;
//        }

//        public static bool operator==(StructureIE left, StructureIE right)
//        {
//            if (left is null && !(right is null))
//                return false;
//            if (!(left is null) && right is null)
//                return false;
//            if (left is null && right is null)
//                return true;

//            for(int i = 0; i < left.Flags.Length; ++i)
//            {
//                if (left.Flags[i] != right.Flags[i])
//                    return false;
//            }

//            if (left.StructureVersion != right.StructureVersion)
//                return false;

//            if (left.StructureID != right.StructureID)
//                return false;

//            if (left.m_LayerNum != right.m_LayerNum)
//                return false;

//            for(int i = 0; i < left.m_LayerNum; ++i)
//            {
//                if (left.m_Layers[i] != right.m_Layers[i])
//                    return false;
//            }

//            if (left.m_BlockNum != right.m_BlockNum)
//                return false;

//            for (int i = 0; i < left.m_BlockNum; ++i)
//            {
//                if (left.m_Blocks[i] != right.m_Blocks[i])
//                    return false;
//            }

//            if (left.m_NameLength != right.m_NameLength)
//                return false;

//            for(int i = 0; i < left.m_NameLength; ++i)
//            {
//                if (left.m_Name[i] != right.m_Name[i])
//                    return false;
//            }

//            if (left.m_ZoneNum != right.m_ZoneNum)
//                return false;

//            for (int i = 0; i < left.m_ZoneNum; ++i)
//            {
//                if (left.m_Zones[i] != right.m_Zones[i])
//                    return false;
//            }

//            return true;
//        }

//        public static bool operator!=(StructureIE left, StructureIE right)
//        {
//            return !(left == right);
//        }

//        public void SaveStructure(string path)
//        {
//            var fw = File.OpenWrite(path);

//            m_IsFromFile = true;

//            var tempArray = new byte[STATIC_SIZE];
//            int lastIdx = 0;
//            // Magic num write
//            for (int i = 0; i < MagicNum.Length; ++i)   // 0 - 3
//                tempArray[lastIdx++] = (byte)MagicNum[i];

//            // Version Write
//            tempArray[lastIdx++] = StructureVersion; // 4

//            // ID Write
//            var tempBytes = BitConverter.GetBytes(StructureID); //5 - 6
//            for (int i = 0; i < tempBytes.Length; ++i)
//                tempArray[lastIdx++] = tempBytes[i];

//            // Flags
//            Flags.CopyTo(tempArray, lastIdx);
//            lastIdx += Mathf.CeilToInt((float)Flag.COUNT / 8.0f); // 7

//            // m_LayerNum write
//            tempArray[lastIdx++] = m_LayerNum; // 8

//            // m_BlockNum write
//            tempBytes = BitConverter.GetBytes(m_BlockNum); //9 - 10
//            for (int i = 0; i < tempBytes.Length; ++i)
//                tempArray[lastIdx++] = tempBytes[i];

//            // m_ZoneNum write
//            tempArray[lastIdx++] = m_ZoneNum; // 11

//            // m_NameLength
//            tempArray[lastIdx++] = m_NameLength; // 12

//            // m_ScreenShotSize write
//            tempBytes = BitConverter.GetBytes(m_ScreenShotSize); // 13 - 16
//            for (int i = 0; i < tempBytes.Length; ++i)
//                tempArray[lastIdx++] = tempBytes[i];

//            if (lastIdx != tempArray.Length)
//                throw new Exception("Not everything has been written, StructureIE");

//            // Write To File
//            fw.Write(tempArray, 0, tempArray.Length);

//            // m_Zones write
//            tempArray = new byte[m_ZoneNum * 2];
//            lastIdx = 0;
//            for(int i = 0; i < m_ZoneNum; ++i)
//            {
//                tempBytes = BitConverter.GetBytes(m_Zones[i]);
//                for (int j = 0; j < tempBytes.Length; ++j)
//                    tempArray[lastIdx++] = tempBytes[j];
//            }

//            if (lastIdx != tempArray.Length)
//                throw new Exception("Not everything has been written, StructureIE");

//            // Write to File
//            fw.Write(tempArray, 0, tempArray.Length);

//            // Layer write
//            for (int i = 0; i < m_LayerNum; ++i)
//                m_Layers[i].WriteLayer(ref fw);

//            // Block write
//            for (int i = 0; i < m_BlockNum; ++i)
//                m_Blocks[i].WriteBlock(ref fw);

//            // Name write
//            fw.Write(m_Name, 0, m_NameLength);

//            // Screenshot write
//            fw.Write(m_ScreenShotData, 0, (int)m_ScreenShotSize);

//            fw.Close();
//        }

//        public static StructureIE FromFile(string path)
//        {
//            StructureIE sie = new StructureIE
//            {
//                m_IsFromFile = true
//            };
//            var fr = File.OpenRead(path);
//            if (!fr.CanRead || fr.Length < STATIC_SIZE)
//            {
//                fr.Close();
//                return null;
//            }

//            var tempArray = new byte[STATIC_SIZE];
//            fr.Read(tempArray, 0, tempArray.Length);

//            int lastIdx = 0;
//            // Magic num read
//            for (int i = 0; i < sie.MagicNum.Length; ++i)
//            {
//                if (tempArray[lastIdx++] != (byte)sie.MagicNum[i])
//                {
//                    fr.Close();
//                    return null; // Magic num was wrong
//                }
//            }

//            var version = tempArray[lastIdx++];
//            if (version != sie.StructureVersion)
//            {
//                fr.Close();
//                return null; // Version mismatch
//            }

//            var id = BitConverter.ToUInt16(tempArray, lastIdx);
//            if(id == ushort.MaxValue)
//            {
//                fr.Close();
//                return null; // Invalid id
//            }
//            sie.StructureID = id;
//            lastIdx += 2;

//            // Read flags
//            var tempBytes = new byte[Mathf.CeilToInt((float)Flag.COUNT / 8.0f)];
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempBytes[i] = tempArray[lastIdx];
//                ++lastIdx;
//            }
//            sie.Flags = new BitArray(tempBytes);

//            // Read layer num
//            var layerNum = tempArray[lastIdx++];
//            if (layerNum < 0 || layerNum > StructureComponent.LayerAmount)
//            {
//                fr.Close();
//                return null; // layer num is invalid
//            }
//            sie.m_LayerNum = layerNum;

//            // Read block num
//            var blockNum = BitConverter.ToUInt16(tempArray, lastIdx);
//            lastIdx += 2;
//            if (blockNum < 0 || blockNum >= (StructureComponent.Width * StructureComponent.Height * StructureComponent.LayerAmount))
//            {
//                fr.Close();
//                return null; // block num was invalid
//            }
//            sie.m_BlockNum = blockNum;

//            // Read zone Num
//            var zoneNum = tempArray[lastIdx++];
//            if(zoneNum < 0)
//            {
//                fr.Close();
//                return null;
//            }
//            sie.m_ZoneNum = zoneNum;

//            // Read Name length
//            sie.m_NameLength = tempArray[lastIdx++];

//            // Read screenshot data size
//            sie.m_ScreenShotSize = BitConverter.ToUInt32(tempArray, lastIdx);
//            lastIdx += 4;

//            if (lastIdx != tempArray.Length)
//                throw new Exception("Not everything has been read, StructureIE");

//            // Read zones
//            sie.m_Zones = new ushort[sie.m_ZoneNum];
//            for(int i = 0; i < zoneNum; ++i)
//            {
//                sie.m_Zones[i] = BitConverter.ToUInt16(tempArray, lastIdx);
//                lastIdx += 2;
//            }

//            // Read each layer
//            sie.m_Layers = new LayerIE[sie.m_LayerNum];
//            for (int i = 0; i < layerNum; ++i)
//            {
//                sie.m_Layers[i] = new LayerIE();
//                bool isOK = sie.m_Layers[i].ReadLayer(ref fr);
//                if (!isOK)
//                {
//                    fr.Close();
//                    return null;
//                }
//            }

//            // Read each block
//            sie.m_Blocks = new BlockIE[sie.m_BlockNum];
//            for(int i = 0; i < sie.m_BlockNum; ++i)
//            {
//                sie.m_Blocks[i] = new BlockIE();
//                bool isOK = sie.m_Blocks[i].ReadBlock(ref fr);
//                if (!isOK)
//                {
//                    fr.Close();
//                    return null;
//                }
//            }

//            // Read name
//            sie.m_Name = new byte[sie.m_NameLength];
//            fr.Read(sie.m_Name, 0, sie.m_NameLength);

//            // Read screen shot
//            sie.m_ScreenShotData = new byte[sie.m_ScreenShotSize];
//            fr.Read(sie.m_ScreenShotData, 0, (int)sie.m_ScreenShotSize);
//            sie.m_ScreenShot = new Texture2D(0, 0);
//            sie.m_ScreenShot.LoadImage(sie.m_ScreenShotData, false);

//            fr.Close();

//            return sie;
//        }

//        public override bool Equals(object obj)
//        {
//            if (!(obj is StructureIE))
//                return false;
//            var iE = obj as StructureIE;
//            return iE == this;
//        }

//        public override int GetHashCode()
//        {
//            var hashCode = -727961520;
//            hashCode = hashCode * -1521134295 + EqualityComparer<BitArray>.Default.GetHashCode(Flags);
//            hashCode = hashCode * -1521134295 + EqualityComparer<char[]>.Default.GetHashCode(MagicNum);
//            hashCode = hashCode * -1521134295 + StructureVersion.GetHashCode();
//            hashCode = hashCode * -1521134295 + StructureID.GetHashCode();
//            hashCode = hashCode * -1521134295 + GenerateBridges.GetHashCode();
//            hashCode = hashCode * -1521134295 + m_LayerNum.GetHashCode();
//            hashCode = hashCode * -1521134295 + m_BlockNum.GetHashCode();
//            hashCode = hashCode * -1521134295 + m_ZoneNum.GetHashCode();
//            hashCode = hashCode * -1521134295 + EqualityComparer<ushort[]>.Default.GetHashCode(m_Zones);
//            hashCode = hashCode * -1521134295 + EqualityComparer<LayerIE[]>.Default.GetHashCode(m_Layers);
//            hashCode = hashCode * -1521134295 + EqualityComparer<BlockIE[]>.Default.GetHashCode(m_Blocks);
//            return hashCode;
//        }
//    }

//    public class LayerIE
//    {
//        private enum Flag
//        {
//            IsLinkedLayer,
//            RandomBlockRotation,
//            RandomMicroHeight,
//            RandomBlockLength,
//            RandomProps,
//            SpawnMonsters,
//            SpawnZoneMonsters,
//            LayerMonstersRespawn,
//            HasEffects,
//            BlockFloat,
//            COUNT
//        }
//        static readonly int STATIC_SIZE = Mathf.CeilToInt((float)Flag.COUNT / 8.0f) + 
//            4 * 1 + 3 * 2 + 13 * 4;

//        public byte Layer;
//        private BitArray Flags;

//        public bool EnableRandomRotation
//        {
//            get
//            {
//                return Flags[(int)Flag.RandomBlockRotation];
//            }
//            set
//            {
//                if (IsLinkedLayer)
//                    return;
//                Flags.Set((int)Flag.RandomBlockRotation, value);
//            }
//        }
//        public BlockRotation DefaultBlockRotation;

//        public List<int> SubMaterials
//        {
//            get
//            {
//                var list = new List<int>(m_SubMaterialNum);
//                for (int i = 0; i < m_SubMaterialNum; ++i)
//                    list.Add(m_SubMaterials[i]);
//                return list;
//            }
//            set
//            {
//                m_SubMaterialNum = (byte)value.Count;
//                m_SubMaterials = new ushort[m_SubMaterialNum];
//                for (int i = 0; i < m_SubMaterialNum; ++i)
//                    m_SubMaterials[i] = (ushort)value[i];
//            }
//        }
//        public List<float> SubMaterialChances
//        {
//            get
//            {
//                return new List<float>(m_SubMaterialChances);
//            }
//            set
//            {
//                m_SubMaterialNum = (byte)value.Count;
//                m_SubMaterialChances = value.ToArray();
//            }
//        }

//        public float BlockHeight;
        
//        public bool EnableRandomMicroHeight
//        {
//            get
//            {
//                return Flags[(int)Flag.RandomMicroHeight];
//            }
//            set
//            {
//                if (IsLinkedLayer)
//                    return;
//                Flags.Set((int)Flag.RandomMicroHeight, value);
//            }
//        }
//        public float BlockMicroHeightMin;
//        public float BlockMicroHeightMax;

//        public bool EnableRandomBlockLenght
//        {
//            get
//            {
//                return Flags[(int)Flag.RandomBlockLength];
//            }
//            set
//            {
//                if (IsLinkedLayer)
//                    return;
//                Flags.Set((int)Flag.RandomBlockLength, value);
//            }
//        }
//        public float BlockLenghtMin;
//        public float BlockLenghtMax;

//        public bool EnableRandomProps
//        {
//            get
//            {
//                return Flags[(int)Flag.RandomProps];
//            }
//            set
//            {
//                if (IsLinkedLayer)
//                    return;
//                Flags.Set((int)Flag.RandomProps, value);
//            }
//        }
//        public float PropChance;
//        public List<int> Props
//        {
//            get
//            {
//                var list = new List<int>(m_PropNum);
//                for (int i = 0; i < m_PropNum; ++i)
//                    list.Add(m_Props[i]);
//                return list;
//            }
//            set
//            {
//                m_PropNum = (ushort)value.Count;
//                m_Props = new ushort[m_PropNum];
//                for (int i = 0; i < m_PropNum; ++i)
//                    m_Props[i] = (ushort)value[i];
//            }
//        }
//        public List<float> PropChances
//        {
//            get
//            {
//                return new List<float>(m_PropChances);
//            }
//            set
//            {
//                m_PropNum = (ushort)value.Count;
//                m_PropChances = value.ToArray();
//            }
//        }
//        public float PropNoSpawnRadius;

//        public bool SpawnMonsters
//        {
//            get
//            {
//                return Flags[(int)Flag.SpawnMonsters];
//            }
//            set
//            {
//                if (IsLinkedLayer)
//                    return;
//                Flags.Set((int)Flag.SpawnMonsters, value);
//            }
//        }
//        public bool SpawnZoneMonsters
//        {
//            get
//            {
//                return Flags[(int)Flag.SpawnZoneMonsters];
//            }
//            set
//            {
//                if (IsLinkedLayer)
//                    return;
//                Flags.Set((int)Flag.SpawnZoneMonsters, value);
//            }
//        }
//        public bool LayerMonstersRespawn
//        {
//            get
//            {
//                return Flags[(int)Flag.LayerMonstersRespawn];
//            }
//            set
//            {
//                if (IsLinkedLayer)
//                    return;
//                Flags.Set((int)Flag.LayerMonstersRespawn, value);
//            }
//        }
//        public List<int> Monsters
//        {
//            get
//            {
//                var list = new List<int>(m_MonsterNum);
//                for (int i = 0; i < m_MonsterNum; ++i)
//                    list.Add(m_Monsters[i]);
//                return list;
//            }
//            set
//            {
//                m_MonsterNum = (ushort)value.Count;
//                m_Monsters = new ushort[m_MonsterNum];
//                for (int i = 0; i < m_MonsterNum; ++i)
//                    m_Monsters[i] = (ushort)value[i];
//            }
//        }
//        public List<float> MonsterChances
//        {
//            get
//            {
//                return new List<float>(m_MonsterChances);
//            }
//            set
//            {
//                m_MonsterNum = (ushort)value.Count;
//                m_MonsterChances = value.ToArray();
//            }
//        }
//        public float MonsterChance;

//        public bool HasEffect
//        {
//            get
//            {
//                return Flags[(int)Flag.HasEffects];
//            }
//            set
//            {
//                if (IsLinkedLayer)
//                    return;
//                Flags.Set((int)Flag.HasEffects, value);
//            }
//        }
//        public List<int> Effects
//        {
//            get
//            {
//                var list = new List<int>(m_EffectNum);
//                for (int i = 0; i < m_EffectNum; ++i)
//                    list.Add(m_Effects[i]);
//                return list;
//            }
//            set
//            {
//                m_EffectNum = (ushort)value.Count;
//                m_Effects = new ushort[m_EffectNum];
//                for (int i = 0; i < m_EffectNum; ++i)
//                    m_Effects[i] = (ushort)value[i];
//            }
//        }

//        public bool EnableBlockFloat
//        {
//            get
//            {
//                return Flags[(int)Flag.BlockFloat];
//            }
//            set
//            {
//                if (IsLinkedLayer)
//                    return;
//                Flags.Set((int)Flag.BlockFloat, value);
//            }
//        }
//        public float BlockFloatMin;
//        public float BlockFloatMax;
//        public float BlockFloatSpeed;

//        public float RandomWideBlockChance;

//        public float RandomStairBlockChance;
        
//        public bool IsLinkedLayer
//        {
//            get
//            {
//                return Flags[(int)Flag.IsLinkedLayer];
//            }
//            set
//            {
//                if (value)
//                    Flags.SetAll(false);
//                Flags.Set((int)Flag.IsLinkedLayer, value);
//            }
//        }
//        public List<int> LinkedLayers
//        {
//            get
//            {
//                var list = new List<int>(m_LayerNum);
//                for (int i = 0; i < m_LayerNum; ++i)
//                    list.Add(m_LinkedLayers[i]);
//                return list;
//            }
//            set
//            {
//                m_LayerNum = (byte)value.Count;
//                m_LinkedLayers = new byte[m_LayerNum];
//                for (int i = 0; i < m_LayerNum; ++i)
//                    m_LinkedLayers[i] = (byte)value[i];
//            }
//        }
//        public List<float> LinkedLayerChances
//        {
//            get
//            {
//                return new List<float>(m_LinkedLayerChances);
//            }
//            set
//            {
//                m_LayerNum = (byte)value.Count;
//                m_LinkedLayerChances = value.ToArray();
//            }
//        }

//        public byte m_SubMaterialNum;
//        public ushort m_PropNum;
//        public ushort m_MonsterNum;
//        public ushort m_EffectNum;
//        public byte m_LayerNum;
        
//        private ushort[] m_SubMaterials;
//        private float[] m_SubMaterialChances;
        
//        private ushort[] m_Props;
//        private float[] m_PropChances;
        
//        private ushort[] m_Monsters;
//        private float[] m_MonsterChances;
        
//        private ushort[] m_Effects;
        
//        private byte[] m_LinkedLayers;
//        private float[] m_LinkedLayerChances;

//        public LayerIE()
//        {
//            Flags = new BitArray((int)Flag.COUNT);
//            Flags.SetAll(false);

//            Layer = 0;
//            m_SubMaterialNum = 0;
//            m_SubMaterials = null;
//            m_SubMaterialChances = null;

//            DefaultBlockRotation = (byte)BlockRotation.Default;

//            BlockHeight = 0.0f;
//            BlockMicroHeightMin = 0.0f;
//            BlockMicroHeightMax = 0.0f;

//            BlockLenghtMin = 1.0f;
//            BlockLenghtMax = 1.0f;

//            PropChance = 0.0f;
//            m_PropNum = 0;
//            m_Props = null;
//            m_PropChances = null;
//            PropNoSpawnRadius = 0.0f;

//            MonsterChance = 0.0f;
//            m_MonsterNum = 0;
//            m_Monsters = null;
//            m_MonsterChances = null;

//            m_EffectNum = 0;
//            m_Effects = null;

//            BlockFloatMin = 0.0f;
//            BlockFloatMax = 0.0f;
//            BlockFloatSpeed = 0.0f;

//            RandomWideBlockChance = 0.0f;
//            RandomStairBlockChance = 0.0f;

//            m_LayerNum = 0;
//            m_LinkedLayers = null;
//            m_LinkedLayerChances = null;

//        }

//        public LayerInfo ToLayerInfo()
//        {
//            var info = new LayerInfo
//            {
//                Layer = Layer,
//                MaterialTypes = new List<int>(m_SubMaterialNum),
//                MaterialTypeChances = new List<float>(m_SubMaterialNum),
//                EnableRandomRotation = EnableRandomRotation,
//                DefaultRotation = DefaultBlockRotation,
//                BlockHeight = BlockHeight,
//                EnableRandomMicroHeight = EnableRandomMicroHeight,
//                BlockMicroHeightMin = BlockMicroHeightMin,
//                BlockMicroHeightMax = BlockMicroHeightMax,
//                EnableRandomBlockLength = EnableRandomBlockLenght,
//                BlockLengthMin = BlockLenghtMin,
//                BlockLengthMax = BlockLenghtMax,
//                EnableRandomProps = EnableRandomProps,
//                PropChance = PropChance,
//                Props = new List<int>(m_PropNum),
//                PropChances = new List<float>(m_PropNum),
//                PropNoSpawnRadius = PropNoSpawnRadius,
//                SpawnMonsters = SpawnMonsters,
//                SpawnZoneMonsters = SpawnZoneMonsters,
//                LayerMonstersRespawn = LayerMonstersRespawn,
//                MonsterChance = MonsterChance,
//                Monsters = new List<int>(m_MonsterNum),
//                MonsterChances = new List<float>(m_MonsterNum),
//                HasEffects = HasEffect,
//                Effects = new List<int>(m_EffectNum),
//                EnableBlockFloat = EnableBlockFloat,
//                BlockFloatMin = BlockFloatMin,
//                BlockFloatMax = BlockFloatMax,
//                BlockFloatSpeed = BlockFloatSpeed,
//                RandomWideBlockChance = RandomWideBlockChance,
//                RandomStairBlockChance = RandomStairBlockChance,
//                IsLinkedLayer = IsLinkedLayer,
//                LinkedLayers = new List<int>(m_LayerNum),
//                LinkedLayerChances = new List<float>(m_LayerNum),
//                //IE = this
//            };

//            for(int i = 0; i < m_SubMaterialNum; ++i)
//            {
//                info.MaterialTypes.Add(m_SubMaterials[i]);
//                info.MaterialTypeChances.Add(m_SubMaterialChances[i]);
//            }

//            for(int i = 0; i < m_PropNum; ++i)
//            {
//                info.Props.Add(m_Props[i]);
//                info.PropChances.Add(m_PropChances[i]);
//            }

//            for (int i = 0; i < m_MonsterNum; ++i)
//            {
//                info.Monsters.Add(m_Monsters[i]);
//                info.MonsterChances.Add(m_MonsterChances[i]);
//            }

//            for (int i = 0; i < m_EffectNum; ++i)
//            {
//                info.Effects.Add(m_Effects[i]);
//            }

//            for(int i = 0; i < m_LayerNum; ++i)
//            {
//                info.LinkedLayers.Add(m_LinkedLayers[i]);
//                info.LinkedLayerChances.Add(m_LinkedLayerChances[i]);
//            }

//            return info;
//        }

//        public static bool operator==(LayerIE left, LayerIE right)
//        {
//            if (left is null && !(right is null))
//                return false;
//            if (!(left is null) && right is null)
//                return false;
//            if (left is null && right is null)
//                return true;

//            if (left.Layer != right.Layer)
//                return false;

//            if(left.IsLinkedLayer)
//            {
//                if (!right.IsLinkedLayer)
//                    return false;

//                if (left.m_LayerNum != right.m_LayerNum)
//                    return false;

//                for(int i = 0; i < left.m_LayerNum; ++i)
//                {
//                    if (left.m_LinkedLayers[i] != right.m_LinkedLayers[i])
//                        return false;

//                    if (left.m_LinkedLayerChances[i] != right.m_LinkedLayerChances[i])
//                        return false;
//                }

//                return true;
//            }

//            for (int i = (int)Flag.IsLinkedLayer + 1; i < (int)Flag.COUNT; ++i)
//            {
//                if (left.Flags[i] != right.Flags[i])
//                    return false;
//            }

//            if (left.m_SubMaterialNum != right.m_SubMaterialNum)
//                return false;

//            for(int i = 0; i < left.m_SubMaterialNum; ++i)
//            {
//                if (left.m_SubMaterials[i] != right.m_SubMaterials[i])
//                    return false;

//                if (left.m_SubMaterialChances[i] != right.m_SubMaterialChances[i])
//                    return false;
//            }

//            if (!left.EnableRandomRotation
//                && left.DefaultBlockRotation != right.DefaultBlockRotation)
//                return false;

//            if (left.BlockHeight != right.BlockHeight)
//                return false;

//            if (left.EnableRandomMicroHeight
//                && (left.BlockMicroHeightMin != right.BlockMicroHeightMin
//                || left.BlockMicroHeightMax != right.BlockMicroHeightMax))
//                return false;

//            if (left.EnableRandomBlockLenght
//                && (left.BlockLenghtMin != right.BlockLenghtMin
//                || left.BlockLenghtMax != right.BlockLenghtMax))
//                return false;

//            if(left.EnableRandomProps)
//            {
//                if (left.PropChance != right.PropChance)
//                    return false;

//                if (left.m_PropNum != right.m_PropNum)
//                    return false;

//                for(int i = 0; i < left.m_PropNum; ++i)
//                {
//                    if (left.m_Props[i] != right.m_Props[i])
//                        return false;

//                    if (left.m_PropChances[i] != right.m_PropChances[i])
//                        return false;
//                }

//                if (left.PropNoSpawnRadius != right.PropNoSpawnRadius)
//                    return false;
//            }

//            if(left.SpawnMonsters)
//            {
//                if (left.MonsterChance != right.MonsterChance)
//                    return false;

//                if (left.m_MonsterNum != right.m_MonsterNum)
//                    return false;

//                for(int i = 0; i < left.m_MonsterNum; ++i)
//                {
//                    if (left.m_Monsters[i] != right.m_Monsters[i])
//                        return false;

//                    if (left.m_MonsterChances[i] != right.m_MonsterChances[i])
//                        return false;
//                }
//            }

//            if(left.HasEffect)
//            {
//                if (left.m_EffectNum != right.m_EffectNum)
//                    return false;

//                for(int i = 0; i < left.m_EffectNum; ++i)
//                {
//                    if (left.m_Effects[i] != right.m_Effects[i])
//                        return false;
//                }
//            }

//            if(left.EnableBlockFloat)
//            {
//                if (left.BlockFloatMin != right.BlockFloatMin)
//                    return false;

//                if (left.BlockFloatMax != right.BlockFloatMax)
//                    return false;

//                if (left.BlockFloatSpeed != right.BlockFloatSpeed)
//                    return false;
//            }

//            if (left.RandomWideBlockChance != right.RandomWideBlockChance)
//                return false;

//            if (left.RandomStairBlockChance != right.RandomStairBlockChance)
//                return false;
            
//            return true;
//        }

//        public static bool operator!=(LayerIE left, LayerIE right)
//        {
//            return !(left == right);
//        }

//        public bool ReadLayer(ref FileStream fr)
//        {
//            if ((fr.Length - fr.Position) < STATIC_SIZE)
//                return false;
//            var tempArray = new byte[STATIC_SIZE];
//            fr.Read(tempArray, 0, tempArray.Length);

//            Layer = tempArray[0];
//            int lastIdx = 1;
//            var tempBytes = new byte[Mathf.CeilToInt((float)Flag.COUNT / 8.0f)];
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempBytes[i] = tempArray[lastIdx];
//                ++lastIdx;
//            }

//            Flags = new BitArray(tempBytes);

//            m_SubMaterialNum = tempArray[lastIdx++];
//            m_SubMaterials = new ushort[m_SubMaterialNum];
//            m_SubMaterialChances = new float[m_SubMaterialNum];
//            DefaultBlockRotation = (BlockRotation)tempArray[lastIdx++];

//            BlockHeight = BitConverter.ToSingle(tempArray, lastIdx);
//            lastIdx += 4;

//            BlockMicroHeightMin = BitConverter.ToSingle(tempArray, lastIdx);
//            lastIdx += 4;

//            BlockMicroHeightMax = BitConverter.ToSingle(tempArray, lastIdx);
//            lastIdx += 4;

//            BlockLenghtMin = BitConverter.ToSingle(tempArray, lastIdx);
//            lastIdx += 4;

//            BlockLenghtMax = BitConverter.ToSingle(tempArray, lastIdx);
//            lastIdx += 4;

//            PropChance = BitConverter.ToSingle(tempArray, lastIdx);
//            lastIdx += 4;

//            m_PropNum = BitConverter.ToUInt16(tempArray, lastIdx);
//            lastIdx += 2;
//            m_Props = new ushort[m_PropNum];
//            m_PropChances = new float[m_PropNum];

//            PropNoSpawnRadius = BitConverter.ToSingle(tempArray, lastIdx);
//            lastIdx += 4;

//            MonsterChance = BitConverter.ToSingle(tempArray, lastIdx);
//            lastIdx += 4;

//            m_MonsterNum = BitConverter.ToUInt16(tempArray, lastIdx);
//            lastIdx += 2;
//            m_Monsters = new ushort[m_MonsterNum];
//            m_MonsterChances = new float[m_MonsterNum];

//            m_EffectNum = BitConverter.ToUInt16(tempArray, lastIdx);
//            lastIdx += 2;
//            m_Effects = new ushort[m_EffectNum];

//            BlockFloatMin = BitConverter.ToSingle(tempArray, lastIdx);
//            lastIdx += 4;

//            BlockFloatMax = BitConverter.ToSingle(tempArray, lastIdx);
//            lastIdx += 4;

//            BlockFloatSpeed = BitConverter.ToSingle(tempArray, lastIdx);
//            lastIdx += 4;

//            RandomWideBlockChance = BitConverter.ToSingle(tempArray, lastIdx);
//            lastIdx += 4;

//            RandomStairBlockChance = BitConverter.ToSingle(tempArray, lastIdx);
//            lastIdx += 4;

//            m_LayerNum = tempArray[lastIdx++];
//            if (lastIdx != tempArray.Length)
//                throw new Exception("Not everything has been read, LayerIE");

//            m_LinkedLayers = new byte[m_LayerNum];
//            m_LinkedLayerChances = new float[m_LayerNum];

//            if(IsLinkedLayer)
//            {
//                tempArray = new byte[m_LayerNum * 1 + m_LayerNum * 4];
//                fr.Read(tempArray, 0, tempArray.Length);

//                for(int i = 0; i < m_LayerNum; ++i)
//                {
//                    m_LinkedLayers[i] = tempArray[i];
//                }
//                lastIdx = m_LayerNum;
//                for (int i = 0; i < m_LayerNum; ++i)
//                {
//                    m_LinkedLayerChances[i] = BitConverter.ToSingle(tempArray, lastIdx);
//                    lastIdx += 4;
//                }
//                if (lastIdx != tempArray.Length)
//                    throw new Exception("Not everything has been read, LayerIE");
//            }
//            else
//            {
//                tempArray = new byte[m_SubMaterialNum * (2 + 4)
//                    + m_PropNum * (2 + 4) + m_MonsterNum * (2 + 4) + m_EffectNum * 2];
//                fr.Read(tempArray, 0, tempArray.Length);

//                lastIdx = 0;
//                // SubMaterials
//                for(int i = 0; i < m_SubMaterialNum; ++i)
//                {
//                    m_SubMaterials[i] = BitConverter.ToUInt16(tempArray, lastIdx);
//                    lastIdx += 2;
//                }
//                for(int i = 0; i < m_SubMaterialNum; ++i)
//                {
//                    m_SubMaterialChances[i] = BitConverter.ToSingle(tempArray, lastIdx);
//                    lastIdx += 4;
//                }
//                // Props
//                for (int i = 0; i < m_PropNum; ++i)
//                {
//                    m_Props[i] = BitConverter.ToUInt16(tempArray, lastIdx);
//                    lastIdx += 2;
//                }
//                for (int i = 0; i < m_PropNum; ++i)
//                {
//                    m_PropChances[i] = BitConverter.ToSingle(tempArray, lastIdx);
//                    lastIdx += 4;
//                }
//                // Monsters
//                for (int i = 0; i < m_MonsterNum; ++i)
//                {
//                    m_Monsters[i] = BitConverter.ToUInt16(tempArray, lastIdx);
//                    lastIdx += 2;
//                }
//                for (int i = 0; i < m_MonsterNum; ++i)
//                {
//                    m_MonsterChances[i] = BitConverter.ToSingle(tempArray, lastIdx);
//                    lastIdx += 4;
//                }
//                // Effects
//                for (int i = 0; i < m_EffectNum; ++i)
//                {
//                    m_Effects[i] = BitConverter.ToUInt16(tempArray, lastIdx);
//                    lastIdx += 2;
//                }
//                if (lastIdx != tempArray.Length)
//                    throw new Exception("Not everything has been read, LayerIE");
//            }

//            return true;
//        }

//        public bool WriteLayer(ref FileStream fw)
//        {
//            var tempArray = new byte[STATIC_SIZE];

//            tempArray[0] = Layer;
//            int lastIdx = 1;
//            Flags.CopyTo(tempArray, lastIdx);
//            lastIdx += Mathf.CeilToInt((float)Flag.COUNT / 8.0f);

//            tempArray[lastIdx++] = m_SubMaterialNum;
//            tempArray[lastIdx++] = (byte)DefaultBlockRotation;

//            var tempBytes = BitConverter.GetBytes(BlockHeight);
//            for(int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx++] = tempBytes[i];
//            }

//            tempBytes = BitConverter.GetBytes(BlockMicroHeightMin);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx++] = tempBytes[i];
//            }

//            tempBytes = BitConverter.GetBytes(BlockMicroHeightMax);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx++] = tempBytes[i];
//            }

//            tempBytes = BitConverter.GetBytes(BlockLenghtMin);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx++] = tempBytes[i];
//            }

//            tempBytes = BitConverter.GetBytes(BlockLenghtMax);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx++] = tempBytes[i];
//            }

//            tempBytes = BitConverter.GetBytes(PropChance);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx++] = tempBytes[i];
//            }

//            tempBytes = BitConverter.GetBytes(m_PropNum);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx++] = tempBytes[i];
//            }

//            tempBytes = BitConverter.GetBytes(PropNoSpawnRadius);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx++] = tempBytes[i];
//            }

//            tempBytes = BitConverter.GetBytes(MonsterChance);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx++] = tempBytes[i];
//            }

//            tempBytes = BitConverter.GetBytes(m_MonsterNum);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx++] = tempBytes[i];
//            }

//            tempBytes = BitConverter.GetBytes(m_EffectNum);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx++] = tempBytes[i];
//            }

//            tempBytes = BitConverter.GetBytes(BlockFloatMin);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx++] = tempBytes[i];
//            }

//            tempBytes = BitConverter.GetBytes(BlockFloatMax);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx++] = tempBytes[i];
//            }

//            tempBytes = BitConverter.GetBytes(BlockFloatSpeed);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx++] = tempBytes[i];
//            }

//            tempBytes = BitConverter.GetBytes(RandomWideBlockChance);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx++] = tempBytes[i];
//            }

//            tempBytes = BitConverter.GetBytes(RandomStairBlockChance);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx++] = tempBytes[i];
//            }

//            tempArray[lastIdx++] = m_LayerNum;
//            if (lastIdx != tempArray.Length)
//                throw new Exception("Not everything has been written, LayerIE");
//            fw.Write(tempArray, 0, tempArray.Length);

//            if(IsLinkedLayer)
//            {
//                tempArray = new byte[(4 + 1) * m_LayerNum];
//                lastIdx = 0;
//                for(int i = 0; i < m_LayerNum; ++i)
//                {
//                    tempArray[lastIdx++] = m_LinkedLayers[i];
//                }
//                for (int i = 0; i < m_LayerNum; ++i)
//                {
//                    tempBytes = BitConverter.GetBytes(m_LinkedLayerChances[i]);
//                    for (int j = 0; j < tempBytes.Length; ++j)
//                    {
//                        tempArray[lastIdx++] = tempBytes[j];
//                    }
//                }
//                if (lastIdx != tempArray.Length)
//                    throw new Exception("Not everything has been written, LayerIE");
//                fw.Write(tempArray, 0, tempArray.Length);
//            }
//            else
//            {
//                tempArray = new byte[(2 + 4) * m_SubMaterialNum
//                    + (2 + 4) * m_PropNum + (2 + 4) * m_MonsterNum
//                    + (2) * m_EffectNum];

//                lastIdx = 0;
//                // Sub material
//                for (int i = 0; i < m_SubMaterialNum; ++i)
//                {
//                    tempBytes = BitConverter.GetBytes(m_SubMaterials[i]);
//                    for (int j = 0; j < tempBytes.Length; ++j)
//                    {
//                        tempArray[lastIdx++] = tempBytes[j];
//                    }
//                }
//                for (int i = 0; i < m_SubMaterialNum; ++i)
//                {
//                    tempBytes = BitConverter.GetBytes(m_SubMaterialChances[i]);
//                    for (int j = 0; j < tempBytes.Length; ++j)
//                    {
//                        tempArray[lastIdx++] = tempBytes[j];
//                    }
//                }
//                // Props
//                for (int i = 0; i < m_PropNum; ++i)
//                {
//                    tempBytes = BitConverter.GetBytes(m_Props[i]);
//                    for (int j = 0; j < tempBytes.Length; ++j)
//                    {
//                        tempArray[lastIdx++] = tempBytes[j];
//                    }
//                }
//                for (int i = 0; i < m_PropNum; ++i)
//                {
//                    tempBytes = BitConverter.GetBytes(m_PropChances[i]);
//                    for (int j = 0; j < tempBytes.Length; ++j)
//                    {
//                        tempArray[lastIdx++] = tempBytes[j];
//                    }
//                }
//                // Monsters
//                for (int i = 0; i < m_MonsterNum; ++i)
//                {
//                    tempBytes = BitConverter.GetBytes(m_Monsters[i]);
//                    for (int j = 0; j < tempBytes.Length; ++j)
//                    {
//                        tempArray[lastIdx++] = tempBytes[j];
//                    }
//                }
//                for (int i = 0; i < m_MonsterNum; ++i)
//                {
//                    tempBytes = BitConverter.GetBytes(m_MonsterChances[i]);
//                    for (int j = 0; j < tempBytes.Length; ++j)
//                    {
//                        tempArray[lastIdx++] = tempBytes[j];
//                    }
//                }
//                // Effects
//                for (int i = 0; i < m_EffectNum; ++i)
//                {
//                    tempBytes = BitConverter.GetBytes(m_Effects[i]);
//                    for (int j = 0; j < tempBytes.Length; ++j)
//                    {
//                        tempArray[lastIdx++] = tempBytes[j];
//                    }
//                }
//                if (lastIdx != tempArray.Length)
//                    throw new Exception("Not everything has been written, LayerIE");
//                fw.Write(tempArray, 0, tempArray.Length);
//            }

//            return true;
//        }

//        public override bool Equals(object obj)
//        {
//            if (!(obj is LayerIE))
//                return false;
//            var iE = obj as LayerIE;
//            return iE == this;
//        }

//        public override int GetHashCode()
//        {
//            var hashCode = 719741704;
//            hashCode = hashCode * -1521134295 + EqualityComparer<BitArray>.Default.GetHashCode(Flags);
//            hashCode = hashCode * -1521134295 + Layer.GetHashCode();
//            hashCode = hashCode * -1521134295 + m_SubMaterialNum.GetHashCode();
//            hashCode = hashCode * -1521134295 + EqualityComparer<ushort[]>.Default.GetHashCode(m_SubMaterials);
//            hashCode = hashCode * -1521134295 + EqualityComparer<float[]>.Default.GetHashCode(m_SubMaterialChances);
//            hashCode = hashCode * -1521134295 + DefaultBlockRotation.GetHashCode();
//            hashCode = hashCode * -1521134295 + BlockHeight.GetHashCode();
//            hashCode = hashCode * -1521134295 + BlockMicroHeightMin.GetHashCode();
//            hashCode = hashCode * -1521134295 + BlockMicroHeightMax.GetHashCode();
//            hashCode = hashCode * -1521134295 + BlockLenghtMin.GetHashCode();
//            hashCode = hashCode * -1521134295 + BlockLenghtMax.GetHashCode();
//            hashCode = hashCode * -1521134295 + PropChance.GetHashCode();
//            hashCode = hashCode * -1521134295 + m_PropNum.GetHashCode();
//            hashCode = hashCode * -1521134295 + EqualityComparer<ushort[]>.Default.GetHashCode(m_Props);
//            hashCode = hashCode * -1521134295 + EqualityComparer<float[]>.Default.GetHashCode(m_PropChances);
//            hashCode = hashCode * -1521134295 + PropNoSpawnRadius.GetHashCode();
//            hashCode = hashCode * -1521134295 + MonsterChance.GetHashCode();
//            hashCode = hashCode * -1521134295 + m_MonsterNum.GetHashCode();
//            hashCode = hashCode * -1521134295 + EqualityComparer<ushort[]>.Default.GetHashCode(m_Monsters);
//            hashCode = hashCode * -1521134295 + EqualityComparer<float[]>.Default.GetHashCode(m_MonsterChances);
//            hashCode = hashCode * -1521134295 + m_EffectNum.GetHashCode();
//            hashCode = hashCode * -1521134295 + EqualityComparer<ushort[]>.Default.GetHashCode(m_Effects);
//            hashCode = hashCode * -1521134295 + BlockFloatMin.GetHashCode();
//            hashCode = hashCode * -1521134295 + BlockFloatMax.GetHashCode();
//            hashCode = hashCode * -1521134295 + BlockFloatSpeed.GetHashCode();
//            hashCode = hashCode * -1521134295 + RandomWideBlockChance.GetHashCode();
//            hashCode = hashCode * -1521134295 + RandomStairBlockChance.GetHashCode();
//            hashCode = hashCode * -1521134295 + m_LayerNum.GetHashCode();
//            hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(m_LinkedLayers);
//            hashCode = hashCode * -1521134295 + EqualityComparer<float[]>.Default.GetHashCode(m_LinkedLayerChances);
//            return hashCode;
//        }
//    }

//    public class BlockIE
//    {
//        public enum Flag
//        {
//            Anchor,
//            Rotation,
//            Monster,
//            Prop,
//            MaterialType,
//            BlockType,
//            Length,
//            Height,
//            COUNT
//        }
//        static readonly int SIZE = Mathf.CeilToInt((float)Flag.COUNT / 8.0f) + 5 * 1 + 3 * 2 + 2 * 4;

//        public byte StructureID;
//        public byte Layer;
//        public StairType Stair;
//        private BitArray _Flags;
//        public BitArray Flags
//        {
//            get
//            {
//                return _Flags;
//            }
//        }

//        public bool Anchor
//        {
//            get
//            {
//                return _Flags.Get((int)Flag.Anchor);
//            }
//            set
//            {
//                _Flags.Set((int)Flag.Anchor, value);
//            }
//        }
//        private BlockRotation m_Rotation;
//        public BlockRotation BlockRotation
//        {
//            get
//            {
//                return m_Rotation;
//            }
//            set
//            {
//                m_Rotation = value;
//                bool flagSet = m_Rotation != BlockRotation.COUNT;
//                _Flags.Set((int)Flag.Rotation, flagSet);
//            }
//        }
//        private ushort m_Monster;
//        public int MonsterID
//        {
//            get
//            {
//                return m_Monster;
//            }
//            set
//            {
//                m_Monster = (ushort)value;
//                bool flagSet = !(m_Monster == ushort.MaxValue);
//                _Flags.Set((int)Flag.Monster, flagSet);
//            }
//        }
//        private ushort m_Prop;
//        public int PropID
//        {
//            get
//            {
//                return m_Prop;
//            }
//            set
//            {
//                m_Prop = (ushort)value;
//                bool flagSet = !(m_Prop == ushort.MaxValue);
//                _Flags.Set((int)Flag.Prop, flagSet);
//            }
//        }
//        private byte m_MaterialFamilyLength;
//        private byte[] m_MaterialFamilyName;
//        private ushort m_MaterialTypeID;
//        public int MaterialTypeID
//        {
//            get
//            {
//                return m_MaterialTypeID;
//            }
//            set
//            {
//                m_MaterialTypeID = (ushort)value;
//                bool flagSet = !(m_MaterialTypeID == 0);
//                _Flags.Set((int)Flag.MaterialType, flagSet);
//            }
//        }
//        private BlockType m_BlockType;
//        public BlockType blockType
//        {
//            get
//            {
//                return m_BlockType;
//            }
//            set
//            {
//                m_BlockType = value;
//                bool flagSet = !(m_BlockType == BlockType.COUNT);
//                _Flags.Set((int)Flag.BlockType, flagSet);
//            }
//        }
//        private float m_Length;
//        public float Length
//        {
//            get
//            {
//                return m_Length;
//            }
//            set
//            {
//                m_Length = value;
//                bool flagSet = !(m_Length < 0.5);
//                _Flags.Set((int)Flag.Length, flagSet);
//            }
//        }
//        private float m_Height;
//        public float Height
//        {
//            get
//            {
//                return m_Height;
//            }
//            set
//            {
//                m_Height = value;
//                bool flagSet = !(float.IsInfinity(m_Height) || float.IsNaN(m_Height));
//                _Flags.Set((int)Flag.Height, flagSet);
//            }
//        }

//        public BlockIE()
//        {
//            _Flags = new BitArray((int)Flag.COUNT);
//            SetDefault();
//        }

//        public void SetDefault()
//        {
//            _Flags.SetAll(false);
//            Layer = 0;
//            Stair = StairType.NONE;
//            m_Monster = 0;
//            m_Prop = 0;
//            m_MaterialTypeID = 0;
//            m_BlockType = BlockType.NORMAL;
//            m_Length = 0.5f;
//            m_Height = 0.0f;
//        }

//        public void Apply(BlockComponent block)
//        {
//            int locked = 0;
//            block.Stair = Stair == StairType.COUNT ? StairType.NONE : Stair;
//            block.Anchor = Anchor;
//            if (_Flags[(int)Flag.Rotation] && m_Rotation != BlockRotation.COUNT)
//            {
//                block.Rotation = m_Rotation;
//                ++locked;
//            }

//            if(_Flags[(int)Flag.Monster] && m_Monster > 0)
//            {
//                if (block.Monster != null)
//                    block.Monster.ReceiveDamage(Def.DamageType.UNAVOIDABLE, block.Monster.GetTotalHealth());
//                var monster = Monsters.MonsterSprites[m_Monster];
//                var mon = new GameObject($"Monster_{MonsterScript.MonsterID++}");
//                block.Monster = mon.AddComponent<MonsterScript>();

//                mon.transform.Translate(block.Pilar.transform.position, Space.World);
//                mon.transform.Translate(new Vector3(0.0f, block.Height + block.MicroHeight, 0.0f), Space.World);
//                block.Monster = Monsters.AddMonsterComponent(mon, m_Monster);
//                block.Monster.InitMonster();
//                //block.Monster.SetMonster(m_Monster);
//                block.Monster.enabled = true;
//                block.Monster.SpriteSR.enabled = false;
//                //block.Monster.SpriteBC.enabled = false;
//                block.Monster.SpriteCC.enabled = false;
//                block.Monster.ShadowSR.enabled = false;
//                block.Monster.Struc = block.Pilar.Struc;
//                var facing = block.Monster.Facing;
//                float nChance = (float)Manager.Mgr.BuildRNG.NextDouble();
//                if (nChance >= 0.5f)
//                    facing.Horizontal = SpriteHorizontal.RIGHT;
//                nChance = (float)Manager.Mgr.BuildRNG.NextDouble();
//                if (nChance >= 0.5f)
//                    facing.Vertical = SpriteVertical.UP;
//                block.Monster.Facing = facing;
//                ++locked;
//            }

//            if(_Flags[(int)Flag.Prop] && m_Prop > 0)
//            {
//                if (block.Prop != null)
//                    block.Prop.ReceiveDamage(Def.DamageType.UNAVOIDABLE, block.Prop.GetTotalHealth());
//                var propFamily = Props.PropFamilies[m_Prop];
//                if (propFamily.Props.Length == 0)
//                    throw new Exception($"This prop {propFamily.FamilyName}, does not have any available prop.");

//                var propID = Manager.Mgr.BuildRNG.Next(0, propFamily.Props.Length);
//                var prop = new GameObject($"Prop_{PropScript.PropID++}");
//                block.Prop = prop.AddComponent<PropScript>();
//                block.Prop.SetProp(m_Prop, propID);
//                block.Prop.Block = block;
//                block.Prop.enabled = true;
//                block.Prop.SpriteSR.enabled = false;
//                block.Prop.SpriteBC.enabled = false;
//                if(block.Prop.ShadowSR != null)
//                    block.Prop.ShadowSR.enabled = false;
//                if (block.Prop.PropLight != null)
//                    block.Prop.PropLight.enabled = false;
//                float nChance = (float)Manager.Mgr.BuildRNG.NextDouble();
//                var facing = block.Prop.Facing;
//                if (nChance >= 0.5f)
//                    facing.Horizontal = SpriteHorizontal.RIGHT;
//                block.Prop.Facing = facing;
//                ++locked;
//            }

//            if(_Flags[(int)Flag.BlockType] && m_BlockType != BlockType.COUNT)
//            {
//                block.blockType = m_BlockType;
//                ++locked;
//            }

//            if(_Flags[(int)Flag.MaterialType] && m_MaterialTypeID > 0)
//            {
//                block.MaterialTypeID = m_MaterialTypeID;
//                ++locked;
//            }

//            var maxLength = Mathf.Abs(BlockMeshDef.MidMesh.VertexHeight[block.blockType == BlockType.WIDE ? 1 : 0].y);
//            if(_Flags[(int)Flag.Length] && m_Length >= 0.5f && m_Length <= maxLength)
//            {
//                block.Length = m_Length;
//                ++locked;
//            }

//            if(_Flags[(int)Flag.Height] && !float.IsInfinity(m_Height) && !float.IsNaN(m_Height))
//            {
//                block.Height = m_Height;
//                ++locked;
//            }
//            // Theres something or everything locked
//            if(locked == 7)
//            {
//                block.Locked = BlockLock.Locked;
//            }
//            else if(locked > 0)
//            {
//                block.Locked = BlockLock.SemiLocked;
//            }
//            else
//            {
//                block.Locked = BlockLock.Unlocked;
//            }
//        }

//        public static bool operator==(BlockIE left, BlockIE right)
//        {
//            if (left is null && !(right is null))
//                return false;
//            if (!(left is null) && right is null)
//                return false;
//            if (left is null && right is null)
//                return true;

//            if (left.StructureID != right.StructureID)
//                return false;

//            if (left.Layer != right.Layer)
//                return false;

//            if (left.Stair != right.Stair)
//                return false;

//            for(int i = 0; i < (int)Flag.COUNT; ++i)
//            {
//                if (left._Flags[i] != right._Flags[i])
//                    return false;
//            }

//            if (left._Flags[(int)Flag.Rotation]
//                && left.m_Rotation != right.m_Rotation)
//                return false;

//            if (left._Flags[(int)Flag.Monster]
//                && left.MonsterID != right.MonsterID)
//                return false;

//            if (left._Flags[(int)Flag.Prop]
//                && left.PropID != right.PropID)
//                return false;

//            if (left._Flags[(int)Flag.MaterialType]
//                && left.MaterialTypeID != right.MaterialTypeID)
//                return false;

//            if (left._Flags[(int)Flag.BlockType]
//                && left.blockType != right.blockType)
//                return false;

//            if (left._Flags[(int)Flag.Length]
//                && left.Length != right.Length)
//                return false;

//            if (left._Flags[(int)Flag.Height]
//                && left.Height != right.Height)
//                return false;

//            return true;
//        }

//        public static bool operator!=(BlockIE left, BlockIE right)
//        {
//            return !(left == right);
//        }

//        public bool ReadBlock(ref FileStream fr)
//        {
//            if ((fr.Length - fr.Position) < SIZE)
//                return false;

//            var tempArray = new byte[SIZE];
//            fr.Read(tempArray, 0, tempArray.Length);

//            StructureID = tempArray[0];
//            Layer = tempArray[1];
//            Stair = (StairType)tempArray[2];
//            int lastIdx = 3;
//            _Flags.Length = (int)Flag.COUNT;
//            var tempBytes = new byte[Mathf.CeilToInt((float)Flag.COUNT / 8.0f)];
//            for(int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempBytes[i] = tempArray[lastIdx];
//                ++lastIdx;
//            }
//            _Flags = new BitArray(tempBytes);
//            m_Rotation = (BlockRotation)tempArray[lastIdx];
//            ++lastIdx;

//            m_Monster = BitConverter.ToUInt16(tempArray, lastIdx);
//            lastIdx += 2;

//            m_Prop = BitConverter.ToUInt16(tempArray, lastIdx);
//            lastIdx += 2;

//            m_MaterialTypeID = BitConverter.ToUInt16(tempArray, lastIdx);
//            lastIdx += 2;

//            m_BlockType = (BlockType)tempArray[lastIdx];
//            ++lastIdx;

//            m_Length = BitConverter.ToSingle(tempArray, lastIdx);
//            lastIdx += 4;

//            m_Height = BitConverter.ToSingle(tempArray, lastIdx);
//            lastIdx += 4;

//            if (lastIdx != tempArray.Length)
//                throw new Exception("Not everything has been read, BlockDefIE");

//            return true;
//        }

//        public bool WriteBlock(ref FileStream fw)
//        {
//            var tempArray = new byte[SIZE];

//            if (Layer == 0)
//                throw new Exception("Trying to save an invalid block!");

//            tempArray[0] = StructureID;
//            tempArray[1] = Layer;
//            tempArray[2] = (byte)Stair;
//            _Flags.CopyTo(tempArray, 3);
//            int lastIdx = 3 + Mathf.CeilToInt((float)Flag.COUNT / 8.0f);
//            tempArray[lastIdx] = (byte)m_Rotation;
//            ++lastIdx;

//            // MonsterID
//            var tempBytes = BitConverter.GetBytes(m_Monster);
//            for(int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx] = tempBytes[i];
//                ++lastIdx;
//            }
            
//            // PropID
//            tempBytes = BitConverter.GetBytes(m_Prop);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx] = tempBytes[i];
//                ++lastIdx;
//            }
            
//            // MaterialType
//            tempBytes = BitConverter.GetBytes(m_MaterialTypeID);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx] = tempBytes[i];
//                ++lastIdx;
//            }
            
//            tempArray[lastIdx] = (byte)m_BlockType;
//            ++lastIdx;

//            // Length
//            tempBytes = BitConverter.GetBytes(m_Length);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx] = tempBytes[i];
//                ++lastIdx;
//            }

//            // Height
//            tempBytes = BitConverter.GetBytes(m_Height);
//            for (int i = 0; i < tempBytes.Length; ++i)
//            {
//                tempArray[lastIdx] = tempBytes[i];
//                ++lastIdx;
//            }

//            if (lastIdx != tempArray.Length)
//                throw new Exception("Not everything has been written, BlockDefIE");

//            fw.Write(tempArray, 0, tempArray.Length);

//            return true;
//        }

//        public override bool Equals(object obj)
//        {
//            if (!(obj is BlockIE))
//                return false;
//            var iE = obj as BlockIE;
//            return iE == this;
//        }

//        public override int GetHashCode()
//        {
//            var hashCode = 1038191710;
//            hashCode = hashCode * -1521134295 + StructureID.GetHashCode();
//            hashCode = hashCode * -1521134295 + Layer.GetHashCode();
//            hashCode = hashCode * -1521134295 + Stair.GetHashCode();
//            hashCode = hashCode * -1521134295 + EqualityComparer<BitArray>.Default.GetHashCode(_Flags);
//            hashCode = hashCode * -1521134295 + m_Rotation.GetHashCode();
//            hashCode = hashCode * -1521134295 + m_Monster.GetHashCode();
//            hashCode = hashCode * -1521134295 + m_Prop.GetHashCode();
//            hashCode = hashCode * -1521134295 + m_MaterialTypeID.GetHashCode();
//            hashCode = hashCode * -1521134295 + m_BlockType.GetHashCode();
//            hashCode = hashCode * -1521134295 + m_Length.GetHashCode();
//            hashCode = hashCode * -1521134295 + m_Height.GetHashCode();
//            return hashCode;
//        }
//    }
//}