/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

namespace Assets
{
	public class SpawnInfo
	{
		string m_MonsterFamily;
		ushort m_TotalProbability;
		uint m_PartialProbability;
		byte m_MinAmount;
		byte m_MaxAmount;
		ushort m_MinRange;
		ushort m_MaxRange;

		public SpawnInfo()
		{
			m_MonsterFamily = "";
			SetTotalProbability(0);
			SetMinAmount(0);
			SetMaxAmount(0);
			SetMinRange(0);
			SetMaxRange(0);
		}
		public SpawnInfo GetACopy()
		{
			return new SpawnInfo()
			{
				m_MonsterFamily = m_MonsterFamily,
				m_TotalProbability = m_TotalProbability,
				m_PartialProbability = m_PartialProbability,
				m_MinAmount = m_MinAmount,
				m_MaxAmount = m_MaxAmount,
				m_MinRange = m_MinRange,
				m_MaxRange = m_MaxRange
			};
		}
		public void SetMonsterFamily(string family) => m_MonsterFamily = family;
		public void SetPartialProbability(uint prob)
		{
			m_TotalProbability = ushort.MaxValue;
			m_PartialProbability = prob;
		}
		public void SetTotalProbability(ushort prob)
		{
			m_PartialProbability = uint.MaxValue;
			m_TotalProbability = prob;
		}
		public void SetMinAmount(int amount) => m_MinAmount = (byte)Mathf.Clamp(amount, byte.MinValue, byte.MaxValue);
		public void SetMaxAmount(int amount) => m_MaxAmount = (byte)Mathf.Clamp(amount, byte.MinValue, byte.MaxValue);
		public void SetMinRange(int range) => m_MinRange = (ushort)Mathf.Clamp(range, ushort.MinValue, ushort.MaxValue);
		public void SetMaxRange(int range) => m_MaxRange = (ushort)Mathf.Clamp(range, ushort.MinValue, ushort.MaxValue);
		public string GetMonsterFamily() => m_MonsterFamily;
		public int GetMinAmount() => m_MinAmount;
		public int GetMaxAmount() => m_MaxAmount;
		public int GetMinRange() => m_MinRange;
		public int GetMaxRange() => m_MaxRange;
		public uint GetPartialProbability() => m_PartialProbability;
		public ushort GetTotalProbability() => m_TotalProbability;
		public bool IsPartialProbability() => m_TotalProbability == ushort.MaxValue;
		public bool IsTotalProbability() => m_PartialProbability == uint.MaxValue;
		public void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement("m_MonsterFamily");
			m_MonsterFamily = reader.ReadContentAsString();
			reader.ReadEndElement();

			reader.ReadStartElement("m_TotalProbability");
			m_TotalProbability = ushort.Parse(reader.ReadContentAsString());
			reader.ReadEndElement();

			reader.ReadStartElement("m_PartialProbability");
			m_PartialProbability = uint.Parse(reader.ReadContentAsString());
			reader.ReadEndElement();

			reader.ReadStartElement("m_MinAmount");
			m_MinAmount = byte.Parse(reader.ReadContentAsString());
			reader.ReadEndElement();

			reader.ReadStartElement("m_MaxAmount");
			m_MaxAmount = byte.Parse(reader.ReadContentAsString());
			reader.ReadEndElement();

			reader.ReadStartElement("m_MinRange");
			m_MinRange = ushort.Parse(reader.ReadContentAsString());
			reader.ReadEndElement();

			reader.ReadStartElement("m_MaxRange");
			m_MaxRange = ushort.Parse(reader.ReadContentAsString());
			reader.ReadEndElement();
		}
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("m_MonsterFamily");
			writer.WriteString(m_MonsterFamily);
			writer.WriteEndElement();

			writer.WriteStartElement("m_TotalProbability");
			writer.WriteString(m_TotalProbability.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement("m_PartialProbability");
			writer.WriteString(m_PartialProbability.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement("m_MinAmount");
			writer.WriteString(m_MinAmount.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement("m_MaxAmount");
			writer.WriteString(m_MaxAmount.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement("m_MinRange");
			writer.WriteString(m_MinRange.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement("m_MaxRange");
			writer.WriteString(m_MaxRange.ToString());
			writer.WriteEndElement();
		}
		public override bool Equals(object obj)
		{
			return obj is SpawnInfo iE &&
				   m_MonsterFamily == iE.m_MonsterFamily &&
				   m_TotalProbability == iE.m_TotalProbability &&
				   m_PartialProbability == iE.m_PartialProbability &&
				   m_MinAmount == iE.m_MinAmount &&
				   m_MaxAmount == iE.m_MaxAmount &&
				   m_MinRange == iE.m_MinRange &&
				   m_MaxRange == iE.m_MaxRange;
		}
		public override int GetHashCode()
		{
			int hashCode = 1163053072;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(m_MonsterFamily);
			hashCode = hashCode * -1521134295 + m_TotalProbability.GetHashCode();
			hashCode = hashCode * -1521134295 + m_PartialProbability.GetHashCode();
			hashCode = hashCode * -1521134295 + m_MinAmount.GetHashCode();
			hashCode = hashCode * -1521134295 + m_MaxAmount.GetHashCode();
			hashCode = hashCode * -1521134295 + m_MinRange.GetHashCode();
			hashCode = hashCode * -1521134295 + m_MaxRange.GetHashCode();
			return hashCode;
		}
		public static bool operator ==(SpawnInfo left, SpawnInfo right)
		{
			return EqualityComparer<SpawnInfo>.Default.Equals(left, right);
		}
		public static bool operator !=(SpawnInfo left, SpawnInfo right)
		{
			return !(left == right);
		}
	}
}
