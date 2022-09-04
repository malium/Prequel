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
using UnityEngine.VFX;

namespace Assets
{
	public class FogComponent : MonoBehaviour
	{
		[SerializeField]
		FogManager m_FogMgr;
		[SerializeField]
		public Vector2 Direction;
		[SerializeField]
		FogPlacement Placement;
		[SerializeField]
		VisualEffect VisualEffect;

		private void OnEnable()
		{
			if (m_FogMgr == null)
				return;
			VisualEffect.enabled = true;
		}

		private void OnDisable()
		{
			if (m_FogMgr == null)
				return;
			VisualEffect.enabled = false;
		}

		static readonly Vector2[] FogDir = new Vector2[(int)FogPlacement.COUNT]
		{
			new Vector2(-1, -1).normalized,
			new Vector2(-1, 0),
			new Vector2(-1, 1).normalized,
			new Vector2(0, -1),
			new Vector2(0, 1),
			new Vector2(1, -1).normalized,
			new Vector2(1, 0),
			new Vector2(1, 1).normalized
		};

		public void SetFog(FogManager manager, FogPlacement placement)
		{
			m_FogMgr = manager;
			Placement = placement;
			Direction = FogDir[(int)placement];
			var vfxIdx = ParticleVFXs.Dict["Fog"];
			var vfx = ParticleVFXs.VisualEffects[vfxIdx];
			VisualEffect = gameObject.AddComponent<VisualEffect>();
			VisualEffect.visualEffectAsset = vfx;
			//var vfx = AssetContainer.Mgr.VisualEffects;
			//for (int i = 0; i < vfx.Length; ++i)
			//{
			//	if(vfx[i].name.ToLower() == "fog")
			//	{
			//		VisualEffect = gameObject.AddComponent<VisualEffect>();
			//		VisualEffect.visualEffectAsset = vfx[i];
			//		break;
			//	}
			//}
		}

		public void Update()
		{
			VisualEffect.enabled = m_FogMgr.Radius < m_FogMgr.CutRadius;
			//var radius = m_FogMgr.Radius;
			//var posOffset = Direction * radius;
			//var pos = m_FogMgr.transform.position + new Vector3(posOffset.x, 0.0f, posOffset.y);
			//var offset = pos - transform.position;
			//transform.Translate(offset, Space.Self);
		}
	}
}
