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
	public static class Blocks
	{
		enum MidBottomType
		{
			WideBottom,
			WideMid,
			NormalBottom,
			NormalMid,

			COUNT
		}
		const int MidBottomTypeCount = (int)MidBottomType.COUNT;

		public static MeshCollider RampCollider;

		public const float MinNormalHeight = -0.1f;
		public const float MaxNormalHeight = -3.4f;
		const float UVMinNormalHeight = 0.02044f;
		const float UVMaxNormalHeight = 0.89631f;

		public const float MinWideHeight = -0.1f;
		public const float MaxWideHeight = -2.5f;
		static readonly Vector4 UVWideHeight = new Vector4(0.01557f, 0.499875f, 0.51557f, 0.999875f);

		static Mesh[] TopMeshes;
		static Dictionary<float, Mesh>[] MidMeshes;
		static Mesh[] StairMeshes;
		public static void SetBlock(GameObject block, Def.BlockType type, Def.BlockMeshType meshType,
			float blockLenght = 0f, Def.StairType stairType = Def.StairType.NORMAL)
		{
			MeshRenderer meshRenderer;
			MeshFilter meshFilter;
			meshRenderer = block.GetComponent<MeshRenderer>();
			meshFilter = block.GetComponent<MeshFilter>();
			if (meshRenderer == null)
				meshRenderer = block.AddComponent<MeshRenderer>();
			if (meshFilter == null)
				meshFilter = block.AddComponent<MeshFilter>();
			
			switch (meshType)
			{
				case Def.BlockMeshType.TOP:
					switch (type)
					{
						case Def.BlockType.NORMAL:
							meshFilter.mesh = TopMeshes[0];
							break;
						case Def.BlockType.STAIRS:
							meshFilter.mesh = StairMeshes[(int)stairType];
							break;
						case Def.BlockType.WIDE:
							meshFilter.mesh = TopMeshes[1];
							break;
						default:
							Debug.LogWarning("Trying to create a block with an unhandled BlockType " + type.ToString());
							break;
					}
					break;
				case Def.BlockMeshType.MID:
					float length;
					int idx;
					if(type == Def.BlockType.WIDE)
					{
						length = blockLenght == 0f ? MaxWideHeight : blockLenght;
						idx = 1;
					}
					else
					{
						length = blockLenght == 0f ? MaxNormalHeight : blockLenght;
						idx = 0;
					}
					if (length > 0f)
						length = -length;
					try
					{
						meshFilter.mesh = MidMeshes[idx][length];
					}
					catch(Exception e)
					{ 
						Debug.Log(e); 
					}
					break;
				default:
					Debug.LogWarning("Trying to create a block with the wrong Def.BlockMeshType " + meshType.ToString());
					break;
			}

			meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
			meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
			meshRenderer.receiveShadows = true;
			meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
		}
		struct UVY
		{
			public float Original;
			public float ToChange;
		};
		static void UVYResize(ref Vector2[] uvs, List<UVY> uvy)
		{
			for (int i = 0; i < uvs.Length; ++i)
			{
				for (int j = 0; j < uvy.Count; ++j)
				{
					if (GameUtils.IsNearlyEqual(uvs[i].y, uvy[j].Original))
					{
						uvs[i].y = uvy[j].ToChange;
						break;
					}
				}
			}
		}
		static void PrepareMidNormal(Mesh midDefault, Mesh botDefault)
		{
			float currentHeight = MaxNormalHeight;

			var firstMesh = new Mesh()
			{
				name = "NORMAL_" + currentHeight.ToString()
			};
			var combines = new CombineInstance[2]
			{
				new CombineInstance()
				{
					mesh = midDefault,
					subMeshIndex = 0
				},
				new CombineInstance()
				{
					mesh = botDefault,
					subMeshIndex = 0
				}
			};
			firstMesh.CombineMeshes(combines, true, false, false);
			firstMesh.Optimize();
			firstMesh.RecalculateBounds();
			firstMesh.UploadMeshData(false);
			MidMeshes[0].Add(currentHeight, firstMesh);
			currentHeight = Mathf.CeilToInt(currentHeight);

			while(currentHeight <= -0.5f)
			{
				var currentMesh = new Mesh()
				{
					name = "NORMAL_" + currentHeight.ToString()
				};
				var midMesh = new Mesh();
				var botMesh = new Mesh();
				GameUtils.CopyMesh(midDefault, ref midMesh);
				GameUtils.CopyMesh(botDefault, ref botMesh);

				var uvs = midMesh.uv;
				var lengthPct = 1 - ((Mathf.Abs(currentHeight) - Mathf.Abs(MinNormalHeight)) / (Mathf.Abs(MaxNormalHeight) - Mathf.Abs(MinNormalHeight)));

				var uvLength = lengthPct * (UVMaxNormalHeight - UVMinNormalHeight) + UVMinNormalHeight;

				var uvys = new List<UVY>()
				{
					new UVY()
					{
						Original = UVMinNormalHeight,
						ToChange = uvLength
					}
				};
				UVYResize(ref uvs, uvys);

				var midVertices = midMesh.vertices;
				var botVertices = botMesh.vertices;

				for (int i = 0; i < midVertices.Length; ++i)
				{
					if (midVertices[i].y != MinNormalHeight)
					{
						var vertex = midMesh.vertices[i];
						vertex.y = currentHeight;
						midVertices[i] = vertex;
					}
				}

				for (int i = 0; i < botVertices.Length; ++i)
				{
					var vertex = botVertices[i];
					vertex.y += (currentHeight - MaxNormalHeight);
					botVertices[i] = vertex;
				}
				botMesh.vertices = botVertices;
				midMesh.vertices = midVertices;
				midMesh.uv = uvs;

				//combines = new CombineInstance[2]; // Mid and Bottom
				combines[0].mesh = midMesh;
				combines[0].subMeshIndex = 0;
				combines[1].mesh = botMesh;
				combines[1].subMeshIndex = 0;

				currentMesh.CombineMeshes(combines, true, false, false);
				GameObject.Destroy(midMesh);
				GameObject.Destroy(botMesh);

				currentMesh.Optimize();
				currentMesh.RecalculateBounds();
				currentMesh.UploadMeshData(false);
				MidMeshes[0].Add(currentHeight, currentMesh);

				currentHeight += 0.5f;
			}
		}
		static void PrepareMidWide(Mesh midDefault, Mesh botDefault)
		{
			var uvHeight = UVWideHeight;

			float currentHeight = MaxWideHeight;

			var firstMesh = new Mesh()
			{
				name = "WIDE_" + currentHeight.ToString()
			};
			var combines = new CombineInstance[2]
			{
				new CombineInstance()
				{
					mesh = midDefault,
					subMeshIndex = 0
				},
				new CombineInstance()
				{
					mesh = botDefault,
					subMeshIndex = 0
				}
			}; firstMesh.CombineMeshes(combines, true, false, false);
			firstMesh.Optimize();
			firstMesh.RecalculateBounds();
			firstMesh.UploadMeshData(false);

			MidMeshes[1].Add(currentHeight, firstMesh);
			currentHeight += 0.5f;

			while(currentHeight <= -0.5f)
			{
				var currentMesh = new Mesh()
				{
					name = "WIDE_" + currentHeight.ToString()
				};
				var midMesh = new Mesh();
				var botMesh = new Mesh();
				GameUtils.CopyMesh(midDefault, ref midMesh);
				GameUtils.CopyMesh(botDefault, ref botMesh);

				var uvs = midMesh.uv;
				var lengthPct = 1 - ((Mathf.Abs(currentHeight) - Mathf.Abs(MinWideHeight)) / (Mathf.Abs(MaxWideHeight) - Mathf.Abs(MinWideHeight)));

				var uvLength0 = lengthPct * (uvHeight.y - uvHeight.x) + uvHeight.x;
				var uvLength1 = lengthPct * (uvHeight.w - uvHeight.z) + uvHeight.z;

				var uvys = new List<UVY>()
				{
					new UVY()
					{
						Original = uvHeight.x,
						ToChange = uvLength0
					},
					new UVY()
					{
						Original = uvHeight.z,
						ToChange = uvLength1
					}
				};
				UVYResize(ref uvs, uvys);

				var midVertices = midMesh.vertices;
				var botVertices = botMesh.vertices;

				for (int i = 0; i < midVertices.Length; ++i)
				{
					if (midVertices[i].y != MinWideHeight)
					{
						var vertex = midMesh.vertices[i];
						vertex.y = currentHeight;
						midVertices[i] = vertex;
					}
				}

				for (int i = 0; i < botVertices.Length; ++i)
				{
					var vertex = botVertices[i];
					vertex.y += (currentHeight - MaxWideHeight);
					botVertices[i] = vertex;
				}
				botMesh.vertices = botVertices;
				midMesh.vertices = midVertices;
				midMesh.uv = uvs;

				//combines = new CombineInstance[2]; // Mid and Bottom
				combines[0].mesh = midMesh;
				combines[0].subMeshIndex = 0;
				combines[1].mesh = botMesh;
				combines[1].subMeshIndex = 0;

				currentMesh.CombineMeshes(combines, true, false, false);
				GameObject.Destroy(midMesh);
				GameObject.Destroy(botMesh);

				currentMesh.Optimize();
				currentMesh.RecalculateBounds();
				currentMesh.UploadMeshData(false);
				MidMeshes[1].Add(currentHeight, currentMesh);

				currentHeight += 0.5f;
			}
		}
		static void PrepareMidMeshes(Mesh[] midBot)
		{
			MidMeshes = new Dictionary<float, Mesh>[2]
			{
				new Dictionary<float, Mesh>(7),
				new Dictionary<float, Mesh>(5)
			};
			PrepareMidNormal(midBot[(int)MidBottomType.NormalMid], midBot[(int)MidBottomType.NormalBottom]);
			PrepareMidWide(midBot[(int)MidBottomType.WideMid], midBot[(int)MidBottomType.WideBottom]);
		}
		public static void Prepare()
		{
			var blocks = AssetLoader.Blocks;
			var midBot = new Mesh[MidBottomTypeCount];
			StairMeshes = new Mesh[Def.StairTypeCount];
			TopMeshes = new Mesh[2]; // Normal and Wide
			var rampColliderGO = Resources.Load("RampCollider") as GameObject;
			if (rampColliderGO != null)
				RampCollider = rampColliderGO.GetComponent<MeshCollider>();

			void Addblock(MeshFilter b, MidBottomType type)
			{
				if(midBot[(int)type] != null)
				{
					Debug.LogWarning("Block with name '" + b.name + "' has repeated type " + type.ToString() + ".");
					return;
				}
				midBot[(int)type] = b.sharedMesh;
			}
			for(int i = 0; i < blocks.Length; ++i)
			{
				var b = blocks[i];
				var split = b.name.ToLower().Split('_');
				if(split.Length != 2 || split[0] != "geo")
				{
					Debug.LogWarning("Block with name '" + b.name + "' is not a valid block.");
					continue;
				}
				switch (split[1])
				{
					case "bb":
						Addblock(b, MidBottomType.WideBottom);
						break;
					case "bc":
						TopMeshes[1] = b.sharedMesh;
						b.sharedMesh.name = "TOP_WIDE";
						break;
					case "bl":
						Addblock(b, MidBottomType.WideMid);
						break;
					case "sb":
						Addblock(b, MidBottomType.NormalBottom);
						break;
					case "sc":
						TopMeshes[0] = b.sharedMesh;
						b.sharedMesh.name = "TOP_NORMAL";
						break;
					case "sl":
						Addblock(b, MidBottomType.NormalMid);
						break;
					case "sr":
						StairMeshes[(int)Def.StairType.RAMP] = b.sharedMesh;
						b.sharedMesh.name = "TOP_STAIR_RAMP";
						break;
					case "ss":
						StairMeshes[(int)Def.StairType.NORMAL] = b.sharedMesh;
						b.sharedMesh.name = "TOP_STAIR_NORMAL";
						break;
					default:
						Debug.LogWarning("Block with name '" + b.name + "' has an invalid block type.");
						break;
				}
			}
			PrepareMidMeshes(midBot);
		}
	}
}
