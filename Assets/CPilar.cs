/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class CPilar : MonoBehaviour
    {
        int m_LastID;
        [SerializeField] List<IBlock> m_Blocks;
        [SerializeField] BridgeComponent m_Bridge;
        [SerializeReference] IStruc m_Struc;
        [SerializeField] int m_StructureID;

        public List<IBlock> GetBlocks() => m_Blocks;
        public BridgeComponent GetBridge() => m_Bridge;
        public IStruc GetStruc() => m_Struc;
        public int GetStructureID() => m_StructureID;
        public long GetMapID()
        {
            var pos = GameUtils.TransformPosition(new Vector2(
                transform.position.x + 0.1f,
                transform.position.z + 0.1f));
            return GameUtils.MapIDFromPos(pos);
        }
        public void ChangeStruc(IStruc struc, int strucID)
        {
            if (struc == null)
            {
                Debug.LogWarning("Trying to set the structure of a pilar but was null.");
                return;
            }
            m_Struc = struc;
            transform.SetParent(struc.transform);
            ChangeID(strucID);
        }
        public void ChangeID(int strucID)
        {
            m_StructureID = strucID;
            var sPos = m_Struc.transform.position;
            var pPosOffset = m_Struc.WPosFromPilarID(strucID);
            //transform.localPosition = new Vector3(pPosOffset.x, 0f, pPosOffset.y);
            var move = sPos + new Vector3(pPosOffset.x, 0f, pPosOffset.y) - transform.position;
            transform.Translate(move, Space.World);
            UpdateName();
        }
        void UpdateName()
        {
            var oldName = gameObject.name;
            var strucPos = m_Struc.VPosFromPilarID(m_StructureID);
            gameObject.name = m_Struc.name + $"_Pilar({strucPos.x},{strucPos.y})";
            for (int i = 0; i < m_Blocks.Count; ++i)
            {
                m_Blocks[i]._OnPilarNameChange(oldName.Length);
            }
        }
        private void Awake()
        {
            m_LastID = 0;
            m_Blocks = new List<IBlock>(1);
            m_Bridge = null;
            m_Struc = null;
            m_StructureID = -1;
        }
        private void OnEnable()
        {
            for(int i = 0; i < m_Blocks.Count; ++i)
            {
                var block = m_Blocks[i];
                if (block == null)
                    continue;
                block.enabled = true;
            }
            if(m_Bridge != null)
                m_Bridge.enabled = true;
        }
        private void OnDisable()
        {
            for (int i = 0; i < m_Blocks.Count; ++i)
            {
                var block = m_Blocks[i];
                if (block == null)
                    continue;
                block.enabled = false;
            }
            if (m_Bridge != null)
                m_Bridge.enabled = false;
        }
        public void RemoveBlock(IBlock block)
		{
            if (block.GetPilar() != this)
                return;
            RemoveBlock(m_Blocks.IndexOf(block));
		}
        public void RemoveBlock(int idx)
		{
            if (idx < 0 || idx >= m_Blocks.Count)
                return;
            m_Blocks.RemoveAt(idx);
            for (int i = idx; i < m_Blocks.Count; ++i)
                m_Blocks[i]._SetPilarIndex(i);
		}
        public void AddBridge(int bridgeTypeID, BridgeType type, bool destroyBlocks)
        {
            while (m_Blocks.Count > 0 && destroyBlocks)
                m_Blocks[0].DestroyBlock(false);

            if (Bridges.BridgeTypeInfos.Count <= bridgeTypeID)
                throw new Exception("Trying to create a bridge but its TypeID its invalid");

            var bridgeInfo = Bridges.BridgeTypeInfos[bridgeTypeID];
            int bridgeID;
            if (type == BridgeType.BIG)
                bridgeID = bridgeInfo.BigBridges[UnityEngine.Random.Range(0, bridgeInfo.BigBridges.Count)];
            else
                bridgeID = bridgeInfo.SmallBridges[UnityEngine.Random.Range(0, bridgeInfo.SmallBridges.Count)];
            m_Bridge = GameObject.Instantiate(Bridges.BridgeInfos[bridgeID].BridgeGO);
            m_Bridge.gameObject.SetActive(true);
            m_Bridge.name = gameObject.name + "_Bridge";
            m_Bridge.Pilar = this;
            m_Bridge.SetBridge(bridgeID);
        }
        public CBlockEdit AddBlock()
        {
            int id = m_LastID++;
            var block = new GameObject(gameObject.name + $"_{id}").AddComponent<CBlockEdit>();
            block.transform.SetParent(transform);
            //block.transform.localPosition = new Vector3(0f, 0f, 0f);
            block.InitBlock(this, Def.BlockType.NORMAL, 0f, 0.5f);
            block._SetPilarIndex(m_Blocks.Count);
            m_Blocks.Add(block);
            block.transform.Translate(transform.position, Space.World);
            return block;
        }
        public CBlock AddGameBlock(bool initBlock = false)
		{
            int id = m_LastID++;
            var block = new GameObject(gameObject.name + $"_{id}").AddComponent<CBlock>();
            block.transform.SetParent(transform);
            if(initBlock)
                block.InitBlock(this, Def.BlockType.NORMAL, 0f, 0.5f);
            block._SetPilarIndex(m_Blocks.Count);
            m_Blocks.Add(block);
            block.transform.Translate(transform.position, Space.World);
            return block;
        }
        public IBlock GetClosestBlock(float height)
		{
            float diff = float.MaxValue;
            IBlock block = null;
            for (int i = 0; i < m_Blocks.Count; ++i)
            {
                var b = m_Blocks[i];
                var blockHeight = b.transform.position.y; //b.GetHeight() + b.GetMicroHeight();
                var d = Mathf.Abs(blockHeight - height);
                if (d < diff)
                {
                    block = b;
                    diff = d;
                }
            }
            return block;
        }
        public void DestroyBridge(bool instant = false)
        {
            if (m_Bridge == null)
                return;
            GameUtils.DeleteGameobject(m_Bridge.gameObject, instant);
            m_Bridge = null;
        }
        public void DestroyPilar(bool preserveEntities, bool instant = false)
        {
            while (m_Blocks.Count > 0)
            {
                if (m_Blocks[0].GetPilar() != this)
                    m_Blocks.RemoveAt(0);
                else
                    m_Blocks[0].DestroyBlock(preserveEntities, instant);
            }
            DestroyBridge(instant);
            if (m_Struc != null)
                m_Struc.GetPilars()[m_StructureID] = null;
            GameUtils.DeleteGameobject(gameObject, instant);
        }
    }
}
