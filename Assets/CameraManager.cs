/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
	public enum ECameraType
	{
		EDITOR,
		INGAME,
		FREE,
		COUNT
	}

	public struct CameraDef
	{
		//public Vector3 Offset;
		public ECameraType Type;
		public Action OnUpdate;
		public Action OnChange;
	}

	public class CameraManager : MonoBehaviour
	{
		static readonly Vector3 EditOffset = new Vector3(-11f, -18f, 0f);
		static readonly Vector3 InGameOffset = new Vector3(-17f, -14f, 0f);
		Timer WheelTimer = new Timer(false, false);
		Vector2 TargetFwdDirection = new Vector2(1f, 0f);
		const float WheelCamMoveSpeed = 3f;
		const float WheelRotationTime = 0.3f;
		public static readonly CameraDef[] CameraDefs = new CameraDef[]
		{
			// EditorCamera
			new CameraDef()
			{
				Type = ECameraType.EDITOR,
				OnUpdate = () =>
				{
					if(Mgr.prevCameraType == ECameraType.FREE)
						return;
					var target = Mgr.Target.transform.position - EditOffset;
					Mgr.transform.position = Vector3.SmoothDamp(Mgr.transform.position, target, ref Mgr.m_Speed, 0.3f);
				},
				OnChange = () =>
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					if(Mgr.prevCameraType == ECameraType.FREE)
					{
						Mgr.FreeCam._enableRotation = false;
						Mgr.FreeCam._active = false;
						Mgr.FreeCam.enabled = false;
						return;
					}
					Mgr.transform.SetPositionAndRotation(Mgr.Target.transform.position - EditOffset, Quaternion.identity);
					Mgr.transform.LookAt(Mgr.Target.transform);
				}
			},
			// InGameCamera
			new CameraDef()
			{
				Type = ECameraType.INGAME,
				OnUpdate = () =>
				{
					Vector3 point = Mgr.Target.transform.position;

					var radius = Mathf.Abs(InGameOffset.x);
					var center = new Vector3(point.x, point.y + Mathf.Abs(InGameOffset.y), point.z);
					
					
					var mgr = Manager.Mgr;
					if(!Mgr.WheelTimer.IsFinished())
					//if(mgr.GameInputControl == Def.GameInputControl.MouseLikeController && Input.GetMouseButton(2))
					{
						var camFwd = Mgr.Camera.transform.forward;
						var camFwdXZ = new Vector2(camFwd.x, camFwd.z).normalized;
						var angle = GameUtils.AngleBetween2D(Mgr.TargetFwdDirection, camFwdXZ) * Mathf.Rad2Deg;
						Mgr.transform.RotateAround(center, Vector3.up, 10f * angle * Time.deltaTime);

						var lookRotation = Quaternion.LookRotation((point - Mgr.transform.position).normalized);
						Mgr.transform.rotation = Quaternion.Slerp(Mgr.transform.rotation, lookRotation, 10f * Time.deltaTime);
					}

					var centerToCamDir = (new Vector2(Mgr.transform.position.x, Mgr.transform.position.z) - new Vector2(center.x, center.z)).normalized;
					var targetPos = new Vector3(center.x + centerToCamDir.x * radius, center.y, center.z + centerToCamDir.y * radius);
					Mgr.transform.position = Vector3.SmoothDamp(Mgr.transform.position, targetPos, ref Mgr.m_Speed, 0.3f);

					float adMovement = 0f;

					float invert = mgr.InvertCameraMovement ? 1f : -1f;
					if(mgr.GameInputControl == Def.GameInputControl.MouseLikeController)
					{
						if(Cursor.visible)
						{
							Cursor.lockState = CursorLockMode.Locked;
							Cursor.visible = false;
						}
						if(!Input.GetMouseButton(2))
						{
							float mouseOffset = invert * Input.GetAxis("Mouse X") * WheelCamMoveSpeed;
							if (mouseOffset != 0f)
								adMovement = mouseOffset;
						}
						if(Input.GetMouseButtonDown(2))
						{
							Mgr.WheelTimer.Reset(WheelRotationTime);
							var targetFwd = Mgr.Target.transform.forward;
							Mgr.TargetFwdDirection = new Vector2(targetFwd.x, targetFwd.z);
						}
					}
					else if(mgr.GameInputControl == Def.GameInputControl.Mouse)
					{
						if (Input.GetMouseButtonDown(2))
						{
							Cursor.lockState = CursorLockMode.Locked;
							Cursor.visible = false;
						}
						else if (Input.GetMouseButton(2))
						{
							float mouseOffset = invert * Input.GetAxis("Mouse X") * WheelCamMoveSpeed;
							if (mouseOffset != 0f)
								adMovement = mouseOffset;
						}
						else
						{
							Cursor.lockState = CursorLockMode.None;
							Cursor.visible = true;
						}

						if (Input.GetKey(KeyCode.LeftArrow))
						{
							adMovement = -invert * WheelCamMoveSpeed;
						}
						else if (Input.GetKey(KeyCode.RightArrow))
						{
							adMovement = invert * WheelCamMoveSpeed;
						}
						else if (Input.GetKey(KeyCode.Period))
						{
							var nPos = point - InGameOffset;
							Mgr.transform.position = nPos;
						}
					}

					Mgr.transform.RotateAround(center, Vector3.up, adMovement * 45f * Time.deltaTime);

					var look = Quaternion.LookRotation((point - Mgr.transform.position).normalized);
					Mgr.transform.rotation = Quaternion.Slerp(Mgr.transform.rotation, look, 10f * Time.deltaTime);
					Mgr.WheelTimer.Update();
				},
				OnChange = () =>
				{
					if(Mgr.prevCameraType == ECameraType.FREE)
					{
						Mgr.FreeCam._enableRotation = false;
						Mgr.FreeCam._active = false;
						Mgr.FreeCam.enabled = false;
					}
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
					Mgr.transform.SetPositionAndRotation(Mgr.Target.transform.position - EditOffset, Quaternion.identity);
					Mgr.transform.LookAt(Mgr.Target.transform);
				}
			},
			// FreeFly camera
			new CameraDef()
			{
				Type = ECameraType.FREE,
				OnUpdate = () =>
				{
					Mgr.FreeCam._enableRotation = Input.GetMouseButton(1);
				},
				OnChange = () =>
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					Mgr.FreeCam._active = true;
					Mgr.FreeCam._enableRotation = false;
					Mgr.FreeCam._initPosition = Mgr.transform.position;
					Mgr.FreeCam._initRotation = Mgr.transform.rotation.eulerAngles;
					Mgr.FreeCam.enabled = true;
				}
			},
			// Invalid camera
			new CameraDef()
			{
				Type = ECameraType.COUNT,
				OnChange = () => { },
				OnUpdate = () => { }
			}
		};

		static readonly Vector3 EditMin = new Vector3(-5f, -10f, 0f);
		static readonly Vector3 EditDefault = new Vector3(-11f, -18f, 0f);
		static readonly Vector3 EditMax = new Vector3(-49f, -50f, 0f);

		public Camera Camera;
		public GameObject DefaultTarget;

		CameraDef m_CameraDef;
		public ECameraType CameraType
		{
			get
			{
				return m_CameraDef.Type;
			}
			set
			{
				if (m_CameraDef.Type == value)
					return;

				prevCameraType = m_CameraDef.Type;

				m_CameraDef = CameraDefs[(int)value];
				m_CameraDef.OnChange();
				{
					//if (value == ECameraType.FREE)
					//{
					//    m_FreeCam._active = true;
					//    m_FreeCam._enableRotation = false;
					//    m_FreeCam._initPosition = transform.position;
					//    m_FreeCam._initRotation = transform.rotation.eulerAngles;
					//    m_FreeCam.enabled = true;
					//}
					//if (prevCameraType == ECameraType.FREE)
					//{
					//    m_FreeCam._enableRotation = false;
					//    m_FreeCam._active = false;
					//    m_FreeCam.enabled = false;
					//}
					//m_CameraDef = CameraDefs[(int)value];
					//if (value == ECameraType.INGAME || (value == ECameraType.EDITOR && prevCameraType != ECameraType.FREE))
					//{
					//    transform.SetPositionAndRotation(RotatingTarget.transform.position - m_CameraDef.Offset, Quaternion.identity);
					//    transform.LookAt(RotatingTarget.transform);
					//}
				}
			}
		}
		private ECameraType prevCameraType;
		//private static string[] CameraModeStr = new string[]
		//{
		//    "EDITOR",
		//    "INGAME",
		//    "FREE"
		//};

		public static CameraManager Mgr;

		public GameObject Target;
		public FreeFlyCamera FreeCam;
		public Canvas Canvas;
		public UnityEngine.UI.CanvasScaler CanvasScaler;
		public RectTransform CanvasRT;
		Vector3 m_Speed;
		//Vector2 m_LastMousePos;

		private void Awake()
		{
			Mgr = this;
			m_CameraDef = CameraDefs[(int)ECameraType.COUNT];
		}

		//public static void Init()
		//{
		//    //var camMgr = Manager.Mgr.m_Camera.gameObject.AddComponent<CameraManager>();
		//    //Manager.Mgr.CamMgr = camMgr;
		//    //camMgr.cameraDef.Type = ECameraType.COUNT;
		//    //var player = Manager.Mgr.OddGO;
		//    //camMgr.CameraType = ECameraType.EDITOR;
		//    //camMgr.FreeCam = camMgr.GetComponent<FreeFlyCamera>();
		//}

		//private void OnGUI()
		//{
		//    if (!Manager.Mgr.IsLoadFinished || Manager.Mgr.HideInfo)
		//        return;
		//    var rect = Manager.Mgr.m_Canvas.pixelRect;
		//    var selControl = GUI.SelectionGrid(
		//        new Rect(rect.width - 225, 30, 200, 25), (int)cameraDef.Type, CameraModeStr, (int)GameState.COUNT);
		//    if (selControl != (int)cameraDef.Type)
		//        CameraType = (ECameraType)selControl;
		//}

		//private void FixedUpdate()
		//{
		//    //if (!Manager.Mgr.IsLoadFinished)
		//    //    return;
		//}

		// Update is called once per frame
		void Update()
		{
			if (!Target)
				Target = DefaultTarget;
			m_CameraDef.OnUpdate();
			{
				////if (!Manager.Mgr.IsLoadFinished)
				////    return;
				////Vector3 offset = Vector3.zero;
				////Vector3 Target = Vector3.zero;
				////float xMovement = 0.0f;
				////float yMovement = 0.0f;
				////float zMovement = 0.0f;
				////Vector3 newPos = Vector3.zero;
				//switch (m_CameraDef.Type)
				//{
				//    case ECameraType.INGAME:
				//        //Target = Odd.Position - cameraDef.Offset;
				//        Vector3 point = RotatingTarget.transform.position;

				//        var radius = Mathf.Abs(m_CameraDef.Offset.x);
				//        var center = new Vector3(point.x, point.y + Mathf.Abs(m_CameraDef.Offset.y), point.z);
				//        var centerToCamDir = (new Vector2(transform.position.x, transform.position.z) - new Vector2(center.x, center.z)).normalized;
				//        var targetPos = new Vector3(center.x + centerToCamDir.x * radius, center.y, center.z + centerToCamDir.y * radius);

				//        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref m_Speed, 0.3f);

				//        //{
				//        //    Vector3 point = Vector3.zero;
				//        //    if (RotatingTarget == null)
				//        //        point = Odd.Position;
				//        //    else
				//        //    {
				//        //        point = RotatingTarget.transform.position;
				//        //    }

				//        //    var center = point;
				//        //    center.y -= cameraDef.Offset.y;
				//        //    var radius = Mathf.Abs(cameraDef.Offset.x);
				//        //    var yOffset = center.y - transform.position.y;

				//        //    // Height
				//        //    if (Mathf.Abs(offset.y) > 10.0f)
				//        //    {
				//        //        yMovement = yOffset;
				//        //    }
				//        //    else
				//        //    {
				//        //        yMovement = GameUtils.LinearMovement1D(transform.position.y, center.y, 1.75f * Time.deltaTime);
				//        //        float ySpeed = m_Speed.y;
				//        //        float nYPos = Mathf.SmoothDamp(transform.position.y, center.y, ref ySpeed, 0.3f);
				//        //        m_Speed.Set(m_Speed.x, ySpeed, m_Speed.z);
				//        //        newPos.y = nYPos;
				//        //    }
				//        //    //var currentRadius = Vector2.Distance(new Vector2(center.x, center.z), new Vector2(transform.position.x, transform.position.z));
				//        //    //var radiusDiff = Mathf.Abs(currentRadius - radius);
				//        //    var dirToCenter = new Vector2(center.x, center.z) - new Vector2(transform.position.x, transform.position.z);
				//        //    dirToCenter.Normalize();
				//        //    var dirToCam = new Vector2(transform.position.x, transform.position.z) - new Vector2(center.x, center.z);
				//        //    dirToCam.Normalize();


				//        //    xMovement = dirToCenter.x * Time.deltaTime;
				//        //    zMovement = dirToCenter.y * Time.deltaTime;

				//        //    var destination = new Vector2(transform.position.x + xMovement, transform.position.z + zMovement);
				//        //    var target = new Vector2(dirToCam.x * radius, dirToCam.y * radius) + new Vector2(center.x, center.z);
				//        //    var targetDist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), target);
				//        //    var destDist = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), destination);
				//        //    if (Mathf.Abs(targetDist) < Mathf.Abs(destDist))
				//        //        destination = target;

				//        //    //xMovement = destination.x - transform.position.x;
				//        //    //zMovement = destination.y - transform.position.z;

				//        //    //xMovement = target.x - transform.position.x;
				//        //    //zMovement = target.y - transform.position.z;
				//        //    float xSpeed = m_Speed.x;
				//        //    float zSpeed = m_Speed.z;
				//        //    float nXPos = Mathf.SmoothDamp(transform.position.x, target.x, ref xSpeed, 0.3f);
				//        //    float nZPos = Mathf.SmoothDamp(transform.position.z, target.y, ref zSpeed, 0.3f);
				//        //    m_Speed.Set(xSpeed, m_Speed.y, zSpeed);
				//        //    newPos.x = nXPos;
				//        //    newPos.z = nZPos;

				//        //    transform.position = newPos;
				//        //}
				//        //transform.Translate(new Vector3(xMovement, yMovement, zMovement), Space.World);

				//        float adMovement = 0.0f;

				//        if (Input.GetMouseButtonDown(2))
				//        {
				//            //Cursor.lockState = CursorLockMode.Locked;

				//            //m_LastMousePos.Set(Screen.width * 0.5f, Screen.height * 0.5f);
				//            //m_LastMousePos.Set(Input.mousePosition.x, Input.mousePosition.y);
				//            Cursor.lockState = CursorLockMode.Locked;
				//            Cursor.visible = false;
				//        }
				//        else if (Input.GetMouseButton(2))
				//        {
				//            //var nMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

				//            //var offset = nMousePos - m_LastMousePos; // - new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
				//            ////offset.Set((offset.x + 0.5f) * 2f, (offset.y + 0.5f) * 2f);
				//            //offset.Set((offset.x / Screen.width) * 100f, (offset.y / Screen.height) * 100f);
				//            //if (offset.x > 0f)
				//            //    adMovement = -offset.x;
				//            //else if (offset.x < 0f)
				//            //    adMovement = -offset.x;


				//            float mouseOffset = -Input.GetAxis("Mouse X") * 1.5f;//(Input.mousePosition.x - Screen.width * 0.5f) + 0.5f;
				//            if (mouseOffset != 0f)
				//                adMovement = mouseOffset;
				//            //if (mouseOffset > 0f)
				//            //    adMovement = mouseOffset;
				//            //else if (mouseOffset < 0f)
				//            //    adMovement = +1f;
				//            //Debug.Log(mouseOffset);

				//            //if (nMousePos.x == 0f && m_LastMousePos.x > 0f)
				//            //{

				//            //}
				//            //else if (nMousePos.y == 0f && m_LastMousePos.y > 0)
				//            //{

				//            //}
				//            //else if (nMousePos.x == Screen.width && m_LastMousePos.x < Screen.width)
				//            //{

				//            //}
				//            //else if (nMousePos.y == Screen.height && m_LastMousePos.y < Screen.height)
				//            //{

				//            //}
				//            //m_LastMousePos = nMousePos;
				//        }
				//        else if (Input.GetMouseButtonUp(2))
				//        {
				//            Cursor.lockState = CursorLockMode.None;
				//            Cursor.visible = true;
				//        }

				//        if (Input.GetKey(KeyCode.LeftArrow))
				//        {
				//            adMovement = +1.0f;
				//        }
				//        else if (Input.GetKey(KeyCode.RightArrow))
				//        {
				//            adMovement = -1.0f;
				//        }
				//        else if (Input.GetKey(KeyCode.C))
				//        {
				//            var nPos = point - m_CameraDef.Offset;
				//            transform.position = nPos;
				//        }

				//        transform.RotateAround(center, Vector3.up, adMovement * 45.0f * Time.deltaTime);

				//        //if (adMovement != 0.0f)
				//        //{
				//        //    //var angle = Mathf.Acos(dirToCam.x);
				//        //    //var angleZ = Mathf.Asin(dirToCam.y);
				//        //    //angle += adMovement * Time.deltaTime;
				//        //    //var nDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
				//        //    //var nPos = new Vector2(center.x, center.z) + nDir * radius;
				//        //    //transform.Translate(new Vector3(nPos.x - transform.position.x, 0.0f, nPos.y - transform.position.z), Space.World);

				//        //    //var angleY = Mathf.Asin(dirToCam.y);
				//        //    //angleX += adMovement;
				//        //    //angleY += adMovement;

				//        //    //if (angleY < 0.0f)
				//        //    //    angleX += Mathf.PI;
				//        //    //if (angleX > (2.0f * Mathf.PI))
				//        //    //    angleX = angleX - (2.0f * Mathf.PI);
				//        //    //if (angleX < 0.0f)
				//        //    //    angleX = angleX + (2.0f * Mathf.PI);


				//        //    //var nAngleX = Mathf.Cos(angleX)

				//        //    //var nextPosX = radius * Mathf.Cos(2.0f * Mathf.PI * 0.001f * Time.time + Mathf.Acos(dirToCam.x) + adMovement);
				//        //    //var nextPosZ = radius * Mathf.Cos(2.0f * Mathf.PI * 0.001f * Time.time - Mathf.Acos(dirToCam.y) + adMovement);

				//        //    //var tX = Mathf.Acos(dirToCam.x) + adMovement;
				//        //    //if (tX > (Mathf.PI))
				//        //    //    tX = tX - (2.0f * Mathf.PI);
				//        //    //if (tX < 0.0f)
				//        //    //    tX = tX + (2.0f * Mathf.PI);
				//        //    //var dirX = Mathf.Cos(tX);
				//        //    //var nX = dirX * radius + center.x;

				//        //    //var tZ = Mathf.Asin(dirToCam.y) * 2.0f + adMovement;
				//        //    //if (tZ > (2.0f * Mathf.PI))
				//        //    //    tZ = tZ - (2.0f * Mathf.PI);
				//        //    //if (tZ < 0.0f)
				//        //    //    tZ = tZ + (2.0f * Mathf.PI);
				//        //    //var dirZ = Mathf.Sin(tZ);
				//        //    //var nZ = dirZ * radius + center.z;

				//        //    //var t = Mathf.Acos((transform.position.x - center.x) / radius);
				//        //    //var t2 = Mathf.Asin((transform.position.z - center.z) / radius);
				//        //    //var t3 = Mathf.Asin((transform.position.x - center.x) / radius);
				//        //    //var t4 = Mathf.Acos((transform.position.z - center.z) / radius);

				//        //    //var nX = center.x * Mathf.Cos(t + Time.deltaTime * adMovement);
				//        //    //var nX2 = center.x * Mathf.Sin(t3 + Time.deltaTime * adMovement);
				//        //    //var nZ = center.z * Mathf.Sin(t2 + Time.deltaTime * adMovement);
				//        //    //var nZ2 = center.z * Mathf.Cos(t4 + Time.deltaTime * adMovement);

				//        //    //var off = GameUtils.LinearMovement2D(new Vector2(transform.position.x, transform.position.z), new Vector2(nX, nZ), 3.0f * Time.deltaTime);
				//        //    //transform.Translate(new Vector3(off.x, 0.0f, off.y));
				//        //    //transform.Translate(new Vector3(nextPosX - transform.position.x, 0.0f, nextPosZ - transform.position.z), Space.World);
				//        //    //transform.Translate(new Vector3(nX - transform.position.x, 0.0f, nZ - transform.position.z), Space.World);
				//        //}
				//        var look = Quaternion.LookRotation((point - transform.position).normalized);
				//        transform.rotation = Quaternion.Slerp(transform.rotation, look, 10.0f * Time.deltaTime);
				//        //transform.LookAt(point);
				//        break;

				//    case ECameraType.EDITOR:
				//        if (prevCameraType == ECameraType.FREE)
				//        {
				//            break;
				//        }
				//        var target = RotatingTarget.transform.position - m_CameraDef.Offset;
				//        //offset = Target - transform.position;
				//        //if (Mathf.Abs(offset.x) > 10.0f || Mathf.Abs(offset.y) > 10.0f || Mathf.Abs(offset.z) > 10.0f)
				//        //    transform.Translate(offset, Space.World);


				//        //xMovement = GameUtils.LinearMovement1D(transform.position.x, Target.x, 1.75f * Time.deltaTime);
				//        //yMovement = GameUtils.LinearMovement1D(transform.position.y, Target.y, 1.75f * Time.deltaTime);
				//        //zMovement = GameUtils.LinearMovement1D(transform.position.z, Target.z, 1.75f * Time.deltaTime);
				//        transform.position = Vector3.SmoothDamp(transform.position, target, ref m_Speed, 0.3f);
				//        //transform.Translate(new Vector3(xMovement, yMovement, zMovement), Space.World);
				//        break;

				//    case ECameraType.FREE:
				//        m_FreeCam._enableRotation = Input.GetMouseButton(1); // right
				//        break;
				//    default:
				//        break;
				//}
			}
		}
	}
}