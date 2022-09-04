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
//    // Contains the blocks in the X,Z position
//    public class PilarComponent : MonoBehaviour
//    {
//        // Last block id to avoid name repetition
//        [SerializeField]
//        int m_LastID;
//        // List of blocks in the same X,Z position
//        public List<BlockComponent> Blocks
//        {
//            get
//            {
//                return m_Blocks;
//            }
//        }
//        [SerializeField]
//        List<BlockComponent> m_Blocks;
//        // Structure where this pilar belongs, maybe null if its a void pilar
//        public StructureComponent Struc;
//        // Is this pilar a bridge?
//        public BridgeComponent Bridge;
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
//        [SerializeField]
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
//        [SerializeField]
//        private int m_MapID;

//        private void Awake()
//        {
//            m_LastID = 0;
//            m_Blocks = new List<BlockComponent>(1);
//            Bridge = null;
//            Struc = null;
//            m_StructureID = -1;
//            m_MapID = -1;
//        }

//        private void Start()
//        {
            
//        }

//        private void OnEnable()
//        {
//            if (m_Blocks == null)
//                return;
//            for (int i = 0; i < m_Blocks.Count; ++i)
//                m_Blocks[i].enabled = true;
//        }

//        private void OnDisable()
//        {
//            if (m_Blocks == null)
//                return;
//            for (int i = 0; i < m_Blocks.Count; ++i)
//                m_Blocks[i].enabled = false;
//        }

//        public void Init(StructureComponent structure, int structureID = -1, int mapID = -1)
//        {
//            Struc = structure;
//            m_StructureID = -1;
//            m_MapID = -1;
//            _UpdateNameAndPosition(structureID, mapID);
//        }

//        public BlockComponent AddBlock()
//        {
//            int id = m_LastID++;
//            var blockGO = new GameObject(gameObject.name + $"_{id}");
//            blockGO.transform.SetParent(transform);
//            var nullMaterial = BlockMaterial.VoidMat[(int)BlockType.NORMAL];
//            var blockMat = nullMaterial;
//            var block = blockGO.AddComponent<BlockComponent>();
//            block.Init(this, BlockType.NORMAL, blockMat, 0.0f, 0.5f);
//            blockGO.transform.Translate(transform.position, Space.World);
//            m_Blocks.Add(block);
//            return block;
//        }

//        public void _UpdateNameAndPosition(int nStructID = int.MinValue, int nMapID = int.MinValue)
//        {
//            if (nStructID < 0 && nMapID < 0)
//                return; // nothing to do

//            bool changeMapID = false;
//            int oldStrucID = -1;
//            if (m_MapID >= 0 && nMapID < 0 && m_StructureID != nStructID && nStructID >= 0)
//            {
//                changeMapID = true;
//                oldStrucID = m_StructureID;
//            }
//            m_StructureID = nStructID != int.MinValue ? nStructID : m_StructureID;

//            if (!changeMapID)
//            {
//                m_MapID = nMapID != int.MinValue ? nMapID : m_MapID;
//            }
//            else
//            {
//                var oStrucPos = GameUtils.PosFromID(oldStrucID);
//                var nStrucPos = GameUtils.PosFromID(m_StructureID);
//                var posOffset = nStrucPos - oStrucPos;
//                var mapPos = GameUtils.PosFromID(m_MapID, Manager.MapWidth, Manager.MapHeight);
//                mapPos += posOffset;
//                m_MapID = GameUtils.IDFromPos(mapPos, Manager.MapWidth, Manager.MapHeight);
//            }

//            if (m_MapID < 0 && m_StructureID < 0)
//                throw new Exception("Pilar without MapID nor StructureID");

//            if (m_StructureID >= 0 && Struc == null)
//                throw new Exception("Pilar has StructureID but its structure is null.");

//            var prevName = gameObject.name;
//            var curPos = transform.position;
//            // Pilar not in PlayMode
//            if (m_MapID < 0)
//            {
//                var inStrucPos = GameUtils.PosFromID(m_StructureID);
//                var strucPos = Struc.transform.position;
//                var newPos = new Vector2(strucPos.x + inStrucPos.x + StructureComponent.Separation * inStrucPos.x, strucPos.z + inStrucPos.y + StructureComponent.Separation * inStrucPos.y);
//                transform.position = new Vector3(newPos.x, transform.position.y, newPos.y);
//                //transform.Translate(new Vector3(newPos.x - curPos.x, 0.0f, newPos.y - curPos.z), Space.World);
//                transform.SetParent(Struc.transform);
//                gameObject.name = Struc.gameObject.name + $"_({inStrucPos.x},{inStrucPos.y})";
//            }
//            // Pilar in PlayMode
//            else
//            {
//                var mapPos = GameUtils.PosFromID(m_MapID, Manager.MapWidth, Manager.MapHeight);
//                var newPos = new Vector2(mapPos.x + StructureComponent.Separation * mapPos.x, mapPos.y + StructureComponent.Separation * mapPos.y);
//                //transform.position = new Vector3(newPos.x, transform.position.y, newPos.y);
//                transform.Translate(newPos.x - curPos.x, 0.0f, newPos.y - curPos.z, Space.World);

//                // A pilar that is in PlayMode and belongs to a Structure
//                if (m_StructureID >= 0)
//                {
//                    var structPos = GameUtils.PosFromID(m_StructureID);
//                    transform.SetParent(Struc.transform);
//                    gameObject.name = Struc.gameObject.name + $"_S({structPos.x},{structPos.y})_M({mapPos.x},{mapPos.y})";
//                }
//                // A pilar that is in PlayMode and not belongs to a structure
//                else
//                {
//                    gameObject.name = $"Pilar_({mapPos.x},{mapPos.y})";
//                }
//            }

//            for (int i = 0; i < m_Blocks.Count; ++i)
//            {
//                m_Blocks[i]._OnPilarNameChanged(prevName.Length);
//            }
//        }

//        public void AddBridge(int bridgeTypeID, BridgeType type, bool destroyBlocks)
//        {
//            while (m_Blocks.Count > 0 && destroyBlocks)
//                m_Blocks[0].DestroyBlock();
//            if (Bridges.BridgeTypeInfos.Count <= bridgeTypeID)
//                throw new Exception("Trying to create a bridge but its TypeID its invalid");
//            var bridgeInfo = Bridges.BridgeTypeInfos[bridgeTypeID];
//            int bridgeID;
//            if (type == BridgeType.BIG)
//                bridgeID = bridgeInfo.BigBridges[UnityEngine.Random.Range(0, bridgeInfo.BigBridges.Count) /*Manager.Mgr.BuildRNG.Next(0, bridgeInfo.BigBridges.Count)*/];
//            else
//                bridgeID = bridgeInfo.SmallBridges[UnityEngine.Random.Range(0, bridgeInfo.SmallBridges.Count) /*Manager.Mgr.BuildRNG.Next(0, bridgeInfo.SmallBridges.Count)*/];
//            var bridgeGO = GameObject.Instantiate(Bridges.BridgeInfos[bridgeID].BridgeGO);
//            bridgeGO.SetActive(true);
//            bridgeGO.name = gameObject.name + "_Bridge";
//            Bridge = bridgeGO.GetComponent<BridgeComponent>();
//            var childNum = bridgeGO.transform.childCount;
//            Bridge.Objects = new GameObject[childNum];
//            for (int i = 0; i < childNum; ++i)
//                Bridge.Objects[i] = bridgeGO.transform.GetChild(i).gameObject;
//            Bridge.Pilar = this;
//            Bridge.SetBridge(bridgeID);
//        }

//        public void DestroyBridge()
//        {
//            if (Bridge == null)
//                return;
//            GameUtils.DeleteGameObjectAndItsChilds(Bridge.gameObject);
//            Bridge = null;
//        }

//        public void DestroyPilar()
//        {
//            while (m_Blocks.Count > 0)
//                Blocks[0].DestroyBlock();
//            DestroyBridge();
//            if (Struc != null)
//                Struc.Pilars[m_StructureID] = null;
//            GameUtils.DeleteGameObjectAndItsChilds(gameObject);
//        }
//    }
//}
