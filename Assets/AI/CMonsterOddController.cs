/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using UnityEngine;

namespace Assets.AI
{
	public class CMonsterOddController : MonoBehaviour
	{
		CLivingEntity m_LE;
		CMovableEntity m_ME;
		CMonster m_Monster;
		ODD.COddController m_Odd;
		CSpellCaster m_SpellCaster;
		BoxCollider m_RayPlane;
		const float Gravity = -9.81f;
		//CBlock m_CurrentBlock;
		const float gAngleDiff = 45f * Mathf.Deg2Rad;
		const float OnPossessSSADuration = 0.66f;

		void Awake()
		{
			//m_CurrentBlock = null;
		}
		public CLivingEntity GetLE() => m_LE;
		public CMovableEntity GetME() => m_ME;
		public CMonster GetMonster() => m_Monster;
		public ODD.COddController GetODD() => m_Odd;
		public void Set(CMonster monster, ODD.COddController odd = null)
		{
			m_Monster = monster;
			m_LE = monster.GetLE();
			m_ME = monster.GetME();
			m_SpellCaster = monster.GetSpellCaster();
			m_Odd = odd;
			m_LE.OnEntityDeath += OnEntityDeath;

			if (!m_Odd)
			{
				m_RayPlane = new GameObject("RayPlane").AddComponent<BoxCollider>();
				m_RayPlane.transform.SetParent(transform);
				m_RayPlane.transform.localPosition = Vector3.zero;
				m_RayPlane.transform.localRotation = Quaternion.identity;
				m_RayPlane.transform.localScale = Vector3.one;
				m_RayPlane.size = new Vector3(100f, 0.01f, 100f);
				m_RayPlane.gameObject.layer = Def.RCLayerRayPlane;
			}
			else // has been possessed
			{
				var ssa = gameObject.AddComponent<SquashStrechAnimation>();
				ssa.Set(transform, Vector3.one, OnPossessSSADuration, true, SquashStrechAnimation.RoundsType.Monsters, true, 1.5f);
				StartCoroutine(OnPosssessColor());
			}
			OnFamilyUpdated();
		}
		IEnumerator OnPosssessColor()
		{
			float time = 0f;
			var material = (m_Monster.GetSprite().GetRenderer() as MeshRenderer).material;
			var halfTime = OnPossessSSADuration * 0.5f;
			while (time <= halfTime)
			{
				material.SetVector(Def.OverlayColorID, new Vector4(0f, 1f, 0.647f, (time / halfTime) * 0.75f));
				time += Time.deltaTime;
				yield return null;
			}
			while(time < OnPossessSSADuration)
			{
				material.SetVector(Def.OverlayColorID, new Vector4(0f, 1f, 0.647f, (1f - ((time - halfTime) / halfTime)) * 0.75f));
				time += Time.deltaTime;
				yield return null;
			}

			material.SetVector(Def.OverlayColorID, new Vector4(0f, 1f, 0.647f, 0f));
		}
		
		public void OnFamilyUpdated()
		{
			var info = m_Monster.GetFamily().Info;

		}
		private void OnEntityDeath(CLivingEntity entity)
		{
			m_LE.OnEntityDeath -= OnEntityDeath;
			enabled = false;
			m_LE.GetCollider().enabled = false;
			var anim = gameObject.AddComponent<FadeoutAnimation>();
			anim.Set(() => m_Monster.GetSprite().GetColor(),
				(Color color) =>
				{
					m_Monster.GetSprite().SetColor(color);
					m_Monster.GetShadow().SetColor(color);
					if(m_LE.GetStatusBars() != null)
						m_LE.GetStatusBars().SetAlpha(color.a);
				},
				1f, false, () => { GameUtils.DeleteGameobject(gameObject); });
			
			if(m_Odd != null)
			{
				m_Odd.transform.SetParent(null);
				m_Odd.enabled = true;
				m_Odd.LE.GetCollider().enabled = true;
				m_Odd.ME.enabled = true;
				m_Odd.LE.enabled = true;
				m_Odd.Odd.enabled = true;
				m_Odd.Odd.GetMeshRenderer().enabled = true;
				m_Odd.Odd.GetFaceRenderer().enabled = true;
				m_Odd.Odd.GetOutline().enabled = false;
				m_Odd.GetWeapons()[0].gameObject.SetActive(true);
				m_Odd.ChangeState(ODD.OddState.Idle);
				CameraManager.Mgr.Target = m_Odd.gameObject;
			}
		}
		public Vector2 UpdateMoveDirection()
		{
			Vector2 movDir = Vector2.zero;
			var camera = CameraManager.Mgr;
			if (Input.GetKey(KeyCode.W))
			{
				movDir += new Vector2(camera.transform.forward.x, camera.transform.forward.z);
			}
			else if (Input.GetKey(KeyCode.S))
			{
				movDir += new Vector2(-camera.transform.forward.x, -camera.transform.forward.z);
			}
			if (Input.GetKey(KeyCode.A))
			{
				movDir += new Vector2(-camera.transform.right.x, -camera.transform.right.z);
			}
			else if (Input.GetKey(KeyCode.D))
			{
				movDir += new Vector2(camera.transform.right.x, camera.transform.right.z);
			}
			return movDir;
			//bool fwd = false;
			//if (Input.GetKey(KeyCode.W))
			//{
			//	movDir.Set(camera.transform.forward.x, camera.transform.forward.z);
			//	fwd = true;
			//}
			//else if(Input.GetKey(KeyCode.S))
			//{
			//	movDir.Set(-camera.transform.forward.x, -camera.transform.forward.z);
			//	fwd = true;
			//}
			//if(Input.GetKey(KeyCode.A))
			//{
			//	if (fwd)
			//		movDir.Set((movDir.x + -camera.transform.right.x) * 0.5f, (movDir.y + -camera.transform.right.z) * 0.5f);
			//	else
			//		movDir.Set(-camera.transform.right.x, -camera.transform.right.z);
			//}
			//else if(Input.GetKey(KeyCode.D))
			//{
			//	//movDir.Set(movDir.x, 1f);
			//	if (fwd)
			//		movDir.Set((movDir.x + camera.transform.right.x) * 0.5f, (movDir.y + camera.transform.right.z) * 0.5f);
			//	else
			//		movDir.Set(camera.transform.right.x, camera.transform.right.z);
			//}
			//return movDir.normalized;
		}
		//public CBlock GetCurrentBlock() => m_CurrentBlock;
		//public void SetCurrentBlock(CBlock block)
		//{
		//	m_CurrentBlock = block;
		//}
		//public CBlock ComputeCurrentBlock()
		//{
		//	var struc = m_LE.GetCurrentStruc();
		//	if (struc == null)
		//		return null;
		//	var vStrucPos = GameUtils.TransformPosition(new Vector2(struc.transform.position.x, struc.transform.position.z));
		//	var vPos = GameUtils.TransformPosition(new Vector2(transform.position.x, transform.position.z));
		//	var vPilarPos = vPos - vStrucPos;
		//	if (vPilarPos.x >= 0 && vPilarPos.x < CStruc.Width &&
		//		vPilarPos.y >= 0 && vPilarPos.y < CStruc.Height)
		//	{
		//		var id = struc.PilarIDFromVPos(vPilarPos);
		//		var pilar = struc.GetPilars()[id];
		//		if (pilar != null)
		//		{
		//			return pilar.GetClosestBlock(transform.position.y) as CBlock;
		//		}
		//	}
		//	return null;
		//}
		void Update()
		{
			if (Manager.Mgr.IsPaused)
				return;
			if(m_Odd != null && Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.LeftShift))
			{
				var ssa = gameObject.AddComponent<SquashStrechAnimation>();
				ssa.Set(transform, Vector3.one, OnPossessSSADuration, true, SquashStrechAnimation.RoundsType.Monsters, true, 1.5f);
				m_Monster.StartCoroutine(OnPosssessColor());

				m_Odd.transform.SetParent(null);
				m_Odd.enabled = true;
				m_Odd.LE.GetCollider().enabled = true;
				m_Odd.ME.enabled = true;
				m_Odd.LE.enabled = true;
				m_Odd.Odd.enabled = true;
				m_Odd.transform.localScale = ODD.COdd.DefaultScale;
				m_Odd.Odd.GetMeshRenderer().enabled = true;
				m_Odd.Odd.GetFaceRenderer().enabled = true;
				m_Odd.Odd.GetOutline().enabled = false;
				m_Odd.GetWeapons()[0].gameObject.SetActive(true);
				m_Odd.ChangeState(ODD.OddState.Idle);
				CameraManager.Mgr.Target = m_Odd.gameObject;
				m_LE.OnEntityDeath -= OnEntityDeath;
				if(gameObject.TryGetComponent(out CMonsterController mc))
				{
					mc.enabled = true;
					GameObject.Destroy(this);
				}
				m_Odd.StartCoroutine(m_Odd.Odd.OnPossessionOut());
				return;
			}

			var movement = UpdateMoveDirection();
			var oldPos = transform.position;

			var spell0State = m_SpellCaster.GetSpell(0).GetCurrentState();
			var spell1State = m_SpellCaster.GetSpell(1).GetCurrentState();


			if(Input.GetMouseButton(0) && spell0State == Def.SpellState.IDLE && (spell1State == Def.SpellState.COOLDOWN || spell1State == Def.SpellState.IDLE))
			{
				if (Physics.Raycast(CameraManager.Mgr.Camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, 1 << Def.RCLayerRayPlane))
				{
					var dir = new Vector2(hit.point.x, hit.point.z) - new Vector2(transform.position.x, transform.position.z);
					m_ME.SetDirection(dir);
					m_ME.SetSightDirection(dir);
					m_SpellCaster.UseSpell(0, null, new Vector3(hit.point.x, transform.position.y + m_LE.GetHeight() * 0.5f, hit.point.z));
				}
			}
			if (Input.GetMouseButton(1) && spell1State == Def.SpellState.IDLE && (spell0State == Def.SpellState.COOLDOWN || spell0State == Def.SpellState.IDLE))
			{
				if (Physics.Raycast(CameraManager.Mgr.Camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, 1 << Def.RCLayerRayPlane))
				{
					var dir = new Vector2(hit.point.x, hit.point.z) - new Vector2(transform.position.x, transform.position.z);
					m_ME.SetDirection(dir);
					m_ME.SetSightDirection(dir);
					m_SpellCaster.UseSpell(1, null, new Vector3(hit.point.x, transform.position.y + m_LE.GetHeight() * 0.5f, hit.point.z));
				}
			}
			if(m_Odd != null)
				m_Odd.Odd.UpdateTarget();

			if (movement != Vector2.zero)
			{
				m_ME.Impulse(movement);
				m_ME.SetSightDirection(movement);
			}
			else if(m_LE.GetCurrentStruc() == null)
			{
				m_LE.UpdateStruc();
			}
			if (m_LE.GetCurrentBlock())
				m_LE.UpdateBlock();
			//if(m_CurrentBlock == null)
			//	SetCurrentBlock(ComputeCurrentBlock());

			Vector2 movXZ = m_ME.UpdateMovement();

			if (m_ME.GetMovementState() != Def.MovementState.Stopped)
			{
				var dir = m_ME.GetDirection();
				float radius = m_LE.GetRadius();
				var collided = CheckMIDCollision(dir, radius * 2f);
				if(collided)
				{
					//Debug.Log("MID collision: " + collided.GetName());
				}
				else
				{
					float angle = GameUtils.AngleFromDir2D(dir);
					var rightDir = GameUtils.DirFromAngle2D(angle + gAngleDiff);
					collided = CheckMIDCollision(rightDir, radius * 2.5f);
					if (collided)
					{
						//Debug.Log("MID left collision: " + collided.GetName());
					}
					else
					{
						var leftDir = GameUtils.DirFromAngle2D(angle - gAngleDiff);
						collided = CheckMIDCollision(leftDir, radius * 2.5f);
						if(collided)
						{
							//Debug.Log("MID right collision: " + collided.GetName());
						}
					}
				}
				if(collided != null)
				{
					m_ME.OnLECollision(collided);
					movXZ = m_ME.GetCurrentSpeed();
				}
			}
			

			var mov = new Vector3(movXZ.x, Gravity, movXZ.y);
			mov *= Time.deltaTime;

			var collision = m_ME.GetController().Move(mov);
			if (collision.HasFlag(CollisionFlags.CollidedSides))
			{
				m_ME.OnCollision();
			}

			if (transform.position.x != oldPos.x || transform.position.z != oldPos.z)
			{
				m_LE.UpdateStruc();
				if (m_LE.GetCurrentStruc() != null)
				{
					var nextBlock = m_LE.ComputeCurrentBlock();
					if (nextBlock == null)
					{
						transform.position = oldPos;
						m_ME.OnCollision();
					}
					else
					{
						//m_ODD.GetAnimator().SetInteger(COdd.HashNextState, (int)Def.OddAnimationState.Walk);
						//Debug.Log("Walk");
						m_LE.SetCurrentBlock(nextBlock);
						//SetCurrentBlock(nextBlock);
					}
				}
			}
			else
			{
				//m_ODD.GetAnimator().SetInteger(COdd.HashNextState, (int)Def.OddAnimationState.Idle);
				//Debug.Log("Idle");
			}

			m_SpellCaster.UpdateSpells();
			m_SpellCaster.UpdateItems();
			m_Monster.FrameUpdate();
			m_LE.UpdateElements();
		}
		CLivingEntity CheckMIDCollision(Vector2 dir, float length)
		{
			var ray = new Ray(transform.position + new Vector3(0f, m_LE.GetHeight() * 0.3f, 0f),
				new Vector3(dir.x, 0f, dir.y));
			int layer = 1 << Def.RCLayerLE;

			if (Physics.Raycast(ray, out RaycastHit hitInfo, length, layer))
			{
				var le = hitInfo.collider.gameObject.GetComponent<CLivingEntity>();
				return le;
			}
			return null;
		}
		//private void OnDrawGizmos()
		//{
  //          var dir = m_ME.GetDirection();
  //          float angle = GameUtils.AngleFromDir2D(dir);
  //          var leftDir = GameUtils.DirFromAngle2D(angle + gAngleDiff);
  //          var rightDir = GameUtils.DirFromAngle2D(angle - gAngleDiff);
  //          var mid = transform.position + new Vector3(0f, m_LE.GetHeight() * 0.5f, 0f);
  //          float diameter = m_LE.GetRadius() * 1.9f;
  //          Gizmos.DrawLine(mid, mid + new Vector3(dir.x * diameter, 0f, dir.y * diameter));
  //          Gizmos.DrawLine(mid, mid + new Vector3(leftDir.x * diameter, 0f, leftDir.y * diameter));
  //          Gizmos.DrawLine(mid, mid + new Vector3(rightDir.x * diameter, 0f, rightDir.y * diameter));
  //      }
	}
}