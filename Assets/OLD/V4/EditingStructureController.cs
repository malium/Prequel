/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEngine.UI;

//namespace Assets
//{
//    public enum EditingStructureMode
//    {
//        None,
//        Structure,
//        Layer,
//        StructureMenu,

//        COUNT
//    }

//    public class EditingStructureController : IGameController
//    {
//        public const float ModificationWait = 0.2f;

//        //bool IsMouseHeld;
//        List<CBlockEdit> Selected;
//        CBlockEdit BlockOver;
//        float lastModificationTime;

//        EditingStructureMode editingMode;

//        LayerEditor layerEditor;
//        StructureEditor structureEditor;
//        StructureMenu structureMenu;

//        bool IsSelected(int cellID, float y)
//        {
//            var struc = Manager.Mgr.Structure;
//            if (cellID >= struc.Pilars.Length)
//                return false;
//            var blocks = struc.Pilars[cellID];
//            foreach(var block in blocks.Blocks)
//            {
//                if (block.transform.position.y == y)
//                    return block.Selected;
//            }
//            return false;
//        }

//        public void ToggleEditingVisibility(bool screenShooting = false, bool screenShootingStart = false)
//        {
//            CStrucEdit struc = Manager.Mgr.Structure;
//            var strucIE = Structures.Strucs[struc.IDXIE];
//            var blocksIE = strucIE.GetBlocks();
//            bool hideInfo = false;
//            if(!screenShooting)
//            {
//                hideInfo = !Manager.Mgr.HideInfo;
//                Manager.Mgr.HideInfo = hideInfo;
//            }
//            else
//            {
//                hideInfo = screenShootingStart;
//            }                

//            // ToggleVisibility of layer0 blocks and non-layer0 info sprites
//            for (int i = 0; i < struc.Pilars.Length; ++i)
//            {
//                var pilar = struc.Pilars[i];
//                for (int j = 0; j < pilar.GetBlocks().Count; ++j)
//                {
//                    var block = (CBlockEdit)pilar.GetBlocks()[j];
//                    if (block.GetLayer() != 0)
//                    {
//                        if (block.IsRemoved())
//                            continue;
//                        var blockIE = blocksIE[block.GetIDXIE()];
//                        block.GetLayerRnd().enabled = !hideInfo;
//                        if(block.IsAnchor())
//                            block.GetAnchorRnd().enabled = !hideInfo;
//                        if(block.GetStairState() == Def.StairState.POSSIBLE)
//                            block.GetStairRnd().enabled = !hideInfo;
//                        block.SetSelected(false);
//                    }
//                    else // layer 0
//                    {
//                        block.GetTopMR().enabled = !hideInfo;
//                        //block.TopBC.enabled = !hideInfo;
//                        block.GetMidMR().enabled = !hideInfo;
//                        //block.MidBC.enabled = !hideInfo;
//                        block.GetCollider().enabled = !hideInfo;
//                    }
//                }
//            }
//            Selected.Clear();
//            // Apply possible stair
//            struc.ApplyStairs(hideInfo);
//            // Convert into WIDE the valid blocks
//            struc.ApplyWides(hideInfo);
//            // Toggle visibility of monsters, props and lock sprite
//            for (int i = 0; i < struc.Pilars.Length; ++i)
//            {
//                var pilar = struc.Pilars[i];
//                for (int j = 0; j < pilar.Blocks.Count; ++j)
//                {
//                    var block = pilar.Blocks[j];
//                    if (block.Layer == 0)
//                        continue;

//                    bool hideLock = hideInfo || block.Removed;

//                    if (hideLock)
//                    {
//                        block.LockSR.enabled = false;
//                    }
//                    if(!hideLock && block.Locked != BlockLock.Unlocked)
//                    {
//                        block.LockSR.enabled = true;
//                    }

//                    if (block.blockType == BlockType.STAIRS)
//                        continue;
//                    if (block.Prop != null)
//                    {
//                        block.Prop.SpriteSR.enabled = hideInfo;
//                        block.Prop.SpriteBC.enabled = hideInfo;
//                        if(block.Prop.ShadowSR != null)
//                            block.Prop.ShadowSR.enabled = hideInfo;
//                        if (block.Prop.PropLight != null)
//                            block.Prop.PropLight.enabled = hideInfo;
//                    }
//                    if (block.Monster != null)
//                    {
//                        block.Monster.SpriteSR.enabled = hideInfo;
//                        //block.Monster.SpriteBC.enabled = hideInfo;
//                        block.Monster.SpriteCC.enabled = hideInfo;
//                        block.Monster.ShadowSR.enabled = hideInfo;
//                    }
//                }
//            }
//        }

//        private void RemoveSelected(Vector3 position)
//        {
//            for (int i = 0; i < Selected.Count; ++i)
//            {
//                if (Selected[i].TopMR.transform.position == position)
//                {
//                    Selected.RemoveAt(i);
//                    return;
//                }
//            }
//        }

//        public EditingStructureController()
//        {

//        }

//        void OnSingleRayCast()
//        {
//            var struc = Manager.Mgr.Structure;

//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            GameObject goOver = null;
//            if (Physics.Raycast(ray, out RaycastHit mouseHit, 10000f))
//            {
//                goOver = mouseHit.transform.gameObject;
//                if(goOver.layer == Manager.BlockLayer)
//                //if (goOver.tag == BlockComponent.BlockTag)
//                {
//                    var blockGO = goOver;//.transform.parent.gameObject;
//                    var block = blockGO.GetComponent<BlockComponent>();
//                    if (BlockOver != null && BlockOver != block)
//                        BlockOver.Highlighted = false;
//                    BlockOver = block;
//                }
//            }
//            else
//            {
//                if (BlockOver != null)
//                    BlockOver.Highlighted = false;
//                BlockOver = null;
//            }
//        }

//        public void FixedUpdate()
//        {
//            if (Manager.Mgr.HideInfo)
//                return;

//            if(BlockOver != null)
//            {
//                BlockOver.Highlighted = true;
//            }
//            OnSingleRayCast();

//            //if (BlockOver == null)
//            //{
//            //    if (mouseClicked && !wasMouseHeld && !shiftPressed)
//            //    {
//            //        foreach (var block in Selected)
//            //            block.Selected = false;
//            //        Selected.Clear();
//            //    }
//            //    return;
//            //}
//        }

//        public GameState GetGameState()
//        {
//            return GameState.EDITING_STRUCTURE;
//        }

//        public void OnGUI()
//        {
//            if(editingMode == EditingStructureMode.Layer)
//            {
//                var finish = layerEditor.OnGUI();
//                if (finish)
//                    editingMode = EditingStructureMode.None;
//                return;
//            }
//            else if(editingMode == EditingStructureMode.Structure)
//            {
//                var finish = structureEditor.OnGUI();
//                if (finish)
//                    editingMode = EditingStructureMode.None;
//                return;
//            }
//            else if(editingMode == EditingStructureMode.StructureMenu)
//            {
//                var finish = structureMenu.OnGUI();
//                if (finish)
//                    editingMode = EditingStructureMode.None;
//                return;
//            }

//            if (Manager.Mgr.HideInfo)
//                return;

//            var rect = Manager.Mgr.m_Canvas.pixelRect;
//            var struc = Manager.Mgr.Structure;
//            float lastHeight = 5.0f;
//            bool strucEdit = GUI.Button(new Rect(5.0f, lastHeight, 75.0f, 20.0f), "Struc Edit");
//            if(strucEdit)
//            {
//                structureMenu.Reset();
//                structureEditor.Reset();
//                editingMode = EditingStructureMode.Structure;
//            }
//            lastHeight += 20.0f;
//            bool layerEdit = GUI.Button(new Rect(5.0f, lastHeight, 75.0f, 20.0f), "Layer Edit");
//            if(layerEdit)
//            {
//                layerEditor.Reset();
//                editingMode = EditingStructureMode.Layer;
//            }
//            lastHeight += 20.0f;


//            if (GUI.Button(new Rect(rect.width * 0.5f - 150f, rect.height - 30f, 100f, 30f), "Rotate 90º"))
//            {
//                var rot = struc.Rotation;
//                if (rot + 1 == BlockRotation.COUNT)
//                    struc.Rotation = BlockRotation.Default;
//                else
//                    struc.Rotation = rot + 1;
//            }
//            if(GUI.Button(new Rect(rect.width * 0.5f - 50f, rect.height - 30f, 100f, 30f), "Horz Flip"))
//            {
//                switch (struc.Flip)
//                {
//                    case StructureFlip.NoFlip:
//                        struc.Flip = StructureFlip.HorizontalFlip;
//                        break;
//                    case StructureFlip.VerticalFlip:
//                        struc.Flip = StructureFlip.VerticalAndHorizontalFlip;
//                        break;
//                    case StructureFlip.HorizontalFlip:
//                        struc.Flip = StructureFlip.NoFlip;
//                        break;
//                    case StructureFlip.VerticalAndHorizontalFlip:
//                        struc.Flip = StructureFlip.VerticalFlip;
//                        break;
//                }
//            }
//            if (GUI.Button(new Rect(rect.width * 0.5f + 50f, rect.height - 30f, 100f, 30f), "Vert Flip"))
//            {
//                switch (struc.Flip)
//                {
//                    case StructureFlip.NoFlip:
//                        struc.Flip = StructureFlip.VerticalFlip;
//                        break;
//                    case StructureFlip.VerticalFlip:
//                        struc.Flip = StructureFlip.NoFlip;
//                        break;
//                    case StructureFlip.HorizontalFlip:
//                        struc.Flip = StructureFlip.VerticalAndHorizontalFlip;
//                        break;
//                    case StructureFlip.VerticalAndHorizontalFlip:
//                        struc.Flip = StructureFlip.HorizontalFlip;
//                        break;
//                }
//            }

//            //bool hReset = GUI.Button(new Rect(5.0f, lastHeight, 75.0f, 20.0f), "HReset");
//            //lastHeight += 20.0f;
//            //if(hReset)
//            //{
//            //    for(int i = 0; i < struc.Blocks.Length; ++i)
//            //    {
//            //        for(int j = 0; j < struc.Blocks[i].BlockDefs.Count; ++j)
//            //        {
//            //            struc.Blocks[i].BlockDefs[j].Height = 0.0f;
//            //        }
//            //    }
//            //}
//            bool sel = GUI.Button(new Rect(5.0f, lastHeight, 75.0f, 20.0f), "Select All");
//            lastHeight += 20.0f;
//            if (sel)
//            {
//                Selected.Clear();
//                for (int i = 0; i < struc.Pilars.Length; ++i)
//                {
//                    for (int j = 0; j < struc.Pilars[i].Blocks.Count; ++j)
//                    {
//                        if (struc.Pilars[i].Blocks[j].Removed)
//                            continue;
//                        struc.Pilars[i].Blocks[j].Selected = true;
//                        Selected.Add(struc.Pilars[i].Blocks[j]);
//                    }
//                }
//            }
            
//            lastHeight += 25.0f;

//            bool menu = GUI.Button(new Rect(rect.width * 0.5f - 75.0f, 0.0f, 150.0f, 25.0f), "Structure Menu");
//            if(menu)
//            {
//                structureMenu.Reset();
//                editingMode = EditingStructureMode.StructureMenu;
//            }

//            bool validLayer = false;
//            for(int i = 0; i < StructureComponent.LayerAmount; ++i)
//            {
//                if(struc.InfoLayers[i].IsValid())
//                {
//                    validLayer = true;
//                    break;
//                }
//            }
//            if(validLayer)
//            {
//                bool reaply = GUI.Button(new Rect(5.0f, lastHeight + 50.0f, 100.0f, 25.0f), "Reapply layers");
//                if(reaply)
//                {
//                    for(int i = 0; i < struc.LivingEntities.Count; ++i)
//                    {
//                        var le = struc.LivingEntities[i];
//                        le.ReceiveDamage(Def.DamageType.UNAVOIDABLE, le.GetTotalHealth());
//                    }
//                    struc.LivingEntities.Clear();
//                    for(int i = 0; i < StructureComponent.LayerAmount; ++i)
//                    {
//                        if (!struc.InfoLayers[i].IsValid())
//                            continue;
//                        struc.SetLayer(i + 1, struc.InfoLayers[i]);
//                    }
//                }
//                bool resetLock = GUI.Button(new Rect(5.0f, lastHeight + 75.0f, 100.0f, 25.0f), "Reset Locks");
//                if(resetLock)
//                {
//                    var structIE = Structures.Strucs[struc.IDXIE];
//                    var blocks = structIE.Blocks;
//                    for (int i = 0; i < struc.Pilars.Length; ++i)
//                    {
//                        var pilar = struc.Pilars[i];
//                        for(int j = 0; j < pilar.Blocks.Count; ++j)
//                        {
//                            var block = pilar.Blocks[j];
//                            if (block.Removed || block.Layer < 1)
//                                continue;

//                            var blockIE = blocks[block.IDXIE];

//                            block.Locked = BlockLock.Unlocked;
//                            if (block.Stair != StairType.NONE)
//                            {
//                                blockIE.SetDefault();
//                                blockIE.Layer = (byte)block.Layer;
//                                blockIE.Anchor = block.Anchor;
//                                blockIE.blockType = block.blockType;
//                                blockIE.Stair = block.Stair;
//                                blockIE.BlockRotation = block.Rotation;
//                                block.Locked = BlockLock.SemiLocked;
//                            }
//                            // unlock the block properties but is a wide block
//                            else if (block.blockType == BlockType.WIDE)
//                            {
//                                blockIE.SetDefault();
//                                blockIE.Layer = (byte)block.Layer;
//                                blockIE.Anchor = block.Anchor;
//                                blockIE.blockType = block.blockType;
//                                block.Locked = BlockLock.SemiLocked;
//                            }
//                            // unlock the block properties
//                            else
//                            {
//                                blockIE.SetDefault();
//                                blockIE.Layer = (byte)block.Layer;
//                                blockIE.Anchor = block.Anchor;
//                            }
//                        }
//                    }
//                }

//                //var ie = Structures.Strucs[struc.IDXIE];
//                //if(ie.m_BlockNum > 0 && Structures.IsStrucModified(struc.IDXIE))
//                //{
//                //    bool save = GUI.Button(new Rect(5.0f, lastHeight + 75.0f, 100.0f, 25.0f), "SAVE");
//                //    if(save)
//                //    {
//                //        Structures.SaveStruc(ie.StructureID);
//                //    }
//                //}
//            }

//        }

//        void SetInitialStructure()
//        {
//            var strucGO = new GameObject("InvalidStruc");
//            var struc = strucGO.AddComponent<StructureComponent>();
//            struc.StructRect = new Rect(0.0f, 0.0f, StructureComponent.Width * (1.0f + StructureComponent.Separation), StructureComponent.Height * (1.0f + StructureComponent.Separation));

//            var id = Structures.AddStructure();
//            struc.IDXIE = id;

//            for (int y = 0; y < StructureComponent.Height; ++y)
//            {
//                var yOffset = y * StructureComponent.Width;
//                for(int x = 0; x < StructureComponent.Width; ++x)
//                {
//                    var idx = yOffset + x;

//                    var structID = GameUtils.IDFromPos(new Vector2Int(x, y));
//                    var pilarGO = new GameObject("InvalidPilar");
//                    var pilar = pilarGO.AddComponent<PilarComponent>();
//                    struc.Pilars[idx] = pilar;
//                    pilar.Init(struc, structID);

//                    pilar.AddBlock();
//                }
//            }
//            Manager.Mgr.Structure = struc;
//        }

//        public void Start()
//        {
//            Selected = new List<BlockComponent>();
//            BlockOver = null;
//            lastModificationTime = 0.0f;

//            SetInitialStructure();
//            editingMode = EditingStructureMode.None;
//            Manager.Mgr.HideInfo = false;
//            layerEditor = new LayerEditor();
//            layerEditor.Start();
//            structureEditor = new StructureEditor();
//            structureEditor.Start();
//            structureMenu = new StructureMenu(this);
//            structureMenu.Start();

//            var oddScript = Manager.Mgr.OddGO.GetComponent<OddScript>();
//            var oddPos = new Vector2((StructureComponent.Width / 2) * (1.0f + StructureComponent.Separation), (StructureComponent.Height / 2) * (1.0f + StructureComponent.Separation));
//            oddScript.MoveTo(oddPos);
//            oddScript.Position = new Vector3(oddPos.x, 0.0f, oddPos.y);
//            oddScript.transform.SetPositionAndRotation(oddScript.Position, Quaternion.identity);
//            oddScript.gameObject.SetActive(false);
//        }

//        public void Stop()
//        {
//            Selected.Clear();
//            var struc = Manager.Mgr.Structure;
//            for (int i = 0; i < struc.Pilars.Length; ++i)
//            {
//                var pilar = struc.Pilars[i];
//                if (pilar == null)
//                    continue;
//                for(int j = 0; j < pilar.Blocks.Count; ++j)
//                {
//                    var block = pilar.Blocks[j];
//                    if (block == null)
//                        continue;
//                    if (block.Prop != null)
//                        block.Prop.ReceiveDamage(Def.DamageType.UNAVOIDABLE, block.Prop.GetTotalHealth());
//                    if (block.Monster != null)
//                        block.Monster.ReceiveDamage(Def.DamageType.UNAVOIDABLE, block.Monster.GetTotalHealth());
//                }
//                pilar.DestroyPilar();
//            }

//            layerEditor.Stop();
//            layerEditor = null;

//            structureEditor.Stop();
//            structureEditor = null;

//            structureMenu.Stop();
//            structureMenu = null;

//            GameUtils.DeleteGameObjectAndItsChilds(struc.gameObject, true);

//            Manager.Mgr.Structure = null;
//        }

//        private int AddBlockToStructure(ref List<IE.V2.BlockIE> blocks)
//        {
//            int idx = -1;
//            for (int j = 0; j < blocks.Count; ++j)
//            {
//                if (blocks[j] == null)
//                {
//                    idx = j;
//                    break;
//                }
//            }
//            if(idx < 0)
//            {
//                idx = blocks.Count;
//                blocks.Add(null);
//            }
//            return idx;
//        }

//        void OnDeleteBlock()
//        {
//            var struc = Manager.Mgr.Structure;
//            var structIE = Structures.Strucs[struc.IDXIE];
//            var ieBlocks = structIE.Blocks;
//            for (int i = 0; i < Selected.Count; ++i)
//            {
//                var selected = Selected[i];

//                if (selected.IDXIE >= 0)
//                {
//                    ieBlocks.RemoveAt(selected.IDXIE);
//                    for (int j = 0; j < struc.Pilars.Length; ++j)
//                    {
//                        var blocks = struc.Pilars[j];
//                        for (int k = 0; k < blocks.Blocks.Count; ++k)
//                        {
//                            var block = blocks.Blocks[k];
//                            if (block.IDXIE > selected.IDXIE)
//                                block.IDXIE -= 1;
//                        }
//                    }
//                    selected.IDXIE = -1;
//                    //structIE.Blocks = blocks;
//                    //Structures.Strucs[struc.IDXIE] = structIE;
//                }
//                if (selected.Pilar.Blocks.Count > 1)
//                {
//                    selected.DestroyBlock();
//                }
//                else
//                {
//                    selected.Layer = 0;
//                    selected.Selected = false;
//                }
//            }
//            structIE.Blocks = ieBlocks;
//            if (Selected.Count > 0)
//                Structures.SetStrucModified(struc.IDXIE);
//            Selected.Clear();
//        }

//        void OnBlockHeightChange(bool up)
//        {
//            var struc = Manager.Mgr.Structure;
//            var strucIE = Structures.Strucs[struc.IDXIE];
//            var blocksIE = strucIE.Blocks;
//            for (int i = 0; i < Selected.Count; ++i)
//            {
//                var selected = Selected[i];
//                if (selected.Layer == 0)
//                    continue;

//                var nHeight = selected.Height;
//                if (up)
//                    nHeight += 0.5f;
//                else
//                    nHeight -= 0.5f;

//                selected.Height = nHeight;

//                if (selected.Locked != BlockLock.Locked)
//                    selected.Locked = BlockLock.SemiLocked;

//                var blockIE = blocksIE[selected.IDXIE];
//                blockIE.Height = selected.Height;
//            }
//            if (Selected.Count > 0)
//                Structures.SetStrucModified(struc.IDXIE);
//        }

//        void OnBlockLengthChange(bool increase)
//        {
//            var struc = Manager.Mgr.Structure;
//            var strucIE = Structures.Strucs[struc.IDXIE];
//            var blocksIE = strucIE.Blocks;
//            if (increase)
//            {
//                for (int i = 0; i < Selected.Count; ++i)
//                {
//                    var selected = Selected[i];
//                    if (selected.Layer == 0)
//                        continue;

//                    var length = selected.Length;
//                    var maxLength = -BlockMeshDef.MidMesh.VertexHeight[selected.blockType == BlockType.WIDE ? 1 : 0].y;
//                    if (length == maxLength)
//                        continue;

//                    var amount = 0.5f;
//                    var mlFloor = Mathf.Floor(maxLength);
//                    var mlDec = maxLength - mlFloor;
//                    if (mlDec > 0.5f && length == (mlFloor + 0.5f))
//                    {
//                        amount = mlDec - 0.5f;
//                    }
//                    else if (mlDec < 0.5f && length == mlFloor)
//                    {
//                        amount = mlDec;
//                    }

//                    selected.Length = selected.Length + amount;

//                    if (selected.Locked != BlockLock.Locked)
//                        selected.Locked = BlockLock.SemiLocked;

//                    var blockIE = blocksIE[selected.IDXIE];
//                    blockIE.Length = selected.Length;
//                }
//            }
//            else
//            {
//                for (int i = 0; i < Selected.Count; ++i)
//                {
//                    var selected = Selected[i];
//                    if (selected.Layer == 0)
//                        continue;
//                    var length = selected.Length;
//                    if (length == 0.5f)
//                        continue;
//                    var maxLength = -BlockMeshDef.MidMesh.VertexHeight[selected.blockType == BlockType.WIDE ? 1 : 0].y;
//                    var amount = 0.5f;
//                    if (length == maxLength)
//                    {
//                        var mlFloor = Mathf.Floor(maxLength);
//                        var mlDec = maxLength - mlFloor;
//                        if (mlDec > 0.5f)
//                        {
//                            amount = mlDec - 0.5f;
//                        }
//                        else if (mlDec < 0.5f)
//                        {
//                            amount = mlDec;
//                        }
//                    }
//                    selected.Length = selected.Length - amount;

//                    if (selected.Locked != BlockLock.Locked)
//                        selected.Locked = BlockLock.SemiLocked;

//                    var blockIE = blocksIE[selected.IDXIE];
//                    blockIE.Length = selected.Length;
//                }
//            }
//            if (Selected.Count > 0)
//                Structures.SetStrucModified(struc.IDXIE);
            
//        }

//        void OnBlockRotationChange(bool right)
//        {
//            var struc = Manager.Mgr.Structure;
//            var strucIE = Structures.Strucs[struc.IDXIE];
//            var blocksIE = strucIE.Blocks;

//            if (right)
//            {
//                for (int i = 0; i < Selected.Count; ++i)
//                {
//                    var selected = Selected[i];
//                    if (selected.Layer == 0)
//                        continue;

//                    var rotated = selected.Rotation;
//                    rotated = (BlockRotation)(((int)rotated) + 1);
//                    if (rotated == BlockRotation.COUNT)
//                        rotated = BlockRotation.Default;
//                    selected.Rotation = rotated;

//                    if (selected.Locked != BlockLock.Locked)
//                        selected.Locked = BlockLock.SemiLocked;

//                    var blockIE = blocksIE[selected.IDXIE];
//                    blockIE.BlockRotation = selected.Rotation;
//                }
//            }
//            else
//            {
//                for (int i = 0; i < Selected.Count; ++i)
//                {
//                    var selected = Selected[i];
//                    if (selected.Layer == 0)
//                        continue;

//                    var rotated = selected.Rotation;
//                    if (rotated == 0)
//                        rotated = BlockRotation.COUNT - 1;
//                    else
//                        rotated = (BlockRotation)(((int)rotated) - 1);
//                    selected.Rotation = rotated;

//                    if (selected.Locked != BlockLock.Locked)
//                        selected.Locked = BlockLock.SemiLocked;

//                    var blockIE = blocksIE[selected.IDXIE];
//                    blockIE.BlockRotation = selected.Rotation;
//                }
//            }
//            if (Selected.Count > 0)
//                Structures.SetStrucModified(struc.IDXIE);
//        }

//        void OnBlockAnchorToggle()
//        {
//            var struc = Manager.Mgr.Structure;
//            var strucIE = Structures.Strucs[struc.IDXIE];
//            var blocksIE = strucIE.Blocks;

//            for (int i = 0; i < Selected.Count; ++i)
//            {
//                var selected = Selected[i];
//                if (selected.Layer == 0)
//                    continue;

//                selected.Anchor = !selected.Anchor;

//                var blockIE = blocksIE[selected.IDXIE];
//                blockIE.Anchor = selected.Anchor;
//                blockIE.Stair = StairType.NONE;

//                //if (selected.Anchor && m_InfoSpritesHidden)
//                //    selected.AnchorGO.GetComponent<SpriteRenderer>().enabled = false;
//                selected.Selected = false;
//            }
//            if (Selected.Count > 0)
//                Structures.SetStrucModified(struc.IDXIE);
//            Selected.Clear();
//        }

//        void OnBlockLockToggle()
//        {
//            var struc = Manager.Mgr.Structure;
//            var strucIE = Structures.Strucs[struc.IDXIE];
//            var blocksIE = strucIE.Blocks;

//            for (int i = 0; i < Selected.Count; ++i)
//            {
//                var selected = Selected[i];
//                if (selected.Layer == 0)
//                    continue;

//                var blockIE = blocksIE[selected.IDXIE];

//                // Lock the block properties
//                if (selected.Locked != BlockLock.Locked)
//                {
//                    blockIE.BlockRotation = selected.Rotation;
//                    blockIE.blockType = selected.blockType;
//                    blockIE.Height = selected.Height;
//                    blockIE.Length = selected.Length;
//                    blockIE.MaterialFamily = selected.MaterialFmly.FamilyName;
//                    blockIE.MonsterID = selected.Monster != null ? selected.Monster.Info.MonsterID : 0;
//                    blockIE.PropID = selected.Prop != null ? Props.PropFamilies.FindIndex((PropFamilyOLD check) => { return check == selected.Prop.Prop.Familiy; }) : 0;
//                    blockIE.Stair = selected.Stair;
//                    selected.Locked = BlockLock.Locked;
//                }
//                // Unlock the block properties
//                else
//                {
//                    blockIE.SetDefault();
//                    blockIE.Layer = (byte)selected.Layer;
//                    blockIE.Anchor = selected.Anchor;
//                    blockIE.Stair = selected.Stair;
//                    if (selected.Stair != StairType.NONE)
//                        blockIE.BlockRotation = selected.Rotation;
//                    if (selected.blockType != BlockType.NORMAL)
//                        blockIE.blockType = selected.blockType;

//                    if (selected.blockType != BlockType.NORMAL || selected.Stair != StairType.NONE)
//                    {
//                        selected.Locked = BlockLock.SemiLocked;
//                    }
//                    else
//                    {
//                        selected.Locked = BlockLock.Unlocked;
//                    }
//                }

//                //selected.Selected = false;
//            }
//            if (Selected.Count > 0)
//                Structures.SetStrucModified(struc.IDXIE);
//            //Selected.Clear();
//        }

//        void OnBlockWideToggle()
//        {
//            var struc = Manager.Mgr.Structure;
//            var strucIE = Structures.Strucs[struc.IDXIE];
//            var blocksIE = strucIE.Blocks;

//            for (int i = 0; i < Selected.Count; ++i)
//            {
//                var selected = Selected[i];
//                if (selected.Layer == 0)
//                    continue;

//                selected.Selected = false;
//                if (selected.Removed)
//                {
//                    continue;
//                }

//                var blocks = StructureComponent.GetNearBlocks(struc, selected.Pilar.StructureID, selected.Height, selected.Layer);
//                if (blocks[0] == null) // There are not 3 adjacent blocks from the same layer
//                    blocks = StructureComponent.GetNearBlocks(struc, selected.Pilar.StructureID, selected.Height, -1); // Get them ignoring the layer
//                if (blocks[0] == null) // There are no adjacent blocks, this block can't be wide
//                    continue;

//                // The block is Normal, lets convert it into WIDE and remove the adjacent ones
//                if (selected.blockType != BlockType.WIDE)
//                {
//                    // Is the MaterialType of this block capable of WIDE blocks?
//                    var matFamily = selected.MaterialFmly;
//                    if (matFamily.WideMaterials.Length == 0)
//                        continue; // It wasn't, ignoring...

//                    bool valid = true;
//                    for (int j = 0; j < blocks.Length; ++j)
//                    {
//                        if (blocks[j].Removed || blocks[j].blockType == BlockType.WIDE || blocks[j].Locked == BlockLock.Locked)
//                        {
//                            valid = false;
//                            break;
//                        }
//                    }
//                    if (!valid)
//                        continue;

//                    var blockIE = blocksIE[selected.IDXIE];
//                    selected.Stair = StairType.NONE;
//                    blockIE.Stair = StairType.NONE;
//                    selected.blockType = BlockType.WIDE;
//                    blockIE.blockType = selected.blockType;
//                    if (selected.Locked != BlockLock.Locked)
//                        selected.Locked = BlockLock.SemiLocked;
                    

//                    for (int j = 0; j < blocks.Length; ++j)
//                    {
//                        var b = blocks[j];

//                        //b.Layer = selected.Layer;
//                        //b.Height = selected.Height;
//                        //b.MicroHeight = selected.MicroHeight;

//                        if (b.IDXIE < 0)
//                        {
//                            b.IDXIE = AddBlockToStructure(ref blocksIE);
//                        }
//                        var bie = blocksIE[b.IDXIE];
//                        bie.Layer = (byte)selected.Layer;
//                        bie.Height = selected.Height;
//                        bie.Length = selected.Length;

//                        //b.TopMR.enabled = false;
//                        //b.TopBC.enabled = false;
//                        //b.MidMR.enabled = false;
//                        //b.MidBC.enabled = false;

//                        //b.LayerSR.enabled = false;
//                        //b.AnchorSR.enabled = false;
//                        //b.StairSR.enabled = false;
//                        //b.LockSR.enabled = false;

//                        //b.Removed = true;
//                    }
//                    selected.SetWIDE(blocks);
//                }
//                // The block is WIDE, lets convert it into normal and change the remove state from the adjancent ones
//                else
//                {
//                    var blockIE = blocksIE[selected.IDXIE];
//                    selected.blockType = BlockType.NORMAL;
//                    blockIE.blockType = BlockType.COUNT;

//                    //for (int j = 0; j < blocks.Length; ++j)
//                    //{
//                    //    var b = blocks[j];

//                    //    b.TopMR.enabled = true;
//                    //    b.TopBC.enabled = true;
//                    //    b.MidMR.enabled = true;
//                    //    b.MidBC.enabled = true;

//                    //    b.LayerSR.enabled = true;
//                    //    if (b.Anchor)
//                    //        b.AnchorSR.enabled = true;
//                    //    if (b.Stair == StairType.POSSIBLE)
//                    //        b.StairSR.enabled = true;

//                    //    if (b.Locked != BlockLock.Unlocked)
//                    //        b.LockSR.enabled = true;

//                    //    b.Removed = false;
//                    //}
//                }

//            }
//            if (Selected.Count > 0)
//                Structures.SetStrucModified(struc.IDXIE);
//            Selected.Clear();
//        }

//        void OnBlockStairChange()
//        {
//            var struc = Manager.Mgr.Structure;
//            var strucIE = Structures.Strucs[struc.IDXIE];
//            var blocksIE = strucIE.Blocks;

//            for (int i = 0; i < Selected.Count; ++i)
//            {
//                var selected = Selected[i];
//                if (selected.Layer == 0 || selected.blockType == BlockType.WIDE)
//                    continue;

//                var stair = selected.Stair + 1;
//                if (stair == StairType.COUNT)
//                    stair = StairType.NONE;

//                selected.Stair = stair;

//                if (selected.Locked != BlockLock.Locked)
//                    selected.Locked = BlockLock.SemiLocked;

//                var blockIE = blocksIE[selected.IDXIE];
//                blockIE.Stair = selected.Stair;
//                blockIE.BlockRotation = selected.Rotation;

//                //if (selected.Stair != StairType.NONE && m_InfoSpritesHidden)
//                //    selected.StairGO.GetComponent<SpriteRenderer>().enabled = false;
//            }
//            if (Selected.Count > 0)
//                Structures.SetStrucModified(struc.IDXIE);
//        }

//        void OnBlockMaterialChange()
//        {
//            var struc = Manager.Mgr.Structure;
//            var strucIE = Structures.Strucs[struc.IDXIE];
//            var blocksIE = strucIE.Blocks;

//            for (int i = 0; i < Selected.Count; ++i)
//            {
//                var selected = Selected[i];
//                if (selected.Layer == 0)
//                    continue;

//                var layerID = selected.Layer;
//                var layerInfo = struc.InfoLayers[layerID - 1];
//                if (layerInfo.IsLinkedLayer)
//                {
//                    int selectedLayer = 0;

//                    float llChance = (float)Manager.Mgr.BuildRNG.NextDouble();
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
//                    layerInfo = struc.InfoLayers[selectedLayer - 1];
//                }

//                List<int> availableSubmaterials = new List<int>(layerInfo.MaterialTypes.Count);
//                List<float> availableSubmaterialChances = new List<float>(layerInfo.MaterialTypes.Count);

//                for (int j = 0; j < layerInfo.MaterialTypes.Count; ++j)
//                {
//                    var matFamily = BlockMaterial.MaterialFamilies[layerInfo.MaterialTypes[j]];
//                    //var matType = BlockMaterial.MaterialTypes[layerInfo.MaterialTypes[j]];
//                    //if (matType.Def.Materials[(int)selected.blockType].Count > 0)
//                    if(matFamily.GetSet(selected.blockType).Length > 0)
//                    {
//                        availableSubmaterials.Add(layerInfo.MaterialTypes[j]);
//                        availableSubmaterialChances.Add(layerInfo.MaterialTypeChances[j]);
//                    }
//                }
//                GameUtils.UpdateChances(ref availableSubmaterialChances);
//                int selectedSubmaterial = 0;

//                float chance = (float)Manager.Mgr.BuildRNG.NextDouble();
//                float accum = 0.0f;
//                for (int j = 0; j < availableSubmaterialChances.Count; ++j)
//                {
//                    float nextChance = accum + availableSubmaterialChances[j];

//                    if (chance > accum && chance < nextChance)
//                    {
//                        selectedSubmaterial = availableSubmaterials[j];
//                        break;
//                    }
//                    accum = nextChance;
//                }

//                selected.MaterialFmly = BlockMaterial.MaterialFamilies[selectedSubmaterial];
//                //selected.MaterialTypeID = selectedSubmaterial;

//                if (selected.Locked != BlockLock.Locked)
//                    selected.Locked = BlockLock.SemiLocked;

//                var blockIE = blocksIE[selected.IDXIE];
//                blockIE.MaterialFamily = selected.MaterialFmly.FamilyName;
//            }
//            if (Selected.Count > 0)
//                Structures.SetStrucModified(struc.IDXIE);
//        }

//        void OnBlockStacking()
//        {
//            var struc = Manager.Mgr.Structure;
//            var strucIE = Structures.Strucs[struc.IDXIE];
//            var blocksIE = strucIE.Blocks;

//            for (int i = 0; i < Selected.Count; ++i)
//            {
//                var selected = Selected[i];
//                if (selected.Layer == 0)
//                    continue;
//                selected.Selected = false;

//                var pilar = selected.Pilar;
//                var stacked = pilar.AddBlock();
//                stacked.IDXIE = AddBlockToStructure(ref blocksIE);
//                var stackedIE = blocksIE[stacked.IDXIE];
//                if (stackedIE == null)
//                {
//                    stackedIE = new IE.V2.BlockIE();
//                    blocksIE[stacked.IDXIE] = stackedIE;
//                }
//                else
//                {
//                    stackedIE.SetDefault();
//                }
//                stackedIE.Layer = (byte)selected.Layer;
//                stackedIE.StructureID = (byte)stacked.Pilar.StructureID;
//                strucIE.Blocks = blocksIE;

//                if (selected.Locked != BlockLock.Locked)
//                    selected.Locked = BlockLock.SemiLocked;
//                var selIE = blocksIE[selected.IDXIE];
//                selIE.Flags[(int)IE.V3.BlockIE.Flag.Height] = true;
//                selIE.Flags[(int)IE.V3.BlockIE.Flag.Length] = true;

//                stacked.Layer = selected.Layer;
//                stacked.Height = selected.Height + 0.5f;
//                stacked.MicroHeight = selected.MicroHeight;
//                stacked.Length = 0.5f;

//                stackedIE.Height = stacked.Height;
//                stackedIE.Length = stacked.Length;

//                if (stacked.Locked != BlockLock.Locked)
//                    stacked.Locked = BlockLock.SemiLocked;

//                if (selected.GetBlockType() == Def.BlockType.WIDE)
//                {
//                    stacked.blockType = BlockType.WIDE;
//                    stackedIE.blockType = BlockType.WIDE;
//                    var pilarPos = GameUtils.PosFromID(pilar.StructureID);
//                    int[] nearBlocks = new int[3]
//                    {
//                        GameUtils.IDFromPos(new Vector2Int(pilarPos.x + 1, pilarPos.y)),
//                        GameUtils.IDFromPos(new Vector2Int(pilarPos.x, pilarPos.y + 1)),
//                        GameUtils.IDFromPos(new Vector2Int(pilarPos.x + 1, pilarPos.y + 1))
//                    };
//                    for (int j = 0; j < nearBlocks.Length; ++j)
//                    {
//                        var nearPilar = pilar.Struc.Pilars[nearBlocks[j]];
//                        var nearBlock = nearPilar.AddBlock();
//                        nearBlock.IDXIE = AddBlockToStructure(ref blocksIE);
//                        var nearBlockIE = blocksIE[nearBlock.IDXIE];
//                        if (nearBlockIE == null)
//                        {
//                            nearBlockIE = new IE.V2.BlockIE();
//                            blocksIE[nearBlock.IDXIE] = nearBlockIE;
//                        }
//                        else
//                        {
//                            nearBlockIE.SetDefault();
//                        }
//                        nearBlockIE.Layer = (byte)stacked.Layer;
//                        nearBlockIE.StructureID = (byte)nearPilar.StructureID;
//                        strucIE.Blocks = blocksIE;

//                        nearBlock.Layer = selected.Layer;
//                        nearBlock.Height = stacked.Height;
//                        nearBlock.MicroHeight = stacked.MicroHeight;

//                        nearBlockIE.Height = nearBlock.Height;

//                        nearBlock.TopMR.enabled = false;
//                        //nearBlock.TopBC.enabled = false;
//                        nearBlock.MidMR.enabled = false;
//                        //nearBlock.MidBC.enabled = false;
//                        nearBlock.BlockBC.enabled = false;

//                        nearBlock.LayerSR.enabled = false;
//                        nearBlock.AnchorSR.enabled = false;
//                        nearBlock.StairSR.enabled = false;
//                        nearBlock.LockSR.enabled = false;
//                        nearBlock.Removed = true;

//                        if (nearBlock.Locked != BlockLock.Locked)
//                            nearBlock.Locked = BlockLock.SemiLocked;
//                    }
//                }

//            }
//            if (Selected.Count > 0)
//                Structures.SetStrucModified(struc.IDXIE);
//            Selected.Clear();
//        }

//        void OnBridgeCreation()
//        {
//            var struc = Manager.Mgr.Structure;
//            for (int i = 0; i < Selected.Count; ++i)
//            {
//                var selected = Selected[i];
//                if (selected.Layer != 0)
//                    continue;
//                selected.Selected = false;
//                var pilar = selected.Pilar;
//                if (pilar.Bridge == null)
//                {
//                    pilar.AddBridge(0, BridgeType.SMALL, false);
//                    var pilarPos = GameUtils.PosFromID(pilar.StructureID);
//                    List<int> nearPilarIDs = new List<int>(4);
//                    if (pilarPos.x != StructureComponent.Width)
//                        nearPilarIDs.Add(GameUtils.IDFromPos(new Vector2Int(pilarPos.x + 1, pilarPos.y)));
//                    else
//                        nearPilarIDs.Add(-1);
//                    if (pilarPos.x != 0)
//                        nearPilarIDs.Add(GameUtils.IDFromPos(new Vector2Int(pilarPos.x - 1, pilarPos.y)));
//                    else
//                        nearPilarIDs.Add(-1);

//                    if (pilarPos.y != StructureComponent.Height)
//                        nearPilarIDs.Add(GameUtils.IDFromPos(new Vector2Int(pilarPos.x, pilarPos.y + 1)));
//                    else
//                        nearPilarIDs.Add(-1);
//                    if (pilarPos.y != 0)
//                        nearPilarIDs.Add(GameUtils.IDFromPos(new Vector2Int(pilarPos.x, pilarPos.y - 1)));
//                    else
//                        nearPilarIDs.Add(-1);

//                    //int[] nearPilarIDs = new int[4]
//                    //{
//                    //    GameUtils.IDFromPos(new Vector2Int(pilarPos.x + 1, pilarPos.y)),
//                    //    GameUtils.IDFromPos(new Vector2Int(pilarPos.x - 1, pilarPos.y)),
//                    //    GameUtils.IDFromPos(new Vector2Int(pilarPos.x, pilarPos.y + 1)),
//                    //    GameUtils.IDFromPos(new Vector2Int(pilarPos.x, pilarPos.y - 1)),
//                    //};
//                    BridgeComponent[] nearBridges = new BridgeComponent[4]
//                    {
//                        null,
//                        null,
//                        null,
//                        null
//                    };
//                    PilarComponent[] nearPilars = new PilarComponent[4]
//                    {
//                        null,
//                        null,
//                        null,
//                        null
//                    };
//                    int lastBridgeFound = -1;
//                    int lastBlockFound = -1;
//                    for (int j = 0; j < nearPilarIDs.Count; ++j)
//                    {
//                        if (nearPilarIDs[j] == -1)
//                            continue;
//                        var nearPilar = struc.Pilars[nearPilarIDs[j]];
//                        if (nearPilar.Bridge != null)
//                        {
//                            nearBridges[j] = nearPilar.Bridge;
//                            pilar.Bridge.Next = nearPilar.Bridge;
//                            lastBridgeFound = j;
//                            continue;
//                        }
//                        for (int k = 0; k < nearPilar.Blocks.Count; ++k)
//                        {
//                            if (nearPilar.Blocks[k].Layer != 0)
//                            {
//                                nearPilars[j] = nearPilar;
//                                lastBlockFound = j;
//                                //pilar.Bridge.Previous = nearPilar.Blocks[k];
//                                break;
//                            }
//                        }
//                    }
//                    BlockRotation[] rotation = null;
//                    // Near bridge found
//                    if (pilar.Bridge.Next != null)
//                    {
//                        var foundBridge = pilar.Bridge.Next.GetComponent<BridgeComponent>();
//                        GameObject otherGO = null;
//                        if (lastBridgeFound < 2)
//                        {
//                            rotation = new BlockRotation[2]
//                            {
//                                BlockRotation.Left,
//                                BlockRotation.Right
//                            };
//                            int otherSideGOIDX = 0;
//                            if (lastBridgeFound == 0)
//                                otherSideGOIDX = 1;
//                            if (nearBridges[otherSideGOIDX] != null)
//                                otherGO = nearBridges[otherSideGOIDX].gameObject;
//                            else if (nearPilars[otherSideGOIDX] != null)
//                                otherGO = nearPilars[otherSideGOIDX].gameObject;
//                        }
//                        else
//                        {
//                            rotation = new BlockRotation[2]
//                            {
//                                BlockRotation.Default,
//                                BlockRotation.Half
//                            };
//                            int otherSideGOIDX = 2;
//                            if (lastBridgeFound == 2)
//                                otherSideGOIDX = 3;
//                            if (nearBridges[otherSideGOIDX] != null)
//                                otherGO = nearBridges[otherSideGOIDX].gameObject;
//                            else if (nearPilars[otherSideGOIDX] != null)
//                                otherGO = nearPilars[otherSideGOIDX].gameObject;
//                        }
//                        pilar.Bridge.Next = null;
//                        pilar.Bridge.Previous = null;
//                        if (foundBridge.Next == null)
//                        {
//                            pilar.Bridge.Previous = foundBridge;//.gameObject;
//                            foundBridge.Next = pilar.Bridge;//.gameObject;
//                        }
//                        else if (foundBridge.Previous == null)
//                        {
//                            pilar.Bridge.Next = foundBridge;//.gameObject;
//                            foundBridge.Previous = pilar.Bridge;//.gameObject;
//                        }
//                        else if (otherGO != null)
//                        {
//                            //pilar.Bridge.Previous = otherGO;
//                            pilar.Bridge.Next = null;
//                        }
//                    }
//                    // No near bridge
//                    else
//                    {
//                        // Near block found
//                        if (pilar.Bridge.Previous != null)
//                        {
//                            if (lastBlockFound < 2)
//                            {
//                                rotation = new BlockRotation[2]
//                                {
//                                                BlockRotation.Left,
//                                                BlockRotation.Right
//                                };
//                            }
//                            else
//                            {
//                                rotation = new BlockRotation[2]
//                                {
//                                                BlockRotation.Default,
//                                                BlockRotation.Half
//                                };
//                            }
//                        }
//                    }
//                    if (rotation != null)
//                        pilar.Bridge.Rotation = rotation[Manager.Mgr.BuildRNG.Next(0, rotation.Length)];
//                }
//                else
//                {
//                    if (pilar.Bridge.Next != null || pilar.Bridge.Previous != null)
//                    {
//                        int rot = (int)pilar.Bridge.Rotation;
//                        rot = (rot + 2) % (int)BlockRotation.COUNT;
//                        pilar.Bridge.Rotation = (BlockRotation)rot;
//                    }
//                    else
//                    {
//                        var rot = pilar.Bridge.Rotation;
//                        rot = (rot + 1);
//                        if (rot == BlockRotation.COUNT)
//                            pilar.DestroyBridge();
//                        else
//                            pilar.Bridge.Rotation = rot;
//                    }
//                }


//            }
//            Selected.Clear();
//        }

//        void OnBlockLayerChange(int layer)
//        {
//            var struc = Manager.Mgr.Structure;
//            var strucIE = Structures.Strucs[struc.IDXIE];
//            var blocksIE = strucIE.Blocks;

//            for (int j = 0; j < Selected.Count; ++j)
//            {
//                var selected = Selected[j];

//                if (selected.IDXIE < 0)
//                {
//                    selected.IDXIE = AddBlockToStructure(ref blocksIE);
//                }
//                var blockIE = blocksIE[selected.IDXIE];
//                bool semiLock = false;
//                if (blockIE == null)
//                {
//                    blockIE = new IE.V2.BlockIE();
//                    blocksIE[selected.IDXIE] = blockIE;
//                }
//                else
//                {
//                    BitArray flags = (BitArray)blockIE.Flags.Clone();
//                    flags[(int)IE.V2.BlockIE.Flag.MaterialType] = false;
//                    flags[(int)IE.V2.BlockIE.Flag.Prop] = false;
//                    flags[(int)IE.V2.BlockIE.Flag.Monster] = false;
//                    bool anchor = blockIE.Anchor; 
//                    BlockType bType = BlockType.NORMAL;
//                    if (flags[(int)IE.V2.BlockIE.Flag.BlockType])
//                        bType = blockIE.blockType;
//                    float height = 0f;
//                    if (flags[(int)IE.V2.BlockIE.Flag.Height])
//                        height = blockIE.Height;
//                    float length = 0.5f;
//                    if (flags[(int)IE.V2.BlockIE.Flag.Length])
//                        length = blockIE.Length;
//                    BlockRotation rotation = BlockRotation.Default;
//                    if (flags[(int)IE.V2.BlockIE.Flag.Rotation])
//                        rotation = blockIE.BlockRotation;

//                    blockIE.SetDefault();
//                    if (flags[(int)IE.V2.BlockIE.Flag.Anchor])
//                    {
//                        blockIE.Anchor = true;
//                        semiLock = true;
//                    }
//                    if (flags[(int)IE.V2.BlockIE.Flag.BlockType])
//                    {
//                        blockIE.blockType = bType;
//                        semiLock = true;
//                    }
//                    if (flags[(int)IE.V2.BlockIE.Flag.Height])
//                    {
//                        blockIE.Height = height;
//                        semiLock = true;
//                    }
//                    if (flags[(int)IE.V2.BlockIE.Flag.Length])
//                    {
//                        blockIE.Length = length;
//                        semiLock = true;
//                    }
//                    if (flags[(int)IE.V2.BlockIE.Flag.Rotation])
//                    {
//                        blockIE.BlockRotation = rotation;
//                        semiLock = true;
//                    }
//                }
//                blockIE.Layer = (byte)layer;
//                blockIE.StructureID = (byte)selected.Pilar.StructureID;
//                strucIE.Blocks = blocksIE;
//                if(semiLock)
//                    selected.Locked = BlockLock.SemiLocked;
//                selected.Layer = layer;


//                //if (m_InfoSpritesHidden)
//                //    selected.LayerGO.GetComponent<SpriteRenderer>().enabled = false;
//                //selected.Selected = false;
//            }
//            if (Selected.Count > 0)
//                Structures.SetStrucModified(struc.IDXIE);
//            //Selected.Clear();
//        }

//        void OnFullRayCast(bool mouseLeftClick, bool mouseRightClick, bool mouseLeftHold, bool mouseRightHold)
//        {
//            bool mouseClicked = mouseLeftClick || mouseRightClick;
//            bool mouseHold = mouseLeftHold || mouseRightHold;

//            bool select = (mouseLeftHold || mouseLeftClick) && (!mouseRightHold || !mouseRightClick);
//            bool unselect = (!mouseLeftHold || !mouseLeftClick) && (mouseRightHold || mouseRightClick);

//            if (!mouseClicked && !mouseHold)
//                return;

//            bool shiftPressed = Input.GetKey(KeyCode.LeftShift);

//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            GameObject goOver = null;
//            bool rayHit = Physics.Raycast(ray, out RaycastHit mouseHit, 10000.0f);
//            if(!rayHit)
//            {
//                if (mouseClicked && !shiftPressed)
//                {
//                    for (int i = 0; i < Selected.Count; ++i)
//                    {
//                        var selBlock = Selected[i];
//                        selBlock.Selected = false;
//                    }
//                    Selected.Clear();
//                }
//                return;
//            }

//            goOver = mouseHit.transform.gameObject;
//            if (goOver.tag != BlockComponent.BlockTag)
//            //if(goOver.layer != Manager.BlockLayer)
//            {
//                return;
//            }

//            var blockGO = goOver;//.transform.parent.gameObject;
//            var block = blockGO.GetComponent<BlockComponent>();
//            if (BlockOver != null && BlockOver != block)
//                BlockOver.Highlighted = false;
//            BlockOver = block;

//            if (shiftPressed && mouseHold) // Drag edit
//            {
//                if (select && !IsSelected(BlockOver.Pilar.StructureID, BlockOver.transform.position.y))
//                {
//                    BlockOver.Selected = true;
//                    Selected.Add(BlockOver);
//                }
//                else if (unselect)
//                {
//                    BlockOver.Selected = false;
//                    RemoveSelected(BlockOver.TopMR.transform.position);
//                }
//            }
//            else if (!shiftPressed && mouseClicked) // Clicky edit
//            {
//                if (select && !IsSelected(BlockOver.Pilar.StructureID, BlockOver.transform.position.y))
//                {
//                    BlockOver.Selected = true;
//                    Selected.Add(BlockOver);
//                }
//                else if (unselect)
//                {
//                    BlockOver.Selected = false;
//                    RemoveSelected(BlockOver.TopMR.transform.position);
//                }
//            }
//        }

//        public void Update()
//        {
//            if (editingMode != EditingStructureMode.None)
//                return;

//            //var struc = Manager.Mgr.Structure;
//            //var strucIE = Structures.Strucs[struc.IDXIE];
//            //var blocksIE = strucIE.Blocks;
            
//            if (!Manager.Mgr.HideInfo)
//            {
//                // Remove block from the layer and make it invalid
//                if (Input.GetKey(KeyCode.Delete))
//                {
//                    OnDeleteBlock();
//                    return;
//                }
//                if (lastModificationTime < Time.time)
//                {
//                    // Move up the block by 0.5m and change the BlockDefIE
//                    if (Input.GetKey(KeyCode.UpArrow))
//                    {
//                        OnBlockHeightChange(true);
//                        lastModificationTime = Time.time + ModificationWait;
//                        return;
//                    }
//                    // Move down the block by 0.5m and change the BlockDefIE
//                    else if (Input.GetKey(KeyCode.DownArrow))
//                    {
//                        OnBlockHeightChange(false);
//                        lastModificationTime = Time.time + ModificationWait;
//                        return;
//                    }
//                    // Make the block larger up to 0.5m more, and change the BlockDefIE
//                    else if (Input.GetKey(KeyCode.KeypadPlus))
//                    {
//                        OnBlockLengthChange(true);
//                        lastModificationTime = Time.time + ModificationWait;
//                        return;
//                    }
//                    // Make the block less larger up to -0.5m, and change the BlockDefIE
//                    else if (Input.GetKey(KeyCode.KeypadMinus))
//                    {
//                        OnBlockLengthChange(false);
//                        lastModificationTime = Time.time + ModificationWait;
//                        return;
//                    }
//                    // Rotate the block 90º to the right, and change the BlockDefIE
//                    else if (Input.GetKey(KeyCode.RightArrow))
//                    {
//                        OnBlockRotationChange(true);
//                        lastModificationTime = Time.time + ModificationWait;
//                        return;
//                    }
//                    // Rotate the block 90º to the left, and change the BlockDefIE
//                    else if (Input.GetKey(KeyCode.LeftArrow))
//                    {
//                        OnBlockRotationChange(false);
//                        lastModificationTime = Time.time + ModificationWait;
//                        return;
//                    }
//                    // Makes the block an anchor point or if it already was it undoes that, and changes BlockDefIE
//                    else if (Input.GetKey(KeyCode.A))
//                    {
//                        OnBlockAnchorToggle();
//                        lastModificationTime = Time.time + ModificationWait;
//                        return;
//                    }
//                    // Locks the block properties and if they are locked it unlocks them
//                    else if (Input.GetKey(KeyCode.L))
//                    {
//                        OnBlockLockToggle();
//                        lastModificationTime = Time.time + ModificationWait;
//                        return;
//                    }
//                    // Converts the selected block into a WIDE one if its possible, if it already is, will convert it into a normal one
//                    else if (Input.GetKey(KeyCode.Z))
//                    {
//                        OnBlockWideToggle();
//                        lastModificationTime = Time.time + ModificationWait;
//                        return;
//                    }
//                    // Interchanges how the block is going to be an stair within these states:
//                    // not been an stair block, there's a chance to be a stair block, block will be always stair
//                    else if (Input.GetKey(KeyCode.E))
//                    {
//                        OnBlockStairChange();
//                        lastModificationTime = Time.time + ModificationWait;
//                        return;
//                    }
//                    // Changes the MaterialType of the selected blocks for the next one in the list of the structure, if there are available
//                    else if (Input.GetKey(KeyCode.Tab))
//                    {
//                        OnBlockMaterialChange();
//                        lastModificationTime = Time.time + ModificationWait;
//                        return;
//                    }
//                    // Adds a block on top of the selected one (stacking)
//                    else if(Input.GetKey(KeyCode.T))
//                    {
//                        OnBlockStacking();
//                        lastModificationTime = Time.time + ModificationWait;
//                        return;
//                    }
//                    else if(Input.GetKey(KeyCode.B))
//                    {
//                        OnBridgeCreation();
//                        lastModificationTime = Time.time + ModificationWait;
//                        return;
//                    }
//                    var struc = Manager.Mgr.Structure;
//                    // Adds the block to a layer, if the block was invalid, now will be valid and with the properties from the layer
//                    for (int i = 1; i <= StructureComponent.LayerAmount; ++i)
//                    {
//                        if (!Input.GetKey(KeyCode.Alpha0 + i) && !Input.GetKey(KeyCode.Keypad0 + i))
//                            continue;

//                        if (!struc.InfoLayers[i - 1].IsValid())
//                            continue;

//                        OnBlockLayerChange(i);

//                        lastModificationTime = Time.time + ModificationWait;
//                        return;
//                    }
//                }
//            }
//            if (lastModificationTime < Time.time)
//            {
//                // Hides all the invalid blocks and all the gizmos, also enables WIDE and stair generation so that 
//                // we can see how should look in game
//                if (Input.GetKey(KeyCode.H))
//                {
//                    ToggleEditingVisibility();
//                    lastModificationTime = Time.time + ModificationWait;
//                    return;
//                }
//            }

//            if (Manager.Mgr.HideInfo || GUIUtility.hotControl != 0)
//                return;

//            bool shiftPressed = Input.GetKey(KeyCode.LeftShift);
//            bool mouseLeftHold = Input.GetMouseButton(0);
//            bool mouseRightHold = Input.GetMouseButton(1);

//            bool mouseLeftClick = Input.GetMouseButtonDown(0);
//            bool mouseRightClick = Input.GetMouseButtonDown(1);

//            OnFullRayCast(mouseLeftClick, mouseRightClick, mouseLeftHold, mouseRightHold);

//            //bool mouseClicked = mouseLeftClick || mouseRightClick;

//            //bool select = (leftHeldDown || mouseLeftClick) && (!rightHeldDown || !mouseRightClick);
//            //bool unselect = (!leftHeldDown || !mouseLeftClick) && (rightHeldDown || mouseRightClick);

//            //if (mouseClicked || IsMouseHeld)
//            //{
//            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            //    RaycastHit mouseHit;
//            //    GameObject goOver = null;
//            //    if (!Physics.Raycast(ray, out mouseHit, 10000.0f))
//            //        return;

//            //    goOver = mouseHit.transform.gameObject;
//            //    if (goOver.tag != BlockComponent.BlockTag)
//            //        return;

//            //    var blockGO = goOver.transform.parent.gameObject;
//            //    var block = blockGO.GetComponent<BlockComponent>();
//            //    if (BlockOver != null && BlockOver != block)
//            //        BlockOver.Highlighted = false;
//            //    BlockOver = block;

//            //    if (shiftPressed && IsMouseHeld) // Drag edit
//            //    {
//            //        if (select && !IsSelected(BlockOver.Pilar.StructureID, BlockOver.transform.position.y))
//            //        {
//            //            BlockOver.Selected = true;
//            //            Selected.Add(BlockOver);
//            //        }
//            //        else if (unselect)
//            //        {
//            //            BlockOver.Selected = false;
//            //            RemoveSelected(BlockOver.TopGO.transform.position);
//            //        }
//            //    }
//            //    else if (!shiftPressed && mouseClicked) // Clicky edit
//            //    {
//            //        if (select && !IsSelected(BlockOver.Pilar.StructureID, BlockOver.transform.position.y))
//            //        {
//            //            BlockOver.Selected = true;
//            //            Selected.Add(BlockOver);
//            //        }
//            //        else if (unselect)
//            //        {
//            //            BlockOver.Selected = false;
//            //            RemoveSelected(BlockOver.TopGO.transform.position);
//            //        }
//            //    }
//            //}
//        }
//    }
//}
