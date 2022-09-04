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

namespace Assets.AI.ODD
{
	public abstract class IOddState
	{
		public COddController Controller { get; set; }
		public abstract string StateName { get; }
		public abstract OddState State { get; }
		public virtual void OnStart() {  }
		public virtual void OnStop() {  }
		public virtual void OnUpdate() {  }
		public virtual void OnFixedUpdate() {  }
		public virtual void OnLateUpdate() {  }
		public virtual void OnGizmos() { }
		public virtual void OnGUI() { }
	}
}
