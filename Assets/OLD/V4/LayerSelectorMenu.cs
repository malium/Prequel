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
//    public class LayerSelectorMenu
//    {
//        const float SearchUpdateWait = 0.2f;
//        struct StructureLayers
//        {
//            public string StrucName;
//            public List<LayerInfo> Layers;
//        }
//        List<StructureLayers> StrucLayers;
//        List<int> ViewingStrucs;

//        Vector2 Scroll;
//        float GUIHeight;
//        float GUIWidth;
//        int SelectedStruc;
//        int SelectedLayer;
//        public bool SelectingLayer;
//        string lastSearchText;
//        string SearchText;
//        float lastSearchUpdate;

//        void FillView()
//        {
//            ViewingStrucs.Clear();
//            ViewingStrucs.Add(0);
//            for (int i = 1; i < StrucLayers.Count; ++i)
//                ViewingStrucs.Add(i);
//        }

//        void OnSearch()
//        {
//            if (SearchText.Length == 0)
//            {
//                lastSearchText = "";
//                if (ViewingStrucs.Count == StrucLayers.Count)
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

//            ViewingStrucs.Clear();
//            ViewingStrucs.Add(0);

//            var searchLower = SearchText.ToLower();

//            for (int j = 1; j < StrucLayers.Count; ++j)
//            {
//                var curLayer = StrucLayers[j];
//                bool alreadyAdded = false;
//                for (int k = 0; k < ViewingStrucs.Count; ++k)
//                {
//                    if (ViewingStrucs[k] == j)
//                    {
//                        alreadyAdded = true;
//                        break;
//                    }
//                }
//                if (alreadyAdded)
//                    continue;

//                var name = curLayer.StrucName;
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
//                    ViewingStrucs.Add(j);
//                }
//            }
//            lastSearchText = SearchText;

//            lastSearchUpdate = Time.time + SearchUpdateWait;
//        }

//        public void Reset()
//        {
//            SelectingLayer = false;
//            SelectedStruc = -1;
//            SelectedLayer = -1;
//            Scroll = Vector2.zero;
//            GUIHeight = 2000.0f;
//            GUIWidth = 2000.0f;
//            StrucLayers.Clear();
//            SearchText = "";
//            lastSearchText = "";
//            StructureLayers currentStrucLayers;
//            currentStrucLayers.StrucName = "Current Layers";
//            currentStrucLayers.Layers = new List<LayerInfo>(StructureComponent.LayerAmount);
//            StrucLayers.Add(currentStrucLayers);
//            var currentStrucID = Manager.Mgr.Structure.IDXIE;
//            for (int i = 0; i < Structures.Strucs.Count; ++i)
//            {
//                var struc = Structures.Strucs[i];
//                if (struc == null || (struc != null && struc.StructureID == currentStrucID))
//                    continue;

//                StructureLayers strucLayer;
//                strucLayer.StrucName = struc.Name;
//                strucLayer.Layers = new List<LayerInfo>(StructureComponent.LayerAmount);
//                var layers = struc.Layers;
//                for(int j = 0; j < layers.Count; ++j)
//                {
//                    var layer = layers[j].ToLayerInfo();
//                    if (!layer.IsValid())
//                        continue;
//                    strucLayer.Layers.Add(layer);
//                }
//                StrucLayers.Add(strucLayer);
//            }
//            FillView();
//        }

//        public void Start()
//        {
//            StrucLayers = new List<StructureLayers>(Structures.Strucs.Count);
//            ViewingStrucs = new List<int>();
//        }

//        public void Stop()
//        {
//            StrucLayers = null;
//            ViewingStrucs = null;
//        }

//        public LayerInfo GetSelectedLayer()
//        {
//            if(SelectedLayer < 0 || SelectedStruc < 0)
//            {
//                return LayerInfo.GetDefaultLayer();
//            }
//            return StrucLayers[SelectedStruc].Layers[SelectedLayer];
//        }

//        public bool OnGUI(LayerInfo[] currentLayers, int currentLayer)
//        {
//            OnSearch();
//            // currentLayers cleaning
//            {
//                var strucLayer = StrucLayers[0];
//                strucLayer.Layers = currentLayers.ToList();
//                for(int i = 0; i < strucLayer.Layers.Count;)
//                {
//                    var curLayer = strucLayer.Layers[i];
//                    if(curLayer.Layer == currentLayer || !curLayer.IsValid())
//                    {
//                        strucLayer.Layers.RemoveAt(i);
//                        continue;
//                    }
//                    ++i;
//                }
//                StrucLayers[0] = strucLayer;
//            }

//            var rect = Manager.Mgr.m_Canvas.pixelRect;
//            var width = rect.width * 0.5f;
//            var height = rect.height < 500.0f ? rect.height : 500.0f;
//            float lastLayerLeftWidth = 0.0f;
//            float lastLayerRightWidth = 0.0f;

//            GUI.Box(new Rect(0.0f, 0.0f, GUIWidth, GUIHeight), "Layer Selector");
            
//            SearchText = GUI.TextField(new Rect(GUIWidth * 0.5f - 50.0f, 25.0f, 100.0f, 25.0f), SearchText);
//            //lastHeight += 25.0f;

//            Scroll = GUI.BeginScrollView(new Rect(0.0f, 50.0f, GUIWidth, height - 50.0f), Scroll, new Rect(0.0f, 0.0f, GUIWidth - 17.0f, ViewingStrucs.Count * 50.0f));

//            float lastHeight = 0.0f;

//            for (int i = 0; i < ViewingStrucs.Count; ++i)
//            {
//                var curStrucLayer = StrucLayers[ViewingStrucs[i]];

//                var content = new GUIContent(curStrucLayer.StrucName);
//                var size = GUI.skin.label.CalcSize(content);
//                GUI.Label(new Rect(5.0f, lastHeight, size.x, 25.0f), content);
//                lastHeight += 25.0f;
//                for(int j = 0; j < curStrucLayer.Layers.Count; ++j)
//                {
//                    var sel = GUI.Button(new Rect(10.0f + j * 60.0f, lastHeight, 60.0f, 25.0f), $"Layer {curStrucLayer.Layers[j].Layer}");
//                    if(sel)
//                    {
//                        SelectedStruc = i;
//                        SelectedLayer = j;
//                    }
//                }
//                lastHeight += 25.0f;
//            }
//            lastLayerLeftWidth = Mathf.Max(lastLayerLeftWidth, StructureComponent.LayerAmount * 60.0f + 10.0f);

//            GUI.EndScrollView();

//            var cancel = GUI.Button(new Rect(GUIWidth * 0.5f - 25.0f, height + 25.0f, 50.0f, 25.0f), "Cancel");

//            GUIHeight = height + 55.0f;
//            GUIWidth = lastLayerLeftWidth + lastLayerRightWidth + 50.0f;

//            if (cancel)
//                return true;

//            if (SelectedStruc >= 0 && SelectedLayer >= 0)
//                return true;

//            return false;
//        }
//    }
//}
