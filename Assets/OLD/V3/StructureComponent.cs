/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Assets
//{
//    // Is the structure flipped? In what direction?
//    public enum StructureFlip
//    {
//        NoFlip,
//        HorizontalFlip,
//        VerticalFlip,
//        VerticalAndHorizontalFlip,
//        COUNT
//    }

//    // GameObject that holds the position and rotation of the structure
//    public class StructureComponent : MonoBehaviour
//    {
//        public const int Width = 8;
//        public const int Height = 8;
//        public const float Separation = 0.02f;
//        public const int LayerAmount = 8;
        
//        public List<LivingEntity> LivingEntities;

//        [SerializeField]
//        Rect m_StructRect;
//        [SerializeField]
//        int m_IDXIE;
//        [SerializeField]
//        LayerInfo[] m_InfoLayers;
//        [SerializeField]
//        List<BlockComponent>[] m_Layers;
//        [SerializeField]
//        PilarComponent[] m_Pilars;
//        [SerializeField]
//        StructureFlip m_Flip;
//        [SerializeField]
//        BlockRotation m_Rotation;

//        public int IDXIE
//        {
//            get
//            {
//                return m_IDXIE;
//            }
//            set
//            {
//                m_IDXIE = value;
//                gameObject.name = $"Structure_{m_IDXIE}";
//            }
//        }
//        // Is the structure rotated?
//        public BlockRotation Rotation
//        {
//            get
//            {
//                return m_Rotation;
//            }
//            set
//            {
//                var rotation = value;
//                if (m_Rotation == rotation)
//                    return;

//                int rotations = 0;
//                if (m_Rotation < rotation)
//                {
//                    rotations = rotation - m_Rotation;
//                }
//                else
//                {
//                    rotations = (BlockRotation.COUNT - m_Rotation) + (int)rotation;
//                }
//                for (int i = 0; i < rotations; ++i)
//                    Rotation90();

//                m_Rotation = rotation;
//            }
//        }
//        // Layer definition and information
//        public LayerInfo[] InfoLayers
//        {
//            get
//            {
//                return m_InfoLayers;
//            }
//        }
//        // What blocks currently belong to each layer
//        public List<BlockComponent>[] Layers
//        {
//            get
//            {
//                return m_Layers;
//            }
//        }
//        // Array of pilars, all the pilars from the structure are here and can be accessed via their ID
//        public PilarComponent[] Pilars
//        {
//            get
//            {
//                return m_Pilars;
//            }
//        }
//        // Is the structure flipped?
//        public StructureFlip Flip
//        {
//            get
//            {
//                return m_Flip;
//            }
//            set
//            {
//                var flip = value;
//                if (m_Flip == flip)
//                    return;

//                switch (m_Flip) // return to original state
//                {
//                    case StructureFlip.HorizontalFlip:
//                        HFlip();
//                        break;
//                    case StructureFlip.VerticalFlip:
//                        VFlip();
//                        break;
//                    case StructureFlip.VerticalAndHorizontalFlip:
//                        HFlip();
//                        VFlip();
//                        break;
//                    default:
//                        break;
//                }

//                switch (flip) // to the new state
//                {
//                    case StructureFlip.HorizontalFlip:
//                        HFlip();
//                        break;
//                    case StructureFlip.VerticalFlip:
//                        VFlip();
//                        break;
//                    case StructureFlip.VerticalAndHorizontalFlip:
//                        VFlip();
//                        HFlip();
//                        break;
//                    default:
//                        break;
//                }

//                m_Flip = flip;
//            }
//        }

//        public Rect StructRect
//        {
//            get
//            {
//                return m_StructRect;
//            }
//            set
//            {
//                m_StructRect = value;
//            }
//        }

//        void ApplyLayerToBlock(int layer, BlockComponent block, bool ignoreLock = false)
//        {
//            var infoLayer = InfoLayers[layer - 1];
//            if (infoLayer.IsLinkedLayer)
//            {
//                //var rng = Manager.Mgr.BuildRNG;
//                float chance = UnityEngine.Random.value; //(float)rng.NextDouble();
//                float accum = 0.0f;
//                for (int i = 0; i < infoLayer.LinkedLayerChances.Count; ++i)
//                {
//                    float nextChance = accum + infoLayer.LinkedLayerChances[i];

//                    if (chance > accum && chance < nextChance)
//                    {
//                        ApplyLayerToBlock(InfoLayers[infoLayer.LinkedLayers[i] - 1], block, ignoreLock, layer);
//                        return;
//                    }
//                    accum = nextChance;
//                }
//            }
//            else
//            {
//                ApplyLayerToBlock(infoLayer, block);
//            }
//        }

//        void ApplyLayerToBlock(LayerInfo layer, BlockComponent block, bool ignoreLock = false, int linkedTo = -1)
//        {
//            if (block.Locked == BlockLock.Locked && !ignoreLock)
//                return;

//            //var rng = Manager.Mgr.BuildRNG;
//            // Layer assign
//            {
//                //block.Layer = layer.Layer;
//            }
//            BitArray flags;
//            if (block.IDXIE >= 0 && !ignoreLock)
//            {
//                var strucIE = Structures.Strucs[IDXIE];
//                var blocks = strucIE.Blocks;
//                var bie = blocks[block.IDXIE];
//                flags = bie.Flags;
//            }
//            else
//            {
//                flags = new BitArray((int)IE.V2.BlockIE.Flag.COUNT);
//                flags.SetAll(false);
//            }
//            // IE
//            {
//                // To default
//                //var layerIE = block.IE.Layer;
//                //block.IE.SetDefault();
//                //block.IE.Layer = layerIE;
//                /*if(block.IDXIE >= 0)
//                {
//                    var strucIE = Structures.Strucs[IDXIE];
//                    var blocks = strucIE.Blocks;
//                    var blk = blocks[block.IDXIE];
//                    blk.SetDefault();
//                    blk.Layer = (byte)(linkedTo >= 0 ? linkedTo : layer.Layer);
//                    blocks[block.IDXIE] = blk;
//                    strucIE.Blocks = blocks;
//                    Structures.Strucs[IDXIE] = strucIE;
                    
//                }*/
//            }
//            // Material assign
//            if (!flags[(int)IE.V2.BlockIE.Flag.MaterialType])
//            {
//                //float chance = (float)rng.NextDouble();
//                //float accum = 0.0f;
//                //for (int i = 0; i < layer.SubMaterialChances.Count; ++i)
//                //{
//                //    float nextChance = accum + layer.SubMaterialChances[i];

//                //    if (chance > accum && chance < nextChance)
//                //    {
//                //        //var matType = BlockMaterial.materialSubTypes[layer.SubMaterials[i]];
//                //        //if(block.blockType == BlockType.STAIRS && matType)

//                //        block.SubMaterialID = layer.SubMaterials[i];
//                //        break;
//                //    }
//                //    accum = nextChance;
//                //}
//                List<int> availableSubmaterials = new List<int>(layer.MaterialTypes.Count);
//                List<float> availableSubmaterialChances = new List<float>(layer.MaterialTypes.Count);

//                for (int j = 0; j < layer.MaterialTypes.Count; ++j)
//                {
//                    var family = BlockMaterial.MaterialFamilies[layer.MaterialTypes[j]];
//                    //var matType = BlockMaterial.MaterialTypes[layer.MaterialTypes[j]];
//                    if(family.GetSet(block.blockType).Length > 0)
//                    //if (matType.Def.Materials[(int)block.blockType].Count > 0)
//                    {
//                        availableSubmaterials.Add(layer.MaterialTypes[j]);
//                        availableSubmaterialChances.Add(layer.MaterialTypeChances[j]);
//                    }
//                }
//                GameUtils.UpdateChances(ref availableSubmaterialChances);

//                float chance = UnityEngine.Random.value; //(float)Manager.Mgr.BuildRNG.NextDouble();
//                float accum = 0.0f;
//                for (int j = 0; j < availableSubmaterialChances.Count; ++j)
//                {
//                    float nextChance = accum + availableSubmaterialChances[j];

//                    if (chance > accum && chance < nextChance)
//                    {
//                        block.MaterialFmly = BlockMaterial.MaterialFamilies[availableSubmaterials[j]];
//                        //block.MaterialTypeID = availableSubmaterials[j];
//                        break;
//                    }
//                    accum = nextChance;
//                }
//            }

//            // Rotation
//            if (!flags[(int)IE.V2.BlockIE.Flag.Rotation])
//            {
//                if (layer.EnableRandomRotation)
//                {
//                    float chance = UnityEngine.Random.value /*(float)rng.NextDouble()*/ * (float)BlockRotation.COUNT;
//                    block.Rotation = (BlockRotation)Mathf.FloorToInt(chance);
//                }
//                else
//                {
//                    block.Rotation = layer.DefaultRotation;
//                }
//            }

//            // Default height
//            if (!flags[(int)IE.V2.BlockIE.Flag.Height])
//            {
//                block.Height = layer.BlockHeight;
//            }

//            // Block height
//            if (!flags[(int)IE.V2.BlockIE.Flag.Height])
//            {
//                if (layer.EnableRandomMicroHeight)
//                {
//                    float chance = /*(float)rng.NextDouble()*/  UnityEngine.Random.value * (layer.BlockMicroHeightMax - layer.BlockMicroHeightMin) + layer.BlockMicroHeightMin;
//                    float chance100 = chance * 100.0f;
//                    float ichance100 = Mathf.Floor(chance100);
//                    float fchance100 = chance100 - ichance100;
//                    fchance100 *= 0.01f;
//                    if (fchance100 >= 0.5f)
//                        chance = ichance100 * 0.01f + 0.05f;
//                    else
//                        chance = ichance100 * 0.01f;

//                    chance = Mathf.Clamp(chance, -0.25f, 0.25f);
//                    block.MicroHeight = chance;
//                }
//                else
//                {
//                    block.MicroHeight = layer.BlockMicroHeightMin;
//                }
//            }

//            // Block length
//            if (!flags[(int)IE.V2.BlockIE.Flag.Length])
//            {
//                if (layer.EnableRandomBlockLength)
//                {

//                    float chance = /*(float)rng.NextDouble()*/ UnityEngine.Random.value * (layer.BlockLengthMax - layer.BlockLengthMin) + layer.BlockLengthMin;
//                    if (chance > 3.0f)
//                    {
//                        chance = 3.4f;
//                    }
//                    else if (chance < 0.5f)
//                    {
//                        chance = 0.1f;
//                    }
//                    else
//                    {
//                        // integer part
//                        float ichance = Mathf.Floor(chance);
//                        // float part
//                        float fchance = chance - ichance;
//                        if (fchance >= 0.75)
//                            chance = ichance + 1.0f;
//                        else if (fchance < 0.75 && fchance > 0.25f)
//                            chance = ichance + 0.5f;
//                        else if (fchance < 0.25f)
//                            chance = ichance;
//                    }
//                    block.Length = chance;
//                }
//                else
//                {
//                    block.Length = layer.BlockLengthMin;
//                }
//            }

//            // Monsters
//            if (!flags[(int)IE.V2.BlockIE.Flag.Monster])
//            {
//                if (block.Monster != null)
//                {
//                    block.Monster.ReceiveDamage(Def.DamageType.UNAVOIDABLE, block.Monster.GetTotalHealth());
//                    LivingEntities.Remove(block.Monster);
//                    block.Monster = null;
//                }
//                if (layer.SpawnMonsters)
//                {
//                    float chance = UnityEngine.Random.value; //(float)rng.NextDouble();
//                    if (chance < layer.MonsterChance)
//                    {
//                        chance = UnityEngine.Random.value; //(float)rng.NextDouble();
//                        float accum = 0.0f;
//                        for (int i = 0; i < layer.Monsters.Count; ++i)
//                        {
//                            float nextChance = accum + layer.MonsterChances[i];

//                            if (chance > accum && chance < nextChance)
//                            {
//                                //var monster = Monsters.MonsterSprites[layer.Monsters[i]];
//                                var mon = new GameObject($"Monster_{MonsterScript.MonsterID++}");
//                                mon.transform.position = block.Pilar.transform.position + new Vector3(0.0f, block.Height + block.MicroHeight, 0.0f);
//                                //mon.transform.Translate(block.Pilar.transform.position, Space.World);
//                                //mon.transform.Translate(new Vector3(0.0f, block.Height + block.MicroHeight, 0.0f), Space.World);
//                                //block.Monster = mon.AddComponent<MonsterScript>();
//                                block.Monster = Monsters.AddMonsterComponent(mon, layer.Monsters[i]);
//                                //block.Monster.SetMonster(layer.Monsters[i]);
//                                block.Monster.InitMonster();
//                                block.Monster.enabled = true;
//                                block.Monster.SpriteSR.enabled = false;
//                                //block.Monster.SpriteBC.enabled = false;
//                                block.Monster.SpriteCC.enabled = false;
//                                block.Monster.ShadowSR.enabled = false;
//                                //block.Monster.Struc = this;
//                                //LivingEntities.Add(block.Monster);
//                                var facing = block.Monster.Facing;
//                                float nChance = UnityEngine.Random.value; //(float)rng.NextDouble();
//                                if (nChance >= 0.5f)
//                                    facing.Horizontal = SpriteHorizontal.RIGHT;
//                                nChance = UnityEngine.Random.value; //(float)rng.NextDouble();
//                                if (nChance >= 0.5f)
//                                    facing.Vertical = SpriteVertical.UP;
//                                block.Monster.Facing = facing;
//                                break;
//                            }
//                            accum = nextChance;
//                        }
//                    }
//                }

//            }
//            if (!flags[(int)IE.V2.BlockIE.Flag.Prop])
//            {
//                bool spawnProp = block.Monster == null;
//                // Prop NoSpawn radius
//                {
//                    if (layer.PropNoSpawnRadius > 0 && spawnProp)
//                    {
//                        List<BlockComponent> layerBlocks = null;
//                        if (linkedTo != -1)
//                            layerBlocks = m_Layers[linkedTo - 1];
//                        else
//                            layerBlocks = m_Layers[layer.Layer - 1];
//                        for (int i = 0; i < layerBlocks.Count; ++i)
//                        {
//                            if (layerBlocks[i] == block || layerBlocks[i].Prop == null)
//                                continue;

//                            var oid = layerBlocks[i].Pilar.StructureID;
//                            var mid = block.Pilar.StructureID;

//                            var oPos = GameUtils.PosFromID(oid);
//                            var mPos = GameUtils.PosFromID(mid);

//                            if (Vector2.Distance(oPos, mPos) <= layer.PropNoSpawnRadius)
//                            {
//                                spawnProp = false;
//                                break;
//                            }
//                        }
//                    }
//                }
//                // Props
//                {
//                    if (block.Prop != null)
//                    {
//                        block.Prop.ReceiveDamage(Def.DamageType.UNAVOIDABLE, block.Prop.GetTotalHealth());
//                        LivingEntities.Remove(block.Prop);
//                        block.Prop = null;
//                    }
//                    if (layer.EnableRandomProps && spawnProp && block.blockType != BlockType.STAIRS)
//                    {
//                        float chance = UnityEngine.Random.value; //(float)rng.NextDouble();
//                        if (chance < layer.PropChance)
//                        {
//                            chance = UnityEngine.Random.value; //(float)rng.NextDouble();
//                            float accum = 0.0f;
//                            for (int i = 0; i < layer.Props.Count; ++i)
//                            {
//                                float nextChance = accum + layer.PropChances[i];

//                                if (chance > accum && chance < nextChance)
//                                {
//                                    var propFamilyID = layer.Props[i];
//                                    var propFamily = Props.PropFamilies[propFamilyID];
//                                    if (propFamily.Props.Length == 0)
//                                        throw new Exception($"This prop {propFamily.FamilyName}, does not have any available prop.");
//                                    var propID = UnityEngine.Random.Range(0, propFamily.Props.Length); //rng.Next(0, propFamily.Props.Length);
//                                    var prop = new GameObject($"Prop_{PropScript.PropID++}");
//                                    block.Prop = prop.AddComponent<PropScript>();
//                                    block.Prop.Block = block;
//                                    block.Prop.SetProp(propFamilyID, propID);
//                                    block.Prop.enabled = true;
//                                    block.Prop.SpriteSR.enabled = false;
//                                    block.Prop.SpriteBC.enabled = false;
//                                    if(block.Prop.ShadowSR != null)
//                                        block.Prop.ShadowSR.enabled = false;
//                                    LivingEntities.Add(block.Prop);
//                                    if (block.Prop.PropLight != null)
//                                        block.Prop.PropLight.enabled = false;
//                                    float nChance = UnityEngine.Random.value; //(float)rng.NextDouble();
//                                    var facing = block.Prop.Facing;
//                                    if (nChance >= 0.5f)
//                                        facing.Horizontal = SpriteHorizontal.RIGHT;
//                                    block.Prop.Facing = facing;
//                                    break;
//                                }
//                                accum = nextChance;
//                            }
//                        }
//                    }
//                }
//            }

//            // Effects
//            {

//            }

//            // Floating
//            {

//            }
//        }

//        public static BlockComponent[] GetNearBlocks(StructureComponent structure, int structID, float height, int layer)
//        {
//            var list = new BlockComponent[3];
//            var blockPos = GameUtils.PosFromID(structID);
//            if (blockPos.x >= (Width - 1) || blockPos.y >= (Height - 1) || structure == null)
//                return list;

//            int[] ids = new int[3]
//            {
//                GameUtils.IDFromPos(new Vector2Int(blockPos.x + 1, blockPos.y)),     // Right
//                GameUtils.IDFromPos(new Vector2Int(blockPos.x, blockPos.y + 1)),     // BottomLeft
//                GameUtils.IDFromPos(new Vector2Int(blockPos.x + 1, blockPos.y + 1))  // BottomRight
//            };

//            for (int i = 0; i < ids.Length; ++i)
//            {
//                var pilar = structure.m_Pilars[ids[i]];
//                if (pilar == null || (pilar != null && pilar.Blocks.Count == 0))
//                    return list;
//            }

//            if (layer < 0)
//            {
//                for (int i = 0; i < ids.Length; ++i)
//                {
//                    BlockComponent nearestBlock = null;
//                    float lastHeightDiff = float.PositiveInfinity;
//                    for (int j = 0; j < structure.m_Pilars[ids[i]].Blocks.Count; ++j)
//                    {
//                        var block = structure.m_Pilars[ids[i]].Blocks[j];
//                        var hDiff = Mathf.Abs(block.Height - height);
//                        if (hDiff < lastHeightDiff)
//                        {
//                            nearestBlock = block;
//                            lastHeightDiff = hDiff;
//                        }
//                    }
//                    list[i] = nearestBlock;
//                }
//            }
//            else
//            {
//                for (int i = 0; i < ids.Length; ++i)
//                {
//                    for (int j = 0; j < structure.m_Pilars[ids[i]].Blocks.Count; ++j)
//                    {
//                        var block = structure.m_Pilars[ids[i]].Blocks[j];
//                        if (block.Height == height && block.Layer == layer)
//                        {
//                            list[i] = block;
//                            break;
//                        }
//                    }
//                }
//            }

//            for (int i = 0; i < list.Length; ++i)
//            {
//                if (list[i] == null)
//                    return new BlockComponent[3];
//            }

//            return list;
//        }

//        void UnapplyWideChange(int layer)
//        {
//            var layerInfo = m_InfoLayers[layer - 1];
//            var blocks = m_Layers[layer - 1];
//            var blocksIE = Structures.Strucs[IDXIE].Blocks;
//            for (int i = 0; i < blocks.Count; ++i)
//            {
//                var block = blocks[i];
//                if (block.blockType != BlockType.WIDE)
//                    continue;

//                if (blocksIE[block.IDXIE].blockType == BlockType.WIDE)
//                    continue;

//                var wblocks = GetNearBlocks(this, block.Pilar.StructureID, block.Height, layer);
//                if (wblocks[0] == null)
//                    throw new Exception("Couldn't find the hidden blocks while returning a block from wide to normal.");
//                block.blockType = BlockType.NORMAL;
//                //block.Layer = 0;
//                //block.Layer = layer;
//                //if (block.PropGO != null)
//                //    block.PropGO.GetComponent<PropScript>().BlockOffset = 0.5f;
//                //if (block.MonsterGO != null)
//                //    block.MonsterGO.GetComponent<MonsterScript>().BlockOffset = 0.5f;
//                //ApplyLayerToBlock(layer, block);

//                //for (int j = 0; j < wblocks.Length; ++j)
//                //{
//                //    var cblock = wblocks[j];
//                //    cblock.TopMR.enabled = true;
//                //    cblock.TopBC.enabled = true;
//                //    cblock.MidMR.enabled = true;
//                //    cblock.MidBC.enabled = true;

//                //    cblock.LayerSR.enabled = true;

//                //    if (cblock.Anchor)
//                //        cblock.AnchorSR.enabled = true;

//                //    if (cblock.Stair == StairType.POSSIBLE)
//                //        cblock.StairSR.enabled = true;

//                //    //if (cblock.PropGO != null)
//                //    //    cblock.PropGO.GetComponent<SpriteRenderer>().enabled = true;

//                //    //if (cblock.MonsterGO != null)
//                //    //    cblock.MonsterGO.GetComponent<SpriteRenderer>().enabled = true;

//                //    cblock.Removed = false;
//                //    //ApplyLayerToBlock(layer, cblock);
//                //}
//            }
//        }

//        void ApplyWideChange(int layer)
//        {
//            var layerInfo = m_InfoLayers[layer - 1];
//            if (layerInfo.RandomWideBlockChance == 0.0f)
//                return;
//            var blocks = m_Layers[layer - 1];
//            Dictionary<int, List<BlockComponent>> possibleWideBlocks = new Dictionary<int, List<BlockComponent>>();
//            for (int i = 0; i < blocks.Count; ++i)
//            {
//                var block = blocks[i];
//                var family = block.MaterialFmly;
//                if(family.WideMaterials.Length > 0)
//                {
//                    var familyID = BlockMaterial.FamilyDict[family.FamilyName];
//                    if (!possibleWideBlocks.ContainsKey(familyID))
//                        possibleWideBlocks.Add(familyID, new List<BlockComponent>());
//                    var list = possibleWideBlocks[familyID];
//                    list.Add(block);
//                    possibleWideBlocks[familyID] = list;
//                }
//                //var matType = BlockMaterial.MaterialTypes[block.MaterialTypeID];
//                //if (matType.Def.Materials[(int)BlockType.WIDE].Count > 0)
//                //{
//                //    if (!possibleWideBlocks.ContainsKey(block.MaterialTypeID))
//                //        possibleWideBlocks.Add(block.MaterialTypeID, new List<BlockComponent>());
//                //    var list = possibleWideBlocks[block.MaterialTypeID];
//                //    list.Add(block);
//                //    possibleWideBlocks[block.MaterialTypeID] = list;
//                //}
//            }
//            for (int i = 0; i < possibleWideBlocks.Count; ++i)
//            {
//                var wblocksKP = possibleWideBlocks.ElementAt(i);
//                //var wblocksKP = possibleWideBlocks.ElementAt(0);
//                if (wblocksKP.Value.Count < 4)
//                    /*return;*/
//                    continue;


//                for (int j = 0; j < wblocksKP.Value.Count; ++j)
//                {
//                    var block = wblocksKP.Value[j];
//                    //var block = wblocksKP.Value[1];
//                    if (block.Stair == StairType.ALWAYS || block.blockType != BlockType.NORMAL || block.Locked == BlockLock.Locked || block.Removed)
//                        /*return;*/
//                        continue;

//                    var chance = UnityEngine.Random.value; //(float)Manager.Mgr.BuildRNG.NextDouble();
//                    if (chance > layerInfo.RandomWideBlockChance)
//                        /*return;*/
//                        continue;

//                    var nearblocks = GetNearBlocks(this, block.Pilar.StructureID, block.Height, layer);
//                    if (nearblocks[0] == null)
//                        /*return;*/
//                        continue;
//                    bool valid = true;
//                    for (int k = 0; k < nearblocks.Length; ++k)
//                    {
//                        var nearBlockFamilyID = BlockMaterial.FamilyDict[nearblocks[k].MaterialFmly.FamilyName];
//                        if (nearBlockFamilyID != wblocksKP.Key || nearblocks[k].Stair == StairType.ALWAYS || nearblocks[k].Removed || nearblocks[k].blockType == BlockType.WIDE
//                            || nearblocks[k].Locked == BlockLock.Locked)
//                        {
//                            valid = false;
//                            break;
//                        }
//                    }
//                    if (!valid)
//                        /*return;*/
//                        continue; // check next block

//                    block.blockType = BlockType.WIDE;
//                    block.SetWIDE(nearblocks);
//                    //if (block.PropGO != null)
//                    //    block.PropGO.GetComponent<PropScript>().BlockOffset = 1.0f;
//                    //if (block.MonsterGO != null)
//                    //    block.MonsterGO.GetComponent<MonsterScript>().BlockOffset = 1.0f;
//                    //for (int k = 0; k < nearblocks.Length; ++k)
//                    //{
//                    //    //Layers[wblocks[k].Layer - 1].Remove(wblocks[k]);
//                    //    //wblocks[k].Layer = 0;
//                    //    var nblock = nearblocks[k];
//                    //    nblock.TopMR.enabled = false;
//                    //    nblock.TopBC.enabled = false;
//                    //    nblock.MidMR.enabled = false;
//                    //    nblock.MidBC.enabled = false;

//                    //    nblock.LayerSR.enabled = false;

//                    //    if (nblock.Anchor)
//                    //        nblock.AnchorSR.enabled = false;

//                    //    if (nblock.Stair != StairType.NONE)
//                    //        nblock.StairSR.enabled = false;

//                    //    //if (cblock.PropGO != null)
//                    //    //    cblock.PropGO.GetComponent<SpriteRenderer>().enabled = false;

//                    //    //if (cblock.MonsterGO != null)
//                    //    //    cblock.MonsterGO.GetComponent<SpriteRenderer>().enabled = false;

//                    //    nblock.Height = block.Height;
//                    //    nblock.MicroHeight = block.MicroHeight;

//                    //    nblock.Removed = true;
//                    //}
//                }
//            }
//        }

//        void UnapplyStairChange(int layer)
//        {
//            var layerInfo = m_InfoLayers[layer - 1];
//            if (layerInfo.RandomStairBlockChance == 0.0f)
//                return;

//            var blocks = m_Layers[layer - 1];
//            for (int i = 0; i < blocks.Count; ++i)
//            {
//                var block = blocks[i];
//                if (block.Stair != StairType.POSSIBLE || block.blockType != BlockType.STAIRS)
//                    continue;

//                block.blockType = BlockType.NORMAL;

//                var li = layerInfo;
//                if (layerInfo.IsLinkedLayer)
//                {
//                    int selectedLayer = 0;

//                    float llChance = UnityEngine.Random.value; //(float)Manager.Mgr.BuildRNG.NextDouble();
//                    float llAccum = 0.0f;
//                    for (int j = 0; j < layerInfo.LinkedLayers.Count; ++j)
//                    {
//                        float nextChance = llAccum + layerInfo.LinkedLayerChances[j];

//                        if (llChance > llAccum && llChance < nextChance)
//                        {
//                            selectedLayer = layerInfo.LinkedLayers[j];
//                            break;
//                        }
//                        llAccum = nextChance;
//                    }
//                    li = m_InfoLayers[selectedLayer - 1];
//                }

//                var familyID = li.MaterialTypes[UnityEngine.Random.Range(0, li.MaterialTypes.Count) /*Manager.Mgr.BuildRNG.Next(0, li.MaterialTypes.Count)*/];
//                block.MaterialFmly = BlockMaterial.MaterialFamilies[familyID];

//                if (block.Prop != null)
//                {
//                    block.Prop.enabled = true;
//                }
//                if (block.Monster != null)
//                {
//                    block.Monster.enabled = true;
//                }
//            }
//        }

//        void ApplyStairChange(int layer)
//        {
//            var layerInfo = m_InfoLayers[layer - 1];
//            if (layerInfo.RandomStairBlockChance == 0.0f)
//                return;

//            var blocks = m_Layers[layer - 1];
//            for (int i = 0; i < blocks.Count; ++i)
//            {
//                var block = blocks[i];
//                if (block.Stair != StairType.POSSIBLE || block.Removed)
//                    continue;

//                var chance = UnityEngine.Random.value; //(float)Manager.Mgr.BuildRNG.NextDouble();
//                if (chance > layerInfo.RandomStairBlockChance)
//                    continue;

//                block.blockType = BlockType.STAIRS;
//                if (block.Prop != null)
//                {
//                    block.Prop.enabled = false;
//                }
//                if (block.Monster != null)
//                {
//                    block.Monster.enabled = false;
//                }
//            }
//        }

//        public void SetLayer(int layer, LayerInfo info, bool ignoreLock = false)
//        {
//            m_InfoLayers[layer - 1] = info;
//            if (!m_InfoLayers[layer - 1].IsValid())
//                return;

//            var blocks = m_Layers[layer - 1];
//            int idx = UnityEngine.Random.Range(0, blocks.Count); //Manager.Mgr.BuildRNG.Next(0, blocks.Count);

//            //for (int i = 0; i < LivingEntities.Count; ++i)
//            //{
//            //    var le = LivingEntities[i];
//            //    if(le.GetLEType() == Def.LivingEntityType.Prop)
//            //    {
//            //        var prop = (PropScript)le;
//            //        prop.Block.Prop = null;
//            //    }
//            //    if (le == null)
//            //        continue;
//            //    le.ReceiveDamage(Def.DamageType.UNAVOIDABLE, le.GetTotalHealth());
//            //}
//            //LivingEntities.Clear();

//            for (int i = idx; i < blocks.Count; ++i)
//                ApplyLayerToBlock(layer, blocks[i], ignoreLock);

//            for (int i = 0; i < idx; ++i)
//                ApplyLayerToBlock(layer, blocks[i], ignoreLock);
//        }

//        public void AddBlockToLayer(int layer, BlockComponent block)
//        {
//            if (m_Layers[layer - 1].Contains(block))
//                return;

//            m_Layers[layer - 1].Add(block);
//            if (!m_InfoLayers[layer - 1].IsValid())
//                return;

//            ApplyLayerToBlock(layer, block);
//        }

//        public void ApplyStairs(bool apply)
//        {
//            for (int i = 0; i < LayerAmount; ++i)
//            {
//                if (!InfoLayers[i].IsValid())
//                    continue;
//                if (apply)
//                    ApplyStairChange(i + 1);
//                else
//                    UnapplyStairChange(i + 1);
//            }
//        }

//        public void ApplyWides(bool apply)
//        {
//            for (int i = 0; i < LayerAmount; ++i)
//            {
//                if (!InfoLayers[i].IsValid())
//                    continue;
//                if (apply)
//                    ApplyWideChange(i + 1);
//                else
//                    UnapplyWideChange(i + 1);
//            }
//        }

//        void InterchangePilars(int leftPilar, int rightPilar)
//        {
//            var lPilar = m_Pilars[leftPilar];
//            var rPilar = m_Pilars[rightPilar];

//            //// Change IDs
//            //for (int i = 0; i < lBlock.BlockDefs.Count; ++i)
//            //{
//            //    var block = lBlock.BlockDefs[i];
//            //    block._SetCellID(rightBlock);
//            //    // Changing the rotation, changes the position and rotation based on the cell ID
//            //    var rot = block.GetRotation();
//            //    if (rot != BlockRotation.Default)
//            //        block.SetRotation(BlockRotation.Default);
//            //    else
//            //        block.SetRotation(BlockRotation.Left);
//            //    block.SetRotation(rot);
//            //    lBlock.BlockDefs[i] = block;
//            //}
//            //for (int i = 0; i < rBlock.BlockDefs.Count; ++i)
//            //{
//            //    var block = rBlock.BlockDefs[i];
//            //    block._SetCellID(leftBlock);
//            //    // Changing the rotation, changes the position and rotation based on the cell ID
//            //    var rot = block.GetRotation();
//            //    if (rot != BlockRotation.Default)
//            //        block.SetRotation(BlockRotation.Default);
//            //    else
//            //        block.SetRotation(BlockRotation.Left);
//            //    block.SetRotation(rot);
//            //    rBlock.BlockDefs[i] = block;
//            //}

//            lPilar.StructureID = rightPilar;
//            rPilar.StructureID = leftPilar;

//            m_Pilars[leftPilar] = rPilar;
//            m_Pilars[rightPilar] = lPilar;
//        }

//        void FixWides()
//        {
//            List<BlockComponent> fixedWides = new List<BlockComponent>();
//            for (int i = 0; i < m_Pilars.Length; ++i)
//            {
//                var pilar = m_Pilars[i];
//                for (int j = 0; j < pilar.Blocks.Count; ++j)
//                {
//                    var block = pilar.Blocks[j];
//                    if (block.blockType != BlockType.WIDE)
//                        continue;
//                    if (fixedWides.Contains(block))
//                        continue;
//                    BlockComponent[] blocks = new BlockComponent[4];
//                    blocks[0] = block;
//                    int lastIdx = 1;
//                    foreach (var hBlock in block.HiddenBlocks)
//                        blocks[lastIdx++] = hBlock;

//                    Vector2Int minPosBlock = new Vector2Int(int.MaxValue, int.MaxValue);
//                    BlockComponent newWide = null;
//                    foreach (var hblock in blocks)
//                    {
//                        var pos = GameUtils.PosFromID(hblock.Pilar.StructureID);
//                        if (minPosBlock.x >= pos.x && minPosBlock.y >= pos.y)
//                        {
//                            minPosBlock = pos;
//                            newWide = hblock;
//                        }
//                    }
//                    lastIdx = 0;
//                    BlockComponent[] hiddenBlocks = new BlockComponent[3];
//                    foreach (var hblock in blocks)
//                    {
//                        if (hblock != newWide)
//                            hiddenBlocks[lastIdx++] = hblock;
//                    }

//                    block.blockType = BlockType.NORMAL;
//                    newWide.SetWIDE(hiddenBlocks);
//                    fixedWides.Add(newWide);
//                }
//            }
//        }

//        void HFlip()
//        {
//            void RotateBlocks(PilarComponent pilar)
//            {
//                foreach (var block in pilar.Blocks)
//                {
//                    if (block.Rotation == BlockRotation.Left)
//                        block.Rotation = BlockRotation.Right;
//                    else if (block.Rotation == BlockRotation.Right)
//                        block.Rotation = BlockRotation.Left;
//                }
//            }
//            for (int y = 0; y < (Height / 2); ++y)
//            {
//                for (int x = 0; x < Width; ++x)
//                {
//                    int nY = (Height - 1) - y; // Flipped Y
//                    int currentID = GameUtils.IDFromPos(new Vector2Int(x, y));
//                    int flippedID = GameUtils.IDFromPos(new Vector2Int(x, nY));
//                    InterchangePilars(currentID, flippedID);
//                    RotateBlocks(m_Pilars[currentID]);
//                    RotateBlocks(m_Pilars[flippedID]);
//                }
//            }

//            // Fix Wides
//            FixWides();
//        }

//        void VFlip()
//        {
//            void RotateBlocks(PilarComponent pilar)
//            {
//                foreach (var block in pilar.Blocks)
//                {
//                    if (block.Rotation == BlockRotation.Default)
//                        block.Rotation = BlockRotation.Half;
//                    else if (block.Rotation == BlockRotation.Half)
//                        block.Rotation = BlockRotation.Default;
//                }
//            }
//            for (int y = 0; y < Height; ++y)
//            {
//                for (int x = 0; x < (Width / 2); ++x)
//                {
//                    int nX = (Width - 1) - x; // Flipped X
//                    int currentID = GameUtils.IDFromPos(new Vector2Int(x, y));
//                    int flippedID = GameUtils.IDFromPos(new Vector2Int(nX, y));
//                    InterchangePilars(currentID, flippedID);
//                    RotateBlocks(m_Pilars[currentID]);
//                    RotateBlocks(m_Pilars[flippedID]);
//                }
//            }

//            // Fix Wides
//            FixWides();
//        }

//        void Rotation90()
//        {
//            void RotateBlocks(PilarComponent pilar)
//            {
//                foreach (var block in pilar.Blocks)
//                {
//                    switch (block.Rotation)
//                    {
//                        case BlockRotation.Default:
//                            block.Rotation = BlockRotation.Right;
//                            break;
//                        case BlockRotation.Left:
//                            block.Rotation = BlockRotation.Default;
//                            break;
//                        case BlockRotation.Half:
//                            block.Rotation = BlockRotation.Left;
//                            break;
//                        case BlockRotation.Right:
//                            block.Rotation = BlockRotation.Half;
//                            break;
//                    }
//                }
//            }
//            for (int x = 0; x < Width / 2; ++x)
//            {
//                for(int y = x; y < Height - x  - 1; ++y)
//                {
//                    var tID = GameUtils.IDFromPos(new Vector2Int(x, y));
//                    var rID = GameUtils.IDFromPos(new Vector2Int(y, Width - 1 - x));
//                    var bID = GameUtils.IDFromPos(new Vector2Int(Width - 1 - x, Height - 1 - y));
//                    var lID = GameUtils.IDFromPos(new Vector2Int(Height - 1 - y, x));

//                    var tlPilar = m_Pilars[tID];
//                    var rtPilar = m_Pilars[rID];
//                    var brPilar = m_Pilars[bID];
//                    var lbPilar = m_Pilars[lID];

//                    m_Pilars[lID] = tlPilar;
//                    tlPilar.StructureID = lID;
//                    RotateBlocks(tlPilar);

//                    m_Pilars[tID] = rtPilar;
//                    rtPilar.StructureID = tID;
//                    RotateBlocks(rtPilar);

//                    m_Pilars[rID] = brPilar;
//                    brPilar.StructureID = rID;
//                    RotateBlocks(brPilar);

//                    m_Pilars[bID] = lbPilar;
//                    lbPilar.StructureID = bID;
//                    RotateBlocks(lbPilar);
//                }
//            }

//            // Fix Wides
//            FixWides();
//        }

//        public StructureComponent()
//        {
//            m_IDXIE = -1;
//            m_InfoLayers = new LayerInfo[LayerAmount];
//            m_Layers = new List<BlockComponent>[LayerAmount];
//            for (int i = 0; i < m_Layers.Length; ++i)
//                m_Layers[i] = new List<BlockComponent>();
//            for (int i = 0; i < m_InfoLayers.Length; ++i)
//            {
//                var layer = LayerInfo.GetDefaultLayer();
//                layer.Layer = i + 1;
//                m_InfoLayers[i] = layer;
//            }

//            m_Pilars = new PilarComponent[Width * Height];
//            m_Flip = StructureFlip.NoFlip;
//            m_Rotation = BlockRotation.Default;
//            LivingEntities = new List<LivingEntity>();
//        }

//        public void DestroyStructure()
//        {
//            foreach(var pilar in m_Pilars)
//            {
//                pilar.DestroyPilar();
//            }
//            if (Manager.Mgr.Strucs.Contains(this))
//                Manager.Mgr.Strucs.Remove(this);
//            if (Manager.Mgr.Structure == this)
//                Manager.Mgr.Structure = null;
//            GameUtils.DeleteGameObjectAndItsChilds(gameObject);
//        }

//        private void Start()
//        {
            
//        }

//        private void OnEnable()
//        {
//            if (m_Pilars == null)
//                return;
//            for(int i = 0; i < m_Pilars.Length; ++i)
//            {
//                var pilar = m_Pilars[i];
//                if (pilar == null)
//                    continue;
//                pilar.enabled = true;
//            }
//        }

//        private void OnDisable()
//        {
//            if (m_Pilars == null)
//                return;
//            for (int i = 0; i < m_Pilars.Length; ++i)
//            {
//                var pilar = m_Pilars[i];
//                if (pilar == null)
//                    continue;
//                pilar.enabled = false;
//            }
//        }
//    }
//}
