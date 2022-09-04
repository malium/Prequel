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
//    public class LayerEditor
//    {
//        [SerializeField]
//        LayerInfo[] TemporalLayers;
//        [SerializeField]
//        int CurrentLayer;
//        [SerializeField]
//        ImageSelector Selector;
//        [SerializeField]
//        LayerSelectorMenu LayerSelector;
//        Vector2 LayerScroll;
//        Vector2 LayerMaterialScroll;
//        Vector2 LayerPropScroll;
//        Vector2 LayerMonsterScroll;
//        float LayerHeight;
//        float LayerWidth;
//        static string[] LayerNames;
//        static string[] RotationNames = new string[] { "Default", "Left", "Half", "Right" };
//        [SerializeField]
//        List<Texture2D>[] LayerMaterials;
//        [SerializeField]
//        List<Texture2D>[] LayerProps;
//        [SerializeField]
//        List<Texture2D>[] LayerMonsters;

//        int LayerMaterialSelected;
//        int LayerPropSelected;
//        int LayerMonsterSelected;
//        float lastModificationTime;

//        public LayerEditor()
//        {
//            LayerNames = new string[Def.MaxLayerSlots];
//            for (int i = 0; i < Def.MaxLayerSlots; ++i)
//                LayerNames[i] = (i + 1).ToString();
//        }

//        void ApplyLayers()
//        {
//            var struc = Manager.Mgr.Structure;
//            for (int i = 0; i < Def.MaxLayerSlots; ++i)
//                struc.SetLayer(i + 1, TemporalLayers[i]);
//        }

//        void ClearLayer(int layer)
//        {
//            TemporalLayers[layer - 1] = LayerInfo.GetDefaultLayer();
//            TemporalLayers[layer - 1].Slot = layer;
//        }

//        public void Reset()
//        {
//            var struc = Manager.Mgr.Structure;
//            Selector.Reset(null);
//            LayerSelector.Reset();
//            for (int i = 1; i < Def.MaxLayerSlots + 1; ++i)
//            {
//                TemporalLayers[i - 1] = struc.InfoLayers[i - 1];
//            }
//            CurrentLayer = 1;
//            Selector.SelectorType = ISType.NONE;
//            LayerScroll = Vector2.zero;
//            LayerMaterialScroll = Vector2.zero;
//            LayerPropScroll = Vector2.zero;
//            LayerMonsterScroll = Vector2.zero;
//            LayerHeight = 2000.0f;
//            LayerWidth = 2000.0f;
//            LayerMaterialSelected = -1;
//            LayerPropSelected = -1;
//            LayerMonsterSelected = -1;
//            for (int i = 0; i < Def.MaxLayerSlots; ++i)
//            {
//                LayerMaterials[i].Clear();
//                LayerProps[i].Clear();
//                LayerMonsters[i].Clear();
//                var tempLayer = TemporalLayers[i];
//                for(int j = 0; j < tempLayer.MaterialFamilies.Count; ++j)
//                {
//                    var materialType = tempLayer.MaterialFamilies[j];
//                    var topMat = BlockMaterial.MaterialFamilies[materialType.ID].NormalMaterials[0].TopPart;
//                    //var topMat = BlockMaterial.MaterialTypes[materialType].Def.Materials[(int)BlockType.NORMAL][0].TopMat;

//                    //var texture = (Texture2D)BlockMaterial.BlockMaterials[topMat].BlockMaterial.GetTexture(Def.MaterialTextureID);
//                    var texture = topMat.Mat.GetTexture(Def.MaterialTextureID);
//                    if (texture == null)
//                        //texture = (Texture2D)BlockMaterial.BlockMaterials[topMat].BlockMaterial.GetTexture(AssetContainer.ColoredMaterialTextureID);
//                        texture = topMat.Mat.GetTexture(AssetContainer.ColoredMaterialTextureID);

//                    LayerMaterials[i].Add((Texture2D)texture);
//                }
//                for(int j = 0; j < tempLayer.PropFamilies.Count; ++j)
//                {
//                    var propTypeID = tempLayer.PropFamilies[j];
//                    var prop = Props.PropFamilies[propTypeID.ID].Props[0];
//                    LayerProps[i].Add(prop.sprite.texture);
//                }
//                for (int j = 0; j < tempLayer.MonsterFamilies.Count; ++j)
//                {
//                    var monsterID = tempLayer.MonsterFamilies[j];
//                    LayerMonsters[i].Add(Monsters.MonsterInfos[monsterID.ID].Sprites[(int)Def.MonsterFrame.FACE_1].texture);
//                }
//            }
//        }

//        public void Start()
//        {
//            Selector = new ImageSelector();
//            Selector.Start();
//            LayerSelector = new LayerSelectorMenu();
//            LayerSelector.Start();
//            TemporalLayers = new LayerInfo[Def.MaxLayerSlots];
//            LayerMaterials = new List<Texture2D>[Def.MaxLayerSlots];
//            LayerProps = new List<Texture2D>[Def.MaxLayerSlots];
//            LayerMonsters = new List<Texture2D>[Def.MaxLayerSlots];
//            for(int i = 0; i < Def.MaxLayerSlots; ++i)
//            {
//                LayerMaterials[i] = new List<Texture2D>();
//                LayerProps[i] = new List<Texture2D>();
//                LayerMonsters[i] = new List<Texture2D>();
//            }
//            lastModificationTime = 0.0f;
//        }

//        public void Stop()
//        {
//            Selector.Stop();
//            Selector = null;
//            LayerSelector.Stop();
//            LayerSelector = null;
//            TemporalLayers = null;
//            LayerMaterials = null;
//            LayerProps = null;
//            LayerMonsters = null;
//        }

//        public void NormalLayerGUI(ref float lastHeight, ref float lastLayerLeftWidth, ref float lastLayerRightWidth, ref LayerInfo layer)
//        {
//            GUIContent content = null;
//            Vector2 contentSize = Vector2.zero;
//            // Material selector
//            {
//                content = new GUIContent("MaterialTypes:");
//                contentSize = GUI.skin.label.CalcSize(content);
//                GUI.Label(new Rect(0.0f, lastHeight, contentSize.x, 25.0f), content);
//                lastHeight += 25.0f;
//                // Selected Material grid
//                float gridHeight = Mathf.CeilToInt(LayerMaterials[CurrentLayer - 1].Count * 0.2f) * 100.0f;
//                float gridWidth = 5.0f * 75.0f;
//                if (LayerMaterials[CurrentLayer - 1].Count > 0)
//                    lastLayerLeftWidth = Mathf.Max(lastLayerLeftWidth, gridWidth + 15.0f);
//                LayerMaterialScroll = GUI.BeginScrollView(new Rect(15.0f, lastHeight, gridWidth + 20.0f, Mathf.Min(gridHeight, 100.0f)), LayerMaterialScroll, new Rect(0.0f, 0.0f, gridWidth, gridHeight));

//                for (int i = 0; i < LayerMaterials[CurrentLayer - 1].Count;)
//                {
//                    float h = Mathf.Floor(i * 0.2f);
//                    for (int j = 0; j < 5 && i < LayerMaterials[CurrentLayer - 1].Count; ++j, ++i)
//                    {
//                        var sel = GUI.Toggle(new Rect(75.0f * j, 100.0f * h, 75.0f, 75.0f), i == LayerMaterialSelected, LayerMaterials[CurrentLayer - 1][i]);
//                        if (sel)
//                        {
//                            LayerMaterialSelected = i;
//                            LayerPropSelected = -1;
//                            LayerMonsterSelected = -1;
//                        }
//                        if (LayerMaterials[CurrentLayer - 1].Count > 1)
//                        {
//                            content = new GUIContent(layer.MaterialFamilies[i].Chance.ToString());
//                            contentSize = GUI.skin.label.CalcSize(content);
//                            float offset = 0.0f;
//                            if(contentSize.x < 75.0f)
//                            {
//                                offset = 75.0f * 0.5f - contentSize.x * 0.5f;
//                            }
//                            GUI.skin.label.wordWrap = false;
//                            GUI.Label(new Rect(75.0f * j + offset, 100.0f * h + 60.0f, 75.0f, 23.0f), content);
//                            GUI.skin.label.wordWrap = true;
//                            var chance = GUI.HorizontalSlider(new Rect(75.0f * j + 25.0f, 100.0f * h + 75.0f, 50.0f, 25.0f), layer.MaterialTypeChances[i], 0.0f, 1.0f);
//                            if (chance > layer.MaterialFamilies[i].Chance)
//                                chance = layer.MaterialFamilies[i].Chance + 0.1f;
//                            else if (chance < layer.MaterialFamilies[i].Chance)
//                                chance = layer.MaterialFamilies[i].Chance - 0.1f;

//                            if (chance != layer.MaterialFamilies[i].Chance)
//                            {
//                                layer.MaterialFamilies[i].Chance = chance;
//                                GameUtils.UpdateChances2(ref layer.MaterialFamilies);
//                            }
//                        }
//                    }
//                }

//                if (Input.GetKeyDown(KeyCode.Delete) && lastModificationTime < Time.time && LayerMaterialSelected >= 0)
//                {
//                    LayerMaterials[CurrentLayer - 1].RemoveAt(LayerMaterialSelected);
//                    layer.MaterialFamilies.RemoveAt(LayerMaterialSelected);
//                    //layer.MaterialTypeChances.RemoveAt(LayerMaterialSelected);
//                    LayerMaterialSelected = 0;
//                    lastModificationTime = Time.time + EditingStructureController.ModificationWait;
//                    GameUtils.UpdateChances2(ref layer.MaterialFamilies);
//                }
//                GUI.EndScrollView();
//                if (layer.MaterialFamilies.Count > 1)
//                {
//                    lastLayerRightWidth = Mathf.Max(lastLayerRightWidth, 100.0f);
//                    var resetChances = GUI.Button(new Rect(LayerWidth - 100.0f, lastHeight, 100.0f, 25.0f), "Reset Chances");
//                    if (resetChances)
//                    {
//                        float chance = 1.0f / layer.MaterialFamilies.Count;
//                        for (int i = 0; i < layer.MaterialFamilies.Count; ++i)
//                            layer.MaterialFamilies[i].Chance = chance;
//                    }
//                }
//                lastHeight += Mathf.Min(gridHeight, 100.0f);
//                var selectMaterial = GUI.Button(new Rect(15.0f, lastHeight, 100.0f, 25.0f), "Add Material");
//                if (selectMaterial && lastModificationTime < Time.time)
//                {
//                    List<int> ignore = new List<int>(layer.MaterialFamilies.Count);
//                    for (int i = 0; i < layer.MaterialFamilies.Count; ++i) ignore.Add(layer.MaterialFamilies[i].ID);
//                    if(!ignore.Contains(0))
//                        ignore.Add(0);
//                    Selector.Reset(ignore);
//                    Selector.SelectorType = ISType.MaterialType;
//                    lastModificationTime = Time.time + EditingStructureController.ModificationWait;
//                }
//                lastHeight += 25.0f;
//            }

//            // Block rotation
//            {
//                lastHeight += 2.0f;
//                layer.EnableRandomRotation = GUI.Toggle(new Rect(0.0f, lastHeight, 200.0f, 25.0f), layer.EnableRandomRotation, "Enable Random Rotation");
//                lastLayerLeftWidth = Mathf.Max(200.0f, lastLayerLeftWidth);
//                lastHeight += 25.0f;
//                if (!layer.EnableRandomRotation)
//                {
//                    float labelOffset = 15.0f;
//                    float labelWidth = 50.0f;
//                    GUI.Label(new Rect(labelOffset, lastHeight, labelWidth, 25.0f), "Rotation:");
//                    layer.Rotation = (Def.RotationState)GUI.SelectionGrid(new Rect(labelOffset + labelWidth + 2.0f, lastHeight, 220.0f, 25.0f), (int)layer.Rotation, RotationNames, RotationNames.Length);
//                    lastHeight += 25.0f;
//                }
//            }
//            // Generate Bridges
//            //{
//            //    lastHeight += 0.2f;
//            //    layer.GenerateBridges = GUI.Toggle(new Rect(0.0f, lastHeight, 200.0f, 25.0f), layer.GenerateBridges, "Generate Bridges");
//            //    lastLayerLeftWidth = Mathf.Max(200.0f, lastLayerLeftWidth);
//            //    lastHeight += 25.0f;
//            //}
//            // Default block height
//            {
//                lastHeight += 0.2f;
//                content = new GUIContent("Block Height:");
//                float labelWidth = GUI.skin.label.CalcSize(content).x;
//                GUI.Label(new Rect(0.0f, lastHeight, labelWidth, 25.0f), content);
//                var blockHeight = GUI.HorizontalSlider(new Rect(labelWidth + 2.5f, lastHeight + 5.0f, 100.0f, 25.0f), layer.BlockHeight, -4.0f, 4.0f);
//                content = new GUIContent(layer.BlockHeight.ToString());
//                contentSize = GUI.skin.label.CalcSize(content);
//                GUI.Label(new Rect(labelWidth + 100.0f + 5.0f, lastHeight, contentSize.x, 25.0f), content);

//                if(blockHeight > layer.BlockHeight)
//                {
//                    layer.BlockHeight += 0.5f;
//                }
//                else if(blockHeight < layer.BlockHeight)
//                {
//                    layer.BlockHeight -= 0.5f;
//                }

//                lastHeight += 25.0f;
//            }
//            // Random block micro height
//            {
//                lastHeight += 0.2f;
//                var blockHeight = GUI.Toggle(new Rect(0.0f, lastHeight, 200.0f, 25.0f), layer.EnableRandomMicroHeight, "Random block height");
//                if (blockHeight != layer.EnableRandomMicroHeight)
//                {
//                    layer.MicroHeightMin = 0f;
//                    layer.MicroHeightMax = 0f;
//                    layer.EnableRandomMicroHeight = blockHeight;
//                }
//                lastLayerLeftWidth = Mathf.Max(200.0f, lastLayerLeftWidth);
//                lastHeight += 25.0f;
//                if (layer.EnableRandomMicroHeight)
//                {
//                    var minContent = new GUIContent("Min:");
//                    var maxContent = new GUIContent("Max:");
//                    var minContentWidth = GUI.skin.label.CalcSize(minContent).x;
//                    var maxContentWidth = GUI.skin.label.CalcSize(maxContent).x;
//                    float labelWidth = Mathf.Max(minContentWidth, maxContentWidth);
                    
//                    GUI.Label(new Rect(15.0f, lastHeight, labelWidth, 25.0f), minContent);
//                    float nMin = GUI.HorizontalSlider(new Rect(15.0f + labelWidth + 2.5f, lastHeight + 5.0f, 100.0f, 25.0f), layer.MicroHeightMin, -0.25f, 0.25f);
//                    content = new GUIContent(layer.MicroHeightMin.ToString());
//                    contentSize = GUI.skin.label.CalcSize(content);
//                    GUI.Label(new Rect(15.0f + labelWidth + 100.0f + 5.0f, lastHeight, contentSize.x, 25.0f), content);
//                    lastHeight += 25.0f;
                    
//                    GUI.Label(new Rect(15.0f, lastHeight, labelWidth, 25.0f), maxContent);
//                    float nMax = GUI.HorizontalSlider(new Rect(15.0f + labelWidth + 2.5f, lastHeight + 5.0f, 100.0f, 25.0f), layer.MicroHeightMax, -0.25f, 0.25f);
//                    content = new GUIContent(layer.MicroHeightMax.ToString());
//                    contentSize = GUI.skin.label.CalcSize(content);
//                    GUI.Label(new Rect(15.0f + labelWidth + 100.0f + 5.0f, lastHeight, contentSize.x, 25.0f), content);

//                    if ((layer.MicroHeightMin != nMin || layer.MicroHeightMax != nMax) && lastModificationTime < Time.time)
//                    {

//                        if (layer.MicroHeightMin < nMin)
//                            layer.MicroHeightMin += 0.05f;
//                        else if (layer.MicroHeightMin > nMin)
//                            layer.MicroHeightMin -= 0.05f;

//                        if (layer.MicroHeightMax < nMax)
//                            layer.MicroHeightMax += 0.05f;
//                        else if (layer.MicroHeightMax > nMax)
//                            layer.MicroHeightMax -= 0.05f;

//                        if (layer.MicroHeightMin > layer.MicroHeightMax)
//                            layer.MicroHeightMin = layer.MicroHeightMax - 0.05f;

//                        layer.MicroHeightMin = Mathf.Clamp(layer.MicroHeightMin, -0.25f, 0.25f);
//                        layer.MicroHeightMax = Mathf.Clamp(layer.MicroHeightMax, -0.25f, 0.25f);
//                        lastModificationTime = Time.time + EditingStructureController.ModificationWait * 0.35f;
//                    }

//                    lastHeight += 25.0f;
//                }
//                //else
//                //{
//                //    GUI.Label(new Rect(15.0f, lastHeight, 100.0f, 25.0f), "Default height:");
//                //    GUI.Label(new Rect(115.0f, lastHeight, 25.0f, 25.0f), layer.BlockHeightMin.ToString());
//                //    float val = GUI.HorizontalSlider(new Rect(140.0f, lastHeight, 100.0f, 25.0f), layer.BlockHeightMin, -1.0f, 1.0f);
//                //    if(layer.BlockHeightMin > val)
//                //    {
//                //        layer.BlockHeightMin -= 0.5f;
//                //    }
//                //    if (layer.BlockHeightMin < val)
//                //    {
//                //        layer.BlockHeightMin += 0.5f;
//                //    }
//                //    layer.BlockHeightMin = Mathf.Clamp(layer.BlockHeightMin, -8.0f, 8.0f);
//                //    layer.BlockHeightMax = layer.BlockHeightMin;
//                //    lastHeight += 25.0f;
//                //}
//            }
//            // Random block length
//            {
//                lastHeight += 0.2f;
//                var blockLenght = GUI.Toggle(new Rect(0.0f, lastHeight, 200.0f, 25.0f), layer.EnableRandomBlockLength, "Random block length");
//                if (blockLenght != layer.EnableRandomBlockLength)
//                {
//                    layer.BlockLengthMin = 1.0f;
//                    layer.BlockLengthMax = 1.0f;
//                    layer.EnableRandomBlockLength = blockLenght;
//                }
//                lastLayerLeftWidth = Mathf.Max(200.0f, lastLayerLeftWidth);
//                lastHeight += 25.0f;
//                if (layer.EnableRandomBlockLength)
//                {
//                    var minContent = new GUIContent("Min:");
//                    var maxContent = new GUIContent("Max:");
//                    var minContentWidth = GUI.skin.label.CalcSize(minContent).x;
//                    var maxContentWidth = GUI.skin.label.CalcSize(maxContent).x;
//                    float labelWidth = Mathf.Max(minContentWidth, maxContentWidth);
                    
//                    GUI.Label(new Rect(15.0f, lastHeight, labelWidth, 25.0f), minContent);
//                    float nMin = GUI.HorizontalSlider(new Rect(15.0f + labelWidth + 2.5f, lastHeight + 5.0f, 100.0f, 25.0f), layer.BlockLengthMin, 0.5f, 3.4f);
//                    content = new GUIContent(layer.BlockLengthMin.ToString());
//                    contentSize = GUI.skin.label.CalcSize(content);
//                    GUI.Label(new Rect(15.0f + labelWidth + 100.0f + 5.0f, lastHeight, contentSize.x, 25.0f), content);
//                    lastHeight += 25.0f;
                    
//                    GUI.Label(new Rect(15.0f, lastHeight, labelWidth, 25.0f), maxContent);
//                    float nMax = GUI.HorizontalSlider(new Rect(15.0f + labelWidth + 2.5f, lastHeight + 5.0f, 100.0f, 25.0f), layer.BlockLengthMax, 0.5f, 3.4f);
//                    content = new GUIContent(layer.BlockLengthMax.ToString());
//                    contentSize = GUI.skin.label.CalcSize(content);
//                    GUI.Label(new Rect(15.0f + labelWidth + 100.0f + 5.0f, lastHeight, contentSize.x, 25.0f), content);

//                    if ((layer.BlockLengthMin != nMin || layer.BlockLengthMax != nMax) && lastModificationTime < Time.time)
//                    {

//                        if (layer.BlockLengthMin < nMin)
//                        {
//                            layer.BlockLengthMin += 0.5f;
//                        }
//                        else if (layer.BlockLengthMin > nMin)
//                        {
//                            if (layer.BlockLengthMin == 3.4f)
//                                layer.BlockLengthMin = 3.0f;
//                            else
//                                layer.BlockLengthMin -= 0.5f;
//                        }
//                        if (layer.BlockLengthMax < nMax)
//                        {
//                            layer.BlockLengthMax += 0.5f;
//                        }
//                        else if (layer.BlockLengthMax > nMax)
//                        {
//                            if (layer.BlockLengthMax == 3.4f)
//                                layer.BlockLengthMax = 3.0f;
//                            else
//                                layer.BlockLengthMax -= 0.5f;
//                        }

//                        if (layer.BlockLengthMin > layer.BlockLengthMax)
//                            layer.BlockLengthMin = layer.BlockLengthMax - 0.5f;

//                        layer.BlockLengthMin = Mathf.Clamp(layer.BlockLengthMin, 0.5f, 3.4f);
//                        layer.BlockLengthMax = Mathf.Clamp(layer.BlockLengthMax, 0.5f, 3.4f);
//                        lastModificationTime = Time.time + EditingStructureController.ModificationWait * 0.35f;
//                    }

//                    lastHeight += 25.0f;
//                }
//                else
//                {
//                    content = new GUIContent("Default Length:");
//                    float labelWidth = GUI.skin.label.CalcSize(content).x;
//                    GUI.Label(new Rect(15.0f, lastHeight, labelWidth, 25.0f), content);
//                    float val = GUI.HorizontalSlider(new Rect(15.0f + labelWidth + 2.5f, lastHeight + 5.0f, 100.0f, 25.0f), layer.BlockLengthMin, 0.5f, 3.4f);
//                    content = new GUIContent(layer.BlockLengthMin.ToString());
//                    contentSize = GUI.skin.label.CalcSize(content);
//                    GUI.Label(new Rect(15.0f + labelWidth + 100.0f + 5.0f, lastHeight, contentSize.x, 25.0f), content);

//                    if (layer.BlockLengthMin > val)
//                    {
//                        layer.BlockLengthMin -= 0.5f;
//                    }
//                    if (layer.BlockLengthMin < val)
//                    {
//                        layer.BlockLengthMin += 0.5f;
//                    }
//                    if (layer.BlockLengthMin == 3.9f)
//                        layer.BlockLengthMin = 3.4f;
//                    else if (layer.BlockLengthMin == 2.9f)
//                        layer.BlockLengthMin = 3.0f;

//                    layer.BlockLengthMin = Mathf.Clamp(layer.BlockLengthMin, 0.5f, 3.4f);

//                    layer.BlockLengthMax = layer.BlockLengthMin;
//                    lastHeight += 25.0f;
//                }
//            }
//            // Random Props
//            {
//                lastHeight += 0.2f;
//                layer.EnableRandomProps = GUI.Toggle(new Rect(0.0f, lastHeight, 100.0f, 25.0f), layer.EnableRandomProps, "Random Props");
//                lastLayerLeftWidth = Mathf.Max(100.0f, lastLayerLeftWidth);
//                lastHeight += 25.0f;
//                if (layer.EnableRandomProps)
//                {
//                    //if (LayerProps[CurrentLayer - 1].Count > 1)
//                    // Spawn chance
//                    {
//                        content = new GUIContent("Spawn Chance:");
//                        float labelWidth = GUI.skin.label.CalcSize(content).x;
//                        GUI.Label(new Rect(15.0f, lastHeight, labelWidth, 25.0f), content);
//                        layer.PropGeneralChance = GUI.HorizontalSlider(new Rect(15.0f + labelWidth + 2.5f, lastHeight + 5.0f, 100.0f, 25.0f), layer.PropGeneralChance, 0.0f, 1.0f);
//                        content = new GUIContent(layer.PropGeneralChance.ToString());
//                        contentSize = GUI.skin.label.CalcSize(content);
//                        GUI.Label(new Rect(15.0f + labelWidth + 100.0f + 5.0f, lastHeight, contentSize.x, 25.0f), content);
                        
//                        lastLayerLeftWidth = Mathf.Max(lastLayerLeftWidth, 120.0f + labelWidth + contentSize.x);
//                        lastHeight += 25.0f;
//                    }
//                    // NoSpawn radius
//                    {
//                        content = new GUIContent("NoSpawn Radius:");
//                        float labelWidth = GUI.skin.label.CalcSize(content).x;
//                        GUI.Label(new Rect(15.0f, lastHeight, labelWidth, 25.0f), content);
//                        var radius = GUI.HorizontalSlider(new Rect(15.0f + labelWidth + 2.5f, lastHeight + 5.0f, 100.0f, 25.0f), layer.PropNoSpawnRadius, 0.0f, 8.0f);
//                        content = new GUIContent(layer.PropSafetyDistance.ToString());
//                        contentSize = GUI.skin.label.CalcSize(content);
//                        GUI.Label(new Rect(15.0f + labelWidth + 100.0f + 5.0f, lastHeight, contentSize.x, 25.0f), content);

//                        if (radius > layer.PropSafetyDistance)
//                            layer.PropSafetyDistance += 0.5f;
//                        else if (radius < layer.PropSafetyDistance)
//                            layer.PropSafetyDistance -= 0.5f;

//                        lastHeight += 25.0f;
//                    }
//                    // Selected Prop grid
//                    float gridHeight = Mathf.CeilToInt(LayerProps[CurrentLayer - 1].Count * 0.2f) * 100.0f;
//                    float gridWidth = 5.0f * 75.0f;
//                    if (LayerProps[CurrentLayer - 1].Count > 0)
//                        lastLayerLeftWidth = Mathf.Max(lastLayerLeftWidth, gridWidth + 15.0f);
//                    LayerPropScroll = GUI.BeginScrollView(new Rect(15.0f, lastHeight, gridWidth + 20.0f, Mathf.Min(gridHeight, 100.0f)), LayerPropScroll, new Rect(0.0f, 0.0f, gridWidth, gridHeight));

//                    for (int i = 0; i < LayerProps[CurrentLayer - 1].Count;)
//                    {
//                        float h = Mathf.Floor(i * 0.2f);
//                        for (int j = 0; j < 5 && i < LayerProps[CurrentLayer - 1].Count; ++j, ++i)
//                        {
//                            var sel = GUI.Toggle(new Rect(75.0f * j, 100.0f * h, 75.0f, 75.0f), i == LayerPropSelected, LayerProps[CurrentLayer - 1][i]);
//                            if (sel)
//                            {
//                                LayerPropSelected = i;
//                                LayerMaterialSelected = -1;
//                                LayerMonsterSelected = -1;
//                            }
//                            if (LayerProps[CurrentLayer - 1].Count > 1)
//                            {
//                                content = new GUIContent(layer.PropFamilies[i].Chance.ToString());
//                                contentSize = GUI.skin.label.CalcSize(content);
//                                float offset = 0.0f;
//                                if (contentSize.x < 75.0f)
//                                {
//                                    offset = 75.0f * 0.5f - contentSize.x * 0.5f;
//                                }
//                                GUI.skin.label.wordWrap = false;
//                                GUI.Label(new Rect(75.0f * j + offset, 100.0f * h + 60.0f, 75.0f, 23.0f), content);
//                                GUI.skin.label.wordWrap = true;

//                                var chance = GUI.HorizontalSlider(new Rect(75.0f * j + 25.0f, 100.0f * h + 75.0f, 50.0f, 25.0f), layer.PropFamilies[i].Chance, 0.0f, 1.0f);
//                                if (chance > layer.PropFamilies[i].Chance)
//                                    chance = layer.PropFamilies[i].Chance + 0.1f;
//                                else if (chance < layer.PropFamilies[i].Chance)
//                                    chance = layer.PropFamilies[i].Chance - 0.1f;

//                                if (chance != layer.PropFamilies[i].Chance)
//                                {
//                                    layer.PropFamilies[i].Chance = chance;
//                                    GameUtils.UpdateChances2(ref layer.PropFamilies);
//                                }
//                            }
//                        }
//                    }
//                    if (Input.GetKeyDown(KeyCode.Delete) && lastModificationTime < Time.time && LayerPropSelected >= 0)
//                    {
//                        LayerProps[CurrentLayer - 1].RemoveAt(LayerPropSelected);
//                        layer.PropFamilies.RemoveAt(LayerPropSelected);
//                        //layer.PropChances.RemoveAt(LayerPropSelected);
//                        LayerPropSelected = 0;
//                        lastModificationTime = Time.time + EditingStructureController.ModificationWait;
//                        GameUtils.UpdateChances2(ref layer.PropFamilies);
//                    }

//                    GUI.EndScrollView();
//                    if (layer.PropFamilies.Count > 1)
//                    {
//                        lastLayerRightWidth = Mathf.Max(lastLayerRightWidth, 100.0f);
//                        var resetChances = GUI.Button(new Rect(LayerWidth - 100.0f, lastHeight, 100.0f, 25.0f), "Reset Chances");
//                        if (resetChances)
//                        {
//                            float chance = 1.0f / layer.PropFamilies.Count;
//                            for (int i = 0; i < layer.PropFamilies.Count; ++i)
//                                layer.PropFamilies[i].Chance = chance;
//                        }
//                    }
//                    lastHeight += Mathf.Min(gridHeight, 100.0f);
//                    var selectProp = GUI.Button(new Rect(15.0f, lastHeight, 100.0f, 25.0f), "Add Prop");
//                    if (selectProp && lastModificationTime < Time.time)
//                    {
//                        List<int> ignore = new List<int>(layer.PropFamilies.Count + 1);
//                        for (int i = 0; i < layer.PropFamilies.Count; ++i) ignore.Add(layer.PropFamilies[i].ID);
//                        if(!ignore.Contains(0))
//                            ignore.Add(0);
//                        Selector.Reset(ignore);
//                        Selector.SelectorType = ISType.Prop;
//                        lastModificationTime = Time.time + EditingStructureController.ModificationWait;
//                    }
//                    lastHeight += 25.0f;
//                }
//            }
//            // Generate Monsters
//            {
//                lastHeight += 0.2f;
//                layer.SpawnMonsters = GUI.Toggle(new Rect(0.0f, lastHeight, 115.0f, 25.0f), layer.SpawnMonsters, "Spawn Monsters");
//                lastLayerLeftWidth = Mathf.Max(115.0f, lastLayerLeftWidth);
//                lastHeight += 25.0f;
//                if(layer.SpawnMonsters)
//                {
//                    // Spawn Zone Monsters
//                    {
//                        layer.SpawnZoneMonsters = GUI.Toggle(new Rect(15.0f, lastHeight, 150.0f, 25.0f), layer.SpawnZoneMonsters, "Spawn Zone Monsters");
//                        lastHeight += 25.0f;
//                    }
//                    // Spawn chance
//                    {
//                        content = new GUIContent("Spawn Chance:");
//                        float labelWidth = GUI.skin.label.CalcSize(content).x;
//                        GUI.Label(new Rect(15.0f, lastHeight, labelWidth, 25.0f), content);
//                        layer.MonsterGeneralChance = GUI.HorizontalSlider(new Rect(15.0f + labelWidth + 2.5f, lastHeight + 5.0f, 100.0f, 25.0f), layer.MonsterGeneralChance, 0.0f, 1.0f);
//                        content = new GUIContent(layer.MonsterGeneralChance.ToString());
//                        contentSize = GUI.skin.label.CalcSize(content);
//                        GUI.Label(new Rect(15.0f + labelWidth + 100.0f + 5.0f, lastHeight, contentSize.x, 25.0f), content);
                        
//                        layer.MonsterGeneralChance = Mathf.Floor(layer.MonsterGeneralChance *= 1000.0f) * 0.001f;

//                        lastLayerLeftWidth = Mathf.Max(lastLayerLeftWidth, 190.0f + 100.0f);
//                        lastHeight += 25.0f;
//                    }

//                    // Select Monster
//                    float gridHeight = Mathf.CeilToInt(LayerMonsters[CurrentLayer - 1].Count * 0.2f) * 100.0f;
//                    float gridWidth = 5.0f * 75.0f;
//                    if (LayerMonsters[CurrentLayer - 1].Count > 0)
//                        lastLayerLeftWidth = Mathf.Max(lastLayerLeftWidth, gridWidth + 15.0f);
//                    LayerMonsterScroll = GUI.BeginScrollView(new Rect(15.0f, lastHeight, gridWidth + 20.0f, Mathf.Min(gridHeight, 100.0f)), LayerMonsterScroll, new Rect(0.0f, 0.0f, gridWidth, gridHeight));

//                    for (int i = 0; i < LayerMonsters[CurrentLayer - 1].Count;)
//                    {
//                        float h = Mathf.Floor(i * 0.2f);
//                        for (int j = 0; j < 5 && i < LayerMonsters[CurrentLayer - 1].Count; ++j, ++i)
//                        {
//                            var sel = GUI.Toggle(new Rect(75.0f * j, 100.0f * h, 75.0f, 75.0f), i == LayerMonsterSelected, LayerMonsters[CurrentLayer - 1][i]);
//                            if (sel)
//                            {
//                                LayerPropSelected = -1;
//                                LayerMaterialSelected = -1;
//                                LayerMonsterSelected = i;
//                            }
//                            if (LayerMonsters[CurrentLayer - 1].Count > 1)
//                            {
//                                content = new GUIContent(layer.MonsterFamilies[i].Chance.ToString());
//                                contentSize = GUI.skin.label.CalcSize(content);
//                                float offset = 0.0f;
//                                if (contentSize.x < 75.0f)
//                                {
//                                    offset = 75.0f * 0.5f - contentSize.x * 0.5f;
//                                }
//                                GUI.skin.label.wordWrap = false;
//                                GUI.Label(new Rect(75.0f * j + offset, 100.0f * h + 60.0f, 75.0f, 23.0f), content);
//                                GUI.skin.label.wordWrap = true;
                                
//                                var chance = GUI.HorizontalSlider(new Rect(75.0f * j + 25.0f, 100.0f * h + 75.0f, 50.0f, 25.0f), layer.MonsterFamilies[i].Chance, 0.0f, 1.0f);
//                                if (chance > layer.MonsterFamilies[i].Chance)
//                                    chance = layer.MonsterFamilies[i].Chance + 0.1f;
//                                else if (chance < layer.MonsterFamilies[i].Chance)
//                                    chance = layer.MonsterFamilies[i].Chance - 0.1f;

//                                if (chance != layer.MonsterFamilies[i].Chance)
//                                {
//                                    layer.MonsterFamilies[i].Chance = chance;
//                                    GameUtils.UpdateChances2(ref layer.MonsterFamilies);
//                                }
//                            }
//                        }
//                    }
//                    if (Input.GetKeyDown(KeyCode.Delete) && lastModificationTime < Time.time && LayerMonsterSelected >= 0)
//                    {
//                        LayerMonsters[CurrentLayer - 1].RemoveAt(LayerMonsterSelected);
//                        layer.MonsterFamilies.RemoveAt(LayerMonsterSelected);
//                        //layer.MonsterChances.RemoveAt(LayerMonsterSelected);
//                        LayerMonsterSelected = 0;
//                        lastModificationTime = Time.time + EditingStructureController.ModificationWait;
//                        GameUtils.UpdateChances2(ref layer.MonsterFamilies);
//                    }

//                    GUI.EndScrollView();
//                    if(layer.MonsterFamilies.Count > 0)
//                    {
//                        layer.LayerMonstersRespawn = GUI.Toggle(new Rect(LayerWidth - 175.0f, lastHeight + 25.0f, 175.0f, 25.0f), layer.LayerMonstersRespawn, "Layer Monsters Respawn");
//                        lastLayerRightWidth = Mathf.Max(lastLayerRightWidth, 175.0f);
//                    }
//                    if (layer.MonsterFamilies.Count > 1)
//                    {
//                        lastLayerRightWidth = Mathf.Max(lastLayerRightWidth, 100.0f);
//                        var resetChances = GUI.Button(new Rect(LayerWidth - 100.0f, lastHeight, 100.0f, 25.0f), "Reset Chances");
//                        if (resetChances)
//                        {
//                            float chance = 1.0f / layer.MonsterFamilies.Count;
//                            for (int i = 0; i < layer.MonsterFamilies.Count; ++i)
//                                layer.MonsterFamilies[i].Chance = chance;
//                        }
                        
//                    }
//                    lastHeight += Mathf.Min(gridHeight, 100.0f);
//                    var selectMonster = GUI.Button(new Rect(15.0f, lastHeight, 100.0f, 25.0f), "Add Monster");
//                    if (selectMonster && lastModificationTime < Time.time)
//                    {
//                        List<int> ignore = new List<int>(layer.MonsterFamilies.Count + 1);
//                        for (int i = 0; i < layer.MonsterFamilies.Count; ++i) ignore.Add(layer.MonsterFamilies[i].ID);
//                            if(!ignore.Contains(0))
//                        ignore.Add(0);
//                        Selector.Reset(ignore);
//                        Selector.SelectorType = ISType.Monster;
//                        lastModificationTime = Time.time + EditingStructureController.ModificationWait;
//                    }
//                    lastHeight += 25.0f;
//                }
//            }
//            // Effects
//            {
//                lastHeight += 0.2f;
//                layer.HasEffects = GUI.Toggle(new Rect(0.0f, lastHeight, 100.0f, 25.0f), layer.HasEffects, "Has effects");
//                lastLayerLeftWidth = Mathf.Max(100.0f, lastLayerLeftWidth);
//                lastHeight += 25.0f;
//            }
//            // Block float
//            {
//                lastHeight += 0.2f;
//                layer.EnableBlockFloat = GUI.Toggle(new Rect(0.0f, lastHeight, 200.0f, 25.0f), layer.EnableBlockFloat, "Enable block float");
//                lastLayerLeftWidth = Mathf.Max(200.0f, lastLayerLeftWidth);
//                lastHeight += 25.0f;
//                if(layer.EnableBlockFloat)
//                {

//                }

//            }
//            // Random wide and stair chance
//            {
//                var wideContent = new GUIContent("Random WideBlock chance:");
//                var stairContent = new GUIContent("Random Stair chance:");
//                var minContentWidth = GUI.skin.label.CalcSize(wideContent).x;
//                var maxContentWidth = GUI.skin.label.CalcSize(stairContent).x;
//                float labelWidth = Mathf.Max(minContentWidth, maxContentWidth);

//                GUI.Label(new Rect(0.0f, lastHeight, labelWidth, 25.0f), wideContent);
//                layer.WideBlockChance = GUI.HorizontalSlider(new Rect(labelWidth + 2.5f, lastHeight + 5.0f, 100.0f, 25.0f), layer.WideBlockChance, 0.0f, 1.0f);
//                content = new GUIContent(layer.WideBlockChance.ToString());
//                contentSize = GUI.skin.label.CalcSize(content);
//                GUI.Label(new Rect(labelWidth + 100.0f + 5.0f, lastHeight, contentSize.x, 25.0f), content);
//                lastHeight += 25.0f;

//                GUI.Label(new Rect(0.0f, lastHeight, labelWidth, 25.0f), stairContent);
//                layer.StairBlockChance = GUI.HorizontalSlider(new Rect(labelWidth + 2.5f, lastHeight + 5.0f, 100.0f, 25.0f), layer.StairBlockChance, 0.0f, 1.0f);
//                content = new GUIContent(layer.StairBlockChance.ToString());
//                contentSize = GUI.skin.label.CalcSize(content);
//                GUI.Label(new Rect(labelWidth + 100.0f + 5.0f, lastHeight, contentSize.x, 25.0f), content);
//                lastHeight += 25.0f;


//                var chance100 = layer.WideBlockChance * 100.0f;
//                var ichance100 = Mathf.Floor(chance100);
//                layer.WideBlockChance = ichance100 * 0.01f;

//                chance100 = layer.StairBlockChance * 100.0f;
//                ichance100 = Mathf.Floor(chance100);
//                layer.StairBlockChance = ichance100 * 0.01f;

//                lastLayerLeftWidth = Mathf.Max(lastLayerLeftWidth, labelWidth + 105.0f + Mathf.Max(contentSize.x, 75.0f));
//            }
//            //{
//            //    lastHeight += 0.2f;
//            //    GUI.Label(new Rect(0.0f, lastHeight, 200.0f, 25.0f), "Random WideBlock chance:");
//            //    GUI.skin.label.wordWrap = false;
//            //    GUI.Label(new Rect(200.0f, lastHeight, 50.0f, 25.0f), layer.RandomWideBlockChance.ToString());
//            //    GUI.skin.label.wordWrap = true;
//            //    layer.RandomWideBlockChance = GUI.HorizontalSlider(new Rect(250.0f, lastHeight, 100.0f, 25.0f), layer.RandomWideBlockChance, 0.0f, 1.0f);
//            //    var chance100 = layer.RandomWideBlockChance * 100.0f;
//            //    var ichance100 = Mathf.Floor(chance100);
//            //    layer.RandomWideBlockChance = ichance100 * 0.01f;
//            //    lastLayerLeftWidth = Mathf.Max(lastLayerLeftWidth, 350.0f);
//            //    lastHeight += 25.0f;
//            //}
//            //// Random stair chance
//            //{
//            //    lastHeight += 0.2f;
//            //    GUI.Label(new Rect(0.0f, lastHeight, 200.0f, 25.0f), "Random Stair chance:");
//            //    GUI.skin.label.wordWrap = false;
//            //    GUI.Label(new Rect(200.0f, lastHeight, 50.0f, 25.0f), layer.RandomStairBlockChance.ToString());
//            //    GUI.skin.label.wordWrap = true;
//            //    layer.RandomStairBlockChance = GUI.HorizontalSlider(new Rect(250.0f, lastHeight, 100.0f, 25.0f), layer.RandomStairBlockChance, 0.0f, 1.0f);
//            //    var chance100 = layer.RandomStairBlockChance * 100.0f;
//            //    var ichance100 = Mathf.Floor(chance100);
//            //    layer.RandomStairBlockChance = ichance100 * 0.01f;
//            //    lastLayerLeftWidth = Mathf.Max(lastLayerLeftWidth, 350.0f);
//            //    lastHeight += 25.0f;
//            //}
//        }

//        public void LinkedLayerGUI(ref float lastHeight, ref float lastLayerLeftWidth, ref float lastLayerRightWidth, ref LayerInfo layer)
//        {
//            var struc = Manager.Mgr.Structure;
//            var layers = new List<int>(7);
//            for (int i = 0; i < Def.MaxLayerSlots; ++i)
//            {
//                if ((i + 1) == layer.Slot)
//                    continue;
//                var infoLayer = struc.InfoLayers[i];
//                if (infoLayer.IsLinkedLayer || !infoLayer.IsValid())
//                    continue;

//                layers.Add(infoLayer.Layer);
//            }
//            lastHeight += 2.0f;
//            if (layers.Count <= 1)
//            {
//                GUI.Label(new Rect(15.0f, lastHeight, 200.0f, 25.0f), "Not enough valid layers available");
//                lastHeight += 25.0f;
//                return;
//            }
//            if (layer.LinkedLayers.Count > 1)
//            {
//                lastLayerLeftWidth = Mathf.Max(lastLayerLeftWidth, 165.0f + 100.0f);
//                bool reset = GUI.Button(new Rect(LayerWidth - 100.0f, lastHeight, 100.0f, 25.0f), "Reset Chances");
//                if (reset)
//                {
//                    float chance = 1.0f / layer.LinkedLayers.Count;
//                    for (int i = 0; i < layer.LinkedLayers.Count; ++i)
//                        layer.LinkedLayers[i].Chance = chance;
//                }
//            }
//            for (int i = 0; i < layers.Count; ++i)
//            {
//                var name = "Layer " + layers[i].ToString();
//                bool wasSelected = false;
//                for(int j = 0; j < layer.LinkedLayers.Count; ++j)
//                {
//                    if(layer.LinkedLayers[j].ID == layers[i])
//                    {
//                        wasSelected = true;
//                        break;
//                    }
//                }
//                bool selected = GUI.Toggle(new Rect(15.0f, lastHeight, 100.0f, 25.0f), wasSelected, name);
//                int selIdx = -1;
//                for (int j = 0; j < layer.LinkedLayers.Count; ++j)
//                {
//                    if (layer.LinkedLayers[j].ID == layers[i])
//                    {
//                        selIdx = j;
//                        break;
//                    }
//                }
//                if (selected && !wasSelected)
//                {
//                    layer.LinkedLayers.Add(new IDChance() { ID = layers[i], Chance = 0 });
//                    if(layer.LinkedLayers.Count == 1)
//                    {
//                        layer.LinkedLayers[0] = new IDChance() { ID = layers[i], Chance = 10000 };
//                    }
//                    else
//                    {
//                        layer.LinkedLayers[layer.LinkedLayers.Count - 1] = new IDChance() { ID = layers[i], Chance = layer.LinkedLayers.Average() };
//                        //layer.LinkedLayerChances.Add(layer.LinkedLayerChances.Average());
//                        GameUtils.UpdateChances2(ref layer.LinkedLayers);
//                    }
//                    selIdx = layer.LinkedLayers.Count - 1;
//                }
//                else if(!selected && wasSelected)
//                {
//                    layer.LinkedLayers.RemoveAt(selIdx);
//                    layer.LinkedLayerChances.RemoveAt(selIdx);
//                    GameUtils.UpdateChances(ref layer.LinkedLayerChances);
//                }
//                if(selected && layer.LinkedLayerChances.Count > 1)
//                {
//                    GUI.skin.label.wordWrap = false;
//                    GUI.Label(new Rect(115.0f, lastHeight, 50.0f, 20.0f), layer.LinkedLayerChances[selIdx].ToString());
//                    GUI.skin.label.wordWrap = true;
//                    float nChance = GUI.HorizontalSlider(new Rect(165.0f, lastHeight, 100.0f, 25.0f), layer.LinkedLayerChances[selIdx], 0.0f, 1.0f);
//                    if (nChance > layer.LinkedLayerChances[selIdx])
//                        nChance = layer.LinkedLayerChances[selIdx] + 0.1f;
//                    else if (nChance < layer.LinkedLayerChances[selIdx])
//                        nChance = layer.LinkedLayerChances[selIdx] - 0.1f;

//                    if(nChance != layer.LinkedLayerChances[selIdx])
//                    {
//                        layer.LinkedLayerChances[selIdx] = nChance;
//                        GameUtils.UpdateChances(ref layer.LinkedLayerChances);
//                    }
                    
//                }
//                lastHeight += 25.0f;
//            }
            
//        }

//        // returns true on finish
//        public bool OnGUI()
//        {
//            if (Selector.SelectorType == ISType.MaterialType)
//            {
//                bool sel = Selector.OnGUI();
//                if(sel)
//                {
//                    var selected = Selector.GetSelected();
//                    for (int i = 0; i < selected.Count; ++i)
//                    {
//                        var selID = selected[i];
//                        var mat = BlockMaterial.MaterialFamilies[selID].NormalMaterials[0];
//                        //var mat = BlockMaterial.MaterialTypes[selID].Def.Materials[(int)BlockType.NORMAL][0];
//                        var tempLayer = TemporalLayers[CurrentLayer - 1];
//                        if (!tempLayer.MaterialTypes.Contains(selID))
//                        {
//                            var texture = mat.TopPart.Mat.GetTexture(Def.MaterialTextureID);
//                            if (texture == null)
//                                texture = mat.TopPart.Mat.GetTexture(AssetContainer.ColoredMaterialTextureID);

//                            //var texture = BlockMaterial.BlockMaterials[mat.TopMat].BlockMaterial.GetTexture(Def.MaterialTextureID);
//                            //if (texture == null)
//                            //    texture = BlockMaterial.BlockMaterials[mat.TopMat].BlockMaterial.GetTexture(AssetContainer.ColoredMaterialTextureID);

//                            LayerMaterials[CurrentLayer - 1].Add((Texture2D)texture);
//                            tempLayer.MaterialTypes.Add(selID);
//                            if (tempLayer.MaterialTypeChances.Count == 0)
//                            {
//                                tempLayer.MaterialTypeChances.Add(1.0f);
//                            }
//                            else
//                            {
//                                tempLayer.MaterialTypeChances.Add(tempLayer.MaterialTypeChances.Average());
//                                GameUtils.UpdateChances(ref tempLayer.MaterialTypeChances);
//                            }
//                            TemporalLayers[CurrentLayer - 1] = tempLayer;
//                        }
//                    }
//                    Selector.SelectorType = ISType.NONE;
//                }
//                return false;
//            }
//            else if(Selector.SelectorType == ISType.Prop)
//            {
//                bool sel = Selector.OnGUI();
//                if (sel)
//                {
//                    var selected = Selector.GetSelected();
//                    for (int i = 0; i < selected.Count; ++i)
//                    {
//                        var selID = selected[i];
//                        var prop = Props.PropFamilies[selID].Props[0];
//                        var tempLayer = TemporalLayers[CurrentLayer - 1];
//                        if (!tempLayer.Props.Contains(selID))
//                        {
//                            LayerProps[CurrentLayer - 1].Add(prop.sprite.texture);
//                            tempLayer.Props.Add(selID);
//                            if (tempLayer.PropChances.Count == 0)
//                            {
//                                tempLayer.PropChances.Add(1.0f);
//                            }
//                            else
//                            {
//                                tempLayer.PropChances.Add(tempLayer.PropChances.Average());
//                                GameUtils.UpdateChances(ref tempLayer.PropChances);
//                            }
//                            TemporalLayers[CurrentLayer - 1] = tempLayer;
//                        }
//                    }
//                    Selector.SelectorType = ISType.NONE;
//                }
//                return false;
//            }
//            else if(Selector.SelectorType == ISType.Monster)
//            {
//                bool sel = Selector.OnGUI();
//                if (sel)
//                {
//                    var selected = Selector.GetSelected();
//                    for (int i = 0; i < selected.Count; ++i)
//                    {
//                        var selID = selected[i];
//                        var monster = Monsters.MonsterInfos[selID];
//                        var tempLayer = TemporalLayers[CurrentLayer - 1];
//                        if (!tempLayer.Monsters.Contains(selID))
//                        {
//                            LayerMonsters[CurrentLayer - 1].Add(monster.Sprites[(int)Def.MonsterFrame.FACE_1].texture);
//                            tempLayer.Monsters.Add(selID);
//                            if (tempLayer.MonsterChances.Count == 0)
//                            {
//                                tempLayer.MonsterChances.Add(1.0f);
//                            }
//                            else
//                            {
//                                tempLayer.MonsterChances.Add(tempLayer.MonsterChances.Average());
//                                GameUtils.UpdateChances(ref tempLayer.MonsterChances);
//                            }
//                            TemporalLayers[CurrentLayer - 1] = tempLayer;
//                        }
//                    }
//                    Selector.SelectorType = ISType.NONE;
//                }
//                return false;
//            }
//            if(LayerSelector.SelectingLayer)
//            {
//                bool sel = LayerSelector.OnGUI(TemporalLayers, CurrentLayer);
//                if(sel)
//                {
//                    var selected = LayerSelector.GetSelectedLayer();
//                    if(selected.IsValid())
//                    {
//                        var sLayer = selected;
//                        sLayer.Layer = CurrentLayer;
//                        LayerMaterials[CurrentLayer - 1].Clear();
//                        LayerProps[CurrentLayer - 1].Clear();
//                        LayerMonsters[CurrentLayer - 1].Clear();
//                        for(int i = 0; i < sLayer.MaterialTypes.Count; ++i)
//                        {
//                            var topMat = BlockMaterial.MaterialFamilies[sLayer.MaterialTypes[i]].NormalMaterials[0].TopPart;
//                            var texture = topMat.Mat.GetTexture(Def.MaterialTextureID);
//                            if (texture == null)
//                                texture = topMat.Mat.GetTexture(AssetContainer.ColoredMaterialTextureID);

//                            //var mat = BlockMaterial.MaterialTypes[sLayer.MaterialTypes[i]].Def.Materials[(int)BlockType.NORMAL][0];
//                            //var texture = BlockMaterial.BlockMaterials[mat.TopMat].BlockMaterial.GetTexture(Def.MaterialTextureID);
//                            //if (texture == null)
//                            //    texture = BlockMaterial.BlockMaterials[mat.TopMat].BlockMaterial.GetTexture(AssetContainer.ColoredMaterialTextureID);
//                            LayerMaterials[CurrentLayer - 1].Add((Texture2D)texture);
//                        }
//                        for (int i = 0; i < sLayer.Props.Count; ++i)
//                        {
//                            var prop = Props.PropFamilies[sLayer.Props[i]].Props[0];
//                            LayerProps[CurrentLayer - 1].Add(prop.sprite.texture);
//                        }
//                        for (int i = 0; i < sLayer.Monsters.Count; ++i)
//                        {
//                            var monster = Monsters.MonsterInfos[sLayer.Monsters[i]];
//                            LayerMonsters[CurrentLayer - 1].Add(monster.Sprites[(int)Def.MonsterFrame.FACE_1].texture);
//                        }
//                        TemporalLayers[CurrentLayer - 1] = sLayer;
//                    }
//                    LayerSelector.SelectingLayer = false;
//                }
//                return false;
//            }
//            var struc = Manager.Mgr.Structure;
//            var rect = Manager.Mgr.m_Canvas.pixelRect;
//            var width = rect.width * 0.5f;
//            var height = rect.height;
//            float lastLayerLeftWidth = 0.0f;
//            float lastLayerRightWidth = 0.0f;
//            //LayerWidth = Mathf.Min(width, LayerWidth);
//            //LayerHeight = Mathf.Min(height, LayerHeight);
//            LayerScroll = GUI.BeginScrollView(new Rect(0.0f, 0.0f, LayerWidth+17.0f, height), LayerScroll, new Rect(0.0f, 0.0f, LayerWidth, LayerHeight));
//            GUI.Box(new Rect(0.0f, 0.0f, LayerWidth, LayerHeight), "Layer editor");

//            CurrentLayer = 1 + GUI.SelectionGrid(new Rect(0.0f, 0.0f, 25.0f * StructureComponent.LayerAmount, 25.0f), CurrentLayer - 1, LayerNames, LayerNames.Length);
//            lastLayerLeftWidth = Mathf.Max(25.0f * StructureComponent.LayerAmount, lastLayerLeftWidth);
//            var layer = TemporalLayers[CurrentLayer - 1];
//            // Reset
//            {
//                var reset = GUI.Button(new Rect(LayerWidth - 100.0f, 0.0f, 100.0f, 25.0f), "Reset Layer");
//                if(reset)
//                {
//                    layer = LayerInfo.GetDefaultLayer();
//                    layer.Layer = CurrentLayer;
//                    LayerMaterials[CurrentLayer - 1].Clear();
//                    LayerMaterialSelected = 0;
//                    LayerProps[CurrentLayer - 1].Clear();
//                    LayerPropSelected = 0;
//                    while(struc.Layers[CurrentLayer - 1].Count != 0)
//                        struc.Layers[CurrentLayer - 1][0].Layer = 0;
//                    struc.InfoLayers[CurrentLayer - 1] = layer;
//                }
//            }
//            // Copy layer from
//            {
//                var copy = GUI.Button(new Rect(LayerWidth - 100.0f, 25.0f, 100.0f, 25.0f), "Copy from");
//                if(copy)
//                {
//                    LayerSelector.Reset();
//                    LayerSelector.SelectingLayer = true;
//                }
//            }
//            lastLayerRightWidth = Mathf.Max(lastLayerRightWidth, 100.0f);
//            layer.IsLinkedLayer = GUI.Toggle(new Rect(0.0f, 27.0f, 110.0f, 25.0f), layer.IsLinkedLayer, "Is Linked Layer");
            
//            float lastHeight = 52.0f;

//            if (layer.IsLinkedLayer)
//                LinkedLayerGUI(ref lastHeight, ref lastLayerLeftWidth, ref lastLayerRightWidth, ref layer);
//            else
//                NormalLayerGUI(ref lastHeight, ref lastLayerLeftWidth, ref lastLayerRightWidth, ref layer);

//            bool apply = false;
//            if (layer.IsValid())
//            {
//                apply = GUI.Button(new Rect(LayerWidth * 0.5f, lastHeight + 2.0f, 50.0f, 25.0f), "Apply");
//            }
//            var cancel = GUI.Button(new Rect(LayerWidth * 0.5f - 50.0f, lastHeight + 2.0f, 50.0f, 25.0f), "Cancel");
//            if (apply)
//            {
//                var strucIE = Structures.Strucs[struc.IDXIE];
//                var IELayers = strucIE.Layers;
                
//                for(int i = 1; i < StructureComponent.LayerAmount + 1; ++i)
//                {
//                    var temporalLayer = TemporalLayers[i - 1];
//                    if (!temporalLayer.IsValid())
//                        continue;

//                    struc.SetLayer(i, temporalLayer);
//                    int idx = -1;

//                    for(int j = 0; j < IELayers.Count; ++j)
//                    {
//                        var ieLayer = IELayers[j];
//                        if (ieLayer == null)
//                            continue;
//                        if (ieLayer.Layer == temporalLayer.Layer)
//                        {
//                            idx = j;
//                            break;
//                        }
//                    }
//                    if(idx < 0)
//                    {
//                        for (int j = 0; j < IELayers.Count; ++j)
//                        {
//                            var ieLayer = IELayers[j];
//                            if (ieLayer != null)
//                                continue;

//                            idx = j;
//                            IELayers[j] = new IE.V2.LayerIE();
//                            break;
//                        }
//                    }
//                    if(idx < 0)
//                    {
//                        var ieLayer = new IE.V2.LayerIE();
//                        idx = IELayers.Count;
//                        IELayers.Add(ieLayer);
//                    }
//                    var iel = IELayers[idx];
//                    temporalLayer.ToLayerIE(ref iel);
//                }
//                strucIE.Layers = IELayers;
//            }
//            lastHeight += 25.0f;
//            GUI.EndScrollView();
//            LayerHeight = lastHeight + 5.0f;
//            LayerWidth = lastLayerLeftWidth + lastLayerRightWidth + 50.0f;
//            if (LayerWidth < 275.0f)
//                LayerWidth = 275.0f;
//            TemporalLayers[CurrentLayer - 1] = layer;
//            if (apply || cancel)
//                return true;
//            return false;
//        }
//    }
//}