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
	public class TestArenaUI : MonoBehaviour
	{
		enum SelectionType
		{
			Items,
			Monster,

		}
		public UnityEngine.UI.Button AddMonsterButton;
		public UnityEngine.UI.Button EditMonsterButton;
		public UnityEngine.UI.Button TagEditorButton;
		public UnityEngine.UI.Button PropTagEditorButton;
		public UnityEngine.UI.Button ItemEditorButton;

		public UnityEngine.UI.Toggle StatToggle;
		public UnityEngine.UI.Toggle MovToggle;
		public UnityEngine.UI.Toggle SpellToggle;
		public UnityEngine.UI.Toggle AIToggle;

		public UnityEngine.UI.Button RefreshOddButton;

		public MainMenuUI MainMenu;
		public CMonsterEditorUI MonsterEditor;
		public CTagEditorUI TagEditorUI;
		public CPropTagEditorUI PropTagEditorUI;
		static ItemEditorUI ItemEditor;
		static SelectorUI Selector;
		SelectionType m_SelectionType;
		static InventoryUI Inventory;

		public GameObject ItemPanel;
		public UnityEngine.UI.Image[] ItemImage;
		public TMPro.TMP_Text[] ItemName;
		public TMPro.TMP_Text[] ItemCount;
		public TMPro.TMP_Text[] Slot;

		public GameObject AnchorGO;

		List<ViewElementBSelectableInfo> m_MonsterList;
		CBlock m_SelectedBlock;
		AI.CMonster m_SelectedMonster;
		AI.ODD.COddController m_Odd;
		AI.CMonsterOddController m_ControlledMonster;

		World.World m_World;

		//bool m_ItemSlotWasChangeable;
		// OLD
		//CStrucEdit m_Struc;
		//AI.Succubus m_TestMonster;

		readonly string DefaultStructureName = "Alpha Test Arena_000"; //"Arena Souls Playground_000";
		//readonly string DefaultBiomeName = "Soul's Playground";

		void LoadSelectorUI()
		{
			if (Selector != null)
				return;

			Selector = Instantiate(SelectorUI.GetInstance());

			var canvas = CameraManager.Mgr.Canvas;
			Selector.transform.SetParent(canvas.transform);

			var rt = Selector.gameObject.GetComponent<RectTransform>();
			rt.sizeDelta = Vector2.zero;
			rt.anchoredPosition = Vector2.zero;

			Selector.OnSelection += OnSelection;
			Selector.OnClose += OnSelectorClose;
		}
		void LoadItemEditorUI()
		{
			if (ItemEditor != null)
				return;

			ItemEditor = Instantiate(ItemEditorUI.GetInstance());
			var canvas = CameraManager.Mgr.Canvas;
			ItemEditor.transform.SetParent(canvas.transform);

			var rt = ItemEditor.gameObject.GetComponent<RectTransform>();
			rt.sizeDelta = Vector2.zero;
			rt.anchoredPosition = Vector2.zero;

			ItemEditor.OnClose += OnEditItemEnd;
		}
		void LoadInventoryUI()
		{
			if (Inventory != null)
				return;

			Inventory = Instantiate(InventoryUI.GetInstance());
			var canvas = CameraManager.Mgr.Canvas;
			Inventory.transform.SetParent(canvas.transform);

			var rt = Inventory.gameObject.GetComponent<RectTransform>();
			rt.sizeDelta = Vector2.zero;
			rt.anchoredPosition = Vector2.zero;

			Inventory.OnClose += OnInventoryClose;
		}
		void OnInventoryClose()
		{
			Inventory.gameObject.SetActive(false);
			Manager.Mgr.IsPaused = false;
			enabled = true;
			CameraManager.Mgr.enabled = true;
		}
		public void OnTagEditor()
		{
			TagEditorUI.gameObject.SetActive(true);
			enabled = false;
			CameraManager.Mgr.enabled = false;
			Manager.Mgr.IsPaused = true;
			TagEditorUI.Init(() =>
			{
				enabled = true;
				CameraManager.Mgr.enabled = true;
				Manager.Mgr.IsPaused = false;
			});
		}
		public void OnPropTagEditor()
		{
			PropTagEditorUI.gameObject.SetActive(true);
			enabled = false;
			CameraManager.Mgr.enabled = false;
			Manager.Mgr.IsPaused = true;
			PropTagEditorUI.Init(() =>
			{
				enabled = true;
				CameraManager.Mgr.enabled = true;
				Manager.Mgr.IsPaused = false;
			});
		}
		public void OnMenuButton()
		{
			if(m_World != null)
				GameUtils.DeleteGameobject(m_World.gameObject);

			//Map.GetCurrent().DestroyMap();
			MainMenu.gameObject.SetActive(true);
			gameObject.SetActive(false);
			Manager.Mgr.GameInputControl = Def.GameInputControl.Mouse;
			var cam = CameraManager.Mgr;
			cam.transform.position = Vector3.zero;
			cam.transform.rotation = Quaternion.identity;
			cam.CameraType = ECameraType.COUNT;
		}
		public void OnAddMonster()
		{
			LoadSelectorUI();

			InitMonsterList();

			Manager.Mgr.IsPaused = true;

			gameObject.SetActive(false);
			CameraManager.Mgr.enabled = false;
			m_SelectionType = SelectionType.Monster;
			if(Selector.ItemView is CViewSorted)
			{
				var view = Selector.ItemView as CViewSorted;
				view.SetSortFn((CViewElementB left, CViewElementB right) =>
				{
					var l = left as CViewElementBImageNameSelectable;
					var r = right as CViewElementBImageNameSelectable;
					int comparison = string.Compare(l.Name.text, r.Name.text, comparisonType: StringComparison.OrdinalIgnoreCase);
					return comparison > 0;
				});
			}
			Selector.gameObject.SetActive(true);
			Selector.Init("Select Monster", CViewElementBImageNameSelectable.GetNameImageInstance(), m_MonsterList, false);
		}
		private void OnEntityDeath(AI.CLivingEntity entity)
		{
			if(entity.gameObject.GetComponent<AI.CMonsterOddController>())
			{
				m_ControlledMonster = null;
				CameraManager.Mgr.Target = CameraManager.Mgr.DefaultTarget;
			}
			if(m_SelectedMonster != null && m_SelectedMonster.gameObject == entity.gameObject)
			{
				m_SelectedMonster = null;
				EditMonsterButton.interactable = false;
			}
			//if (entity.GetCurrentStruc() != null && entity.GetCurrentStruc().GetLES().Contains(entity))
			//	entity.GetCurrentStruc().GetLES().Remove(entity);
			//GameUtils.DeleteGameObjectAndItsChilds(entity.gameObject);
		}
		public void OnEditMonster()
		{
			Manager.Mgr.IsPaused = true;
			gameObject.SetActive(false);
			MonsterEditor.gameObject.SetActive(true);
			CameraManager.Mgr.enabled = false;
			MonsterEditor.Init(m_SelectedMonster.GetFamily(), OnEditMonsterEnd);
		}
		public void OnEditMonsterEnd()
		{
			Manager.Mgr.IsPaused = false;
			MonsterEditor.gameObject.SetActive(false);
			gameObject.SetActive(true);
			CameraManager.Mgr.enabled = true;
		}
		public void OnSelectItem()
		{
			LoadSelectorUI();

			if (Selector.ItemView is CViewSorted)
			{
				(Selector.ItemView as CViewSorted).SetSortFn(null);
			}

			var items = new List<ViewElementBSelectableInfo>(AI.Items.ItemLoader.ItemIDDict.Count + 1)
			{
				new ViewElementBItemSelectableInfo()
				{
					Selector = Selector,
					Image = null,
					Item = null,
					Name = "New Item"
				},
			};

			for (int i = 0; i < AI.Items.ItemLoader.ItemIDDict.Count; ++i)
			{
				var itemIdx = AI.Items.ItemLoader.ItemIDDict.ElementAt(i).Value;
				var item = AI.Items.ItemLoader.Items[itemIdx];
				Sprite itemImage;
				if (AI.Items.ItemLoader.ItemSpriteDict.TryGetValue(item.GetImageName(), out int spriteIdx))
					itemImage = AI.Items.ItemLoader.ItemSprites[spriteIdx];
				else
					itemImage = AI.Items.ItemLoader.InvalidItem;

				items.Add(new ViewElementBItemSelectableInfo()
				{
					Selector = Selector,
					Image = itemImage,
					Item = item,
					Name = item.GetName() + '_' + item.GetID().ToString()
				});
			}
			Manager.Mgr.IsPaused = true;
			CameraManager.Mgr.enabled = false;
			gameObject.SetActive(false);
			Selector.gameObject.SetActive(true);
			m_SelectionType = SelectionType.Items;
			Selector.Init("Item Selector", CViewElementBItemSelectable.GetItemInstance(), items, false);

			m_SelectionType = SelectionType.Items;
		}
		private void OnSelectorClose()
		{
			Selector.gameObject.SetActive(false);
			if(Selector.GetSelected().Count == 0 || m_SelectionType == SelectionType.Monster)
			{
				Manager.Mgr.IsPaused = false;
				gameObject.SetActive(true);
				CameraManager.Mgr.enabled = true;
			}
		}
		private void OnSelection(List<ViewElementBSelectableInfo> selection)
		{
			if (selection.Count == 0)
				return;

			switch (m_SelectionType)
			{
				case SelectionType.Items:
					{
						LoadItemEditorUI();

						var selItem = (selection[0] as ViewElementBItemSelectableInfo).Item;
						if (selItem == null)
							selItem = new AI.Items.Item();

						Manager.Mgr.IsPaused = true;
						CameraManager.Mgr.enabled = false;
						gameObject.SetActive(false);
						ItemEditor.gameObject.SetActive(true);
						ItemEditor.Init(selItem);
					}
					break;
				case SelectionType.Monster:
					{
						var sel = selection[0] as ViewElementBImageNameSelectableInfo;
						if(!Monsters.FamilyDict.TryGetValue(sel.Name, out int monID))
						{
							Debug.LogWarning("Couldn't find a monster named " + sel.Name);
							return;
						}

						var mon = new GameObject().AddComponent<AI.CMonster>();
						mon.SetMonster(Monsters.MonsterFamilies[monID]);
						mon.transform.position = m_SelectedBlock.GetPilar().transform.position +
							new Vector3(UnityEngine.Random.value, m_SelectedBlock.GetHeight() +
							m_SelectedBlock.GetMicroHeight(), UnityEngine.Random.value);

						mon.GetME().Impulse(new Vector2(UnityEngine.Random.value * 2f - 1f, UnityEngine.Random.value * 2f - 1f), 0f);

						if (m_ControlledMonster == null && m_Odd == null)
						{
							m_ControlledMonster = mon.gameObject.AddComponent<AI.CMonsterOddController>();
							m_ControlledMonster.Set(mon);
							CameraManager.Mgr.Target = m_ControlledMonster.gameObject;
						}
						else
						{
							var controller = mon.gameObject.AddComponent<AI.CMonsterController>();
							controller.Set(mon);
							controller.SetTargetPostion(controller.transform.position);
						}
						mon.GetLE().OnEntityDeath += OnEntityDeath;


					}
					break;
			}
		}
		public void OnEditItemEnd()
		{
			Manager.Mgr.IsPaused = false;
			ItemEditor.gameObject.SetActive(false);
			gameObject.SetActive(true);
			CameraManager.Mgr.enabled = true;
		}
		public void InitMap()
		{
			if(!Structures.StrucDict.TryGetValue(DefaultStructureName, out int ieIDX))
			{
				Debug.LogError("Structure: '" + DefaultStructureName + "' is missing, Structure selector not added.");
				return;
			}

			while(m_World.GetStrucInfos() == null || m_World.GetStrucInfos().Count == 0)
				m_World.Generate(new Vector2Int(32, 32), UnityEngine.Random.Range(int.MinValue, int.MaxValue), new List<string>() { DefaultStructureName });

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

			for(int i = 0; i < m_World.GetStrucs().Length; ++i)
			{
				var struc = m_World.GetStrucs()[i];
				struc.gameObject.SetActive(true);
				struc.LoadFromWorld(m_World);
			}

			//var struc = CStrucEdit.LoadStruc(ieIDX);

			//if(BiomeLoader.Dict.TryGetValue(DefaultBiomeName, out int biomeIdx))
			//{
			//	var biome = BiomeLoader.Biomes[biomeIdx];
			//	struc.SetBiome(biome.ToBiome());
			//	struc.ReapplyLayers();
			//	struc.ApplyVoid();
			//	struc.ApplyStairs();
			//	//var wides = struc.ApplyWides();
			//	//struc.ApplyMicroheight(wides);
			//	struc.ApplyPropsMonsters();
			//}


			//// Will prepare everything for the game map
			//// After we have to destroy the CStrucEdits
			//Map.Init(new List<CStrucEdit>() { struc });

			//// Set the camera to the starting point
			//float strucHalfX = struc.transform.position.x + (struc.GetWidth() * (1f + Def.BlockSeparation)) * 0.5f;
			//float strucHalfZ = struc.transform.position.z + (struc.GetHeight() * (1f + Def.BlockSeparation)) * 0.5f;
			//CameraManager.Mgr.Target.transform.position = new Vector3(strucHalfX, 0f, strucHalfZ);
			//CameraManager.Mgr.CameraType = ECameraType.INGAME;
		}
		public void InitMonsterList()
		{
			if (m_MonsterList != null)
				return;

			m_MonsterList = new List<ViewElementBSelectableInfo>(Monsters.MonsterFamilies.Count - 1);
			for(int i = 1; i < Monsters.MonsterFamilies.Count; ++i)
			{
				var family = Monsters.MonsterFamilies[i];
				m_MonsterList.Add(new ViewElementBImageNameSelectableInfo()
				{
					Image = family.Frames[0],
					Name = family.Name,
					Selector = Selector
				});
			}
		}
		public void Init()
		{
			var manager = Manager.Mgr;
			manager.HideInfo = true;
			manager.UIAnchor = AnchorGO;
			AIToggle.isOn = manager.DebugAI = false;
			MovToggle.isOn = manager.DebugMovement = false;
			SpellToggle.isOn = manager.DebugSpells = false;
			StatToggle.isOn = manager.DebugStats = false;

			m_World = new GameObject("World").AddComponent<World.World>();
			manager.GameInputControl = Def.GameInputControl.Mouse;

			InitMap();
		}
		//public void Init2()
		//{
		//	void HideDebugInfo()
		//	{
		//		var pilars = m_Struc.GetPilars();
		//		for (int i = 0; i < pilars.Length; ++i)
		//		{

		//			var pilar = pilars[i];
		//			if (pilar == null)
		//				continue;
		//			var blocks = pilar.GetBlocks();
		//			for (int j = 0; j < blocks.Count; ++j)
		//			{

		//				var block = blocks[j] as CBlockEdit;
		//				if (block.GetLayer() == 0)
		//				{
		//					block.DestroyBlock(false);
		//				}
		//			}
		//		}
		//		m_Struc.ApplyVoid(/*true*/);
		//		m_Struc.ApplyStairs(/*true*/);
		//		m_Struc.ApplyWides(/*true*/);
		//	}
		//	var ieIDX = Structures.StrucDict[DefaultStructureName];
		//	if (ieIDX < 0)
		//	{
		//		Debug.LogError("Structure: '" + DefaultStructureName + "' is missing, Structure selector not added.");
		//		return;
		//	}
		//	Manager.Mgr.HideInfo = true;
		//	m_Struc = CStrucEdit.LoadStruc(ieIDX);
		//	HideDebugInfo();
		//	float strucHalfX = m_Struc.transform.position.x + (m_Struc.GetWidth() * (1f + Def.BlockSeparation)) * 0.5f;
		//	float strucHalfZ = m_Struc.transform.position.z + (m_Struc.GetHeight() * (1f + Def.BlockSeparation)) * 0.5f;
		//	CameraManager.Mgr.Target.transform.position = new Vector3(strucHalfX, 0f, strucHalfZ);
		//	CameraManager.Mgr.CameraType = ECameraType.INGAME;
		//	//for (int i = 0; i < 10; ++i)
		//	//{
		//	//	var testMonster = Monsters.AddMonsterComponent(new GameObject($"Monster_{MonsterScript.MonsterID++}"), Monsters.FamilyDict["Succubus"]) as AI.Succubus;
		//	//	testMonster.InitMonster();
		//	//	var monsterPos = new Vector3(strucHalfX + (UnityEngine.Random.value * 5f - 2.5f), 0f, strucHalfZ + (UnityEngine.Random.value * 5f - 2.5f));
		//	//	testMonster._TargetPos = monsterPos;
		//	//	testMonster.transform.position = monsterPos;
		//	//}
		//	var testSprite = Monsters.MonsterFamilies[Monsters.FamilyDict["Succubus"]].Frames[0];
		//	var texSize = testSprite.texture.width;
		//	var scale = texSize / testSprite.pixelsPerUnit;
		//	var pivot = testSprite.pivot / testSprite.pixelsPerUnit;
		//	for (int i = 0; i < (int)SpriteBackendType.COUNT; ++i)
		//	{
		//		var backendType = (SpriteBackendType)i;
		//		var componentType = SpriteUtils.GetSpriteBackend(backendType);
		//		var obj = new GameObject(backendType.ToString());
		//		var cmp = obj.AddComponent(componentType) as ISpriteBackend;
		//		cmp.SetSprite(testSprite);
		//		obj.transform.position = new Vector3(strucHalfX + i * 1.25f, 0f, strucHalfZ);
		//		obj.layer = Def.RCLayerLE;
		//		obj.transform.Rotate(0f, 180f, 0f, Space.World);
		//		//if (backendType > SpriteBackendType.MIX)
		//		//{
		//		//	obj.transform.localScale = new Vector3(scale, scale, 1f);
		//		//	obj.transform.position = obj.transform.position + new Vector3(pivot.x, -pivot.y, 0f);
		//		//}
		//	}
		//}
		public void OnStatToggle(bool value) => Manager.Mgr.DebugStats = value;
		public void OnMovToggle(bool value) => Manager.Mgr.DebugMovement = value;
		public void OnSpellToggle(bool value) => Manager.Mgr.DebugSpells = value;
		public void OnAIToggle(bool value) => Manager.Mgr.DebugAI = value;
		private void Awake()
		{
			AddMonsterButton.onClick.AddListener(OnAddMonster);
			EditMonsterButton.onClick.AddListener(OnEditMonster);
			ItemEditorButton.onClick.AddListener(OnSelectItem);
			TagEditorButton.onClick.AddListener(OnTagEditor);
			PropTagEditorButton.onClick.AddListener(OnPropTagEditor);
			StatToggle.onValueChanged.AddListener(OnStatToggle);
			MovToggle.onValueChanged.AddListener(OnMovToggle);
			SpellToggle.onValueChanged.AddListener(OnSpellToggle);
			AIToggle.onValueChanged.AddListener(OnAIToggle);
			RefreshOddButton.onClick.AddListener(OnOddRefresh);
		}
		void UpdateCameraMovement()
		{
			const float MoveAmount = 10f;
			var movement = Vector2.zero;
			var camera = CameraManager.Mgr;
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
				var target = CameraManager.Mgr.Target;
				if(target != null)
					target.transform.Translate(movement.y, 0f, movement.x, Space.World);
			}
		}
		void OnMouseRaycast()
		{
			bool mouseLeftClick = Input.GetMouseButtonDown(0);
			if (!mouseLeftClick || UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
				return;

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			// MonsterHit
			bool rayHit = Physics.Raycast(ray, out RaycastHit mouseHit, 10000f, 1 << Def.RCLayerLE);
			if (!rayHit)
			{
				m_SelectedMonster = null;
				EditMonsterButton.interactable = false;
			}
			else
			{
				if(mouseHit.transform.gameObject.TryGetComponent(out m_SelectedMonster))
				{
					EditMonsterButton.interactable = true;
				}
				else
				{
					EditMonsterButton.interactable = false;
				}
				return;
			}
			// BlockHit
			rayHit = Physics.Raycast(ray, out mouseHit, 10000f, 1 << Def.RCLayerBlock);
			if(!rayHit)
			{
				m_SelectedBlock = null;
				AddMonsterButton.interactable = false;
				return;
			}
			m_SelectedBlock = mouseHit.transform.gameObject.GetComponent<CBlock>();
			if(m_SelectedBlock == null || m_SelectedBlock.GetPilar() == null)
			{
				m_SelectedBlock = null;
				AddMonsterButton.interactable = false;
			}
			else
			{ 
				AddMonsterButton.interactable = true;
			}
		}
		void OnOddRefresh()
		{
			AssetLoader.OddWeaponInfos = Resources.LoadAll<OddWeaponInfo>("OddWeaponInfos");
			WeaponLoader.Prepare();

			if(m_Odd != null)
			{
				if (WeaponLoader.Dict.TryGetValue(AI.ODD.COddController.BaseWeaponName, out int weaponIdx))
				{
					m_Odd.GetWeapons()[0].Init(m_Odd, WeaponLoader.WeaponInfos[weaponIdx]);
				}
			}
		}
		//int FixItemSlot(int slotIdx, int slotCount)
		//{
		//	if(slotIdx == slotCount)
		//	{
		//		slotIdx = 0;
		//	}
		//	else if (slotIdx > slotCount)
		//	{
		//		slotIdx = (slotIdx - slotCount) + 1;
		//	}
		//	else if (slotIdx < 0)
		//	{
		//		slotIdx = slotCount + slotIdx;
		//	}
		//	return slotIdx;
		//}
		//void OnSelectedItemChange(int nSelectedItem)
		//{
		//	var sc = m_Odd.SpellCaster;
		//	int slotCount = sc.GetItems().Length;
		//	nSelectedItem = FixItemSlot(nSelectedItem, slotCount);
		//	sc.SetSelectedSlot(nSelectedItem);
		//	nSelectedItem = sc.GetSelectedSlot();
		//	var iv0SlotIdx = FixItemSlot(nSelectedItem - 1, slotCount);
		//	var iv1SlotIdx = nSelectedItem;
		//	var iv2SlotIdx = FixItemSlot(nSelectedItem + 1, slotCount);
		//	SetItemView(0, iv0SlotIdx);
		//	SetItemView(1, iv1SlotIdx);
		//	SetItemView(2, iv2SlotIdx);
		//}
		//void SetItemView(int idx, int slotIdx)
		//{
		//	var sc = m_Odd.SpellCaster;
		//	var slot = sc.GetItems()[slotIdx];

		//	string countTxt;
		//	string nameTxt;
		//	Sprite image;

		//	if(slot.Count > 0) // Display Item
		//	{
		//		countTxt = slot.Count.ToString();
		//		nameTxt = slot[0].GetName();
		//		if(AI.Items.ItemLoader.ItemSpriteDict.TryGetValue(slot[0].GetImageName(), out int imageIdx))
		//		{
		//			image = AI.Items.ItemLoader.ItemSprites[imageIdx];
		//		}
		//		else
		//		{
		//			image = AI.Items.ItemLoader.InvalidItem;
		//		}
		//	}
		//	else // Display empty
		//	{
		//		countTxt = "";
		//		nameTxt = "";
		//		image = AI.Items.ItemLoader.InvalidItem;
		//	}

		//	Slot[idx].text = (slotIdx + 1).ToString();
		//	ItemCount[idx].text = countTxt;
		//	ItemName[idx].text = nameTxt;
		//	ItemImage[idx].sprite = image;
		//}
		void OnOddSpawn()
		{
			//ItemPanel.gameObject.SetActive(true);
			m_Odd.LE.OnEntityDeath += OnOddDeath;
			//var sc = m_Odd.SpellCaster;
			//for
			//for(int i = 0; i < AI.Items.ItemLoader.Items.Count; ++i)
			//{
			//	var item = AI.Items.ItemLoader.Items[i];
			//	for (int j = 0; j < 50; ++j)
			//	{
			//		bool success = sc.AddItemToInventory();
			//	}
			//}
			//m_ItemSlotWasChangeable = true;
			//OnSelectedItemChange(0);
		}
		void OnOddDeath(AI.CLivingEntity _)
		{
			ItemPanel.gameObject.SetActive(false);
			m_Odd.LE.OnEntityDeath -= OnOddDeath;
		}
		void UpdateGameInputControl()
		{
			if(Input.GetKeyDown(KeyCode.Q))
			{
				var mgr = Manager.Mgr;
				if (mgr.GameInputControl == Def.GameInputControl.Mouse)
					mgr.GameInputControl = Def.GameInputControl.MouseLikeController;
				else if (mgr.GameInputControl == Def.GameInputControl.MouseLikeController)
					mgr.GameInputControl = Def.GameInputControl.Mouse;
			}
		}
		void OnOpenInventory(AI.CSpellCaster spellCaster)
		{
			LoadInventoryUI();

			Manager.Mgr.IsPaused = true;
			enabled = false;
			CameraManager.Mgr.enabled = false;
			Inventory.gameObject.SetActive(true);

			Inventory.Init(spellCaster);
		}
		void Update()
		{
			if(m_ControlledMonster == null && m_Odd == null)
			{
				if(Input.GetKey(KeyCode.O))
				{
					m_Odd = Instantiate(AssetLoader.Odd);
					var cammgr = CameraManager.Mgr;
					m_Odd.transform.position = cammgr.Target.transform.position + new Vector3(0f, 2f, 0f);
					m_Odd.Init();
					cammgr.Target = m_Odd.gameObject;
					OnOddSpawn();
					return;
				}

				UpdateCameraMovement();
			}
			else if(m_SelectedMonster == null)
			{
				bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
				if (shift && Input.GetKey(KeyCode.Delete))
				{
					if (m_ControlledMonster != null)
						m_ControlledMonster.GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, m_ControlledMonster.GetLE().GetCurrentHealth());
					else
						m_Odd.LE.ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, m_Odd.LE.GetCurrentHealth());
				}
			}
			OnMouseRaycast();
			UpdateGameInputControl();

			if(Input.GetKeyDown(KeyCode.I))
			{
				if(m_Odd != null)
				{
					OnOpenInventory(m_Odd.SpellCaster);
					return;
				}
				else if(m_ControlledMonster != null)
				{
					OnOpenInventory(m_ControlledMonster.GetMonster().GetSpellCaster());
					return;
				}
			}

			//if(ItemPanel.gameObject.activeSelf)
			//{
			//	var sc = m_Odd.SpellCaster;

			//	if (!m_ItemSlotWasChangeable && sc.CanChangeSelectedSlot())
			//		OnSelectedItemChange(sc.GetSelectedSlot());

			//	var wheel = (int)Input.mouseScrollDelta.y;
			//	if (wheel != 0)
			//	{
			//		//Debug.Log(wheel);
			//		OnSelectedItemChange(sc.GetSelectedSlot() + wheel);
			//	}
			//	m_ItemSlotWasChangeable = sc.CanChangeSelectedSlot();
			//}

			if(m_SelectedMonster != null)
			{
				if(Input.GetKey(KeyCode.Delete))
				{
					m_SelectedMonster.GetLE().ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, m_SelectedMonster.GetLE().GetCurrentHealth());
				}
			}
		}
	}
}