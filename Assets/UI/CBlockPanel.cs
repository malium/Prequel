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

namespace Assets.UI
{
	public class CBlockPanel : MonoBehaviour
	{
		public UnityEngine.UI.Button HideButton;

		public TMPro.TMP_Text CurrentRotationValue;
		public UnityEngine.UI.Image CurrentMaterialImage;
		public TMPro.TMP_Text CurrentLengthValue;
		public TMPro.TMP_Text CurrentHeightValue;
		public TMPro.TMP_Text CurrentMicroHeightValue;
		public TMPro.TMP_Text CurrentLayerValue;
		public TMPro.TMP_Text CurrentBlockTypeValue;
		public TMPro.TMP_Text CurrentVoidValue;
		public TMPro.TMP_Text CurrentStairValue;
		public TMPro.TMP_Text CurrentLockValue;
		public UnityEngine.UI.Image CurrentPropImage;
		public UnityEngine.UI.Image CurrentMonsterImage;
		public TMPro.TMP_Text CurrentStackedBelow;
		public TMPro.TMP_Text CurrentStackedAbove;

		public TMPro.TMP_Text IEIDXIEText;
		public UnityEngine.UI.Toggle IERotationToggle;
		public TMPro.TMP_Text IERotationValue;
		public UnityEngine.UI.Toggle IEMaterialToggle;
		public UnityEngine.UI.Image IEMaterialImage;
		public UnityEngine.UI.Toggle IELengthToggle;
		public TMPro.TMP_Text IELengthValue;
		public TMPro.TMP_Text IEHeightValue;
		public TMPro.TMP_Text IELayerValue;
		public TMPro.TMP_Text IEBlockTypeValue;
		public TMPro.TMP_Text IEVoidValue;
		public TMPro.TMP_Text IEStairValue;
		public UnityEngine.UI.Toggle IEPropToggle;
		public UnityEngine.UI.Image IEPropImage;
		public UnityEngine.UI.Image IEMonsterImage;
		public TMPro.TMP_Text IEStackedBelow;
		public TMPro.TMP_Text IEStackedAbove;

		Action m_OnHideButton;
		
		private void Awake()
		{
			HideButton.onClick.AddListener(OnHideButton);
			m_OnHideButton = () => { };
		}
		public void Init(Action onHideButton = null)
		{
			m_OnHideButton = onHideButton;
			if (m_OnHideButton == null)
				m_OnHideButton = () => { };
		}
		public void SetBlock(CBlockEdit block, int structIDXIE)
		{
			var bIDXIE = block.GetIDXIE();
			if (bIDXIE < 0)
				return;
			var strucIE = Structures.Strucs[structIDXIE];
			var blockIE = strucIE.GetBlocks()[bIDXIE];

			IEIDXIEText.text = bIDXIE.ToString();
			IELengthToggle.isOn = blockIE.GetFlag(IE.V4.BlockIE.Flag.Length);
			IEMaterialToggle.isOn = blockIE.GetFlag(IE.V4.BlockIE.Flag.MaterialFamily);
			IERotationToggle.isOn = blockIE.GetFlag(IE.V4.BlockIE.Flag.Rotation);
			IEPropToggle.isOn = blockIE.GetFlag(IE.V4.BlockIE.Flag.Prop);

			CurrentStackedBelow.text = block.GetStackedBlocksIdx()[0].ToString();
			CurrentStackedAbove.text = block.GetStackedBlocksIdx()[1].ToString();
			IEStackedBelow.text = blockIE.StackedIdx[0].ToString();
			IEStackedAbove.text = blockIE.StackedIdx[1].ToString();

			void SetMaterialSprite(UnityEngine.UI.Image image, string materialFamily)
			{
				if (image.sprite != null)
				{
					Sprite.Destroy(image.sprite);
					image.sprite = null;
				}

				var family = BlockMaterial.GetMaterialFamily(materialFamily);
				if(family != null)
				{
					var texture = BlockMaterial.GetTextureFromMaterial(family.NormalMaterials[0].TopPart.Mat);
					image.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero, 100f, 0, SpriteMeshType.FullRect);
				}
			}
			void SetPropSprite(UnityEngine.UI.Image image, string propFamily)
			{
				image.sprite = null;

				var family = Props.GetPropFamily(propFamily);
				if(family != null)
				{
					image.sprite = family.Props[0].PropSprite;
				}
			}
			void SetMonsterSprite(UnityEngine.UI.Image image, string monsterFamily)
			{
				image.sprite = null;

				var family = Monsters.GetMonsterFamily(monsterFamily);
				if(family != null)
				{
					image.sprite = family.Frames[0];
				}
			}

			CurrentRotationValue.text = block.GetRotation().ToString();
			IERotationValue.text = blockIE.Rotation == Def.RotationState.COUNT ? "RANDOM" : blockIE.Rotation.ToString();

			CurrentLengthValue.text = block.GetLength().ToString();
			var len = blockIE.GetLength();
			IELengthValue.text = len.ToString();

			CurrentHeightValue.text = block.GetHeight().ToString();
			var height = blockIE.GetHeight();
			IEHeightValue.text = height.ToString();

			CurrentMicroHeightValue.text = block.GetMicroHeight().ToString();

			CurrentLayerValue.text = block.GetLayer().ToString();
			IELayerValue.text = blockIE.Layer.ToString();

			CurrentBlockTypeValue.text = block.GetBlockType().ToString();
			IEBlockTypeValue.text = blockIE.BlockType.ToString();

			CurrentVoidValue.text = block.GetVoidState().ToString();
			IEVoidValue.text = blockIE.BlockVoid.ToString();

			CurrentStairValue.text = block.GetStairState().ToString();
			IEStairValue.text = blockIE.Stair.ToString();

			CurrentLockValue.text = block.GetLockState().ToString();

			string materialFamilyName = "";
			if (block.GetMaterialFamily() != null)
				materialFamilyName = block.GetMaterialFamily().FamilyInfo.FamilyName;
			SetMaterialSprite(CurrentMaterialImage, materialFamilyName);
			SetMaterialSprite(IEMaterialImage, blockIE.MaterialFamily);
			SetPropSprite(CurrentPropImage, block.GetProp() == null ? "" : block.GetProp().GetInfo().FamilyName);
			SetPropSprite(IEPropImage, blockIE.PropFamily);
			SetMonsterSprite(CurrentMonsterImage, block.GetMonster() == null ? "" : block.GetMonster().GetFamily().Name);
		}
		void OnHideButton()
		{
			m_OnHideButton();
		}
	}
}
