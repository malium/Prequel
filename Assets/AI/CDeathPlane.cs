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

namespace Assets.AI
{
	class CDeathPlane : MonoBehaviour
	{
		//void OnTriggerStay(Collider other)
		//{
		//	Debug.Log(other.name);
		//	if (other.gameObject.layer != Def.RCLayerLE)
		//		return;

		//	var le = other.gameObject.GetComponent<CLivingEntity>();
		//	le.ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, le.GetCurrentHealth());
		//}
		void OnTriggerEnter(Collider other)
		{
			Debug.Log(other.name);
			if (other.gameObject.layer != Def.RCLayerLE)
				return;

			var le = other.gameObject.GetComponent<CLivingEntity>();
			le.ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, le.GetCurrentHealth());
		}
	}
}
