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
	public class ParticleInfo
	{
		public Texture2D Texture;
		public int Version;
		public ParticleFamily Family;
	}
	public class ParticleFamily
	{
		public string FamilyName;
		public List<ParticleInfo> Particles;
	}
	public static class Particles
	{
		public static List<ParticleFamily> ParticleFamilies;
		public static Dictionary<string, int> FamilyDict;
		public static AI.Spells.CSprayParticles DefaultParticle;

		public static ParticleFamily GetParticleFamily(string familyName)
		{
			if (FamilyDict == null || ParticleFamilies == null)
				throw new Exception("Particles is not ready, trying to get a ParticleFamily before loading?");
			if (FamilyDict.ContainsKey(familyName))
			{
				return ParticleFamilies[FamilyDict[familyName]];
			}
			if (ParticleFamilies.Count == 0)
				throw new Exception("There are no Particles families, something went really wrong.");

			return ParticleFamilies[0];
		}
		public static void Prepare()
		{
			var textures = AssetLoader.Particles;
			ParticleFamilies = new List<ParticleFamily>(textures.Length);
			FamilyDict = new Dictionary<string, int>(textures.Length);

			bool parseOK = false;

			for(int i = 0; i < textures.Length; ++i)
			{
				var texture = textures[i];
				var split = texture.name.Split('_');
				var familyName = split[0].Trim();
				var versionStr = split[1].Trim();
				parseOK = int.TryParse(versionStr, out int version);
				if(!parseOK)
				{
					Debug.LogWarning("Error while parsing version '" + versionStr + "' from " + texture.name);
					continue;
				}
				ParticleFamily family = null;
				if(FamilyDict.ContainsKey(familyName))
				{
					family = ParticleFamilies[FamilyDict[familyName]];
				}
				else
				{
					family = new ParticleFamily()
					{
						FamilyName = familyName,
						Particles = new List<ParticleInfo>(1)
					};
					FamilyDict.Add(familyName, ParticleFamilies.Count);
					ParticleFamilies.Add(family);
				}

				if(family.Particles.Count < (version + 1))
				{
					family.Particles.AddRange(Enumerable.Repeat<ParticleInfo>(null, (version + 1) - family.Particles.Count));
				}
				family.Particles[version] = new ParticleInfo()
				{
					Family = family,
					Texture = texture,
					Version = version
				};
			}

			if(AssetLoader.ParticleEffect != null)
			{
				DefaultParticle =AssetLoader.ParticleEffect.GetComponent<AI.Spells.CSprayParticles>();
				DefaultParticle.enabled = false;
				DefaultParticle.StopEffect();
				DefaultParticle.gameObject.SetActive(false);
			}
			if(DefaultParticle == null)
			{
				Debug.LogError("Something went wrong instantiating ParticleEffect");
			}
		}
	}
}
