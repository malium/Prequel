/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEngine.UI;

//namespace Assets
//{
//    public enum ISType
//    {
//        NONE = -1,
//        MaterialType,
//        Prop,
//        Monster,
//        Struc,
//        Background,

//        COUNT
//    }
//    public class ImageSelector
//    {
//        List<Texture2D>[] Images;
//        List<string>[] Names;

//        struct ImageInfo
//        {
//            public string Name;
//            public int ID;
//        }
//        List<Texture2D> ViewingImages;
//        List<ImageInfo> ViewingInfo;
//        int IgnoredNum;
//        List<int> Ignored;
//        List<int> Selected;
//        string lastSearchText;
//        string SearchText;
//        float lastSearchUpdate;
//        const float SearchUpdateWait = 0.2f;
//        public bool MultiSelection;

//        static readonly string[] SelectorTypeStr = new string[(int)ISType.COUNT]
//        { "Material Selector", "Prop Selector", "Enemy Selector", "Structure Selector", "Background Selector" };

//        //int Selected;
//        Vector2 Scroll;
//        Vector2 ImageScroll;
//        float LayerHeight;

//        private ISType m_SelectorType;
//        public ISType SelectorType
//        {
//            get
//            {
//                return m_SelectorType;
//            }
//            set
//            {
//                var type = value;
//                if(type != m_SelectorType)
//                {
//                    m_SelectorType = type;
//                    FillView();
//                }
//            }
//        }

//        public ImageSelector()
//        {
//            Images = new List<Texture2D>[(int)ISType.COUNT];
//            Names = new List<string>[(int)ISType.COUNT];
//            Selected = new List<int>();
//            m_SelectorType = ISType.NONE;
//            for (int i = 0; i < (int)ISType.COUNT; ++i)
//            {
//                Images[i] = new List<Texture2D>();
//                Names[i] = new List<string>();
//            }
//        }

//        void FillView()
//        {
//            ViewingImages.Clear();
//            ViewingInfo.Clear();
//            if (m_SelectorType == ISType.NONE)
//                return;
//            IgnoredNum = 0;
//            for (int i = 0; i < Images[(int)m_SelectorType].Count; ++i)
//            {
//                bool ignore = false;
//                for(int j = 0; j < Ignored.Count; ++j)
//                {
//                    if(Ignored[j] == i)
//                    {
//                        ignore = true;
//                        break;
//                    }

//                }
//                if(ignore)
//                {
//                    ++IgnoredNum;
//                    continue;
//                }
//                var tex = Images[(int)m_SelectorType][i];
//                var name = Names[(int)m_SelectorType][i];
//                if (tex == null || name == null)
//                {
//                    ++IgnoredNum;
//                    continue;
//                }
//                ViewingImages.Add(tex);
//                ImageInfo info;
//                info.ID = i;
//                info.Name = name;
//                ViewingInfo.Add(info);
//            }
//        }
        
//        void SetStructures()
//        {
//            const int strucIdx = (int)ISType.Struc;
//            Images[strucIdx].Clear();
//            Names[strucIdx].Clear();
//            for (int i = 0; i < Structures.Strucs.Count; ++i)
//            {
//                var curStruc = Structures.Strucs[i];
//                Images[(int)ISType.Struc].Add(null);
//                Names[(int)ISType.Struc].Add(null);
//                if (curStruc == null)
//                    continue;
//                var name = curStruc.GetName();
//                if (curStruc.GetBlocks().Length == 0 || curStruc.GetLayerNum() == 0 || name.Length == 0)
//                    continue;
//                Images[(int)ISType.Struc][i] = curStruc.GetScreenshot();
//                Names[(int)ISType.Struc][i] = GameUtils.RemoveStructureID(curStruc.GetName());
//            }
//        }

//        public void Reset(List<int> ignoreIDs)
//        {
//            Ignored = ignoreIDs;
//            if(Ignored == null)
//            {
//                Ignored = new List<int>();
//            }
//            //Selected = 0;
//            Selected.Clear();
//            Scroll = Vector2.zero;
//            //ImageScroll = Vector2.zero;
//            LayerHeight = 2000.0f;
//            SearchText = "";
//            MultiSelection = true;
//            if(m_SelectorType == ISType.Struc)
//            {
//                SetStructures();
//            }
//            FillView();
//        }

//        public void Start()
//        {
//            //for (int i = 0; i < BlockMaterial.BlockMaterials.Count; ++i)
//            //{
//            //    var blockMat = BlockMaterial.BlockMaterials[i];
//            //    if (blockMat.Placement.Where != Def.BlockMeshType.TOP
//            //        || blockMat.Placement.Type != BlockType.NORMAL)
//            //        continue;

//            //    if (blockMat.MaterialTypeID == Images[(int)ISType.MaterialType].Count - 1)
//            //        continue;

//            //    var texture = blockMat.BlockMaterial.GetTexture(Def.MaterialTextureID);
//            //    if (texture == null)
//            //        texture = blockMat.BlockMaterial.GetTexture(AssetContainer.ColoredMaterialTextureID);
//            //    Images[(int)ISType.MaterialType].Add((Texture2D)texture);

//            //    Names[(int)ISType.MaterialType].Add(BlockMaterial.MaterialTypes[blockMat.MaterialTypeID].MaterialTypeName);
//            //}

//            for(int i = 0; i < BlockMaterial.MaterialFamilies.Count; ++i)
//            {
//                var curFamily = BlockMaterial.MaterialFamilies[i];
//                var blockMat = curFamily.NormalMaterials[0].TopPart.Mat;
//                var texture = blockMat.GetTexture(Def.MaterialTextureID);
//                if (texture == null)
//                    texture = blockMat.GetTexture(AssetContainer.ColoredMaterialTextureID);

//                Images[(int)ISType.MaterialType].Add((Texture2D)texture);
//                Names[(int)ISType.MaterialType].Add(curFamily.FamilyInfo.FamilyName);
//            }

//            for(int i = 0; i < Props.PropFamilies.Count; ++i)
//            {
//                var family = Props.PropFamilies[i];
//                var prop = family.Props[0];
//                Images[(int)ISType.Prop].Add(prop.PropSprite.texture);
//                Names[(int)ISType.Prop].Add(family.FamilyName);
//            }

//            for(int i = 0; i < Monsters.MonsterFamilies.Count; ++i)
//            {
//                var monster = Monsters.MonsterFamilies[i];
//                Images[(int)ISType.Monster].Add(monster.Frames[(int)Def.MonsterFrame.FACE_1].texture);
//                Names[(int)ISType.Monster].Add(monster.Name);
//            }
//            SetStructures();

//            for(int i = 0; i < Backgrounds.Infos.Count; ++i)
//            {
//                var info = Backgrounds.Infos[i];
//                Images[(int)ISType.Background].Add(info.BackgroundTexture);
//                Names[(int)ISType.Background].Add(info.name);
//            }

//            //for(int i = 0; i < AssetContainer.Mgr.BackgroundTextures.Length; ++i)
//            //{
//            //    var curTexture = AssetContainer.Mgr.BackgroundTextures[i];
//            //    Images[(int)ISType.Background].Add(curTexture);
//            //    Names[(int)ISType.Background].Add(curTexture.name);
//            //}

//            Selected = new List<int>();
//            ViewingImages = new List<Texture2D>();
//            ViewingInfo = new List<ImageInfo>();
//            lastSearchUpdate = 0.0f;
//        }

//        public void Stop()
//        {
//            for (int i = 0; i < (int)ISType.COUNT; ++i)
//            {
//                Images[i].Clear();
//                Names[i].Clear();
//            }
//            if(Ignored != null)
//                Ignored.Clear();
//            IgnoredNum = 0;
//            Selected.Clear();
//            ViewingImages.Clear();
//            ViewingInfo.Clear();
//        }

//        public List<int> GetSelected() { return Selected; }

//        void OnSearch()
//        {
//            if(SearchText.Length == 0)
//            {
//                lastSearchText = "";
//                if (ViewingImages.Count == (Images[(int)m_SelectorType].Count - IgnoredNum))
//                    return;
//                // SearchText emptied
//                FillView();
//                return;
//            }
//            if (lastSearchUpdate > Time.time)
//                return;

//            // SearchText contains info
//            if (lastSearchText == SearchText)
//                return;

//            ViewingImages.Clear();
//            ViewingInfo.Clear();

//            var searchLower = SearchText.ToLower();

//            for (int j = 1; j < Names[(int)m_SelectorType].Count; ++j)
//            {
//                bool alreadyAdded = false;
//                for (int k = 0; k < ViewingInfo.Count; ++k)
//                {
//                    if (ViewingInfo[k].ID == j)
//                    {
//                        alreadyAdded = true;
//                        break;
//                    }
//                }
//                if (alreadyAdded)
//                    continue;

//                var name = Names[(int)m_SelectorType][j];
//                if (name == null)
//                    continue;
//                bool containsAllChars = true;
//                var lowerName = name.ToLower();
//                for (int i = 0; i < searchLower.Length; ++i)
//                {
//                    var idx = lowerName.IndexOf(searchLower[i]);
//                    if (idx == -1)
//                    {
//                        containsAllChars = false;
//                        break;
//                    }
//                    if (idx == 0)
//                        lowerName = lowerName.Substring(1);
//                    else if (idx == (lowerName.Length - 1))
//                        lowerName = lowerName.Substring(0, lowerName.Length - 1);
//                    else
//                        lowerName = lowerName.Substring(0, idx) + lowerName.Substring(idx + 1);
//                }
//                if (containsAllChars)
//                {
//                    ViewingImages.Add(Images[(int)m_SelectorType][j]);
//                    ImageInfo info;
//                    info.ID = j;
//                    info.Name = name;
//                    ViewingInfo.Add(info);
//                }
//            }
//            lastSearchText = SearchText;

//            lastSearchUpdate = Time.time + SearchUpdateWait;
//        }

//        // returns true on finish
//        public bool OnGUI()
//        {
//            OnSearch();
//            var rect = Manager.Mgr.m_Canvas.pixelRect;
//            //var width = rect.width * 0.5f;
//            var height = Mathf.Min(rect.height, 600.0f);
//            var width = Mathf.Min(rect.width * 0.5f, 600.0f);
//            Scroll = GUI.BeginScrollView(new Rect(0.0f, 0.0f, width, height), Scroll, new Rect(0.0f, 0.0f, width - 20.0f, LayerHeight));
//            GUI.Box(new Rect(0.0f, 0.0f, width, LayerHeight), SelectorTypeStr[(int)m_SelectorType]);
//            SearchText = GUI.TextField(new Rect(width * 0.5f - 50.0f, 25.0f, 100.0f, 25.0f), SearchText);
//            float lastHeight = 50.0f;
//            //float gridWidth = 5.0f * 115.0f;
//            float gridHeight = Mathf.Ceil(ViewingImages.Count * 0.2f) * 140.0f;

//            ImageScroll = GUI.BeginScrollView(new Rect(0.0f, lastHeight, width, Mathf.Min(gridHeight, height - 100.0f)), ImageScroll, new Rect(0.0f, 0.0f, width - 20.0f, gridHeight));
//            for (int i = 0; i < ViewingImages.Count;)
//            {
//                float h = Mathf.Floor(i * 0.2f);
//                for (int j = 0; j < 5 && i < ViewingImages.Count; ++j, ++i)
//                {
//                    bool sel = false;
//                    for (int k = 0; k < Selected.Count; ++k)
//                    {
//                        if (Selected[k] == ViewingInfo[i].ID)
//                        {
//                            sel = true;
//                            break;
//                        }
//                    }
//                    bool nSel = false;
//                    if(MultiSelection)
//                    {
//                        nSel = GUI.Toggle(new Rect(115.0f * j, 140.0f * h, 115.0f, 115.0f), sel, ViewingImages[i]);
//                    }
//                    else
//                    {
//                        nSel = GUI.Button(new Rect(115.0f * j, 140.0f * h, 115.0f, 115.0f), ViewingImages[i]);
//                    }
//                    if (!nSel && sel)
//                    {
//                        Selected.Remove(ViewingInfo[i].ID);
//                    }
//                    else if (nSel && !sel)
//                    {
//                        Selected.Add(ViewingInfo[i].ID);

//                    }
//                    var name = ViewingInfo[i].Name;

//                    var content = new GUIContent(name);
//                    var style = GUI.skin.label;
//                    var size = style.CalcSize(content);
//                    var origFontSize = style.fontSize;
//                    if(size.x > 115.0f)
//                    {
//                        style.fontSize = Mathf.FloorToInt(style.fontSize * (115.0f / size.x));
//                        size = style.CalcSize(content);
//                    }
//                    var xOffset = 115.0f * 0.5f - size.x * 0.5f + 10.0f;
//                    if(size.x > 105.0f)
//                        xOffset -= 10.0f;

//                    GUI.Label(new Rect(115.0f * j + xOffset /*+ lenOffset*/, 140.0f * h + 105.0f, 115.0f, 35.0f), content, style);
//                    style.fontSize = origFontSize;
//                }
//            }
//            GUI.EndScrollView();
            
//            //Selected = GUI.SelectionGrid(new Rect(0.0f, lastHeight, width - 20.0f, gridHeight), Selected, Images[(int)m_SelectorType].ToArray(), 5);

//            //var close = GUI.Button(new Rect(width - 70.0f, 0.0f, 50.0f, 25.0f), "Close");
//            //if(close)
//            //{
//            //    Selected.Clear();
//            //    GUI.EndScrollView();
//            //    return true;
//            //}

//            var cancel = GUI.Button(new Rect(width * 0.5f - 50.0f, LayerHeight - 35.0f, 50.0f, 25.0f), "Cancel");
//            var apply = GUI.Button(new Rect(width * 0.5f, LayerHeight - 35.0f, 50.0f, 25.0f), "Apply");

//            if (cancel)
//            {
//                Selected.Clear();
//            }
//            if(cancel || apply || (Selected.Count > 0 && !MultiSelection))
//            {
//                GUI.EndScrollView();
//                return true;
//            }
            
//            GUI.EndScrollView();
//            LayerHeight = Mathf.Min(gridHeight + 100.0f, height);
//            return false;
//        }
//    }
//}