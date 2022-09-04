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
//    public class AntSelector
//    {
//        List<Texture2D> m_Images;
//        List<Rect> m_ImageRects;
//        //int m_Selected;
//        static readonly string[] m_AntSideDirectionStr =
//            new string[3]
//            {
//                "NONE",
//                "UP",
//                "DOWN"
//            };
//        static readonly string[] m_AntTopDirectionStr =
//            new string[(int)AntTopDirection.COUNT + 1]
//            {
//                ((AntTopDirection)0).ToString(),
//                ((AntTopDirection)1).ToString(),
//                ((AntTopDirection)2).ToString(),
//                ((AntTopDirection)3).ToString(),
//                ((AntTopDirection)4).ToString(),
//                ((AntTopDirection)5).ToString(),
//                ((AntTopDirection)6).ToString(),
//                ((AntTopDirection)7).ToString(),
//                ((AntTopDirection)8).ToString(),
//                ((AntTopDirection)9).ToString(),
//                ((AntTopDirection)10).ToString(),
//                ((AntTopDirection)11).ToString(),
//                "NONE"
//            };
//        int[] m_AntDirection;
//        //Vector2 m_ImageScroll;
//        //float m_GridHeight;
//        //float m_Height;
//        //float m_Width;

//        public void SetSelectedOptions(int selectedMapID, int selectedBlockIdx)
//        {
//            var block = Manager.Mgr.Pilars[selectedMapID].Blocks[selectedBlockIdx];

//            int version = -1;
//            switch ((AntTopDirection)m_AntDirection[0])
//            {
//                case AntTopDirection.SOUTH_NORTH:
//                case AntTopDirection.NORTH_SOUTH:
//                case AntTopDirection.EAST_WEST:
//                case AntTopDirection.WEST_EAST:
//                    version = UnityEngine.Random.Range(0, AntManager.StraightAnts.Count);
//                    //version = Manager.Mgr.SpawnRNG.Next(0, AntManager.StraightAnts.Count);
//                    break;
//                case AntTopDirection.SOUTH_EAST:
//                case AntTopDirection.WEST_SOUTH:
//                case AntTopDirection.NORTH_WEST:
//                case AntTopDirection.EAST_NORTH:
//                    version = UnityEngine.Random.Range(0, AntManager.TurnRightAnts.Count);
//                    //version = Manager.Mgr.SpawnRNG.Next(0, AntManager.TurnRightAnts.Count);
//                    break;
//                case AntTopDirection.SOUTH_WEST:
//                case AntTopDirection.NORTH_EAST:
//                case AntTopDirection.WEST_NORTH:
//                case AntTopDirection.EAST_SOUTH:
//                    version = UnityEngine.Random.Range(0, AntManager.TurnLeftAnts.Count);
//                    //version = Manager.Mgr.SpawnRNG.Next(0, AntManager.TurnLeftAnts.Count);
//                    break;
//            }
//            block.SetTopAnt(version, (AntTopDirection)m_AntDirection[0]);

//            // Sides
//            for (int i = 1; i < m_AntDirection.Length; ++i)
//            {
//                version = m_AntDirection[i] == 0 ? -1 : UnityEngine.Random.Range(0, AntManager.StraightAnts.Count); // Manager.Mgr.SpawnRNG.Next(0, AntManager.StraightAnts.Count);
//                block.SetSideAnt(version, (SpaceDirection)(i - 1), m_AntDirection[i] == 1);
//            }

//            GameUtils.FixSideAnts(block, (AntTopDirection)m_AntDirection[0]);
//        }

//        public AntSelector()
//        {
//            m_Images = new List<Texture2D>();
//            m_ImageRects = new List<Rect>();
//            m_Images.Capacity = AntManager.StraightAnts.Count + AntManager.TurnLeftAnts.Count + AntManager.TurnRightAnts.Count;
//            m_ImageRects.Capacity = m_Images.Capacity;
//            m_AntDirection = new int[Def.DecoPositionCount];
//        }

//        public void Reset()
//        {
//            //m_ImageScroll = Vector2.zero;
//            m_AntDirection[0] = m_AntTopDirectionStr.Length - 1;
//            for (int i = 1; i < m_AntDirection.Length; ++i) m_AntDirection[i] = 0;
//            //m_Selected = 0;
//            //m_Height = 2000f;
//            //m_Width = 2000f;
//        }

//        public void Start()
//        {
//            void addImages(List<AntDef> list)
//            {
//                for(int i = 0; i < list.Count; ++i)
//                {
//                    var ant = list[i];
//                    var frame = ant.Frames[0];
//                    m_Images.Add(frame.texture);
//                    var rect = frame.textureRect;
//                    float width = 1f / frame.texture.width;
//                    float height = 1f / frame.texture.height;
//                    m_ImageRects.Add(new Rect(rect.x * width, rect.y * height, rect.width * width, rect.height * height));
//                }
//            }
//            addImages(AntManager.StraightAnts);
//            addImages(AntManager.TurnLeftAnts);
//            addImages(AntManager.TurnRightAnts);
//            //m_GridHeight = Mathf.CeilToInt(m_Images.Count * 0.2f) * 100f;
//        }

//        public void Stop()
//        {
//            m_Images.Clear();
//            m_Images.Clear();
//        }

//        public bool OnGUI(int selectedMapID, int selectedBlockIdx)
//        {
//            //float lastHeight = 5f;
//            //float maxWidth = 0f;

//            GUI.Box(new Rect(0f, 0f, 575f, 400f), "");

//            // NORTH
//            {
//                m_AntDirection[(int)Def.DecoPosition.NORTH] =
//                    GUI.SelectionGrid(new Rect(180f, 10f, 200f, 30f), m_AntDirection[(int)Def.DecoPosition.NORTH], m_AntSideDirectionStr, 3);
//            }

//            // WEST
//            {
//                m_AntDirection[(int)Def.DecoPosition.WEST] =
//                    GUI.SelectionGrid(new Rect(10f, 65f, 50f, 150f), m_AntDirection[(int)Def.DecoPosition.WEST], m_AntSideDirectionStr, 1);
//            }

//            // EAST
//            {
//                m_AntDirection[(int)Def.DecoPosition.EAST] =
//                    GUI.SelectionGrid(new Rect(500f, 65f, 50f, 150f), m_AntDirection[(int)Def.DecoPosition.EAST], m_AntSideDirectionStr, 1);
//            }

//            // SOUTH
//            {
//                m_AntDirection[(int)Def.DecoPosition.SOUTH] =
//                    GUI.SelectionGrid(new Rect(180f, 300f, 200f, 30f), m_AntDirection[(int)Def.DecoPosition.SOUTH], m_AntSideDirectionStr, 3);
//            }

//            // TOP
//            {
//                m_AntDirection[(int)Def.DecoPosition.TOP] =
//                    GUI.SelectionGrid(new Rect(80f, 65f, 400f, 200f), m_AntDirection[(int)Def.DecoPosition.TOP], m_AntTopDirectionStr, 3);
//            }

//            //// Direction
//            //{
//            //    m_AntDirection = (SpaceDirection)
//            //        GUI.SelectionGrid(new Rect(5f, lastHeight, AntDirectionWidth, 25f), (int)m_AntDirection, m_AntDirectionStr,
//            //        (int)SpaceDirection.COUNT);
//            //    lastHeight += 25f;
//            //    maxWidth = Mathf.Max(maxWidth, 200f);
//            //}

//            //// Ant selector
//            //{
//            //    m_ImageScroll = GUI.BeginScrollView(new Rect(5f, lastHeight, 5f * 100f + 20f, 100f), m_ImageScroll, new Rect(0f, 0f, 5f * 100f, m_GridHeight));
//            //    for(int i = 0; i < m_Images.Count;)
//            //    {
//            //        float h = Mathf.Floor(i * 0.2f);
//            //        for(int j = 0; j < 5 && i < m_Images.Count; ++j, ++i)
//            //        {
//            //            var sel = GUI.Toggle(new Rect(100f * j, 100f * h, 25f, 25f), i == m_Selected, "");
//            //            GUI.DrawTextureWithTexCoords(new Rect(100f * j + 25f, 100f * h, 75f, 75f), m_Images[i], m_ImageRects[i]);
//            //            if (sel)
//            //                m_Selected = i;
//            //        }
//            //    }
//            //    GUI.EndScrollView();
//            //}
//            //lastHeight += 100f;
//            //maxWidth = Mathf.Max(maxWidth, 5f * 100f + 20f);

//            //// Positions
//            //switch (m_AntDirection)
//            //{
//            //    case SpaceDirection.NORTH:
//            //    case SpaceDirection.SOUTH:
//            //        m_AntPositions[(int)BlockDecoPosition.NORTH] =
//            //            GUI.Toggle(new Rect(20f, lastHeight, 20f, 20f), m_AntPositions[(int)BlockDecoPosition.NORTH], "");
//            //        lastHeight += 20f;
//            //        m_AntPositions[(int)BlockDecoPosition.TOP]
//            //            = GUI.Toggle(new Rect(20f, lastHeight, 20f, 20f), m_AntPositions[(int)BlockDecoPosition.TOP], "");
//            //        lastHeight += 20f;
//            //        m_AntPositions[(int)BlockDecoPosition.SOUTH]
//            //            = GUI.Toggle(new Rect(20f, lastHeight, 20f, 20f), m_AntPositions[(int)BlockDecoPosition.SOUTH], "");
//            //        lastHeight += 20f;
//            //        maxWidth = Mathf.Max(maxWidth, 15f + 20f);
//            //        break;
//            //    case SpaceDirection.EAST:
//            //    case SpaceDirection.WEST:
//            //        lastHeight += 20f;
//            //        m_AntPositions[(int)BlockDecoPosition.WEST]
//            //            = GUI.Toggle(new Rect(10f, lastHeight, 20f, 20f), m_AntPositions[(int)BlockDecoPosition.WEST], "");
//            //        m_AntPositions[(int)BlockDecoPosition.TOP]
//            //            = GUI.Toggle(new Rect(30f, lastHeight, 20f, 20f), m_AntPositions[(int)BlockDecoPosition.TOP], "");
//            //        m_AntPositions[(int)BlockDecoPosition.EAST]
//            //            = GUI.Toggle(new Rect(50f, lastHeight, 20f, 20f), m_AntPositions[(int)BlockDecoPosition.EAST], "");
//            //        lastHeight += 20f;
//            //        lastHeight += 20f;
//            //        maxWidth = Mathf.Max(maxWidth, 45f + 20f);
//            //        break;
//            //}

//            bool cancel = GUI.Button(new Rect(230f, 350f, 50f, 25f), "Cancel");
//            bool apply = GUI.Button(new Rect(280f, 350f, 50f, 25f), "Apply");
//            //lastHeight += 25f;

//            //m_Width = maxWidth * 1.05f;
//            //m_Height = lastHeight * 1.05f;

//            if (apply)
//                SetSelectedOptions(selectedMapID, selectedBlockIdx);

//            if (cancel || apply)
//                return true;
//            return false;
//        }
//    }
//}
