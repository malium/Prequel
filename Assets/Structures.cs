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
using UnityEngine;

namespace Assets
{
	public static class Structures
	{
		//public static int LastStructureID;
		//public static List<int> StructureAvailableIDs;
		//public static int LastZoneID;
		//public static List<int> ZoneAvailableIDs;

		public static List<IE.V4.StructureIE> Strucs;
		public static List<int> ModifiedStrucs;
		public static Dictionary<string, int> StrucDict;
		//public static List<IE.V2.ZoneIE> Zones;
		//public static List<int> ModifiedZones;
		//public static Dictionary<string, int> ZoneDict;

		public static int GetStrucVersion(string path)
		{
			const int staticSize = 4 + 1; //STRC + Version
			var fr = File.OpenRead(path);
			if (!fr.CanRead || fr.Length <= staticSize)
			{
				fr.Close();
				return -1;
			}

			var header = new byte[staticSize];
			fr.Read(header, 0, staticSize);
			fr.Close();
			return header[staticSize - 1];
		}

		static List<FileInfo> GetFiles(string path, byte[] header)
		{
			var filesPath = Application.streamingAssetsPath + path;
			var di = new DirectoryInfo(filesPath);
			if (!di.Exists)
			{
				di.Create();
				return new List<FileInfo>();
			}
			var allFiles = di.GetFiles();
			var correctFiles = new List<FileInfo>(allFiles.Length);
			for (int i = 0; i < allFiles.Length; ++i)
			{
				var fr = allFiles[i].OpenRead();
				var magic = new byte[4];
				fr.Read(magic, 0, magic.Length);
				bool mnOK = true;
				for (int j = 0; j < 4; ++j)
				{
					if (magic[j] != header[j])
					{
						mnOK = false;
						break;
					}
				}
				if (mnOK)
				{
					correctFiles.Add(allFiles[i]);
				}
				fr.Close();
			}
			return correctFiles;
		}

		//static int GetNextStructureID()
		//{
		//	int id;
		//	if (LastStructureID == 10000)
		//	{
		//		id = StructureAvailableIDs[0];
		//		StructureAvailableIDs.RemoveAt(0);
		//	}
		//	else
		//	{
		//		id = LastStructureID++;
		//	}
		//	return id;
		//}

		public static void RemoveStructure(int id)
		{
			if(Strucs.Count <= id)
			{
				Debug.LogWarning($"Trying to remove a non existant structure {id}.");
				return;
			}

			var struc = Strucs[id];
			if (struc == null)
			{
				Debug.LogWarning($"Trying to remove a null structure {id}.");
				return;
			}

			var name = Strucs[id].GetName();
			if (StrucDict.ContainsKey(name))
				StrucDict.Remove(name);

			Strucs[id] = null;

			if (ModifiedStrucs.Contains(id))
				ModifiedStrucs.Remove(id);
		}

		public static int AddStructure()
		{
			int id = Strucs.Count;

			//if(Strucs.Count <= id)
			//{
			//	Strucs.AddRange(Enumerable.Repeat<IE.V3.StructureIE>(null, 1 + id - Strucs.Count));
			//}
			var struc = new IE.V4.StructureIE()
			{
				StructureID = (ushort)id
			};

			Strucs.Add(struc);

			return id;
		}
		
		//static int GetNextZoneID()
		//{
		//    int id = -1;
		//    if (ZoneAvailableIDs.Count > 0)
		//    {
		//        id = ZoneAvailableIDs[0];
		//        ZoneAvailableIDs.RemoveAt(0);
		//    }
		//    else
		//    {
		//        id = LastZoneID++;
		//    }
		//    return id;
		//}

		//public static void RemoveZone(int id)
		//{
		//    if (Zones.Count <= id)
		//    {
		//        throw new Exception("Trying to remove a non existant Zone");
		//    }
		//    ZoneAvailableIDs.Add(id);
		//    Zones[id] = null;
		//}

		//public static int AddZone()
		//{
		//    int id = GetNextZoneID();

		//    if (Zones.Count <= id)
		//    {
		//        Zones.AddRange(Enumerable.Repeat<IE.V2.ZoneIE>(null, 1 + id - Zones.Count));
		//    }
		//    var zone = new IE.V2.ZoneIE
		//    {
		//        ZoneID = (ushort)id
		//    };

		//    Zones[id] = zone;

		//    return id;
		//}

		public static void LoadStrucs(bool reload)
		{
			if (reload)
				Strucs.Clear();
			var strucFiles = GetFiles("/Structures", new byte[4] { (byte)'S', (byte)'T', (byte)'R', (byte)'C' });
			var path = Application.streamingAssetsPath + "/Structures/";
			for (int i = 0; i < strucFiles.Count; ++i)
			{
				var ver = GetStrucVersion(strucFiles[i].FullName);
				IE.V4.StructureIE structure = null;
				switch(ver)
				{
					case 1:
						{
							var tStruc = IE.V1.StructureIE.FromFile(strucFiles[i].FullName);
							if(tStruc == null)
							{
								Debug.LogWarning("Couldn't load struct'" + strucFiles[i].FullName + "'.");
							}
							else
							{
								structure = tStruc.Upgrade().Upgrade().Upgrade();
								// Save upgraded
								strucFiles[i].Delete();
								structure.SaveStruc(path + structure.GetName() + ".STRC");
							}
						}
						break;
					case 2:
						{
							var tStruc = IE.V2.StructureIE.FromFile(strucFiles[i].FullName);
							if (tStruc == null)
							{
								Debug.LogWarning("Couldn't load struct'" + strucFiles[i].FullName + "'.");
							}
							else
							{
								structure = tStruc.Upgrade().Upgrade();
								// Save upgraded
								strucFiles[i].Delete();
								structure.SaveStruc(path + structure.GetName() + ".STRC");
							}
						}
						break;
					case 3:
						{
							var tStruc = IE.V3.StructureIE.FromFile(strucFiles[i].FullName);
							if (tStruc == null)
							{
								Debug.LogWarning("Couldn't load struct '" + strucFiles[i].FullName + "'.");
							}
							else
							{
								structure = tStruc.Upgrade();
								// Save upgraded
								strucFiles[i].Delete();
								structure.SaveStruc(path + structure.GetName() + ".STRC");
							}
						}
						break;
					case 4:
						{
							var tStruc = IE.V4.StructureIE.FromFile(strucFiles[i].FullName);
							if (tStruc == null)
							{
								Debug.LogWarning("Couldn't load struct '" + strucFiles[i].FullName + "'.");
							}
							else
							{
								structure = tStruc;
							}
						}
						break;
				}
				if (structure == null)
					continue;

				Strucs.Add(structure);
				structure.StructureID = (ushort)(Strucs.Count - 1);

				//if(Strucs.Count <= structure.StructureID)
				//{
				//	Strucs.AddRange(Enumerable.Repeat<IE.V3.StructureIE>(null, (structure.StructureID + 1) - Strucs.Count));
				//}
				//if(reload || Strucs[structure.StructureID] == null)
				//	Strucs[structure.StructureID] = structure;
				var strucName = structure.GetName();
				if (StrucDict.ContainsKey(strucName))
				{
					Debug.LogWarning($"Structure {structure.StructureID} has a name collision with {StrucDict[strucName]}");
				}
				else
				{
					StrucDict.Add(strucName, structure.StructureID);
				}
			}
			//StructureAvailableIDs.Capacity = StructureAvailableIDs.Capacity < Strucs.Count ? Strucs.Count : StructureAvailableIDs.Capacity;
			//for (int i = 0; i < Strucs.Count; ++i)
			//{
			//	if (Strucs[i] == null && !StructureAvailableIDs.Contains(i))
			//		StructureAvailableIDs.Add(i);
			//}
			//LastStructureID = Strucs.Count;
		}

		//public static void LoadZones(bool reload)
		//{
		//    var zoneFiles = GetFiles("/Zones", new byte[4] { (byte)'Z', (byte)'O', (byte)'N', (byte)'E' });

		//    for (int i = 0; i < zoneFiles.Count; ++i)
		//    {
		//        var zone = IE.V2.ZoneIE.FromFile(zoneFiles[i].FullName);
		//        if (zone == null)
		//            continue;
				
		//        if (Zones.Count <= zone.ZoneID)
		//        {
		//            Zones.AddRange(Enumerable.Repeat<IE.V2.ZoneIE>(null, 1 + (zone.ZoneID - Zones.Count)));
		//        }
		//        if (reload || Zones[zone.ZoneID] == null)
		//            Zones[zone.ZoneID] = zone;
		//        ZoneDict.Add(zone.Name, zone.ZoneID);
		//    }
		//    ZoneAvailableIDs.Capacity = ZoneAvailableIDs.Capacity < Zones.Count ? Zones.Count : ZoneAvailableIDs.Capacity;
		//    for (int i = 0; i < Zones.Count; ++i)
		//    {
		//        if (Zones[i] == null && !ZoneAvailableIDs.Contains(i))
		//            ZoneAvailableIDs.Add(i);
		//    }
		//    LastZoneID = Zones.Count;
		//}

		public static void Init()
		{
			//LastStructureID = 0;
			//StructureAvailableIDs = new List<int>();
			//LastZoneID = 0;
			//ZoneAvailableIDs = new List<int>();
			StrucDict = new Dictionary<string, int>();

			Strucs = new List<IE.V4.StructureIE>();
			ModifiedStrucs = new List<int>();
			//Zones = new List<IE.V2.ZoneIE>();
			//ModifiedZones = new List<int>();
			//ZoneDict = new Dictionary<string, int>();

			LoadStrucs(true);
			//LoadZones(true);
		}
		
		public static void SetStrucModified(int id, bool modified = true)
		{
			if (modified)
			{
				if (!IsStrucModified(id))
					ModifiedStrucs.Add(id);
			}
			else
			{
				if (IsStrucModified(id))
					ModifiedStrucs.Remove(id);
			}
		}

		//public static void SetZoneModified(int id)
		//{
		//    if (!IsZoneModified(id))
		//        ModifiedZones.Add(id);
		//}

		public static bool IsStrucModified(int id)
		{
			return ModifiedStrucs.Contains(id);
		}

		//public static bool IsZoneModified(int id)
		//{
		//    return ModifiedZones.Find(iid => iid == id) >= 0;
		//}

		public static void SaveStruc(int id)
		{
			if (Strucs.Count <= id)
				throw new Exception("Trying to save a structure with an invalid ID.");

			var struc = Strucs[id];
			if (struc == null)
				throw new Exception("Trying to save a null structure.");
			var strucName = struc.GetName();

			//var file = new FileInfo(Application.dataPath
			//        + $"/Structures/STRC." + string.Format("{0:0000}", struc.StructureID));
			var file = new FileInfo(Application.streamingAssetsPath
					+ "/Structures/" + strucName + ".STRC");
			if (file.Exists)
				file.Delete();
			file.Create().Close();
			struc.SaveStruc(file.FullName);
			SetStrucModified(id, false);
		}

		//public static void SaveZone(int id)
		//{
		//    if (Zones.Count <= id)
		//        throw new Exception("Trying to save a zone with an invalid ID.");

		//    var zone = Zones[id];
		//    if (zone == null)
		//        throw new Exception("Trying to save a null zone.");
		//    var file = new FileInfo(Application.dataPath
		//            + $"/Zones/ZONE." + string.Format("{0:0000}", zone.ZoneID));
		//    if (file.Exists)
		//        file.Delete();
		//    file.Create().Close();
		//    zone.SaveZone(file.FullName);
		//    var modifiedIdx = ModifiedZones.FindIndex(idx => idx == id);
		//    if (modifiedIdx >= 0)
		//        ModifiedZones.RemoveAt(modifiedIdx);
		//}

		public static void SaveStructs(bool modifiedOnly)
		{
			if(modifiedOnly)
			{
				for (int i = 0; i < ModifiedStrucs.Count; ++i)
				{
					var struc = Strucs[ModifiedStrucs[i]];
					if (struc == null)
						continue;
					var file = new FileInfo(Application.streamingAssetsPath
						+ $"/Structures/STRC." + string.Format("{0:0000}", struc.StructureID));
					if (file.Exists)
						file.Delete();
					file.Create();
					struc.SaveStruc(file.FullName);
				}
				ModifiedStrucs.Clear();
				return;
			}
			for(int i = 0; i < Strucs.Count; ++i)
			{
				var struc = Strucs[i];
				if (struc == null)
					continue;
				var file = new FileInfo(Application.streamingAssetsPath
						+ $"/Structures/STRC." + string.Format("{0:0000}", struc.StructureID));
				if (file.Exists)
					file.Delete();
				file.Create();
				struc.SaveStruc(file.FullName);
			}
			ModifiedStrucs.Clear();
		}

		//public static void SaveZones(bool modifiedOnly)
		//{
		//    if (modifiedOnly)
		//    {
		//        for (int i = 0; i < ModifiedZones.Count; ++i)
		//        {
		//            var zone = Zones[ModifiedZones[i]];
		//            if (zone == null)
		//                continue;
		//            var file = new FileInfo(Application.dataPath
		//                + $"/Zones/ZONE." + string.Format("{0:0000}", zone.ZoneID));
		//            if (file.Exists)
		//                file.Delete();
		//            file.Create();
		//            zone.SaveZone(file.FullName);
		//        }
		//        ModifiedZones.Clear();
		//        return;
		//    }
		//    for (int i = 0; i < Zones.Count; ++i)
		//    {
		//        var zone = Zones[i];
		//        if (zone == null)
		//            continue;
		//        var file = new FileInfo(Application.dataPath
		//                + $"/Zones/ZONE." + string.Format("{0:0000}", zone.ZoneID));
		//        if (file.Exists)
		//            file.Delete();
		//        file.Create();
		//        zone.SaveZone(file.FullName);
		//    }
		//    ModifiedZones.Clear();
		//}

		public static void Deinit()
		{
			SaveStructs(true);
			//SaveZones(true);
		}
	}
}
