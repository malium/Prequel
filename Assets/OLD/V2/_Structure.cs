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
//using cakeslice;
//using System.Collections;
//using System.Windows.Forms;

//namespace Assets
//{
//    public enum StructureFlip
//    {
//        NoFlip,
//        VerticalFlip,
//        HorizontalFlip,
//        VerticalAndHorizontalFlip,
//        COUNT
//    }

//    public struct LayerInfo
//    {
//        public int Layer;
        
//        public List<int> MaterialTypes;
//        public List<float> MaterialTypeChances;

//        public bool EnableRandomRotation;
//        public BlockRotation DefaultRotation;

//        public float BlockHeight;

//        public bool EnableRandomMicroHeight;
//        public float BlockMicroHeightMin;
//        public float BlockMicroHeightMax;

//        public bool EnableRandomBlockLength;
//        public float BlockLengthMin;
//        public float BlockLengthMax;

//        public bool EnableRandomProps;
//        public float PropChance;
//        public List<int> Props;
//        public List<float> PropChances;
//        public float PropNoSpawnRadius;

//        public bool SpawnMonsters;
//        public bool SpawnZoneMonsters;
//        public bool LayerMonstersRespawn;
//        public float MonsterChance;
//        public List<int> Monsters;
//        public List<float> MonsterChances;

//        public bool HasEffects;
//        public List<int> Effects;

//        public bool EnableBlockFloat;
//        public float BlockFloatMin;
//        public float BlockFloatMax;
//        public float BlockFloatSpeed;

//        public float RandomWideBlockChance;

//        public float RandomStairBlockChance;

//        public bool IsLinkedLayer;
//        public List<int> LinkedLayers;
//        public List<float> LinkedLayerChances;
        
//        public bool IsValid()
//        {
//            if (Layer == 0 || Layer > Structure.LayerAmount)
//                return false;
//            if(IsLinkedLayer)
//            {
//                if (LinkedLayers.Count < 2)
//                    return false;
//            }
//            else
//            {
//                if (MaterialTypes.Count == 0)
//                    return false;
//                if (MaterialTypes.Count != MaterialTypeChances.Count)
//                    return false;
//                if (EnableRandomProps && Props.Count == 0)
//                    return false;
//                if (PropChances.Count != Props.Count)
//                    return false;
//                if (SpawnMonsters && Monsters.Count == 0)
//                    return false;
//                if (MonsterChances.Count != Monsters.Count)
//                    return false;
//                if (HasEffects && Effects.Count == 0)
//                    return false;
//                if (!EnableRandomMicroHeight && (BlockMicroHeightMax != BlockMicroHeightMin))
//                    return false;
//                if (!EnableRandomBlockLength && (BlockLengthMin != BlockLengthMax))
//                    return false;
//            }
            
//            return true;
//        }

//        public void ToLayerIE(ref LayerIE ie)
//        {
//            ie.Layer = (byte)Layer;
//            ie.IsLinkedLayer = IsLinkedLayer;

//            ie.EnableRandomRotation = EnableRandomRotation;
//            ie.DefaultBlockRotation = DefaultRotation;

//            var subMats = ie.SubMaterials;
            
//            ie.SubMaterials = MaterialTypes;
//            ie.SubMaterialChances = MaterialTypeChances;

//            ie.BlockHeight = BlockHeight;

//            ie.EnableRandomMicroHeight = EnableRandomMicroHeight;
//            ie.BlockMicroHeightMin = BlockMicroHeightMin;
//            ie.BlockMicroHeightMax = BlockMicroHeightMax;

//            ie.EnableRandomBlockLenght = EnableRandomBlockLength;
//            ie.BlockLenghtMin = BlockLengthMin;
//            ie.BlockLenghtMax = BlockLengthMax;

//            ie.EnableRandomProps = EnableRandomProps;
//            ie.PropChance = PropChance;
//            ie.Props = Props;
//            ie.PropChances = PropChances;
//            ie.PropNoSpawnRadius = PropNoSpawnRadius;

//            ie.SpawnMonsters = SpawnMonsters;
//            ie.SpawnZoneMonsters = SpawnZoneMonsters;
//            ie.LayerMonstersRespawn = LayerMonstersRespawn;
//            ie.Monsters = Monsters;
//            ie.MonsterChances = MonsterChances;
//            ie.MonsterChance = MonsterChance;

//            ie.HasEffect = HasEffects;
//            ie.Effects = Effects;

//            ie.EnableBlockFloat = EnableBlockFloat;
//            ie.BlockFloatMin = BlockFloatMin;
//            ie.BlockFloatMax = BlockFloatMax;
//            ie.BlockFloatSpeed = BlockFloatSpeed;

//            ie.RandomWideBlockChance = RandomWideBlockChance;
//            ie.RandomStairBlockChance = RandomStairBlockChance;

//            ie.LinkedLayers = LinkedLayers;
//            ie.LinkedLayerChances = LinkedLayerChances;
//        }

//        public static LayerInfo GetDefaultLayer()
//        {
//            LayerInfo layer;
//            layer.Layer = 0;
//            layer.MaterialTypes = new List<int>();
//            layer.MaterialTypeChances = new List<float>();
//            layer.EnableRandomRotation = true;
//            layer.DefaultRotation = BlockRotation.Default;
//            layer.BlockHeight = 0.0f;
//            layer.EnableRandomMicroHeight = false;
//            layer.BlockMicroHeightMin = 0.0f;
//            layer.BlockMicroHeightMax = 0.0f;
//            layer.EnableRandomBlockLength = false;
//            layer.BlockLengthMin = 1.0f;
//            layer.BlockLengthMax = 1.0f;
//            layer.EnableRandomProps = false;
//            layer.Props = new List<int>();
//            layer.PropChance = 0.5f;
//            layer.PropChances = new List<float>();
//            layer.PropNoSpawnRadius = 1;
//            layer.SpawnMonsters = false;
//            layer.SpawnZoneMonsters = true;
//            layer.LayerMonstersRespawn = false;
//            layer.Monsters = new List<int>();
//            layer.MonsterChance = 0.5f;
//            layer.MonsterChances = new List<float>();
//            layer.HasEffects = false;
//            layer.Effects = new List<int>();
//            layer.EnableBlockFloat = false;
//            layer.BlockFloatMin = 0.0f;
//            layer.BlockFloatMax = 0.0f;
//            layer.BlockFloatSpeed = 0.0f;
//            layer.RandomWideBlockChance = 0.25f;
//            layer.RandomStairBlockChance = 0.50f;
//            layer.IsLinkedLayer = false;
//            layer.LinkedLayers = new List<int>();
//            layer.LinkedLayerChances = new List<float>();
//            return layer;
//        }
//    }

//    public class Structure
//    {
//        private static int NextStructID = 0;
//        // Number of blocks in the x direction
//        public const int Width = 8;
//        // Number of blocks in the z direction
//        public const int Height = 8;
//        // Separation of each block between the others
//        public const float Separation = 0.02f;
//        // Amount of layers that can be at the same time
//        public const int LayerAmount = 8;
//        //
//        public List<GameObject> PropsInStruc;
//        public List<GameObject> MonstersInStruc;

//        //public StructureIE IE;
//        private int _IDXIE;
//        public int IDXIE
//        {
//            get
//            {
//                return _IDXIE;
//            }
//            set
//            {
//                _IDXIE = value;
//                if (m_StructGO != null)
//                    m_StructGO.name = $"Structure_{_IDXIE}";
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
//        private LayerInfo[] m_InfoLayers;
//        // What blocks currently belong to each layer
//        public List<Block>[] Layers
//        {
//            get
//            {
//                return m_Layers;
//            }
//        }
//        private List<Block>[] m_Layers;
//        // GameObject that holds the position and rotation of the structure
//        public GameObject GO
//        {
//            get
//            {
//                return m_StructGO;
//            }
//        }
//        private GameObject m_StructGO;
//        // Array of blocks, all the blocks from the structure are here and can be accessed via their ID
//        public Pilar[] Pilars
//        {
//            get
//            {
//                return m_Pilars;
//            }
//        }
//        private Pilar[] m_Pilars;
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
//        private StructureFlip m_Flip;
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
//        private BlockRotation m_Rotation;

//        private void ApplyLayerToBlock(int layer, Block block, bool ignoreLock = false)
//        {
//            var infoLayer = InfoLayers[layer - 1];
//            if(infoLayer.IsLinkedLayer)
//            {
//                var rng = Manager.Mgr.RNG;
//                float chance = (float)rng.NextDouble();
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

//        private void ApplyLayerToBlock(LayerInfo layer, Block block, bool ignoreLock = false, int linkedTo = -1)
//        {
//            if (block.Locked == BlockLock.Locked && !ignoreLock)
//                return;

//            var rng = Manager.Mgr.RNG;
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
//                flags = new BitArray((int)BlockIE.Flag.COUNT);
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
//            if(!flags[(int)BlockIE.Flag.MaterialType])
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
//                    var matType = BlockMaterial.MaterialTypes[layer.MaterialTypes[j]];
//                    if (matType.Def.Materials[(int)block.blockType].Count > 0)
//                    {
//                        availableSubmaterials.Add(matType.MaterialSubID);
//                        availableSubmaterialChances.Add(layer.MaterialTypeChances[j]);
//                    }
//                }
//                GameUtils.UpdateChances(ref availableSubmaterialChances);

//                float chance = (float)Manager.Mgr.RNG.NextDouble();
//                float accum = 0.0f;
//                for (int j = 0; j < availableSubmaterialChances.Count; ++j)
//                {
//                    float nextChance = accum + availableSubmaterialChances[j];

//                    if (chance > accum && chance < nextChance)
//                    {
//                        block.MaterialTypeID = availableSubmaterials[j];
//                        break;
//                    }
//                    accum = nextChance;
//                }
//            }

//            // Rotation
//            if (!flags[(int)BlockIE.Flag.Rotation])
//            {
//                if(layer.EnableRandomRotation)
//                {
//                    float chance = (float)rng.NextDouble() * (float)BlockRotation.COUNT;
//                    block.Rotation = (BlockRotation)Mathf.FloorToInt(chance);
//                }
//                else
//                {
//                    block.Rotation = layer.DefaultRotation;
//                }
//            }

//            // Default height
//            if (!flags[(int)BlockIE.Flag.Height])
//            {
//                block.Height = layer.BlockHeight;
//            }

//            // Block height
//            {
//                if (layer.EnableRandomMicroHeight)
//                {
//                    float chance = (float)rng.NextDouble() * (layer.BlockMicroHeightMax - layer.BlockMicroHeightMin) + layer.BlockMicroHeightMin;
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
//            if (!flags[(int)BlockIE.Flag.Length])
//            {
//                if(layer.EnableRandomBlockLength)
//                {

//                    float chance = (float)rng.NextDouble() * (layer.BlockLengthMax - layer.BlockLengthMin) + layer.BlockLengthMin;
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
//            if (!flags[(int)BlockIE.Flag.Monster])
//            {
//                block.MonsterGO = null;
//                if(layer.SpawnMonsters)
//                {
//                    float chance = (float)rng.NextDouble();
//                    if (chance < layer.MonsterChance)
//                    {
//                        chance = (float)rng.NextDouble();
//                        float accum = 0.0f;
//                        for (int i = 0; i < layer.Monsters.Count; ++i)
//                        {
//                            float nextChance = accum + layer.MonsterChances[i];

//                            if (chance > accum && chance < nextChance)
//                            {
//                                var monster = Monsters.MonsterSprites[layer.Monsters[i]];
//                                var mon = new GameObject($"Monster_{MonsterScript.MonsterID++}");
//                                if (block.MonsterGO != null)
//                                    block.MonsterGO.GetComponent<MonsterScript>().TakeDamage(100.0f);
//                                block.MonsterGO = mon;
//                                mon.transform.Translate(block.pilar.GO.transform.position, Space.World);
//                                mon.transform.Translate(new Vector3(0.0f, block.Height + block.MicroHeight, 0.0f), Space.World);
//                                var smon = mon.AddComponent<MonsterScript>();
//                                smon.SetMonster(layer.Monsters[i]);
//                                smon.SpriteRnd.enabled = false;
//                                smon.ShadowGO.GetComponent<SpriteRenderer>().enabled = false;
//                                smon.Struc = this;
//                                //block.MonsterGO.GetComponent<SpriteRenderer>().enabled = false;
//                                var facing = smon.Facing;
//                                float nChance = (float)rng.NextDouble();
//                                if (nChance >= 0.5f)
//                                    facing.Horizontal = SpriteHorizontal.RIGHT;
//                                nChance = (float)rng.NextDouble();
//                                if (nChance >= 0.5f)
//                                    facing.Vertical = SpriteVertical.UP;
//                                smon.Facing = facing;
//                                break;
//                            }
//                            accum = nextChance;
//                        }
//                    }
//                }
                
//            }
//            if (!flags[(int)BlockIE.Flag.Prop])
//            {
//                bool spawnProp = block.MonsterGO == null;
//                // Prop NoSpawn radius
//                {
//                    if (layer.PropNoSpawnRadius > 0 && spawnProp)
//                    {
//                        List<Block> layerBlocks = null;
//                        if (linkedTo != -1)
//                            layerBlocks = m_Layers[linkedTo - 1];
//                        else
//                            layerBlocks = m_Layers[layer.Layer - 1];
//                        for (int i = 0; i < layerBlocks.Count; ++i)
//                        {
//                            if (layerBlocks[i] == block || layerBlocks[i].PropGO == null)
//                                continue;

//                            var oid = layerBlocks[i].pilar.StructureID;
//                            var mid = block.pilar.StructureID;

//                            var oPos = GameUtils.PosFromID(oid, Structure.Width, Structure.Height);
//                            var mPos = GameUtils.PosFromID(mid, Structure.Width, Structure.Height);

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
//                    block.PropGO = null;
//                    if (layer.EnableRandomProps && spawnProp && block.blockType != BlockType.STAIRS)
//                    {
//                        float chance = (float)rng.NextDouble();
//                        if (chance < layer.PropChance)
//                        {
//                            chance = (float)rng.NextDouble();
//                            float accum = 0.0f;
//                            for (int i = 0; i < layer.Props.Count; ++i)
//                            {
//                                float nextChance = accum + layer.PropChances[i];

//                                if (chance > accum && chance < nextChance)
//                                {
//                                    var propTypeID = layer.Props[i];
//                                    var propType = Props.PropTypes[propTypeID];
//                                    if (propType.Props.Count == 0)
//                                        throw new Exception($"This prop {propType.ProbTypeNames[0]}, does not have any available prop.");
//                                    var propID = rng.Next(0, propType.Props.Count);
//                                    var prop = new GameObject($"Prop_{PropScript.PropID++}");
//                                    if (block.PropGO != null)
//                                        block.PropGO.GetComponent<PropScript>().TakeDamage(100.0f);
//                                    block.PropGO = prop;
//                                    //prop.transform.Translate(block.pilar.GO.transform.position, Space.World);
//                                    //prop.transform.Translate(new Vector3(0.0f, block.Height + block.MicroHeight, 0.0f), Space.World);
//                                    var sprop = prop.AddComponent<PropScript>();
//                                    sprop.Block = block;
//                                    sprop.SetProp(Props.PropList[propType.Props[propID]].PropID);
//                                    sprop.SpriteGO.GetComponent<SpriteRenderer>().enabled = false;
//                                    sprop.ShadowGO.GetComponent<SpriteRenderer>().enabled = false;
//                                    //block.PropGO.GetComponent<SpriteRenderer>().enabled = false;
//                                    //sprop.ShadowGO.GetComponent<SpriteRenderer>().enabled = false;
//                                    float nChance = (float)rng.NextDouble();
//                                    var facing = sprop.Facing;
//                                    if (nChance >= 0.5f)
//                                        facing.Horizontal = SpriteHorizontal.RIGHT;
//                                    sprop.Facing = facing;
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

//        public static Block[] GetNearBlocks(Structure structure, int structID, float height, int layer)
//        {
//            var list = new Block[3];
//            var blockPos = GameUtils.PosFromID(structID, Structure.Width, Structure.Height);
//            if (blockPos.x >= (Structure.Width - 1) || blockPos.y >= (Structure.Height - 1) || structure == null)
//                return list;

//            int[] ids = new int[3]
//            {
//                GameUtils.IDFromPos(new Vector2Int(blockPos.x + 1, blockPos.y), Structure.Width, Structure.Height),     // Right
//                GameUtils.IDFromPos(new Vector2Int(blockPos.x, blockPos.y + 1), Structure.Width, Structure.Height),     // BottomLeft
//                GameUtils.IDFromPos(new Vector2Int(blockPos.x + 1, blockPos.y + 1), Structure.Width, Structure.Height)  // BottomRight
//            };

//            for(int i = 0; i < ids.Length; ++i)
//            {
//                var pilar = structure.Pilars[ids[i]];
//                if (pilar == null || (pilar != null && pilar.Blocks.Count == 0))
//                    return list;
//            }

//            if (layer < 0)
//            {
//                for(int i = 0; i < ids.Length; ++i)
//                {
//                    Block nearestBlock = null;
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
//                    return new Block[3];
//            }

//            return list;
//        }

//        private void UnapplyWideChange(int layer)
//        {
//            var layerInfo = m_InfoLayers[layer - 1];
//            var blocks = m_Layers[layer - 1];
//            var blocksIE = Structures.Strucs[IDXIE].Blocks;
//            for(int i = 0; i < blocks.Count; ++i)
//            {
//                var block = blocks[i];
//                if (block.blockType != BlockType.WIDE)
//                    continue;

//                if (blocksIE[block.IDXIE].blockType == BlockType.WIDE)
//                    continue;

//                var wblocks = GetNearBlocks(this, block.pilar.StructureID, block.Height, layer);
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

//                for (int j = 0; j < wblocks.Length; ++j)
//                {
//                    var cblock = wblocks[j];
//                    cblock.TopGO.GetComponent<MeshRenderer>().enabled = true;
//                    cblock.TopGO.GetComponent<BoxCollider>().enabled = true;
//                    cblock.MidGO.GetComponent<MeshRenderer>().enabled = true;
//                    //cblock.BotGO.GetComponent<MeshRenderer>().enabled = true;

//                    cblock.LayerGO.GetComponent<SpriteRenderer>().enabled = true;

//                    if (cblock.Anchor)
//                        cblock.AnchorGO.GetComponent<SpriteRenderer>().enabled = true;

//                    if (cblock.Stair == StairType.POSSIBLE)
//                        cblock.StairGO.GetComponent<SpriteRenderer>().enabled = true;

//                    //if (cblock.PropGO != null)
//                    //    cblock.PropGO.GetComponent<SpriteRenderer>().enabled = true;

//                    //if (cblock.MonsterGO != null)
//                    //    cblock.MonsterGO.GetComponent<SpriteRenderer>().enabled = true;

//                    cblock.Removed = false;
//                    //ApplyLayerToBlock(layer, cblock);
//                }
//            }
//        }

//        private void ApplyWideChange(int layer)
//        {
//            var layerInfo = m_InfoLayers[layer - 1];
//            if (layerInfo.RandomWideBlockChance == 0.0f)
//                return;
//            var blocks = m_Layers[layer - 1];
//            Dictionary<int, List<Block>> possibleWideBlocks = new Dictionary<int, List<Block>>();
//            for (int i = 0; i < blocks.Count; ++i)
//            {
//                var block = blocks[i];
//                var matType = BlockMaterial.MaterialTypes[block.MaterialTypeID];
//                if (matType.Def.Materials[(int)BlockType.WIDE].Count > 0)
//                {
//                    if (!possibleWideBlocks.ContainsKey(block.MaterialTypeID))
//                        possibleWideBlocks.Add(block.MaterialTypeID, new List<Block>());
//                    var list = possibleWideBlocks[block.MaterialTypeID];
//                    list.Add(block);
//                    possibleWideBlocks[block.MaterialTypeID] = list;
//                }
//            }
//            for (int i = 0; i < possibleWideBlocks.Count; ++i)
//            {
//                var wblocksKP = possibleWideBlocks.ElementAt(i);
//                //var wblocksKP = possibleWideBlocks.ElementAt(0);
//                if (wblocksKP.Value.Count < 4)
//                    /*return;*/ continue;


//                for (int j = 0; j < wblocksKP.Value.Count; ++j)
//                {
//                    var block = wblocksKP.Value[j];
//                    //var block = wblocksKP.Value[1];
//                    if (block.Stair == StairType.ALWAYS || block.blockType != BlockType.NORMAL || block.Locked == BlockLock.Locked || block.Removed)
//                        /*return;*/ continue;

//                    var chance = (float)Manager.Mgr.RNG.NextDouble();
//                    if (chance > layerInfo.RandomWideBlockChance)
//                        /*return;*/ continue;

//                    var nearblocks = GetNearBlocks(this, block.pilar.StructureID, block.Height, layer);
//                    if (nearblocks[0] == null)
//                        /*return;*/ continue;
//                    bool valid = true;
//                    for(int k = 0; k < nearblocks.Length; ++k)
//                    {
//                        if (nearblocks[k].MaterialTypeID != wblocksKP.Key || nearblocks[k].Stair == StairType.ALWAYS || nearblocks[k].Removed || nearblocks[k].blockType == BlockType.WIDE
//                            || nearblocks[k].Locked == BlockLock.Locked)
//                        {
//                            valid = false;
//                            break;
//                        }
//                    }
//                    if (!valid)
//                        /*return;*/ continue; // check next block

//                    block.blockType = BlockType.WIDE;
//                    //if (block.PropGO != null)
//                    //    block.PropGO.GetComponent<PropScript>().BlockOffset = 1.0f;
//                    //if (block.MonsterGO != null)
//                    //    block.MonsterGO.GetComponent<MonsterScript>().BlockOffset = 1.0f;
//                    for (int k = 0; k < nearblocks.Length; ++k)
//                    {
//                        //Layers[wblocks[k].Layer - 1].Remove(wblocks[k]);
//                        //wblocks[k].Layer = 0;
//                        var nblock = nearblocks[k];

//                        nblock.TopGO.GetComponent<MeshRenderer>().enabled = false;
//                        nblock.TopGO.GetComponent<BoxCollider>().enabled = false;
//                        nblock.MidGO.GetComponent<MeshRenderer>().enabled = false;
//                        //nblock.BotGO.GetComponent<MeshRenderer>().enabled = false;

//                        nblock.LayerGO.GetComponent<SpriteRenderer>().enabled = false;

//                        if (nblock.Anchor)
//                            nblock.AnchorGO.GetComponent<SpriteRenderer>().enabled = false;

//                        if (nblock.Stair != StairType.NONE)
//                            nblock.StairGO.GetComponent<SpriteRenderer>().enabled = false;

//                        //if (cblock.PropGO != null)
//                        //    cblock.PropGO.GetComponent<SpriteRenderer>().enabled = false;

//                        //if (cblock.MonsterGO != null)
//                        //    cblock.MonsterGO.GetComponent<SpriteRenderer>().enabled = false;

//                        nblock.Height = block.Height;
//                        nblock.MicroHeight = block.MicroHeight;

//                        nblock.Removed = true;
//                    }
//                }
//            }
//        }

//        private void UnapplyStairChange(int layer)
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

//                    float llChance = (float)Manager.Mgr.RNG.NextDouble();
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


//                block.MaterialTypeID = li.MaterialTypes[Manager.Mgr.RNG.Next(0, li.MaterialTypes.Count)];

//                if (block.PropGO != null)
//                {
//                    var sprop = block.PropGO.GetComponent<PropScript>();
//                    sprop.SpriteGO.GetComponent<SpriteRenderer>().enabled = true;
//                    sprop.ShadowGO.GetComponent<SpriteRenderer>().enabled = true;
//                }
//                if (block.MonsterGO != null)
//                {
//                    var smon = block.MonsterGO.GetComponent<MonsterScript>();
//                    smon.SpriteRnd.enabled = true;
//                    smon.ShadowGO.GetComponent<SpriteRenderer>().enabled = true;
//                }
//            }
//        }

//        private void ApplyStairChange(int layer)
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

//                var chance = (float)Manager.Mgr.RNG.NextDouble();
//                if (chance > layerInfo.RandomStairBlockChance)
//                    continue;

//                block.blockType = BlockType.STAIRS;
//                if (block.PropGO != null)
//                {
//                    var sprop = block.PropGO.GetComponent<PropScript>();
//                    sprop.SpriteGO.GetComponent<SpriteRenderer>().enabled = false;
//                    sprop.ShadowGO.GetComponent<SpriteRenderer>().enabled = false;
//                }
//                if (block.MonsterGO != null)
//                {
//                    var smon = block.MonsterGO.GetComponent<MonsterScript>();
//                    smon.SpriteRnd.enabled = false;
//                    smon.ShadowGO.GetComponent<SpriteRenderer>().enabled = false;
//                }
//            }
//        }

//        public void SetLayer(int layer, LayerInfo info, bool ignoreLock = false)
//        {
//            m_InfoLayers[layer - 1] = info;
//            if (!m_InfoLayers[layer - 1].IsValid())
//                return;

//            var blocks = m_Layers[layer - 1];
//            int idx = Manager.Mgr.RNG.Next(0, blocks.Count);
//            for (int i = idx; i < blocks.Count; ++i)
//                ApplyLayerToBlock(layer, blocks[i], ignoreLock);

//            for (int i = 0; i < idx; ++i)
//                ApplyLayerToBlock(layer, blocks[i], ignoreLock);
//        }

//        public void AddBlockToLayer(int layer, Block block)
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
//            for (int i = 0; i < Structure.LayerAmount; ++i)
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
//            for(int i = 0; i < Structure.LayerAmount; ++i)
//            {
//                if (!InfoLayers[i].IsValid())
//                    continue;
//                if (apply)
//                    ApplyWideChange(i + 1);
//                else
//                    UnapplyWideChange(i + 1);
//            }
//        }

//        private void InterchangeBlocks(int leftBlock, int rightBlock)
//        {
//            var lBlock = m_Pilars[leftBlock];
//            var rBlock = m_Pilars[rightBlock];

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
//            lBlock.StructureID = rightBlock;
//            rBlock.StructureID = leftBlock;

//            m_Pilars[leftBlock] = rBlock;
//            m_Pilars[rightBlock] = lBlock;
//        }

//        private void HFlip()
//        {
//            for(int y = 0; y < 8; ++y)
//            {
//                for(int x = 0; x < (8/2); ++x)
//                {
//                    int nX = (8 - 1) - x; // Flipped X
//                    int currentID = GameUtils.IDFromPos(new Vector2Int(x, y), 8, 8);
//                    int flippedID = GameUtils.IDFromPos(new Vector2Int(nX, y), 8, 8);
//                    InterchangeBlocks(currentID, flippedID);
//                }
//            }
//        }

//        private void VFlip()
//        {
//            for (int y = 0; y < (8/2); ++y)
//            {
//                for (int x = 0; x < 8; ++x)
//                {
//                    int nY = (8 - 1) - y; // Flipped Y
//                    int currentID = GameUtils.IDFromPos(new Vector2Int(x, y), 8, 8);
//                    int flippedID = GameUtils.IDFromPos(new Vector2Int(x, nY), 8, 8);
//                    InterchangeBlocks(currentID, flippedID);
//                }
//            }
//        }

//        private void Rotation90()
//        {
//            throw new Exception("90º degree structure rotation not done.");
//        }

//        public Structure()
//        {
//            //IE = new StructureIE();
//            _IDXIE = -1;
//            m_InfoLayers = new LayerInfo[LayerAmount];
//            m_Layers = new List<Block>[LayerAmount];
//            for (int i = 0; i < Layers.Length; ++i)
//                Layers[i] = new List<Block>();
//            for(int i = 0; i < m_InfoLayers.Length; ++i)
//            {
//                var layer = LayerInfo.GetDefaultLayer();
//                layer.Layer = i + 1;
//                m_InfoLayers[i] = layer;
//            }
//            m_StructGO = new GameObject($"Structure_{NextStructID++}")
//            {
//                isStatic = true
//            };
//            m_Pilars = new Pilar[Width * Height];
//            m_Flip = StructureFlip.NoFlip;
//            m_Rotation = BlockRotation.Default;
//            PropsInStruc = new List<GameObject>();
//            MonstersInStruc = new List<GameObject>();
//        }
//    }
//}
