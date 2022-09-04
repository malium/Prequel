/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.UI
{
    public class LoadingUI : MonoBehaviour
    {
        public TMPro.TMP_Text LoadingText;
        public MainMenuUI MainMenu;

        static readonly Action<LoadingUI>[] Actions = new Action<LoadingUI>[]
        {
        (LoadingUI lui) => { lui.LoadAssets(); },
        (LoadingUI lui) => { lui.PrepareAssets(); },
        };
        static int ActionIdx = 0;

        private void Start()
        {
            InitLoading();
        }

        public void InitLoading()
        {
            AssetLoader.Init();
        }

        void OnLoadEnd()
        {
            // Stop and unload the loading go
            gameObject.SetActive(false);
            MainMenu.gameObject.SetActive(true);
        }

        void LoadAssets()
        {
            LoadingText.text = $"Loading Assets ({AssetLoader.GetCurrentLoadFunctionIdx()}/{AssetLoader.GetLoadFunctionCount()})...";
            if(AssetLoader.LoadNext())
            {
                ActionIdx = 1;
            }
        }

        void PrepareAssets()
        {
            LoadingText.text = $"Preparing Assets ({AssetLoader.GetCurrentPrepareFunctionIdx()}/{AssetLoader.GetPrepareFunctionCount()})...";
            if (AssetLoader.PrepareNext())
            {
                OnLoadEnd();
            }
        }

        private void Update()
        {
            Actions[ActionIdx](this);
        }
    }
}