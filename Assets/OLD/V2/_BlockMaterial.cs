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
//    public struct MaterialPlacement
//    {
//        public BlockType Type;
//        public Def.BlockMeshType Where;

//        public override bool Equals(object obj)
//        {
//            if (!(obj is MaterialPlacement))
//            {
//                return false;
//            }

//            var placement = (MaterialPlacement)obj;
//            return Type == placement.Type &&
//                   Where == placement.Where;
//        }

//        public override int GetHashCode()
//        {
//            var hashCode = 201220973;
//            hashCode = hashCode * -1521134295 + Type.GetHashCode();
//            hashCode = hashCode * -1521134295 + Where.GetHashCode();
//            return hashCode;
//        }

//        public static bool operator ==(MaterialPlacement placement1, MaterialPlacement placement2)
//        {
//            return placement1.Equals(placement2);
//        }

//        public static bool operator !=(MaterialPlacement placement1, MaterialPlacement placement2)
//        {
//            return !(placement1 == placement2);
//        }
//    }

//    public struct BlockMat
//    {
//        public int MaterialID;
//        public int MaterialTypeID;
//        public MaterialPlacement Placement;
//        public Material BlockMaterial;

//        public override bool Equals(object obj)
//        {
//            if (!(obj is BlockMat))
//            {
//                return false;
//            }
//            var mat = (BlockMat)obj;
//            return MaterialID == mat.MaterialID &&
//                   MaterialTypeID == mat.MaterialTypeID &&
//                   Placement.Type == mat.Placement.Type &&
//                   Placement.Where == mat.Placement.Where &&
//                   BlockMaterial == mat.BlockMaterial;/* &&
//                   BlockMaterial.mainTexture == mat.BlockMaterial.mainTexture;*/
//        }

//        public override int GetHashCode()
//        {
//            var hashCode = -1932315383;
//            hashCode = hashCode * -1521134295 + MaterialID.GetHashCode();
//            hashCode = hashCode * -1521134295 + MaterialTypeID.GetHashCode();
//            hashCode = hashCode * -1521134295 + Placement.GetHashCode();
//            hashCode = hashCode * -1521134295 + BlockMaterial.GetHashCode();
//            return hashCode;
//        }

//        public static bool operator ==(BlockMat mat1, BlockMat mat2)
//        {
//            return mat1.Equals(mat2);
//        }

//        public static bool operator !=(BlockMat mat1, BlockMat mat2)
//        {
//            return !(mat1 == mat2);
//        }
//    }

//    public struct FullBlockMat
//    {
//        public int TopMat;
//        public int MidMat;

//        public FullBlockMat(int topMat, int midMat)
//        {
//            TopMat = topMat;
//            MidMat = midMat;
//        }

//        public override bool Equals(object obj)
//        {
//            if (!(obj is FullBlockMat))
//            {
//                return false;
//            }

//            var mat = (FullBlockMat)obj;
//            return mat == this;
//        }

//        public override int GetHashCode()
//        {
//            var hashCode = -1770788669;
//            hashCode = hashCode * -1521134295 + TopMat.GetHashCode();
//            hashCode = hashCode * -1521134295 + MidMat.GetHashCode();
//            return hashCode;
//        }

//        public static bool operator ==(FullBlockMat left, FullBlockMat right)
//        {
//            return left.TopMat == right.TopMat && left.MidMat == right.MidMat;
//        }

//        public static bool operator !=(FullBlockMat left, FullBlockMat right)
//        {
//            return !(left == right);
//        }
//    }

//    public struct MaterialType
//    {
//        public int MaterialSubID;
//        public string MaterialTypeName;
//        public List<FullBlockMat>[] Def.Materials;
//    }

    ////public struct MaterialType
    ////{
    ////    public int MaterialTypeID;
    ////    public string MaterialTypeName;
    ////    public List<int> Def.Materials;
    ////}

    //public class BlockMaterial
    //{
        //static int LastBlockMatID;
        //static int LastMaterialID;

        //public static FullBlockMat[] VoidMat;

        //public static List<BlockMat> BlockMaterials;
        //public static List<MaterialType> MaterialTypes;
        //public static Dictionary<string, int> MaterialTypeDic;

        //public static List<MaterialType> materialTypes = new List<MaterialType>();

        //public static Dictionary<string, int> materialTypeDic = new Dictionary<string, int>();

        //public static void AddMaterialType(string materialTypeName, List<int> materials)
        //{
        //    MaterialType materialType;
        //    materialType.MaterialTypeID = LastMaterialID++;
        //    materialType.MaterialTypeName = materialTypeName;
        //    materialType.Def.Materials = materials;
        //    materialTypeDic.Add(materialTypeName, materialType.MaterialTypeID);
        //    materialTypes.Add(materialType);
        //}

        //public static List<MaterialPart> MaterialParts;
        //public static List<MaterialFamily> MaterialFamilies;
        //public static Dictionary<string, MaterialFamily> MaterialFamilyDict;

        //static void CreateSet(MaterialFamily curFamily, ref MaterialSet[] materials, MaterialType matType, BlockType blockType, string blockTypeName)
        //{
        //    for (int i = 0; i < materials.Length; ++i)
        //    {
        //        var curSet = ScriptableObject.CreateInstance<MaterialSet>();
        //        var fullMat = matType.Def.Materials[(int)blockType][i];
        //        var topMat = BlockMaterials[fullMat.TopMat];
        //        var botMat = BlockMaterials[fullMat.MidMat];
        //        curSet.Family = curFamily;
        //        curSet.TopPart = ScriptableObject.CreateInstance<MaterialPart>();
        //        curSet.TopPart.FullMaterial = curSet;
        //        curSet.TopPart.Type = topMat.Placement.Type;
        //        curSet.TopPart.Where = topMat.Placement.Where;
        //        curSet.TopPart.Mat = new Material(topMat.BlockMaterial);

        //        curSet.BottomPart = ScriptableObject.CreateInstance<MaterialPart>();
        //        curSet.BottomPart.FullMaterial = curSet;
        //        curSet.BottomPart.Type = botMat.Placement.Type;
        //        curSet.BottomPart.Where = botMat.Placement.Where;
        //        curSet.BottomPart.Mat = new Material(botMat.BlockMaterial);

        //        UnityEditor.AssetDatabase.CreateAsset(curSet.TopPart.Mat, $"Assets/BlockMaterials/MP_{curFamily.FamilyName}_{blockTypeName}_{i}_TOP.mat");
        //        UnityEditor.AssetDatabase.CreateAsset(curSet.TopPart, $"Assets/BlockMaterials/MP_{curFamily.FamilyName}_{blockTypeName}_{i}_TOP.asset");

        //        UnityEditor.AssetDatabase.CreateAsset(curSet.BottomPart.Mat, $"Assets/BlockMaterials/MP_{curFamily.FamilyName}_{blockTypeName}_{i}_BOT.mat");
        //        UnityEditor.AssetDatabase.CreateAsset(curSet.BottomPart, $"Assets/BlockMaterials/MP_{curFamily.FamilyName}_{blockTypeName}_{i}_BOT.asset");

        //        UnityEditor.AssetDatabase.CreateAsset(curSet, $"Assets/BlockMaterials/MS_{curFamily.FamilyName}_{blockTypeName}_{i}.asset");
        //        materials[i] = curSet;
        //    }
        //}

        //static void EditorInit()
        //{
        //    UnityEditor.AssetDatabase.CreateFolder("Assets", "BlockMaterials");
        //    for(int i = 0; i < MaterialTypes.Count; ++i)
        //    {
        //        var matType = MaterialTypes[i];
        //        var curFamily = ScriptableObject.CreateInstance<MaterialFamily>();
        //        curFamily.FamilyName = matType.MaterialTypeName;
        //        curFamily.NormalMaterials = new MaterialSet[matType.Def.Materials[(int)BlockType.NORMAL].Count];
        //        curFamily.StairMaterials = new MaterialSet[matType.Def.Materials[(int)BlockType.STAIRS].Count];
        //        curFamily.WideMaterials = new MaterialSet[matType.Def.Materials[(int)BlockType.WIDE].Count];
        //        CreateSet(curFamily, ref curFamily.NormalMaterials, matType, BlockType.NORMAL, "NORMAL");
        //        CreateSet(curFamily, ref curFamily.StairMaterials, matType, BlockType.STAIRS, "STAIRS");
        //        CreateSet(curFamily, ref curFamily.WideMaterials, matType, BlockType.WIDE, "WIDE");

        //        UnityEditor.AssetDatabase.CreateAsset(curFamily, $"Assets/BlockMaterials/MF_{curFamily.FamilyName}.asset");
        //    }
        //}

        //public static void EditorInit(BlockMaterials info, Material[] materialsList)
        //{
        //    info.VoidMaterial = new MaterialSet[(int)BlockType.COUNT];

        //    var floorTextures = Resources.LoadAll<Texture2D>("Floor");

        //    List<MaterialPart> materialParts = new List<MaterialPart>(floorTextures.Length);
        //    List<MaterialFamily> materialFamilies = new List<MaterialFamily>(floorTextures.Length / 3);

        //    List<PartInfo> partInfos = new List<PartInfo>(materialParts.Capacity);

        //    int MaterialTextureID = Shader.PropertyToID("_BaseMap");
        //    int ColoredMaterialTextureID = Shader.PropertyToID("Texture2D_556CDA01");
        //    int ColoredMaterialCSHBID = Shader.PropertyToID("Vector4_34A1AF91");
        //    int BackgroundColorID = Shader.PropertyToID("ParticleColor");

        //    Func<MaterialSet, MaterialSet[], MaterialSet[]> addSet = (MaterialSet fullMat, MaterialSet[] list) =>
        //    {
        //        MaterialSet[] result = null;
        //        if (list != null)
        //        {
        //            result = new MaterialSet[list.Length + 1];
        //            list.CopyTo(result, 0);
        //        }
        //        else
        //        {
        //            result = new MaterialSet[1];
        //        }
        //        result[result.Length - 1] = fullMat;

        //        return result;
        //    };

        //    Func<int, MaterialSet[], MaterialSet[]> setSetLenght = (int newLength, MaterialSet[] list) =>
        //    {
        //        MaterialSet[] result = new MaterialSet[newLength];

        //        if (list != null)
        //        {
        //            list.CopyTo(result, 0);
        //        }

        //        return result;
        //    };

        //    Action<MaterialPart, MaterialSet[]> setMIDParts = (MaterialPart midPart, MaterialSet[] list) =>
        //    {
        //        for (int i = 0; i < list.Length; ++i)
        //        {
        //            var set = list[i];
        //            set.BottomPart = midPart;
        //            midPart.FullMaterial = set;
        //        }
        //    };

        //    Action<Material, string, MaterialSet[]> setMaterial = (Material matToCopy, string defMaterialName, MaterialSet[] list) =>
        //    {
        //        for (int i = 0; i < list.Length; ++i)
        //        {
        //            var fullMat = list[i];
        //            var topPart = fullMat.TopPart;
        //            var midPart = fullMat.BottomPart;

        //            if (topPart.Mat.name == defMaterialName)
        //            {
        //                var texture = topPart.Mat.GetTexture(MaterialTextureID);
        //                UnityEngine.Object.DestroyImmediate(topPart.Mat);
        //                topPart.Mat = new Material(matToCopy);
        //                topPart.Mat.SetTexture(MaterialTextureID, texture);
        //            }
        //            if (midPart.Mat.name == defMaterialName)
        //            {
        //                var texture = midPart.Mat.GetTexture(MaterialTextureID);
        //                UnityEngine.Object.DestroyImmediate(midPart.Mat);
        //                midPart.Mat = new Material(matToCopy);
        //                midPart.Mat.SetTexture(MaterialTextureID, texture);
        //            }
        //        }
        //    };

        //    Func<MaterialFamily, MaterialSet[], Material, Vector4, MaterialSet[]> _createColorVariation =
        //        (MaterialFamily nMatFamily, MaterialSet[] materialSet, Material material, Vector4 CSHB) =>
        //    {
        //        MaterialSet[] nMatSet = new MaterialSet[materialSet.Length];
        //        for (int i = 0; i < materialSet.Length; ++i)
        //        {
        //            var curSet = materialSet[i];
        //            var curTopPart = curSet.TopPart;
        //            var curMidPart = curSet.BottomPart;

        //            MaterialPart topPart = new MaterialPart();
        //            MaterialPart midPart = new MaterialPart();
        //            MaterialSet set = new MaterialSet();
        //            set.Family = nMatFamily;
        //            set.TopPart = topPart;
        //            set.BottomPart = midPart;

        //            topPart.FullMaterial = set;
        //            topPart.Type = curTopPart.Type;
        //            topPart.Where = curTopPart.Where;
        //            topPart.Mat = new Material(material);
        //            topPart.Mat.SetTexture(ColoredMaterialTextureID, curTopPart.Mat.GetTexture(MaterialTextureID));
        //            topPart.Mat.SetVector(ColoredMaterialCSHBID, CSHB);

        //            midPart.FullMaterial = set;
        //            midPart.Type = curMidPart.Type;
        //            midPart.Where = curMidPart.Where;
        //            midPart.Mat = new Material(material);
        //            midPart.Mat.SetTexture(ColoredMaterialTextureID, curMidPart.Mat.GetTexture(MaterialTextureID));
        //            midPart.Mat.SetVector(ColoredMaterialCSHBID, CSHB);

        //            materialParts.Add(topPart);
        //            materialParts.Add(midPart);
        //            nMatSet[i] = set;
        //        }
        //        return nMatSet;
        //    };

        //    Action<MaterialFamily, MaterialFamily, Material, Vector4> createColorVariation =
        //        (MaterialFamily nMatFamily, MaterialFamily materialFamily, Material material, Vector4 CSHB) =>
        //    {
        //        nMatFamily.NormalMaterials = _createColorVariation(nMatFamily, materialFamily.NormalMaterials, material, CSHB);
        //        nMatFamily.StairMaterials = _createColorVariation(nMatFamily, materialFamily.StairMaterials, material, CSHB);
        //        nMatFamily.WideMaterials = _createColorVariation(nMatFamily, materialFamily.WideMaterials, material, CSHB);
        //    };

        //    // Obtain all the names of the material textures and extract their information, transfering it into MaterialParts
        //    for (int i = 0; i < floorTextures.Length; ++i)
        //    {
        //        var floorTexture = floorTextures[i];
        //        var split = floorTexture.name.ToLower().Split('_');
        //        if (split.Length != 3)
        //            throw new Exception($"Floor texture {i} has an invalid name:'{floorTexture.name}'.");
        //        if (split[0][0] != 't')
        //            throw new Exception($"Floor texture {i}, is not a Floor texture.");
        //        var familyIDStr = split[0].Substring(1);
        //        bool parseOK = int.TryParse(familyIDStr, out int familyID);
        //        if (!parseOK)
        //            throw new Exception($"Floor texture {i}, has an invalid FamilyID.");
        //        var placement = split[1];
        //        BlockType blockType = BlockType.COUNT;
        //        Def.BlockMeshType position = Def.BlockMeshType.COUNT;
        //        switch (placement)
        //        {
        //            case "bc":
        //                blockType = BlockType.WIDE;
        //                position = Def.BlockMeshType.TOP;
        //                break;
        //            case "bl":
        //                blockType = BlockType.WIDE;
        //                position = Def.BlockMeshType.MID;
        //                break;
        //            case "sc":
        //                blockType = BlockType.NORMAL;
        //                position = Def.BlockMeshType.TOP;
        //                break;
        //            case "sl":
        //                blockType = BlockType.NORMAL;
        //                position = Def.BlockMeshType.MID;
        //                break;
        //            case "ss":
        //                blockType = BlockType.STAIRS;
        //                position = Def.BlockMeshType.TOP;
        //                break;
        //            case "ssl":
        //                blockType = BlockType.STAIRS;
        //                position = Def.BlockMeshType.MID;
        //                break;
        //            default:
        //                throw new Exception($"Floor texture {i}, has an invalid placement '{placement}'.");
        //        }
        //        int version = -1;
        //        if (split[2] != "vd")
        //        {
        //            if (split[2][0] != 'v')
        //                throw new Exception($"Floor texture {i}, has an invalid version '{split[2]}'.");
        //            var versionStr = split[2].Substring(1);
        //            parseOK = int.TryParse(versionStr, out version);
        //            if (!parseOK)
        //                throw new Exception($"Floor texture {i}, has an invalid version '{versionStr}'.");
        //        }

        //        MaterialPart part = new MaterialPart();
        //        part.Mat = new Material(materialsList[(int)Def.Materials.Default]);
        //        part.Mat.SetTexture(MaterialTextureID, floorTexture);
        //        part.Type = blockType;
        //        part.Where = position;
        //        part.FullMaterial = null;

        //        materialParts.Add(part);
        //        PartInfo partInfo;
        //        partInfo.FamilyID = familyID;
        //        partInfo.Version = version;
        //        partInfo.IsVD = version == -1;
        //        partInfos.Add(partInfo);
        //    }
        //    // First pass over all MaterialParts, create MaterialFamilies and the MaterialSets with TOP MaterialParts 
        //    for (int i = 0; i < materialParts.Count; ++i)
        //    {
        //        var partInfo = partInfos[i];
        //        var part = materialParts[i];

        //        MaterialFamily family = null;
        //        if (materialFamilies.Count <= partInfo.FamilyID)
        //        {
        //            materialFamilies.AddRange(Enumerable.Repeat<MaterialFamily>(null, (partInfo.FamilyID + 1) - materialFamilies.Count));
        //            family = new MaterialFamily
        //            {
        //                NormalMaterials = new MaterialSet[0],
        //                StairMaterials = new MaterialSet[0],
        //                WideMaterials = new MaterialSet[0]
        //            };
        //            materialFamilies[partInfo.FamilyID] = family;
        //        }
        //        else
        //        {
        //            family = materialFamilies[partInfo.FamilyID];
        //        }

        //        if (part.Where != Def.BlockMeshType.TOP)
        //            continue;

        //        MaterialSet fullMat = null;
        //        switch (part.Type)
        //        {
        //            case BlockType.NORMAL:
        //                if (family.NormalMaterials.Length <= partInfo.Version)
        //                    family.NormalMaterials = setSetLenght(partInfo.Version + 1, family.NormalMaterials);
        //                fullMat = family.NormalMaterials[partInfo.Version];
        //                if (fullMat == null)
        //                {
        //                    fullMat = new MaterialSet();
        //                    family.NormalMaterials[partInfo.Version] = fullMat;
        //                    fullMat.Family = family;
        //                }
        //                break;
        //            case BlockType.STAIRS:
        //                if (family.StairMaterials.Length <= partInfo.Version)
        //                    family.StairMaterials = setSetLenght(partInfo.Version + 1, family.StairMaterials);
        //                fullMat = family.StairMaterials[partInfo.Version];
        //                if (fullMat == null)
        //                {
        //                    fullMat = new MaterialSet();
        //                    family.StairMaterials[partInfo.Version] = fullMat;
        //                    fullMat.Family = family;
        //                }
        //                break;
        //            case BlockType.WIDE:
        //                if (family.WideMaterials.Length <= partInfo.Version)
        //                    family.WideMaterials = setSetLenght(partInfo.Version + 1, family.WideMaterials);
        //                fullMat = family.WideMaterials[partInfo.Version];
        //                if (fullMat == null)
        //                {
        //                    fullMat = new MaterialSet();
        //                    family.WideMaterials[partInfo.Version] = fullMat;
        //                    fullMat.Family = family;
        //                }
        //                break;
        //        }
        //        fullMat.TopPart = part;
        //        part.FullMaterial = fullMat;
        //    }
        //    // Second pass over all MaterialParts, set the linked MID MaterialParts in the MaterialSets
        //    for (int i = 0; i < materialParts.Count; ++i)
        //    {
        //        var part = materialParts[i];
        //        if (part.Where != Def.BlockMeshType.MID)
        //            continue;
        //        var partInfo = partInfos[i];
        //        var family = materialFamilies[partInfo.FamilyID];
        //        if (partInfo.IsVD)
        //        {
        //            switch (part.Type)
        //            {
        //                case BlockType.NORMAL:
        //                    setMIDParts(part, family.NormalMaterials);
        //                    break;
        //                case BlockType.STAIRS:
        //                    setMIDParts(part, family.StairMaterials);
        //                    break;
        //                case BlockType.WIDE:
        //                    setMIDParts(part, family.WideMaterials);
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            MaterialSet set = null;
        //            switch (part.Type)
        //            {
        //                case BlockType.NORMAL:
        //                    set = family.NormalMaterials[partInfo.Version];
        //                    break;
        //                case BlockType.STAIRS:
        //                    set = family.StairMaterials[partInfo.Version];
        //                    break;
        //                case BlockType.WIDE:
        //                    set = family.WideMaterials[partInfo.Version];
        //                    break;
        //            }
        //            set.BottomPart = part;
        //            part.FullMaterial = set;
        //        }
        //    }
        //    // Third pass over all MaterialParts, set the MID MaterialParts if they are not done
        //    for (int i = 0; i < materialFamilies.Count; ++i)
        //    {
        //        var family = materialFamilies[i];
        //        for (int j = 0; j < family.StairMaterials.Length; ++j)
        //        {
        //            var fullMat = family.StairMaterials[j];
        //            if (fullMat.BottomPart != null)
        //                continue;
        //            fullMat.BottomPart = family.NormalMaterials[j].BottomPart;
        //        }
        //    }
        //    // Find the MaterialList asset
        //    var textAssets = Resources.LoadAll<TextAsset>("Floor");
        //    TextAsset materialList = null;
        //    foreach (var textAsset in textAssets)
        //    {
        //        if (textAsset.name.ToLower() == "materiallist")
        //        {
        //            materialList = textAsset;
        //            break;
        //        }
        //    }
        //    if (materialList == null)
        //        throw new Exception("Couldn't find the MaterialList file in Resources/Floor/");// AssetContainer.TextAssets.");

        //    // Parse the MaterialList, in order to specify the caracteristics of the materials, set the material visibility and apply shader variations
        //    var text = materialList.text;
        //    var reader = new StringReader(text);
        //    var line = reader.ReadLine();
        //    int lineNum = 0;
        //    while (line != null)
        //    {
        //        var lineBlocks = line.Split(':');
        //        if (lineBlocks.Length != 4)
        //            throw new Exception($"MaterialList has incorrect format at line:{lineNum}");

        //        bool parseOK = int.TryParse(lineBlocks[0], out int materialFamilyID);
        //        if (!parseOK)
        //            throw new Exception($"MaterialList has incorrect MaterialFamilyID at line:{lineNum}");

        //        var family = materialFamilies[materialFamilyID];

        //        var colorVariations = lineBlocks[1].Split('.');
        //        var cvLength = colorVariations[0].Length == 0 ? 0 : colorVariations.Length;
        //        var nCV = new string[cvLength + 1];
        //        for (int i = 0; i < cvLength; ++i)
        //            nCV[i + 1] = colorVariations[i];
        //        colorVariations = nCV;

        //        var materialModeStr = lineBlocks[2].ToLower();
        //        var matMode = materialModeStr[0];
        //        Material matToCopy = null;
        //        Material defMaterial = materialsList[(int)Def.Materials.Default];
        //        Def.Materials materialMode = Def.Materials.Default;
        //        switch (matMode)
        //        {
        //            case 'f':
        //                {
        //                    matToCopy = materialsList[(int)Def.Materials.Fade];
        //                    materialMode = Def.Materials.Fade;
        //                }
        //                break;
        //            case 't':
        //                {
        //                    matToCopy = materialsList[(int)Def.Materials.Transparent];
        //                    materialMode = Def.Materials.Transparent;
        //                }
        //                break;
        //            case 'c':
        //                {
        //                    matToCopy = materialsList[(int)Def.Materials.Cutout];
        //                    materialMode = Def.Materials.Cutout;
        //                }
        //                break;
        //            default:
        //                break;
        //        }
        //        if (matToCopy != null)
        //        {
        //            setMaterial(matToCopy, defMaterial.name, family.NormalMaterials);
        //            setMaterial(matToCopy, defMaterial.name, family.StairMaterials);
        //            setMaterial(matToCopy, defMaterial.name, family.WideMaterials);
        //        }

        //        var names = lineBlocks[3].Split(',');
        //        if ((colorVariations.Length) != names.Length)
        //            throw new Exception("Error parsing the MaterialList, ColorVariation length mismatch the Name length.");

        //        for (int i = 1; i < names.Length; ++i) names[i] = names[i].Trim();

        //        family.FamilyName = names[0];

        //        Vector4 CSHB = Vector4.zero;
        //        for (int i = 1; i < names.Length; ++i)
        //        {
        //            var variation = colorVariations[i].Split('_');
        //            if (variation.Length != 4)
        //                throw new Exception($"Invalid Variation:{i}, in MaterialList line:{lineNum}");

        //            for (int j = 0; j < variation.Length; ++j)
        //            {
        //                parseOK = float.TryParse(variation[j], out float varVal);
        //                if (!parseOK)
        //                    throw new Exception($"Couldn't parse Variation{i}, CSHB:{j}, in MaterialList line:{lineNum}");
        //                CSHB[j] = varVal;
        //            }

        //            var nMatFamily = new MaterialFamily();
        //            materialFamilies.Add(nMatFamily);
        //            nMatFamily.NormalMaterials = new MaterialSet[0];
        //            nMatFamily.StairMaterials = new MaterialSet[0];
        //            nMatFamily.WideMaterials = new MaterialSet[0];

        //            Material coloredMat = null;
        //            switch (materialMode)
        //            {
        //                case Def.Materials.Default:
        //                    coloredMat = materialsList[(int)Def.Materials.ColoredDefault];
        //                    break;
        //                case Def.Materials.Fade:
        //                case Def.Materials.Transparent:
        //                    coloredMat = materialsList[(int)Def.Materials.ColoredTransparent];
        //                    break;
        //                case Def.Materials.Cutout:
        //                    coloredMat = materialsList[(int)Def.Materials.ColoredCutout];
        //                    break;
        //            }

        //            createColorVariation(nMatFamily, family, coloredMat, CSHB);

        //            nMatFamily.FamilyName = names[i];
        //        }

        //        ++lineNum;
        //        line = reader.ReadLine();
        //    }
        //    // Set the void material
        //    info.VoidMaterial[(int)BlockType.NORMAL] = materialFamilies[0].NormalMaterials[0];
        //    info.VoidMaterial[(int)BlockType.STAIRS] = materialFamilies[0].StairMaterials[0];
        //    info.VoidMaterial[(int)BlockType.WIDE] = materialFamilies[0].WideMaterials[0];
        //    info.MaterialFamilies = materialFamilies.ToArray();
        //    for (int i = 0; i < materialParts.Count; ++i)
        //        UnityEditor.AssetDatabase.AddObjectToAsset(materialParts[i].Mat, info);

        //    UnityEditor.AssetDatabase.SaveAssets();
        //}

        //public static void Init()
        //{
        //    VoidMat = new FullBlockMat[(int)BlockType.COUNT];
        //    BlockMaterials = new List<BlockMat>(AssetContainer.Mgr.FloorTextures.Length);
        //    MaterialTypes = new List<MaterialType>(BlockMaterials.Capacity / 3);
        //    MaterialTypeDic = new Dictionary<string, int>(MaterialTypes.Capacity);
        //    int LastBlockMatID = 0;

        //    //LastMaterialID = 0;

        //    int lastSubMaterialID = 0;
        //    //BlockMaterials.Capacity = AssetContainer.Mgr.FloorTextures.Length;
        //    List<string> vs = new List<string>(BlockMaterials.Capacity);
        //    for(int i = 0; i < AssetContainer.Mgr.FloorTextures.Length; ++i)
        //    {
        //        var floorTexture = AssetContainer.Mgr.FloorTextures[i];
        //        //if (floorTexture.name == "TVOID" || floorTexture.name == "TVOIDL")   
        //        //    continue;
        //        var texName = new StringReader(floorTexture.name);
        //        texName.Read(); // T
        //        var number = new char[3];
        //        texName.Read(number, 0, number.Length);
        //        var numberStr = new string(number);
        //        int materialSubID = int.Parse(numberStr);
        //        if (lastSubMaterialID < materialSubID)
        //            lastSubMaterialID = materialSubID;
        //        texName.Read(); // _
        //        var placementCA = new char[3];
        //        texName.Read(placementCA, 0, placementCA.Length);
        //        string placement = new string(placementCA);
        //        MaterialPlacement matPlacement;
        //        if(placement == "BC_")
        //        {
        //            matPlacement.Type = BlockType.WIDE;
        //            matPlacement.Where = Def.BlockMeshType.TOP;
        //        }
        //        else if(placement == "BL_")
        //        {
        //            matPlacement.Type = BlockType.WIDE;
        //            matPlacement.Where = Def.BlockMeshType.MID;
        //        }
        //        else if(placement == "SC_")
        //        {
        //            matPlacement.Type = BlockType.NORMAL;
        //            matPlacement.Where = Def.BlockMeshType.TOP;
        //        }
        //        else if(placement == "SL_")
        //        {
        //            matPlacement.Type = BlockType.NORMAL;
        //            matPlacement.Where = Def.BlockMeshType.MID;
        //        }
        //        else if(placement == "SS_")
        //        {
        //            matPlacement.Type = BlockType.STAIRS;
        //            matPlacement.Where = Def.BlockMeshType.TOP;
        //        }
        //        else if(placement == "SSL")
        //        {
        //            matPlacement.Type = BlockType.STAIRS;
        //            matPlacement.Where = Def.BlockMeshType.MID;
        //            texName.Read();
        //        }
        //        else
        //        {
        //            throw new Exception("Unable to recognize material list file, something went wrong iwth the placement.");
        //        }

        //        BlockMat blockMat;
        //        blockMat.MaterialID = LastBlockMatID++;
        //        blockMat.MaterialTypeID = materialSubID;
        //        blockMat.Placement = matPlacement;
        //        blockMat.BlockMaterial = new Material(Materials.GetMaterial(Def.Materials.Default));
        //        blockMat.BlockMaterial.SetTexture(Def.MaterialTextureID, floorTexture);
        //        //blockMat.BlockMaterial.mainTexture = floorTexture;
        //        BlockMaterials.Add(blockMat);
        //        vs.Add(texName.ReadToEnd());
        //    }
        //    MaterialTypes.Capacity = lastSubMaterialID + 1;
        //    MaterialTypes.AddRange(Enumerable.Repeat(new MaterialType(), lastSubMaterialID + 1));
        //    foreach(var mat in BlockMaterials)
        //    {
        //        MaterialType materialType = MaterialTypes[mat.MaterialTypeID];
        //        materialType.MaterialSubID = mat.MaterialTypeID;

        //        if(materialType.Def.Materials == null)
        //        {
        //            materialType.Def.Materials = new List<FullBlockMat>[(int)BlockType.COUNT];
        //            for (int i = 0; i < (int)BlockType.COUNT; ++i)
        //                materialType.Def.Materials[i] = new List<FullBlockMat>();
        //        }

        //        if (mat.Placement.Where != Def.BlockMeshType.TOP)
        //            continue;

        //        var v = vs[mat.MaterialID];
        //        FullBlockMat fullBlockMat;
        //        fullBlockMat.TopMat = mat.MaterialID;
        //        fullBlockMat.MidMat = -1;

        //        int vid = int.Parse(v.Substring(1));
        //        if (materialType.Def.Materials[(int)mat.Placement.Type].Count <= vid)
        //            materialType.Def.Materials[(int)mat.Placement.Type].AddRange(Enumerable.Repeat(new FullBlockMat(), (vid - materialType.Def.Materials[(int)mat.Placement.Type].Count) + 1));
        //        materialType.Def.Materials[(int)mat.Placement.Type][vid] = fullBlockMat;
        //        MaterialTypes[mat.MaterialTypeID] = materialType;
        //    }
        //    foreach (var mat in BlockMaterials)
        //    {
        //        if (mat.Placement.Where != Def.BlockMeshType.MID)
        //            continue;

        //        var materialType = MaterialTypes[mat.MaterialTypeID];
        //        var v = vs[mat.MaterialID];
        //        if (v == "VD")
        //        {
        //            for (int i = 0; i < materialType.Def.Materials[(int)mat.Placement.Type].Count; ++i)
        //            {
        //                var matType = materialType.Def.Materials[(int)mat.Placement.Type][i];
        //                matType.MidMat = mat.MaterialID;
        //                materialType.Def.Materials[(int)mat.Placement.Type][i] = matType;
        //            }
        //        }
        //        else
        //        {
        //            int id = int.Parse(v.Substring(1));
        //            var matType = materialType.Def.Materials[(int)mat.Placement.Type][id];
        //            matType.MidMat = mat.MaterialID;
        //            materialType.Def.Materials[(int)mat.Placement.Type][id] = matType;
        //        }
        //        MaterialTypes[mat.MaterialTypeID] = materialType;
        //    }
        //    foreach(var matType in MaterialTypes)
        //    {
        //        var normal = matType.Def.Materials[(int)BlockType.NORMAL];
        //        var stairs = matType.Def.Materials[(int)BlockType.STAIRS];

        //        for(int i = 0; i < stairs.Count; ++i)
        //        {
        //            if (stairs[i].MidMat != -1)
        //                continue;
        //            var full = stairs[i];
        //            full.MidMat = normal[i].MidMat;
        //            stairs[i] = full;
        //        }
        //    }
        //    TextAsset materialList = null;
        //    foreach(var textAsset in AssetContainer.Mgr.TextAssets)
        //    {
        //        if(textAsset.name == "MaterialList")
        //        {
        //            materialList = textAsset;
        //            break;
        //        }
        //    }
        //    if(materialList == null)
        //        throw new Exception("Couldn't read Material list file, materials cannot be read.");
        //    var text = materialList.text;
        //    var reader = new StringReader(text);
        //    string line = reader.ReadLine();

        //    while(line != null)
        //    {
        //        var lineBlocks = line.Split(':');
        //        if (lineBlocks.Length != 4)
        //            throw new Exception("MaterialList has incorrect format");
        //        int materialTypeID = int.Parse(lineBlocks[0]);
        //        var colorVariations = lineBlocks[1].Split('.');

        //        var cvLength = colorVariations[0].Length == 0 ? 0 : colorVariations.Length;

        //        var nCV = new string[cvLength + 1];
        //        for (int i = 0; i < cvLength; ++i)
        //            nCV[i + 1] = colorVariations[i];
        //        colorVariations = nCV;

        //        char matMode = lineBlocks[2][0];
        //        Material matToCopy = null;
        //        Material defMaterial = Materials.GetMaterial(Def.Materials.Default);
        //        Def.Materials materialMode = Def.Materials.Default;
        //        switch(matMode)
        //        {
        //            case 'F':
        //                {
        //                    matToCopy = Materials.GetMaterial(Def.Materials.Fade);
        //                    materialMode = Def.Materials.Fade;
        //                }
        //                break;
        //            case 'T':
        //                {
        //                    matToCopy = Materials.GetMaterial(Def.Materials.Transparent);
        //                    materialMode = Def.Materials.Transparent;
        //                }
        //                break;
        //            case 'C':
        //                {
        //                    matToCopy = Materials.GetMaterial(Def.Materials.Cutout);
        //                    materialMode = Def.Materials.Cutout;
        //                }
        //                break;
        //            default:
        //                break;
        //        }
        //        var materialType = MaterialTypes[materialTypeID];
        //        if (matToCopy != null)
        //        {
        //            for(int i = 0; i < materialType.Def.Materials.Length; ++i)
        //            {
        //                for(int j = 0; j < materialType.Def.Materials[i].Count; ++j)
        //                {
        //                    var topBlockMat = BlockMaterials[materialType.Def.Materials[i][j].TopMat];
        //                    var midBlockMat = BlockMaterials[materialType.Def.Materials[i][j].MidMat];

        //                    if (topBlockMat.BlockMaterial.name == defMaterial.name)
        //                    {
        //                        //var texture = topBlockMat.BlockMaterial.mainTexture;
        //                        var texture = topBlockMat.BlockMaterial.GetTexture(Def.MaterialTextureID);
        //                        UnityEngine.Object.Destroy(topBlockMat.BlockMaterial);
        //                        topBlockMat.BlockMaterial = new Material(matToCopy);
        //                        //topBlockMat.BlockMaterial.mainTexture = texture;
        //                        topBlockMat.BlockMaterial.SetTexture(Def.MaterialTextureID, texture);
        //                    }
        //                    if (midBlockMat.BlockMaterial.name == defMaterial.name)
        //                    {
        //                        //var texture = midBlockMat.BlockMaterial.mainTexture;
        //                        var texture = midBlockMat.BlockMaterial.GetTexture(Def.MaterialTextureID);
        //                        UnityEngine.Object.Destroy(midBlockMat.BlockMaterial);
        //                        midBlockMat.BlockMaterial = new Material(matToCopy);
        //                        //midBlockMat.BlockMaterial.mainTexture = texture;
        //                        midBlockMat.BlockMaterial.SetTexture(Def.MaterialTextureID, texture);
        //                    }
        //                    BlockMaterials[materialType.Def.Materials[i][j].TopMat] = topBlockMat;
        //                    BlockMaterials[materialType.Def.Materials[i][j].MidMat] = midBlockMat;
        //                }
        //            }
        //        }

        //        var names = lineBlocks[3].Split(',');
        //        if ((colorVariations.Length) != names.Length)
        //            throw new Exception("Error parsing the MaterialList, ColorVariation length mismatch the Name length.");
        //        for (int i = 1; i < names.Length; ++i) names[i] = names[i].Trim();

        //        materialType.MaterialTypeName = names[0];
        //        MaterialTypeDic.Add(materialType.MaterialTypeName, materialType.MaterialSubID);

        //        Vector4 CSHB = Vector4.zero;
        //        for(int i = 1; i < names.Length; ++i)
        //        {
        //            var variation = colorVariations[i].Split('_');
        //            if (variation.Length != 4)
        //                throw new Exception($"Invalid Variation:{i}, in MaterialType:{materialTypeID}");

        //            for(int j = 0; j < variation.Length; ++j)
        //            {
        //                bool parseOK = float.TryParse(variation[j], out float varVal);
        //                if (!parseOK)
        //                    throw new Exception($"Couldn't parse Variation{i}, CSHB:{j}, in MaterialType:{materialTypeID}");
        //                CSHB[j] = varVal;
        //            }

        //            MaterialTypes.Add(new MaterialType());
        //            var nMatType = MaterialTypes[MaterialTypes.Count - 1];
        //            nMatType.Def.Materials = new List<FullBlockMat>[(int)BlockType.COUNT];
        //            nMatType.MaterialSubID = MaterialTypes.Count - 1;
        //            for (int j = 0; j < (int)BlockType.COUNT; ++j)
        //            {
        //                nMatType.Def.Materials[j] = new List<FullBlockMat>(materialType.Def.Materials[j].Capacity);
        //                var v0BM = materialType.Def.Materials[j];
        //                for(int k = 0; k < v0BM.Count; ++k)
        //                {
        //                    var cBM = v0BM[k];
        //                    var topMat0ID = cBM.TopMat;
        //                    var midMat0ID = cBM.MidMat;
        //                    var topMat0 = BlockMaterials[topMat0ID];
        //                    var midMat0 = BlockMaterials[midMat0ID];

        //                    BlockMat topMat;
        //                    topMat.MaterialID = LastBlockMatID++;
        //                    topMat.MaterialTypeID = nMatType.MaterialSubID;
        //                    topMat.Placement = topMat0.Placement;

        //                    BlockMat midMat;
        //                    midMat.MaterialID = LastBlockMatID++;
        //                    midMat.MaterialTypeID = nMatType.MaterialSubID;
        //                    midMat.Placement = midMat0.Placement;

        //                    Material coloredMat = null;
        //                    switch(materialMode)
        //                    {
        //                        case Def.Materials.Default:
        //                            coloredMat = Materials.GetMaterial(Def.Materials.ColoredDefault);
        //                            break;
        //                        case Def.Materials.Fade:
        //                        case Def.Materials.Transparent:
        //                            coloredMat = Materials.GetMaterial(Def.Materials.ColoredTransparent);
        //                            break;
        //                        case Def.Materials.Cutout:
        //                            coloredMat = Materials.GetMaterial(Def.Materials.ColoredCutout);
        //                            break;
        //                    }
        //                    if (coloredMat == null)
        //                        throw new Exception("Couldn't find the desired ColoredMaterial for the current variation.");


        //                    topMat.BlockMaterial = new Material(coloredMat);
        //                    midMat.BlockMaterial = new Material(coloredMat);
        //                    topMat.BlockMaterial.SetTexture(AssetContainer.ColoredMaterialTextureID, topMat0.BlockMaterial.GetTexture(Def.MaterialTextureID));
        //                    midMat.BlockMaterial.SetTexture(AssetContainer.ColoredMaterialTextureID, midMat0.BlockMaterial.GetTexture(Def.MaterialTextureID));

        //                    topMat.BlockMaterial.SetVector(AssetContainer.ColoredMaterialCSHBID, CSHB);
        //                    midMat.BlockMaterial.SetVector(AssetContainer.ColoredMaterialCSHBID, CSHB);

        //                    BlockMaterials.Add(topMat);
        //                    BlockMaterials.Add(midMat);

        //                    nMatType.Def.Materials[j].Add(new FullBlockMat(topMat.MaterialID, midMat.MaterialID));
        //                }
        //            }
        //            nMatType.MaterialTypeName = names[i];
        //            MaterialTypes[nMatType.MaterialSubID] = nMatType;
        //            MaterialTypeDic.Add(nMatType.MaterialTypeName, nMatType.MaterialSubID);
        //        }
        //        MaterialTypes[materialTypeID] = materialType;
        //        line = reader.ReadLine();
        //    }
        //    for (int i = 0; i < (int)BlockType.COUNT; ++i)
        //        VoidMat[i] = MaterialTypes[0].Def.Materials[i][0];

        //    //if(Application.isEditor)
        //    //{
        //    //    EditorInit();
        //    //}
        //}

//        public static void Deinit()
//        {
//            VoidMat = null;
//            //BlockMaterials.Clear();
//            //BlockMaterials = null;
//            //MaterialTypes.Clear();
//            //MaterialTypes = null;
//            //MaterialTypeDic.Clear();
//            //MaterialTypeDic = null;
//        }
//    }
//}
