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
	public class AttackInState : IOddState
	{
		static readonly int Animation1Hash = Animator.StringToHash("BaseLayer.HeavyAttack1_In");
		static readonly int Animation2Hash = Animator.StringToHash("BaseLayer.HeavyAttack2_In");
		public override string StateName => "AttackIn";
		const float AnimationLength = 0.625f;
		const float AnimationSpeed = 1f;
		public const float AnimationTime = AnimationLength / AnimationSpeed;
		public override OddState State => OddState.AttackIn;

		float m_Time;
		Timer m_AnimationTimer;
		OddState m_NextState;

		public AttackInState()
		{
			m_AnimationTimer = new Timer(AnimationTime, true, false);
			m_AnimationTimer.OnTimerTrigger += OnAnimationFinish;
		}
		public override void OnFixedUpdate()
		{
			base.OnFixedUpdate();
			if (Controller.Odd.GetTargetEntity() == null)
			{
				var cam = CameraManager.Mgr.Camera;
				if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, 1 << Def.RCLayerRayPlane))
				{
					Controller.ME.SetDirection(new Vector2(hit.point.x, hit.point.z) - new Vector2(Controller.transform.position.x, Controller.transform.position.z));
				}
			}
		}
		void OnAnimationFinish()
		{
			Controller.ChangeState(m_NextState);
			Controller.CurrentStateController.OnUpdate();
		}
		public override void OnLateUpdate()
		{
			base.OnLateUpdate();
		}
		public override void OnStart()
		{
			base.OnStart();
			m_Time = Time.time;
			m_AnimationTimer.Reset(AnimationTime);
			var weapon = Controller.GetWeapons()[0] as CMeleeWeapon;
			weapon.LookAtCamera(false);
			weapon.GetTrailRenderer().gameObject.SetActive(true);
			weapon.GetTrailRenderer().material.SetFloat(COdd.DashTrailAlphaMultiplyID, 0f);
			//Controller.ME.SetAngularSpeed(COddController.OddAngularSpeed * 0.2f);


			float angularSpeed;
			int animationHash;
			float transition = 0.05f;
			bool attackShouldEnd = true;
			switch (Controller.PreviousState)
			{
				case OddState.Idle:
					angularSpeed = COddController.OddAngularSpeed;
					animationHash = Animation1Hash;
					attackShouldEnd = false;
					break;
				case OddState.Walking:
					angularSpeed = COddController.OddAngularSpeed;
					animationHash = Animation1Hash;
					attackShouldEnd = false;
					break;
				case OddState.Running:
					angularSpeed = COddController.OddAngularSpeed;
					animationHash = Animation1Hash;
					attackShouldEnd = false;
					break;
				case OddState.Attacking1:
					angularSpeed = COddController.OddAngularSpeed * 0.2f;
					animationHash = Animation2Hash;
					break;
				case OddState.Attacking2:
					angularSpeed = COddController.OddAngularSpeed * 0.2f;
					animationHash = Animation1Hash;
					break;
				case OddState.Attacking3:
					angularSpeed = COddController.OddAngularSpeed * 0.2f;
					animationHash = Animation2Hash;
					break;
				case OddState.Attacking4:
					angularSpeed = COddController.OddAngularSpeed * 0.05f;
					animationHash = Animation1Hash;
					break;
				case OddState.AttackingHeavy1:
					angularSpeed = COddController.OddAngularSpeed * 0.2f;
					animationHash = Animation2Hash;
					break;
				case OddState.DashAttack:
					angularSpeed = COddController.OddAngularSpeed * 0.2f;
					animationHash = Animation2Hash;
					break;
				case OddState.AttackRecovering:
					{
						switch (Controller.GetCurrentAttack())
						{
							case Def.OddAttackType.Normal1:
								angularSpeed = COddController.OddAngularSpeed * 0.2f;
								animationHash = Animation2Hash;
								break;
							case Def.OddAttackType.Normal2:
								angularSpeed = COddController.OddAngularSpeed * 0.2f;
								animationHash = Animation1Hash;
								break;
							case Def.OddAttackType.Normal3:
								angularSpeed = COddController.OddAngularSpeed * 0.2f;
								animationHash = Animation2Hash;
								break;
							case Def.OddAttackType.Normal4:
								angularSpeed = COddController.OddAngularSpeed;
								animationHash = Animation1Hash;
								break;
							case Def.OddAttackType.Heavy1:
								angularSpeed = COddController.OddAngularSpeed * 0.05f;
								animationHash = Animation2Hash;
								break;
							case Def.OddAttackType.Dash:
								angularSpeed = COddController.OddAngularSpeed * 0.20f;
								animationHash = Animation2Hash;
								break;
							default:
								Debug.LogWarning($"You should not reach here from: {Controller.GetCurrentAttack()}");
								return;
						}
					}
					break;
				default:
					Debug.LogWarning($"You should not reach here from: {Controller.PreviousState}");
					return;
			}
			Controller.Odd.GetAnimator().CrossFadeInFixedTime(animationHash, transition, 0);
			Controller.ME.SetAngularSpeed(angularSpeed);
			Controller.ME.SetSightSpeed(Controller.ME.GetAngularSpeed());
			if(attackShouldEnd)
				Controller.GetWeapons()[0].OnAttackEnd();
		}
		public override void OnStop()
		{
			base.OnStop();
		}
		public override void OnUpdate()
		{
			base.OnUpdate();

			float timeDiff = Time.time - m_Time;
			if (Input.GetMouseButtonUp(0))
			{
				if(timeDiff < 0.25f) // Normal
				{
					switch (Controller.PreviousState)
					{
						case OddState.Idle:
						case OddState.Walking:
						case OddState.Running:
						case OddState.Attacking4:
							Controller.ChangeState(OddState.Attacking1);
							Controller.CurrentStateController.OnUpdate();
							break;
						case OddState.AttackingHeavy1:
						case OddState.Attacking1:
						case OddState.DashAttack:
							Controller.ChangeState(OddState.Attacking2);
							Controller.CurrentStateController.OnUpdate();
							break;
						case OddState.Attacking2:
							Controller.ChangeState(OddState.Attacking3);
							Controller.CurrentStateController.OnUpdate();
							break;
						case OddState.Attacking3:
							Controller.ChangeState(OddState.Attacking4);
							Controller.CurrentStateController.OnUpdate();
							break;
						case OddState.AttackRecovering:
							{
								switch (Controller.GetCurrentAttack())
								{
									case Def.OddAttackType.Normal1:
									case Def.OddAttackType.Dash:
										Controller.ChangeState(OddState.Attacking2);
										Controller.CurrentStateController.OnUpdate();
										break;
									case Def.OddAttackType.Heavy1:
									case Def.OddAttackType.Normal2:
										Controller.ChangeState(OddState.Attacking3);
										Controller.CurrentStateController.OnUpdate();
										break;
									case Def.OddAttackType.Normal3:
										Controller.ChangeState(OddState.Attacking4);
										Controller.CurrentStateController.OnUpdate();
										break;
									case Def.OddAttackType.Normal4:
										Controller.ChangeState(OddState.Attacking1);
										Controller.CurrentStateController.OnUpdate();
										break;
									default:
										Debug.LogWarning($"You should not reach here from: {Controller.GetCurrentAttack()}");
										break;
								}
							}
							break;
						default:
							Debug.LogWarning($"You should not reach here from: {Controller.PreviousState}");
							break;
					}
				}
			}
			if(timeDiff > 0.25f) // Heavy
			{
				switch (Controller.PreviousState)
				{
					case OddState.Idle:
					case OddState.Walking:
					case OddState.Running:
					case OddState.Attacking4:
					case OddState.Attacking2:
						m_NextState = OddState.AttackingHeavy1;
						break;
					case OddState.Attacking1:
					case OddState.Attacking3:
					case OddState.AttackingHeavy1:
					case OddState.DashAttack:
						m_NextState = OddState.AttackingHeavy2;
						break;
					case OddState.AttackRecovering:
						{
							switch (Controller.GetCurrentAttack())
							{
								case Def.OddAttackType.Normal1:
								case Def.OddAttackType.Normal3:
								case Def.OddAttackType.Heavy1:
								case Def.OddAttackType.Dash:
									m_NextState = OddState.AttackingHeavy2;
									break;
								case Def.OddAttackType.Normal2:
								case Def.OddAttackType.Normal4:
									m_NextState = OddState.AttackingHeavy1;
									break;
								default:
									Debug.LogWarning($"You should not reach here from: {Controller.GetCurrentAttack()}");
									break;
							}
						}
						break;
					default:
						Debug.LogWarning($"You should not reach here from: {Controller.PreviousState}");
						break;
				}
			}

			var targetEntity = Controller.Odd.GetTargetEntity();
			if(targetEntity != null)
			{
				var posXZ = new Vector2(Controller.transform.position.x, Controller.transform.position.z);
				var trgXZ = new Vector2(targetEntity.transform.position.x, targetEntity.transform.position.z);
				Controller.ME.SetDirection(trgXZ - posXZ);
			}
			else if(Manager.Mgr.GameInputControl == Def.GameInputControl.MouseLikeController)
			{
				var cam = CameraManager.Mgr.Camera;
				var posXZ = new Vector2(Controller.transform.position.x, Controller.transform.position.z);
				var camXZ = new Vector2(cam.transform.position.x, cam.transform.position.z);
				Controller.ME.SetDirection(posXZ - camXZ);
			}

			var movXZ = Controller.ME.UpdateMovement();
			var mov = new Vector3(movXZ.x, GameUtils.Gravity, movXZ.y);
			mov *= Time.deltaTime;

			var cc = Controller.ME.GetController();
			var collision = cc.Move(mov);
			if (collision.HasFlag(CollisionFlags.CollidedSides))
				Controller.ME.OnCollision();

			m_AnimationTimer.Update();
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