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
	public class MainMenuUI : MonoBehaviour
	{
		//public StrucEditUI StrucEdit;
		public TestArenaUI TestArena;
		//public SpriteTestUI SpriteTest;
		public WorldGenUI WorldGen;
		public GardenUI GardenUI;
		public StructureEditUI StructureEdit;
		//public BossTestUI BossUI;

		public void OnPlay()
		{

		}

		//public void OnBiome()
		//{

		//}

		public void OnTestArena()
		{
			TestArena.gameObject.SetActive(true);
			gameObject.SetActive(false);
			TestArena.Init();
		}

		//public void OnSpriteTest()
		//{
		//	SpriteTest.gameObject.SetActive(true);
		//	gameObject.SetActive(false);
		//	SpriteTest.Init();
		//}

		//public void OnStruct()
		//{
		//	StrucEdit.gameObject.SetActive(true);
		//	gameObject.SetActive(false);
		//	StrucEdit.Init();
		//}
		public void OnStructure()
		{
			StructureEdit.gameObject.SetActive(true);
			gameObject.SetActive(false);
			StructureEdit.Init();
		}
		public void OnWorldGen()
		{
			WorldGen.gameObject.SetActive(true);
			gameObject.SetActive(false);
			WorldGen.Init();
		}
		public void OnEldenGarden()
		{
			GardenUI.gameObject.SetActive(true);
			gameObject.SetActive(false);
			GardenUI.Init();
		}
		//public void OnBoss()
		//{
		//	BossUI.gameObject.SetActive(true);
		//	gameObject.SetActive(false);
		//	BossUI.Init();
		//}

		//public void OnBuilding()
		//{

		//}

		public void OnSettings(GameObject SettingsUI)
		{
			//Debug.Log("ABRIR SETTINGS");
			SettingsUI.SetActive(true);
		}

		public void OnExit()
		{
			Application.Quit();
		}



		public void OnContact(string URL)
		{
			if (URL == null) return; //URL = "https://www.google.com";
			if ( !(URL.StartsWith("http://") || URL.StartsWith("https://")) ) URL = "http://" + URL;
			Application.OpenURL(URL);
		}
	}
}
