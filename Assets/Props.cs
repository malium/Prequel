/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace Assets
{
	//[Serializable]
	//public class PropInfoOLD
	//{
	//    public Sprite sprite;
	//    public PropFamilyOLD Familiy;
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
	//    public Sprite DamageSprite;
	//}

	//[Serializable]
	//public class PropFamilyOLD
	//{
	//    public string FamilyName;
	//    [NonSerialized]
	//    public PropInfoOLD[] Props;
	//}

	[Serializable]
	public class PropFamily
	{
		public string FamilyName;
		[NonSerialized]
		public List<PropInfo> Props;
	}

	public static class Props
	{
		static readonly string Path = UnityEngine.Application.streamingAssetsPath + "/Props";
		public static List<FamilyTag> FamilyTags;
		public static Dictionary<string, int> FamilyTagDict;
		public static List<PropInfo> PropList;
		public static List<PropFamily> PropFamilies;
		public static Dictionary<string, int> FamilyDict;

		public static List<UI.CImageSelectorUI.ElementInfo> UIProps;
		public static List<UI.CImageSelectorUI.ElementInfo> UIFamilyTags;

		public static void UpdateUIFamilyTags()
		{
			UIFamilyTags.Clear();
			for (int i = 0; i < FamilyTags.Count; ++i)
			{
				var tag = FamilyTags[i];

				UIFamilyTags.Add(new UI.CImageSelectorUI.ElementInfo()
				{
					Name = tag.TagName
				});
			}
		}
		public static PropFamily GetPropFamily(string familyName)
		{
			if(FamilyDict == null || PropFamilies == null)
				throw new Exception("Props is not ready, trying to get a PropFamily before loading?");
			if (FamilyDict.ContainsKey(familyName))
			{
				return PropFamilies[FamilyDict[familyName]];
			}
			if (PropFamilies.Count == 0)
				throw new Exception("There are no Props families, something went really wrong.");

			return PropFamilies[0];
		}
		public static FamilyTag GetFamilyTag(string familyTagName)
		{
			if (FamilyTagDict == null || FamilyTags == null)
				throw new Exception("Props is not ready, trying to get a FamilyTag before loading?");
			if(FamilyTagDict.ContainsKey(familyTagName))
			{
				return FamilyTags[FamilyTagDict[familyTagName]];
			}
			return new FamilyTag() { TagName = "Null", Friendships = new List<AI.FamilyFriendship>() };
		}
		static void Serial_UnknownNode(object sender, XmlNodeEventArgs e)
		{
			Debug.LogWarning("XMLSerializer unknown node." + e.Name);
		}
		static void Serial_UnknownAttribute(object sender, XmlAttributeEventArgs e)
		{
			Debug.LogWarning("XMLSerializer unknown attribute.");
		}
		public static void SaveFamilyTags()
		{
			var tagSerializer = new XmlSerializer(FamilyTags.GetType());
			tagSerializer.UnknownNode += Serial_UnknownNode;
			tagSerializer.UnknownAttribute += Serial_UnknownAttribute;
			var tagFilePath = Path + "/FamilyTags.xml";
			if (File.Exists(tagFilePath))
				File.Delete(tagFilePath);

			var file = new FileStream(tagFilePath, FileMode.CreateNew);
			tagSerializer.Serialize(file, FamilyTags);
			file.Close();
		  
		}
		public static void Prepare()
		{
			PropList = new List<PropInfo>(AssetLoader.PropInfos);
			PropFamilies = new List<PropFamily>((int)(PropList.Count * 0.75f));
			FamilyDict = new Dictionary<string, int>(PropFamilies.Capacity);
			FamilyTags = new List<FamilyTag>();
			FamilyTagDict = new Dictionary<string, int>();


			for (int i = 0; i < PropList.Count; ++i)
			{
				var prop = PropList[i];
				if (Application.isEditor && (prop.PropSprite.texture.width != prop.PropSprite.texture.height || !Mathf.IsPowerOfTwo(prop.PropSprite.texture.width))) 
				{
					Debug.LogWarning("Prop has an invalid texture size, family:'" + prop.FamilyName + "' filename: '" + prop.name + "'");
				}
				if(!FamilyDict.ContainsKey(prop.FamilyName))
				{
					var family = new PropFamily()
					{
						FamilyName = prop.FamilyName,
						Props = new List<PropInfo>(1)
					};
					prop.Family = family;
					family.Props.Add(prop);
					PropFamilies.Add(family);
					FamilyDict.Add(prop.FamilyName, PropFamilies.Count - 1);
				}
				else
				{
					var idx = FamilyDict[prop.FamilyName];
					var family = PropFamilies[idx];
					prop.Family = family;
					family.Props.Add(prop);
				}
			}

			var tagSerializer = new XmlSerializer(FamilyTags.GetType());
			tagSerializer.UnknownNode += Serial_UnknownNode;
			tagSerializer.UnknownAttribute += Serial_UnknownAttribute;

			var tagFilePath = Path + "/FamilyTags.xml";
			if (File.Exists(tagFilePath))
			{
				var file = new FileStream(tagFilePath, FileMode.Open);
				FamilyTags = (List<FamilyTag>)tagSerializer.Deserialize(file);
				for (int i = 0; i < FamilyTags.Count; ++i)
					FamilyTagDict.Add(FamilyTags[i].TagName, i);

				file.Close();
			}
			else
			{
				SaveFamilyTags();
			}


			UIProps = new List<UI.CImageSelectorUI.ElementInfo>(PropList.Count - 1);
			UIFamilyTags = new List<UI.CImageSelectorUI.ElementInfo>(FamilyTags.Count);
			for (int i = 1; i < PropFamilies.Count; ++i)
			{
				var family = PropFamilies[i];
				if (family == null)
					continue;

				UIProps.Add(new UI.CImageSelectorUI.ElementInfo()
				{
					Image = family.Props[0].PropSprite,
					Name = family.FamilyName
				});
			}
			UpdateUIFamilyTags();
		}

		public static void Init()
		{
			//var props = Resources.LoadAll<Sprite>("Props");
			//PropList = new List<PropInfoOLD>(props.Length);
			//PropFamilies = new List<PropFamilyOLD>(props.Length);
			//FamilyDict = new Dictionary<string, int>(props.Length);

			//PropInfoOLD[] addProp(PropInfoOLD prop, int version, PropInfoOLD[] list)
			//{
			//    PropInfoOLD[] result = null;
			//    if (list != null)
			//    {
			//        if (list.Length > version)
			//        {
			//            list[version] = prop;
			//            return list;
			//        }
			//        result = new PropInfoOLD[version + 1];
			//        list.CopyTo(result, 0);
			//    }
			//    else
			//    {
			//        result = new PropInfoOLD[version + 1];
			//    }
			//    result[version] = prop;

			//    return result;
			//}

			//void readSprite(PropInfoOLD prop)
			//{
			//    Vector2Int topPixel = new Vector2Int(-1, -1);
			//    Vector2Int bottomPixel = new Vector2Int(-1, -1);
			//    Vector2Int LeftPixel = new Vector2Int(-1, -1);
			//    Vector2Int RightPixel = new Vector2Int(-1, -1);

			//    var height25 = prop.sprite.texture.height / 4;

			//    List<Color> damageColors = new List<Color>(4);
			//    int colorIdx = 0;

			//    for (int y = 0; y < prop.sprite.texture.height; ++y)
			//    {
			//        for (int x = 0; x < prop.sprite.texture.width; ++x)
			//        {
			//            var color = prop.sprite.texture.GetPixel(x, y);
			//            if (color.a == 0.0f)
			//                continue;

			//            if(damageColors.Count < 4)
			//            {
			//                damageColors.Add(color);
			//            }

			//            if(damageColors.Count == 4)
			//            {
			//                if(Manager.Mgr.SpawnRNG.NextDouble() < 0.1)
			//                {
			//                    damageColors[colorIdx] = color;
			//                    colorIdx = (colorIdx + 1) % 4;
			//                }
			//            }

			//            var pixel = new Vector2Int(x, y);
			//            if (topPixel.y < 0 || y > topPixel.y)
			//            {
			//                topPixel = pixel;
			//            }
			//            if (bottomPixel.y < 0 || y < bottomPixel.y)
			//            {
			//                bottomPixel = pixel;
			//            }
			//            if (LeftPixel.x < 0 || x < LeftPixel.x)
			//            {
			//                LeftPixel = pixel;
			//            }
			//            if (RightPixel.x < 0 || x > RightPixel.x)
			//            {
			//                RightPixel = pixel;
			//            }
			//        }
			//    }
			//    int firstPixel25 = -1, lastPixel25 = -1;
			//    Color prevPixel = prop.sprite.texture.GetPixel(0, height25);
			//    Color curPixel = prop.sprite.texture.GetPixel(1, height25);
			//    for (int j = 1; j < (prop.sprite.texture.width - 1); ++j)
			//    {
			//        var nextPixel = prop.sprite.texture.GetPixel(j + 1, height25);
			//        if (firstPixel25 < 0)
			//        {
			//            if (prevPixel.a == 0 && curPixel.a != 0)
			//                firstPixel25 = j;
			//        }

			//        if (curPixel.a != 0 && nextPixel.a == 0)
			//            lastPixel25 = j;

			//        prevPixel = curPixel;
			//        curPixel = nextPixel;
			//    }

			//    prop.VisibleRect = new RectInt(LeftPixel.x, bottomPixel.y, RightPixel.x - LeftPixel.x, topPixel.y - bottomPixel.y);
			//    prop.LastPixel = bottomPixel;
			//    prop.BoxSize = new Vector2(prop.VisibleRect.width, prop.VisibleRect.height) / prop.sprite.pixelsPerUnit;

			//    var visCenter = new Vector2(prop.VisibleRect.width * 0.5f + prop.VisibleRect.x, prop.VisibleRect.height * 0.5f + prop.VisibleRect.y);
			//    var texCenter = new Vector2(prop.sprite.texture.width * 0.5f, prop.sprite.texture.height * 0.5f);
			//    prop.BoxCenterOffset = visCenter - texCenter;
			//    prop.BoxCenterOffset /= prop.sprite.pixelsPerUnit;
			//    prop.BoxWidth = (lastPixel25 - firstPixel25) / (float)prop.sprite.texture.width;

			//    Texture2D damageTex = new Texture2D(2, 2);
			//    damageTex.SetPixels(damageColors.ToArray());
			//    damageTex.Apply(true, true);
			//    prop.DamageSprite = Sprite.Create(damageTex, new Rect(0f, 0f, damageTex.width, damageTex.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, Vector4.zero, false);
			//}
			//// Obtain all the names of the props and extract their information
			//for (int i = 0; i < props.Length; ++i)
			//{
			//    var prop = props[i];
			//    var split = prop.name.ToLower().Split('_');
			//    if (split.Length != 2)
			//        throw new Exception($"Prop sprite {i} has an invalid name:'{prop.name}'");
			//    if (split[0][0] != 'p')
			//        throw new Exception($"Prop sprite {i}, is not a Prop sprite.");

			//    var familyIDStr = split[0].Substring(1);
			//    bool parseOK = int.TryParse(familyIDStr, out int familyID);
			//    if (!parseOK)
			//        throw new Exception($"Prop sprite {i}, has an invalid FamiliyID.");

			//    if (split[1][0] != 'v')
			//        throw new Exception($"Prop sprite {i}, has an invalid version format.");
			//    var versionStr = split[1].Substring(1);
			//    parseOK = int.TryParse(versionStr, out int version);
			//    if (!parseOK)
			//        throw new Exception($"Prop sprite {i}, has an invalid version.");

			//    PropInfoOLD curProp = new PropInfoOLD
			//    {
			//        sprite = prop
			//    };
			//    if (PropFamilies.Count <= familyID)
			//    {
			//        PropFamilies.AddRange(Enumerable.Repeat<PropFamilyOLD>(null, (familyID + 1) - PropFamilies.Count));
			//    }
			//    var family = PropFamilies[familyID];
			//    if (family == null)
			//    {
			//        family = new PropFamilyOLD();
			//        PropFamilies[familyID] = family;
			//    }

			//    family.Props = addProp(curProp, version, family.Props);

			//    curProp.Familiy = family;
			//    curProp.LastPixel = Vector2Int.zero;
			//    curProp.SpriteScale = 1.0f;
			//    curProp.HasLighting = false;
			//    curProp.LightColor = new Color();
			//    curProp.LightHeight = 1.0f;
			//    curProp.LightIntensity = 1.0f;
			//    curProp.LightRange = 10.0f;
			//    curProp.HasShadow = true;
			//    readSprite(curProp);
			//    PropList.Add(curProp);
			//}
			//// Find the PropList asset
			//var textAssets = Resources.LoadAll<TextAsset>("Props");
			//TextAsset propList = null;
			//foreach (var textAsset in textAssets)
			//{
			//    if (textAsset.name.ToLower() == "proplist")
			//    {
			//        propList = textAsset;
			//        break;
			//    }
			//}
			//if (propList == null)
			//    throw new Exception("Couldn't find the PropList file in Resources/Props/");

			//var text = propList.text;
			//var reader = new StringReader(text);
			//var line = reader.ReadLine();
			//int lineNum = 0;
			//while (line != null)
			//{
			//    var lineBlocks = line.Split(':');
			//    if (lineBlocks.Length != 5)
			//        throw new Exception($"PropList has incorrect format at line:{lineNum}.");
			//    int curLineBlock = 0;

			//    bool parseOK = int.TryParse(lineBlocks[curLineBlock], out int familyID);
			//    if (!parseOK)
			//        throw new Exception($"PropList has an incorrect FamilyID at line:{lineNum}.");
			//    ++curLineBlock;

			//    var family = PropFamilies[familyID];
			//    var shdw = lineBlocks[curLineBlock].ToLower()[0];
			//    ++curLineBlock;
			//    var lights = lineBlocks[curLineBlock].Split('.');
			//    ++curLineBlock;

			//    if (lights.Length != family.Props.Length)
			//        throw new Exception($"Error in PropList, Prop '{lineNum}' has different lights '{lights.Length}' than props '{family.Props.Length}'");

			//    for (int i = 0; i < lights.Length; ++i)
			//    {
			//        var lightInfo = lights[i].Split('_');
			//        var le = lightInfo[0][0];
			//        bool isLightEnabled = le == 'y' || le == 'Y';
			//        var prop = family.Props[i];
			//        prop.HasShadow = shdw == 'y';
			//        if (!isLightEnabled)
			//        {
			//            continue;
			//        }
			//        prop.HasLighting = true;

			//        parseOK = float.TryParse(lightInfo[1], out float lightHeight);
			//        if (!parseOK)
			//            throw new Exception($"Error in PropList, Couldn't parse prop light info, at line:{lineNum}");
			//        prop.LightHeight = lightHeight;

			//        parseOK = float.TryParse(lightInfo[2], out float lightRange);
			//        if (!parseOK)
			//            throw new Exception($"Error in PropList, Couldn't parse prop light info, at line:{lineNum}");
			//        prop.LightRange = lightRange;

			//        parseOK = float.TryParse(lightInfo[3], out float lightIntensity);
			//        if (!parseOK)
			//            throw new Exception($"Error in PropList, Couldn't parse prop light info, at line:{lineNum}");
			//        prop.LightIntensity = lightIntensity;

			//        parseOK = byte.TryParse(lightInfo[4].Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out byte r);
			//        if (!parseOK)
			//            throw new Exception($"Error in PropList, Couldn't parse prop light info, at line:{lineNum}");

			//        parseOK = byte.TryParse(lightInfo[4].Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out byte g);
			//        if (!parseOK)
			//            throw new Exception($"Error in PropList, Couldn't parse prop light info, at line:{lineNum}");

			//        parseOK = byte.TryParse(lightInfo[4].Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out byte b);
			//        if (!parseOK)
			//            throw new Exception($"Error in PropList, Couldn't parse prop light info, at line:{lineNum}");

			//        prop.LightColor = new Color(r / 255.0f, g / 255.0f, b / 255.0f);
			//    }

			//    var scales = lineBlocks[curLineBlock].Split('.');
			//    ++curLineBlock;
			//    if (scales.Length != family.Props.Length)
			//        throw new Exception($"Error in PropList, Prop '{lineNum}' has different scales '{scales.Length}' than props '{family.Props.Length }'");

			//    for (int i = 0; i < scales.Length; ++i)
			//    {
			//        parseOK = float.TryParse(scales[i], out float scale);
			//        if (!parseOK || (parseOK && scale <= 0.0f))
			//            throw new Exception($"Error in PropList, Prop '{lineNum}', has an invalid scale {i}.");

			//        family.Props[i].SpriteScale = scale;
			//    }

			//    var name = lineBlocks[curLineBlock].Trim();
			//    ++curLineBlock;

			//    family.FamilyName = name;
			//    FamilyDict.Add(name, familyID);

			//    ++lineNum;
			//    line = reader.ReadLine();
			//}
		}

		public static void Deinit()
		{

		}
	}
}
