/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
	
	
	//public struct VFXInfo
	//{
	//    public int VFXID;
	//    public int VFXTypeID;
	//    public int VFXVersion;
	//    public Def.VFXType Type;
	//    public int FramesPerSec;
	//    public List<Sprite> Frames;
	//}

	//public struct VFXTypeInfo
	//{
	//    public int VFXTypeID;

	//    public string name;
	//    public List<int> CastVFX;
	//    public List<int> TravelVFX;
	//    public List<int> OnHitVFX;
	//}

	[Serializable]
	public class VFXInfoDef
	{
		public int VFXID;
		public VFXFamily Family;
		public int VFXVersion;
		public Def.VFXType Type;
		public int FramesPerSec;
		public Sprite[] Frames;
	}

	[Serializable]
	public class VFXFamily
	{
		public string Name;
		[NonSerialized]
		public VFXInfoDef[] CastVFX;
		[NonSerialized]
		public VFXInfoDef[] TravelVFX;
		[NonSerialized]
		public VFXInfoDef[] OnHitVFX;

		public VFXInfoDef[] GetVFXInfo(Def.VFXType target)
		{
			switch (target)
			{
				case Def.VFXType.CAST:
					return CastVFX;
				case Def.VFXType.TRAVEL:
					return TravelVFX;
				case Def.VFXType.ONHIT:
					return OnHitVFX;
				default:
					return null;
			}
		}
	}

	public static class VFXs
	{
		//public static List<VFXInfo>[] VFXInfos;
		//public static List<VFXTypeInfo>[] VFXTypeInfos;
		//public static Dictionary<string, int>[] VFXDict;

		//public static void _Init()
		//{
		//    VFXInfos = new List<VFXInfo>[(int)Def.VFXTarget.COUNT];
		//    VFXTypeInfos = new List<VFXTypeInfo>[(int)Def.VFXTarget.COUNT];
		//    VFXDict = new Dictionary<string, int>[(int)Def.VFXTarget.COUNT];

		//    Sprite[][] vfxs = new Sprite[(int)Def.VFXTarget.COUNT][]
		//    {
		//        AssetContainer.Mgr.OddVFX,
		//        AssetContainer.Mgr.PropVFX,
		//        AssetContainer.Mgr.MonsterVFX,
		//        AssetContainer.Mgr.GeneralVFX,
		//    };

		//    for (int i = 0; i < (int)Def.VFXTarget.COUNT; ++i)
		//    {
		//        var vfxCount = vfxs[i].Length / 16;
		//        VFXInfos[i] = new List<VFXInfo>(vfxCount);
		//        VFXTypeInfos[i] = new List<VFXTypeInfo>(vfxCount);
		//        VFXDict[i] = new Dictionary<string, int>(vfxCount);
		//        int LastVFXID = 0;
		//        int LastVFXTypeID = 0;
		//        for(int j = 0; j < vfxs[i].Length; ++j)
		//        {
		//            var curVFX = vfxs[i][j];
		//            //VFX_HIT_BlinkingBubble_00_0
		//            var name = curVFX.name.Split('_');
		//            if (name[0].ToLower() != "vfx")
		//                throw new Exception($"Trying to import a VFX which is not a VFX, name:{curVFX.name}.");
		//            var vfxTypeStr = name[1].ToLower();
		//            Def.VFXType type = Def.VFXType.COUNT;
		//            if (vfxTypeStr == "hit")
		//                type = Def.VFXType.ONHIT;
		//            else if (vfxTypeStr == "trvl")
		//                type = Def.VFXType.TRAVEL;
		//            else if (vfxTypeStr == "cst")
		//                type = Def.VFXType.CAST;

		//            if (type == Def.VFXType.COUNT)
		//                throw new Exception($"Couldn't identify which type of VFX '{curVFX.name}' is.");

		//            var vfxName = name[2];

		//            bool parseOK = int.TryParse(name[3], out int vfxid);
		//            if (!parseOK)
		//                throw new Exception($"Couldn't parse the id of the VFX '{curVFX.name}'.");

		//            parseOK = int.TryParse(name[4], out int vfxFrame);
		//            if (!parseOK)
		//                throw new Exception($"Couldn't parse the frame number of the VFX '{curVFX.name}'.");

		//            VFXTypeInfo typeInfo;
		//            VFXInfo vfxInfo;
		//            vfxInfo.VFXID = -1;
		//            vfxInfo.Frames = null;
		//            if(!VFXDict[i].ContainsKey(vfxName))
		//            {
		//                typeInfo.CastVFX = new List<int>(1);
		//                typeInfo.OnHitVFX = new List<int>(1);
		//                typeInfo.TravelVFX = new List<int>(1);

		//                typeInfo.VFXTypeID = LastVFXTypeID++;
		//                typeInfo.name = vfxName;

		//                VFXDict[i].Add(vfxName, typeInfo.VFXTypeID);
		//                if(VFXTypeInfos[i].Count < (typeInfo.VFXTypeID + 1))
		//                {
		//                    VFXTypeInfos[i].AddRange(Enumerable.Repeat(new VFXTypeInfo(), (typeInfo.VFXTypeID + 1) - VFXTypeInfos[i].Count));
		//                }
		//                VFXTypeInfos[i][typeInfo.VFXTypeID] = typeInfo;
		//            }
		//            else
		//            {
		//                var typeID = VFXDict[i][vfxName];
		//                typeInfo = VFXTypeInfos[i][typeID];
		//                switch(type)
		//                {
		//                    case Def.VFXType.CAST:
		//                        if (typeInfo.CastVFX.Count >= (vfxid + 1))
		//                            vfxInfo = VFXInfos[i][typeInfo.CastVFX[vfxid]];
		//                        break;
		//                    case Def.VFXType.ONHIT:
		//                        if (typeInfo.OnHitVFX.Count >= (vfxid + 1))
		//                            vfxInfo = VFXInfos[i][typeInfo.OnHitVFX[vfxid]];
		//                        break;
		//                    case Def.VFXType.TRAVEL:
		//                        if (typeInfo.TravelVFX.Count >= (vfxid + 1))
		//                            vfxInfo = VFXInfos[i][typeInfo.TravelVFX[vfxid]];
		//                        break;
		//                }
		//            }
		//            // Was not created
		//            if(vfxInfo.VFXID < 0)
		//            {
		//                vfxInfo.VFXID = LastVFXID++;
		//                vfxInfo.Frames = new List<Sprite>();
		//                if(VFXInfos[i].Count <= vfxInfo.VFXID)
		//                {
		//                    VFXInfos[i].AddRange(Enumerable.Repeat(new VFXInfo(), (vfxInfo.VFXID + 1) - VFXInfos[i].Count));
		//                }
						
		//                switch (type)
		//                {
		//                    case Def.VFXType.CAST:
		//                        if (typeInfo.CastVFX.Count <= vfxid)
		//                        {
		//                            typeInfo.CastVFX.AddRange(Enumerable.Repeat(-1, (vfxid + 1) - typeInfo.CastVFX.Count));
		//                        }
		//                        typeInfo.CastVFX[vfxid] = vfxInfo.VFXID;
		//                        break;
		//                    case Def.VFXType.ONHIT:
		//                        if (typeInfo.OnHitVFX.Count <= vfxid)
		//                        {
		//                            typeInfo.OnHitVFX.AddRange(Enumerable.Repeat(-1, (vfxid + 1) - typeInfo.OnHitVFX.Count));
		//                        }
		//                        typeInfo.OnHitVFX[vfxid] = vfxInfo.VFXID;
		//                        break;
		//                    case Def.VFXType.TRAVEL:
		//                        if (typeInfo.TravelVFX.Count <= vfxid)
		//                        {
		//                            typeInfo.TravelVFX.AddRange(Enumerable.Repeat(-1, (vfxid + 1) - typeInfo.TravelVFX.Count));
		//                        }
		//                        typeInfo.TravelVFX[vfxid] = vfxInfo.VFXID;
		//                        break;
		//                }
		//            }
		//            vfxInfo.VFXVersion = vfxid;
		//            vfxInfo.VFXTypeID = typeInfo.VFXTypeID;
		//            vfxInfo.Type = type;
		//            // ODD VFX 24FPS others 12FPS
		//            vfxInfo.FramesPerSec = i == 0 ? 24 : 12;
		//            if (vfxInfo.Frames.Count <= vfxFrame)
		//                vfxInfo.Frames.AddRange(Enumerable.Repeat<Sprite>(null, (vfxFrame + 1) - vfxInfo.Frames.Count));
		//            vfxInfo.Frames[vfxFrame] = curVFX;
		//            VFXInfos[i][vfxInfo.VFXID] = vfxInfo;
		//            VFXTypeInfos[i][vfxInfo.VFXTypeID] = typeInfo;
		//        }
		//    }
		//}

		//public static void Deinit()
		//{

		//}
		
		public static List<VFXFamily>[] VFXFamilies;
		public static Dictionary<string, int>[] FamilyDict;

		public static VFXFamily GetVFXFamily(Def.VFXTarget vfxTarget, string familyName)
		{
			if (vfxTarget == Def.VFXTarget.COUNT)
				throw new Exception("Trying to obtain a VFXFamily with target COUNT");

			if (FamilyDict == null || VFXFamilies == null)
				throw new Exception("VFXs is not ready, trying to get a VFXFamily before loading?");
			int target = (int)vfxTarget;
			if (FamilyDict[target].ContainsKey(familyName))
			{
				return VFXFamilies[target][FamilyDict[target][familyName]];
			}
			if (VFXFamilies[target].Count == 0)
				throw new Exception("There are no VFXs families, something went really wrong.");

			return VFXFamilies[target][0];
		}
		public static void Prepare()
		{
			VFXFamilies = new List<VFXFamily>[Def.VFXTargetCount];
			FamilyDict = new Dictionary<string, int>[Def.VFXTargetCount];

			Sprite[] setFrameCount(int frameNum, Sprite[] list)
			{
				Sprite[] result = new Sprite[frameNum];

				if (list != null)
				{
					list.CopyTo(result, 0);
				}

				return result;
			}

			VFXInfoDef[] setVFXCount(int versionCount, VFXInfoDef[] list)
			{
				VFXInfoDef[] result = new VFXInfoDef[versionCount];

				if (list != null)
				{
					list.CopyTo(result, 0);
				}

				return result;
			}

			void LoadVFXTarget(Sprite[] vfxs, Def.VFXTarget target)
			{
				var families = new List<VFXFamily>(vfxs.Length / 10);
				var dict = new Dictionary<string, int>(families.Capacity);

				for (int j = 0; j < vfxs.Length; ++j)
				{
					var curVFX = vfxs[j];
					//VFX_HIT_BlinkingBubble_00_0
					var name = curVFX.name.Split('_');
					if (name.Length == 4)
						continue; // not the wanted sprite
					if (name.Length != 5)
						throw new Exception("The current vfx '" + curVFX.name + "' targetting " + target.ToString() + ", has an invalid name.");
					if (name[0].ToLower() != "vfx")
						throw new Exception("Trying to import a VFX which is not a VFX, invalid name format '" + curVFX.name + "'.");
					var vfxTypeStr = name[1].ToLower();
					Def.VFXType vfxType = Def.VFXType.COUNT;
					switch (vfxTypeStr)
					{
						case "hit":
							vfxType = Def.VFXType.ONHIT;
							break;
						case "trvl":
							vfxType = Def.VFXType.TRAVEL;
							break;
						case "cst":
							vfxType = Def.VFXType.CAST;
							break;
						default:
							throw new Exception("The current vfx '" + curVFX.name + "' targetting " + "target.ToString() " + ", has an invalid type '" + name[1] + "'.");
					}
					var vfxName = name[2];

					bool parseOK = int.TryParse(name[3], out int vfxVersion);
					if (!parseOK)
						throw new Exception("Couldn't parse the VFXVersion of the VFX '" + curVFX.name + "'.");

					parseOK = int.TryParse(name[4], out int vfxFrame);
					if (!parseOK)
						throw new Exception("Couldn't parse the frame number of the VFX '" + curVFX.name + "'.");

					VFXFamily family = null;
					if (!dict.ContainsKey(vfxName))
					{
						family = new VFXFamily
						{
							Name = vfxName,
							CastVFX = new VFXInfoDef[0],
							OnHitVFX = new VFXInfoDef[0],
							TravelVFX = new VFXInfoDef[0]
						};
						dict.Add(vfxName, families.Count);
						families.Add(family);
					}
					else
					{
						family = families[dict[vfxName]];
					}

					VFXInfoDef info = null;
					switch (vfxType)
					{
						case Def.VFXType.CAST:
							if (family.CastVFX.Length > vfxVersion)
							{
								info = family.CastVFX[vfxVersion];
							}
							break;
						case Def.VFXType.TRAVEL:
							if (family.TravelVFX.Length > vfxVersion)
							{
								info = family.TravelVFX[vfxVersion];
							}
							break;
						case Def.VFXType.ONHIT:
							if (family.OnHitVFX.Length > vfxVersion)
							{
								info = family.OnHitVFX[vfxVersion];
							}
							break;
					}
					if (info == null)
					{
						info = new VFXInfoDef
						{
							Family = family,
							Frames = new Sprite[0],
							FramesPerSec = target == Def.VFXTarget.ODD ? 24 : 12,
							Type = vfxType,
							VFXID = dict[family.Name],
							VFXVersion = vfxVersion
						};
						switch (vfxType)
						{
							case Def.VFXType.CAST:
								family.CastVFX = setVFXCount(vfxVersion + 1, family.CastVFX);
								family.CastVFX[vfxVersion] = info;
								break;
							case Def.VFXType.TRAVEL:
								family.TravelVFX = setVFXCount(vfxVersion + 1, family.TravelVFX);
								family.TravelVFX[vfxVersion] = info;
								break;
							case Def.VFXType.ONHIT:
								family.OnHitVFX = setVFXCount(vfxVersion + 1, family.OnHitVFX);
								family.OnHitVFX[vfxVersion] = info;
								break;
						}
					}
					info.Frames = setFrameCount(vfxFrame + 1, info.Frames);
					info.Frames[vfxFrame] = curVFX;
				}

				VFXFamilies[(int)target] = families;
				FamilyDict[(int)target] = dict;
			}

			LoadVFXTarget(AssetLoader.VFXSprites[0], Def.VFXTarget.ODD);
			LoadVFXTarget(AssetLoader.VFXSprites[1], Def.VFXTarget.PROP);
			LoadVFXTarget(AssetLoader.VFXSprites[2], Def.VFXTarget.MONSTER);
			LoadVFXTarget(AssetLoader.VFXSprites[3], Def.VFXTarget.GENERAL);
		}
		public static void Init()
		{
			string[] path = new string[]
			{
				"Odd",
				"Prop",
				"Monster",
				"General"
			};
			VFXFamilies = new List<VFXFamily>[Def.VFXTargetCount];
			FamilyDict = new Dictionary<string, int>[Def.VFXTargetCount];

			Sprite[] setFrameCount(int frameNum, Sprite[] list)
			{
				Sprite[] result = new Sprite[frameNum];

				if (list != null)
				{
					list.CopyTo(result, 0);
				}

				return result;
			}

			VFXInfoDef[] setVFXCount(int versionCount, VFXInfoDef[] list)
			{
				VFXInfoDef[] result = new VFXInfoDef[versionCount];

				if (list != null)
				{
					list.CopyTo(result, 0);
				}

				return result;
			}



			//Sprite[] addFrame(Sprite frame, int frameNum, Sprite[] list)
			//{
			//    Sprite[] result = null;
			//    if (list != null)
			//    {
			//        int nLength = list.Length;
			//        if (list.Length <= frameNum)
			//        {
			//            nLength = frameNum + 1;
			//        }
			//        result = new Sprite[nLength];
			//        list.CopyTo(result, 0);
			//    }
			//    else
			//    {
			//        result = new Sprite[frameNum + 1];
			//    }
			//    result[frameNum] = frame;

			//    return result;
			//}

			//VFXInfoDef[] addVFX(VFXInfoDef vfx, int version, VFXInfoDef[] list)
			//{
			//    VFXInfoDef[] result = null;
			//    if(list != null)
			//    {
			//        int nLength = list.Length;
			//        if(list.Length <= version)
			//        {
			//            nLength = version + 1;
			//        }
			//        result = new VFXInfoDef[nLength];
			//        list.CopyTo(result, 0);
			//    }
			//    else
			//    {
			//        result = new VFXInfoDef[version + 1];
			//    }
			//    result[version] = vfx;

			//    return result;
			//}

			for (int i = 0; i < (int)Def.VFXTarget.COUNT; ++i)
			{
				var vfxs = Resources.LoadAll<Sprite>("VFX/" + path[i]);

				var families = new List<VFXFamily>(vfxs.Length / 10);
				var dict = new Dictionary<string, int>(families.Capacity);

				for(int j = 0; j < vfxs.Length; ++j)
				{
					var curVFX = vfxs[j];
					//VFX_HIT_BlinkingBubble_00_0
					var name = curVFX.name.Split('_');
					if (name.Length == 4)
						continue; // not the wanted sprite
					if (name.Length != 5)
						throw new Exception($"The current vfx '{curVFX.name}' targetting {((Def.VFXTarget)i).ToString()}, has an invalid name.");
					if (name[0].ToLower() != "vfx")
						throw new Exception($"Trying to import a VFX which is not a VFX, invalid name format '{curVFX.name}'.");
					var vfxTypeStr = name[1].ToLower();
					Def.VFXType vfxType = Def.VFXType.COUNT;
					switch(vfxTypeStr)
					{
						case "hit":
							vfxType = Def.VFXType.ONHIT;
							break;
						case "trvl":
							vfxType = Def.VFXType.TRAVEL;
							break;
						case "cst":
							vfxType = Def.VFXType.CAST;
							break;
						default:
							throw new Exception($"The current vfx '{curVFX.name}' targetting {((Def.VFXTarget)i).ToString()}, has an invalid type '{name[1]}'.");
					}
					var vfxName = name[2];

					bool parseOK = int.TryParse(name[3], out int vfxVersion);
					if (!parseOK)
						throw new Exception($"Couldn't parse the VFXVersion of the VFX '{curVFX.name}'.");

					parseOK = int.TryParse(name[4], out int vfxFrame);
					if (!parseOK)
						throw new Exception($"Couldn't parse the frame number of the VFX '{curVFX.name}'.");

					VFXFamily family = null;
					if(!dict.ContainsKey(vfxName))
					{
						family = new VFXFamily
						{
							Name = vfxName,
							CastVFX = new VFXInfoDef[0],
							OnHitVFX = new VFXInfoDef[0],
							TravelVFX = new VFXInfoDef[0]
						};
						dict.Add(vfxName, families.Count);
						families.Add(family);
					}
					else
					{
						family = families[dict[vfxName]];
					}

					VFXInfoDef info = null;
					switch (vfxType)
					{
						case Def.VFXType.CAST:
							if(family.CastVFX.Length > vfxVersion)
							{
								info = family.CastVFX[vfxVersion];
							}
							break;
						case Def.VFXType.TRAVEL:
							if (family.TravelVFX.Length > vfxVersion)
							{
								info = family.TravelVFX[vfxVersion];
							}
							break;
						case Def.VFXType.ONHIT:
							if (family.OnHitVFX.Length > vfxVersion)
							{
								info = family.OnHitVFX[vfxVersion];
							}
							break;
					}
					if(info == null)
					{
						info = new VFXInfoDef
						{
							Family = family,
							Frames = new Sprite[0],
							FramesPerSec = i == (int)Def.VFXTarget.ODD ? 24 : 12,
							Type = vfxType,
							VFXID = dict[family.Name],
							VFXVersion = vfxVersion
						};
						switch (vfxType)
						{
							case Def.VFXType.CAST:
								family.CastVFX = setVFXCount(vfxVersion + 1, family.CastVFX);
								family.CastVFX[vfxVersion] = info;
								break;
							case Def.VFXType.TRAVEL:
								family.TravelVFX = setVFXCount(vfxVersion + 1, family.TravelVFX);
								family.TravelVFX[vfxVersion] = info;
								break;
							case Def.VFXType.ONHIT:
								family.OnHitVFX = setVFXCount(vfxVersion + 1, family.OnHitVFX);
								family.OnHitVFX[vfxVersion] = info;
								break;
						}
					}
					info.Frames = setFrameCount(vfxFrame + 1, info.Frames);
					info.Frames[vfxFrame] = curVFX;
				}

				VFXFamilies[i] = families;
				FamilyDict[i] = dict;
			}
		}

		public static void Deinit()
		{

		}
	}
}
