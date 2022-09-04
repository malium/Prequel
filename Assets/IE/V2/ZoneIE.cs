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
    public class ZoneIE
    {
        const int STATIC_SIZE = 8 * 1 + 1 * 2;

        public string Name
        {
            get
            {
                var chr = new char[m_NameLength];
                for (int i = 0; i < m_NameLength; ++i)
                    chr[i] = (char)m_Name[i];
                return new string(chr);
            }
            set
            {
                var str = value;
                if (str.Length > 255)
                    str = str.Substring(0, 255);
                m_NameLength = (byte)str.Length;
                m_Name = new byte[m_NameLength];
                for (int i = 0; i < m_NameLength; ++i)
                {
                    if (str[i] > 'ÿ')
                        m_Name[i] = (byte)'~';
                    else
                        m_Name[i] = (byte)str[i];
                }
            }
        }
        public List<int> Strucs
        {
            get
            {
                List<int> temp = new List<int>(m_StructureNum);
                for (int i = 0; i < m_StructureNum; ++i)
                    temp.Add(m_Structures[i]);
                return temp;
            }
            set
            {
                m_StructureNum = (byte)value.Count;
                m_Structures = new ushort[m_StructureNum];
                for (int i = 0; i < m_StructureNum; ++i)
                    m_Structures[i] = (ushort)value[i];
            }
        }
        public List<int> Monsters
        {
            get
            {
                List<int> temp = new List<int>(m_MonsterNum);
                for (int i = 0; i < m_MonsterNum; ++i)
                    temp.Add(m_Monsters[i]);
                return temp;
            }
            set
            {
                m_MonsterNum = (byte)value.Count;
                m_Monsters = new ushort[m_MonsterNum];
                for (int i = 0; i < m_StructureNum; ++i)
                    m_Monsters[i] = (ushort)value[i];
            }
        }
        public List<int> MonsterChances
        {
            get
            {
                List<int> temp = new List<int>(m_MonsterNum);
                for (int i = 0; i < m_MonsterNum; ++i)
                    temp.Add(m_MonsterChances[i]);
                return temp;
            }
            set
            {
                m_MonsterNum = (byte)value.Count;
                m_MonsterChances = new ushort[m_MonsterNum];
                for (int i = 0; i < m_StructureNum; ++i)
                    m_MonsterChances[i] = (ushort)value[i];
            }
        }

        public char[] MagicNum;
        public ushort ZoneID;
        public byte ZoneVersion;
        public byte m_NameLength;
        public byte m_StructureNum;
        public byte m_MonsterNum;

        private byte[] m_Name;
        private ushort[] m_Structures;
        private ushort[] m_Monsters;
        private ushort[] m_MonsterChances;

        public ZoneIE()
        {
            MagicNum = new char[] { 'Z', 'O', 'N', 'E' };
            ZoneID = ushort.MaxValue;
            ZoneVersion = 1;
            m_NameLength = 0;
            m_StructureNum = 0;
            m_MonsterNum = 0;
        }

        public void SaveZone(string path)
        {
            var fw = File.OpenWrite(path);

            if (!fw.CanWrite)
                return;

            var tempArray = new byte[STATIC_SIZE];
            int lastIdx = 0;
            for (int i = 0; i < MagicNum.Length; ++i)
                tempArray[lastIdx++] = (byte)MagicNum[i];

            var tempBytes = BitConverter.GetBytes(ZoneID);
            for (int i = 0; i < tempBytes.Length; ++i)
                tempArray[lastIdx++] = tempBytes[i];

            tempArray[lastIdx++] = ZoneVersion;
            tempArray[lastIdx++] = m_NameLength;
            tempArray[lastIdx++] = m_StructureNum;
            tempArray[lastIdx++] = m_MonsterNum;

            fw.Write(tempArray, 0, tempArray.Length);

            tempArray = new byte[m_NameLength * 1
                + m_StructureNum * 2 + m_MonsterNum * (4 + 2)];

            for (int i = 0; i < m_NameLength; ++i)
                tempArray[lastIdx++] = (byte)m_Name[i];

            for (int i = 0; i < m_StructureNum; ++i)
            {
                tempBytes = BitConverter.GetBytes(m_Structures[i]);
                for (int j = 0; j < tempBytes.Length; ++j)
                    tempArray[lastIdx++] = tempBytes[j];
            }

            for (int i = 0; i < m_MonsterNum; ++i)
            {
                tempBytes = BitConverter.GetBytes(m_Monsters[i]);
                for (int j = 0; j < tempBytes.Length; ++j)
                    tempArray[lastIdx++] = tempBytes[j];
            }

            for (int i = 0; i < m_MonsterNum; ++i)
            {
                tempBytes = BitConverter.GetBytes(m_MonsterChances[i]);
                for (int j = 0; j < tempBytes.Length; ++j)
                    tempArray[lastIdx++] = tempBytes[j];
            }
            fw.Write(tempArray, 0, tempArray.Length);
            fw.Close();
        }

        public static ZoneIE FromFile(string path)
        {
            ZoneIE zone = new ZoneIE();
            var fr = File.OpenRead(path);
            if (!fr.CanRead || fr.Length < STATIC_SIZE)
            {
                fr.Close();
                return null;
            }

            var tempArray = new byte[STATIC_SIZE];
            fr.Read(tempArray, 0, tempArray.Length);

            int lastIdx = 0;
            for (int i = 0; i < zone.MagicNum.Length; ++i)
            {
                if ((char)tempArray[lastIdx++] != zone.MagicNum[i])
                {
                    fr.Close();
                    return null; // Magic num error
                }
            }

            var id = BitConverter.ToUInt16(tempArray, lastIdx);
            if (id == ushort.MaxValue)
            {
                fr.Close();
                return null;
            }
            zone.ZoneID = id;
            lastIdx += 2;

            var version = tempArray[lastIdx++];
            if (version != zone.ZoneVersion)
            {
                fr.Close();
                return null;
            }

            zone.m_NameLength = tempArray[lastIdx++];
            zone.m_StructureNum = tempArray[lastIdx++];
            zone.m_MonsterNum = tempArray[lastIdx++];

            tempArray = new byte[zone.m_NameLength * 1
                + zone.m_StructureNum * 2 + zone.m_MonsterNum * (4 + 2)];

            zone.m_Name = new byte[zone.m_NameLength];
            zone.m_Structures = new ushort[zone.m_StructureNum];
            zone.m_Monsters = new ushort[zone.m_MonsterNum];
            zone.m_MonsterChances = new ushort[zone.m_MonsterNum];

            for (int i = 0; i < zone.m_NameLength; ++i)
            {
                zone.m_Name[i] = tempArray[lastIdx++];
            }
            for (int i = 0; i < zone.m_StructureNum; ++i)
            {
                zone.m_Structures[i] = BitConverter.ToUInt16(tempArray, lastIdx);
                lastIdx += 2;
            }
            for (int i = 0; i < zone.m_MonsterNum; ++i)
            {
                zone.m_Monsters[i] = BitConverter.ToUInt16(tempArray, lastIdx);
                lastIdx += 2;
            }
            for (int i = 0; i < zone.m_MonsterNum; ++i)
            {
                zone.m_MonsterChances[i] = BitConverter.ToUInt16(tempArray, lastIdx);
                lastIdx += 2;
            }
            fr.Close();

            return zone;
        }

        public override bool Equals(object obj)
        {
            var iE = obj as ZoneIE;
            return iE == this;
        }

        public override int GetHashCode()
        {
            var hashCode = -1009486901;
            hashCode = hashCode * -1521134295 + EqualityComparer<char[]>.Default.GetHashCode(MagicNum);
            hashCode = hashCode * -1521134295 + ZoneID.GetHashCode();
            hashCode = hashCode * -1521134295 + ZoneVersion.GetHashCode();
            hashCode = hashCode * -1521134295 + m_NameLength.GetHashCode();
            hashCode = hashCode * -1521134295 + m_StructureNum.GetHashCode();
            hashCode = hashCode * -1521134295 + m_MonsterNum.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(m_Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<ushort[]>.Default.GetHashCode(m_Structures);
            hashCode = hashCode * -1521134295 + EqualityComparer<ushort[]>.Default.GetHashCode(m_Monsters);
            hashCode = hashCode * -1521134295 + EqualityComparer<ushort[]>.Default.GetHashCode(m_MonsterChances);
            return hashCode;
        }

        public static bool operator ==(ZoneIE left, ZoneIE right)
        {
            if (left is null && !(right is null))
                return false;
            if (!(left is null) && right is null)
                return false;
            if (left is null && right is null)
                return true;

            if (left.ZoneID != right.ZoneID)
                return false;

            if (left.ZoneVersion != right.ZoneVersion)
                return false;

            if (left.m_NameLength != right.m_NameLength)
                return false;

            if (left.m_StructureNum != right.m_StructureNum)
                return false;

            if (left.m_MonsterNum != right.m_MonsterNum)
                return false;

            for (int i = 0; i < left.m_NameLength; ++i)
            {
                if (left.m_Name[i] != right.m_Name[i])
                    return false;
            }

            for (int i = 0; i < left.m_StructureNum; ++i)
            {
                if (left.m_Structures[i] != right.m_Structures[i])
                    return false;
            }

            for (int i = 0; i < left.m_MonsterNum; ++i)
            {
                if (left.m_Monsters[i] != right.m_Monsters[i])
                    return false;
            }

            for (int i = 0; i < left.m_MonsterNum; ++i)
            {
                if (left.m_MonsterChances[i] != right.m_MonsterChances[i])
                    return false;
            }

            return true;
        }
        public static bool operator !=(ZoneIE left, ZoneIE right)
        {
            return !(left == right);
        }
    }
}
