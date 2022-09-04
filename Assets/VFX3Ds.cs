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
    public struct VFX3DInfo
    {
        public int VFXID;
        public int VFXTypeID;
        public int VFXVersion;
        public Def.VFXType Type;
        public int FramesPerSec;
        public List<Texture2D> Frames;
    }

    [Serializable]
    public class VFX3DInfoDef
    {
        public int VFXID;
        public VFX3DFamily Family;
        public int VFXVersion;
        public Def.VFXType Type;
        public int FramesPerSec;
        public Texture2D[] Frames;
        public Texture2DArray TAFrames;
    }

    [Serializable]
    public class VFX3DFamily
    {
        public string Name;
        [NonSerialized]
        public VFX3DInfoDef[] CastVFX;
        [NonSerialized]
        public VFX3DInfoDef[] TravelVFX;
        [NonSerialized]
        public VFX3DInfoDef[] OnHitVFX;
    }

    public static class VFX3Ds
    {
        //public static List<VFX3DInfo> VFXInfos;
        //public static List<VFXTypeInfo> VFXTypeInfos;
        //public static Dictionary<string, int> VFXDict;

        //public static void Init()
        //{
        //    VFXInfos = new List<VFX3DInfo>();
        //    VFXTypeInfos = new List<VFXTypeInfo>();
        //    VFXDict = new Dictionary<string, int>();

        //    Texture2D[] vfxs = AssetContainer.Mgr.TridimensionalVFX;

        //    var vfxCount = vfxs.Length / 16;
        //    VFXInfos = new List<VFX3DInfo>(vfxCount);
        //    VFXTypeInfos = new List<VFXTypeInfo>(vfxCount);
        //    VFXDict = new Dictionary<string, int>(vfxCount);
        //    int LastVFXID = 0;
        //    int LastVFXTypeID = 0;
        //    for (int j = 0; j < vfxs.Length; ++j)
        //    {
        //        var curVFX = vfxs[j];
        //        //VFX_HIT_BlinkingBubble_00_0
        //        var name = curVFX.name.Split('_');
        //        if (name[0].ToLower() != "vfx")
        //            throw new Exception($"Trying to import a VFX which is not a VFX, name:{curVFX.name}.");
        //        var vfxTypeStr = name[1].ToLower();
        //        Def.VFXType type = Def.VFXType.COUNT;
        //        if (vfxTypeStr == "hit")
        //            type = Def.VFXType.ONHIT;
        //        else if (vfxTypeStr == "trvl")
        //            type = Def.VFXType.TRAVEL;
        //        else if (vfxTypeStr == "cst")
        //            type = Def.VFXType.CAST;

        //        if (type == Def.VFXType.COUNT)
        //            throw new Exception($"Couldn't identify which type of VFX '{curVFX.name}' is.");

        //        var vfxName = name[2];

        //        bool parseOK = int.TryParse(name[3], out int vfxid);
        //        if (!parseOK)
        //            throw new Exception($"Couldn't parse the id of the VFX '{curVFX.name}'.");

        //        parseOK = int.TryParse(name[4], out int vfxFrame);
        //        if (!parseOK)
        //            throw new Exception($"Couldn't parse the frame number of the VFX '{curVFX.name}'.");

        //        VFXTypeInfo typeInfo;
        //        VFX3DInfo vfxInfo;
        //        vfxInfo.VFXID = -1;
        //        vfxInfo.Frames = null;
        //        if (!VFXDict.ContainsKey(vfxName))
        //        {
        //            typeInfo.CastVFX = new List<int>(1);
        //            typeInfo.OnHitVFX = new List<int>(1);
        //            typeInfo.TravelVFX = new List<int>(1);

        //            typeInfo.VFXTypeID = LastVFXTypeID++;
        //            typeInfo.name = vfxName;

        //            VFXDict.Add(vfxName, typeInfo.VFXTypeID);
        //            if (VFXTypeInfos.Count < (typeInfo.VFXTypeID + 1))
        //            {
        //                VFXTypeInfos.AddRange(Enumerable.Repeat(new VFXTypeInfo(), (typeInfo.VFXTypeID + 1) - VFXTypeInfos.Count));
        //            }
        //            VFXTypeInfos[typeInfo.VFXTypeID] = typeInfo;
        //        }
        //        else
        //        {
        //            var typeID = VFXDict[vfxName];
        //            typeInfo = VFXTypeInfos[typeID];
        //            switch (type)
        //            {
        //                case Def.VFXType.CAST:
        //                    if (typeInfo.CastVFX.Count >= (vfxid + 1))
        //                        vfxInfo = VFXInfos[typeInfo.CastVFX[vfxid]];
        //                    break;
        //                case Def.VFXType.ONHIT:
        //                    if (typeInfo.OnHitVFX.Count >= (vfxid + 1))
        //                        vfxInfo = VFXInfos[typeInfo.OnHitVFX[vfxid]];
        //                    break;
        //                case Def.VFXType.TRAVEL:
        //                    if (typeInfo.TravelVFX.Count >= (vfxid + 1))
        //                        vfxInfo = VFXInfos[typeInfo.TravelVFX[vfxid]];
        //                    break;
        //            }
        //        }
        //        // Was not created
        //        if (vfxInfo.VFXID < 0)
        //        {
        //            vfxInfo.VFXID = LastVFXID++;
        //            vfxInfo.Frames = new List<Texture2D>();
        //            if (VFXInfos.Count <= vfxInfo.VFXID)
        //            {
        //                VFXInfos.AddRange(Enumerable.Repeat(new VFX3DInfo(), (vfxInfo.VFXID + 1) - VFXInfos.Count));
        //            }

        //            switch (type)
        //            {
        //                case Def.VFXType.CAST:
        //                    if (typeInfo.CastVFX.Count <= vfxid)
        //                    {
        //                        typeInfo.CastVFX.AddRange(Enumerable.Repeat(-1, (vfxid + 1) - typeInfo.CastVFX.Count));
        //                    }
        //                    typeInfo.CastVFX[vfxid] = vfxInfo.VFXID;
        //                    break;
        //                case Def.VFXType.ONHIT:
        //                    if (typeInfo.OnHitVFX.Count <= vfxid)
        //                    {
        //                        typeInfo.OnHitVFX.AddRange(Enumerable.Repeat(-1, (vfxid + 1) - typeInfo.OnHitVFX.Count));
        //                    }
        //                    typeInfo.OnHitVFX[vfxid] = vfxInfo.VFXID;
        //                    break;
        //                case Def.VFXType.TRAVEL:
        //                    if (typeInfo.TravelVFX.Count <= vfxid)
        //                    {
        //                        typeInfo.TravelVFX.AddRange(Enumerable.Repeat(-1, (vfxid + 1) - typeInfo.TravelVFX.Count));
        //                    }
        //                    typeInfo.TravelVFX[vfxid] = vfxInfo.VFXID;
        //                    break;
        //            }
        //        }
        //        vfxInfo.VFXVersion = vfxid;
        //        vfxInfo.VFXTypeID = typeInfo.VFXTypeID;
        //        vfxInfo.Type = type;

        //        vfxInfo.FramesPerSec = 24;
        //        if (vfxInfo.Frames.Count <= vfxFrame)
        //            vfxInfo.Frames.AddRange(Enumerable.Repeat<Texture2D>(null, (vfxFrame + 1) - vfxInfo.Frames.Count));
        //        vfxInfo.Frames[vfxFrame] = curVFX;
        //        VFXInfos[vfxInfo.VFXID] = vfxInfo;
        //        VFXTypeInfos[vfxInfo.VFXTypeID] = typeInfo;
        //    }
        //}

        //public static void Deinit()
        //{

        //}

        public static List<VFX3DFamily> VFXFamilies;
        public static Dictionary<string, int> FamilyDict;

        public static VFX3DFamily GetVFXFamily(string familyName)
		{
            if (FamilyDict == null || VFXFamilies == null)
                throw new Exception("VFX3Ds is not ready, trying to get a VFX3DFamily before loading?");
            if (FamilyDict.ContainsKey(familyName))
            {
                return VFXFamilies[FamilyDict[familyName]];
            }
            if (VFXFamilies.Count == 0)
                throw new Exception("There are no VFX3Ds families, something went really wrong.");

            return VFXFamilies[0];
        }
        public static void Prepare()
		{
            var vfxs = AssetLoader.VFX3DTextures;

            VFXFamilies = new List<VFX3DFamily>(vfxs.Length / 10);
            FamilyDict = new Dictionary<string, int>(VFXFamilies.Capacity);

            Texture2D[] addFrame(Texture2D frame, int frameNum, Texture2D[] list)
            {
                Texture2D[] result;
                if (list != null)
                {
                    int nLength = list.Length;
                    if (list.Length <= frameNum)
                    {
                        nLength = frameNum + 1;
                    }
                    result = new Texture2D[nLength];
                    list.CopyTo(result, 0);
                }
                else
                {
                    result = new Texture2D[frameNum + 1];
                }
                result[frameNum] = frame;

                return result;
            }

            VFX3DInfoDef[] addVFX(VFX3DInfoDef vfx, int version, VFX3DInfoDef[] list)
            {
                VFX3DInfoDef[] result = null;
                if (list != null)
                {
                    int nLength = list.Length;
                    if (list.Length <= version)
                    {
                        nLength = version + 1;
                    }
                    result = new VFX3DInfoDef[nLength];
                    list.CopyTo(result, 0);
                }
                else
                {
                    result = new VFX3DInfoDef[version + 1];
                }
                result[version] = vfx;

                return result;
            }

            for (int i = 0; i < vfxs.Length; ++i)
            {
                var curVFX = vfxs[i];
                //VFX_HIT_BlinkingBubble_00_0
                var name = curVFX.name.Split('_');
                if (name[0].ToLower() != "vfx")
                    throw new Exception($"Trying to import a VFX3D which is not a VFX, invalid name format '{curVFX.name}'.");
                var vfxTypeStr = name[1].ToLower();
                Def.VFXType type = Def.VFXType.COUNT;
                switch (vfxTypeStr)
                {
                    case "hit":
                        type = Def.VFXType.ONHIT;
                        break;
                    case "trvl":
                        type = Def.VFXType.TRAVEL;
                        break;
                    case "cst":
                        type = Def.VFXType.CAST;
                        break;
                    default:
                        throw new Exception($"The current vfx '{curVFX.name}' targetting {((Def.VFXTarget)i).ToString()}, has an invalid type '{name[1]}'.");
                }
                var vfxName = name[2];

                bool parseOK = int.TryParse(name[3], out int vfxid);
                if (!parseOK)
                    throw new Exception($"Couldn't parse the VFXVersion of the VFX '{curVFX.name}'.");

                parseOK = int.TryParse(name[4], out int vfxFrame);
                if (!parseOK)
                    throw new Exception($"Couldn't parse the frame number of the VFX '{curVFX.name}'.");

                VFX3DFamily family = null;
                if (!FamilyDict.ContainsKey(vfxName))
                {
                    family = new VFX3DFamily
                    {
                        Name = vfxName,
                        CastVFX = new VFX3DInfoDef[0],
                        OnHitVFX = new VFX3DInfoDef[0],
                        TravelVFX = new VFX3DInfoDef[0]
                    };
                    FamilyDict.Add(vfxName, VFXFamilies.Count);
                    VFXFamilies.Add(family);
                }
                else
                {
                    family = VFXFamilies[FamilyDict[vfxName]];
                }

                VFX3DInfoDef info = null;
                switch (type)
                {
                    case Def.VFXType.CAST:
                        if (family.CastVFX.Length > vfxid)
                        {
                            info = family.CastVFX[vfxid];
                        }
                        break;
                    case Def.VFXType.TRAVEL:
                        if (family.TravelVFX.Length > vfxid)
                        {
                            info = family.TravelVFX[vfxid];
                        }
                        break;
                    case Def.VFXType.ONHIT:
                        if (family.OnHitVFX.Length > vfxid)
                        {
                            info = family.OnHitVFX[vfxid];
                        }
                        break;
                }
                if (info == null)
                {
                    info = new VFX3DInfoDef
                    {
                        Family = family,
                        Frames = new Texture2D[0],
                        FramesPerSec = 12,
                        Type = type,
                        VFXID = FamilyDict[family.Name],
                        VFXVersion = vfxid
                    };
                    switch (type)
                    {
                        case Def.VFXType.CAST:
                            family.CastVFX = addVFX(info, vfxid, family.CastVFX);
                            break;
                        case Def.VFXType.TRAVEL:
                            family.TravelVFX = addVFX(info, vfxid, family.TravelVFX);
                            break;
                        case Def.VFXType.ONHIT:
                            family.OnHitVFX = addVFX(info, vfxid, family.OnHitVFX);
                            break;
                    }
                }
                info.Frames = addFrame(curVFX, vfxFrame, info.Frames);
            }
        }
        public static void Init()
        {
            var vfxs = Resources.LoadAll<Texture2D>("VFX/3D");

            VFXFamilies = new List<VFX3DFamily>(vfxs.Length / 10);
            FamilyDict = new Dictionary<string, int>(VFXFamilies.Capacity);

            Texture2D[] addFrame(Texture2D frame, int frameNum, Texture2D[] list)
            {
                Texture2D[] result;
                if (list != null)
                {
                    int nLength = list.Length;
                    if (list.Length <= frameNum)
                    {
                        nLength = frameNum + 1;
                    }
                    result = new Texture2D[nLength];
                    list.CopyTo(result, 0);
                }
                else
                {
                    result = new Texture2D[frameNum + 1];
                }
                result[frameNum] = frame;

                return result;
            }

            VFX3DInfoDef[] addVFX(VFX3DInfoDef vfx, int version, VFX3DInfoDef[] list)
            {
                VFX3DInfoDef[] result = null;
                if (list != null)
                {
                    int nLength = list.Length;
                    if (list.Length <= version)
                    {
                        nLength = version + 1;
                    }
                    result = new VFX3DInfoDef[nLength];
                    list.CopyTo(result, 0);
                }
                else
                {
                    result = new VFX3DInfoDef[version + 1];
                }
                result[version] = vfx;

                return result;
            }

            for(int i = 0; i < vfxs.Length; ++i)
            {
                var curVFX = vfxs[i];
                //VFX_HIT_BlinkingBubble_00_0
                var name = curVFX.name.Split('_');
                if (name[0].ToLower() != "vfx")
                    throw new Exception($"Trying to import a VFX3D which is not a VFX, invalid name format '{curVFX.name}'.");
                var vfxTypeStr = name[1].ToLower();
                Def.VFXType type = Def.VFXType.COUNT;
                switch (vfxTypeStr)
                {
                    case "hit":
                        type = Def.VFXType.ONHIT;
                        break;
                    case "trvl":
                        type = Def.VFXType.TRAVEL;
                        break;
                    case "cst":
                        type = Def.VFXType.CAST;
                        break;
                    default:
                        throw new Exception($"The current vfx '{curVFX.name}' targetting {((Def.VFXTarget)i).ToString()}, has an invalid type '{name[1]}'.");
                }
                var vfxName = name[2];

                bool parseOK = int.TryParse(name[3], out int vfxid);
                if (!parseOK)
                    throw new Exception($"Couldn't parse the VFXVersion of the VFX '{curVFX.name}'.");

                parseOK = int.TryParse(name[4], out int vfxFrame);
                if (!parseOK)
                    throw new Exception($"Couldn't parse the frame number of the VFX '{curVFX.name}'.");

                VFX3DFamily family = null;
                if (!FamilyDict.ContainsKey(vfxName))
                {
                    family = new VFX3DFamily
                    {
                        Name = vfxName,
                        CastVFX = new VFX3DInfoDef[0],
                        OnHitVFX = new VFX3DInfoDef[0],
                        TravelVFX = new VFX3DInfoDef[0]
                    };
                    FamilyDict.Add(vfxName, VFXFamilies.Count);
                    VFXFamilies.Add(family);
                }
                else
                {
                    family = VFXFamilies[FamilyDict[vfxName]];
                }

                VFX3DInfoDef info = null;
                switch (type)
                {
                    case Def.VFXType.CAST:
                        if (family.CastVFX.Length > vfxid)
                        {
                            info = family.CastVFX[vfxid];
                        }
                        break;
                    case Def.VFXType.TRAVEL:
                        if (family.TravelVFX.Length > vfxid)
                        {
                            info = family.TravelVFX[vfxid];
                        }
                        break;
                    case Def.VFXType.ONHIT:
                        if (family.OnHitVFX.Length > vfxid)
                        {
                            info = family.OnHitVFX[vfxid];
                        }
                        break;
                }
                if (info == null)
                {
                    info = new VFX3DInfoDef
                    {
                        Family = family,
                        Frames = new Texture2D[0],
                        FramesPerSec = 12,
                        Type = type,
                        VFXID = FamilyDict[family.Name],
                        VFXVersion = vfxid
                    };
                    switch (type)
                    {
                        case Def.VFXType.CAST:
                            family.CastVFX = addVFX(info, vfxid, family.CastVFX);
                            break;
                        case Def.VFXType.TRAVEL:
                            family.TravelVFX = addVFX(info, vfxid, family.TravelVFX);
                            break;
                        case Def.VFXType.ONHIT:
                            family.OnHitVFX = addVFX(info, vfxid, family.OnHitVFX);
                            break;
                    }
                }
                info.Frames = addFrame(curVFX, vfxFrame, info.Frames);
            }

            //void setTextureArray(VFX3DInfoDef[] infolist)
            //{
            //    if (infolist == null)
            //        return;
            //    for(int i = 0; i < infolist.Length; ++i)
            //    {
            //        var info = infolist[i];
            //        info.TAFrames = new Texture2DArray(info.Frames[0].width, info.Frames[0].height, info.Frames.Length, info.Frames[0].format, info.Frames[0].mipmapCount > 1);
            //        for(int j = 0; j < info.Frames.Length; ++j)
            //        {
            //            info.TAFrames.SetPixels32(info.Frames[j].GetPixels32(), j);
            //        }
            //    }
            //}
            //for(int i = 0; i < VFXFamilies.Count; ++i)
            //{
            //    var family = VFXFamilies[i];
            //    setTextureArray(family.CastVFX);
            //    setTextureArray(family.TravelVFX);
            //    setTextureArray(family.OnHitVFX);
            //}
        }

        public static void Deinit()
        {

        }
    }
}
