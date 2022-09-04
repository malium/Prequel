/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Assets
//{
//    public class StrucQT
//    {
//        public const int MaxStrucCount = 16;
//        readonly List<IStruc> m_Strucs;
//        readonly StrucQT[] m_SubNodes;
//        StrucQT m_Parent;
//        RectInt m_Bounds;

//        public StrucQT GetParent() => m_Parent;
//        public StrucQT GetSubNode(int idx) => m_SubNodes[idx];
//        public RectInt GetBounds() => m_Bounds;

//        public StrucQT(RectInt bounds)
//        {
//            m_Bounds = bounds;
//            m_Strucs = new List<IStruc>(MaxStrucCount);
//            m_SubNodes = new StrucQT[4] { null, null, null, null };
//        }

//        int GetBestSubNode(RectInt area)
//        {
//            int nodeIdx = -1;
//            int testArea = -1;
//            for (int i = 0; i < m_SubNodes.Length; ++i)
//            {
//                int intArea = m_SubNodes[i].IntersectingArea(area);
//                if (intArea > testArea)
//                {
//                    testArea = intArea;
//                    nodeIdx = i;
//                }
//            }
//            return nodeIdx;
//        }

//        void InsertToSubNode(IStruc struc)
//        {
//            var nodeIdx = GetBestSubNode(struc.GetBounds());
//            m_SubNodes[nodeIdx].Insert(struc);
//        }

//        void RemoveFromSubNode(IStruc struc)
//        {
//            var nodeIdx = GetBestSubNode(struc.GetBounds());
//            m_SubNodes[nodeIdx].Remove(struc);
//        }

//        void Split()
//        {
//            int halfWidth = m_Bounds.width / 2;
//            int halfHeight = m_Bounds.height / 2;
			
//            // TopLeft
//            m_SubNodes[0] = new StrucQT(new RectInt(m_Bounds.x, m_Bounds.y, halfWidth, halfHeight));
//            // TopRight
//            m_SubNodes[1] = new StrucQT(new RectInt(m_Bounds.x + halfWidth, m_Bounds.y, halfWidth, halfHeight));
//            // BottomLeft
//            m_SubNodes[2] = new StrucQT(new RectInt(m_Bounds.x, m_Bounds.y + halfHeight, halfWidth, halfHeight));
//            // BottomRight
//            m_SubNodes[3] = new StrucQT(new RectInt(m_Bounds.x + halfWidth, m_Bounds.y + halfHeight, halfWidth, halfHeight));

//            for (int i = 0; i < m_SubNodes.Length; ++i)
//                m_SubNodes[i].m_Parent = this;

//            for (int i = 0; i < m_Strucs.Count; ++i)
//                InsertToSubNode(m_Strucs[i]);
//            m_Strucs.Clear();
//        }

//        public bool Contains(RectInt rect)
//        {
//            return m_Bounds.Contains(new Vector2Int(rect.x, rect.y))
//                || m_Bounds.Contains(new Vector2Int(rect.x + rect.width, rect.y))
//                || m_Bounds.Contains(new Vector2Int(rect.x, rect.y + rect.height))
//                || m_Bounds.Contains(new Vector2Int(rect.x + rect.width, rect.y + rect.height));
//        }

//        public int IntersectingArea(RectInt rect)
//        {
//            int width = Mathf.Min(m_Bounds.x + m_Bounds.width, rect.x + rect.width) - Mathf.Max(m_Bounds.x, rect.x);
//            int height = Mathf.Min(m_Bounds.y + m_Bounds.height, rect.y + rect.height) - Mathf.Max(m_Bounds.y, rect.y);
//            if (width < 0 || height < 0)
//                return 0;
//            return width * height;
//        }

//        public StrucQT GetParentNode(RectInt rect)
//        {
//            if(!Contains(rect))
//            {
//                return null;
//            }
//            if(m_SubNodes[0] != null)
//            {
//                for(int i = 0; i < m_SubNodes.Length; ++i)
//                {
//                    var parent = m_SubNodes[i].GetParentNode(rect);
//                    if (parent != null)
//                        return parent;
//                }
//                Debug.LogWarning("Couldn't find a suitable parent.");
//            }
//            return this;
//        }

//        public bool Contains(Vector2Int point)
//        {
//            return m_Bounds.Contains(point);
//        }

//        public bool Insert(IStruc struc)
//        {
//            if (!Contains(struc.GetBounds()))
//                return false;
//            if (m_SubNodes[0] != null)
//            {
//                InsertToSubNode(struc);
//                return true;
//            }
//            if((m_Strucs.Count + 1) > MaxStrucCount)
//            {
//                Split();
//                InsertToSubNode(struc);
//                return true;
//            }

//            m_Strucs.Add(struc);
//            //struc.SetQT(this);
//            return true;
//        }

//        public void Remove(IStruc struc)
//        {
//            if (!Contains(struc.GetBounds()))
//                return;
//            if(m_SubNodes[0] != null)
//            {
//                RemoveFromSubNode(struc);
//                return;
//            }
//            if (m_Strucs.Contains(struc))
//                m_Strucs.Remove(struc);
//        }

//        public void Clear(bool clearSubnodes = true)
//        {
//            m_Strucs.Clear();
//            if(clearSubnodes)
//            {
//                for (int i = 0; i < m_SubNodes.Length; ++i)
//                    m_SubNodes[i].Clear(clearSubnodes);
//            }
//        }

//        public List<IStruc> GetStrucs(bool recursive)
//        {
//            var rtn = new List<IStruc>(MaxStrucCount * m_SubNodes.Length);
//            rtn.AddRange(m_Strucs);
//            if (recursive)
//            {
//                for(int i = 0; i < m_SubNodes.Length; ++i)
//                {
//                    var childs = m_SubNodes[i].GetStrucs(recursive);
//                    rtn.AddRange(childs);
//                }
//            }
//            return rtn;
//        }

//        public List<IStruc> GetStrucsInArea(RectInt area)
//        {
//            List<IStruc> strucs = new List<IStruc>();
//            if(m_SubNodes[0] != null)
//            {
//                for(int i = 0; i < m_SubNodes.Length; ++i)
//                {
//                    strucs.AddRange(m_SubNodes[i].GetStrucsInArea(area));
//                }
//                return strucs;
//            }
//            for(int i = 0; i < m_Strucs.Count; ++i)
//            {
//                if (area.Overlaps(m_Strucs[i].GetBounds()))
//                    strucs.Add(m_Strucs[i]);
//            }
//            return strucs;
//        }

//        int GetStrucsNoAlloc(RectInt area, IStruc[] strucs, int idx)
//        {
//            int count = idx;
//            if (m_SubNodes[0] != null)
//            {
//                for (int i = 0; i < m_SubNodes.Length; ++i)
//                {
//                    count += GetStrucsNoAlloc(area, strucs, count);
//                }
//                return count;
//            }
//            for(int i = 0; i < m_Strucs.Count; ++i)
//            {
//                if(area.Overlaps(m_Strucs[i].GetBounds()))
//                {
//                    strucs[count++] = m_Strucs[i];
//                    if (strucs.Length == count)
//                        return count;
//                }
//            }
//            return count;
//        }

//        public int GetStrucsInAreaNoAlloc(RectInt area, IStruc[] strucs)
//        {
//            if (strucs.Length == 0)
//                return 0;
//            int count = 0;
//            if (m_SubNodes[0] != null)
//            {
//                for (int i = 0; i < m_SubNodes.Length; ++i)
//                {
//                    count += GetStrucsNoAlloc(area, strucs, count);
//                }
//                return count;
//            }

//            count = GetStrucsNoAlloc(area, strucs, count);

//            return count;
//        }

//        public List<IStruc> GetStrucsAtPoint(Vector2Int point)
//        {
//            List<IStruc> strucs = new List<IStruc>();
//            if (!m_Bounds.Contains(point))
//                return strucs;
//            if (m_SubNodes[0] != null)
//            {
//                for (int i = 0; i < m_SubNodes.Length; ++i)
//                {
//                    strucs.AddRange(m_SubNodes[i].GetStrucsAtPoint(point));
//                }
//                return strucs;
//            }
//            for (int i = 0; i < m_Strucs.Count; ++i)
//            {
//                if (m_Strucs[i].GetBounds().Contains(point))
//                    strucs.Add(m_Strucs[i]);
//            }
//            return strucs;
//        }

//        public CPilar GetPilarWithMapID(long mapID)
//        {
//            var pos = GameUtils.PosFromMapID(mapID);
//            if (!m_Bounds.Contains(pos))
//                return null;
//            if (m_SubNodes[0] != null)
//            {
//                for (int i = 0; i < m_SubNodes.Length; ++i)
//                {
//                    var pilar = m_SubNodes[i].GetPilarWithMapID(mapID);
//                    if (pilar != null)
//                        return pilar;
//                }
//                return null;
//            }
//            for(int i = 0; i < m_Strucs.Count; ++i)
//            {
//                var bounds = m_Strucs[i].GetBounds();
//                if (!bounds.Contains(pos))
//                    continue;
//                var pilarID = m_Strucs[i].PilarIDFromVPos(pos - new Vector2Int(bounds.x, bounds.y));
//                var pilar = m_Strucs[i].GetPilars()[pilarID];
//                if (pilar != null)
//                    return pilar;
//            }
//            return null;
//        }
		
//        public override bool Equals(object obj)
//        {
//            return obj is StrucQT qT &&
//                   EqualityComparer<List<IStruc>>.Default.Equals(m_Strucs, qT.m_Strucs) &&
//                   EqualityComparer<StrucQT[]>.Default.Equals(m_SubNodes, qT.m_SubNodes) &&
//                   EqualityComparer<StrucQT>.Default.Equals(m_Parent, qT.m_Parent) &&
//                   m_Bounds.Equals(qT.m_Bounds);
//        }

//        public override int GetHashCode()
//        {
//            int hashCode = -640306508;
//            hashCode = hashCode * -1521134295 + EqualityComparer<List<IStruc>>.Default.GetHashCode(m_Strucs);
//            hashCode = hashCode * -1521134295 + EqualityComparer<StrucQT[]>.Default.GetHashCode(m_SubNodes);
//            hashCode = hashCode * -1521134295 + EqualityComparer<StrucQT>.Default.GetHashCode(m_Parent);
//            hashCode = hashCode * -1521134295 + m_Bounds.GetHashCode();
//            return hashCode;
//        }

//        public static bool operator ==(StrucQT left, StrucQT right)
//        {
//            return EqualityComparer<StrucQT>.Default.Equals(left, right);
//        }

//        public static bool operator !=(StrucQT left, StrucQT right)
//        {
//            return !(left == right);
//        }
//    }
//}