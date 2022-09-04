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
	public static class ParticleVFXs
	{
		public static VisualEffectAsset[] VisualEffects;
		public static Dictionary<string, int> Dict;

		public static VisualEffectAsset GetParticleVFX(string vfxName)
		{
			if (Dict == null || VisualEffects == null)
				throw new Exception("ParticleVFXs is not ready, trying to get a VisualEffectAsset before loading?");
			if (Dict.ContainsKey(vfxName))
			{
				return VisualEffects[Dict[vfxName]];
			}
			return null;
		}
		public static void Prepare()
		{
			VisualEffects = AssetLoader.VisualEffects;
			Dict = new Dictionary<string, int>(VisualEffects.Length);

			for(int i = 0; i < VisualEffects.Length; ++i)
			{
				Dict.Add(VisualEffects[i].name, i);
			}
		}
	}
}
