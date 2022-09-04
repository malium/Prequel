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
    public enum BridgeType
    {
        SMALL,
        BIG,
        COUNT
    }

    public struct BridgeInfo
    {
        public BridgeType Type;
        public int TypeID;
        public int BridgeID;
        public BridgeComponent BridgeGO;
    }

    public struct BridgeTypeInfo
    {
        public string TypeName;
        public int TypeID;
        public List<int> SmallBridges;
        public List<int> BigBridges;
    }

    public static class Bridges
    {
        public static List<BridgeInfo> BridgeInfos;
        public static List<BridgeTypeInfo> BridgeTypeInfos;
        public static Dictionary<string, int> BridgeTypeDict;

        public static BridgeTypeInfo GetBridgeTypeInfo(string bridgeTypeName)
		{
            if (BridgeTypeDict == null || BridgeTypeInfos == null)
                throw new Exception("Bridges is not ready, trying to get a BridgeTypeInfo before loading?");
            if (BridgeTypeDict.ContainsKey(bridgeTypeName))
            {
                return BridgeTypeInfos[BridgeTypeDict[bridgeTypeName]];
            }
            if (BridgeTypeInfos.Count == 0)
                throw new Exception("There are no Bridges, something went really wrong.");

            return BridgeTypeInfos[0];
        }
        public static void Prepare()
		{
            BridgeInfos = new List<BridgeInfo>();
            BridgeTypeInfos = new List<BridgeTypeInfo>();
            BridgeTypeDict = new Dictionary<string, int>();
            int LastBridgeID = 0;
            int LastBridgeTypeID = 0;

            var bridges = AssetLoader.Bridges;
            for (int i = 0; i < bridges.Length; ++i)
            {
                var bridgeGO = bridges[i];
                if (bridgeGO == null)
                    throw new Exception($"Trying to init Bridges but Bridge_{i} was null.");
                var name = bridges[i].name.Split('_');
                var bridgeName = name[0].ToLower();
                if (bridgeName != "bridge")
                    throw new Exception($"Trying to init Bridges but Bridge_{i} was not a bridge.");

                var bridgeTypeName = name[1];
                int typeID = -1;
                if (BridgeTypeDict.ContainsKey(bridgeTypeName))
                {
                    typeID = BridgeTypeDict[bridgeTypeName];
                }
                BridgeTypeInfo typeInfo;
                typeInfo.TypeID = typeID;
                typeInfo.TypeName = bridgeTypeName;
                if (typeID == -1)
                {
                    typeID = LastBridgeTypeID++;
                    typeInfo.SmallBridges = new List<int>(2);
                    typeInfo.BigBridges = new List<int>(2);
                    typeInfo.TypeID = typeID;
                    BridgeTypeDict.Add(bridgeTypeName, typeID);
                    if (BridgeTypeInfos.Count < (typeID + 1))
                    {
                        BridgeTypeInfos.AddRange(Enumerable.Repeat(new BridgeTypeInfo(), (typeID + 1) - BridgeTypeInfos.Count));
                    }
                    BridgeTypeInfos[typeID] = typeInfo;
                }
                else
                {
                    typeInfo = BridgeTypeInfos[typeID];
                }

                bool isBig = name[2].ToLower() == "big";
                BridgeInfo bridgeInfo;
                bridgeInfo.TypeID = typeID;
                bridgeInfo.Type = isBig ? BridgeType.BIG : BridgeType.SMALL;
                bridgeInfo.BridgeID = LastBridgeID++;
                bridgeInfo.BridgeGO = bridgeGO;
                if (bridgeInfo.Type == BridgeType.BIG)
                    typeInfo.BigBridges.Add(bridgeInfo.BridgeID);
                else
                    typeInfo.SmallBridges.Add(bridgeInfo.BridgeID);
                if (BridgeInfos.Count < (bridgeInfo.BridgeID + 1))
                {
                    BridgeInfos.AddRange(Enumerable.Repeat(new BridgeInfo(), (bridgeInfo.BridgeID + 1) - BridgeInfos.Count));
                }
                BridgeInfos[bridgeInfo.BridgeID] = bridgeInfo;
            }
        }
        //public static void Init()
        //{
        //    BridgeInfos = new List<BridgeInfo>();
        //    BridgeTypeInfos = new List<BridgeTypeInfo>();
        //    BridgeTypeDict = new Dictionary<string, int>();
        //    int LastBridgeID = 0;
        //    int LastBridgeTypeID = 0;

        //    var bridges = AssetContainer.Mgr.Bridges;
        //    for(int i = 0; i < bridges.Length; ++i)
        //    {
        //        var bridgeGO = bridges[i];
        //        if (bridgeGO == null)
        //            throw new Exception($"Trying to init Bridges but Bridge_{i} was null.");
        //        var name = bridges[i].name.Split('_');
        //        var bridgeName = name[0].ToLower();
        //        if (bridgeName != "bridge")
        //            throw new Exception($"Trying to init Bridges but Bridge_{i} was not a bridge.");

        //        var bridgeTypeName = name[1];
        //        int typeID = -1;
        //        if(BridgeTypeDict.ContainsKey(bridgeTypeName))
        //        {
        //            typeID = BridgeTypeDict[bridgeTypeName];
        //        }
        //        BridgeTypeInfo typeInfo;
        //        typeInfo.TypeID = typeID;
        //        typeInfo.TypeName = bridgeTypeName;
        //        if(typeID == -1)
        //        {
        //            typeID = LastBridgeTypeID++;
        //            typeInfo.SmallBridges = new List<int>(2);
        //            typeInfo.BigBridges = new List<int>(2);
        //            typeInfo.TypeID = typeID;
        //            BridgeTypeDict.Add(bridgeTypeName, typeID);
        //            if(BridgeTypeInfos.Count < (typeID + 1))
        //            {
        //                BridgeTypeInfos.AddRange(Enumerable.Repeat(new BridgeTypeInfo(), (typeID + 1) - BridgeTypeInfos.Count));
        //            }
        //            BridgeTypeInfos[typeID] = typeInfo;
        //        }
        //        else
        //        {
        //            typeInfo = BridgeTypeInfos[typeID];
        //        }

        //        bool isBig = name[2].ToLower() == "big";
        //        BridgeInfo bridgeInfo;
        //        bridgeInfo.TypeID = typeID;
        //        bridgeInfo.Type = isBig ? BridgeType.BIG : BridgeType.SMALL;
        //        bridgeInfo.BridgeID = LastBridgeID++;
        //        bridgeInfo.BridgeGO = bridgeGO;
        //        if (bridgeInfo.Type == BridgeType.BIG)
        //            typeInfo.BigBridges.Add(bridgeInfo.BridgeID);
        //        else
        //            typeInfo.SmallBridges.Add(bridgeInfo.BridgeID);
        //        if(BridgeInfos.Count < (bridgeInfo.BridgeID + 1))
        //        {
        //            BridgeInfos.AddRange(Enumerable.Repeat(new BridgeInfo(), (bridgeInfo.BridgeID + 1) - BridgeInfos.Count));
        //        }
        //        BridgeInfos[bridgeInfo.BridgeID] = bridgeInfo;
        //    }
        //}

        //public static void Deinit()
        //{

        //}
    }
}
