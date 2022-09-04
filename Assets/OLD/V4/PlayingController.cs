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
//using UnityEngine.VFX;

//namespace Assets
//{
//    public enum PC_State
//    {
//        Default,
//        StructSel,
//        PropSel,
//        MonsterSel,
//        BackgroundSel,
//        AntSet,
//        PlacingStruct,

//        COUNT
//    }
//    public class PlayingController : IGameController
//    {
//        public const float ModificationWait = 0.2f;
//        public const float RayWait = 0.25f;
//        const int MistSize = 16;
//        const int HorizontalMistCount = Manager.MapWidth / MistSize;
//        const int VerticalMistCount = Manager.MapHeight / MistSize;

//        int SelectedPilarMapID;
//        int SelectedBlockIdx;
//        IBlock BlockOver;
//        ImageSelector Selector;
//        PC_State State;
//        OddScript oddScript;
//        GameObject MistHeightObject;
//        GameObject[] MistObjects;
//        AntSelector m_AntSelector;
//        BridgeComponent m_SelectedBridge;
//        int[] m_PrevAntState;
//        IStruc m_SelectedStruc;
//        float m_SelectedStrucOffset;
//        int m_SelectedStrucID;
//        //RaycastHit[] m_RayCasts;
//        //RaycastHit[] m_TempCasts;
//        //int m_RayAmount;

//        float LastModificationTime;
//        float LastRayTime;
//        //float m_NextFogTime;

//        public PlayingController()
//        {
//            MistHeightObject = new GameObject("Mist Objects");
//            MistHeightObject.transform.Translate(new Vector3(0.0f, -1.5f, 0.0f), Space.World);
//            MistObjects = new GameObject[HorizontalMistCount * VerticalMistCount];
//            for(int y = 0; y < VerticalMistCount; ++y)
//            {
//                int yOffset = y * HorizontalMistCount;
//                int yPos = y * MistSize + MistSize / 2;
//                for(int x = 0; x < HorizontalMistCount; ++x)
//                {
//                    MistObjects[yOffset + x] = new GameObject($"Mist({x},{y})");
//                    int xPos = x * MistSize + MistSize / 2;
//                    MistObjects[yOffset + x].transform.Translate(new Vector3(xPos, MistHeightObject.transform.position.y, yPos), Space.World);
//                    MistObjects[yOffset + x].transform.SetParent(MistHeightObject.transform);
//                }
//            }
//        }

//        bool OnSingleRayCast(out RaycastHit mouseHit, bool blocks, bool livingEntities, bool bridges, bool rayPlane)
//        {
//            Ray ray = Manager.Mgr.m_Camera.ScreenPointToRay(Input.mousePosition);

//            int mask = 0;
//            if (blocks)
//                mask = mask | (1 << Manager.BlockLayer);
//            if (livingEntities)
//                mask = mask | (1 << Manager.LivingEntityLayer);
//            if (bridges)
//                mask = mask | (1 << Manager.BridgeLayer);
//            if (rayPlane)
//                mask = mask | (1 << Manager.RayPlaneLayer);

//            return Physics.Raycast(ray, out mouseHit, 1000f, mask);
//        }

//        void OnSingleCastDone(bool hasHitted, RaycastHit mouseHit)
//        {
//            if(hasHitted)
//            {
//                var block = mouseHit.transform.gameObject.GetComponent<BlockComponent>();
//                if (BlockOver != null && BlockOver != block)
//                {
//                    BlockOver.Highlighted = false;
//                }
//                BlockOver = block;
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
//            if (BlockOver != null)
//            {
//                BlockOver.Highlighted = true;
//            }
//            if (!Manager.Mgr.HideInfo)
//            {

//                var hasHitted = OnSingleRayCast(out RaycastHit mouseHit, true, false, false, false);
//                OnSingleCastDone(hasHitted, mouseHit);
//            }
//        }

//        public GameState GetGameState()
//        {
//            return GameState.PLAYING;
//        }

//        void MoveYStruc(float offset)
//        {
//            m_SelectedStrucOffset += offset;
//            //foreach(var pilar in m_SelectedStruc.Pilars)
//            //{
//            //    pilar.transform.Translate(0f, offset, 0f, Space.World);
//            //    foreach(var block in pilar.Blocks)
//            //    {
//            //        if (block.Prop != null)
//            //            block.Prop.transform.Translate(0f, offset, 0f, Space.World);
//            //        if (block.Monster != null)
//            //            block.Monster.transform.Translate(0f, offset, 0f, Space.World);
//            //    }
//            //}
//            ///m_SelectedStruc.transform.Translate(0f, offset, 0f, Space.World);
//            foreach(var pilar in m_SelectedStruc.Pilars)
//            {
//                foreach(var block in pilar.Blocks)
//                {
//                    if (block.Removed)
//                        continue;
//                    block.Height += offset;
//                }
//            }
//        }

//        void MoveXZStruc(Vector2Int offset)
//        {
//            const float blockSep = 1f + StructureComponent.Separation;
//            Vector2 translation = new Vector2(offset.x * blockSep, offset.y * blockSep);
//            m_SelectedStruc.transform.Translate(translation.x, 0f, translation.y, Space.World);
//            var rect = m_SelectedStruc.StructRect;
//            rect.x = m_SelectedStruc.transform.position.x;
//            rect.y = m_SelectedStruc.transform.position.z;
//            m_SelectedStruc.StructRect = rect;
//            foreach(var pilar in m_SelectedStruc.Pilars)
//            {
//                var pilarPos = pilar.transform.position + new Vector3(0.1f, 0f, 0.1f);
//                var mapID = GameUtils.MapIDFromPosition(new Vector2(pilarPos.x, pilarPos.z));
//                pilar.MapID = mapID;
//            }

//            foreach (var le in m_SelectedStruc.LivingEntities)
//                le.transform.Translate(offset.x * blockSep, 0f, offset.y * blockSep, Space.World);
//        }

//        void OnPlacingStruct()
//        {
//            var rect = Manager.Mgr.m_Canvas.pixelRect;

//            if(LastModificationTime < Time.time)
//            {
//                if(Input.GetKey(KeyCode.KeypadPlus))
//                {
//                    MoveYStruc(0.5f);
//                    LastModificationTime = Time.time + ModificationWait;
//                }
//                else if(Input.GetKey(KeyCode.KeypadMinus))
//                {
//                    MoveYStruc(-0.5f);
//                    LastModificationTime = Time.time + ModificationWait;
//                }
//                else if(Input.GetKey(KeyCode.UpArrow))
//                {
//                    MoveXZStruc(new Vector2Int(-1, 0));
//                    LastModificationTime = Time.time + ModificationWait;
//                }
//                else if(Input.GetKey(KeyCode.DownArrow))
//                {
//                    MoveXZStruc(new Vector2Int(+1, 0));
//                    LastModificationTime = Time.time + ModificationWait;
//                }
//                else if(Input.GetKey(KeyCode.LeftArrow))
//                {
//                    MoveXZStruc(new Vector2Int(0, -1));
//                    LastModificationTime = Time.time + ModificationWait;
//                }
//                else if(Input.GetKey(KeyCode.RightArrow))
//                {
//                    MoveXZStruc(new Vector2Int(0, +1));
//                    LastModificationTime = Time.time + ModificationWait;
//                }
//            }

//            if (GUI.Button(new Rect(rect.width * 0.5f - 150f, rect.height - 30f, 100f, 30f), "Rotate 90º"))
//            {
//                var rot = m_SelectedStruc.Rotation;
//                if (rot + 1 == Def.RotationStateCount)
//                    m_SelectedStruc.Rotation = Def.RotationState.Default;
//                else
//                    m_SelectedStruc.Rotation = rot + 1;
//            }
//            if (GUI.Button(new Rect(rect.width * 0.5f - 50f, rect.height - 30f, 100f, 30f), "Horz Flip"))
//            {
//                switch (m_SelectedStruc.Flip)
//                {
//                    case StructureFlip.NoFlip:
//                        m_SelectedStruc.Flip = StructureFlip.HorizontalFlip;
//                        break;
//                    case StructureFlip.VerticalFlip:
//                        m_SelectedStruc.Flip = StructureFlip.VerticalAndHorizontalFlip;
//                        break;
//                    case StructureFlip.HorizontalFlip:
//                        m_SelectedStruc.Flip = StructureFlip.NoFlip;
//                        break;
//                    case StructureFlip.VerticalAndHorizontalFlip:
//                        m_SelectedStruc.Flip = StructureFlip.VerticalFlip;
//                        break;
//                }
//            }
//            if (GUI.Button(new Rect(rect.width * 0.5f + 50f, rect.height - 30f, 100f, 30f), "Vert Flip"))
//            {
//                switch (m_SelectedStruc.Flip)
//                {
//                    case StructureFlip.NoFlip:
//                        m_SelectedStruc.Flip = StructureFlip.VerticalFlip;
//                        break;
//                    case StructureFlip.VerticalFlip:
//                        m_SelectedStruc.Flip = StructureFlip.NoFlip;
//                        break;
//                    case StructureFlip.HorizontalFlip:
//                        m_SelectedStruc.Flip = StructureFlip.VerticalAndHorizontalFlip;
//                        break;
//                    case StructureFlip.VerticalAndHorizontalFlip:
//                        m_SelectedStruc.Flip = StructureFlip.HorizontalFlip;
//                        break;
//                }
//            }

//            if(GUI.Button(new Rect(rect.width * 0.5f - 200f, rect.height - 30f, 50f, 30f), "Cancel"))
//            {
//                // Destroy structure
//                foreach (var pilar in m_SelectedStruc.Pilars)
//                {
//                    foreach (var block in pilar.Blocks)
//                    {
//                        Material.Destroy(block.TopMR.material);
//                        Material.Destroy(block.MidMR.material);
//                    }
//                }
//                m_SelectedStruc.DestroyStructure();

//                if (BlockOver != null)
//                    BlockOver.Selected = false;
//                if (SelectedPilarMapID >= 0 && SelectedBlockIdx >= 0)
//                    Manager.Mgr.Pilars[SelectedPilarMapID].Blocks[SelectedBlockIdx].Selected = false;

//                SelectedPilarMapID = -1;
//                SelectedBlockIdx = -1;

//                State = PC_State.Default;
//            }
//            if (GUI.Button(new Rect(rect.width * 0.5f + 150f, rect.height - 30f, 50f, 30f), "Apply")
//                || Input.GetKey(KeyCode.Return))
//            {
//                // Restore materials and alpha
//                foreach(var pilar in m_SelectedStruc.Pilars)
//                {
//                    foreach(var block in pilar.Blocks)
//                    {
//                        Material.Destroy(block.TopMR.material);
//                        Material.Destroy(block.MidMR.material);
//                        block.TopMR.material = block.TopMaterialPart.Mat;
//                        block.MidMR.material = block.MidMaterialPart.Mat;
//                        if (block.Prop != null)
//                        {
//                            var pColor = block.Prop.SpriteSR.color;
//                            pColor.a = 1f;
//                            block.Prop.SpriteSR.color = pColor;
//                        }
//                        if (block.Monster != null)
//                        {
//                            var mColor = block.Monster.SpriteSR.color;
//                            mColor.a = 1f;
//                            block.Monster.SpriteSR.color = mColor;
//                        }
//                    }
//                }

//                MapCommandType cmd = MapCommandType.COUNT;
//                if (m_SelectedStruc.Rotation == Def.RotationState.Default && m_SelectedStruc.Flip == StructureFlip.NoFlip)
//                {
//                    cmd = MapCommandType.STRUC_PLACED;
//                }
//                else if(m_SelectedStruc.Rotation != Def.RotationState.Default && m_SelectedStruc.Flip == StructureFlip.NoFlip)
//                {
//                    cmd = MapCommandType.STRUC_PLACED_ROTATED90 + ((int)m_SelectedStruc.Rotation - 1);
//                }
//                else if(m_SelectedStruc.Flip != StructureFlip.NoFlip && m_SelectedStruc.Rotation == Def.RotationState.Default)
//                {
//                    cmd = MapCommandType.STRUC_PLACED_FLIPPEDX + ((int)m_SelectedStruc.Flip - 1);
//                }
//                else if (m_SelectedStruc.Flip == StructureFlip.HorizontalFlip && m_SelectedStruc.Rotation != Def.RotationState.Default)
//                {
//                    cmd = MapCommandType.STRUC_PLACED_FLIPPEDX_ROTATED90 + ((int)m_SelectedStruc.Rotation - 1);
//                }
//                else if (m_SelectedStruc.Flip == StructureFlip.VerticalFlip && m_SelectedStruc.Rotation != Def.RotationState.Default)
//                {
//                    cmd = MapCommandType.STRUC_PLACED_FLIPPEDY_ROTATED90 + ((int)m_SelectedStruc.Rotation - 1);
//                }
//                else if (m_SelectedStruc.Flip == StructureFlip.VerticalAndHorizontalFlip && m_SelectedStruc.Rotation != Def.RotationState.Default)
//                {
//                    cmd = MapCommandType.STRUC_PLACED_FLIPPEDXY_ROTATED90 + ((int)m_SelectedStruc.Rotation - 1);
//                }
//                else
//                {
//                    throw new Exception("Unknown how to save this structure.");
//                }

//                var pilarID = GameUtils.MapIDFromPosition(new Vector2(m_SelectedStruc.transform.position.x + 0.1f, m_SelectedStruc.transform.position.z + 0.1f));
//                Manager.Mgr.Map.Record(new MapCommand(
//                    cmd, $"{Structures.Strucs[m_SelectedStrucID].GetName()}@{m_SelectedStrucOffset}",
//                    pilarID, -1));
//                m_SelectedStruc.ApplyStairs(true);
//                m_SelectedStruc.ApplyWides(true);
//                Manager.Mgr.PlaceStruct(m_SelectedStruc);
//                Manager.Mgr.AddEmptyBlocks(pilarID);
//                m_SelectedStruc = null;
//                if (BlockOver != null)
//                    BlockOver.Selected = false;
//                if (SelectedPilarMapID >= 0 && SelectedBlockIdx >= 0)
//                    Manager.Mgr.Pilars[SelectedPilarMapID].Blocks[SelectedBlockIdx].Selected = false;

//                SelectedPilarMapID = -1;
//                SelectedBlockIdx = -1;
//                State = PC_State.Default;
//            }
//        }

//        void OnSelectingStruc()
//        {
//            var sel = Selector.OnGUI();
//            if (sel)
//            {
//                var selList = Selector.GetSelected();
//                if (selList.Count > 0)
//                {
//                    //m_SelectedStructure = selList[0];
//                    const int SWidth = StructureComponent.Width;
//                    const int SHeight = StructureComponent.Height;
//                    const float BSeparation = StructureComponent.Separation;
//                    const float BlockSize = 1f + BSeparation;

//                    m_SelectedStrucOffset = 0f;
//                    m_SelectedStrucID = selList[0];
//                    var mapPos = GameUtils.PosFromID(SelectedPilarMapID, Manager.MapWidth, Manager.MapHeight);
//                    m_SelectedStruc = new GameObject($"Struc_({mapPos.x},{mapPos.y})").AddComponent<StructureComponent>();
//                    var strucPos = new Vector3(mapPos.x * BlockSize, 0.0f, mapPos.y * BlockSize);
//                    m_SelectedStruc.StructRect = new Rect(strucPos.x, strucPos.z, SWidth * BlockSize, SHeight * BlockSize);
//                    m_SelectedStruc.transform.SetPositionAndRotation(strucPos, Quaternion.identity);
//                    //m_SelectedStruc.
//                    Structures.Strucs[m_SelectedStrucID].ToStructure(ref m_SelectedStruc, false);

//                    var transparent = Materials.GetMaterial(Def.Materials.Transparent);

//                    var colored = Materials.GetMaterial(Def.Materials.ColoredDefault);
//                    var colTrans = Materials.GetMaterial(Def.Materials.ColoredTransparent);
//                    var colCutout = Materials.GetMaterial(Def.Materials.ColoredCutout);

//                    Material GetChangedMaterial(Material original)
//                    {
//                        Material rtn = null;
//                        if (original.name == colored.name 
//                            || original.name == colTrans.name
//                            || original.name == colCutout.name)
//                        {
//                            rtn = new Material(colTrans);
//                            rtn.SetTexture(AssetContainer.ColoredMaterialTextureID, original.GetTexture(AssetContainer.ColoredMaterialTextureID));
//                            rtn.SetVector(AssetContainer.ColoredMaterialCSHBID, original.GetVector(AssetContainer.ColoredMaterialCSHBID));
//                        }
//                        else
//                        {
//                            rtn = new Material(transparent);
//                            rtn.SetTexture(Def.MaterialTextureID, original.GetTexture(Def.MaterialTextureID));
//                        }
//                        return rtn;
//                    }

//                    foreach (var pilar in m_SelectedStruc.Pilars)
//                    {
//                        foreach(var block in pilar.Blocks)
//                        {
//                            var tempTopMaterial = GetChangedMaterial(block.TopMaterialPart.Mat);
//                            var tempMidMaterial = GetChangedMaterial(block.MidMaterialPart.Mat);

//                            var topColor = tempTopMaterial.GetColor(AssetContainer.MaterialColorID);
//                            topColor.a = 0.5f;
//                            tempTopMaterial.SetColor(AssetContainer.MaterialColorID, topColor);
//                            var midColor = tempMidMaterial.GetColor(AssetContainer.MaterialColorID);
//                            midColor.a = 0.5f;
//                            tempMidMaterial.SetColor(AssetContainer.MaterialColorID, midColor);
//                            block.TopMR.material = tempTopMaterial;
//                            block.MidMR.material = tempMidMaterial;
//                            if (block.Prop != null)
//                            {
//                                var pColor = block.Prop.SpriteSR.color;
//                                pColor.a = 0.5f;
//                                block.Prop.SpriteSR.color = pColor;
//                            }
//                            if(block.Monster != null)
//                            {
//                                var mColor = block.Monster.SpriteSR.color;
//                                mColor.a = 0.5f;
//                                block.Monster.SpriteSR.color = mColor;
//                            }
//                        }
//                    }
//                    State = PC_State.PlacingStruct;
//                }
//                else
//                {
//                    if (BlockOver != null)
//                        BlockOver.Selected = false;
//                    if (SelectedPilarMapID >= 0 && SelectedBlockIdx >= 0)
//                        Manager.Mgr.Pilars[SelectedPilarMapID].Blocks[SelectedBlockIdx].Selected = false;

//                    SelectedPilarMapID = -1;
//                    SelectedBlockIdx = -1;
//                    State = PC_State.Default;
//                }
//                //if (selList.Count > 0)
//                //{
//                //    int selected = selList[0];
//                //    // Load struct
//                //    {
//                //        //var mapPos = GameUtils.PosFromID(SelectedPilarMapID, Manager.MapWidth, Manager.MapHeight);
//                //        //var strucGO = new GameObject($"Struc_({mapPos.x},{mapPos.y})");
//                //        //var struc = strucGO.AddComponent<StructureComponent>();
//                //        //var strucPos = new Vector3(mapPos.x * (1.0f + StructureComponent.Separation), 0.0f, mapPos.y * (1.0f + StructureComponent.Separation));
//                //        //struc.StructRect = new Rect(strucPos.x, strucPos.z, StructureComponent.Width * (1.0f + StructureComponent.Separation), StructureComponent.Height * (1.0f + StructureComponent.Separation));
//                //        //strucGO.transform.SetPositionAndRotation(strucPos, Quaternion.identity);
//                //        //strucGO.isStatic = true;
//                //        //Manager.Mgr.Map.Record(new MapCommand(MapCommandType.STRUC_PLACED, Structures.Strucs[selected].Name, SelectedPilarMapID, SelectedBlockIdx));
//                //        //Structures.Strucs[selected].ToStructure(ref struc, false);
//                //        //struc.ApplyStairs(true);
//                //        //struc.ApplyWides(true);
//                //        //for (int y = 0; y < StructureComponent.Height; ++y)
//                //        //{
//                //        //    for (int x = 0; x < StructureComponent.Width; ++x)
//                //        //    {
//                //        //        var pilarID = GameUtils.IDFromPos(new Vector2Int(x, y));
//                //        //        var pilar = struc.Pilars[pilarID];
//                //        //        bool hasStrucPilarValidBlocks = false;
//                //        //        for (int i = 0; i < pilar.Blocks.Count; ++i)
//                //        //        {
//                //        //            var sBlock = pilar.Blocks[i];
//                //        //            if (sBlock != null && sBlock.Layer > 0)
//                //        //            {
//                //        //                hasStrucPilarValidBlocks = true;
//                //        //                break;
//                //        //            }
//                //        //        }
//                //        //        if (!hasStrucPilarValidBlocks)
//                //        //        {
//                //        //            pilar.DestroyPilar();
//                //        //        }
//                //        //        else
//                //        //        {
//                //        //            bool hasMapRemovedBlocks = false;
//                //        //            bool hasMapWideBlocks = false;
//                //        //            var mapID = GameUtils.IDFromPos(new Vector2Int(mapPos.x + x, mapPos.y + y), Manager.MapWidth, Manager.MapHeight);
//                //        //            var mapPilar = Manager.Mgr.Pilars[mapID];
//                //        //            for (int i = 0; i < mapPilar.Blocks.Count; ++i)
//                //        //            {
//                //        //                var pBlock = mapPilar.Blocks[i];
//                //        //                if (pBlock == null)
//                //        //                    continue;
//                //        //                if (pBlock.Removed)
//                //        //                {
//                //        //                    hasMapRemovedBlocks = true;
//                //        //                }
//                //        //                if (pBlock.blockType == BlockType.WIDE)
//                //        //                {
//                //        //                    hasMapWideBlocks = true;
//                //        //                }
//                //        //            }
//                //        //            if (!hasMapRemovedBlocks)
//                //        //            {
//                //        //                pilar.MapID = mapID;
//                //        //                if (hasMapWideBlocks)
//                //        //                {
//                //        //                    var pilarPos = GameUtils.PosFromID(pilar.MapID, Manager.MapWidth, Manager.MapHeight);
//                //        //                    int[] blocks = new int[3]
//                //        //                    {
//                //        //                    GameUtils.IDFromPos(new Vector2Int(pilarPos.x + 1, pilarPos.y), Manager.MapWidth, Manager.MapHeight),       // right
//                //        //                    GameUtils.IDFromPos(new Vector2Int(pilarPos.x + 1, pilarPos.y + 1), Manager.MapWidth, Manager.MapHeight),   // bottomRight
//                //        //                    GameUtils.IDFromPos(new Vector2Int(pilarPos.x, pilarPos.y + 1), Manager.MapWidth, Manager.MapHeight)        // bottom
//                //        //                    };
//                //        //                    for (int i = 0; i < blocks.Length; ++i)
//                //        //                    {
//                //        //                        var wPilar = Manager.Mgr.Pilars[blocks[i]];
//                //        //                        for (int j = 0; j < wPilar.Blocks.Count; ++j)
//                //        //                        {
//                //        //                            var wBlock = wPilar.Blocks[j];
//                //        //                            if (!wBlock.Removed)
//                //        //                                continue;

//                //        //                            wBlock.TopMR.enabled = true;
//                //        //                            //wBlock.TopBC.enabled = true;
//                //        //                            wBlock.MidMR.enabled = true;
//                //        //                            //wBlock.MidBC.enabled = true;
//                //        //                            wBlock.BlockBC.enabled = true;

//                //        //                            wBlock.Removed = false;
//                //        //                        }
//                //        //                    }
//                //        //                }

//                //        //                mapPilar.DestroyPilar();
//                //        //                Manager.Mgr.Pilars[pilar.MapID] = pilar;
//                //        //            }
//                //        //        }
//                //        //    }
//                //        //}
//                //        //Manager.Mgr.Strucs.Add(struc);
//                //        //// Add the empty spaces
//                //        //var emptyPos = mapPos - new Vector2Int(8, 8);
//                //        //for (int y = 0; y < (8 * 3); ++y)
//                //        //{
//                //        //    for (int x = 0; x < (8 * 3); ++x)
//                //        //    {
//                //        //        var mapID = GameUtils.IDFromPos(emptyPos + new Vector2Int(x, y), Manager.MapWidth, Manager.MapHeight);
//                //        //        var pilar = Manager.Mgr.Pilars[mapID];
//                //        //        bool added = false;
//                //        //        if (pilar == null)
//                //        //        {
//                //        //            var pilarGO = new GameObject("InvalidPilar");
//                //        //            pilar = pilarGO.AddComponent<PilarComponent>();
//                //        //            pilar.Init(null, -1, mapID);
//                //        //            Manager.Mgr.Pilars[mapID] = pilar;
//                //        //            pilar.AddBlock();
//                //        //            added = true;
//                //        //        }
//                //        //        else
//                //        //        {
//                //        //            if (pilar.Blocks.Count == 0)
//                //        //            {
//                //        //                pilar.AddBlock();
//                //        //                added = true;
//                //        //            }
//                //        //        }
//                //        //        if (added)
//                //        //        {
//                //        //            var block = pilar.Blocks[0];
//                //        //            block.gameObject.isStatic = true;
//                //        //            block.TopMR.gameObject.isStatic = true;
//                //        //            block.LayerSR.enabled = false;
//                //        //            block.LayerSR.gameObject.isStatic = true;
//                //        //            block.LayerSR.gameObject.SetActive(false);
//                //        //            block.StairSR.enabled = false;
//                //        //            block.StairSR.gameObject.isStatic = true;
//                //        //            block.StairSR.gameObject.SetActive(false);
//                //        //            block.LockSR.enabled = false;
//                //        //            block.LockSR.gameObject.isStatic = true;
//                //        //            block.LockSR.gameObject.SetActive(false);
//                //        //            block.AnchorSR.enabled = false;
//                //        //            block.AnchorSR.gameObject.isStatic = true;
//                //        //            block.AnchorSR.gameObject.SetActive(false);

//                //        //            block.MidMR.enabled = false;
//                //        //            //block.MidBC.enabled = false;
//                //        //            block.MidMR.gameObject.isStatic = true;
//                //        //            block.MidMR.gameObject.SetActive(false);
//                //        //        }
//                //        //    }
//                //        //}
//                //        //for (int i = 0; i < struc.Pilars.Length; ++i)
//                //        //{
//                //        //    var pilar = struc.Pilars[i];
//                //        //    if (pilar == null)
//                //        //        continue;
//                //        //    pilar.gameObject.isStatic = true;
//                //        //    for (int j = 0; j < pilar.Blocks.Count; ++j)
//                //        //    {
//                //        //        var block = pilar.Blocks[j];
//                //        //        block.gameObject.isStatic = true;
//                //        //        block.TopMR.gameObject.isStatic = false;
//                //        //        block.MidMR.gameObject.isStatic = false;
//                //        //        block.AnchorSR.enabled = false;
//                //        //        block.AnchorSR.gameObject.SetActive(false);
//                //        //        block.LayerSR.enabled = false;
//                //        //        block.LayerSR.gameObject.SetActive(false);
//                //        //        block.LockSR.enabled = false;
//                //        //        block.LockSR.gameObject.SetActive(false);
//                //        //        block.StairSR.enabled = false;
//                //        //        block.StairSR.gameObject.SetActive(false);

//                //        //        if (block.Monster != null)
//                //        //        {
//                //        //            //var rng = Manager.Mgr.BuildRNG;
//                //        //            //var offset = new Vector2((float)rng.NextDouble(), (float)rng.NextDouble());
//                //        //            var offset = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
//                //        //            block.Monster.transform.position = new Vector3(pilar.transform.position.x + offset.x, block.Height + block.MicroHeight, pilar.transform.position.z + offset.y);
//                //        //            //var pos = new Vector3(pilar.transform.position.x + offset.x, block.Height + block.MicroHeight, pilar.transform.position.z + offset.y);
//                //        //            //block.Monster.transform.Translate(pos - block.Monster.transform.position, Space.World);
//                //        //            block.Monster.SpriteSR.enabled = true;
//                //        //            //block.Monster.SpriteBC.enabled = true;
//                //        //            block.Monster.SpriteCC.enabled = true;
//                //        //            block.Monster.ShadowSR.enabled = true;
//                //        //            //block.Monster.Struc = struc;
//                //        //            //struc.LivingEntities.Add(block.Monster);
//                //        //            block.Monster = null;
//                //        //        }
//                //        //        if (block.Prop != null)
//                //        //        {
//                //        //            if (block.blockType == BlockType.STAIRS)
//                //        //            {
//                //        //                if (struc.LivingEntities.Contains(block.Prop))
//                //        //                    struc.LivingEntities.Remove(block.Prop);
//                //        //                GameUtils.DeleteGameObjectAndItsChilds(block.Prop.gameObject);
//                //        //                block.Prop = null;
//                //        //            }
//                //        //            else
//                //        //            {
//                //        //                block.Prop.Block = block;
//                //        //                block.Prop.SpriteSR.enabled = true;
//                //        //                block.Prop.SpriteBC.enabled = true;
//                //        //                if (block.Prop.ShadowSR != null)
//                //        //                    block.Prop.ShadowSR.enabled = true;
//                //        //                if (block.Prop.PropLight != null)
//                //        //                    block.Prop.PropLight.enabled = true;
//                //        //            }
//                //        //        }
//                //        //    }
//                //        //}
//                //    }
//                //    Manager.Mgr.Map.Record(new MapCommand(
//                //        MapCommandType.STRUC_PLACED, Structures.Strucs[selected].Name, SelectedPilarMapID, SelectedBlockIdx));
//                //    var struc = Manager.Mgr.PlaceStruct(SelectedPilarMapID, selected);
//                //    Manager.Mgr.AddEmptyBlocks(SelectedPilarMapID);
//                //    var oddPosition = new Vector2(struc.transform.position.x, struc.transform.position.z) +
//                //        new Vector2((StructureComponent.Width / 2) * (1.0f + StructureComponent.Separation), (StructureComponent.Height / 2) * (1.0f + StructureComponent.Separation));
//                //    oddScript.MoveTo(oddPosition);
//                //}
//                //State = PC_State.Default;
//                //if (BlockOver != null)
//                //    BlockOver.Selected = false;
//                //if (SelectedPilarMapID >= 0 && SelectedBlockIdx >= 0)
//                //    Manager.Mgr.Pilars[SelectedPilarMapID].Blocks[SelectedBlockIdx].Selected = false;

//                //SelectedPilarMapID = -1;
//                //SelectedBlockIdx = -1;
//            }
//        }

//        void OnSelectingProp()
//        {
//            var sel = Selector.OnGUI();
//            if (sel)
//            {
//                var selected = Selector.GetSelected();
//                State = PC_State.Default;
//                if (selected.Count == 0)
//                {
//                    return;
//                }

//                Manager.Mgr.PlaceProp(SelectedPilarMapID, SelectedBlockIdx, selected[0]);

//                //var selectedBlock = Manager.Mgr.Pilars[SelectedPilarMapID].Blocks[SelectedBlockIdx];

//                //if (selectedBlock.Prop != null)
//                //    selectedBlock.Prop.ReceiveDamage(Def.DamageType.UNAVOIDABLE, selectedBlock.Prop.GetTotalHealth());

//                //var propFamily = Props.PropFamilies[selected[0]];
//                //var propID = Manager.Mgr.BuildRNG.Next(0, propFamily.Props.Length);
//                //var prop = new GameObject($"Prop_{PropScript.PropID++}");
//                //selectedBlock.Prop = prop.AddComponent<PropScript>();
//                //Manager.Mgr.Map.Record(new MapCommand(MapCommandType.PROP_PLACED, propFamily.FamilyName, SelectedPilarMapID, SelectedBlockIdx));
//                //selectedBlock.Prop.SetProp(selected[0], propID);
//                //selectedBlock.Prop.Block = selectedBlock;
//                //float nChance = UnityEngine.Random.value;
//                //var facing = selectedBlock.Prop.Facing;
//                //if (nChance >= 0.5f)
//                //    facing.Horizontal = SpriteHorizontal.RIGHT;
//                //selectedBlock.Prop.Facing = facing;
//            }
//        }

//        void OnSelectingMonster()
//        {
//            var sel = Selector.OnGUI();
//            if (sel)
//            {
//                var selected = Selector.GetSelected();
//                State = PC_State.Default;
//                if (selected.Count == 0)
//                {
//                    return;
//                }

//                Manager.Mgr.PlaceMonster(SelectedPilarMapID, SelectedBlockIdx, selected[0]);
//                //var selectedBlock = Manager.Mgr.Pilars[SelectedPilarMapID].Blocks[SelectedBlockIdx];

//                ////var monster = Monsters.MonsterInfos[selected[0]];
//                //var mon = new GameObject($"Monster_{MonsterScript.MonsterID++}");
//                ////mon.transform.Translate(selectedBlock.Pilar.transform.position, Space.World);
                
//                ////var rng = Manager.Mgr.BuildRNG;
//                //var offset = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
//                //mon.transform.position = selectedBlock.Pilar.transform.position + new Vector3(offset.x, selectedBlock.Height + selectedBlock.MicroHeight, offset.y);
//                ////mon.transform.Translate(new Vector3(offset.x, selectedBlock.Height + selectedBlock.MicroHeight, offset.y), Space.World);
//                //var smon = Monsters.AddMonsterComponent(mon, selected[0]);
//                ////selectedBlock.MonsterGO = mon;
//                //smon.InitMonster();
//                //Manager.Mgr.Map.Record(new MapCommand(MapCommandType.MONSTER_PLACED, smon.Info.Name, SelectedPilarMapID, SelectedBlockIdx));
//                ////smon.SetMonster(selected[0]);
//                ////smon.Struc = selectedBlock.Pilar.Struc;
//                ////selectedBlock.Pilar.Struc.LivingEntities.Add(smon);
//                //smon.enabled = true;
//                //var facing = smon.Facing;
//                //float nChance = UnityEngine.Random.value;
//                //if (nChance >= 0.5f)
//                //    facing.Horizontal = SpriteHorizontal.RIGHT;
//                //nChance = UnityEngine.Random.value;
//                //if (nChance >= 0.5f)
//                //    facing.Vertical = SpriteVertical.UP;
//                //smon.Facing = facing;
//            }
//        }

//        void OnSelectingBackground()
//        {
//            var sel = Selector.OnGUI();
//            if(sel)
//            {
//                var selected = Selector.GetSelected();
//                State = PC_State.Default;
//                if (selected.Count == 0)
//                    return;

//                var selectedBackground = selected[0];
//                if (selectedBackground < 0 || selectedBackground >= Backgrounds.Infos.Count)
//                    return;
//                //if (selectedBackground < 0 || selectedBackground >= AssetContainer.Mgr.BackgroundTextures.Length)
//                //    return;

//                var backgroundInfo = Backgrounds.Infos[selectedBackground];
//                var backgroundMistColor = backgroundInfo.MistGradient;
//                var backgroundTexture = backgroundInfo.BackgroundTexture;

//                //var backgroundMistColor = AssetContainer.Mgr.BackgroundMistColor[selectedBackground];
//                var backgroundMaterial = AssetContainer.Mgr.BackgroundMaterial;
//                //var backgroundTexture = AssetContainer.Mgr.BackgroundTextures[selectedBackground];
//                backgroundMaterial.mainTexture = backgroundTexture;
//                for(int i = 0; i < MistObjects.Length; ++i)
//                {
//                    var effect = MistObjects[i].GetComponent<VisualEffect>();
//                    effect.SetGradient(AssetContainer.BackgroundColorID, backgroundMistColor);
//                }
//            }
//        }

//        void OnSelectingAnt()
//        {
//            if(m_AntSelector.OnGUI(SelectedPilarMapID, SelectedBlockIdx))
//            {
//                SaveAntState();
//                State = PC_State.Default;
//            }
//        }

//        void OnFogDistanceChange(bool near)
//        {
//            if(near)
//            {
//                oddScript.FogMgr.Radius = oddScript.FogMgr.Radius - 2.0f * Time.deltaTime;
//                oddScript.FogMgr.Radius = Mathf.Max(oddScript.FogMgr.Radius, 0.0f);
//            }
//            else
//            {
//                oddScript.FogMgr.Radius = oddScript.FogMgr.Radius + 2.0f * Time.deltaTime;
//                oddScript.FogMgr.Radius = Mathf.Min(oddScript.FogMgr.Radius, 50.0f);
//            }
//        }

//        void OnGUIMenus()
//        {
//            var changeBackground = GUI.Button(new Rect(5.0f, 5.0f, 130.0f, 25.0f), "Change Background");
//            if(changeBackground)
//            {
//                State = PC_State.BackgroundSel;
//                Selector.Reset(null);
//                Selector.SelectorType = ISType.Background;
//                Selector.MultiSelection = false;
//                return;
//            }
//            float lastHeight = 5f + 25f;
//            if (SelectedPilarMapID < 0 || SelectedBlockIdx < 0)
//                return;

//            var addStruct = GUI.Button(new Rect(5.0f, lastHeight, 100.0f, 25.0f), "Add Structure");
//            lastHeight += 25f;
//            if (addStruct)
//            {
//                State = PC_State.StructSel;
//                Selector.Reset(null);
//                Selector.SelectorType = ISType.Struc;
//                Selector.MultiSelection = false;
//                return;
//            }

//            var block = Manager.Mgr.Pilars[SelectedPilarMapID].Blocks[SelectedBlockIdx];
//            if (block.Layer == 0)
//                return;

//            var addProp = GUI.Button(new Rect(5.0f, lastHeight, 100.0f, 25.0f), "Add Prop");
//            lastHeight += 25f;
//            if (addProp)
//            {
//                State = PC_State.PropSel;
//                Selector.Reset(new List<int>(Enumerable.Repeat(0, 1)));
//                Selector.SelectorType = ISType.Prop;
//                Selector.MultiSelection = false;
//                return;
//            }

//            var addMonster = GUI.Button(new Rect(5.0f, lastHeight, 100.0f, 25.0f), "Add Monster");
//            lastHeight += 25f;
//            if (addMonster)
//            {
//                State = PC_State.MonsterSel;

//                Selector.Reset(new List<int>(Enumerable.Repeat(0, 1)));
//                Selector.SelectorType = ISType.Monster;
//                Selector.MultiSelection = false;
//                return;
//            }

//            var setAnt = GUI.Button(new Rect(5f, lastHeight, 100f, 25f), "Set Ants");
//            lastHeight += 25f;
//            if(setAnt)
//            {
//                RecordAntState();
//                m_AntSelector.Reset();
//                State = PC_State.AntSet;
//                return;
//            }
//        }

//        void RecordAntState()
//        {
//            var block = Manager.Mgr.Pilars[SelectedPilarMapID].Blocks[SelectedBlockIdx];
//            var ants = block.Ants;
//            for(int i = 0; i < m_PrevAntState.Length; ++i)
//            {
//                if (ants[i] != null)
//                    m_PrevAntState[i] = ants[i].AntState;
//                else
//                    m_PrevAntState[i] = -1;
//            }
//        }

//        void SaveAntState()
//        {
//            //var preAntState = m_PrevAntState;
//            var preAntState = new int[m_PrevAntState.Length];
//            m_PrevAntState.CopyTo(preAntState, 0);
//            RecordAntState();

//            for (int i = 1; i < m_PrevAntState.Length; ++i)
//            {
//                if(preAntState[i] !=  m_PrevAntState[i])
//                {
//                    if (m_PrevAntState[i] < 0)
//                        Manager.Mgr.Map.Record(
//                            new MapCommand(MapCommandType.ANT_DEATH, ((Def.DecoPosition)i).ToString(), SelectedPilarMapID, SelectedBlockIdx));
//                    else
//                        Manager.Mgr.Map.Record(
//                            new MapCommand(MapCommandType.ANT_PLACED_SIDE_U + (m_PrevAntState[i] % 2 != 0 ? 1 : 0), ((Def.DecoPosition)i).ToString(), SelectedPilarMapID, SelectedBlockIdx));
//                }
//            }

//            if (preAntState[0] != m_PrevAntState[0])
//            {
//                if (m_PrevAntState[0] < 0)
//                    Manager.Mgr.Map.Record(
//                        new MapCommand(MapCommandType.ANT_DEATH, Def.DecoPosition.TOP.ToString(), SelectedPilarMapID, SelectedBlockIdx));
//                else
//                    Manager.Mgr.Map.Record(
//                        new MapCommand(MapCommandType.ANT_PLACED_TOP, ((AntTopDirection)m_PrevAntState[0]).ToString(), SelectedPilarMapID, SelectedBlockIdx));
//            }
//        }

//        public void OnGUI()
//        {
//            var canvas = Manager.Mgr.m_Canvas;
//            var rect = canvas.pixelRect;
//            switch(State)
//            {
//                case PC_State.StructSel:
//                    OnSelectingStruc();
//                    break;
//                case PC_State.PropSel:
//                    OnSelectingProp();
//                    break;
//                case PC_State.MonsterSel:
//                    OnSelectingMonster();
//                    break;
//                case PC_State.BackgroundSel:
//                    OnSelectingBackground();
//                    break;
//                case PC_State.AntSet:
//                    OnSelectingAnt();
//                    break;
//                case PC_State.PlacingStruct:
//                    OnPlacingStruct();
//                    break;
//                case PC_State.Default:
//                    OnGUIMenus();
//                    Manager.Mgr.Map.OnGUI(canvas);
//                    break;
//            }
//        }

//        public void Start()
//        {
//            const int startingX = Manager.MapWidth / 2 - StructureComponent.Width / 2;
//            const int startingY = Manager.MapHeight / 2 - StructureComponent.Height / 2;
//            Manager.Mgr.StartSimpleMap();
//            Manager.Mgr.OddGO.SetActive(true);
//            oddScript = Manager.Mgr.OddGO.GetComponent<OddScript>();
//            var oddPos = new Vector2((startingX + StructureComponent.Width / 2) * (1.0f + StructureComponent.Separation), (startingY + StructureComponent.Height / 2) * (1.0f + StructureComponent.Separation));
//            oddScript.MoveTo(oddPos);
//            oddScript.Position = new Vector3(oddPos.x, 0.0f, oddPos.y);
//            oddScript.transform.SetPositionAndRotation(oddScript.Position, Quaternion.identity);
//            SelectedPilarMapID = -1;
//            SelectedBlockIdx = -1;
//            //StrucSelector = new StructureSelector();
//            //StrucSelector.Start();
//            Selector = new ImageSelector();
//            Selector.Start();
//            State = PC_State.Default;
//            Manager.Mgr.HideInfo = false;
//            //IsPlaying = false;

//            // Add mist components
//            VisualEffectAsset mistAsset = null;
//            for(int i = 0; i < AssetContainer.Mgr.VisualEffects.Length; ++i)
//            {
//                var curEffect = AssetContainer.Mgr.VisualEffects[i];
//                if(curEffect.name.ToLower() == "mist")
//                {
//                    mistAsset = curEffect;
//                    break;
//                }
//            }
//            if (mistAsset == null)
//                throw new Exception("Couldn't find the Mist Visual Effect Asset in the AssetContainer");
//            var texName = AssetContainer.Mgr.BackgroundMaterial.mainTexture.name;
//            int backGroundID = -1;
//            Gradient backgroundMistColor = null;
//            for(int i = 0; i < Backgrounds.Infos.Count; ++i)
//            {
//                var curInfo = Backgrounds.Infos[i];
//                if(curInfo.BackgroundTexture.name == texName)
//                {
//                    backGroundID = i;
//                    backgroundMistColor = curInfo.MistGradient;
//                    break;
//                }
//            }
//            //for (int i = 0; i < AssetContainer.Mgr.BackgroundTextures.Length; ++i) 
//            //{
//            //    var curTex = AssetContainer.Mgr.BackgroundTextures[i];
//            //    if(curTex.name == texName)
//            //    {
//            //        backGroundID = i;
//            //        break;
//            //    }
//            //}
//            if (backGroundID < 0)
//                backGroundID = 0;
//            //var backgroundMistColor = AssetContainer.Mgr.BackgroundMistColor[backGroundID];
//            for(int i = 0; i < MistObjects.Length; i++)
//            {
//                var curObj = MistObjects[i];
//                var ve = curObj.GetComponent<VisualEffect>();
//                if(ve == null)
//                    ve = curObj.AddComponent<VisualEffect>();

//                ve.visualEffectAsset = mistAsset;
//                ve.SetGradient(AssetContainer.BackgroundColorID, backgroundMistColor);
//            }

//            Manager.Mgr.RayPlane.SetActive(true);
//            m_AntSelector = new AntSelector();
//            m_AntSelector.Start();
//            m_PrevAntState = new int[Def.DecoPositionCount];
//            //m_RayCasts = new RaycastHit[32];
//            //m_TempCasts = new RaycastHit[32];
//        }

//        public void Stop()
//        {
//            Manager.Mgr.OddGO.SetActive(false);

//            Manager.Mgr.ClearMap();
//            //StrucSelector.Stop();
//            //StrucSelector = null;
//            Selector.Stop();
//            Selector = null;
//            BlockOver = null;

//            foreach(var mistObj in MistObjects)
//            {
//                Component.DestroyImmediate(mistObj.GetComponent<VisualEffect>());
//            }
//            //for(int i = 0; i < MistObjects.Length; ++i)
//            //{
//            //    Component.Destroy(MistObjects[i].GetComponent<VisualEffect>());
//            //}

//            Manager.Mgr.RayPlane.SetActive(false);
//            m_AntSelector.Stop();
//            m_AntSelector = null;
//        }

//        public void ToggleHideInfo()
//        {
//            var hideInfo = !Manager.Mgr.HideInfo;
//            Manager.Mgr.HideInfo = hideInfo;
//            if(m_SelectedBridge != null)
//            {
//                m_SelectedBridge.BridgeOutline = false;
//                m_SelectedBridge = null;
//            }
//            if (BlockOver != null)
//            {
//                BlockOver.Selected = false;
//                BlockOver.Highlighted = false;
//                BlockOver = null;
//            }
//            if (SelectedPilarMapID >= 0 && SelectedBlockIdx >= 0)
//            {
//                var block = Manager.Mgr.Pilars[SelectedPilarMapID].Blocks[SelectedBlockIdx];
//                SelectedBlockIdx = -1;
//                SelectedPilarMapID = -1;
//                block.Selected = false;
//                block.Highlighted = false;
//            }
//            for (int i = 0; i < Manager.Mgr.Pilars.Count; ++i)
//            {
//                var pilar = Manager.Mgr.Pilars[i];
//                if (pilar == null || (pilar != null && pilar.Blocks.Count == 0))
//                    continue;

//                for (int j = 0; j < pilar.Blocks.Count; ++j)
//                {
//                    var block = pilar.Blocks[j];
//                    if (block.Layer != 0)
//                        continue;
//                    //block.TopBC.enabled = !hideInfo;
//                    block.TopMR.enabled = !hideInfo;
//                    //block.MidBC.enabled = !hideInfo;
//                    block.MidMR.enabled = !hideInfo;
//                    block.BlockBC.enabled = !hideInfo;
//                }
//            }
//        }

//        void RepeatAntSelection()
//        {
//            m_AntSelector.SetSelectedOptions(SelectedPilarMapID, SelectedBlockIdx);
//        }

//        //void CreateBridge()
//        //{
//        //    var oddFront = oddScript.transform.forward;
//        //    var oddFrontX = Mathf.Abs(oddFront.x);
//        //    var oddFrontZ = Mathf.Abs(oddFront.z);
//        //    Vector2Int dir = Vector2Int.zero;
//        //    BridgeDirection direction = BridgeDirection.COUNT;
//        //    if (oddFrontX > oddFrontZ)
//        //    {
//        //        if(oddFront.x > 0.0f)
//        //        {
//        //            dir.Set(1, 0);
//        //            direction = BridgeDirection.SOUTH;
//        //        }
//        //        else
//        //        {
//        //            dir.Set(-1, 0);
//        //            direction = BridgeDirection.NORTH;
//        //        }
//        //    }
//        //    else
//        //    {
//        //        if (oddFront.z > 0.0f)
//        //        {
//        //            dir.Set(0, 1);
//        //            direction = BridgeDirection.EAST;
//        //        }
//        //        else
//        //        {
//        //            dir.Set(0, -1);
//        //            direction = BridgeDirection.WEST;
//        //        }
//        //    }
            

//        //    List<PilarComponent> foundPilars = new List<PilarComponent>(16);
//        //    var oddMapID = GameUtils.MapIDFromPosition(new Vector2(oddScript.transform.position.x, oddScript.transform.position.z));
//        //    var checkPos = GameUtils.PosFromID(oddMapID, Manager.MapWidth, Manager.MapHeight);
//        //    PilarComponent oddPilar = Manager.Mgr.Pilars[oddMapID];
//        //    BlockComponent oddBlock = oddPilar.Blocks[oddPilar.Blocks.Count - 1];
//        //    float oddBlockHeight = oddBlock.Height + oddBlock.MicroHeight;
//        //    if (oddBlock.blockType == BlockType.STAIRS)
//        //    {
//        //        switch (oddBlock.Rotation)
//        //        {
//        //            case BlockRotation.Default:
//        //                if (direction == BridgeDirection.NORTH)
//        //                    oddBlockHeight += 0.5f;
//        //                //if (dir.x < 0)
//        //                //{
//        //                //    oddBlockHeight += 0.5f;
//        //                //}
//        //                break;
//        //            case BlockRotation.Left:
//        //                if (direction == BridgeDirection.EAST)
//        //                    oddBlockHeight += 0.5f;
//        //                //if (dir.y > 0)
//        //                //{
//        //                //    oddBlockHeight += 0.5f;
//        //                //}
//        //                break;
//        //            case BlockRotation.Half:
//        //                if(direction == BridgeDirection.SOUTH)
//        //                    oddBlockHeight += 0.5f;
//        //                //if (dir.x > 0)
//        //                //{
//        //                //    oddBlockHeight += 0.5f;
//        //                //}
//        //                break;
//        //            case BlockRotation.Right:
//        //                if(direction == BridgeDirection.WEST)
//        //                    oddBlockHeight += 0.5f;
//        //                //if (dir.y < 0)
//        //                //{
//        //                //    oddBlockHeight += 0.5f;
//        //                //}
//        //                break;
//        //        }
//        //    }

//        //    for (int i = 0; i < foundPilars.Capacity; ++i)
//        //    {
//        //        var currentPos = checkPos + dir * (i + 1);

//        //        var currentMapID = GameUtils.IDFromPos(currentPos, Manager.MapWidth, Manager.MapHeight);
//        //        var currentPilar = Manager.Mgr.Pilars[currentMapID];
//        //        if (currentPilar == null || (currentPilar != null && currentPilar.Blocks.Count == 0))
//        //            break;
//        //        foundPilars.Add(currentPilar);
//        //    }
//        //    // Blocks that the bridge can be attached to
//        //    BlockComponent TargetBlock = null;
//        //    int targetPilar = -1;
//        //    float heightDiff = 0.0f;
//        //    float lastBlockHeight = 0.0f;
//        //    for (int i = 0; i < foundPilars.Count; ++i)
//        //    {
//        //        var maxDist = (i + 1) * 0.3f;
//        //        var curPilar = foundPilars[i];
//        //        for (int j = 0; j < curPilar.Blocks.Count; ++j)
//        //        {
//        //            var curBlock = curPilar.Blocks[j];
//        //            if (curBlock == null)
//        //                continue;

//        //            if (curBlock.Layer == 0)
//        //                continue;

//        //            var blockHeight = curBlock.Height + curBlock.MicroHeight;
//        //            if (curBlock.blockType == BlockType.STAIRS)
//        //            {
//        //                switch (curBlock.Rotation)
//        //                {
//        //                    case BlockRotation.Default:
//        //                        if (direction == BridgeDirection.SOUTH)
//        //                            blockHeight += 0.5f;
//        //                        //if (dir.y < 0)
//        //                        //{
//        //                        //    blockHeight += 0.5f;
//        //                        //}
//        //                        break;
//        //                    case BlockRotation.Left:
//        //                        if (direction == BridgeDirection.WEST)
//        //                            blockHeight += 0.5f;
//        //                        //if (dir.x > 0)
//        //                        //{
//        //                        //    blockHeight += 0.5f;
//        //                        //}
//        //                        break;
//        //                    case BlockRotation.Half:
//        //                        if (direction == BridgeDirection.NORTH)
//        //                            blockHeight += 0.5f;
//        //                        //if (dir.y > 0)
//        //                        //{
//        //                        //    blockHeight += 0.5f;
//        //                        //}
//        //                        break;
//        //                    case BlockRotation.Right:
//        //                        if (direction == BridgeDirection.EAST)
//        //                            blockHeight += 0.5f;
//        //                        //if (dir.x < 0)
//        //                        //{
//        //                        //    blockHeight += 0.5f;
//        //                        //}
//        //                        break;
//        //                }
//        //            }
//        //            heightDiff = Mathf.Abs(oddBlockHeight - blockHeight);
//        //            if (heightDiff > maxDist)
//        //                continue;
//        //            TargetBlock = curBlock;
//        //            lastBlockHeight = blockHeight;
//        //            break;
//        //        }
//        //        targetPilar = i;
//        //        if (TargetBlock != null)
//        //            break;
//        //    }

//        //    // Can the bridge be made
//        //    if (TargetBlock != null && targetPilar > 0)
//        //    {
//        //        //BlockRotation[] rotation = null;

//        //        //if (dir.x != 0)
//        //        //{
//        //        //    rotation = new BlockRotation[2]
//        //        //    {
//        //        //                BlockRotation.Left,
//        //        //                BlockRotation.Right
//        //        //    };
//        //        //}
//        //        //else
//        //        //{
//        //        //    rotation = new BlockRotation[2]
//        //        //    {
//        //        //                BlockRotation.Default,
//        //        //                BlockRotation.Half
//        //        //    };
//        //        //}
//        //        //Vector2 posOffset = Vector2.zero;
//        //        //if (dir.x > 0 || dir.y < 0)
//        //        //{
//        //        //    posOffset = Vector2.one * 0.5f;
//        //        //}



//        //        //Vector2 initialPos = new Vector2(foundPilars[0].transform.position.x, foundPilars[0].transform.position.z) + posOffset;
//        //        //Vector2 finalPos = new Vector2(foundPilars[targetPilar - 1].transform.position.x, foundPilars[targetPilar - 1].transform.position.z) + posOffset;

//        //        //if(dir.x > 0)
//        //        //{
//        //        //    finalPos.x += 1.02f;
//        //        //}
//        //        //else if(dir.y > 0)
//        //        //{
//        //        //    finalPos.y += 1.02f;
//        //        //}
//        //        Vector2 posOffset = Vector2.zero;
//        //        if (dir.x < 0)
//        //        {
//        //            posOffset.x += (1.0f + StructureComponent.Separation);
//        //        }
//        //        else if(dir.y < 0)
//        //        {
//        //            posOffset.y += (1.0f + StructureComponent.Separation);
//        //        }

//        //        BlockRotation rotation = BlockRotation.COUNT;
//        //        switch (direction)
//        //        {
//        //            case BridgeDirection.NORTH:
//        //                rotation = BlockRotation.Left;
//        //                break;
//        //            case BridgeDirection.SOUTH:
//        //                rotation = BlockRotation.Right;
//        //                break;
//        //            case BridgeDirection.EAST:
//        //                rotation = BlockRotation.Default;
//        //                break;
//        //            case BridgeDirection.WEST:
//        //                rotation = BlockRotation.Half;
//        //                break;
//        //        }
//        //        //float bridgeLength = Vector2.Distance(initialPos, finalPos);
//        //        float bridgeLength = targetPilar * (1.0f + StructureComponent.Separation);
//        //        Vector2 initialPos = Vector2.zero;
//        //        bool inverse = false;
//        //        if (dir.x < 0 || dir.y < 0)
//        //        {
//        //            initialPos = new Vector2(oddPilar.transform.position.x, oddPilar.transform.position.z);
//        //            inverse = true;
//        //        }
//        //        else
//        //        {
//        //            initialPos = new Vector2(foundPilars[0].transform.position.x, foundPilars[0].transform.position.z);
//        //        }

//        //        //Vector2 initialPos = new Vector2(foundPilars[0].transform.position.x, foundPilars[0].transform.position.z) + posOffset;
//        //        Vector2 finalPos = initialPos + new Vector2(dir.x, dir.y) * bridgeLength;

//        //        //float appearOffset = 0.0f;
//        //        //float startAnimOffset = targetPilar * 2.0f * WoodenBridgeAnimationComponent.AppearOffset;
//        //        float endAnimOffset = WoodenBridgeAnimationComponent.AnimationDuration * 0.5f * targetPilar * 2.0f;

//        //        for (int i = 0; i < targetPilar; ++i)
//        //        {
//        //            var curPilar = foundPilars[i];
//        //            curPilar.DestroyBridge();
//        //            curPilar.AddBridge(0, BridgeType.SMALL, false);
//        //            curPilar.Bridge.Rotation = rotation;

//        //            //var curBridgePos = new Vector2(foundPilars[i].transform.position.x, foundPilars[i].transform.position.z) + posOffset;
//        //            //var distanceFromInit = Vector2.Distance(initialPos, curBridgePos);
//        //            //float bridgeHeight = GameUtils.BridgeYPosition(bridgeLength, distanceFromInit, oddBlockHeight, lastBlockHeight);
//        //            //curPilar.Bridge.transform.Translate(new Vector3(0.0f, bridgeHeight, 0.0f), Space.World);
//        //            float[] objLH = new float[curPilar.Bridge.Objects.Length];
//        //            for (int j = 0; j < curPilar.Bridge.Objects.Length; ++j)
//        //            {
//        //                var curBridgeObj = curPilar.Bridge.Objects[j];
//        //                var curObjPos = new Vector2(curBridgeObj.transform.position.x, curBridgeObj.transform.position.z);// + posOffset;
//        //                float distanceFromInit = 0.0f;
//        //                if (inverse)
//        //                {
//        //                    if (dir.x != 0)
//        //                    {
//        //                        distanceFromInit = Mathf.Abs(curObjPos.x - finalPos.x);
//        //                    }
//        //                    else if (dir.y != 0)
//        //                    {
//        //                        distanceFromInit = Mathf.Abs(curObjPos.y - finalPos.y);
//        //                    }
//        //                    distanceFromInit = bridgeLength - distanceFromInit;
//        //                }
//        //                else
//        //                {
//        //                    if (dir.x != 0)
//        //                    {
//        //                        distanceFromInit = Mathf.Abs(curObjPos.x - initialPos.x);
//        //                    }
//        //                    else if (dir.y != 0)
//        //                    {
//        //                        distanceFromInit = Mathf.Abs(curObjPos.y - initialPos.y);
//        //                    }
//        //                }

//        //                float objHeight = GameUtils.BridgeYPosition(bridgeLength, distanceFromInit, oddBlockHeight, lastBlockHeight, out float lh);
//        //                objLH[j] = lh;
//        //                curBridgeObj.transform.Translate(new Vector3(0.0f, objHeight, 0.0f), Space.Self);
//        //            }
//        //            curPilar.Bridge.UpdateCollision();

//        //            float startingDistance = i * (1.0f + StructureComponent.Separation);
//        //            float endingDistance = (i + 1) * (1.0f + StructureComponent.Separation);
//        //            float startingHeight = GameUtils.BridgeYPosition(bridgeLength, startingDistance, oddBlockHeight, lastBlockHeight, out float sLh);
//        //            float endingHeight = GameUtils.BridgeYPosition(bridgeLength, endingDistance, oddBlockHeight, lastBlockHeight, out float eLh);
//        //            curPilar.Bridge.SetBridgeHeightInfo(startingHeight, endingHeight, direction);
//        //            float appearOffset = i * WoodenBridgeAnimationComponent.AppearOffset * curPilar.Bridge.Objects.Length;
//        //            curPilar.Bridge.StartAnimation(/*new Vector2(oddBlock.transform.position.x, oddBlock.transform.position.z),*/ appearOffset, endAnimOffset, objLH);
//        //            //appearOffset += curPilar.Bridge.Objects.Length * WoodenBridgeAnimationComponent.AppearOffset;
//        //            //startAnimOffset += curPilar.Bridge.Objects.Length * WoodenBridgeAnimationComponent.AnimationDuration;
//        //        }
//        //    }
//        //}

//        void ChangePivot()
//        {
//            if (SelectedBlockIdx >= 0 && SelectedPilarMapID >= 0)
//                Manager.Mgr.CamMgr.RotatingTarget = Manager.Mgr.Pilars[SelectedPilarMapID].Blocks[SelectedBlockIdx].gameObject;
//            else
//                Manager.Mgr.CamMgr.RotatingTarget = null;
//        }

//        //RaycastHit[] SortRayCast(RaycastHit[] hits)
//        //{
//        //    List<RaycastHit> sortedHits = new List<RaycastHit>(hits.Length);

//        //    while(hits.Length > 0)
//        //    {
//        //        float minDistance = float.MaxValue;
//        //        int hitIdx = -1;
//        //        for (int i = 0; i < hits.Length; ++i)
//        //        {
//        //            if(hits[i].distance < minDistance)
//        //            {
//        //                minDistance = hits[i].distance;
//        //                hitIdx = i;
//        //            }
//        //        }
//        //        sortedHits.Add(hits[hitIdx]);
//        //        var tmp = new RaycastHit[hits.Length - 1];
//        //        for(int i = 0; i < hits.Length; ++i)
//        //        {
//        //            if (i == hitIdx)
//        //                continue;
//        //            int idx = i < hitIdx ? i : (i - 1);
//        //            tmp[idx] = hits[i];
//        //        }
//        //        hits = tmp;
//        //    }

//        //    for(int i = 0; i < sortedHits.Count; ++i)
//        //    {
//        //        if(sortedHits[i].collider.tag == DeathPlaneComponent.TAG)
//        //        {
//        //            sortedHits.RemoveAt(i);
//        //        }
//        //    }

//        //    return sortedHits.ToArray();
//        //}

//        //void SortRayCast()
//        //{
//        //    for (int i = 0; i < m_RayAmount; ++i)
//        //    {
//        //        float minDist = float.MaxValue;
//        //        int minIdx = int.MaxValue;
//        //        for (int j = 0; j < m_RayAmount; ++j)
//        //        {
//        //            if (m_TempCasts[j].distance < minDist)
//        //            {
//        //                minIdx = j;
//        //                minDist = m_TempCasts[j].distance;
//        //            }
//        //        }
//        //        m_RayCasts[i] = m_TempCasts[minIdx];
//        //        m_TempCasts[minIdx].distance = float.MaxValue;
//        //    }

//        //    //for (int i = 0; i < m_RayAmount; ++i)
//        //    //    m_RayCasts[i] = m_TempCasts[i];
//        //}

//        //void OnCastDonePlay(RaycastHit mouseHit, bool mouseLeftClick, bool mouseRightClick)
//        //{
//        //    Vector2 target = new Vector2(mouseHit.point.x, mouseHit.point.z);

//        //    if (mouseLeftClick)
//        //    {
//        //        oddScript.Attack(target);
//        //    }
//        //    else if (mouseRightClick)
//        //    {
//        //        oddScript.SecondaryAction(mouseHit.point);
//        //    }
//        //}

//        //void OnCastDone(int rayAmount, bool mouseLeftClick, bool mouseRightClick, bool mouseLeftHold, bool mouseRightHold)
//        //{
//        //    bool hideInfo = Manager.Mgr.HideInfo;
//        //    bool blockTouched = false;
//        //    bool findingTouch = false;
//        //    for (int i = 0; i < rayAmount; ++i)
//        //    {
//        //        var mouseHit = m_RayCasts[i];
//        //        var goOver = mouseHit.transform.gameObject;
//        //        Vector2 target = new Vector2(mouseHit.point.x, mouseHit.point.z);

//        //        if(mouseLeftClick)
//        //        {
//        //            oddScript.Attack(target);
//        //        }
//        //        else if(mouseRightClick)
//        //        {
//        //            oddScript.SecondaryAction(mouseHit.point);
//        //        }
//        //        //if (hideInfo && mouseRightClick && goOver.tag == LivingEntity.TAG && oddScript.CurrentInvItem == InvItemType.WEAPON)
//        //        //{
//        //        //    var trfm = goOver.transform.parent;
//        //        //    //var oddPosXZ = new Vector2(oddScript.transform.position.x, oddScript.transform.position.z);
//        //        //    //var enemyPosXZ = new Vector2(trfm.position.x, trfm.position.z);
//        //        //    //var enemyDistance = Vector2.Distance(oddPosXZ, enemyPosXZ);
//        //        //    //var inRangeDistance = oddScript.Weapon.AttackRange - oddScript.Weapon.AttackRange * 0.2f;
//        //        //    //if (enemyDistance <= oddScript.Weapon.AttackRange)
//        //        //    //{

//        //        //    //}
//        //        //    //else
//        //        //    //{
//        //        //    //    var dir = (enemyPosXZ - oddPosXZ).normalized;
//        //        //    //    var amount = enemyDistance - inRangeDistance;
//        //        //    //    oddScript.MoveTo(oddPosXZ + dir * amount);
//        //        //    //}
//        //        //    oddScript.SecondaryAction(new Vector3(trfm.position.x, 0.0f, trfm.position.z));
//        //        //    break; //return;
//        //        //}

//        //        bool isEmptyRay = false;
//        //        Vector2 Target = new Vector2(mouseHit.point.x, mouseHit.point.z);

//        //        if (goOver.tag == BlockComponent.BlockTag)
//        //        {
//        //            blockTouched = true;
//        //            var blockGO = goOver;//.transform.parent.gameObject;
//        //            var block = blockGO.GetComponent<BlockComponent>();
//        //            if (block.blockType == BlockType.WIDE)
//        //            {
//        //                var mapID = GameUtils.MapIDFromPosition(new Vector2(mouseHit.point.x, mouseHit.point.z));
//        //                for (int j = 0; j < block.HiddenBlocks.Length; ++j)
//        //                {
//        //                    if (block.HiddenBlocks[j].Pilar.MapID == mapID)
//        //                    {
//        //                        block = block.HiddenBlocks[j];
//        //                        break;
//        //                    }
//        //                }
//        //            }
//        //            var currentBlockIdx = block.Pilar.Blocks.IndexOf(block);
//        //            var currentPilarMapID = block.Pilar.MapID;

//        //            if (!hideInfo)
//        //            {
//        //                if (BlockOver != null && BlockOver != block)
//        //                {
//        //                    BlockOver.Highlighted = false;
//        //                }
//        //                BlockOver = block;
//        //                if (mouseLeftClick || mouseLeftHold)
//        //                {
//        //                    if (SelectedPilarMapID >= 0 && SelectedBlockIdx >= 0)
//        //                    {
//        //                        Manager.Mgr.Pilars[SelectedPilarMapID].Blocks[SelectedBlockIdx].Selected = false;
//        //                    }
//        //                    BlockOver.Selected = true;
//        //                    SelectedBlockIdx = currentBlockIdx;
//        //                    SelectedPilarMapID = currentPilarMapID;
//        //                }
//        //                else if (mouseRightClick || mouseRightHold)
//        //                {
//        //                    if (SelectedPilarMapID != BlockOver.Pilar.MapID && SelectedPilarMapID >= 0 && SelectedBlockIdx >= 0)
//        //                    {
//        //                        Manager.Mgr.Pilars[SelectedPilarMapID].Blocks[SelectedBlockIdx].Selected = false;
//        //                    }
//        //                    BlockOver.Selected = false;
//        //                    SelectedPilarMapID = -1;
//        //                    SelectedBlockIdx = -1;
//        //                }
//        //            }
//        //            else
//        //            {
//        //                if (mouseLeftClick || mouseLeftHold)
//        //                {
//        //                    SelectedBlockIdx = currentBlockIdx;
//        //                    SelectedPilarMapID = currentPilarMapID;
//        //                }
//        //            }
//        //        }
//        //        else if (goOver.tag == BridgeComponent.BridgeTag)
//        //        {
//        //            /* No-op*/
//        //        }
//        //        else if (goOver.tag == LivingEntity.TAG)
//        //        {
//        //            var trfm = goOver.transform.parent;
//        //            Target.Set(trfm.position.x, trfm.position.z);
//        //        }
//        //        else if (goOver.tag == "RAYPLANE")
//        //        {
//        //            if ((rayAmount - i) == 1)
//        //            {
//        //                isEmptyRay = true;
//        //            }
//        //            else
//        //            {
//        //                continue;
//        //            }
//        //        }
//        //        else
//        //        {
//        //            continue;
//        //        }

//        //        if (findingTouch)
//        //            continue;

//        //        if (mouseLeftClick || mouseLeftHold)
//        //        {
//        //            if (isEmptyRay)
//        //                ++oddScript.JumpToVoid;
//        //            else
//        //                oddScript.JumpToVoid = 0;
//        //            oddScript.MoveTo(Target);
//        //        }
//        //        else if (mouseRightClick && hideInfo)
//        //        {
//        //            oddScript.SecondaryAction(new Vector3(Target.x, mouseHit.point.y, Target.y));
//        //        }

//        //        //    if (hideInfo)
//        //        //{
//        //        //    SelectedBlockIdx = block.Pilar.Blocks.IndexOf(block);
//        //        //    SelectedPilarMapID = block.Pilar.MapID;
//        //        //    if (mouseLeftClick || mouseLeftHold)
//        //        //    {
//        //        //        oddScript.MoveTo(new Vector2(mouseHit.point.x, mouseHit.point.z));
//        //        //    }
//        //        //    if (mouseRightClick)
//        //        //    {
//        //        //        oddScript.SecondaryAction(new Vector2(mouseHit.point.x, mouseHit.point.z));
//        //        //    }
//        //        //}
//        //        //else
//        //        //{
//        //        //    if (BlockOver != null && BlockOver != block)
//        //        //    {
//        //        //        BlockOver.Highlighted = false;
//        //        //    }
//        //        //    BlockOver = block;

//        //        //    if (mouseLeftClick || mouseLeftHold)
//        //        //    {
//        //        //        if (SelectedPilarMapID >= 0 && SelectedBlockIdx >= 0)
//        //        //        {
//        //        //            Manager.Mgr.Pilars[SelectedPilarMapID].Blocks[SelectedBlockIdx].Selected = false;
//        //        //        }
//        //        //        BlockOver.Selected = true;
//        //        //        oddScript.MoveTo(new Vector2(mouseHit.point.x, mouseHit.point.z));
//        //        //        SelectedPilarMapID = BlockOver.Pilar.MapID;
//        //        //        SelectedBlockIdx = BlockOver.Pilar.Blocks.IndexOf(BlockOver);
//        //        //    }
//        //        //    else if (mouseRightClick || mouseRightHold)
//        //        //    {
//        //        //        if (SelectedPilarMapID != BlockOver.Pilar.MapID && SelectedPilarMapID >= 0 && SelectedBlockIdx >= 0)
//        //        //        {
//        //        //            Manager.Mgr.Pilars[SelectedPilarMapID].Blocks[SelectedBlockIdx].Selected = false;
//        //        //        }
//        //        //        BlockOver.Selected = false;
//        //        //        SelectedPilarMapID = -1;
//        //        //        SelectedBlockIdx = -1;
//        //        //    }
//        //        //}
//        //        if (!hideInfo && !blockTouched)
//        //        {
//        //            findingTouch = true;
//        //            continue;
//        //        }
//        //        break;
//        //    }
//        //}

//        //int OnFullRayCast(bool mouseLeftClick, bool mouseRightClick, bool mouseLeftHold, bool mouseRightHold)
//        //{
//        //    bool mouseClicked = mouseLeftClick || mouseRightClick;
//        //    bool mouseHold = mouseLeftHold || mouseRightHold;

//        //    var hideInfo = Manager.Mgr.HideInfo;
//        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//        //    //bool hit = Physics.Raycast(ray, out RaycastHit mouseHit, 1000.0f);
//        //    int rayAmount = 0;
//        //    //if (!hideInfo)
//        //        //rayAmount = Physics.RaycastNonAlloc(ray, m_RayCasts, 1000f);
//        //    //else
//        //        rayAmount = Physics.RaycastNonAlloc(ray, m_TempCasts, 1000f);
//        //    //var hits = Physics.RaycastAll(ray, 1000.0f);
//        //    //if (hits.Length == 0)
//        //    //if(!hit)
//        //    if(rayAmount == 0)
//        //    {
//        //        if (hideInfo)
//        //        {
//        //            SelectedBlockIdx = -1;
//        //            SelectedPilarMapID = -1;
//        //        }
//        //    }
//        //    else
//        //    {
//        //        GameUtils.SortRayCast(ref m_TempCasts, ref m_RayCasts, rayAmount);
//        //    }
//        //    //if (hideInfo)
//        //         //SortRayCast();
//        //    //SortRayCast();
//        //    //hits = SortRayCast(hits);
//        //    return rayAmount;
//        //}

//        void OnBridgeSelect(RaycastHit mouseHit)
//        {
//            if(SelectedBlockIdx >= 0 && SelectedPilarMapID >= 0)
//            {
//                Manager.Mgr.Pilars[SelectedPilarMapID].Blocks[SelectedBlockIdx].Selected = false;
//            }
//            SelectedBlockIdx = -1;
//            SelectedPilarMapID = -1;

//            m_SelectedBridge = mouseHit.transform.gameObject.GetComponent<BridgeComponent>();
//            m_SelectedBridge.BridgeOutline = true;
//        }

//        void OnBridgeDestroy()
//        {
//            m_SelectedBridge.Destroy();
//            Manager.Mgr.Map.Record(new MapCommand(
//                MapCommandType.BRIDGE_DEATH, "", m_SelectedBridge.Pilar.MapID, -1));
//            m_SelectedBridge = null;
//        }

//        void OnCastDoneEdit(bool hitFound, RaycastHit mouseHit, bool mouseLeftClick, bool mouseRightClick)
//        {
//            if(!hitFound)
//            {
//                SelectedBlockIdx = -1;
//                SelectedPilarMapID = -1;
//                return;
//            }
//            var blockGO = mouseHit.transform.gameObject;//.transform.parent.gameObject;
//            var block = blockGO.GetComponent<BlockComponent>();
//            if (block.blockType == Def.BlockType.WIDE)
//            {
//                var mapID = GameUtils.MapIDFromPosition(new Vector2(mouseHit.point.x, mouseHit.point.z));
//                for (int j = 0; j < block.HiddenBlocks.Length; ++j)
//                {
//                    if (block.HiddenBlocks[j].Pilar.MapID == mapID)
//                    {
//                        block = block.HiddenBlocks[j];
//                        break;
//                    }
//                }
//            }
//            if (m_SelectedBridge != null)
//            {
//                m_SelectedBridge.BridgeOutline = false;
//                m_SelectedBridge = null;
//            }

//            var currentBlockIdx = block.Pilar.Blocks.IndexOf(block);
//            var currentPilarMapID = block.Pilar.MapID;

//            if (BlockOver != null && BlockOver != block)
//            {
//                BlockOver.Highlighted = false;
//            }
//            BlockOver = block;
//            if (mouseLeftClick)
//            {
//                if (SelectedPilarMapID >= 0 && SelectedBlockIdx >= 0)
//                {
//                    Manager.Mgr.Pilars[SelectedPilarMapID].Blocks[SelectedBlockIdx].Selected = false;
//                }
//                BlockOver.Selected = true;
//                SelectedBlockIdx = currentBlockIdx;
//                SelectedPilarMapID = currentPilarMapID;
//            }
//            else if (mouseRightClick)
//            {
//                if (SelectedPilarMapID != BlockOver.Pilar.MapID && SelectedPilarMapID >= 0 && SelectedBlockIdx >= 0)
//                {
//                    Manager.Mgr.Pilars[SelectedPilarMapID].Blocks[SelectedBlockIdx].Selected = false;
//                }
//                BlockOver.Selected = false;
//                SelectedPilarMapID = -1;
//                SelectedBlockIdx = -1;
//            }
//        }

//        public void Update()
//        {
//            if (State != PC_State.Default || GUIUtility.hotControl != 0)
//            {
//                return;
//            }
//            //if (m_NextFogTime < Time.time)
//            {
//                if (Input.GetKey(KeyCode.KeypadPlus))
//                {
//                    OnFogDistanceChange(true);
//                    //m_NextFogTime = Time.time + 0.05f;
//                    return;
//                }
//                if (Input.GetKey(KeyCode.KeypadMinus))
//                {
//                    OnFogDistanceChange(false);
//                    //m_NextFogTime = Time.time + 0.05f;
//                    return;
//                }
//            }

//            if (LastModificationTime < Time.time)
//            {
//                if (Input.GetKey(KeyCode.H))
//                {
//                    ToggleHideInfo();

//                    LastModificationTime = Time.time + ModificationWait;
//                }
//                if (Manager.Mgr.HideInfo)
//                {
//                    if (Input.GetKey(KeyCode.P))
//                    {
//                        ChangePivot();
//                        LastModificationTime = Time.time + ModificationWait;
//                    }
//                    else if (Input.GetKey(KeyCode.F))
//                    {
//                        if (SelectedBlockIdx >= 0 && SelectedPilarMapID >= 0)
//                        {
//                            RepeatAntSelection();
//                            LastModificationTime = Time.time + ModificationWait;
//                        }
//                    }
//                    else if (Input.GetKey(KeyCode.V))
//                    {
//                        if (oddScript.GetCurrentHealth() <= 0f)
//                            oddScript._Health = oddScript.GetTotalHealth();
//                        else
//                            oddScript._Health = 0f;
//                        LastModificationTime = Time.time + ModificationWait;
//                    }
//                }
//                else
//                {
//                    if(m_SelectedBridge != null && Input.GetKey(KeyCode.Delete))
//                    {
//                        OnBridgeDestroy();
//                        LastModificationTime = Time.time + ModificationWait;
//                    }
//                }
//                //else if (Input.GetKey(KeyCode.B))
//                //{
//                //    //CreateBridge();
//                //    LastModificationTime = Time.time + ModificationWait;
//                //}
//                //if (Manager.Mgr.HideInfo)
//                //{
//                //    if(Input.GetKey(KeyCode.P))
//                //    {
//                //        ChangePivot();
//                //        LastModificationTime = Time.time + ModificationWait;
//                //    }
//                //}
                
//                //if (SelectedBlockIdx >= 0 && SelectedPilarMapID >= 0)
//                //{
//                //    if (Input.GetKey(KeyCode.X))
//                //    {
//                //        var pilar = Manager.Mgr.Pilars[SelectedPilarMapID];
//                //        var block = pilar.Blocks[SelectedBlockIdx];
//                //        var go = GameObject.Instantiate(AssetContainer.Mgr.TriVFXGameObjects[0]);
//                //        go.SetActive(true);
//                //        go.transform.Translate(new Vector3(pilar.transform.position.x + 0.5f, block.Height + block.MicroHeight, pilar.transform.position.z + 0.5f), Space.World);
//                //        var pos = go.GetComponent<PossessionVFXComponent>();
//                //        pos.Set();
//                //        LastModificationTime = Time.time + ModificationWait;
//                //    }
//                //}
//            }

//            bool mouseLeftClick = Input.GetMouseButtonDown(0);
//            bool mouseRightClick = Input.GetMouseButtonDown(1);
//            //bool mouseLeftHold = Input.GetMouseButton(0);
//            //bool mouseRightHold = Input.GetMouseButton(1);
//            bool mouseActive = mouseLeftClick || mouseRightClick; //|| mouseLeftHold || mouseRightHold;

//            if(mouseActive && LastRayTime < Time.time)
//            {
//                //var rayAmount = OnFullRayCast(mouseLeftClick, mouseRightClick, mouseLeftHold, mouseRightHold);
//                //OnCastDone(rayAmount, mouseLeftClick, mouseRightClick, mouseLeftHold, mouseRightHold);

//                if(Manager.Mgr.HideInfo)
//                {
//                    if(mouseLeftClick)
//                    {
//                        if (OnSingleRayCast(out RaycastHit mouseHit, false, true, false, false))
//                        {
//                            oddScript.Attack(mouseHit.transform.parent.position);
//                        }
//                        else if (OnSingleRayCast(out mouseHit, true, false, false, false))
//                        {
//                            oddScript.Attack(mouseHit.point);
//                        }
//                        else if(OnSingleRayCast(out mouseHit, false, false, false, true))
//                        {
//                            oddScript.Attack(mouseHit.point);
//                        }
//                    }
//                    else if(OnSingleRayCast(out RaycastHit mouseHit, true, false, false, false))
//                    {
//                        oddScript.SecondaryAction(mouseHit.point);
//                    }
//                }
//                else
//                {
//                    if(mouseLeftClick && OnSingleRayCast(out RaycastHit mouseHit, false, false, true, false))
//                    {
//                        OnBridgeSelect(mouseHit);
//                    }
//                    else
//                    {
//                        OnCastDoneEdit(
//                            OnSingleRayCast(out mouseHit, true, false, false, false),
//                            mouseHit, mouseLeftClick, mouseRightClick);
//                    }
//                }

//                LastRayTime = Time.time + RayWait;
//            }
//        }
//    }
//}
