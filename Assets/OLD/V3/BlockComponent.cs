/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

////using cakeslice;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Assets
//{
//    // What of the 4 possible square rotations is
//    public enum BlockRotation
//    {
//        Default,
//        Left,
//        Half,
//        Right,
//        COUNT
//    }

//    // Is the block locked or some property locked?
//    public enum BlockLock
//    {
//        Unlocked,
//        SemiLocked,
//        Locked,

//        COUNT
//    }

//    // What type of stair is?
//    public enum StairType
//    {
//        NONE,
//        POSSIBLE,
//        ALWAYS,

//        COUNT
//    }

//    // Where a Block decoration can be
//    public enum BlockDecoPosition
//    {
//        TOP,
//        NORTH,
//        SOUTH,
//        EAST,
//        WEST,

//        COUNT
//    }

//    // Contains the Y position and rotation of the block
//    public class BlockComponent : MonoBehaviour
//    {
//        public const string BlockTag = "BLOCK";

//        [SerializeField]
//        public int IDXIE;
//        [SerializeField]
//        // Blocks hidden because its a WIDE block
//        BlockComponent[] m_HiddenBlocks;
//        public BlockComponent[] HiddenBlocks
//        {
//            get
//            {
//                return m_HiddenBlocks;
//            }
//        }
//        [SerializeField]
//        // Block that represents the WIDE block
//        BlockComponent m_WIDEParent;
//        public BlockComponent WIDEParent
//        {
//            get
//            {
//                return m_WIDEParent;
//            }
//        }
//        // Anchor sprite
//        public SpriteRenderer AnchorSR
//        {
//            get
//            {
//                return m_AnchorSR;
//            }
//        }
//        //[SerializeField]
//        //GameObject m_AnchorGO;
//        [SerializeField]
//        SpriteRenderer m_AnchorSR;
//        // Stair sprite
//        public SpriteRenderer StairSR
//        {
//            get
//            {
//                return m_StairSR;
//            }
//        }
//        //[SerializeField]
//        //GameObject m_StairGO;
//        [SerializeField]
//        SpriteRenderer m_StairSR;
//        // Layer sprite
//        public SpriteRenderer LayerSR
//        {
//            get
//            {
//                return m_LayerSR;
//            }
//        }
//        //[SerializeField]
//        //GameObject m_LayerGO;
//        [SerializeField]
//        SpriteRenderer m_LayerSR;
//        // Lock sprite
//        public SpriteRenderer LockSR
//        {
//            get
//            {
//                return m_LockSR;
//            }
//        }
//        //[SerializeField]
//        //GameObject m_LockGO;
//        [SerializeField]
//        SpriteRenderer m_LockSR;
//        [SerializeField]
//        BoxCollider m_BlockBC;
//        public BoxCollider BlockBC
//        {
//            get
//            {
//                return m_BlockBC;
//            }
//        }
//        // TOP block object GO, doesn't contain any transform
//        //public GameObject TopGO
//        //{
//        //    get
//        //    {
//        //        return m_Top;
//        //    }
//        //}
//        public MeshRenderer TopMR
//        {
//            get
//            {
//                return m_TopMR;
//            }
//        }
//        //public BoxCollider TopBC
//        //{
//        //    get
//        //    {
//        //        return m_TopBC;
//        //    }
//        //}
//        //[SerializeField]
//        //GameObject m_Top;
//        [SerializeField]
//        MeshRenderer m_TopMR;
//        //[SerializeField]
//        //BoxCollider m_TopBC;
//        //[SerializeField]
//        //cakeslice.Outline m_TopOutline;
//        [SerializeField]
//        Outline m_TopOutline;
//        // MID block object GO, doesn't contain any transform
//        //public GameObject MidGO
//        //{
//        //    get
//        //    {
//        //        return m_Middle;
//        //    }
//        //}
//        public MeshRenderer MidMR
//        {
//            get
//            {
//                return m_MiddleMR;
//            }
//        }
//        //public BoxCollider MidBC
//        //{
//        //    get
//        //    {
//        //        return m_MiddleBC;
//        //    }
//        //}
//        //[SerializeField]
//        //GameObject m_Middle;
//        [SerializeField]
//        MeshRenderer m_MiddleMR;
//        //[SerializeField]
//       // BoxCollider m_MiddleBC;
//        [SerializeField]
//        Outline m_MiddleOutline;
//        // Prop on top of this block
//        public PropScript Prop;
//        // Monster spawner on top of this block
//        public MonsterScript Monster;
//        // Material of the whole Block
//        public MaterialPart TopMaterialPart
//        {
//            get
//            {
//                return m_TopMaterialPart;
//            }
//            set
//            {
//                m_TopMaterialPart = value;
//                m_TopMR.material = m_TopMaterialPart.Mat;
//            }
//        }
//        public MaterialPart MidMaterialPart
//        {
//            get
//            {
//                return m_MidMaterialPart;
//            }
//            set
//            {
//                m_MidMaterialPart = value;
//                m_MiddleMR.material = m_MidMaterialPart.Mat;
//            }
//        }
//        [SerializeField]
//        MaterialPart m_TopMaterialPart;
//        [SerializeField]
//        MaterialPart m_MidMaterialPart;
//        //public FullBlockMat BlockMat
//        //{
//        //    get
//        //    {
//        //        return m_BlockMaterial;
//        //    }
//        //    set
//        //    {
//        //        var blockMat = value;

//        //        var topMat = BlockMaterial.BlockMaterials[blockMat.TopMat];
//        //        var midMat = BlockMaterial.BlockMaterials[blockMat.MidMat];
//        //        m_MaterialTypeID = topMat.MaterialTypeID;
//        //        m_TopMR.material = topMat.BlockMaterial;
//        //        m_MiddleMR.material = midMat.BlockMaterial;

//        //        m_BlockMaterial = blockMat;
//        //    }
//        //}
//        //[SerializeField]
//        //private FullBlockMat m_BlockMaterial;
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

//                //GameObject.Destroy(m_TopBC);
//                //m_TopBC = null;
//                if (m_BlockType == BlockType.WIDE)
//                {
//                    UnsetWIDE();
//                    //GameObject.Destroy(m_MiddleBC);
//                    //m_MiddleBC = null;
//                }
//                BlockMeshDef.SetBlock(m_TopMR.gameObject, type, Def.BlockMeshType.TOP, 0.0f);
//                BlockMeshDef.SetBlock(m_MiddleMR.gameObject, type, Def.BlockMeshType.MID, 0.0f);
//                //m_TopBC = m_TopMR.gameObject.AddComponent<BoxCollider>();
//                //if(m_MiddleBC == null)
//                //    m_MiddleBC = m_MiddleMR.gameObject.AddComponent<BoxCollider>();
//                if (type == BlockType.WIDE)
//                {
//                    m_AnchorSR.transform.localScale = new Vector3(1.6f, 1.6f, 1.0f);
//                    m_LayerSR.transform.localScale = new Vector3(1.6f, 1.6f, 1.0f);
//                    //m_StairGO.transform.localScale = new Vector3(1.6f, 1.6f, 1.0f);
//                    if (Stair == StairType.ALWAYS)
//                        Stair = StairType.NONE;
//                    m_LockSR.transform.localScale = new Vector3(1.6f, 1.6f, 1.0f);
//                }
//                else
//                {
//                    m_AnchorSR.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
//                    m_LayerSR.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
//                    //m_StairGO.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
//                    m_LockSR.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
//                }
//                if (type == BlockType.STAIRS)
//                {
//                    m_LockSR.transform.Translate(new Vector3(0.0f, 0.5f, 0.0f), Space.World);
//                    m_LayerSR.transform.Translate(new Vector3(0.0f, 0.5f, 0.0f), Space.World);
//                }
//                if (m_BlockType == BlockType.STAIRS)
//                {
//                    m_LockSR.transform.Translate(new Vector3(0.0f, -0.5f, 0.0f), Space.World);
//                    m_LayerSR.transform.Translate(new Vector3(0.0f, -0.5f, 0.0f), Space.World);
//                }
//                if (m_Layer != 0)
//                {
//                    m_TopMR.receiveShadows = true;
//                    m_TopMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
//                    m_MiddleMR.receiveShadows = true;
//                    m_MiddleMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
//                }

//                switch (type)
//                {
//                    case BlockType.NORMAL:
//                        m_BlockBC.size = new Vector3(1f, m_Length, 1f);
//                        m_BlockBC.center = new Vector3(-0.5f, m_Length * -0.5f, 0.5f);
//                        break;
//                    case BlockType.STAIRS:
//                        m_BlockBC.size = new Vector3(1f, m_Length + 0.5f, 1f);
//                        m_BlockBC.center = new Vector3(-0.5f, m_Length * -0.5f + 0.25f, 0.5f);
//                        break;
//                    case BlockType.WIDE:
//                        m_BlockBC.size = new Vector3(2f, m_Length, 2f);
//                        m_BlockBC.center = new Vector3(-1f, m_Length * -0.5f, 1f);
//                        break;
//                }
//                m_BlockType = type;
//                MaterialFmly = m_MaterialFamily;
//                //MaterialTypeID = m_MaterialTypeID;
//                var length = m_Length;
//                m_Length = 0.0f;
//                Length = length;
//                var rot = m_Rotation;
//                m_Rotation = BlockRotation.COUNT;
//                Rotation = rot;
//            }
//        }
//        [SerializeField]
//        BlockType m_BlockType;
//        // MaterialType which the material of the block belongs
//        public MaterialFamily MaterialFmly
//        {
//            get
//            {
//                return m_MaterialFamily;
//            }
//            set
//            {
//                m_MaterialFamily = value;
//                MaterialSet[] sets = null;
//                switch (m_BlockType)
//                {
//                    case BlockType.NORMAL:
//                        sets = m_MaterialFamily.NormalMaterials;
//                        break;
//                    case BlockType.STAIRS:
//                        sets = m_MaterialFamily.StairMaterials;
//                        break;
//                    case BlockType.WIDE:
//                        sets = m_MaterialFamily.WideMaterials;
//                        break;
//                }
//                if(sets == null || (sets != null && sets.Length == 0))
//                {
//                    var stonefamily = BlockMaterial.MaterialFamilies[BlockMaterial.FamilyDict["Stone"]];
//                    switch (m_BlockType)
//                    {
//                        case BlockType.NORMAL:
//                            sets = stonefamily.NormalMaterials;
//                            break;
//                        case BlockType.STAIRS:
//                            sets = stonefamily.StairMaterials;
//                            break;
//                        case BlockType.WIDE:
//                            sets = stonefamily.WideMaterials;
//                            break;
//                    }
//                }
//                //var rng = Manager.Mgr.BuildRNG;
//                var matIdx = UnityEngine.Random.Range(0, sets.Length);//rng.Next(0, sets.Length);
//                var fullMat = sets[matIdx];
//                TopMaterialPart = fullMat.TopPart;
//                MidMaterialPart = fullMat.BottomPart;
//            }
//        }
//        [SerializeField]
//        MaterialFamily m_MaterialFamily;
//        //public int MaterialTypeID
//        //{
//        //    get
//        //    {
//        //        return m_MaterialTypeID;
//        //    }
//        //    set
//        //    {
//        //        var materialTypeID = value;

//        //        if (BlockMaterial.MaterialTypes.Count <= materialTypeID)
//        //            throw new Exception($"Trying to apply a non existant MaterialType {materialTypeID}");

//        //        var materialType = BlockMaterial.MaterialTypes[materialTypeID];
//        //        var materials = materialType.Def.Materials[(int)blockType];
//        //        if (materials.Count == 0)
//        //        {
//        //            MaterialTypeID = 1;
//        //            return;
//        //        }
//        //        //throw new Exception($"This MaterialType {materialType.MaterialTypeNames[0]}, does not have any available material for the requested block type {blockType}");
//        //        var rng = Manager.Mgr.BuildRNG;
//        //        var matIdx = rng.Next(0, materials.Count);
//        //        var blockMat = materials[matIdx];
//        //        BlockMat = blockMat;
//        //        m_MaterialTypeID = materialTypeID;
//        //    }
//        //}
//        //[SerializeField]
//        //private int m_MaterialTypeID;
//        // Is this block inside a WIDE block?
//        public bool Removed
//        {
//            get
//            {
//                return m_Removed;
//            }
//            set
//            {
//                var remove = value;
//                if (remove)
//                {
//                    m_TopMR.tag = "Untagged";
//                    m_MiddleMR.tag = "Untagged";
//                    m_BlockBC.tag = "Untagged";
//                }
//                else
//                {
//                    m_TopMR.tag = BlockTag;
//                    m_MiddleMR.tag = BlockTag;
//                    m_BlockBC.tag = BlockTag;
//                }
//                m_Removed = remove;
//            }
//        }
//        [SerializeField]
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

//                //int color = 0;
//                //if (m_Selected)
//                //{
//                //    color = 1;
//                //    highlight = true;
//                //}
//                //m_TopOutline.color = color;
//                //m_TopOutline.enabled = highlight;

//                Color color = Color.red;
//                bool midOutline = false;
//                if(m_Selected)
//                {
//                    highlight = true;
//                    color = Color.green;
//                    midOutline = true;
//                }
//                if(m_TopOutline.OutlineColor != color)
//                    m_TopOutline.OutlineColor = color;

//                if (midOutline && m_MiddleOutline.OutlineColor != color)
//                    m_MiddleOutline.OutlineColor = color;

//                m_TopOutline.enabled = highlight;
//                m_MiddleOutline.enabled = midOutline;

//                m_Highlighted = highlight;
//            }
//        }
//        [SerializeField]
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
//        [SerializeField]
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
//                if (length == 0.0f)
//                {
//                    length = vHeight.y;
//                }
//                length = Mathf.Clamp(Mathf.Abs(length), Mathf.Abs(vHeight.x), Mathf.Abs(vHeight.y));

//                if (length == m_Length)
//                    return;

//                //GameObject.Destroy(m_MiddleBC);
//                BlockMeshDef.SetBlock(m_MiddleMR.gameObject, blockType, Def.BlockMeshType.MID, length);
//                //m_MiddleBC = m_MiddleMR.gameObject.AddComponent<BoxCollider>();
//                if (m_Layer != 0)
//                {
//                    m_MiddleMR.receiveShadows = true;
//                    m_MiddleMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
//                }
//                Vector2 stairOffset = Vector2.zero;
//                if(m_BlockType == BlockType.STAIRS)
//                {
//                    stairOffset.x = 0.5f;
//                    stairOffset.y = 0.25f;
//                }
//                m_BlockBC.size = new Vector3(m_BlockBC.size.x, length + stairOffset.x, m_BlockBC.size.z);
//                m_BlockBC.center = new Vector3(m_BlockBC.center.x, length * -0.5f + stairOffset.y, m_BlockBC.center.z);
//                m_Length = length;
//            }
//        }
//        [SerializeField]
//        private float m_Length;
//        // What is the height of the object
//        public float Height
//        {
//            get
//            {
//                return m_Height;
//            }
//            set
//            {
//                var height = value;
//                if (m_BlockType == BlockType.WIDE)
//                {
//                    //var blocks = StructureComponent.GetNearBlocks(m_Pilar.Struc, m_Pilar.StructureID, m_Height, Layer);
//                    //if (blocks[0] == null)
//                    //{
//                    //    throw new Exception("Couldn't find the WIDE hidden blocks while changing the WIDE block height");
//                    //}
//                    //for (int i = 0; i < blocks.Length; ++i)
//                    //{
//                    //    blocks[i].Height = height;
//                    //}
//                    if (m_HiddenBlocks == null || (m_HiddenBlocks != null && m_HiddenBlocks.Length != 3))
//                        throw new Exception("Trying to change the block height from a WIDE block, but the HiddenBlocks is invalid");

//                    for (int i = 0; i < m_HiddenBlocks.Length; ++i)
//                    {
//                        var block = m_HiddenBlocks[i];
//                        block.m_Height = value;
//                        block.SetHeight();
//                    }
//                }
//                else if(m_Removed)
//                {
//                    return;
//                }
//                m_Height = height;
//                SetHeight();
//            }
//        }
//        [SerializeField]
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
//                    if (m_HiddenBlocks == null || (m_HiddenBlocks != null && m_HiddenBlocks.Length != 3))
//                        throw new Exception("Trying to change the block height from a WIDE block, but the HiddenBlocks is invalid");

//                    for (int i = 0; i < m_HiddenBlocks.Length; ++i)
//                    {
//                        var block = m_HiddenBlocks[i];
//                        block.m_MicroHeight = value;
//                        block.SetHeight();
//                    }
//                }
//                else if(m_Removed)
//                {
//                    return;
//                }
//                m_MicroHeight = mheight;
//                SetHeight();
//            }
//        }
//        [SerializeField]
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
//                Vector3 pos = m_Pilar.gameObject.transform.position;
//                pos.y = Height + MicroHeight;
//                transform.SetPositionAndRotation(pos, transform.parent.rotation);

//                if (blockType == BlockType.WIDE)
//                {
//                    switch (rotation)
//                    {
//                        case BlockRotation.Default:
//                            transform.Rotate(Vector3.up, 90.0f, Space.Self);
//                            break;
//                        case BlockRotation.Right:
//                            transform.Translate(Vector3.right * 2.0f, Space.Self);
//                            break;
//                        case BlockRotation.Half:
//                            transform.Translate(new Vector3(2.0f, 0.0f, 2.0f), Space.Self);
//                            transform.Rotate(Vector3.up, 270.0f, Space.Self);
//                            break;
//                        case BlockRotation.Left:
//                            transform.Translate(Vector3.forward * 2.0f, Space.Self);
//                            transform.Rotate(Vector3.up, 180.0f, Space.Self);
//                            break;
//                    }
//                }
//                else
//                {
//                    switch (rotation)
//                    {
//                        case BlockRotation.Default:
//                            transform.Rotate(Vector3.up, 90.0f, Space.Self);
//                            break;
//                        case BlockRotation.Right:
//                            transform.Translate(Vector3.right, Space.Self);
//                            break;
//                        case BlockRotation.Half:
//                            transform.Translate(new Vector3(1.0f, 0.0f, 1.0f), Space.Self);
//                            transform.Rotate(Vector3.up, 270.0f, Space.Self);
//                            break;
//                        case BlockRotation.Left:
//                            transform.Translate(Vector3.forward, Space.Self);
//                            transform.Rotate(Vector3.up, 180.0f, Space.Self);
//                            break;
//                    }
//                }
//                m_Rotation = rotation;
//            }
//        }
//        [SerializeField]
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


//                m_AnchorSR.enabled = anchor;
//                if (anchor)
//                    Stair = StairType.NONE;

//                m_Anchor = anchor;
//            }
//        }
//        [SerializeField]
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
                
//                switch (locked)
//                {
//                    case BlockLock.Unlocked:
//                        m_LockSR.enabled = false;
//                        break;
//                    case BlockLock.SemiLocked:
//                        foreach (var sprite in AssetContainer.Mgr.EditorSprites)
//                        {
//                            if (sprite.name == "SemiLockedSprite")
//                            {
//                                m_LockSR.sprite = sprite;
//                                break;
//                            }
//                        }
//                        if (m_LockSR.sprite == null)
//                            throw new Exception("SemiLockedSprite was not found in AssetContainer.EditorSprites");
//                        m_LockSR.enabled = true;
//                        break;
//                    case BlockLock.Locked:
//                        foreach (var sprite in AssetContainer.Mgr.EditorSprites)
//                        {
//                            if (sprite.name == "LockedSprite")
//                            {
//                                m_LockSR.sprite = sprite;
//                                break;
//                            }
//                        }
//                        if (m_LockSR.sprite == null)
//                            throw new Exception("LockedSprite was not found in AssetContainer.EditorSprites");
//                        m_LockSR.enabled = true;
//                        break;
//                }

//                m_Locked = locked;
//            }
//        }
//        [SerializeField]
//        private BlockLock m_Locked;
//        // Is this block a Stair block? What type of stair block is?
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
                
//                switch (stair)
//                {
//                    case StairType.NONE:
//                        m_StairSR.enabled = false;
//                        blockType = BlockType.NORMAL;
//                        break;
//                    case StairType.POSSIBLE:
//                        Anchor = false;
//                        m_StairSR.enabled = true;
//                        blockType = BlockType.NORMAL;
//                        break;
//                    case StairType.ALWAYS:
//                        Anchor = false;
//                        m_StairSR.enabled = false;
//                        if(Prop != null)
//                        {
//                            Prop.ReceiveDamage(Def.DamageType.UNAVOIDABLE, Prop.GetTotalHealth());
//                            Prop = null;
//                        }
//                        blockType = BlockType.STAIRS;
//                        break;
//                }
//                MaterialFmly = m_MaterialFamily;

//                m_Stair = stair;
//            }
//        }
//        [SerializeField]
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
//                    m_Pilar.Struc.Layers[m_Layer - 1].Remove(this);
                
//                if (layer == 0)
//                {
//                    m_LayerSR.enabled = false;
//                    Anchor = false;
//                    Stair = StairType.NONE;
//                    Locked = BlockLock.Unlocked;
//                    var fullVoid = BlockMaterial.VoidMat[(int)blockType];
//                    var topMat = fullVoid.TopPart;
//                    var midMat = fullVoid.BottomPart;

//                    blockType = BlockType.NORMAL;
//                    m_TopMR.receiveShadows = false;
//                    m_TopMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
//                    m_TopMR.material = topMat.Mat;
//                    m_MiddleMR.receiveShadows = false;
//                    m_MiddleMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
//                    m_MiddleMR.material = midMat.Mat;
                    
//                    if(Prop != null)
//                    {
//                        Prop.ReceiveDamage(Def.DamageType.UNAVOIDABLE, Prop.GetTotalHealth());
//                        Prop = null;
//                    }
//                    if(Monster != null)
//                    {
//                        Monster.ReceiveDamage(Def.DamageType.UNAVOIDABLE, Monster.GetTotalHealth());
//                        Monster = null;
//                    }
//                    Length = 0.5f;
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
//                        m_LayerSR.sprite = sprite;
//                        break;
//                    }
//                }
//                if (m_LayerSR.sprite == null)
//                    throw new Exception("AnchorSprite was not found in AssetContainer.PropSprites");
//                m_LayerSR.enabled = true;
//                m_TopMR.receiveShadows = true;
//                m_TopMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
//                m_MiddleMR.receiveShadows = true;
//                m_MiddleMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
//                m_Layer = layer;
//                m_Pilar.Struc.AddBlockToLayer(layer, this);
//            }
//        }
//        [SerializeField]
//        private int m_Layer;
//        // The Pilar class that this Bloc belongs
//        public PilarComponent Pilar
//        {
//            get
//            {
//                return m_Pilar;
//            }
//        }
//        [SerializeField]
//        private PilarComponent m_Pilar;

//        [SerializeField]
//        AntComponent[] m_Ants;
//        public AntComponent[] Ants
//        {
//            get
//            {
//                return m_Ants;
//            }
//        }

//        public void SetTopAnt(int version = -1, AntTopDirection direction = AntTopDirection.COUNT)
//        {
//            if (m_Ants[0] != null)
//            {
//                GameObject.Destroy(m_Ants[0].gameObject);
//                m_Ants[0] = null;
//            }
//            if (direction == AntTopDirection.COUNT || version < 0)
//            {
//                return;
//            }

//            AntDef def = null;
//            switch (direction)
//            {
//                case AntTopDirection.SOUTH_NORTH:
//                case AntTopDirection.NORTH_SOUTH:
//                case AntTopDirection.EAST_WEST:
//                case AntTopDirection.WEST_EAST:
//                    if (version >= AntManager.StraightAnts.Count)
//                        throw new Exception("Trying to set a TOP Straight Ant but with a higher version than the available ones.");
//                    def = AntManager.StraightAnts[version];
//                    break;
//                case AntTopDirection.SOUTH_EAST:
//                case AntTopDirection.WEST_SOUTH:
//                case AntTopDirection.NORTH_WEST:
//                case AntTopDirection.EAST_NORTH:
//                    if (version >= AntManager.TurnRightAnts.Count)
//                        throw new Exception("Trying to set a TOP Straight Ant but with a higher version than the available ones.");
//                    def = AntManager.TurnRightAnts[version];
//                    break;
//                case AntTopDirection.SOUTH_WEST:
//                case AntTopDirection.NORTH_EAST:
//                case AntTopDirection.WEST_NORTH:
//                case AntTopDirection.EAST_SOUTH:
//                    if (version >= AntManager.TurnLeftAnts.Count)
//                        throw new Exception("Trying to set a TOP Straight Ant but with a higher version than the available ones.");
//                    def = AntManager.TurnLeftAnts[version];
//                    break;
//            }
//            var ant = m_Ants[0] = new GameObject(gameObject.name + "_ANT_TOP").AddComponent<AntComponent>();
//            ant.SetAnt(def, (int)direction);
//            ant.transform.SetParent(m_Pilar.transform);
//            var baseHeight = m_Height + m_MicroHeight;
//            ant.transform.localPosition = new Vector3(0.501f, baseHeight + 0.01f, 0.501f);
//            switch (direction)
//            {
//                case AntTopDirection.SOUTH_NORTH:
//                    ant.transform.localRotation = Quaternion.Euler(270f, 0f, 90f);
//                    break;
//                case AntTopDirection.NORTH_SOUTH:
//                    ant.transform.localRotation = Quaternion.Euler(270f, 0f, 90f);
//                    ant.Renderer.flipY = true;
//                    break;
//                case AntTopDirection.EAST_WEST:
//                    ant.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
//                    break;
//                case AntTopDirection.WEST_EAST:
//                    ant.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
//                    ant.Renderer.flipY = true;
//                    break;
//                case AntTopDirection.SOUTH_WEST:
//                    ant.transform.localRotation = Quaternion.Euler(270f, 90f, 0f);
//                    break;
//                case AntTopDirection.SOUTH_EAST:
//                    ant.transform.localRotation = Quaternion.Euler(270f, 90f, 0f);
//                    break;
//                case AntTopDirection.NORTH_WEST:
//                    ant.transform.localRotation = Quaternion.Euler(270f, 270f, 0f);
//                    break;
//                case AntTopDirection.NORTH_EAST:
//                    ant.transform.localRotation = Quaternion.Euler(270f, 270f, 0f);
//                    break;
//                case AntTopDirection.WEST_NORTH:
//                    ant.transform.localRotation = Quaternion.Euler(270f, 180f, 0f);
//                    break;
//                case AntTopDirection.WEST_SOUTH:
//                    ant.transform.localRotation = Quaternion.Euler(270f, 180f, 0f);
//                    break;
//                case AntTopDirection.EAST_NORTH:
//                    ant.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
//                    break;
//                case AntTopDirection.EAST_SOUTH:
//                    ant.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
//                    break;
//            }
//            ant.Renderer.flipX = true;
//            ant.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
//        }

//        public void SetSideAnt(int version = -1, SpaceDirection direction = SpaceDirection.COUNT, bool upDirection = true)
//        {
//            if (direction == SpaceDirection.COUNT)
//                return;
//            if (m_Ants[((int)direction) + 1] != null)
//            {
//                GameObject.Destroy(m_Ants[((int)direction) + 1].gameObject);
//                m_Ants[(int)direction + 1] = null;
//            }

//            if (version < 0)
//            {
//                return;
//            }

//            if (version >= AntManager.StraightAnts.Count)
//                throw new Exception("Trying to set a SIDE Ant but with a higher version than the available ones.");
            
//            var ant = m_Ants[((int)direction) + 1] = new GameObject(gameObject.name + "_ANT_" + direction.ToString()).AddComponent<AntComponent>();
//            ant.SetAnt(AntManager.StraightAnts[version], ((int)direction) * 2 + (upDirection ? 0 : 1));
//            ant.transform.SetParent(m_Pilar.transform);
            
//            var baseHeight = m_Height + m_MicroHeight;
//            switch (direction)
//            {
//                case SpaceDirection.NORTH:
//                    ant.transform.localPosition = new Vector3(-0.02f, baseHeight - 0.5f, 0.501f);
//                    ant.transform.localRotation = Quaternion.Euler(0f, 270f, 0f);
//                    ant.Renderer.flipY = !upDirection;
//                    break;
//                case SpaceDirection.SOUTH:
//                    ant.transform.localPosition = new Vector3(1.02f, baseHeight - 0.5f, 0.501f);
//                    ant.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
//                    ant.Renderer.flipY = !upDirection;
//                    ant.Renderer.flipX = true;
//                    break;
//                case SpaceDirection.EAST:
//                    ant.transform.localPosition = new Vector3(0.501f, baseHeight - 0.5f, 1.02f);
//                    ant.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
//                    ant.Renderer.flipY = !upDirection;
//                    break;
//                case SpaceDirection.WEST:
//                    ant.transform.localPosition = new Vector3(0.501f, baseHeight - 0.5f, -0.02f);
//                    ant.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
//                    ant.Renderer.flipY = !upDirection;
//                    break;
//            }
//            ant.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
//        }

//        public void SetWIDE(BlockComponent[] hiddenBlocks)
//        {
//            if (hiddenBlocks == null || (hiddenBlocks != null && hiddenBlocks.Length != 3))
//                throw new Exception("Trying to change the block into a WIDE, but the provided HiddenBlock list is invalid.");

//            blockType = BlockType.WIDE;
//            m_HiddenBlocks = hiddenBlocks;
//            for(int i = 0; i < m_HiddenBlocks.Length; ++i)
//            {
//                var block = m_HiddenBlocks[i];
//                block.Layer = m_Layer;
//                block.TopMR.enabled = false;
//                //block.TopBC.enabled = false;
//                block.MidMR.enabled = false;
//                //block.MidBC.enabled = false;
//                block.BlockBC.enabled = false;

//                block.LockSR.enabled = false;
//                block.LayerSR.enabled = false;
//                block.AnchorSR.enabled = false;
//                block.StairSR.enabled = false;

//                block.Height = m_Height;
//                block.MicroHeight = m_MicroHeight;
//                block.Length = m_Length;
//                block.Removed = true;
//                block.m_WIDEParent = this;
//            }
//        }

//        void UnsetWIDE()
//        {
//            if (m_HiddenBlocks == null || (m_HiddenBlocks != null && m_HiddenBlocks.Length != 3))
//                throw new Exception("Trying to change the block type from WIDE to another type, but the HiddenBlocks is invalid");

//            //blockType = BlockType.NORMAL;

//            for(int i = 0; i < m_HiddenBlocks.Length; ++i)
//            {
//                var block = m_HiddenBlocks[i];
//                block.TopMR.enabled = true;
//                //block.TopBC.enabled = true;
//                block.MidMR.enabled = true;
//                //block.MidBC.enabled = true;
//                block.BlockBC.enabled = true;
                
//                block.LayerSR.enabled = true;

//                if (block.Anchor)
//                    block.AnchorSR.enabled = true;
//                if (block.Stair == StairType.POSSIBLE)
//                    block.StairSR.enabled = true;
//                if (block.Locked != BlockLock.Unlocked)
//                    block.LockSR.enabled = true;

//                block.Removed = false;
//                block.m_WIDEParent = null;
//            }
//            m_HiddenBlocks = null;
//        }

//        void SetHeight()
//        {
//            var nPos = Height + MicroHeight;
//            var diff = nPos - transform.position.y;

//            transform.Translate(new Vector3(0.0f, diff, 0.0f), Space.World);
//            m_AnchorSR.transform.Translate(new Vector3(0.0f, diff, 0.0f), Space.World);
//            m_LockSR.transform.Translate(new Vector3(0.0f, diff, 0.0f), Space.World);
//            m_StairSR.transform.Translate(new Vector3(0.0f, diff, 0.0f), Space.World);
//            m_LayerSR.transform.Translate(new Vector3(0.0f, diff, 0.0f), Space.World);
//            //if (m_Ants != null)
//            //{
//                for (int i = 0; i < m_Ants.Length; ++i)
//                {
//                    if (m_Ants[i] != null)
//                    {
//                        m_Ants[i].transform.Translate(0f, diff, 0f, Space.World);
//                    }
//                }
//            //}

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

//        void AddSprites()
//        {
//            // Create GO
//            var anchorGO = new GameObject(gameObject.name + "_anchor");
//            anchorGO.transform.SetParent(m_Pilar.gameObject.transform);
//            //anchorGO.transform.SetParent(transform);

//            var layerGO = new GameObject(gameObject.name + "_layer");
//            layerGO.transform.SetParent(m_Pilar.gameObject.transform);
//            //layerGO.transform.SetParent(transform);


//            var stairGO = new GameObject(gameObject.name + "_stair");
//            stairGO.transform.SetParent(m_Pilar.gameObject.transform);

//            var lockGO = new GameObject(gameObject.name + "_lock");
//            lockGO.transform.SetParent(m_Pilar.gameObject.transform);

//            // Position GO
//            {
//                var pos = m_Pilar.gameObject.transform.position;
//                pos.y = Height;

//                anchorGO.transform.Translate(new Vector3(pos.x, pos.y + 0.01f, pos.z), Space.World);
//                layerGO.transform.Translate(new Vector3(pos.x, pos.y + 0.02f, pos.z), Space.World);
//                stairGO.transform.Translate(new Vector3(pos.x, pos.y + 0.03f, pos.z), Space.World);
//                lockGO.transform.Translate(new Vector3(pos.x, pos.y + 0.04f, pos.z), Space.World);

//                anchorGO.transform.Rotate(Vector3.right, 90.0f, Space.World);
//                anchorGO.transform.Rotate(Vector3.up, -90.0f, Space.World);
//                layerGO.transform.Rotate(Vector3.right, 90.0f, Space.World);
//                layerGO.transform.Rotate(Vector3.up, -90.0f, Space.World);
//                stairGO.transform.Rotate(Vector3.right, 90.0f, Space.World);
//                stairGO.transform.Rotate(Vector3.up, -90.0f, Space.World);
//                lockGO.transform.Rotate(Vector3.right, 90.0f, Space.World);
//                lockGO.transform.Rotate(Vector3.up, -90.0f, Space.World);

//                anchorGO.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
//                layerGO.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
//                stairGO.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
//                lockGO.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);
//            }
//            // Anchor Sprite
//            {
//                m_AnchorSR = anchorGO.AddComponent<SpriteRenderer>();
//                m_AnchorSR.drawMode = SpriteDrawMode.Simple;
//                m_AnchorSR.spriteSortPoint = SpriteSortPoint.Center;
//                m_AnchorSR.material = Materials.GetMaterial(Def.Materials.Sprite);
//                foreach (var sprite in AssetContainer.Mgr.EditorSprites)
//                {
//                    if (sprite.name == "AnchorSprite")
//                    {
//                        m_AnchorSR.sprite = sprite;
//                        break;
//                    }
//                }
//                if (m_AnchorSR.sprite == null)
//                    throw new Exception("AnchorSprite was not found in AssetContainer.EditorSprites");
//                m_AnchorSR.enabled = false;
//            }
//            // Layer Sprite
//            {
//                m_LayerSR = layerGO.AddComponent<SpriteRenderer>();
//                m_LayerSR.drawMode = SpriteDrawMode.Simple;
//                m_LayerSR.spriteSortPoint = SpriteSortPoint.Center;
//                m_LayerSR.material = Materials.GetMaterial(Def.Materials.Sprite);
//                m_LayerSR.enabled = false;
//            }
//            // Stair sprite
//            {
//                m_StairSR = stairGO.AddComponent<SpriteRenderer>();
//                m_StairSR.drawMode = SpriteDrawMode.Simple;
//                m_StairSR.spriteSortPoint = SpriteSortPoint.Center;
//                m_StairSR.material = Materials.GetMaterial(Def.Materials.Sprite);
//                foreach (var sprite in AssetContainer.Mgr.EditorSprites)
//                {
//                    if (sprite.name == "StairSprite")
//                    {
//                        m_StairSR.sprite = sprite;
//                        break;
//                    }
//                }
//                if (m_StairSR.sprite == null)
//                    throw new Exception("StairSprite was not found in AssetContainer.EditorSprites");
//                m_StairSR.enabled = false;
//            }
//            // Lock sprite
//            {
//                m_LockSR = lockGO.AddComponent<SpriteRenderer>();
//                m_LockSR.drawMode = SpriteDrawMode.Simple;
//                m_LockSR.spriteSortPoint = SpriteSortPoint.Center;
//                m_LockSR.material = Materials.GetMaterial(Def.Materials.Sprite);
//                m_LockSR.enabled = false;
//            }
//        }

//        public void DestroyBlock()
//        {
//            Layer = 0;
//            m_Pilar.Blocks.Remove(this);
//            if(Prop != null)
//            {
//                Prop.ReceiveDamage(Def.DamageType.UNAVOIDABLE, Prop.GetTotalHealth());
//                if (Pilar.Struc != null && Pilar.Struc.LivingEntities.Contains(Prop))
//                    Pilar.Struc.LivingEntities.Remove(Prop);
//                Prop = null;
//            }
//            if(Monster != null)
//            {
//                Monster.ReceiveDamage(Def.DamageType.UNAVOIDABLE, Prop.GetTotalHealth());
//                if(Pilar.Struc != null && Pilar.Struc.LivingEntities.Contains(Monster))
//                    Pilar.Struc.LivingEntities.Remove(Monster);
//                Monster = null;
//            }

//            GameUtils.DeleteGameObjectAndItsChilds(gameObject);
//            GameUtils.DeleteGameObjectAndItsChilds(m_AnchorSR.gameObject);
//            GameUtils.DeleteGameObjectAndItsChilds(m_LayerSR.gameObject);
//            GameUtils.DeleteGameObjectAndItsChilds(m_StairSR.gameObject);
//            GameUtils.DeleteGameObjectAndItsChilds(m_LockSR.gameObject);
//            for(int i = 0; i < m_Ants.Length; ++i)
//            {
//                if (m_Ants[i] != null)
//                    GameUtils.DeleteGameObjectAndItsChilds(m_Ants[i].gameObject);
//            }
//        }

//        public BlockComponent()
//        {
//            m_Anchor = false;
//            //m_AnchorGO = null;
//            m_AnchorSR = null;

//            m_Layer = 0;
//            //m_LayerGO = null;
//            m_LayerSR = null;

//            m_Stair = StairType.NONE;
//            //m_StairGO = null;
//            m_StairSR = null;

//            m_Locked = BlockLock.Unlocked;
//            //m_LockGO = null;
//            m_LockSR = null;

//            Prop = null;
//            Monster = null;

//            //m_Top = null;
//            //m_TopBC = null;
//            m_TopMR = null;
//            //m_TopOutline = null;
//            m_TopOutline = null;

//            //m_Middle = null;
//            //m_MiddleBC = null;
//            m_MiddleMR = null;

//            m_BlockBC = null;

//            m_BlockType = BlockType.NORMAL;
//            m_TopMaterialPart = null;
//            m_MidMaterialPart = null;
//            m_MaterialFamily = null;
//            m_Removed = false;
//            m_Highlighted = false;
//            m_Selected = false;
//            m_Length = -1.0f;
//            m_Height = 0.0f;
//            m_MicroHeight = 0.0f;
//            m_Rotation = BlockRotation.Default;
//            IDXIE = -1;
//            m_Pilar = null;
//        }

//        void Start()
//        {
            
//        }

//        private void OnEnable()
//        {
//            if (m_TopMR == null)
//                return;
//            //m_TopBC.enabled = true;
//            m_TopMR.enabled = true;

//            //m_MiddleBC.enabled = true;
//            m_MiddleMR.enabled = true;
//            m_BlockBC.enabled = true;

//            if (m_Anchor)
//                m_AnchorSR.enabled = true;

//            if (m_Layer != 0)
//                m_LayerSR.enabled = true;

//            if (m_Locked != BlockLock.Unlocked)
//                m_LockSR.enabled = true;

//            if (m_Stair == StairType.POSSIBLE)
//                m_StairSR.enabled = true;
            
//        }

//        private void OnDisable()
//        {
//            if (m_TopMR == null)
//                return;
//            //m_TopBC.enabled = false;
//            m_TopMR.enabled = false;

//            //m_MiddleBC.enabled = false;
//            m_MiddleMR.enabled = false;
//            m_BlockBC.enabled = false;

//            m_AnchorSR.enabled = false;
//            m_LayerSR.enabled = false;
//            m_LockSR.enabled = false;
//            m_StairSR.enabled = false;
//        }

//        public void _OnPilarNameChanged(int prevNameLength)
//        {
//            m_AnchorSR.gameObject.name = m_Pilar.gameObject.name + m_AnchorSR.gameObject.name.Substring(prevNameLength);
//            m_StairSR.gameObject.name = m_Pilar.gameObject.name + m_StairSR.gameObject.name.Substring(prevNameLength);
//            m_LockSR.gameObject.name = m_Pilar.gameObject.name + m_LockSR.gameObject.name.Substring(prevNameLength);
//            m_LayerSR.gameObject.name = m_Pilar.gameObject.name + m_LayerSR.gameObject.name.Substring(prevNameLength);
//            m_TopMR.gameObject.name = m_Pilar.gameObject.name + m_TopMR.gameObject.name.Substring(prevNameLength);
//            m_MiddleMR.gameObject.name = m_Pilar.gameObject.name + m_MiddleMR.gameObject.name.Substring(prevNameLength);
//            gameObject.name = m_Pilar.gameObject.name + gameObject.name.Substring(prevNameLength);
//        }

//        public void Init(PilarComponent pilar, BlockType type, MaterialSet materialSet, float height = 0.0f, float length = 0.0f)
//        {
//            m_Pilar = pilar;

//            var top = new GameObject(gameObject.name + "_top");
//            BlockMeshDef.SetBlock(top, type, Def.BlockMeshType.TOP);
//            top.transform.SetParent(transform);
//            //m_TopBC = top.AddComponent<BoxCollider>();
//            m_TopMR = top.GetComponent<MeshRenderer>();
//            //top.tag = BlockTag;

//            var mid = new GameObject(gameObject.name + "_mid");
//            BlockMeshDef.SetBlock(mid, type, Def.BlockMeshType.MID);
//            mid.transform.SetParent(transform);
//            //m_MiddleBC = mid.AddComponent<BoxCollider>();
//            m_MiddleMR = mid.GetComponent<MeshRenderer>();
//            //mid.tag = BlockTag;

//            transform.Rotate(Vector3.up, 90.0f, Space.World);

//            m_BlockBC = gameObject.AddComponent<BoxCollider>();
//            m_BlockBC.center = new Vector3(-0.5f, -0.5f, 0.5f);
//            gameObject.tag = BlockTag;
//            gameObject.layer = Manager.BlockLayer;

//            m_BlockType = type;
//            MaterialFmly = BlockMaterial.VoidMat[0].Family;

//            //m_TopOutline = m_Top.AddComponent<cakeslice.Outline>();
//            //m_TopOutline.enabled = false;

//            m_TopOutline = top.AddComponent<Outline>();
//            m_TopOutline.enabled = false;
//            m_TopOutline.OutlineMode = Outline.Mode.OutlineAll;
//            m_MiddleOutline = mid.AddComponent<Outline>();
//            m_MiddleOutline.enabled = false;
//            m_MiddleOutline.OutlineMode = Outline.Mode.OutlineAll;

//            AddSprites();

//            m_Ants = new AntComponent[(int)BlockDecoPosition.COUNT];

//            Height = height;

//            Length = length;
//        }
//    }
//}
