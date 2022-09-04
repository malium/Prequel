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
//	public enum SpriteVertical
//	{
//		DOWN,
//		UP,
//		COUNT
//	}
//	public enum SpriteHorizontal
//	{
//		LEFT,
//		RIGHT,
//		COUNT
//	}

//	public struct SpriteFacing
//	{
//		public SpriteVertical Vertical;
//		public SpriteHorizontal Horizontal;

//		public override bool Equals(object obj)
//		{
//			if (!(obj is SpriteFacing))
//			{
//				return false;
//			}

//			var facing = (SpriteFacing)obj;
//			return Vertical == facing.Vertical &&
//				   Horizontal == facing.Horizontal;
//		}
//		public override int GetHashCode()
//		{
//			var hashCode = 812912412;
//			hashCode = hashCode * -1521134295 + Vertical.GetHashCode();
//			hashCode = hashCode * -1521134295 + Horizontal.GetHashCode();
//			return hashCode;
//		}
//		static public bool operator==(SpriteFacing left, SpriteFacing right)
//		{
//			return left.Vertical == right.Vertical
//				&& left.Horizontal == right.Horizontal;
//		}
//		static public bool operator!=(SpriteFacing left, SpriteFacing right)
//		{
//			return !(left == right);
//		}
//	}
	
//	public class PropScript : LivingEntity
//	{
//		public static int PropID = 0;
//		SpriteBackendSprite m_Shadow;
//		SpriteBackendSQuad m_Sprite;
//		//SpriteRenderer m_ShadowSR;
//		BoxCollider m_SpriteBC;
//		//SpriteRenderer m_SpriteSR;
//		Light m_Light;
//		PropInfo m_Prop;
//		SpriteFacing m_Facing;
//		//float m_SpriteYOffset;
//		public IBlock Block;
//		float m_LastReceiveDamageTime;
//		public SpriteRenderer ShadowSR
//		{
//			get
//			{
//				//return m_ShadowSR;
//				if (m_Shadow == null)
//					return null;
//				return m_Shadow.GetRenderer() as SpriteRenderer;
//			}
//		}
//		public Renderer SpriteSR => m_Sprite.GetRenderer();
//		//public SpriteRenderer SpriteSR
//		//{
//		//    get
//		//    {
//		//        return m_SpriteSR;
//		//    }
//		//}
//		public BoxCollider SpriteBC
//		{
//			get
//			{
//				return m_SpriteBC;
//			}
//		}
//		public Light PropLight
//		{
//			get
//			{
//				return m_Light;
//			}
//		}
//		public PropInfo Prop
//		{
//			get
//			{
//				return m_Prop;
//			}
//		}
//		public SpriteFacing Facing
//		{
//			get
//			{
//				return m_Facing;
//			}
//			set
//			{
//				var facing = value;
//				if (m_Facing == facing)
//					return;
//				if((m_Facing.Horizontal == SpriteHorizontal.LEFT && facing.Horizontal == SpriteHorizontal.RIGHT)
//					|| (m_Facing.Horizontal == SpriteHorizontal.RIGHT && facing.Horizontal == SpriteHorizontal.LEFT))
//				{
//					m_Sprite.Flip(!m_Sprite.IsHorizontalFlip(), m_Sprite.IsVerticalFlip());
//					//m_SpriteSR.flipX = !m_SpriteSR.flipX;
//					//m_SpriteSR.material.SetVector(AssetContainer.FlipShaderID, new Vector4(m_SpriteSR.flipX ? -1f : 1f, m_SpriteSR.flipY ? -1f : 1f, 1f, 1f));
//					m_SpriteBC.center = new Vector3(-m_SpriteBC.center.x, m_SpriteBC.center.y, m_SpriteBC.center.z);
//				}
//				else
//				{
//					return;
//				}


//				m_Facing = facing;
//			}
//		}

//		public PropScript()
//			: base(new LivingEntityDef()
//			{
//				TotalHealth = 30f,
//				Speed = 0f,
//				AngularSpeed = 0f,
//				Radius = 0f,
//				Height = 1f,
//				MaxJump = 0f,
//				FlySpeed = 0f,
//				FallSpeed = 0f,
//				LEType = Def.LivingEntityType.Prop
//			})
//			//: base(30.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, Def.LivingEntityType.Prop)
//		{
//			AI.Resistance cutResistance;
//			cutResistance.Type = Def.ResistanceType.PHYSICAL;
//			cutResistance.Value = 0.2f;
//			SetResistance(cutResistance);
//		}

//		public void SetProp(int familyID, int propID)
//		{
//			m_Prop = Props.PropFamilies[familyID].Props[propID];

//			m_Sprite = SpriteUtils.AddSprite(
//				new GameObject(gameObject.name + "_Sprite"), 
//				SpriteBackendType.SQUAD,
//				m_Prop.PropSprite) as SpriteBackendSQuad;
//			m_Sprite.transform.SetParent(transform);
//			var texSize = m_Prop.PropSprite.texture.width;
//			float scale = texSize / m_Prop.PropSprite.pixelsPerUnit;
//			var pivot = m_Prop.PropSprite.pivot / m_Prop.PropSprite.pixelsPerUnit;

//			//var pivot = m_Prop.PropSprite.pivot;
//			//m_SpriteYOffset = pivot.y * texSize - m_Prop.GetVisibleRect().y;
//			//m_SpriteYOffset /= texSize;
//			//m_SpriteYOffset *= scale * 0.5f;
//			//m_Sprite.transform.localPosition = new Vector3(0f, 0.02f + m_SpriteYOffset, 0f);
//			m_Sprite.transform.localScale = new Vector3(m_Prop.SpriteScale, m_Prop.SpriteScale, 1f);
//			///m_Sprite.transform.localPosition = new Vector3(pivot.x, -pivot.y + 0.02f + m_SpriteYOffset, 0f);
//			///m_Sprite.transform.localScale = new Vector3(scale * m_Prop.SpriteScale, scale * m_Prop.SpriteScale, 1f);
//			gameObject.layer = Def.RCLayerLE;

//			//m_SpriteSR = new GameObject(gameObject.name + "_Sprite").AddComponent<SpriteRenderer>();
//			//m_SpriteSR.transform.SetParent(transform);
//			//SpriteUtils.InitSpriteLit(m_SpriteSR, m_Prop.PropSprite);
//			//SpriteUtils.InitSprite(m_SpriteSR, m_Prop.sprite, Materials.GetMaterial(Def.Materials.Sprite));
//			m_Facing.Vertical = SpriteVertical.UP;
//			m_Facing.Horizontal = SpriteHorizontal.LEFT;
//			//m_SpriteSR.transform.localScale = new Vector3(m_Prop.SpriteScale, m_Prop.SpriteScale, 1.0f);
//			//m_SpriteSR.tag = TAG;
//			m_SpriteBC = /*m_SpriteSR.*/gameObject.AddComponent<BoxCollider>();
//			m_SpriteBC.size = new Vector3(m_Prop.GetBoxWidth() * scale, m_Prop.GetBoxSize().y, m_Prop.GetBoxWidth() * scale);
//			m_SpriteBC.center = new Vector3(0f, m_Prop.PropSprite.bounds.center.y + m_Prop.GetBoxCenterOffset().y, 0f);
//			//m_SpriteBC.tag = TAG;
//			//m_SpriteBC.size = new Vector3(m_Prop.BoxSize.x, m_Prop.BoxSize.y, 0.01f);
//			//m_SpriteBC.center = new Vector3(m_SpriteSR.sprite.bounds.center.x + m_Prop.BoxCenterOffset.x,
//			//    m_SpriteSR.sprite.bounds.center.y + m_Prop.BoxCenterOffset.y, 0.0f);
//			//m_SpriteBC.size = new Vector3(m_Prop.GetBoxWidth() * m_SpriteSR.size.x, m_Prop.GetBoxSize().y, m_Prop.GetBoxWidth() * m_SpriteSR.size.x);
//			//m_SpriteBC.center = new Vector3(0.0f,
//			//    m_SpriteSR.sprite.bounds.center.y + m_Prop.GetBoxCenterOffset().y, 0.0f);

//			if (m_SpriteBC.size.y < 1.0f)
//			{
//				var halfDiff = (1.0f - m_SpriteBC.size.y) * 0.5f;
//				var center = m_SpriteBC.center;
//				center.y = center.y + halfDiff;
//				m_SpriteBC.center = center;
//				var size = m_SpriteBC.size;
//				size.y = 1.0f;
//				m_SpriteBC.size = size;
//			}
//			m_Height = m_SpriteBC.size.y;

//			m_Radius = (m_Prop.GetVisibleRect().width * 0.5f) / m_Prop.PropSprite.pixelsPerUnit;
			
			

//			if (m_Prop.HasShadow)
//			{
//				m_Shadow = SpriteUtils.AddSprite(
//					new GameObject(gameObject.name + "_Shadow"),
//					SpriteBackendType.SPRITE,
//					AssetContainer.Mgr.SpriteShadow) as SpriteBackendSprite;
//				m_Shadow.transform.SetParent(transform);
//				var shadowSize = AssetContainer.Mgr.SpriteShadowTex.width;
//				var shadowOffset = 0.5f + shadowSize * 0.001f;
//				//m_Shadow.transform.position = transform.position;
//				m_Shadow.transform.localPosition = new Vector3(-shadowOffset, 0.01f, -shadowOffset);
//				m_Shadow.transform.Rotate(Vector3.right, 90f, Space.Self);
//				//m_Shadow.transform.Translate(-(scale * 0.1f), 0f, -(scale * 0.1f), Space.Self);
//				//m_ShadowSR = new GameObject(gameObject.name + "_Shadow").AddComponent<SpriteRenderer>();
//				//m_ShadowSR.transform.Translate(transform.position, Space.World);
//				//m_ShadowSR.transform.Translate(new Vector3(-0.5f, 0.01f, -0.5f), Space.World);
//				//m_ShadowSR.transform.Rotate(Vector3.right, 90.0f, Space.World);
//				//m_ShadowSR.transform.Translate(new Vector3(-0.128f, 0.0f, -0.128f), Space.World);
//				//m_ShadowSR.transform.SetParent(transform);
//				//var shadowTex = AssetContainer.Mgr.SpriteShadow;
//				//SpriteUtils.InitSprite(m_ShadowSR,
//				//    Sprite.Create(shadowTex, new Rect(0.0f, 0.0f, shadowTex.width, shadowTex.height), new Vector2(0.0f, 0.0f), 100, 0, SpriteMeshType.FullRect),
//				//    Materials.GetMaterial(Def.Materials.Sprite));
//			}

//			if(m_Prop.HasLighting)
//			{
//				m_Light = new GameObject(gameObject.name + "_Light").AddComponent<Light>();
//				m_Light.transform.SetPositionAndRotation(gameObject.transform.position, Quaternion.identity);
//				m_Light.transform.SetParent(transform);
//				m_Light.transform.Translate(new Vector3(0f, m_Prop.LightHeight, 0f), Space.Self);
//				m_Light.intensity = m_Prop.LightIntensity;
//				m_Light.color = m_Prop.LightColor;
//				m_Light.range = m_Prop.LightRange;
//			}

//			//gameObject.tag = TAG;
//		}

//		public override Collider GetCollider()
//		{
//			return m_SpriteBC;
//		}

//		void CheckHealth()
//		{
//			//if (m_Health <= 0)
//			//{
//			//	//if (Manager.Mgr.CurrentControllerSel == (int)GameState.PLAY && Block.GetPilar().GetMapID() >= 0)
//			//	//{
//			//	//	Manager.Mgr.Map.Record(
//			//	//		new MapCommand(MapCommandType.PROP_DEATH, m_Prop.Family.FamilyName, Block.GetPilar().GetMapID(), Block.GetPilar().GetBlocks().IndexOf(Block)));
//			//	//}
//			//	Block.SetProp(null);
//			//	if (Block.GetPilar().GetStruc() != null && Block.GetPilar().GetStruc().GetLivingEntities().Contains(this))
//			//		Block.GetPilar().GetStruc().GetLivingEntities().Remove(this);
//			//	m_SpriteBC.enabled = false;
//			//}
//		}

//		public void CastDamageParticles(Vector3 pos, Vector3 dir)
//		{
//			//var rng = Manager.Mgr.DamageRNG;
//			//var particleCount = rng.Next(3, 4);
//			//for(int i = 0; i < particleCount; ++i)
//			//{
//			//    var particle = new GameObject(gameObject.name + "_Particle").AddComponent<PropDamageParticle>();
//			//    particle.transform.position = pos;
//			//    Vector3 dirOffset = new Vector3((float)rng.NextDouble() * 0.4f - 0.2f, (float)rng.NextDouble() * 0.4f - 0.2f, (float)rng.NextDouble() * 0.4f - 0.2f);
//			//    var particleDir = (dir + dirOffset).normalized;
//			//    particle.Set(m_Prop.DamageSprite, particleDir);
//			//}
//			//if (m_Health <= 0f)
//			//{
//			//    particleCount = (int)Mathf.Ceil(particleCount * ((float)rng.NextDouble() + 3f)) - particleCount;
//			//    for (int i = 0; i < particleCount; ++i)
//			//    {
//			//        var particle = new GameObject(gameObject.name + "_Particle").AddComponent<PropDamageParticle>();
//			//        particle.transform.position = pos;
//			//        Vector3 particleDir = new Vector3((float)rng.NextDouble() * 2f - 1f, dir.y + (float)rng.NextDouble() * 0.4f - 0.2f, (float)rng.NextDouble() * 2f - 1f);
//			//        particleDir.Normalize();
//			//        particle.Set(m_Prop.DamageSprite, particleDir);
//			//    }
//			//}
//		}

//		protected override void OnReceiveDamage()
//		{
//			m_LastReceiveDamageTime = Time.time;
//			CheckHealth();
//		}

//		protected override void OnReceiveElement()
//		{
//			CheckHealth();
//		}

//		void DamageAnimationUpdate()
//		{
//			//var timeOffset = (m_LastReceiveDamageTime + 1f) - Time.time;
//			//var defaultScale = m_Prop.SpriteScale;
//			//float t = 0f;
//			//float xScale = 0f;
//			//float yScale = 0f;
//			//float timeScale = 1f / 6f;

//			//if(timeOffset > (5f * timeScale))
//			//{
//			//    t = 1f - timeOffset;
//			//    t = t / timeScale;
//			//    xScale = (1f - t) * defaultScale + t * (defaultScale * 0.8f);
//			//    yScale = (1f - t) * defaultScale + t * (defaultScale * 1.2f);
//			//}
//			//else if(timeOffset > (4f * timeScale))
//			//{
//			//    t = (5f * timeScale) - timeOffset;
//			//    t = t / timeScale;
//			//    xScale = (1f - t) * (defaultScale * 0.8f) + t * (defaultScale);
//			//    yScale = (1f - t) * (defaultScale * 1.2f) + t * (defaultScale);
//			//}
//			//else if(timeOffset > (3f * timeScale))
//			//{
//			//    t = (4f * timeScale) - timeOffset;
//			//    t = t / timeScale;
//			//    xScale = (1f - t) * (defaultScale) + t * (defaultScale * 0.9f);
//			//    yScale = (1f - t) * (defaultScale) + t * (defaultScale * 1.1f);
//			//}
//			//else if(timeOffset > (2f * timeScale))
//			//{
//			//    t = (3f * timeScale) - timeOffset;
//			//    t = t / timeScale;
//			//    xScale = (1f - t) * (defaultScale * 0.9f) + t * (defaultScale);
//			//    yScale = (1f - t) * (defaultScale * 1.1f) + t * (defaultScale);
//			//}
//			//else if (timeOffset > (timeScale))
//			//{
//			//    t = (2f * timeScale) - timeOffset;
//			//    t = t / timeScale;
//			//    xScale = (1f - t) * (defaultScale) + t * (defaultScale * 0.95f);
//			//    yScale = (1f - t) * (defaultScale) + t * (defaultScale * 1.05f);
//			//}
//			//else if (timeOffset > 0f)
//			//{
//			//    t = (timeScale) - timeOffset;
//			//    t = t / timeScale;
//			//    xScale = (1f - t) * (defaultScale * 0.95f) + t * (defaultScale);
//			//    yScale = (1f - t) * (defaultScale * 1.05f) + t * (defaultScale);
//			//}
//			//else
//			//{
//			//    xScale = defaultScale;
//			//    yScale = defaultScale;
//			//}
//			//m_Sprite.transform.localScale = new Vector3(xScale, yScale, 1f);
//		}

//		private void Update()
//		{
//			//if(m_InPainTime > Time.time)
//			//{
//			//    m_SpriteSR.color = new Color(1.0f, 0.0f, 0.0f, m_SpriteSR.color.a);
//			//}
//			//else
//			//{
//			//    m_SpriteSR.color = new Color(1.0f, 1.0f, 1.0f, m_SpriteSR.color.a);
//			//}
//			//DamageAnimationUpdate();
//			var color = m_Sprite.GetColor();
//			if (m_Health <= 0.0f)
//			{
//				color.a -= color.a * Time.deltaTime * 2.5f;
//			}
//			if (GameUtils.IsNearlyEqual(color.a, 0.0f)) // die
//			{
//				GameUtils.DeleteGameObjectAndItsChilds(gameObject, true);
//				return;
//			}
//			else
//			{
//				m_Sprite.SetColor(color);
//				//m_SpriteSR.material.color = color;
//				if (m_Prop.HasShadow)
//					m_Shadow.SetColor(color);
//					//m_ShadowSR.color = color;
//				if(m_Prop.HasLighting)
//					m_Light.intensity = m_Prop.LightIntensity * color.a;
//			}
//			if (m_Health <= 0.0f)
//				return;

//			if (Block == null)
//				throw new Exception("Prop has no block assigned");
//			var pilarPos = Block.GetPilar().transform.position;
//			transform.position = new Vector3(pilarPos.x + 0.5f, Block.GetHeight() + Block.GetMicroHeight(), pilarPos.z + 0.5f);
//			//var pos = new Vector3(Block.GetPilar().transform.position.x + 0.5f, Block.GetHeight() + Block.GetMicroHeight(), Block.GetPilar().transform.position.z + 0.5f);
//			//var offset = pos - transform.position;
//			//transform.Translate(offset, Space.World);
//			if (Manager.Mgr.CurrentControllerSel != (int)GameState.PLAY)
//				return;

//			RegisterLE(false);
//			//if((offset.x != 0.0f || offset.z != 0.0f) && Block.Pilar.Struc != null)
//			//{
//			//    if (!Block.Pilar.Struc.LivingEntities.Contains(this))
//			//        Block.Pilar.Struc.LivingEntities.Add(this);
//			//}

//			ElementUpdate();
//			//AIUpdate();
//		}

//		private void OnEnable()
//		{
//			if (m_Sprite == null)
//				return;
//			if(m_Shadow != null)
//				m_Shadow.enabled = true;
//			if (m_Light != null)
//				m_Light.enabled = true;
//			m_Sprite.enabled = true;
//		}

//		private void OnDisable()
//		{
//			if (m_Sprite == null)
//				return;
//			if (m_Shadow != null)
//				m_Shadow.enabled = false;
//			if (m_Light != null)
//				m_Light.enabled = false;
//			m_Sprite.enabled = false;
//		}
		
//		private void LateUpdate()
//		{
//			//if (false)
//			//{
//			//    //var oldPos = transform.position;
//			//    //var sprite = GetComponent<SpriteRenderer>().sprite;
//			//    //var height = sprite.texture.height;
//			//    //var defPos = oldPos;
//			//    //defPos.y -= sprite.pivot.y * height;



//			//    transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
//			//    transform.Translate(transform.parent.position, Space.World);
//			//    transform.Translate(new Vector3(m_BlockOffset, m_YOffset, m_BlockOffset), Space.Self);
//			//    transform.LookAt(Camera.transform);
//			//    var camDir = (Camera.transform.position - transform.position).normalized;
//			//    var sr = GetComponent<SpriteRenderer>();
//			//    var pivot = sr.sprite.pivot;
//			//    var lastPixel = m_Prop.LastPixel;
//			//    var yOffset = pivot.y - lastPixel.y;
//			//    yOffset /= sr.sprite.texture.height;
//			//    //var xOffset = pivot.x - lastPixel.x;
//			//    //xOffset /= sr.sprite.texture.height;

//			//    //var angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(transform.up, Vector3.up));
//			//    //angle = 90.0f - angle;
//			//    //var hypo = Mathf.Cos(Mathf.Deg2Rad * angle) / yOffset;
//			//    //var move = hypo * camDir;
//			//    //transform.Translate(move, Space.Self);



//			//    var move = yOffset * camDir.y * sr.sprite.texture.height / 64.0f;
//			//    transform.Translate(camDir * move, Space.Self);
//			//    //transform.Translate(new Vector3(0.0f, yOffset, 0.0f), Space.Self);






//			//    //transform.Translate(new Vector3(0.0f, yOffset, 0.0f), Space.Self);
//			//    //var d = (yOffset * yOffset) / xOffset;
//			//    //var camD = d * transform.forward;
//			//    //camD.y = 0.0f;
//			//    //transform.Translate(camD, Space.Self);

//			//    //transform.Translate(defPos, Space.World);
//			//    //transform.Translate(new Vector3(0.0f, sprite.pivot.y * height, 0.0f), Space.World);
//			//}


//			//m_Sprite.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
//			var posY = transform.position.y;
//			//m_SpriteGO.transform.Translate(, Space.World);
//			//m_Sprite.transform.Translate(new Vector3(0.0f, 0.02f, 0.0f), Space.Self); // block offset
//			var camPos = CameraManager.Mgr.Camera.transform.position;
//			//var camPos = Manager.Mgr.m_Camera.transform.position;
//			//var yOffset = camPos.y - posY;
//			m_Sprite.transform.LookAt(new Vector3(camPos.x, posY + 1f, camPos.z));
//			//m_SpriteSR.transform.LookAt(new Vector3(camPos.x, posY + 1.0f, camPos.z));
//			//m_SpriteGO.transform.LookAt(Manager.Mgr.m_Camera.transform);
//			//var pos = transform.position;
//			//pos.y += m_SpriteYOffset;

//			//transform.Translate(pos, Space.World);
//			//m_SpriteSR.transform.Translate(new Vector3(0.0f, m_SpriteYOffset, 0.0f), Space.World);
//		}

//		//public void TakeDamage(float amount)
//		//{
//		//    if (amount <= 0.0f)
//		//        return;
			
//		//    m_Health -= amount;
//		//}

//		public void OnGUI()
//		{
//			if (m_Health <= 0.0f || m_Health == GetTotalHealth() || Manager.Mgr.HideInfo)
//				return;
//			//var viewPoint = Manager.Mgr.m_Camera.WorldToViewportPoint(transform.position);
//			//var screenPoint = Manager.Mgr.m_Camera.ViewportToScreenPoint(viewPoint);
//			var wPos = transform.position;
//			wPos.y += (m_Prop.PropSprite.rect.height / m_Prop.PropSprite.pixelsPerUnit) * m_Prop.SpriteScale;
//			var screenPoint = Manager.Mgr.m_Camera.WorldToScreenPoint(wPos);
//			if (screenPoint.x < 0.0f || screenPoint.y < 0.0f || screenPoint.x > Screen.width || screenPoint.y > Screen.height)
//				return;

//			var canvas = Manager.Mgr.m_Canvas;
//			var rect = canvas.pixelRect;
//			screenPoint.y = Screen.height - screenPoint.y;
//			var guiPoint = GUIUtility.ScreenToGUIPoint(screenPoint);

//			GUI.Label(new Rect(guiPoint.x, guiPoint.y, 50.0f, 25.0f), $"{m_Health}%");
//		}
//	}
//}
