/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum AntTopDirection
{
    SOUTH_NORTH,
    NORTH_SOUTH,
    EAST_WEST,
    WEST_EAST,
    SOUTH_WEST,
    SOUTH_EAST,
    NORTH_WEST,
    NORTH_EAST,
    WEST_NORTH,
    WEST_SOUTH,
    EAST_NORTH,
    EAST_SOUTH,

    COUNT
}

public enum AntType
{
    STRAIGHT,
    TURN_LEFT,
    TURN_RIGHT,

    COUNT
}

[Serializable]
public class AntDef
{
    public int AntVersion;
    public string Name;
    public AntType Type;
    public Sprite[] Frames;
}

public class AntManager : MonoBehaviour
{
    public static AntManager Mgr;

    public static List<AntDef> StraightAnts;
    public static List<AntDef> TurnLeftAnts;
    public static List<AntDef> TurnRightAnts;

    const int FrameCount = 64;
    const float FramesPerSec = 12f;
    const float FramePeriod = 1f / FramesPerSec;

    public int GetCurrentFrame()
    {
        return m_CurrentFrame;
    }

    public float GetNextFrameChange()
    {
        return m_NextFrameChange;
    }

    int m_CurrentFrame;
    float m_NextFrameChange;

    public static void Init()
    {
        var antList = Resources.LoadAll<Sprite>("Ants");
        StraightAnts = new List<AntDef>(antList.Length / (64 * 3));
        TurnLeftAnts = new List<AntDef>(StraightAnts.Capacity);
        TurnRightAnts = new List<AntDef>(TurnLeftAnts.Capacity);

        bool parseOK = false;

        Sprite[] addFrame(Sprite sprite, int frame, Sprite[] list)
        {
            Sprite[] result = null;

            if(list != null)
            {
                if(list.Length > frame)
                {
                    list[frame] = sprite;
                    return list;
                }
                result = new Sprite[frame + 1];
                list.CopyTo(result, 0);
            }
            else
            {
                result = new Sprite[frame + 1];
            }
            result[frame] = sprite;

            return result;
        }

        void addAnt(ref List<AntDef> list, Sprite sprite, AntType type, string name, int version, int frame)
        {
            AntDef antDef = null;
            if (list.Count <= version)
            {
                list.AddRange(Enumerable.Repeat<AntDef>(null, (version + 1) - list.Count));
                antDef = new AntDef
                {
                    AntVersion = version,
                    Name = name,
                    Type = type,
                    Frames = null
                };
                list[version] = antDef;
            }
            if (antDef == null)
                antDef = list[version];

            antDef.Frames = addFrame(sprite, frame, antDef.Frames);
        }

        for(int i = 0; i < antList.Length; ++i)
        {
            var ant = antList[i];
            var split = ant.name.ToLower().Split('_');
            if (split.Length != 5)
                throw new Exception($"Block decoration {i}, has an invalid name:'{ant.name}'.");
            if (split[0][0] != 'd')
                throw new Exception($"Block decoration {i}, is not a BlockDecoration sprite.");

            var typeStr = split[1];
            AntType type = AntType.COUNT;
            switch(typeStr)
            {
                case "s":
                    type = AntType.STRAIGHT;
                    break;
                case "tl":
                    type = AntType.TURN_LEFT;
                    break;
                case "tr":
                    type = AntType.TURN_RIGHT;
                    break;
                default:
                    throw new Exception($"Unknown Ant type: '{ant.name}'");
            }

            var name = split[2];

            parseOK = int.TryParse(split[3], out int version);
            if (!parseOK || version < 0)
                throw new Exception($"Block decoration {i}, has an invalid version: '{split[3]}'.");

            parseOK = int.TryParse(split[4], out int frame);
            if (!parseOK || frame < 0)
                throw new Exception($"Block decoration {i}, has an invalid frame: '{split[4]}'.");

            switch (type)
            {
                case AntType.STRAIGHT:
                    addAnt(ref StraightAnts, ant, type, name, version, frame);
                    break;
                case AntType.TURN_LEFT:
                    addAnt(ref TurnLeftAnts, ant, type, name, version, frame);
                    break;
                case AntType.TURN_RIGHT:
                    addAnt(ref TurnRightAnts, ant, type, name, version, frame);
                    break;
            }
        }
    }

    private void Awake()
    {
        Mgr = this;
        m_CurrentFrame = 0;
        m_NextFrameChange = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_NextFrameChange < Time.time)
        {
            m_NextFrameChange = Time.time + FramePeriod;
            m_CurrentFrame = ++m_CurrentFrame % FrameCount;
        }
    }
}
