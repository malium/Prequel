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
//using Assets.AI;

//namespace Assets
//{
//	//public enum MonsterAIType
//	//{
//	//	NO_AI, // Static without behaviour
//	//	ROAMING_AI, // It just roams around
//	//	AGRESSIVE_AI, // Roams until find an enemy
//	//	SCARED_AI, // It just roams, but if it finds an enemy scapes

//	//	COUNT
//	//}

//	//[Serializable]
//	//public struct EnemySeen
//	//{
//	//	public LivingEntity Enemy;
//	//	public float Distance;
//	//	public float SeenTime;
//	//	public bool heard;
//	//}

//	public abstract class MonsterScript : LivingEntity
//	{
//	//	readonly static float[] FrameChangeWait = new float[(int)AIState.COUNT]
//	//	{ 0.5f, 0.5f, 0.4f, 0.3f, 0.25f, 0.4f, 20000.0f };
//	//	const float TargetChangeWait = 5.0f;
//	//	const float InPainAmount = 0.25f;
//	//	public static int MonsterID = 0;
		
//		//protected SpriteRenderer m_ShadowSR;
//		//protected SpriteRenderer m_SpriteSR;
//		protected SpriteBackendSprite m_Shadow;
//		protected SpriteBackendSQuad m_Sprite;
//	//	//[SerializeField]
//	//	//protected BoxCollider m_SpriteBC;
//	//	protected CapsuleCollider m_SpriteCC;
//	//	protected List<EnemySeen> m_SeenEnemies;
//		protected List<LivingEntity> m_EnemyList;
//	//	protected MonsterAlertComponent m_AlertSign;

//	//	//public GameObject TargetGO;
//	//	//public StructureComponent Struc;
//		protected MonsterFamily m_Info;
//		//	//[SerializeField]
//		//	//protected MonsterSprite m_Monster;
//		//	//[SerializeField]
//		//	//protected MonsterAttributes m_Attribs;
//		//	protected SpriteFacing m_Facing;
//			protected GameAI m_AI;
//		//	float m_SpriteYOffset;
//		//	int m_Frame;
//		//	int m_LastIDX;
//			Vector2 m_Direction;
//			public Vector2 Direction
//			{
//				get
//				{
//					return m_Direction;
//				}
//				set
//				{
//					m_Direction = value;
//				}
//			}
//		//	float m_NextFrameChange;
//		//	float m_NextTargetChange;
//			float m_NextAttackTime;
//			public float _NextAttackTime
//			{
//				get
//				{
//					return m_NextAttackTime;
//				}
//				set
//				{
//					m_NextAttackTime = value;
//				}
//			}
//		//	float m_LastReceivedDamageTime;
//		//	static VFXDef[] m_BloodTrails;
//		//	static float[] m_BloodTrailScales;
//		//	//Vector3 TargetOffset = Vector3.zero;

//			public SpriteRenderer ShadowSR
//			{
//				get
//				{
//					return m_Shadow.GetRenderer() as SpriteRenderer;
//				}
//			}
//			public Renderer SpriteSR
//			{
//				get
//				{
//					return m_Sprite.GetRenderer();
//				}
//			}
//		//	//public BoxCollider SpriteBC
//		//	//{
//		//	//    get
//		//	//    {
//		//	//        return m_SpriteBC;
//		//	//    }
//		//	//}
//		//	public CapsuleCollider SpriteCC
//		//	{
//		//		get
//		//		{
//		//			return m_SpriteCC;
//		//		}
//		//	}
//			public MonsterFamily Info
//			{
//				get
//				{
//					return m_Info;
//				}
//			}
//		//	//public MonsterSprite Monster
//		//	//{
//		//	//    get
//		//	//    {
//		//	//        return m_Monster;
//		//	//    }
//		//	//}
//		//	//public MonsterAttributes Attributes
//		//	//{
//		//	//    get
//		//	//    {
//		//	//        return m_Attribs;
//		//	//    }
//		//	//    set
//		//	//    {
//		//	//        m_Attribs = value;
//		//	//    }
//		//	//}
//		//	public SpriteFacing Facing
//		//	{
//		//		get
//		//		{
//		//			return m_Facing;
//		//		}
//		//		set
//		//		{
//		//			var facing = value;
//		//			if (m_Facing == facing)
//		//				return;
//		//			if ((m_Facing.Horizontal == SpriteHorizontal.LEFT && facing.Horizontal == SpriteHorizontal.RIGHT)
//		//				|| (m_Facing.Horizontal == SpriteHorizontal.RIGHT && facing.Horizontal == SpriteHorizontal.LEFT))
//		//			{
//		//				m_Sprite.Flip(!m_Sprite.IsHorizontalFlip(), m_Sprite.IsVerticalFlip());
//		//				//m_SpriteSR.flipX = !m_SpriteSR.flipX;
//		//				m_Facing.Horizontal = facing.Horizontal;
//		//				//m_SpriteSR.material.SetVector(AssetContainer.FlipShaderID, new Vector4(m_SpriteSR.flipX ? -1f : 1f, m_SpriteSR.flipY ? -1f : 1f, 1f, 1f));
//		//			}
//		//			else
//		//			{
//		//				m_Facing = facing;
//		//			}
//		//		}
//		//	}
//			public GameAI AI
//			{
//				get
//				{
//					return m_AI;
//				}
//			}

//		//	int GetCurrentIndex()
//		//	{
//		//		int idx = ((int)m_Facing.Vertical) * 2;
//		//		idx += m_Frame;
//		//		return idx;
//		//	}
//		//	Sprite GetCurrentSprite()
//		//	{
//		//		var idx = GetCurrentIndex();
//		//		return m_Info.Frames[idx];
//		//	}
//		//	float GetCurrentYOffset()
//		//	{
//		//		//var pivot = m_SpriteSR.sprite.pivot;
//		//		//var idx = GetCurrentIndex();
//		//		//var lastPixel = m_Info.LastPixel[idx];
//		//		//var yOffset = pivot.y - lastPixel.y;
//		//		//yOffset /= m_SpriteSR.sprite.texture.height;
//		//		//yOffset *= m_SpriteSR.size.y * 0.5f;
//		//		//return yOffset;
//		//		return 0f;
//		//	}

//		//	public MonsterScript()
//		//		:base()
//		//	{

//		//	}

//		//	public void OnEnemySeen()
//		//	{

//		//	}

//		//	public AIState RoamingComputeNextPacific()
//		//	{
//		//		m_SeenEnemies.Clear();

//		//		float targetDistance = Vector3.Distance(m_TargetPosition, transform.position);
//		//		if (targetDistance > 0.01f)
//		//			return AIState.ROAMING;
//		//		m_NextTargetChange = Time.time + ((float)Manager.Mgr.SpawnRNG.NextDouble()) * 2.0f;
//		//		return AIState.IDLE;
//		//	}

//		//	public AIState RoamingComputeNextAggresive()
//		//	{
//		//		float minDist = float.MaxValue;
//		//		float minDistSeen = float.MaxValue;
//		//		EnemySeen enemy = new EnemySeen();
//		//		EnemySeen enemySeen = new EnemySeen();
//		//		for (int i = 0; i < m_SeenEnemies.Count; ++i)
//		//		{
//		//			var curEnemy = m_SeenEnemies[i];
//		//			if (curEnemy.Distance < minDist)
//		//			{
//		//				enemy = curEnemy;
//		//				minDist = curEnemy.Distance;
//		//			}
//		//			if(!curEnemy.heard && minDistSeen < curEnemy.Distance)
//		//			{
//		//				enemySeen = curEnemy;
//		//				minDistSeen = curEnemy.Distance;
//		//			}
//		//		}
//		//		m_SeenEnemies.Clear();
//		//		if (minDistSeen != float.MaxValue)
//		//		{
//		//			m_TargetLE = enemySeen.Enemy;
//		//			OnEnemySeen();
//		//			return AIState.ALERT;
//		//		}
//		//		if(minDist != float.MaxValue)
//		//		{
//		//			m_TargetLE = enemy.Enemy;
//		//			return AIState.AWARE;
//		//		}

//		//		float targetDistance = Vector3.Distance(m_TargetPosition, transform.position);
//		//		if (targetDistance > 0.01f)
//		//			return AIState.ROAMING;
//		//		m_NextTargetChange = Time.time + ((float)Manager.Mgr.SpawnRNG.NextDouble()) * 2.0f;
//		//		return AIState.IDLE;
//		//	}

//		//	public AIState RoamingComputeNextScared()
//		//	{
//		//		float minDist = float.MaxValue;
//		//		EnemySeen enemy = new EnemySeen();
//		//		for(int i = 0; i < m_SeenEnemies.Count; ++i)
//		//		{
//		//			if(m_SeenEnemies[i].Distance < minDist)
//		//			{
//		//				enemy = m_SeenEnemies[i];
//		//				minDist = enemy.Distance;
//		//			}
//		//		}
//		//		m_SeenEnemies.Clear();
//		//		if(minDist > 2.0f)
//		//		{
//		//			float targetDistance = Vector3.Distance(m_TargetPosition, transform.position);
//		//			if (targetDistance > 0.01f)
//		//				return AIState.ROAMING;
//		//			m_NextTargetChange = Time.time + ((float)Manager.Mgr.SpawnRNG.NextDouble()) * 2.0f;
//		//			return AIState.IDLE;
//		//		}

//		//		m_TargetLE = enemy.Enemy;
//		//		return AIState.FLEE;
//		//	}

//		//	void AddSeenEnemy(EnemySeen enemy)
//		//	{
//		//		bool enemyUpdated = false;
//		//		for(int i = 0; i < m_SeenEnemies.Count; ++i)
//		//		{
//		//			var curEnemy = m_SeenEnemies[i];
//		//			if (curEnemy.Enemy != enemy.Enemy)
//		//				continue;
//		//			curEnemy.Distance = enemy.Distance;
//		//			if (!enemy.heard)
//		//				curEnemy.heard = false;
//		//			curEnemy.SeenTime = enemy.SeenTime;
//		//			enemyUpdated = true;
//		//		}
//		//		if(!enemyUpdated)
//		//		{
//		//			m_SeenEnemies.Add(enemy);
//		//		}
//		//	}

//		//	void CheckEnemies()
//		//	{
//		//		var curPos = new Vector2(transform.position.x, transform.position.z);
//		//		Vector2 curDir = Vector2.zero;
//		//		if (m_TargetPosition == transform.position)
//		//			curDir.Set(0.0f, m_Facing.Vertical == SpriteVertical.UP ? -1.0f : 1.0f);
//		//		else
//		//			curDir = (new Vector2(m_TargetPosition.x, m_TargetPosition.z) - curPos).normalized;

//		//		////var odd = Manager.Mgr.OddScript;
//		//		////if (odd.GetCurrentHealth() > 0.0f)
//		//		////{
//		//		////    var oddPos = new Vector2(odd.transform.position.x, odd.transform.position.z);
//		//		////    var oddDistance = Vector2.Distance(curPos, oddPos);

//		//		////    bool oddFound = false;
//		//		////    if (oddDistance <= m_Info.SightRange)
//		//		////    {
//		//		////        var oddDirection = (oddPos - curPos).normalized;
//		//		////        var oddCosDot = Vector2.Dot(curDir, oddDirection);
//		//		////        var oddAngle = Mathf.Abs(Mathf.Acos(oddCosDot) * Mathf.Rad2Deg);
//		//		////        if (oddAngle <= m_Info.SightAngle)
//		//		////        {
//		//		////            oddFound = true;
//		//		////            EnemySeen enemy;
//		//		////            enemy.Distance = oddDistance;
//		//		////            enemy.Enemy = odd;
//		//		////            enemy.heard = false;
//		//		////            enemy.SeenTime = Time.time;
//		//		////            AddSeenEnemy(enemy);
//		//		////        }
//		//		////    }
//		//		////    if (oddDistance <= m_Info.HearingRange && !oddFound)
//		//		////    {
//		//		////        EnemySeen enemy;
//		//		////        enemy.Distance = oddDistance;
//		//		////        enemy.Enemy = odd;
//		//		////        enemy.heard = true;
//		//		////        enemy.SeenTime = Time.time;
//		//		////        AddSeenEnemy(enemy);
//		//		////    }
//		//		////}
//		//		//for (int i = 0; i < Manager.Mgr.Strucs.Count; ++i)
//		//		//{
//		//		//    var curStruc = Manager.Mgr.Strucs[i];
//		//		//    for (int j = 0; j < curStruc.LivingEntities.Count; ++j)
//		//		//    {
//		//		//        var curLE = curStruc.LivingEntities[j];
//		//		//        if ((curLE.GetLEType() != Def.LivingEntityType.Monster && curLE.GetLEType() != Def.LivingEntityType.ODD) || curLE.GetCurrentHealth() <= 0f)
//		//		//            continue;
//		//		//        var lePos = new Vector2(curLE.transform.position.x, curLE.transform.position.z);
//		//		//        var leDist = Vector2.Distance(lePos, curPos);
//		//		//        if(m_EnemyList.Contains(curLE))
//		//		//        {
//		//		//            if (leDist <= m_Info.SightRange)
//		//		//            {
//		//		//                var leDirection = (lePos - curPos).normalized;
//		//		//                var leCosDot = Vector2.Dot(curDir, leDirection);
//		//		//                var leAngle = Mathf.Abs(Mathf.Acos(leCosDot) * Mathf.Rad2Deg);
//		//		//                if (leAngle <= m_Info.SightAngle)
//		//		//                {
//		//		//                    EnemySeen enemy;
//		//		//                    enemy.Distance = leDist;
//		//		//                    enemy.Enemy = curLE;
//		//		//                    enemy.heard = false;
//		//		//                    enemy.SeenTime = Time.time;
//		//		//                    AddSeenEnemy(enemy);
//		//		//                    continue;
//		//		//                }
//		//		//            }
//		//		//            if (leDist <= m_Info.HearingRange)
//		//		//            {
//		//		//                EnemySeen enemy;
//		//		//                enemy.Distance = leDist;
//		//		//                enemy.Enemy = curLE;
//		//		//                enemy.heard = true;
//		//		//                enemy.SeenTime = Time.time;
//		//		//                AddSeenEnemy(enemy);
//		//		//                continue;
//		//		//            }
//		//		//        }
//		//		//        if (curLE.GetLEType() == Def.LivingEntityType.Monster)
//		//		//        {
//		//		//            var curMon = (MonsterScript)curLE;
//		//		//            if (m_Info.Team == curMon.m_Info.Team)
//		//		//                continue;
//		//		//        }
//		//		//        if (leDist <= m_Info.SightRange)
//		//		//        {
//		//		//            var leDirection = (lePos - curPos).normalized;
//		//		//            var leCosDot = Vector2.Dot(curDir, leDirection);
//		//		//            var leAngle = Mathf.Abs(Mathf.Acos(leCosDot) * Mathf.Rad2Deg);
//		//		//            if (leAngle <= m_Info.SightAngle)
//		//		//            {
//		//		//                EnemySeen enemy;
//		//		//                enemy.Distance = leDist;
//		//		//                enemy.Enemy = curLE;
//		//		//                enemy.heard = false;
//		//		//                enemy.SeenTime = Time.time;
//		//		//                AddSeenEnemy(enemy);
//		//		//                continue;
//		//		//            }
//		//		//        }

//		//		//        if (leDist <= m_Info.HearingRange)
//		//		//        {
//		//		//            EnemySeen enemy;
//		//		//            enemy.Distance = leDist;
//		//		//            enemy.Enemy = curLE;
//		//		//            enemy.heard = true;
//		//		//            enemy.SeenTime = Time.time;
//		//		//            AddSeenEnemy(enemy);
//		//		//            continue;
//		//		//        }
//		//		//    }
//		//		//}
//		//	}

//			public override Collider GetCollider()
//			{
//			//		return m_SpriteCC;
//			return null;
//			}
//			public void _OnReceiveDamageUpdate()
//			{
//		//		if ((m_LastReceivedDamageTime + InPainAmount) > Time.time)
//		//		{
//		//			m_Sprite.SetColor(new Color(1f, 0f, 0f, m_Sprite.GetColor().a));
//		//			//m_SpriteSR.material.color = new Color(1.0f, 0.0f, 0.0f, m_SpriteSR.material.color.a);
//		//		}
//		//		else
//		//		{
//		//			m_Sprite.SetColor(new Color(1f, 1f, 1f, m_Sprite.GetColor().a));
//		//			//m_SpriteSR.material.color = new Color(1.0f, 1.0f, 1.0f, m_SpriteSR.material.color.a);
//		//		}
//			}

//		//	void CheckHealth()
//		//	{
//		//		if (m_Health <= 0)
//		//		{
//		//			//if (Manager.Mgr.CurrentControllerSel == (int)GameState.PLAY)
//		//			//{
//		//			//	var pilarID = GameUtils.MapIDFromPos(GameUtils.TransformPosition(new Vector2(transform.position.x, transform.position.z)));
//		//			//	Manager.Mgr.Map.Record(
//		//			//		new MapCommand(MapCommandType.MONSTER_DEATH, m_Info.Name, pilarID, -1));
//		//			//}
//		//			if (m_Struc != null && m_Struc.GetLivingEntities().Contains(this))
//		//				m_Struc.GetLivingEntities().Remove(this);
//		//			m_SpriteCC.enabled = false;
//		//			m_AI.ChangeStateTo(AIState.DEAD);
//		//			var cutting = Instantiate(AssetContainer.Mgr.TriVFXGameObjects[1]).GetComponent<CuttingPlaneVFX>();
//		//			cutting.gameObject.SetActive(true);
//		//			cutting.Set(m_Info, GetCurrentIndex(), transform.position, m_SpriteYOffset);

//		//			GameUtils.DeleteGameObjectAndItsChilds(gameObject, false);
//		//		}
//		//	}

//			protected override void OnReceiveDamage()
//			{
//		//		m_LastReceivedDamageTime = Time.time;
//		//		CheckHealth();
//			}

//			protected override void OnReceiveElement()
//			{
//		//		CheckHealth();
//			}

//		//	void SetLookingDirection()
//		//	{
//		//		var dir = new Vector2(m_TargetPosition.x, m_TargetPosition.z) - new Vector2(transform.position.x, transform.position.z);
//		//		dir.Normalize();
//		//		if (dir != Vector2.zero)
//		//			m_Direction = dir;
//		//	}

//		//	public void RoamingState()
//		//	{
//		//		CheckEnemies();

//		//		if (m_NextTargetChange >= Time.time)
//		//			return;
//		//		var extraTime = Time.time - m_NextTargetChange;
//		//		float targetDistance = Vector3.Distance(m_TargetPosition, transform.position);
//		//		if (targetDistance > 0.1f && extraTime < 3.0f)
//		//			return;

//		//		m_NextTargetChange = Time.time + TargetChangeWait;

//		//		void SetRandomTarget()
//		//		{
//		//			var dir = new Vector2(UnityEngine.Random.value * 2f - 1f, UnityEngine.Random.value * 2f - 1f).normalized;
//		//			if (dir == Vector2.zero)
//		//				dir = new Vector2(1f, 0f);

//		//			var distance = UnityEngine.Random.value * 4f;
//		//			var posOffset = dir * distance;
//		//			m_TargetPosition = new Vector3(transform.position.x + posOffset.x, m_TargetPosition.y, transform.position.z + posOffset.y);
//		//			SetLookingDirection();
//		//		}

//		//		if(m_Struc == null)
//		//		{
//		//			SetRandomTarget();
//		//			return;
//		//		}

//		//		CPilar randomPilar = null;
//		//		int times = 0;
//		//		while (randomPilar == null && times < m_Struc.GetPilars().Length)
//		//		{
//		//			randomPilar = m_Struc.GetPilars()[Manager.Mgr.SpawnRNG.Next(0, m_Struc.GetPilars().Length)];
//		//			++times;
//		//		}
//		//		if (randomPilar == null)
//		//		{
//		//			SetRandomTarget();
//		//			return;
//		//		}

//		//		var randomBlock = randomPilar.GetBlocks()[randomPilar.GetBlocks().Count - 1];
//		//		Vector3 blockOffset = new Vector3((float)Manager.Mgr.SpawnRNG.NextDouble(), 0.0f, (float)Manager.Mgr.SpawnRNG.NextDouble());
//		//		m_TargetPosition = new Vector3(randomPilar.transform.position.x, randomBlock.GetHeight() + randomBlock.GetMicroHeight(), randomPilar.transform.position.z) + blockOffset;
//		//		SetLookingDirection();
//		//	}

//		//	//public void _RoamingState()
//		//	//{
//		//	//    if (Struc == null)
//		//	//        return;// AIState.DEAD;

//		//	//    var curPos = new Vector2(transform.position.x, transform.position.z);
//		//	//    Vector2 curDir = Vector2.zero;
//		//	//    if (m_TargetPosition == transform.position)
//		//	//        curDir.Set(0.0f, m_Facing.Vertical == SpriteVertical.UP ? -1.0f : 1.0f);
//		//	//    else
//		//	//        curDir = (new Vector2(m_TargetPosition.x, m_TargetPosition.z) - curPos).normalized;

//		//	//    var odd = Manager.Mgr.OddGO;
//		//	//    var oddPos = new Vector2(odd.transform.position.x, odd.transform.position.z);
//		//	//    var oddDistance = Vector2.Distance(curPos, oddPos);

//		//	//    if(oddDistance <= m_Attribs.SightRange)
//		//	//    {
//		//	//        var oddDirection = (oddPos - curPos).normalized;
//		//	//        var oddCosDot = Vector2.Dot(curDir, oddDirection);
//		//	//        var oddAngle = Mathf.Abs(Mathf.Acos(oddCosDot) * Mathf.Rad2Deg);
//		//	//        if (oddAngle <= m_Attribs.SightAngle)
//		//	//        {
//		//	//            m_TargetLE = odd.GetComponent<LivingEntity>();
//		//	//            var targetPos = oddDirection * (oddDistance - m_Attribs.AttackRange);
//		//	//            m_TargetPosition.Set(targetPos.x, 0.0f, targetPos.y);
//		//	//            OnEnemySeen();
//		//	//            return; // AIState.ALERT;
//		//	//        }
//		//	//    }
//		//	//    if (oddDistance <= m_Attribs.HearingRange)
//		//	//    {
//		//	//        m_TargetPosition = odd.transform.position;
//		//	//        m_TargetLE = null;
//		//	//        return; // AIState.AWARE;
//		//	//    }

//		//	//    for(int i = 0; i < Manager.Mgr.Strucs.Count; ++i)
//		//	//    {
//		//	//        var curStruc = Manager.Mgr.Strucs[i];
//		//	//        for(int j = 0; j < curStruc.LivingEntities.Count; ++j)
//		//	//        {
//		//	//            var curLE = curStruc.LivingEntities[j];
//		//	//            if (curLE.GetLEType() != Def.LivingEntityType.Monster)
//		//	//                continue;
//		//	//            var curMon = (MonsterScript)curLE;
//		//	//            if (m_Attribs.Team == curMon.m_Attribs.Team)
//		//	//                continue;
//		//	//            var lePos = new Vector2(curLE.transform.position.x, curLE.transform.position.z);
//		//	//            var leDist = Vector2.Distance(lePos, curPos);
//		//	//            if(leDist <= m_Attribs.SightRange)
//		//	//            {
//		//	//                var leDirection = (lePos - curPos).normalized;
//		//	//                var leCosDot = Vector2.Dot(curDir, leDirection);
//		//	//                var leAngle = Mathf.Abs(Mathf.Acos(leCosDot) * Mathf.Rad2Deg);
//		//	//                if (leAngle <= m_Attribs.SightAngle)
//		//	//                {
//		//	//                    m_TargetLE = curLE;
//		//	//                    var targetPos = leDirection * (leDist - m_Attribs.AttackRange);
//		//	//                    m_TargetPosition.Set(targetPos.x, 0.0f, targetPos.y);
//		//	//                    OnEnemySeen();
//		//	//                    return; //AIState.ALERT;
//		//	//                }
//		//	//            }

//		//	//            if(leDist <= m_Attribs.HearingRange)
//		//	//            {
//		//	//                m_TargetLE = null;
//		//	//                m_TargetPosition.Set(lePos.x, 0.0f, lePos.y);
//		//	//                return; //AIState.AWARE;
//		//	//            }
//		//	//        }
//		//	//    }

//		//	//    if (m_LastTargetChange < Time.time)
//		//	//    {
//		//	//        m_LastTargetChange = Time.time + TargetChangeWait;

//		//	//        PilarComponent randomPilar = null;
//		//	//        int times = 0;
//		//	//        while (randomPilar == null && times < Struc.Pilars.Length)
//		//	//        {
//		//	//            randomPilar = Struc.Pilars[Manager.Mgr.BuildRNG.Next(0, Struc.Pilars.Length)];
//		//	//            ++times;
//		//	//        }
//		//	//        if (randomPilar == null)
//		//	//            return; //AIState.IDLE;

//		//	//        var randomBlock = randomPilar.Blocks[randomPilar.Blocks.Count - 1];
//		//	//        Vector3 blockOffset = new Vector3((float)Manager.Mgr.BuildRNG.NextDouble(), 0.0f, (float)Manager.Mgr.BuildRNG.NextDouble());
//		//	//        m_TargetPosition = new Vector3(randomPilar.transform.position.x, randomBlock.Height + randomBlock.MicroHeight, randomPilar.transform.position.z) + blockOffset;

//		//	//    }
//		//	//    return; //AIState.ROAMING;
//		//	//}

//		//	public void AlertState()
//		//	{
//		//		if (m_TargetLE == null)
//		//			return;

//		//		var curPos = new Vector2(transform.position.x, transform.position.z);
//		//		var targetPos = new Vector2(m_TargetLE.transform.position.x, m_TargetLE.transform.position.z);
//		//		var targetDistance = Vector2.Distance(curPos, targetPos);
//		//		if (targetDistance > (m_Info.SightRange * 2.0f))
//		//		{
//		//			m_TargetLE = null;
//		//			return;
//		//		}

//		//		var targetDirection = (targetPos - curPos).normalized;
//		//		var targetAttackPos = curPos + targetDirection * (targetDistance - m_Info.AttackRange * 0.9f);
//		//		var attackPos = curPos + targetDirection * (targetDistance - m_Info.AttackRange);
//		//		m_TargetPosition.Set(targetAttackPos.x, 0.0f, targetAttackPos.y);
//		//		SetLookingDirection();
//		//	}

//		//	public AIState AlertComputeNext()
//		//	{
//		//		if (m_TargetLE == null)
//		//			return AIState.ROAMING;

//		//		if (m_TargetLE.GetCurrentHealth() <= 0)
//		//		{
//		//			m_TargetLE = null;
//		//			return AIState.ROAMING;
//		//		}

//		//		var curPos = new Vector2(transform.position.x, transform.position.z);
//		//		var targetPos = new Vector2(m_TargetLE.transform.position.x, m_TargetLE.transform.position.z);
//		//		var targetDistance = Vector2.Distance(curPos, targetPos);
//		//		if (targetDistance <= m_Info.AttackRange)
//		//			return AIState.AGRESSIVE;
//		//		return AIState.ALERT;
//		//	}

//		//	//public void _AlertState()
//		//	//{
//		//	//    if (m_TargetLE == null)
//		//	//        return;// AIState.ROAMING;

//		//	//    var curPos = new Vector2(transform.position.x, transform.position.z);
//		//	//    var targetPos = new Vector2(m_TargetLE.transform.position.x, m_TargetLE.transform.position.z);
//		//	//    var targetDistance = Vector2.Distance(curPos, targetPos);
//		//	//    if(targetDistance > (m_Attribs.SightRange * 2.0f))
//		//	//    {
//		//	//        m_TargetLE = null;
//		//	//        m_TargetPosition = transform.position;
//		//	//        return;// AIState.ROAMING;
//		//	//    }
//		//	//    var targetDirection = (targetPos - curPos).normalized;
//		//	//    var targetAttackPos = curPos + targetDirection * (targetDistance - m_Attribs.AttackRange * 0.9f);
//		//	//    var attackPos = curPos + targetDirection * (targetDistance - m_Attribs.AttackRange );
//		//	//    m_TargetPosition.Set(targetAttackPos.x, 0.0f, targetAttackPos.y);

//		//	//    var attackPosDist = Vector2.Distance(attackPos, curPos);
//		//	//    if(GameUtils.IsNearlyEqual(attackPosDist, 0.0f, 0.1f))
//		//	//    {
//		//	//        return;// AIState.AGRESSIVE;
//		//	//    }

//		//	//    return;// AIState.ALERT;
//		//	//}

//		//	public void AwareState()
//		//	{
//		//		if (m_TargetLE == null)
//		//			return;

//		//		var curPos = new Vector2(transform.position.x, transform.position.z);
//		//		var targetPos = new Vector2(m_TargetLE.transform.position.x, m_TargetLE.transform.position.z);
//		//		var targetDistance = Vector2.Distance(curPos, targetPos);
//		//		if (targetDistance > (m_Info.HearingRange * 1.25f))
//		//		{
//		//			//m_TargetLE = null;
//		//			return;
//		//		}

//		//		var targetDirection = (targetPos - curPos).normalized;
//		//		var targetAttackPos = curPos + targetDirection * (targetDistance - m_Info.AttackRange * 0.9f);
//		//		var attackPos = curPos + targetDirection * (targetDistance - m_Info.AttackRange);
//		//		m_TargetPosition.Set(targetAttackPos.x, 0.0f, targetAttackPos.y);
//		//		SetLookingDirection();
//		//	}

//		//	public AIState AwareComputeNext()
//		//	{
//		//		if (m_TargetLE == null)
//		//			return AIState.ROAMING;

//		//		if (m_TargetLE.GetCurrentHealth() <= 0)
//		//		{
//		//			m_TargetLE = null;
//		//			return AIState.ROAMING;
//		//		}

//		//		var curPos = new Vector2(transform.position.x, transform.position.z);
//		//		var targetPos = new Vector2(m_TargetLE.transform.position.x, m_TargetLE.transform.position.z);
//		//		var targetDistance = Vector2.Distance(curPos, targetPos);
//		//		if (targetDistance > (m_Info.HearingRange * 1.25f))
//		//		{
//		//			m_TargetLE = null;
//		//			return AIState.ROAMING;
//		//		}
//		//		if(targetDistance <= m_Info.SightRange)
//		//		{
//		//			return AIState.ALERT;
//		//		}

//		//		return AIState.AWARE;
//		//	}

//		//	//public void _AwareState()
//		//	//{
//		//	//    var curPos = new Vector2(transform.position.x, transform.position.z);
//		//	//    Vector2 targetPos = new Vector2(m_TargetPosition.x, m_TargetPosition.z);
//		//	//    if (curPos == targetPos)
//		//	//        return;// AIState.ROAMING;
//		//	//    Vector2 curDir = Vector2.zero;
//		//	//    if (m_TargetPosition == transform.position)
//		//	//        curDir.Set(0.0f, m_Facing.Vertical == SpriteVertical.UP ? -1.0f : 1.0f);
//		//	//    else
//		//	//        curDir = (new Vector2(m_TargetPosition.x, m_TargetPosition.z) - curPos).normalized;

//		//	//    var odd = Manager.Mgr.OddGO;
//		//	//    var oddPos = new Vector2(odd.transform.position.x, odd.transform.position.z);
//		//	//    var oddDistance = Vector2.Distance(curPos, oddPos);

//		//	//    if (oddDistance <= m_Attribs.SightRange)
//		//	//    {
//		//	//        var oddDirection = (oddPos - curPos).normalized;
//		//	//        var oddCosDot = Vector2.Dot(curDir, oddDirection);
//		//	//        var oddAngle = Mathf.Abs(Mathf.Acos(oddCosDot) * Mathf.Rad2Deg);
//		//	//        if (oddAngle <= m_Attribs.SightAngle)
//		//	//        {
//		//	//            m_TargetLE = odd.GetComponent<LivingEntity>();
//		//	//            targetPos = oddDirection * (oddDistance - m_Attribs.AttackRange);
//		//	//            m_TargetPosition.Set(targetPos.x, 0.0f, targetPos.y);
//		//	//            OnEnemySeen();
//		//	//            return;// AIState.ALERT;
//		//	//        }
//		//	//    }
//		//	//    if (oddDistance <= m_Attribs.HearingRange)
//		//	//    {
//		//	//        m_TargetPosition = odd.transform.position;
//		//	//        m_TargetLE = null;
//		//	//        return;// AIState.AWARE;
//		//	//    }

//		//	//    for (int i = 0; i < Manager.Mgr.Strucs.Count; ++i)
//		//	//    {
//		//	//        var curStruc = Manager.Mgr.Strucs[i];
//		//	//        for (int j = 0; j < curStruc.LivingEntities.Count; ++j)
//		//	//        {
//		//	//            var curLE = curStruc.LivingEntities[j];
//		//	//            if (curLE.GetLEType() != Def.LivingEntityType.Monster)
//		//	//                continue;
//		//	//            var curMon = (MonsterScript)curLE;
//		//	//            if (m_Attribs.Team == curMon.m_Attribs.Team || curMon.m_Health <= 0)
//		//	//                continue;
//		//	//            var lePos = new Vector2(curLE.transform.position.x, curLE.transform.position.z);
//		//	//            var leDist = Vector2.Distance(lePos, curPos);
//		//	//            if (leDist <= m_Attribs.SightRange)
//		//	//            {
//		//	//                var leDirection = (lePos - curPos).normalized;
//		//	//                var leCosDot = Vector2.Dot(curDir, leDirection);
//		//	//                var leAngle = Mathf.Abs(Mathf.Acos(leCosDot) * Mathf.Rad2Deg);
//		//	//                if (leAngle <= m_Attribs.SightAngle)
//		//	//                {
//		//	//                    m_TargetLE = curLE;
//		//	//                    targetPos = leDirection * (leDist - m_Attribs.AttackRange);
//		//	//                    m_TargetPosition.Set(targetPos.x, 0.0f, targetPos.y);
//		//	//                    OnEnemySeen();
//		//	//                    return;// AIState.ALERT;
//		//	//                }
//		//	//            }

//		//	//            if (leDist <= m_Attribs.HearingRange)
//		//	//            {
//		//	//                m_TargetLE = null;
//		//	//                m_TargetPosition.Set(lePos.x, 0.0f, lePos.y);
//		//	//                return;// AIState.AWARE;
//		//	//            }
//		//	//        }
//		//	//    }
//		//	//    return;// AIState.AWARE;
//		//	//}

//		//	public void AggressiveState()
//		//	{
//		//		if (m_TargetLE == null)
//		//			return;
//		//		// Have you killed?
//		//		if(m_TargetLE.GetCurrentHealth() < 0)
//		//		{
//		//			m_TargetLE = null;
//		//			return;
//		//		}

//		//		var curPos = new Vector2(transform.position.x, transform.position.z);
//		//		var targetPos = new Vector2(m_TargetLE.transform.position.x, m_TargetLE.transform.position.z);
//		//		var dir = (targetPos - curPos).normalized;
//		//		if (dir != Vector2.zero)
//		//			m_Direction = dir;
//		//		var targetDistance = Vector2.Distance(curPos, targetPos);
//		//		if (targetDistance >= m_Info.AttackRange)
//		//			return;

//		//		if (m_NextAttackTime < Time.time)
//		//		{
//		//			m_NextAttackTime = Time.time + m_Info.AttackRate;

//		//			Attack(m_TargetLE);
//		//			//m_TargetLE.ReceiveDamage(m_Attribs.DmgType, m_Attribs.AttackDamage);
//		//		}
//		//	}

//		//	public AIState AggressiveComputeNext()
//		//	{
//		//		if (m_TargetLE == null)
//		//			return AIState.ROAMING;

//		//		if(m_TargetLE.GetCurrentHealth() <= 0)
//		//		{
//		//			m_TargetLE = null;
//		//			return AIState.ROAMING;
//		//		}

//		//		var curPos = new Vector2(transform.position.x, transform.position.z);
//		//		var targetPos = new Vector2(m_TargetLE.transform.position.x, m_TargetLE.transform.position.z);
//		//		var targetDistance = Vector2.Distance(curPos, targetPos);
//		//		if (targetDistance > m_Info.AttackRange)
//		//		{
//		//			if (targetDistance >= (m_Info.HearingRange * 1.25f))
//		//			{
//		//				m_TargetLE = null;
//		//				return AIState.IDLE;
//		//			}
//		//			if (targetDistance >= (m_Info.SightRange * 1.25f))
//		//			{
//		//				return AIState.AWARE;
//		//			}
//		//			return AIState.ALERT;
//		//		}

//		//		return AIState.AGRESSIVE;
//		//	}

//		//	//public void _AggressiveState()
//		//	//{
//		//	//    if (m_TargetLE == null)
//		//	//        return;// AIState.ROAMING;

//		//	//    var curPos = new Vector2(transform.position.x, transform.position.z);
//		//	//    var targetPos = new Vector2(m_TargetLE.transform.position.x, m_TargetLE.transform.position.z);
//		//	//    var targetDistance = Vector2.Distance(curPos, targetPos);
//		//	//    if (targetDistance >= m_Attribs.AttackRange)
//		//	//        return;// AIState.ALERT;

//		//	//    if(m_NextAttackTime < Time.time)
//		//	//    {
//		//	//        m_NextAttackTime = Time.time + m_Attribs.AttackRate;

//		//	//        m_TargetLE.ReceiveDamage(m_Attribs.DmgType, m_Attribs.AttackDamage);
//		//	//    }

//		//	//    return; //AIState.AGRESSIVE;
//		//	//}

//		//	public void FleeState()
//		//	{
//		//		if (m_TargetLE == null)
//		//			return;

//		//		//if (m_LastTargetChange >= Time.time)
//		//		//    return;

//		//		var curPos = new Vector2(transform.position.x, transform.position.z);
//		//		var targetPos = new Vector2(m_TargetLE.transform.position.x, m_TargetLE.transform.position.z);
//		//		var targetDistance = Vector2.Distance(curPos, targetPos);
//		//		if(targetDistance > 5.0f)
//		//		{
//		//			m_TargetLE = null;
//		//			return;
//		//		}
//		//		var dir = (targetPos - curPos).normalized;
//		//		dir = -dir;
//		//		m_TargetPosition.Set(targetPos.x + dir.x * 5.0f, m_TargetPosition.y, targetPos.y + dir.y * 5.0f);
//		//		SetLookingDirection();
//		//		//m_LastTargetChange = Time.time + TargetChangeWait;
//		//	}

//		//	public AIState FleeComputeNext()
//		//	{
//		//		if (m_TargetLE == null)
//		//			return AIState.ROAMING;

//		//		return AIState.FLEE;
//		//	}

//		//	public void SetAI()
//		//	{
//		//		m_AI = new GameAI();
//		//		m_SeenEnemies = new List<EnemySeen>();
//		//		m_AI.SetReactionTime(0.25f);

//		//		//m_Attribs = Monsters.MonsterAttribs[m_Monster.AttribID];
//		//		if(m_Info.AIType == Def.MonsterAIType.ROAMING_AI || m_Info.AIType == Def.MonsterAIType.AGRESSIVE_AI || m_Info.AIType == Def.MonsterAIType.SCARED_AI)
//		//		{
//		//			AIFunction[] functions = new AIFunction[(int)AIState.COUNT];
//		//			functions[(int)AIState.IDLE] = new AIFunction(AIState.IDLE, () => { }, () => { return AIState.ROAMING; });
//		//			functions[(int)AIState.DEAD] = new AIFunction(AIState.DEAD, () => { }, () => { return AIState.DEAD; });
//		//			functions[(int)AIState.FLEE] = new AIFunction(AIState.FLEE, FleeState, FleeComputeNext);
//		//			switch (m_Info.AIType)
//		//			{
//		//				case Def.MonsterAIType.ROAMING_AI:
//		//					functions[(int)AIState.AWARE] = new AIFunction(AIState.AWARE, RoamingState, () => { return AIState.ROAMING; });
//		//					functions[(int)AIState.ALERT] = new AIFunction(AIState.ALERT, RoamingState, () => { return AIState.ROAMING; });
//		//					functions[(int)AIState.ROAMING] = new AIFunction(AIState.ROAMING, RoamingState, RoamingComputeNextPacific);
//		//					functions[(int)AIState.AGRESSIVE] = new AIFunction(AIState.AGRESSIVE, RoamingState, () => { return AIState.ROAMING; });
//		//					break;
//		//				case Def.MonsterAIType.AGRESSIVE_AI:
//		//					functions[(int)AIState.AWARE] = new AIFunction(AIState.AWARE, AwareState, AwareComputeNext);
//		//					functions[(int)AIState.ALERT] = new AIFunction(AIState.ALERT, AlertState, AlertComputeNext);
//		//					functions[(int)AIState.ROAMING] = new AIFunction(AIState.ROAMING, RoamingState, RoamingComputeNextAggresive);
//		//					functions[(int)AIState.AGRESSIVE] = new AIFunction(AIState.AGRESSIVE, AggressiveState, AggressiveComputeNext);
//		//					break;
//		//				case Def.MonsterAIType.SCARED_AI:
//		//					functions[(int)AIState.AWARE] = new AIFunction(AIState.AWARE, RoamingState, () => { return AIState.ROAMING; });
//		//					functions[(int)AIState.ALERT] = new AIFunction(AIState.ALERT, RoamingState, () => { return AIState.ROAMING; });
//		//					functions[(int)AIState.ROAMING] = new AIFunction(AIState.ROAMING, RoamingState, RoamingComputeNextScared);
//		//					functions[(int)AIState.AGRESSIVE] = new AIFunction(AIState.AGRESSIVE, RoamingState, () => { return AIState.ROAMING; });
//		//					break;
//		//			}
//		//			for(int i = 0; i < functions.Length; ++i)
//		//			{
//		//				m_AI.SetFunction(functions[i]);
//		//			}
//		//		}
//		//	}

//		protected void SetMonster(int monsterID)
//		{
//	//		m_Info = Instantiate(Monsters.MonsterFamilies[monsterID]);
//	//		m_Frame = 0;
//	//		m_Facing.Vertical = SpriteVertical.UP;
//	//		m_Facing.Horizontal = SpriteHorizontal.LEFT;
//	//		m_NextFrameChange = Time.time + UnityEngine.Random.value * 1.5f;
//	//		m_EnemyList = new List<LivingEntity>();

//	//		// Sprite
//	//		m_Sprite = SpriteUtils.AddSprite(
//	//			new GameObject(gameObject.name + "_Sprite"),
//	//			SpriteBackendType.SQUAD,
//	//			m_Info.Frames[0]) as SpriteBackendSQuad;
//	//		m_Sprite.transform.SetParent(transform);
//	//		var texSize = m_Info.Frames[0].texture.width;
//	//		float scale = texSize / m_Info.Frames[0].pixelsPerUnit;
//	//		var pivot = m_Info.Frames[0].pivot / m_Info.Frames[0].pixelsPerUnit;
//	//		m_Sprite.transform.localScale = new Vector3(m_Info.SpriteScale, m_Info.SpriteScale, 1f);
//	//		gameObject.layer = Def.RCLayerLE;


//	//		//m_SpriteSR = new GameObject(gameObject.name + "_Sprite").AddComponent<SpriteRenderer>();
//	//		//m_SpriteSR.transform.SetParent(transform);
//	//		//SpriteUtils.InitSpriteLit(m_SpriteSR, m_Info.Frames[0]);
//	//		////SpriteUtils.InitSprite(m_SpriteSR, GetCurrentSprite(), Materials.GetMaterial(Def.Materials.Sprite));
//	//		//m_SpriteSR.flipX = true;
//	//		//m_SpriteSR.transform.localScale = new Vector3(m_Info.SpriteScale, m_Info.SpriteScale, 1.0f);
//	//		//m_SpriteYOffset = GetCurrentYOffset();
//	//		//m_SpriteSR.tag = TAG;
//	//		////m_SpriteBC = m_SpriteGO.AddComponent<BoxCollider>();
//	//		////m_SpriteBC.tag = TAG;
//	//		//m_LastIDX = GetCurrentIndex();
//	//		////m_SpriteBC.size = new Vector3(m_Monster.VisibleRect[m_LastIDX].width / m_Monster.Sprites[m_LastIDX].pixelsPerUnit, m_Monster.VisibleRect[m_LastIDX].height / m_Monster.Sprites[m_LastIDX].pixelsPerUnit, 0.01f);
//	//		float nRadius = 0f;
//	//		for (int i = 0; i < Def.MonsterFrameCount; ++i)
//	//		{
//	//			var rect = m_Info.VisibleRect[i];
//	//			var radius = rect.width * 0.5f;
//	//			radius /= m_Info.Frames[i].pixelsPerUnit;
//	//			if (nRadius < radius)
//	//				nRadius = radius;
//	//		}

//	//		m_SpriteCC = /*m_SpriteSR.*/gameObject.AddComponent<CapsuleCollider>();
//	//		m_SpriteCC.radius = nRadius * 0.75f;
//	//		m_SpriteCC.center.Set(0.0f, m_SpriteCC.center.y, 0.0f);
//	//		//m_SpriteCC.tag = TAG;

//	//		var nHeight = m_SpriteCC.height;

//	//		SetLivingEntity(new LivingEntityDef()
//	//		{
//	//			TotalHealth = m_Info.BaseHealth,
//	//			Speed = m_Info.BaseSpeed,
//	//			AngularSpeed = 1f,
//	//			Radius = nRadius,
//	//			Height = nHeight,
//	//			MaxJump = 0.5f,
//	//			FlySpeed = 6f,
//	//			FallSpeed = 3f,
//	//			LEType = Def.LivingEntityType.Monster
//	//		});

//	//		// Shadow
//	//		m_Shadow = SpriteUtils.AddSprite(
//	//			new GameObject(gameObject.name + "_Shadow"),
//	//			SpriteBackendType.SPRITE,
//	//			AssetContainer.Mgr.SpriteShadow) as SpriteBackendSprite;
//	//		m_Shadow.transform.SetParent(transform);
//	//		var shadowSize = m_Shadow.GetSprite().texture.width;
//	//		var shadowOffset = 0.5f + shadowSize * 0.001f;
//	//		m_Shadow.transform.localPosition = new Vector3(-shadowOffset, 0.01f, -shadowOffset);
//	//		m_Shadow.transform.Rotate(Vector3.right, 90f, Space.Self);
//	//		//m_ShadowSR = new GameObject(gameObject.name + "_Shadow").AddComponent<SpriteRenderer>();
//	//		//m_ShadowSR.transform.Translate(transform.position, Space.World);
//	//		//m_ShadowSR.transform.Translate(new Vector3(-0.5f, 0.01f, -0.5f), Space.World);
//	//		//m_ShadowSR.transform.Rotate(Vector3.right, 90.0f, Space.World);
//	//		//m_ShadowSR.transform.Translate(new Vector3(-0.128f, 0.0f, -0.128f), Space.World);
//	//		//m_ShadowSR.transform.SetParent(transform);
//	//		////var shadowTex = AssetContainer.Mgr.SpriteShadow;
//	//		////SpriteUtils.InitSprite(m_ShadowSR,
//	//		////    Sprite.Create(shadowTex, new Rect(0.0f, 0.0f, shadowTex.width, shadowTex.height), new Vector2(0.0f, 0.0f), 100, 0, SpriteMeshType.FullRect),
//	//		////    Materials.GetMaterial(Def.Materials.Sprite));
			
//	//		//m_ShadowSR.sprite = AssetContainer.Mgr.SpriteShadow;

//	//		m_TargetPosition = transform.position;
//	//		m_Direction = new Vector2(1f, 0f);
//	//		//gameObject.tag = TAG;

//	//		// AlertSign
//	//		var alertGO = new GameObject(gameObject.name + "_Sign");
//	//		alertGO.transform.Translate(transform.position, Space.World);
			
//	//		//var spriteHeight = m_ShadowSR.sprite.rect.height / m_SpriteSR.sprite.pixelsPerUnit * m_Info.SpriteScale;// + 1.0f;
//	//		//alertGO.transform.Translate(new Vector3(0.0f, spriteHeight, 0.0f), Space.World);
//	//		alertGO.transform.SetParent(gameObject.transform);
//	//		m_AlertSign = alertGO.AddComponent<MonsterAlertComponent>();

//	//		m_AlertSign.SetVFX(new VFXDef(Def.VFXTarget.GENERAL, "WARNING", Def.VFXType.CAST, 0, Def.VFXFacing.FaceCameraFull, Def.VFXEnd.Stop, 12.0f));
//	//		m_AlertSign.ResetVFX(200000.0f);

//	//		SetAI();

//	//		if (m_BloodTrails == null)
//	//		{
//	//			var familyIdx = VFXs.FamilyDict[(int)Def.VFXTarget.GENERAL]["Blood"];
//	//			var family = VFXs.VFXFamilies[(int)Def.VFXTarget.GENERAL][familyIdx];

//	//			m_BloodTrails = new VFXDef[family.CastVFX.Length];
//	//			m_BloodTrailScales = new float[m_BloodTrails.Length];
//	//			for (int i = 0; i < m_BloodTrails.Length; ++i)
//	//			{
//	//				m_BloodTrails[i] = new VFXDef(Def.VFXTarget.GENERAL, "Blood", Def.VFXType.CAST, i, Def.VFXFacing.DontFaceAnything, Def.VFXEnd.SelfDestroy, 12f);
//	//				m_BloodTrailScales[i] = 1f;
//	//			}
//	//			m_BloodTrailScales[1] = 0.5f;
//	//		}
//		}

//		public void SetEnemy(LivingEntity enemy)
//		{
//	//		if (enemy == this || enemy == null || m_EnemyList.Contains(enemy))
//	//			return;
//	//		m_EnemyList.Add(enemy);
//	//		for(int i = 0; i < m_EnemyList.Count; )
//	//		{
//	//			var curEnemy = m_EnemyList[i];
//	//			if(curEnemy == null || (curEnemy != null && curEnemy.GetCurrentHealth() <= 0.0f))
//	//			{
//	//				m_EnemyList.RemoveAt(i);
//	//				continue;
//	//			}
//	//			++i;
//	//		}
//		}

//		public abstract void InitMonster();

//		public abstract void Attack(LivingEntity target, Vector3 targetPos = default);

//	//	bool IsFailing(IBlock currentBlock, BridgeComponent currentBridge)
//	//	{
//	//		return (/*(currentBlock != null && currentBlock.Layer == 0) ||*/ currentBlock == null) && currentBridge == null;
//	//	}

//	//	void WhileFailing()
//	//	{
//	//		float failSpeed = Mathf.Min(-9.81f * Mathf.Abs(((transform.position.y) * 0.1f)), -9.81f);
//	//		failSpeed *= Time.deltaTime;
//	//		transform.Translate(new Vector3(0.0f, failSpeed, 0.0f), Space.World);
//	//	}

//		public void CastBloodTrails(Vector3 pos, Vector3 dir)
//		{
//	//		var version = Manager.Mgr.DamageRNG.Next(m_BloodTrails.Length);
//	//		var btfx = new GameObject(gameObject.name + "_BloodTrail").AddComponent<VFXComponent>();
//	//		btfx.SetVFX(m_BloodTrails[version]);
//	//		btfx.transform.position = pos;
//	//		float scale = m_BloodTrailScales[version];
//	//		btfx.transform.localScale = new Vector3(scale, scale, 1f);
//	//		//btfx.transform.Translate(dir.x * 0.1f, 0f, dir.z * 0.1f, Space.World);
//	//		float zAngle = 0f;
//	//		if(dir.z >= -1f && dir.z < 0f)
//	//		{
//	//			float zDir = dir.z * -1f;
//	//			if(dir.x < 0f)
//	//			{
//	//				zAngle = (1f - zDir) * -90f;// + zDir * 0f;
//	//			}
//	//			else
//	//			{
//	//				zAngle = (1f - zDir) * 90f;// + zDir * 0f;
//	//			}
//	//		}
//	//		else // z =< 1f && z >= 0f
//	//		{
//	//			if(dir.x < 0f)
//	//			{
//	//				zAngle = (1f - dir.z) * -90f + dir.z * 180f;
//	//			}
//	//			else
//	//			{
//	//				zAngle = (1f - dir.z) * 90f + dir.z * 180f;
//	//			}
//	//		}

//	//		btfx.transform.Rotate(90f, 0f, zAngle, Space.World);
//	//		btfx.Renderer.color = new Color(215f / 255f, 0f, 0f, 1f);
//	//		btfx.Renderer.flipY = version == 1;
//		}

//		public void _FacingUpdate()
//		{
//	//		var facing = m_Facing;
//	//		var xCamDistance = Manager.Mgr.m_Camera.transform.position.x - transform.position.x;
//	//		if (xCamDistance < 0.0f)
//	//		{
//	//			if (m_Direction.x < 0.0f)
//	//			{
//	//				facing.Vertical = SpriteVertical.DOWN;
//	//			}
//	//			else
//	//			{
//	//				facing.Vertical = SpriteVertical.UP;
//	//			}
//	//			if (m_Direction.y < 0.0f)
//	//			{
//	//				facing.Horizontal = SpriteHorizontal.RIGHT;
//	//			}
//	//			else
//	//			{
//	//				facing.Horizontal = SpriteHorizontal.LEFT;
//	//			}
//	//		}
//	//		else
//	//		{
//	//			if (m_Direction.x < 0.0f)
//	//			{
//	//				facing.Vertical = SpriteVertical.UP;
//	//			}
//	//			else
//	//			{
//	//				facing.Vertical = SpriteVertical.DOWN;
//	//			}
//	//			if (m_Direction.y < 0.0f)
//	//			{
//	//				facing.Horizontal = SpriteHorizontal.LEFT;
//	//			}
//	//			else
//	//			{
//	//				facing.Horizontal = SpriteHorizontal.RIGHT;
//	//			}
//	//		}

//	//		Facing = facing;
//		}

//	//	private void Update()
//	//	{
//	//		_OnReceiveDamageUpdate();
//	//		CheckHealth();
//	//		//var color = m_SpriteSR.color;
//	//		//if (m_Health <= 0.0f)
//	//		//{
//	//		//    //if(m_AI.GetCurrentState() != AIState.DEAD)
//	//		//    //{
//	//		//    //    if (Struc != null && Struc.LivingEntities.Contains(this))
//	//		//    //        Struc.LivingEntities.Remove(this);
//	//		//    //    m_AI.ChangeStateTo(AIState.DEAD);
//	//		//    //}
//	//		//    color.a -= color.a * Time.deltaTime * 2.0f;
//	//		//}
//	//		//if (GameUtils.IsNearlyEqual(color.a, 0.0f)) // die
//	//		//{
//	//		//    //if (Struc != null && Struc.LivingEntities.Contains(this))
//	//		//    //    Struc.LivingEntities.Remove(this);
//	//		//    GameUtils.DeleteGameObjectAndItsChilds(gameObject, true);
//	//		//    return;
//	//		//}
//	//		//else
//	//		//{
//	//		//    m_SpriteSR.color = color;
//	//		//    m_ShadowSR.color = color;
//	//		//}
//	//		if (Manager.Mgr.CurrentControllerSel != (int)GameState.PLAY || m_AI.GetCurrentState() == AIState.DEAD || Manager.Mgr.HideInfo == false)
//	//			return;

//	//		var curAIState = m_AI.GetCurrentState();
//	//		m_AlertSign.State = curAIState;

//	//		bool onGround = UpdateMovement(true, transform.position.y, Time.deltaTime, out Vector3 movement,
//	//			out IBlock currentBlock, out BridgeComponent currentBridge);

//	//		//bool onGround = GameUtils.PFMovement(transform.position, new Vector2(m_TargetPosition.x, m_TargetPosition.z), m_Speed,
//	//		//    6.0f, 3.0f, m_Radius, m_MaxJump, Time.deltaTime, true, out Vector3 movement, out BlockComponent currentBlock,
//	//		//    out BridgeComponent currentBridge);

//	//		//var curPos = new Vector2(transform.position.x, transform.position.z);
//	//		//Vector2 curDir = Vector2.zero;
//	//		//if (m_TargetPosition == transform.position)
//	//		//    curDir.Set(0.0f, m_Facing.Vertical == SpriteVertical.UP ? -1.0f : 1.0f);
//	//		//else
//	//		//    curDir = (new Vector2(m_TargetPosition.x, m_TargetPosition.z) - curPos).normalized;


//	//		if (IsFailing(currentBlock, currentBridge) && Manager.Mgr.HideInfo)
//	//		{
//	//			WhileFailing();
//	//			return;
//	//		}
//	//		//Position += movement;
//	//		bool posChanged = new Vector2(movement.x, movement.z) != Vector2.zero;

//	//		_FacingUpdate();

//	//		RegisterLE(posChanged);
//	//		//if(currentBlock != null && (posChanged || Struc == null))
//	//		//{
//	//		//    if(Struc != currentBlock.Pilar.Struc)
//	//		//    {
//	//		//        if (Struc != null && Struc.LivingEntities.Contains(this))
//	//		//            Struc.LivingEntities.Remove(this);
//	//		//        Struc = currentBlock.Pilar.Struc;
//	//		//    }
//	//		//    if (Struc != null && !Struc.LivingEntities.Contains(this))
//	//		//        Struc.LivingEntities.Add(this);
//	//		//}
//	//		transform.Translate(movement, Space.World);
//	//		ElementUpdate();
//	//		m_AI.Update();
//	//	}

//		public void _SpriteFrameUpdate()
//		{
//	//		if (m_NextFrameChange < Time.time)
//	//		{
//	//			if (m_Frame == 1)
//	//				m_Frame = 0;
//	//			else
//	//				m_Frame = 1;

//	//			m_NextFrameChange = Time.time + FrameChangeWait[(int)m_AI.GetCurrentState()];
//	//		}
//	//		var curIdx = GetCurrentIndex();
//	//		if (m_LastIDX != curIdx)
//	//		{
//	//			var sprite = m_Info.Frames[curIdx];
//	//			//var visibleRect = m_Info.VisibleRect[curIdx];
//	//			m_Sprite.ChangeSprite(sprite);
//	//			//m_SpriteSR.sprite = sprite;
//	//			//m_SpriteSR.material.mainTexture = sprite.texture;
				
//	//			//m_SpriteCC.center.Set(sprite.bounds.center.x + m_Monster.BoxCenterOffset[curIdx].x, sprite.bounds.center.y + m_Monster.BoxCenterOffset[curIdx].y, 0.0f);
//	//			//m_SpriteBC.size = new Vector3(m_Monster.BoxSize[curIdx].x, m_Monster.BoxSize[curIdx].y, 0.01f);
//	//			//m_SpriteBC.center = new Vector3(sprite.bounds.center.x + m_Monster.BoxCenterOffset[curIdx].x,
//	//			//sprite.bounds.center.y + m_Monster.BoxCenterOffset[curIdx].y, 0.0f);

//	//			//if(m_SpriteSR.flipX)
//	//			//    m_SpriteBC.center = new Vector3(-m_SpriteBC.center.x, m_SpriteBC.center.y, m_SpriteBC.center.z);
//	//			m_LastIDX = curIdx;
//	//		}
//		}

//	//	private void FixedUpdate()
//	//	{
//	//		_SpriteFrameUpdate();
//	//		if(m_AI.GetCurrentState() != AIState.DEAD && m_Health <= 0.0f)
//	//		{
//	//			m_AI.ChangeStateTo(AIState.DEAD);
//	//		}

//	//		//if (Manager.Mgr.CurrentControllerSel != (int)GameState.PLAYING)
//	//		//    return;
			
//	//		//if (m_LastTargetChange < Time.time && Struc != null)
//	//		//{
//	//		//    PilarComponent randomPilar = null;
//	//		//    while (randomPilar == null)
//	//		//    {
//	//		//        randomPilar = Struc.Pilars[Manager.Mgr.BuildRNG.Next(0, Struc.Pilars.Length)];
//	//		//    }
//	//		//    TargetGO = randomPilar.Blocks[randomPilar.Blocks.Count - 1].TopGO;
//	//		//    TargetOffset.x = (float)Manager.Mgr.BuildRNG.NextDouble();
//	//		//    TargetOffset.z = (float)Manager.Mgr.BuildRNG.NextDouble();
//	//		//    var pos = randomPilar.transform.position + TargetOffset;
//	//		//    TargetOffset = pos - TargetGO.transform.position;
//	//		//    m_LastTargetChange = Time.time + TargetChangeWait;
//	//		//}
//	//	}

//		public void _FaceCamera()
//		{
//	//		var posY = transform.position.y;
//	//		var camPos = CameraManager.Mgr.Camera.transform.position;
//	//		m_Sprite.transform.LookAt(new Vector3(camPos.x, posY + 1f, camPos.z));
//	//		//m_SpriteSR.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
//	//		//var posY = transform.position.y;
//	//		////m_SpriteGO.transform.Translate(transform.position, Space.World);
//	//		//m_SpriteSR.transform.Translate(new Vector3(0.0f, 0.02f, 0.0f), Space.Self); // block offset
//	//		////SpriteGO.transform.Translate(new Vector3(0.5f, 0.02f, 0.5f), Space.Self); // block offset
//	//		//var camPos = Manager.Mgr.m_Camera.transform.position;
//	//		////m_SpriteGO.transform.LookAt(new Vector3(camPos.x, camPos.y - 5.0f, camPos.z));
//	//		////var yOffset = camPos.y - posY;
//	//		//m_SpriteSR.transform.LookAt(new Vector3(camPos.x, posY + 1.0f, camPos.z));
//	//		////m_SpriteGO.transform.LookAt(Manager.Mgr.m_Camera.transform);
//	//		//m_SpriteSR.transform.Translate(new Vector3(0.0f, m_SpriteYOffset, 0.0f), Space.World);
//		}

//	//	private void LateUpdate()
//	//	{
//	//		_FaceCamera();
//	//	}

//	//	private void OnEnable()
//	//	{
//	//		if (m_Sprite == null)
//	//			return;
//	//		m_Sprite.enabled = true;
//	//		m_Shadow.enabled = true;
//	//	}

//	//	private void OnDisable()
//	//	{
//	//		if (m_Sprite == null)
//	//			return;
//	//		m_Sprite.enabled = false;
//	//		m_Shadow.enabled = false;
//	//	}

//	//	private void OnDrawGizmos()
//	//	{
//	//		if (Manager.Mgr.HideInfo)
//	//			return;
//	//		var curPos = new Vector2(transform.position.x, transform.position.z);
//	//		Vector2 curDir = Vector2.zero;
//	//		if (m_TargetPosition == transform.position)
//	//			curDir.Set(0.0f, m_Facing.Vertical == SpriteVertical.UP ? -1.0f : 1.0f);
//	//		else
//	//			curDir = (new Vector2(m_TargetPosition.x, m_TargetPosition.z) - curPos).normalized;
//	//		Gizmos.color = Color.green;
//	//		var startPos = new Vector3(curPos.x, transform.position.y + 0.5f, curPos.y);
//	//		var endPos = startPos + new Vector3(curDir.x * 2.0f, 0.0f, curDir.y * 2.0f);
//	//		Gizmos.DrawLine(startPos, endPos);
//	//	}

//	//	//public void TakeDamage(float amount)
//	//	//{
//	//	//    if (amount <= 0.0f)
//	//	//        return;
//	//	//    m_Health -= amount;
//	//	//}

//	//	public void OnGUI()
//	//	{
//	//		if (Manager.Mgr.HideInfo)
//	//			return;



//	//		//var viewPoint = Manager.Mgr.m_Camera.WorldToViewportPoint(transform.position);
//	//		//var screenPoint = Manager.Mgr.m_Camera.ViewportToScreenPoint(viewPoint);
//	//		var wPos = transform.position;
//	//		var currentSprite = GetCurrentSprite();
//	//		wPos.y += (currentSprite.rect.height / currentSprite.pixelsPerUnit) * m_Info.SpriteScale;
//	//		var screenPoint = Manager.Mgr.m_Camera.WorldToScreenPoint(wPos);
//	//		if (screenPoint.x < 0.0f || screenPoint.y < 0.0f || screenPoint.x > Screen.width || screenPoint.y > Screen.height)
//	//			return;

//	//		var canvas = Manager.Mgr.m_Canvas;
//	//		var rect = canvas.pixelRect;
//	//		screenPoint.y = Screen.height - screenPoint.y;
//	//		var guiPoint = GUIUtility.ScreenToGUIPoint(screenPoint);

//	//		var iaYPos = guiPoint.y - 50.0f;

//	//		var aiContent = new GUIContent(m_AI.GetCurrentState().ToString());
//	//		var aiContentSize = GUI.skin.label.CalcSize(aiContent);
//	//		GUI.Label(new Rect(guiPoint.x, iaYPos, aiContentSize.x, 25.0f), aiContent);

//	//		if (m_Health <= 0.0f || m_Health == GetTotalHealth())
//	//			return;
//	//		var healthContent = new GUIContent($"{m_Health / m_TotalHealth * 100.0f}%");
//	//		var healthContentSize = GUI.skin.label.CalcSize(healthContent);
//	//		GUI.Label(new Rect(guiPoint.x, guiPoint.y - 25.0f, healthContentSize.x, 25.0f), healthContent);
//	//	}
//	}
//}
