/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UI
{
	public class CViewSorted : CViewB
	{
		Func<CViewElementB, CViewElementB, bool> m_SortFn;
		void Sort()
		{
			if (m_SortFn == null)
				return;

			void BubbleSort(int n)
			{
				if (n <= 1)
					return;

				int count = 0;

				for(int i = 0; i < (n - 1); ++i)
				{
					var elemLeft = m_Elements[i];
					var elemRight = m_Elements[i + 1];

					if(m_SortFn(elemLeft, elemRight))
					{
						m_Elements[i + 1] = elemLeft;
						m_Elements[i] = elemRight;
						++count;
					}
				}

				if (count == 0)
					return;

				BubbleSort(n - 1);
			}

			BubbleSort(m_Elements.Count);
			SetElements();
		}
		// Has to return true, when the right element has to go previously than the left one, false otherwise
		public void SetSortFn(Func<CViewElementB, CViewElementB, bool> sortFn)
		{
			m_SortFn = sortFn;
			Sort();
		}
		public override void AddElement(ViewElementInfoB info)
		{
			base.AddElement(info);

			Sort();
		}
		public override void AddElements(List<ViewElementInfoB> infos)
		{
			base.AddElements(infos);

			Sort();
		}
	}
}
