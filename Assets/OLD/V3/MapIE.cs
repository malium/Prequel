/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

namespace Assets
{
    //public enum MapCommandType
    //{
    //    STRUC_PLACED,
    //    STRUC_PLACED_ROTATED90,
    //    STRUC_PLACED_ROTATED180,
    //    STRUC_PLACED_ROTATED270,
    //    STRUC_PLACED_FLIPPEDX,
    //    STRUC_PLACED_FLIPPEDY,
    //    STRUC_PLACED_FLIPPEDXY,

    //    STRUC_PLACED_FLIPPEDX_ROTATED90,
    //    STRUC_PLACED_FLIPPEDX_ROTATED180,
    //    STRUC_PLACED_FLIPPEDX_ROTATED270,

    //    STRUC_PLACED_FLIPPEDY_ROTATED90,
    //    STRUC_PLACED_FLIPPEDY_ROTATED180,
    //    STRUC_PLACED_FLIPPEDY_ROTATED270,

    //    STRUC_PLACED_FLIPPEDXY_ROTATED90,
    //    STRUC_PLACED_FLIPPEDXY_ROTATED180,
    //    STRUC_PLACED_FLIPPEDXY_ROTATED270,

    //    PROP_PLACED,
    //    PROP_DEATH,

    //    MONSTER_PLACED,
    //    MONSTER_DEATH,

    //    BRIDGE_PLACED,
    //    BRIDGE_DEATH,

    //    ANT_PLACED_TOP,
    //    ANT_PLACED_SIDE_U,
    //    ANT_PLACED_SIDE_D,
    //    ANT_DEATH,

    //    PLACED_BLOCK,

    //    PLACED_BOMB,

    //    COUNT
    //}

    //public struct MapCommand
    //{
    //    public MapCommandType CommandType;
    //    public string ObjectName;
    //    public long MapPilarID;
    //    public int PilarBlockID;
    //    public int RngSeed;

    //    public MapCommand(MapCommandType type, string objName, long mapPilarID, int pilarBlockID)
    //    {
    //        CommandType = type;
    //        ObjectName = objName;
    //        MapPilarID = mapPilarID;
    //        PilarBlockID = pilarBlockID;
    //        RngSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    //        UnityEngine.Random.InitState(RngSeed);
    //    }
    //}
    
    //public class MapIE
    //{
        //List<MapCommand> m_Commands;
        //bool m_IsRecording;
        //static readonly string MapFolderPath = Application.dataPath + "/Maps";

        //void Serial_UnknownNode(object sender, XmlNodeEventArgs e)
        //{
        //    Debug.LogWarning("XMLSerializer unknown node.");
        //}

        //void Serial_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        //{
        //    Debug.LogWarning("XMLSerializer unknown attribute.");
        //}

        //void LoadMapFile()
        //{
        //    FileStream fs = null;
        //    var serializer = new XmlSerializer(m_Commands.GetType());
        //    serializer.UnknownNode += new XmlNodeEventHandler(Serial_UnknownNode);
        //    serializer.UnknownAttribute += new XmlAttributeEventHandler(Serial_UnknownAttribute);

        //    string path = MapFolderPath + "/MAP.0000";
        //    var di = new DirectoryInfo(MapFolderPath);
        //    if(!di.Exists)
        //    {
        //        di.Create();
        //    }
        //    if(!File.Exists(path))
        //    {
        //        Save();
        //        return;
        //    }

        //    fs = new FileStream(path, FileMode.Open);
        //    m_Commands = (List<MapCommand>)serializer.Deserialize(fs);
        //    fs.Close();
        //}

        //public MapIE()
        //{
        //    m_Commands = new List<MapCommand>();
        //    m_IsRecording = false;
        //    LoadMapFile();
        //}

        //public bool IsRecording
        //{
        //    get
        //    {
        //        return m_IsRecording;
        //    }
        //    set
        //    {
        //        m_IsRecording = value;
        //    }
        //}

        //void PlaceStruc(int mapPilarID, string strucName, StructureFlip flipped, Def.RotationState rotated)
        //{
        //    var nameSplit = strucName.Split('@');
        //    float height = 0f;
        //    if(nameSplit.Length >= 2)
        //    {
        //        bool parseOK = float.TryParse(nameSplit[1], out float parseHeight);
        //        if(!parseOK)
        //        {
        //            Debug.Log("Couldn't parse height offset loading a structure from map file.");
        //        }
        //        else
        //        {
        //            height = parseHeight;
        //        }
        //    }
        //    Manager.Mgr.PlaceStruct(mapPilarID, Structures.StrucDict[nameSplit[0]], flipped, rotated, height);
        //    Manager.Mgr.AddEmptyBlocks(mapPilarID);
        //}

        //void PlaceProp(int mapPilarID, int pilarBlockID, string familyName)
        //{
        //    var familyID = Props.FamilyDict[familyName];
        //    Manager.Mgr.PlaceProp(mapPilarID, pilarBlockID, familyID);
        //}

        //void PlaceMonster(int mapPilarID, int pilarBlockID, string familyName)
        //{
        //    var familyID = Monsters.MonsterDict[familyName];
        //    Manager.Mgr.PlaceMonster(mapPilarID, pilarBlockID, familyID);
        //}

        //void RemoveProp(int mapPilarID, int pilarBlockID)
        //{
        //    var pilar = Manager.Mgr.Pilars[mapPilarID];
        //    if (pilar == null || pilar.Blocks.Count <= pilarBlockID)
        //    {
        //        Debug.LogWarning("Trying to delete a prop placed in a block which does not exist, while loading a map.");
        //        return;
        //    }
        //    var block = pilar.Blocks[pilarBlockID];
        //    if (block.Prop == null)
        //    {
        //        Debug.LogWarning("Trying to delete a prop placed in a block that does not have a prop.");
        //        return;
        //    }
        //    block.Prop.ReceiveDamage(Def.DamageType.UNAVOIDABLE, block.Prop.GetTotalHealth());
        //}

        //void RemoveMonster(int mapPilarID, string familyName)
        //{
        //    var pilar = Manager.Mgr.Pilars[mapPilarID];
        //    if (pilar == null)
        //    {
        //        Debug.LogWarning("Trying to delete a monster placed in a pilar which does not exist, while loading a map.");
        //        return;
        //    }
        //    var familyID = Monsters.MonsterDict[familyName];
        //    List<MonsterScript> possibleMonsters = new List<MonsterScript>();
        //    foreach(var le in pilar.Struc.LivingEntities)
        //    {
        //        if (le == null || (le != null && le.GetLEType() != Def.LivingEntityType.Monster))
        //            continue;
        //        var mon = le as MonsterScript;
        //        if (mon.Info.MonsterID != familyID)
        //            continue;
        //        possibleMonsters.Add(mon);
        //    }
        //    if(possibleMonsters.Count == 0)
        //    {
        //        Debug.LogWarning("Trying to delete a monster placed in a structure which does not currently is.");
        //        return;
        //    }
        //    const float blockSep = Def.BlockSeparation;
        //    const float halfSize = (1f + blockSep) * 0.5f;
        //    var pilarCenter = new Vector2(pilar.transform.position.x + halfSize, pilar.transform.position.z + halfSize);
        //    float closestDist = float.PositiveInfinity;
        //    MonsterScript closestMon = null;
        //    foreach(var mon in possibleMonsters)
        //    {
        //        var dist = Vector2.Distance(new Vector2(mon.transform.position.x, mon.transform.position.z),
        //            pilarCenter);
        //        if(closestDist > dist)
        //        {
        //            closestDist = dist;
        //            closestMon = mon;
        //        }
        //    }
        //    closestMon.ReceiveDamage(Def.DamageType.UNAVOIDABLE, closestMon.GetTotalHealth());
        //}

        //void PlaceBridge(int mapPilarID, int pilarBlockID, string directionStr)
        //{
        //    bool parseOK = Enum.TryParse(directionStr, out SpaceDirection dir);
        //    if(!parseOK || dir == SpaceDirection.COUNT)
        //    {
        //        throw new Exception("Trying to place a bridge but the saved direction is invalid, while loading a map.");
        //        //return false;
        //    }

        //    Vector2 buildingDir = Vector2.zero;
        //    switch (dir)
        //    {
        //        case SpaceDirection.NORTH:
        //            buildingDir.Set(-1f, 0f);
        //            break;
        //        case SpaceDirection.SOUTH:
        //            buildingDir.Set(+1f, 0f);
        //            break;
        //        case SpaceDirection.EAST:
        //            buildingDir.Set(0f, +1f);
        //            break;
        //        case SpaceDirection.WEST:
        //            buildingDir.Set(0f, -1f);
        //            break;
        //    }
        //    var pilar = Manager.Mgr.Pilars[mapPilarID];
        //    if(pilar == null || (pilar != null && pilar.Blocks.Count <= pilarBlockID))
        //    {
        //        throw new Exception("Trying to place a bridge but the saved location is invalid, while loading a map.");
        //    }
        //    var block = pilar.Blocks[pilarBlockID];
        //    float yPos = block.Height + block.MicroHeight;
        //    var buildPos = GameUtils.PositionFromMapID(pilar.MapID);
        //    const float blockHalfSize = (1f + StructureComponent.Separation) * 0.5f;
        //    buildPos += new Vector2(blockHalfSize, blockHalfSize);
        //    bool canPlace = Manager.Mgr.CanBridgeBePlaced(buildPos, buildingDir, yPos);
        //    if(!canPlace)
        //    {
        //        throw new Exception("Cannot place a saved bridge!");
        //    }
        //    Manager.Mgr.PlaceBridge();
        //}

        //void RemoveBridge(int mapPilarID)
        //{
        //    var pilar = Manager.Mgr.Pilars[mapPilarID];
        //    if(pilar == null || (pilar != null && pilar.Bridge == null))
        //    {
        //        throw new Exception("Trying to remove a saved bridge, but the pilar is invalid or does not have a bridge.");
        //    }
        //    pilar.Bridge.Destroy();
        //}

        //void PlaceTopAnt(int mapPilarID, int pilarBlockID, string antDirection)
        //{
        //    var pilar = Manager.Mgr.Pilars[mapPilarID];
        //    if(pilar == null || (pilar != null && pilar.Blocks.Count <= pilarBlockID))
        //    {
        //        throw new Exception("Trying to place a saved Ant but the target pilar is invalid or does not contain the target block.");
        //    }
        //    var block = pilar.Blocks[pilarBlockID];
        //    bool parseOK = Enum.TryParse(antDirection, out AntTopDirection direction);
        //    if(!parseOK || direction == AntTopDirection.COUNT)
        //    {
        //        throw new Exception("Trying to obtain the direction of a stored Ant but it was invalid.");
        //    }
        //    int version = -1;
        //    switch (direction)
        //    {
        //        case AntTopDirection.SOUTH_NORTH:
        //        case AntTopDirection.NORTH_SOUTH:
        //        case AntTopDirection.EAST_WEST:
        //        case AntTopDirection.WEST_EAST:
        //            version = UnityEngine.Random.Range(0, AntManager.StraightAnts.Count);
        //            break;
        //        case AntTopDirection.SOUTH_EAST:
        //        case AntTopDirection.WEST_SOUTH:
        //        case AntTopDirection.NORTH_WEST:
        //        case AntTopDirection.EAST_NORTH:
        //            version = UnityEngine.Random.Range(0, AntManager.TurnRightAnts.Count);
        //            break;
        //        case AntTopDirection.SOUTH_WEST:
        //        case AntTopDirection.NORTH_EAST:
        //        case AntTopDirection.WEST_NORTH:
        //        case AntTopDirection.EAST_SOUTH:
        //            version = UnityEngine.Random.Range(0, AntManager.TurnLeftAnts.Count);
        //            break;
        //    }
        //    block.SetTopAnt(version, direction);
        //    GameUtils.FixSideAnts(block, direction);
        //}

        //void PlaceSideAnt(int mapPilarID, int pilarBlockID, string blockAntPositionStr, bool up)
        //{
        //    var pilar = Manager.Mgr.Pilars[mapPilarID];
        //    if (pilar == null || (pilar != null && pilar.Blocks.Count <= pilarBlockID))
        //    {
        //        throw new Exception("Trying to place a saved Ant but the target pilar is invalid or does not contain the target block.");
        //    }
        //    var block = pilar.Blocks[pilarBlockID];
        //    bool parseOK = Enum.TryParse(blockAntPositionStr, out Def.DecoPosition position);
        //    if (!parseOK || position == Def.DecoPosition.COUNT)
        //    {
        //        throw new Exception("Trying to obtain the position of a stored Ant but it was invalid.");
        //    }
        //    int version = UnityEngine.Random.Range(0, AntManager.StraightAnts.Count);
        //    block.SetSideAnt(version, (SpaceDirection)(position - 1), true);
        //}

        //void RemoveAnt(int mapPilarID, int pilarBlockID, string blockAntPositionStr)
        //{
        //    var pilar = Manager.Mgr.Pilars[mapPilarID];
        //    if (pilar == null || (pilar != null && pilar.Blocks.Count <= pilarBlockID))
        //    {
        //        throw new Exception("Trying to remove an Ant but the target pilar is invalid or does not contain the target block.");
        //    }
        //    var block = pilar.Blocks[pilarBlockID];
        //    bool parseOK = Enum.TryParse(blockAntPositionStr, out Def.DecoPosition position);
        //    if (!parseOK || position == Def.DecoPosition.COUNT)
        //    {
        //        throw new Exception("Trying to obtain the position of a stored Ant but it was invalid.");
        //    }
        //    switch (position)
        //    {
        //        case Def.DecoPosition.TOP:
        //            block.SetTopAnt();
        //            break;
        //        case Def.DecoPosition.NORTH:
        //        case Def.DecoPosition.SOUTH:
        //        case Def.DecoPosition.EAST:
        //        case Def.DecoPosition.WEST:
        //            block.SetSideAnt(-1, (SpaceDirection)(position - 1));
        //            break;
        //    }
        //}

        //void PlaceBlock(int mapPilarID, int pilarBlockID, string blockMaterialFamily)
        //{
        //    var pilar = Manager.Mgr.Pilars[mapPilarID];
        //    if(pilar == null || (pilar != null && pilar.Blocks.Count <= pilarBlockID))
        //    {
        //        throw new Exception("Couldn't place a saved added block because the block below it does not exist.");
        //    }
        //    var oblock = pilar.Blocks[pilarBlockID];
        //    var nblock = pilar.AddBlock();
        //    var lastBlockHeight = oblock.Height + oblock.MicroHeight;

        //    nblock.Layer = oblock.Layer;
        //    nblock.LayerSR.enabled = false;
        //    nblock.MaterialFmly = BlockMaterial.MaterialFamilies[BlockMaterial.FamilyDict[blockMaterialFamily]];
        //    nblock.Height = lastBlockHeight + 0.5f + ((oblock.blockType == Def.BlockType.STAIRS) ? 0.5f : 0.0f);
        //    nblock.Length = 0.5f;
        //    nblock.MicroHeight = 0.0f;
        //    nblock.Rotation = (Def.RotationState)UnityEngine.Random.Range(0, Def.RotationStateCount);
        //}

        //void PlaceBomb(string positionStr)
        //{
        //    var posSplit = positionStr.Split(':');
        //    if (posSplit.Length != 3)
        //    {
        //        throw new Exception("Trying to place a saved bomb, but the position was invalid.");
        //    }
        //    bool parseOK = float.TryParse(posSplit[0].Trim(), out float x);
        //    if(!parseOK)
        //    {
        //        throw new Exception("Trying to place a saved bomb, but the position X was invalid.");
        //    }
        //    parseOK = float.TryParse(posSplit[1].Trim(), out float y);
        //    if (!parseOK)
        //    {
        //        throw new Exception("Trying to place a saved bomb, but the position Y was invalid.");
        //    }
        //    parseOK = float.TryParse(posSplit[2].Trim(), out float z);
        //    if (!parseOK)
        //    {
        //        throw new Exception("Trying to place a saved bomb, but the position Z was invalid.");
        //    }
        //    Vector3 bombPos = new Vector3(x,y,z);
        //    var bomb = new GameObject("ClassicBomb").AddComponent<BombComponent>();
        //    bomb.transform.Translate(bombPos, Space.World);
        //    bomb.SetInstaBomb(2.5f, 100f, 5f);
        //    //bomb.SetBomb("BombClassic", 2.0f, 2.5f, 100.0f, 5.0f);
        //}

        //public void Load()
        //{
        //    m_IsRecording = false;
        //    var controller = Manager.Mgr.GetController(GameState.PLAYING);
        //    controller.Stop();
        //    controller.Start();
        //    foreach (var command in m_Commands)
        //    {
        //        switch (command.CommandType)
        //        {
        //            case MapCommandType.STRUC_PLACED:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceStruc(command.MapPilarID, command.ObjectName, StructureFlip.NoFlip, Def.RotationState.Default);
        //                break;
        //            case MapCommandType.STRUC_PLACED_ROTATED90:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceStruc(command.MapPilarID, command.ObjectName, StructureFlip.NoFlip, Def.RotationState.Left);
        //                break;
        //            case MapCommandType.STRUC_PLACED_ROTATED180:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceStruc(command.MapPilarID, command.ObjectName, StructureFlip.NoFlip, Def.RotationState.Half);
        //                break;
        //            case MapCommandType.STRUC_PLACED_ROTATED270:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceStruc(command.MapPilarID, command.ObjectName, StructureFlip.NoFlip, Def.RotationState.Right);
        //                break;
        //            case MapCommandType.STRUC_PLACED_FLIPPEDX:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceStruc(command.MapPilarID, command.ObjectName, StructureFlip.HorizontalFlip, Def.RotationState.Default);
        //                break;
        //            case MapCommandType.STRUC_PLACED_FLIPPEDY:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceStruc(command.MapPilarID, command.ObjectName, StructureFlip.VerticalFlip, Def.RotationState.Default);
        //                break;
        //            case MapCommandType.STRUC_PLACED_FLIPPEDXY:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceStruc(command.MapPilarID, command.ObjectName, StructureFlip.VerticalAndHorizontalFlip, Def.RotationState.Default);
        //                break;
        //            case MapCommandType.STRUC_PLACED_FLIPPEDX_ROTATED90:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceStruc(command.MapPilarID, command.ObjectName, StructureFlip.HorizontalFlip, Def.RotationState.Left);
        //                break;
        //            case MapCommandType.STRUC_PLACED_FLIPPEDX_ROTATED180:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceStruc(command.MapPilarID, command.ObjectName, StructureFlip.HorizontalFlip, Def.RotationState.Half);
        //                break;
        //            case MapCommandType.STRUC_PLACED_FLIPPEDX_ROTATED270:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceStruc(command.MapPilarID, command.ObjectName, StructureFlip.HorizontalFlip, Def.RotationState.Right);
        //                break;
        //            case MapCommandType.STRUC_PLACED_FLIPPEDY_ROTATED90:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceStruc(command.MapPilarID, command.ObjectName, StructureFlip.VerticalFlip, Def.RotationState.Left);
        //                break;
        //            case MapCommandType.STRUC_PLACED_FLIPPEDY_ROTATED180:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceStruc(command.MapPilarID, command.ObjectName, StructureFlip.VerticalFlip, Def.RotationState.Half);
        //                break;
        //            case MapCommandType.STRUC_PLACED_FLIPPEDY_ROTATED270:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceStruc(command.MapPilarID, command.ObjectName, StructureFlip.VerticalFlip, Def.RotationState.Right);
        //                break;
        //            case MapCommandType.STRUC_PLACED_FLIPPEDXY_ROTATED90:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceStruc(command.MapPilarID, command.ObjectName, StructureFlip.VerticalAndHorizontalFlip, Def.RotationState.Left);
        //                break;
        //            case MapCommandType.STRUC_PLACED_FLIPPEDXY_ROTATED180:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceStruc(command.MapPilarID, command.ObjectName, StructureFlip.VerticalAndHorizontalFlip, Def.RotationState.Half);
        //                break;
        //            case MapCommandType.STRUC_PLACED_FLIPPEDXY_ROTATED270:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceStruc(command.MapPilarID, command.ObjectName, StructureFlip.VerticalAndHorizontalFlip, Def.RotationState.Right);
        //                break;
        //            case MapCommandType.PROP_PLACED:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceProp(command.MapPilarID, command.PilarBlockID, command.ObjectName);
        //                break;
        //            case MapCommandType.PROP_DEATH:
        //                RemoveProp(command.MapPilarID, command.PilarBlockID);
        //                break;
        //            case MapCommandType.MONSTER_PLACED:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceMonster(command.MapPilarID, command.PilarBlockID, command.ObjectName);
        //                break;
        //            case MapCommandType.MONSTER_DEATH:
        //                RemoveMonster(command.MapPilarID, command.ObjectName);
        //                break;
        //            case MapCommandType.BRIDGE_PLACED:
        //                PlaceBridge(command.MapPilarID, command.PilarBlockID, command.ObjectName);
        //                break;
        //            case MapCommandType.BRIDGE_DEATH:
        //                RemoveBridge(command.MapPilarID);
        //                break;
        //            case MapCommandType.ANT_PLACED_TOP:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceTopAnt(command.MapPilarID, command.PilarBlockID, command.ObjectName);
        //                break;
        //            case MapCommandType.ANT_PLACED_SIDE_U:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceSideAnt(command.MapPilarID, command.PilarBlockID, command.ObjectName, true);
        //                break;
        //            case MapCommandType.ANT_PLACED_SIDE_D:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceSideAnt(command.MapPilarID, command.PilarBlockID, command.ObjectName, false);
        //                break;
        //            case MapCommandType.ANT_DEATH:
        //                RemoveAnt(command.MapPilarID, command.PilarBlockID, command.ObjectName);
        //                break;
        //            case MapCommandType.PLACED_BLOCK:
        //                UnityEngine.Random.InitState(command.RngSeed);
        //                PlaceBlock(command.MapPilarID, command.PilarBlockID, command.ObjectName);
        //                break;
        //            case MapCommandType.PLACED_BOMB:
        //                PlaceBomb(command.ObjectName);
        //                break;
        //            default:
        //                Debug.LogWarning($"Unhandled map command type: {command.CommandType}.");
        //                break;
        //        }
        //    }
        //    m_IsRecording = true;
        //}

        //public void Save()
        //{
        //    string path = MapFolderPath + "/MAP.0000";
        //    var serializer = new XmlSerializer(m_Commands.GetType());
        //    serializer.UnknownNode += new XmlNodeEventHandler(Serial_UnknownNode);
        //    serializer.UnknownAttribute += new XmlAttributeEventHandler(Serial_UnknownAttribute);
        //    var fs = File.Create(path);
        //    serializer.Serialize(fs, m_Commands);
        //    fs.Close();
        //}

        //public void Record(MapCommand command)
        //{
        //    if(m_IsRecording)
        //        m_Commands.Add(command);
        //}

        //public void RemoveLastCommand()
        //{
        //    if (m_IsRecording)
        //        m_Commands.RemoveAt(m_Commands.Count - 1);
        //}

        //public void OnGUI(Canvas canvas)
        //{
        //    if(!m_IsRecording)
        //    {
        //        if(GUI.Button(new Rect(canvas.pixelRect.width * 0.5f - 40f, 0f, 80f, 30f), "Load Map"))
        //        {
        //            Load();
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        if(GUI.Button(new Rect(canvas.pixelRect.width * 0.5f - 75f, 0f, 75f, 30f), "Save Map")
        //            || Input.GetKeyDown(KeyCode.F5))
        //        {
        //            Save();
        //            return;
        //        }
        //        if(GUI.Button(new Rect(canvas.pixelRect.width * 0.5f, 0f, 90f, 30f), "Stop record"))
        //        {
        //            LoadMapFile();
        //            m_IsRecording = false;
        //            return;
        //        }
        //    }
        //}
    //}
}