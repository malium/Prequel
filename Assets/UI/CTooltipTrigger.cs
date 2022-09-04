/***********************************************************************************
*   Copyright 2022 Marcos Sánchez Torrent.                                         *
*   All Rights Reserved.                                                           *
***********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.UI
{
	[RequireComponent(typeof(RectTransform))]
	public class CTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
	{
		static CTooltip DefaultTooltip;
		static Dictionary<int, CTooltip> Cache;
		[Multiline()]
		public string Text;
		public float TextSize = 25;
		public Vector4 TextMargin = new Vector4(10f, 10f, 10f, 10f);
		public Vector2 Offset = new Vector2(10f, -10f);
		public CTooltip CustomTooltip;

		CTooltip m_Tooltip;

		private void Awake()
		{
			if (DefaultTooltip == null)
				DefaultTooltip = Resources.Load<CTooltip>("UI/DefaultTooltip");
			if (Cache == null)
				Cache = new Dictionary<int, CTooltip>(1);

			CTooltip tooltipToInstantiate;
			if (CustomTooltip != null)
			{
				tooltipToInstantiate = CustomTooltip;
				Cache.TryGetValue(CustomTooltip.GetInstanceID(), out m_Tooltip);
			}
			else if (DefaultTooltip != null)
			{
				tooltipToInstantiate = DefaultTooltip;
				Cache.TryGetValue(DefaultTooltip.GetInstanceID(), out m_Tooltip);
			}
			else
			{
				throw new Exception("Trying to Awake a CTooltipTrigger but there's not CTooltip available to use.");
			}

			if (m_Tooltip == null)
			{
				m_Tooltip = Instantiate(tooltipToInstantiate);
				m_Tooltip.name = tooltipToInstantiate.name;

				var canvas = CameraManager.Mgr.Canvas;
				m_Tooltip.transform.SetParent(canvas.transform);

				if (Cache.ContainsKey(tooltipToInstantiate.GetInstanceID()))
					Cache[tooltipToInstantiate.GetInstanceID()] = m_Tooltip;
				else
					Cache.Add(tooltipToInstantiate.GetInstanceID(), m_Tooltip);
			}

			m_Tooltip.gameObject.SetActive(false);
		}
		public CTooltip GetTooltip() => m_Tooltip;
		IEnumerator UpdateNextFrame()
		{
			yield return null;

			m_Tooltip.ChildScaler.UpdateTransform();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (m_Tooltip == null)
				return;

			m_Tooltip.Text.text = Text;
			if(Text.Length == 0)
			{
				m_Tooltip.gameObject.SetActive(false);
				return;
			}
			m_Tooltip.Text.fontSize = TextSize;
			m_Tooltip.Text.margin = TextMargin;
			m_Tooltip.gameObject.SetActive(true);
			StartCoroutine(UpdateNextFrame());
			m_Tooltip.transform.position = new Vector3(eventData.position.x + Offset.x, eventData.position.y + Offset.y, m_Tooltip.transform.position.z);
		}
		public void OnPointerExit(PointerEventData eventData)
		{
			if (m_Tooltip == null)
				return;

			m_Tooltip.gameObject.SetActive(false);
		}

		public void OnPointerMove(PointerEventData eventData)
		{
			if (m_Tooltip == null)
				return;

			m_Tooltip.transform.position = new Vector3(eventData.position.x + Offset.x, eventData.position.y + Offset.y, m_Tooltip.transform.position.z);
		}
	}
}
