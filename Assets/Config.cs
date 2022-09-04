/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
	public struct ConfigInfo
	{
		public string Name;
		public Def.ConfigType ConfigType;
		public string Value;
		public IConfig Create(IConfig defaultEnumConf = null)
		{
			IConfig config = null;
			switch (ConfigType)
			{
				case Def.ConfigType.STRING:
					config = new ConfigString(Name, "");
					break;
				case Def.ConfigType.INTEGER:
					config = new ConfigInteger(Name, 0);
					break;
				case Def.ConfigType.FLOAT:
					config = new ConfigFloat(Name, 0f);
					break;
				case Def.ConfigType.BOOLEAN:
					config = new ConfigBoolean(Name, false);
					break;
				case Def.ConfigType.VECTOR2:
					config = new ConfigVector2(Name, Vector2.zero);
					break;
				case Def.ConfigType.VECTOR3:
					config = new ConfigVector3(Name, Vector3.zero);
					break;
				case Def.ConfigType.ENUM:
					if (defaultEnumConf == null)
					{
						Debug.LogWarning("In order to create a ConfigEnum, you must provide a default one!, " + Name);
						return null;
					}
					config = Activator.CreateInstance(defaultEnumConf.GetType(), (IConfig)defaultEnumConf) as IConfig;
					break;
				case Def.ConfigType.ENTITYLIST:
					config = new ConfigEntityList(Name, new List<string>());
					break;
				case Def.ConfigType.PARTICLE_TEXTURE:
					config = new ConfigParticleTexture(Name, "");
					break;
				case Def.ConfigType.SPELL_WEAPON:
					config = new ConfigSpellWeapon(Name, "");
					break;
				default:
					Debug.LogError("Unhandled config type: " + ConfigType.ToString());
					break;
			}
			config.FromString(Value);
			return config;
		}
	}
	public abstract class IConfig
	{
		string m_Name;
		Def.ConfigType m_ConfigType;

		protected IConfig(string name, Def.ConfigType configType)
		{
			m_Name = name;
			m_ConfigType = configType;
		}
		public Def.ConfigType GetConfigType() => m_ConfigType;
		public abstract bool FromString(string value);
		public abstract string GetValueString();
		public string GetConfigName() => m_Name;
		public abstract Type GetBaseType();
	}
	public class ConfigString : IConfig
	{
		string m_Value;

		public ConfigString(IConfig copy)
			: base(copy.GetConfigName(), Def.ConfigType.STRING)
		{
			if (copy.GetConfigType() != Def.ConfigType.STRING)
				return;
			var conf = copy as ConfigString;
			m_Value = conf.m_Value;
		}
		public ConfigString(string name, string value)
			: base(name, Def.ConfigType.STRING)
		{
			m_Value = value;
		}
		public void SetValue(string value) => m_Value = value;
		public string GetValue() => m_Value;
		public override bool FromString(string value)
		{
			SetValue(value);
			return true;
		}
		public override string GetValueString() => m_Value;
		public override Type GetBaseType() => m_Value.GetType();
	}
	public class ConfigInteger : IConfig
	{
		int m_Value;

		public ConfigInteger(IConfig copy)
			: base(copy.GetConfigName(), Def.ConfigType.INTEGER)
		{
			if (copy.GetConfigType() != Def.ConfigType.INTEGER)
				return;
			var conf = copy as ConfigInteger;
			m_Value = conf.m_Value;
		}
		public ConfigInteger(string name, int value)
			: base(name, Def.ConfigType.INTEGER)
		{
			m_Value = value;
		}
		public void SetValue(int value) => m_Value = value;
		public int GetValue() => m_Value;
		public override bool FromString(string value)
		{
			if (int.TryParse(value, out int res))
			{
				SetValue(res);
				return true;
			}
			return false;
		}
		public override string GetValueString() => m_Value.ToString();
		public override Type GetBaseType() => m_Value.GetType();
	}
	public class ConfigFloat : IConfig
	{
		float m_Value;

		public ConfigFloat(IConfig copy)
			: base(copy.GetConfigName(), Def.ConfigType.FLOAT)
		{
			if (copy.GetConfigType() != Def.ConfigType.FLOAT)
				return;
			var conf = copy as ConfigFloat;
			m_Value = conf.m_Value;
		}
		public ConfigFloat(string name, float value)
			: base(name, Def.ConfigType.FLOAT)
		{
			m_Value = value;
		}
		public void SetValue(float value) => m_Value = value;
		public float GetValue() => m_Value;
		public override bool FromString(string value)
		{
			if (float.TryParse(value, out float res))
			{
				SetValue(res);
				return true;
			}
			return false;
		}
		public override string GetValueString() => m_Value.ToString();
		public override Type GetBaseType() => m_Value.GetType();
	}
	public class ConfigBoolean : IConfig
	{
		bool m_Value;

		public ConfigBoolean(IConfig copy)
			: base(copy.GetConfigName(), Def.ConfigType.BOOLEAN)
		{
			if (copy.GetConfigType() != Def.ConfigType.BOOLEAN)
				return;
			var conf = copy as ConfigBoolean;
			m_Value = conf.m_Value;
		}
		public ConfigBoolean(string name, bool value)
			: base(name, Def.ConfigType.BOOLEAN)
		{
			m_Value = value;
		}
		public void SetValue(bool value) => m_Value = value;
		public bool GetValue() => m_Value;
		public override bool FromString(string value)
		{
			if (bool.TryParse(value, out bool res))
			{
				SetValue(res);
				return true;
			}
			return false;
		}
		public override string GetValueString() => m_Value.ToString();
		public override Type GetBaseType() => m_Value.GetType();
	}
	public class ConfigVector2 : IConfig
	{
		Vector2 m_Value;

		public ConfigVector2(IConfig copy)
			: base(copy.GetConfigName(), Def.ConfigType.VECTOR2)
		{
			if (copy.GetConfigType() != Def.ConfigType.VECTOR2)
				return;
			var conf = copy as ConfigVector2;
			m_Value = conf.m_Value;
		}
		public ConfigVector2(string name, Vector2 value)
			: base(name, Def.ConfigType.VECTOR2)
		{
			m_Value = value;
		}
		public void SetValue(Vector2 value) => m_Value = value;
		public Vector2 GetValue() => m_Value;
		public override bool FromString(string value)
		{
			var split = value.Split(';');
			if (split.Length != 2)
				return false; 
			for (int i = 0; i < split.Length; ++i)
				split[i] = split[i].Trim();

			bool parseOK = float.TryParse(split[0], out float x);
			if (!parseOK)
				return false;
			parseOK = float.TryParse(split[1], out float y);
			if (!parseOK)
				return false;
			SetValue(new Vector2(x, y));
			return true;
		}
		public override string GetValueString()
		{
			return m_Value.x.ToString() + ';' + m_Value.y.ToString();
		}
		public override Type GetBaseType() => m_Value.GetType();
	}
	public class ConfigVector3 : IConfig
	{
		Vector3 m_Value;

		public ConfigVector3(IConfig copy)
			: base(copy.GetConfigName(), Def.ConfigType.VECTOR3)
		{
			if (copy.GetConfigType() != Def.ConfigType.VECTOR3)
				return;
			var conf = copy as ConfigVector3;
			m_Value = conf.m_Value;
		}
		public ConfigVector3(string name, Vector3 value)
			: base(name, Def.ConfigType.VECTOR3)
		{
			m_Value = value;
		}
		public void SetValue(Vector3 value) => m_Value = value;
		public Vector3 GetValue() => m_Value;
		public override bool FromString(string value)
		{
			var split = value.Split(';');
			if (split.Length != 3)
				return false;
			for (int i = 0; i < split.Length; ++i)
				split[i] = split[i].Trim();

			bool parseOK = float.TryParse(split[0], out float x);
			if (!parseOK)
				return false;
			parseOK = float.TryParse(split[1], out float y);
			if (!parseOK)
				return false;
			parseOK = float.TryParse(split[2], out float z);
			if (!parseOK)
				return false;
			SetValue(new Vector3(x, y, z));
			return true;
		}
		public override string GetValueString()
		{
			return m_Value.x.ToString() + ';' + m_Value.y.ToString() + ';' + m_Value.z.ToString();
		}
		public override Type GetBaseType() => m_Value.GetType();
	}
	public class ConfigEnum<T> : IConfig where T : struct
	{
		T m_Value;

		public ConfigEnum(IConfig copy)
			: base(copy.GetConfigName(), Def.ConfigType.ENUM)
		{
			if (copy.GetConfigType() != Def.ConfigType.ENUM)
				return;
			var conf = copy as ConfigEnum<T>;
			m_Value = conf.m_Value;
		}
		public ConfigEnum(string name, T value)
			: base(name, Def.ConfigType.ENUM)
		{
			m_Value = value;
		}
		public override bool FromString(string value)
		{
			if (Enum.TryParse(value, out T eValue))
			{
				m_Value = eValue;
				return true;
			}
			return false;
		}
		public override string GetValueString() => m_Value.ToString();
		public void SetValue(T value) => m_Value = value;
		public T GetValue() => m_Value;
		public override Type GetBaseType() => m_Value.GetType();
	}
	public class ConfigEntityList : IConfig
	{
		List<string> m_Value;

		public ConfigEntityList(IConfig copy)
			: base(copy.GetConfigName(), Def.ConfigType.ENTITYLIST)
		{
			if (copy.GetConfigType() != Def.ConfigType.ENTITYLIST)
				return;
			var conf = copy as ConfigEntityList;
			m_Value = new List<string>(conf.m_Value);
		}
		public ConfigEntityList(string name, List<string> value)
			: base(name, Def.ConfigType.ENTITYLIST)
		{
			m_Value = new List<string>(value);
		}
		public void SetValue(List<string> value) => m_Value = value;
		public List<string> GetValue() => m_Value;
		public override bool FromString(string value)
		{
			m_Value = new List<string>(value.Split(','));
			return true;
		}
		public override string GetValueString()
		{
			string temp = default;
			for (int i = 0; i < m_Value.Count; ++i)
			{
				temp += m_Value[i];
				if (i < (m_Value.Count - 1))
					temp += ',';
			}
			return temp;
		}
		public override Type GetBaseType() => m_Value.GetType();
	}
	public class ConfigParticleTexture : IConfig
	{
		string m_Value;

		public ConfigParticleTexture(IConfig copy)
			: base(copy.GetConfigName(), Def.ConfigType.PARTICLE_TEXTURE)
		{
			if (copy.GetConfigType() != Def.ConfigType.PARTICLE_TEXTURE)
				return;
			var conf = copy as ConfigParticleTexture;
			m_Value = conf.m_Value;
		}
		public ConfigParticleTexture(string name, string value)
			: base(name, Def.ConfigType.PARTICLE_TEXTURE)
		{
			m_Value = value;
		}
		public void SetValue(string value) => m_Value = value;
		public string GetValue() => m_Value;
		public override bool FromString(string value)
		{
			SetValue(value);
			return true;
		}
		public override string GetValueString() => m_Value;
		public override Type GetBaseType() => m_Value.GetType();
	}
	public class ConfigSpellWeapon : IConfig
	{
		string m_Value;

		public ConfigSpellWeapon(IConfig copy)
			: base(copy.GetConfigName(), Def.ConfigType.SPELL_WEAPON)
		{
			if (copy.GetConfigType() != Def.ConfigType.SPELL_WEAPON)
				return;
			var conf = copy as ConfigSpellWeapon;
			m_Value = conf.m_Value;
		}
		public ConfigSpellWeapon(string name, string value)
			: base(name, Def.ConfigType.SPELL_WEAPON)
		{
			m_Value = value;
		}
		public void SetValue(string value) => m_Value = value;
		public string GetValue() => m_Value;
		public override bool FromString(string value)
		{
			SetValue(value);
			return true;
		}
		public override string GetValueString() => m_Value;
		public override Type GetBaseType() => m_Value.GetType();
	}
}