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

//namespace Assets.AI.Quirks
//{
//	public class RoamingQuirk : IQuirk
//	{
//		public new static IConfig[] DefaultConfig = new IConfig[]
//		{
//			CreateConfig("StayInStructure", false),
//			CreateConfig("MaxRoamDistance", 10f),
//			CreateConfig("ExtraTime", 3f),
//		};

//		ConfigBoolean StayInStructure;
//		ConfigFloat MaxRoamDistance;
//		ConfigFloat ExtraTime;

//		float NextWakeUpTime;
//		float NextTargetTime;
//		static List<CStruc> m_NearStrucs;

//		public RoamingQuirk()
//			:base("RoamingQuirk")
//		{
//			StayInStructure = new ConfigBoolean(DefaultConfig[0]);
//			MaxRoamDistance = new ConfigFloat(DefaultConfig[1]);
//			ExtraTime = new ConfigFloat(DefaultConfig[2]);

//			m_Configuration = new List<IConfig>()
//			{
//				StayInStructure,
//				MaxRoamDistance,
//				ExtraTime,
//			};

//			if(m_NearStrucs == null)
//				m_NearStrucs = new List<CStruc>();
//			NextTargetTime = 0f;
//		}
//		public override void FixedUpdate()
//		{
//			// If it took so long to reach, compute another target
//			if (Time.time > NextTargetTime)
//				ComputeNextTarget();
//		}
//		//public override void SetConfig(IQuirkConfig config)
//		//{
//		//	var obj = GetType().InvokeMember(config.GetConfigName(), 
//		//		System.Reflection.BindingFlags.Instance |
//		//		System.Reflection.BindingFlags.NonPublic |
//		//		System.Reflection.BindingFlags.GetField, 
//		//		null, this, null);

//		//	var type = GetType().GetField(config.GetConfigName());
//		//	type.SetValue(obj, config.GetValueGen());
			
//		//	//if (type == null)
//		//	//	return;
//		//	//switch (config.GetConfigType())
//		//	//{
//		//	//	case QuirkConfigType.STRING:
//		//	//		break;
//		//	//	case QuirkConfigType.INTEGER:
//		//	//		break;
//		//	//	case QuirkConfigType.FLOAT:
//		//	//		break;
//		//	//	case QuirkConfigType.BOOLEAN:
//		//	//		break;
//		//	//}
//		//	//if (config.GetConfigName() == DefaultConfig[0].GetConfigName())
//		//	//{
//		//	//	SetConfig(ref StayInStructure, config);
//		//	//}
//		//	//else if(config.GetConfigName() == DefaultConfig[1].GetConfigName())
//		//	//{
//		//	//	SetConfig(ref MaxRoamDistance, config);
//		//	//}
//		//	//else if(config.GetConfigName() == DefaultConfig[2].GetConfigName())
//		//	//{
//		//	//	SetConfig(ref ExtraTime, config);
//		//	//}
//		//}
//		void ComputeNextTarget()
//		{
//			CStruc struc = null;
//			if (StayInStructure.GetValue())
//			{
//				struc = m_Monster.GetLE().GetCurrentStruc() as CStruc;
//			}
//			else
//			{
//				m_NearStrucs.Clear();
//				var pos = m_Monster.transform.position;
//				var map = Map.GetCurrent();
//				map.GetStrucsRange(new Vector2(pos.x, pos.z), MaxRoamDistance.GetValue(), ref m_NearStrucs);
//				if (m_NearStrucs.Count == 0)
//				{
//					Debug.LogWarning("There weren't near structs to roam!");
//					return;
//				}
//				struc = m_NearStrucs[UnityEngine.Random.Range(0, m_NearStrucs.Count)];
//			}

//			var validPilars = struc.GetValidPilars();
//			var pilar = validPilars[UnityEngine.Random.Range(0, validPilars.Count)];
//			var targetPos = new Vector3(pilar.transform.position.x + UnityEngine.Random.value, 0f, pilar.transform.position.z + UnityEngine.Random.value);
//			var targetDir = (targetPos - m_Monster.transform.position);
//			targetDir.y = 0f;
//			targetDir.Normalize();
//			m_Monster.SetTargetPostion(targetPos);
//			//m_Monster.GetME().SetTarget(targetPos, targetDir);
//			var speed = m_Monster.GetME().GetMaxSpeed() * 0.75f;
//			var dist = Vector3.Distance(targetPos, m_Monster.transform.position);
//			var extra = UnityEngine.Random.Range(0f, ExtraTime.GetValue());
//			NextTargetTime = Time.time + dist / speed + extra;
//		}
//	}
//}
