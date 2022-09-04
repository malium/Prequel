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
	public struct QuirkInfo
	{
		public List<IQuirkTrigger> Triggers;
		public IQuirk Quirk;
		public int Priority;
	}
	public abstract class IQuirk
	{
		protected CMonsterController m_Monster;
		protected string m_Name;
		protected List<IConfig> m_Configuration;

		protected IQuirk(string name)
		{
			m_Name = name;
			m_Configuration = new List<IConfig>();
		}
		public void SetMonster(CMonsterController monster)
		{
			m_Monster = monster;
		}
		public void SetConfiguration(IConfig config)
		{
			for(int i = 0; i < m_Configuration.Count; ++i)
			{
				var conf = m_Configuration[i];
				if (conf.GetConfigName() != config.GetConfigName())
					continue;
				conf.FromString(config.GetValueString());
			}
		}
		public List<IConfig> GetConfiguration() => m_Configuration;
		public abstract void UpdateQuirk();
		public virtual void OnFirstTrigger()
		{

		}
		public virtual void OnTransitioning(IQuirk nextQuirk)
		{

		}
		public string GetName() => m_Name;
	}
}
