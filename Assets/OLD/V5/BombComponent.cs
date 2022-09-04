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
//    public class BombComponent : VFXComponent
//    {
//        static readonly Collider[] BombCasts = new Collider[30];
//        static readonly float[] Distances = new float[9];

//        // daño min reduzca el block daño max reduccion y a partir de ahi destruccion
//        const float BlockMinDamage = 20f;
//        const float BlockMaxDamage = 60f;

//        const float BlockWIDEMinDamage = BlockMinDamage * 1.4f;
//        const float BlockWIDEMaxDamage = BlockMaxDamage * 1.4f;

//        float m_BombTime;
//        float m_ExplosionRadius;
//        float m_ActivationTime;
//        float m_MaxDamage;
//        float m_MinDamage;

//        float m_MinExponent;
//        float m_MaxExponent;

//        void SetExplosion(string bombName)
//        {
//            GameObject explosionGO = new GameObject(bombName + "_explosion");
//            explosionGO.transform.Translate(gameObject.transform.position, Space.World);
//            explosionGO.transform.Translate(new Vector3(0.0f, Renderer.size.y * 0.5f, 0.0f), Space.World);
//            var camDir = (CameraManager.Mgr.transform.position - explosionGO.transform.position).normalized;
//            explosionGO.transform.Translate(camDir * 0.05f, Space.World);
//            explosionGO.transform.localScale = new Vector3(2.0f, 2.0f, 1.0f);
//            var vfxCmp = explosionGO.AddComponent<VFXComponent>();
//            vfxCmp.SetVFX(new VFXDef(Def.VFXTarget.GENERAL, bombName + "Explosion", Def.VFXType.CAST, 0, Def.VFXFacing.FaceCameraFull, Def.VFXEnd.SelfDestroy, 24.0f));
//            vfxCmp.ResetVFX(m_BombTime - 0.25f);
//        }

//        public void SetBomb(string bombName, float bombTime, float explosionRadius, float maxDamage, float minDamage)
//        {
//            SetVFX(new VFXDef(Def.VFXTarget.GENERAL, bombName, Def.VFXType.CAST, 0, Def.VFXFacing.FaceCameraFull, Def.VFXEnd.Repeat, 24.0f));
//            ResetVFX();
//            m_BombTime = bombTime;
//            m_ExplosionRadius = explosionRadius;
//            m_MaxDamage = maxDamage;
//            m_MinDamage = minDamage;
//            m_MaxExponent = Mathf.Log(m_MaxDamage);
//            m_MinExponent = Mathf.Log(m_MinDamage);
//            m_ActivationTime = Time.time;
//            SetExplosion(bombName);
//        }

//        public void SetInstaBomb(float explosionRadius, float maxDamage, float minDamage)
//        {
//            m_ExplosionRadius = explosionRadius;
//            m_MinDamage = minDamage;
//            m_MaxDamage = maxDamage;
//            m_MaxExponent = Mathf.Log(m_MaxDamage);
//            m_MinExponent = Mathf.Log(m_MinDamage);
//            OnExplode2();
//        }

//        float ComputeDamage(float distance)
//        {
//            if (distance > m_ExplosionRadius)
//                distance = m_ExplosionRadius;
//            float t = distance / m_ExplosionRadius;
//            float exp = (1 - t) * m_MaxExponent + t * m_MinExponent;
//            return Mathf.Exp(exp);
//        }

//        int ObtainHits(out List<IBlock> blocks, out List<AI.CLivingEntity> les)
//        {
//            var hitCount = Physics.OverlapSphereNonAlloc(transform.position, m_ExplosionRadius, BombCasts);
//            blocks = new List<IBlock>(hitCount);
//            les = new List<AI.CLivingEntity>(hitCount);
//            for (int i = 0; i < hitCount; ++i)
//            {
//                var hit = BombCasts[i];
//                switch(hit.gameObject.layer)
//                {
//                    case Def.RCLayerBlock:
//                        var blockGO = hit.gameObject;
//                        IBlock blockToAdd = null;
//                        if (blockGO.TryGetComponent(out CBlockEdit bedit))
//                        {
//                            blockToAdd = bedit;
//                            if(bedit.IsRemoved())
//                            {
//                                blockToAdd = bedit.GetParentWIDE();
//                            }
//                        }
//                        else if(blockGO.TryGetComponent(out CBlock cblock))
//                        {
//                            blockToAdd = cblock;
//                        }
//                        else
//                        {
//                            Debug.LogWarning("Found a block that is nor CBlock nor CBlockEdit.");
//                            continue;
//                        }
//                        if (!blocks.Contains(blockToAdd))
//                        {
//                            blocks.Add(blockToAdd);
//                        }

//                        break;
//                    case Def.RCLayerBridge:
//                        Debug.Log("Bombs and bridges interaction not done.");
//                        break;
//                    case Def.RCLayerLE:
//                        var le = hit.transform.parent.gameObject.GetComponent<AI.CLivingEntity>();
//                        if (!les.Contains(le))
//                        {
//                            les.Add(le);
//                        }
//                        break;
//                }
//            }
//            return hitCount;
//        }

//        enum ExplosionBlockEffect
//        {
//            ShrinkDown,
//            ShrinkUp,
//            Split
//        }

//        static void ObtainBlockBounds(IBlock block, out float minY, out float maxY, out float blockSize)
//        {
//            maxY = block.transform.position.y;
//            minY = maxY - block.GetLength();
//            if (block.GetBlockType() == Def.BlockType.STAIRS) maxY += 0.5f;
//            const float separation = 1f + Def.BlockSeparation;
//            blockSize = (block.GetBlockType() == Def.BlockType.WIDE ? 2f : 1f);
//            blockSize *= separation;
//        }

//        ExplosionBlockEffect ObtainExplosionBlockEffect(float minBlockY, float maxBlockY)
//        {
//            ExplosionBlockEffect hit = ExplosionBlockEffect.Split;
//            if(transform.position.y >= maxBlockY) // Is over the block
//            {
//                hit = ExplosionBlockEffect.ShrinkDown;
//            }
//            else if(transform.position.y <= minBlockY) // Is under the block
//            {
//                hit = ExplosionBlockEffect.ShrinkUp;
//            }
//            return hit;
//        }

//        float ObtainDistanceBlock(IBlock block, ExplosionBlockEffect effect, float minBlockY, float maxBlockY, float blockSize)
//        {
//            switch (effect)
//            {
//                case ExplosionBlockEffect.ShrinkDown:
//                    Distances[0] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x,                     maxBlockY, block.transform.position.z));
//                    Distances[1] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x + blockSize,         maxBlockY, block.transform.position.z));
//                    Distances[2] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x,                     maxBlockY, block.transform.position.z + blockSize));
//                    Distances[3] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x + blockSize,         maxBlockY, block.transform.position.z + blockSize));
//                    Distances[4] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x + blockSize * 0.5f,  maxBlockY, block.transform.position.z));
//                    Distances[5] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x,                     maxBlockY, block.transform.position.z + blockSize + 0.5f));
//                    Distances[6] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x + blockSize * 0.5f,  maxBlockY, block.transform.position.z + blockSize * 0.5f));
//                    Distances[7] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x + blockSize * 0.5f,  maxBlockY, block.transform.position.z + blockSize));
//                    Distances[8] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x + blockSize,         maxBlockY, block.transform.position.z + blockSize * 0.5f));
//                    break;
//                case ExplosionBlockEffect.ShrinkUp:
//                    Distances[0] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x,                     minBlockY, block.transform.position.z));
//                    Distances[1] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x + blockSize,         minBlockY, block.transform.position.z));
//                    Distances[2] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x,                     minBlockY, block.transform.position.z + blockSize));
//                    Distances[3] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x + blockSize,         minBlockY, block.transform.position.z + blockSize));
//                    Distances[4] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x + blockSize * 0.5f,  minBlockY, block.transform.position.z));
//                    Distances[5] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x,                     minBlockY, block.transform.position.z + blockSize + 0.5f));
//                    Distances[6] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x + blockSize * 0.5f,  minBlockY, block.transform.position.z + blockSize * 0.5f));
//                    Distances[7] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x + blockSize * 0.5f,  minBlockY, block.transform.position.z + blockSize));
//                    Distances[8] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x + blockSize,         minBlockY, block.transform.position.z + blockSize * 0.5f));
//                    break;
//                case ExplosionBlockEffect.Split:
//                    Distances[0] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x,                     transform.position.y, block.transform.position.z));
//                    Distances[1] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x + blockSize,         transform.position.y, block.transform.position.z));
//                    Distances[2] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x,                     transform.position.y, block.transform.position.z + blockSize));
//                    Distances[3] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x + blockSize,         transform.position.y, block.transform.position.z + blockSize));
//                    Distances[4] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x + blockSize * 0.5f,  transform.position.y, block.transform.position.z));
//                    Distances[5] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x,                     transform.position.y, block.transform.position.z + blockSize + 0.5f));
//                    Distances[6] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x + blockSize * 0.5f,  transform.position.y, block.transform.position.z + blockSize * 0.5f));
//                    Distances[7] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x + blockSize * 0.5f,  transform.position.y, block.transform.position.z + blockSize));
//                    Distances[8] = Vector3.Distance(transform.position, new Vector3(block.transform.position.x + blockSize,         transform.position.y, block.transform.position.z + blockSize * 0.5f));
//                    break;
//            }
//            float distance = float.MaxValue;
//            for(int i = 0; i < Distances.Length; ++i)
//            {
//                if (Distances[i] < distance)
//                    distance = Distances[i];
//            }
//            return distance;
//        }

//        float ObtainDestructionAmount(IBlock block, float damage)
//        {
//            float destruction;
//            if (block.GetBlockType() == Def.BlockType.WIDE)
//            {
//                if (damage < BlockWIDEMinDamage)
//                {
//                    destruction = 0f;
//                }
//                else if(damage > BlockWIDEMaxDamage)
//                {
//                    destruction = 1f;
//                }
//                else
//                {
//                    destruction = (damage - BlockWIDEMinDamage) / (BlockWIDEMaxDamage - BlockWIDEMinDamage);
//                }
//            }
//            else
//            {
//                if (damage < BlockMinDamage)
//                {
//                    destruction = 0f;
//                }
//                else if (damage > BlockMaxDamage)
//                {
//                    destruction = 1f;
//                }
//                else
//                {
//                    destruction = (damage - BlockMinDamage) / (BlockMaxDamage - BlockMinDamage);
//                }
//            }
//            return destruction;
//        }

//        void ComputeBlock(IBlock block)
//        {
//            ObtainBlockBounds(block, out float minBlockY, out float maxBlockY, out float blockSize);
//            var effect = ObtainExplosionBlockEffect(minBlockY, maxBlockY);
//            var distance = ObtainDistanceBlock(block, effect, minBlockY, maxBlockY, blockSize);
//            if (distance > m_ExplosionRadius)
//                return;
//            var damage = ComputeDamage(distance);
//            var destruction = ObtainDestructionAmount(block, damage);
//            if (destruction == 0f)
//                return;
//            var nLength = block.GetLength() * (1 - destruction);
//            if(nLength < 0.5f)
//            {
//                if(block is CBlockEdit bedit)
//                {
//                    CBlockEdit[] hiddenBlocks = null;
//                    // should destroy the prop on top
//                    if (bedit.GetBlockType() == Def.BlockType.WIDE)
//                    {
//                        hiddenBlocks = new CBlockEdit[bedit.GetHiddenBlocks().Length];
//                        bedit.GetHiddenBlocks().CopyTo(hiddenBlocks, 0);
//                    }
//                    if (bedit.GetPilar().GetBlocks().Count > 1)
//                    {
//                        bedit.DestroyBlock(true);
//                    }
//                    else
//                    {
//                        bedit.SetLayer(0);
//                        if (Manager.Mgr.HideInfo)
//                        {
//                            bedit.GetTopMR().enabled = false;
//                            bedit.GetCollider().enabled = false;
//                            bedit.GetMidMR().enabled = false;
//                        }
//                    }
//                    if (hiddenBlocks != null)
//                    {
//                        for (int i = 0; i < hiddenBlocks.Length; ++i)
//                        {
//                            var hblock = hiddenBlocks[i];
//                            if (hblock.GetPilar().GetBlocks().Count > 1)
//                            {
//                                hblock.DestroyBlock(true);
//                            }
//                            else
//                            {
//                                hblock.SetRemoved(false);
//                                hblock.SetLayer(0);
//                                if (Manager.Mgr.HideInfo)
//                                {
//                                    hblock.GetTopMR().enabled = false;
//                                    hblock.GetCollider().enabled = false;
//                                    hblock.GetMidMR().enabled = false;
//                                }
//                            }
//                        }
//                    }
//                }
//                else if(block is CBlock)
//                {
//                    block.DestroyBlock(true);
//                }
                
//                return;
//            }

//            if (effect == ExplosionBlockEffect.Split)
//            {
//                float distanceToMax = Mathf.Abs(maxBlockY - transform.position.y);
//                float distanceToMin = Mathf.Abs(minBlockY - transform.position.y);
//                if (distanceToMax > distanceToMin)
//                {
//                    effect = ExplosionBlockEffect.ShrinkUp;
//                }
//                else
//                {
//                    effect = ExplosionBlockEffect.ShrinkDown;
//                }
//            }

//            switch (effect)
//            {
//                case ExplosionBlockEffect.ShrinkDown:
//                    {
//                        float iPart = Mathf.Floor(nLength);
//                        float fPart = nLength - iPart;
//                        if (fPart >= 0.5f)
//                            nLength = iPart + 0.5f;
//                        else
//                            nLength = iPart;
//                        var diff = block.GetLength() - nLength;

//                        block.SetLength(nLength);
//                        block.SetHeight(block.GetHeight() + block.GetMicroHeight() - diff);
//                        if (block is CBlockEdit edit)
//                            edit.SetMicroHeight(0f);
//                        else
//                            block.SetMicroHeight(0f);
//                    }
//                    break;
//                case ExplosionBlockEffect.ShrinkUp:
//                    {
//                        float iPart = Mathf.Floor(nLength);
//                        float fPart = nLength - iPart;
//                        if (fPart >= 0.5f)
//                            nLength = iPart + 0.5f;
//                        else
//                            nLength = iPart;
//                        var diff = block.GetLength() - nLength;

//                        block.SetLength(nLength);
//                        block.SetHeight(block.GetHeight() + block.GetMicroHeight() + diff);
//                        if (block is CBlockEdit edit)
//                            edit.SetMicroHeight(0f);
//                        else
//                            block.SetMicroHeight(0f);
//                    }
//                    break;
//            }
//        }

//        void ComputeLE(AI.CLivingEntity le)
//        {
//            float maxHeight = le.transform.position.y + le.GetHeight();
//            float minHeight = le.transform.position.y;
//            float height = transform.position.y;
//            if (transform.position.y > maxHeight)
//                height = maxHeight;
//            else if (transform.position.y < minHeight)
//                height = minHeight;

//            float distance = Vector3.Distance(transform.position, new Vector3(le.transform.position.x, height, le.transform.position.z));
//            float damage = ComputeDamage(distance);
//            le.ReceiveDamage(null, Def.DamageType.FIRE, damage);
//        }

//        void OnExplode2()
//        {
//            var hitCount = ObtainHits(out List<IBlock> blocks, out List<AI.CLivingEntity> les);

//            for (int i = 0; i < les.Count; ++i)
//            {
//                ComputeLE(les[i]);
//            }

//            for(int i = 0; i < blocks.Count; ++i)
//            {
//                ComputeBlock(blocks[i]);
//            }

//            GameUtils.DeleteGameobject(gameObject);
//        }

//        //void OnExplode()
//        //{
//        //    float diagMult = Mathf.Sin(Mathf.PI * 0.25f);
//        //    List<BlockComponent> affectedBlocks = new List<BlockComponent>(Mathf.CeilToInt(m_ExplosionRadius * 6.0f));
//        //    var bombMapID = GameUtils.MapIDFromPosition(new Vector2(transform.position.x, transform.position.z));
//        //    var bombPilar = Manager.Mgr.Pilars[bombMapID];
//        //    affectedBlocks.AddRange(bombPilar.Blocks);
//        //    Vector2[] checkPos = new Vector2[8];
//        //    // Obtain all the affected blocks by the explosion
//        //    for(float currentCheck = 0.5f; currentCheck < m_ExplosionRadius; currentCheck += 0.5f)
//        //    {
//        //        float checkDiag = currentCheck * diagMult;
//        //        checkPos[0] = new Vector2(transform.position.x + currentCheck, transform.position.z);
//        //        checkPos[1] = new Vector2(transform.position.x, transform.position.z + currentCheck);
//        //        checkPos[2] = new Vector2(transform.position.x - currentCheck, transform.position.z);
//        //        checkPos[3] = new Vector2(transform.position.x, transform.position.z - currentCheck);
//        //        checkPos[4] = new Vector2(transform.position.x + checkDiag, transform.position.z + checkDiag);
//        //        checkPos[5] = new Vector2(transform.position.x + checkDiag, transform.position.z - checkDiag);
//        //        checkPos[6] = new Vector2(transform.position.x - checkDiag, transform.position.z + checkDiag);
//        //        checkPos[7] = new Vector2(transform.position.x - checkDiag, transform.position.z - checkDiag);
//        //        for (int i = 0; i < checkPos.Length; ++i)
//        //        {
//        //            var curPilarID = GameUtils.MapIDFromPosition(checkPos[i]);
//        //            var curPilar = Manager.Mgr.Pilars[curPilarID];
//        //            for(int j = 0; j < curPilar.Blocks.Count; ++j)
//        //            {
//        //                if (!affectedBlocks.Contains(curPilar.Blocks[j]))
//        //                    affectedBlocks.Add(curPilar.Blocks[j]);
//        //            }
//        //        }
//        //    }
//        //    List<Vector2Int> blownUpWides = new List<Vector2Int>();
//        //    for(int i = 0; i < affectedBlocks.Count; ++i)
//        //    {
//        //        var currentBlock = affectedBlocks[i];
//        //        Vector3 checkBlockPos = new Vector3(currentBlock.Pilar.transform.position.x + 0.5f, currentBlock.Height + currentBlock.MicroHeight, currentBlock.Pilar.transform.position.z + 0.5f);
//        //        float blockLength = currentBlock.Length;
//        //        bool isWide = currentBlock.blockType == BlockType.WIDE || currentBlock.Removed;
//        //        float distanceHigh = Vector3.Distance(transform.position, checkBlockPos);
//        //        float distanceMid = Vector3.Distance(transform.position, new Vector3(checkBlockPos.x, checkBlockPos.y - blockLength * 0.5f, checkBlockPos.z));
//        //        float distanceLow = Vector3.Distance(transform.position, new Vector3(checkBlockPos.x, checkBlockPos.y - blockLength, checkBlockPos.z));
//        //        float highDamage = 0.0f;
//        //        float midDamage = 0.0f;
//        //        float lowDamage = 0.0f;
//        //        float distEncoded = 0.0f;
//        //        if (distanceHigh <= m_ExplosionRadius)
//        //        {
//        //            distEncoded = distanceHigh / m_ExplosionRadius;
//        //            highDamage = (1.0f - distEncoded) * m_MaxDamage + distEncoded * m_MinDamage;
//        //        }
//        //        if (distanceMid <= m_ExplosionRadius)
//        //        {
//        //            distEncoded = distanceMid / m_ExplosionRadius;
//        //            midDamage = (1.0f - distEncoded) * m_MaxDamage + distEncoded * m_MinDamage;
//        //        }
//        //        if (distanceLow <= m_ExplosionRadius)
//        //        {
//        //            distEncoded = distanceLow / m_ExplosionRadius;
//        //            lowDamage = (1.0f - distEncoded) * m_MaxDamage + distEncoded * m_MinDamage;
//        //        }
//        //        float damage = (highDamage + midDamage + lowDamage) / 3.0f;

//        //        //float wideHeight = 0.0f;
//        //        Vector2Int wideBlockPos = Vector2Int.zero;

//        //        if (isWide)
//        //        {
//        //            if(damage >= BlockWideDestroyDamage)
//        //            {
//        //                BlockComponent WideBlock = null;
//        //                if(currentBlock.blockType == BlockType.WIDE)
//        //                {
//        //                    WideBlock = currentBlock;
//        //                    //wideBlockPos = GameUtils.PosFromID(currentBlock.Pilar.MapID, Manager.MapWidth, Manager.MapHeight);
//        //                }
//        //                else
//        //                {
//        //                    WideBlock = currentBlock.WIDEParent;
//        //                }
//        //                //else
//        //                //{
//        //                //    Vector2Int currentBlockPos = GameUtils.PosFromID(currentBlock.Pilar.MapID, Manager.MapWidth, Manager.MapHeight);
//        //                //    wideHeight = currentBlock.Height + currentBlock.MicroHeight;
//        //                //    // Find the WIDE one
//        //                //    Vector2Int[] possibleWidePos = new Vector2Int[]
//        //                //    {
//        //                //        new Vector2Int(currentBlockPos.x, currentBlockPos.y - 1),
//        //                //        new Vector2Int(currentBlockPos.x - 1, currentBlockPos.y),
//        //                //        new Vector2Int(currentBlockPos.x - 1, currentBlockPos.y - 1)
//        //                //    };
//        //                //    for(int j = 0; j < possibleWidePos.Length; ++j)
//        //                //    {
//        //                //        var pilarID = GameUtils.IDFromPos(possibleWidePos[j], Manager.MapWidth, Manager.MapHeight);
//        //                //        var pilar = Manager.Mgr.Pilars[pilarID];
//        //                //        for(int k = 0; k < pilar.Blocks.Count; ++k)
//        //                //        {
//        //                //            var b = pilar.Blocks[k];
//        //                //            if (b == null || (b != null && (b.Layer == 0 || b.Height != currentBlock.Height || b.MicroHeight != currentBlock.MicroHeight || b.blockType != BlockType.WIDE)))
//        //                //                continue;
//        //                //            WideBlock = b;
//        //                //            wideBlockPos = possibleWidePos[j];
//        //                //            break;
//        //                //        }
//        //                //        if (WideBlock != null)
//        //                //            break;
//        //                //    }
//        //                //}
//        //                wideBlockPos = GameUtils.PosFromID(WideBlock.Pilar.MapID, Manager.MapWidth, Manager.MapHeight);
//        //                if (WideBlock != null)
//        //                {
//        //                    if (!blownUpWides.Contains(wideBlockPos))
//        //                    {
//        //                        currentBlock = WideBlock;
//        //                        blownUpWides.Add(wideBlockPos);
//        //                        if (affectedBlocks.Contains(WideBlock))
//        //                            affectedBlocks.Remove(WideBlock);
//        //                    }
//        //                }
//        //                else
//        //                {
//        //                    Debug.LogWarning("Couldn't find the WIDE block in the explosion.");
//        //                    currentBlock = null;
//        //                }
//        //            }
//        //        }
//        //        if(currentBlock != null && damage >= BlockDestroyDamage)
//        //        {
//        //            var maxLength = Mathf.Abs(BlockMeshDef.MidMesh.VertexHeight[0].y);
//        //            // Destroy the block compleatly
//        //            if (distanceHigh <= m_ExplosionRadius && distanceLow <= m_ExplosionRadius)
//        //            {
//        //                if (currentBlock.Pilar.Blocks.Count == 1)
//        //                {
//        //                    var nBlock = currentBlock.Pilar.AddBlock();
//        //                    if (Manager.Mgr.HideInfo)
//        //                    {
//        //                        //nBlock.TopBC.enabled = false;
//        //                        nBlock.TopMR.enabled = false;
//        //                        //nBlock.MidBC.enabled = false;
//        //                        nBlock.MidMR.enabled = false;
//        //                        nBlock.BlockBC.enabled = false;
//        //                    }
//        //                }
//        //                currentBlock.DestroyBlock();
//        //                currentBlock = null;
//        //            }
//        //            // Destroy part of the block, leaving part of the above
//        //            else if (distanceHigh > m_ExplosionRadius)
//        //            {
//        //                var newLength = distanceHigh - m_ExplosionRadius;
//        //                if (newLength < 0.5f)
//        //                {
//        //                    newLength = 0.5f;
//        //                }
//        //                else
//        //                {
//        //                    int iPart = Mathf.FloorToInt(newLength);
//        //                    float decimals = newLength - iPart;
//        //                    if (decimals > 0.65f)
//        //                        newLength = iPart + 1.0f;
//        //                    else if (decimals > 0.35f)
//        //                        newLength = iPart + 0.5f;
//        //                    else
//        //                        newLength = iPart;
//        //                }
//        //                if (newLength > maxLength)
//        //                    newLength = maxLength;

//        //                currentBlock.Length = newLength;
//        //            }
//        //            // Destroy part of the block, leaving part of the bottom
//        //            else if (distanceLow > m_ExplosionRadius)
//        //            {
//        //                float newLength = distanceLow - m_ExplosionRadius;
//        //                currentBlock.Height = transform.position.y - m_ExplosionRadius;
//        //                currentBlock.MicroHeight = 0.0f;
//        //                if (newLength < 0.5f)
//        //                {
//        //                    newLength = 0.5f;
//        //                }
//        //                else
//        //                {
//        //                    int iPart = Mathf.FloorToInt(newLength);
//        //                    float decimals = newLength - iPart;
//        //                    if (decimals > 0.65f)
//        //                        newLength = iPart + 1.0f;
//        //                    else if (decimals > 0.35f)
//        //                        newLength = iPart + 0.5f;
//        //                    else
//        //                        newLength = iPart;
//        //                }
//        //                if (newLength > maxLength)
//        //                    newLength = maxLength;

//        //                currentBlock.Length = newLength;
//        //            }
//        //        }
//        //        if(isWide)
//        //        {
//        //            Vector2Int[] wideRemovedBlocks = new Vector2Int[3]
//        //            {
//        //                new Vector2Int(wideBlockPos.x + 1, wideBlockPos.y),
//        //                new Vector2Int(wideBlockPos.x, wideBlockPos.y + 1),
//        //                new Vector2Int(wideBlockPos.x + 1, wideBlockPos.y + 1)
//        //            };
//        //            int removedLayer = 0;
//        //            float removedHeight = 0.0f;
//        //            float removedMicroHeight = 0.0f;
//        //            if(currentBlock != null)
//        //            {
//        //                removedLayer = currentBlock.Layer;
//        //                removedHeight = currentBlock.Height;
//        //                removedMicroHeight = currentBlock.MicroHeight;
//        //            }
//        //            for(int j = 0; j < wideRemovedBlocks.Length; ++j)
//        //            {
//        //                var pilarID = GameUtils.IDFromPos(wideRemovedBlocks[j], Manager.MapWidth, Manager.MapHeight);
//        //                var pilar = Manager.Mgr.Pilars[pilarID];
//        //                for(int k = 0; k < pilar.Blocks.Count; ++k)
//        //                {
//        //                    var remBlock = pilar.Blocks[k];
//        //                    if (remBlock == null || (remBlock != null && !remBlock.Removed))
//        //                        continue;

//        //                    remBlock.Removed = false;
//        //                    remBlock.Layer = removedLayer;
//        //                    remBlock.LayerSR.enabled = false;
//        //                    remBlock.Height = removedHeight;
//        //                    remBlock.MicroHeight = removedMicroHeight;

//        //                    break;
//        //                }
//        //            }
//        //        }
//        //    }

//        //    // Check all structures near the bomb and check if any livingEntity has receive bomb damage
//        //    Vector2[] checkStrucPos = new Vector2[8];
//        //    for(int i = 0; i < Manager.Mgr.Strucs.Count; ++i)
//        //    {
//        //        var curStruc = Manager.Mgr.Strucs[i];
//        //        if (curStruc.LivingEntities.Count == 0)
//        //            continue;
//        //        checkStrucPos[0] = new Vector2(curStruc.transform.position.x, curStruc.transform.position.z);
//        //        checkStrucPos[1] = new Vector2(curStruc.transform.position.x + StructureComponent.Width, curStruc.transform.position.z);
//        //        checkStrucPos[2] = new Vector2(curStruc.transform.position.x, curStruc.transform.position.z + StructureComponent.Height);
//        //        checkStrucPos[3] = new Vector2(curStruc.transform.position.x + StructureComponent.Width, curStruc.transform.position.z + StructureComponent.Height);
//        //        checkStrucPos[4] = new Vector2(curStruc.transform.position.x + StructureComponent.Width * 0.5f, curStruc.transform.position.z);
//        //        checkStrucPos[5] = new Vector2(curStruc.transform.position.x, curStruc.transform.position.z + StructureComponent.Height * 0.5f);
//        //        checkStrucPos[6] = new Vector2(curStruc.transform.position.x + StructureComponent.Width, curStruc.transform.position.z + StructureComponent.Height * 0.5f);
//        //        checkStrucPos[7] = new Vector2(curStruc.transform.position.x + StructureComponent.Width * 0.5f, curStruc.transform.position.z + StructureComponent.Height);
//        //        bool checkStruc = false;
//        //        for(int j = 0; j < checkStrucPos.Length; ++j)
//        //        {
//        //            float strucDistance = Vector2.Distance(checkStrucPos[j], new Vector2(transform.position.x, transform.position.z));
//        //            if(strucDistance <= m_ExplosionRadius)
//        //            {
//        //                checkStruc = true;
//        //                break;
//        //            }
//        //        }
//        //        if(checkStruc)
//        //        {
//        //            for(int j = 0; j < curStruc.LivingEntities.Count; ++j)
//        //            {
//        //                var curEntity = curStruc.LivingEntities[j];
//        //                float entityDistance = Vector3.Distance(curEntity.transform.position, transform.position);
//        //                if (entityDistance <= m_ExplosionRadius)
//        //                {
//        //                    float distanceEncoded = entityDistance / m_ExplosionRadius;
//        //                    float damage = (1.0f - distanceEncoded) * m_MaxDamage + distanceEncoded * m_MinDamage;
//        //                    curEntity.ReceiveDamage(Def.DamageType.FIRE, damage);
//        //                }
//        //            }
//        //        }
//        //    }

//        //    var oddPos = Manager.Mgr.OddGO.transform.position;
//        //    var oddDistance = Vector3.Distance(oddPos, transform.position);
//        //    if (oddDistance <= m_ExplosionRadius)
//        //    {
//        //        float oddDistanceEncoded = oddDistance / m_ExplosionRadius;
//        //        float damage = (1.0f - oddDistanceEncoded) * m_MaxDamage + oddDistanceEncoded * m_MinDamage;
//        //        Manager.Mgr.OddGO.GetComponent<LivingEntity>().ReceiveDamage(Def.DamageType.FIRE, damage);
//        //    }

//        //    // Todo spawn VFX

//        //    GameUtils.DeleteGameObjectAndItsChilds(gameObject);
//        //}

//        private void Update()
//        {
//            var timeOffset = Time.time - m_ActivationTime;
//            var timeEncoded = timeOffset / m_BombTime;
//            var scale = (1.0f - timeEncoded) * 0.5f + timeEncoded * 1.25f;
//            transform.localScale = new Vector3(scale, scale, 1.0f);
//            m_WaitTime = 1.0f / (VFXInfo.Frames.Length * Mathf.Exp(timeOffset));
//            OnUpdate();
//            if ((m_ActivationTime + m_BombTime) <= Time.time)
//                OnExplode2();
//        }
//    }
//}
