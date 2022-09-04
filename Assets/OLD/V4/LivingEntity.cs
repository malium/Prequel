/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace Assets
//{
//	public struct DealingDamage
//	{
//		public Def.DamageType DamageType;
//		public float DamageAmount;
//	}
//	public struct LivingEntityDef
//	{
//		public float TotalHealth;
//		public float Speed;
//		public float AngularSpeed;
//		public float Radius;
//		public float Height;
//		public float MaxJump;
//		public float FlySpeed;
//		public float FallSpeed;
//		public Def.LivingEntityType LEType;
//	}

//	public abstract class LivingEntity : MonoBehaviour
//	{
//		public const string TAG = "LIVING_ENTITY";

//		static readonly Vector2Int[] DirCheck = new Vector2Int[4]
//		{
//			new Vector2Int(1, 0),
//			new Vector2Int(0, 1),
//			new Vector2Int(-1, 0),
//			new Vector2Int(0, -1)
//		};

//		[SerializeField]
//		protected AI.Resistance[] m_Resistances;
//		[SerializeField]
//		protected List<AI.AppliedElment> m_CurrentElements;
//		[SerializeField]
//		protected Def.LivingEntityType m_LEType;
//		//[SerializeField]
//		//protected GameAI m_AI;
//		[SerializeField]
//		protected int m_ID;
//		[SerializeField]
//		protected float m_TotalHealth;
//		[SerializeField]
//		protected float m_Health;
//		[SerializeField]
//		protected float m_Speed;
//		[SerializeField]
//		protected float m_AngularSpeed;
//		[SerializeField]
//		protected float m_Radius;
//		[SerializeField]
//		protected float m_Height;
//		[SerializeField]
//		protected float m_MaxJump;
//		[SerializeField]
//		protected float m_FlySpeed;
//		[SerializeField]
//		protected float m_FallSpeed;
//		[SerializeField]
//		protected IStruc m_Struc;

//		readonly Vector2[] m_CheckPositions = new Vector2[4];
//		readonly int[] m_NearMapIDs = new int[4];
//		protected static IStruc[] m_NearStrucs = new IStruc[32];

//		[SerializeField]
//		protected LivingEntity m_TargetLE;
//		[SerializeField]
//		protected Vector3 m_TargetPosition;
//		[SerializeField]
//		protected Quaternion m_TargetOrientation;

//		public abstract Collider GetCollider();

//		protected abstract void OnReceiveDamage();

//		protected abstract void OnReceiveElement();

//		void LEDefaultInit()
//		{
//			m_Resistances = new AI.Resistance[Def.ResistanceTypeCount];
//			for (int i = 0; i < m_Resistances.Length; ++i)
//			{
//				AI.Resistance res;
//				res.Type = (Def.ResistanceType)i;
//				res.Value = 0.5f;
//				m_Resistances[i] = res;
//			}

//			m_CurrentElements = new List<AI.AppliedElment>();
//			m_TotalHealth = 100f;
//			m_Health = m_TotalHealth;
//			m_TargetLE = null;
//			m_TargetPosition = Vector3.zero;
//			m_TargetOrientation = Quaternion.identity;
//			m_Speed = 2f;
//			m_AngularSpeed = 5f;
//			m_Radius = 0.25f;
//			m_Height = 1f;
//			m_MaxJump = 0.6f;
//			m_LEType = Def.LivingEntityType.COUNT;
//			m_FlySpeed = 2f;
//			m_FallSpeed = 1f;
//			m_ID = 0;
//		}

//		public void SetLivingEntity(LivingEntityDef entityDef)
//		{
//			if (m_Health == m_TotalHealth)
//				m_Health = entityDef.TotalHealth;
//			m_TotalHealth = entityDef.TotalHealth;
//			m_Speed = entityDef.Speed;
//			m_AngularSpeed = entityDef.AngularSpeed;
//			m_Radius = entityDef.Radius;
//			m_Height = entityDef.Height;
//			m_MaxJump = entityDef.MaxJump;
//			m_LEType = entityDef.LEType;
//			m_FlySpeed = entityDef.FlySpeed;
//			m_FallSpeed = entityDef.FallSpeed;
//		}

//		public LivingEntity()
//		{
//			LEDefaultInit();
//		}

//		public LivingEntity(LivingEntityDef entityDef)
//		{
//			LEDefaultInit();
//			SetLivingEntity(entityDef);
//		}
		
//		protected void SetResistance(AI.Resistance resistance)
//		{
//			m_Resistances[(int)resistance.Type] = resistance;
//		}

//		protected float ApplyResistances(Def.DamageType type, float amount)
//		{
//			var resistanceType = AI.Resistance.GetResistanceTypeFromDamageType(type);
//			if (resistanceType == Def.ResistanceType.COUNT)
//				return amount;
//			return amount * m_Resistances[(int)resistanceType].Value;
//		}

//		protected void SpawnElement(Def.ElementType type)
//		{
//			float nDuration = AI.ElementInfo.Duration[(int)type];
//			if (AI.ElementInfo.Stackable[(int)type])
//			{
//				AI.AppliedElment stacked;
//				stacked.Type = type;
//				stacked.Duration = nDuration;
//				stacked.NextApplyTime = Time.time + 0.75f;
//				m_CurrentElements.Add(stacked);
//			}
//			else
//			{
//				bool hasCurElement = false;
//				for(int i = 0; i < m_CurrentElements.Count; ++i)
//				{
//					if (m_CurrentElements[i].Type == type)
//					{
//						hasCurElement = true;
//						break;
//					}
//				}
//				if(!hasCurElement)
//				{
//					AI.AppliedElment elem;
//					elem.Type = type;
//					elem.Duration = nDuration;
//					elem.NextApplyTime = Time.time + 0.75f;
//					m_CurrentElements.Add(elem);
//				}
//			}
//			if (AI.ElementInfo.Renewable[(int)type])
//			{
//				for (int i = 0; i < m_CurrentElements.Count; ++i)
//				{
//					var curEl = m_CurrentElements[i];
//					if (curEl.Type != type)
//						continue;

//					curEl.Duration = nDuration;

//					m_CurrentElements[i] = curEl;
//				}
//			}
//		}

//		protected void ReceiveElementDamage(Def.ElementType type)
//		{
//			float damage = AI.ElementInfo.DamagePerSecond[(int)type];
//			var damageType = AI.ElementInfo.DmgType[(int)type];
//			damage = ApplyResistances(damageType, damage);
//			// Reapply
//			//var resisType = Resistance.GetResistanceTypeFromDamageType(damageType);
//			//if (resisType != Def.ResistanceTypeCount)
//			//{
//			//    float reapplyChance = m_Resistances[(int)resisType].Value * 0.25f;
//			//    var rng = (float)Manager.Mgr.DamageRNG.NextDouble();
//			//    if (rng < reapplyChance)
//			//    {
//			//        SpawnElement(type);
//			//    }
//			//}
//			m_Health -= damage;
//			OnReceiveElement();
//		}

//		public void ReceiveDamage(Def.DamageType type, float amount)
//		{
//			float damage = ApplyResistances(type, amount);

//			Def.ElementType nElement = Def.ElementType.COUNT;
//			switch(type)
//			{
//				case Def.DamageType.CUT:
//					nElement = Def.ElementType.BLEEDING;
//					break;
//				case Def.DamageType.FIRE:
//				case Def.DamageType.ELECTRICAL:
//					nElement = Def.ElementType.BURNING;
//					break;
//				case Def.DamageType.ICE:
//					nElement = Def.ElementType.FREEZING;
//					break;
//				case Def.DamageType.DEPRESSION:
//					nElement = Def.ElementType.CURSED;
//					break;
//				case Def.DamageType.POISON:
//					nElement = Def.ElementType.POISONED;
//					break;
//				case Def.DamageType.QUICKSILVER:
//					nElement = Def.ElementType.DISEASE;
//					break;
//			}
//			if(nElement != Def.ElementType.COUNT)
//			{
//				var resistanceType = AI.Resistance.GetResistanceTypeFromDamageType(type);
//				float chance = m_Resistances[(int)resistanceType].Value;
//				float rng = (float)Manager.Mgr.DamageRNG.NextDouble();
//				if(rng < chance)
//				{
//					SpawnElement(nElement);
//				}
//			}
//			m_Health -= damage;
//			OnReceiveDamage();
//		}

//		public void ReceiveHealingFixed(float amount)
//		{
//			m_Health += amount;
//		}

//		public void ReceiveHealingPct(float pct)
//		{
//			m_Health += m_TotalHealth * pct;
//		}

//		public float GetCurrentHealth()
//		{
//			return m_Health;
//		}

//		public float GetTotalHealth()
//		{
//			return m_TotalHealth;
//		}
			
//		protected void ElementUpdate()
//		{
//			for(int i = 0; i < m_CurrentElements.Count; )
//			{
//				var curelem = m_CurrentElements[i];
//				if(curelem.NextApplyTime > Time.time)
//				{
//					++i;
//					continue;
//				}
//				curelem.Duration -= 1.0f;
//				curelem.NextApplyTime = Time.time + 1.0f;
//				m_CurrentElements[i] = curelem;
//				ReceiveElementDamage(curelem.Type);
//				if(curelem.Duration <= 0.0f)
//				{
//					m_CurrentElements.RemoveAt(i);
//				}
//				else
//				{
//					++i;
//				}
//			}
//		}

//		//protected void AIUpdate()
//		//{
//		//    m_AI.Update();
//		//}

//		//public AIState GetCurrentAIState()
//		//{
//		//    return m_AI.GetCurrentState();
//		//}

//		public int GetLivingEntityID()
//		{
//			return m_ID;
//		}

//		public Def.LivingEntityType GetLEType()
//		{
//			return m_LEType;
//		}

//		public float GetSpeed()
//		{
//			return m_Speed;
//		}

//		public float GetRadius()
//		{
//			return m_Radius;
//		}

//		public float GetHeight()
//		{
//			return m_Height;
//		}

//		public float GetMaxJump()
//		{
//			return m_MaxJump;
//		}

//		public float GetFallSpeed()
//		{
//			return m_FallSpeed;
//		}

//		public float GetFlySpeed()
//		{
//			return m_FlySpeed;
//		}

//		public float GetAngularSpeed()
//		{
//			return m_AngularSpeed;
//		}

//		public LivingEntity GetTargetLE()
//		{
//			return m_TargetLE;
//		}

//		//public StructureComponent GetCurrentStruc()
//		//{
//		//    return m_Struc;
//		//}
		
//		public Vector3 GetTargetPosition()
//		{
//			return m_TargetPosition;
//		}

//		public Quaternion GetTargetOrientation()
//		{
//			return m_TargetOrientation;
//		}

//		public void OnControllerChange(IGameController controller)
//		{

//		}

//		float ObtainGround(Vector3 actualPos, float verticalRadius, float horizontalRadius,
//			out IBlock currentBlock, out BridgeComponent currentBridge)
//		{
//			float ground = 0f;

//			float baseGround = GameUtils.GetNearGround(actualPos, out currentBlock, out currentBridge, m_MaxJump);

//			float halfVertialRadius = verticalRadius * 0.5f;
//			float halfHorizontalRadius = horizontalRadius * 0.5f;
//			m_CheckPositions[0] = new Vector2(actualPos.x + halfVertialRadius, actualPos.z);
//			m_CheckPositions[1] = new Vector2(actualPos.x - halfVertialRadius, actualPos.z);
//			m_CheckPositions[2] = new Vector2(actualPos.x, actualPos.z + halfHorizontalRadius);
//			m_CheckPositions[3] = new Vector2(actualPos.x, actualPos.z - halfHorizontalRadius);

//			bool nearGroundFound = false;
//			foreach (var posCheck in m_CheckPositions)
//			{
//				var nearGround = GameUtils.GetNearGround(new Vector3(posCheck.x, actualPos.y, posCheck.y), out IBlock b, out BridgeComponent bc, m_MaxJump);
//				if (ground < nearGround && (nearGround - baseGround) <= m_MaxJump)
//				{
//					ground = nearGround;
//					nearGroundFound = true;
//				}
//			}

//			return (nearGroundFound) ?
//				((baseGround + ground) * 0.5f) :
//				(baseGround);
//		}

//		bool OnGround(Vector3 actualPos, float ground, out float movementY, float deltaTime)
//		{
//			// Below ground ?
//			if (actualPos.y < ground)
//			{
//				movementY = GameUtils.LinearMovement1D(actualPos.y, ground, m_FlySpeed * deltaTime);
//				return false;
//			}
//			// Above ground ?
//			else if (actualPos.y > ground)
//			{
//				// Y movement (Gravity)
//				movementY = GameUtils.LinearMovement1D(actualPos.y, ground, m_FallSpeed * deltaTime);
//				return false;
//			}
//			movementY = 0f;
//			return true;
//		}

//		void NextPosCheck(Vector3 actualPos, ref Vector2 nextPos, ref Vector2 movementXZ, float ground, 
//			bool avoidVoid)
//		{
//			bool canMove = GameUtils.CanGoThere(ground, nextPos, m_MaxJump, avoidVoid);
//			if (!canMove)
//			{
//				nextPos.Set(actualPos.x + movementXZ.x, actualPos.z);
//				canMove = GameUtils.CanGoThere(ground, nextPos, m_MaxJump, avoidVoid);
//				if (!canMove)
//				{
//					nextPos.Set(actualPos.x, actualPos.z + movementXZ.y);
//					canMove = GameUtils.CanGoThere(ground, nextPos, m_MaxJump, avoidVoid);
//					if (!canMove)
//					{
//						movementXZ = Vector2.zero;
//						nextPos.Set(actualPos.x, actualPos.z);
//					}
//					else
//					{
//						movementXZ.Set(0.0f, movementXZ.y);
//					}
//				}
//				else
//				{
//					movementXZ.Set(movementXZ.x, 0.0f);
//				}
//			}
//		}

//		void BoundaryCheck(long curMapID, Vector3 nextPos, float ground, float verticalRadius, 
//			float horizontalRadius, ref Vector2 movementXZ)
//		{
//			var currentMapPosition = GameUtils.PosFromMapID(curMapID);
//			m_NearMapIDs[0] = GameUtils.IDFromPos(new Vector2Int(currentMapPosition.x + 1, currentMapPosition.y), Manager.MapWidth, Manager.MapHeight); // bot
//			m_NearMapIDs[1] = GameUtils.IDFromPos(new Vector2Int(currentMapPosition.x, currentMapPosition.y + 1), Manager.MapWidth, Manager.MapHeight); // left
//			m_NearMapIDs[2] = GameUtils.IDFromPos(new Vector2Int(currentMapPosition.x - 1, currentMapPosition.y), Manager.MapWidth, Manager.MapHeight); // top
//			m_NearMapIDs[3] = GameUtils.IDFromPos(new Vector2Int(currentMapPosition.x, currentMapPosition.y - 1), Manager.MapWidth, Manager.MapHeight); // right

//			for (int i = 0; i < m_NearMapIDs.Length; ++i)
//			{
//				var pilar = Manager.Mgr.MapQT.GetPilarWithMapID(m_NearMapIDs[i]);
//				if (pilar == null || pilar.GetBlocks().Count == 0)
//					continue;
//				var block = (CBlock)pilar.GetBlocks()[pilar.GetBlocks().Count - 1];
//				//if (block.GetLayer() == 0)
//				//    continue;
//				var height = block.GetHeight() + block.GetMicroHeight();
//				if (block.GetBlockType() == Def.BlockType.STAIRS)
//				{
//					height += GameUtils.GetStairYOffset(block.transform.position, block.GetRotation(), nextPos);
//				}
//				if (height < (ground + m_MaxJump))
//					continue;

//				var nextPilarPos = pilar.transform.position;

//				Rect pilarRect = new Rect(nextPilarPos.x - 0.1f, nextPilarPos.z - 0.1f, 1.2f, 1.2f);

//				float top = nextPilarPos.x - 0.1f;
//				float bottom = top + 1.2f;
//				float left = nextPilarPos.z - 0.1f;
//				float right = left + 1.2f;

//				// Cylinder collision
//				//  -   X Collision
//				if ((movementXZ.x > 0f && DirCheck[i].x > 0) || (movementXZ.x < 0f && DirCheck[i].x < 0))
//				{
//					var rad = verticalRadius * DirCheck[i].x;
//					Vector2 pointA = new Vector2(nextPos.x, nextPos.z);
//					Vector2 pointB = new Vector2(nextPos.x + rad, nextPos.z);
//					if (pilarRect.Contains(pointA) || pilarRect.Contains(pointB))
//					{
//						movementXZ.x = 0f;
//					}
//				}
//				//  -   Z Collision
//				if ((movementXZ.y > 0f && DirCheck[i].y > 0) || (movementXZ.y < 0f && DirCheck[i].y < 0))
//				{
//					var rad = horizontalRadius * DirCheck[i].y;
//					Vector2 pointA = new Vector2(nextPos.x, nextPos.z);
//					Vector2 pointB = new Vector2(nextPos.x, nextPos.z + rad);
//					if (pilarRect.Contains(pointA) || pilarRect.Contains(pointB))
//					{
//						movementXZ.y = 0f;
//					}
//				}
//			}
//		}

//		void MonsterBoundaryCheck(Vector2 nextPos, ref Vector2 movementXZ, float ground)
//		{
//			////Manager.GetStrucsNoAlloc(ref m_NearStrucs, nextPos);
//			//for (int i = 0; i < m_NearStrucs.Length; ++i)
//			//{
//			//	var nearStruc = m_NearStrucs[i];
//			//	if (nearStruc == null)
//			//		break;
//			//	for(int j = 0; j < nearStruc.GetLivingEntities().Count; ++j)
//			//	{
//			//		var le = nearStruc.GetLivingEntities()[j];
//			//		if (le == this || le.GetLEType() == Def.LivingEntityType.Prop || le.GetCurrentHealth() <= 0f)
//			//			continue;
//			//		
//			//		var dist = Vector2.Distance(new Vector2(le.transform.position.x, le.transform.position.z), nextPos);
//			//		if(dist < 0.25f)
//			//		{
//			//			var diff = new Vector2(le.transform.position.x, le.transform.position.z) - nextPos;
//			//			if(diff.x > 0f && movementXZ.x > 0f)
//			//			{
//			//				if (diff.x < 0.25f)
//			//				{
//			//					var mult = diff.x * 4f;
//			//					movementXZ.x *= mult;
//			//				}
//			//			}
//			//			else if(diff.x < 0f && movementXZ.x < 0f)
//			//			{
//			//				if (diff.x > -0.25f)
//			//				{
//			//					var mult = diff.x * -4f;
//			//					movementXZ.x *= mult;
//			//				}
//			//			}
//			//			if (diff.y > 0f && movementXZ.y > 0f)
//			//			{
//			//				if (diff.y < 0.25f)
//			//				{
//			//					var mult = diff.y * 4f;
//			//					movementXZ.y *= mult;
//			//				}
//			//			}
//			//			else if (diff.y < 0f && movementXZ.y < 0f)
//			//			{
//			//				if (diff.y > -0.25f)
//			//				{
//			//					var mult = diff.y * -4f;
//			//					movementXZ.y *= mult;
//			//				}
//			//			}
//			//		}
//			//	}
//			//}
//		}

//		public bool UpdateMovement(bool avoidVoid, float yPos, float deltaTime, out Vector3 movement,
//			out IBlock currentBlock, out BridgeComponent currentBridge)
//		{
//			movement = Vector3.zero;
//			currentBlock = null;
//			currentBridge = null;
//			var posXZ = new Vector2(transform.position.x, transform.position.z);
//			var actualPos = new Vector3(posXZ.x, yPos, posXZ.y);
//			var targetXZ = new Vector2(m_TargetPosition.x, m_TargetPosition.z);
//			var movementXZ = GameUtils.LinearMovement2D(posXZ, targetXZ, m_Speed * deltaTime);
//			var curMapID = GameUtils.MapIDFromPos(GameUtils.TransformPosition(posXZ));

//			if (curMapID < 0)
//				return true;

//			var horizontalRadius = m_Radius;
//			var verticalRadius = m_Radius;
//			if (m_Radius > 0.5f)
//				verticalRadius *= 0.5f;

//			float ground = ObtainGround(actualPos, verticalRadius, horizontalRadius,
//				out currentBlock, out currentBridge);

//			bool onGround = OnGround(actualPos, ground, out float movementY, deltaTime);

//			// Can we go to the next position?
//			var nextPos = posXZ + movementXZ;
//			var nextMapID = GameUtils.MapIDFromPos(GameUtils.TransformPosition(nextPos));
//			if (curMapID != nextMapID)
//			{
//				NextPosCheck(actualPos, ref nextPos, ref movementXZ, ground, avoidVoid);
//			}

//			// Height check
//			if (currentBridge == null)
//			{
//				BoundaryCheck(curMapID, new Vector3(nextPos.x, actualPos.y, nextPos.y), ground, 
//					verticalRadius, horizontalRadius, ref movementXZ);
//			}

//			if(movementXZ != Vector2.zero)
//			{
//				nextPos = posXZ + movementXZ;
//				MonsterBoundaryCheck(nextPos, ref movementXZ, ground);
//			}

//			movement.Set(movementXZ.x, movementY, movementXZ.y);

//			return onGround;
//		}

//		public void RegisterLE(bool hasMoved)
//		{
//			//if(m_Struc == null || hasMoved)
//			//{
//			//	//Manager.GetStrucsNoAlloc(ref m_NearStrucs, new Vector2(transform.position.x, transform.position.z));
//			//	bool strucFound = false;
//			//	for(int i = 0; i < m_NearStrucs.Length; ++i)
//			//	{
//			//		var nearStruc = m_NearStrucs[i];
//			//		if (nearStruc == null)
//			//			break;
//			//
//			//		strucFound = true;
//			//		if (m_Struc != nearStruc)
//			//		{
//			//			if (m_Struc != null && m_Struc.GetLivingEntities().Contains(this))
//			//				m_Struc.GetLivingEntities().Remove(this);
//			//			m_Struc = nearStruc;
//			//			if(!m_Struc.GetLivingEntities().Contains(this))
//			//				m_Struc.GetLivingEntities().Add(this);
//			//			break;
//			//		}
//			//	}
//			//	if(!strucFound)
//			//	{
//			//		if(m_Struc != null && m_Struc.GetLivingEntities().Contains(this))
//			//		{
//			//			m_Struc.GetLivingEntities().Remove(this);
//			//		}
//			//		m_Struc = null;
//			//	}
//			//}
//		}

//		public Vector3 _TargetPos
//		{
//			set
//			{
//				m_TargetPosition = value;
//			}
//		}
//		public Quaternion _TargetRot
//		{
//			set
//			{
//				m_TargetOrientation = value;
//			}
//		}
//		public float _Speed
//		{
//			set
//			{
//				m_Speed = value;
//			}
//		}
//		public float _AngularSpeed
//		{
//			set
//			{
//				m_AngularSpeed = value;
//			}
//		}
//		public LivingEntity _TargetLE
//		{
//			set
//			{
//				m_TargetLE = value;
//			}
//		}
//		public float _Health
//		{
//			set
//			{
//				m_Health = value;
//				m_Health = Mathf.Min(m_Health, m_TotalHealth);
//			}
//		}
//		public float _Radius
//		{
//			set
//			{
//				m_Radius = value;
//			}
//		}
//		public float _MaxJump
//		{
//			set
//			{
//				m_MaxJump = value;
//			}
//		}
//		public void _ElmUpdate()
//		{
//			ElementUpdate();
//		}
//		public float _Height
//		{
//			set
//			{
//				m_Height = value;
//			}
//		}
//		public float _FallSpeed
//		{
//			set
//			{
//				m_FallSpeed = value;
//			}
//		}
//		public float _FlySpeed
//		{
//			set
//			{
//				m_FlySpeed = value;
//			}
//		}
//	}
//}
