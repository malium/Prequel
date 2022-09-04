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
using UnityEngine;

namespace Assets
{
	[Serializable]
	public class MaterialPart
	{
		public MaterialSet FullMaterial;
		public Def.BlockType Type;
		public Def.BlockMeshType Where;
		public Material Mat;
		public Def.StairType StairType;
	}

	[Serializable]
	public class MaterialSet
	{
		[SerializeReference]
		public MaterialPart TopPart;
		[SerializeReference]
		public MaterialPart BottomPart;
		public MaterialFamily Family;
	}

	[Serializable]
	public class MaterialFamily
	{
		[SerializeReference] public MaterialSet[] NormalMaterials;
		[SerializeReference] public MaterialSet[] WideMaterials;
		[SerializeReference] public MaterialSet[] StairMaterials;
		[SerializeReference] public MaterialSet[] RampMaterials;

		public MaterialFamilyInfo FamilyInfo;

		public MaterialSet[] GetSet(Def.BlockType type, Def.StairType stairType = Def.StairType.NORMAL)
		{
			switch (type)
			{
				case Def.BlockType.NORMAL:
					return NormalMaterials;
				case Def.BlockType.STAIRS:
					if (stairType == Def.StairType.NORMAL)
						return StairMaterials;
					else if (stairType == Def.StairType.RAMP)
						return RampMaterials;
					break;
				case Def.BlockType.WIDE:
					return WideMaterials;
			}
			return null;
		}
	}

	public static class BlockMaterial
	{
		public static MaterialSet[] VoidMat;
		public static List<MaterialFamily> MaterialFamilies;
		public static Dictionary<string, int> FamilyDict;

		public static List<UI.CImageSelectorUI.ElementInfo> UIMaterialFamilies;

		public static Material[] BiomeMaterials;

		//public static Texture2DArray TexturesOpaque;
		//public static Texture2DArray TexturesAlpha;
		public static Texture2D GetTextureFromMaterial(Material mat)
		{
			var texture = mat.GetTexture(Def.MaterialTextureID);
			if (texture == null)
				return (Texture2D)mat.GetTexture(Def.ColoredMaterialTextureID);
			return (Texture2D)texture;
		}
		public static MaterialFamily GetMaterialFamily(string familyName)
		{
			if (FamilyDict == null || MaterialFamilies == null)
				throw new Exception("BlockMaterial is not ready, trying to get a MaterialFamily before loading?");

			if(FamilyDict.ContainsKey(familyName))
			{
				return MaterialFamilies[FamilyDict[familyName]];
			}
			if (MaterialFamilies.Count == 0)
				throw new Exception("There are no BlockMaterial families, something went really wrong.");

			return MaterialFamilies[0];
		}
		struct PartInfo
		{
			public int FamilyID;
			public int Version;
			public bool IsVD;
		}

		public static void Prepare()
		{
			VoidMat = new MaterialSet[Def.BlockTypeCount];
			var materialParts = new List<MaterialPart>(AssetLoader.FloorTextures.Length);
			MaterialFamilies = new List<MaterialFamily>(AssetLoader.FloorTextures.Length / 3);
			FamilyDict = new Dictionary<string, int>(MaterialFamilies.Capacity);
			BiomeMaterials = new Material[Def.BiomeLayerTypeCount];
			//var opaqueTextures = new List<Texture2D>(AssetLoader.FloorTextures.Length);
			//var alphaTextures = new List<Texture2D>(AssetLoader.FloorTextures.Length);
			//var opaqueMaterial = new Material(Materials.GetMaterial(Def.Materials.TextureArrayLit));
			//var alphaMaterial = new Material(Materials.GetMaterial(Def.Materials.TextureArrayLit));

			var partInfos = new List<PartInfo>(materialParts.Capacity);

			MaterialSet[] setSetLenght(int newLength, MaterialSet[] list)
			{
				MaterialSet[] result = new MaterialSet[newLength];

				if (list != null)
				{
					list.CopyTo(result, 0);
				}

				return result;
			}

			void setMIDParts(MaterialPart midPart, MaterialSet[] list)
			{
				for (int i = 0; i < list.Length; ++i)
				{
					var set = list[i];
					set.BottomPart = midPart;
					midPart.FullMaterial = set;
				}
			}

			void setMaterial(Material matToCopy, string defMaterialName, MaterialSet[] list)
			{
				for (int i = 0; i < list.Length; ++i)
				{
					var fullMat = list[i];
					var topPart = fullMat.TopPart;
					var midPart = fullMat.BottomPart;

					if (topPart.Mat.name == defMaterialName)
					{
						var texture = topPart.Mat.GetTexture(Def.MaterialTextureID);
						UnityEngine.Object.Destroy(topPart.Mat);
						topPart.Mat = new Material(matToCopy);
						topPart.Mat.SetTexture(Def.MaterialTextureID, texture);
					}
					if (midPart.Mat.name == defMaterialName)
					{
						var texture = midPart.Mat.GetTexture(Def.MaterialTextureID);
						UnityEngine.Object.Destroy(midPart.Mat);
						midPart.Mat = new Material(matToCopy);
						midPart.Mat.SetTexture(Def.MaterialTextureID, texture);
					}
				}
			}

			MaterialSet[] _createColorVariation(MaterialFamily nMatFamily, MaterialSet[] materialSet, Material material, Vector4 CSHB)
			{
				MaterialSet[] nMatSet = new MaterialSet[materialSet.Length];
				for (int i = 0; i < materialSet.Length; ++i)
				{
					var curSet = materialSet[i];
					var curTopPart = curSet.TopPart;
					var curMidPart = curSet.BottomPart;

					MaterialPart topPart = new MaterialPart();
					MaterialPart midPart = new MaterialPart();
					var set = new MaterialSet
					{
						Family = nMatFamily,
						TopPart = topPart,
						BottomPart = midPart
					};

					topPart.FullMaterial = set;
					topPart.Type = curTopPart.Type;
					topPart.Where = curTopPart.Where;
					topPart.Mat = new Material(material);
					topPart.Mat.SetTexture(Def.ColoredMaterialTextureID, curTopPart.Mat.GetTexture(Def.MaterialTextureID));
					topPart.Mat.SetVector(Def.ColoredMaterialCSHBID, CSHB);

					midPart.FullMaterial = set;
					midPart.Type = curMidPart.Type;
					midPart.Where = curMidPart.Where;
					midPart.Mat = new Material(material);
					midPart.Mat.SetTexture(Def.ColoredMaterialTextureID, curMidPart.Mat.GetTexture(Def.MaterialTextureID));
					midPart.Mat.SetVector(Def.ColoredMaterialCSHBID, CSHB);

					materialParts.Add(topPart);
					materialParts.Add(midPart);
					nMatSet[i] = set;
				}
				return nMatSet;
			}

			void createColorVariation(MaterialFamily nMatFamily, MaterialFamily materialFamily, Material material, Vector4 CSHB)
			{
				nMatFamily.NormalMaterials = _createColorVariation(nMatFamily, materialFamily.NormalMaterials, material, CSHB);
				nMatFamily.StairMaterials = _createColorVariation(nMatFamily, materialFamily.StairMaterials, material, CSHB);
				nMatFamily.RampMaterials = _createColorVariation(nMatFamily, materialFamily.RampMaterials, material, CSHB);
				nMatFamily.WideMaterials = _createColorVariation(nMatFamily, materialFamily.WideMaterials, material, CSHB);
			}

			// Obtain all the names of the material textures and extract their information, transfering it into MaterialParts
			for (int i = 0; i < AssetLoader.FloorTextures.Length; ++i)
			{
				var floorTexture = AssetLoader.FloorTextures[i];
				//int arrIdx;
				//if (floorTexture.format == TextureFormat.DXT1)
				//{
				//    arrIdx = opaqueTextures.Count;
				//    opaqueTextures.Add(floorTexture);
				//}
				//else
				//{
				//    arrIdx = alphaTextures.Count;
				//    alphaTextures.Add(floorTexture);
				//}
				var split = floorTexture.name.ToLower().Split('_');
				if (split.Length != 3)
					throw new Exception($"Floor texture {i} has an invalid name:'{floorTexture.name}'.");
				if (split[0][0] != 't')
					throw new Exception($"Floor texture {i}, is not a Floor texture.");
				var familyIDStr = split[0].Substring(1);
				bool parseOK = int.TryParse(familyIDStr, out int familyID);
				if (!parseOK)
					throw new Exception($"Floor texture {i}, has an invalid FamilyID.");
				var placement = split[1];
				Def.BlockType blockType;
				Def.BlockMeshType position;
				Def.StairType stairType = Def.StairType.NORMAL;
				switch (placement)
				{
					case "bc":
						blockType = Def.BlockType.WIDE;
						position = Def.BlockMeshType.TOP;
						break;
					case "bl":
						blockType = Def.BlockType.WIDE;
						position = Def.BlockMeshType.MID;
						break;
					case "sc":
						blockType = Def.BlockType.NORMAL;
						position = Def.BlockMeshType.TOP;
						break;
					case "sl":
						blockType = Def.BlockType.NORMAL;
						position = Def.BlockMeshType.MID;
						break;
					case "ss":
						blockType = Def.BlockType.STAIRS;
						position = Def.BlockMeshType.TOP;
						break;
					case "ssl":
						blockType = Def.BlockType.STAIRS;
						position = Def.BlockMeshType.MID;
						break;
					case "sr":
						blockType = Def.BlockType.STAIRS;
						position = Def.BlockMeshType.TOP;
						stairType = Def.StairType.RAMP;
						break;
					default:
						Debug.LogWarning($"Floor texture {i}, has an invalid placement '{placement}'.");
						continue;
				}
				int version = -1;
				if (split[2] != "vd")
				{
					if (split[2][0] != 'v')
						throw new Exception($"Floor texture {i}, has an invalid version '{split[2]}'.");
					var versionStr = split[2].Substring(1);
					parseOK = int.TryParse(versionStr, out version);
					if (!parseOK)
						throw new Exception($"Floor texture {i}, has an invalid version '{versionStr}'.");
				}

				var part = new MaterialPart
				{
					Mat = new Material(Materials.GetMaterial(Def.Materials.Default)),
					Type = blockType,
					Where = position,
					FullMaterial = null,
					StairType = stairType,
					//ArrayIdx = arrIdx
				};
				part.Mat.SetTexture(Def.MaterialTextureID, floorTexture);

				materialParts.Add(part);
				var partInfo = new PartInfo
				{
					FamilyID = familyID,
					Version = version,
					IsVD = version == -1,
				};
				partInfos.Add(partInfo);
			}
			
			// Set the texture arrays
			//TexturesOpaque = new Texture2DArray(128, 128, opaqueTextures.Count, TextureFormat.DXT1, false);
			//for (int i = 0; i < opaqueTextures.Count; ++i)
			//    Graphics.CopyTexture(opaqueTextures[i], 0, TexturesOpaque, i);
			//TexturesOpaque.Apply(false, true);
			
			//TexturesAlpha = new Texture2DArray(128, 128, alphaTextures.Count, TextureFormat.DXT5, false);
			//for (int i = 0; i < alphaTextures.Count; ++i)
			//    Graphics.CopyTexture(alphaTextures[i], 0, TexturesAlpha, i);
			//TexturesAlpha.Apply(false, true);

			//opaqueMaterial.SetTexture(Def.Material_TA_Texture, TexturesOpaque);
			//alphaMaterial.SetTexture(Def.Material_TA_Texture, TexturesAlpha);

			// First pass over all MaterialParts, create MaterialFamilies and the MaterialSets with TOP MaterialParts 
			for (int i = 0; i < materialParts.Count; ++i)
			{
				var partInfo = partInfos[i];
				var part = materialParts[i];

				MaterialFamily family;
				if (MaterialFamilies.Count <= partInfo.FamilyID)
				{
					MaterialFamilies.AddRange(Enumerable.Repeat<MaterialFamily>(null, (partInfo.FamilyID + 1) - MaterialFamilies.Count));
					family = new MaterialFamily
					{
						NormalMaterials = new MaterialSet[0],
						StairMaterials = new MaterialSet[0],
						RampMaterials = new MaterialSet[0],
						WideMaterials = new MaterialSet[0]
					};
					MaterialFamilies[partInfo.FamilyID] = family;
				}
				else
				{
					family = MaterialFamilies[partInfo.FamilyID];
				}

				if (part.Where != Def.BlockMeshType.TOP)
					continue;
				MaterialSet fullMat = null;
				switch (part.Type)
				{
					case Def.BlockType.NORMAL:
						if (family.NormalMaterials.Length <= partInfo.Version)
							family.NormalMaterials = setSetLenght(partInfo.Version + 1, family.NormalMaterials);
						fullMat = family.NormalMaterials[partInfo.Version];
						if (fullMat == null)
						{
							fullMat = new MaterialSet();
							family.NormalMaterials[partInfo.Version] = fullMat;
							fullMat.Family = family;
						}
						break;
					case Def.BlockType.STAIRS:
						if (part.StairType == Def.StairType.NORMAL)
						{
							if (family.StairMaterials.Length <= partInfo.Version)
								family.StairMaterials = setSetLenght(partInfo.Version + 1, family.StairMaterials);
							fullMat = family.StairMaterials[partInfo.Version];
							if (fullMat == null)
							{
								fullMat = new MaterialSet();
								family.StairMaterials[partInfo.Version] = fullMat;
								fullMat.Family = family;
							}
						}
						else if(part.StairType == Def.StairType.RAMP)
						{
							if (family.RampMaterials.Length <= partInfo.Version)
								family.RampMaterials = setSetLenght(partInfo.Version + 1, family.RampMaterials);
							fullMat = family.RampMaterials[partInfo.Version];
							if (fullMat == null)
							{
								fullMat = new MaterialSet();
								family.RampMaterials[partInfo.Version] = fullMat;
								fullMat.Family = family;
							}
						}
						break;
					case Def.BlockType.WIDE:
						if (family.WideMaterials.Length <= partInfo.Version)
							family.WideMaterials = setSetLenght(partInfo.Version + 1, family.WideMaterials);
						fullMat = family.WideMaterials[partInfo.Version];
						if (fullMat == null)
						{
							fullMat = new MaterialSet();
							family.WideMaterials[partInfo.Version] = fullMat;
							fullMat.Family = family;
						}
						break;
				}
				fullMat.TopPart = part;
				part.FullMaterial = fullMat;
			}
			// Second pass over all MaterialParts, set the linked MID MaterialParts in the MaterialSets
			for (int i = 0; i < materialParts.Count; ++i)
			{
				var part = materialParts[i];
				if (part.Where != Def.BlockMeshType.MID)
					continue;
				var partInfo = partInfos[i];
				var family = MaterialFamilies[partInfo.FamilyID];
				if (partInfo.IsVD)
				{
					switch (part.Type)
					{
						case Def.BlockType.NORMAL:
							setMIDParts(part, family.NormalMaterials);
							break;
						case Def.BlockType.STAIRS:
							if (part.StairType == Def.StairType.NORMAL)
								setMIDParts(part, family.StairMaterials);
							else if (part.StairType == Def.StairType.RAMP)
								setMIDParts(part, family.RampMaterials);
							break;
						case Def.BlockType.WIDE:
							setMIDParts(part, family.WideMaterials);
							break;
					}
				}
				else
				{
					MaterialSet set = null;
					switch (part.Type)
					{
						case Def.BlockType.NORMAL:
							set = family.NormalMaterials[partInfo.Version];
							break;
						case Def.BlockType.STAIRS:
							if(part.StairType == Def.StairType.NORMAL)
								set = family.StairMaterials[partInfo.Version];
							if (part.StairType == Def.StairType.RAMP)
								set = family.RampMaterials[partInfo.Version];
							break;
						case Def.BlockType.WIDE:
							set = family.WideMaterials[partInfo.Version];
							break;
					}
					set.BottomPart = part;
					part.FullMaterial = set;
				}
			}
			// Third pass over all MaterialParts, set the MID MaterialParts if they are not done
			for (int i = 0; i < MaterialFamilies.Count; ++i)
			{
				var family = MaterialFamilies[i];
				for (int j = 0; j < family.StairMaterials.Length; ++j)
				{
					var fullMat = family.StairMaterials[j];
					if (fullMat.BottomPart != null)
						continue;
					//try
					//{
						fullMat.BottomPart = family.NormalMaterials[j].BottomPart;
					//}
					//catch(Exception e)
					//{
					//    throw e;
					//}
				}
				for (int j = 0; j < family.RampMaterials.Length; ++j)
				{
					var fullMat = family.RampMaterials[j];
					if (fullMat.BottomPart != null)
						continue;
					//try
					//{
					fullMat.BottomPart = family.NormalMaterials[j].BottomPart;
					//}
					//catch(Exception e)
					//{
					//    throw e;
					//}
				}
			}
			// Parse the MaterialList, in order to specify the caracteristics of the materials, set the material visibility and apply shader variations

			void SetFamilyInfo(MaterialFamilyInfo info)
			{
				var family = MaterialFamilies[info.ID];
				family.FamilyInfo = info;

				Material matToCopy = null;
				Material defMaterial = Materials.GetMaterial(Def.Materials.Default);
				switch (info.MaterialMode)
				{
					case Def.MaterialMode.Fade:
						matToCopy = Materials.GetMaterial(Def.Materials.Fade);
						break;
					case Def.MaterialMode.Transparent:
						matToCopy = Materials.GetMaterial(Def.Materials.Transparent);
						break;
					case Def.MaterialMode.Cutout:
						matToCopy = Materials.GetMaterial(Def.Materials.Cutout);
						break;
				}
				if (matToCopy != null)
				{
					setMaterial(matToCopy, defMaterial.name, family.NormalMaterials);
					setMaterial(matToCopy, defMaterial.name, family.StairMaterials);
					setMaterial(matToCopy, defMaterial.name, family.RampMaterials);
					setMaterial(matToCopy, defMaterial.name, family.WideMaterials);
				}
			}
			void AddColoredFamily(MaterialFamilyInfo info)
			{
				var origFamily = MaterialFamilies[info.ID];
				var nMatFamily = new MaterialFamily();
				MaterialFamilies.Add(nMatFamily);
				nMatFamily.NormalMaterials = new MaterialSet[0];
				nMatFamily.StairMaterials = new MaterialSet[0];
				nMatFamily.RampMaterials = new MaterialSet[0];
				nMatFamily.WideMaterials = new MaterialSet[0];
				nMatFamily.FamilyInfo = info;

				Material coloredMat = null;
				switch (info.MaterialMode)
				{
					case Def.MaterialMode.Default:
						coloredMat = Materials.GetMaterial(Def.Materials.ColoredDefault);
						break;
					case Def.MaterialMode.Fade:
					case Def.MaterialMode.Transparent:
						coloredMat = Materials.GetMaterial(Def.Materials.ColoredTransparent);
						break;
					case Def.MaterialMode.Cutout:
						coloredMat = Materials.GetMaterial(Def.Materials.ColoredCutout);
						break;
				}

				createColorVariation(nMatFamily, origFamily, coloredMat, info.CSHB);
			}

			for(int i = 0; i < AssetLoader.MaterialFamilies.Length; ++i)
			{
				var familyInfo = AssetLoader.MaterialFamilies[i];
				bool isColored = false;
				for (int j = 0; j < familyInfo.name.Length; ++j)
				{
					if(!char.IsDigit(familyInfo.name[j]))
					{
						isColored = true;
						break;
					}
				}
				if (isColored)
					AddColoredFamily(familyInfo);
				else
					SetFamilyInfo(familyInfo);
			}

			//var text = AssetLoader.FloorInfoText.text;
			//var reader = new StringReader(text);
			//var line = reader.ReadLine();
			//int lineNum = 0;
			//while (line != null)
			//{
			//    var lineBlocks = line.Split(':');
			//    if (lineBlocks.Length != 4)
			//        throw new Exception($"MaterialList has incorrect format at line:{lineNum}");

			//    bool parseOK = int.TryParse(lineBlocks[0], out int materialFamilyID);
			//    if (!parseOK)
			//        throw new Exception($"MaterialList has incorrect MaterialFamilyID at line:{lineNum}");

			//    var family = MaterialFamilies[materialFamilyID];

			//    var colorVariations = lineBlocks[1].Split('.');
			//    var cvLength = colorVariations[0].Length == 0 ? 0 : colorVariations.Length;
			//    var nCV = new string[cvLength + 1];
			//    for (int i = 0; i < cvLength; ++i)
			//        nCV[i + 1] = colorVariations[i];
			//    colorVariations = nCV;

			//    var materialModeStr = lineBlocks[2].ToLower();
			//    var matMode = materialModeStr[0];
			//    Material matToCopy = null;
			//    Material defMaterial = Materials.GetMaterial(Def.Materials.Default);
			//    Def.Materials materialMode = Def.Materials.Default;
			//    switch (matMode)
			//    {
			//        case 'f':
			//            {
			//                matToCopy = Materials.GetMaterial(Def.Materials.Fade);
			//                materialMode = Def.Materials.Fade;
			//            }
			//            break;
			//        case 't':
			//            {
			//                matToCopy = Materials.GetMaterial(Def.Materials.Transparent);
			//                materialMode = Def.Materials.Transparent;
			//            }
			//            break;
			//        case 'c':
			//            {
			//                matToCopy = Materials.GetMaterial(Def.Materials.Cutout);
			//                materialMode = Def.Materials.Cutout;
			//            }
			//            break;
			//        default:
			//            break;
			//    }
			//    if (matToCopy != null)
			//    {
			//        setMaterial(matToCopy, defMaterial.name, family.NormalMaterials);
			//        setMaterial(matToCopy, defMaterial.name, family.StairMaterials);
			//        setMaterial(matToCopy, defMaterial.name, family.WideMaterials);
			//    }

			//    var names = lineBlocks[3].Split(',');
			//    if ((colorVariations.Length) != names.Length)
			//        throw new Exception("Error parsing the MaterialList, ColorVariation length mismatch the Name length.");

			//    for (int i = 1; i < names.Length; ++i) names[i] = names[i].Trim();

			//    family.FamilyName = names[0];

			//    Vector4 CSHB = Vector4.zero;
			//    for (int i = 1; i < names.Length; ++i)
			//    {
			//        var variation = colorVariations[i].Split('_');
			//        if (variation.Length != 4)
			//            throw new Exception($"Invalid Variation:{i}, in MaterialList line:{lineNum}");

			//        for (int j = 0; j < variation.Length; ++j)
			//        {
			//            parseOK = float.TryParse(variation[j], out float varVal);
			//            if (!parseOK)
			//                throw new Exception($"Couldn't parse Variation{i}, CSHB:{j}, in MaterialList line:{lineNum}");
			//            CSHB[j] = varVal;
			//        }

			//        CSHB.x = CSHB.x * 1.4f;
			//        CSHB.w = CSHB.w * 1.5f;

			//        var nMatFamily = new MaterialFamily();
			//        MaterialFamilies.Add(nMatFamily);
			//        nMatFamily.NormalMaterials = new MaterialSet[0];
			//        nMatFamily.StairMaterials = new MaterialSet[0];
			//        nMatFamily.WideMaterials = new MaterialSet[0];

			//        Material coloredMat = null;
			//        switch (materialMode)
			//        {
			//            case Def.Materials.Default:
			//                coloredMat = Materials.GetMaterial(Def.Materials.ColoredDefault);
			//                break;
			//            case Def.Materials.Fade:
			//            case Def.Materials.Transparent:
			//                coloredMat = Materials.GetMaterial(Def.Materials.ColoredTransparent);
			//                break;
			//            case Def.Materials.Cutout:
			//                coloredMat = Materials.GetMaterial(Def.Materials.ColoredCutout);
			//                break;
			//        }

			//        createColorVariation(nMatFamily, family, coloredMat, CSHB);

			//        nMatFamily.FamilyName = names[i];
			//    }

			//    ++lineNum;
			//    line = reader.ReadLine();
			//}
			// Set the family dictionary
			//try
			//{
				for (int i = 0; i < MaterialFamilies.Count; ++i)
				{
					FamilyDict.Add(MaterialFamilies[i].FamilyInfo.FamilyName, i);
				}
			//}
			//catch (Exception e)
			//{
			//    throw new Exception("Is MaterialList Updated?\n" + e.Message);
			//}
			// Set the void material
			VoidMat[(int)Def.BlockType.NORMAL] = MaterialFamilies[0].NormalMaterials[0];
			VoidMat[(int)Def.BlockType.STAIRS] = MaterialFamilies[0].StairMaterials[0];
			VoidMat[(int)Def.BlockType.WIDE] = MaterialFamilies[0].WideMaterials[0];

			UIMaterialFamilies = new List<UI.CImageSelectorUI.ElementInfo>(MaterialFamilies.Count - 1);
			for(int i = 1; i < MaterialFamilies.Count; ++i)
			{
				var family = MaterialFamilies[i];
				if (family == null)
					continue;

				var mat = family.NormalMaterials[0].TopPart.Mat;

				var texture = (Texture2D)mat.GetTexture(Def.MaterialTextureID);
				if (texture == null)
					texture = (Texture2D)mat.GetTexture(Def.ColoredMaterialTextureID);

				var sprite = Sprite.Create(texture,
							new Rect(0f, 0f, texture.width, texture.height),
							new Vector2(0f, 0f), 100f, 0, SpriteMeshType.FullRect);

				UIMaterialFamilies.Add(new UI.CImageSelectorUI.ElementInfo()
				{
					Image = sprite,
					Name = family.FamilyInfo.FamilyName
				});
			}
			for(int i = 0; i < BiomeMaterials.Length; ++i)
			{
				BiomeMaterials[i] = new Material(Materials.GetMaterial(Def.Materials.Default))
				{
					name = ((Def.BiomeLayerType)i).ToString() + "_MATERIAL",
					mainTexture = null,
					color = Def.BiomeLayerTypeColors[i]
				};
			}
		}

		public static void Init()
		{
			VoidMat = new MaterialSet[Def.BlockTypeCount];

			var floorTextures = Resources.LoadAll<Texture2D>("Floor");

			List<MaterialPart> materialParts = new List<MaterialPart>(floorTextures.Length);
			MaterialFamilies = new List<MaterialFamily>(floorTextures.Length / 3);
			FamilyDict = new Dictionary<string, int>(MaterialFamilies.Capacity);

			List<PartInfo> partInfos = new List<PartInfo>(materialParts.Capacity);

			int MaterialTextureID = Shader.PropertyToID("_BaseMap");
			int ColoredMaterialTextureID = Shader.PropertyToID("Texture2D_556CDA01");
			int ColoredMaterialCSHBID = Shader.PropertyToID("Vector4_34A1AF91");
			int BackgroundColorID = Shader.PropertyToID("ParticleColor");

			//MaterialSet[] addSet(MaterialSet fullMat, MaterialSet[] list)
			//{
			//    MaterialSet[] result = null;
			//    if (list != null)
			//    {
			//        result = new MaterialSet[list.Length + 1];
			//        list.CopyTo(result, 0);
			//    }
			//    else
			//    {
			//        result = new MaterialSet[1];
			//    }
			//    result[result.Length - 1] = fullMat;

			//    return result;
			//}

			MaterialSet[] setSetLenght(int newLength, MaterialSet[] list)
			{
				MaterialSet[] result = new MaterialSet[newLength];

				if (list != null)
				{
					list.CopyTo(result, 0);
				}

				return result;
			}

			void setMIDParts(MaterialPart midPart, MaterialSet[] list)
			{
				for (int i = 0; i < list.Length; ++i)
				{
					var set = list[i];
					set.BottomPart = midPart;
					midPart.FullMaterial = set;
				}
			}

			void setMaterial(Material matToCopy, string defMaterialName, MaterialSet[] list)
			{
				for (int i = 0; i < list.Length; ++i)
				{
					var fullMat = list[i];
					var topPart = fullMat.TopPart;
					var midPart = fullMat.BottomPart;

					if (topPart.Mat.name == defMaterialName)
					{
						var texture = topPart.Mat.GetTexture(MaterialTextureID);
						UnityEngine.Object.DestroyImmediate(topPart.Mat);
						topPart.Mat = new Material(matToCopy);
						topPart.Mat.SetTexture(MaterialTextureID, texture);
					}
					if (midPart.Mat.name == defMaterialName)
					{
						var texture = midPart.Mat.GetTexture(MaterialTextureID);
						UnityEngine.Object.DestroyImmediate(midPart.Mat);
						midPart.Mat = new Material(matToCopy);
						midPart.Mat.SetTexture(MaterialTextureID, texture);
					}
				}
			}

			MaterialSet[] _createColorVariation(MaterialFamily nMatFamily, MaterialSet[] materialSet, Material material, Vector4 CSHB)
			{
				MaterialSet[] nMatSet = new MaterialSet[materialSet.Length];
				for (int i = 0; i < materialSet.Length; ++i)
				{
					var curSet = materialSet[i];
					var curTopPart = curSet.TopPart;
					var curMidPart = curSet.BottomPart;

					MaterialPart topPart = new MaterialPart();
					MaterialPart midPart = new MaterialPart();
					var set = new MaterialSet
					{
						Family = nMatFamily,
						TopPart = topPart,
						BottomPart = midPart
					};

					topPart.FullMaterial = set;
					topPart.Type = curTopPart.Type;
					topPart.Where = curTopPart.Where;
					topPart.Mat = new Material(material);
					topPart.Mat.SetTexture(ColoredMaterialTextureID, curTopPart.Mat.GetTexture(MaterialTextureID));
					topPart.Mat.SetVector(ColoredMaterialCSHBID, CSHB);

					midPart.FullMaterial = set;
					midPart.Type = curMidPart.Type;
					midPart.Where = curMidPart.Where;
					midPart.Mat = new Material(material);
					midPart.Mat.SetTexture(ColoredMaterialTextureID, curMidPart.Mat.GetTexture(MaterialTextureID));
					midPart.Mat.SetVector(ColoredMaterialCSHBID, CSHB);

					materialParts.Add(topPart);
					materialParts.Add(midPart);
					nMatSet[i] = set;
				}
				return nMatSet;
			}

			void createColorVariation(MaterialFamily nMatFamily, MaterialFamily materialFamily, Material material, Vector4 CSHB)
			{
				nMatFamily.NormalMaterials = _createColorVariation(nMatFamily, materialFamily.NormalMaterials, material, CSHB);
				nMatFamily.StairMaterials = _createColorVariation(nMatFamily, materialFamily.StairMaterials, material, CSHB);
				nMatFamily.WideMaterials = _createColorVariation(nMatFamily, materialFamily.WideMaterials, material, CSHB);
			}

			// Obtain all the names of the material textures and extract their information, transfering it into MaterialParts
			for (int i = 0; i < floorTextures.Length; ++i)
			{
				var floorTexture = floorTextures[i];
				var split = floorTexture.name.ToLower().Split('_');
				if (split.Length != 3)
					throw new Exception($"Floor texture {i} has an invalid name:'{floorTexture.name}'.");
				if (split[0][0] != 't')
					throw new Exception($"Floor texture {i}, is not a Floor texture.");
				var familyIDStr = split[0].Substring(1);
				bool parseOK = int.TryParse(familyIDStr, out int familyID);
				if (!parseOK)
					throw new Exception($"Floor texture {i}, has an invalid FamilyID.");
				var placement = split[1];
				Def.BlockType blockType;
				Def.BlockMeshType position;
				switch (placement)
				{
					case "bc":
						blockType = Def.BlockType.WIDE;
						position = Def.BlockMeshType.TOP;
						break;
					case "bl":
						blockType = Def.BlockType.WIDE;
						position = Def.BlockMeshType.MID;
						break;
					case "sc":
						blockType = Def.BlockType.NORMAL;
						position = Def.BlockMeshType.TOP;
						break;
					case "sl":
						blockType = Def.BlockType.NORMAL;
						position = Def.BlockMeshType.MID;
						break;
					case "ss":
						blockType = Def.BlockType.STAIRS;
						position = Def.BlockMeshType.TOP;
						break;
					case "ssl":
						blockType = Def.BlockType.STAIRS;
						position = Def.BlockMeshType.MID;
						break;
					default:
						throw new Exception($"Floor texture {i}, has an invalid placement '{placement}'.");
				}
				int version = -1;
				if (split[2] != "vd")
				{
					if (split[2][0] != 'v')
						throw new Exception($"Floor texture {i}, has an invalid version '{split[2]}'.");
					var versionStr = split[2].Substring(1);
					parseOK = int.TryParse(versionStr, out version);
					if (!parseOK)
						throw new Exception($"Floor texture {i}, has an invalid version '{versionStr}'.");
				}

				var part = new MaterialPart
				{
					Mat = new Material(Materials.GetMaterial(Def.Materials.Default)),
					Type = blockType,
					Where = position,
					FullMaterial = null
				};
				part.Mat.SetTexture(MaterialTextureID, floorTexture);

				materialParts.Add(part);
				var partInfo = new PartInfo
				{
					FamilyID = familyID,
					Version = version,
					IsVD = version == -1,
				};
				partInfos.Add(partInfo);
			}
			// First pass over all MaterialParts, create MaterialFamilies and the MaterialSets with TOP MaterialParts 
			for (int i = 0; i < materialParts.Count; ++i)
			{
				var partInfo = partInfos[i];
				var part = materialParts[i];

				MaterialFamily family;
				if (MaterialFamilies.Count <= partInfo.FamilyID)
				{
					MaterialFamilies.AddRange(Enumerable.Repeat<MaterialFamily>(null, (partInfo.FamilyID + 1) - MaterialFamilies.Count));
					family = new MaterialFamily
					{
						NormalMaterials = new MaterialSet[0],
						StairMaterials = new MaterialSet[0],
						WideMaterials = new MaterialSet[0]
					};
					MaterialFamilies[partInfo.FamilyID] = family;
				}
				else
				{
					family = MaterialFamilies[partInfo.FamilyID];
				}

				if (part.Where != Def.BlockMeshType.TOP)
					continue;
				MaterialSet fullMat = null;
				switch (part.Type)
				{
					case Def.BlockType.NORMAL:
						if (family.NormalMaterials.Length <= partInfo.Version)
							family.NormalMaterials = setSetLenght(partInfo.Version + 1, family.NormalMaterials);    
						fullMat = family.NormalMaterials[partInfo.Version];
						if (fullMat == null)
						{
							fullMat = new MaterialSet();
							family.NormalMaterials[partInfo.Version] = fullMat;
							fullMat.Family = family;
						}
						break;
					case Def.BlockType.STAIRS:
						if (family.StairMaterials.Length <= partInfo.Version)
							family.StairMaterials = setSetLenght(partInfo.Version + 1, family.StairMaterials);
						fullMat = family.StairMaterials[partInfo.Version];
						if (fullMat == null)
						{
							fullMat = new MaterialSet();
							family.StairMaterials[partInfo.Version] = fullMat;
							fullMat.Family = family;
						}
						break;
					case Def.BlockType.WIDE:
						if (family.WideMaterials.Length <= partInfo.Version)
							family.WideMaterials = setSetLenght(partInfo.Version + 1, family.WideMaterials);
						fullMat = family.WideMaterials[partInfo.Version];
						if (fullMat == null)
						{
							fullMat = new MaterialSet();
							family.WideMaterials[partInfo.Version] = fullMat;
							fullMat.Family = family;
						}
						break;
				}
				fullMat.TopPart = part;
				part.FullMaterial = fullMat;
			}
			// Second pass over all MaterialParts, set the linked MID MaterialParts in the MaterialSets
			for (int i = 0; i < materialParts.Count; ++i)
			{
				var part = materialParts[i];
				if (part.Where != Def.BlockMeshType.MID)
					continue;
				var partInfo = partInfos[i];
				var family = MaterialFamilies[partInfo.FamilyID];
				if (partInfo.IsVD)
				{
					switch (part.Type)
					{
						case Def.BlockType.NORMAL:
							setMIDParts(part, family.NormalMaterials);
							break;
						case Def.BlockType.STAIRS:
							setMIDParts(part, family.StairMaterials);
							break;
						case Def.BlockType.WIDE:
							setMIDParts(part, family.WideMaterials);
							break;
					}
				}
				else
				{
					MaterialSet set = null;
					switch (part.Type)
					{
						case Def.BlockType.NORMAL:
							set = family.NormalMaterials[partInfo.Version];
							break;
						case Def.BlockType.STAIRS:
							set = family.StairMaterials[partInfo.Version];
							break;
						case Def.BlockType.WIDE:
							set = family.WideMaterials[partInfo.Version];
							break;
					}
					set.BottomPart = part;
					part.FullMaterial = set;
				}
			}
			// Third pass over all MaterialParts, set the MID MaterialParts if they are not done
			for (int i = 0; i < MaterialFamilies.Count; ++i)
			{
				var family = MaterialFamilies[i];
				for (int j = 0; j < family.StairMaterials.Length; ++j)
				{
					var fullMat = family.StairMaterials[j];
					if (fullMat.BottomPart != null)
						continue;
					fullMat.BottomPart = family.NormalMaterials[j].BottomPart;
				}
			}
			// Find the MaterialList asset
			var textAssets = Resources.LoadAll<TextAsset>("Floor");
			TextAsset materialList = null;
			foreach (var textAsset in textAssets)
			{
				if (textAsset.name.ToLower() == "materiallist")
				{
					materialList = textAsset;
					break;
				}
			}
			if (materialList == null)
				throw new Exception("Couldn't find the MaterialList file in Resources/Floor/");// AssetContainer.TextAssets.");

			// Parse the MaterialList, in order to specify the caracteristics of the materials, set the material visibility and apply shader variations
			var text = materialList.text;
			var reader = new StringReader(text);
			var line = reader.ReadLine();
			int lineNum = 0;
			while (line != null)
			{
				var lineBlocks = line.Split(':');
				if (lineBlocks.Length != 4)
					throw new Exception($"MaterialList has incorrect format at line:{lineNum}");

				bool parseOK = int.TryParse(lineBlocks[0], out int materialFamilyID);
				if (!parseOK)
					throw new Exception($"MaterialList has incorrect MaterialFamilyID at line:{lineNum}");

				var family = MaterialFamilies[materialFamilyID];

				var colorVariations = lineBlocks[1].Split('.');
				var cvLength = colorVariations[0].Length == 0 ? 0 : colorVariations.Length;
				var nCV = new string[cvLength + 1];
				for (int i = 0; i < cvLength; ++i)
					nCV[i + 1] = colorVariations[i];
				colorVariations = nCV;

				var materialModeStr = lineBlocks[2].ToLower();
				var matMode = materialModeStr[0];
				Material matToCopy = null;
				Material defMaterial = Materials.GetMaterial(Def.Materials.Default);
				Def.Materials materialMode = Def.Materials.Default;
				switch (matMode)
				{
					case 'f':
						{
							matToCopy = Materials.GetMaterial(Def.Materials.Fade);
							materialMode = Def.Materials.Fade;
						}
						break;
					case 't':
						{
							matToCopy = Materials.GetMaterial(Def.Materials.Transparent);
							materialMode = Def.Materials.Transparent;
						}
						break;
					case 'c':
						{
							matToCopy = Materials.GetMaterial(Def.Materials.Cutout);
							materialMode = Def.Materials.Cutout;
						}
						break;
					default:
						break;
				}
				if (matToCopy != null)
				{
					setMaterial(matToCopy, defMaterial.name, family.NormalMaterials);
					setMaterial(matToCopy, defMaterial.name, family.StairMaterials);
					setMaterial(matToCopy, defMaterial.name, family.WideMaterials);
				}

				var names = lineBlocks[3].Split(',');
				if ((colorVariations.Length) != names.Length)
					throw new Exception("Error parsing the MaterialList, ColorVariation length mismatch the Name length.");

				for (int i = 1; i < names.Length; ++i) names[i] = names[i].Trim();

				//family.FamilyName = names[0];

				Vector4 CSHB = Vector4.zero;
				for (int i = 1; i < names.Length; ++i)
				{
					var variation = colorVariations[i].Split('_');
					if (variation.Length != 4)
						throw new Exception($"Invalid Variation:{i}, in MaterialList line:{lineNum}");

					for (int j = 0; j < variation.Length; ++j)
					{
						parseOK = float.TryParse(variation[j], out float varVal);
						if (!parseOK)
							throw new Exception($"Couldn't parse Variation{i}, CSHB:{j}, in MaterialList line:{lineNum}");
						CSHB[j] = varVal;
					}

					CSHB.x = CSHB.x * 1.4f;
					CSHB.w = CSHB.w * 1.5f;

					var nMatFamily = new MaterialFamily();
					MaterialFamilies.Add(nMatFamily);
					nMatFamily.NormalMaterials = new MaterialSet[0];
					nMatFamily.StairMaterials = new MaterialSet[0];
					nMatFamily.WideMaterials = new MaterialSet[0];

					Material coloredMat = null;
					switch (materialMode)
					{
						case Def.Materials.Default:
							coloredMat = Materials.GetMaterial(Def.Materials.ColoredDefault);
							break;
						case Def.Materials.Fade:
						case Def.Materials.Transparent:
							coloredMat = Materials.GetMaterial(Def.Materials.ColoredTransparent);
							break;
						case Def.Materials.Cutout:
							coloredMat = Materials.GetMaterial(Def.Materials.ColoredCutout);
							break;
					}

					createColorVariation(nMatFamily, family, coloredMat, CSHB);

					//nMatFamily.FamilyName = names[i];
				}

				++lineNum;
				line = reader.ReadLine();
			}
			// Set the family dictionary
			//try
			//{
			//    for (int i = 0; i < MaterialFamilies.Count; ++i)
			//    {
			//        FamilyDict.Add(MaterialFamilies[i].FamilyName, i);
			//    }
			//}
			//catch (Exception)
			//{
			//    throw new Exception("Is MaterialList Updated?");
			//}
			// Set the void material
			VoidMat[(int)Def.BlockType.NORMAL] = MaterialFamilies[0].NormalMaterials[0];
			VoidMat[(int)Def.BlockType.STAIRS] = MaterialFamilies[0].StairMaterials[0];
			VoidMat[(int)Def.BlockType.WIDE] = MaterialFamilies[0].WideMaterials[0];
		}

		public static void Deinit()
		{
			VoidMat = null;
			//BlockMaterials.Clear();
			//BlockMaterials = null;
			//MaterialTypes.Clear();
			//MaterialTypes = null;
			//MaterialTypeDic.Clear();
			//MaterialTypeDic = null;
		}
	}
}
