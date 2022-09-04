/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using Assets;
//using System.IO;
//using System.Reflection;
//using UnityEngine.UI;
//using System;
//using System.Diagnostics;
////using System.Windows.Forms;
//using System.Linq;

//namespace Assets
//{
//    public enum GameState
//    {
//        //PLAYING,
//        //EDITING_STRUCTURE,
//        //EDITING_MAP,

//        EDITOR,
//        PLAY,

//        BIOME_EDITOR,
//        STRUC_EDITOR,
//        BUILD_EDITOR,

//        COUNT
//    }

//    public struct InitFunction
//    {
//        public Action Function;
//        public string Name;
//    }

//    public class Manager : MonoBehaviour
//    {
//		public static bool IsPaused = false;

//        public const int BlockLayer = 8;
//        public const int LivingEntityLayer = 9;
//        public const int BridgeLayer = 10;
//        public const int RayPlaneLayer = 11;

//        public const int BridgeMaxLength = 16;
//        //List<IGameController> Controllers = new List<IGameController>((int)GameState.COUNT);
//        //IGameController Controller = null;
//        readonly string[] ControllerSelectionStr = new string[(int)GameState.COUNT]
//        {
//            "Editor",
//            "Play",
//            "BiomeEditor",
//            "StrucEditor",
//            "BuildEditor"
//        };
//        public static Manager Mgr;
//        public readonly static string ExecutablePath = System.AppDomain.CurrentDomain.BaseDirectory;
//        public int CurrentControllerSel = -1;
//        public const int MapWidth = 128;
//        public const int MapHeight = 128;
//        //public List<CPilar> Pilars = new List<CPilar>(MapWidth * MapHeight);
//        //public List<StructureComponent> Strucs = new List<StructureComponent>((MapWidth * MapHeight) / (StructureComponent.Width * StructureComponent.Height));
//        CPilar[] m_BridgePilars = new CPilar[BridgeMaxLength];
//        //BlockComponent m_BridgeBuildBlock;
//        //SpaceDirection m_BridgeDirection;
//        IBlock m_BridgeBuildBlock;
//        Def.SpaceDirection m_BridgeDirection;
//        int m_BridgeLength;
//        float m_BridgeInitialHeight;
//        float m_BridgeFinalHeight;

//        public StrucQT MapQT;

//        //public static List<StructureComponent> GetStrucs(Vector2 pos)
//        //{
//        //    List<StructureComponent> strucs = new List<StructureComponent>();
//        //    for(int i = 0; i < Mgr.Strucs.Count; ++i)
//        //    {
//        //        var struc = Mgr.Strucs[i];
//        //        if (struc.StructRect.Contains(pos))
//        //            strucs.Add(struc);
//        //    }

//        //    return strucs;
//        //}

//        //public static void GetStrucsNoAlloc(ref StructureComponent[] strucs, Vector2 pos)
//        //{
//        //    int lastStruct = 0;
//        //    for (int i = 0; i < Mgr.Strucs.Count; ++i)
//        //    {
//        //        var struc = Mgr.Strucs[i];
//        //        if (struc.StructRect.Contains(pos))
//        //            strucs[lastStruct++] = struc;
//        //    }
//        //    if (lastStruct < (strucs.Length))
//        //        strucs[lastStruct] = null;
//        //}

//        public Canvas m_Canvas;
//        public Camera m_Camera;
//        public CameraManager CamMgr;
//        public GameObject OddGO;
//        //public OddScript OddScript;
//        public GameState StartingState;
//        public System.Random BuildRNG = new System.Random(System.DateTime.Now.Millisecond);
//        public System.Random DamageRNG = new System.Random(System.DateTime.Now.Millisecond);
//        public System.Random SpawnRNG = new System.Random(System.DateTime.Now.Millisecond);
//        public bool HideInfo;
//        //public MapIE Map;

//        public GameObject RayPlane;

//        //public StructureComponent Structure = null;
//        float[] AvgFPS;
//        float fps;

//        public Sprite LoadingSprite;

//        public List<MeshFilter> importedMeshes = null;

//        public bool IsLoadFinished;
//        List<InitFunction> LoadingFunctions;
//        string CurrentlyLoadingFNName;
//        int CurrentlyLoadingFN;

//        bool m_ExceptionFound;
//        string m_ExceptionMessage;
//        string m_ExceptionStackTrace;

//        //public IGameController GetController(GameState state)
//        //{
//        //    return Controllers[(int)state];
//        //}

//        //public void StartSimpleMap()
//        //{
//        //    //const int startingX = MapWidth / 2 - StructureComponent.Width / 2;
//        //    //const int startingY = MapHeight / 2 - StructureComponent.Height / 2;
//        //    //for (int y = 0; y < MapHeight; ++y)
//        //    //{
//        //    //    var yOffset = y * MapHeight;
//        //    //    for (int x = 0; x < MapWidth; ++x)
//        //    //    {
//        //    //        var mapID = GameUtils.IDFromPos(new Vector2Int(x, y), MapWidth, MapHeight);
//        //    //        var pilar = new GameObject("InvalidPilar").AddComponent<PilarComponent>();
//        //    //        pilar.Init(null, -1, mapID);
//        //    //        pilar.gameObject.isStatic = true;
//        //    //        Pilars.Add(pilar);
//        //    //    }
//        //    //}
//        //    //for (int y = 0; y < StructureComponent.Height; ++y)
//        //    //{
//        //    //    for (int x = 0; x < StructureComponent.Width; ++x)
//        //    //    {
//        //    //        var idx = GameUtils.IDFromPos(new Vector2Int(startingX + x, startingY + y), MapWidth, MapHeight);
//        //    //        var pilar = Pilars[idx];
//        //    //        var block = pilar.AddBlock();

//        //    //        block.gameObject.isStatic = true;
//        //    //        block.TopMR.gameObject.isStatic = true;
//        //    //        block.LayerSR.enabled = false;
//        //    //        block.LayerSR.gameObject.isStatic = true;
//        //    //        block.LayerSR.gameObject.SetActive(false);
//        //    //        block.StairSR.enabled = false;
//        //    //        block.StairSR.gameObject.isStatic = true;
//        //    //        block.StairSR.gameObject.SetActive(false);
//        //    //        block.LockSR.enabled = false;
//        //    //        block.LockSR.gameObject.isStatic = true;
//        //    //        block.LockSR.gameObject.SetActive(false);
//        //    //        block.AnchorSR.enabled = false;
//        //    //        block.AnchorSR.gameObject.isStatic = true;
//        //    //        block.AnchorSR.gameObject.SetActive(false);

//        //    //        block.MidMR.enabled = false;
//        //    //        //block.MidBC.enabled = false;
//        //    //        block.MidMR.gameObject.isStatic = true;
//        //    //        block.MidMR.gameObject.SetActive(false);
//        //    //    }
//        //    //}
//        //}

//        //public void PlaceStruct(StructureComponent struc)
//        //{
//        //    //const int StrucWidth = StructureComponent.Width;
//        //    //const int StrucHeight = StructureComponent.Height;
//        //    //var mapPos = GameUtils.PosFromID(
//        //    //    GameUtils.MapIDFromPosition(new Vector2(struc.transform.position.x + 0.1f, struc.transform.position.z + 0.1f)),
//        //    //    MapWidth, MapHeight);

//        //    //for (int y = 0; y < StrucHeight; ++y)
//        //    //{
//        //    //    for (int x = 0; x < StrucWidth; ++x)
//        //    //    {
//        //    //        var pilarID = GameUtils.IDFromPos(new Vector2Int(x, y));
//        //    //        var pilar = struc.Pilars[pilarID];
//        //    //        bool hasStrucPilarValidBlocks = false;
//        //    //        // Has the pilar valid blocks? If not destroy the pilar and use the old pilar, otherwise destroy the old one
//        //    //        foreach (var sBlock in pilar.Blocks)
//        //    //        {
//        //    //            if (sBlock != null && sBlock.Layer > 0)
//        //    //            {
//        //    //                hasStrucPilarValidBlocks = true;
//        //    //                break;
//        //    //            }
//        //    //        }
//        //    //        if (!hasStrucPilarValidBlocks)
//        //    //        {
//        //    //            pilar.DestroyPilar();
//        //    //        }
//        //    //        else
//        //    //        {
//        //    //            bool hasMapRemovedBlocks = false;
//        //    //            bool hasMapWideBlocks = false;
//        //    //            var mapID = GameUtils.IDFromPos(new Vector2Int(mapPos.x + x, mapPos.y + y), MapWidth, MapHeight);
//        //    //            var mapPilar = Pilars[mapID];
//        //    //            // Find which blocks are in this pilar and if they are removed or WIDE
//        //    //            foreach (var pBlock in mapPilar.Blocks)
//        //    //            {
//        //    //                if (pBlock == null)
//        //    //                    continue;
//        //    //                if (pBlock.Removed)
//        //    //                {
//        //    //                    hasMapRemovedBlocks = true;
//        //    //                }
//        //    //                if (pBlock.blockType == BlockType.WIDE)
//        //    //                {
//        //    //                    hasMapWideBlocks = true;
//        //    //                }
//        //    //            }
//        //    //            // The blocks in this pilar must be WIDE or normal, not hidden
//        //    //            if (!hasMapRemovedBlocks)
//        //    //            {
//        //    //                pilar.MapID = mapID;
//        //    //                if (hasMapWideBlocks)
//        //    //                {
//        //    //                    var pilarPos = GameUtils.PosFromID(pilar.MapID, MapWidth, MapHeight);
//        //    //                    int[] blocks = new int[3]
//        //    //                    {
//        //    //                        GameUtils.IDFromPos(new Vector2Int(pilarPos.x + 1, pilarPos.y), MapWidth, MapHeight),       // right
//        //    //                        GameUtils.IDFromPos(new Vector2Int(pilarPos.x + 1, pilarPos.y + 1), MapWidth, MapHeight),   // bottomRight
//        //    //                        GameUtils.IDFromPos(new Vector2Int(pilarPos.x, pilarPos.y + 1), MapWidth, MapHeight)        // bottom
//        //    //                    };
//        //    //                    foreach (var blockIdx in blocks)
//        //    //                    {
//        //    //                        var wPilar = Pilars[blockIdx];
//        //    //                        foreach (var wBlock in wPilar.Blocks)
//        //    //                        {
//        //    //                            if (!wBlock.Removed)
//        //    //                                continue;
//        //    //                            wBlock.TopMR.enabled = true;
//        //    //                            //wBlock.TopBC.enabled = true;
//        //    //                            wBlock.MidMR.enabled = true;
//        //    //                            //wBlock.MidBC.enabled = true;
//        //    //                            wBlock.BlockBC.enabled = true;

//        //    //                            wBlock.Removed = false;
//        //    //                        }
//        //    //                    }
//        //    //                }
//        //    //                mapPilar.DestroyPilar();
//        //    //                Pilars[pilar.MapID] = pilar;
//        //    //            }
//        //    //        }
//        //    //    }
//        //    //}
//        //    //Strucs.Add(struc);

//        //    //foreach (var pilar in struc.Pilars)
//        //    //{
//        //    //    if (pilar == null)
//        //    //        continue;

//        //    //    pilar.gameObject.isStatic = true;

//        //    //    foreach (var block in pilar.Blocks)
//        //    //    {
//        //    //        block.gameObject.isStatic = true;
//        //    //        block.TopMR.gameObject.isStatic = false;
//        //    //        block.MidMR.gameObject.isStatic = false;
//        //    //        block.AnchorSR.enabled = false;
//        //    //        block.AnchorSR.gameObject.SetActive(false);
//        //    //        block.LayerSR.enabled = false;
//        //    //        block.LayerSR.gameObject.SetActive(false);
//        //    //        block.LockSR.enabled = false;
//        //    //        block.LockSR.gameObject.SetActive(false);
//        //    //        block.StairSR.enabled = false;
//        //    //        block.StairSR.gameObject.SetActive(false);
//        //    //        if (block.Monster != null)
//        //    //        {
//        //    //            //var rng = Manager.Mgr.BuildRNG;
//        //    //            //var offset = new Vector2((float)rng.NextDouble(), (float)rng.NextDouble());
//        //    //            var offset = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
//        //    //            block.Monster.transform.position = new Vector3(pilar.transform.position.x + offset.x, block.Height + block.MicroHeight, pilar.transform.position.z + offset.y);
//        //    //            //var pos = new Vector3(pilar.transform.position.x + offset.x, block.Height + block.MicroHeight, pilar.transform.position.z + offset.y);
//        //    //            //block.Monster.transform.Translate(pos - block.Monster.transform.position, Space.World);
//        //    //            block.Monster.SpriteSR.enabled = true;
//        //    //            //block.Monster.SpriteBC.enabled = true;
//        //    //            block.Monster.SpriteCC.enabled = true;
//        //    //            block.Monster.ShadowSR.enabled = true;
//        //    //            //block.Monster.Struc = struc;
//        //    //            //struc.LivingEntities.Add(block.Monster);
//        //    //            block.Monster = null;
//        //    //        }
//        //    //        if (block.Prop != null)
//        //    //        {
//        //    //            if (block.blockType == BlockType.STAIRS)
//        //    //            {
//        //    //                if (struc.LivingEntities.Contains(block.Prop))
//        //    //                    struc.LivingEntities.Remove(block.Prop);
//        //    //                GameUtils.DeleteGameObjectAndItsChilds(block.Prop.gameObject);
//        //    //                block.Prop = null;
//        //    //            }
//        //    //            else
//        //    //            {
//        //    //                block.Prop.Block = block;
//        //    //                block.Prop.SpriteSR.enabled = true;
//        //    //                block.Prop.SpriteBC.enabled = true;
//        //    //                if (block.Prop.ShadowSR != null)
//        //    //                    block.Prop.ShadowSR.enabled = true;
//        //    //                if (block.Prop.PropLight != null)
//        //    //                    block.Prop.PropLight.enabled = true;
//        //    //            }
//        //    //        }
//        //    //    }
//        //    //}
//        //}

//        //public StructureComponent PlaceStruct(int structurePilarID, int strucID,
//        //    StructureFlip flipped = StructureFlip.NoFlip, 
//        //    Definitions.RotationState rotated = Definitions.RotationState.Default, 
//        //    float heightOffset = 0f)
//        //{
//        //    //const int StrucWidth = StructureComponent.Width;
//        //    //const int StrucHeight = StructureComponent.Height;
//        //    //const float StrucSeparation = StructureComponent.Separation;
//        //    //var mapPos = GameUtils.PosFromID(structurePilarID, MapWidth, MapHeight);
//        //    //var struc = new GameObject($"Struc_({mapPos.x},{mapPos.y})").AddComponent<StructureComponent>();
//        //    //var strucPos = new Vector3(mapPos.x * (1.0f + StrucSeparation), 0.0f, mapPos.y * (1.0f + StrucSeparation));
//        //    //struc.StructRect = new Rect(strucPos.x, strucPos.z, StrucWidth * (1.0f + StrucSeparation), StrucHeight * (1.0f + StrucSeparation));
//        //    //struc.transform.SetPositionAndRotation(strucPos, Quaternion.identity);
//        //    //struc.gameObject.isStatic = true;
//        //    //Structures.Strucs[strucID].ToStructure(ref struc, false);
//        //    //struc.ApplyStairs(true);
//        //    //struc.ApplyWides(true);
//        //    //struc.Flip = flipped;
//        //    //struc.Rotation = rotated;

//        //    //foreach (var pilar in struc.Pilars)
//        //    //{
//        //    //    foreach (var block in pilar.Blocks)
//        //    //    {
//        //    //        if (block.Removed)
//        //    //            continue;
//        //    //        block.Height += heightOffset;
//        //    //    }
//        //    //}
//        //    //PlaceStruct(struc);

//        //    //return struc;
//        //    return null;
//        //}

//        //public void AddEmptyBlocks(int structurePilarID)
//        //{
//        //    //const int StrucWidth = StructureComponent.Width;
//        //    //const int StrucHeight = StructureComponent.Height;
//        //    ////const float StrucSeparation = StructureComponent.Separation;

//        //    //var structurePlacePosition = GameUtils.PosFromID(structurePilarID, MapWidth, MapHeight);

//        //    //var emptyPos = structurePlacePosition - new Vector2Int(StrucWidth, StrucHeight);

//        //    //for (int y = 0; y < (StrucHeight * 3); ++y)
//        //    //{
//        //    //    for (int x = 0; x < (StrucWidth * 3); ++x)
//        //    //    {
//        //    //        var mapID = GameUtils.IDFromPos(emptyPos + new Vector2Int(x, y), MapWidth, MapHeight);
//        //    //        var pilar = Pilars[mapID];
//        //    //        bool added = false;
//        //    //        if (pilar == null)
//        //    //        {
//        //    //            pilar = new GameObject("InvalidPilar").AddComponent<PilarComponent>();
//        //    //            pilar.Init(null, -1, mapID);
//        //    //            Pilars[mapID] = pilar;
//        //    //            pilar.AddBlock();
//        //    //            added = true;
//        //    //        }
//        //    //        else
//        //    //        {
//        //    //            if (pilar.Blocks.Count == 0)
//        //    //            {
//        //    //                pilar.AddBlock();
//        //    //                added = true;
//        //    //            }
//        //    //        }
//        //    //        if (added)
//        //    //        {
//        //    //            var block = pilar.Blocks[0];
//        //    //            block.gameObject.isStatic = true;
//        //    //            block.TopMR.gameObject.isStatic = true;
//        //    //            block.LayerSR.enabled = false;
//        //    //            block.LayerSR.gameObject.isStatic = true;
//        //    //            block.LayerSR.gameObject.SetActive(false);
//        //    //            block.StairSR.enabled = false;
//        //    //            block.StairSR.gameObject.isStatic = true;
//        //    //            block.StairSR.gameObject.SetActive(false);
//        //    //            block.LockSR.enabled = false;
//        //    //            block.LockSR.gameObject.isStatic = true;
//        //    //            block.LockSR.gameObject.SetActive(false);
//        //    //            block.AnchorSR.enabled = false;
//        //    //            block.AnchorSR.gameObject.isStatic = true;
//        //    //            block.AnchorSR.gameObject.SetActive(false);

//        //    //            block.MidMR.enabled = false;
//        //    //            //block.MidBC.enabled = false;
//        //    //            block.MidMR.gameObject.isStatic = true;
//        //    //            block.MidMR.gameObject.SetActive(false);
//        //    //        }
//        //    //    }
//        //    //}
//        //}

//        //public PropScript PlaceProp(int mapPilarID, int pilarBlockID, int familyID)
//        //{
//        //    //var selectedPilar = Pilars[mapPilarID];
//        //    //if (selectedPilar == null || selectedPilar.Blocks.Count <= pilarBlockID)
//        //    //    throw new Exception("Trying to place a prop in a block which does not exist.");

//        //    //var selectedBlock = selectedPilar.Blocks[pilarBlockID];
//        //    //if (selectedBlock.Prop != null)
//        //    //{
//        //    //    selectedBlock.Prop.ReceiveDamage(Def.DamageType.UNAVOIDABLE, selectedBlock.Prop.GetTotalHealth());
//        //    //    Map.RemoveLastCommand();
//        //    //}

//        //    //var propFamily = Props.PropFamilies[familyID];
//        //    //var propID = UnityEngine.Random.Range(0, propFamily.Props.Length);
//        //    //var prop = new GameObject($"Prop_{PropScript.PropID++}").AddComponent<PropScript>();
//        //    //selectedBlock.Prop = prop;
//        //    //Map.Record(new MapCommand(
//        //    //    MapCommandType.PROP_PLACED, propFamily.FamilyName, mapPilarID, pilarBlockID));
//        //    //prop.SetProp(familyID, propID);
//        //    //prop.Block = selectedBlock;
//        //    //if (!selectedPilar.Struc.LivingEntities.Contains(prop))
//        //    //    selectedPilar.Struc.LivingEntities.Add(prop);
//        //    //float nChance = UnityEngine.Random.value;
//        //    //var facing = prop.Facing;
//        //    //if (nChance >= 0.5f)
//        //    //    facing.Horizontal = SpriteHorizontal.RIGHT;
//        //    //prop.Facing = facing;
//        //    //return prop;
//        //    return null;
//        //}

//        //public MonsterScript PlaceMonster(int mapPilarID, int pilarBlockID, int familyID)
//        //{
//        //    //var selectedPilar = Pilars[mapPilarID];
//        //    //if (selectedPilar == null || selectedPilar.Blocks.Count <= pilarBlockID)
//        //    //    throw new Exception("Trying to place a monster in a block which does not exist.");

//        //    //var selectedBlock = selectedPilar.Blocks[pilarBlockID];
//        //    //var mon = Monsters.AddMonsterComponent(
//        //    //    new GameObject($"Monster_{MonsterScript.MonsterID++}"), familyID);

//        //    //var offset = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
//        //    //mon.transform.position = selectedBlock.Pilar.transform.position + new Vector3(offset.x, selectedBlock.Height + selectedBlock.MicroHeight, offset.y);

//        //    //mon.InitMonster();
//        //    //Map.Record(new MapCommand(
//        //    //    MapCommandType.MONSTER_PLACED, mon.Info.Name, mapPilarID, pilarBlockID));

//        //    //mon.enabled = true;

//        //    //if (!selectedPilar.Struc.LivingEntities.Contains(mon))
//        //    //    selectedPilar.Struc.LivingEntities.Add(mon);

//        //    //var facing = mon.Facing;
//        //    //float nChance = UnityEngine.Random.value;
//        //    //if (nChance >= 0.5f)
//        //    //    facing.Horizontal = SpriteHorizontal.RIGHT;
//        //    //nChance = UnityEngine.Random.value;
//        //    //if (nChance >= 0.5f)
//        //    //    facing.Vertical = SpriteVertical.UP;
//        //    //mon.Facing = facing;

//        //    //return mon;
//        //    return null;
//        //}

//        public bool CanBridgeBePlaced(Vector2 buildPos, Vector2 buildDir, float oddYPos)
//        {
//            return false;
//            //    //Vector2Int dir = Vector2Int.zero;
//            //    //m_BridgeDirection = SpaceDirection.COUNT;
//            //    //if (Mathf.Abs(buildDir.x) > Mathf.Abs(buildDir.y))
//            //    //{
//            //    //    if (buildDir.x > 0.0f)
//            //    //    {
//            //    //        dir.Set(1, 0);
//            //    //        m_BridgeDirection = SpaceDirection.SOUTH;
//            //    //    }
//            //    //    else
//            //    //    {
//            //    //        dir.Set(-1, 0);
//            //    //        m_BridgeDirection = SpaceDirection.NORTH;
//            //    //    }
//            //    //}
//            //    //else
//            //    //{
//            //    //    if (buildDir.y > 0.0f)
//            //    //    {
//            //    //        dir.Set(0, 1);
//            //    //        m_BridgeDirection = SpaceDirection.EAST;
//            //    //    }
//            //    //    else
//            //    //    {
//            //    //        dir.Set(0, -1);
//            //    //        m_BridgeDirection = SpaceDirection.WEST;
//            //    //    }
//            //    //}

//            //    //var buildMapID = GameUtils.MapIDFromPosition(buildPos);
//            //    //var checkPos = GameUtils.PosFromID(buildMapID, MapWidth, MapHeight);
//            //    //var buildingPilar = Pilars[buildMapID];
//            //    //m_BridgeBuildBlock = null;
//            //    //float closestDist = float.PositiveInfinity;
//            //    //m_BridgeInitialHeight = 0f;
//            //    //foreach (var bblock in buildingPilar.Blocks)
//            //    //{
//            //    //    m_BridgeInitialHeight = bblock.Height + bblock.MicroHeight;
//            //    //    if (bblock.blockType == BlockType.STAIRS)
//            //    //    {
//            //    //        switch (bblock.Rotation)
//            //    //        {
//            //    //            case BlockRotation.Default:
//            //    //                if (m_BridgeDirection == SpaceDirection.NORTH)
//            //    //                    m_BridgeInitialHeight += 0.5f;
//            //    //                break;
//            //    //            case BlockRotation.Left:
//            //    //                if (m_BridgeDirection == SpaceDirection.EAST)
//            //    //                    m_BridgeInitialHeight += 0.5f;
//            //    //                break;
//            //    //            case BlockRotation.Half:
//            //    //                if (m_BridgeDirection == SpaceDirection.SOUTH)
//            //    //                    m_BridgeInitialHeight += 0.5f;
//            //    //                break;
//            //    //            case BlockRotation.Right:
//            //    //                if (m_BridgeDirection == SpaceDirection.WEST)
//            //    //                    m_BridgeInitialHeight += 0.5f;
//            //    //                break;
//            //    //        }
//            //    //    }
//            //    //    var dist = Mathf.Abs(m_BridgeInitialHeight - oddYPos);
//            //    //    if (dist < closestDist)
//            //    //    {
//            //    //        m_BridgeBuildBlock = bblock;
//            //    //        closestDist = dist;
//            //    //    }
//            //    //}
//            //    //m_BridgeLength = 0;
//            //    //for (int i = 0; i < BridgeMaxLength; ++i)
//            //    //{
//            //    //    var currentPos = checkPos + dir * (i + 1);

//            //    //    var currentMapID = GameUtils.IDFromPos(currentPos, MapWidth, MapHeight);
//            //    //    var currentPilar = Pilars[currentMapID];
//            //    //    if (currentPilar == null || (currentPilar != null && currentPilar.Blocks.Count == 0))
//            //    //        break;
//            //    //    m_BridgePilars[m_BridgeLength++] = currentPilar;
//            //    //}
//            //    //BlockComponent targetBlock = null;
//            //    //int targetPilar = -1;
//            //    //float heightDiff = 0f;
//            //    //m_BridgeFinalHeight = 0f;
//            //    //for (int i = 0; i < m_BridgeLength; ++i)
//            //    //{
//            //    //    var curPilar = m_BridgePilars[i];
//            //    //    var maxDist = (i + 1) * 0.3f;
//            //    //    foreach(var curBlock in curPilar.Blocks)
//            //    //    { 
//            //    //        if (curBlock == null)
//            //    //            continue;

//            //    //        if (curBlock.Layer == 0)
//            //    //            continue;

//            //    //        var blockHeight = curBlock.Height + curBlock.MicroHeight;
//            //    //        if (curBlock.blockType == BlockType.STAIRS)
//            //    //        {
//            //    //            switch (curBlock.Rotation)
//            //    //            {
//            //    //                case BlockRotation.Default:
//            //    //                    if (m_BridgeDirection == SpaceDirection.SOUTH)
//            //    //                        blockHeight += 0.5f;
//            //    //                    break;
//            //    //                case BlockRotation.Left:
//            //    //                    if (m_BridgeDirection == SpaceDirection.WEST)
//            //    //                        blockHeight += 0.5f;
//            //    //                    break;
//            //    //                case BlockRotation.Half:
//            //    //                    if (m_BridgeDirection == SpaceDirection.NORTH)
//            //    //                        blockHeight += 0.5f;
//            //    //                    break;
//            //    //                case BlockRotation.Right:
//            //    //                    if (m_BridgeDirection == SpaceDirection.EAST)
//            //    //                        blockHeight += 0.5f;
//            //    //                    break;
//            //    //            }
//            //    //        }
//            //    //        heightDiff = Mathf.Abs(m_BridgeInitialHeight - blockHeight);
//            //    //        if (heightDiff > maxDist)
//            //    //            continue;
//            //    //        targetBlock = curBlock;
//            //    //        m_BridgeFinalHeight = blockHeight;
//            //    //        break;
//            //    //    }
//            //    //    targetPilar = i;
//            //    //    if (targetBlock != null)
//            //    //        break;
//            //    //}

//            //    //if (targetBlock == null || targetPilar <= 0)
//            //    //    return false;
//            //    //m_BridgeLength = targetPilar;
//            //    //for (int i = m_BridgeLength; i < BridgeMaxLength; ++i)
//            //    //    m_BridgePilars[i] = null;
//            //    //return true;
//            //    return false;
//        }

//        //public BridgeComponent PlaceBridge()
//        //{
//        //    //Vector2 posOffset = Vector2.zero;
//        //    //Vector2 initialPos = Vector2.zero;
//        //    //BlockRotation rotation = BlockRotation.COUNT;
//        //    //bool inverse = false;
//        //    //Vector2Int dir = Vector2Int.zero;
//        //    //const float blockSeparation = StructureComponent.Separation;
//        //    //switch (m_BridgeDirection)
//        //    //{
//        //    //    case SpaceDirection.NORTH:
//        //    //        posOffset.x += (1.0f + blockSeparation);
//        //    //        initialPos = new Vector2(m_BridgeBuildBlock.Pilar.transform.position.x,
//        //    //            m_BridgeBuildBlock.Pilar.transform.position.z);
//        //    //        inverse = true;
//        //    //        rotation = BlockRotation.Left;
//        //    //        dir.Set(-1, 0);
//        //    //        break;
//        //    //    case SpaceDirection.SOUTH:
//        //    //        rotation = BlockRotation.Right;
//        //    //        initialPos = new Vector2(m_BridgePilars[0].transform.position.x,
//        //    //            m_BridgePilars[0].transform.position.z);
//        //    //        dir.Set(1, 0);
//        //    //        break;
//        //    //    case SpaceDirection.EAST:
//        //    //        rotation = BlockRotation.Default;
//        //    //        initialPos = new Vector2(m_BridgePilars[0].transform.position.x,
//        //    //            m_BridgePilars[0].transform.position.z);
//        //    //        dir.Set(0, 1);
//        //    //        break;
//        //    //    case SpaceDirection.WEST:
//        //    //        posOffset.y += (1.0f + blockSeparation);
//        //    //        initialPos = new Vector2(m_BridgeBuildBlock.Pilar.transform.position.x,
//        //    //            m_BridgeBuildBlock.Pilar.transform.position.z);
//        //    //        inverse = true;
//        //    //        rotation = BlockRotation.Half;
//        //    //        dir.Set(0, -1);
//        //    //        break;
//        //    //}
//        //    //Map.Record(new MapCommand(
//        //    //    MapCommandType.BRIDGE_PLACED, m_BridgeDirection.ToString(), m_BridgeBuildBlock.Pilar.MapID,
//        //    //    m_BridgeBuildBlock.Pilar.Blocks.IndexOf(m_BridgeBuildBlock)));

//        //    //float bridgeLength = m_BridgeLength * (1.0f + blockSeparation);
//        //    //Vector2 finalPos = initialPos + new Vector2(dir.x, dir.y) * bridgeLength;
//        //    //float endAnimOffset = WoodenBridgeAnimationComponent.AnimationDuration /* * 0.5f*/ * m_BridgeLength /* * 2.0f*/;

//        //    //var bridgeVFX = new GameObject("Bridge VFX").AddComponent<VFXComponent>();
//        //    //bridgeVFX.transform.Translate(new Vector3(
//        //    //    m_BridgePilars[0].transform.position.x + 0.5f,
//        //    //    m_BridgeInitialHeight + 0.2f,
//        //    //    m_BridgePilars[0].transform.position.z + 0.5f), Space.World);
//        //    //bridgeVFX.SetVFX(new VFXDef(
//        //    //    Def.VFXTarget.GENERAL, "BridgeBombExplosion", Def.VFXType.CAST, 0, Def.VFXFacing.FaceCameraFull,
//        //    //    Def.VFXEnd.SelfDestroy, 24.0f));

//        //    //BridgeComponent prevBridge = null;
//        //    //for (int i = 0; i < m_BridgeLength; ++i)
//        //    //{
//        //    //    var curPilar = m_BridgePilars[i];
//        //    //    curPilar.DestroyBridge();
//        //    //    curPilar.AddBridge(0, BridgeType.SMALL, false);
//        //    //    curPilar.Bridge.Rotation = rotation;
//        //    //    if (prevBridge != null)
//        //    //        prevBridge.Next = curPilar.Bridge;
//        //    //    curPilar.Bridge.Previous = prevBridge;
//        //    //    prevBridge = curPilar.Bridge;
//        //    //    float[] objLH = new float[curPilar.Bridge.Objects.Length];
//        //    //    for (int j = 0; j < curPilar.Bridge.Objects.Length; ++j)
//        //    //    {
//        //    //        var curBridgeObj = curPilar.Bridge.Objects[j];
//        //    //        var curObjPos = new Vector2(curBridgeObj.transform.position.x, curBridgeObj.transform.position.z);// + posOffset;
//        //    //        float distanceFromInit = 0.0f;
//        //    //        if (inverse)
//        //    //        {
//        //    //            if (dir.x != 0)
//        //    //            {
//        //    //                distanceFromInit = Mathf.Abs(curObjPos.x - finalPos.x);
//        //    //            }
//        //    //            else if (dir.y != 0)
//        //    //            {
//        //    //                distanceFromInit = Mathf.Abs(curObjPos.y - finalPos.y);
//        //    //            }
//        //    //            distanceFromInit = bridgeLength - distanceFromInit;
//        //    //        }
//        //    //        else
//        //    //        {
//        //    //            if (dir.x != 0)
//        //    //            {
//        //    //                distanceFromInit = Mathf.Abs(curObjPos.x - initialPos.x);
//        //    //            }
//        //    //            else if (dir.y != 0)
//        //    //            {
//        //    //                distanceFromInit = Mathf.Abs(curObjPos.y - initialPos.y);
//        //    //            }
//        //    //        }

//        //    //        float objHeight = GameUtils.BridgeYPosition(bridgeLength, distanceFromInit, m_BridgeInitialHeight, m_BridgeFinalHeight, out float lh);
//        //    //        objLH[j] = lh;
//        //    //        curBridgeObj.transform.Translate(new Vector3(0.0f, objHeight, 0.0f), Space.Self);
//        //    //    }
//        //    //    curPilar.Bridge.UpdateCollision();

//        //    //    float startingDistance = i * (1.0f + StructureComponent.Separation);
//        //    //    float endingDistance = (i + 1) * (1.0f + StructureComponent.Separation);
//        //    //    float startingHeight = GameUtils.BridgeYPosition(bridgeLength, startingDistance, m_BridgeInitialHeight, m_BridgeFinalHeight, out float sLh);
//        //    //    float endingHeight = GameUtils.BridgeYPosition(bridgeLength, endingDistance, m_BridgeInitialHeight, m_BridgeFinalHeight, out float eLh);
//        //    //    curPilar.Bridge.SetBridgeHeightInfo(startingHeight, endingHeight, m_BridgeDirection);
//        //    //    float appearOffset = i * WoodenBridgeAnimationComponent.AppearOffset * curPilar.Bridge.Objects.Length;
//        //    //    curPilar.Bridge.StartAnimation(appearOffset, endAnimOffset, objLH);
//        //    //}
//        //    //return m_BridgePilars[0].Bridge;
//        //    return null;
//        //}

//        //public void ClearMap()
//        //{
//        //    //foreach(var struc in Strucs)
//        //    //{ 
//        //    //    if (struc == null)
//        //    //        continue;
//        //    //    foreach(var pilar in struc.Pilars)
//        //    //    { 
//        //    //        if (pilar == null)
//        //    //            continue;
//        //    //        var mapID = pilar.MapID;
//        //    //        if (mapID >= 0)
//        //    //        {
//        //    //            Pilars[mapID] = null;
//        //    //        }
//        //    //        pilar.DestroyPilar();
//        //    //    }
//        //    //    for(int i = 0; i < struc.LivingEntities.Count; ++i)
//        //    //    {
//        //    //        var le = struc.LivingEntities[i];
//        //    //        if (le == null)
//        //    //            continue;
//        //    //        le.ReceiveDamage(Def.DamageType.UNAVOIDABLE, le.GetTotalHealth());
//        //    //    }
//        //    //    GameUtils.DeleteGameObjectAndItsChilds(struc.gameObject);
//        //    //}
//        //    //Strucs.Clear();
//        //    //foreach(var pilar in Pilars)
//        //    //{
//        //    //    if (pilar == null)
//        //    //        continue;
//        //    //    pilar.DestroyPilar();
//        //    //}
//        //    //Pilars.Clear();
//        //}

//        public static void ThrowException(string msg = "")
//        {
//            var stackTrace = new StackTrace();
//            if (UnityEngine.Application.isEditor)
//            {
//                Mgr.m_ExceptionFound = true;
//                Mgr.m_ExceptionMessage = msg;
//                Mgr.m_ExceptionStackTrace = stackTrace.ToString();
//                throw new Exception(msg);

//            }
//            else
//            {
//                ExceptionManagement(msg, stackTrace.ToString());
//            }
//        }

//        static void ExceptionManagement(string message, string stackTrace)
//        {
//            Mgr.m_ExceptionFound = true;
//            Mgr.m_ExceptionMessage = message;
//            Mgr.m_ExceptionStackTrace = stackTrace;

//            message += '\n';
//            //MessageBox.Show(
//            //    "An exception has been found!\nMessage:\n" +
//            //    m_ExceptionMessage + "\nA file will be created with the exception data in the executable directory.\nThe application will exit after this dialog.", "Exception!");
//            var time = System.DateTime.Now;
//            var fs = File.Create(ExecutablePath + $"/{time.Year}_{time.Month}_{time.Day}-{time.Hour}.{time.Minute}.{time.Second}.except");
//            var fData = new byte[message.Length * 2 + Mgr.m_ExceptionStackTrace.Length * 2];
//            int lastIdx = 0;
//            for (int i = 0; i < message.Length; ++i)
//            {
//                var bytes = BitConverter.GetBytes(message[i]);
//                for (int j = 0; j < bytes.Length; ++j)
//                {
//                    fData[lastIdx++] = bytes[j];
//                }
//            }
//            for (int i = 0; i < Mgr.m_ExceptionStackTrace.Length; ++i)
//            {
//                var bytes = BitConverter.GetBytes(Mgr.m_ExceptionStackTrace[i]);
//                for (int j = 0; j < bytes.Length; ++j)
//                {
//                    fData[lastIdx++] = bytes[j];
//                }
//            }
//            fs.Write(fData, 0, fData.Length);
//            fs.Close();
//            //UnityEngine.Application.Quit();
//        }

//        //void SetController(IGameController controller)
//        //{
//        //    if (Controller != null)
//        //    {
//        //        Controller.Stop();
//        //    }
//        //    Controller = controller;
//        //    CurrentControllerSel = (int)Controller.GetGameState();
//        //    //if (CurrentControllerSel == (int)GameState.EDITING_STRUCTURE)
//        //    //    OddGO.transform.SetPositionAndRotation(new Vector3(4.0f, 0.0f, 4.0f), Quaternion.identity);
//        //    //else
//        //    //    OddGO.transform.SetPositionAndRotation(new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
//        //    Controller.Start();
//        //    //OddGO.GetComponent<OddScript>().OnControllerChange(Controller);
//        //    CamMgr.CameraType = ECameraType.INGAME;
//        //    CamMgr.CameraType = ECameraType.EDITOR;
//        //}

//        void ImportMesh()
//        {
//            if (importedMeshes != null)
//            {
//                int j = 0;
//                foreach (MeshFilter importedMesh in importedMeshes)
//                {
//                    if (importedMesh == null)
//                        continue;
//                    string path = $"Assets/Mesh{j}.txt";
//                    StreamWriter writer = new StreamWriter(path, false);
//                    writer.WriteLine("vertices:");
//                    for (int i = 0; i < importedMesh.sharedMesh.vertices.Length; i++)
//                        writer.WriteLine($"new Vector3({importedMesh.sharedMesh.vertices[i].x}f, {importedMesh.sharedMesh.vertices[i].y}f, {importedMesh.sharedMesh.vertices[i].z}f),");
//                    writer.WriteLine("normals:");
//                    for (int i = 0; i < importedMesh.sharedMesh.normals.Length; i++)
//                        writer.WriteLine($"new Vector3({importedMesh.sharedMesh.normals[i].x}f, {importedMesh.sharedMesh.normals[i].y}f, {importedMesh.sharedMesh.normals[i].z}f),");
//                    writer.WriteLine("uvs:");
//                    for (int i = 0; i < importedMesh.sharedMesh.uv.Length; i++)
//                        writer.WriteLine($"new Vector2({importedMesh.sharedMesh.uv[i].x}f, {importedMesh.sharedMesh.uv[i].y}f),");
//                    writer.WriteLine("indices:");
//                    for (int i = 0; i < importedMesh.sharedMesh.triangles.Length; i += 3)
//                        writer.WriteLine($"{importedMesh.sharedMesh.triangles[i]}, {importedMesh.sharedMesh.triangles[i + 1]}, {importedMesh.sharedMesh.triangles[i + 2]},");

//                    writer.Flush();
//                    writer.Close();
//                    ++j;
//                }
//            }
//        }

//        void Init()
//        {
//            AvgFPS = new float[20];
//            for (int i = 0; i < AvgFPS.Length; ++i) AvgFPS[i] = 60.0f;
//            HideInfo = false;
//            //CamMgr = m_Camera.gameObject.AddComponent<CameraManager>();
//            ImportMesh();
//            MapQT = new StrucQT(new RectInt(Def.MapStart, Def.MapStart,
//                Def.MapSideSize, Def.MapSideSize));
//            //Map = new MapIE();
//            //Controllers.Add(new PlayingController());
//            //Controllers.Add(new EditingStructureController());
//            //OddScript = OddGO.GetComponent<OddScript>();
//            //SetController(Controllers[(int)StartingState]);
//        }

//        void HandleException(string condition, string stackTrace, LogType type)
//        {
//            if (type != LogType.Exception)
//                return;
//            if (m_ExceptionFound == true)
//                return;
//            if (Application.isEditor)
//            {
//                m_ExceptionFound = true;
//                m_ExceptionMessage = condition;
//                m_ExceptionStackTrace = stackTrace;
//            }
//            else
//            {
//                ExceptionManagement(condition, stackTrace);
//            }
//        }

//        private void Awake()
//        {
//            Application.logMessageReceived += HandleException;
//            Mgr = this;
//            IsLoadFinished = false;
//            LoadingFunctions = new List<InitFunction>(7);
//            {
//                //InitFunction initFN;
//                //initFN.Function = BlockMeshDef.InitBlocks;
//                //initFN.Name = "Loading Block Meshes...";
//                //LoadingFunctions.Add(initFN);
//            }
//            {
//                InitFunction initFN;
//                initFN.Function = BlockMaterial.Init;
//                initFN.Name = "Loading Block Def.Materials...";
//                LoadingFunctions.Add(initFN);
//            }
//            {
//                InitFunction initFN;
//                initFN.Function = Props.Init;
//                initFN.Name = "Loading Props...";
//                LoadingFunctions.Add(initFN);
//            }
//            {
//                InitFunction initFN;
//                initFN.Function = Monsters.Init;
//                initFN.Name = "Loading Monsters...";
//                LoadingFunctions.Add(initFN);
//            }
//            {
//                InitFunction initFN;
//                initFN.Function = VFXs.Init;
//                initFN.Name = "Loading VFXs...";
//                LoadingFunctions.Add(initFN);
//            }
//            {
//                InitFunction initFN;
//                initFN.Function = VFX3Ds.Init;
//                initFN.Name = "Loading 3D VFXs...";
//                LoadingFunctions.Add(initFN);
//            }
//            {
//                InitFunction initFN;
//                initFN.Function = Bridges.Init;
//                initFN.Name = "Loading Bridges...";
//                LoadingFunctions.Add(initFN);
//            }
//            {
//                InitFunction initFN;
//                initFN.Function = Structures.Init;
//                initFN.Name = "Loading Structures...";
//                LoadingFunctions.Add(initFN);
//            }
//            {
//                InitFunction initFN;
//                initFN.Function = Backgrounds.Init;
//                initFN.Name = "Loading Backgrounds...";
//                LoadingFunctions.Add(initFN);
//            }
//            {
//                InitFunction initFN;
//                initFN.Function = () => { };//CameraManager.Init;
//                initFN.Name = "Loading Camera...";
//                LoadingFunctions.Add(initFN);
//            }
//            {
//                InitFunction initFN;
//                //initFN.Function = OddScript.Init;
//                initFN.Name = "Loading Odd...";
//                //LoadingFunctions.Add(initFN);
//            }
//            {
//                InitFunction initFN;
//                initFN.Function = AntManager.Init;
//                initFN.Name = "Loading Ants...";
//                LoadingFunctions.Add(initFN);
//            }
//            {
//                InitFunction initFN;
//                initFN.Function = Init;
//                initFN.Name = "Loading Controllers...";
//                LoadingFunctions.Add(initFN);
//            }

//            CurrentlyLoadingFN = 0;
//            //LoadingText.text = LoadingFunctions[CurrentlyLoadingFN].Name;
//            CurrentlyLoadingFNName = LoadingFunctions[CurrentlyLoadingFN].Name;

//        }

//        public void Stop()
//        {
//            //try
//            //{
//            Structures.Deinit();
//            Bridges.Deinit();
//            VFXs.Deinit();
//            Monsters.Deinit();
//            Props.Deinit();
//            BlockMaterial.Deinit();
//            //}
//            //catch(Exception e)
//            //{
//            //    if (UnityEngine.Application.isEditor)
//            //    {
//            //        throw;
//            //    }
//            //    else
//            //    {
//            //        ExceptionManagement(e.Message, e.StackTrace);
//            //    }
//            //}
//        }
//        // Start is called before the first frame update
//        void Start()
//        {
//            //try
//            //{
//            //    //AvgFPS = new float[20];
//            //    //for (int i = 0; i < AvgFPS.Length; ++i) AvgFPS[i] = 60.0f;
//            //    //CamMgr = m_Camera.GetComponent<CameraManager>();
//            //    //ImportMesh();
//            //    //BlockMeshDef.InitBlocks();
//            //    //BlockMaterial.Init();
//            //    //Props.Init();
//            //    //Monsters.Init();
//            //    //VFXs.Init();
//            //    //Bridges.Init();
//            //    //Structures.Init();
//            //    //Controllers.Add(new PlayingController());
//            //    //Controllers.Add(new EditingStructureController());

//            //    //SetController(Controllers[(int)StartingState]);
//            //}
//            //catch (Exception e)
//            //{
//            //    if (UnityEngine.Application.isEditor)
//            //    {
//            //        throw;
//            //    }
//            //    else
//            //    {
//            //        ExceptionManagement(e.Message, e.StackTrace);
//            //    }
//            //}
//        }

//        // Update is called once per frame
//        void Update()
//        {
//            if (m_ExceptionFound)
//                return;
//            //try
//            //{
//            if (CurrentlyLoadingFN < LoadingFunctions.Count)
//            {
//                LoadingFunctions[CurrentlyLoadingFN].Function();
//                CurrentlyLoadingFNName = LoadingFunctions[CurrentlyLoadingFN].Name;
//                //LoadingText.text = LoadingFunctions[CurrentlyLoadingFN].Name;
//                ++CurrentlyLoadingFN;
//                return;
//            }
//            else if (CurrentlyLoadingFN == LoadingFunctions.Count)
//            {
//                IsLoadFinished = true;
//            }
//            AvgFPS[Time.frameCount % AvgFPS.Length] = 1.0f / Time.deltaTime;
//            //Controller.Update();
//            //}
//            //catch (Exception e)
//            //{
//            //    if (UnityEngine.Application.isEditor)
//            //    {
//            //        throw;
//            //    }
//            //    else
//            //    {
//            //        ExceptionManagement(e.Message, e.StackTrace);
//            //    }
//            //}
//        }

//        private void FixedUpdate()
//        {
//            if (m_ExceptionFound)
//                return;
//            //try
//            //{
//            if (!IsLoadFinished)
//            {
//                return;
//            }
//            //Controller.FixedUpdate();
//            fps = AvgFPS.Average();
//            fps *= 100.0f;
//            fps = Mathf.Floor(fps);
//            fps *= 0.01f;
//            //}
//            //catch (Exception e)
//            //{
//            //    if (UnityEngine.Application.isEditor)
//            //    {
//            //        throw;
//            //    }
//            //    else
//            //    {
//            //        ExceptionManagement(e.Message, e.StackTrace);
//            //    }
//            //}
//        }

//        private void OnGUI()
//        {
//            if (m_ExceptionFound)
//            {
//                GUI.DrawTexture(new Rect(0, 0, m_Canvas.pixelRect.width, m_Canvas.pixelRect.height), Texture2D.blackTexture, ScaleMode.StretchToFill, false);
//                var titleContent = new GUIContent("An Exception has been found!");
//                var textContent = new GUIContent("Check if a file with '.except' extension has been created, report it, and close the Application.");
//                var messageContent = new GUIContent(m_ExceptionMessage);
//                var stackTraceContent = new GUIContent(m_ExceptionStackTrace);
//                var titleSize = GUI.skin.label.CalcSize(titleContent);
//                var textSize = GUI.skin.label.CalcSize(textContent);
//                var messageSize = GUI.skin.label.CalcSize(messageContent);
//                var stackTraceSize = GUI.skin.label.CalcSize(stackTraceContent);
//                var titlePosX = m_Canvas.pixelRect.width * 0.5f - titleSize.x * 0.5f;
//                var titlePosY = 0.0f;
//                var textPosX = m_Canvas.pixelRect.width * 0.5f - textSize.x * 0.5f;
//                var textPosY = titlePosY + titleSize.y;
//                var messagePosX = m_Canvas.pixelRect.width * 0.5f - messageSize.x * 0.5f;
//                var messagePosY = textPosY + textSize.y + 25.0f;
//                var stackTracePosX = m_Canvas.pixelRect.width * 0.5f - stackTraceSize.x * 0.5f;
//                var stackTracePosY = messagePosY + messageSize.y + 5.0f;
//                GUI.Label(new Rect(titlePosX, titlePosY, titleSize.x, titleSize.y), titleContent);
//                GUI.Label(new Rect(textPosX, textPosY, textSize.x, textSize.y), textContent);
//                GUI.Label(new Rect(messagePosX, messagePosY, messageSize.x, messageSize.y), messageContent);
//                GUI.Label(new Rect(stackTracePosX, stackTracePosY, stackTraceSize.x, stackTraceSize.y), stackTraceContent);
//                if (GUI.Button(new Rect(m_Canvas.pixelRect.width * 0.5f - 25f, stackTracePosY + stackTraceSize.y + 50f, 50f, 25f), "Skip?"))
//                {
//                    m_ExceptionFound = false;
//                }
//                if (!UnityEngine.Application.isEditor)
//                {
//                    bool close = GUI.Button(new Rect(m_Canvas.pixelRect.width - 25.0f, 0.0f, 25.0f, 25.0f), "X");
//                    if (close)
//                    {
//                        UnityEngine.Application.Quit();
//                    }
//                }
//                return;
//            }
//            //try
//            //{
//            if (GUI.skin.label.font == null)
//            {
//                GUI.skin.label.font = Font.CreateDynamicFontFromOSFont("arial", 12);
//                GUI.skin.label.fontSize = 12;
//            }
//            var rect = m_Canvas.pixelRect;
//            if (!IsLoadFinished)
//            {
//                GUI.DrawTexture(new Rect(0.0f, 0.0f, rect.width, rect.height), LoadingSprite.texture, ScaleMode.ScaleAndCrop, false);

//                var textContent = new GUIContent(LoadingFunctions[CurrentlyLoadingFN - 1].Name + $" ({CurrentlyLoadingFN - 1}/{LoadingFunctions.Count - 1})");
//                var prevSize = GUI.skin.label.fontSize;
//                GUI.skin.label.fontSize = 20;
//                var textSize = GUI.skin.label.CalcSize(textContent);
//                //var prevColor = GUI.color;
//                //GUI.color = Color.black;
//                GUI.Label(new Rect(rect.width * 0.5f - textSize.x * 0.5f, rect.height - 50.0f, textSize.x, 25.0f), textContent);
//                //GUI.color = prevColor;
//                GUI.skin.label.fontSize = prevSize;
//                return;
//            }
//            if (HideInfo)
//                return;
//            //Controller.OnGUI();
//            // FPS
//            {
//                GUI.Label(new Rect(rect.width - 150.0f, rect.height - 25.0f, 150.0f, 25.0f), "FPS: " + fps.ToString());

//            }
//            var selControl = GUI.SelectionGrid(
//                new Rect(rect.width - 225.0f, 0.0f, 200.0f, 25.0f), CurrentControllerSel, ControllerSelectionStr, (int)GameState.COUNT);
//            //if (selControl != CurrentControllerSel)
//            //    SetController(Controllers[selControl]);

//            if (!UnityEngine.Application.isEditor)
//            {
//                bool close = GUI.Button(new Rect(rect.width - 25.0f, 0.0f, 25.0f, 25.0f), "X");
//                if (close)
//                {
//                    Stop();
//                    UnityEngine.Application.Quit();
//                }
//            }
//            //}
//            //catch (Exception e)
//            //{
//            //    if (UnityEngine.Application.isEditor)
//            //    {
//            //        throw;
//            //    }
//            //    else
//            //    {
//            //        ExceptionManagement(e.Message, e.StackTrace);
//            //    }
//            //}
//        }
//    }
//}