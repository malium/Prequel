/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using UnityEngine;

namespace Assets
{
	public class SpriteBackendSprite : MonoBehaviour, ISpriteBackend
	{
		[SerializeField] SpriteRenderer m_Renderer;
		Action<Collider> m_OnTriggerEnter;

		public void Flip(bool horizontal, bool vertical)
		{
			m_Renderer.flipX = horizontal;
			m_Renderer.flipY = vertical;
		}
		public SpriteBackendType GetBackendType()
		{
			return SpriteBackendType.SPRITE;
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
		}
		public void SetSprite(Sprite sprite)
		{
			if (m_Renderer)
				Renderer.Destroy(m_Renderer);
			m_Renderer = gameObject.AddComponent<SpriteRenderer>();
			m_Renderer.sprite = sprite;
			m_Renderer.drawMode = SpriteDrawMode.Simple;
			m_Renderer.material = Materials.GetMaterial(Def.Materials.Sprite);
			m_Renderer.allowOcclusionWhenDynamic = true;
			m_Renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
			m_Renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			m_Renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
			m_Renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
			m_Renderer.receiveShadows = false;
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
			Renderer.Destroy(m_Renderer);
		}
		private void OnTriggerEnter(Collider other)
		{
			m_OnTriggerEnter(other);
		}
		public void ChangeSprite(Sprite sprite)
		{
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
