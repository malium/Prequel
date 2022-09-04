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
//    enum SM_GUIState
//    {
//        Default,
//        LoadStrucMenu,
//        SaveDialog,
//        ScreenCapDone,
//        CreatingNew,

//        COUNT,
//    }
//    enum SM_State
//    {
//        Default,
//        LoadingStruc,
//        ScreenShooting,
//        SavingStruc,
//        CreatingStruc,
//        CopyingStruc,

//        COUNT,
//    }

//    public class StructureMenu
//    {
//        float Width;
//        float Height;
//        int loadSelected;

//        SM_GUIState GUIState;

//        SM_State State;

//        SM_State NextState;
//        ImageSelector Selector;
//        //StructureSelector Selector;
        
//        EditingStructureController Controller;

//        Vector2 StrucScroll;
//        int m_ScreenshotFrame;

//        public StructureMenu(EditingStructureController controller)
//        {
//            Controller = controller;
//        }

//        public void Start()
//        {
//            Selector = new ImageSelector();
//            Selector.Start();
//            Selector.MultiSelection = false;
//        }

//        public void Stop()
//        {
//            Selector.Stop();
//            Selector = null;
//        }
        
//        public void Reset()
//        {
//            Selector.Stop();
//            Selector.Start();
//            Selector.Reset(new List<int>(Enumerable.Repeat(Manager.Mgr.Structure.IDXIE, 1)));
//            Selector.SelectorType = ISType.Struc;
//            Selector.MultiSelection = false;
//            GUIState = SM_GUIState.Default;
//            State = SM_State.Default;
//            NextState = SM_State.COUNT;
//            //StrucScroll = Vector2.zero;
//            Width = 2000.0f;
//            Height = 2000.0f;
//            loadSelected = -1;
//        }

//        void CreateNew()
//        {
//            var struc = Manager.Mgr.Structure;
//            if(struc == null)
//            {
//                var strucGO = new GameObject("InvalidStruc");
//                struc = strucGO.AddComponent<StructureComponent>();
//                struc.StructRect = new Rect(0.0f, 0.0f, StructureComponent.Width * (1.0f + StructureComponent.Separation), StructureComponent.Height * (1.0f + StructureComponent.Separation));
//                var id = Structures.AddStructure();
//                struc.IDXIE = id;
//            }
//            else
//            {
//                Structures.RemoveStructure(struc.IDXIE);

//                struc.IDXIE = Structures.AddStructure();

//                for(int i = 0; i < struc.Pilars.Length; ++i)
//                {
//                    struc.Pilars[i].DestroyPilar();
//                }
//                for(int i = 0; i < struc.LivingEntities.Count; ++i)
//                {
//                    var le = struc.LivingEntities[i];
//                    le.ReceiveDamage(Def.DamageType.UNAVOIDABLE, le.GetTotalHealth());
//                }
//                for(int i = 0; i < StructureComponent.LayerAmount; ++i)
//                {
//                    var layer = LayerInfo.GetDefaultLayer();
//                    layer.Layer = i + 1;
//                    struc.SetLayer(i + 1, layer);
//                }
//            }
//            for (int y = 0; y < StructureComponent.Height; ++y)
//            {
//                var yOffset = y * StructureComponent.Width;
//                for (int x = 0; x < StructureComponent.Width; ++x)
//                {
//                    var idx = yOffset + x;

//                    var strucID = GameUtils.IDFromPos(new Vector2Int(x, y));
//                    var pilarGO = new GameObject("InvalidPilar");
//                    var pilar = pilarGO.AddComponent<PilarComponent>();
//                    struc.Pilars[idx] = pilar;

//                    pilar.Init(struc, strucID);
//                    pilar.AddBlock();
//                }
//            }
//            Manager.Mgr.Structure = struc;
//        }

//        void SaveStructure()
//        {
//            var struc = Manager.Mgr.Structure;
//            Structures.SaveStruc(struc.IDXIE);
//        }

//        void CaptueScreenshot()
//        {
//            var tex = ScreenCapture.CaptureScreenshotAsTexture();
//            int initialX = tex.width / 2 - tex.height / 2;
//            var nTex = new Texture2D(tex.height, tex.height);
            
//            var origPixels = tex.GetPixels();
//            var newPixels = new Color[tex.height * tex.height];
//            int ynOffset = 0;
//            int yoOffset = 0;
//            for(int y = 0; y < tex.height; ++y)
//            {
//                for(int x = 0; x < tex.height; ++x)
//                {
//                    newPixels[x + ynOffset] = origPixels[initialX + x + yoOffset];
//                }
//                ynOffset += tex.height;
//                yoOffset += tex.width;
//            }

//            nTex.SetPixels(newPixels);
//            nTex.Apply();

//            var scaledTex = GameUtils.ResizeTexture(nTex, GameUtils.ImageFilterMode.Average, 128.0f / nTex.height);

//            var struc = Manager.Mgr.Structure;
//            var strucIE = Structures.Strucs[struc.IDXIE];
//            strucIE.ScreenShot = scaledTex;
//        }

//        void LoadStructure()
//        {
//            if(loadSelected < 0)
//            {
//                CreateNew();
//                return;
//            }
//            var struc = Manager.Mgr.Structure;
//            if(!Structures.Strucs[struc.IDXIE].IsFromFile && State != SM_State.CopyingStruc)
//                Structures.RemoveStructure(struc.IDXIE);
            
//            var selectedStruc = Structures.Strucs[loadSelected];

//            for (int i = 0; i < struc.Pilars.Length; ++i)
//            {
//                struc.Pilars[i].DestroyPilar();
//            }
//            for (int i = 0; i < StructureComponent.LayerAmount; ++i)
//            {
//                var layer = LayerInfo.GetDefaultLayer();
//                layer.Layer = i + 1;
//                struc.SetLayer(i + 1, layer);
//            }
//            selectedStruc.ToStructure(ref struc, State != SM_State.CopyingStruc);
//            struc.StructRect = new Rect(0.0f, 0.0f, StructureComponent.Width * (1.0f + StructureComponent.Separation), StructureComponent.Height * (1.0f + StructureComponent.Separation));
//            if (State == SM_State.CopyingStruc)
//            {
//                var strucIE = Structures.Strucs[struc.IDXIE];
//                strucIE.GenerateBridges = selectedStruc.GenerateBridges;
//                strucIE.Layers = selectedStruc.Layers;
//                strucIE.Blocks = selectedStruc.Blocks;
//                strucIE.Zones = selectedStruc.Zones;
//            }
//        }

//        void LoadStrucMenu()
//        {
//            loadSelected = -1;
//            var sel = Selector.OnGUI();
//            if (sel)
//            {
//                var selList = Selector.GetSelected();
//                if (selList.Count < 1)
//                    loadSelected = int.MinValue;
//                else
//                    loadSelected = selList[0];
//            }
//        }

//        bool StrucMenu()
//        {
//            var struc = Manager.Mgr.Structure;
//            var rect = Manager.Mgr.m_Canvas.pixelRect;
//            var pos = new Vector2(rect.width * 0.5f - Width * 0.5f, rect.height * 0.5f - Height * 0.5f);
//            float lastHeight = 0.0f;
//            float lastWidth = 0.0f;
//            const float Separation = 25.0f;

//            GUI.Box(new Rect(pos.x, pos.y, Width, Height), "Structure Menu");

//            lastHeight += Separation;
//            bool createStruc = GUI.Button(new Rect(pos.x + Separation, pos.y + lastHeight, 100.0f, 25.0f), "Create new");
//            if (createStruc)
//            {
//                State = SM_State.CreatingStruc;
//                GUIState = SM_GUIState.Default;
//                NextState = SM_State.COUNT;
//            }
//            lastHeight += Separation;
//            lastWidth = Mathf.Max(lastWidth, 100.0f);

//            bool copyStruc = GUI.Button(new Rect(pos.x + Separation, pos.y + lastHeight, 100.0f, 25.0f), "Copy Struc");
//            if(copyStruc)
//            {
//                State = SM_State.CopyingStruc;
//                GUIState = SM_GUIState.LoadStrucMenu;
//                NextState = SM_State.COUNT;
//            }
//            lastHeight += Separation;
//            lastWidth = Mathf.Max(lastWidth, 100.0f);

//            bool loadStruc = false;
//            if (Structures.Strucs.Count > 1)
//            {
//                loadStruc = GUI.Button(new Rect(pos.x + Separation, pos.y + lastHeight, 100.0f, 25.0f), "Load");
//                if (loadStruc)
//                {
//                    State = SM_State.LoadingStruc;
//                    GUIState = SM_GUIState.LoadStrucMenu;
//                    NextState = SM_State.COUNT;
//                }
//                lastHeight += Separation;
//                lastWidth = Mathf.Max(lastWidth, 100.0f);
//            }

//            bool saveStruc = false;
//            var ie = Structures.Strucs[struc.IDXIE];
//            if (ie.m_BlockNum > 0 && Structures.IsStrucModified(struc.IDXIE) && ie.m_NameLength > 0)
//            {
//                saveStruc = GUI.Button(new Rect(pos.x + Separation, pos.y + lastHeight, 100.0f, 25.0f), "Save");
//                if (saveStruc)
//                {
//                    State = SM_State.SavingStruc;
//                    GUIState = SM_GUIState.Default;
//                    NextState = SM_State.COUNT;
//                }
//                lastHeight += Separation;
//                lastWidth = Mathf.Max(lastWidth, 100.0f);
//            }

//            lastHeight += 10.0f;
//            bool close = GUI.Button(new Rect(pos.x + Separation + 25.0f, pos.y + lastHeight, 50.0f, 25.0f), "Close");
//            lastHeight += Separation;
//            lastWidth = Mathf.Max(lastWidth, 50.0f);

//            Width = lastWidth + Separation * 2.0f;
//            Height = lastHeight;

//            if (close || (createStruc || saveStruc) && State == SM_State.Default)
//                return true;

//            return false;
//        }
        
//        bool OnLoadingStruc()
//        {
//            switch (GUIState)
//            {
//                case SM_GUIState.LoadStrucMenu:
//                    LoadStrucMenu();
//                    if (loadSelected >= 0)
//                    {
//                        var strucIE = Structures.Strucs[Manager.Mgr.Structure.IDXIE];
//                        if (strucIE.m_BlockNum > 0 && strucIE.m_LayerNum > 0 || strucIE.m_NameLength > 0)
//                        {
//                            GUIState = SM_GUIState.SaveDialog;
//                        }
//                        else
//                        {
//                            GUIState = SM_GUIState.Default;
//                        }
//                    }
//                    else if (loadSelected == int.MinValue)
//                    {
//                        State = SM_State.Default;
//                        GUIState = SM_GUIState.Default;
//                    }
//                    break;
//                case SM_GUIState.SaveDialog:
//                    var response = GUIUtils.CenteredDialog("Do you want to save the current Structure?");
//                    if (response == DialogResponse.Yes)
//                    {
//                        Controller.ToggleEditingVisibility(true, true);
//                        Manager.Mgr.m_Camera.GetComponent<CameraManager>().CameraType = ECameraType.EDITOR;
//                        State = SM_State.ScreenShooting;
//                        m_ScreenshotFrame = Time.frameCount + 2;
//                        NextState = SM_State.LoadingStruc;
//                        GUIState = SM_GUIState.Default;
//                    }
//                    else if (response == DialogResponse.No)
//                    {
//                        GUIState = SM_GUIState.Default;
//                    }
//                    break;
//                case SM_GUIState.Default:
//                    if (m_ScreenshotFrame == Time.frameCount)
//                    {
//                        Controller.ToggleEditingVisibility(true, false);
//                        SaveStructure();
//                    }
//                    LoadStructure();
//                    loadSelected = -1;
//                    State = SM_State.Default;
//                    NextState = SM_State.COUNT;
//                    GUIState = SM_GUIState.Default;

//                    return true;
//            }
//            return false;
//        }

//        bool OnScreenshotting()
//        {
//            if (m_ScreenshotFrame != Time.frameCount)
//                return false;
//            CaptueScreenshot();
//            if (NextState != SM_State.COUNT)
//            {
//                State = NextState;
//                NextState = SM_State.COUNT;
//            }
//            else
//            {
//                State = SM_State.Default;
//            }
//            return false;
//        }

//        bool OnSavingStruc()
//        {
//            switch (GUIState)
//            {
//                case SM_GUIState.ScreenCapDone:
//                    //Manager.Mgr.m_Camera.GetComponent<CameraManager>().CameraType = ECameraType.EDITOR;
//                    Controller.ToggleEditingVisibility(true, false);
//                    SaveStructure();
//                    State = SM_State.Default;
//                    NextState = SM_State.COUNT;
//                    GUIState = SM_GUIState.Default;
//                    return true;
//                case SM_GUIState.Default:
//                    Manager.Mgr.m_Camera.GetComponent<CameraManager>().CameraType = ECameraType.EDITOR;
//                    Controller.ToggleEditingVisibility(true, true);
//                    m_ScreenshotFrame = Time.frameCount + 2;
//                    State = SM_State.ScreenShooting;
//                    NextState = SM_State.SavingStruc;
//                    GUIState = SM_GUIState.ScreenCapDone;
//                    break;
//            }
//            return false;
//        }

//        bool OnCreatingStruc()
//        {
//            switch (GUIState)
//            {
//                case SM_GUIState.SaveDialog:
//                    var response = GUIUtils.CenteredDialog("Do you want to save the current Structure?");
//                    if (response == DialogResponse.Yes)
//                    {
//                        Manager.Mgr.m_Camera.GetComponent<CameraManager>().CameraType = ECameraType.EDITOR;
//                        Controller.ToggleEditingVisibility(true, true);
//                        m_ScreenshotFrame = Time.frameCount + 2;
//                        State = SM_State.ScreenShooting;
//                        NextState = SM_State.CreatingStruc;
//                        GUIState = SM_GUIState.ScreenCapDone;
//                    }
//                    else if (response == DialogResponse.No)
//                    {
//                        GUIState = SM_GUIState.CreatingNew;
//                    }
//                    break;
//                case SM_GUIState.ScreenCapDone:
//                    Controller.ToggleEditingVisibility(true, false);
//                    SaveStructure();
//                    GUIState = SM_GUIState.CreatingNew;
//                    break;
//                case SM_GUIState.CreatingNew:
//                    CreateNew();
//                    State = SM_State.Default;
//                    NextState = SM_State.COUNT;
//                    GUIState = SM_GUIState.Default;
//                    return true;
//                case SM_GUIState.Default:
//                    var strucIE = Structures.Strucs[Manager.Mgr.Structure.IDXIE];
//                    if (strucIE.m_BlockNum > 0 && strucIE.m_LayerNum > 0 || strucIE.m_NameLength > 0)
//                    {
//                        GUIState = SM_GUIState.SaveDialog;
//                    }
//                    else
//                    {
//                        GUIState = SM_GUIState.CreatingNew;
//                    }
//                    break;
//            }
//            return false;
//        }

//        bool OnCopyingStruc()
//        {
//            switch (GUIState)
//            {
//                case SM_GUIState.LoadStrucMenu:
//                    LoadStrucMenu();
//                    if (loadSelected >= 0)
//                    {
//                        GUIState = SM_GUIState.Default;
//                    }
//                    else if (loadSelected == int.MinValue)
//                    {
//                        State = SM_State.Default;
//                        GUIState = SM_GUIState.Default;
//                    }
//                    break;
//                case SM_GUIState.Default:
//                    LoadStructure();
//                    loadSelected = -1;
//                    State = SM_State.Default;
//                    NextState = SM_State.COUNT;
//                    GUIState = SM_GUIState.Default;
//                    return true;
//            }
//            return false;
//        }

//        public bool OnGUI()
//        {
//            switch(State)
//            {
//                case SM_State.LoadingStruc:
//                    return OnLoadingStruc();
//                case SM_State.ScreenShooting:
//                    return OnScreenshotting();
//                case SM_State.SavingStruc:
//                    return OnSavingStruc();
//                case SM_State.CreatingStruc:
//                    return OnCreatingStruc();
//                case SM_State.CopyingStruc:
//                    return OnCopyingStruc();
//                case SM_State.Default:
//                    return StrucMenu();
//                default:
//                    return true;
//            }
//        }
//    }
//}
