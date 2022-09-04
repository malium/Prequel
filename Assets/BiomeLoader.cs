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
	public static class BiomeLoader
	{
		public static List<IE.V4.BiomeIE> Biomes;
		public static List<int> ModifiedBiomes;
		public static Dictionary<string, int> Dict;
		static readonly char[] BiomeHeader = new char[] { 'B', 'I', 'O', 'M', 'E' };

		public static int GetBiomeVersion(string path)
		{
			const int staticSize = 5 + 1; //BIOME + Version
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
		static List<FileInfo> GetFiles(string path)
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
					if (magic[j] != (byte)BiomeHeader[j])
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
		public static bool IsBiomeModified(int id)
		{
			return ModifiedBiomes.Contains(id);
		}
		public static void SetBiomeModified(int id, bool modified = true)
		{
			if (modified)
			{
				if (!IsBiomeModified(id))
					ModifiedBiomes.Add(id);
			}
			else
			{
				if (IsBiomeModified(id))
					ModifiedBiomes.Remove(id);
			}
		}
		public static int AddBiome()
		{
			int id = Biomes.Count;
			var biome = new IE.V4.BiomeIE()
			{
				BiomeID = (ushort)id
			};
			Biomes.Add(biome);

			return id;
		}
		public static void LoadBiomes(bool reload)
		{
			if (reload)
				Biomes.Clear();
			var biomeFiles = GetFiles("/Biomes");
			//var path = Application.streamingAssetsPath + "/Biomes/";
			for (int i = 0; i < biomeFiles.Count; ++i)
			{
				var ver = GetBiomeVersion(biomeFiles[i].FullName);
				IE.V4.BiomeIE biome = null;
				switch(ver)
				{
					case 4:
						{
							var tBiome = IE.V4.BiomeIE.FromFile(biomeFiles[i].FullName);
							if(tBiome == null)
							{
								Debug.LogWarning("Couldn't load biome '" + biomeFiles[i].FullName + "'.");
							}
							else
							{
								biome = tBiome;
							}
						}
						break;
				}
				if (biome == null)
					continue;

				Biomes.Add(biome);
				biome.BiomeID = (ushort)(Biomes.Count - 1);

				var biomeName = biome.GetName();
				if(Dict.ContainsKey(biomeName))
				{
					Debug.LogWarning($"Biome {biome.BiomeID} has a name collision with {Dict[biomeName]}");
				}
				else
				{
					Dict.Add(biomeName, biome.BiomeID);
				}
			}
		}
		public static void SaveBiome(int id)
		{
			if (Biomes.Count <= id)
				throw new Exception("Trying to save a biome with an invalid ID.");

			var biome = Biomes[id];
			if (biome == null)
				throw new Exception("Trying to save a null biome.");
			var strucName = biome.GetName();

			var file = new FileInfo(Application.streamingAssetsPath
					+ "/Biomes/" + strucName + ".BIOME");

			if (file.Exists)
				file.Delete();
			file.Create().Close();
			biome.SaveBiome(file.FullName);
			SetBiomeModified(id, false);
		}
		public static void Prepare()
		{
			Dict = new Dictionary<string, int>();
			ModifiedBiomes = new List<int>();
			Biomes = new List<IE.V4.BiomeIE>();

			LoadBiomes(true);
		}
	}
}
