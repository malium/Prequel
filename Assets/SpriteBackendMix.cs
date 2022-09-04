/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using UnityEngine;

namespace Assets
{
	public class SpriteBackendMix : MonoBehaviour, ISpriteBackend
	{
		SpriteRenderer m_Renderer;
		Action<Collider> m_OnTriggerEnter;

		public void Flip(bool horizontal, bool vertical)
		{
			m_Renderer.flipX = horizontal;
			m_Renderer.flipY = vertical;
			m_Renderer.material.SetVector(Def.FlipShaderID, new Vector4(horizontal ? -1f : 1f, vertical ? -1f : 1f, 1f, 1f));
		}
		public SpriteBackendType GetBackendType()
		{
			return SpriteBackendType.MIX;
		}
		public Color GetColor()
		{
			return m_Renderer.color;
		}
		public Renderer GetRenderer()
		{
			return m_Renderer;
		}
		public Sprite GetSprite()
		{
			return m_Renderer.sprite;
		}
		public bool IsHorizontalFlip()
		{
			return m_Renderer.flipX;
		}
		public bool IsVerticalFlip()
		{
			return m_Renderer.flipY;
		}
		public void SetColor(Color color)
		{
			m_Renderer.color = color;
			m_Renderer.material.color = color;
		}
		public void SetSprite(Sprite sprite)
		{
			if (m_Renderer)
				Renderer.Destroy(m_Renderer);
			m_Renderer = gameObject.AddComponent<SpriteRenderer>();
			m_Renderer.sprite = sprite;
			m_Renderer.drawMode = SpriteDrawMode.Simple;
			m_Renderer.material = new Material(Materials.GetMaterial(Def.Materials.SpriteLitDS))
			{
				mainTexture = sprite.texture,
				color = m_Renderer.color
			};
			m_Renderer.allowOcclusionWhenDynamic = true;
			m_Renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
			m_Renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			m_Renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
			m_Renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
			m_Renderer.enabled = true;
		}
		private void Awake()
		{
			SetOnTriggerEnter(null);
		}
		private void OnEnable()
		{
			if (m_Renderer != null)
				m_Renderer.enabled = true;
		}
		private void OnDisable()
		{
			if (m_Renderer != null)
				m_Renderer.enabled = false;
		}
		private void OnDestroy()
		{
			Material.Destroy(m_Renderer.material);
			Renderer.Destroy(m_Renderer);
		}
		public void ChangeSprite(Sprite sprite)
		{
			m_Renderer.material.mainTexture = sprite.texture;
			m_Renderer.sprite = sprite;
		}
		public void SetEnabled(bool enable)
		{
			enabled = enable;
		}
		public void SetOnTriggerEnter(Action<Collider> onTriggerEnter)
		{
			m_OnTriggerEnter = onTriggerEnter;
			if (m_OnTriggerEnter == null)
				m_OnTriggerEnter = (Collider) => { };
		}
	}
}
