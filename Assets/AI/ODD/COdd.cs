/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AI.ODD
{
	public enum OddState
	{
		Idle,
		Walking,
		Running,
		Attacking1,
		Attacking2,
		Attacking3,
		Attacking4,
		AttackingHeavy1,
		AttackingHeavy2,
		DashAttack,
		Jumping,
		Building,
		Possessing,
		Unpossessing,
		Covering,
		Dashing,
		BackDashing,
		Throwing,
		Death,
		AttackRecovering,
		AttackIn,
		Consumable,

		COUNT
	}
	public class COdd : MonoBehaviour
	{
		public static int HashNextState = Animator.StringToHash("NextState");
		public static int DashTrailAlphaMultiplyID = Shader.PropertyToID("_AlphaMultiply");

		public static readonly Vector3 DefaultScale = new Vector3(0.9f, 0.9f, 0.9f);
		
		const float PossessionOutColorDuration = 0.66f;

		const float MaxLockDistance = 12f;
		const float LockFindRadius = 5f;

		[SerializeReference] CLivingEntity m_LE;
		[SerializeReference] CMovableEntity m_ME;
		[SerializeReference] CSpellCaster m_SpellCaster;
		[SerializeReference] Animator m_Animator;
		[SerializeReference] SkinnedMeshRenderer m_MeshRenderer;
		[SerializeReference] SpriteRenderer m_FaceRenderer;
		[SerializeReference] SpriteRenderer m_ItemRenderer;
		[SerializeReference] GameObject m_MeshGO;
		[SerializeReference] Outline m_Outline;
		[SerializeReference] Material m_DeathMaterial;
		[SerializeReference] TrailRenderer m_DashTrailRenderer;
		
		[SerializeReference] UnityEngine.VFX.VisualEffect m_PossessionVFX;

		[SerializeReference] GameObject FaceBone;
		[SerializeReference] GameObject HeadBone;
		[SerializeReference] GameObject ChestBone;
		[SerializeReference] GameObject ArmsMainBone;
		[SerializeReference] GameObject ArmLeftBone;
		[SerializeReference] GameObject ArmRightBone;
		[SerializeReference] GameObject SwordLBone;
		[SerializeReference] GameObject SmallSwordLBone;
		[SerializeReference] GameObject ShieldLBone;
		[SerializeReference] GameObject SmallSwordRBone;
		[SerializeReference] GameObject ShieldRBone;
		
		
		CLivingEntity m_TargetEntity;
		static RaycastHit[] m_TargetRC;
		static Collider[] m_TargetColliders;
		List<CLivingEntity> m_TargetList;
		UI.CLockArrow[] m_LockArrow;
		int m_LockArrowIdx;

		const float LooseTargetPressingAfter = 0.15f;
		float m_PressingTargetTime;

		//public Vector3 LeftArmRot;
		//public Vector3 RightArmRot;

		public Outline GetOutline() => m_Outline;
		public Animator GetAnimator() => m_Animator;
		public CLivingEntity LE { get => m_LE; }
		public CMovableEntity ME { get => m_ME; }
		public CSpellCaster SpellCaster { get => m_SpellCaster; }
		public SkinnedMeshRenderer GetMeshRenderer() => m_MeshRenderer;
		public SpriteRenderer GetFaceRenderer() => m_FaceRenderer;
		public TrailRenderer GetDashTrailRenderer() => m_DashTrailRenderer;
		public UnityEngine.VFX.VisualEffect GetPossessionVFX() => m_PossessionVFX;
		public SpriteRenderer GetItemRenderer() => m_ItemRenderer;
		public GameObject MeshGO => m_MeshGO;
		public GameObject GetHeadBone() => HeadBone;
		public GameObject GetArmsMainBone() => ArmsMainBone;
		public GameObject GetLArmBone() => ArmLeftBone;
		public GameObject GetRArmBone() => ArmRightBone;
		public Material DeathMaterial => m_DeathMaterial;

		public COdd()
		{
			if(m_TargetRC == null)
				m_TargetRC = new RaycastHit[16];
			if (m_TargetColliders == null)
				m_TargetColliders = new Collider[32];
			m_TargetList = new List<CLivingEntity>(32);
			m_LockArrow = new UI.CLockArrow[2];
			m_LockArrowIdx = 0;
		}
		public void Init()
		{
			if(m_LE == null)
				m_LE = gameObject.GetComponent<CLivingEntity>();

			if(m_ME == null)
				m_ME = gameObject.GetComponent<CMovableEntity>();

			if (m_SpellCaster == null)
				m_SpellCaster = gameObject.GetComponent<CSpellCaster>();

			//m_SpiritMovementTime = 0f;

			//m_MeshGO.transform.localRotation = Quaternion.identity;

			m_SpellCaster.Init(m_LE, m_ME, new List<AttackInfo>());

			m_MeshRenderer.gameObject.layer = Def.RCLayerLE;
			m_FaceRenderer.gameObject.layer = Def.RCLayerLE;
			m_ME.GetController().gameObject.layer = Def.RCLayerLE;
		}
		public IEnumerator OnPossessionOut()
		{
			float time = 0f;

			var material = m_MeshRenderer.material;
			while (time <= PossessionOutColorDuration)
			{
				material.SetVector(Def.OverlayColorID, new Vector4(0f, 1f, 0.647f, 1f - (time / PossessionOutColorDuration)));
				time += Time.deltaTime;
				yield return null;
			}
			material.SetVector(Def.OverlayColorID, new Vector4(0f, 1f, 0.647f, 0f));
		}
		void LockTarget(CLivingEntity le)
		{
			if (m_TargetEntity != null && m_TargetEntity != le)
			{
				m_TargetEntity.OnEntityDeath -= OnTargetDeath;
			}
			m_TargetEntity = le;
			if(!m_TargetList.Contains(le))
				m_TargetList.Add(le);
			m_TargetEntity.OnEntityDeath += OnTargetDeath;

			var prevArrow = m_LockArrow[m_LockArrowIdx];
			if (prevArrow != null)
				prevArrow.Unset();

			m_LockArrowIdx = GameUtils.Mod(++m_LockArrowIdx, m_LockArrow.Length);
			var lockArrow = m_LockArrow[m_LockArrowIdx];
			if(lockArrow == null)
				lockArrow =  m_LockArrow[m_LockArrowIdx] = Instantiate(AssetLoader.LockArrow);

			lockArrow.gameObject.SetActive(true);
			lockArrow.Set(m_TargetEntity);
		}
		CLivingEntity FindTargetRC(int hitRCCount, bool checkList)
		{
			CLivingEntity closest = null;
			float distance = float.MaxValue;
			for (int i = 0; i < hitRCCount; ++i)
			{
				var mon = m_TargetRC[i].transform.gameObject.GetComponent<CMonsterController>();
				if (mon == null || !mon.enabled)
					continue;

				if (checkList && m_TargetList.Contains(mon.GetLE()))
					continue;

				var dist = Vector3.Distance(transform.position, mon.transform.position);
				if (dist < distance)
				{
					closest = mon.GetLE();
					distance = dist;
				}
			}

			if (distance > MaxLockDistance)
				closest = null;

			return closest;
		}
		CLivingEntity FindTargetCol(int hitColCount, Vector3 hitPoint, bool checkList)
		{
			float distance = float.MaxValue;
			CLivingEntity closest = null;
			for (int i = 0; i < hitColCount; ++i)
			{
				var mon = m_TargetColliders[i].gameObject.GetComponent<CMonsterController>();
				if (mon == null || !mon.enabled)
					continue;

				if (checkList && m_TargetList.Contains(mon.GetLE()))
					continue;

				var distOdd = Vector3.Distance(transform.position, mon.transform.position);
				var distHit = Vector3.Distance(hitPoint, mon.transform.position);

				if (distOdd >= MaxLockDistance)
					continue;

				if (distHit < distance)
				{
					closest = mon.GetLE();
					distance = distHit;
				}
			}

			return closest;
		}
		public void OnLockTarget()
		{
			var cam = CameraManager.Mgr.Camera;

			var hitRCCount = Physics.RaycastNonAlloc(cam.ScreenPointToRay(Input.mousePosition), m_TargetRC, 100f, 1 << Def.RCLayerLE);

			var closest = FindTargetRC(hitRCCount, true);

			if(closest != null)
			{
				LockTarget(closest);
				//Debug.Log("Dict OnTop");
				return;
			}

			if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, 1 << Def.RCLayerRayPlane))
				return;

			float radiusMult = 1f;
			if (Manager.Mgr.GameInputControl == Def.GameInputControl.MouseLikeController)
				radiusMult = 2f;

			var hitColCount = Physics.OverlapSphereNonAlloc(hit.point, LockFindRadius * radiusMult, m_TargetColliders, 1 << Def.RCLayerLE);

			closest = FindTargetCol(hitColCount, hit.point, true);

			if (closest != null)
			{
				LockTarget(closest);
				//Debug.Log("Dict Near");
				return;
			}

			hitRCCount = Physics.RaycastNonAlloc(cam.ScreenPointToRay(Input.mousePosition), m_TargetRC, 100f, 1 << Def.RCLayerLE);

			closest = FindTargetRC(hitRCCount, false);

			if (closest != null)
			{
				if(m_TargetList.Count == 1 && m_TargetEntity == closest)
				{
					OnUnlockTarget();
					//Debug.Log("Found just previous OnTop");
					return;
				}
				m_TargetList.Clear();
				LockTarget(closest);
				//Debug.Log("NoDict OnTop");
				return;
			}

			if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, 100f, 1 << Def.RCLayerRayPlane))
				return;

			hitColCount = Physics.OverlapSphereNonAlloc(hit.point, LockFindRadius * radiusMult, m_TargetColliders, 1 << Def.RCLayerLE);

			closest = FindTargetCol(hitColCount, hit.point, false);

			if (closest != null)
			{
				if (m_TargetList.Count == 1 && m_TargetEntity == closest)
				{
					OnUnlockTarget();
					//Debug.Log("Found just previous Near");
					return;
				}
				m_TargetList.Clear();
				LockTarget(closest);
				//Debug.Log("NoDict Near");
				return;
			}


			if (m_TargetEntity != null || m_TargetList.Count > 0)
			{
				//Debug.Log("Found nothing, unlock");
				OnUnlockTarget();
			}
			else
			{
				//Debug.Log("Found nothing");
			}
		}
		private void OnTargetDeath(CLivingEntity entity)
		{
			OnUnlockTarget();
			OnLockTarget();
		}
		public void OnUnlockTarget()
		{
			m_TargetList.Clear();
			m_TargetEntity = null;
			var lockArrow = m_LockArrow[m_LockArrowIdx];
			if(lockArrow != null)
			{
				lockArrow.Unset();
			}
		}
		public CLivingEntity GetTargetEntity() => m_TargetEntity;
		public void SetTargtEntity(CLivingEntity le) => m_TargetEntity = le;
		public void UpdateTarget()
		{
			if (Input.GetKeyDown(KeyCode.F))
			{
				m_PressingTargetTime = Time.time;
			}
			if (Input.GetKey(KeyCode.F))
			{
				if (m_TargetEntity != null)
				{
					var diff = Time.time - m_PressingTargetTime;
					if (diff > LooseTargetPressingAfter)
						OnUnlockTarget();
				}
			}
			if (Input.GetKeyUp(KeyCode.F))
			{
				var diff = Time.time - m_PressingTargetTime;
				if (diff < LooseTargetPressingAfter)
					OnLockTarget();
			}
		}
		private void Update()
		{
			if(m_TargetEntity != null && Input.GetMouseButtonDown(2))
			{
				var targetXZ = new Vector2(m_TargetEntity.transform.position.x, m_TargetEntity.transform.position.z);
				var posXZ = new Vector2(transform.position.x, transform.position.z);
				m_ME.SetDirectionInstantly(targetXZ - posXZ);
				m_ME.SetSightDirectionInstantly(m_ME.GetDirection());
			}
		}
		private void FixedUpdate()
		{
			if(m_TargetEntity != null)
			{
				//var posXZ = new Vector2(transform.position.x, transform.position.z);
				//var trgXZ = new Vector2(m_TargetEntity.transform.position.x, m_TargetEntity.transform.position.z);
				//var dist = Vector.Distance(posXZ, trgXZ);
				var dist = Vector3.Distance(transform.position, m_TargetEntity.transform.position);
				if (dist > MaxLockDistance)
					OnUnlockTarget();
			}
		}
		private void LateUpdate()
		{
			//m_MeshGO.transform.localRotation = Quaternion.identity;
			//var leftRot = ArmLeftBone.transform.localRotation.eulerAngles;
			//var rightRot = ArmRightBone.transform.localRotation.eulerAngles;
			//Debug.Log($"Left: {leftRot}");
			//Debug.Log($"Right: {rightRot}");

			//ArmLeftBone.transform.Rotate(LeftArmRot, Space.Self);
			//ArmRightBone.transform.Rotate(RightArmRot, Space.Self);
		}
		private void OnDrawGizmos()
		{
			
		}
		private void OnDestroy()
		{
			if (m_LockArrow != null)
			{
				for(int i = 0; i < m_LockArrow.Length; ++i)
				{
					var arrow = m_LockArrow[i];
					if (arrow != null)
						GameUtils.DeleteGameobject(arrow.gameObject);
				}
			}
		}
	}
}
