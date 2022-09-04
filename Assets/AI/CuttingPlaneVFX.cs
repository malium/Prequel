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

namespace Assets
{
	class CuttingPlaneVFX : MonoBehaviour
	{
		const float AnimLength = 1.458f;
		static readonly int AnimHash = Shader.PropertyToID("Layer.Cutting");

		[SerializeField]
		MeshRenderer m_TopPart = null;
		[SerializeField]
		MeshRenderer m_BotPart = null;
		[SerializeField]
		Animator m_Animator = null;

		Vector3 m_Position;
		float m_SpriteYOffset;

		public void Set(MonsterFamily info, int frameIdx, Vector3 pos, float spriteYOffset)
		{
			m_SpriteYOffset = spriteYOffset;
			var sprite = info.Frames[frameIdx];
			m_TopPart.material.SetTexture(Def.MaterialTextureID, sprite.texture);
			m_BotPart.material.SetTexture(Def.MaterialTextureID, sprite.texture);

			var pivot = sprite.pivot / sprite.pixelsPerUnit;

			m_Position = pos + new Vector3(pivot.x - 0.5f, 0.52f + pivot.y * 1.3f, 0.0f);
			transform.localScale = new Vector3(1.3f * info.SpriteScale, 1.3f * info.SpriteScale, 1f);
			m_Animator.Play("Cutting");
			//m_Animator.Play("Layer.Cutting");
			//m_Animator.Play("Cutting", 0);
			//m_Animator.Play("Layer.Cutting", 0);
		}

		private void Update()
		{
			var color = m_TopPart.material.GetColor(Def.MaterialColorID);
			color.a -= color.a * Time.deltaTime * 2f;
			if(GameUtils.IsNearlyEqual(color.a, 0f))
			{
				GameUtils.DeleteGameobject(gameObject, false);
				return;
			}
			m_TopPart.material.SetColor(Def.MaterialColorID, color);
			m_BotPart.material.SetColor(Def.MaterialColorID, color);
		}

		private void LateUpdate()
		{
			transform.position = m_Position;
			var camPos = CameraManager.Mgr.transform.position;
			transform.LookAt(new Vector3(camPos.x, transform.position.y + 0.98f, camPos.z));
			transform.Rotate(0f, 180f, 0f);
			transform.position += new Vector3(0f, m_SpriteYOffset, 0f);
		}
	}
}
