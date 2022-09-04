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

namespace Assets.AI.Quirks
{
	public class QuirkRoaming : IQuirk
	{
		ConfigBoolean StayInStructure;
		ConfigFloat MaxRoamDistance;
		ConfigFloat ExtraTime;

		Timer m_Timer;
		//static List<CStruc> m_NearStrucs;
		public QuirkRoaming()
			: base("Roaming")
		{
			StayInStructure = new ConfigBoolean("StayInStructure", false);
			MaxRoamDistance = new ConfigFloat("MaxRoamDistance", 10f);
			ExtraTime = new ConfigFloat("ExtraTime", 3f);
			m_Configuration.Add(StayInStructure);
			m_Configuration.Add(MaxRoamDistance);
			m_Configuration.Add(ExtraTime);

			m_Timer = new Timer();
			m_Timer.OnTimerTrigger += OnTimerFinished;
			//if (m_NearStrucs == null)
			//	m_NearStrucs = new List<CStruc>();
			//OnTimerFinished();
		}
		public override void OnFirstTrigger()
		{
			OnTimerFinished();
		}
		void OnTimerFinished()
		{
			//if(m_Monster == null)
			//{
			//	m_Timer.Reset(1f);
			//	return;
			//}
			CStruc struc;
			if (StayInStructure.GetValue())
			{
				struc = m_Monster.GetLE().GetCurrentStruc() as CStruc;
			}
			else
			{
				//m_NearStrucs.Clear();
				var nearStrucs = m_Monster._GetNearStrucs();
				var distanceToCheck = MaxRoamDistance.GetValue();
				var distanceChecked = m_Monster._GetNearStrucsDistance();
				if (distanceToCheck > distanceChecked)
				{
					nearStrucs.Clear();
					var world = World.World.gWorld;
					world.StrucsRangeWPos(new Vector2(m_Monster.transform.position.x, m_Monster.transform.position.z), distanceToCheck, ref nearStrucs);
					m_Monster._SetNearStructs(nearStrucs, distanceToCheck);
				}

				//var pos = m_Monster.transform.position;
				
				
				//var vwPos = GameUtils.TransformPosition(new Vector2(pos.x, pos.z));
				//var vPos = world.VPosFromVWPos(vwPos);
				//world.StrucsRangeVPos(vPos, Mathf.FloorToInt(MaxRoamDistance.GetValue()), ref m_NearStrucs);

				//var map = Map.GetCurrent();
				//map.GetStrucsRange(new Vector2(pos.x, pos.z), MaxRoamDistance.GetValue(), ref m_NearStrucs);
				if (nearStrucs.Count == 0)
				{
					Debug.LogWarning("There weren't near structs to roam!");
					return;
				}
				struc = nearStrucs[UnityEngine.Random.Range(0, nearStrucs.Count)];
			}
			var validPilars = struc.GetValidPilars();
			var pilar = validPilars[UnityEngine.Random.Range(0, validPilars.Count)];
			var targetPos = new Vector3(pilar.transform.position.x + UnityEngine.Random.value, 0f, pilar.transform.position.z + UnityEngine.Random.value);
			m_Monster.SetTargetPostion(targetPos);
			var speed = m_Monster.GetME().GetMaxSpeed() * 0.75f;
			var dist = Vector3.Distance(targetPos, m_Monster.transform.position);
			m_Timer.Reset(dist / speed + ExtraTime.GetValue());
		}
		public override void UpdateQuirk()
		{
			m_Timer.Update();
			if (m_Monster.IsTargetReached() && m_Timer.GetRemainingTime() > ExtraTime.GetValue())
			{
				m_Timer.Reset(ExtraTime.GetValue());
			}

		}
	}
}