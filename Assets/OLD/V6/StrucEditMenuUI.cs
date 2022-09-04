/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;

//namespace Assets.UI
//{
//	public class StrucEditMenuUI : MonoBehaviour
//	{
//		public StrucEditUI EditUI;
//		public UnityEngine.UI.Button CrossBttn;
//		public UnityEngine.UI.Button CreateBttn;
//		public UnityEngine.UI.Button CopyBttn;
//		public UnityEngine.UI.Button LoadBttn;
//		public UnityEngine.UI.Button SaveBttn;
//		public UnityEngine.UI.Button ExitBttn;

//		public UnityEngine.UI.Image SavePanel;
//		public UnityEngine.UI.Text SaveInvalidStrucNameText;
//		public UnityEngine.UI.InputField SaveStrucIF;
//		public UnityEngine.UI.Button SaveCancelButton;
//		public UnityEngine.UI.Button SaveDontButton;
//		public UnityEngine.UI.Button SaveApplyButton;
//		public CImageSelectorUI SelectorUI;

//		public RenderTexture ScreenRNDTex;

//		Def.StrucEditMenuFunc m_CurrentFunc;
//		int m_SelectedStruc;

//		List<CImageSelectorUI.ElementInfo> m_Strucs;
//		string m_TempStrucName;

//		bool CanStrucBeSaved()
//		{
//			var struc = EditUI.GetStruc();
//			if (!Structures.IsStrucModified(struc.IDXIE))
//				return false; // Not needed

//			var sLayers = struc.GetLayers();
//			bool anyLayerValid = false;
//			for(int i = 0; i < sLayers.Length; ++i)
//			{
//				if(sLayers[i].IsValid())
//				{
//					anyLayerValid = true;
//					break;
//				}
//			}
//			if (!anyLayerValid)
//				return false;

//			if(struc.IDXIE < 0 || struc.IDXIE >= Structures.Strucs.Count || Structures.Strucs[struc.IDXIE] == null)
//			{
//				Debug.LogWarning("Invalid structure IDXIE, something must went wrong!, IDXIE=" + struc.IDXIE.ToString());
//				return false;
//			}
//			var ie = Structures.Strucs[struc.IDXIE];
//			return ie.GetBlocks().Length > 0;
//		}
//		void AfterSelect()
//		{
//			SelectorUI.gameObject.SetActive(false);
//			var selected = SelectorUI.GetSelected();
//			if(selected.Count > 0)
//			{
//				m_SelectedStruc = Structures.StrucDict[selected[0]];
//			}
//			else
//			{
//				m_SelectedStruc = -1;
//			}

//			CrossBttn.interactable	= true;
//			CreateBttn.interactable = true;
//			CopyBttn.interactable	= true;
//			LoadBttn.interactable	= true;
//			SaveBttn.interactable	= CanStrucBeSaved();
//			ExitBttn.interactable	= true;

//			switch (m_CurrentFunc)
//			{
//				case Def.StrucEditMenuFunc.CopyStruc:
//					OnCopyEnd();
//					break;
//				case Def.StrucEditMenuFunc.LoadStruc:
//					OnLoadEnd();
//					break;
//				default:
//					break;
//			}
//		}
//		void UpdateStrucList()
//		{
//			for(int i = 0; i < m_Strucs.Count; ++i)
//			{
//				Sprite.Destroy(m_Strucs[i].Image);
//			}
//			m_Strucs.Clear();
//			var curStuc = EditUI.GetStruc();
//			for(int i = 0; i < Structures.Strucs.Count; ++i)
//			{
//				if (curStuc != null && curStuc.IDXIE == i)
//					continue;
//				var struc = Structures.Strucs[i];
//				if (struc == null || struc != null && struc.GetScreenshot() == null)
//					continue;
//				m_Strucs.Add(new CImageSelectorUI.ElementInfo()
//				{
//					Image = Sprite.Create(struc.GetScreenshot(), new Rect(0f, 0f, struc.GetScreenshot().width, struc.GetScreenshot().height),
//						new Vector2(0f, 0f), 100f, 0, SpriteMeshType.FullRect),
//					Name = struc.GetName()
//				});

//			}
//		}
//		#region Menu
//		void ShowSaveDialog()
//		{
//			CrossBttn.interactable = false;
//			CreateBttn.interactable = false;
//			CopyBttn.interactable = false;
//			LoadBttn.interactable = false;
//			SaveBttn.interactable = false;
//			ExitBttn.interactable = false;

//			SaveDontButton.interactable = m_CurrentFunc != Def.StrucEditMenuFunc.SaveStruc;
//			SavePanel.gameObject.SetActive(true);

//			var struciE = Structures.Strucs[EditUI.GetStruc().IDXIE];
//			var strucName = struciE.GetName();
//			var cleanName = GameUtils.RemoveStructureID(strucName);
//			if(cleanName.Length == 0)
//			{
//				SaveInvalidStrucNameText.enabled = true;
//				SaveApplyButton.interactable = false;
//			}
//			else
//			{
//				SaveInvalidStrucNameText.enabled = false;
//				SaveApplyButton.interactable = true;
//			}
//			SaveStrucIF.text = cleanName;
//			m_TempStrucName = strucName;
//		}
//		void StopSaveDialog(bool useAfterSaveFN)
//		{
//			SavePanel.gameObject.SetActive(false);
//			CrossBttn.interactable = true;
//			CreateBttn.interactable = true;
//			CopyBttn.interactable = true;
//			LoadBttn.interactable = true;
//			SaveBttn.interactable = CanStrucBeSaved();
//			ExitBttn.interactable = true;

//			if (!useAfterSaveFN)
//				return;

//			switch (m_CurrentFunc)
//			{
//				case Def.StrucEditMenuFunc.CreatingNew:
//					OnCreateNewEnd();
//					break;
//				case Def.StrucEditMenuFunc.CopyStruc:
//					OnSelectingStruc();
//					break;
//				case Def.StrucEditMenuFunc.LoadStruc:
//					OnSelectingStruc();
//					break;
//				case Def.StrucEditMenuFunc.COUNT:
//					break;
//			}
//		}
//		void OnCrossButton()
//		{
//			gameObject.SetActive(false);
//			EditUI.enabled = true;
//			if (!EditUI.CameraFreeBttn.interactable)
//				CameraManager.Mgr.FreeCam.enabled = true;
//		}
//		void OnCreateButton()
//		{
//			m_CurrentFunc = Def.StrucEditMenuFunc.CreatingNew;
//			if(CanStrucBeSaved())
//			//if(Structures.IsStrucModified(EditUI.GetStruc().IDXIE))
//			{
//				ShowSaveDialog();
//				return;
//			}
//			OnCreateNewEnd();
//		}
//		void OnCreateNewEnd()
//		{
//			EditUI.LoadStruc(-1);
//			OnCrossButton();
//		}
//		void OnCopyButton()
//		{
//			m_CurrentFunc = Def.StrucEditMenuFunc.CopyStruc;
//			if(CanStrucBeSaved())
//			//if (Structures.IsStrucModified(EditUI.GetStruc().IDXIE))
//			{
//				ShowSaveDialog();
//				return;
//			}
//			OnSelectingStruc();
//		}
//		void OnSelectingStruc()
//		{
//			CrossBttn.interactable = false;
//			CreateBttn.interactable = false;
//			CopyBttn.interactable = false;
//			LoadBttn.interactable = false;
//			SaveBttn.interactable = false;
//			ExitBttn.interactable = false;
//			UpdateStrucList();
//			SelectorUI.gameObject.SetActive(true);
//			SelectorUI.Init(m_Strucs, false, AfterSelect, Def.ImageSelectorPosition.Center);
//		}
//		void OnCopyEnd()
//		{
//			SelectorUI.gameObject.SetActive(false);
//			if (m_SelectedStruc >= 0)
//			{
//				EditUI.LoadStruc(m_SelectedStruc, true);
//				OnCrossButton();
//			}
//		}
//		void OnLoadButton()
//		{
//			m_CurrentFunc = Def.StrucEditMenuFunc.LoadStruc;
//			if(CanStrucBeSaved())
//			//if (Structures.IsStrucModified(EditUI.GetStruc().IDXIE))
//			{
//				ShowSaveDialog();
//				return;
//			}
//			OnSelectingStruc();
//		}
//		void OnLoadEnd()
//		{
//			SelectorUI.gameObject.SetActive(false);
//			if (m_SelectedStruc >= 0)
//			{
//				EditUI.LoadStruc(m_SelectedStruc, false);
//				OnCrossButton();
//			}
//		}
//		void OnSaveButton()
//		{
//			m_CurrentFunc = Def.StrucEditMenuFunc.SaveStruc;
//			ShowSaveDialog();
//		}
//		void OnSaveEnd()
//		{
//			OnExitButton();
//		}
//		void OnExitButton()
//		{
//			EditUI.enabled = true;
//			EditUI.ExitUI();
//		}
//		#endregion
//		#region SaveDialog
//		void OnStrucNameIF(string value)
//		{
//			value = GameUtils.RemoveStructureID(value);
//			if(value.Length == 0)
//			{
//				SaveApplyButton.interactable = false;
//				SaveInvalidStrucNameText.enabled = true;
//			}
//			else
//			{
//				SaveInvalidStrucNameText.enabled = false;
//				SaveApplyButton.interactable = true;
//			}
//			int id = 0;
//			bool isNameValid(string nameToTest)
//			{
//				if(Structures.StrucDict.ContainsKey(nameToTest))
//				{
//					return Structures.StrucDict[nameToTest] == EditUI.GetStruc().IDXIE;
//				}
//				return true;
//			}
//			string testName = value + "_" + id.ToString().PadLeft(3, '0');
//			while(!isNameValid(testName))
//			{
//				++id;
//				testName = value + "_" + id.ToString().PadLeft(3, '0');
//			}
//			m_TempStrucName = testName;
//		}
//		void OnSaveDont()
//		{
//			var idxie = EditUI.GetStruc().IDXIE;
//			StopSaveDialog(true);
//			var ie = Structures.Strucs[idxie];
//			if (Structures.StrucDict.ContainsKey(ie.GetName()))
//				Structures.StrucDict.Remove(ie.GetName());
//			if (!ie.IsFromFile())
//			{
//				Structures.Strucs.RemoveAt(idxie);
//				return;
//			}
//			ie = IE.V3.StructureIE.FromFile(ie.GetFilePath());
//			ie.StructureID = (ushort)idxie;
//			Structures.Strucs[idxie] = ie;
//			Structures.StrucDict.Add(ie.GetName(), idxie);
//		}
//		void OnSaveCancel()
//		{
//			StopSaveDialog(false);
//		}
//		void ScreenshootPrepareTask()
//		{
//			gameObject.SetActive(false);
//			EditUI.enabled = false;
//			EditUI.ToggleVisibility(true, true);
//			EditUI.gameObject.SetActive(false);
//			EditUI.OnCamGameButton();
//			EditUI.OnCamEditButton();
//			//CameraManager.Mgr.CameraType = ECameraType.INGAME;
//			//CameraManager.Mgr.CameraType = ECameraType.EDITOR;
//			CameraManager.Mgr.enabled = false;
//		}
//		IEnumerator ScreenshootTask()
//		{
//			yield return new WaitForEndOfFrame();

//			//var tex = ScreenCapture.CaptureScreenshotAsTexture();
//			if(ScreenRNDTex == null || 
//				(ScreenRNDTex != null && (ScreenRNDTex.width != Screen.width || ScreenRNDTex.height != Screen.height)))
//			{
//				if (ScreenRNDTex != null)
//					RenderTexture.Destroy(ScreenRNDTex);
//				ScreenRNDTex = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
//			}
//			var oldRT = RenderTexture.active;
//			RenderTexture.active = ScreenRNDTex;
//			ScreenCapture.CaptureScreenshotIntoRenderTexture(ScreenRNDTex);
//			RenderTexture.active = oldRT;

//			int minSize = Screen.width > Screen.height ? Screen.height : Screen.width;
//			int nX = Screen.width / 2 - minSize / 2;
//			int nY = Screen.height / 2 - minSize / 2; 

//			var tex = new Texture2D(minSize, minSize, TextureFormat.RGBA32, false);
//			tex.ReadPixels(new Rect(nX, nY, minSize, minSize), 0, 0);
//			tex.Apply();
			

//			var scaledTex = GameUtils.ResizeTexture(tex, GameUtils.ImageFilterMode.Nearest, 
//				128f / tex.height, true);
			
//			var ie = Structures.Strucs[EditUI.GetStruc().IDXIE];
//			var oScreenshot = ie.GetScreenshot();

//			ie.SetScreenshot(scaledTex);

//			// Clean-up
//			if (oScreenshot != null)
//				Texture.Destroy(oScreenshot);
//			Texture.Destroy(tex);

//			BackgroundQueue.Mgr.ScheduleOnIdleTask(AfterScreenshootTask);
//		}
//		void AfterScreenshootTask()
//		{
//			CameraManager.Mgr.enabled = true;
//			EditUI.ToggleVisibility(true, false);
//			var ie = Structures.Strucs[EditUI.GetStruc().IDXIE];
//			Structures.SaveStruc(ie.StructureID);
//			EditUI.gameObject.SetActive(true);
//			EditUI.enabled = false;
//			gameObject.SetActive(true);
//			StopSaveDialog(true);
//		}
//		void OnSaveApply()
//		{
//			var path = Application.streamingAssetsPath + "/Structures/";
//			var ie = Structures.Strucs[EditUI.GetStruc().IDXIE];
//			if(ie.IsFromFile())
//			{
//				var file = new FileInfo(ie.GetFilePath());
//				if (file.Exists)
//					file.Delete();
//			}
//			var oldName = ie.GetName();
//			//var file = new FileInfo(path + oldName + ".STRC");
//			//if (file.Exists)
//			//	file.Delete();
//			ie.SetName(m_TempStrucName);
//			var sLayers = EditUI.GetStruc().GetLayers();
//			for(int i = 0; i < sLayers.Length; ++i)
//			{
//				var sLayer = sLayers[i];
//				if(!sLayer.IsValid())
//				{
//					ie.RemoveLayer(i + 1);
//				}
//				else
//				{
//					var layerIE = new IE.V3.LayerIE();
//					sLayer.ToLayerIE(ref layerIE);
//					ie.SetLayer(i + 1, layerIE);
//				}
//			}

//			if(oldName != m_TempStrucName)
//			{
//				if (Structures.StrucDict.ContainsKey(oldName))
//					Structures.StrucDict.Remove(oldName);
//				Structures.StrucDict.Add(m_TempStrucName, EditUI.GetStruc().IDXIE);
//			}

//			// Screenshoot
//			BackgroundQueue.Mgr.ScheduleTask(new List<BackgroundQueue.Task>()
//			{
//				new BackgroundQueue.Task()
//				{
//					Cost = BackgroundQueue.DefaultMaxCostPerFrame,
//					FN = ScreenshootPrepareTask
//				},
//				new BackgroundQueue.Task()
//				{
//					Cost = BackgroundQueue.DefaultMaxCostPerFrame,
//					FN = () => { /* frame wait*/ }
//				},
//				new BackgroundQueue.Task()
//				{
//					Cost = BackgroundQueue.DefaultMaxCostPerFrame,
//					FN = () => { BackgroundQueue.Mgr.StartCoroutine(ScreenshootTask()); }
//				},
//			});
//		}
//		#endregion
//		private void OnEnable()
//		{
//			var struc = EditUI.GetStruc();
//			if (struc == null)
//				return;
//			SaveBttn.interactable = CanStrucBeSaved();
//			var strucName = Structures.Strucs[struc.IDXIE].GetName();
//			SaveStrucIF.text = GameUtils.RemoveStructureID(strucName);
//		}
//		private void Awake()
//		{
//			CrossBttn.onClick.AddListener(OnCrossButton);
//			CreateBttn.onClick.AddListener(OnCreateButton);
//			CopyBttn.onClick.AddListener(OnCopyButton);
//			LoadBttn.onClick.AddListener(OnLoadButton);
//			SaveBttn.onClick.AddListener(OnSaveButton);
//			ExitBttn.onClick.AddListener(OnExitButton);
//			m_Strucs = new List<CImageSelectorUI.ElementInfo>();
//			SaveStrucIF.onEndEdit.AddListener(OnStrucNameIF);
//			SaveCancelButton.onClick.AddListener(OnSaveCancel);
//			SaveDontButton.onClick.AddListener(OnSaveDont);
//			SaveApplyButton.onClick.AddListener(OnSaveApply);
//		}
//		void Update()
//		{
//			if (Input.GetKeyDown(KeyCode.Escape))
//			{
//				if(SelectorUI.isActiveAndEnabled)
//				{
//					SelectorUI.OnSelectionEnd();
//				}
//				else if(SavePanel.isActiveAndEnabled)
//				{
//					OnSaveCancel();
//				}
//				else
//				{
//					OnCrossButton();
//				}
//			}
//		}
//		public void Init()
//		{

//		}
//	}
//}