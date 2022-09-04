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

namespace Assets.AI.ODD
{
	public class CMeleeWeapon : IOddWeapon
	{
		const int VFXCount = 2;
		HashSet<CLivingEntity> m_HitEntities;
		Timer m_ColliderActivationTimer;
		Timer m_VFXActivationTimer;
		//float m_ActivationDelay;
		bool m_LookAtCamera;
		Quaternion m_TargetRotation;
		int m_VfxIdx;
		UnityEngine.VFX.VisualEffect[] m_AttackVFXs;
		Quaternion[] m_VFXBaseRotation;
		Def.OddAttackType m_AttackType;
		[SerializeField] TrailRenderer m_TrailRenderer;
		[SerializeField] Vector3[] m_VFXArmRotation = new Vector3[Def.OddAttackTypeCount];
		//Timer m_DeactivationTimer;

		public TrailRenderer GetTrailRenderer() => m_TrailRenderer;
		public bool IsLookingAtCamera() => m_LookAtCamera;
		public void LookAtCamera(bool look) => m_LookAtCamera = look;
		public CMeleeWeapon()
		{
			m_HitEntities = new HashSet<CLivingEntity>();
			m_ColliderActivationTimer = new Timer(true, false);
			m_ColliderActivationTimer.OnTimerTrigger += OnColliderActivation;
			m_VFXActivationTimer = new Timer(true, false);
			m_VFXActivationTimer.OnTimerTrigger += OnVFXActivation;
			//m_ActivationDelay = 0f;
			m_LookAtCamera = true;
			m_AttackType = Def.OddAttackType.COUNT;
			m_VFXBaseRotation = new Quaternion[VFXCount];
		}
		void OnColliderActivation()
		{
			//m_Collider.enabled = true;
			//m_Odd.OnStartAttack(m_AttackType);
			m_Collider.OnColliderActivation(m_AttackType);

			if(Manager.Mgr.DebugOddAttack)
				m_Renderer.color = Color.green;
		}
		void OnColliderDeactivation()
		{
			//m_Collider.enabled = false;
			//m_Odd.OnEndAttack(m_AttackType);
			m_Collider.OnColliderDeactivation();

			//m_LookAtCamera = true;
			if (Manager.Mgr.DebugOddAttack)
				m_Renderer.color = Color.red;
		}
		void OnVFXActivation()
		{
			var vfx = m_AttackVFXs[m_VfxIdx];
			vfx.gameObject.SetActive(true);
			vfx.Play();
			var offset = m_Info.PosOffsetOnAttack[(int)m_AttackType];
			var scale = m_Info.ScaleOnAttack[(int)m_AttackType];
			var rotation = m_Info.RotationOnAttack[(int)m_AttackType];
			vfx.transform.localPosition = new Vector3(offset.x, offset.y, m_Odd.Odd.MeshGO.transform.localPosition.z + offset.z);
			vfx.transform.localScale = scale;
			vfx.transform.localRotation = Quaternion.identity;
			vfx.transform.SetParent(null);
			m_VFXBaseRotation[m_VfxIdx] = vfx.transform.localRotation;
			vfx.transform.localRotation *= Quaternion.Euler(rotation);
		}
		public override void Init(COddController odd, OddWeaponInfo info)
		{
			base.Init(odd, info);
			if (m_AttackVFXs == null)
			{
				m_AttackVFXs = new UnityEngine.VFX.VisualEffect[VFXCount];
				for (int i = 0; i < m_AttackVFXs.Length; ++i)
				{
					var vfx = new GameObject("MeleeVFX" + i.ToString()).AddComponent<UnityEngine.VFX.VisualEffect>();
					//m_AttackVFX.visualEffectAsset = m_Info.VFX;
					vfx.transform.SetParent(m_Odd.transform);
					vfx.transform.localPosition = Vector3.zero;
					vfx.transform.localRotation = Quaternion.identity;
					vfx.transform.localScale = Vector3.one;
					vfx.gameObject.layer = Def.RCLayerProjectile;
					vfx.gameObject.SetActive(false);
					m_AttackVFXs[i] = vfx;
				}
			}
			if (m_Collider != null)
				m_Collider.Init(this);
		}
		public override void OnAttackBegin(Def.OddAttackType attackType)
		{
			base.OnAttackBegin(attackType);
			m_AttackType = attackType;
			m_VfxIdx = GameUtils.Mod(++m_VfxIdx, m_AttackVFXs.Length);
			var vfx = m_AttackVFXs[m_VfxIdx];
			vfx.transform.SetParent(m_Odd.transform);
			vfx.gameObject.SetActive(false);
			vfx.visualEffectAsset = m_Info.VFXs[(int)attackType];

			var colliderDelay = m_Info.ColliderActivationDelay[(int)attackType];
			if (colliderDelay > 0f)
				m_ColliderActivationTimer.Reset(colliderDelay);
			else
				OnColliderActivation();
			var vfxDelay = m_Info.VFXActivationDelay[(int)attackType];
			if (vfxDelay > 0f)
				m_VFXActivationTimer.Reset(vfxDelay);
			else
				OnVFXActivation();
			//m_LookAtCamera = false;
			m_HitEntities.Clear();
		}
		public override void OnAttackEnd()
		{
			base.OnAttackEnd();

			OnColliderDeactivation();
		}
		public void OnArmsRotation(float angleToTarget, float chestHandAngle)
		{
			m_Collider?.OnArmsRotation(angleToTarget, chestHandAngle);

			var vfx = m_AttackVFXs[m_VfxIdx];
			var baseRot = m_VFXBaseRotation[m_VfxIdx];
			vfx.transform.localRotation = baseRot;
			vfx.transform.Rotate(m_Info.RotationOnAttack[(int)m_AttackType], Space.Self);
			vfx.transform.Rotate(m_VFXArmRotation[(int)m_AttackType] * angleToTarget, Space.Self);
		}
		public void OnColliderEnter(Collider self, Collider other)
		{
			//Debug.Log($"{self.name} collided with {other.name}");
			if (other.gameObject.layer != Def.RCLayerLE || other.gameObject == m_Odd.gameObject)
				return;

			if (other.gameObject.TryGetComponent(out CLivingEntity le))
			{
				if (m_HitEntities.Add(le))
				{
					var hitPos = new Vector3(le.transform.position.x, transform.position.y, le.transform.position.z);
					var hitDir = le.transform.position - m_Odd.transform.position;
					hitDir.y = 0f;
					le.TriggerDamageFX(hitPos, hitDir.normalized);

					if (!Def.IsOddHeavyAttack(m_Odd.GetCurrentAttack()))
						le.ReceiveDamage(m_Odd.LE, m_Info.DamageType, m_Info.Damage);
					else
						le.ReceiveDamage(m_Odd.LE, m_Info.HeavyDamageType, m_Info.HeavyDamage);
				}
			}
		}
		private void Update()
		{
			m_ColliderActivationTimer.Update();
			m_VFXActivationTimer.Update();
		}
		private void LateUpdate()
		{
			Quaternion current = transform.localRotation;
			if (m_LookAtCamera)
			{
				var cam = CameraManager.Mgr.Camera;
				Vector3 v = cam.transform.position - transform.position;
				v.x = v.z = 0.0f;
				var dir = cam.transform.position - v;
				transform.LookAt(dir);
				m_TargetRotation = transform.localRotation;
			}
			else
			{
				m_TargetRotation = Quaternion.Euler(0f, 0f, 0f);
			}
			transform.localRotation = Quaternion.Slerp(current, m_TargetRotation, 6f * Time.deltaTime);
		}
		private void OnDestroy()
		{
			if (m_AttackVFXs != null)
			{
				for (int i = 0; i < m_AttackVFXs.Length; ++i)
				{
					var vfx = m_AttackVFXs[i];
					if (vfx != null)
						GameUtils.DeleteGameobject(vfx.gameObject);
				}
				m_AttackVFXs = null;
			}
				
		}
	}
}
