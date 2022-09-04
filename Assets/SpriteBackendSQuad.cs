/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using UnityEngine;

namespace Assets
{
	public class SpriteBackendSQuad : MonoBehaviour, ISpriteBackend
	{
		[SerializeField] MeshRenderer m_Renderer;
		[SerializeField] MeshFilter m_Mesh;
		[SerializeField] Sprite m_Sprite;
		Action<Collider> m_OnTriggerEnter;
		[SerializeField] bool m_FlipX;
		[SerializeField] bool m_FlipY;

		static Mesh SQuadMesh;

		static void InitSQuad()
		{
			if (SQuadMesh != null)
				return;

			SQuadMesh = new Mesh
			{
				vertices = new Vector3[4]
				{
					new Vector3(0f, 0f, 0f),
					new Vector3(1f, 0f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(1f, 1f, 0f)
				},
				triangles = new int[6]
				{
					0, 1, 2,
					2, 1, 3
				},
				normals = new Vector3[4]
				{
					Vector3.forward,
					Vector3.forward,
					Vector3.forward,
					Vector3.forward,
				},
				uv = new Vector2[4]
				{
					new Vector2(0f, 0f),
					new Vector2(1f, 0f),
					new Vector2(0f, 1f),
					new Vector2(1f, 1f)
				}
			};
			SQuadMesh.Optimize();
			SQuadMesh.UploadMeshData(true);
		}
		public void Flip(bool horizontal, bool vertical)
		{
			m_FlipX = horizontal;
			m_FlipY = vertical;

			var texSize = m_Sprite.texture.width;
			float scale = texSize / m_Sprite.pixelsPerUnit;
			var pivot = m_Sprite.pivot / m_Sprite.pixelsPerUnit;
			if (m_FlipX)
				pivot.Set(scale - pivot.x, pivot.y);
			if (m_FlipY)
				pivot.Set(pivot.x, scale - pivot.y);

			var innerGO = m_Renderer.gameObject;
			innerGO.transform.localPosition = new Vector3(-pivot.x, -pivot.y, 0f);
			m_Renderer.material.SetVector(
				Def.FlipShaderID, 
				new Vector4(horizontal ? -1f : 1f, vertical ? -1f : 1f, 1f, 1f));
		}
		public SpriteBackendType GetBackendType()
		{
			return SpriteBackendType.MIX;
		}
		public Renderer GetRenderer()
		{
			return m_Renderer;
		}
		public Sprite GetSprite()
		{
			return m_Sprite;
		}
		public bool IsHorizontalFlip()
		{
			return m_FlipX;
		}
		public bool IsVerticalFlip()
		{
			return m_FlipY;
		}
		public void SetColor(Color color)
		{
			m_Renderer.material.color = color;
		}
		public Color GetColor()
		{
			return m_Renderer.material.color;
		}
		public void SetSprite(Sprite sprite)
		{
			if (m_Renderer != null)
			{
				Material.Destroy(m_Renderer.material);
				GameObject.Destroy(m_Renderer.gameObject);
			}

			InitSQuad();
			m_Sprite = sprite;

			var texSize = m_Sprite.texture.width;
			float scale = texSize / m_Sprite.pixelsPerUnit;
			var pivot = m_Sprite.pivot / m_Sprite.pixelsPerUnit;
			if (m_FlipX)
				pivot.Set(scale - pivot.x, pivot.y);
			if (m_FlipY)
				pivot.Set(pivot.x, scale - pivot.y);

			//var texSize = m_Sprite.texture.width;
			//float scale = texSize / m_Sprite.pixelsPerUnit;
			//var pivot = m_Sprite.pivot / m_Sprite.pixelsPerUnit;
			//if (m_FlipX)
			//	pivot += new Vector2(pivot.x - 0.5f, 0f);

			var innerGO = new GameObject("_InnerSprite");
			innerGO.transform.SetParent(gameObject.transform);
			innerGO.transform.localPosition = new Vector3(-pivot.x, -pivot.y, 0f);
			innerGO.transform.localScale = new Vector3(scale, scale, 1f);

			m_Renderer = innerGO.AddComponent<MeshRenderer>();
			m_Mesh = innerGO.AddComponent<MeshFilter>();
			m_Mesh.mesh = SQuadMesh;

			m_Renderer.material = new Material(Materials.GetMaterial(Def.Materials.SpriteLit))
			{
				mainTexture = m_Sprite.texture,
				color = Color.white
			};
			m_Renderer.allowOcclusionWhenDynamic = true;
			m_Renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
			m_Renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			m_Renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
			m_Renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
			m_Renderer.receiveShadows = true;
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
			GameObject.Destroy(m_Renderer.gameObject);
		}
		public void ChangeSprite(Sprite sprite)
		{
			m_Sprite = sprite;
			var texSize = m_Sprite.texture.width;
			float scale = texSize / m_Sprite.pixelsPerUnit;
			var pivot = m_Sprite.pivot / m_Sprite.pixelsPerUnit;
			if (m_FlipX)
				pivot.Set(scale - pivot.x, pivot.y);
			if (m_FlipY)
				pivot.Set(pivot.x, scale - pivot.y);
			var innerGO = m_Renderer.gameObject;
			innerGO.transform.localPosition = new Vector3(-pivot.x, -pivot.y, 0f);
			innerGO.transform.localScale = new Vector3(scale, scale, 1f);
			m_Renderer.material.mainTexture = m_Sprite.texture;
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
