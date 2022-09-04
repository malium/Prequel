/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets
{
#if UNITY_EDITOR
	public static class AssetUpgrader
	{
		[MenuItem("Tools/AssetUpgrader/Upgrade Materials")]
		static void UpgradeMaterials()
		{
			var materialList = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/Floor/MaterialList.txt");
			if (materialList == null)
			{
				EditorUtility.DisplayDialog("AssetUpgrader", "Couldn't find 'Assets/Resources/Floor/MaterialList.txt'.", "OK");
				return;
			}
			var text = materialList.text;
			var reader = new StringReader(text);
			var line = reader.ReadLine();
			int lineNum = 0;
			while(line != null)
			{
				var lineBlocks = line.Split(':');
				if(lineBlocks.Length != 4)
				{
					EditorUtility.DisplayDialog("AssetUpgrader", $"MaterialList has incorrect format at line:{lineNum}", "OK");
					return;
				}

				var baseFamily = ScriptableObject.CreateInstance<MaterialFamilyInfo>();

				bool parseOK = int.TryParse(lineBlocks[0], out int materialFamilyID);
				if (!parseOK)
				{
					EditorUtility.DisplayDialog("AssetUpgrader", $"MaterialList has incorrect MaterialFamilyID at line:{lineNum}", "OK");
					return;
				}

				baseFamily.ID = materialFamilyID;

				var colorVariations = lineBlocks[1].Split('.');
				var cvLength = colorVariations[0].Length == 0 ? 0 : colorVariations.Length;
				var nCV = new string[cvLength + 1];
				for (int i = 0; i < cvLength; ++i)
					nCV[i + 1] = colorVariations[i];
				colorVariations = nCV;

				var materialModeStr = lineBlocks[2].ToLower();
				var matMode = materialModeStr[0];

				var materialMode = Def.MaterialMode.Default;
				switch (matMode)
				{
					case 'f':
						{
							materialMode = Def.MaterialMode.Fade;
						}
						break;
					case 't':
						{
							materialMode = Def.MaterialMode.Transparent;
						}
						break;
					case 'c':
						{
							materialMode = Def.MaterialMode.Cutout;
						}
						break;
					case 'd':break;
					default:
						EditorUtility.DisplayDialog("AssetUpgrader", $"MaterialList has incorrect MaterialMode at line:{lineNum}", "OK");
						return;
				}
				baseFamily.MaterialMode = materialMode;

				var names = lineBlocks[3].Split(',');
				if (colorVariations.Length != names.Length)
				{
					EditorUtility.DisplayDialog("AssetUpgrader", "Error parsing the MaterialList, ColorVariation length mismatch the Name length.", "OK");
					return;
				}

				for (int i = 1; i < names.Length; ++i) names[i] = names[i].Trim();

				baseFamily.FamilyName = names[0];
				string familyIDStr = String.Format("{0:D3}", materialFamilyID);
				AssetDatabase.CreateAsset(baseFamily,
					"Assets/Resources/MaterialFamilies/" + familyIDStr + ".asset");

				Vector4 CSHB = Vector4.zero;
				for (int i = 1; i < names.Length; ++i)
				{
					var variation = colorVariations[i].Split('_');
					if (variation.Length != 4)
					{
						EditorUtility.DisplayDialog("AssetUpgrader", $"Invalid Variation:{i}, in MaterialList line:{lineNum}", "OK");
						return;
					}

					for (int j = 0; j < variation.Length; ++j)
					{
						parseOK = float.TryParse(variation[j], out float varVal);
						if (!parseOK)
						{
							EditorUtility.DisplayDialog("AssetUpgrader", $"Couldn't parse Variation{i}, CSHB:{j}, in MaterialList line:{lineNum}", "OK");
							return;
						}
						CSHB[j] = varVal;
					}

					CSHB.x *= 1.4f;
					CSHB.w *= 1.5f;
					var coloredFamily = ScriptableObject.CreateInstance<MaterialFamilyInfo>();
					coloredFamily.ID = materialFamilyID;
					coloredFamily.FamilyName = names[i];
					coloredFamily.MaterialMode = materialMode;
					coloredFamily.CSHB = CSHB;
					AssetDatabase.CreateAsset(coloredFamily,
						"Assets/Resources/MaterialFamilies/" + familyIDStr + "_" + coloredFamily.FamilyName + ".asset");
				}

				++lineNum;
				line = reader.ReadLine();
			}
			EditorUtility.DisplayDialog("AssetUpgrader", "Upgrade finished", "OK");
		}


		[MenuItem("Tools/AssetUpgrader/Upgrade Props")]
		static void UpgradeProps()
		{
			var propList = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/Props/PropList.txt");
			if (propList == null)
			{
				EditorUtility.DisplayDialog("AssetUpgrader", "Couldn't find 'Assets/Resources/Props/PropList.txt'.", "OK");
				return;
			}
			var props = Resources.LoadAll<Sprite>("Props");
			var familyDict = new List<List<PropInfo>>(props.Length);
			{
				//void ReadSprite(PropInfo info)
				//{
				//    var topPixel = new Vector2Int(-1, -1);
				//    var bottomPixel = new Vector2Int(-1, -1);
				//    var LeftPixel = new Vector2Int(-1, -1);
				//    var RightPixel = new Vector2Int(-1, -1);

				//    var height25 = info.Texture.height / 4;

				//    var damageColors = new List<Color32>((255 / MaxColorDistance) * 3);
				//    var damageColorProbs = new List<float>(damageColors.Capacity);

				//    int colorCount = 0;
				//    var texColors = info.Texture.GetPixels32();
				//    for (int y = 0; y < info.Texture.height; ++y)
				//    {
				//        int hOffset = y * info.Texture.width;
				//        for (int x = 0; x < info.Texture.width; ++x)
				//        {
				//            var color = texColors[hOffset + x];
				//            if (color.a == 0)
				//                continue;

				//            ++colorCount;
				//            // Damage check
				//            int damageIdx = -1;
				//            for (int i = 0; i < damageColors.Count; ++i)
				//            {
				//                var dColor = damageColors[i];
				//                if (Vector3Int.Distance(new Vector3Int(dColor.r, dColor.g, dColor.b), new Vector3Int(color.r, color.g, color.b)) <= MaxColorDistance)
				//                {
				//                    damageIdx = i;
				//                    break;
				//                }
				//            }
				//            if (damageIdx == -1)
				//            {
				//                damageColors.Add(color);
				//                damageColorProbs.Add(1f);
				//            }
				//            else
				//            {
				//                damageColorProbs[damageIdx] = damageColorProbs[damageIdx] + 1f;
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


				//    for (int i = 0; i < damageColorProbs.Count; ++i)
				//    {
				//        damageColorProbs[i] = damageColorProbs[i] / colorCount;
				//    }
				//    GameUtils.Sort(damageColorProbs, (int prev, int cur) => { var tempColor = damageColors[cur]; damageColors[cur] = damageColors[prev]; damageColors[prev] = tempColor; });

				//    int firstPixel25 = -1, lastPixel25 = -1;

				//    var prevPixel = texColors[height25 * info.Texture.width];
				//    var curPixel = texColors[height25 * info.Texture.width + 1];

				//    for (int j = 1; j < (info.Texture.width - 1); ++j)
				//    {
				//        var nextPixel = texColors[height25 * info.Texture.width + j + 1];
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
				//    info.VisibleRect = new RectInt(LeftPixel.x, bottomPixel.y, RightPixel.x - LeftPixel.x, topPixel.y - bottomPixel.y);
				//    info.LastPixel = bottomPixel;
				//    info.BoxSize = new Vector2(info.VisibleRect.width, info.VisibleRect.height) / info.PropSprite.pixelsPerUnit;

				//    var visCenter = new Vector2(info.VisibleRect.width * 0.5f + info.VisibleRect.x, info.VisibleRect.height * 0.5f + info.VisibleRect.y);
				//    var texCenter = new Vector2(info.Texture.width * 0.5f, info.Texture.height * 0.5f);
				//    info.BoxCenterOffset = visCenter - texCenter;
				//    info.BoxCenterOffset /= info.PropSprite.pixelsPerUnit;
				//    info.BoxWidth = (lastPixel25 - firstPixel25) / (float)info.Texture.width;

				//    info.DamageColors = damageColors;
				//    info.DamageColorProbs = damageColorProbs;
				//}
			}
			for (int i = 0; i < props.Length; ++i)
			{
				var prop = props[i];
				var split = prop.name.ToLower().Split('_');
				if (split.Length != 2)
				{
					EditorUtility.DisplayDialog("AssetUpgrader", "Prop sprite, has an invalid name:'" + prop.name + "'.", "OK");
					return;
				}
				if (split[0][0] != 'p')
				{
					EditorUtility.DisplayDialog("AssetUpgrader", "Prop sprite '" + prop.name + "', is not a Prop sprite.", "OK");
					return;
				}

				var familyIDStr = split[0].Substring(1);
				bool parseOK = int.TryParse(familyIDStr, out int familyID);
				if (!parseOK)
				{
					EditorUtility.DisplayDialog("AssetUpgrader", "Prop sprite '" + prop.name + "', has an invalid FamiliyID.", "OK");
					return;
				}

				if (split[1][0] != 'v')
				{
					EditorUtility.DisplayDialog("AssetUpgrader", "Prop sprite '" + prop.name + "', has an invalid version format.", "OK");
					return;
				}

				var versionStr = split[1].Substring(1);
				parseOK = int.TryParse(versionStr, out int version);
				if (!parseOK)
				{
					EditorUtility.DisplayDialog("AssetUpgrader", "Prop sprite '" + prop.name + "', has an invalid version.", "OK");
					return;
				}
				if(familyDict.Count <= familyID)
				{
					familyDict.AddRange(Enumerable.Repeat<List<PropInfo>>(null, (familyID + 1) - familyDict.Count));
				}
				var family = familyDict[familyID];
				if(family == null)
				{
					family = new List<PropInfo>(1);
					familyDict[familyID] = family;
				}
				if(family.Count <= version)
				{
					family.AddRange(Enumerable.Repeat<PropInfo>(null, (version + 1) - family.Count));
				}
				var pInfo = family[version];
				if(pInfo == null)
				{
					pInfo = ScriptableObject.CreateInstance<PropInfo>();
					family[version] = pInfo;
				}
				pInfo.PropSprite = prop;
				//pInfo.Texture = prop.texture;
				//pInfo.Pivot = prop.pivot;
				//pInfo.LastPixel = Vector2Int.zero;
				pInfo.SpriteScale = 1f;
				pInfo.HasLighting = false;
				pInfo.LightColor = new Color32(255, 255, 255, 255);
				pInfo.LightHeight = 1f;
				pInfo.LightIntensity = 1f;
				pInfo.LightRange = 10f;
				pInfo.HasShadow = true;
				//ReadSprite(pInfo);
			}

			var text = propList.text;
			var reader = new StringReader(text);
			var line = reader.ReadLine();
			int lineNum = 0;
			while (line != null)
			{
				var lineBlocks = line.Split(':');
				if (lineBlocks.Length != 5)
				{
					EditorUtility.DisplayDialog("AssetUpgrader", $"PropList has incorrect format at line:{lineNum}.", "OK");
					return;
				}

				int curLineBlock = 0;

				bool parseOK = int.TryParse(lineBlocks[curLineBlock], out int familyID);
				if (!parseOK)
				{
					EditorUtility.DisplayDialog("AssetUpgrader", $"PropList has an incorrect FamilyID at line:{lineNum}.", "OK");
					return;
				}

				++curLineBlock;

				var family = familyDict[familyID];
				var shdw = lineBlocks[curLineBlock].ToLower()[0];
				++curLineBlock;
				var lights = lineBlocks[curLineBlock].Split('.');
				++curLineBlock;

				if (lights.Length != family.Count)
				{
					EditorUtility.DisplayDialog("AssetUpgrader", $"Error in PropList, Prop '{lineNum}' has different lights '{lights.Length}' than props '{family.Count}'.", "OK");
					return;
				}
				for (int i = 0; i < lights.Length; ++i)
				{
					var lightInfo = lights[i].Split('_');
					var le = lightInfo[0][0];
					bool isLightEnabled = le == 'y' || le == 'Y';
					var prop = family[i];
					prop.HasShadow = shdw == 'y';
					if (!isLightEnabled)
					{
						continue;
					}
					prop.HasLighting = true;

					parseOK = float.TryParse(lightInfo[1], out float lightHeight);
					if (!parseOK)
					{
						EditorUtility.DisplayDialog("AssetUpgrader", $"Error in PropList, Couldn't parse prop light info, at line:{lineNum}", "OK");
						return;
					}
					prop.LightHeight = lightHeight;

					parseOK = float.TryParse(lightInfo[2], out float lightRange);
					if (!parseOK)
					{
						EditorUtility.DisplayDialog("AssetUpgrader", $"Error in PropList, Couldn't parse prop light info, at line:{lineNum}", "OK");
						return;
					}
					prop.LightRange = lightRange;

					parseOK = float.TryParse(lightInfo[3], out float lightIntensity);
					if (!parseOK)
					{
						EditorUtility.DisplayDialog("AssetUpgrader", $"Error in PropList, Couldn't parse prop light info, at line:{lineNum}", "OK");
						return;
					}
					prop.LightIntensity = lightIntensity;

					parseOK = byte.TryParse(lightInfo[4].Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out byte r);
					if (!parseOK)
					{
						EditorUtility.DisplayDialog("AssetUpgrader", $"Error in PropList, Couldn't parse prop light info, at line:{lineNum}", "OK");
						return;
					}

					parseOK = byte.TryParse(lightInfo[4].Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out byte g);
					if (!parseOK)
					{
						EditorUtility.DisplayDialog("AssetUpgrader", $"Error in PropList, Couldn't parse prop light info, at line:{lineNum}", "OK");
						return;
					}

					parseOK = byte.TryParse(lightInfo[4].Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out byte b);
					if (!parseOK)
					{
						EditorUtility.DisplayDialog("AssetUpgrader", $"Error in PropList, Couldn't parse prop light info, at line:{lineNum}", "OK");
						return;
					}

					prop.LightColor = new Color32(r, g, b, 255);
				}

				var scales = lineBlocks[curLineBlock].Split('.');
				++curLineBlock;
				if (scales.Length != family.Count)
				{
					EditorUtility.DisplayDialog("AssetUpgrader", $"Error in PropList, Prop '{lineNum}' has different scales '{scales.Length}' than props '{family.Count}'.", "OK");
					return;
				}

				for (int i = 0; i < scales.Length; ++i)
				{
					parseOK = float.TryParse(scales[i], out float scale);
					if (!parseOK || (parseOK && scale <= 0.0f))
					{
						EditorUtility.DisplayDialog("AssetUpgrader", $"Error in PropList, Prop '{lineNum}', has an invalid scale {i}.", "OK");
						return;
					}

					family[i].SpriteScale = scale;
				}
				var name = lineBlocks[curLineBlock].Trim();
				++curLineBlock;
				for (int i = 0; i < family.Count; ++i)
				{
					var info = family[i];
					info.FamilyName = name;
					info.BaseHealth = 100f;
				}
				++lineNum;
				line = reader.ReadLine();
			}

			for(int i = 0; i < familyDict.Count; ++i)
			{
				var family = familyDict[i];
				string familyIDStr = String.Format("{0:D4}", i);
				for (int j = 0; j < family.Count; ++j)
				{
					string versionIDStr = String.Format("{0:D2}", j);
					family[j].OnUpdateInfo();
					AssetDatabase.CreateAsset(family[j],
						"Assets/Resources/PropInfos/" + familyIDStr + "_" + versionIDStr + ".asset");
				}                
			}
			EditorUtility.DisplayDialog("AssetUpgrader", "Upgrade finished", "OK");
		}
		[MenuItem("Tools/AssetUpgrader/Upgrade Monsters")]
		static void UpgradeMonsters()
		{
			var monsterList = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/Monsters/MonsterList.txt");
			if (monsterList == null)
			{
				EditorUtility.DisplayDialog("AssetUpgrader", "Couldn't find 'Assets/Resources/Monsters/MonsterList.txt'.", "OK");
				return;
			}
			var monsters = Resources.LoadAll<Sprite>("Monsters");
			var families = new List<MonsterFamily>(monsters.Length);
			{
				//void setVisibility(MonsterFamily family, int curSpriteIdx)
				//{
				//    var topPixel = new Vector2Int(-1, -1);
				//    var bottomPixel = new Vector2Int(-1, -1);
				//    var LeftPixel = new Vector2Int(-1, -1);
				//    var RightPixel = new Vector2Int(-1, -1);
				//    var smon = family.Frames[curSpriteIdx];

				//    var colors = smon.texture.GetPixels32();

				//    for (int y = 0; y < smon.texture.height; ++y)
				//    {
				//        int yOffset = smon.texture.width * y;
				//        for (int x = 0; x < smon.texture.width; ++x)
				//        {
				//            var color = colors[yOffset + x];
				//            if (color.a == 0)
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

				//    family.VisibleRect[curSpriteIdx] = new RectInt(LeftPixel.x, bottomPixel.y, RightPixel.x - LeftPixel.x, topPixel.y - bottomPixel.y);
				//    family.LastPixel[curSpriteIdx] = bottomPixel;
				//    family.BoxSize[curSpriteIdx] = new Vector2(family.VisibleRect[curSpriteIdx].width, family.VisibleRect[curSpriteIdx].height) / smon.pixelsPerUnit;

				//    var visCenter = new Vector2(family.VisibleRect[curSpriteIdx].width * 0.5f + family.VisibleRect[curSpriteIdx].x, family.VisibleRect[curSpriteIdx].height * 0.5f + family.VisibleRect[curSpriteIdx].y);
				//    var texCenter = new Vector2(smon.texture.width * 0.5f, smon.texture.height * 0.5f);
				//    family.BoxCenterOffset[curSpriteIdx] = visCenter - texCenter;
				//    family.BoxCenterOffset[curSpriteIdx] /= smon.pixelsPerUnit;
				//}
			}
			bool parseOK;
			// Parse all monster sprites names and set the Lists
			for (int i = 0; i < monsters.Length; ++i)
			{
				var mon = monsters[i];
				var split = mon.name.ToLower().Split('_');
				if (split.Length != 2)
				{
					EditorUtility.DisplayDialog("AssetUpgrader", "Monster file '" + mon.name + "', has an invalid format.", "OK");
					return;
				}
				if (split[0][0] != 'm')
				{
					EditorUtility.DisplayDialog("AssetUpgrader", "Monster file '" + mon.name + "', is not a Monster.", "OK");
					return;
				}

				var monIDStr = split[0].Substring(1);
				parseOK = int.TryParse(monIDStr, out int monID);
				if (!parseOK || monID < 0)
				{
					EditorUtility.DisplayDialog("AssetUpgrader", "Monster file '" + mon.name + "', has an invalid MonsterID.", "OK");
					return;
				}

				var facingFrame = split[1];

				Def.MonsterFrame monFrame;
				switch (facingFrame)
				{
					case "b1":
						monFrame = Def.MonsterFrame.BACK_1;
						break;
					case "b2":
						monFrame = Def.MonsterFrame.BACK_2;
						break;
					case "f1":
						monFrame = Def.MonsterFrame.FACE_1;
						break;
					case "f2":
						monFrame = Def.MonsterFrame.FACE_2;
						break;
					case "fa":
						monFrame = Def.MonsterFrame.FACE_ATTACK;
						break;
					case "ba":
						monFrame = Def.MonsterFrame.BACK_ATTACK;
						break;
					default:
						EditorUtility.DisplayDialog("AssetUpgrader", "Monster file '" + mon.name + "', has an invalid facing frame format.", "OK");
						return;
				}

				if (families.Count <= monID)
				{
					families.AddRange(Enumerable.Repeat<MonsterFamily>(null, (monID + 1) - families.Count));
				}
				var family = families[monID];
				if (family == null)
				{
					family = ScriptableObject.CreateInstance<MonsterFamily>();
					families[monID] = family;
					family.Frames = new Sprite[Def.MonsterFrameCount];
				}
				if(family.Frames.Length < Def.MonsterFrameCount)
				{
					var arr = new Sprite[Def.MonsterFrameCount];
					family.Frames.CopyTo(arr, 0);
					family.Frames = arr;
				}	
				family.Frames[(int)monFrame] = mon;
				//family.Textures[(int)monFrame] = mon.texture;
				//family.Pivots[(int)monFrame] = mon.pivot;
				//setVisibility(family, (int)monFrame);
			}

			int firstFamily = 0;
			if (false)
			{
				//var text = monsterList.text;
				//var reader = new StringReader(text);
				//var line = reader.ReadLine();
				//int lineNum = 0;
				//while (line != null)
				//{
				//	var lineBlocks = line.Split(':');
				//	if (lineBlocks.Length != 11)
				//	{
				//		EditorUtility.DisplayDialog("AssetUpgrader", $"MonsterList has incorrect format, at line: {lineNum}.", "OK");
				//		return;
				//	}
				//	// ID
				//	parseOK = int.TryParse(lineBlocks[0], out int monsterID);
				//	if (!parseOK)
				//	{
				//		EditorUtility.DisplayDialog("AssetUpgrader", $"MonsterList has incorrect ID, at line: {lineNum}.", "OK");
				//		return;
				//	}
				//
				//	var monster = families[monsterID];
				//
				//	// BaseHealth
				//	parseOK = float.TryParse(lineBlocks[1], out float baseHealth);
				//	if (!parseOK)
				//	{
				//		EditorUtility.DisplayDialog("AssetUpgrader", $"Couldn't read monster list BaseHealth, monster:{monsterID}.", "OK");
				//		return;
				//	}
				//
				//	// BaseSpeed
				//	parseOK = float.TryParse(lineBlocks[2], out float baseSpeed);
				//	if (!parseOK)
				//	{
				//		EditorUtility.DisplayDialog("AssetUpgrader", $"Couldn't read monster list BaseSpeed, monster:{monsterID}.", "OK");
				//		return;
				//	}
				//
				//	// AttackRange
				//	parseOK = float.TryParse(lineBlocks[3], out float attackRange);
				//	if (!parseOK)
				//	{
				//		EditorUtility.DisplayDialog("AssetUpgrader", $"Couldn't read monster list AttackRange, monster:{monsterID}.", "OK");
				//		return;
				//	}
				//
				//	// AttackRate
				//	parseOK = float.TryParse(lineBlocks[4], out float attackRate);
				//	if (!parseOK)
				//	{
				//		EditorUtility.DisplayDialog("AssetUpgrader", $"Couldn't read monster list AttackRate, monster:{monsterID}.", "OK");
				//		return;
				//	}
				//
				//	// AttackDamage
				//	parseOK = float.TryParse(lineBlocks[5], out float attackDamage);
				//	if (!parseOK)
				//	{
				//		EditorUtility.DisplayDialog("AssetUpgrader", $"Couldn't read monster list AttackDamage, monster:{monsterID}.", "OK");
				//		return;
				//	}
				//
				//	// DamageType
				//	char damageTypeCHR = lineBlocks[6].ToLower()[0];
				//	Def.DamageType damageType;
				//	switch (damageTypeCHR)
				//	{
				//		case 'h':
				//			damageType = Def.DamageType.HIT;
				//			break;
				//		case 'c':
				//			damageType = Def.DamageType.CUT;
				//			break;
				//		case 'f':
				//			damageType = Def.DamageType.FIRE;
				//			break;
				//		case 'i':
				//			damageType = Def.DamageType.ICE;
				//			break;
				//		case 'l':
				//			damageType = Def.DamageType.LIGHT;
				//			break;
				//		case 'e':
				//			damageType = Def.DamageType.ELECTRICAL;
				//			break;
				//		case 'a':
				//			damageType = Def.DamageType.ASPHYXIA;
				//			break;
				//		case 'd':
				//			damageType = Def.DamageType.DEPRESSION;
				//			break;
				//		case 'p':
				//			damageType = Def.DamageType.POISON;
				//			break;
				//		case 'q':
				//			damageType = Def.DamageType.QUICKSILVER;
				//			break;
				//		case 'u':
				//			damageType = Def.DamageType.UNAVOIDABLE;
				//			break;
				//		default:
				//			EditorUtility.DisplayDialog("AssetUpgrader", $"Couldn't read Monster list DamageType, monster:{monsterID}.", "OK");
				//			return;
				//	}
				//
				//	parseOK = int.TryParse(lineBlocks[7], out int aiType);
				//	if (!parseOK || aiType < 0 || aiType >= (int)MonsterAIType.COUNT)
				//	{
				//		EditorUtility.DisplayDialog("AssetUpgrader", $"Couldn't read Monster list AIType, monster:{monsterID}.", "OK");
				//		return;
				//	}
				//
				//	//var teamCHR = lineBlocks[8].ToLower()[0];
				//	//MonsterTeam team = MonsterTeam.COUNT;
				//	//switch (teamCHR)
				//	//{
				//	//    case 'a':
				//	//        team = MonsterTeam.ATeam;
				//	//        break;
				//	//    case 'b':
				//	//        team = MonsterTeam.BTeam;
				//	//        break;
				//	//    case 'o':
				//	//        team = MonsterTeam.OddTeam;
				//	//        break;
				//	//    default:
				//	//        EditorUtility.DisplayDialog("AssetUpgrader", $"Couldn't read Monster list Team, monster:{monsterID}.", "OK");
				//	//        return;
				//	//}
				//	parseOK = float.TryParse(lineBlocks[9], out float scale);
				//	if (!parseOK)
				//	{
				//		EditorUtility.DisplayDialog("AssetUpgrader", $"Couldn't read monster list Scale, monster:{monsterID}.", "OK");
				//		return;
				//	}
				//
				//	var name = lineBlocks[10];
				//
				//	monster.AIType = (Def.MonsterAIType)aiType;
				//	monster.BaseHealth = baseHealth;
				//	monster.BaseSpeed = baseSpeed;
				//	monster.HearingRange = 5.0f;
				//	monster.SightAngle = 60.0f;
				//	monster.SightRange = 7.0f;
				//	monster.AttackRange = attackRange;
				//	monster.AttackRate = attackRate;
				//	monster.AttackDamage = attackDamage;
				//	monster.DamageType = damageType;
				//	monster.Name = name;
				//	monster.Class = Type.GetType("Assets.AI." + monster.Name);
				//	monster.SpriteScale = scale;
				//	monster.FriendlyFamilies = new MonsterFamily[0];
				//
				//	line = reader.ReadLine();
				//	++lineNum;
				//}
			}
			else
			{
				var createdFamilies = Resources.LoadAll<MonsterFamily>("MonsterFamilies");
				firstFamily = createdFamilies.Length;

				var text = monsterList.text;
				var reader = new StringReader(text);
				var line = reader.ReadLine();
				int lineNum = 0;
				while (line != null)
				{
					var lineBlocks = line.Split(':');
					// ID
					parseOK = int.TryParse(lineBlocks[0], out int monsterID);
					if (!parseOK)
					{
						EditorUtility.DisplayDialog("AssetUpgrader", $"MonsterList has incorrect ID, at line: {lineNum}.", "OK");
						return;
					}
					if(monsterID < firstFamily)
					{
						line = reader.ReadLine();
						++lineNum;
						continue;
					}
					var monster = families[monsterID];
					monster.Name = lineBlocks[2].Trim();


					var cleanName = GameUtils.RemoveWhitespaces(monster.Name);

					//var aiFile = File.Create("Assets/AI/" + monster.name + ".cs");
					string aiFileContents = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AI
{
	public class " + cleanName + @" : MonsterScript
	{
		static int MonsterTypeID = 0;
		//static int AttackVFXTypeID = 0;
		static float AttackHeight = -1.0f;

		public override void InitMonster()
		{
			if (MonsterTypeID < 1)
			{
				MonsterTypeID = Monsters.FamilyDict[""" + monster.Name + @"""];

				//var monster = Monsters.MonsterFamilies[MonsterTypeID];
			}
			SetMonster(MonsterTypeID);
			
			if (AttackHeight < 0.0f)
			{
				AttackHeight = (m_Info.Frames[0].rect.height / m_Info.Frames[0].pixelsPerUnit) * m_Info.SpriteScale;
			}
		}
		public override void Attack(LivingEntity target, Vector3 targetPos)
		{

		}
	}
}
";
					File.WriteAllText("Assets/AI/" + cleanName + ".cs", aiFileContents);
					monster.AIType = Def.MonsterAIType.NO_AI;
					monster.BaseHealth = 100f;
					monster.BaseSpeed = 1f;
					monster.HearingRange = 5.0f;
					monster.SightAngle = 60.0f;
					monster.SightRange = 7.0f;
					monster.AttackRange = 10f;
					monster.AttackRate = 1f;
					monster.AttackDamage = 10f;
					monster.DamageType = Def.DamageType.HIT;
					//monster.Class = Type.GetType("Assets.AI." + cleanName);
					monster.SpriteScale = 1f;
					monster.FriendlyFamilies = new MonsterFamily[0];

					line = reader.ReadLine();
					++lineNum;
				}
			}

			for(int i = firstFamily; i < families.Count; ++i)
			{
				var monster = families[i];
				
				monster.OnUpdateInfo();
				string familyIDStr = String.Format("{0:D4}", i);
				AssetDatabase.CreateAsset(monster, "Assets/Resources/MonsterFamilies/" + familyIDStr + ".asset");
			}

			EditorUtility.DisplayDialog("AssetUpgrader", "Upgrade finished", "OK");
		}
		[MenuItem("Tools/AssetUpgrader/Upgrade Monsters AI")]
		static void UpgradeMonstersAI()
		{
			var monsters = Resources.LoadAll<MonsterFamily>("MonsterFamilies");
			for(int i = 0; i < monsters.Length; ++i)
			{
				var monster = monsters[i];
				var cleanName = GameUtils.RemoveWhitespaces(monster.Name);
				monster.Class = Type.GetType("Assets.AI." + cleanName);
			}
			AssetDatabase.SaveAssets();
			EditorUtility.DisplayDialog("AssetUpgrader", "Upgrade finished", "OK");
		}
		[MenuItem("Tools/AssetUpgrader/Upgrade Monster Sprites")]
		static void UpgradeMonstersSprites()
		{
			var monsters = Resources.LoadAll<Sprite>("Monsters");
			var families = Resources.LoadAll<MonsterFamily>("MonsterFamilies");

			bool parseOK;
			for(int i = 0; i < monsters.Length; ++i)
			{
				var mon = monsters[i];
				var split = mon.name.ToLower().Split('_');
				if (split.Length != 2)
				{
					EditorUtility.DisplayDialog("AssetUpgrader", "Monster file '" + mon.name + "', has an invalid format.", "OK");
					return;
				}
				if (split[0][0] != 'm')
				{
					EditorUtility.DisplayDialog("AssetUpgrader", "Monster file '" + mon.name + "', is not a Monster.", "OK");
					return;
				}

				var monIDStr = split[0].Substring(1);
				parseOK = int.TryParse(monIDStr, out int monID);
				if (!parseOK || monID < 0)
				{
					EditorUtility.DisplayDialog("AssetUpgrader", "Monster file '" + mon.name + "', has an invalid MonsterID.", "OK");
					return;
				}

				var facingFrame = split[1];

				Def.MonsterFrame monFrame;
				switch (facingFrame)
				{
					case "b1":
						monFrame = Def.MonsterFrame.BACK_1;
						break;
					case "b2":
						monFrame = Def.MonsterFrame.BACK_2;
						break;
					case "f1":
						monFrame = Def.MonsterFrame.FACE_1;
						break;
					case "f2":
						monFrame = Def.MonsterFrame.FACE_2;
						break;
					case "fa":
						monFrame = Def.MonsterFrame.FACE_ATTACK;
						break;
					case "ba":
						monFrame = Def.MonsterFrame.BACK_ATTACK;
						break;
					default:
						EditorUtility.DisplayDialog("AssetUpgrader", "Monster file '" + mon.name + "', has an invalid facing frame format.", "OK");
						return;
				}
				var family = families[monID];
				if(family.Frames.Length < Def.MonsterFrameCount)
				{
					var temp = new Sprite[Def.MonsterFrameCount];
					family.Frames.CopyTo(temp, 0);
					family.Frames = temp;
				}	
				family.Frames[(int)monFrame] = mon;
				if (monFrame == Def.MonsterFrame.BACK_1 && family.Frames[(int)Def.MonsterFrame.BACK_ATTACK] == null)
					family.Frames[(int)Def.MonsterFrame.BACK_ATTACK] = mon;
				else if (monFrame == Def.MonsterFrame.FACE_1 && family.Frames[(int)Def.MonsterFrame.FACE_ATTACK] == null)
					family.Frames[(int)Def.MonsterFrame.FACE_ATTACK] = mon;
			}

			for(int i = 0; i < families.Length; ++i)
			{
				var family = families[i];

				family.OnUpdateInfo();
				//string familyPath = "Assets/Resources/MonsterFamilies/" + String.Format("{0:D4}", i) + ".asset";
				//if(AssetDatabase.)
				//AssetDatabase.CreateAsset(family, "Assets/Resources/MonsterFamilies/" + familyIDStr + ".asset");
			}
			AssetDatabase.SaveAssets();

			EditorUtility.DisplayDialog("AssetUpgrader", "Upgrade finished", "OK");
		}
		static readonly string UINameToUpdate = "StrucEditHelp";
		static void HandleUIChild(GameObject go)
		{
			for (int i = 0; i < go.transform.childCount; ++i)
			{
				HandleUIChild(go.transform.GetChild(i).gameObject);
			}
			if (go.TryGetComponent(out UnityEngine.UI.Text text))
			{
				var value = text.text;
				var alignment = text.alignment;
				var horizontalOverflow = text.horizontalOverflow;
				var bestFit = text.resizeTextForBestFit;
				var richText = text.supportRichText;
				var fontSize = text.fontSize;
				var color = text.color;
				var minSize = text.resizeTextMinSize;
				var maxSize = text.resizeTextMaxSize;

				GameObject.DestroyImmediate(text);
				//var tmp = ObjectFactory.AddComponent(go, typeof(TMPro.TMP_Text)) as TMPro.TMP_Text;
				var tmp = go.AddComponent<TMPro.TextMeshProUGUI>();
				tmp.text = value;
				switch (alignment)
				{
					case TextAnchor.UpperLeft:
						tmp.alignment = TMPro.TextAlignmentOptions.TopLeft;
						break;
					case TextAnchor.UpperCenter:
						tmp.alignment = TMPro.TextAlignmentOptions.Top;
						break;
					case TextAnchor.UpperRight:
						tmp.alignment = TMPro.TextAlignmentOptions.TopRight;
						break;
					case TextAnchor.MiddleLeft:
						tmp.alignment = TMPro.TextAlignmentOptions.Left;
						break;
					case TextAnchor.MiddleCenter:
						tmp.alignment = TMPro.TextAlignmentOptions.Center;
						break;
					case TextAnchor.MiddleRight:
						tmp.alignment = TMPro.TextAlignmentOptions.Right;
						break;
					case TextAnchor.LowerLeft:
						tmp.alignment = TMPro.TextAlignmentOptions.BottomLeft;
						break;
					case TextAnchor.LowerCenter:
						tmp.alignment = TMPro.TextAlignmentOptions.Bottom;
						break;
					case TextAnchor.LowerRight:
						tmp.alignment = TMPro.TextAlignmentOptions.BottomRight;
						break;
				}
				if (horizontalOverflow == HorizontalWrapMode.Wrap)
					tmp.enableWordWrapping = true;

				tmp.fontSize = fontSize;
				tmp.enableAutoSizing = bestFit;
				tmp.fontSizeMin = minSize;
				tmp.fontSizeMax = maxSize;
				tmp.color = color;
				tmp.richText = richText;
			}
		}
		[MenuItem("Tools/UpdateUI")]
		static void UpdateUI()
		{
			var go = GameObject.Find(UINameToUpdate);
			if(go == null)
			{
				EditorUtility.DisplayDialog("Update UI", "Couldn't find the UI GameObject " + UINameToUpdate, "OK");
				return;
			}
			HandleUIChild(go);
		}
	}
#endif
}