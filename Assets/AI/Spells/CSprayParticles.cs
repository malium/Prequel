/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.AI.Spells
{
	public class ConfigSprayParticles
	{
		public ConfigVector3 SpawnCenter;
		public ConfigVector3 SpawnSize;
		public ConfigParticleTexture Texture;
		public ConfigInteger ParticleVersion;
		public ConfigInteger ImageRowCount;
		public ConfigVector2 SizeCurve;
		public ConfigVector2 LifeTime;
		public ConfigVector2 Speed;
		public ConfigInteger SpawnRate;

		public void AddToConfig(List<IConfig> config)
		{
			config.AddRange(new IConfig[]
				{
					SpawnCenter,
					SpawnSize,
					Texture,
					ParticleVersion,
					ImageRowCount,
					SizeCurve,
					LifeTime,
					Speed,
					SpawnRate
				});
		}
	}
	public class CSprayParticles : MonoBehaviour
	{
		static int SpawnCenterID = Shader.PropertyToID("SpawnCenter");
		static int SpawnSizeID = Shader.PropertyToID("SpawnSize");
		static int ParticleTextureID = Shader.PropertyToID("ParticleTexture");
		static int ImageRowCountID = Shader.PropertyToID("ImageRowCount");
		static int SizeID = Shader.PropertyToID("Size");
		static int LifeTimeMinID = Shader.PropertyToID("LifeTimeMin");
		static int LifeTimeMaxID = Shader.PropertyToID("LifeTimeMax");
		static int SpeedMinID = Shader.PropertyToID("SpeedMin");
		static int SpeedMaxID = Shader.PropertyToID("SpeedMax");
		static int SpawnRateID = Shader.PropertyToID("SpawnRate");

		public UnityEngine.VFX.VisualEffect Effect;

		public void Set(ConfigSprayParticles def)
		{
			Effect.SetVector3(SpawnCenterID, def.SpawnCenter.GetValue());
			Effect.SetVector3(SpawnSizeID, def.SpawnSize.GetValue());
			var textureName = def.Texture.GetValue();
			var particleVersion = def.ParticleVersion.GetValue();
			var particleFamily = Particles.ParticleFamilies[Particles.FamilyDict[textureName]];
			var particle = particleFamily.Particles[particleVersion];
			Effect.SetTexture(ParticleTextureID, particle.Texture);
			Effect.SetFloat(ImageRowCountID, def.ImageRowCount.GetValue());
			var curve = def.SizeCurve.GetValue();
			Effect.SetAnimationCurve(SizeID, new AnimationCurve(
				new Keyframe[]
				{
					new Keyframe(0f, curve.x),
					new Keyframe(1f, curve.y)
				}));
			var lifeTime = def.LifeTime.GetValue();
			Effect.SetFloat(LifeTimeMinID, lifeTime.x);
			Effect.SetFloat(LifeTimeMaxID, lifeTime.y);
			var speed = def.Speed.GetValue();
			Effect.SetFloat(SpeedMinID, speed.x);
			Effect.SetFloat(SpeedMaxID, speed.y);
			Effect.SetFloat(SpawnRateID, def.SpawnRate.GetValue());
            //var mult = new Vector3(1f / transform.lossyScale.x, 1f / transform.lossyScale.y, 1f / transform.lossyScale.z);
            //transform.localScale = new Vector3(transform.localScale.x * mult.x, transform.localScale.y * mult.y, transform.localScale.z * mult.z);
		}
		public void StopEffect()
		{
			Effect.Stop();
		}
		public void StartEffect()
		{
			Effect.Play();
		}
		private void Update()
		{
			Effect.pause = Manager.Mgr.IsPaused;
		}
	}
}