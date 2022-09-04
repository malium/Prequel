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
	public class PropDamageParticle : MonoBehaviour
	{
		SpriteBackendSQuad m_Sprite;
		Vector3 m_Direction;
		public SpriteBackendSQuad GetSprite() => m_Sprite;
		public Vector3 GetDirection() => m_Direction;
		public void Set(Color32[] colors, Vector3 dir)
		{
			var tex = new Texture2D(2, 2);
			tex.SetPixels32(colors);
			tex.Apply(false, true);
			m_Sprite = SpriteUtils.AddSprite(
				gameObject,
				SpriteBackendType.SQUAD,
				Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), Vector2.zero, 100f, 0, SpriteMeshType.FullRect))
				as SpriteBackendSQuad;
			m_Sprite.Flip(
				UnityEngine.Random.value < 0.5f,
				UnityEngine.Random.value > 0.5f);
			var size = UnityEngine.Random.Range(4f, 8f);
			transform.localScale = new Vector3(size, size, 1f);
			m_Direction = dir;
		}
		
		private void Update()
		{
			var color = m_Sprite.GetColor();
			color.a -= Time.deltaTime;
			if(color.a <= 0f)
			{
				GameUtils.DeleteGameobject(gameObject);
				return;
			}
			m_Sprite.SetColor(color);
			transform.Translate(m_Direction * Time.deltaTime * 2f, Space.World);
		}

		private void LateUpdate()
		{
			transform.LookAt(CameraManager.Mgr.transform);
		}
	}
}
