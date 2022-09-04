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
	[Serializable]
	public class VFXDef
	{
		public Def.VFXTarget VfxTarget;
		public int VFXTypeID;
		public Def.VFXType VFXType;
		public int VFXVersion;
		public Def.VFXFacing Facing;
		public Def.VFXEnd OnEnd;
		public Vector2 VFXScale;
		public float FPSOverride;

		public VFXDef(Def.VFXTarget target, string typeName, Def.VFXType type, int vfxVersion,
			Def.VFXFacing facing = Def.VFXFacing.FaceCameraFull, Def.VFXEnd onEnd = Def.VFXEnd.SelfDestroy,
			float fpsOverride = 0)
		{
			VfxTarget = target;
			//Def.VFXTypeID = VFXs.VFXDict[(int)target][typeName];
			VFXTypeID = VFXs.FamilyDict[(int)target][typeName];
			VFXType = type;
			VFXVersion = vfxVersion;
			Facing = facing;
			OnEnd = onEnd;
			VFXScale = Vector2.one;
			FPSOverride = fpsOverride;
		}

		public VFXDef(Def.VFXTarget target, int VFXTypeID, Def.VFXType type, int vfxVersion,
			Def.VFXFacing facingCamera = Def.VFXFacing.FaceCameraFull, Def.VFXEnd onEnd = Def.VFXEnd.SelfDestroy,
			float fpsOverride = 0)
		{
			VfxTarget = target;
			this.VFXTypeID = VFXTypeID;
			VFXType = type;
			VFXVersion = vfxVersion;
			Facing = facingCamera;
			OnEnd = onEnd;
			VFXScale = Vector2.one;
			FPSOverride = fpsOverride;
		}

		public bool IsValid()
		{
			if (VfxTarget == Def.VFXTarget.COUNT)
				return false;

			if (VFXTypeID < 0)
				return false;

			if (VFXType == Def.VFXType.COUNT)
				return false;

			if (VFXVersion < 0)
				return false;

			return true;
		}
	}

	public class ConfigVFXDef
	{
		public ConfigEnum<Def.VFXTarget> VFXTarget;
		public ConfigString VFXTypeName;
		public ConfigEnum<Def.VFXType> VFXType;
		public ConfigInteger VFXVersion;
		public ConfigEnum<Def.VFXFacing> VFXFacing;
		public ConfigEnum<Def.VFXEnd> VFXEnd;
		public ConfigVector2 VFXScale;
		public ConfigFloat FPSOverride;

		public void AddToConfig(List<IConfig> config)
		{
			config.Add(VFXTarget);
			config.Add(VFXTypeName);
			config.Add(VFXType);
			config.Add(VFXVersion);
			config.Add(VFXFacing);
			config.Add(VFXEnd);
			config.Add(VFXScale);
			config.Add(FPSOverride);
		}

		public VFXDef GetValue()
		{
			int typeID = -1;
			if (VFXTarget.GetValue() != Def.VFXTarget.COUNT)
			{
				if (VFXs.FamilyDict[(int)VFXTarget.GetValue()].ContainsKey(VFXTypeName.GetValue()))
				{
					typeID = VFXs.FamilyDict[(int)VFXTarget.GetValue()][VFXTypeName.GetValue()];
				}
			}
			return new VFXDef(VFXTarget.GetValue(), typeID, VFXType.GetValue(), VFXVersion.GetValue(),
				VFXFacing.GetValue(), VFXEnd.GetValue(), FPSOverride.GetValue())
			{
				VFXScale = VFXScale.GetValue(),
			};
		}
	}
	public class CVFX : MonoBehaviour
	{
		static readonly Action<CVFX> DefaultOnEnd = (CVFX) => { };
		SpriteBackendSprite m_Sprite;
		FramedAnimation m_Animation;

		[SerializeField] VFXDef m_Def;
		[SerializeField] Sprite[] m_Frames;
		[SerializeField] int m_CurrentFrame;
		[SerializeField] bool m_HasStarted;
		Action<CVFX> m_OnEnd;

		public SpriteBackendSprite GetSprite() => m_Sprite;
		public FramedAnimation GetAnimation() => m_Animation;
		public VFXDef GetVFXDef() => m_Def;
		void OnAnimation()
		{
			if (m_CurrentFrame >= m_Frames.Length)
			{
				switch (m_Def.OnEnd)
				{
					case Def.VFXEnd.Stop:
						m_Animation.GetTimer().SetEnabled(false);
						m_OnEnd(this);
						enabled = false;
						return;
					case Def.VFXEnd.SelfDestroy:
						m_OnEnd(this);
						GameUtils.DeleteGameobject(gameObject);
						return;
					case Def.VFXEnd.Repeat:
						m_CurrentFrame = 0;
						break;
				}
			}
			m_Sprite.ChangeSprite(m_Frames[m_CurrentFrame++]);
		}
		public void Set(VFXDef def, Action<CVFX> onEnd = null, Action<Collider> onTriggerEnter = null)
		{
			m_Def = def;
			m_OnEnd = onEnd;

			if (m_OnEnd == null)
				m_OnEnd = DefaultOnEnd;

			m_CurrentFrame = 0;
			if(m_HasStarted)
			{
				Restart();
				return;
			}
			m_Sprite = GetComponent<SpriteBackendSprite>();
			if(m_Sprite == null)
			{
				m_Sprite = gameObject.AddComponent<SpriteBackendSprite>();
			}
			m_Sprite.SetOnTriggerEnter(onTriggerEnter);
			m_Animation = GetComponent<FramedAnimation>();
			if(m_Animation == null)
			{
				m_Animation = gameObject.AddComponent<FramedAnimation>();
			}
			var family = VFXs.VFXFamilies[(int)m_Def.VfxTarget][m_Def.VFXTypeID];
			var info = family.GetVFXInfo(m_Def.VFXType)[m_Def.VFXVersion];
			m_Frames = info.Frames;
			float fps = m_Def.FPSOverride > 0f ? m_Def.FPSOverride : info.FramesPerSec;
			m_Animation.Set(fps, OnAnimation);
			m_Animation.GetTimer().SetAutoReset(true);
			m_Sprite.SetSprite(m_Frames[m_CurrentFrame]);
			gameObject.transform.localScale = new Vector3(m_Def.VFXScale.x, m_Def.VFXScale.y, 1f);
			m_HasStarted = true;
		}
		public void Restart()
		{
			var family = VFXs.VFXFamilies[(int)m_Def.VfxTarget][m_Def.VFXTypeID];
			var info = family.GetVFXInfo(m_Def.VFXType)[m_Def.VFXVersion];
			float fps = m_Def.FPSOverride > 0f ? m_Def.FPSOverride : info.FramesPerSec;
			m_Animation.Restart();
			m_Sprite.ChangeSprite(m_Frames[m_CurrentFrame]);
			m_Animation.GetTimer().SetEnabled(true);
			enabled = true;
		}
		private void LateUpdate()
		{
			var cam = CameraManager.Mgr.Camera;
			Vector3 v;
			Vector3 dir;
			switch (m_Def.Facing)
			{
				case Def.VFXFacing.FaceCameraFull:
					transform.LookAt(cam.transform);
					break;
				case Def.VFXFacing.FaceCameraFreezeX:
					v = cam.transform.position - transform.position;
					v.x = 0f;
					dir = cam.transform.position - v;
					transform.LookAt(dir);
					break;
				case Def.VFXFacing.FaceCameraFreezeY:
					v = cam.transform.position - transform.position;
					v.y = 0f;
					dir = cam.transform.position - v;
					transform.LookAt(dir);
					break;
				case Def.VFXFacing.FaceCameraFreezeZ:
					v = cam.transform.position - transform.position;
					v.z = 0f;
					dir = cam.transform.position - v;
					transform.LookAt(dir);
					break;
				case Def.VFXFacing.FaceCameraFreezeXY:
					v = cam.transform.position - transform.position;
					v.x = 0f;
					v.y = 0f;
					dir = cam.transform.position - v;
					transform.LookAt(dir);
					break;
				case Def.VFXFacing.FaceCameraFreezeXZ:
					v = cam.transform.position - transform.position;
					v.x = 0f;
					v.z = 0f;
					dir = cam.transform.position - v;
					transform.LookAt(dir);
					break;
				case Def.VFXFacing.FaceCameraFreezeYZ:
					v = cam.transform.position - transform.position;
					v.y = 0f;
					v.z = 0f;
					dir = cam.transform.position - v;
					transform.LookAt(dir);
					break;
				case Def.VFXFacing.FaceXUp:
					dir = transform.position + new Vector3(100f, 0f, 0f);
					transform.LookAt(dir);
					break;
				case Def.VFXFacing.FaceXDown:
					dir = transform.position + new Vector3(-100f, 0f, 0f);
					transform.LookAt(dir);
					break;
				case Def.VFXFacing.FaceYUp:
					dir = transform.position + new Vector3(0f, 100f, 0f);
					transform.LookAt(dir);
					break;
				case Def.VFXFacing.FaceYDown:
					dir = transform.position + new Vector3(0f, -100f, 0f);
					transform.LookAt(dir);
					break;
				case Def.VFXFacing.FaceZUp:
					dir = transform.position + new Vector3(0f, 0f, 100f);
					transform.LookAt(dir);
					break;
				case Def.VFXFacing.FaceZDown:
					dir = transform.position + new Vector3(0f, 0f, -100f);
					transform.LookAt(dir);
					break;
			}
		}
		private void OnEnable()
		{
			if(m_Sprite != null)
				m_Sprite.SetEnabled(true);
		}
		private void OnDisable()
		{
			if (m_Sprite != null)
				m_Sprite.SetEnabled(false);
		}
	}
}
