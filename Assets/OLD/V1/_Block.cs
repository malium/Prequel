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

//namespace Assets
//{
//    public enum BlockRotation
//    {
//        Default,
//        Left,
//        Half,
//        Right,
//        COUNT
//    }

//    public enum BlockLock
//    {
//        Unlocked,
//        SemiLocked,
//        Locked,

//        COUNT
//    }

//    public enum StairType
//    {
//        NONE,
//        POSSIBLE,
//        ALWAYS,

//        COUNT
//    }

//    public class Block
//    {
//        public const string BlockTag = "BLOCK";

//        private void SetHeight()
//        {
//            var nPos = Height + MicroHeight;
//            var diff = nPos - m_BlockGO.transform.position.y;

//            m_BlockGO.transform.Translate(new Vector3(0.0f, diff, 0.0f), Space.World);
//            m_AnchorGO.transform.Translate(new Vector3(0.0f, diff, 0.0f), Space.World);
//            m_LayerGO.transform.Translate(new Vector3(0.0f, diff, 0.0f), Space.World);
//            m_StairGO.transform.Translate(new Vector3(0.0f, diff, 0.0f), Space.World);
//            m_LockGO.transform.Translate(new Vector3(0.0f, diff, 0.0f), Space.World);

//            //if (m_Prop != null)
//            //{
//            //    //var script = m_Prop.GetComponent<PropScript>();
//            //    //script.YOffset += diff;
//            //    m_Prop.transform.Translate(new Vector3(0.0f, diff, 0.0f), Space.World);
//            //    //script.Position = new Vector3(script.Position.x, script.Position.y + diff, script.Position.z);
//            //}
//            //if(m_Monster != null)
//            //{
//            //    //var script = m_Monster.GetComponent<MonsterScript>();
//            //    //script.YOffset += diff;
//            //    m_Monster.transform.Translate(new Vector3(0.0f, diff, 0.0f), Space.World);
//            //}
//        }

//        public int IDXIE;

//        // Contains the Y position and rotation of the block
//        public GameObject BlockGO
//        {
//            get
//            {
//                return m_BlockGO;
//            }
//        }
//        private GameObject m_BlockGO;
//        // Anchor sprite GO
//        public GameObject AnchorGO
//        {
//            get
//            {
//                return m_AnchorGO;
//            }
//        }
//        private GameObject m_AnchorGO;
//        public GameObject StairGO
//        {
//            get
//            {
//                return m_StairGO;
//            }
//        }
//        private GameObject m_StairGO;
//        // Layer sprite GO
//        public GameObject LayerGO
//        {
//            get
//            {
//                return m_LayerGO;
//            }
//        }
//        private GameObject m_LayerGO;
//        // Lock sprite GO
//        public GameObject LockGO
//        {
//            get
//            {
//                return m_LockGO;
//            }
//        }
//        private GameObject m_LockGO;
//        // TOP block object GO, doesn't contain any transform
//        public GameObject TopGO
//        {
//            get
//            {
//                return m_Top;
//            }
//        }
//        private GameObject m_Top;
//        // MID block object GO, doesn't contain any transform
//        public GameObject MidGO
//        {
//            get
//            {
//                return m_Middle;
//            }
//        }
//        private GameObject m_Middle;
//        // BOT block object GO, may contain Y-offset transform, due to mid shrinking
//        //public GameObject BotGO
//        //{
//        //    get
//        //    {
//        //        return m_Bottom;
//        //    }
//        //}
//        //private GameObject m_Bottom;
//        public GameObject PropGO
//        {
//            get
//            {
//                return m_Prop;
//            }
//            set
//            {
//                var prop = value;
//                //if(m_Prop != null)
//                //{
//                //    m_Prop.GetComponent<PropScript>().TakeDamage(100.0f);
//                //}
//                //if (prop != null)
//                //{
//                //    var pos = m_Pilar.GO.transform.position;
//                //    pos.y = Height + MicroHeight;
//                //    prop.transform.SetPositionAndRotation(pos, Quaternion.identity);
//                //    prop.transform.SetParent(pilar.GO.transform);
//                //    //script.BlockOffset = halfWidth;
//                //    //script.Position = BlockGO.transform.position;
//                //    //script.Position = new Vector3(script.Position.x + halfWidth, script.Position.y, script.Position.z + halfWidth);
//                //    if(Stair == StairType.ALWAYS)
//                //        Stair = StairType.NONE;
//                //}
//                m_Prop = prop;
//            }
//        }
//        private GameObject m_Prop;
//        public GameObject MonsterGO
//        {
//            get
//            {
//                return m_Monster;
//            }
//            set
//            {
//                var monster = value;
//                //if(m_Monster != null)
//                //{
//                //    m_Monster.GetComponent<MonsterScript>().TakeDamage(100.0f);
//                //}
//                //if(monster != null)
//                //{
//                //    var pos = m_Pilar.GO.transform.position;
//                //    pos.y = Height + MicroHeight;
//                //    monster.transform.SetPositionAndRotation(pos, Quaternion.identity);
//                //    monster.transform.SetParent(pilar.GO.transform);

//                //    if (Stair == StairType.ALWAYS)
//                //        Stair = StairType.NONE;
//                //}

//                m_Monster = monster;
//            }
//        }
//        private GameObject m_Monster;
//        // Material of the whole Block
//        public FullBlockMat BlockMat
//        {
//            get
//            {
//                return m_BlockMaterial;
//            }
//            set
//            {
//                var blockMat = value;
//                //if (blockMat.TopMat == m_BlockMaterial.TopMat && blockMat.MidMat == m_BlockMaterial.MidMat)
//                //    return;

//                var topMat = BlockMaterial.BlockMaterials[blockMat.TopMat];
//                var midMat = BlockMaterial.BlockMaterials[blockMat.MidMat];
//                m_MaterialTypeID = topMat.MaterialTypeID;
//                m_Top.GetComponent<Renderer>().material = topMat.BlockMaterial;
//                m_Middle.GetComponent<Renderer>().material = midMat.BlockMaterial;
//                //m_Bottom.GetComponent<Renderer>().material = midMat.BlockMaterial;

//                m_BlockMaterial = blockMat;
//            }
//        }
//        private FullBlockMat m_BlockMaterial;
//        // Type of the block (NORMAL, STAIRS, WIDE)
//        public BlockType blockType
//        {
//            get
//            {
//                return m_BlockType;
//            }
//            set
//            {
//                var type = value;
//                if (type == m_BlockType)
//                    return;

//                GameObject.Destroy(m_Top.GetComponent<BoxCollider>());
//                BlockMeshDef.SetBlock(m_Top, type, Def.BlockMeshType.TOP, 0.0f);
//                BlockMeshDef.SetBlock(m_Middle, type, Def.BlockMeshType.MID, 0.0f);
//                //BlockMeshDef.SetBlock(m_Bottom, type, Def.BlockMeshType.BOTTOM, 0.0f);
//                m_Top.AddComponent<BoxCollider>();
//                if(type == BlockType.WIDE)
//                {
//                    m_AnchorGO.transform.localScale = new Vector3(1.6f, 1.6f, 1.0f);
//                    m_LayerGO.transform.localScale = new Vector3(1.6f, 1.6f, 1.0f);
//                    //m_StairGO.transform.localScale = new Vector3(1.6f, 1.6f, 1.0f);
//                    if(Stair == StairType.ALWAYS)
//                        Stair = StairType.NONE;
//                    m_LockGO.transform.localScale = new Vector3(1.6f, 1.6f, 1.0f);
//                }
//                else
//                {
//                    m_AnchorGO.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
//                    m_LayerGO.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
//                    //m_StairGO.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
//                    m_LockGO.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
//                }
//                if(type == BlockType.STAIRS)
//                {
//                    m_LockGO.transform.Translate(new Vector3(0.0f, 0.5f, 0.0f), Space.World);
//                    m_LayerGO.transform.Translate(new Vector3(0.0f, 0.5f, 0.0f), Space.World);
//                }
//                if(m_BlockType == BlockType.STAIRS)
//                {
//                    m_LockGO.transform.Translate(new Vector3(0.0f, -0.5f, 0.0f), Space.World);
//                    m_LayerGO.transform.Translate(new Vector3(0.0f, -0.5f, 0.0f), Space.World);
//                }
//                if (m_Layer != 0)
//                {
//                    var topMeshRenderer = TopGO.GetComponent<MeshRenderer>();
//                    topMeshRenderer.receiveShadows = true;
//                    topMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
//                    var midMeshRenderer = MidGO.GetComponent<MeshRenderer>();
//                    midMeshRenderer.receiveShadows = true;
//                    midMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
//                }
//                m_BlockType = type;
//                MaterialTypeID = m_MaterialTypeID;
//                var length = m_Length;
//                m_Length = 0.0f;
//                Length = length;
//                var rot = m_Rotation;
//                m_Rotation = BlockRotation.COUNT;
//                Rotation = rot;
//            }
//        }
//        private BlockType m_BlockType;
//        // MaterialType which the material of the block belongs
//        public int MaterialTypeID
//        {
//            get
//            {
//                return m_MaterialTypeID;
//            }
//            set
//            {
//                var materialTypeID = value;

//                if (BlockMaterial.MaterialTypes.Count <= materialTypeID)
//                    throw new Exception($"Trying to apply a non existant MaterialType {materialTypeID}");
                
//                var materialType = BlockMaterial.MaterialTypes[materialTypeID];
//                var materials = materialType.Def.Materials[(int)blockType];
//                if (materials.Count == 0)
//                {
//                    MaterialTypeID = 1;
//                    return;
//                }
//                //throw new Exception($"This MaterialType {materialType.MaterialTypeNames[0]}, does not have any available material for the requested block type {blockType}");
//                var rng = Manager.Mgr.RNG;
//                var matIdx = rng.Next(0, materials.Count);
//                var blockMat = materials[matIdx];
//                BlockMat = blockMat;
//                m_MaterialTypeID = materialTypeID;
//            }
//        }
//        private int m_MaterialTypeID;
//        // Is removed or belongs to layer 0?
//        public bool Removed
//        {
//            get
//            {
//                return m_Removed;
//            }
//            set
//            {
//                var remove = value;
//                if(remove)
//                {
//                    m_Top.tag = "Untagged";
//                    m_Middle.tag = "Untagged";
//                    //m_Bottom.tag = "Untagged";
//                }
//                else
//                {
//                    m_Top.tag = BlockTag;
//                    m_Middle.tag = BlockTag;
//                    //m_Bottom.tag = BlockTag;
//                }
//                //if (m_Removed == remove)
//                //    return;

//                //if (remove)
//                //{
//                //    //var fullVoid = BlockMaterial.VoidMat[(int)blockType];
//                //    //var topMat = BlockMaterial.blockMats[fullVoid.TopMat].BlockMaterial;
//                //    //var midMat = BlockMaterial.blockMats[fullVoid.MidMat].BlockMaterial;

//                //    //TopGO.GetComponent<Renderer>().material = topMat;
//                //    //MidGO.GetComponent<Renderer>().material = midMat;
//                //    //BottomGO.GetComponent<Renderer>().material = midMat;
//                //    Layer = 0;
//                //    //SetAnchor(false);
//                //}
//                //else
//                //{
//                //    var topMat = BlockMaterial.blockMats[BlockMat.TopMat].BlockMaterial;
//                //    var midMat = BlockMaterial.blockMats[BlockMat.MidMat].BlockMaterial;

//                //    m_Top.GetComponent<Renderer>().material = topMat;
//                //    m_Middle.GetComponent<Renderer>().material = midMat;
//                //    m_Bottom.GetComponent<Renderer>().material = midMat;
//                //}
//                m_Removed = remove;
//            }
//        }
//        private bool m_Removed;
//        // Is highlighted or selected
//        public bool Highlighted
//        {
//            get
//            {
//                return m_Highlighted;
//            }
//            set
//            {
//                var highlight = value;
//                if (highlight == m_Highlighted)
//                    return;

//                int color = 0;
//                var TopOutline = m_Top.GetComponent<Outline>();
//                //var midOutline = MidGO.GetComponent<Outline>();
//                //var botOutline = MidGO.GetComponent<Outline>();

//                if (m_Selected)
//                {
//                    color = 1;
//                    highlight = true;

//                    //midOutline.color = color;
//                    //botOutline.color = color;
//                    //midOutline.enabled = true;
//                    //botOutline.enabled = true;
//                }
//                else
//                {
//                    //midOutline.enabled = false;
//                    //botOutline.enabled = false;
//                }
//                TopOutline.color = color;
//                TopOutline.enabled = highlight;

//                m_Highlighted = highlight;
//            }
//        }
//        private bool m_Highlighted;
//        // Is selected
//        public bool Selected
//        {
//            get
//            {
//                return m_Selected;
//            }
//            set
//            {
//                var selected = value;
//                if (selected == m_Selected)
//                    return;

//                m_Selected = selected;
//                Highlighted = selected;
//            }
//        }
//        private bool m_Selected;
//        // What is the length of the MID block object
//        public float Length
//        {
//            get
//            {
//                return m_Length;
//            }
//            set
//            {
//                var length = value;
//                int idx = blockType == BlockType.WIDE ? 1 : 0;
//                var vHeight = BlockMeshDef.MidMesh.VertexHeight[idx];
//                //bool changeBotHeight = true;
//                if (length == 0.0f)
//                {
//                    length = vHeight.y;
//                    //changeBotHeight = false;
//                }
//                length = Mathf.Clamp(Mathf.Abs(length), Mathf.Abs(vHeight.x), Mathf.Abs(vHeight.y));

//                if (length == m_Length)
//                    return;

//                BlockMeshDef.SetBlock(m_Middle, blockType, Def.BlockMeshType.MID, length);
//                if(m_Layer != 0)
//                {
//                    var midMeshRenderer = MidGO.GetComponent<MeshRenderer>();
//                    midMeshRenderer.receiveShadows = true;
//                    midMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
//                }

//                //if (changeBotHeight)
//                //{
//                //    var botPos = m_Bottom.transform.localPosition;
//                //    float yPos = (-length) - vHeight.y;
//                //    float movement = yPos - botPos.y;
//                //    m_Bottom.transform.Translate(new Vector3(0.0f, movement, 0.0f), Space.Self);
//                //}
//                m_Length = length;
//            }
//        }
//        private float m_Length;
//        // What is the height of the object == BlockGO.transform.position.y
//        public float Height
//        {
//            get
//            {
//                return m_Height;
//            }
//            set
//            {
//                var height = value;
//                if(m_BlockType == BlockType.WIDE)
//                {
//                    var blocks = Structure.GetNearBlocks(pilar.Struct, pilar.StructureID, m_Height, Layer);
//                    if(blocks[0] == null)
//                    {
//                        throw new Exception("Something went wrong!");
//                    }
//                    for (int i = 0; i < blocks.Length; ++i)
//                    {
//                        blocks[i].Height = height;
//                    }
//                }
//                //m_BlockGO.transform.Translate(new Vector3(0.0f, diff, 0.0f), Space.World);
//                //m_AnchorGO.transform.Translate(new Vector3(0.0f, diff, 0.0f), Space.World);
//                //m_LayerGO.transform.Translate(new Vector3(0.0f, diff, 0.0f), Space.World);
//                m_Height = height;
//                SetHeight();
//            }
//        }
//        private float m_Height;
//        // Whats the micro height variation, so we can extract the real block height
//        public float MicroHeight
//        {
//            get
//            {
//                return m_MicroHeight;
//            }
//            set
//            {
//                var mheight = value;
//                if (mheight == m_MicroHeight)
//                    return;

//                if (m_BlockType == BlockType.WIDE)
//                {
//                    var blocks = Structure.GetNearBlocks(pilar.Struct, pilar.StructureID, m_Height, Layer);
//                    for (int i = 0; i < blocks.Length; ++i)
//                        blocks[i].MicroHeight = mheight;
//                }

//                //m_BlockGO.transform.Translate(new Vector3(0.0f, diff, 0.0f), Space.World);
//                //m_AnchorGO.transform.Translate(new Vector3(0.0f, diff, 0.0f), Space.World);
//                //m_LayerGO.transform.Translate(new Vector3(0.0f, diff, 0.0f), Space.World);

//                m_MicroHeight = mheight;
//                SetHeight();
//            }
//        }
//        private float m_MicroHeight;
//        // What of the 4 rotations currently is
//        public BlockRotation Rotation
//        {
//            get
//            {
//                return m_Rotation;
//            }
//            set
//            {
//                var rotation = value;
//                Vector3 pos = pilar.GO.transform.position;
//                pos.y = Height + MicroHeight;
//                m_BlockGO.transform.SetPositionAndRotation(pos, m_BlockGO.transform.parent.rotation);

//                if (blockType == BlockType.WIDE)
//                {
//                    switch (rotation)
//                    {
//                        case BlockRotation.Default:
//                            m_BlockGO.transform.Rotate(Vector3.up, 90.0f, Space.Self);
//                            break;
//                        case BlockRotation.Right:
//                            m_BlockGO.transform.Translate(Vector3.right * 2.0f, Space.Self);
//                            break;
//                        case BlockRotation.Half:
//                            m_BlockGO.transform.Translate(new Vector3(2.0f, 0.0f, 2.0f), Space.Self);
//                            m_BlockGO.transform.Rotate(Vector3.up, 270.0f, Space.Self);
//                            break;
//                        case BlockRotation.Left:
//                            m_BlockGO.transform.Translate(Vector3.forward * 2.0f, Space.Self);
//                            m_BlockGO.transform.Rotate(Vector3.up, 180.0f, Space.Self);
//                            break;
//                    }
//                }
//                else
//                {
//                    switch (rotation)
//                    {
//                        case BlockRotation.Default:
//                            m_BlockGO.transform.Rotate(Vector3.up, 90.0f, Space.Self);
//                            break;
//                        case BlockRotation.Right:
//                            m_BlockGO.transform.Translate(Vector3.right, Space.Self);
//                            break;
//                        case BlockRotation.Half:
//                            m_BlockGO.transform.Translate(new Vector3(1.0f, 0.0f, 1.0f), Space.Self);
//                            m_BlockGO.transform.Rotate(Vector3.up, 270.0f, Space.Self);
//                            break;
//                        case BlockRotation.Left:
//                            m_BlockGO.transform.Translate(Vector3.forward, Space.Self);
//                            m_BlockGO.transform.Rotate(Vector3.up, 180.0f, Space.Self);
//                            break;
//                    }
//                }
//                m_Rotation = rotation;
//            }
//        }
//        private BlockRotation m_Rotation;
//        // Is an Anchor block?
//        public bool Anchor
//        {
//            get
//            {
//                return m_Anchor;
//            }
//            set
//            {
//                var anchor = value;
//                if (anchor == m_Anchor)
//                    return;

//                var sr = m_AnchorGO.GetComponent<SpriteRenderer>();
//                sr.enabled = anchor;
//                if (anchor)
//                    Stair = StairType.NONE;

//                m_Anchor = anchor;
//            }
//        }
//        private bool m_Anchor;
//        // Is this block locked or has some property locked
//        public BlockLock Locked
//        {
//            get
//            {
//                return m_Locked;
//            }
//            set
//            {
//                var locked = value;

//                var sr = m_LockGO.GetComponent<SpriteRenderer>();
                
//                switch(locked)
//                {
//                    case BlockLock.Unlocked:
//                        sr.enabled = false;
//                        break;
//                    case BlockLock.SemiLocked:
//                        foreach (var sprite in AssetContainer.Mgr.EditorSprites)
//                        {
//                            if (sprite.name == "SemiLockedSprite")
//                            {
//                                sr.sprite = sprite;
//                                break;
//                            }
//                        }
//                        if (sr.sprite == null)
//                            throw new Exception("SemiLockedSprite was not found in AssetContainer.EditorSprites");
//                        sr.enabled = true;
//                        break;
//                    case BlockLock.Locked:
//                        foreach (var sprite in AssetContainer.Mgr.EditorSprites)
//                        {
//                            if (sprite.name == "LockedSprite")
//                            {
//                                sr.sprite = sprite;
//                                break;
//                            }
//                        }
//                        if (sr.sprite == null)
//                            throw new Exception("LockedSprite was not found in AssetContainer.EditorSprites");
//                        sr.enabled = true;
//                        break;
//                }

//                m_Locked = locked;
//            }
//        }
//        private BlockLock m_Locked;
//        public StairType Stair
//        {
//            get
//            {
//                return m_Stair;
//            }
//            set
//            {
//                var stair = value;
//                if (stair == m_Stair)
//                    return;
                
//                var sr = m_StairGO.GetComponent<SpriteRenderer>();
//                switch(stair)
//                {
//                    case StairType.NONE:
//                        sr.enabled = false;
//                        blockType = BlockType.NORMAL;
//                        MaterialTypeID = MaterialTypeID;
//                        break;
//                    case StairType.POSSIBLE:
//                        Anchor = false;
//                        sr.enabled = true;
//                        blockType = BlockType.NORMAL;
//                        MaterialTypeID = MaterialTypeID;
//                        break;
//                    case StairType.ALWAYS:
//                        Anchor = false;
//                        sr.enabled = false;
//                        PropGO = null;
//                        blockType = BlockType.STAIRS;
//                        MaterialTypeID = MaterialTypeID;
//                        break;
//                }

//                m_Stair = stair;
//            }
//        }
//        private StairType m_Stair;
//        // What layer this block belongs
//        public int Layer
//        {
//            get
//            {
//                return m_Layer;
//            }
//            set
//            {
//                var layer = value;
//                if (layer == m_Layer)
//                    return;

//                if (m_Layer != 0)
//                    m_Pilar.Struct.Layers[m_Layer - 1].Remove(this);

//                var topMeshRenderer = TopGO.GetComponent<MeshRenderer>();
//                var midMeshRenderer = MidGO.GetComponent<MeshRenderer>();

//                var sr = LayerGO.GetComponent<SpriteRenderer>();
//                if (layer == 0)
//                {
//                    sr.enabled = false;
//                    //m_AnchorGO.GetComponent<SpriteRenderer>().enabled = false;
//                    Anchor = false;
//                    Stair = StairType.NONE;
//                    Locked = BlockLock.Unlocked;
//                    var fullVoid = BlockMaterial.VoidMat[(int)blockType];
//                    var topMat = BlockMaterial.BlockMaterials[fullVoid.TopMat].BlockMaterial;
//                    var midMat = BlockMaterial.BlockMaterials[fullVoid.MidMat].BlockMaterial;

//                    topMeshRenderer.receiveShadows = false;
//                    topMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
//                    midMeshRenderer.receiveShadows = false;
//                    midMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

//                    m_Top.GetComponent<Renderer>().material = topMat;
//                    m_Middle.GetComponent<Renderer>().material = midMat;
//                    //m_Bottom.GetComponent<Renderer>().material = midMat;
//                    PropGO = null;
//					MonsterGO = null;
//                    Length = 0.5f;
//                    blockType = BlockType.NORMAL;
//                    Height = 0.0f;
//                    MicroHeight = 0.0f;
//                    Rotation = BlockRotation.Default;
//                    m_Layer = layer;
//                    return;
//                }
//                var name = $"LayerSprite_{layer}";
//                foreach (var sprite in AssetContainer.Mgr.EditorSprites)
//                {
//                    if (sprite.name == name)
//                    {
//                        sr.sprite = sprite;
//                        break;
//                    }
//                }
//                if (sr.sprite == null)
//                    throw new Exception("AnchorSprite was not found in AssetContainer.PropSprites");
//                sr.enabled = true;
//                topMeshRenderer.receiveShadows = true;
//                topMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
//                midMeshRenderer.receiveShadows = true;
//                midMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
//                m_Layer = layer;
//                m_Pilar.Struct.AddBlockToLayer(layer, this);
//            }
//        }
//        private int m_Layer;
//        // The Pilar class that this Bloc belongs
//        public Pilar pilar
//        {
//            get
//            {
//                return m_Pilar;
//            }
//        }
//        private Pilar m_Pilar;

//        public void DestroyBlock()
//        {
//            Layer = 0;
//            m_Pilar.Blocks.Remove(this);

//            GameUtils.DeleteGameObjectAndItsChilds(m_BlockGO);
//            GameUtils.DeleteGameObjectAndItsChilds(m_AnchorGO);
//            GameUtils.DeleteGameObjectAndItsChilds(m_LayerGO);
//            GameUtils.DeleteGameObjectAndItsChilds(m_StairGO);
//        }

//        private void AddSprites()
//        {
//            // Create GO
//            {
//                m_AnchorGO = new GameObject(BlockGO.name + "_anchor");
//                m_AnchorGO.transform.SetParent(pilar.GO.transform);
//                //m_AnchorGO.transform.SetParent(BlockGO.transform);
//                m_LayerGO = new GameObject(BlockGO.name + "_layer");
//                m_LayerGO.transform.SetParent(pilar.GO.transform);
//                //m_LayerGO.transform.SetParent(BlockGO.transform);
//                m_StairGO = new GameObject(BlockGO.name + "_stair");
//                m_StairGO.transform.SetParent(pilar.GO.transform);

//                m_LockGO = new GameObject(BlockGO.name + "_lock");
//                m_LockGO.transform.SetParent(pilar.GO.transform);
//            }
//            // Position GO
//            {
//                var pos = pilar.GO.transform.position;
//                pos.y = Height;

//                m_AnchorGO.transform.Translate(new Vector3(pos.x, pos.y + 0.01f, pos.z), Space.World);
//                m_LayerGO.transform.Translate(new Vector3(pos.x, pos.y + 0.02f, pos.z), Space.World);
//                m_StairGO.transform.Translate(new Vector3(pos.x, pos.y + 0.03f, pos.z), Space.World);
//                m_LockGO.transform.Translate(new Vector3(pos.x, pos.y + 0.04f, pos.z), Space.World);

//                m_AnchorGO.transform.Rotate(Vector3.right, 90.0f, Space.World);
//                m_AnchorGO.transform.Rotate(Vector3.up, -90.0f, Space.World);
//                m_LayerGO.transform.Rotate(Vector3.right, 90.0f, Space.World);
//                m_LayerGO.transform.Rotate(Vector3.up, -90.0f, Space.World);
//                m_StairGO.transform.Rotate(Vector3.right, 90.0f, Space.World);
//                m_StairGO.transform.Rotate(Vector3.up, -90.0f, Space.World);
//                m_LockGO.transform.Rotate(Vector3.right, 90.0f, Space.World);
//                m_LockGO.transform.Rotate(Vector3.up, -90.0f, Space.World);

//                m_AnchorGO.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
//                m_LayerGO.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
//                m_StairGO.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
//                m_LockGO.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
//            }
//            // Anchor Sprite
//            {
//                var sr = m_AnchorGO.AddComponent<SpriteRenderer>();
//                sr.drawMode = SpriteDrawMode.Simple;
//                sr.spriteSortPoint = SpriteSortPoint.Center;
//                sr.material = new Material(AssetContainer.Mgr.SpriteMaterial);
//                foreach (var sprite in AssetContainer.Mgr.EditorSprites)
//                {
//                    if (sprite.name == "AnchorSprite")
//                    {
//                        sr.sprite = sprite;
//                        break;
//                    }
//                }
//                if (sr.sprite == null)
//                    throw new Exception("AnchorSprite was not found in AssetContainer.EditorSprites");
//                sr.enabled = false;
//            }
//            // Layer Sprite
//            {
//                var sr = m_LayerGO.AddComponent<SpriteRenderer>();
//                sr.drawMode = SpriteDrawMode.Simple;
//                sr.spriteSortPoint = SpriteSortPoint.Center;
//                sr.material = new Material(AssetContainer.Mgr.SpriteMaterial);
//                sr.enabled = false;
//            }
//            // Stair sprite
//            {
//                var sr = m_StairGO.AddComponent<SpriteRenderer>();
//                sr.drawMode = SpriteDrawMode.Simple;
//                sr.spriteSortPoint = SpriteSortPoint.Center;
//                sr.material = new Material(AssetContainer.Mgr.SpriteMaterial);
//                foreach (var sprite in AssetContainer.Mgr.EditorSprites)
//                {
//                    if (sprite.name == "StairSprite")
//                    {
//                        sr.sprite = sprite;
//                        break;
//                    }
//                }
//                if (sr.sprite == null)
//                    throw new Exception("StairSprite was not found in AssetContainer.EditorSprites");
//                sr.enabled = false;
//            }
//            // Lock sprite
//            {
//                var sr = m_LockGO.AddComponent<SpriteRenderer>();
//                sr.drawMode = SpriteDrawMode.Simple;
//                sr.spriteSortPoint = SpriteSortPoint.Center;
//                sr.material = new Material(AssetContainer.Mgr.SpriteMaterial);
                
//                sr.enabled = false;
//            }
//        }

//        public Block(Pilar pilar, GameObject blockGO, GameObject topGO, GameObject midGO/*, GameObject bottomGO*/,
//            BlockType type, FullBlockMat blockMat, float height = 0.0f, float length = 0.0f)
//        {
//            // Defaults
//            m_BlockGO = blockGO;
//            m_AnchorGO = null;
//            m_LayerGO = null;
//            m_StairGO = null;
//            m_LockGO = null;
//            m_Prop = null;
//            m_Top = topGO;
//            m_Middle = midGO;
//            //m_Bottom = bottomGO;
//            m_BlockType = type;
//            m_MaterialTypeID = 0;
//            m_Removed = false;
//            m_Highlighted = false;
//            m_Selected = false;
//            m_Length = -1.0f;
//            m_Height = 0.0f;
//            m_MicroHeight = 0.0f;
//            m_Rotation = BlockRotation.Default;
//            m_Anchor = false;
//            m_Locked = BlockLock.Unlocked;
//            m_Stair = StairType.NONE;
//            m_Layer = 0;
//            IDXIE = -1;
//            m_Pilar = pilar;

//            // Tag set
//            m_Top.tag = BlockTag;
//            m_Middle.tag = BlockTag;
//            //m_Bottom.tag = BlockTag;

//            // Material set
//            BlockMat = blockMat;

//            // Outline set
//            m_Top.AddComponent<Outline>().enabled = false;
//            //m_Middle.AddComponent<Outline>().enabled = false;
//            //m_Bottom.AddComponent<Outline>().enabled = false;

//            // Sprite set
//            AddSprites();

//            // Height set
//            Height = height;

//            // Length set
//            Length = length;
//        }
//    }

//    public class Pilar
//    {
//        private int LastID;

//        public void _UpdateNameAndPosition(int nStructID = int.MinValue, int nMapID = int.MinValue)
//        {
//            if (nStructID < 0 && nMapID < 0)
//                return; // nothing to do

//            m_StructureID = nStructID != int.MinValue ? nStructID : m_StructureID;
//            m_MapID = nMapID != int.MinValue ? nMapID : m_MapID;

//            if (m_MapID < 0 && m_StructureID < 0)
//                throw new Exception("Pilar without MapID nor StructureID");

//            if (m_StructureID >= 0 && m_Structure == null)
//                throw new Exception("Pilar has StructureID but its structure is null.");

//            var prevName = m_PilarGO.name;
//            var curPos = m_PilarGO.transform.position;
//            // Pilar not in PlayMode
//            if (m_MapID < 0)
//            {
//                var structPos = GameUtils.PosFromID(m_StructureID, Structure.Width, Structure.Height);
//                var newPos = new Vector2(structPos.x + Structure.Separation * structPos.x, structPos.y + Structure.Separation * structPos.y);
//                m_PilarGO.transform.Translate(new Vector3(newPos.x - curPos.x, 0.0f, newPos.y - curPos.z), Space.World);
//                m_PilarGO.transform.SetParent(m_Structure.GO.transform);
//                m_PilarGO.name = m_Structure.GO.name + $"_({structPos.x},{structPos.y})";
//            }
//            // Pilar in PlayMode
//            else
//            {
//                var mapPos = GameUtils.PosFromID(m_MapID, Manager.MapWidth, Manager.MapHeight);
//                var newPos = new Vector2(mapPos.x + Structure.Separation * mapPos.x, mapPos.y + Structure.Separation * mapPos.y);
//                m_PilarGO.transform.Translate(new Vector3(newPos.x - curPos.x, 0.0f, newPos.y - curPos.z), Space.World);
                
//                // A pilar that is in PlayMode and belongs to a Structure
//                if(m_StructureID >= 0)
//                {
//                    var structPos = GameUtils.PosFromID(m_StructureID, Structure.Width, Structure.Height);
//                    m_PilarGO.transform.SetParent(m_Structure.GO.transform);
//                    m_PilarGO.name = m_Structure.GO.name + $"_S({structPos.x},{structPos.y})_M({mapPos.x},{mapPos.y})";
//                }
//                // A pilar that is in PlayMode and not belongs to a structure
//                else
//                {
//                    m_PilarGO.name = $"Pilar_({mapPos.x},{mapPos.y})";
//                }
//            }

//            for (int i = 0; i < m_Blocks.Count; ++i)
//            {
//                var block = m_Blocks[i];
//                block.AnchorGO.name = m_PilarGO.name + block.AnchorGO.name.Substring(prevName.Length);
//                block.StairGO.name = m_PilarGO.name + block.StairGO.name.Substring(prevName.Length);
//                block.LockGO.name = m_PilarGO.name + block.LockGO.name.Substring(prevName.Length);
//                block.LayerGO.name = m_PilarGO.name + block.LayerGO.name.Substring(prevName.Length);
//                block.BlockGO.name = m_PilarGO.name + block.BlockGO.name.Substring(prevName.Length);
//                block.TopGO.name = m_PilarGO.name + block.TopGO.name.Substring(prevName.Length);
//                block.MidGO.name = m_PilarGO.name + block.MidGO.name.Substring(prevName.Length);
//                //block.BotGO.name = m_PilarGO.name + block.BotGO.name.Substring(prevName.Length);

//                m_Blocks[i] = block;
//            }
//        }

//        // List of block definitions in the same (X, Z) position
//        public List<Block> Blocks
//        {
//            get
//            {
//                return m_Blocks;
//            }
//        }
//        private List<Block> m_Blocks;
//        // Structure that this block belongs
//        public Structure Struct
//        {
//            get
//            {
//                return m_Structure;
//            }
//            set
//            {
//                m_Structure = value;
//            }
//        }
//        private Structure m_Structure;
//        // GameObject that holds the (X, Z) position
//        public GameObject GO
//        {
//            get
//            {
//                return m_PilarGO;
//            }
//        }
//        private GameObject m_PilarGO;
//        // Current position in the structure, matches with the index on the Pilars variable and determines a valid position in the structure
//        public int StructureID
//        {
//            get
//            {
//                return m_StructureID;
//            }
//            set
//            {
//                var structID = value;
//                if (structID == m_StructureID)
//                    return;

//                _UpdateNameAndPosition(structID);
//            }
//        }
//        private int m_StructureID;
//        // Current position in the map, matches with the index on the Pilars variable and determines a valid position in the map
//        public int MapID
//        {
//            get
//            {
//                return m_MapID;
//            }
//            set
//            {
//                var mapID = value;
//                if (mapID == m_MapID)
//                    return;

//                _UpdateNameAndPosition(int.MinValue, mapID);
//            }
//        }
//        private int m_MapID;

//        public Pilar(Structure structure, int structureID = -1, int mapID = -1)
//        {
//            LastID = 0;
//            m_StructureID = -1;
//            m_MapID = -1;
//            m_Blocks = new List<Block>();
//            m_Structure = structure;
//            m_PilarGO = new GameObject();
//            _UpdateNameAndPosition(structureID, mapID);
//        }

//        public void AddBlock()
//        {
//            int id = LastID++;
//            var blockGO = new GameObject(m_PilarGO.name + $"_{id}");
//            blockGO.transform.SetParent(m_PilarGO.transform);
//            var topGO = new GameObject(blockGO.name + "_top");
//            var midGO = new GameObject(blockGO.name + "_mid");
//            //var botGO = new GameObject(blockGO.name + "_bot");
//            BlockMeshDef.SetBlock(topGO, BlockType.NORMAL, Def.BlockMeshType.TOP);
//            BlockMeshDef.SetBlock(midGO, BlockType.NORMAL, Def.BlockMeshType.MID);
//            //BlockMeshDef.SetBlock(botGO, BlockType.NORMAL, Def.BlockMeshType.BOTTOM);
//            topGO.transform.SetParent(blockGO.transform);
//            midGO.transform.SetParent(blockGO.transform);
//            //botGO.transform.SetParent(blockGO.transform);

//            blockGO.transform.Rotate(Vector3.up, 90.0f, Space.World);

//            var box = topGO.AddComponent<BoxCollider>();

//            // Null material
//            var nullMaterial = BlockMaterial.VoidMat[(int)BlockType.NORMAL];
//            var rng = Manager.Mgr.RNG;
//            var blockMat = nullMaterial;

//            Block block = new Block(this, blockGO, topGO, midGO/*, botGO*/, BlockType.NORMAL, blockMat, 0.0f, 0.5f);
//            blockGO.transform.Translate(m_PilarGO.transform.position, Space.World);
//            Blocks.Add(block);
//        }

//        public void DestroyPilar()
//        {
//            while (m_Blocks.Count > 0)
//                Blocks[0].DestroyBlock();
//            GameUtils.DeleteGameObjectAndItsChilds(m_PilarGO);
//            if(m_Structure != null)
//                m_Structure.Pilars[m_StructureID] = null;
//        }
//    }
//}