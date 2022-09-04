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
	public class SpriteTestUI : MonoBehaviour
	{
		public MainMenuUI MainMenu;

		public UnityEngine.UI.Button[] SpriteButtons;
		public UnityEngine.UI.Text[] SpriteTexts;
		SpriteBackendType m_CurrentType;

		GameObject[] m_Objects;
		Sprite m_TestSprite;
		int m_StopFrame = int.MinValue;

		float[] m_FrameTimes;

		private void Awake()
		{
			SpriteButtons[0].onClick.AddListener(SpriteBttn);
			SpriteButtons[1].onClick.AddListener(MixBttn);
			SpriteButtons[2].onClick.AddListener(SquadBttn);
			SpriteButtons[3].onClick.AddListener(SquadDSBttn);
			SpriteButtons[4].onClick.AddListener(DquadBttn);
		}

		public void Init()
		{
			const float side = (Def.MaxStrucSide * (1f + Def.BlockSeparation)) * 0.5f;
			CameraManager.Mgr.Target.transform.position = new Vector3(side, 0f, side);
			CameraManager.Mgr.CameraType = ECameraType.FREE;
			var camPos = new Vector3(48f, 53f, 15f);
			CameraManager.Mgr.transform.position = camPos;
			CameraManager.Mgr.transform.LookAt(CameraManager.Mgr.Target.transform);
			m_Objects = new GameObject[Def.MaxStrucSide * Def.MaxStrucSide];
			for (int y = 0; y < Def.MaxStrucSide; ++y)
			{
				int yOffset = y * Def.MaxStrucSide;
				for (int x = 0; x < Def.MaxStrucSide; ++x)
				{
					var obj = new GameObject($"Sprite_({x},{y})");
					obj.transform.position =
						new Vector3(x * (1f + Def.BlockSeparation), 0f, y * (1f + Def.BlockSeparation));
					obj.transform.LookAt(camPos);
					m_Objects[x + yOffset] = obj;
				}
			}
			m_TestSprite = Monsters.MonsterFamilies[Monsters.FamilyDict["Succubus"]].Frames[0];
			m_FrameTimes = new float[500];
		}

		void OnButtonPress()
		{
			for (int i = 0; i < SpriteButtons.Length; ++i)
			{
				SpriteButtons[i].interactable = false;
			}
			var componentType = SpriteUtils.GetSpriteBackend(m_CurrentType);
			for (int i = 0; i < m_Objects.Length; ++i)
			{
				var sprite = (ISpriteBackend)m_Objects[i].AddComponent(componentType);
				sprite.SetSprite(m_TestSprite);
			}
			m_StopFrame = Time.frameCount + m_FrameTimes.Length + 1;
		}

		public void SpriteBttn()
		{
			m_CurrentType = SpriteBackendType.SPRITE;
			OnButtonPress();
		}
		public void MixBttn()
		{
			m_CurrentType = SpriteBackendType.MIX;
			OnButtonPress();
		}
		public void SquadBttn()
		{
			m_CurrentType = SpriteBackendType.SQUAD;
			OnButtonPress();
		}
		public void SquadDSBttn()
		{
			m_CurrentType = SpriteBackendType.SQUAD_DS;
			OnButtonPress();
		}
		public void DquadBttn()
		{
			m_CurrentType = SpriteBackendType.DQUAD;
			OnButtonPress();
		}

		void Update()
		{
			if (m_StopFrame > Time.frameCount)
			{
				m_FrameTimes[Time.frameCount % m_FrameTimes.Length] = Time.smoothDeltaTime;
			}
			else if (m_StopFrame > 0 && m_StopFrame >= Time.frameCount)
			{
				m_StopFrame = int.MinValue;
				for (int i = 0; i < SpriteButtons.Length; ++i)
				{
					SpriteButtons[i].interactable = true;
				}
				var cmpType = SpriteUtils.GetSpriteBackend(m_CurrentType);
				for (int i = 0; i < m_Objects.Length; ++i)
					Component.Destroy(m_Objects[i].GetComponent(cmpType));
				SpriteTexts[(int)m_CurrentType].text = (1f / m_FrameTimes.Average()).ToString();
			}
		}
	}
}
