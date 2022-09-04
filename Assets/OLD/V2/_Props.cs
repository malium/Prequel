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
    //public struct Prop
    //{
    //    public Sprite sprite;
    //    public int PropID;
    //    public int PropTypeID;
    //    public float SpriteScale;
    //    public Vector2Int LastPixel;
    //    public RectInt VisibleRect;
    //    public Vector2 BoxCenterOffset;
    //    public Vector2 BoxSize;
    //    public float BoxWidth;
    //    public bool HasShadow;
    //    public bool HasLighting;
    //    public float LightHeight;
    //    public float LightRange;
    //    public float LightIntensity;
    //    public Color LightColor;
    //}

    //public struct PropType
    //{
    //    public int ProbTypeID;
    //    public string ProbTypeName;
    //    public List<int> Props;
    //}

    //public class Props
    //{
        //private static int LastPropID = 0;

        //public static List<Prop> PropList = new List<Prop>();
        //public static List<PropType> PropTypes = new List<PropType>();
        //public static Dictionary<string, int> PropTypeDic = new Dictionary<string, int>();

        //public static void Init()
        //{
        //    int LastPropID = 0;
        //    int lastPropTypeID = 0;
        //    PropList.Capacity = AssetContainer.Mgr.PropSprites.Length;
        //    for(int i = 0; i < AssetContainer.Mgr.PropSprites.Length; ++i)
        //    {
        //        var sprop = AssetContainer.Mgr.PropSprites[i];
        //        if (sprop.name[0] != 'P' && sprop.name[0] != 'p')
        //            continue;
        //        var nameReader = new StringReader(sprop.name);
        //        nameReader.Read(); // P

        //        var number = new char[4];
        //        nameReader.Read(number, 0, number.Length);
        //        var numberStr = new string(number);
        //        int propTypeID = int.Parse(numberStr);

        //        if (lastPropTypeID < propTypeID)
        //            lastPropTypeID = propTypeID;

        //        var v = new char[2];
        //        nameReader.Read(v, 0, v.Length);
        //        if (v[0] != '_' && (v[1] != 'v' && v[1] != 'V'))
        //            throw new Exception($"Trying to parse a prop with name '{sprop.name}', but its not in the correct format.");
        //        var version = new char[2];
        //        nameReader.Read(version, 0, version.Length);

        //        var versionStr = new string(version);
        //        int ver = int.Parse(versionStr);
        //        Prop prop;
        //        prop.sprite = sprop;
        //        prop.PropID = LastPropID++;
        //        prop.PropTypeID = propTypeID;
        //        prop.LastPixel = Vector2Int.zero;
        //        prop.SpriteScale = 1.0f;

        //        //Vector2Int lp = Vector2Int.zero;
        //        //for (int y = sprop.texture.height - 1; y >= 0; --y)
        //        //{
        //        //    for (int x = sprop.texture.width - 1; x >= 0; --x)
        //        //    {
        //        //        var pixel = sprop.texture.GetPixel(x, y);
        //        //        if (pixel.a == 0.0f)
        //        //            continue;
        //        //        lp = new Vector2Int(x, y);
        //        //    }
        //        //}

        //        Vector2Int topPixel = new Vector2Int(-1, -1);
        //        Vector2Int bottomPixel = new Vector2Int(-1, -1);
        //        Vector2Int LeftPixel = new Vector2Int(-1, -1);
        //        Vector2Int RightPixel = new Vector2Int(-1, -1);

        //        var height25 = sprop.texture.height / 4;

        //        for (int y = 0; y < sprop.texture.height; ++y)
        //        {
        //            for (int x = 0; x < sprop.texture.width; ++x)
        //            {
        //                var color = sprop.texture.GetPixel(x, y);
        //                if (color.a == 0.0f)
        //                    continue;

        //                var pixel = new Vector2Int(x, y);
        //                if (topPixel.y < 0 || y > topPixel.y)
        //                {
        //                    topPixel = pixel;
        //                }
        //                if (bottomPixel.y < 0 || y < bottomPixel.y)
        //                {
        //                    bottomPixel = pixel;
        //                }
        //                if (LeftPixel.x < 0 || x < LeftPixel.x)
        //                {
        //                    LeftPixel = pixel;
        //                }
        //                if (RightPixel.x < 0 || x > RightPixel.x)
        //                {
        //                    RightPixel = pixel;
        //                }
        //            }
        //        }
        //        int firstPixel25 = -1, lastPixel25 = -1;
        //        Color prevPixel = sprop.texture.GetPixel(0, height25);
        //        Color curPixel = sprop.texture.GetPixel(1, height25);
        //        for(int j = 1; j < (sprop.texture.width - 1); ++j)
        //        {
        //            var nextPixel = sprop.texture.GetPixel(j + 1, height25);
        //            if(firstPixel25 < 0)
        //            {
        //                if (prevPixel.a == 0 && curPixel.a != 0)
        //                    firstPixel25 = j;
        //            }

        //            if (curPixel.a != 0 && nextPixel.a == 0)
        //                lastPixel25 = j;

        //            prevPixel = curPixel;
        //            curPixel = nextPixel;
        //        }

        //        prop.VisibleRect = new RectInt(LeftPixel.x, bottomPixel.y, RightPixel.x - LeftPixel.x, topPixel.y - bottomPixel.y);
        //        prop.LastPixel = bottomPixel;
        //        prop.BoxSize = new Vector2(prop.VisibleRect.width, prop.VisibleRect.height) / prop.sprite.pixelsPerUnit;

        //        var visCenter = new Vector2(prop.VisibleRect.width * 0.5f + prop.VisibleRect.x, prop.VisibleRect.height * 0.5f + prop.VisibleRect.y);
        //        var texCenter = new Vector2(prop.sprite.texture.width * 0.5f, prop.sprite.texture.height * 0.5f);
        //        prop.BoxCenterOffset = visCenter - texCenter;
        //        prop.BoxCenterOffset /= prop.sprite.pixelsPerUnit;
        //        prop.BoxWidth = (lastPixel25 - firstPixel25) / (float)sprop.texture.width;
        //        prop.HasLighting = false;
        //        prop.LightColor = new Color();
        //        prop.LightHeight = 1.0f;
        //        prop.LightIntensity = 1.0f;
        //        prop.LightRange = 10.0f;
        //        prop.HasShadow = true;

        //        PropList.Add(prop);
        //    }

        //    PropTypes.Capacity = lastPropTypeID + 1;
        //    PropTypes.AddRange(Enumerable.Repeat(new PropType(), lastPropTypeID + 1));
        //    foreach(var prop in PropList)
        //    {
        //        PropType propType = PropTypes[prop.PropTypeID];
        //        propType.ProbTypeID = prop.PropTypeID;
        //        if(propType.Props == null)
        //        {
        //            propType.Props = new List<int>();
        //        }
        //        propType.Props.Add(prop.PropID);
        //        PropTypes[prop.PropTypeID] = propType;
        //    }

        //    TextAsset propList = null;
        //    foreach (var textAsset in AssetContainer.Mgr.TextAssets)
        //    {
        //        var lowerName = textAsset.name.ToLower();
        //        if (lowerName == "proplist")
        //        {
        //            propList = textAsset;
        //            break;
        //        }
        //    }
        //    if (propList == null)
        //        throw new Exception("Couldn't read Prop list file, props cannot be read.");
        //    var text = propList.text;
        //    var textReader = new StringReader(text);
        //    string line = textReader.ReadLine();
        //    while(line != null)
        //    {
        //        var lineBlocks = line.Split(':');
        //        if (lineBlocks.Length != 5)
        //            throw new Exception("PropList has incorrect format.");
        //        int curLineBlock = 0;
        //        int propTypeID = int.Parse(lineBlocks[curLineBlock]);
        //        ++curLineBlock;

        //        PropType propType = PropTypes[propTypeID];
        //        var shdw = lineBlocks[curLineBlock].ToLower()[0];
        //        ++curLineBlock;

        //        var lights = lineBlocks[curLineBlock].Split('.');
        //        ++curLineBlock;
        //        if (lights.Length != propType.Props.Count)
        //            throw new Exception($"Error in PropList, PropType '{propType.ProbTypeID}' has different lights '{lights.Length}' than props '{propType.Props.Count}'");
        //        for(int i = 0; i < lights.Length; ++i)
        //        {
        //            var lightInfo = lights[i].Split('_');
        //            var le = lightInfo[0][0];
        //            bool isLightEnabled = le == 'y' || le == 'Y';
        //            var propID = propType.Props[i];
        //            var prop = PropList[propID];
        //            prop.HasShadow = shdw == 'y';
        //            if (!isLightEnabled)
        //            {
        //                PropList[propID] = prop;
        //                continue;
        //            }
        //            prop.HasLighting = true;

        //            bool parseOK = float.TryParse(lightInfo[1], out float lightHeight);
        //            if (!parseOK)
        //                throw new Exception("Couldn't parse prop light info!");
        //            prop.LightHeight = lightHeight;

        //            parseOK = float.TryParse(lightInfo[2], out float lightRange);
        //            if (!parseOK)
        //                throw new Exception("Couldn't parse prop light info!");
        //            prop.LightRange = lightRange;

        //            parseOK = float.TryParse(lightInfo[3], out float lightIntensity);
        //            if (!parseOK)
        //                throw new Exception("Couldn't parse prop light info!");
        //            prop.LightIntensity = lightIntensity;

        //            parseOK = byte.TryParse(lightInfo[4].Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out byte r);
        //            if (!parseOK)
        //                throw new Exception("Couldn't parse prop light info!");

        //            parseOK = byte.TryParse(lightInfo[4].Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out byte g);
        //            if (!parseOK)
        //                throw new Exception("Couldn't parse prop light info!");

        //            parseOK = byte.TryParse(lightInfo[4].Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out byte b);
        //            if (!parseOK)
        //                throw new Exception("Couldn't parse prop light info!");

        //            //parseOK = ColorUtility.TryParseHtmlString(lightInfo[4], out Color lightColor);
        //            //if (!parseOK)
        //            //    throw new Exception("Couldn't parse prop light info!");

        //            prop.LightColor = new Color(r / 255.0f, g / 255.0f, b / 255.0f);

        //            PropList[propID] = prop;
        //        }

        //        var scales = lineBlocks[curLineBlock].Split('.');
        //        ++curLineBlock;
        //        if (scales.Length != propType.Props.Count)
        //            throw new Exception($"Error in PropList, PropType '{propType.ProbTypeID}' has different scales '{scales.Length}' than props '{propType.Props.Count}'");

        //        for (int i = 0; i < scales.Length; ++i)
        //        {
        //            float scale = -1.0f;
        //            bool parseOK = float.TryParse(scales[i], out scale);
        //            if (!parseOK || (parseOK && scale <= 0.0f))
        //                throw new Exception("Couldn't parse the prop scales!");
        //            var propID = propType.Props[i];
        //            var prop = PropList[propID];
        //            prop.SpriteScale = scale;
        //            PropList[propID] = prop;
        //        }

        //        var name = lineBlocks[curLineBlock].Trim();
        //        ++curLineBlock;
        //        //for (int i = 1; i < names.Length; ++i) names[i] = names[i].Substring(1);

        //        propType.ProbTypeName = name;
        //        PropTypes[propTypeID] = propType;
        //        PropTypeDic.Add(name, propTypeID);
        //        //foreach(var name in names)
        //        //{
        //        //    PropTypeDic.Add(name, propTypeID);
        //        //}
        //        line = textReader.ReadLine();
        //    }
        //}

//        public static void Deinit()
//        {

//        }
//    }
//}
