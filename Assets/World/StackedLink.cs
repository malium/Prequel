/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.World
{
	public struct StackedLink
	{
		public int PilarID;
		public int BlockIndex;

		public StackedLink(int pilarID = -1, int blockIndex = -1)
		{
			PilarID = pilarID;
			BlockIndex = blockIndex;
		}
		public bool IsValid() => PilarID >= 0 && BlockIndex >= 0;
	}
}