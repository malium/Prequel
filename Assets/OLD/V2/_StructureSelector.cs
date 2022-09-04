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

//namespace Assets
//{
//    public class StructureSelector
//    {
//        const float SearchUpdateWait = 0.2f;

//        float Width;
//        float Height;
//        [SerializeField]
//        List<int> Strucs;
//        struct ImageInfo
//        {
//            public string Name;
//            public int ID;
//        }
//        [SerializeField]
//        List<Texture2D> ViewingImages;
//        [SerializeField]
//        List<ImageInfo> ViewingInfo;

//        float lastSearchUpdate;
//        string lastSearchText;
//        string SearchText;
//        Vector2 StrucScroll;
//        int Selected;

//        void FillView()
//        {
//            ViewingImages.Clear();
//            ViewingInfo.Clear();
//            for (int i = 0; i < Strucs.Count; ++i)
//            {
//                var curStruc = Structures.Strucs[Strucs[i]];
//                if (curStruc.m_BlockNum == 0 || curStruc.m_LayerNum == 0)
//                    continue;
//                ViewingImages.Add(curStruc.ScreenShot);
//                ImageInfo info;
//                info.ID = i;
//                info.Name = curStruc.Name;
//                ViewingInfo.Add(info);
//            }
//        }

//        void OnSearch()
//        {
//            if (SearchText.Length == 0)
//            {
//                lastSearchText = "";
//                if (ViewingImages.Count == Strucs.Count)
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

//            for (int j = 0; j < Strucs.Count; ++j)
//            {
//                var curStruc = Structures.Strucs[Strucs[j]];
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
//                if (curStruc.m_BlockNum == 0 || curStruc.m_LayerNum == 0)
//                    continue;

//                var name = curStruc.Name;
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
//                    ViewingImages.Add(curStruc.ScreenShot);
//                    ImageInfo info;
//                    info.ID = j;
//                    info.Name = curStruc.Name;
//                    ViewingInfo.Add(info);
//                }
//            }
//            lastSearchText = SearchText;

//            lastSearchUpdate = Time.time + SearchUpdateWait;
//        }

//        public void Start()
//        {
//            Strucs = new List<int>(Structures.Strucs.Count);
//            ViewingImages = new List<Texture2D>(Strucs.Capacity);
//            ViewingInfo = new List<ImageInfo>(Strucs.Capacity);
//            lastSearchUpdate = 0.0f;
//            lastSearchText = "";
//            SearchText = "";
//            Selected = -1;
//        }

//        public void Stop()
//        {
//            Strucs = null;
//            ViewingImages = null;
//            ViewingInfo = null;
//        }

//        public void Reset(int currentStrucIDX = -1)
//        {
//            Structures.LoadStrucs(false);
//            Strucs.Clear();

//            for (int i = 0; i < Structures.Strucs.Count; ++i)
//            {
//                var curStruc = Structures.Strucs[i];
//                if (curStruc == null || curStruc.StructureID == currentStrucIDX)
//                    continue;
//                if (curStruc.m_BlockNum == 0 || curStruc.m_LayerNum == 0)
//                    continue;
//                Strucs.Add(curStruc.StructureID);
//            }
//            FillView();
//            lastSearchText = "";
//            SearchText = "";
//            StrucScroll = Vector2.zero;
//            Width = 2000.0f;
//            Height = 2000.0f;
//            Selected = -1;
//        }

//        public int GetSelectedID()
//        {
//            if (Selected < 0)
//                return -1;
//            return Strucs[Selected];
//        }

//        public bool OnGUI()
//        {
//            OnSearch();
//            float scrollSize = Mathf.Ceil(Strucs.Count / 5.0f) * (128.0f + 25.0f);
//            var rect = Manager.Mgr.m_Canvas.pixelRect;
//            var pos = new Vector2(rect.width * 0.5f - Width * 0.5f, rect.height * 0.5f - Height * 0.5f);
//            float lastHeight = 0.0f;
//            float lastWidth = 0.0f;
//            const float Separation = 25.0f;

//            GUI.Box(new Rect(pos.x, pos.y, Width, Height), "Structure Selector");
//            lastHeight += Separation;
//            SearchText = GUI.TextField(new Rect(pos.x + Width * 0.5f - 50.0f, pos.y + lastHeight, 100.0f, 25.0f), SearchText);
//            lastHeight += 35.0f;
//            float yOffset = 0.0f;
//            StrucScroll = GUI.BeginScrollView(new Rect(pos.x + Separation, pos.y + lastHeight, 5.0f * 128.0f + 20.0f, 200.0f), StrucScroll, new Rect(0.0f, 0.0f, 5.0f * 128.0f, scrollSize));
//            var style = GUI.skin.label;
//            for (int y = 0; y < ViewingImages.Count;)
//            {
//                for (int x = 0; x < 5 && y < ViewingImages.Count; ++x, ++y)
//                {
//                    bool sel = GUI.Button(new Rect(x * 128.0f, yOffset, 128.0f, 128.0f), ViewingImages[y]);

//                    var content = new GUIContent(ViewingInfo[y].Name);
//                    var size = style.CalcSize(content);
//                    var origFontSize = style.fontSize;
//                    if (size.x > 128.0f)
//                    {
//                        style.fontSize = Mathf.FloorToInt(style.fontSize * (128.0f / size.x));
//                        size = style.CalcSize(content);
//                    }
//                    var xOffset = 128.0f * 0.5f - size.x * 0.5f;

//                    GUI.Label(new Rect(x * 128.0f + xOffset, yOffset + 128.0f, 128.0f, 25.0f), ViewingInfo[y].Name);
//                    style.fontSize = origFontSize;
//                    if (sel)
//                        Selected = ViewingInfo[y].ID;
//                }
//                yOffset += 128.0f + 25.0f;
//            }
//            GUI.EndScrollView();
//            lastWidth = Mathf.Max(lastWidth, 5 * 128.0f + 20.0f);
//            lastHeight += 200.0f;

//            lastHeight += 10.0f;
//            bool cancel = GUI.Button(new Rect(pos.x + Width * 0.5f - 25.0f, pos.y + lastHeight, 50.0f, 25.0f), "Cancel");
//            lastHeight += 25.0f;

//            Width = lastWidth + Separation * 2.0f;
//            Height = lastHeight;
//            if (cancel || Selected >= 0)
//                return true;
//            return false;
//        }
//    }
//}
