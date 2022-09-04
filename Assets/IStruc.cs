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

namespace Assets
{
	public abstract class IStruc : MonoBehaviour
	{
		public abstract int GetWidth();
		public abstract int GetHeight();
		public abstract CPilar[] GetPilars();
		public abstract RectInt GetBounds();
		public abstract Rect GetBoundsF();
		//public abstract List<LivingEntity> GetLivingEntities();
		public abstract List<AI.CLivingEntity> GetLES();
		//public abstract StrucQT GetQT();
		//public abstract void SetQT(StrucQT qt);

		public int PilarIDFromVPos(Vector2Int vPos)
		{
			if (vPos.x >= GetWidth() || vPos.y >= GetHeight()
				|| vPos.x < 0 || vPos.y < 0)
			{
				Debug.LogWarning($"Out of bounds - !PilarIDFromVPos {vPos}.");
				return 0;
			}
			int pilarID = vPos.y * GetWidth() + vPos.x;
			return pilarID;
		}

		public Vector2Int VPosFromPilarID(int pilarID)
		{
			if (pilarID < 0 || pilarID >= GetPilars().Length)
			{
				Debug.LogWarning($"Out of bounds - !VPosFromPilarID {pilarID}.");
				return Vector2Int.zero;
			}
			var vPos = new Vector2Int(
				pilarID % GetWidth(),
				Mathf.FloorToInt(pilarID / GetHeight()));
			if (vPos.x < 0 || vPos.x >= GetWidth() ||
				vPos.y < 0 || vPos.y >= GetHeight())
			{
				Debug.LogWarning($"Something went wrong - !VPosFromPilarID {vPos} from {pilarID}");
				return Vector2Int.zero;
			}
			return vPos;
		}

		public int PilarIDFromWPos(Vector2 wPos)
		{
			const float blockWidth = 1f + Def.BlockSeparation;
			const float invBlockWidth = 1f / blockWidth;
			float strucWidth = GetWidth() * blockWidth;
			float strucHeight = GetHeight() * blockWidth;

			if (wPos.x < 0f || wPos.x > strucWidth || wPos.y < 0f || wPos.y > strucHeight)
			{
				Debug.LogWarning($"Out of bounds - !PilarIDFromWPos {wPos}.");
				return 0;
			}

			var nPos = wPos * invBlockWidth;
			return PilarIDFromVPos(new Vector2Int(Mathf.FloorToInt(nPos.x), Mathf.FloorToInt(nPos.y)));
		}

		public Vector2 WPosFromPilarID(int pilarID)
		{
			const float blockWidth = 1f + Def.BlockSeparation;

			if (pilarID < 0 || pilarID >= GetPilars().Length)
			{
				Debug.LogWarning($"Out of bounds - !PilarIDWPos {pilarID}.");
				return Vector2.zero;
			}

			var vPos = VPosFromPilarID(pilarID);
			return new Vector2(vPos.x * blockWidth, vPos.y * blockWidth);
		}

		public void DestroyStruc(bool preserveEntities, bool instant = false)
		{
			for (int i = 0; i < GetPilars().Length; ++i)
			{
				var pilar = GetPilars()[i];
				if (pilar == null)
					continue;
				pilar.DestroyPilar(preserveEntities, instant);
			}
			//if(this is CStruc)
			//	Debug.LogWarning("CStruc not removed from other places!");
			for(int i = 0; i < GetLES().Count; ++i)
			{
				var le = GetLES()[i];
				le.ReceiveDamage(null, Def.DamageType.UNAVOIDABLE, le.GetCurrentHealth());
			}

			GameUtils.DeleteGameobject(gameObject, instant);
		}
	}
}
