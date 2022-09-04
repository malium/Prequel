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
	public class PossessingState : IOddState
	{
		static readonly int AnimationHash = Animator.StringToHash("BaseLayer.PossessIn");
		public override string StateName => "Possessing";
		public override OddState State => OddState.Possessing;
		const float AnimationLength = 1f;
		const float AnimationSpeed = 1f;
		const float AnimationTime = AnimationLength / AnimationSpeed;


		const float AnimationFrameCount = 24f;
		const float VFXStartFrame = 18f;
		const float VFXStartRemainingPCT = VFXStartFrame / AnimationFrameCount;

		const float VFXTime = AnimationTime * VFXStartRemainingPCT + 1.15f;

		const float OddMaxSpeed = 4f;
		const float OddImpulse = 10f;
		static Collider[] gCollidingEntities;

		Timer m_AnimationTimer;
		Timer m_VFXTimer;
		//UnityEngine.Rendering.Universal.MotionBlur m_Blur;
		Vector2 m_DashDir;

		public PossessingState()
		{
			m_AnimationTimer = new Timer(true, false);
			m_VFXTimer = new Timer(true, false);
			m_VFXTimer.OnTimerTrigger += OnAnimationEnd;
			//m_AnimationTimer.OnTimerTrigger += OnAnimationEnd;
			if (gCollidingEntities == null)
				gCollidingEntities = new Collider[8];
		}

		bool Possess()
		{
			var amount = Physics.OverlapSphereNonAlloc(Controller.transform.position, 1f, gCollidingEntities, 1 << Def.RCLayerLE);
			CLivingEntity toPossess = null;
			float minDistance = float.MaxValue;
			for (int i = 0; i < amount; ++i)
			{
				var le = gCollidingEntities[i].gameObject.GetComponent<CLivingEntity>();
				if (le.GetLEType() != Def.LivingEntityType.Monster)
					continue;

				var dist = Vector3.Distance(Controller.transform.position, le.transform.position);
				if (dist < minDistance)
				{
					minDistance = dist;
					toPossess = le;
				}
			}

			if (toPossess != null)
			{
				Controller.enabled = false;
				Controller.LE.GetCollider().enabled = false;
				Controller.ME.enabled = false;
				Controller.LE.enabled = false;
				Controller.Odd.enabled = false;
				Controller.Odd.GetMeshRenderer().enabled = false;
				Controller.Odd.GetFaceRenderer().enabled = false;
				Controller.Odd.GetOutline().enabled = false;
				Controller.GetWeapons()[0].gameObject.SetActive(false);
				if (toPossess.TryGetComponent(out CMonsterController mc))
				{
					mc.enabled = false;
				}

				var targetEntity = Controller.Odd.GetTargetEntity();
				if (targetEntity == toPossess)
					Controller.Odd.OnUnlockTarget();

				var moc = mc.gameObject.AddComponent<CMonsterOddController>();
				moc.Set(toPossess.GetComponent<CMonster>(), Controller);
				Controller.transform.SetParent(moc.transform);
				Controller.transform.localPosition = Vector3.zero;
				CameraManager.Mgr.Target = moc.gameObject;
				return true;
			}
			return false;
		}
		void OnAnimationEnd()
		{
			var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
			weapon.LookAtCamera(true);
			Controller.LE.GetCollider().gameObject.layer = Def.RCLayerLE;
			Controller.LE.GetCollider().enabled = true;
			Controller.ChangeState(OddState.Idle);
			Controller.StartCoroutine(Controller.Odd.OnPossessionOut());
		}
		public override void OnFixedUpdate()
		{
			base.OnFixedUpdate();
		}
		public override void OnLateUpdate()
		{
			base.OnLateUpdate();
		}
		public override void OnStart()
		{
			base.OnStart();

			//if (!AssetLoader.PospoVolume.TryGet(out m_Blur))
			//	m_Blur = null;

			//if (m_Blur != null)
			//{
			//	m_Blur.active = true;
			//	m_Blur.intensity.Interp(m_Blur.intensity.min, m_Blur.intensity.max, 0f);
			//}

			Controller.Odd.GetAnimator().CrossFadeInFixedTime(AnimationHash, 0.2f, 0);
			var targetEntity = Controller.Odd.GetTargetEntity();
			if (targetEntity == null)
			{
				var mgr = Manager.Mgr;
				if (mgr.GameInputControl == Def.GameInputControl.Mouse)
				{
					var cam = CameraManager.Mgr.Camera;
					if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, 1 << Def.RCLayerRayPlane))
					{
						m_DashDir = (new Vector2(hit.point.x, hit.point.z) - new Vector2(Controller.transform.position.x, Controller.transform.position.z)).normalized;
						Controller.ME.SetDirection(m_DashDir);
					}
				}
				else
				{
					m_DashDir = Controller.ME.GetDirection();
					Controller.ME.SetDirection(m_DashDir);
				}
			}
			else
			{
				var posXZ = new Vector2(Controller.transform.position.x, Controller.transform.position.z);
				var trgXZ = new Vector2(targetEntity.transform.position.x, targetEntity.transform.position.z);
				m_DashDir = (trgXZ - posXZ).normalized;
				Controller.ME.SetDirection(m_DashDir);
			}

			Controller.Odd.GetPossessionVFX().gameObject.SetActive(false);
			m_AnimationTimer.Reset(AnimationTime);
			m_VFXTimer.Reset(VFXTime);
			Controller.ME.SetMaxSpeed(OddMaxSpeed);
			Controller.ME.Impulse(m_DashDir, 0f);
			Controller.LE.GetCollider().gameObject.layer = Def.RCLayerAvoidLE;
			var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
			weapon.LookAtCamera(false);
		}
		public override void OnStop()
		{
			base.OnStop();
			//if (m_Blur != null)
			//	m_Blur.active = false;
		}
		public override void OnUpdate()
		{
			base.OnUpdate();

			var remainingTimePCT = m_AnimationTimer.GetRemainingPct();
			//if (remainingTimePCT >= 0.75f)
			//{
			//	float remainingPCT = (remainingTimePCT - 0.75f) * 4f;
			//	if (m_Blur != null)
			//		m_Blur.intensity.Interp(m_Blur.intensity.max, m_Blur.intensity.min, remainingPCT);
			//}

			if(!Controller.Odd.GetPossessionVFX().gameObject.activeInHierarchy && remainingTimePCT >= VFXStartRemainingPCT)
			{
				Controller.LE.GetCollider().enabled = false;
				var vfx = Controller.Odd.GetPossessionVFX();
				vfx.transform.SetParent(null);
				vfx.transform.localPosition = Controller.transform.position;
				vfx.transform.localRotation = Quaternion.identity;
				vfx.transform.localScale = Vector3.one;
				vfx.gameObject.SetActive(true);
				vfx.Play();
				if(Possess())
				{
					Controller.LE.GetCollider().gameObject.layer = Def.RCLayerLE;
				}
			}

			if(Controller.Odd.GetPossessionVFX().gameObject.activeInHierarchy)
			{
				m_AnimationTimer.Update();
				m_VFXTimer.Update();
				return;
			}
			var oldPos = Controller.transform.position;
			Controller.ME.Impulse(m_DashDir, OddImpulse);
			Controller.LE.UpdateStruc();

			var struc = Controller.LE.GetCurrentStruc();
			if (struc == null)
			{
				Controller.LE.UpdateStruc();
				struc = Controller.LE.GetCurrentStruc();
			}
			if (Controller.LE.GetCurrentBlock() == null && struc != null)
			{
				Controller.LE.UpdateBlock();
			}

			var movXZ = Controller.ME.UpdateMovement();
			var mov = new Vector3(movXZ.x, -1f, movXZ.y);
			mov *= Time.deltaTime;

			var cc = Controller.ME.GetController();
			var collision = cc.Move(mov);
			if (collision.HasFlag(CollisionFlags.CollidedSides))
				Controller.ME.OnCollision();

			var tempBlock = Controller.LE.ComputeCurrentBlock() as CBlock;
			if (Controller.LE.GetCurrentBlock() != null && tempBlock == null)
			{
				Controller.transform.position = oldPos;
				Controller.ME.OnCollision();
			}
			else
			{
				Controller.LE.SetCurrentBlock(tempBlock);
			}

			m_AnimationTimer.Update();
			m_VFXTimer.Update();
		}
		public override void OnGizmos()
		{
			base.OnGizmos();
		}
		public override void OnGUI()
		{
			base.OnGUI();
		}
	}
}
