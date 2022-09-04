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
//    public class StructureEditor
//    {
//        Vector2 m_UIScroll;
//        float m_UIHeight;
//        float m_UIWidth;
//        string m_NameInput;
//        CStrucEdit m_Structure;
//        int m_IDXIE;
//        IE.V3.StructureIE m_IE;

//        public StructureEditor()
//        {

//        }

//        public void Reset()
//        {
//            m_UIScroll = Vector2.zero;
//            m_UIHeight = 2000.0f;
//            m_UIWidth = 2000.0f;
//            var strucIE = Structures.Strucs[Manager.Mgr.Structure.IDXIE];
//            var name = strucIE.GetName();
//            m_NameInput = (name.Length == 0 ? "Unnamed" : name);
//            m_Structure = Manager.Mgr.Structure;
//            m_IDXIE = Manager.Mgr.Structure.IDXIE;
//            m_IE = Structures.Strucs[m_IDXIE];
//        }

//        public void Start()
//        {

//        }

//        public void Stop()
//        {

//        }

//        void SetStrucName()
//        {
//            bool isNameAvailable(string nameToTest)
//            {
//                if(Structures.StrucDict.ContainsKey(nameToTest))
//                {
//                    if (Structures.StrucDict[nameToTest] == m_IDXIE)
//                        return true;
//                    return false;
//                }
//                return true;
//            }

//            // Name cleaning
//            var name = GameUtils.RemoveStructureID(m_NameInput);

//            int structureIdx = 0;
//            string testName = name + "_" + structureIdx.ToString().PadLeft(3, '0');
//            while (!isNameAvailable(testName))
//            {
//                ++structureIdx;
//                testName = name + "_" + structureIdx.ToString().PadLeft(3, '0');
//            }
//            var originalName = m_IE.GetName();
//            if (originalName.Length > 0 && Structures.StrucDict.ContainsKey(originalName))
//                Structures.StrucDict.Remove(originalName);
//            m_IE.SetName(testName);
//            if(!Structures.StrucDict.ContainsKey(testName))
//                Structures.StrucDict.Add(testName, m_IDXIE);
                
//        }

//        // returns true on finish
//        public bool OnGUI()
//        {
//            var rect = Manager.Mgr.m_Canvas.pixelRect;
//            var width = rect.width * 0.5f;
//            var height = rect.height;
//            float lastLayerLeftWidth = 0.0f;
//            float lastLayerRightWidth = 0.0f;

//            m_UIScroll = GUI.BeginScrollView(new Rect(0.0f, 0.0f, m_UIWidth + 17.0f, height), m_UIScroll, new Rect(0.0f, 0.0f, m_UIWidth, m_UIHeight));
//            GUI.Box(new Rect(0.0f, 0.0f, m_UIWidth, m_UIHeight), "Structure editor");

//            // Reset structure
//            {
//                var reset = GUI.Button(new Rect(m_UIWidth - 50.0f, 5.0f, 50.0f, 25.0f), "Reset");
//                lastLayerRightWidth = Mathf.Max(lastLayerRightWidth, 50.0f);
//                if(reset)
//                {
//                    for(int i = 0; i < m_Structure.GetPilars().Length; ++i)
//                    {
//                        for(int j = 0; j < m_Structure.GetPilars()[i].GetBlocks().Count; ++j)
//                        {
//                            ((CBlockEdit)m_Structure.GetPilars()[i].GetBlocks()[j]).SetLayer(0);
//                        }
//                    }
//                    for(int i = 0; i < Def.MaxLayerSlots; ++i)
//                    {
//                        var info = LayerInfo.GetDefaultLayer();
//                        info.Slot = i + 1;
//                        m_Structure.SetLayer(info);
//                    }
//                    //m_IE.GenerateBridges = true;
//                    m_IE.SetName("");

//                    return true;
//                }
//            }

//            float lastHeight = 50.0f;

//            // Generate Bridges
//            //{
//            //    lastHeight += 0.2f;

//            //    m_IE.GenerateBridges = GUI.Toggle(new Rect(0.0f, lastHeight, 200.0f, 25.0f), m_IE.GenerateBridges, "Generate Bridges");
//            //    lastLayerLeftWidth = Mathf.Max(200.0f, lastLayerLeftWidth);
//            //    lastHeight += 25.0f;
//            //}

//            // Set name
//            {
//                GUI.Label(new Rect(0.0f, lastHeight, 50.0f, 25.0f), "Name:");
//                m_NameInput = GUI.TextField(new Rect(50.0f, lastHeight, 200.0f, 25.0f), m_NameInput, 256);
//                lastHeight += 25.0f;
//                lastLayerLeftWidth = Mathf.Max(lastLayerLeftWidth, 250.0f);
//                m_NameInput.Trim();
//            }
//            var tempName = m_NameInput.ToLower();
//            bool isValid = tempName != "unnamed";
//            //var mName = ie.Name.ToLower();
//            //for (int i = 0; i < Structures.Strucs.Count; ++i)
//            //{
//            //    if (i == struc.IDXIE || Structures.Strucs[i] == null)
//            //        continue;

//            //    var otherName = Structures.Strucs[i].Name.ToLower();
//            //    if(mName == otherName)
//            //    {
//            //        isValid = false;
//            //        break;
//            //    }
//            //}

//            bool apply = false;
//            if(!isValid)
//            {
//                GUI.Label(new Rect(m_UIWidth * 0.5f - 50.0f, lastHeight, 100.0f, 25.0f), "Invalid Name!");
//                lastHeight += 25.0f;
//            }
//            else
//            {
//                apply = GUI.Button(new Rect(m_UIWidth * 0.5f, lastHeight + 2.0f, 50.0f, 25.0f), "Apply");
//            }
//            bool cancel = GUI.Button(new Rect(m_UIWidth * 0.5f - 50.0f, lastHeight + 2.0f, 50.0f, 25.0f), "Cancel");
//            lastHeight += 25.0f;
            
//            GUI.EndScrollView();
//            m_UIHeight = lastHeight + 5.0f;
//            m_UIWidth = lastLayerLeftWidth + lastLayerRightWidth + 50.0f;
//            if (m_UIWidth < 275.0f)
//                m_UIWidth = 275.0f;

//            if(apply)
//            {
//                SetStrucName();
//            }

//            if (cancel || apply)
//                return true;


//            return false;
//        }
//    }
//}
