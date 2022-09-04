/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Assets
//{
    //public struct LayerInfo
    //{
    //    public int Layer;

    //    public List<int> MaterialTypes;
    //    public List<float> MaterialTypeChances;

    //    public bool EnableRandomRotation;
    //    public Definitions.RotationState DefaultRotation;

    //    public float BlockHeight;

    //    public bool EnableRandomMicroHeight;
    //    public float BlockMicroHeightMin;
    //    public float BlockMicroHeightMax;

    //    public bool EnableRandomBlockLength;
    //    public float BlockLengthMin;
    //    public float BlockLengthMax;

    //    public bool EnableRandomProps;
    //    public float PropChance;
    //    public List<int> Props;
    //    public List<float> PropChances;
    //    public float PropNoSpawnRadius;

    //    public bool SpawnMonsters;
    //    public bool SpawnZoneMonsters;
    //    public bool LayerMonstersRespawn;
    //    public float MonsterChance;
    //    public List<int> Monsters;
    //    public List<float> MonsterChances;

    //    public bool HasEffects;
    //    public List<int> Effects;

    //    public bool EnableBlockFloat;
    //    public float BlockFloatMin;
    //    public float BlockFloatMax;
    //    public float BlockFloatSpeed;

    //    public float RandomWideBlockChance;

    //    public float RandomStairBlockChance;

    //    public bool IsLinkedLayer;
    //    public List<int> LinkedLayers;
    //    public List<float> LinkedLayerChances;

    //    public bool IsValid()
    //    {
    //        if (Layer == 0 || Layer > Definitions.MaxLayerSlots)
    //            return false;
    //        if (IsLinkedLayer)
    //        {
    //            if (LinkedLayers.Count < 2)
    //                return false;
    //        }
    //        else
    //        {
    //            if (MaterialTypes.Count == 0)
    //                return false;
    //            if (MaterialTypes.Count != MaterialTypeChances.Count)
    //                return false;
    //            if (EnableRandomProps && Props.Count == 0)
    //                return false;
    //            if (PropChances.Count != Props.Count)
    //                return false;
    //            if (SpawnMonsters && Monsters.Count == 0)
    //                return false;
    //            if (MonsterChances.Count != Monsters.Count)
    //                return false;
    //            if (HasEffects && Effects.Count == 0)
    //                return false;
    //            if (!EnableRandomMicroHeight && (BlockMicroHeightMax != BlockMicroHeightMin))
    //                return false;
    //            if (!EnableRandomBlockLength && (BlockLengthMin != BlockLengthMax))
    //                return false;
    //        }

    //        return true;
    //    }

    //    public void ToLayerIE(ref IE.V2.LayerIE ie)
    //    {
    //        ie.Layer = (byte)Layer;
    //        ie.IsLinkedLayer = IsLinkedLayer;

    //        ie.EnableRandomRotation = EnableRandomRotation;
    //        ie.DefaultBlockRotation = DefaultRotation;

    //        var familyNames = new List<string>(MaterialTypes.Count);
    //        for(int i = 0; i < MaterialTypes.Count; ++i)
    //        {
    //            familyNames.Add(BlockMaterial.MaterialFamilies[MaterialTypes[i]].FamilyName);
    //        }
    //        ie.MaterialFamilies = familyNames;
    //        ie.MaterialFamilyChances = MaterialTypeChances;

    //        ie.BlockHeight = BlockHeight;

    //        ie.EnableRandomMicroHeight = EnableRandomMicroHeight;
    //        ie.BlockMicroHeightMin = BlockMicroHeightMin;
    //        ie.BlockMicroHeightMax = BlockMicroHeightMax;

    //        ie.EnableRandomBlockLenght = EnableRandomBlockLength;
    //        ie.BlockLenghtMin = BlockLengthMin;
    //        ie.BlockLenghtMax = BlockLengthMax;

    //        ie.EnableRandomProps = EnableRandomProps;
    //        ie.PropChance = PropChance;
    //        ie.Props = Props;
    //        ie.PropChances = PropChances;
    //        ie.PropNoSpawnRadius = PropNoSpawnRadius;

    //        ie.SpawnMonsters = SpawnMonsters;
    //        ie.SpawnZoneMonsters = SpawnZoneMonsters;
    //        ie.LayerMonstersRespawn = LayerMonstersRespawn;
    //        ie.Monsters = Monsters;
    //        ie.MonsterChances = MonsterChances;
    //        ie.MonsterChance = MonsterChance;

    //        ie.HasEffect = HasEffects;
    //        ie.Effects = Effects;

    //        ie.EnableBlockFloat = EnableBlockFloat;
    //        ie.BlockFloatMin = BlockFloatMin;
    //        ie.BlockFloatMax = BlockFloatMax;
    //        ie.BlockFloatSpeed = BlockFloatSpeed;

    //        ie.RandomWideBlockChance = RandomWideBlockChance;
    //        ie.RandomStairBlockChance = RandomStairBlockChance;

    //        ie.LinkedLayers = LinkedLayers;
    //        ie.LinkedLayerChances = LinkedLayerChances;
    //    }

    //    public static LayerInfo GetDefaultLayer()
    //    {
    //        LayerInfo layer;
    //        layer.Layer = 0;
    //        layer.MaterialTypes = new List<int>();
    //        layer.MaterialTypeChances = new List<float>();
    //        layer.EnableRandomRotation = true;
    //        layer.DefaultRotation = Definitions.RotationState.Default;
    //        layer.BlockHeight = 0.0f;
    //        layer.EnableRandomMicroHeight = false;
    //        layer.BlockMicroHeightMin = 0.0f;
    //        layer.BlockMicroHeightMax = 0.0f;
    //        layer.EnableRandomBlockLength = false;
    //        layer.BlockLengthMin = 1.0f;
    //        layer.BlockLengthMax = 1.0f;
    //        layer.EnableRandomProps = false;
    //        layer.Props = new List<int>();
    //        layer.PropChance = 0.5f;
    //        layer.PropChances = new List<float>();
    //        layer.PropNoSpawnRadius = 1;
    //        layer.SpawnMonsters = false;
    //        layer.SpawnZoneMonsters = true;
    //        layer.LayerMonstersRespawn = false;
    //        layer.Monsters = new List<int>();
    //        layer.MonsterChance = 0.5f;
    //        layer.MonsterChances = new List<float>();
    //        layer.HasEffects = false;
    //        layer.Effects = new List<int>();
    //        layer.EnableBlockFloat = false;
    //        layer.BlockFloatMin = 0.0f;
    //        layer.BlockFloatMax = 0.0f;
    //        layer.BlockFloatSpeed = 0.0f;
    //        layer.RandomWideBlockChance = 0.25f;
    //        layer.RandomStairBlockChance = 0.50f;
    //        layer.IsLinkedLayer = false;
    //        layer.LinkedLayers = new List<int>();
    //        layer.LinkedLayerChances = new List<float>();
    //        return layer;
    //    }
    //}
//}