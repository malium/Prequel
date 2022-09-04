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
//	public abstract class IQuirk
//	{
//		public static IConfig CreateConfig(string name, float defaultValue)
//		{
//			return new ConfigFloat(name, defaultValue);
//		}
//		public static IConfig CreateConfig(string name, bool defaultValue)
//		{
//			return new ConfigBoolean(name, defaultValue);
//		}
//		public static IConfig CreateConfig(string name, int defaultValue)
//		{
//			return new ConfigInteger(name, defaultValue);
//		}
//		public static IConfig CreateConfig(string name, string defaultValue)
//		{
//			return new ConfigString(name, defaultValue);
//		}
//		public static IConfig CreateConfig(string name, Vector2 defaultValue)
//		{
//			return new ConfigVector2(name, defaultValue);
//		}
//		public static IConfig CreateConfig(string name, Vector3 defaultValue)
//		{
//			return new ConfigVector3(name, defaultValue);
//		}
//		public static IConfig CreateConfig<T>(string name, T defaultValue) where T : struct
//		{
//			return new ConfigEnum<T>(name, defaultValue);
//		}
//		public static IConfig[] DefaultConfig;

//		public IConfig[] GetDefaultConfig() => DefaultConfig;
//		public List<IConfig> GetCurrentConfig() => m_Configuration;

//		protected CMonsterController m_Monster;
//		protected string m_Name;
//		protected List<IConfig> m_Configuration;
//		bool m_Enabled;

//		protected IQuirk(string name)
//		{
//			m_Name = name;
//			m_Enabled = true;
//		}

//		//public abstract void SetConfig(IQuirkConfig config);
//		public void Init(CMonsterController monsterController)
//		{
//			m_Monster = monsterController;
//		}
//		public string GetName() => m_Name;
//		public CMonsterController GetMonsterController() => m_Monster;
//		public void SetConfig(IConfig config)
//		{
//			for (int i = 0; i < m_Configuration.Count; ++i)
//			{
//				var conf = m_Configuration[i];
//				if(conf.GetConfigName() == config.GetConfigName())
//				{
//					conf.FromString(config.GetValueString());
//					break;
//				}
//			}
//		}
//		public IConfig GetConfig(string configName)
//		{
//			for(int i = 0; i < m_Configuration.Count; ++i)
//			{
//				var conf = m_Configuration[i];
//				if (conf.GetConfigName() == configName)
//					return conf;
//			}
//			return null;
//		}
//		public virtual void NearEntites(List<CLivingEntity> entities)
//		{

//		}
//		public virtual void NearEnemies(List<CLivingEntity> enemies)
//		{

//		}
//		public virtual void NearFriends(List<CLivingEntity> friends)
//		{

//		}
//		public virtual void Moved(Vector3 prev, Vector3 next)
//		{

//		}
//		public virtual void DamageReceived(CLivingEntity caster, Def.DamageType damageType, float amount)
//		{

//		}
//		public virtual void ElementReceived(Def.ElementType elementType, float amount)
//		{

//		}
//		public virtual void TargetReached()
//		{

//		}
//		public virtual void Death()
//		{

//		}
//		public virtual void Update()
//		{

//		}
//		public virtual void FixedUpdate()
//		{

//		}
//		public virtual void OnEnabled()
//		{

//		}
//		public virtual void OnDisabled()
//		{

//		}
//		public void SetEnabled(bool enabled)
//		{
//			if (enabled == m_Enabled)
//				return;

//			m_Enabled = enabled;
//			if(enabled)
//			{
//				//m_Monster.OnQuirkEnabled(this);
//				OnEnabled();
//			}
//			else
//			{
//				//m_Monster.OnQuirkDisabled(this);
//				OnDisabled();
//			}
//		}
//		public bool IsEnabled() => m_Enabled;
//	}
//}
