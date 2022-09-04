/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.UI
{
	public class WorldGenUI : MonoBehaviour
	{
		const int ViewStrucSide = 5;
		const int ViewSide = CStruc.Height * ViewStrucSide; //World.WStruc.Size * ViewStrucSide;
		const int ViewArea = ViewSide * ViewSide;
		//const float PlacementDelay = 0.5f;
		public MainMenuUI MainMenu;
		//public string StructureName = "SmallIsland_000";
		public Vector2Int MapSize = new Vector2Int(1024, 1024);
		Vector2Int StrucMapSize;
		public int PerlinSeed = 0;
		public Vector2 PerlinOffset = Vector2.zero;
		//public float PerlinFrequency = 1f;
		//[Range(-100f, 100f)] public float PerlinContrast = 0f;
		public bool WideDebug;
		bool m_Generated;
		Vector2Int m_LastUpdatePos;
		CStruc[] m_Strucs;
		CPilar[] m_ViewPilars;
		CStruc[] m_VisibleStrucs;
		CStruc[] m_TempVisibleStrucs;
		int[] m_VisibleStrucIDs;
		AI.ODD.COddController m_ODD;

		//public Texture2D PerlinTexture;

		//public bool Automatic = true;

		public float CurrentPerlinValue = 0f;
		//const int NextPositionOffsetMin = 4;
		//const int NextPositionOffsetMax = 100;
		
		//float m_NextPlacement = 0f;
		//IE.V3.StructureIE m_Structure;
		//int m_Created = 0;
		//float[] m_PerlinSamples;
		//float m_RowPerlin;
		//bool m_UpdateRowPerlin;
		//bool m_EndOfMap;
		//Vector2Int m_LastStrucPos = Vector2Int.zero;
		//Vector2Int m_NextStrucPos = Vector2Int.zero;
		//List<CStrucEdit> m_Structures;

		World.World m_World;
		//void NormalizeArray(float[] arr)
		//{
		//	float min = float.PositiveInfinity;
		//	float max = float.NegativeInfinity;

		//	for (int i = 0; i < arr.Length; ++i)
		//	{
		//		float value = arr[i];
		//		if (value < min) min = value;
		//		if (value > max) max = value;
		//	}
		//	float range = max - min;
		//	for (int i = 0; i < arr.Length; ++i)
		//	{
		//		float value = arr[i];
		//		arr[i] = (value - min) / range;
		//	}
		//}
		//public void GenerateNoise()
		//{
		//	PerlinTexture = new Texture2D(MapSize.x, MapSize.y, TextureFormat.RGB24, false, true);
		//	var noise = new ProceduralNoiseProject.PerlinNoise(PerlinSeed, PerlinFrequency)
		//	{
		//		Offset = PerlinOffset
		//	};
		//	m_PerlinSamples = new float[MapSize.x * MapSize.y];

		//	float xMult = 1f / (MapSize.x - 1f);
		//	float yMult = 1f / (MapSize.y - 1f);
		//	for (int i = 0; i < m_PerlinSamples.Length; ++i)
		//	{
		//		var pos = GameUtils.PosFromID(i, MapSize.x, MapSize.y);
		//		float fx = pos.x * xMult;
		//		float fy = pos.y * yMult;

		//		m_PerlinSamples[i] = noise.Sample2D(fx, fy);
		//	}
		//	NormalizeArray(m_PerlinSamples);

		//	var contrastValue = (100f + PerlinContrast) / 100f;
		//	contrastValue *= contrastValue;
		//	var colors = PerlinTexture.GetPixels32();
		//	var color = new Color32(0, 0, 0, 255);
		//	float inv255 = 1f / 255f;
		//	for (int i = 0; i < colors.Length; ++i)
		//	{
		//		var colorValue = (int)((((m_PerlinSamples[i] - 0.5f) * contrastValue) + 0.5f) * 255f);
		//		color.r = color.g = color.b = (byte)Mathf.Clamp(colorValue, 0, 255);
		//		m_PerlinSamples[i] = color.r * inv255;
		//		colors[i] = color;
		//	}
		//	PerlinTexture.SetPixels32(colors);
		//	PerlinTexture.Apply();
		//}
		private void Awake()
		{
			m_VisibleStrucIDs = new int[ViewStrucSide * ViewStrucSide];
			m_VisibleStrucs = new CStruc[ViewStrucSide * ViewStrucSide];
			m_TempVisibleStrucs = new CStruc[ViewStrucSide * ViewStrucSide];
		}
		public void Init()
		{
			var mgr = Manager.Mgr;
			mgr.HideInfo = true;
			mgr.DebugAI = false;
			mgr.DebugMovement = false;
			mgr.DebugSpells = false;
			mgr.DebugStats = false;
			//if (m_Structures == null)
			//	m_Structures = new List<CStrucEdit>((MapSize.x * MapSize.y) / (8 * 8));
			//else
			//	m_Structures.Clear();
			m_Generated = false;
			var camMgr = CameraManager.Mgr;
			camMgr.CameraType = ECameraType.FREE;

			m_World = new GameObject("World").AddComponent<World.World>();
			m_ViewPilars = new CPilar[ViewArea];
			//m_Created = 0;
			//m_RowPerlin = 0f;
			//m_UpdateRowPerlin = true;
			//m_EndOfMap = false;
			//m_LastStrucPos = m_NextStrucPos = Vector2Int.zero;
			//GenerateNoise();
			//CurrentPerlinValue = m_PerlinSamples[0];
			//if(Structures.StrucDict.ContainsKey(StructureName))
			//{
			//	m_Structure = Structures.Strucs[Structures.StrucDict[StructureName]];
			//}
			//PlaceStructures();
		}
		public void OnMenuButton()
		{
			MainMenu.gameObject.SetActive(true);
			//if (m_Strucs != null)
			//{
			//	for (int i = 0; i < m_Strucs.Length; ++i)
			//	{
			//		var struc = m_Strucs[i];
			//		if (struc != null)
			//		{
			//			struc.UnloadFromWorld();
			//			for (int j = 0; j < struc.GetLES().Count; ++j)
			//				GameUtils.DeleteGameobject(struc.GetLES()[j].gameObject);
			//			GameUtils.DeleteGameobject(struc.gameObject);
			//		}
			//	}
			//}
			if (m_ODD != null)
				GameUtils.DeleteGameobject(m_ODD.gameObject);
			m_ODD = null;
			//for (int i = 0; i < m_Structures.Count; ++i)
			//	GameUtils.DeleteGameobject(m_Structures[i].gameObject);
			GameUtils.DeleteGameobject(m_World.gameObject);
			m_World = null;
			gameObject.SetActive(false);
		}
		//void PlaceStructures()
		//{
		//	for(int i = 0; i < m_PerlinSamples.Length; ++i)
		//	{
		//		var prob = m_PerlinSamples[i];
		//		var chance = UnityEngine.Random.value;
		//		if (chance > prob)
		//			continue;
		//		PlaceStruc(GameUtils.PosFromID(i, MapSize.x, MapSize.y), false);
		//	}
		//}
		//void PlaceStruc(Vector2Int position, bool randomYOffset = true)
		//{
		//	var struc = CStrucEdit.LoadStruc(m_Structure.StructureID);
		//	struc.name = $"Structure_{m_Created++}";
		//	m_LastStrucPos = position;
		//	bool IsPilarValid(CPilar pilar)
		//	{
		//		if (pilar.GetBlocks().Count == 0)
		//			return false;

		//		if (pilar.GetBlocks().Count == 1 && pilar.GetBlocks()[0].GetLayer() == 0)
		//			return false;
		//		return true;
		//	}
		//	int PerlinOffset(float perlin) => Mathf.FloorToInt(NextPositionOffsetMax * (1f - perlin) + NextPositionOffsetMin * perlin);
		//	Vector2Int topLeft = new Vector2Int(int.MaxValue, int.MaxValue), botRight = new Vector2Int(int.MinValue, int.MinValue);
		//	for (int i = 0; i < struc.GetPilars().Length; ++i)
		//	{
		//		var pilar = struc.GetPilars()[i];
		//		if (pilar == null)
		//			continue;
		//		if (!IsPilarValid(pilar))
		//		{
		//			pilar.DestroyPilar(false);
		//			continue;
		//		}
		//		pilar.ChangeStruc(struc, pilar.GetStructureID());
		//		var pos = struc.VPosFromPilarID(pilar.GetStructureID());
		//		if (pos.x < topLeft.x)
		//			topLeft.x = pos.x;
		//		if (pos.y < topLeft.y)
		//			topLeft.y = pos.y;
		//		if (pos.x > botRight.x)
		//			botRight.x = pos.x;
		//		if (pos.y > botRight.y)
		//			botRight.y = pos.y;
		//	}
		//	var width = (botRight.x - topLeft.x) + 1;
		//	var height = (botRight.y - topLeft.y) + 1;
		//	//Debug.Log($"Size:{width}x{height}");
		//	Vector2 displacement = new Vector2(
		//		-(Def.MaxStrucSide - width) * 0.5f * (1f + Def.BlockSeparation),
		//		-(Def.MaxStrucSide - height) * 0.5f * (1f + Def.BlockSeparation));
		//	struc.transform.position = new Vector3(displacement.x, 0f, displacement.y);
		//	int rndYOffset = 0;
		//	if (randomYOffset)
		//	{
		//		var rndYOffsetRange = PerlinOffset(CurrentPerlinValue) / 2;
		//		rndYOffset = UnityEngine.Random.Range(-rndYOffsetRange, rndYOffsetRange);
		//	}
		//	struc.transform.Translate(new Vector3((1f + Def.BlockSeparation) * position.x, 0f, (1f + Def.BlockSeparation) * (position.y + rndYOffset)));
		//	CurrentPerlinValue = m_PerlinSamples[GameUtils.IDFromPos(position, MapSize.x, MapSize.y)];

		//	var xOffset = PerlinOffset(CurrentPerlinValue);
		//	//Debug.Log("Offset: " + xOffset.ToString());
		//	m_NextStrucPos = m_LastStrucPos + new Vector2Int(xOffset, 0);
		//	if (m_NextStrucPos.x >= MapSize.x)
		//	{
		//		var yOffset = PerlinOffset(m_RowPerlin);
		//		m_UpdateRowPerlin = true;
		//		m_NextStrucPos = new Vector2Int(m_NextStrucPos.x - MapSize.x, m_NextStrucPos.y + yOffset);
		//	}
		//	if (m_NextStrucPos.y >= MapSize.y)
		//	{
		//		Debug.Log("End of Map");
		//		m_EndOfMap = true;
		//	}
		//	m_Structures.Add(struc);
		//}
		//void PlaceNextStructure()
		//{
		//	if (m_Structure == null || m_EndOfMap)
		//		return;
		//	var updatePerlin = m_UpdateRowPerlin;
		//	PlaceStruc(m_NextStrucPos);
		//	if (updatePerlin)
		//		m_RowPerlin = CurrentPerlinValue;
		//}
		void GenerateMap()
		{
			m_World.Generate(MapSize, PerlinSeed, new List<string>(World.World.DefaultStructureNames), PerlinOffset, WideDebug);
			//if(m_Strucs != null)
			//{
			//	for(int i = 0; i < m_Strucs.Length; ++i)
			//	{
			//		var struc = m_Strucs[i];
			//		if (struc != null)
			//		{
			//			struc.UnloadFromWorld();
			//			for (int j = 0; j < struc.GetLES().Count; ++j)
			//				GameUtils.DeleteGameobject(struc.GetLES()[j].gameObject);
			//			GameUtils.DeleteGameobject(struc.gameObject);
			//		}
			//	}
			//}
			for (int i = 0; i < m_VisibleStrucs.Length; ++i)
			{
				m_VisibleStrucs[i] = null;
				m_TempVisibleStrucs[i] = null;
				m_VisibleStrucIDs[i] = -1;
			}
			StrucMapSize = m_World.GetWorldStrucSize();
			//var strucCount = StrucMapSize.x * StrucMapSize.y;
			//m_Strucs = new CStruc[strucCount];
			//for(int i = 0; i < m_Strucs.Length; ++i)
			//{
			//	var struc = CStruc.CreateFromWorld(i, StrucMapSize.x, StrucMapSize.y);
			//	struc.transform.SetParent(m_World.transform, true);
			//	struc.gameObject.SetActive(false);
			//	m_Strucs[i] = struc;
			//}
			m_Generated = true;
			var camMgr = CameraManager.Mgr;
			camMgr.CameraType = ECameraType.INGAME;
			var camTarget = camMgr.Target;
			var vCenterPos = new Vector2Int(m_World.GetWorldSize().x / 2, m_World.GetWorldSize().y / 2);
			var wCenterPos = GameUtils.TransformPosition(vCenterPos);
			var pilarID = GameUtils.IDFromPosUnsafe(vCenterPos, m_World.GetWorldSize().x, m_World.GetWorldSize().y);
			float height = 0f;
			if (m_World.GetPilars()[pilarID] != null)
				height = m_World.GetPilars()[pilarID].GetStruc().GetHeight();

			camTarget.transform.position = new Vector3(wCenterPos.x, height, wCenterPos.y);
			m_LastUpdatePos = new Vector2Int(int.MinValue, int.MinValue);
			UpdateStrucs(); //UpdatePilars();
		}
		bool UpdateMovement()
		{
			if(m_ODD != null)
			{
				var vwCamTarget = GameUtils.TransformPosition(new Vector2(m_ODD.transform.position.x, m_ODD.transform.position.z));
				var dist = Vector2Int.Distance(m_LastUpdatePos, vwCamTarget);
				if (dist < (CStruc.Height / 2))
					return true;
				return false;
			}

			const float MoveAmount = 10f;
			var movement = Vector2.zero;
			var camMgr = CameraManager.Mgr;
			var camera = camMgr.Camera;
			if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.CamMoveForward))
			{
				movement.Set(movement.x + camera.transform.forward.z, movement.y + camera.transform.forward.x);
			}
			if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.CamMoveBackward))
			{
				movement.Set(movement.x + -camera.transform.forward.z, movement.y + -camera.transform.forward.x);
			}
			if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.CamMoveLeft))
			{
				movement.Set(movement.x + -camera.transform.right.z, movement.y + -camera.transform.right.x);
			}
			if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.CamMoveRight))
			{
				movement.Set(movement.x + camera.transform.right.z, movement.y + camera.transform.right.x);
			}
			if (movement != Vector2.zero)
			{
				movement.Normalize();
				movement *= MoveAmount;
				movement *= Time.deltaTime;
				var target = camMgr.Target;
				target.transform.Translate(movement.y, 0f, movement.x, Space.World);
				return true;
			}
			return false;
		}
		void UpdateStrucs()
		{
			var camMgr = CameraManager.Mgr;
			var camTarget = camMgr.Target;
			var vwCamTarget = GameUtils.TransformPosition(new Vector2(camTarget.transform.position.x, camTarget.transform.position.z));
			var vCamTarget = m_World.VPosFromVWPos(vwCamTarget); //new Vector2Int(GameUtils.Mod(vwCamTarget.x, m_World.GetWorldSize().x), GameUtils.Mod(vwCamTarget.y, m_World.GetWorldSize().y));
			
			var vCamID = GameUtils.IDFromPosUnsafe(vCamTarget, m_World.GetWorldSize().x, m_World.GetWorldSize().y);
			var vCamPilar = m_World.GetPilars()[vCamID];
			if (vCamPilar != null)
			{
				var height = vCamPilar.GetStruc().GetHeight();
				camTarget.transform.position = new Vector3(
					camTarget.transform.position.x,
					height,
					camTarget.transform.position.z);
				//World.BlockInfo lastBlock = null;
				//for (int i = 0; i < vCamPilar.GetBlocks().Count; ++i)
				//{
				//	var block = vCamPilar.GetBlocks()[i];
				//	if (block == null)
				//		continue;
				//	lastBlock = block;
				//}
				//if (lastBlock != null)
				//{
				//	var height = vCamPilar.GetStruc().GetHeight() + lastBlock.Height + lastBlock.MicroHeight;
				//	camTarget.transform.position = new Vector3(
				//		camTarget.transform.position.x,
				//		height,
				//		camTarget.transform.position.z);
				//}
			}
			var dist = Vector2Int.Distance(m_LastUpdatePos, vwCamTarget);
			if (dist < (CStruc.Height / 2))
				return;

			m_LastUpdatePos = vwCamTarget;

			var vsPos = World.World.VSPosFromVPos(vCamTarget); // Struc on top of the camera
			var offset = Mathf.FloorToInt(ViewStrucSide * 0.5f);
			var initPos = vsPos - new Vector2Int(offset, offset);
			var endPos = vsPos + new Vector2Int(offset, offset);

			for(int y = initPos.y; y <= endPos.y; ++y)
			{
				for(int x = initPos.x; x <= endPos.x; ++x)
				{
					var sPos = new Vector2Int(GameUtils.Mod(x, StrucMapSize.x),  GameUtils.Mod(y, StrucMapSize.y));
					var sID = GameUtils.IDFromPosUnsafe(sPos, StrucMapSize.x, StrucMapSize.y);
					var arrPos = new Vector2Int(x - initPos.x, y - initPos.y);
					var arrID = GameUtils.IDFromPosUnsafe(arrPos, ViewStrucSide, ViewStrucSide);
					m_VisibleStrucIDs[arrID] = sID;
				}
			}
			for(int i = 0; i < m_VisibleStrucs.Length; ++i)
			{
				var struc = m_VisibleStrucs[i];
				if (struc == null)
					continue;

				int copyToIdx = -1;
				for(int j = 0; j < m_VisibleStrucIDs.Length; ++j)
				{
					if(m_VisibleStrucIDs[j] == struc.GetStrucID() && m_TempVisibleStrucs[j] == null)
					{
						copyToIdx = j;
						break;
					}
				}
				if(copyToIdx >= 0)
				{
					m_TempVisibleStrucs[copyToIdx] = struc;
					continue;
				}
				struc.UnloadFromWorld();
				struc.gameObject.SetActive(false);
				//if(strucIDX < 0)
				//{
				//	struc.UnloadFromWorld();
				//	m_VisibleStrucs[i] = null;
				//	m_TempVisibleStrucs[i] = null;
				//}
				//else
				//{
				//	m_VisibleStrucIDs[i] = -1;
				//	m_TempVisibleStrucs[strucIDX] = struc;
				//}

			}
			for(int i = 0; i < m_VisibleStrucIDs.Length; ++i)
			{
				var strucID = m_VisibleStrucIDs[i];
				if (strucID < 0 || m_TempVisibleStrucs[i] != null)
					continue;
				//if(m_TempVisibleStrucs[i] != null)
				//{
				//	if (m_TempVisibleStrucs[i].GetStrucID() == strucID)
				//		continue;
				//	Debug.LogWarning("Something went terribly wrong.");
				//	break;
				//}
				var struc = m_World.GetStrucs()[strucID];
				struc.gameObject.SetActive(true);
				var viewPos = GameUtils.PosFromIDUnsafe(i, ViewStrucSide, ViewStrucSide);
				viewPos -= new Vector2Int(offset, offset);
				var vsStrucPos = vsPos + viewPos;
				var vStrucPos = World.World.VPosFromVSPos(vsStrucPos);
				var wStrucPos = GameUtils.TransformPosition(vStrucPos);
				var loopOffset = new Vector2Int(Mathf.FloorToInt(vwCamTarget.x / (float)MapSize.x) * MapSize.x, Mathf.FloorToInt(vwCamTarget.y / (float)MapSize.y) * MapSize.y);
				var wLoopOffset = GameUtils.TransformPosition(loopOffset);
				struc.transform.localPosition = new Vector3(wStrucPos.x + wLoopOffset.x, 0f, wStrucPos.y + wLoopOffset.y);
				struc.LoadFromWorld(m_World);
				m_TempVisibleStrucs[i] = struc;
			}
			m_TempVisibleStrucs.CopyTo(m_VisibleStrucs, 0);
			for (int i = 0; i < m_TempVisibleStrucs.Length; ++i) { m_TempVisibleStrucs[i] = null; m_VisibleStrucIDs[i] = -1; }
		}
		void UpdatePilars()
		{
			var camMgr = CameraManager.Mgr;
			var camTarget = camMgr.Target;
			var vCamTarget = GameUtils.TransformPosition(new Vector2(camTarget.transform.position.x, camTarget.transform.position.z));
			//vCamTarget = new Vector2Int(vCamTarget.x % m_World.GetWorldSize().x, vCamTarget.y % m_World.GetWorldSize().y);

			var vCamID = GameUtils.IDFromPosUnsafe(vCamTarget, m_World.GetWorldSize().x, m_World.GetWorldSize().y);
			var vCamPilar = m_World.GetPilars()[vCamID];
			if (vCamPilar != null)
			{
				World.BlockInfo lastBlock = null;
				for (int i = 0; i < vCamPilar.GetBlocks().Count; ++i)
				{
					var block = vCamPilar.GetBlocks()[i];
					if (block == null)
						continue;
					lastBlock = block;
				}
				if (lastBlock != null)
				{
					var height = vCamPilar.GetStruc().GetHeight() + lastBlock.Height + lastBlock.MicroHeight;
					camTarget.transform.position = new Vector3(
						camTarget.transform.position.x,
						height,
						camTarget.transform.position.z);
				}
			}


			var dist = Vector2Int.Distance(m_LastUpdatePos, vCamTarget);
			if (dist < (CStruc.Height / 2))
				return;

			m_LastUpdatePos = vCamTarget;
			

			const int XYOffset = (int)(ViewStrucSide * 0.5f * CStruc.Height);

			for(int i = 0; i < m_ViewPilars.Length; ++i)
			{
				var pilar = m_ViewPilars[i];
				if (pilar == null)
					continue;

				World.WorldPool.ReturnPilar(pilar);
				m_ViewPilars[i] = null;
			}

			var initPos = new Vector2Int(vCamTarget.x - XYOffset, vCamTarget.y - XYOffset);

			for (int i = 0; i < ViewArea; ++i)
			{
				var pos = initPos + GameUtils.PosFromIDUnsafe(i, ViewSide, ViewSide);

				var pilarID = GameUtils.IDFromPosUnsafe(pos, m_World.GetWorldSize().x, m_World.GetWorldSize().y);

				var vPilar = m_World.GetPilars()[pilarID];
				if (vPilar == null)
					continue;

				var wPilar = World.WorldPool.GetPilar();
				//wPilar.name = $"Pilar_({pos.x},{pos.y})";
				var sHeight = vPilar.GetStruc().GetHeight();
				wPilar.transform.SetParent(m_World.transform);
				wPilar.gameObject.SetActive(true);
				wPilar.transform.localPosition = new Vector3(pos.x * (1f + Def.BlockSeparation), sHeight, pos.y * (1f + Def.BlockSeparation));
				wPilar.transform.localRotation = Quaternion.identity;
				wPilar.transform.localScale = Vector3.one;
				m_ViewPilars[i] = wPilar;
				var vPilarBlocks = vPilar.GetBlocks();
				var wPilarBlocks = wPilar.GetBlocks();
				wPilarBlocks.AddRange(Enumerable.Repeat<IBlock>(null, vPilarBlocks.Count));

				for (int j = 0; j < vPilarBlocks.Count; ++j)
				{
					var blockInfo = vPilarBlocks[j];
					if(blockInfo.BlockType == Def.BlockType.WIDE)
					{
						if (blockInfo.Pilar != vPilar)
						{
							continue;
						}
					}
					//if (blockInfo.BlockType == Def.BlockType.STAIRS && blockInfo.StairType == Def.StairType.NORMAL)
					//	Debug.Log("hi");
					//Debug.Log("Showing block: L" + blockInfo.Length.ToString() + " T" + blockInfo.BlockType + " S" + blockInfo.StairType.ToString());
					var block = World.WorldPool.GetBlock(blockInfo.Length, blockInfo.BlockType, blockInfo.StairType);

					wPilarBlocks[j] = block;
					block.gameObject.SetActive(true);
					block.transform.SetParent(wPilar.transform);
					block.transform.localPosition = Vector3.zero;
					block.transform.localRotation = Quaternion.identity;
					block.transform.localScale = Vector3.one;

					// Apply 
					block.InitFromWorld(blockInfo);
				}
			}
		}
		private void Update()
		{
			if(WideDebug)
			{
				if(Input.GetKey(KeyCode.Space))
				{
					if (!m_Generated)
					{
						GenerateMap();
					}
					else
					{
						m_World.DebugContinue();
						for(int i = 0; i < m_VisibleStrucs.Length; ++i)
						{
							var struc = m_VisibleStrucs[i];
							if (struc == null)
								continue;
							struc.UnloadFromWorld();
							m_VisibleStrucs[i] = null;
						}
						m_LastUpdatePos = new Vector2Int(int.MinValue, int.MinValue);
						UpdateStrucs();
					}
				}
				return;
			}
			
			if (Input.GetKey(KeyCode.Space))
				GenerateMap();

			if (!m_Generated)
				return;

			if (UpdateMovement())
				UpdateStrucs(); //UpdatePilars();

			if(m_ODD == null)
			{
				if (Input.GetKeyDown(KeyCode.O))
				{
					m_ODD = Instantiate(AssetLoader.Odd);
					var cammgr = CameraManager.Mgr;
					m_ODD.transform.position = cammgr.Target.transform.position + new Vector3(0f, 2f, 0f);
					m_ODD.Init();
					cammgr.Target = m_ODD.gameObject;
				}
			}
			else
			{
				if(Input.GetKeyDown(KeyCode.Delete))
				{
					var cammgr = CameraManager.Mgr;
					cammgr.DefaultTarget.transform.position = m_ODD.transform.position;
					cammgr.Target = cammgr.DefaultTarget;
					GameUtils.DeleteGameobject(m_ODD.gameObject);
					m_ODD = null;
				}
			}

				//m_World.Generate(MapSize, PerlinSeed, PerlinFrequency, PerlinContrast, PerlinOffset);
			//if (m_EndOfMap || m_NextPlacement >= Time.time)
			//	return;

			//if(Automatic)
			//{
			//	PlaceNextStructure();
			//}
			//else
			//{
			//	if(Input.GetKey(KeyCode.Space))
			//	{
			//		PlaceNextStructure();
			//		m_NextPlacement = Time.time + PlacementDelay;
			//	}
			//}
		}
	}
}