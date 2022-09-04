/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AI.ODD
{
	public class NullState : IOddState
	{
		public override string StateName => "Null";
		public override OddState State => OddState.COUNT;
		public override void OnFixedUpdate()
		{
			base.OnFixedUpdate();
		}
		public override void OnLateUpdate()
		{
			base.OnLateUpdate();
		}
		public override void OnStart()
		{
			base.OnStart();
		}
		public override void OnStop()
		{
			base.OnStop();
		}
		public override void OnUpdate()
		{
			base.OnUpdate();
		}
		public override void OnGizmos()
		{
			base.OnGizmos();
		}
		public override void OnGUI()
		{
			base.OnGUI();
		}
	}
}
