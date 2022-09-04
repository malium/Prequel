/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace Assets
{
	public static class SpawnerInfoLoader
	{
		public static readonly string SpawnerInfoPath = Application.streamingAssetsPath + "/SpawnerInfo";

		public static List<SpawnerInfo> SpawnerInfos;
		public static Dictionary<string, int> Dict;

		static void Serial_UnknownNode(object sender, XmlNodeEventArgs e)
		{
			Debug.LogWarning("XMLSerializer unknown node." + e.Name);
		}
		static void Serial_UnknownAttribute(object sender, XmlAttributeEventArgs e)
		{
			Debug.LogWarning("XMLSerializer unknown attribute.");
		}
		public static void Prepare()
		{
			var di = new DirectoryInfo(SpawnerInfoPath);
			if (!di.Exists)
				di.Create();

			var serializer = new XmlSerializer(typeof(SpawnerInfo));
			serializer.UnknownNode += Serial_UnknownNode;
			serializer.UnknownAttribute += Serial_UnknownAttribute;

			var files = di.GetFiles("*.xml");
			SpawnerInfos = new List<SpawnerInfo>(files.Length);
			Dict = new Dictionary<string, int>(files.Length);
			for (int i = 0; i < files.Length; ++i)
			{
				var fi = files[i];
				var fs = fi.OpenRead();
				var spawnerInfo = (SpawnerInfo)serializer.Deserialize(fs);

				Dict.Add(spawnerInfo.GetName(), SpawnerInfos.Count);
				SpawnerInfos.Add(spawnerInfo);
				fs.Close();
			}
		}
		public static void SaveSpawnerInfos()
		{
			var serializer = new XmlSerializer(typeof(SpawnerInfo));
			serializer.UnknownNode += Serial_UnknownNode;
			serializer.UnknownAttribute += Serial_UnknownAttribute;

			for (int i = 0; i < SpawnerInfos.Count; ++i)
			{
				var si = SpawnerInfos[i];
				if (si == null)
					continue;
				var name = si.GetName();
				var fi = new FileInfo(SpawnerInfoPath + '/' + name + ".xml");
				if (fi.Exists)
					fi.Delete();
				var fs = fi.Create();
				serializer.Serialize(fs, si);
				fs.Close();
			}
		}
		public static void RemoveSpawnerInfo(string spawnerName)
		{
			if(Dict.TryGetValue(spawnerName, out int spawnIdx))
			{
				Dict.Remove(spawnerName);
				SpawnerInfos.RemoveAt(spawnIdx);
				var fi = new FileInfo(SpawnerInfoPath + '/' + spawnerName + ".xml");
				if (fi.Exists)
					fi.Delete();
			}
		}
	}
}