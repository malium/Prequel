/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace Assets.UI
//{
//	public class StrucEditUI : MonoBehaviour
//	{
//		#region GUI
//		public MainMenuUI MainMenu;
//		public LayerEditUI LayerEditor;

//		public UnityEngine.UI.Image LeftPanel;
//		public UnityEngine.UI.Button LayerEditBttn;
//		public UnityEngine.UI.Button SelectAllBttn;
//		public UnityEngine.UI.Button ResetLocksBttn;
//		public UnityEngine.UI.Button ReapplyLayersBttn;
//		public CSlider StructureSizeXSlider;
//		public CSlider StructureSizeYSlider;

//		public UnityEngine.UI.Button MenuBttn;
//		public UnityEngine.UI.Button CameraEditBttn;
//		public UnityEngine.UI.Button CameraGameBttn;
//		public UnityEngine.UI.Button CameraFreeBttn;

//		public StrucEditMenuUI EditMenuUI;
//		public UnityEngine.UI.Image CameraPanel;

//		public UnityEngine.UI.Text FPSText;

//		public UnityEngine.UI.Button BlockPanelShowBttn;
//		public GameObject BlockPanel;
//		public UnityEngine.UI.Button BlockPanelHideBttn;
//		public UnityEngine.UI.Text BPRotation;
//		public UnityEngine.UI.Text BPRotationIE;
//		public UnityEngine.UI.Toggle BPRotationTggl;
//		public UnityEngine.UI.Image BPMaterial;
//		public UnityEngine.UI.Image BPMaterialIE;
//		public UnityEngine.UI.Toggle BPMaterialTggl;
//		public UnityEngine.UI.Text BPLength;
//		public UnityEngine.UI.Text BPLengthIE;
//		public UnityEngine.UI.Toggle BPLengthTggl;
//		public UnityEngine.UI.Text BPHeight;
//		public UnityEngine.UI.Text BPHeightIE;
//		public UnityEngine.UI.Toggle BPHeightTggl;
//		public UnityEngine.UI.Text BPMicroHeight;
//		public UnityEngine.UI.Text BPLayer;
//		public UnityEngine.UI.Text BPLayerIE;
//		public UnityEngine.UI.Text BPBlockType;
//		public UnityEngine.UI.Text BPBlockTypeIE;
//		public UnityEngine.UI.Text BPVoid;
//		public UnityEngine.UI.Text BPVoidIE;
//		public UnityEngine.UI.Text BPStair;
//		public UnityEngine.UI.Text BPStairIE;
//		public UnityEngine.UI.Text BPLock;
//		public UnityEngine.UI.Image BPProp;
//		public UnityEngine.UI.Image BPPropIE;
//		public UnityEngine.UI.Toggle BPPropTggl;
//		public UnityEngine.UI.Image BPMonster;
//		public UnityEngine.UI.Image BPMonsterIE;
//		public UnityEngine.UI.Toggle BPMonsterTggl;

//		public StrucEditHelpUI HelpUI;

//		float m_NextStrucModification;
//		int m_NextStrucWidth;
//		int m_NextStrucHeight;

//		private void Awake()
//		{
//			m_EditKeyFuncs = new Func<bool>[0];
//			m_KeyFuncs = new Func<bool>[0];

//			LayerEditBttn.onClick.AddListener(OnLayerEditButton);
//			SelectAllBttn.onClick.AddListener(OnSelectAllButton);
//			ResetLocksBttn.onClick.AddListener(OnResetLocksButton);
//			ReapplyLayersBttn.onClick.AddListener(OnReapplyLayersButton);

//			StructureSizeXSlider.SetMinValue(Def.MinStrucSide);
//			StructureSizeXSlider.SetMaxValue(Def.MaxStrucSide);
//			StructureSizeXSlider.SetValue(8f);
//			StructureSizeXSlider.SetCallback(OnStructureSizeXSlider);

//			StructureSizeYSlider.SetMinValue(Def.MinStrucSide);
//			StructureSizeYSlider.SetMaxValue(Def.MaxStrucSide);
//			StructureSizeYSlider.SetValue(8f);
//			StructureSizeYSlider.SetCallback(OnStructureSizeYSlider);

//			MenuBttn.onClick.AddListener(OnMenuButton);
//			CameraEditBttn.onClick.AddListener(OnCamEditButton);
//			CameraGameBttn.onClick.AddListener(OnCamGameButton);
//			CameraFreeBttn.onClick.AddListener(OnCamFreeButton);

//			HelpUI.Init(OnHelpUIEnd);

//			//BlockPanelHideBttn.onClick.AddListener(OnBlockPanelHide);
//			//BlockPanelShowBttn.onClick.AddListener(OnBlockPanelShow);
//		}

//		public void OnMenuButton()
//		{
//			EditMenuUI.gameObject.SetActive(true);
//			if (!CameraFreeBttn.interactable)
//				CameraManager.Mgr.FreeCam.enabled = false;
//			enabled = false;
//		}
//		public void OnCamEditButton()
//		{
//			CameraEditBttn.interactable = false;
//			CameraFreeBttn.interactable = true;
//			CameraGameBttn.interactable = true;
//			CameraManager.Mgr.CameraType = ECameraType.EDITOR;
//		}
//		public void OnCamGameButton()
//		{
//			CameraEditBttn.interactable = true;
//			CameraFreeBttn.interactable = true;
//			CameraGameBttn.interactable = false;
//			CameraManager.Mgr.CameraType = ECameraType.INGAME;
//		}
//		public void OnCamFreeButton()
//		{
//			CameraEditBttn.interactable = true;
//			CameraFreeBttn.interactable = false;
//			CameraGameBttn.interactable = true;
//			CameraManager.Mgr.CameraType = ECameraType.FREE;
//		}
//		public void OnLayerEditButton()
//		{
//			LayerEditor.gameObject.SetActive(true);
//			enabled = false;
//			LayerEditor.Init(m_StrucEdit, OnLayerEditEnd);
//			if (!CameraFreeBttn.interactable)
//				CameraManager.Mgr.FreeCam.enabled = false;
//		}
//		public void OnLayerEditEnd()
//		{
//			LayerEditor.gameObject.SetActive(false);
//			var tLayers = LayerEditor.GetLayers();
//			var sLayers = m_StrucEdit.GetLayers();
//			int validLayers = 0;
//			// Dump valid temporal layers to the real ones, and apply them.
//			for (int i = 0; i < Def.MaxLayerSlots; ++i)
//			{
//				var layer = sLayers[i];
//				if(tLayers[i].IsValid())
//				{
//					if (layer != tLayers[i])
//						Structures.SetStrucModified(m_StrucEdit.IDXIE, true);
//					++validLayers;
//					layer.CopyFrom(tLayers[i]);
//					layer.SortProbabilities();
//				}
//				else if(layer.IsValid()) // invalidate
//				{
//					if (layer != tLayers[i])
//						Structures.SetStrucModified(m_StrucEdit.IDXIE, true);
//					layer.CopyFrom(LayerInfo.GetDefaultLayer());
//					layer.Slot = i + 1;
//				}
//				m_StrucEdit.SetLayer(layer);
//			}
//			for(int i = 0; i < m_Selected.Count; ++i)
//			{
//				if (m_Selected[i] == null)
//					continue;
//				m_Selected[i].SetSelected(false);
//			}
//			m_Selected.Clear();

//			//ReapplyLayers();
//			var enable = validLayers > 0;
//			ResetLocksBttn.interactable = enable;
//			ReapplyLayersBttn.interactable = enable;
//			m_NextModificationTime = Time.time + 0.5f;
//			enabled = true;
//			if (!CameraFreeBttn.interactable)
//				CameraManager.Mgr.FreeCam.enabled = true;
//		}
//		public void OnSelectAllButton()
//		{
//			m_Selected.Clear();
//			var pilars = m_StrucEdit.GetPilars();
//			for(int i = 0; i < pilars.Length; ++i)
//			{
//				var pilar = pilars[i];
//				if (pilar == null)
//					continue;
//				var blocks = pilar.GetBlocks();
//				for(int j = 0; j < blocks.Count; ++j)
//				{
//					var block = (CBlockEdit)blocks[j];
//					if (block.IsRemoved())
//						continue;
//					block.SetSelected(true);
//					m_Selected.Add(block);
//				}
//			}
//		}
//		public void OnResetLocksButton()
//		{
//			var strucIE = Structures.Strucs[m_StrucEdit.IDXIE];
//			var pilars = m_StrucEdit.GetPilars();

//			for(int i = 0; i < pilars.Length; ++i)
//			{
//				var pilar = pilars[i];
//				if (pilar == null)
//					continue;

//				var blocks = pilar.GetBlocks();
//				for(int j = 0; j < blocks.Count; ++j)
//				{
//					var block = (CBlockEdit)blocks[j];

//					if (block.IsRemoved() || block.GetLayer() == 0)
//						continue;

//					var blockIE = strucIE.GetBlocks()[block.GetIDXIE()];

//					var layer = m_StrucEdit.GetLayers()[block.GetLayer() - 1];
//					if (layer.IsLinkedLayer)
//					{
//						layer = m_StrucEdit.GetLayers()[block.GetLinkedTo() - 1];
//					}

//					block.SetLockState(Def.LockState.Unlocked);
//					blockIE.SetDefault();
//					blockIE.Layer = (byte)block.GetLayer();
//					blockIE.SetFlag(IE.V4.BlockIE.Flag.Anchor, block.IsAnchor());
//					blockIE.BlockType = block.GetBlockType();
//					if(block.GetStairState() != Def.StairState.NONE)
//					{
//						blockIE.Stair = block.GetStairState();
//						blockIE.Rotation = block.GetRotation();
//						blockIE.SetFlag(IE.V4.BlockIE.Flag.Rotation, true);
//						block.SetLockState(Def.LockState.SemiLocked);
//					}
//					blockIE.VHeight = (short)(block.GetHeight() * 2f);
//					if (Mathf.Clamp(block.GetLength(), layer.BlockLengthMin, layer.BlockLengthMax) != block.GetLength())
//					{
//						var len = block.GetLength();
//						if (len == 3.4f)
//							len = 3.5f;
//						blockIE.VLength = (byte)(((int)(len * 2f)) - 1);
//						blockIE.SetFlag(IE.V4.BlockIE.Flag.Length, true);
//						block.SetLockState(Def.LockState.SemiLocked);
//					}
//				}
//			}
//		}
//		public void OnReapplyLayersButton()
//		{
//			ReapplyLayers();
//		}
//		public void OnModRotateButton()
//		{

//		}
//		public void OnModHFlipButton()
//		{

//		}
//		public void OnModVFlipButton()
//		{

//		}
//		public void OnStructureSizeXSlider(int _1, float value)
//		{
//			//StructureSizeXSliderText.text = StructureSizeXSlider.value.ToString();
//			m_NextStrucModification = Time.time + 1f;
//			m_NextStrucHeight = (int)value;
//			//if(Input.GetKey(KeyCode.LeftShift))
//			//{
//			//    int diff = (int)StructureSizeXSlider.value - NextStrucHeight;
//			//    int temp = NextStrucWidth + diff;
//			//    if (temp >= Def.MinStrucSide && temp <= Def.MaxStrucSide)
//			//    {
//			//        NextStrucWidth = temp;
//			//        StructureSizeYSlider.value = NextStrucWidth;
//			//        StructureSizeYSliderText.text = NextStrucWidth.ToString();
//			//    }
//			//}
//			//NextStrucHeight = (int)StructureSizeXSlider.value;
//		}
//		public void OnStructureSizeYSlider(int _1, float value)
//		{
//			//StructureSizeYSliderText.text = StructureSizeYSlider.value.ToString();
//			m_NextStrucModification = Time.time + 1f;
//			m_NextStrucWidth = (int)value;
//			//if (Input.GetKey(KeyCode.LeftShift))
//			//{
//			//    int diff = (int)StructureSizeYSlider.value - NextStrucWidth;
//			//    int temp = NextStrucHeight + diff;
//			//    if(temp >= Def.MinStrucSide && temp <= Def.MaxStrucSide)
//			//    {
//			//        NextStrucHeight = temp;
//			//        StructureSizeXSlider.value = NextStrucHeight;
//			//        StructureSizeXSliderText.text = NextStrucHeight.ToString();
//			//    }
//			//}
//			//NextStrucWidth = (int)StructureSizeYSlider.value;
//		}
//		void OnHelpUIEnd()
//		{
//			HelpUI.gameObject.SetActive(false);
//			m_NextModificationTime = Time.time + 0.5f;
//			enabled = true;
//		}
//		public void OnBlockPanelHide()
//		{
//			BlockPanel.SetActive(false);
//			BlockPanelShowBttn.gameObject.SetActive(true);
//		}
//		public void OnBlockPanelShow()
//		{
//			BlockPanel.SetActive(true);
//			BlockPanelShowBttn.gameObject.SetActive(false);
//			UpdateBlockInfo();
//		}
//		void UpdateBlockInfo()
//		{
//			if (BlockPanelShowBttn.isActiveAndEnabled || m_BlockOver == null)
//				return;

//			var bIDXIE = m_BlockOver.GetIDXIE();
//			if (bIDXIE < 0)
//				return;
//			var sIDXIE = m_StrucEdit.IDXIE;
//			var strucIE = Structures.Strucs[sIDXIE];
//			var blockIE = strucIE.GetBlocks()[bIDXIE];

//			BPHeightTggl.isOn = blockIE.GetFlag(IE.V3.BlockIE.Flag.Height);
//			BPLengthTggl.isOn = blockIE.GetFlag(IE.V3.BlockIE.Flag.Length);
//			BPMaterialTggl.isOn = blockIE.GetFlag(IE.V3.BlockIE.Flag.MaterialFamily);
//			BPRotationTggl.isOn = blockIE.GetFlag(IE.V3.BlockIE.Flag.Rotation);
//			BPPropTggl.isOn = blockIE.GetFlag(IE.V3.BlockIE.Flag.Prop);
//			BPMonsterTggl.isOn = blockIE.GetFlag(IE.V3.BlockIE.Flag.Monster);

//			void SetMaterialSprite(UnityEngine.UI.Image image, string materialFamily)
//			{
//				if (image.sprite != null)
//				{
//					Sprite.DestroyImmediate(image.sprite);
//					image.sprite = null;
//				}

//				if (BlockMaterial.FamilyDict.ContainsKey(materialFamily))
//				{
//					var family = BlockMaterial.MaterialFamilies[BlockMaterial.FamilyDict[materialFamily]];
//					var texture = BlockMaterial.GetTextureFromMaterial(family.NormalMaterials[0].TopPart.Mat);
//					image.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero, 100f, 0, SpriteMeshType.FullRect);
//				}
//			}
//			void SetPropSprite(UnityEngine.UI.Image image, string propFamily)
//			{
//				image.sprite = null;

//				if (Props.FamilyDict.ContainsKey(propFamily))
//				{
//					var family = Props.PropFamilies[Props.FamilyDict[propFamily]];
//					image.sprite = family.Props[0].PropSprite;
//				}
//			}
//			void SetMonsterSprite(UnityEngine.UI.Image image, string monsterFamily)
//			{
//				image.sprite = null;

//				if (Monsters.FamilyDict.ContainsKey(monsterFamily))
//				{
//					var family = Monsters.MonsterFamilies[Monsters.FamilyDict[monsterFamily]];
//					image.sprite = family.Frames[0];
//				}
//			}

//			BPRotation.text = m_BlockOver.GetRotation().ToString();
//			BPRotationIE.text = blockIE.Rotation == Def.RotationState.COUNT ? "RANDOM" : blockIE.Rotation.ToString();
//			BPLength.text = m_BlockOver.GetLength().ToString();
//			BPLengthIE.text = blockIE.Length.ToString();
//			BPHeight.text = m_BlockOver.GetHeight().ToString();
//			BPHeightIE.text = blockIE.Height.ToString();
//			BPMicroHeight.text = m_BlockOver.GetMicroHeight().ToString();
//			BPLayer.text = m_BlockOver.GetLayer().ToString();
//			BPLayerIE.text = blockIE.Layer.ToString();
//			BPBlockType.text = m_BlockOver.GetBlockType().ToString();
//			BPBlockTypeIE.text = blockIE.BlockType.ToString();
//			BPVoid.text = m_BlockOver.GetVoidState().ToString();
//			BPVoidIE.text = blockIE.BlockVoid.ToString();
//			BPStair.text = m_BlockOver.GetStairState().ToString();
//			BPStairIE.text = blockIE.Stair.ToString();
//			BPLock.text = m_BlockOver.GetLockState().ToString();

//			SetMaterialSprite(BPMaterialIE, blockIE.MaterialFamily);
//			SetMaterialSprite(BPMaterial, m_BlockOver.GetMaterialFamily().FamilyInfo.FamilyName);
//			SetPropSprite(BPProp, m_BlockOver.GetProp() == null ? "" : m_BlockOver.GetProp().GetInfo().FamilyName);
//			SetPropSprite(BPPropIE, blockIE.PropFamily);
//			SetMonsterSprite(BPMonster, m_BlockOver.GetMonster() == null ? "" : m_BlockOver.GetMonster().GetFamily().Name);
//			SetMonsterSprite(BPMonsterIE, blockIE.MonsterFamily);
//		}

//		#endregion

//		#region Struc
//		List<CBlockEdit> m_Selected;
//		CBlockEdit m_BlockOver;
//		CStrucEdit m_StrucEdit;
//		public CStrucEdit GetStruc() => m_StrucEdit;
//		int m_StrucWidth;
//		int m_StrucHeight;
//		const float MouseHoverDelay = 0.05f;
//		float m_NextMouseHover;
//		const float ModificationDelay = 0.2f;
//		float m_NextModificationTime;
//		Vector3 m_LastHoverMousePosition;
//		const int MaxFramesCaputred = 20;
//		static readonly float[] CapturedFrametimes = new float[MaxFramesCaputred];
//		float m_NextFPSTextUpdate;
//		const float FPSTextUpdateDelay = 1f / 3f;
//		List<CBlockEdit> m_HiddenBlocks;
//		Func<bool>[] m_EditKeyFuncs;
//		Func<bool>[] m_KeyFuncs;

//		public void Init()
//		{
//			EditMenuUI.gameObject.SetActive(true);
//			EditMenuUI.Init();
//			EditMenuUI.gameObject.SetActive(false);

//			m_Selected = new List<CBlockEdit>(Def.MaxStrucSide * Def.MaxStrucSide);
			
//			m_HiddenBlocks = new List<CBlockEdit>();
//			LoadStruc(-1);

//			m_EditKeyFuncs = new Func<bool>[]
//			{
//				OnMaterialChange,
//				OnStairToggle,
//				OnRampToggle,
//				//OnWideToggle,
//				OnLockStateToggle,
//				OnAnchorToggle,
//				OnRotationToggle,
//				OnSelectLayer,
//				OnLengthChange,
//				OnHeightChange,
//				OnDestroyBlock,
//				OnStacking,
//				OnBlockLayerChange,
//				OnSelectAll,
//				OnMenuStart,
//				//OnStructureSize,
//				OnReapplyLayers,
//				OnResetLocks,
//				OnLayerEdit,
//				OnVoidToggle,
//				OnBlockUnhide,
//				OnBlockHide,
//			};

//			m_KeyFuncs = new Func<bool>[]
//			{
//				() => { return ToggleVisibility(); },
//				OnCamModeChange,
//				OnHelpUIStart,
//			};
//		}
//		void ReapplyLayers()
//		{
//			//var les = m_StrucEdit.GetLivingEntities();
//			//for(int i = 0; i < les.Count; ++i)
//			//{
//			//	var le = les[i];
//			//	le.ReceiveDamage(Def.DamageType.UNAVOIDABLE, le.GetTotalHealth());
//			//}
//			//les.Clear();
//			var layers = m_StrucEdit.GetLayers();
//			for(int i = 0; i < Def.MaxLayerSlots; ++i)
//			{
//				if (!layers[i].IsValid())
//					continue;

//				m_StrucEdit.SetLayer(layers[i]);
//			}
//		}
//		void OnMouseHover()
//		{
//			if (m_BlockOver != null)
//			{
//				m_BlockOver.SetHighlighted(true);
//			}

//			if (m_NextMouseHover > Time.time || m_LastHoverMousePosition == Input.mousePosition
//				|| UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
//				return;

//			m_LastHoverMousePosition = Input.mousePosition;
//			m_NextMouseHover = Time.time + MouseHoverDelay;

//			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//			bool rayHit = Physics.Raycast(ray, out RaycastHit mouseHit, 10000f, 1 << Def.RCLayerBlock);
//			if (rayHit)
//			{
//				var block = mouseHit.transform.gameObject.GetComponent<CBlockEdit>();
//				if (m_BlockOver != null && m_BlockOver != block)
//					m_BlockOver.SetHighlighted(false);
//				m_BlockOver = block;
//				UpdateBlockInfo();
//			}
//			else
//			{
//				if (m_BlockOver != null)
//					m_BlockOver.SetHighlighted(false);
//				m_BlockOver = null;
//			}
//		}
//		void OnMouseRaycast()
//		{
//			if (Manager.Mgr.HideInfo)
//				return;

//			bool shiftPressed = Input.GetKey(KeyCode.LeftShift);
//			bool mouseLeftHold = Input.GetMouseButton(0);
//			bool mouseRightHold = Input.GetMouseButton(1);

//			bool mouseLeftClick = Input.GetMouseButtonDown(0);
//			bool mouseRightClick = Input.GetMouseButtonDown(1);
//			bool mouseClicked = mouseLeftClick || mouseRightClick;
//			bool mouseHold = mouseLeftHold || mouseRightHold;
			
//			if ((!mouseClicked && !mouseHold) 
//				|| UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
//				return;

//			bool select = (mouseLeftHold || mouseLeftClick) && (!mouseRightHold || !mouseRightClick);
//			bool unselect = (!mouseLeftHold || !mouseLeftClick) && (mouseRightHold || mouseRightClick);

//			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

//			bool rayHit = Physics.Raycast(ray, out RaycastHit mouseHit, 10000f, 1 << Def.RCLayerBlock);
//			if (!rayHit)
//			{
//				if (mouseClicked && !shiftPressed)
//				{
//					for (int i = 0; i < m_Selected.Count; ++i)
//					{
//						var selBlock = m_Selected[i];
//						if (selBlock == null)
//							continue;
//						selBlock.SetSelected(false);
//					}
//					m_Selected.Clear();
//				}
//				return;
//			}
//			var block = mouseHit.transform.gameObject.GetComponent<CBlockEdit>();
//			if (m_BlockOver != null && m_BlockOver != block)
//				m_BlockOver.SetHighlighted(false);
//			m_BlockOver = block;

//			if ((shiftPressed && mouseHold) || (!shiftPressed && mouseClicked)) // Drag edit || clicky edit
//			{
//				if (select && !m_Selected.Contains(m_BlockOver))
//				{
//					m_BlockOver.SetSelected(true);
//					m_Selected.Add(m_BlockOver);
//				}
//				else if (unselect)
//				{
//					m_BlockOver.SetSelected(false);
//					if(m_Selected.Contains(m_BlockOver))
//					{
//						m_Selected.Remove(m_BlockOver);
//					}
//				}
//			}
//		}
//		// Changes the MaterialFamily of the selected blocks for the next one in the list of the structure, if there are available
//		bool OnMaterialChange()
//		{
//			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.MaterialCycle))
//				return true; // Nothing has been done
//			int done = 0;
//			var strucIE = Structures.Strucs[m_StrucEdit.IDXIE];
//			for(int i = 0; i < m_Selected.Count; ++i)
//			{
//				var cur = m_Selected[i];
//				if (cur.GetLayer() == 0)
//					continue;

//				var layer = m_StrucEdit.GetLayers()[cur.GetLayer() - 1];
//				if (!layer.IsValid())
//					continue;

//				if(layer.IsLinkedLayer)
//				{
//					int selectedLayer = layer.LinkedLayers[LayerInfo.RandomFromList(layer.LinkedLayers)].ID;
//					layer = m_StrucEdit.GetLayers()[selectedLayer - 1];
//				}

//				var availableMaterials = new List<IDChance>(layer.MaterialFamilies.Count);
//				for(int j = 0; j < layer.MaterialFamilies.Count; ++j)
//				{
//					var matFamily = BlockMaterial.MaterialFamilies[layer.MaterialFamilies[j].ID];
//					if(matFamily.GetSet(cur.GetBlockType()).Length > 0)
//					{
//						availableMaterials.Add(layer.MaterialFamilies[j]);
//					}
//				}
//				if (availableMaterials.Count == 0)
//					continue; // not enough materials

//				GameUtils.UpdateChances2(ref availableMaterials);
//				int curMaterialID = availableMaterials[LayerInfo.RandomFromList(availableMaterials)].ID;
//				var curMaterial = BlockMaterial.MaterialFamilies[curMaterialID];
//				cur.SetMaterialFamily(curMaterial);

//				if (cur.GetLockState() != Def.LockState.Locked)
//					cur.SetLockState(Def.LockState.SemiLocked);
//				var blockIE = strucIE.GetBlocks()[cur.GetIDXIE()];
//				blockIE.MaterialFamily = curMaterial.FamilyInfo.FamilyName;
//				++done;
//			}
//			if(done > 0)
//			{
//				Structures.SetStrucModified(m_StrucEdit.IDXIE);
//				return false; // Something has been done, stop changing things this frame
//			}
//			return true;
//		}
//		// Interchanges how the block is going to be an stair within these states:
//		// not been an stair block, there's a chance to be a stair block, block will be always stair
//		bool OnStairToggle()
//		{
//			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.Stair))
//				return true; // Nothing has been done
//			var strucIE = Structures.Strucs[m_StrucEdit.IDXIE];
//			bool shift = Input.GetKey(KeyCode.LeftShift);
//			int done = 0;
//			for (int i = 0; i < m_Selected.Count; ++i) 
//			{
//				var cur = m_Selected[i];
//				if (cur.GetLayer() == 0 || cur.GetBlockType() == Def.BlockType.WIDE)
//					continue;

//				var blockIE = strucIE.GetBlocks()[cur.GetIDXIE()];

//				if (shift)
//				{
//					if(cur.GetStairState() == Def.StairState.ALWAYS || cur.GetStairState() == Def.StairState.RAMP_ALWAYS)
//					{
//						//var check = cur;
//						GameUtils.ApplyFnBlockAbove(cur, (IBlock b) => b.SetHeight(b.GetHeight() - 0.5f));
//						//while (check.GetStackedBlocks()[1] != null)
//						//{
//						//	check = (CBlockEdit)check.GetStackedBlocks()[1];
//						//	check.SetHeight(check.GetHeight() - 0.5f);
//						//}
//					}
//					cur.SetStairState(Def.StairState.STAIR_OR_RAMP);

//					if (cur.GetLockState() != Def.LockState.Locked)
//						cur.SetLockState(Def.LockState.SemiLocked);

//					blockIE.Stair = cur.GetStairState();
//					blockIE.BlockType = cur.GetBlockType();
//					blockIE.Rotation = cur.GetRotation();
//					blockIE.Flags[(int)IE.V3.BlockIE.Flag.Rotation] = true;
//					++done;
//					continue;
//				}

//				var stair = cur.GetStairState() + 1;
//				if (stair > Def.StairState.ALWAYS)
//				{
//					//var check = cur;
//					GameUtils.ApplyFnBlockAbove(cur, (IBlock b) => b.SetHeight(b.GetHeight() - 0.5f));
//					//while(check.GetStackedBlocks()[1] != null)
//					//{
//					//	check = (CBlockEdit)check.GetStackedBlocks()[1];
//					//	check.SetHeight(check.GetHeight() - 0.5f);
//					//}
//					stair = Def.StairState.NONE;
//				}

//				if(stair == Def.StairState.ALWAYS)
//				{
//					//var check = cur;
//					GameUtils.ApplyFnBlockAbove(cur, (IBlock b) => b.SetHeight(b.GetHeight() + 0.5f));
//					//while(check.GetStackedBlocks()[1] != null)
//					//{
//					//	check = (CBlockEdit)check.GetStackedBlocks()[1];
//					//	check.SetHeight(check.GetHeight() + 0.5f);
//					//}
//				}

//				cur.SetStairState(stair);

//				if (cur.GetLockState() != Def.LockState.Locked)
//					cur.SetLockState(Def.LockState.SemiLocked);
				
//				blockIE.Stair = cur.GetStairState();
//				blockIE.BlockType = cur.GetBlockType();
//				blockIE.Rotation = cur.GetRotation();
//				blockIE.Flags[(int)IE.V3.BlockIE.Flag.Rotation] = true;
//				++done;
//			}
//			if (done > 0)
//			{
//				Structures.SetStrucModified(m_StrucEdit.IDXIE);
//				return false; // Something has been done, stop changing things this frame
//			}
//			return true;
//		}
//		bool OnRampToggle()
//		{
//			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.Ramp))
//				return true; // Nothing has been done

//			bool shift = Input.GetKey(KeyCode.LeftShift);

//			var strucIE = Structures.Strucs[m_StrucEdit.IDXIE];
//			int done = 0;
//			for (int i = 0; i < m_Selected.Count; ++i)
//			{
//				var cur = m_Selected[i];
//				if (cur.GetLayer() == 0 || cur.GetBlockType() == Def.BlockType.WIDE)
//					continue;

//				var blockIE = strucIE.GetBlocks()[cur.GetIDXIE()];

//				if (shift)
//				{
//					if (cur.GetStairState() == Def.StairState.ALWAYS || cur.GetStairState() == Def.StairState.RAMP_ALWAYS)
//					{
//						//var check = cur;
//						GameUtils.ApplyFnBlockAbove(cur, (IBlock b) => b.SetHeight(b.GetHeight() - 0.5f));
//						//while (check.GetStackedBlocks()[1] != null)
//						//{
//						//	check = (CBlockEdit)check.GetStackedBlocks()[1];
//						//	check.SetHeight(check.GetHeight() - 0.5f);
//						//}
//					}
//					cur.SetStairState(Def.StairState.STAIR_OR_RAMP);

//					if (cur.GetLockState() != Def.LockState.Locked)
//						cur.SetLockState(Def.LockState.SemiLocked);

//					blockIE.Stair = cur.GetStairState();
//					blockIE.BlockType = cur.GetBlockType();
//					blockIE.Rotation = cur.GetRotation();
//					blockIE.Flags[(int)IE.V3.BlockIE.Flag.Rotation] = true;
//					++done;
//					continue;
//				}

//				var stair = cur.GetStairState();
//				switch (stair)
//				{
						
//					case Def.StairState.NONE:
//					case Def.StairState.POSSIBLE:
//					case Def.StairState.ALWAYS:
//					case Def.StairState.STAIR_OR_RAMP:
//						{
//							if (stair == Def.StairState.ALWAYS)
//							{
//								//var check = cur;
//								GameUtils.ApplyFnBlockAbove(cur, (IBlock b) => b.SetHeight(b.GetHeight() - 0.5f));
//								//while (check.GetStackedBlocks()[1] != null)
//								//{
//								//	check = (CBlockEdit)check.GetStackedBlocks()[1];
//								//	check.SetHeight(check.GetHeight() - 0.5f);
//								//}
//							}
//							cur.SetStairState(Def.StairState.RAMP_POSSIBLE);
//						}
//						break;
//					case Def.StairState.RAMP_POSSIBLE:
//						{
//							//var check = cur;
//							GameUtils.ApplyFnBlockAbove(cur, (IBlock b) => b.SetHeight(b.GetHeight() + 0.5f));
//							//while (check.GetStackedBlocks()[1] != null)
//							//{
//							//	check = (CBlockEdit)check.GetStackedBlocks()[1];
//							//	check.SetHeight(check.GetHeight() + 0.5f);
//							//}
//							cur.SetStairState(Def.StairState.RAMP_ALWAYS);
//						}
//						break;
//					case Def.StairState.RAMP_ALWAYS:
//						{
//							//var check = cur;
//							GameUtils.ApplyFnBlockAbove(cur, (IBlock b) => b.SetHeight(b.GetHeight() - 0.5f));
//							//while (check.GetStackedBlocks()[1] != null)
//							//{
//							//	check = (CBlockEdit)check.GetStackedBlocks()[1];
//							//	check.SetHeight(check.GetHeight() - 0.5f);
//							//}
//							cur.SetStairState(Def.StairState.NONE);
//						}
//						break;
//				}

//				if (cur.GetLockState() != Def.LockState.Locked)
//					cur.SetLockState(Def.LockState.SemiLocked);

//				blockIE.Stair = cur.GetStairState();
//				blockIE.BlockType = cur.GetBlockType();
//				blockIE.Rotation = cur.GetRotation();
//				blockIE.Flags[(int)IE.V3.BlockIE.Flag.Rotation] = true;
//				++done;
//			}
//			if (done > 0)
//			{
//				Structures.SetStrucModified(m_StrucEdit.IDXIE);
//				return false; // Something has been done, stop changing things this frame
//			}
//			return true;
//		}
//		// Converts the selected block into a WIDE one if its possible, if it already is, will convert it into a normal one
//		bool OnWideToggle()
//		{
//			if (!Input.GetKey(KeyCode.Z))
//				return true;  // Nothing has been done
//			//var strucIE = Structures.Strucs[m_StrucEdit.IDXIE];

//			for(int i = 0; i < m_Selected.Count; ++i)
//			{
//				var cur = m_Selected[i];
//				cur.SetSelected(false);
//				if(cur.GetLayer() == 0 || cur.IsRemoved())
//				{
//					continue;
//				}
//				m_StrucEdit.SetBlockWide(m_Selected[i]);
//			}
//			m_Selected.Clear();

//			//void task()
//			//{
//			//    if (m_Selected.Count == 0)
//			//        return;

//			//    var cur = m_Selected[0];

//			//    if (cur.GetLayer() == 0 || cur.IsRemoved())
//			//    {
//			//        cur.SetSelected(false);
//			//        m_Selected.RemoveAt(0);
//			//        if (m_Selected.Count == 0)
//			//            return;
//			//        BackgroundQueue.Mgr.ScheduleTask(new BackgroundQueue.Task()
//			//        {
//			//            Cost = BackgroundQueue.DefaultMaxCostPerFrame,
//			//            FN = task
//			//        });
//			//        return;
//			//    }

//			//    var cPilar = cur.GetPilar();
//			//    var blockPos = m_StrucEdit.VPosFromPilarID(cPilar.GetStructureID());
//			//    if (blockPos.x >= m_StrucEdit.GetWidth() || blockPos.x < 0 ||
//			//        blockPos.y >= m_StrucEdit.GetHeight() || blockPos.y < 0)
//			//    {
//			//        // Ouside bounds or too close to them
//			//        cur.SetSelected(false);
//			//        m_Selected.RemoveAt(0);
//			//        if (m_Selected.Count == 0)
//			//            return;
//			//        BackgroundQueue.Mgr.ScheduleTask(new BackgroundQueue.Task()
//			//        {
//			//            Cost = BackgroundQueue.DefaultMaxCostPerFrame,
//			//            FN = task
//			//        });
//			//        return;
//			//    }

//			//    var rPos = new Vector2Int(blockPos.x + 1, blockPos.y);
//			//    var bPos = new Vector2Int(blockPos.x, blockPos.y + 1);
//			//    var brPos = new Vector2Int(blockPos.x + 1, blockPos.y + 1);

//			//    var rPID = m_StrucEdit.PilarIDFromVPos(rPos);
//			//    var bPID = m_StrucEdit.PilarIDFromVPos(bPos);
//			//    var brPID = m_StrucEdit.PilarIDFromVPos(brPos);

//			//    var rPilar = m_StrucEdit.GetPilars()[rPID];
//			//    var bPilar = m_StrucEdit.GetPilars()[bPID];
//			//    var brPilar = m_StrucEdit.GetPilars()[brPID];

//			//    if (rPilar == null || bPilar == null || brPilar == null)
//			//    {
//			//        // Some pilars are missing
//			//        cur.SetSelected(false);
//			//        m_Selected.RemoveAt(0);
//			//        if (m_Selected.Count == 0)
//			//            return;
//			//        BackgroundQueue.Mgr.ScheduleTask(new BackgroundQueue.Task()
//			//        {
//			//            Cost = BackgroundQueue.DefaultMaxCostPerFrame,
//			//            FN = task
//			//        });
//			//        return;
//			//    }

//			//    var wides = m_StrucEdit.SetBlockWide(cur);
//			//    if(wides.Count == 0)
//			//    {
//			//        if (m_Selected.Contains(cur))
//			//        {
//			//            cur.SetSelected(false);
//			//            m_Selected.Remove(cur);
//			//        }
//			//        if (m_Selected.Count == 0)
//			//            return;
//			//        BackgroundQueue.Mgr.ScheduleTask(new BackgroundQueue.Task()
//			//        {
//			//            Cost = BackgroundQueue.DefaultMaxCostPerFrame,
//			//            FN = task
//			//        });
//			//        return;
//			//    }

//			//    void RemoveDuplicates(CPilar pilar)
//			//    {
//			//        for (int j = 0; j < pilar.GetBlocks().Count; ++j)
//			//        {
//			//            var block = (CBlockEdit)pilar.GetBlocks()[j];
//			//            block.SetSelected(false);
//			//            if (m_Selected.Contains(block))
//			//                m_Selected.Remove(block);
//			//        }
//			//    }

//			//    if (wides.Count > 0)
//			//    {
//			//        RemoveDuplicates(cPilar);
//			//        RemoveDuplicates(rPilar);
//			//        RemoveDuplicates(bPilar);
//			//        RemoveDuplicates(brPilar);
//			//    }

//			//    if (m_Selected.Contains(cur))
//			//    {
//			//        cur.SetSelected(false);
//			//        m_Selected.Remove(cur);
//			//    }
//			//    if (m_Selected.Count == 0)
//			//        return;
//			//    BackgroundQueue.Mgr.ScheduleTask(new BackgroundQueue.Task()
//			//    {
//			//        Cost = BackgroundQueue.DefaultMaxCostPerFrame,
//			//        FN = task
//			//    });
//			//    return;
//			//}

//			//BackgroundQueue.Mgr.ScheduleTask(new BackgroundQueue.Task()
//			//{
//			//    Cost = BackgroundQueue.DefaultMaxCostPerFrame,
//			//    FN = task
//			//});

//			////int done = 0;
//			////for (int i = 0; i < m_Selected.Count; ++i)
//			////{
//			////    var cur = m_Selected[i];

//			////    if (cur.GetLayer() == 0 || cur.IsRemoved())
//			////        continue;

//			////    //IE.V3.BlockIE blockIE = null;

//			////    //if(cur.GetBlockType() == Def.BlockType.WIDE)
//			////    //{
//			////    //    cur.SetBlockType(Def.BlockType.NORMAL);
//			////    //    blockIE = strucIE.GetBlocks()[cur.GetIDXIE()];
//			////    //    blockIE.BlockType = Def.BlockType.COUNT;
//			////    //    blockIE.SetFlag(IE.V3.BlockIE.Flag.BlockType, false);
//			////    //    if(cur.GetLockState() == Def.LockState.Locked)
//			////    //    {
//			////    //        cur.SetLockState(Def.LockState.SemiLocked);
//			////    //    }
//			////    //    else
//			////    //    {
//			////    //        int flags = blockIE.Flags.Count((bool b) => b == true);
//			////    //        if (cur.IsAnchor())
//			////    //            flags -= 1;
//			////    //        if (flags > 0)
//			////    //            cur.SetLockState(Def.LockState.SemiLocked);
//			////    //    }
//			////    //    continue;
//			////    //}
//			////    var cPilar = cur.GetPilar();
//			////    var blockPos = m_StrucEdit.VPosFromPilarID(cPilar.GetStructureID());
//			////    if (blockPos.x >= m_StrucEdit.GetWidth() || blockPos.x < 0 ||
//			////        blockPos.y >= m_StrucEdit.GetHeight() || blockPos.y < 0)
//			////        continue; // Ouside bounds or too close to them

//			////    var rPos = new Vector2Int(blockPos.x + 1, blockPos.y);
//			////    var bPos = new Vector2Int(blockPos.x, blockPos.y + 1);
//			////    var brPos = new Vector2Int(blockPos.x + 1, blockPos.y + 1);

//			////    var rPID = m_StrucEdit.PilarIDFromVPos(rPos);
//			////    var bPID = m_StrucEdit.PilarIDFromVPos(bPos);
//			////    var brPID = m_StrucEdit.PilarIDFromVPos(brPos);

//			////    var rPilar = m_StrucEdit.GetPilars()[rPID];
//			////    var bPilar = m_StrucEdit.GetPilars()[bPID];
//			////    var brPilar = m_StrucEdit.GetPilars()[brPID];

//			////    if (rPilar == null || bPilar == null || brPilar == null)
//			////        continue;

//			////    var wides = m_StrucEdit.SetBlockWide(cur);

//			////    void RemoveDuplicates(CPilar pilar)
//			////    {
//			////        for(int j = 0; j < pilar.GetBlocks().Count; ++j)
//			////        {
//			////            var block = (CBlockEdit)pilar.GetBlocks()[j];
//			////            block.SetSelected(false);
//			////            if (m_Selected.Contains(block))
//			////                m_Selected.Remove(block);
//			////        }
//			////    }

//			////    if (wides.Count > 0)
//			////    {
//			////        RemoveDuplicates(cPilar);
//			////        RemoveDuplicates(rPilar);
//			////        RemoveDuplicates(bPilar);
//			////        RemoveDuplicates(brPilar);
//			////        ++done;
//			////    }
//			////}
//			////if (done > 0)
//			////{
//			////    Structures.SetStrucModified(m_StrucEdit.IDXIE);
//			////    return false; // Something has been done, stop changing things this frame
//			////}
//			return true;
//		}
//		// Locks the block properties and if they are locked it unlocks them
//		bool OnLockStateToggle()
//		{
//			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.Lock))
//				return true;  // Nothing has been done
//			var strucIE = Structures.Strucs[m_StrucEdit.IDXIE];
//			int done = 0;
//			for (int i = 0; i < m_Selected.Count; ++i)
//			{
//				var cur = m_Selected[i];
//				if (cur.GetLayer() == 0)
//					continue;

//				var blockIE = strucIE.GetBlocks()[cur.GetIDXIE()];
//				var layer = m_StrucEdit.GetLayers()[cur.GetLayer() - 1];
//				if(layer.IsLinkedLayer)
//				{
//					layer = m_StrucEdit.GetLayers()[cur.GetLinkedTo() - 1];
//				}

//				// Lock the block properties
//				if(cur.GetLockState() != Def.LockState.Locked)
//				{
//					blockIE.Rotation = cur.GetRotation();
//					blockIE.SetFlag(IE.V3.BlockIE.Flag.BlockType, true);
//					blockIE.BlockType = cur.GetBlockType();
//					blockIE.SetFlag(IE.V3.BlockIE.Flag.Height, true);
//					blockIE.Height = cur.GetHeight();
//					blockIE.MicroHeight = cur.GetMicroHeight();
//					blockIE.SetFlag(IE.V3.BlockIE.Flag.Length, true);
//					blockIE.Length = cur.GetLength();
//					blockIE.SetFlag(IE.V3.BlockIE.Flag.MaterialFamily, true);
//					blockIE.MaterialFamily = cur.GetMaterialFamily().FamilyInfo.FamilyName;
//					blockIE.SetFlag(IE.V3.BlockIE.Flag.Monster, true);
//					blockIE.MonsterFamily = cur.GetMonster() != null ? cur.GetMonster().GetFamily().Name : "";
//					blockIE.SetFlag(IE.V3.BlockIE.Flag.Prop, true);
//					blockIE.PropFamily = cur.GetProp() != null ? cur.GetProp().GetInfo().FamilyName : "";
//					blockIE.SetFlag(IE.V3.BlockIE.Flag.Void, true);
//					blockIE.BlockVoid = cur.GetVoidState();
//					blockIE.Stair = cur.GetStairState();
//					cur.SetLockState(Def.LockState.Locked);
//				}
//				// Unlock the block properties
//				else
//				{
//					blockIE.SetDefault();
//					blockIE.SetFlag(IE.V3.BlockIE.Flag.Anchor, cur.IsAnchor());
//					blockIE.Layer = (byte)cur.GetLayer();
//					blockIE.Stair = cur.GetStairState();
//					if (cur.GetStairState() != Def.StairState.NONE)
//					{
//						blockIE.Rotation = cur.GetRotation();
//						blockIE.SetFlag(IE.V3.BlockIE.Flag.Rotation, true);
//					}
//					blockIE.BlockType = cur.GetBlockType();
//					if(cur.GetHeight() != layer.BlockHeight && !cur.IsStackLinkValid(0) /*cur.GetStackedBlocks()[0] == null*/)
//					{
//						blockIE.Height = cur.GetHeight();
//						blockIE.SetFlag(IE.V3.BlockIE.Flag.Height, true);
//					}
//					if(Mathf.Clamp(cur.GetLength(), layer.BlockLengthMin, layer.BlockLengthMax) != cur.GetLength())
//					{
//						blockIE.Length = cur.GetLength();
//						blockIE.SetFlag(IE.V3.BlockIE.Flag.Length, true);
//					}

//					if(blockIE.Flags.Contains(true))
//					{
//						cur.SetLockState(Def.LockState.SemiLocked);
//					}
//					else
//					{
//						cur.SetLockState(Def.LockState.Unlocked);
//					}
//				}
//				++done;
//			}
//			if (done > 0)
//			{
//				Structures.SetStrucModified(m_StrucEdit.IDXIE);
//				return false; // Something has been done, stop changing things this frame
//			}
//			return true;
//		}
//		// Makes the block an anchor point or if it already was it undoes that, and changes BlockIE
//		bool OnAnchorToggle()
//		{
//			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.Anchor))
//				return true;  // Nothing has been done
//			var strucIE = Structures.Strucs[m_StrucEdit.IDXIE];
//			int done = 0;
//			for (int i = 0; i < m_Selected.Count; ++i)
//			{
//				var cur = m_Selected[i];
//				if (cur.GetLayer() == 0)
//					continue;

//				cur.SetAnchor(!cur.IsAnchor());
//				cur.SetStairState(Def.StairState.NONE);

//				var blockIE = strucIE.GetBlocks()[cur.GetIDXIE()];
//				blockIE.Flags[(int)IE.V3.BlockIE.Flag.Anchor] = cur.IsAnchor();
//				blockIE.Stair = Def.StairState.NONE;

//				++done;
//			}
//			if (done > 0)
//			{
//				Structures.SetStrucModified(m_StrucEdit.IDXIE);
//				return false; // Something has been done, stop changing things this frame
//			}
//			return true;
//		}
//		// Rotate the block 90º, and change the BlockIE
//		bool OnRotationToggle()
//		{
//			int rotation;
//			if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.RotateLeft))
//				rotation = -1;
//			else if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.RotateRight))
//				rotation = +1;
//			else
//				return true;  // Nothing has been done
//			var strucIE = Structures.Strucs[m_StrucEdit.IDXIE];
//			int done = 0;
//			for (int i = 0; i < m_Selected.Count; ++i)
//			{
//				var cur = m_Selected[i];
//				if (cur.GetLayer() == 0)
//					continue;

//				var rot = cur.GetRotation();
//				if (rotation < 0 && rot == Def.RotationState.Default)
//					rot = Def.RotationState.Left;
//				else if (rotation > 0 && rot == Def.RotationState.Left)
//					rot = Def.RotationState.Default;
//				else
//					rot = rot + rotation;

//				cur.SetRotation(rot);
//				if (cur.GetLockState() != Def.LockState.Locked)
//					cur.SetLockState(Def.LockState.SemiLocked);

//				var blockIE = strucIE.GetBlocks()[cur.GetIDXIE()];
//				blockIE.Rotation = rot;
//				blockIE.SetFlag(IE.V3.BlockIE.Flag.Rotation, true);
//				++done;
//			}
//			if (done > 0)
//			{
//				Structures.SetStrucModified(m_StrucEdit.IDXIE);
//				return false; // Something has been done, stop changing things this frame
//			}
//			return true;
//		}
//		// Make the block larger up to +-0.5m, and change the BlockIE
//		bool OnLengthChange()
//		{
//			float length;
//			if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.LengthDown))
//				length = -0.5f;
//			else if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.LengthUp))
//				length = +0.5f;
//			else
//				return true;  // Nothing has been done

//			//bool shift = Input.GetKey(KeyCode.LeftShift);

//			var strucIE = Structures.Strucs[m_StrucEdit.IDXIE];
//			int done = 0;
//			for (int i = 0; i < m_Selected.Count; ++i)
//			{
//				var cur = m_Selected[i];

//				if (cur.GetLayer() == 0)
//					continue;

//				var oLen = cur.GetLength();
//				var nLen = cur.GetLength();
//				if (nLen == 3.4f && length < 0f)
//					nLen = 3f;
//				else
//					nLen += length;
//				if (nLen < 0.5f)
//				{
//					nLen = 0.5f;
//				}
//				else if (nLen > 3.4f)
//				{
//					nLen = 3.4f;
//				}
//				if (oLen == nLen)
//					continue;
//				//var oHeight = cur.GetHeight();
//				cur.SetLength(nLen);

//				nLen = cur.GetLength();

//				if (oLen == nLen)
//					continue;

//				void setInfoToIE(CBlockEdit block)
//				{
//					if (block.GetLockState() != Def.LockState.Locked)
//						block.SetLockState(Def.LockState.SemiLocked);

//					var blockIE = strucIE.GetBlocks()[block.GetIDXIE()];
//					blockIE.Length = block.GetLength();
//					blockIE.SetFlag(IE.V3.BlockIE.Flag.Length, true);
//				}

//				//if (length > 0f)
//				//	cur.IncreaseLengthCheck(false, true);
//				//else
//				//	cur.DecreseLengthCheck(false, true);

//				setInfoToIE(cur);

//				//var hDiff = nLen - oLen;

//				//void setInfoToIE(CBlockEdit block)
//				//{
//				//    if (block.GetLockState() != Def.LockState.Locked)
//				//        block.SetLockState(Def.LockState.SemiLocked);

//				//    var blockIE = strucIE.GetBlocks()[block.GetIDXIE()];
//				//    blockIE.Length = block.GetLength();
//				//    blockIE.Flags[(int)IE.V3.BlockIE.Flag.Length] = true;
//				//}

//				//CBlockEdit getBlockAbove(float wantedHeight)
//				//{
//				//    CBlockEdit block = null;
//				//    var pilar = cur.GetPilar();
//				//    for (int j = 0; j < pilar.GetBlocks().Count; ++j)
//				//    {
//				//        var b = (CBlockEdit)pilar.GetBlocks()[j];
//				//        var len = b.GetLength();
//				//        if (len == 3.4f)
//				//            len = 3.5f;
//				//        if ((b.GetHeight() - len) == wantedHeight)
//				//        {
//				//            block = b;
//				//            break;
//				//        }
//				//    }
//				//    return block;
//				//}

//				//CBlockEdit getBlockBelow(float wantedHeight)
//				//{
//				//    CBlockEdit block = null;
//				//    var pilar = cur.GetPilar();
//				//    for (int j = 0; j < pilar.GetBlocks().Count; ++j)
//				//    {
//				//        var b = (CBlockEdit)pilar.GetBlocks()[j];
//				//        float h = b.GetHeight() + (b.GetBlockType() == Def.BlockType.STAIRS ? 0.5f : 0f);
//				//        if (h == wantedHeight)
//				//        {
//				//            block = b;
//				//            break;
//				//        }
//				//    }
//				//    return block;
//				//}

//				//if (length > 0f)
//				//{
//				//    while (cur != null)
//				//    {
//				//        var curPilarID = cur.GetPilar().GetBlocks().IndexOf(cur);
//				//        bool growUp = curPilarID != 0;
//				//        if (shift)
//				//            growUp = !growUp;

//				//        if (cur.GetStackedBlocks()[1] != null)
//				//        {
//				//            if (growUp)
//				//            {
//				//                //cur.GetStackedBlocks()[1].SetHeight(cur.GetStackedBlocks()[1].GetHeight() + hDiff);
//				//                cur.SetHeight(cur.GetHeight() + 0.5f);
//				//                setInfoToIE(cur);
//				//                if (curPilarID == 0 && shift)
//				//                    shift = false;
//				//                cur = (CBlockEdit)cur.GetStackedBlocks()[1];
//				//            }
//				//            else
//				//            {
//				//                if (cur.GetStackedBlocks()[0] != null)
//				//                {
//				//                    cur = (CBlockEdit)cur.GetStackedBlocks()[0];
//				//                    cur.SetHeight(cur.GetHeight() - 0.5f);
//				//                    setInfoToIE(cur);
//				//                    if (cur.GetPilar().GetBlocks().IndexOf(cur) == 0)
//				//                    {
//				//                        cur = null;
//				//                    }
//				//                }
//				//                else
//				//                {
//				//                    var l = cur.GetLength();
//				//                    if (l == 3.4f)
//				//                        l = 3.5f;
//				//                    var below = getBlockBelow(cur.GetHeight() - l);
//				//                    if(below != null)
//				//                    {
//				//                        cur.GetStackedBlocks()[0] = below;
//				//                        below.GetStackedBlocks()[1] = cur;
//				//                    }
//				//                    cur = null;
//				//                }
//				//            }
//				//        }
//				//        else if (cur.GetStackedBlocks()[0] != null)
//				//        {
//				//            if (growUp)
//				//            {
//				//                cur.SetHeight(cur.GetHeight() + 0.5f);
//				//                setInfoToIE(cur);
//				//                var above = getBlockAbove(cur.GetHeight() + (cur.GetBlockType() == Def.BlockType.STAIRS ? 0.5f : 0f));
//				//                if (above != null)
//				//                {
//				//                    cur.GetStackedBlocks()[1] = above;
//				//                    above.GetStackedBlocks()[0] = cur;
//				//                }
//				//                cur = null;
//				//            }
//				//            else
//				//            {
//				//                cur = (CBlockEdit)cur.GetStackedBlocks()[0];
//				//                cur.SetHeight(cur.GetHeight() - 0.5f);
//				//                setInfoToIE(cur);
//				//                if (cur.GetPilar().GetBlocks().IndexOf(cur) == 0)
//				//                {
//				//                    cur = null;
//				//                }
//				//            }
//				//        }
//				//        else
//				//        {
//				//            if (growUp)
//				//            {
//				//                cur.SetHeight(cur.GetHeight() + 0.5f);
//				//                var above = getBlockAbove(cur.GetHeight() + (cur.GetBlockType() == Def.BlockType.STAIRS ? 0.5f : 0f));
//				//                if (above != null)
//				//                {
//				//                    cur.GetStackedBlocks()[1] = above;
//				//                    above.GetStackedBlocks()[0] = cur;
//				//                }
//				//            }
//				//            else
//				//            {
//				//                var l = cur.GetLength();
//				//                if (l == 3.4f)
//				//                    l = 3.5f;
//				//                var below = getBlockBelow(cur.GetHeight() - l);
//				//                if (below != null)
//				//                {
//				//                    cur.GetStackedBlocks()[0] = below;
//				//                    below.GetStackedBlocks()[1] = cur;
//				//                }
//				//            }
//				//            cur = null;
//				//        }
//				//    }
//				//}
//				//else
//				//{
//				//    while (cur != null)
//				//    {
//				//        var curPilarID = cur.GetPilar().GetBlocks().IndexOf(cur);
//				//        bool shrinkUp = curPilarID == 0;
//				//        if (shift)
//				//            shrinkUp = !shrinkUp;

//				//        if(shrinkUp && cur.GetStackedBlocks()[0] != null)
//				//        {
//				//            cur = (CBlockEdit)cur.GetStackedBlocks()[0];
//				//            cur.SetHeight(cur.GetHeight() + 0.5f);
//				//            if (cur.GetStackedBlocks()[0] == null)
//				//            {
//				//                cur = null;
//				//            }
//				//            else if (cur.GetPilar().GetBlocks().IndexOf(cur) == 0)
//				//            {
//				//                shift = !shift;
//				//            }
//				//        }
//				//        else if(!shrinkUp)
//				//        {
//				//            cur.SetHeight(cur.GetHeight() - 0.5f);
//				//            if(cur.GetStackedBlocks()[1] != null)
//				//            {
//				//                cur = (CBlockEdit)cur.GetStackedBlocks()[1];
//				//                if (cur.GetStackedBlocks()[1] == null)
//				//                {
//				//                    cur.SetHeight(cur.GetHeight() - 0.5f);
//				//                    cur = null;
//				//                }
//				//                else if(curPilarID == 0)
//				//                {
//				//                    shift = !shift;
//				//                }
//				//            }
//				//            else
//				//            {
//				//                cur = null;
//				//            }
//				//        }
//				//        else
//				//        {
//				//            cur = null;
//				//        }
//				//    }
//				//}

//				++done;
//			}
//			if (done > 0)
//			{
//				Structures.SetStrucModified(m_StrucEdit.IDXIE);
//				return false; // Something has been done, stop changing things this frame
//			}
//			return true;
//		}
//		// Move up/down the block by 0.5m and change the BlockIE
//		bool OnHeightChange()
//		{
//			float height;
//			if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.HeightDown))
//				height = -0.5f;
//			else if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.HeightUp))
//				height = +0.5f;
//			else
//				return true;  // Nothing has been done

//			//bool shift = Input.GetKey(KeyCode.LeftShift);

//			var strucIE = Structures.Strucs[m_StrucEdit.IDXIE];
//			int done = 0;
//			for (int i = 0; i < m_Selected.Count; ++i)
//			{
//				var cur = m_Selected[i];
//				if (cur.GetLayer() == 0)
//					continue;

//				void setInfoToIE(CBlockEdit block)
//				{
//					if (block.GetLockState() != Def.LockState.Locked)
//						block.SetLockState(Def.LockState.SemiLocked);

//					var blockIE = strucIE.GetBlocks()[block.GetIDXIE()];
//					blockIE.Height = block.GetHeight();
//					blockIE.SetFlag(IE.V3.BlockIE.Flag.Height, true);
//				}
//				cur.SetHeight(cur.GetHeight() + height);
//				//if (height > 0f)
//				//	cur.IncreaseHeightCheck(false, false, true);
//				//else
//				//	cur.DecreseHeightCheck(false, false, true);

//				setInfoToIE(cur);
//				GameUtils.ApplyFnBlockAbove(cur, (IBlock b) => setInfoToIE(b as CBlockEdit));
//				GameUtils.ApplyFnBlockBelow(cur, (IBlock b) => setInfoToIE(b as CBlockEdit));

//				//CBlockEdit getBlockAbove(float wantedHeight)
//				//{
//				//    CBlockEdit block = null;
//				//    var pilar = cur.GetPilar();
//				//    for(int j = 0; j < pilar.GetBlocks().Count; ++j)
//				//    {
//				//        var b = (CBlockEdit)pilar.GetBlocks()[j];
//				//        var len = b.GetLength();
//				//        if (len == 3.4f)
//				//            len = 3.5f;
//				//        if((b.GetHeight() - len) == wantedHeight)
//				//        {
//				//            block = b;
//				//            break;
//				//        }
//				//    }
//				//    return block;
//				//}

//				//CBlockEdit getBlockBelow(float wantedHeight)
//				//{
//				//    CBlockEdit block = null;
//				//    var pilar = cur.GetPilar();
//				//    for (int j = 0; j < pilar.GetBlocks().Count; ++j)
//				//    {
//				//        var b = (CBlockEdit)pilar.GetBlocks()[j];
//				//        float h = b.GetHeight() + (b.GetBlockType() == Def.BlockType.STAIRS ? 0.5f : 0f);
//				//        if (h == wantedHeight)
//				//        {
//				//            block = b;
//				//            break;
//				//        }
//				//    }
//				//    return block;
//				//}

//				//void dragCheck(CBlockEdit block, CBlockEdit prev)
//				//{
//				//    var upBlock = (CBlockEdit)block.GetStackedBlocks()[1];
//				//    var dnBlock = (CBlockEdit)block.GetStackedBlocks()[0];
//				//    if (upBlock == prev)
//				//        upBlock = null;
//				//    if (dnBlock == prev)
//				//        dnBlock = null;
//				//    if (height > 0f)
//				//    {
//				//        if (upBlock != null)
//				//        {
//				//            upBlock.SetHeight(upBlock.GetHeight() + 0.5f);
//				//            setInfoToIE(upBlock);
//				//            dragCheck(upBlock, block);
//				//        }
//				//        else
//				//        {
//				//            upBlock = getBlockAbove(block.GetHeight() + (block.GetBlockType() == Def.BlockType.STAIRS ? 0.5f : 0f));
//				//            if(upBlock != null)
//				//            {
//				//                upBlock.GetStackedBlocks()[0] = block;
//				//                block.GetStackedBlocks()[1] = upBlock;
//				//            }
//				//        }
//				//        if(dnBlock != null)
//				//        {
//				//            if(shift)
//				//            {
//				//                dnBlock.GetStackedBlocks()[1] = null;
//				//                block.GetStackedBlocks()[0] = null;
//				//            }
//				//            else
//				//            {
//				//                dnBlock.SetHeight(dnBlock.GetHeight() + 0.5f);
//				//                setInfoToIE(dnBlock);
//				//                dragCheck(dnBlock, block);
//				//            }
//				//        }
//				//    }
//				//    else
//				//    {
//				//        if (upBlock != null)
//				//        {
//				//            if (shift)
//				//            {
//				//                upBlock.GetStackedBlocks()[0] = null;
//				//                block.GetStackedBlocks()[1] = null;
//				//            }
//				//            else
//				//            {
//				//                upBlock.SetHeight(upBlock.GetHeight() - 0.5f);
//				//                setInfoToIE(upBlock);
//				//                dragCheck(upBlock, block);
//				//            }
//				//        }
//				//        if (dnBlock != null)
//				//        {
//				//            dnBlock.SetHeight(dnBlock.GetHeight() - 0.5f);
//				//            setInfoToIE(dnBlock);
//				//            dragCheck(dnBlock, block);
//				//        }
//				//        else
//				//        {
//				//            var len = block.GetLength();
//				//            if (len == 3.4f)
//				//                len = 3.5f;
//				//            dnBlock = getBlockBelow(block.GetHeight() - len);
//				//            if (dnBlock != null)
//				//            {
//				//                dnBlock.GetStackedBlocks()[1] = block;
//				//                block.GetStackedBlocks()[0] = dnBlock;
//				//            }
//				//        }
//				//    }
//				//}

//				//setInfoToIE(cur);
//				//dragCheck(cur, null);

//				++done;
//			}
//			if (done > 0)
//			{
//				Structures.SetStrucModified(m_StrucEdit.IDXIE);
//				return false; // Something has been done, stop changing things this frame
//			}
//			return true;
//		}
//		bool OnDestroyBlock()
//		{
//			if(!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.DestroyBlock))
//				return true;  // Nothing has been done
//			var strucIE = Structures.Strucs[m_StrucEdit.IDXIE];
//			int done = 0;
//			for (int i = 0; i < m_Selected.Count; ++i)
//			{
//				var cur = m_Selected[i];

//				if (cur.GetLayer() == 0)
//				{
//					cur.SetSelected(false);
//					continue;
//				}
//				//for(int j = 0; j < m_StrucEdit.GetPilars().Length; ++j)
//				//{
//				//	var pilar = m_StrucEdit.GetPilars()[j];
//				//	if (pilar == null)
//				//		continue;

//				//	for(int k = 0; k < pilar.GetBlocks().Count; ++k)
//				//	{
//				//		var block = (CBlockEdit)pilar.GetBlocks()[k];
//				//		if (block.GetIDXIE() > cur.GetIDXIE())
//				//			block.SetIDXIE(block.GetIDXIE() - 1);
//				//	}
//				//}

//				strucIE.RemoveBlock(cur.GetIDXIE());
//				var above = cur.GetStackedAbove();
//				var below = cur.GetStackedBelow();
//				if(above != null)
//				{
//					above.GetStackedBlocksIdx()[0] = -1;
//				}
//				if(below != null)
//				{
//					below.GetStackedBlocksIdx()[1] = -1;
//				}
//				//if(cur.GetStackedBlocks()[0] != null)
//				//{
//				//	((CBlockEdit)cur.GetStackedBlocks()[0]).GetStackedBlocks()[1] = null;
//				//}
//				//if (cur.GetStackedBlocks()[1] != null)
//				//{
//				//	((CBlockEdit)cur.GetStackedBlocks()[1]).GetStackedBlocks()[0] = null;
//				//}
//				//cur.GetStackedBlocks()[0] = null;
//				//cur.GetStackedBlocks()[1] = null;
//				cur.GetStackedBlocksIdx()[0] = -1;
//				cur.GetStackedBlocksIdx()[1] = -1;
//				cur.SetLayer(0);
//				cur.SetIDXIE(-1);
//				m_Selected.Remove(cur);
//				--i;
//				if (cur.GetPilar().GetBlocks().Count > 1)
//				{
//					cur.DestroyBlock(false);
//				}
//				else
//				{
//					cur.SetSelected(false);
//				}

//				++done;
//			}
//			m_Selected.Clear();
//			if (done > 0)
//			{
//				Structures.SetStrucModified(m_StrucEdit.IDXIE);
//				return false; // Something has been done, stop changing things this frame
//			}
//			return true;
//		}
//		// Adds a block on top of the selected one (stacking)
//		bool OnStacking()
//		{
//			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.StackBlock))
//				return true;  // Nothing has been done
//			var strucIE = Structures.Strucs[m_StrucEdit.IDXIE];
//			int done = 0;
//			var nSelected = new List<CBlockEdit>(m_Selected.Count);
//			bool AreBlocksAbove(CBlockEdit block)
//			{
//				var pilar = block.GetPilar();
//				var height = block.GetHeight();

//				for(int i = 0; i < pilar.GetBlocks().Count; ++i)
//				{
//					var b = pilar.GetBlocks()[i] as CBlockEdit;
//					if (b.GetHeight() > height)
//						return true;
//				}

//				return false;
//			}
//			void SortBlocks(CPilar pilar)
//			{
//				//var strucIE = Structures.Strucs[((CStrucEdit)pilar.GetStruc()).IDXIE];
//				// First sort the pilar block list
//				var nList = new List<IBlock>(pilar.GetBlocks().Count);
//				var ieList = new List<IE.V3.BlockIE>(pilar.GetBlocks().Count);
//				while(pilar.GetBlocks().Count > 0)
//				{
//					float minHeight = float.MaxValue;
//					CBlockEdit lowest = null;
//					int idx = 0;
//					for(int i = 0; i < pilar.GetBlocks().Count; ++i)
//					{
//						var block = pilar.GetBlocks()[i] as CBlockEdit;
//						if(block.GetHeight() < minHeight)
//						{
//							minHeight = block.GetHeight();
//							lowest = block;
//							idx = i;
//						}
//					}
//					nList.Add(lowest);
//					pilar.GetBlocks().RemoveAt(idx);
//					var blockIE = strucIE.GetBlocks()[lowest.GetIDXIE()];
//					ieList.Add(blockIE);
//					strucIE.RemoveBlock(lowest.GetIDXIE());
//				}
//				pilar.GetBlocks().AddRange(nList);
//				for(int i = 0; i < ieList.Count; ++i)
//				{
//					var block = pilar.GetBlocks()[i] as CBlockEdit;
//					var blockIE = ieList[i];
//					block.SetIDXIE(strucIE.AddBlock(blockIE));
//				}
//			}
//			for (int i = 0; i < m_Selected.Count; ++i)
//			{
//				var cur = m_Selected[i];

//				if (cur.GetLayer() == 0 || cur.IsRemoved() || cur.IsStackLinkValid(1) /*cur.GetStackedBlocks()[1] != null*/
//					/*|| cur.GetBlockType() == Def.BlockType.STAIRS*/)
//				{
//					cur.SetSelected(false);
//					m_Selected.RemoveAt(i);
//					--i;
//					continue;
//				}
//				var pilar = cur.GetPilar();

//				//for(int j = 0; j < pilar.GetBlocks().Count; ++j)
//				//{
//				//    var other = pilar.GetBlocks()[j];

//				//    if (other.GetHeight() <= cur.GetHeight())
//				//        continue;

//				//    var hDiff = (other.GetHeight() - other.GetLength()) - cur.GetHeight();
//				//    if(hDiff < 0.5f)
//				//    {
//				//        continue;
//				//    }
//				//}

//				float stairOffset = cur.GetBlockType() == Def.BlockType.STAIRS ? 0.5f : 0f;

//				if(cur.GetProp() != null)
//				{
//					//m_StrucEdit.GetLivingEntities().Remove(cur.GetProp());
//					//cur.GetProp().ReceiveDamage(Def.DamageType.UNAVOIDABLE, cur.GetProp().Prop.BaseHealth);
//					cur.GetProp().GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE,
//						cur.GetProp().GetLE().GetCurrentHealth());
//					//if (m_StrucEdit.GetLES().Contains(cur.GetProp().GetLE()))
//					//	m_StrucEdit.GetLES().Remove(cur.GetProp().GetLE());
//					//cur.SetProp(null);
//				}
//				if(cur.GetMonster() != null)
//				{
//					//m_StrucEdit.GetLivingEntities().Remove(cur.GetMonster());
//					//cur.GetMonster().ReceiveDamage(Def.DamageType.UNAVOIDABLE, cur.GetMonster().Info.BaseHealth);
//					if (m_StrucEdit.GetLES().Contains(cur.GetMonster().GetLE()))
//						m_StrucEdit.GetLES().Remove(cur.GetMonster().GetLE());
//					cur.GetMonster().GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, cur.GetMonster().GetLE().GetCurrentHealth());
//					cur.SetMonster(null);
//				}

//				var nBlock = pilar.AddBlock();
//				nBlock.SetHeight(cur.GetHeight() + stairOffset + 0.5f);
//				nBlock.SetMicroHeight(cur.GetMicroHeight());
//				var blockIE = new IE.V3.BlockIE()
//				{
//					Layer = (byte)cur.GetLayer(),
//					StructureID = (ushort)pilar.GetStructureID(),
//					//MicroHeight = nBlock.GetMicroHeight(),
//				};
//				nBlock.SetIDXIE(strucIE.AddBlock(blockIE));
//				nBlock.SetLength(0.5f);
//				cur.GetStackedBlocksIdx()[1] = nBlock.GetPilarIndex();
//				//cur.GetStackedBlocks()[1] = nBlock;
//				nBlock.GetStackedBlocksIdx()[0] = cur.GetPilarIndex();
//				//nBlock.GetStackedBlocks()[0] = cur;
//				nBlock.SetLayer(cur.GetLayer());
//				nBlock.SetMaterialFamily(cur.GetMaterialFamily());
//				nBlock.SetSelected(true);
//				nSelected.Add(nBlock);

//				if (AreBlocksAbove(nBlock))
//					SortBlocks(nBlock.GetPilar());

//				cur.SetSelected(false);
//				m_Selected.RemoveAt(i);
//				--i;

//				++done;
//			}
//			if (done > 0)
//			{
//				Structures.SetStrucModified(m_StrucEdit.IDXIE);
//				m_Selected = nSelected;
//				return false; // Something has been done, stop changing things this frame
//			}
//			return true;
//		}
//		// Adds the block to a layer, if the block was invalid, now will be valid and with the properties from the layer
//		bool OnBlockLayerChange()
//		{
//			var strucIE = Structures.Strucs[m_StrucEdit.IDXIE];
//			void ChangeLayer(int layer)
//			{
//				for(int i = 0; i < m_Selected.Count; ++i)
//				{
//					var cur = m_Selected[i];

//					IE.V3.BlockIE blockIE = null;
//					bool semiLock = false;
//					if(cur.GetIDXIE() < 0)
//					{
//						blockIE = new IE.V3.BlockIE()
//						{
//							Layer = (byte)layer
//						};
//						cur.SetIDXIE(strucIE.AddBlock(blockIE));
//					}
//					else
//					{
//						blockIE = strucIE.GetBlocks()[cur.GetIDXIE()];
//						blockIE.Flags[(int)IE.V3.BlockIE.Flag.MaterialFamily] = false;
//						blockIE.Flags[(int)IE.V3.BlockIE.Flag.Prop] = false;
//						blockIE.Flags[(int)IE.V3.BlockIE.Flag.Monster] = false;
//						bool anchor = blockIE.Flags[(int)IE.V3.BlockIE.Flag.Anchor];
						
//						//var blockType = blockIE.Flags[(int)IE.V3.BlockIE.Flag.BlockType] ?
//						//    blockIE.BlockType : Def.BlockType.NORMAL;

//						//var height = blockIE.Flags[(int)IE.V3.BlockIE.Flag.Height] ?
//						//    blockIE.Height : 0f;

//						//var microHeight = blockIE.Flags[(int)IE.V3.BlockIE.Flag.Height] ?
//						//    blockIE.MicroHeight : 0f;

//						//var length = blockIE.Flags[(int)IE.V3.BlockIE.Flag.Length] ?
//						//    blockIE.Length : 1f;

//						//var rotation = blockIE.Flags[(int)IE.V3.BlockIE.Flag.Rotation] ?
//						//    blockIE.Rotation : Def.RotationState.Default;

//						//var voidState = blockIE.Flags[(int)IE.V3.BlockIE.Flag.Void] ?
//						//    blockIE.BlockVoid : Def.BlockVoid.NORMAL;

//						int eFlags = blockIE.Flags.Count(p => p == true);
//						if (anchor)
//							--eFlags;
//						semiLock = eFlags > 0;

//					}
//					blockIE.Layer = (byte)layer;
//					blockIE.StructureID = (ushort)cur.GetPilar().GetStructureID();
//					if (semiLock)
//						cur.SetLockState(Def.LockState.SemiLocked);
//					cur.SetLayer(layer);
//					//if(cur.GetStackedBlocks()[0] == null)
//					//{
//					//	var li = m_StrucEdit.GetLayers()[layer - 1];
//					//	if (li.IsLinkedLayer)
//					//		li = m_StrucEdit.GetLayers()[cur.GetLinkedTo() - 1];

//					//	float chance = UnityEngine.Random.value *
//					//	(li.MicroHeightMax - li.MicroHeightMin) + li.MicroHeightMin;
//					//	float chance100 = chance * 100f;
//					//	float ichance100 = Mathf.Floor(chance100);
//					//	float fchance100 = chance100 - ichance100;
//					//	fchance100 *= 0.01f;
//					//	if (fchance100 >= 0.5f)
//					//		chance = ichance100 * 0.01f + 0.05f;
//					//	else
//					//		chance = ichance100 * 0.01f;

//					//	chance = Mathf.Clamp(chance, -0.25f, 0.25f);
//					//	cur.SetMicroHeight(chance);
//					//	var topBlock = cur.GetStackedBlocks()[1] as CBlockEdit;
//					//	while (topBlock != null)
//					//	{
//					//		topBlock.SetMicroHeight(chance);
//					//		topBlock = topBlock.GetStackedBlocks()[1] as CBlockEdit;
//					//	}
//					//}
//				}
//				if (m_Selected.Count > 0)
//					Structures.SetStrucModified(m_StrucEdit.IDXIE);
//			}

//			for(int i = Def.MaxLayerSlots - 1; i >= 0; --i)
//			{
//				if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.Layer1 + i))
//					continue;

//				if (!m_StrucEdit.GetLayers()[i].IsValid())
//					continue;

//				ChangeLayer(i + 1);
//				return false;
//			}

//			//if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
//			//{
//			//	for(int i = 0; i < 3; ++i) // 10, 11, 12
//			//	{
//			//		if (!Input.GetKey(KeyCode.Alpha0 + i) && !Input.GetKey(KeyCode.Keypad0 + i))
//			//			continue;

//			//		if (!m_StrucEdit.GetLayers()[((10 + i) - 1)].IsValid())
//			//			continue;
//			//		ChangeLayer(10 + i);
//			//		return false;
//			//	}
//			//}
//			//else
//			//{
//			//	for (int i = 1; i < 10; ++i) // 1, 2, 3, ... 9
//			//	{
//			//		if (!Input.GetKey(KeyCode.Alpha0 + i) && !Input.GetKey(KeyCode.Keypad0 + i))
//			//			continue;

//			//		if (!m_StrucEdit.GetLayers()[(i - 1)].IsValid())
//			//			continue;
//			//		ChangeLayer(i);
//			//		return false;
//			//	}
//			//}
//			return true;
//		}
//		// Hides all the invalid blocks and all the gizmos, also enables WIDE and stair generation so that
//		// we can see how should look in game
//		public bool ToggleVisibility(bool screenShooting = false, bool screenShootingStart = false)
//		{
//			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.Visibility) && !screenShooting)
//				return true;
//			//if (BackgroundQueue.Mgr.GetScheduledTaskCount() > 0 && !screenShooting)
//			//    return;

//			//var strucIE = Structures.Strucs[m_StrucEdit.IDXIE];
//			//var blocksIE = strucIE.GetBlocks();
//			bool hideInfo;
//			if (!screenShooting)
//			{
//				hideInfo = Manager.Mgr.HideInfo = !Manager.Mgr.HideInfo;
//			}
//			else
//			{
//				hideInfo = screenShootingStart;
//			}

//			void hideUITask()
//			{
//				LeftPanel.gameObject.SetActive(!hideInfo);
//				MenuBttn.gameObject.SetActive(!hideInfo);
//				CameraPanel.gameObject.SetActive(!hideInfo);
//				FPSText.gameObject.SetActive(!hideInfo);
//				BlockPanelShowBttn.gameObject.SetActive(!hideInfo);
//				BlockPanel.SetActive(false);
//				//StrucModPanel.gameObject.SetActive(!hideInfo);

//				if (m_BlockOver != null)
//				{
//					if (!m_BlockOver.IsSelected())
//						m_BlockOver.SetHighlighted(false);
//					m_BlockOver = null;
//				}
//			}

//#pragma warning disable CS8321 // La función local se declara pero nunca se usa
//			void microHeightPrepass()
//#pragma warning restore CS8321 // La función local se declara pero nunca se usa
//			{
//				for(int i = 0; i < m_StrucEdit.GetPilars().Length; ++i)
//				{
//					var pilar = m_StrucEdit.GetPilars()[i];
//					if (pilar == null ||
//						(pilar != null && pilar.GetBlocks().Count == 0) ||
//						(pilar != null && pilar.GetBlocks().Count == 1 && pilar.GetBlocks()[0].GetLayer() == 0))
//						continue;

//					for(int j = 0; j < pilar.GetBlocks().Count; ++j)
//					{
//						var block = pilar.GetBlocks()[j] as CBlockEdit;
//						//if (block.GetStackedBlocks()[0] != null)
//						if(block.IsStackLinkValid(0))
//							continue;

//						var mheight = block.GetMicroHeight();
//						GameUtils.ApplyFnBlockAbove(block, (IBlock b) => { b.SetMicroHeight(mheight); });
//					}
//				}
//			}

//			//void microHeightPostpass()
//			//{
//			//	for (int i = 0; i < m_StrucEdit.GetPilars().Length; ++i)
//			//	{
//			//		var pilar = m_StrucEdit.GetPilars()[i];
//			//		if (pilar == null ||
//			//			(pilar != null && pilar.GetBlocks().Count == 0) ||
//			//			(pilar != null && pilar.GetBlocks().Count == 1 && pilar.GetBlocks()[0].GetLayer() == 0))
//			//			continue;

//			//		for (int j = 0; j < pilar.GetBlocks().Count; ++j)
//			//		{
//			//			var block = pilar.GetBlocks()[j];
//			//			if(block.GetBlockType() == Def.BlockType.WIDE)
//			//			{

//			//			}
//			//		}
//			//	}
//			//}

//			void applyChangesTask()
//			{
//				if (hideInfo)
//				{
//					// Enable invisible blocks
//					SetBlockHidding(false);
//					// Set coherent microheight before anything
//					//microHeightPrepass();
//					// Apply possible void
//					m_StrucEdit.ApplyVoid(hideInfo);
//					// Apply possible stair
//					m_StrucEdit.ApplyStairs(hideInfo);
//					// Convert into WIDE the valid blocks
//					m_StrucEdit.ApplyWides(hideInfo);
//					// Set coherent microheight after doing wides
//					//microHeightPostpass();
//				}
//				else
//				{
//					// Convert into WIDE the valid blocks
//					m_StrucEdit.ApplyWides(hideInfo);
//					// Unapply possible stair
//					m_StrucEdit.ApplyStairs(hideInfo);
//					// Unapply possible void
//					m_StrucEdit.ApplyVoid(hideInfo);
//				}
//			}

//			bool IsStairPossible(CBlockEdit block)
//			{
//				switch (block.GetStairState())
//				{
//					case Def.StairState.POSSIBLE:
//					case Def.StairState.STAIR_OR_RAMP:
//					case Def.StairState.RAMP_POSSIBLE:
//						return true;
//				}
//				return false;
//			}

//			void hideInfoSpritesTask()
//			{
//				//if(BackgroundQueue.Mgr.GetPendingTaskCount() > 0)
//				//{
//				//    BackgroundQueue.Mgr.ScheduleTask(new BackgroundQueue.Task()
//				//    {
//				//        Cost = BackgroundQueue.DefaultTaskCost,
//				//        FN = hideInfoSpritesTask
//				//    });
//				//    return;
//				//}

//				var pilars = m_StrucEdit.GetPilars();
//				for (int i = 0; i < pilars.Length; ++i)
//				{
//					var pilar = pilars[i];
//					if (pilar == null)
//						continue;
//					var blocks = pilar.GetBlocks();
//					for (int j = 0; j < blocks.Count; ++j)
//					{
//						var block = (CBlockEdit)blocks[j];
//						// ToggleVisibility of layer0 blocks and non-layer0 info sprites
//						if (block.GetLayer() != 0)
//						{
//							if (block.IsRemoved())
//								continue;
//							//var blockIE = blocksIE[block.GetIDXIE()];
//							block.GetLayerRnd().enabled = !hideInfo;
//							if (block.IsAnchor())
//								block.GetAnchorRnd().enabled = !hideInfo;
//							if (IsStairPossible(block))
//								block.GetStairRnd().enabled = !hideInfo;
//							if (block.GetVoidState() != Def.BlockVoid.NORMAL)
//								block.GetVoidRnd().enabled = !hideInfo;

//							bool hideLock = hideInfo || block.IsRemoved();

//							if (hideLock)
//							{
//								block.GetLockRnd().enabled = false;
//							}
//							if (!hideLock && block.GetLockState() != Def.LockState.Unlocked)
//							{
//								block.GetLockRnd().enabled = true;
//							}
//						}
//						else // layer 0
//						{
//							block.GetTopMR().enabled = !hideInfo;
//							block.GetMidMR().enabled = !hideInfo;
//						}
//						block.SetSelected(false);

//						// Toggle visibility of monsters, props
//						if (block.GetLayer() == 0 || block.GetBlockType() == Def.BlockType.STAIRS)
//							continue;
//						var prop = block.GetProp();
//						if (prop != null)
//						{
//							prop.enabled = hideInfo;
//							//prop.SpriteSR.enabled = hideInfo;
//							//if (prop.ShadowSR != null)
//							//	prop.ShadowSR.enabled = hideInfo;
//							//if (prop.PropLight != null)
//							//	prop.PropLight.enabled = hideInfo;
//						}
//						var monster = block.GetMonster();
//						if (monster != null)
//						{
//							monster.enabled = hideInfo;
//							//monster.GetSprite().SetEnabled(hideInfo);
//							//monster.GetShadow().SetEnabled(hideInfo);
//							//monster.SpriteSR.enabled = hideInfo;
//							//monster.ShadowSR.enabled = hideInfo;
//						}
//					}
//				}
//				m_Selected.Clear();
//			}

//			hideUITask();
//			applyChangesTask();
//			hideInfoSpritesTask();
//			return false;
//		}
//		bool OnHelpUIStart()
//		{
//			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.Info))
//				return true;

//			HelpUI.gameObject.SetActive(true);
//			enabled = false;
//			return false;
//		}
//		bool OnMenuStart()
//		{
//			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.Menu))
//				return true;

//			OnMenuButton();
//			return false;
//		}
//		bool OnCamModeChange()
//		{
//			if(CameraEditBttn.interactable && KeyMap.StrucEditCheck(Def.StrucEditKeyMap.CamEdit))
//			{
//				OnCamEditButton();
//				return false;
//			}
//			if (CameraGameBttn.interactable && KeyMap.StrucEditCheck(Def.StrucEditKeyMap.CamGame))
//			{
//				OnCamGameButton();
//				return false;
//			}
//			if(CameraFreeBttn.interactable && KeyMap.StrucEditCheck(Def.StrucEditKeyMap.CamFree))
//			{
//				OnCamFreeButton();
//				return false;
//			}

//			return true;
//		}
//		bool OnSelectAll()
//		{
//			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.SelectAll))
//				return true;

//			// TODO if there are selected blocks un select them
//			OnSelectAllButton();
//			return false;
//		}
//		bool OnSelectLayer()
//		{
//			if(KeyMap.StrucEditCheck(Def.StrucEditKeyMap.SelectLayer0))
//			{
//				for(int i = 0; i < m_StrucEdit.GetPilars().Length; ++i)
//				{
//					var pilar = m_StrucEdit.GetPilars()[i];
//					if (pilar == null)
//						continue;
//					if (pilar.GetBlocks().Count != 1)
//						continue;
//					var block = pilar.GetBlocks()[0] as CBlockEdit;
//					if (block.GetLayer() != 0)
//						continue;
//					block.SetSelected(true);
//					if(!m_Selected.Contains(block))
//						m_Selected.Add(block);
//				}
//				return false;
//			}
//			for(int i = Def.MaxLayerSlots - 1; i >= 0; --i)
//			{
//				if(KeyMap.StrucEditCheck(Def.StrucEditKeyMap.SelectLayer1 + i))
//				{
//					// Select layer i + 1
//					var blocks = m_StrucEdit.GetLayerBlocks(i + 1);
//					for(int j = 0; j < blocks.Count; ++j)
//					{
//						var block = blocks[j];
//						if (block == null)
//							continue;

//						block.SetSelected(true);
//						if (!m_Selected.Contains(block))
//							m_Selected.Add(block);
//					}
					
//					return false;
//				}
//			}

//			return true;
//		}
//		void OnStrucSizeMod()
//		{
//			if (m_StrucWidth != m_NextStrucWidth || m_StrucHeight != m_NextStrucHeight)
//			{
//				const int halfSide = Def.MaxStrucSide / 2;
//				int startingXIdx = halfSide - m_NextStrucWidth / 2;
//				int startingYIdx = halfSide - m_NextStrucHeight / 2;
//				int endingXIdx = startingXIdx + m_NextStrucWidth;
//				int endingYIdx = startingYIdx + m_NextStrucHeight;
//				var materialFamily = BlockMaterial.VoidMat[(int)Def.BlockType.NORMAL].Family;

//				for (int y = 0; y < Def.MaxStrucSide; ++y)
//				{
//					var yOffset = y * Def.MaxStrucSide;
//					for (int x = 0; x < Def.MaxStrucSide; ++x)
//					{
//						var pilarID = yOffset + x;
//						var pilar = m_StrucEdit.GetPilars()[pilarID];
//						if (x < startingXIdx || x >= endingXIdx ||
//							y < startingYIdx || y >= endingYIdx)
//						{
//							if (pilar != null)
//							{
//								var pBlocks = pilar.GetBlocks();
//								for(int i = 0; i < pBlocks.Count; ++i)
//								{
//									var block = (CBlockEdit)pBlocks[i];
//									if(m_Selected.Contains(block))
//									{
//										m_Selected.Remove(block);
//									}
//									if (m_BlockOver == block)
//										m_BlockOver = null;
//								}
//								pilar.DestroyPilar(false);
//							}
//						}
//						else if (pilar == null)
//						{
//							pilar = new GameObject(m_StrucEdit.gameObject.name + "_TempPilar").AddComponent<CPilar>();
//							m_StrucEdit.GetPilars()[pilarID] = pilar;
//							pilar.ChangeStruc(m_StrucEdit, pilarID);
//							var block = pilar.AddBlock();
//							block.SetMaterialFamily(materialFamily);
//							block.GetTopMR().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
//							block.GetTopMR().receiveShadows = true;
//							block.GetMidMR().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
//							block.GetMidMR().receiveShadows = true;
//						}
//					}
//				}
//				m_StrucWidth = m_NextStrucWidth;
//				m_StrucHeight = m_NextStrucHeight;
//				var ie = Structures.Strucs[m_StrucEdit.IDXIE];
//				ie.SetWidth(m_StrucWidth);
//				ie.SetHeight(m_StrucHeight);
//				m_NextStrucModification = float.MaxValue;
//			}
//		}
//		bool OnStructureSize()
//		{
//			Vector2Int mod = Vector2Int.zero;
//			if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.IncreaseStrucHeight))
//				mod.Set(0, 1);
//			else if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.DecreaseStrucHeight))
//				mod.Set(0, -1);
//			else if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.IncreaseStrucWidth))
//				mod.Set(1, 0);
//			else if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.DecreaseStrucWidth))
//				mod.Set(-1, 0);
//			else
//				return true;

//			if(mod.x != 0)
//			{
//				if ((m_NextStrucHeight <= Def.MinStrucSide && mod.x < 0) ||
//					(m_NextStrucHeight >= Def.MaxStrucSide && mod.x > 0))
//					return true;

//				StructureSizeXSlider.SetValue(StructureSizeXSlider.Slider.value + mod.x);
//				m_NextStrucHeight += mod.x;
//			}
//			else
//			{
//				if ((m_NextStrucWidth <= Def.MinStrucSide && mod.y < 0) ||
//					(m_NextStrucWidth >= Def.MaxStrucSide && mod.y > 0))
//					return true;

//				StructureSizeYSlider.SetValue(StructureSizeYSlider.Slider.value + mod.y);
//				m_NextStrucWidth += mod.y;
//			}
//			m_NextStrucModification = Time.time;

//			return false;
//		}
//		bool OnReapplyLayers()
//		{
//			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.ReapplyLayers))
//				return true;

//			OnReapplyLayersButton();
//			return false;
//		}
//		bool OnResetLocks()
//		{
//			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.ResetLocks))
//				return true;

//			OnResetLocksButton();
//			return false;
//		}
//		bool OnLayerEdit()
//		{
//			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.LayerEdit))
//				return true;

//			OnLayerEditButton();
//			return false;
//		}
//		bool OnVoidToggle()
//		{
//			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.Void))
//				return true;

//			var strucIE = Structures.Strucs[m_StrucEdit.IDXIE];
//			int done = 0;
//			for(int i = 0; i < m_Selected.Count; ++i)
//			{
//				var cur = m_Selected[i];

//				if (cur.GetLayer() == 0 || cur.GetStairState() != Def.StairState.NONE)
//				{
//					continue;
//				}

//				var val = cur.GetVoidState() + 1;
//				if (val == Def.BlockVoid.COUNT)
//					val = Def.BlockVoid.NORMAL;

//				cur.SetVoidState(val);
//				var blockIE = strucIE.GetBlocks()[cur.GetIDXIE()];
//				blockIE.BlockVoid = val;
//				++done;
//			}
//			if (done > 0)
//			{
//				Structures.SetStrucModified(m_StrucEdit.IDXIE);
//				return false; // Something has been done, stop changing things this frame
//			}
//			return true;
//		}
//		void SetBlockHidding(bool hide)
//		{
//			if (hide)
//			{
//				for (int i = 0; i < m_Selected.Count; ++i)
//				{
//					var cur = m_Selected[i];
//					if (cur == null)
//						continue;
//					m_HiddenBlocks.Add(cur);
//					cur.SetSelected(false);
//					cur.GetCollider().enabled = false;
//					cur.GetTopMR().enabled = false;
//					cur.GetMidMR().enabled = false;
//					cur.GetAnchorRnd().enabled = false;
//					cur.GetLayerRnd().enabled = false;
//					cur.GetLockRnd().enabled = false;
//					cur.GetStairRnd().enabled = false;
//					cur.GetVoidRnd().enabled = false;
//				}
//				m_Selected.Clear();
//			}
//			else
//			{
//				for (int i = 0; i < m_HiddenBlocks.Count; ++i)
//				{
//					var cur = m_HiddenBlocks[i];
//					if (cur == null)
//						continue;
//					cur.GetTopMR().enabled = true;
//					cur.GetMidMR().enabled = true;
//					cur.GetCollider().enabled = true;
//					cur.GetAnchorRnd().enabled = cur.IsAnchor();
//					cur.GetLayerRnd().enabled = cur.GetLayer() > 0;
//					cur.GetLockRnd().enabled = cur.GetLockState() != Def.LockState.Unlocked;
//					cur.GetStairRnd().enabled = GameUtils.IsStairPossible(cur);
//					cur.GetVoidRnd().enabled = cur.GetVoidState() != Def.BlockVoid.NORMAL;
//				}
//				m_HiddenBlocks.Clear();
//			}
//		}
//		bool OnBlockHide()
//		{
//			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.BlockHide))
//				return true;

//			SetBlockHidding(true);
//			return false;
//		}
//		bool OnBlockUnhide()
//		{
//			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.BlockUnhide))
//				return true;

//			SetBlockHidding(false);
//			return false;
//		}
//		void UpdateMovement()
//		{
//			const float MoveAmount = 10f;
//			var movement = Vector2.zero;
//			var camera = CameraManager.Mgr.Camera;
//			if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.CamMoveForward))
//			{
//				movement.Set(movement.x + camera.transform.forward.z, movement.y + camera.transform.forward.x);
//			}
//			if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.CamMoveBackward))
//			{
//				movement.Set(movement.x + -camera.transform.forward.z, movement.y + -camera.transform.forward.x );
//			}
//			if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.CamMoveLeft))
//			{
//				movement.Set(movement.x + -camera.transform.right.z, movement.y + -camera.transform.right.x );
//			}
//			if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.CamMoveRight))
//			{
//				movement.Set(movement.x + camera.transform.right.z, movement.y + camera.transform.right.x);
//			}

//			if (movement != Vector2.zero)
//			{
//				movement.Normalize();
//				movement *= MoveAmount;
//				movement *= Time.deltaTime;
//				var target = CameraManager.Mgr.Target;
//				target.transform.Translate(movement.y, 0f, movement.x, Space.World);
//			}
//		}
//		private void Update()
//		{
//			void handleKeys(Func<bool>[] funcs)
//			{
//				for(int i = 0; i < funcs.Length; ++i)
//				{
//					bool doNext = funcs[i]();
//					if(!doNext)
//					{
//						m_NextModificationTime = Time.time + ModificationDelay;
//						break;
//					}
//				}
//			}
//			CapturedFrametimes[Time.frameCount % MaxFramesCaputred] = Time.smoothDeltaTime;
//			if (m_NextModificationTime < Time.time)
//			{
//				handleKeys(m_KeyFuncs);
//			}
//			if(!Manager.Mgr.HideInfo)
//			{
//				if (m_NextStrucModification < Time.time)
//				{
//					OnStrucSizeMod();
//				}

//				if (m_NextModificationTime < Time.time)
//				{
//					handleKeys(m_EditKeyFuncs);
//				}

//				OnMouseHover();
//				OnMouseRaycast();
//			}

//			// Is not freefly camera used
//			if (CameraFreeBttn.interactable)
//			{
//				UpdateMovement();
//			}

//			if(m_NextFPSTextUpdate < Time.time)
//			{
//				float avgFrametime = CapturedFrametimes.Average();
//				FPSText.text = "FPS: " + (1f / avgFrametime).ToString(".0##");
//				m_NextFPSTextUpdate = Time.time + FPSTextUpdateDelay;
//			}
//		}
//		private void OnEnable()
//		{
//			m_NextStrucModification = Time.time + ModificationDelay;
//			m_NextModificationTime = Time.time + ModificationDelay;
//		}
//		public void LoadStruc(int id, bool copyStruc = false)
//		{
//			m_Selected.Clear();
//			m_HiddenBlocks.Clear();
//			if (m_StrucEdit != null)
//			{
//				m_StrucEdit.DestroyStruc(false);
//				m_StrucEdit = null;
//			}
//			const float side = (Def.MaxStrucSide * (1f + Def.BlockSeparation)) * 0.5f;
//			//float GetSide(float blockCount)
//			//{
//			//	return (blockCount * (1f + Def.BlockSeparation)) * 0.5f;
//			//}

//			m_StrucEdit = CStrucEdit.LoadStruc(id, copyStruc);
//			m_StrucEdit.name = "EditingStruc";
//			var ie = Structures.Strucs[m_StrucEdit.IDXIE];
//			m_NextStrucHeight = ie.GetHeight();
//			m_NextStrucWidth = ie.GetWidth();
//			m_StrucHeight = ie.GetHeight();
//			m_StrucWidth = ie.GetWidth();

//			m_NextStrucModification = float.MaxValue;

//			StructureSizeXSlider.SetValue(m_StrucWidth);
//			StructureSizeYSlider.SetValue(m_StrucHeight);

//			CameraManager.Mgr.Target.transform.position = new Vector3(side, 0f, side);
//			CameraManager.Mgr.CameraType = ECameraType.EDITOR;
//			m_NextMouseHover = 0f;
//			m_BlockOver = null;
//			CameraEditBttn.interactable = false;
//			CameraFreeBttn.interactable = true;
//			CameraGameBttn.interactable = true;

//			ResetLocksBttn.interactable = id >= 0;
//			ReapplyLayersBttn.interactable = id >= 0;
//		}
//		public void ExitUI()
//		{
//			Manager.Mgr.HideInfo = false;
//			EditMenuUI.gameObject.SetActive(false);
//			MainMenu.gameObject.SetActive(true);
//			gameObject.SetActive(false);
//			m_StrucEdit.DestroyStruc(false);
//			m_StrucEdit = null;
//		}
//		#endregion
//	}
//}