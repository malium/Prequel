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
	public class SpawnerInfo : IXmlSerializable
	{
		string m_Name;
		uint m_DefaultMaxRange;
		ushort m_LastWaveChance;
		List<SpawnInfo> m_Spawns;
		List<SpawnInfo> m_LastWaveSpawns;

		public SpawnerInfo()
		{
			m_Name = "";
			m_Spawns = new List<SpawnInfo>();
			m_LastWaveChance = Def.DefaultLastWaveChance;
			m_LastWaveSpawns = new List<SpawnInfo>();
		}
		public SpawnerInfo GetACopy()
		{
			var spawner = new SpawnerInfo()
			{
				m_Name = m_Name,
				m_Spawns = new List<SpawnInfo>(m_Spawns.Count),
				m_DefaultMaxRange = m_DefaultMaxRange,
				m_LastWaveChance = m_LastWaveChance,
				m_LastWaveSpawns = new List<SpawnInfo>(m_LastWaveSpawns.Count)
			};
			for (int i = 0; i < m_Spawns.Count; ++i)
				spawner.m_Spawns.Add(m_Spawns[i].GetACopy());
			for (int i = 0; i < m_LastWaveSpawns.Count; ++i)
				spawner.m_LastWaveSpawns.Add(m_LastWaveSpawns[i].GetACopy());
			return spawner;
		}
		public void SetName(string name) => m_Name = name;
		public string GetName() => m_Name;
		public List<SpawnInfo> GetSpawns() => m_Spawns;
		public List<SpawnInfo> GetLastWaveSpawns() => m_LastWaveSpawns;
		public ushort GetLasWaveChance() => m_LastWaveChance;
		public void SetLastWaveChance(ushort chance) => m_LastWaveChance = chance;
		public uint GetDefaultMaxRange() => m_DefaultMaxRange;
		public void SetDefaultMaxRange(uint range) => m_DefaultMaxRange = range;
		public override bool Equals(object obj)
		{
			return obj is SpawnerInfo info &&
				   m_Name == info.m_Name &&
				   EqualityComparer<List<SpawnInfo>>.Default.Equals(m_Spawns, info.m_Spawns) &&
				   m_DefaultMaxRange == info.m_DefaultMaxRange &&
				   m_LastWaveChance == info.m_LastWaveChance &&
				   EqualityComparer<List<SpawnInfo>>.Default.Equals(m_LastWaveSpawns, info.m_LastWaveSpawns);
		}
		public override int GetHashCode()
		{
			int hashCode = 1214030388;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(m_Name);
			hashCode = hashCode * -1521134295 + EqualityComparer<List<SpawnInfo>>.Default.GetHashCode(m_Spawns);
			hashCode = hashCode * -1521134295 + m_DefaultMaxRange.GetHashCode();
			hashCode = hashCode * -1521134295 + m_LastWaveChance.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<List<SpawnInfo>>.Default.GetHashCode(m_LastWaveSpawns);
			return hashCode;
		}
		public XmlSchema GetSchema() => null;
		public void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement("SpawnerInfo");

			reader.ReadStartElement("m_Name");
			m_Name = reader.ReadContentAsString();
			reader.ReadEndElement();

			reader.ReadStartElement("m_DefaultMaxRange");
			m_DefaultMaxRange = uint.Parse(reader.ReadContentAsString());
			reader.ReadEndElement();

			reader.ReadStartElement("m_LastWaveChance");
			m_LastWaveChance = ushort.Parse(reader.ReadContentAsString());
			reader.ReadEndElement();

			reader.ReadStartElement("m_Spawns");
			reader.ReadStartElement("Count");
			var count = int.Parse(reader.ReadContentAsString());
			reader.ReadEndElement();
			m_Spawns = new List<SpawnInfo>(count);

			for(int i = 0; i < count; ++i)
			{
				var spawn = new SpawnInfo();
				reader.ReadStartElement("item_" + i.ToString());
				spawn.ReadXml(reader);
				reader.ReadEndElement();
				m_Spawns.Add(spawn);
			}
			reader.ReadEndElement();

			reader.ReadStartElement("m_LastWaveSpawns");
			reader.ReadStartElement("Count");
			count = int.Parse(reader.ReadContentAsString());
			reader.ReadEndElement();
			m_LastWaveSpawns = new List<SpawnInfo>(count);

			for (int i = 0; i < count; ++i)
			{
				var spawn = new SpawnInfo();
				reader.ReadStartElement("item_" + i.ToString());
				spawn.ReadXml(reader);
				reader.ReadEndElement();
				m_LastWaveSpawns.Add(spawn);
			}
			reader.ReadEndElement();

			reader.ReadEndElement();
		}
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("m_Name");
			writer.WriteString(m_Name);
			writer.WriteEndElement();
			
			writer.WriteStartElement("m_DefaultMaxRange");
			writer.WriteString(m_DefaultMaxRange.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement("m_LastWaveChance");
			writer.WriteString(m_LastWaveChance.ToString());
			writer.WriteEndElement();

			writer.WriteStartElement("m_Spawns");
			writer.WriteStartElement("Count");
			writer.WriteString(m_Spawns.Count.ToString());
			writer.WriteEndElement();

			for(int i = 0; i < m_Spawns.Count; ++i)
			{
				writer.WriteStartElement("item_" + i.ToString());
				m_Spawns[i].WriteXml(writer);
				writer.WriteEndElement();
			}

			writer.WriteEndElement();

			writer.WriteStartElement("m_LastWaveSpawns");
			writer.WriteStartElement("Count");
			writer.WriteString(m_LastWaveSpawns.Count.ToString());
			writer.WriteEndElement();

			for (int i = 0; i < m_LastWaveSpawns.Count; ++i)
			{
				writer.WriteStartElement("item_" + i.ToString());
				m_LastWaveSpawns[i].WriteXml(writer);
				writer.WriteEndElement();
			}

			writer.WriteEndElement();
		}
		public static bool operator ==(SpawnerInfo left, SpawnerInfo right)
		{
			return EqualityComparer<SpawnerInfo>.Default.Equals(left, right);
		}
		public static bool operator !=(SpawnerInfo left, SpawnerInfo right)
		{
			return !(left == right);
		}
	}
}
