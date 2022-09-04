/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using UnityEngine;

namespace Assets
{
	public class SpriteBackendDQuad : MonoBehaviour, ISpriteBackend
	{
		MeshRenderer m_Renderer;
		MeshFilter m_Mesh;
		Sprite m_Sprite;
		Action<Collider> m_OnTriggerEnter;
		bool m_FlipX;
		bool m_FlipY;

		static Mesh DQuadMesh;
		
		static void InitDQuad()
		{
			if (DQuadMesh != null)
				return;

			DQuadMesh = new Mesh
			{
				vertices = new Vector3[4 * 2]
				{
					// Front
					new Vector3(0f, 0f, 0f),
					new Vector3(1f, 0f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(1f, 1f, 0f),

					new Vector3(0f, 0f, 0f),
					new Vector3(1f, 0f, 0f),
					new Vector3(0f, 1f, 0f),
					new Vector3(1f, 1f, 0f),
				},
				triangles = new int[6 * 2]
				{
					// Front
					0, 2, 1,
					2, 3, 1,

					// Back
					4, 5, 7,
					4, 7, 6,
				},
				normals = new Vector3[4 * 2]
				{
					// Front
					-Vector3.forward,
					-Vector3.forward,
					-Vector3.forward,
					-Vector3.forward,

					Vector3.forward,
					Vector3.forward,
					Vector3.forward,
					Vector3.forward,
				},
				uv = new Vector2[4 * 2]
				{
					// Front
					new Vector2(0f, 0f),
					new Vector2(1f, 0f),
					new Vector2(0f, 1f),
					new Vector2(1f, 1f),

					new Vector2(0f, 0f),
					new Vector2(1f, 0f),
					new Vector2(0f, 1f),
					new Vector2(1f, 1f),
				}
			};
			//DQuadMesh.RecalculateNormals();
			DQuadMesh.Optimize();
			DQuadMesh.UploadMeshData(true);
		}
		public void Flip(bool horizontal, bool vertical)
		{
			m_FlipX = horizontal;
			m_FlipY = vertical;
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

			m_Sprite = sprite;
			InitDQuad();
			var texSize = sprite.texture.width;
			float scale = texSize / sprite.pixelsPerUnit;
			var pivot = sprite.pivot / sprite.pixelsPerUnit;

			var innerGO = new GameObject("_InnerSprite");
			innerGO.transform.SetParent(gameObject.transform);
			innerGO.transform.localPosition = new Vector3(-pivot.x, -pivot.y, 0f);
			innerGO.transform.localScale = new Vector3(scale, scale, 1f);

			m_Renderer = innerGO.AddComponent<MeshRenderer>();
			m_Mesh = innerGO.AddComponent<MeshFilter>();
			m_Mesh.mesh = DQuadMesh;

			m_Renderer.material = new Material(Materials.GetMaterial(Def.Materials.SpriteLit))
			{
				mainTexture = sprite.texture,
				color = Color.white,
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
			if(m_Renderer != null)
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
			var texSize = sprite.texture.width;
			float scale = texSize / sprite.pixelsPerUnit;
			var pivot = sprite.pivot / sprite.pixelsPerUnit;
			if (m_FlipX)
				pivot.Set(scale - pivot.x, pivot.y);
			if (m_FlipY)
				pivot.Set(pivot.x, scale - pivot.y);
			var innerGO = m_Renderer.gameObject;
			innerGO.transform.localPosition = new Vector3(-pivot.x, -pivot.y, 0f);
			innerGO.transform.localScale = new Vector3(scale, scale, 1f);
			m_Renderer.material.mainTexture = sprite.texture;
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
