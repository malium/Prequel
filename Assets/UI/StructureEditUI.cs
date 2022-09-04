/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UI
{
	public class StructureEditUI : MonoBehaviour
	{
		const float MouseHoverDelay = 0.05f;
		const float ModificationDelay = 0.2f;
		const float StrucSizeDelay = 1f;
		const int MaxCapturedFrames = 20;
		const float FPSTextUpdateDelay = 1f / 3f;

		public MainMenuUI MainMenu;
		public LayerEditUI LayerEditor;
		public StructureEditMenuUI EditMenuUI;
		public StrucEditHelpUI HelpUI;
		public CImageSelectorUI ImageSelectorUI;
		public BiomeEditorUI BiomeEditor;
		public SpawnerEditorUI SpawnerEditor;

		public GameObject LeftPanel;
		public UnityEngine.UI.Button SelectBiomeButton;
		public UnityEngine.UI.Button EditBiomeButton;
		public UnityEngine.UI.Button EditLayersButton;
		public UnityEngine.UI.Button ReapplyLayersButton;
		public UnityEngine.UI.Button SpawnerEditorButton;
		public UnityEngine.UI.Button SelectAllButton;
		public UnityEngine.UI.Button ResetLocksButton;
		public CTMPSlider StrucXSizeSlider;
		public CTMPSlider StrucYSizeSlider;

		public UnityEngine.UI.Button MenuButton;

		public GameObject CameraPanel;
		public UnityEngine.UI.Button CameraEditButton;
		public UnityEngine.UI.Button CameraGameButton;
		public UnityEngine.UI.Button CameraFreeButton;

		public UnityEngine.UI.Button BlockPanelShowButton;
		public CBlockPanel BlockPanel;

		public TMPro.TMP_Text FPSText;
		public GameObject SeedPanel;
		public UnityEngine.UI.Toggle SeedLock;
		public TMPro.TMP_InputField SeedIF;

		bool m_ValueLock;
		int m_Seed;
		CStrucEdit m_Structure;
		World.Biome m_Biome;
		float m_NextStrucSizeTime;
		int m_NextStrucWidth;
		int m_NextStrucHeight;
		List<CBlockEdit> m_Selected;
		List<CBlockEdit> m_HiddenBlocks;
		CBlockEdit m_BlockOver;
		Vector3 m_LastHoverMousePos;
		float m_NextMouseHoverTime;
		float m_NextModificationTime;
		float m_NextFPSTextUpdate;
		Func<bool>[] m_EditKeyFunctions;
		Func<bool>[] m_KeyFunctions;
		List<CImageSelectorUI.ElementInfo> m_BiomeList;

		static float[] CapturedFrametimes;

		private void Awake()
		{
			MenuButton.onClick.AddListener(OnMenuButton);

			CameraEditButton.onClick.AddListener(OnCameraEditButton);
			CameraGameButton.onClick.AddListener(OnCameraGameButton);
			CameraFreeButton.onClick.AddListener(OnCameraFreeButton);

			SelectBiomeButton.onClick.AddListener(OnSelectBiomeButton);
			EditBiomeButton.onClick.AddListener(OnEditBiomeButton);
			EditLayersButton.onClick.AddListener(OnEditLayersButton);
			ReapplyLayersButton.onClick.AddListener(OnReapplyLayersButton);
			SelectAllButton.onClick.AddListener(OnSelectAllButton);
			ResetLocksButton.onClick.AddListener(OnResetLocksButton);
			SpawnerEditorButton.onClick.AddListener(OnSpawnerEditor);

			SeedIF.onEndEdit.AddListener(OnSeedValueChange);

			StrucXSizeSlider.Set(Def.MinStrucSide, Def.MaxStrucSide, 1f, 8f);

			StrucYSizeSlider.Set(Def.MinStrucSide, Def.MaxStrucSide, 1f, 8f);

			BlockPanelShowButton.onClick.AddListener(OnBlockPanelShowButton);

			HelpUI.Init(OnHelpUIEnd);

			m_Selected = new List<CBlockEdit>(Def.MaxStrucSide * Def.MaxStrucSide);
			m_HiddenBlocks = new List<CBlockEdit>(Def.MaxStrucSide * Def.MaxStrucSide);

			m_EditKeyFunctions = new Func<bool>[]
			{
				OnMaterialChange,
				OnStairToggle,
				OnRampToggle,
				OnLockStateToggle,
				OnAnchorToggle,
				OnRotationToggle,
				OnSelectLayer,
				OnLengthChange,
				OnHeightChange,
				OnDestroyBlock,
				OnStacking,
				OnBlockLayerChange,
				OnSelectAll,
				OnMenuStart,
				OnReapplyLayers,
				OnResetLocks,
				OnLayerEdit,
				OnVoidToggle,
				OnBlockUnhide,
				OnBlockHide,
			};

			m_KeyFunctions = new Func<bool>[]
			{
				() => { return ToggleVisibility(); },
				OnCamModeChange,
				OnHelpUIStart,
			};

			CapturedFrametimes = new float[MaxCapturedFrames];
		}
		private void OnEnable()
		{
			m_NextModificationTime = Time.time + ModificationDelay;
			m_NextStrucSizeTime = Time.time + ModificationDelay;
		}
		private void Update()
		{
			// Update fps
			CapturedFrametimes[Time.frameCount % MaxCapturedFrames] = Time.smoothDeltaTime;

			// Handle function keys
			if(m_NextModificationTime < Time.time)
			{
				HandleKeys(m_KeyFunctions);
			}

			// Are we editing?
			if(!Manager.Mgr.HideInfo)
			{
				// Check if structure size have changed and apply those changes
				if(m_NextStrucSizeTime < Time.time)
				{
					OnStrucSize();
				}

				// Handle editing keys
				if(m_NextModificationTime < Time.time)
				{
					HandleKeys(m_EditKeyFunctions);
				}

				OnMouseHover();
				OnMouseRaycast();
			}

			// Is not freefly camera used -> handle WASD movement
			if (CameraFreeButton.interactable)
			{
				UpdateMovement();
			}

			// Update the FPS text
			if(m_NextFPSTextUpdate < Time.time)
			{
				float avgFrametime = CapturedFrametimes.Average();
				FPSText.text = "FPS: " + (1f / avgFrametime).ToString(".0##");
				m_NextFPSTextUpdate = Time.time + FPSTextUpdateDelay;
			}
		}
		public CStrucEdit GetStruc() => m_Structure;
		public World.Biome GetBiome() => m_Biome;
		void UpdateBiomeList()
		{
			m_BiomeList.Clear();

			for (int i = 0; i < BiomeLoader.Dict.Count; ++i)
			{
				var pair = BiomeLoader.Dict.ElementAt(i);
				
				if (pair.Value >= BiomeLoader.Biomes.Count || pair.Value < 0 || (m_Biome != null && m_Biome.IDXIE == pair.Value))
					continue;

				m_BiomeList.Add(new CImageSelectorUI.ElementInfo()
				{
					Name = pair.Key
				});
			}
		}
		void LoadBiomeList()
		{
			if (m_BiomeList == null)
				m_BiomeList = new List<CImageSelectorUI.ElementInfo>(BiomeLoader.Dict.Count + 1);

			UpdateBiomeList();
		}
		public void Init()
		{
			var manager = Manager.Mgr;
			manager.HideInfo = false;
			manager.DebugAI = false;
			manager.DebugMovement = false;
			manager.DebugSpells = false;
			manager.DebugStats = false;

			EditMenuUI.gameObject.SetActive(false);
			BlockPanelShowButton.gameObject.SetActive(true);

			m_Selected.Clear();
			m_HiddenBlocks.Clear();
			BlockPanel.gameObject.SetActive(true);
			BlockPanel.Init(() =>
			{
				BlockPanel.gameObject.SetActive(false);
				BlockPanelShowButton.gameObject.SetActive(true);
			});
			BlockPanel.gameObject.SetActive(false);
			StrucXSizeSlider.SetCallback(OnStrucXSizeSlider);
			StrucYSizeSlider.SetCallback(OnStrucYSizeSlider);

			LoadBiomeList();
			LoadStruc(-1);
		}
		public void LoadStruc(int id, bool copyStruc = false)
		{
			// Clear previous data
			m_BlockOver = null;
			m_Selected.Clear();
			m_HiddenBlocks.Clear();
			m_Biome = null;
			EditBiomeButton.interactable = false;
			
			if(m_Structure != null)
			{
				m_Structure.DestroyStruc(false);
				m_Structure = null;
			}

			const float side = (Def.MaxStrucSide * (1f + Def.BlockSeparation)) * 0.5f;

			m_Structure = CStrucEdit.LoadStruc(id, copyStruc);
			m_Structure.name = "EditingStruc";

			var ie = Structures.Strucs[m_Structure.IDXIE];
			m_NextStrucHeight = ie.GetHeight();
			m_NextStrucWidth = ie.GetWidth();

			m_NextStrucSizeTime = float.MaxValue;

			StrucXSizeSlider.SetValue(m_NextStrucWidth);
			StrucYSizeSlider.SetValue(m_NextStrucHeight);

			var camMgr = CameraManager.Mgr;
			camMgr.Target.transform.position = new Vector3(side, 0f, side);
			camMgr.CameraType = ECameraType.EDITOR;

			m_NextMouseHoverTime = 0f;
			CameraEditButton.interactable = false;
			CameraFreeButton.interactable = true;
			CameraGameButton.interactable = true;

			ResetLocksButton.interactable = id >= 0;
			ReapplyLayersButton.interactable = id >= 0;
		}
		void HandleKeys(Func<bool>[] functions)
		{
			for(int i = 0; i < functions.Length; ++i)
			{
				var doNext = functions[i]();
				if(!doNext)
				{
					m_NextModificationTime = Time.time + ModificationDelay;
					break;
				}
			}
		}
		void OnMouseHover()
		{
			if (m_BlockOver != null)
				m_BlockOver.SetHighlighted(true);

			if (m_NextMouseHoverTime > Time.time || m_LastHoverMousePos == Input.mousePosition
				|| UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
				return;

			m_LastHoverMousePos = Input.mousePosition;
			m_NextMouseHoverTime = Time.time + MouseHoverDelay;

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			bool rayHit = Physics.Raycast(ray, out RaycastHit mouseHit, 10000f, 1 << Def.RCLayerBlock);
			if (rayHit)
			{
				var block = mouseHit.transform.gameObject.GetComponent<CBlockEdit>();
				if (m_BlockOver != null && m_BlockOver != block)
					m_BlockOver.SetHighlighted(false);
				m_BlockOver = block;
				if(BlockPanel.isActiveAndEnabled)
					BlockPanel.SetBlock(block, m_Structure.IDXIE);
			}
			else
			{
				if (m_BlockOver != null)
					m_BlockOver.SetHighlighted(false);
				m_BlockOver = null;
			}
		}
		void OnMouseRaycast()
		{
			bool shiftPressed = Input.GetKey(KeyCode.LeftShift);
			bool mouseLeftHold = Input.GetMouseButton(0);
			bool mouseRightHold = Input.GetMouseButton(1);

			bool mouseLeftClick = Input.GetMouseButtonDown(0);
			bool mouseRightClick = Input.GetMouseButtonDown(1);
			bool mouseClicked = mouseLeftClick || mouseRightClick;
			bool mouseHold = mouseLeftHold || mouseRightHold;

			if ((!mouseClicked && !mouseHold)
				|| UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
				return;

			bool select = (mouseLeftHold || mouseLeftClick) && (!mouseRightHold || !mouseRightClick);
			bool unselect = (!mouseLeftHold || !mouseLeftClick) && (mouseRightHold || mouseRightClick);

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			bool rayHit = Physics.Raycast(ray, out RaycastHit mouseHit, 10000f, 1 << Def.RCLayerBlock);
			if (!rayHit)
			{
				// Unselect all when clicking outside
				//if (mouseClicked && !shiftPressed)
				//{
				//	for (int i = 0; i < m_Selected.Count; ++i)
				//	{
				//		var selBlock = m_Selected[i];
				//		if (selBlock == null)
				//			continue;
				//		selBlock.SetSelected(false);
				//	}
				//	m_Selected.Clear();
				//}
				return;
			}
			var block = mouseHit.transform.gameObject.GetComponent<CBlockEdit>();
			if (m_BlockOver != null && m_BlockOver != block)
				m_BlockOver.SetHighlighted(false);
			m_BlockOver = block;

			if ((shiftPressed && mouseHold) || (!shiftPressed && mouseClicked)) // Drag edit || clicky edit
			{
				if (select && !m_Selected.Contains(m_BlockOver))
				{
					m_BlockOver.SetSelected(true);
					m_Selected.Add(m_BlockOver);
				}
				else if (unselect && CameraFreeButton.interactable)
				{
					m_BlockOver.SetSelected(false);
					if (m_Selected.Contains(m_BlockOver))
					{
						m_Selected.Remove(m_BlockOver);
					}
				}
			}
		}
		void OnStrucSize()
		{
			var strucIE = Structures.Strucs[m_Structure.IDXIE];
			var strucWidth = strucIE.GetWidth();
			var strucHeight = strucIE.GetHeight();
			if (strucWidth == m_NextStrucWidth && strucHeight == m_NextStrucHeight)
				return; // Everything is the same

			const int halfSide = Def.MaxStrucSide / 2;
			int startingXIdx = halfSide - m_NextStrucWidth / 2;
			int startingYIdx = halfSide - m_NextStrucHeight / 2;
			int endingXIdx = startingXIdx + m_NextStrucWidth;
			int endingYIdx = startingYIdx + m_NextStrucHeight;
			var voidMatFamily = BlockMaterial.VoidMat[(int)Def.BlockType.NORMAL].Family;

			for (int y = 0; y < Def.MaxStrucSide; ++y)
			{
				var yOffset = y * Def.MaxStrucSide;
				for (int x = 0; x < Def.MaxStrucSide; ++x)
				{
					var pilarID = yOffset + x;
					var pilar = m_Structure.GetPilars()[pilarID];
					if (x < startingXIdx || x >= endingXIdx ||
						y < startingYIdx || y >= endingYIdx)
					{
						if (pilar != null)
						{
							var pBlocks = pilar.GetBlocks();
							for (int i = 0; i < pBlocks.Count; ++i)
							{
								var block = (CBlockEdit)pBlocks[i];
								if (m_Selected.Contains(block))
								{
									m_Selected.Remove(block);
								}
								if (m_BlockOver == block)
									m_BlockOver = null;
							}
							pilar.DestroyPilar(false);
						}
					}
					else if (pilar == null)
					{
						pilar = new GameObject(m_Structure.gameObject.name + "_TempPilar").AddComponent<CPilar>();
						m_Structure.GetPilars()[pilarID] = pilar;
						pilar.ChangeStruc(m_Structure, pilarID);
						var block = pilar.AddBlock();
						block.SetMaterialFamily(voidMatFamily);
						block.GetTopMR().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
						block.GetTopMR().receiveShadows = true;
						block.GetMidMR().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
						block.GetMidMR().receiveShadows = true;
					}
				}
			}
			var ie = Structures.Strucs[m_Structure.IDXIE];
			ie.SetWidth(m_NextStrucWidth);
			ie.SetHeight(m_NextStrucHeight);
			m_NextStrucSizeTime = float.MaxValue;
		}
		void UpdateMovement()
		{
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
			}
		}
		void SetBlockHidding(bool hide)
		{
			if (hide)
			{
				for (int i = 0; i < m_Selected.Count; ++i)
				{
					var cur = m_Selected[i];
					if (cur == null)
						continue;
					m_HiddenBlocks.Add(cur);
					cur.SetSelected(false);
					cur.GetCollider().enabled = false;
					cur.GetTopMR().enabled = false;
					cur.GetMidMR().enabled = false;
					cur.GetAnchorRnd().enabled = false;
					cur.GetLayerRnd().enabled = false;
					cur.GetLockRnd().enabled = false;
					cur.GetStairRnd().enabled = false;
					cur.GetVoidRnd().enabled = false;
				}
				m_Selected.Clear();
			}
			else
			{
				for (int i = 0; i < m_HiddenBlocks.Count; ++i)
				{
					var cur = m_HiddenBlocks[i];
					if (cur == null)
						continue;
					cur.GetTopMR().enabled = true;
					cur.GetMidMR().enabled = true;
					cur.GetCollider().enabled = true;
					cur.GetAnchorRnd().enabled = cur.IsAnchor();
					cur.GetLayerRnd().enabled = cur.GetLayer() > 0;
					cur.GetLockRnd().enabled = cur.GetLockState() != Def.LockState.Unlocked;
					cur.GetStairRnd().enabled = GameUtils.IsStairPossible(cur.GetStairState());
					cur.GetVoidRnd().enabled = cur.GetVoidState() != Def.BlockVoid.NORMAL;
				}
				m_HiddenBlocks.Clear();
			}
		}
		public void ExitUI()
		{
			Manager.Mgr.HideInfo = false;
			EditMenuUI.gameObject.SetActive(false);
			MainMenu.gameObject.SetActive(true);
			gameObject.SetActive(false);
			m_Structure.DestroyStruc(false);
			m_Structure = null;
			m_Biome = null;
			m_BlockOver = null;
		}
		#region EditFunctions
		bool OnMaterialChange()
		{
			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.MaterialCycle))
				return true; // Nothing has been done

			int done = 0;
			var strucIE = Structures.Strucs[m_Structure.IDXIE];
			List<IDChance> availableMaterials = new List<IDChance>();
			for (int i = 0; i < m_Selected.Count; ++i)
			{
				var cur = m_Selected[i];
				if (cur.GetLayer() == 0)
					continue;

				var layer = m_Structure.GetLayers()[cur.GetLayer() - 1];
				if (!layer.IsValid())
					continue;

				if (layer.IsLinkedLayer)
				{
					int selectedLayer = layer.LinkedLayers[LayerInfo.RandomFromList(layer.LinkedLayers)].ID;
					layer = m_Structure.GetLayers()[selectedLayer - 1];
				}
				availableMaterials.Clear();

				if(layer.LayerType == Def.BiomeLayerType.OTHER)
				{
					if (availableMaterials.Capacity < layer.MaterialFamilies.Count)
						availableMaterials.Capacity = layer.MaterialFamilies.Count;
					for (int j = 0; j < layer.MaterialFamilies.Count; ++j)
					{
						var matFamily = BlockMaterial.MaterialFamilies[layer.MaterialFamilies[j].ID];
						if (matFamily.GetSet(cur.GetBlockType()).Length > 0)
						{
							availableMaterials.Add(layer.MaterialFamilies[j]);
						}
					}
				}
				else if(layer.LayerType == Def.BiomeLayerType.FULLVOID || m_Biome == null)
				{
					// No-op
				}
				else
				{
					var bLayer = m_Biome.GetLayers()[(int)layer.LayerType];
					if (availableMaterials.Capacity < bLayer.MaterialFamilies.Count)
						availableMaterials.Capacity = bLayer.MaterialFamilies.Count;
					for (int j = 0; j < bLayer.MaterialFamilies.Count; ++j)
					{
						var matFamily = BlockMaterial.MaterialFamilies[bLayer.MaterialFamilies[j].ID];
						if (matFamily.GetSet(cur.GetBlockType()).Length > 0)
						{
							availableMaterials.Add(bLayer.MaterialFamilies[j]);
						}
					}
				}
				if (availableMaterials.Count == 0)
					continue; // not enough materials

				GameUtils.UpdateChances2(ref availableMaterials);
				int curMaterialID = availableMaterials[LayerInfo.RandomFromList(availableMaterials)].ID;
				var curMaterial = BlockMaterial.MaterialFamilies[curMaterialID];
				cur.SetMaterialFamily(curMaterial);

				if (cur.GetLockState() != Def.LockState.Locked)
					cur.SetLockState(Def.LockState.SemiLocked);
				var blockIE = strucIE.GetBlocks()[cur.GetIDXIE()];
				blockIE.MaterialFamily = curMaterial.FamilyInfo.FamilyName;
				blockIE.SetFlag(IE.V4.BlockIE.Flag.MaterialFamily, true);
				++done;
			}
			if (done > 0)
			{
				Structures.SetStrucModified(m_Structure.IDXIE);
				return false; // Something has been done, stop changing things this frame
			}
			return false;
		}
		bool OnStairToggle()
		{
			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.Stair))
				return true; // Nothing has been done

			var strucIE = Structures.Strucs[m_Structure.IDXIE];
			bool shift = Input.GetKey(KeyCode.LeftShift);
			int done = 0;
			for (int i = 0; i < m_Selected.Count; ++i)
			{
				var cur = m_Selected[i];
				if (cur.GetLayer() == 0 || cur.GetBlockType() == Def.BlockType.WIDE)
					continue;

				var blockIE = strucIE.GetBlocks()[cur.GetIDXIE()];

				if (shift)
				{
					if (cur.GetStairState() == Def.StairState.ALWAYS || cur.GetStairState() == Def.StairState.RAMP_ALWAYS)
					{
						//var check = cur;
						GameUtils.ApplyFnBlockAbove(cur, (IBlock b) => b.SetHeight(b.GetHeight() - 0.5f));
						//while (check.GetStackedBlocks()[1] != null)
						//{
						//	check = (CBlockEdit)check.GetStackedBlocks()[1];
						//	check.SetHeight(check.GetHeight() - 0.5f);
						//}
					}
					cur.SetStairState(Def.StairState.STAIR_OR_RAMP);

					if (cur.GetLockState() != Def.LockState.Locked)
						cur.SetLockState(Def.LockState.SemiLocked);

					blockIE.Stair = cur.GetStairState();
					blockIE.BlockType = cur.GetBlockType();
					blockIE.Rotation = cur.GetRotation();
					blockIE.Flags[(int)IE.V3.BlockIE.Flag.Rotation] = true;
					++done;
					continue;
				}

				var stair = cur.GetStairState() + 1;
				if (stair > Def.StairState.ALWAYS)
				{
					//var check = cur;
					GameUtils.ApplyFnBlockAbove(cur, (IBlock b) => b.SetHeight(b.GetHeight() - 0.5f));
					//while (check.GetStackedBlocks()[1] != null)
					//{
					//	check = (CBlockEdit)check.GetStackedBlocks()[1];
					//	check.SetHeight(check.GetHeight() - 0.5f);
					//}
					stair = Def.StairState.NONE;
				}

				if (stair == Def.StairState.ALWAYS)
				{
					//var check = cur;
					GameUtils.ApplyFnBlockAbove(cur, (IBlock b) => b.SetHeight(b.GetHeight() + 0.5f));
					//while (check.GetStackedBlocks()[1] != null)
					//{
					//	check = (CBlockEdit)check.GetStackedBlocks()[1];
					//	check.SetHeight(check.GetHeight() + 0.5f);
					//}
				}

				cur.SetStairState(stair);

				if (cur.GetLockState() != Def.LockState.Locked)
					cur.SetLockState(Def.LockState.SemiLocked);

				blockIE.Stair = cur.GetStairState();
				blockIE.BlockType = cur.GetBlockType();
				blockIE.Rotation = cur.GetRotation();
				blockIE.Flags[(int)IE.V3.BlockIE.Flag.Rotation] = true;
				++done;
			}
			if (done > 0)
			{
				Structures.SetStrucModified(m_Structure.IDXIE);
				return false; // Something has been done, stop changing things this frame
			}
			return true;
		}
		bool OnRampToggle()
		{
			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.Ramp))
				return true; // Nothing has been done

			bool shift = Input.GetKey(KeyCode.LeftShift);

			var strucIE = Structures.Strucs[m_Structure.IDXIE];
			int done = 0;
			for (int i = 0; i < m_Selected.Count; ++i)
			{
				var cur = m_Selected[i];
				if (cur.GetLayer() == 0 || cur.GetBlockType() == Def.BlockType.WIDE)
					continue;

				var blockIE = strucIE.GetBlocks()[cur.GetIDXIE()];

				if (shift)
				{
					if (cur.GetStairState() == Def.StairState.ALWAYS || cur.GetStairState() == Def.StairState.RAMP_ALWAYS)
					{
						//var check = cur;
						GameUtils.ApplyFnBlockAbove(cur, (IBlock b) => b.SetHeight(b.GetHeight() - 0.5f));
						//while (check.GetStackedBlocks()[1] != null)
						//{
						//	check = (CBlockEdit)check.GetStackedBlocks()[1];
						//	check.SetHeight(check.GetHeight() - 0.5f);
						//}
					}
					cur.SetStairState(Def.StairState.STAIR_OR_RAMP);

					if (cur.GetLockState() != Def.LockState.Locked)
						cur.SetLockState(Def.LockState.SemiLocked);

					blockIE.Stair = cur.GetStairState();
					blockIE.BlockType = cur.GetBlockType();
					blockIE.Rotation = cur.GetRotation();
					blockIE.Flags[(int)IE.V3.BlockIE.Flag.Rotation] = true;
					++done;
					continue;
				}

				var stair = cur.GetStairState();
				switch (stair)
				{

					case Def.StairState.NONE:
					case Def.StairState.POSSIBLE:
					case Def.StairState.ALWAYS:
					case Def.StairState.STAIR_OR_RAMP:
						{
							if (stair == Def.StairState.ALWAYS)
							{
								//var check = cur;
								GameUtils.ApplyFnBlockAbove(cur, (IBlock b) => b.SetHeight(b.GetHeight() - 0.5f));
								//while (check.GetStackedBlocks()[1] != null)
								//{
								//	check = (CBlockEdit)check.GetStackedBlocks()[1];
								//	check.SetHeight(check.GetHeight() - 0.5f);
								//}
							}
							cur.SetStairState(Def.StairState.RAMP_POSSIBLE);
						}
						break;
					case Def.StairState.RAMP_POSSIBLE:
						{
							//var check = cur;
							GameUtils.ApplyFnBlockAbove(cur, (IBlock b) => b.SetHeight(b.GetHeight() + 0.5f));
							//while (check.GetStackedBlocks()[1] != null)
							//{
							//	check = (CBlockEdit)check.GetStackedBlocks()[1];
							//	check.SetHeight(check.GetHeight() + 0.5f);
							//}
							cur.SetStairState(Def.StairState.RAMP_ALWAYS);
						}
						break;
					case Def.StairState.RAMP_ALWAYS:
						{
							//var check = cur;
							GameUtils.ApplyFnBlockAbove(cur, (IBlock b) => b.SetHeight(b.GetHeight() - 0.5f));
							//while (check.GetStackedBlocks()[1] != null)
							//{
							//	check = (CBlockEdit)check.GetStackedBlocks()[1];
							//	check.SetHeight(check.GetHeight() - 0.5f);
							//}
							cur.SetStairState(Def.StairState.NONE);
						}
						break;
				}

				if (cur.GetLockState() != Def.LockState.Locked)
					cur.SetLockState(Def.LockState.SemiLocked);

				blockIE.Stair = cur.GetStairState();
				blockIE.BlockType = cur.GetBlockType();
				blockIE.Rotation = cur.GetRotation();
				blockIE.Flags[(int)IE.V3.BlockIE.Flag.Rotation] = true;
				++done;
			}
			if (done > 0)
			{
				Structures.SetStrucModified(m_Structure.IDXIE);
				return false; // Something has been done, stop changing things this frame
			}
			return true;
		}
		// Locks the block properties and if they are locked it unlocks them
		bool OnLockStateToggle()
		{
			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.Lock))
				return true;  // Nothing has been done

			var strucIE = Structures.Strucs[m_Structure.IDXIE];
			int done = 0;
			for (int i = 0; i < m_Selected.Count; ++i)
			{
				var cur = m_Selected[i];
				if (cur.GetLayer() == 0)
					continue;

				var blockIE = strucIE.GetBlocks()[cur.GetIDXIE()];
				var layer = m_Structure.GetLayers()[cur.GetLayer() - 1];
				if (layer.IsLinkedLayer)
				{
					layer = m_Structure.GetLayers()[cur.GetLinkedTo() - 1];
				}

				// Lock the block properties
				if (cur.GetLockState() != Def.LockState.Locked)
				{
					blockIE.Rotation = cur.GetRotation();
					blockIE.SetFlag(IE.V4.BlockIE.Flag.Rotation, true);
					blockIE.BlockType = cur.GetBlockType();
					blockIE.SetHeight(cur.GetHeight());
					blockIE.SetFlag(IE.V4.BlockIE.Flag.Length, true);
					blockIE.SetLength(cur.GetLength());
					if (cur.GetMaterialFamily() != null)
					{
						blockIE.MaterialFamily = cur.GetMaterialFamily().FamilyInfo.FamilyName;
						blockIE.SetFlag(IE.V4.BlockIE.Flag.MaterialFamily, true);
					}
					else
					{
						blockIE.MaterialFamily = "";
						blockIE.SetFlag(IE.V4.BlockIE.Flag.MaterialFamily, false);
					}
					blockIE.SetFlag(IE.V4.BlockIE.Flag.Prop, true);
					blockIE.PropFamily = cur.GetProp() != null ? cur.GetProp().GetInfo().FamilyName : "";
					blockIE.BlockVoid = cur.GetVoidState();
					blockIE.Stair = cur.GetStairState();
					cur.SetLockState(Def.LockState.Locked);
				}
				// Unlock the block properties
				else
				{
					blockIE.SetDefault();
					blockIE.SetFlag(IE.V4.BlockIE.Flag.Anchor, cur.IsAnchor());
					blockIE.Layer = (byte)cur.GetLayer();
					blockIE.Stair = cur.GetStairState();
					if (cur.GetStairState() != Def.StairState.NONE)
					{
						blockIE.Rotation = cur.GetRotation();
						blockIE.SetFlag(IE.V4.BlockIE.Flag.Rotation, true);
					}
					blockIE.BlockType = cur.GetBlockType();
					blockIE.SetHeight(cur.GetHeight());
					//if (cur.GetHeight() != layer.BlockHeight && !cur.IsStackLinkValid(0)/*cur.GetStackedBlocks()[0] == null*/)
					//{
					//	blockIE.Height = cur.GetHeight();
					//	blockIE.SetFlag(IE.V3.BlockIE.Flag.Height, true);
					//}
					if (Mathf.Clamp(cur.GetLength(), layer.BlockLengthMin, layer.BlockLengthMax) != cur.GetLength())
					{
						blockIE.SetLength(cur.GetLength());
						blockIE.SetFlag(IE.V4.BlockIE.Flag.Length, true);
					}

					if (blockIE.Flags.Contains(true))
					{
						cur.SetLockState(Def.LockState.SemiLocked);
					}
					else
					{
						cur.SetLockState(Def.LockState.Unlocked);
					}
				}
				++done;
			}
			if (done > 0)
			{
				Structures.SetStrucModified(m_Structure.IDXIE);
				return false; // Something has been done, stop changing things this frame
			}
			return true;
		}
		bool OnAnchorToggle()
		{
			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.Anchor))
				return true;  // Nothing has been done

			// TODO
			return false;
		}
		bool OnRotationToggle()
		{
			int rotation;
			if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.RotateLeft))
				rotation = -1;
			else if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.RotateRight))
				rotation = +1;
			else
				return true;  // Nothing has been done

			var strucIE = Structures.Strucs[m_Structure.IDXIE];
			int done = 0;
			for (int i = 0; i < m_Selected.Count; ++i)
			{
				var cur = m_Selected[i];
				if (cur.GetLayer() == 0)
					continue;

				var rot = cur.GetRotation();
				if (rotation < 0 && rot == Def.RotationState.Default)
					rot = Def.RotationState.Left;
				else if (rotation > 0 && rot == Def.RotationState.Left)
					rot = Def.RotationState.Default;
				else
					rot += rotation;

				cur.SetRotation(rot);
				if (cur.GetLockState() != Def.LockState.Locked)
					cur.SetLockState(Def.LockState.SemiLocked);

				var blockIE = strucIE.GetBlocks()[cur.GetIDXIE()];
				blockIE.Rotation = rot;
				blockIE.SetFlag(IE.V4.BlockIE.Flag.Rotation, true);
				++done;
			}
			if (done > 0)
			{
				Structures.SetStrucModified(m_Structure.IDXIE);
				return false; // Something has been done, stop changing things this frame
			}
			return true;
		}
		bool OnSelectLayer()
		{
			if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.SelectLayer0))
			{
				for (int i = 0; i < m_Structure.GetPilars().Length; ++i)
				{
					var pilar = m_Structure.GetPilars()[i];
					if (pilar == null)
						continue;
					if (pilar.GetBlocks().Count != 1)
						continue;
					var block = pilar.GetBlocks()[0] as CBlockEdit;
					if (block.GetLayer() != 0)
						continue;
					block.SetSelected(true);
					if (!m_Selected.Contains(block))
						m_Selected.Add(block);
				}
				return false;
			}

			for (int i = Def.MaxLayerSlots - 1; i >= 0; --i)
			{
				if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.SelectLayer1 + i))
				{
					// Select layer i + 1
					var blocks = m_Structure.GetLayerBlocks(i + 1);
					for (int j = 0; j < blocks.Count; ++j)
					{
						var block = blocks[j];
						if (block == null)
							continue;

						block.SetSelected(true);
						if (!m_Selected.Contains(block))
							m_Selected.Add(block);
					}

					return false;
				}
			}

			return true;
		}
		bool OnLengthChange()
		{
			Action<CBlockEdit> fn;
			bool shift = Input.GetKey(KeyCode.LeftShift);
			if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.LengthDown))
				fn = (CBlockEdit block) => block.DecreaseLength(shift, true);
			else if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.LengthUp))
				fn = (CBlockEdit block) => block.IncreaseLength(shift, true);
			else
				return true;  // Nothing has been done

			int done = 0;
			for (int i = 0; i < m_Selected.Count; ++i)
			{
				var cur = m_Selected[i];

				if (cur.GetLayer() == 0)
					continue;

				fn(cur);

				++done;
			}
			if (done > 0)
			{
				Structures.SetStrucModified(m_Structure.IDXIE);
				return false; // Something has been done, stop changing things this frame
			}
			return true;
		}
		bool OnHeightChange()
		{
			Action<CBlockEdit> fn;

			bool shift = Input.GetKey(KeyCode.LeftShift);
			var strucIE = Structures.Strucs[m_Structure.IDXIE];

			void DeStack(CBlockEdit block, int stackIdx)
			{
				if(block.GetStackedBlocksIdx()[stackIdx] >= 0)
				{
					var sBlock = block.GetPilar().GetBlocks()[block.GetStackedBlocksIdx()[stackIdx]] as CBlockEdit;
					block.GetStackedBlocksIdx()[stackIdx] = -1;
					var blockIE = strucIE.GetBlocks()[block.GetIDXIE()];
					blockIE.StackedIdx[stackIdx] = -1;
					var sStackIdx = stackIdx == 0 ? 1 : 0;
					sBlock.GetStackedBlocksIdx()[sStackIdx] = -1;
					var sBlockIE = strucIE.GetBlocks()[sBlock.GetIDXIE()];
					sBlockIE.StackedIdx[sStackIdx] = -1;
				}
			}

			if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.HeightDown))
			{
				if(!shift)
					fn = (CBlockEdit block) => block.DecreaseHeight(false, false, true);
				else
					fn = (CBlockEdit block) => { DeStack(block, 1); block.DecreaseHeight(false, false, true); };
			}
			else if (KeyMap.StrucEditCheck(Def.StrucEditKeyMap.HeightUp))
			{
				if(!shift)
					fn = (CBlockEdit block) => block.IncreaseHeight(false, false, true);
				else
					fn = (CBlockEdit block) => { DeStack(block, 0); block.IncreaseHeight(false, false, true); };
			}
			else
			{
				return true;  // Nothing has been done
			}

			int done = 0;
			for (int i = 0; i < m_Selected.Count; ++i)
			{
				var cur = m_Selected[i];
				if (cur.GetLayer() == 0)
					continue;

				fn(cur);

				++done;
			}
			if (done > 0)
			{
				Structures.SetStrucModified(m_Structure.IDXIE);
				return false; // Something has been done, stop changing things this frame
			}
			return true;
		}
		bool OnDestroyBlock()
		{
			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.DestroyBlock))
				return true;  // Nothing has been done
			var strucIE = Structures.Strucs[m_Structure.IDXIE];
			int done = 0;
			void FixPilar(int removeIdx, CPilar pilar)
			{
				for(int i = 0; i < pilar.GetBlocks().Count; i++)
				{
					if (i == removeIdx)
						continue;

					var block = pilar.GetBlocks()[i] as CBlockEdit;
					var blockIE = strucIE.GetBlocks()[block.GetIDXIE()];

					if(i < removeIdx)
					{
						if(block.GetStackedBlocksIdx()[1] == removeIdx)
						{
							block.GetStackedBlocksIdx()[1] = -1;
							blockIE.StackedIdx[1] = -1;
						}
					}
					else
					{
						block._SetPilarIndex(i - 1);

						if(block.GetStackedBlocksIdx()[0] == removeIdx)
						{
							block.GetStackedBlocksIdx()[0] = -1;
							blockIE.StackedIdx[0] = -1;
						}
						else if(block.GetStackedBlocksIdx()[0] >= 0)
						{
							block.GetStackedBlocksIdx()[0] = block.GetStackedBlocksIdx()[0] - 1;
						}
						if(block.GetStackedBlocksIdx()[1] >= 0)
						{
							block.GetStackedBlocksIdx()[1] = block.GetStackedBlocksIdx()[1] - 1;
						}
					}
				}
			}
			for (int i = 0; i < m_Selected.Count; ++i)
			{
				var cur = m_Selected[i];

				if (cur.GetLayer() == 0)
				{
					cur.SetSelected(false);
					continue;
				}

				FixPilar(cur.GetPilarIndex(), cur.GetPilar());
				strucIE.RemoveBlock(cur.GetIDXIE());
				//var curIE = strucIE.GetBlocks()[cur.GetIDXIE()];
				//if(curIE.StackedIdx[0] >= 0)
				//{
				//	var stackIE = strucIE.GetBlocks()[curIE.StackedIdx[0]];
				//	stackIE.StackedIdx[1] = -1;
				//}
				//if(curIE.StackedIdx[1] >= 0)
				//{
				//	var stackIE = strucIE.GetBlocks()[curIE.StackedIdx[1]];
				//	stackIE.StackedIdx[0] = -1;
				//}
				//strucIE.RemoveBlock(cur.GetIDXIE());
				//var above = cur.GetStackedAbove() as CBlockEdit;
				//var below = cur.GetStackedBelow() as CBlockEdit;
				//if (above != null)
				//{
				//	above.GetStackedBlocksIdx()[0] = -1;
				//	var aboveIE = strucIE.GetBlocks()[above.GetIDXIE()];
				//	aboveIE.StackedIdx[0] = -1;
				//}
				//if (below != null)
				//{
				//	below.GetStackedBlocksIdx()[1] = -1;
				//	var belowIE = strucIE.GetBlocks()[below.GetIDXIE()];
				//	belowIE.StackedIdx[0] = -1;
				//}

				//cur.GetStackedBlocksIdx()[0] = -1;
				//cur.GetStackedBlocksIdx()[1] = -1;
				//cur.SetIDXIE(-1);
				m_Selected.Remove(cur);
				--i;
				if (cur.GetPilar().GetBlocks().Count > 1)
				{
					cur.DestroyBlock(false);
				}
				else
				{
					cur.SetLayer(0);
					cur.SetSelected(false);
					cur.SetIDXIE(-1);
				}

				++done;
			}
			m_Selected.Clear();
			if (done > 0)
			{
				Structures.SetStrucModified(m_Structure.IDXIE);
				return false; // Something has been done, stop changing things this frame
			}
			return true;
		}
		bool OnStacking()
		{
			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.StackBlock))
				return true;  // Nothing has been done

			var strucIE = Structures.Strucs[m_Structure.IDXIE];
			int done = 0;
			var nSelected = new List<CBlockEdit>(m_Selected.Count);
			bool shift = Input.GetKey(KeyCode.LeftShift);

			bool AreBlocksAbove(CPilar pilar, float height)
			{
				for (int i = 0; i < pilar.GetBlocks().Count; ++i)
				{
					var b = pilar.GetBlocks()[i];
					if (b.GetHeight() > height)
						return true;
				}

				return false;
			}
			void SortBlocks(CPilar pilar)
			{
				// First sort the pilar block list
				var nList = new List<CBlockEdit>(pilar.GetBlocks().Count);
				var stackList = new List<KeyValuePair<CBlockEdit, CBlockEdit>>(pilar.GetBlocks().Count);
				var ieList = new List<IE.V4.BlockIE>(pilar.GetBlocks().Count);
				for(int j = 0; j < pilar.GetBlocks().Count; ++j)
				{
					float minHeight = float.MaxValue;
					CBlockEdit lowest = null;
					for (int i = 0; i < pilar.GetBlocks().Count; ++i)
					{
						var block = pilar.GetBlocks()[i] as CBlockEdit;
						if (block == null || nList.Contains(block))
							continue;
						if (block.GetHeight() < minHeight)
						{
							minHeight = block.GetHeight();
							lowest = block;
						}
					}
					nList.Add(lowest);
					stackList.Add(new KeyValuePair<CBlockEdit, CBlockEdit>(lowest.GetStackedBelow() as CBlockEdit, lowest.GetStackedAbove() as CBlockEdit));
					var blockIE = strucIE.GetBlocks()[lowest.GetIDXIE()];
					ieList.Add(blockIE);
					strucIE.RemoveBlock(lowest.GetIDXIE());
				}
				for(int i = 0; i < pilar.GetBlocks().Count; ++i)
				{
					var block = nList[i];
					var blockIE = ieList[i];
					
					block._SetPilarIndex(i);

					pilar.GetBlocks()[i] = block;
					block.SetIDXIE(strucIE.AddBlock(blockIE));
				}
				for(int i = 0; i < pilar.GetBlocks().Count; ++i)
				{
					var block = pilar.GetBlocks()[i] as CBlockEdit;
					var stacks = stackList[i];
					var blockIE = ieList[i];
					var lowerBlock = stacks.Key;
					var upperBlock = stacks.Value;

					if (lowerBlock != null)
					{
						block.GetStackedBlocksIdx()[0] = lowerBlock.GetPilarIndex();
						lowerBlock.GetStackedBlocksIdx()[1] = i;
						var lowerIDXIE = lowerBlock.GetIDXIE();
						blockIE.StackedIdx[0] = (short)lowerBlock.GetIDXIE();
						var lowerIE = strucIE.GetBlocks()[lowerIDXIE];
						lowerIE.StackedIdx[1] = (short)block.GetIDXIE();
					}
					if (upperBlock != null)
					{
						block.GetStackedBlocksIdx()[1] = upperBlock.GetPilarIndex();
						upperBlock.GetStackedBlocksIdx()[0] = i;
						var upperIDXIE = upperBlock.GetIDXIE();
						blockIE.StackedIdx[1] = (short)upperBlock.GetIDXIE();
						var upperIE = strucIE.GetBlocks()[upperIDXIE];
						upperIE.StackedIdx[0] = (short)block.GetIDXIE();
					}
				}
			}
			if (!shift)
			{
				for (int i = 0; i < m_Selected.Count; ++i)
				{
					var cur = m_Selected[i];

					if (cur.GetLayer() == 0 || cur.IsRemoved() || cur.IsStackLinkValid(1))
					{
						cur.SetSelected(false);
						continue;
					}
					var pilar = cur.GetPilar();
					float stairOffset = cur.GetBlockType() == Def.BlockType.STAIRS ? 0.5f : 0f;

					if (cur.GetProp() != null)
					{
						cur.GetProp().GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE,
							cur.GetProp().GetLE().GetCurrentHealth());
					}
					if (cur.GetMonster() != null)
					{
						if (m_Structure.GetLES().Contains(cur.GetMonster().GetLE()))
							m_Structure.GetLES().Remove(cur.GetMonster().GetLE());
						cur.GetMonster().GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, cur.GetMonster().GetLE().GetCurrentHealth());
						cur.SetMonster(null);
					}

					var nBlock = pilar.AddBlock();
					nBlock.SetHeight(cur.GetHeight() + stairOffset + 0.5f);
					nBlock.SetMicroHeight(cur.GetMicroHeight());
					var curIE = strucIE.GetBlocks()[cur.GetIDXIE()];
					var nBlockIE = new IE.V4.BlockIE()
					{
						Layer = (byte)cur.GetLayer(),
						StructureID = (ushort)pilar.GetStructureID(),
						VLength = 0,
					};
					nBlockIE.SetHeight(nBlock.GetHeight());
					nBlock.SetIDXIE(strucIE.AddBlock(nBlockIE));
					nBlock.SetLength(0.5f);
					cur.GetStackedBlocksIdx()[1] = nBlock.GetPilarIndex();
					curIE.StackedIdx[1] = (short)nBlock.GetIDXIE();
					nBlock.GetStackedBlocksIdx()[0] = cur.GetPilarIndex();
					nBlockIE.StackedIdx[0] = (short)cur.GetIDXIE();
					nBlock.SetLayer(cur.GetLayer());
					nBlock.SetMaterialFamily(cur.GetMaterialFamily());
					nBlock.SetSelected(true);
					nSelected.Add(nBlock);

					if (AreBlocksAbove(pilar, nBlock.GetHeight()))
						SortBlocks(pilar);

					cur.SetSelected(false);

					++done;
				}
			}
			else
			{
				for (int i = 0; i < m_Selected.Count; ++i)
				{
					var cur = m_Selected[i];

					if (cur.GetLayer() == 0 || cur.IsRemoved() || cur.IsStackLinkValid(0))
					{
						cur.SetSelected(false);
						continue;
					}
					var pilar = cur.GetPilar();

					var nBlock = pilar.AddBlock();
					var cHeight = cur.GetHeight();
					if (cHeight == 3.4f)
						cHeight = 3.5f;
					nBlock.SetHeight(cHeight - cur.GetLength());
					nBlock.SetMicroHeight(cur.GetMicroHeight());
					var curIE = strucIE.GetBlocks()[cur.GetIDXIE()];
					var nBlockIE = new IE.V4.BlockIE()
					{
						Layer = (byte)cur.GetLayer(),
						StructureID = (ushort)pilar.GetStructureID(),
						VLength = 0
					};
					nBlockIE.SetHeight(nBlock.GetHeight());
					nBlock.SetIDXIE(strucIE.AddBlock(nBlockIE));
					nBlock.SetLength(0.5f);
					nBlock.GetStackedBlocksIdx()[1] = cur.GetPilarIndex();
					nBlockIE.StackedIdx[1] = (short)cur.GetIDXIE();
					cur.GetStackedBlocksIdx()[0] = nBlock.GetPilarIndex();
					curIE.StackedIdx[0] = (short)nBlock.GetIDXIE();
					nBlock.SetLayer(cur.GetLayer());
					nBlock.SetMaterialFamily(cur.GetMaterialFamily());
					nBlock.SetSelected(true);
					nSelected.Add(nBlock);

					SortBlocks(pilar);

					cur.SetSelected(false);

					++done;
				}
			}

			m_Selected = nSelected;
			
			if (done > 0)
			{
				Structures.SetStrucModified(m_Structure.IDXIE);
				return false; // Something has been done, stop changing things this frame
			}
			return true;
		}
		bool OnBlockLayerChange()
		{
			var strucIE = Structures.Strucs[m_Structure.IDXIE];
			void ChangeLayer(int layer)
			{
				for (int i = 0; i < m_Selected.Count; ++i)
				{
					var cur = m_Selected[i];

					IE.V4.BlockIE blockIE = null;
					bool semiLock = false;
					if (cur.GetIDXIE() < 0)
					{
						blockIE = new IE.V4.BlockIE()
						{
							Layer = (byte)layer
						};
						cur.SetIDXIE(strucIE.AddBlock(blockIE));
					}
					else
					{
						blockIE = strucIE.GetBlocks()[cur.GetIDXIE()];
						blockIE.SetFlag(IE.V4.BlockIE.Flag.MaterialFamily, false);
						blockIE.SetFlag(IE.V4.BlockIE.Flag.Prop, false);

						int eFlags = blockIE.Flags.Count(p => p == true);
						if (blockIE.GetFlag(IE.V4.BlockIE.Flag.Anchor))
							--eFlags;
						semiLock = eFlags > 0;
					}
					blockIE.Layer = (byte)layer;
					blockIE.StructureID = (ushort)cur.GetPilar().GetStructureID();
					if (semiLock)
						cur.SetLockState(Def.LockState.SemiLocked);
					cur.SetLayer(layer);
				}
				if (m_Selected.Count > 0)
					Structures.SetStrucModified(m_Structure.IDXIE);
			}
			for (int i = Def.MaxLayerSlots - 1; i >= 0; --i)
			{
				if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.Layer1 + i))
					continue;

				if (!m_Structure.GetLayers()[i].IsValid())
					continue;

				ChangeLayer(i + 1);
				return false;
			}

			return true;
		}
		bool OnSelectAll()
		{
			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.SelectAll))
				return true;

			if(m_Selected.Count > 0)
			{
				for (int i = 0; i < m_Selected.Count; ++i)
				{
					var selBlock = m_Selected[i];
					if (selBlock == null)
						continue;
					selBlock.SetSelected(false);
				}
				m_Selected.Clear();
			}
			else
			{
				OnSelectAllButton();
			}

			return false;
		}
		bool OnMenuStart()
		{
			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.Menu))
				return true;

			OnMenuButton();
			return false;
		}
		bool OnReapplyLayers()
		{
			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.ReapplyLayers))
				return true;

			OnReapplyLayersButton();
			return false;
		}
		bool OnResetLocks()
		{
			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.ResetLocks))
				return true;

			OnResetLocksButton();
			return false;
		}
		bool OnLayerEdit()
		{
			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.LayerEdit))
				return true;

			OnEditLayersButton();
			return false;
		}
		bool OnVoidToggle()
		{
			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.Void))
				return true;

			var strucIE = Structures.Strucs[m_Structure.IDXIE];
			int done = 0;
			for (int i = 0; i < m_Selected.Count; ++i)
			{
				var cur = m_Selected[i];

				if (cur.GetLayer() == 0/* || cur.GetStairState() != Def.StairState.NONE*/)
				{
					continue;
				}

				var val = cur.GetVoidState() + 1;
				if (val == Def.BlockVoid.COUNT)
					val = Def.BlockVoid.NORMAL;

				cur.SetVoidState(val);
				var blockIE = strucIE.GetBlocks()[cur.GetIDXIE()];
				blockIE.BlockVoid = val;
				++done;
			}
			if (done > 0)
			{
				Structures.SetStrucModified(m_Structure.IDXIE);
				return false; // Something has been done, stop changing things this frame
			}
			return true;
		}
		bool OnBlockUnhide()
		{
			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.BlockUnhide))
				return true;

			SetBlockHidding(false);
			return false;
		}
		bool OnBlockHide()
		{
			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.BlockHide))
				return true;

			SetBlockHidding(true);
			return false;
		}
		#endregion
		#region KeyFunctions
		public bool ToggleVisibility(bool screenShooting = false, bool screenShootingStart = false)
		{
			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.Visibility) && !screenShooting)
				return true;

			bool hideInfo;
			if (!screenShooting)
			{
				hideInfo = Manager.Mgr.HideInfo = !Manager.Mgr.HideInfo;
			}
			else
			{
				hideInfo = screenShootingStart;
			}
			if (hideInfo)
			{
				if (SeedLock.isOn)
				{
					UnityEngine.Random.InitState(m_Seed);
				}
				else
				{
					m_ValueLock = true;
					m_Seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
					SeedIF.text = m_Seed.ToString();
					m_ValueLock = false;
				}
			}

			void hideUITask()
			{
				SeedPanel.SetActive(!hideInfo);
				LeftPanel.SetActive(!hideInfo);
				MenuButton.gameObject.SetActive(!hideInfo);
				CameraPanel.SetActive(!hideInfo);
				FPSText.gameObject.SetActive(!hideInfo);
				BlockPanelShowButton.gameObject.SetActive(!hideInfo);
				BlockPanel.gameObject.SetActive(false);

				if (m_BlockOver != null)
				{
					if (!m_BlockOver.IsSelected())
						m_BlockOver.SetHighlighted(false);
					m_BlockOver = null;
				}
			}

			void applyChangesTask()
			{
				if (hideInfo)
				{
					m_Structure.DuplicatePilars();
					// Enable invisible blocks
					SetBlockHidding(false);
					// Apply possible void
					m_Structure.ApplyVoid(/*hideInfo*/);
					// Apply possible stair
					m_Structure.ApplyStairs(/*hideInfo*/);
					// Convert into WIDE the valid blocks
					var wides = m_Structure.ApplyWides(/*hideInfo*/);
					m_Structure.ApplyMicroheight(wides);
					m_Structure.ApplyPropsMonsters();
				}
				else
				{
					m_Structure.RestorePilars();
					//// Convert into WIDE the valid blocks
					//m_Structure.ApplyWides(hideInfo);
					//// Unapply possible stair
					//m_Structure.ApplyStairs(hideInfo);
					//// Unapply possible void
					//m_Structure.ApplyVoid(hideInfo);
				}
			}

			void hideInfoSpritesTask()
			{
				var pilars = m_Structure.GetPilars();
				for (int i = 0; i < pilars.Length; ++i)
				{
					var pilar = pilars[i];
					if (pilar == null)
						continue;
					var blocks = pilar.GetBlocks();
					for (int j = 0; j < blocks.Count; ++j)
					{
						var block = blocks[j] as CBlockEdit;
						if (!block.gameObject.activeSelf)
							continue;
						// ToggleVisibility of layer0 blocks and non-layer0 info sprites
						if (block.GetLayer() != 0)
						{
							if (block.IsRemoved())
								continue;
							block.GetLayerRnd().enabled = !hideInfo;
							if (block.IsAnchor())
								block.GetAnchorRnd().enabled = !hideInfo;
							if (GameUtils.IsStairPossible(block.GetStairState()))
								block.GetStairRnd().enabled = !hideInfo;
							if (block.GetVoidState() != Def.BlockVoid.NORMAL)
							{
								if (hideInfo)
									block.GetVoidRnd().enabled = false;
								else
									block.SetVoidState(block.GetVoidState());
							}

							bool hideLock = hideInfo || block.IsRemoved();

							if (hideLock)
							{
								block.GetLockRnd().enabled = false;
							}
							if (!hideLock && block.GetLockState() != Def.LockState.Unlocked)
							{
								block.GetLockRnd().enabled = true;
							}
						}
						else // layer 0
						{
							block.GetTopMR().enabled = !hideInfo;
							block.GetMidMR().enabled = !hideInfo;
						}
						block.SetSelected(false);

						// Toggle visibility of monsters, props
						if (block.GetLayer() == 0 || block.GetBlockType() == Def.BlockType.STAIRS)
							continue;
						var prop = block.GetProp();
						if (prop != null)
						{
							prop.enabled = hideInfo;
						}
						var monster = block.GetMonster();
						if (monster != null)
						{
							monster.enabled = hideInfo;
							monster.GetLE().GetStatusBars().gameObject.SetActive(false);
						}
					}
				}
				m_Selected.Clear();
			}

			hideUITask();
			applyChangesTask();
			hideInfoSpritesTask();
			return false;
		}
		bool OnCamModeChange()
		{
			if (CameraEditButton.interactable && KeyMap.StrucEditCheck(Def.StrucEditKeyMap.CamEdit))
			{
				OnCameraEditButton();
				return false;
			}
			if (CameraGameButton.interactable && KeyMap.StrucEditCheck(Def.StrucEditKeyMap.CamGame))
			{
				OnCameraGameButton();
				return false;
			}
			if (CameraFreeButton.interactable && KeyMap.StrucEditCheck(Def.StrucEditKeyMap.CamFree))
			{
				OnCameraFreeButton();
				return false;
			}

			return true;
		}
		bool OnHelpUIStart()
		{
			if (!KeyMap.StrucEditCheck(Def.StrucEditKeyMap.Info))
				return true;

			HelpUI.gameObject.SetActive(true);
			enabled = false;

			return false;
		}
		void OnHelpUIEnd()
		{
			HelpUI.gameObject.SetActive(false);
			m_NextModificationTime = Time.time + 0.5f;
			enabled = true;
		}
		#endregion
		#region GUIActions
		void OnSeedValueChange(string value)
		{
			if (m_ValueLock)
				return;

			if(!int.TryParse(value, out int seed))
			{
				m_ValueLock = true;
				SeedIF.text = m_Seed.ToString();
				m_ValueLock = false;
			}

			m_Seed = seed;
			SeedLock.isOn = true;
		}
		void OnMenuButton()
		{
			EditMenuUI.gameObject.SetActive(true);
			if (!CameraFreeButton.interactable)
				CameraManager.Mgr.FreeCam.enabled = false;
			enabled = false;
		}
		public void OnCameraEditButton()
		{
			CameraEditButton.interactable = false;
			CameraFreeButton.interactable = true;
			CameraGameButton.interactable = true;
			CameraManager.Mgr.CameraType = ECameraType.EDITOR;
		}
		public void OnCameraGameButton()
		{
			CameraEditButton.interactable = true;
			CameraFreeButton.interactable = true;
			CameraGameButton.interactable = false;
			CameraManager.Mgr.CameraType = ECameraType.INGAME;
		}
		public void OnCameraFreeButton()
		{
			CameraEditButton.interactable = true;
			CameraFreeButton.interactable = false;
			CameraGameButton.interactable = true;
			CameraManager.Mgr.CameraType = ECameraType.FREE;
		}
		void OnSelectBiomeButton()
		{
			enabled = false;
			var curBiome = m_Structure.GetBiome();
			int removedElementPos = -1;
			string removedBiomeName = "";
			if(curBiome != null && curBiome.IDXIE >= 0)
			{
				var biomeIE = BiomeLoader.Biomes[curBiome.IDXIE];
				removedBiomeName = biomeIE.GetName();
				for(int i = 0; i < m_BiomeList.Count; ++i)
				{
					if(m_BiomeList[i].Name == removedBiomeName)
					{
						removedElementPos = i;
						break;
					}
				}
				if(removedElementPos >= 0)
				{
					m_BiomeList.RemoveAt(removedElementPos);
				}
			}
			m_BiomeList.Insert(0, new CImageSelectorUI.ElementInfo()
			{
				Name = "NEW"
			});
			ImageSelectorUI.gameObject.SetActive(true);
			ImageSelectorUI.Init(m_BiomeList, false, OnSelectBiomeButtonEnd, Def.ImageSelectorPosition.Center);
			m_BiomeList.RemoveAt(0);
			if(removedElementPos >= 0)
			{
				m_BiomeList.Insert(removedElementPos, new CImageSelectorUI.ElementInfo() { Name = removedBiomeName });
			}
			if (!CameraFreeButton.interactable)
				CameraManager.Mgr.FreeCam.enabled = false;
		}
		void OnSelectBiomeButtonEnd()
		{
			var selected = ImageSelectorUI.GetSelected();
			enabled = true;
			ImageSelectorUI.gameObject.SetActive(false);
			if (selected.Count != 1)
				return;

			var selection = selected[0];
			int bie;
			bool isNew = false;
			if(selection == "NEW")
			{
				bie = BiomeLoader.AddBiome();
				isNew = true;
			}
			else
			{
				bie = BiomeLoader.Dict[selection];
			}
			var biomeIE = BiomeLoader.Biomes[bie];
			m_Biome = biomeIE.ToBiome();
			m_Structure.SetBiome(m_Biome);
			if (isNew)
			{
				OnEditBiomeButton();
			}
			else
			{
				EditBiomeButton.interactable = true;
				OnReapplyLayersButton();
			}
			if (!CameraFreeButton.interactable)
				CameraManager.Mgr.FreeCam.enabled = true;
		}
		void OnEditBiomeButton()
		{
			enabled = false;
			BiomeEditor.gameObject.SetActive(true);
			BiomeEditor.Init(m_Biome, OnEditBiomeButtonEnd);
			if (!CameraFreeButton.interactable)
				CameraManager.Mgr.FreeCam.enabled = false;
		}
		void OnEditBiomeButtonEnd()
		{
			BiomeEditor.gameObject.SetActive(false);
			enabled = true;
			var tLayers = BiomeEditor.GetLayers();
			bool validLayers = false;
			var biomeIE = BiomeLoader.Biomes[m_Biome.IDXIE];
			for (int i = 0; i < tLayers.Length; ++i)
			{
				var tlayer = tLayers[i];
				m_Biome.GetLayers()[i] = tlayer.GetACopy();
				if (tlayer.IsValid())
				{
					validLayers = true;
					biomeIE.SetLayer((Def.BiomeLayerType)i, m_Biome.GetLayers()[i].ToIE());
				}
				else
				{
					biomeIE.SetLayer((Def.BiomeLayerType)i, null);
				}
			}
			EditBiomeButton.interactable = false;
			if (validLayers)
			{
				var biomeName = biomeIE.GetName();
				if (biomeName.Length == 0)
				{
					m_Biome = new World.Biome()
					{
						IDXIE = m_Biome.IDXIE
					};
				}
				else
				{
					EditBiomeButton.interactable = true;
					if(biomeIE.IsFromFile())
					{
						var fi = new FileInfo(biomeIE.GetFilePath());
						if (fi.Exists)
							fi.Delete();
					}
					biomeIE.SaveBiome(UnityEngine.Application.streamingAssetsPath + "/Biomes/" + biomeName + ".biome");
				}
			}
			
			UpdateBiomeList();
			OnReapplyLayersButton();
			if (!CameraFreeButton.interactable)
				CameraManager.Mgr.FreeCam.enabled = true;
		}
		void OnEditLayersButton()
		{
			enabled = false;
			LayerEditor.gameObject.SetActive(true);
			LayerEditor.Init(m_Structure, OnEditLayersButtonEnd);
			if (!CameraFreeButton.interactable)
				CameraManager.Mgr.FreeCam.enabled = false;
		}
		void OnEditLayersButtonEnd()
		{
			enabled = true;
			LayerEditor.gameObject.SetActive(false);
			var tLayers = LayerEditor.GetLayers();
			var sLayers = m_Structure.GetLayers();
			int validLayers = 0;
			// Dump valid temporal layers to the real ones, and apply them.
			for (int i = 0; i < Def.MaxLayerSlots; ++i)
			{
				var layer = sLayers[i];
				if (tLayers[i].IsValid())
				{
					if (layer != tLayers[i])
						Structures.SetStrucModified(m_Structure.IDXIE, true);
					++validLayers;
					layer.CopyFrom(tLayers[i]);
					layer.SortProbabilities();
				}
				else if (layer.IsValid()) // invalidate
				{
					if (layer != tLayers[i])
						Structures.SetStrucModified(m_Structure.IDXIE, true);
					layer.CopyFrom(LayerInfo.GetDefaultLayer());
					layer.Slot = i + 1;
				}
				m_Structure.SetLayer(layer);
			}
			for (int i = 0; i < m_Selected.Count; ++i)
			{
				if (m_Selected[i] == null)
					continue;
				m_Selected[i].SetSelected(false);
			}
			m_Selected.Clear();

			var enable = validLayers > 0;
			ResetLocksButton.interactable = enable;
			ReapplyLayersButton.interactable = enable;
			m_NextModificationTime = Time.time + 0.5f;
			enabled = true;
			if (!CameraFreeButton.interactable)
				CameraManager.Mgr.FreeCam.enabled = true;
		}
		void OnReapplyLayersButton()
		{
			m_Structure.ReapplyLayers();
		}
		void OnSelectAllButton()
		{
			SetBlockHidding(false);
			m_Selected.Clear();
			var pilars = m_Structure.GetPilars();
			for (int i = 0; i < pilars.Length; ++i)
			{
				var pilar = pilars[i];
				if (pilar == null)
					continue;
				var blocks = pilar.GetBlocks();
				for (int j = 0; j < blocks.Count; ++j)
				{
					var block = blocks[j] as CBlockEdit;
					if (block.IsRemoved())
						continue;
					block.SetSelected(true);
					m_Selected.Add(block);
				}
			}
		}
		void OnResetLocksButton()
		{
			var strucIE = Structures.Strucs[m_Structure.IDXIE];
			var pilars = m_Structure.GetPilars();

			for (int i = 0; i < pilars.Length; ++i)
			{
				var pilar = pilars[i];
				if (pilar == null)
					continue;

				var blocks = pilar.GetBlocks();
				for (int j = 0; j < blocks.Count; ++j)
				{
					var block = (CBlockEdit)blocks[j];

					if (block.IsRemoved() || block.GetLayer() == 0)
						continue;

					var blockIE = strucIE.GetBlocks()[block.GetIDXIE()];

					var layer = m_Structure.GetLayers()[block.GetLayer() - 1];
					if (layer.IsLinkedLayer)
					{
						layer = m_Structure.GetLayers()[block.GetLinkedTo() - 1];
					}

					block.SetLockState(Def.LockState.Unlocked);
					blockIE.SetDefault();
					blockIE.Layer = (byte)block.GetLayer();
					blockIE.SetFlag(IE.V4.BlockIE.Flag.Anchor, block.IsAnchor());
					blockIE.BlockType = block.GetBlockType();
					if (block.GetStairState() != Def.StairState.NONE)
					{
						blockIE.Stair = block.GetStairState();
						blockIE.Rotation = block.GetRotation();
						blockIE.SetFlag(IE.V4.BlockIE.Flag.Rotation, true);
						block.SetLockState(Def.LockState.SemiLocked);
					}
					blockIE.SetHeight(block.GetHeight());
					if (Mathf.Clamp(block.GetLength(), layer.BlockLengthMin, layer.BlockLengthMax) != block.GetLength())
					{
						blockIE.SetLength(block.GetLength());
						blockIE.SetFlag(IE.V4.BlockIE.Flag.Length, true);
						block.SetLockState(Def.LockState.SemiLocked);
					}
				}
			}
		}
		void OnStrucXSizeSlider(float value)
		{
			m_NextStrucSizeTime = Time.time + StrucSizeDelay;
			m_NextStrucHeight = (int)value;
		}
		void OnStrucYSizeSlider(float value)
		{
			m_NextStrucSizeTime = Time.time + StrucSizeDelay;
			m_NextStrucWidth = (int)value;
		}
		void OnBlockPanelShowButton()
		{
			BlockPanelShowButton.gameObject.SetActive(false);
			BlockPanel.gameObject.SetActive(true);
		}
		void OnSpawnerEditor()
		{
			enabled = false;
			if (!CameraFreeButton.interactable)
				CameraManager.Mgr.FreeCam.enabled = false;
			SpawnerEditor.gameObject.SetActive(true);
			SpawnerEditor.Init(OnSpawnerEditorEnd);
		}
		void OnSpawnerEditorEnd()
		{
			enabled = true;
			if (!CameraFreeButton.interactable)
				CameraManager.Mgr.FreeCam.enabled = true;
			SpawnerEditor.gameObject.SetActive(false);
		}
		#endregion
	}
}
