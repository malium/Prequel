/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Assets.UI
{
	public class ExceptionUI : MonoBehaviour
	{
        public UnityEngine.UI.Image ExceptionPanel;
        public UnityEngine.UI.Text ExceptionTitle;
        public UnityEngine.UI.Text ExceptionDescription;

		readonly static string ExecutablePath = AppDomain.CurrentDomain.BaseDirectory;
		void HandleException(string condition, string stackTrace, LogType type)
		{
			if (type != LogType.Exception)
				return;
			if (!Application.isEditor)
			{
				ExceptionTitle.text = condition;
				ExceptionDescription.text = stackTrace;
				ExceptionPanel.gameObject.SetActive(true);
				condition += '\n';
				var time = DateTime.Now;
				var fs = File.Create(ExecutablePath + $"/{time.Year}_{time.Month}_{time.Day}-{time.Hour}.{time.Minute}.{time.Second}.except");
				var fData = new byte[condition.Length * 2 + stackTrace.Length * 2];
				int lastIdx = 0;
				for (int i = 0; i < condition.Length; ++i)
				{
					var bytes = BitConverter.GetBytes(condition[i]);
					for (int j = 0; j < bytes.Length; ++j)
					{
						fData[lastIdx++] = bytes[j];
					}
				}
				for (int i = 0; i < stackTrace.Length; ++i)
				{
					var bytes = BitConverter.GetBytes(stackTrace[i]);
					for (int j = 0; j < bytes.Length; ++j)
					{
						fData[lastIdx++] = bytes[j];
					}
				}
				fs.Write(fData, 0, fData.Length);
				fs.Close();
			}
		}

		private void Awake()
		{
			Application.logMessageReceived += HandleException;
		}
        public void OnExit()
		{
			Application.Quit();
		}
        public void OnContinue()
		{
			ExceptionPanel.gameObject.SetActive(false);
		}
	}
}