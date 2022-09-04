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

namespace Assets.UI
{
	public class CViewElementBConfig : CViewElementB
	{
		// Name
		public TMPro.TMP_Text Name;
		// Boolean
		public UnityEngine.UI.Toggle ValueToggle;
		// Float, Int, String
		public TMPro.TMP_InputField ValueIF;
		// Vector2, Vector3
		public TMPro.TMP_Text[] ValueTextV3;
		public TMPro.TMP_InputField[] ValueV3IF;
		// Enum
		public TMPro.TMP_Dropdown ValueDropdown;

		IConfig m_Config;
		bool m_ValueLock;
		public IConfig GetConfig() => m_Config;

		UnityEngine.Events.UnityAction<string>[] OnV3IF;

		static CViewElementBConfig InstanceConfig;
		public static CViewElementBConfig GetConfigInstance()
		{
			if (InstanceConfig == null)
			{
				InstanceConfig = Resources.Load<CViewElementBConfig>("UI/ViewElementConfig");
				if (InstanceConfig == null)
					throw new Exception("Couldn't load ViewElementConfig");
			}
			return InstanceConfig;
		}

		void StringConfig()
		{
			ValueToggle.gameObject.SetActive(false);
			ValueIF.gameObject.SetActive(true);
			for (int i = 0; i < ValueTextV3.Length; ++i) ValueTextV3[i].gameObject.SetActive(false);
			for (int i = 0; i < ValueV3IF.Length; ++i) ValueV3IF[i].gameObject.SetActive(false);
			ValueDropdown.gameObject.SetActive(false);
			m_ValueLock = true;
			ValueIF.text = m_Config.GetValueString();
			ValueIF.contentType = TMPro.TMP_InputField.ContentType.Alphanumeric;
			m_ValueLock = false;
		}
		void IntegerConfig()
		{
			ValueToggle.gameObject.SetActive(false);
			ValueIF.gameObject.SetActive(true);
			for (int i = 0; i < ValueTextV3.Length; ++i) ValueTextV3[i].gameObject.SetActive(false);
			for (int i = 0; i < ValueV3IF.Length; ++i) ValueV3IF[i].gameObject.SetActive(false);
			ValueDropdown.gameObject.SetActive(false);

			m_ValueLock = true;
			ValueIF.text = m_Config.GetValueString();
			ValueIF.contentType = TMPro.TMP_InputField.ContentType.IntegerNumber;
			m_ValueLock = false;
		}
		void FloatConfig()
		{
			ValueToggle.gameObject.SetActive(false);
			ValueIF.gameObject.SetActive(true);
			for (int i = 0; i < ValueTextV3.Length; ++i) ValueTextV3[i].gameObject.SetActive(false);
			for (int i = 0; i < ValueV3IF.Length; ++i) ValueV3IF[i].gameObject.SetActive(false);
			ValueDropdown.gameObject.SetActive(false);

			m_ValueLock = true;
			ValueIF.text = m_Config.GetValueString();
			ValueIF.contentType = TMPro.TMP_InputField.ContentType.DecimalNumber;
			m_ValueLock = false;
		}
		void BoolConfig()
		{
			ValueToggle.gameObject.SetActive(true);
			ValueIF.gameObject.SetActive(false);
			for (int i = 0; i < ValueTextV3.Length; ++i) ValueTextV3[i].gameObject.SetActive(false);
			for (int i = 0; i < ValueV3IF.Length; ++i) ValueV3IF[i].gameObject.SetActive(false);
			ValueDropdown.gameObject.SetActive(false);

			m_ValueLock = true;
			ValueToggle.isOn = (m_Config as ConfigBoolean).GetValue();
			m_ValueLock = false;
		}
		void Vector2Config()
		{
			ValueToggle.gameObject.SetActive(false);
			ValueIF.gameObject.SetActive(false);
			for (int i = 0; i < ValueTextV3.Length; ++i) ValueTextV3[i].gameObject.SetActive(i < 2);
			for (int i = 0; i < ValueV3IF.Length; ++i) ValueV3IF[i].gameObject.SetActive(i < 2);
			ValueDropdown.gameObject.SetActive(false);

			m_ValueLock = true;
			var cfg = m_Config as ConfigVector2;
			ValueV3IF[0].text = cfg.GetValue().x.ToString();
			ValueV3IF[1].text = cfg.GetValue().y.ToString();
			m_ValueLock = false;
		}
		void Vector3Config()
		{
			ValueToggle.gameObject.SetActive(false);
			ValueIF.gameObject.SetActive(false);
			for (int i = 0; i < ValueTextV3.Length; ++i) ValueTextV3[i].gameObject.SetActive(true);
			for (int i = 0; i < ValueV3IF.Length; ++i) ValueV3IF[i].gameObject.SetActive(true);
			ValueDropdown.gameObject.SetActive(false);

			m_ValueLock = true;
			var cfg = m_Config as ConfigVector3;
			ValueV3IF[0].text = cfg.GetValue().x.ToString();
			ValueV3IF[1].text = cfg.GetValue().y.ToString();
			ValueV3IF[2].text = cfg.GetValue().z.ToString();
			m_ValueLock = false;
		}
		void EnumConfig()
		{
			ValueToggle.gameObject.SetActive(false);
			ValueIF.gameObject.SetActive(false);
			for (int i = 0; i < ValueTextV3.Length; ++i) ValueTextV3[i].gameObject.SetActive(false);
			for (int i = 0; i < ValueV3IF.Length; ++i) ValueV3IF[i].gameObject.SetActive(false);
			ValueDropdown.gameObject.SetActive(true);

			m_ValueLock = true;
			var names = new List<string>(Enum.GetNames(m_Config.GetBaseType()));
			var curIdx = names.FindIndex((string test) => test == m_Config.GetValueString());
			if (curIdx < 0)
				curIdx = 0;
			ValueDropdown.ClearOptions();
			ValueDropdown.AddOptions(names);
			ValueDropdown.value = curIdx;
			m_ValueLock = false;
			ValueDropdown.RefreshShownValue();
		}
		public override void ElementInit(CViewB view, ViewElementInfoB info)
		{
			base.ElementInit(view, info);
			m_Config = (info as ViewElementBConfigInfo).Config;
			
			if (Name != null)
				Name.text = m_Config.GetConfigName();

			switch (m_Config.GetConfigType())
			{
				case Def.ConfigType.STRING:
					StringConfig();
					break;
				case Def.ConfigType.INTEGER:
					IntegerConfig();
					break;
				case Def.ConfigType.FLOAT:
					FloatConfig();
					break;
				case Def.ConfigType.BOOLEAN:
					BoolConfig();
					break;
				case Def.ConfigType.VECTOR2:
					Vector2Config();
					break;
				case Def.ConfigType.VECTOR3:
					Vector3Config();
					break;
				case Def.ConfigType.ENUM:
					EnumConfig();
					break;
				default:
					Debug.LogWarning("Unhandled ConfigType " + m_Config.GetConfigType().ToString());
					break;
			}
		}
		public override void _Awake()
		{
			base._Awake();
			m_ValueLock = false;
			ValueToggle.onValueChanged.AddListener(OnToggleEdit);
			ValueIF.onEndEdit.AddListener(OnIFEdit);
			OnV3IF = new UnityEngine.Events.UnityAction<string>[3]
			{
				(string val) => OnVectorEdit(0, val),
				(string val) => OnVectorEdit(1, val),
				(string val) => OnVectorEdit(2, val)
			};
			for (int i = 0; i < ValueV3IF.Length; ++i)
			{
				var vif = ValueV3IF[i];
				vif.onEndEdit.AddListener(OnV3IF[i]);
			}
			ValueDropdown.onValueChanged.AddListener(OnDropdownEdit);
		}
		void OnToggleEdit(bool val)
		{
			if (m_ValueLock)
				return;
			(m_Config as ConfigBoolean).SetValue(val);
		}
		void OnIFEdit(string val)
		{
			if (m_ValueLock)
				return;

			switch (m_Config.GetConfigType())
			{
				case Def.ConfigType.STRING:
				case Def.ConfigType.INTEGER:
				case Def.ConfigType.FLOAT:
					if(!m_Config.FromString(val))
					{
						m_ValueLock = true;
						ValueIF.text = m_Config.GetValueString();
						m_ValueLock = false;
					}
					break;
				default:
					Debug.LogWarning("Editing a Config using the ValueIF but config hasn't a valid type " + m_Config.GetConfigType().ToString());
					break;
			}
		}
		void OnVectorEdit(int idx, string val)
		{
			if (m_ValueLock)
				return;

			if(!float.TryParse(val, out float fVal))
			{
				m_ValueLock = true;
				switch (m_Config.GetConfigType())
				{
					case Def.ConfigType.VECTOR2:
						{
							var v2 = m_Config as ConfigVector2;
							ValueV3IF[idx].text = v2.GetValue()[idx].ToString();
						}
						break;
					case Def.ConfigType.VECTOR3:
						{
							var v3 = m_Config as ConfigVector3;
							ValueV3IF[idx].text = v3.GetValue()[idx].ToString();
						}
						break;
					default:
						Debug.LogWarning("Editing a Config using the ValueV3IF but config hasn't a valid type " + m_Config.GetConfigType().ToString());
						break;
				}
				m_ValueLock = false;
				return;
			}

			switch (m_Config.GetConfigType())
			{
				case Def.ConfigType.VECTOR2:
					{
						var v2 = m_Config as ConfigVector2;
						if (idx == 0)
							v2.SetValue(new Vector2(fVal, v2.GetValue().y));
						else if (idx == 1)
							v2.SetValue(new Vector2(v2.GetValue().x, fVal));
					}
					break;
				case Def.ConfigType.VECTOR3:
					{
						var v3 = m_Config as ConfigVector3;
						if (idx == 0)
							v3.SetValue(new Vector3(fVal, v3.GetValue().y, v3.GetValue().z));
						else if (idx == 1)
							v3.SetValue(new Vector3(v3.GetValue().x, fVal, v3.GetValue().z));
						else if(idx == 2)
							v3.SetValue(new Vector3(v3.GetValue().x, v3.GetValue().y, fVal));
					}
					break;
				default:
					Debug.LogWarning("Editing a Config using the ValueV3IF but config hasn't a valid type " + m_Config.GetConfigType().ToString());
					break;
			}
		}
		void OnDropdownEdit(int val)
		{
			if (m_ValueLock)
				return;

			var option = ValueDropdown.options[val];
			m_Config.FromString(option.text);
		}
	}
	public class ViewElementBConfigInfo : ViewElementInfoB
	{
		public IConfig Config;
	}
}
