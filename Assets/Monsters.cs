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
	public enum MonsterTeam
	{
		ATeam,
		BTeam,
		OddTeam,
		COUNT
	}
	
	//[Serializable]
	//public class MonsterInfo
	//{
	//    public string Name;
	//    public int MonsterID;
	//    public float SpriteScale;
	//    public Vector2Int[] LastPixel;
	//    public RectInt[] VisibleRect;
	//    public Vector2[] BoxCenterOffset;
	//    public Vector2[] BoxSize;
	//    public Sprite[] Sprites;
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

	//    public MonsterInfo()
	//    {

	//    }

	//    public MonsterInfo(MonsterInfo copy)
	//    {
	//        Name = copy.Name;
	//        MonsterID = copy.MonsterID;
	//        SpriteScale = copy.SpriteScale;
	//        LastPixel = new Vector2Int[copy.LastPixel.Length];
	//        copy.LastPixel.CopyTo(LastPixel, 0);
	//        VisibleRect = new RectInt[copy.VisibleRect.Length];
	//        copy.VisibleRect.CopyTo(VisibleRect, 0);
	//        BoxCenterOffset = new Vector2[copy.BoxCenterOffset.Length];
	//        copy.BoxCenterOffset.CopyTo(BoxCenterOffset, 0);
	//        BoxSize = new Vector2[copy.BoxSize.Length];
	//        copy.BoxSize.CopyTo(BoxSize, 0);
	//        Sprites = new Sprite[copy.Sprites.Length];
	//        copy.Sprites.CopyTo(Sprites, 0);
	//        BaseHealth = copy.BaseHealth;
	//        BaseSpeed = copy.BaseSpeed;
	//        SightRange = copy.SightRange;
	//        SightAngle = copy.SightAngle;
	//        HearingRange = copy.HearingRange;
	//        AttackRange = copy.AttackRange;
	//        AttackRate = copy.AttackRate;
	//        AttackDamage = copy.AttackDamage;
	//        DmgType = copy.DmgType;
	//        Team = copy.Team;
	//        AIType = copy.AIType;
	//        Class = copy.Class;
	//    }
	//}
	public struct FamilyTag
	{
		public string TagName;
		public List<AI.FamilyFriendship> Friendships;
	}
	public static class Monsters
	{
		public static List<FamilyTag> FamilyTags;
		public static Dictionary<string, int> FamilyTagDict;
		public static List<MonsterFamily> MonsterFamilies;
		public static Dictionary<string, int> FamilyDict;
		static bool XMLError;

		public static List<UI.CImageSelectorUI.ElementInfo> UIMonsters;
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
		public static MonsterFamily GetMonsterFamily(string familyName)
		{
			if (FamilyDict == null || MonsterFamilies == null)
				throw new Exception("Monsters is not ready, trying to get a MonsterFamily before loading?");
			if (FamilyDict.ContainsKey(familyName))
			{
				return MonsterFamilies[FamilyDict[familyName]];
			}
			if (MonsterFamilies.Count == 0)
				throw new Exception("There are no Monsters families, something went really wrong.");

			return MonsterFamilies[0];
		}
		public static FamilyTag GetFamilyTag(string familyTagName)
		{
			if (FamilyTagDict == null || FamilyTags == null)
				throw new Exception("Monsters is not ready, trying to get a FamilyTag before loading?");
			if (FamilyTagDict.ContainsKey(familyTagName))
			{
				return FamilyTags[FamilyTagDict[familyTagName]];
			}
			return new FamilyTag() { TagName = "Null", Friendships = new List<AI.FamilyFriendship>() };
		}
		static void Serial_UnknownNode(object sender, XmlNodeEventArgs e)
		{
			XMLError = true;
			Debug.LogWarning("XMLSerializer unknown node." + e.Name);
		}
		static void Serial_UnknownAttribute(object sender, XmlAttributeEventArgs e)
		{
			Debug.LogWarning("XMLSerializer unknown attribute.");
		}
		static void AddHippieAndWarrior()
		{
			var hippieFamilies = new List<AI.FamilyFriendship>(MonsterFamilies.Count)
			{
				new AI.FamilyFriendship()
				{
					FamilyName = "ODD",
					Friend = true
				}
			};
			for (int i = 1; i < MonsterFamilies.Count; ++i)
			{
				hippieFamilies.Add(new AI.FamilyFriendship()
				{
					FamilyName = MonsterFamilies[i].Name,
					Friend = true
				});
			}
			FamilyTags.Insert(0, new FamilyTag()
			{
				TagName = "HIPPIE",
				Friendships = hippieFamilies // Full, he loves all the others
			});
			FamilyTagDict.Add("HIPPIE", 0);
			//var warriorFamilies = new List<AI.FamilyFriendship>(hippieFamilies);
			//for (int i = 0; i < warriorFamilies.Count; ++i)
			//	warriorFamilies[i] = new AI.FamilyFriendship() { FamilyName = warriorFamilies[i].FamilyName, Friend = false };
			FamilyTags.Insert(1, new FamilyTag()
			{
				TagName = "WARRIOR",
				Friendships = new List<AI.FamilyFriendship>() // Empty, he hates all the others
			});
			FamilyTagDict.Add("WARRIOR", 1);
		}
		static void RemoveHippieAndWarrior()
		{
			if(FamilyTagDict.ContainsKey("HIPPIE"))
			{
				FamilyTags.RemoveAt(0);
				//FamilyTags.RemoveAt(FamilyTagDict["HIPPIE"]);
				FamilyTagDict.Remove("HIPPIE");
			}
			if(FamilyTagDict.ContainsKey("WARRIOR"))
			{
				FamilyTags.RemoveAt(0);
				//FamilyTags.RemoveAt(FamilyTagDict["WARRIOR"]);
				FamilyTagDict.Remove("WARRIOR");
			}
		}
		public static void OnFamilyTagsUpdated()
		{
			for(int i = 0; i < MonsterFamilies.Count; ++i)
			{
				var family = MonsterFamilies[i];
				if(family.FamilyTags == null)
				{
					family.FamilyTags = new List<string>();
					continue;
				}
				family.FamilyTags.Clear();
			}
			RemoveHippieAndWarrior();
			for(int i = 0; i < FamilyTags.Count; ++i)
			{
				var tag = FamilyTags[i];

				for(int j = 0; j < tag.Friendships.Count; ++j)
				{
					var friend = tag.Friendships[j];
					if (friend.FamilyName == "ODD")
						continue; // ODD is not a monster

					if(!FamilyDict.ContainsKey(friend.FamilyName))
					{
						Debug.LogWarning("Monster not found " + friend.FamilyName + " MonsterTag: " + tag.TagName);
						continue;
					}
					var monster = MonsterFamilies[FamilyDict[friend.FamilyName]];
					if(monster.FamilyTags.Contains(tag.TagName))
					{
						Debug.LogWarning("Monster Tag: " + tag.TagName + " has multiple " + monster.Name);
						continue;
					}
					monster.FamilyTags.Add(tag.TagName);
				}
			}
			AddHippieAndWarrior();
		}
		public static void SaveFamilyTags(bool addHippieWarrior = true)
		{
			RemoveHippieAndWarrior();


			var tagSerializer = new XmlSerializer(FamilyTags.GetType());
			tagSerializer.UnknownNode += Serial_UnknownNode;
			tagSerializer.UnknownAttribute += Serial_UnknownAttribute;
			var tagFilePath = AI.MonsterInfo.Path + "/FamilyTags.xml";
			if (File.Exists(tagFilePath))
				File.Delete(tagFilePath);

			var file = new FileStream(tagFilePath, FileMode.CreateNew);
			tagSerializer.Serialize(file, FamilyTags);
			file.Close();

			if(addHippieWarrior)
				AddHippieAndWarrior();
		}
		public static void SaveMonsterInfo(int monsterID)
		{
			var family = MonsterFamilies[monsterID];

			var monsterSerializer = new XmlSerializer(typeof(AI.MonsterInfo));
			monsterSerializer.UnknownNode += Serial_UnknownNode;
			monsterSerializer.UnknownAttribute += Serial_UnknownAttribute;
			var infoFilePath = AI.MonsterInfo.Path + "/" + GameUtils.RemoveWhitespaces(family.Name) + ".xml";

			if (File.Exists(infoFilePath))
				File.Delete(infoFilePath);

			var file = new FileStream(infoFilePath, FileMode.CreateNew);
			monsterSerializer.Serialize(file, family.Info);
			file.Close();
		}
		public static void Prepare()
		{
			MonsterFamilies = new List<MonsterFamily>(AssetLoader.MonsterFamilies);
			FamilyDict = new Dictionary<string, int>(MonsterFamilies.Count);
			FamilyTags = new List<FamilyTag>();
			FamilyTagDict = new Dictionary<string, int>();


			var di = new DirectoryInfo(AI.MonsterInfo.Path);
			if (!di.Exists)
				di.Create();
			var monsterSerializer = new XmlSerializer(typeof(AI.MonsterInfo));
			monsterSerializer.UnknownNode += Serial_UnknownNode;
			monsterSerializer.UnknownAttribute += Serial_UnknownAttribute;

			for (int i = 0; i < MonsterFamilies.Count; ++i)
			{
				var family = MonsterFamilies[i];
				//family.Class = Type.GetType("Assets.AI." + GameUtils.RemoveWhitespaces(family.Name));
				FamilyDict.Add(family.Name, i);

				FileStream file = null;
				var infoFilePath = AI.MonsterInfo.Path + "/" + GameUtils.RemoveWhitespaces(family.Name) + ".xml";
				if (!File.Exists(infoFilePath))
				{
					family.Info = new AI.MonsterInfo
					{
						MonsterFamily = family.Name,
						BaseHealth = family.BaseHealth,
						HealthRegen = 1f,
						BaseSoulness = 100f,
						SoulnessRegen = 1f,
						BaseSpeed = family.BaseSpeed,
						Weight = 1f,
						SightRange = family.SightRange,
						SightAngle = family.SightAngle,
						HearingRange = family.HearingRange,
						PhysicalResistance	= 0.01f * family.PhysicalResistance,
						ElementalResistance = 0.01f * family.ElementalResistance,
						UltimateResistance	= 0.01f * family.UltimateResistance,
						SoulResistance		= 0.01f * family.SoulResistance,
						PoisonResistance	= 0.01f * family.PoisonResistance,
						PhysicalHealing = false,
						ElementalHealing = false,
						UltimateHealing = false,
						SoulHealing = false,
						PoisonHealing = false,
						Friendships = new List<AI.FamilyFriendship>(),
						Quirks = new List<AI.QuirkInfo>(),
						StepDistance = 0.5f,
						SpriteScale = family.SpriteScale,
						Attacks = new AI.AttackInfo[Def.MonsterSpellSlotsCount],
						AlertTime = 3f,
						WarnTime = 3f,
					};
					file = new FileStream(infoFilePath, FileMode.OpenOrCreate);
					monsterSerializer.Serialize(file, family.Info);
				}
				else
				{
					file = new FileStream(infoFilePath, FileMode.Open);
					XMLError = false;
					family.Info = (AI.MonsterInfo)monsterSerializer.Deserialize(file);
					// New info to be updated for old XMLs
					if (family.Info.SpriteScale <= 0f)
					{
						family.Info.SpriteScale = family.SpriteScale;
						XMLError = true;
					}
					if (family.Info.StepDistance <= 0f)
					{
						family.Info.StepDistance = 0.5f;
						XMLError = true;
					}
					if (family.Info.WarnTime < 0f)
					{
						family.Info.WarnTime = 3f;
						XMLError = true;
					}
					if (family.Info.AlertTime < 0f)
					{
						family.Info.AlertTime = 3f;
						XMLError = true;
					}
					if(XMLError) // Something is not updated
					{
						file.SetLength(0);
						file.Flush(true);
						//file.Seek(0, SeekOrigin.Begin); // Not needed in theory
						monsterSerializer.Serialize(file, family.Info);
						Debug.Log("Monster: " + family.Name + " XML updated.");
					}
				}
				file.Close();
			}
			var tagSerializer = new XmlSerializer(FamilyTags.GetType());
			tagSerializer.UnknownNode += Serial_UnknownNode;
			tagSerializer.UnknownAttribute += Serial_UnknownAttribute;

			var tagFilePath = AI.MonsterInfo.Path + "/FamilyTags.xml";
			if(File.Exists(tagFilePath))
			{
				var file = new FileStream(tagFilePath, FileMode.Open);
				FamilyTags = (List<FamilyTag>)tagSerializer.Deserialize(file);
				AddHippieAndWarrior();
				for (int i = 2; i < FamilyTags.Count; ++i)
					FamilyTagDict.Add(FamilyTags[i].TagName, i);

				file.Close();
			}
			else
			{
				SaveFamilyTags();
			}
			OnFamilyTagsUpdated();

			UIMonsters = new List<UI.CImageSelectorUI.ElementInfo>(MonsterFamilies.Count - 1);
			UIFamilyTags = new List<UI.CImageSelectorUI.ElementInfo>(FamilyTags.Count);

			for(int i = 1; i < MonsterFamilies.Count; ++i)
			{
				var family = MonsterFamilies[i];
				if (family == null)
					continue;

				UIMonsters.Add(new UI.CImageSelectorUI.ElementInfo()
				{
					Image = family.Frames[0],
					Name = family.Name
				});
			}
			UpdateUIFamilyTags();
		}
		public static void Init()
		{
			//var monsters = Resources.LoadAll<Sprite>("Monsters");
			//MonsterInfos = new List<MonsterInfo>(monsters.Length / 4);
			//MonsterDict = new Dictionary<string, int>(MonsterInfos.Capacity);
			//bool parseOK = false;

			//void setVisibility(MonsterInfo info, int curSpriteIdx)
			//{
			//    Vector2Int topPixel = new Vector2Int(-1, -1);
			//    Vector2Int bottomPixel = new Vector2Int(-1, -1);
			//    Vector2Int LeftPixel = new Vector2Int(-1, -1);
			//    Vector2Int RightPixel = new Vector2Int(-1, -1);
			//    var smon = info.Sprites[curSpriteIdx];

			//    for (int y = 0; y < smon.texture.height; ++y)
			//    {
			//        for (int x = 0; x < smon.texture.width; ++x)
			//        {
			//            var color = smon.texture.GetPixel(x, y);
			//            if (color.a == 0.0f)
			//                continue;

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

			//    info.VisibleRect[curSpriteIdx] = new RectInt(LeftPixel.x, bottomPixel.y, RightPixel.x - LeftPixel.x, topPixel.y - bottomPixel.y);
			//    info.LastPixel[curSpriteIdx] = bottomPixel;
			//    info.BoxSize[curSpriteIdx] = new Vector2(info.VisibleRect[curSpriteIdx].width, info.VisibleRect[curSpriteIdx].height) / smon.pixelsPerUnit;

			//    var visCenter = new Vector2(info.VisibleRect[curSpriteIdx].width * 0.5f + info.VisibleRect[curSpriteIdx].x, info.VisibleRect[curSpriteIdx].height * 0.5f + info.VisibleRect[curSpriteIdx].y);
			//    var texCenter = new Vector2(smon.texture.width * 0.5f, smon.texture.height * 0.5f);
			//    info.BoxCenterOffset[curSpriteIdx] = visCenter - texCenter;
			//    info.BoxCenterOffset[curSpriteIdx] /= smon.pixelsPerUnit;
			//}

			//// Parse all monster sprites names and set the Lists
			//for (int i = 0; i < monsters.Length; ++i)
			//{
			//    var mon = monsters[i];
			//    var split = mon.name.ToLower().Split('_');
			//    if (split.Length != 2)
			//        throw new Exception($"Monster file '{mon.name}', has an invalid format.");
			//    if (split[0][0] != 'm')
			//        throw new Exception($"Monster file '{mon.name}', is not a Monster.");

			//    var monIDStr = split[0].Substring(1);
			//    parseOK = int.TryParse(monIDStr, out int monID);
			//    if (!parseOK || monID < 0)
			//        throw new Exception($"Monster file '{mon.name}', has an invalid MonsterID.");

			//    Def.MonsterFrame monFrame = Def.MonsterFrame.COUNT;
			//    var facingFrame = split[1];
			//    switch(facingFrame)
			//    {
			//        case "b1":
			//            monFrame = Def.MonsterFrame.BACK_1;
			//            break;
			//        case "b2":
			//            monFrame = Def.MonsterFrame.BACK_2;
			//            break;
			//        case "f1":
			//            monFrame = Def.MonsterFrame.FACE_1;
			//            break;
			//        case "f2":
			//            monFrame = Def.MonsterFrame.FACE_2;
			//            break;
			//        default:
			//            throw new Exception($"Monster file '{mon.name}', has an invalid facing frame format.");
			//    }

			//    MonsterInfo info = null;
			//    if(MonsterInfos.Count <= monID)
			//    {
			//        MonsterInfos.AddRange(Enumerable.Repeat<MonsterInfo>(null, (monID + 1) - MonsterInfos.Count));
			//    }
			//    else
			//    {
			//        info = MonsterInfos[monID];
			//    }
			//    if(info == null)
			//    {
			//        info = new MonsterInfo();
			//        MonsterInfos[monID] = info;
			//        info.MonsterID = monID;
			//        info.Sprites = new Sprite[Def.MonsterFrameCount];
			//        info.LastPixel = new Vector2Int[Def.MonsterFrameCount];
			//        info.VisibleRect = new RectInt[Def.MonsterFrameCount];
			//        info.BoxCenterOffset = new Vector2[Def.MonsterFrameCount];
			//        info.BoxSize = new Vector2[Def.MonsterFrameCount];
			//    }
			//    info.Sprites[(int)monFrame] = mon;
			//    setVisibility(info, (int)monFrame);
			//}

			//// Find the MonsterList asset
			//var textAssets = Resources.LoadAll<TextAsset>("Monsters");
			//TextAsset monsterList = null;
			//foreach(var textAsset in textAssets)
			//{
			//    if(textAsset.name.ToLower() == "monsterlist")
			//    {
			//        monsterList = textAsset;
			//        break;
			//    }
			//}
			//if (monsterList == null)
			//    throw new Exception("Couldn't find MonsterList.txt in Resources/Monsters folder.");

			//var text = monsterList.text;
			//var reader = new StringReader(text);
			//var line = reader.ReadLine();
			//int lineNum = 0;
			//while(line != null)
			//{
			//    var lineBlocks = line.Split(':');
			//    if (lineBlocks.Length != 11)
			//        throw new Exception($"MonsterList has incorrect format, at line: {lineNum}.");
			//    // ID
			//    parseOK = int.TryParse(lineBlocks[0], out int monsterID);
			//    if (!parseOK)
			//        throw new Exception("Couldn't read Monster list ID");

			//    var monster = MonsterInfos[monsterID];
				
			//    // BaseHealth
			//    parseOK = float.TryParse(lineBlocks[1], out float baseHealth);
			//    if (!parseOK)
			//        throw new Exception($"Couldn't read monster list BaseHealth, monster:{monsterID}");

			//    // BaseSpeed
			//    parseOK = float.TryParse(lineBlocks[2], out float baseSpeed);
			//    if (!parseOK)
			//        throw new Exception($"Couldn't read monster list BaseSpeed, monster:{monsterID}");

			//    // AttackRange
			//    parseOK = float.TryParse(lineBlocks[3], out float attackRange);
			//    if (!parseOK)
			//        throw new Exception($"Couldn't read monster list AttackRange, monster:{monsterID}");

			//    // AttackRate
			//    parseOK = float.TryParse(lineBlocks[4], out float attackRate);
			//    if (!parseOK)
			//        throw new Exception($"Couldn't read monster list AttackRate, monster:{monsterID}");

			//    // AttackDamage
			//    parseOK = float.TryParse(lineBlocks[5], out float attackDamage);
			//    if (!parseOK)
			//        throw new Exception($"Couldn't read monster list AttackDamage, monster:{monsterID}");

			//    // DamageType
			//    char damageTypeCHR = lineBlocks[6].ToLower()[0];
			//    DamageType damageType = Def.DamageType.COUNT;
			//    switch (damageTypeCHR)
			//    {
			//        case 'h':
			//            damageType = Def.DamageType.HIT;
			//            break;
			//        case 'c':
			//            damageType = Def.DamageType.CUT;
			//            break;
			//        case 'f':
			//            damageType = Def.DamageType.FIRE;
			//            break;
			//        case 'i':
			//            damageType = Def.DamageType.ICE;
			//            break;
			//        case 'l':
			//            damageType = Def.DamageType.LIGHT;
			//            break;
			//        case 'e':
			//            damageType = Def.DamageType.ELECTRICAL;
			//            break;
			//        case 'a':
			//            damageType = Def.DamageType.ASPHYXIA;
			//            break;
			//        case 'd':
			//            damageType = Def.DamageType.DEPRESSION;
			//            break;
			//        case 'p':
			//            damageType = Def.DamageType.POISON;
			//            break;
			//        case 'q':
			//            damageType = Def.DamageType.QUICKSILVER;
			//            break;
			//        case 'u':
			//            damageType = Def.DamageType.UNAVOIDABLE;
			//            break;
			//        default:
			//            throw new Exception($"Couldn't read Monster list DamageType, monster:{monsterID}");
			//    }

			//    parseOK = int.TryParse(lineBlocks[7], out int aiType);
			//    if (!parseOK || aiType < 0 || aiType >= (int)MonsterAIType.COUNT)
			//        throw new Exception($"Couldn't read Monster list AIType, monster:{monsterID}");

			//    var teamCHR = lineBlocks[8].ToLower()[0];
			//    MonsterTeam team = MonsterTeam.COUNT;
			//    switch (teamCHR)
			//    {
			//        case 'a':
			//            team = MonsterTeam.ATeam;
			//            break;
			//        case 'b':
			//            team = MonsterTeam.BTeam;
			//            break;
			//        case 'o':
			//            team = MonsterTeam.OddTeam;
			//            break;
			//        default:
			//            throw new Exception($"Couldn't read Monster list Team, monster:{monsterID}");
			//    }
			//    parseOK = float.TryParse(lineBlocks[9], out float scale);
			//    if (!parseOK)
			//        throw new Exception($"Couldn't read monster list Scale, monster:{monsterID}");

			//    var name = lineBlocks[10];

			//    monster.AIType = (MonsterAIType)aiType;
			//    monster.Team = team;
			//    monster.BaseHealth = baseHealth;
			//    monster.BaseSpeed = baseSpeed;
			//    monster.HearingRange = 5.0f;
			//    monster.SightAngle = 60.0f;
			//    monster.SightRange = 7.0f;
			//    monster.AttackRange = attackRange;
			//    monster.AttackRate = attackRate;
			//    monster.AttackDamage = attackDamage;
			//    monster.DmgType = damageType;
			//    monster.Name = name;
			//    monster.Class = Type.GetType("Assets.AI." + monster.Name);
			//    monster.SpriteScale = scale;
			//    MonsterDict.Add(name, monster.MonsterID);

			//    line = reader.ReadLine();
			//    ++lineNum;
			//}
		}
		public static void Deinit()
		{

		}
		//public static MonsterScript AddMonsterComponent(GameObject gameObject, int monsterID)
		//{
		//	var monster = MonsterFamilies[monsterID];
		//	if (monster.Class == null)
		//		return gameObject.AddComponent<AI.TheAngel>();
		//	return (MonsterScript)gameObject.AddComponent(monster.Class);
		//}
	}
}
