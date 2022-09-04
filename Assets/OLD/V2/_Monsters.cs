/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Assets
//{
//    public enum Def.MonsterFrame
//    {
//        FACE_1,
//        FACE_2,
//        BACK_1,
//        BACK_2,

//        COUNT
//    }

//    public enum MonsterTeam
//    {
//        ATeam,
//        BTeam,
//        OddTeam,
//        COUNT
//    }

    //public struct MonsterSprite
    //{
    //    public int MonsterID;
    //    public int AttribID;
    //    public float SpriteScale;
    //    public Vector2Int[] LastPixel;
    //    public RectInt[] VisibleRect;
    //    public Vector2[] BoxCenterOffset;
    //    public Vector2[] BoxSize;
    //    public Sprite[] Sprites;
    //}

    //public struct MonsterAttributes
    //{
    //    public int AttribID;
    //    public int MonsterID;
    //    public string Name;
    //    public float BaseHealth;
    //    public float BaseSpeed;
    //    public float SightRange;
    //    public float SightAngle;
    //    public float HearingRange;
    //    public float AttackRange;
    //    public float AttackRate;
    //    public float AttackDamage;
    //    public DamageType DmgType;
    //    public MonsterTeam Team;
    //    public MonsterAIType AIType;
    //    public Type Class;
    //}

    //public class Monsters
    //{
        //static int LastMonsterID = 0;

        //public static List<MonsterSprite> MonsterSprites = new List<MonsterSprite>();
        //public static List<MonsterAttributes> MonsterAttribs = new List<MonsterAttributes>();
        //public static Dictionary<string, int> MonsterAttribDic = new Dictionary<string, int>();

        //public static void Init()
        //{
        //    int LastMonsterID = 0;
        //    int lastAttributeID = 0;
        //    MonsterSprites.Capacity = AssetContainer.Mgr.MonsterSprites.Length;
        //    for(int i = 0; i < AssetContainer.Mgr.MonsterSprites.Length; ++i)
        //    {
        //        var smon = AssetContainer.Mgr.MonsterSprites[i];
        //        if (smon.name[0] != 'M' && smon.name[0] != 'm')
        //            continue;

        //        var nameReader = new StringReader(smon.name);
        //        nameReader.Read(); // M

        //        var number = new char[4];
        //        nameReader.Read(number, 0, number.Length);
        //        var numberStr = new string(number);
        //        int monsterID = int.Parse(numberStr);

        //        if (LastMonsterID < monsterID)
        //            LastMonsterID = monsterID + 1;

        //        nameReader.Read(); // _
        //        var frame = new char[2];
        //        nameReader.Read(frame, 0, frame.Length);
        //        var frameStr = new string(frame);
        //        frameStr = frameStr.ToLower();
        //        Def.MonsterFrame monsterFrame = Def.MonsterFrameCount;
        //        if (frameStr == "f1")
        //            monsterFrame = Def.MonsterFrame.FACE_1;
        //        else if (frameStr == "f2")
        //            monsterFrame = Def.MonsterFrame.FACE_2;
        //        else if (frameStr == "b1")
        //            monsterFrame = Def.MonsterFrame.BACK_1;
        //        else if (frameStr == "b2")
        //            monsterFrame = Def.MonsterFrame.BACK_2;
        //        else
        //            throw new Exception("Unknown frame type '" + frameStr + "'.");

        //        int attribID = 0;
        //        if(MonsterSprites.Count < (monsterID + 1))
        //        {
        //            MonsterSprites.AddRange(Enumerable.Repeat(new MonsterSprite(), (monsterID + 1) - MonsterSprites.Count));
        //            attribID = lastAttributeID++;
        //        }
        //        else
        //        {
        //            attribID = MonsterSprites[monsterID].AttribID;
        //        }

        //        var sprite = MonsterSprites[monsterID];
        //        sprite.AttribID = attribID;
        //        sprite.MonsterID = monsterID;
        //        if(sprite.Sprites == null)
        //        {
        //            sprite.Sprites = new Sprite[Def.MonsterFrameCount];
        //            sprite.LastPixel = new Vector2Int[Def.MonsterFrameCount];
        //            sprite.VisibleRect = new RectInt[Def.MonsterFrameCount];
        //            sprite.BoxSize = new Vector2[Def.MonsterFrameCount];
        //            sprite.BoxCenterOffset = new Vector2[Def.MonsterFrameCount];
        //        }
        //        sprite.Sprites[(int)monsterFrame] = smon;

        //        Vector2Int topPixel = new Vector2Int(-1,-1);
        //        Vector2Int bottomPixel = new Vector2Int(-1, -1);
        //        Vector2Int LeftPixel = new Vector2Int(-1, -1);
        //        Vector2Int RightPixel = new Vector2Int(-1, -1);

        //        for(int y = 0; y < smon.texture.height; ++y)
        //        {
        //            for(int x = 0; x < smon.texture.width; ++x)
        //            {
        //                var color = smon.texture.GetPixel(x, y);
        //                if (color.a == 0.0f)
        //                    continue;

        //                var pixel = new Vector2Int(x, y);
        //                if (topPixel.y < 0 || y > topPixel.y)
        //                {
        //                    topPixel = pixel;
        //                }
        //                if(bottomPixel.y < 0 || y < bottomPixel.y)
        //                {
        //                    bottomPixel = pixel;
        //                }
        //                if(LeftPixel.x < 0 || x < LeftPixel.x)
        //                {
        //                    LeftPixel = pixel;
        //                }
        //                if(RightPixel.x < 0 || x > RightPixel.x)
        //                {
        //                    RightPixel = pixel;
        //                }
        //            }
        //        }

        //        sprite.VisibleRect[(int)monsterFrame] = new RectInt(LeftPixel.x, bottomPixel.y, RightPixel.x - LeftPixel.x, topPixel.y - bottomPixel.y);
        //        sprite.LastPixel[(int)monsterFrame] = bottomPixel;
        //        sprite.BoxSize[(int)monsterFrame] = new Vector2(sprite.VisibleRect[(int)monsterFrame].width, sprite.VisibleRect[(int)monsterFrame].height) / sprite.Sprites[(int)monsterFrame].pixelsPerUnit;

        //        var visCenter = new Vector2(sprite.VisibleRect[(int)monsterFrame].width * 0.5f + sprite.VisibleRect[(int)monsterFrame].x, sprite.VisibleRect[(int)monsterFrame].height * 0.5f + sprite.VisibleRect[(int)monsterFrame].y);
        //        var texCenter = new Vector2(sprite.Sprites[(int)monsterFrame].texture.width * 0.5f, sprite.Sprites[(int)monsterFrame].texture.height * 0.5f);
        //        sprite.BoxCenterOffset[(int)monsterFrame] = visCenter - texCenter;
        //        sprite.BoxCenterOffset[(int)monsterFrame] /= sprite.Sprites[(int)monsterFrame].pixelsPerUnit;

        //        //Vector2Int lp = Vector2Int.zero;
        //        //for (int y = smon.texture.height - 1; y >= 0; --y)
        //        //{
        //        //    for (int x = smon.texture.width - 1; x >= 0; --x)
        //        //    {
        //        //        var pixel = smon.texture.GetPixel(x, y);
        //        //        if (pixel.a == 0.0f)
        //        //            continue;
        //        //        lp = new Vector2Int(x, y);
        //        //    }
        //        //}
        //        MonsterSprites[monsterID] = sprite;
        //    }

        //    MonsterAttribs.Capacity = lastAttributeID;
        //    MonsterAttribs.AddRange(Enumerable.Repeat(new MonsterAttributes(), lastAttributeID));
        //    foreach(var monster in MonsterSprites)
        //    {
        //        var attribs = MonsterAttribs[monster.AttribID];
        //        attribs.MonsterID = monster.MonsterID;
        //        attribs.AttribID = monster.AttribID;
        //        MonsterAttribs[attribs.AttribID] = attribs;
        //    }

        //    TextAsset monsterList = null;
        //    foreach(var textAsset in AssetContainer.Mgr.TextAssets)
        //    {
        //        var lowerName = textAsset.name.ToLower();
        //        if(lowerName == "monsterlist")
        //        {
        //            monsterList = textAsset;
        //            break;
        //        }
        //    }
        //    if (monsterList == null)
        //        throw new Exception("Couldn't read Monster list file, props cannot be read.");
        //    var text = monsterList.text;
        //    var textReader = new StringReader(text);
        //    string line = textReader.ReadLine();
        //    while (line != null)
        //    {
        //        var lineBlocks = line.Split(':');
        //        if (lineBlocks.Length != 11)
        //            throw new Exception("MonsterList has incorrect format.");
        //        // ID
        //        bool parseOK = int.TryParse(lineBlocks[0], out int monsterID);
        //        if (!parseOK)
        //            throw new Exception("Couldn't read Monster list ID");

        //        var monster = MonsterSprites[monsterID];
        //        var attrib = MonsterAttribs[monster.AttribID];

        //        // BaseHealth
        //        parseOK = float.TryParse(lineBlocks[1], out float baseHealth);
        //        if (!parseOK)
        //            throw new Exception($"Couldn't read monster list BaseHealth, monster:{monsterID}");

        //        // BaseSpeed
        //        parseOK = float.TryParse(lineBlocks[2], out float baseSpeed);
        //        if (!parseOK)
        //            throw new Exception($"Couldn't read monster list BaseSpeed, monster:{monsterID}");

        //        // AttackRange
        //        parseOK = float.TryParse(lineBlocks[3], out float attackRange);
        //        if (!parseOK)
        //            throw new Exception($"Couldn't read monster list AttackRange, monster:{monsterID}");

        //        // AttackRate
        //        parseOK = float.TryParse(lineBlocks[4], out float attackRate);
        //        if (!parseOK)
        //            throw new Exception($"Couldn't read monster list AttackRate, monster:{monsterID}");

        //        // AttackDamage
        //        parseOK = float.TryParse(lineBlocks[5], out float attackDamage);
        //        if (!parseOK)
        //            throw new Exception($"Couldn't read monster list AttackDamage, monster:{monsterID}");

        //        // DamageType
        //        char damageTypeCHR = lineBlocks[6].ToLower()[0];
        //        DamageType damageType = Def.DamageType.COUNT;
        //        switch(damageTypeCHR)
        //        {
        //            case 'h':
        //                damageType = Def.DamageType.HIT;
        //                break;
        //            case 'c':
        //                damageType = Def.DamageType.CUT;
        //                break;
        //            case 'f':
        //                damageType = Def.DamageType.FIRE;
        //                break;
        //            case 'i':
        //                damageType = Def.DamageType.ICE;
        //                break;
        //            case 'l':
        //                damageType = Def.DamageType.LIGHT;
        //                break;
        //            case 'e':
        //                damageType = Def.DamageType.ELECTRICAL;
        //                break;
        //            case 'a':
        //                damageType = Def.DamageType.ASPHYXIA;
        //                break;
        //            case 'd':
        //                damageType = Def.DamageType.DEPRESSION;
        //                break;
        //            case 'p':
        //                damageType = Def.DamageType.POISON;
        //                break;
        //            case 'q':
        //                damageType = Def.DamageType.QUICKSILVER;
        //                break;
        //            case 'u':
        //                damageType = Def.DamageType.UNAVOIDABLE;
        //                break;
        //            default:
        //                throw new Exception($"Couldn't read Monster list DamageType, monster:{monsterID}");
        //        }

        //        parseOK = int.TryParse(lineBlocks[7], out int aiType);
        //        if (!parseOK || aiType < 0 || aiType >= (int)MonsterAIType.COUNT)
        //            throw new Exception($"Couldn't read Monster list AIType, monster:{monsterID}");

        //        var teamCHR = lineBlocks[8].ToLower()[0];
        //        MonsterTeam team = MonsterTeam.COUNT;
        //        switch(teamCHR)
        //        {
        //            case 'a':
        //                team = MonsterTeam.ATeam;
        //                break;
        //            case 'b':
        //                team = MonsterTeam.BTeam;
        //                break;
        //            case 'o':
        //                team = MonsterTeam.OddTeam;
        //                break;
        //            default:
        //                throw new Exception($"Couldn't read Monster list Team, monster:{monsterID}");
        //        }
        //        parseOK = float.TryParse(lineBlocks[9], out float scale);
        //        if (!parseOK)
        //            throw new Exception($"Couldn't read monster list Scale, monster:{monsterID}");

        //        var name = lineBlocks[10];

        //        attrib.AIType = (MonsterAIType)aiType;
        //        attrib.Team = team;
        //        attrib.BaseHealth = baseHealth;
        //        attrib.BaseSpeed = baseSpeed;
        //        attrib.HearingRange = 5.0f;
        //        attrib.SightAngle = 60.0f;
        //        attrib.SightRange = 7.0f;
        //        attrib.AttackRange = attackRange;
        //        attrib.AttackRate = attackRate;
        //        attrib.AttackDamage = attackDamage;
        //        attrib.DmgType = damageType;
        //        attrib.Name = name;
        //        attrib.Class = Type.GetType("Assets.AI." + attrib.Name);

        //        monster.SpriteScale = scale;
        //        MonsterAttribDic.Add(name, monster.AttribID);
        //        MonsterSprites[monsterID] = monster;
        //        MonsterAttribs[monster.AttribID] = attrib;
        //        line = textReader.ReadLine();
        //    }
        //}

//        public static void Deinit()
//        {

//        }

//        public static MonsterScript AddMonsterComponent(GameObject gameObject, int monsterID)
//        {
//            //var monster = MonsterSprites[monsterID];
//            //var attrib = MonsterAttribs[monster.AttribID];
//            //if (attrib.Class == null)
//            //    return gameObject.AddComponent<AI.TheAngel>();
//            //return (MonsterScript)gameObject.AddComponent(attrib.Class);
//        }
//    }
//}
