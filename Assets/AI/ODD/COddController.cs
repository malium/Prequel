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
	public class COddController : MonoBehaviour
	{
		public struct TargetArmRotationInfo
		{
			public float Angle;
			public Vector3 StartAxis;
			public Vector3 EndAxis;
			public float Duration;
			//public float AttackStartPCT;
			//public float AttackEndPCT;
			public float StartFadeInPCT;
			public float EndFadeInPCT;
			public float StartFadeOutPCT;
			public float EndFadeOutPCT;
		}
		public enum RotateTo
		{
			Movement,
			Sight
		}
		public readonly static string BaseWeaponName = "Classic Sword";
		const int WeaponSlots = 2;
		const float TPWaitTime = 0.2f;
		public const float OddHeight = 1.4f;
		public const float OddRadius = 0.4f;
		public const float OddMaxSpeed = 3f;
		public const float OddWeight = 1f;
		public const float OddAngularSpeed = 12f;

		static readonly int DamageFrontAnimationHash = Animator.StringToHash("DamageLayer.DamageFront");
		static readonly int DamageBackAnimationHash = Animator.StringToHash("DamageLayer.DamageBack");
		static readonly int DamageLeftAnimationHash = Animator.StringToHash("DamageLayer.DamageLeft");
		static readonly int DamageRightAnimationHash = Animator.StringToHash("DamageLayer.DamageRight");
		static readonly int DamageCoverAnimationHash = Animator.StringToHash("DamageLayer.DamageCover");
		static readonly int DamageNoneAnimationHash = Animator.StringToHash("DamageLayer.Empty");
		
		const float DamageAnimationLength = 0.5f;
		const float DamageAnimationSpeed = 1f;
		const float DamageAnimationTime = DamageAnimationLength / DamageAnimationSpeed;
		const float CoverDamageAnimationLength = 0.333f;
		const float CoverDamageAnimationSpeed = 1f;
		const float CoverDamageAnimationTime = CoverDamageAnimationLength / CoverDamageAnimationSpeed;
		
		static readonly IOddState[] OddStates = new IOddState[(int)OddState.COUNT]
		{
			new IdleState(),
			new WalkingState(),
			new RunningState(),
			new Attack1State(),
			new Attack2State(),
			new Attack3State(),
			new Attack4State(),
			new HeavyAttack1State(),
			new HeavyAttack2State(),
			new DashAttackState(),
			new JumpState(),
			new NullState(), // Building
			new PossessingState(),
			new NullState(), // Unpossessing
			new CoverState(),
			new DashState(),
			new BackDashState(),
			new NullState(), // Throwing
			new DeathState(),
			new AttackRecoveringState(),
			new AttackInState(),
			new ConsumableState(),
		};

		public Vector3[] StartAxis;
		public Vector3[] EndAxis;

		[SerializeReference] CLivingEntity m_LE;
		[SerializeReference] CMovableEntity m_ME;
		[SerializeReference] CSpellCaster m_SpellCaser;
		[SerializeReference] COdd m_ODD;
		[SerializeReference] IOddWeapon[] m_Weapons;
		[SerializeField] OddState m_CurrentState = OddState.COUNT;
		[SerializeField] OddState m_PreviousState = OddState.COUNT;
		[SerializeField] float m_OddHeadSpeed;
		[SerializeField] float m_HeadYRot;

		float m_SpiritMovementTime;

		IOddState m_CurrentStateController;
		Quaternion m_PrevHeadRot;
		TargetArmRotationInfo m_TARInfo;
		Timer m_TARTimer;
		RotateTo m_RotateTo;
		bool m_NextDamageCovered;
		Timer m_OnDamageReceivedTimer;
		Def.SpaceDirection m_DamageDirection;
		[SerializeField] Def.OddAttackType m_CurrentAttack;
		[SerializeField] bool m_IsCovered;
		
		public COddController()
		{
			m_OnDamageReceivedTimer = new Timer(true, false);
			m_OnDamageReceivedTimer.OnTimerTrigger += OnDamageAnimationEnd;
			m_Weapons = new IOddWeapon[WeaponSlots];
		}
		public COdd Odd { get => m_ODD; }
		public CMovableEntity ME { get => m_ME; }
		public CLivingEntity LE { get => m_LE; }
		public CSpellCaster SpellCaster { get => m_SpellCaser; }
		public Def.OddAttackType GetCurrentAttack() => m_CurrentAttack;
		public void SetCurrentAttack(Def.OddAttackType attack) => m_CurrentAttack = attack;
		public IOddWeapon[] GetWeapons() => m_Weapons;
		public OddState CurrentState => m_CurrentState;
		public OddState PreviousState => m_PreviousState;
		public IOddState CurrentStateController => m_CurrentStateController;
		public bool IsCovered() => m_IsCovered;
		public void SetIsCovered(bool isCovered) => m_IsCovered = isCovered;
		public void ChangeState(OddState state)
		{
			if (m_CurrentState == state && m_CurrentStateController != null && m_CurrentStateController.State == state)
				return;
			m_PreviousState = m_CurrentState;
			m_CurrentState = state;
			//Debug.Log($"State from {m_PreviousState} to {m_CurrentState}");
			if(m_CurrentStateController != null)
			{
				m_CurrentStateController.OnStop();
				m_CurrentStateController = null;
			}
			m_CurrentStateController = OddStates[(int)m_CurrentState];
			m_CurrentStateController.Controller = this;
			m_CurrentStateController.OnStart();
		}
		public void Init()
		{
			if(m_LE == null && (m_LE = gameObject.GetComponent<CLivingEntity>()) == null)
				m_LE = gameObject.AddComponent<CLivingEntity>();

			if (m_ME == null && (m_ME = gameObject.GetComponent<CMovableEntity>()) == null)
				m_ME = gameObject.AddComponent<CMovableEntity>();

			if (m_SpellCaser == null && (m_SpellCaser = gameObject.GetComponent<CSpellCaster>()) == null)
				m_SpellCaser = gameObject.AddComponent<CSpellCaster>();

			if (m_ODD == null && (m_ODD = gameObject.GetComponent<COdd>()) == null)
				m_ODD = gameObject.AddComponent<COdd>();

			

			m_LE.SetRadius(OddRadius);
			m_LE.SetHeight(OddHeight);

			m_LE.OnEntityDeath += OnDeath;

			m_PrevHeadRot = m_ODD.GetHeadBone().transform.localRotation;
			//m_LE.OnReceiveDamage += OnReceiveDamage;

			m_RotateTo = RotateTo.Movement;
			m_SpiritMovementTime = 0f;

			m_TARTimer = new Timer(true, false);

			if (m_OddHeadSpeed <= 0f)
				m_OddHeadSpeed = 10f;

			m_ME.SetME(new MovableInfo()
			{
				AngularSpeed = OddAngularSpeed,
				ControllerCenter = new Vector3(0f, 0.7f, 0f),
				MaxSpeed = OddMaxSpeed,
				Weight = OddWeight,
				MaxStep = 0.5f,
				MaxSightMovDir = 70f,
				SightSpeed = OddAngularSpeed
			});
			var characterController = m_ME.GetController();
			characterController.skinWidth = 0.01f;
			transform.localScale = COdd.DefaultScale;

			m_LE.Init(Def.LivingEntityType.ODD, 100f, 0.05f, 100f, 0.05f, new Resistance[]
			{
				new Resistance() { Type = Def.ResistanceType.PHYSICAL, Value = 0.5f, HealIfNegative = false },
				new Resistance() { Type = Def.ResistanceType.ELEMENTAL, Value = 0.5f, HealIfNegative = false },
				new Resistance() { Type = Def.ResistanceType.ULTIMATE, Value = 0.5f, HealIfNegative = false },
				new Resistance() { Type = Def.ResistanceType.SOUL, Value = 0.5f, HealIfNegative = false },
				new Resistance() { Type = Def.ResistanceType.POISON, Value = 0.5f, HealIfNegative = false },
			}, characterController, OddRadius * COdd.DefaultScale.x, OddHeight * COdd.DefaultScale.y, "ODD");

			m_ODD.Init();

			m_LE.OnReceiveDamage += OnReceiveDamage;
			m_LE.OnDamageFX += OnDamageFX;

			if (WeaponLoader.Dict.TryGetValue(BaseWeaponName, out int weaponIdx))
			{
				m_Weapons[0].Init(this, WeaponLoader.WeaponInfos[weaponIdx]);
			}

			ChangeState(OddState.Idle);
		}
		private void OnDeath(CLivingEntity entity)
		{
			ChangeState(OddState.Death);
		}
		private void Awake()
		{
			if (StartAxis == null)
				StartAxis = new Vector3[Def.OddAttackTypeCount];
			if (EndAxis == null)
				EndAxis = new Vector3[Def.OddAttackTypeCount];
		}
		private void Update()
		{
			if (Manager.Mgr.IsPaused)
				return;

			UpdateSpiritMovement();
			UpdateRotation();
			m_ODD.UpdateTarget();

			m_CurrentStateController?.OnUpdate();

			m_TARTimer.Update();
			m_OnDamageReceivedTimer.Update();

			m_SpellCaser.UpdateSpells();
			m_SpellCaser.UpdateItems();
			m_LE.UpdateElements();

			//m_ODD.UpdateSpiritMovement();
			//m_ODD.UpdateRotation();

			//var movement = m_ME.ComputeKeyboardMoveDirectionRelativeToCamera(CameraManager.Mgr.Camera, KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D); //UpdateMoveDirection();
			//var oldPos = transform.position;
			//if (movement != Vector2.zero)
			//{
			//	m_ME.Impulse(movement);
			//}
			//else if (m_LE.GetCurrentStruc() == null)
			//{
			//	m_LE.UpdateStruc();
			//}
			//if (m_LE.GetCurrentBlock() == null)
			//	m_LE.UpdateBlock();
			//Vector2 movXZ = m_ME.UpdateMovement();

			//var mov = new Vector3(movXZ.x, Gravity, movXZ.y);
			//mov *= Time.deltaTime;

			//var collision = m_ME.GetController().Move(mov);
			//if (collision.HasFlag(CollisionFlags.CollidedSides))
			//{
			//	m_ME.OnCollision();
			//}
			//if(transform.position.x != oldPos.x || transform.position.z != oldPos.z)
			//{
			//	m_LE.UpdateStruc();
			//	if(m_LE.GetCurrentStruc() != null)
			//	{
			//		var nextBlock = m_LE.ComputeCurrentBlock() as CBlock;

			//		if(nextBlock == null)
			//		{
			//			transform.position = oldPos;
			//			m_ME.OnCollision();
			//		}
			//		else
			//		{
			//			m_ODD.GetAnimator().SetInteger(COdd.HashNextState, (int)Def.OddAnimationState.Walk);
			//			//Debug.Log("Walk");
			//			m_LE.SetCurrentBlock(nextBlock);
			//		}
			//	}
			//}
			//else
			//{
			//	m_ODD.GetAnimator().SetInteger(COdd.HashNextState, (int)Def.OddAnimationState.Idle);
			//	//Debug.Log("Idle");
			//}
			//m_CurrentStateController?.OnUpdate();
			//m_SpellCaser.UpdateSpells();
			//m_LE.UpdateElements();
		}
		private void FixedUpdate()
		{
			m_CurrentStateController?.OnFixedUpdate();

			var targetEntity = m_ODD.GetTargetEntity();
			if (targetEntity == null)
			{
				if (m_RotateTo == RotateTo.Movement)
				{
					m_ME.SetTargetSightAsTargetMov();
					m_HeadYRot = 0f;
				}
			}
			else
			{
				Vector2 movDir;
				if (m_RotateTo == RotateTo.Movement)
					movDir = m_ME.GetTargetDirection();
				else
					movDir = m_ME.GetTargetSightDirection();

				var trgTemp = new Vector2(targetEntity.transform.position.x - transform.position.x, targetEntity.transform.position.z - transform.position.z);
				var trgDir = trgTemp.normalized;
				var angle = GameUtils.AngleBetween2D(movDir, trgDir) * Mathf.Rad2Deg;
				if (Mathf.Abs(angle) <= m_ME.GetMaxSightMoveDiff())
				{
					m_ME.SetSightDirection(trgDir);
					float dist = Mathf.Sqrt(trgTemp.x * trgTemp.x + trgTemp.y * trgTemp.y);
					float trgHeight = targetEntity.transform.position.y + targetEntity.GetHeight() * 0.5f;
					float oddHeadHeight = m_ODD.GetHeadBone().transform.position.y;
					float diff = oddHeadHeight - trgHeight;
					if (!GameUtils.IsNearlyEqual(diff, 0f))
					{
						var atan = Mathf.Atan(dist / diff);
						if (diff >= 0f)
						{
							m_HeadYRot = (Def.HALFPI - atan) * Mathf.Rad2Deg;
							m_HeadYRot = Mathf.Min(m_HeadYRot, 45f);
						}
						else
						{
							m_HeadYRot = -(atan + Def.HALFPI) * Mathf.Rad2Deg;
							m_HeadYRot = Mathf.Max(m_HeadYRot, -85f);
							//m_HeadYRot = -(HALFPI + atan) * Mathf.Rad2Deg;
						}
					}
					else
					{
						m_HeadYRot = 0f;
					}
				}
				else
				{
					if (m_RotateTo == RotateTo.Movement)
					{
						m_ME.SetTargetSightAsTargetMov();
						m_HeadYRot = 0f;
					}
				}
			}
		}
		public void SetRotateTo(RotateTo rotateTo) => m_RotateTo = rotateTo;
		public RotateTo GetRotateTo() => m_RotateTo;
		public void SetTargetArmRotation(TargetArmRotationInfo info)
		{
			m_TARInfo = info;
			m_TARTimer.Reset(info.Duration);
			UpdateArmsRotation();
		}
		public float GetArmRotationForTarget(CLivingEntity targetEntity)
		{
			var trgPos = targetEntity.transform.position;
			trgPos.Set(trgPos.x, trgPos.y + targetEntity.GetHeight() * 0.5f, trgPos.z);

			var bone = m_ODD.GetArmsMainBone();
			float diff = bone.transform.position.y - trgPos.y;
			float dist = Vector3.Distance(bone.transform.position, trgPos);
			if (!GameUtils.IsNearlyEqual(diff, 0f))
			{
				var atan = Mathf.Atan(dist / diff);
				if (diff >= 0f)
				{
					return (Def.HALFPI - atan) * Mathf.Rad2Deg;
				}
				else
				{
					return -(atan + Def.HALFPI) * Mathf.Rad2Deg;
				}
			}
			else
			{
				return 0f;
			}
		}
		void UpdateOddHeadRotation()
		{
			var sight = m_ME.GetSightDirection();
			var fwd = new Vector2(transform.forward.x, transform.forward.z);
			var angle =  GameUtils.AngleBetween2D(sight, fwd) * Mathf.Rad2Deg;

			//Debug.Log("Angle between " + angle.ToString());
			var target = Quaternion.Euler(m_HeadYRot, angle, 0f);
			var current = Quaternion.Slerp(m_PrevHeadRot, target, m_OddHeadSpeed * Time.deltaTime);
			m_ODD.GetHeadBone().transform.localRotation *= current;
			m_PrevHeadRot = current;
		}
		float GetAngleBetweenChestHand(GameObject chest, GameObject hand)
		{
			float diff = chest.transform.position.y - hand.transform.position.y;
			var handXZ = new Vector2(hand.transform.position.x, hand.transform.position.z);
			var chestXZ = new Vector2(chest.transform.position.x, chest.transform.position.z);
			var dist = Vector2.Distance(handXZ, chestXZ);
			if (!GameUtils.IsNearlyEqual(diff, 0f))
			{
				var atan = Mathf.Atan(dist / diff);
				if (diff >= 0f)
				{
					return (Def.HALFPI - atan) * Mathf.Rad2Deg;
				}
				else
				{
					return -(atan + Def.HALFPI) * Mathf.Rad2Deg;
				}
			}
			else
			{
				return 0f;
			}
		}
		void UpdateArmsRotation()
		{
			if (m_TARTimer.IsPaused() || m_TARTimer.IsFinished())
				return;

			var info = m_TARInfo;
			var angle = info.Angle;
			var pct = m_TARTimer.GetRemainingPct();

			//if (!m_SlashCollider.gameObject.activeSelf && pct >= info.AttackStartPCT)
			//	OnStartAttack();

			//if (m_SlashDuration == SlashDuration)
			//	m_SlashCollider.gameObject.SetActive(false);
			//if (m_SlashCollider.gameObject.activeSelf)
			//{
			//	var startAngle = SlashColliderStartRotation[(int)m_CurrentAttack];
			//	var endAngle = SlashColliderEndRotation[(int)m_CurrentAttack];
			//	m_SlashDuration = Mathf.Min(m_SlashDuration + Time.deltaTime, SlashDuration);
			//	var slashAngle = Vector3.LerpUnclamped(startAngle, endAngle, m_SlashDuration / SlashDuration);
			//	m_SlashCollider.transform.localRotation = Quaternion.Euler(slashAngle);

			//	//if (pct > info.AttackEndPCT)
			//	//	OnEndAttack();
			//}

			var chRotation = GetAngleBetweenChestHand(m_ODD.GetArmsMainBone(), m_Weapons[0].gameObject);

			(m_Weapons[0] as CMeleeWeapon)?.OnArmsRotation(angle, chRotation);

			angle -= chRotation;

			var axisA = (1f - pct) * angle * info.StartAxis;
			var axisB = angle * pct * info.EndAxis;
			var axis = axisA + axisB;

			float fadeIn;
			if(pct >= info.StartFadeInPCT)
			{
				if(pct <= info.EndFadeInPCT)
				{
					var diff = Mathf.Abs(info.EndFadeInPCT - info.StartFadeInPCT);
					fadeIn = (pct - info.StartFadeInPCT) / diff;
				}
				else
				{
					fadeIn = 1f;
				}
			}
			else
			{
				fadeIn = 0f;
			}

			float fadeOut;
			if(pct >= info.StartFadeOutPCT)
			{
				if(pct <= info.EndFadeOutPCT)
				{
					var diff = Mathf.Abs(info.EndFadeOutPCT - info.StartFadeOutPCT);
					fadeOut = 1f - ((pct - info.StartFadeOutPCT) / diff);
				}
				else
				{
					fadeOut = 0f;
				}
			}
			else
			{
				fadeOut = 1f;
			}

			var fade = fadeIn * fadeOut;
			
			//Debug.Log(fade);

			axis *= fade;

			m_ODD.GetArmsMainBone().transform.Rotate(axis, Space.Self);
		}
		void UpdateRotation()
		{
			Vector2 dir;
			if (m_RotateTo == RotateTo.Movement)
				dir = m_ME.GetDirection();
			else
				dir = m_ME.GetSightDirection();
			//var angle = GameUtils.AngleFromDir2D(dir);

			transform.LookAt(new Vector3(transform.position.x + dir.x, transform.position.y, transform.position.z + dir.y));
			//transform.localRotation *= Quaternion.Euler(-90f, 0f, 0f);
			//transform.localRotation = Quaternion.Euler(-90f, Mathf.Rad2Deg * angle, 0f);
		}
		void UpdateSpiritMovement()
		{
			float yOffset = GameUtils.SinMovement1D(0f, 0.25f, 0.6f, m_SpiritMovementTime);
			m_SpiritMovementTime += Time.deltaTime;

			var meshGO = m_ODD.MeshGO;
			meshGO.transform.localPosition = new Vector3(meshGO.transform.localPosition.x, yOffset, meshGO.transform.localPosition.z);
		}
		private void OnDamageFX(Vector3 pos, Vector3 dir)
		{
			var angle = GameUtils.AngleBetween2D(m_ME.GetDirection(), new Vector2(dir.x, dir.z)) * Mathf.Rad2Deg;
			var absAngle = Mathf.Abs(angle);

			if (absAngle >= 135f) // Front
			{
				m_DamageDirection = Def.SpaceDirection.NORTH;
			}
			else if (absAngle <= 45f) //Back
			{
				m_DamageDirection = Def.SpaceDirection.SOUTH;
			}
			else
			{
				if (angle < 0f) // Left
				{
					m_DamageDirection = Def.SpaceDirection.WEST;
				}
				else // Right
				{
					m_DamageDirection = Def.SpaceDirection.EAST;
				}
			}

			if (m_IsCovered)
			{
				var weapon = m_Weapons[0] as CMeleeWeapon;
				var info = weapon.WeaponInfo;
				var minAngle = 180f - info.CoverRangeDegrees * 0.5f;
				if (absAngle > minAngle) // Covered!
				{
					m_NextDamageCovered = true;
				}
				else
				{
					m_NextDamageCovered = false;
				}
			}
			else
			{
				m_NextDamageCovered = false;
			}
		}
		private void OnReceiveDamage(CLivingEntity caster, CLivingEntity receiver, Def.DamageType type, float damageAmout)
		{
			if (m_NextDamageCovered)
			{
				var weaponInfo = m_Weapons[0].WeaponInfo;
				var damage = Mathf.Max(damageAmout - weaponInfo.DamageReductionFixed, 0f);
				damage *= weaponInfo.DamageReductionPCT;
				m_LE.ReceiveHealing(m_LE, damage);

				m_ODD.GetAnimator().SetLayerWeight(1, 1f);
				m_ODD.GetAnimator().CrossFadeInFixedTime(DamageCoverAnimationHash, 0.2f, 1);
				m_OnDamageReceivedTimer.Reset(CoverDamageAnimationTime);
			}
			else
			{
				m_ODD.GetAnimator().SetLayerWeight(1, 0.75f);
				switch (m_DamageDirection)
				{
					case Def.SpaceDirection.NORTH:
						m_ODD.GetAnimator().CrossFadeInFixedTime(DamageFrontAnimationHash, 0.2f, 1);
						m_OnDamageReceivedTimer.Reset(DamageAnimationTime);
						break;
					case Def.SpaceDirection.SOUTH:
						m_ODD.GetAnimator().CrossFadeInFixedTime(DamageBackAnimationHash, 0.2f, 1);
						m_OnDamageReceivedTimer.Reset(DamageAnimationTime);
						break;
					case Def.SpaceDirection.EAST:
						m_ODD.GetAnimator().CrossFadeInFixedTime(DamageRightAnimationHash, 0.2f, 1);
						m_OnDamageReceivedTimer.Reset(DamageAnimationTime);
						break;
					case Def.SpaceDirection.WEST:
						m_ODD.GetAnimator().CrossFadeInFixedTime(DamageLeftAnimationHash, 0.2f, 1);
						m_OnDamageReceivedTimer.Reset(DamageAnimationTime);
						break;
				}
			}
		}
		void OnDamageAnimationEnd()
		{
			m_ODD.GetAnimator().SetLayerWeight(1, 0.75f);
			m_ODD.GetAnimator().CrossFadeInFixedTime(DamageNoneAnimationHash, 0.2f, 1);
		}
		private void LateUpdate()
		{
			m_CurrentStateController?.OnLateUpdate();
			
			UpdateOddHeadRotation();
			UpdateArmsRotation();
			//Debug.Log(diff * Mathf.Rad2Deg);
		}
		private void OnDrawGizmos()
		{
			if (Manager.Mgr != null && m_LE != null && Manager.Mgr.DebugOddPosition && m_LE.GetCurrentBlock() != null)
			{
				var oldColor = Gizmos.color;
				Gizmos.color = Color.red;
				var pilar = m_LE.GetCurrentBlock().GetPilar();
				Gizmos.DrawSphere(new Vector3(pilar.transform.position.x + 0.5f, m_LE.GetCurrentBlock().transform.position.y, pilar.transform.position.z + 0.5f), 0.25f);
				Gizmos.color = oldColor;
			}
			m_CurrentStateController?.OnGizmos();
		}
		private void OnGUI()
		{
			m_CurrentStateController?.OnGUI();
		}
	}
}
