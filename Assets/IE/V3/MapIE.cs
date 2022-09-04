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

namespace Assets.IE.V3
{
	public class MapIE
	{
		public enum CommandType
		{
			STRUC_PLACED,

			PROP_PLACED,
			PROP_DEATH,

			MONSTER_PLACED,
			MONSTER_DEATH,

			ANT_PLACED_TOP,
			ANT_PLACED_SIDE_U,
			ANT_PLACED_SIDE_D,
			ANT_DEATH,

			PLACED_BLOCK,
			PLACED_BOMB,

			COUNT
		}

		public struct Command
		{
			public CommandType CmdType;
			public string ObjectName;
			public string ObjectAttribs;
			public long MapPilarID;
			public int PilarBlockID;
			public int RngSeed;

			public Command(
				CommandType type, string objName, string objAttribs,
				long mapPilarID, int pilarBlockID)
			{
				CmdType = type;
				ObjectName = objName;
				ObjectAttribs = objAttribs;
				MapPilarID = mapPilarID;
				PilarBlockID = pilarBlockID;
				RngSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
				//UnityEngine.Random.InitState(RngSeed);
			}
		}

		List<Command> m_Commands;
	}
}
