/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.UI
{
	public class GardenUI : MonoBehaviour
	{
		public MainMenuUI MainMenu;

		public void Init()
		{

		}

		public void OnMenuButton()
		{
			MainMenu.gameObject.SetActive(true);
			gameObject.SetActive(false);
		}
	}
}