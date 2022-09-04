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

namespace Assets.AI
{
	public struct MovableInfo
	{
		public float MaxSpeed;
		public float AngularSpeed;
		public float Weight;
		public float MaxStep;
		public Vector3 ControllerCenter;
		public float MaxSightMovDir;
		public float SightSpeed;
	}

	public class CMovableEntity : MonoBehaviour
	{
		const float TWOPI = Mathf.PI * 2f;

		[SerializeReference] CLivingEntity m_LE;
		[SerializeField] CharacterController m_Controller;
		[SerializeField] float m_MaxSpeed;
		[SerializeField] float m_AngularSpeed;
		[SerializeField] float m_Weight;
		[SerializeField] Vector2 m_MoveDirection;
		[SerializeField] Vector2 m_SightDirection;
		[SerializeField] float m_MaxSightMovDiff;
		float m_TargetAngle;
		float m_AngleSign;
		float m_CurAngle;
		float m_TargetSightAngle;
		float m_SightAngleSign;
		float m_CurSightAngle;
		[SerializeField] Vector2 m_Speed;
		[SerializeField] float m_Moment;
		[SerializeField] float m_ImpulseStrength;
		[SerializeField] float m_StoppingMoment;
		[SerializeField] float m_SightSpeed;
		[SerializeField] Def.MovementState m_CurrentState;

		public float GUIHeight { get; private set; }
		public float GetCurrentMoment() => m_Moment;
		public float GetMaxSpeed() => m_MaxSpeed;
		public void SetMaxSpeed(float speed) => m_MaxSpeed = speed;
		public float GetAngularSpeed() => m_AngularSpeed;
		public void SetAngularSpeed(float speed) => m_AngularSpeed = speed;
		public float GetSightSpeed() => m_SightSpeed;
		public void SetSightSpeed(float speed) => m_SightSpeed = speed;
		public float GetMaxSightMoveDiff() => m_MaxSightMovDiff;
		public void SetMaxSightMoveDiff(float diff) => m_MaxSightMovDiff = diff;
		public float GetWeight() => m_Weight;
		public void SetWeight(float weight) => m_Weight = weight;
		public Def.MovementState GetMovementState() => m_CurrentState;
		public Vector2 GetDirection() => m_MoveDirection;
		public Vector2 GetSightDirection() => m_SightDirection;
		public void SetDirectionInstantly(Vector2 direction)
		{
			m_MoveDirection = direction.normalized;
			var dir = GameUtils.AngleFromDir2D(m_MoveDirection);
			m_TargetAngle = dir;
			m_CurAngle = dir;
		}
		public void SetDirection(Vector2 direction)
		{
			direction.Normalize();
			m_TargetAngle = GameUtils.AngleFromDir2D(direction);
			m_CurAngle = GameUtils.AngleFromDir2D(m_MoveDirection);
			//var curAngle = GameUtils.AngleFromDir2D(m_Direction);

			if (m_CurAngle >= m_TargetAngle)
			{
				float adist = m_CurAngle - m_TargetAngle;
				float ldist = GameUtils.Mod(m_TargetAngle - m_CurAngle, TWOPI);
				if (adist < ldist)
				{
					m_AngleSign = -1f;
				}
				else
				{
					m_AngleSign = 1f;
					m_TargetAngle = m_CurAngle + ldist;
				}
			}
			else 
			{
				float adist = m_TargetAngle - m_CurAngle;
				float ldist = GameUtils.Mod(m_CurAngle - m_TargetAngle, TWOPI);
				if (adist > ldist)
				{
					m_AngleSign = -1f;
					m_TargetAngle -= TWOPI;
				}
				else
				{
					m_AngleSign = 1f;
				}
			}
		}
		public void SetSightDirectionInstantly(Vector2 direction)
		{
			m_SightDirection = direction.normalized;
			var dir = GameUtils.AngleFromDir2D(m_SightDirection);
			if(dir > (m_TargetAngle + m_MaxSightMovDiff))
			{
				dir = m_TargetAngle + m_MaxSightMovDiff;
				m_SightDirection = GameUtils.DirFromAngle2D(dir);
			}
			else if(dir < (m_TargetAngle - m_MaxSightMovDiff))
			{
				dir = m_TargetAngle - m_MaxSightMovDiff;
				m_SightDirection = GameUtils.DirFromAngle2D(dir);
			}
			m_TargetSightAngle = dir;
			m_CurSightAngle = dir;
		}
		public void SetSightDirection(Vector2 direction)
		{
			direction.Normalize();
			m_TargetSightAngle = GameUtils.AngleFromDir2D(direction);

			if (m_TargetSightAngle > (m_TargetAngle + m_MaxSightMovDiff))
			{
				m_TargetSightAngle = m_TargetAngle + m_MaxSightMovDiff;
			}
			else if (m_TargetSightAngle < (m_TargetAngle - m_MaxSightMovDiff))
			{
				m_TargetSightAngle = m_TargetAngle - m_MaxSightMovDiff;
			}

			m_CurSightAngle = GameUtils.AngleFromDir2D(m_SightDirection);
			//var curAngle = GameUtils.AngleFromDir2D(m_Direction);

			if (m_CurSightAngle >= m_TargetSightAngle)
			{
				// Dist Cur-Trg left to right
				float adist = m_CurSightAngle - m_TargetSightAngle;
				// Dist Trg-Cur right to left
				float ldist = GameUtils.Mod(m_TargetSightAngle - m_CurSightAngle, TWOPI);
				if (adist < ldist)
				{
					m_SightAngleSign = -1f;
				}
				else
				{
					m_SightAngleSign = 1f;
					m_TargetSightAngle = m_CurSightAngle + ldist;
				}
			}
			else
			{
				float adist = m_TargetSightAngle - m_CurSightAngle;
				float ldist = GameUtils.Mod(m_CurSightAngle - m_TargetSightAngle, TWOPI);
				if (adist > ldist)
				{
					m_SightAngleSign = -1f;
					m_TargetSightAngle -= TWOPI;
				}
				else
				{
					m_SightAngleSign = 1f;
				}
			}
		}
		public Vector2 GetTargetDirection() => GameUtils.DirFromAngle2D(m_TargetAngle);
		public Vector2 GetTargetSightDirection() => GameUtils.DirFromAngle2D(m_TargetSightAngle);
		public void SetTargetSightAsTargetMov()
		{
			SetSightDirection(GetDirection());
			//if (m_TargetAngle >= m_CurSightAngle)
			//	m_SightAngleSign = +1f;
			//else
			//	m_SightAngleSign = -1f;
			//m_TargetSightAngle = m_TargetAngle;
		}
		public float GetCurrentAngleDir() => m_CurAngle;
		public float GetCurrentSightAngleDir() => m_CurSightAngle;
		public CharacterController GetController() => m_Controller;
		public CLivingEntity GetLE() => m_LE;
		private void Awake()
		{
			m_CurrentState = Def.MovementState.Stopped;
			m_MoveDirection = new Vector2(1f, 0f);
			m_SightDirection = new Vector2(1f, 0f);
			m_ImpulseStrength = 0f;
			m_CurAngle = GameUtils.AngleFromDir2D(m_MoveDirection);
			m_TargetAngle = m_CurAngle;
			m_CurSightAngle = m_CurAngle;
			m_TargetSightAngle = m_CurSightAngle;
		}
		public void SetME(MovableInfo info)
		{
			m_LE = gameObject.GetComponent<CLivingEntity>();
			m_Controller = null;
			m_Controller = gameObject.GetComponent<CharacterController>();
			if (m_Controller == null)
				m_Controller = gameObject.AddComponent<CharacterController>();

			m_Controller.radius = m_LE.GetRadius();
			m_Controller.height = m_LE.GetHeight();
			m_Controller.center = info.ControllerCenter;
			m_Controller.stepOffset = info.MaxStep;
			m_Controller.slopeLimit = 45f;

			m_SightSpeed = info.SightSpeed;
			m_MaxSightMovDiff = info.MaxSightMovDir;

			m_MaxSpeed = info.MaxSpeed;
			m_AngularSpeed = info.AngularSpeed;
			m_Weight = info.Weight;
		}
		public void Impulse(Vector2 impulseDir, float impulseStrength = 1f)
		{
			if ((impulseDir.x > 0.001f || impulseDir.x < -0.001f) && (impulseDir.y > 0.001f || impulseDir.y < -0.001f))
			{
				SetDirection(impulseDir);
				//m_Direction = impulseDir.normalized;
				//m_Speed = m_Direction * m_Moment;
			}
			m_ImpulseStrength = impulseStrength;
		}
		void ComputeStopping()
		{
			if (m_ImpulseStrength <= 0.001f)
			{
				float slowDownTime = 1f;
				if (m_LE.GetCurrentBlock() != null && m_LE.GetCurrentBlock().GetMaterialFamily() != null)
					slowDownTime = m_LE.GetCurrentBlock().GetMaterialFamily().FamilyInfo.SlowdownTime;

				float speed;
				if (slowDownTime <= 0f)
				{
					speed = m_MaxSpeed;
				}
				else
				{
					float timePCT = Time.deltaTime / slowDownTime;
					speed = m_StoppingMoment * timePCT;
				}

				
				m_Speed -= m_MoveDirection * speed;
				var tempMoment = m_Speed.magnitude;
				if(tempMoment > m_Moment)
				{
					m_Speed = Vector2.zero;
					m_Moment = 0f;
					m_CurrentState = Def.MovementState.Stopped;
				}
				else
				{
					m_Moment = tempMoment;
				}
			}
			else
			{
				m_CurrentState = Def.MovementState.RampingUp;
				ComputeRampingUp();
			}
		}
		void ComputeRampingUp()
		{
			if (m_ImpulseStrength > 0.001f)
			{
				float speedUpTime = 1f;
				if (m_LE.GetCurrentBlock() != null && m_LE.GetCurrentBlock().GetMaterialFamily() != null)
					speedUpTime = m_LE.GetCurrentBlock().GetMaterialFamily().FamilyInfo.SlowdownTime;

				speedUpTime /= m_ImpulseStrength;

				float speed;
				if (speedUpTime > 0f)
				{
					float timePCT = Time.deltaTime / speedUpTime;
					speed = m_MaxSpeed * timePCT;
				}
				else
				{
					speed = m_MaxSpeed;
				}
				
				m_Speed += m_MoveDirection * speed;
				m_Moment = m_Speed.magnitude;
				if(m_Moment >= m_MaxSpeed)
				{
					m_Speed *= (m_MaxSpeed / m_Moment);
					m_Moment = m_MaxSpeed;
					//m_CurrentState = Def.MovementState.Continuous;
				}

				//float momentPCT = m_Moment / m_MaxSpeed;

				//float speedTime = speedUpTime * momentPCT;
				//speedTime += Time.deltaTime;
				////Debug.Log("Moment:" + (momentPCT * 100f).ToString() + "% TotalSlow:" + speedUpTime.ToString() + " CurrentSlow:" + speedTime.ToString());

				//if (speedTime >= speedUpTime)
				//{
				//	//Debug.Log("Continuous!");
				//	m_Moment = m_MaxSpeed;
				//	m_Speed = m_Direction * m_MaxSpeed;
				//	m_CurrentState = Def.MovementState.Continuous;
				//}
				//else
				//{
				//	float speedTimePCT = speedTime / speedUpTime;
				//	m_Moment = m_MaxSpeed * speedTimePCT;
				//	m_Speed = m_Direction * m_Moment;
				//}
			}
			else
			{
				m_CurrentState = Def.MovementState.Stopping;
				m_StoppingMoment = m_Moment;
				ComputeStopping();
			}
		}
		void ComputeContinuous()
		{
			if (m_ImpulseStrength > 0.001f)
			{
				m_Speed += m_MoveDirection * m_MaxSpeed;
				m_Speed *= (m_MaxSpeed / m_Speed.magnitude);
				//m_Speed = m_Direction * m_MaxSpeed;
			}
			else
			{
				m_CurrentState = Def.MovementState.Stopping;
				m_StoppingMoment = m_MaxSpeed;
				ComputeStopping();
			}
		}
		void ComputeStopped()
		{
			if (m_ImpulseStrength > 0.001f)
			{
				m_CurrentState = Def.MovementState.RampingUp;
				ComputeRampingUp();
			}
		}
		public void OnCollision(/*Def.SpaceDirection direction*/)
		{
			if(m_CurrentState != Def.MovementState.Stopped)
			{
				m_Speed *= 0.85f;
				m_Moment *= 0.85f;
				if (m_CurrentState != Def.MovementState.Stopping)
				{
					m_CurrentState = Def.MovementState.RampingUp;
				}
			}
		}
		public void OnReceivedCollision(Vector2 dir)
		{
			var oldDir = m_MoveDirection;
			Impulse(dir);
			var movement = UpdateMovement();
			m_Controller.Move(new Vector3(movement.x, -9.81f, movement.y) * Time.deltaTime);
			m_MoveDirection = oldDir;
		}
		public void OnLECollision(CLivingEntity le)
		{
			var posXZ = new Vector2(transform.position.x, transform.position.z);
			var colXZ = new Vector2(le.transform.position.x, le.transform.position.z);
			var offXZ = (posXZ - colXZ);
			var dist = offXZ.magnitude;
			var colDir = offXZ.normalized;
			//var dist = Vector2.Distance(posXZ, colXZ);
			dist -= m_LE.GetRadius() + le.GetRadius();
			//Debug.Log("Collision dist: " + dist.ToString());
			if (dist >= 0.5f)
				return;
			if(le.gameObject.TryGetComponent(out CMovableEntity me))
			{
				me.OnReceivedCollision(-colDir);
			}
			if (dist > 0.1f && dist < 0.5f)
			{
				float mult = Mathf.Lerp(0.1f, 0.5f, dist);
				m_Speed = m_Speed * (1f - mult) + Vector2.zero * mult;
			}
			else if (dist < 0.1f)
			{
				m_Speed = colDir * m_Moment;
				//m_Speed = Vector2.zero;
			}
			m_Moment = m_Speed.magnitude;
		}
		//void OnTriggerEnter(Collider other)
		//{
		//	if (other.gameObject.layer != Def.RCLayerLE)
		//		return;
		//	var le = other.gameObject.GetComponent<CLivingEntity>();
		//	Debug.Log(le.GetName());
		//}
		private void UpdateDirection()
		{
			//var curAngle = GameUtils.AngleFromDir2D(m_Direction);
			var amount = m_AngularSpeed * Time.deltaTime;
			m_CurAngle += m_AngleSign * amount;
			if (m_AngleSign > 0f)
			{
				m_CurAngle = Mathf.Min(m_CurAngle, m_TargetAngle);
			}
			else
			{
				m_CurAngle = Mathf.Max(m_CurAngle, m_TargetAngle);                                    
			}
			//Debug.Log($"Cur:{m_CurAngle}Target:{m_TargetAngle}");
			m_MoveDirection = GameUtils.DirFromAngle2D(GameUtils.Mod(m_CurAngle, TWOPI));
		}
		void UpdateSightDirection()
		{
			var amount = m_SightSpeed * Time.deltaTime;
			m_CurSightAngle += m_SightAngleSign * amount;
			if (m_SightAngleSign > 0f)
			{
				m_CurSightAngle = Mathf.Min(m_CurSightAngle, m_TargetSightAngle);
			}
			else
			{
				m_CurSightAngle = Mathf.Max(m_CurSightAngle, m_TargetSightAngle);
			}
			//Debug.Log($"Cur:{m_CurAngle}Target:{m_TargetAngle}");
			m_SightDirection = GameUtils.DirFromAngle2D(GameUtils.Mod(m_CurSightAngle, TWOPI));
		}
		public Vector2 UpdateMovement()
		{
			UpdateDirection();
			UpdateSightDirection();
			switch (m_CurrentState)
			{
				case Def.MovementState.RampingUp: // Trying to reach top speed
					ComputeRampingUp();
					break;
				case Def.MovementState.Continuous: // At top speed
					ComputeContinuous();
					break;
				case Def.MovementState.Stopping: // Trying to reach stop
					ComputeStopping();
					break;
				case Def.MovementState.Stopped: // No movement
					ComputeStopped();
					break;
			}
			m_ImpulseStrength = 0f;
			return m_Speed;
		}
		public Vector2 ComputeKeyboardMoveDirectionRelativeToCamera(Camera camera, KeyCode forward, KeyCode backward,
			KeyCode left, KeyCode right)
		{
			Vector2 movDir = Vector2.zero;
			if (Input.GetKey(forward))
			{
				movDir += new Vector2(camera.transform.forward.x, camera.transform.forward.z);
			}
			else if (Input.GetKey(backward))
			{
				movDir += new Vector2(-camera.transform.forward.x, -camera.transform.forward.z);
			}
			if (Input.GetKey(left))
			{
				movDir += new Vector2(-camera.transform.right.x, -camera.transform.right.z);
			}
			else if (Input.GetKey(right))
			{
				movDir += new Vector2(camera.transform.right.x, camera.transform.right.z);
			}
			return movDir.normalized;
		}
		public Vector2 GetCurrentSpeed() => m_Speed;
		void OnDrawGizmos()
		{
			if (Manager.Mgr == null || m_LE == null || !Manager.Mgr.DebugMovement )
				return;

			var oColor = Gizmos.color;

			Gizmos.color = Color.white;
			Gizmos.DrawRay(transform.position + new Vector3(0f, m_LE.GetHeight(), 0f), new Vector3(m_MoveDirection.x, 0f, m_MoveDirection.y));

			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(transform.position + new Vector3(0f, m_LE.GetHeight(), 0f), new Vector3(m_SightDirection.x, 0f, m_SightDirection.y));

			Gizmos.color = oColor;
		}
		private void OnGUI()
		{
			if(m_LE == null || m_LE.GUIHeight < 0f || !Manager.Mgr.DebugMovement)
			{
				GUIHeight = 0f;
				return;
			}

			var oldColor = GUI.contentColor;

			var cam = CameraManager.Mgr;

			var wPos = transform.position;
			wPos += new Vector3(0f, m_LE.GetHeight(), 0f);
			var sPos = cam.Camera.WorldToScreenPoint(wPos);

			var stateContent = new GUIContent("MovState: " + m_CurrentState.ToString());
			var momentContent = new GUIContent("Moment " + m_Moment.ToString("f2"));

			var stateSize = GUI.skin.label.CalcSize(stateContent);
			var momentSize = GUI.skin.label.CalcSize(momentContent);

			GUIHeight = stateSize.y + momentSize.y;

			var sPoint = new Vector2(sPos.x, Screen.height - sPos.y);
			var gPoint = GUIUtility.ScreenToGUIPoint(sPoint);

			var stateRect = new Rect(gPoint.x, gPoint.y - (GUIHeight + m_LE.GUIHeight), stateSize.x, stateSize.y);
			var momentRect = new Rect(gPoint.x, stateRect.y + stateSize.y, momentSize.x, momentSize.y);

			GUI.color = Color.white;
			GUI.Label(stateRect, stateContent);
			GUI.Label(momentRect, momentContent);

			GUI.contentColor = oldColor;
		}
	}
}
