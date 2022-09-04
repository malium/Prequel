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
//    public class Editor : IGameController
//    {
//        static readonly string[] SelectionString = new string[3]
//        {
//            "Biome",
//            "Structure",
//            "Building"
//        };
//        IGameController[] m_Controllers;
//        int m_CurrentController;
//        Action<Editor> m_OnGUIFn = OnGUISelectionFN;

//        static Action<Editor> OnGUISelectedFN = (Editor editor) => { };
//        static Action<Editor> OnGUISelectionFN = (Editor editor) => { editor.OnGUISelection(); };

//        void OnGUISelection()
//        {
//            var canvas = Manager.Mgr.m_Canvas;
//            var rect = canvas.pixelRect;

//            const float BoxWidth = 100f;
//            const float BoxHeight = 150f;
//            var BoxRect = new Rect(rect.width * 0.5f - BoxWidth * 0.5f, rect.height * 0.5f - BoxHeight * 0.5f, BoxWidth, BoxHeight);
//            GUI.Box(BoxRect, "");

//            var SelectionRect = new Rect(BoxRect.x + 5f, BoxRect.y + 5, BoxWidth - 10f, BoxHeight - 10f);
//            int selected = GUI.SelectionGrid(SelectionRect, -1, SelectionString, 1);
//            if(selected >= 0 || selected < m_Controllers.Length)
//            {
//                m_CurrentController = selected;
//                m_Controllers[m_CurrentController].Start();
//                m_OnGUIFn = OnGUISelectedFN;
//            }
//        }

//        public void FixedUpdate()
//        {
//            m_Controllers[m_CurrentController].FixedUpdate();
//        }

//        public GameState GetCurrentState() => m_Controllers[m_CurrentController].GetGameState();

//        public IGameController GetCurrent() => m_Controllers[m_CurrentController];

//        public GameState GetGameState()
//        {
//            return GameState.EDITOR;
//        }

//        public void OnGUI()
//        {
//            m_OnGUIFn(this);
//            m_Controllers[m_CurrentController].OnGUI();
//        }

//        public void Start()
//        {
//            m_Controllers = new IGameController[3]
//            {
//                new NullGameController(),
//                new StrucEditor(),
//                new BuildEditor()
//            };
//            m_CurrentController = 0;
//        }

//        public void Stop()
//        {
//            m_Controllers[m_CurrentController].Stop();
//        }

//        public void Update()
//        {
//            m_Controllers[m_CurrentController].Update();
//        }
//    }
//}
