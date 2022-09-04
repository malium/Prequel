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
	public static class Def
	{
		public const float BlockSeparation = 0.02f;
		public const int MinStrucSide = 3;
		public const int MaxStrucSide = 32;
		public const int MaxBlockID = MaxStrucSide * MaxStrucSide - 1;
		public const int DefaultStrucSide = 8;
		public const int MaxLayerSlots = 12;
		public const int MapStart = -1000000000; // 1.000.000.000
		public const int MapEnd = 1000000000; // 1.000.000.000
		public const int MapSideSize = MapEnd - MapStart; // 2.000.000.000
		public const long MaxMapID = (long)MapSideSize * (long)MapSideSize; // 4.000.000.000.000.000.000

		public const int BridgeMaxLength = 16;

		public const int RCLayerBlock = 8;
		public const int RCLayerLE = 9;
		public const int RCLayerBridge = 10;
		public const int RCLayerRayPlane = 11;
		public const int RCLayerProjectile = 12;
		public const int RCLayerDeahtPlane = 13;
		public const int RCLayerLEMoveCollision = 15;
		public const int RCLayerAvoidLE = 16;

		public static readonly int Material_CSHB_Color = Shader.PropertyToID("CSHBColor");
		public static readonly int Material_TA_Texture = Shader.PropertyToID("_TexArr");
		public static readonly int Material_TA_Index = Shader.PropertyToID("_TextureIndex");
		public static readonly int Material_Normal_Texture = Shader.PropertyToID("_BaseMap");

		public static readonly int MaterialTextureID = Shader.PropertyToID("_BaseMap");
		public static readonly int ColoredMaterialTextureID = Shader.PropertyToID("Texture2D_556CDA01");
		public static readonly int ColoredMaterialCSHBID = Shader.PropertyToID("Vector4_34A1AF91");
		public static readonly int BackgroundColorID = Shader.PropertyToID("ParticleColor");
		public static readonly int MaterialColorID = Shader.PropertyToID("_BaseColor");
		public static readonly int FlipShaderID = Shader.PropertyToID("_Flip");
		public static readonly int OverlayColorID = Shader.PropertyToID("_OverlayColor");

		public const float HALFPI = Mathf.PI * 0.5f;

		public static int RayCastMask(bool block, bool le, bool bridge, bool rayPlane)
		{
			int mask = 0;

			if (block)
				mask |= (1 << RCLayerBlock);
			if (le)
				mask |= (1 << RCLayerLE);
			if (bridge)
				mask |= (1 << RCLayerBridge);
			if (rayPlane)
				mask |= (1 << RCLayerRayPlane);

			return mask;
		}
		public enum BlockMeshType
		{
			TOP,
			MID,
		}
		public enum StrucMod
		{
			HorzFlip,
			VertFlip,
			Rotated90,

			COUNT
		}
		public const int StrucModCount = (int)StrucMod.COUNT;
		public enum BlockEffects
		{
			Poison,
			Slowness,

			COUNT
		}
		public const int BlockEffectCount = (int)BlockEffects.COUNT;
		// What of the 4 possible square rotations is
		public enum RotationState
		{
			Default,
			Right,
			Half,
			Left,
			COUNT
		}
		public const int RotationStateCount = (int)RotationState.COUNT;
		// Are there any property locked?
		public enum LockState
		{
			Unlocked,
			SemiLocked,
			Locked,

			COUNT
		}
		public const int LockStateCount = (int)LockState.COUNT;
		// Can be a stair?
		public enum StairState
		{
			NONE,
			POSSIBLE,
			ALWAYS,

			STAIR_OR_RAMP,

			RAMP_POSSIBLE,
			RAMP_ALWAYS,

			COUNT
		}
		public const int StairStateCount = (int)StairState.COUNT;
		// Where a Block decoration can be
		public enum DecoPosition
		{
			TOP,
			NORTH,
			SOUTH,
			EAST,
			WEST,

			COUNT
		}
		public const int DecoPositionCount = (int)DecoPosition.COUNT;
		public enum DecoSpriteType
		{
			TOP,
			SIDE
		}
		public enum BlockType
		{
			NORMAL,
			STAIRS,
			WIDE,
			COUNT
		}
		public const int BlockTypeCount = (int)BlockType.COUNT;
		public enum AntTopDirection
		{
			SOUTH_NORTH,
			NORTH_SOUTH,
			EAST_WEST,
			WEST_EAST,
			SOUTH_WEST,
			SOUTH_EAST,
			NORTH_WEST,
			NORTH_EAST,
			WEST_NORTH,
			WEST_SOUTH,
			EAST_NORTH,
			EAST_SOUTH,

			COUNT
		}
		public const int AntTopDirectionCount = (int)AntTopDirection.COUNT;
		public enum AntType
		{
			STRAIGHT,
			TURN_LEFT,
			TURN_RIGHT,

			COUNT
		}
		public const int AntTypeCount = (int)AntType.COUNT;
		public enum SpaceDirection
		{
			NORTH,
			SOUTH,
			EAST,
			WEST,

			COUNT
		}
		public const int SpaceDirectionCount = (int)SpaceDirection.COUNT;
		public enum BlockVoid
		{
			NORMAL,
			SEMIVOID,
			FULLVOID,

			COUNT
		}
		public const int BlockVoidCount = (int)BlockVoid.COUNT;
		public enum BlockState
		{
			Edit,
			Game
		}
		public enum BiomeLayerType
		{
			HARD_GROUND1, // E0CDCD
			HARD_GROUND2, // 877279
			SOFT_GROUND1, // 74C44A
			SOFT_GROUND2, // 919823
			POOL1, // 56C6D9
			POOL2, // 5680B1
			SHORE1, // F5D056
			SHORE2, // DB9841
			STACK1, // CCCCCC
			STACK2, // 666666
			FULLVOID, // FF00FF
			OTHER, // FF0000
			COUNT
		}
		public const int BiomeLayerTypeCount = (int)BiomeLayerType.COUNT;
		public const int BiomeTypeCount = BiomeLayerTypeCount - 2;
		public static readonly Color32[] BiomeLayerTypeColors = new Color32[]
		{
			GameUtils.UnpackColor(0xE0CDCDFF),
			GameUtils.UnpackColor(0x877279FF),
			GameUtils.UnpackColor(0x74C44AFF),
			GameUtils.UnpackColor(0x919823FF),
			GameUtils.UnpackColor(0x56C6D9FF),
			GameUtils.UnpackColor(0x5680B1FF),
			GameUtils.UnpackColor(0xF5D056FF),
			GameUtils.UnpackColor(0xDB9841FF),
			GameUtils.UnpackColor(0xCCCCCCFF),
			GameUtils.UnpackColor(0x666666FF),
			GameUtils.UnpackColor(0xFF00FFFF),
			GameUtils.UnpackColor(0xFF0000FF),
		};
		public enum LivingEntityType
		{
			ODD,
			Monster,
			Prop,

			COUNT
		}
		public const int LivingEntityTypeCount = (int)LivingEntityType.COUNT;
		public enum ElementType
		{
			BLEEDING,
			BURNING,
			FREEZING,
			CURSED,
			POISONED,
			DISEASE,

			COUNT
		}
		public const int ElementTypeCount = (int)ElementType.COUNT;
		public enum ResistanceType
		{
			PHYSICAL,
			ELEMENTAL,
			ULTIMATE,
			SOUL,
			POISON,

			COUNT
		}
		public const int ResistanceTypeCount = (int)ResistanceType.COUNT;
		public enum DamageType
		{
			HIT,
			CUT,
			FIRE,
			ICE,
			LIGHT,
			ELECTRICAL,
			ASPHYXIA,
			DEPRESSION,
			POISON,
			QUICKSILVER,

			UNAVOIDABLE,

			COUNT
		}
		public const int DamageTypeCount = (int)DamageType.COUNT;
		public enum MonsterAIType
		{
			NO_AI, // Static without behaviour
			ROAMING_AI, // It just roams around
			AGRESSIVE_AI, // Roams until find an enemy
			SCARED_AI, // It just roams, but if it finds an enemy scapes

			COUNT
		}
		public const int MonsterAITypeCount = (int)MonsterAIType.COUNT;

		public enum VFXType
		{
			CAST,
			TRAVEL,
			ONHIT,

			COUNT
		}
		public const int VFXTypeCount = (int)VFXType.COUNT;

		public enum VFXTarget
		{
			ODD,
			PROP,
			MONSTER,
			GENERAL,

			COUNT
		}
		public const int VFXTargetCount = (int)VFXTarget.COUNT;
		public enum BlockPhysicState
		{
			SOLID,
			LIQUID,
		}
		public enum BlockStability
		{
			STABLE,
			STACKED,
			ATTACHED_1,
			ATTACHED_2,
			ATTACHED_3,
			ATTACHED_4,
			CONTAINED, // ATTACHED_4 + STACKED
			STACK_OR_ATTACHED_1,
			STACK_OR_ATTACHED_2,
			STACK_OR_ATTACHED_3,
			STACK_OR_ATTACHED_4,
			WEIGHT_OVER_IS_LESS,
		}
		public enum MaterialMode
		{
			Default,
			Fade,
			Transparent,
			Cutout
		}
		public enum LayerEditAccesor
		{
			BIOME,
			STRUC,
			BUILD
		}
		public enum MonsterFrame
		{
			FACE_1,
			FACE_2,
			BACK_1,
			BACK_2,
			FACE_ATTACK,
			BACK_ATTACK,

			COUNT
		}
		public const int MonsterFrameCount = (int)MonsterFrame.COUNT;
		public enum StrucEditKeyMap
		{
			Info,
			Menu,
			CamEdit,
			CamGame,
			CamFree,
			LayerEdit,
			SelectAll,
			IncreaseStrucWidth,
			DecreaseStrucWidth,
			IncreaseStrucHeight,
			DecreaseStrucHeight,
			ReapplyLayers,
			ResetLocks,
			MaterialCycle,
			Stair,
			Ramp,
			Lock,
			Anchor,
			RotateLeft,
			RotateRight,
			LengthUp,
			LengthDown,
			HeightUp,
			HeightDown,
			DestroyBlock,
			Void,
			StackBlock,
			Visibility,
			Layer1,
			Layer2,
			Layer3,
			Layer4,
			Layer5,
			Layer6,
			Layer7,
			Layer8,
			Layer9,
			Layer10,
			Layer11,
			Layer12,
			SelectLayer0,
			SelectLayer1,
			SelectLayer2,
			SelectLayer3,
			SelectLayer4,
			SelectLayer5,
			SelectLayer6,
			SelectLayer7,
			SelectLayer8,
			SelectLayer9,
			SelectLayer10,
			SelectLayer11,
			SelectLayer12,
			CamMoveForward,
			CamMoveBackward,
			CamMoveLeft,
			CamMoveRight,
			BlockHide,
			BlockUnhide,
			Undo, // TODO

			COUNT
		}
		public const int StrucEditKeyMapCount = (int)StrucEditKeyMap.COUNT;
		public enum ImageSelectorPosition
		{
			Left,
			Center
		}
		public enum StrucEditMenuFunc
		{
			CreatingNew,
			CopyStruc,
			LoadStruc,
			SaveStruc,
			COUNT
		}
		public const int StrucEditMenuFuncCount = (int)StrucEditMenuFunc.COUNT;
		public enum MonsterSpellSlots
		{
			AUTO,
			SPELL1,
			SPELL2,
			SPELL3,

			COUNT
		}
		public const int MonsterSpellSlotsCount = (int)MonsterSpellSlots.COUNT;
		public enum MonsterAttackFrameTrigger
		{
			CAST,
			RELEASE,
		}
		public enum MonsterAwarenessState
		{
			CALM,
			WARN,
			ALERT
		}
		public enum SpellState
		{
			IDLE,
			CASTING,
			RELEASING,
			COOLDOWN
		}
		public enum ProjectileTravelType
		{
			LINEAR,
			PARABOLIC,
			TARGETTED,

			COUNT
		}
		public const int ProjectileTravelTypeCount = (int)ProjectileTravelType.COUNT;
		public enum ProjectileTravelCollider
		{
			Sphere,
			Prism,

			COUNT
		}
		public const int ProjectileTravelColliderCount = (int)ProjectileTravelCollider.COUNT;
		public enum ProjectileAreaDamageType
		{
			RECT2D, // rectangulo
			RECT3D, // cubo
			CIRC2D, // circulo
			CIRC3D, // sphera
			CONE2D, // cono2d
			CONE3D, // cono3d

			COUNT
		}
		[Flags]
		public enum SpellHitMask
		{
			NONE = 0,
			BLOCKS = 1,
			PROPS = 2,
			MONSTERS = 4,

			LIVINIG_ENTITIES = (PROPS | MONSTERS),
			ALL = (LIVINIG_ENTITIES | BLOCKS),
		}
		[Flags]
		public enum SpellDamageMask
		{
			NONE = 0,
			BLOCKS = 1,
			PROPS = 2,
			MONSTERS = 4,

			LIVINIG_ENTITIES = (PROPS | MONSTERS),
			ALL = (LIVINIG_ENTITIES | BLOCKS),
		}
		public enum VFXEnd
		{
			Stop,
			SelfDestroy,
			Repeat,

			COUNT
		}
		public const int VFXEndCount = (int)VFXEnd.COUNT;
		[Flags]
		public enum VFXFacing
		{
			FaceCameraFull,
			FaceCameraFreezeX,
			FaceCameraFreezeY,
			FaceCameraFreezeZ,
			FaceCameraFreezeXY,
			FaceCameraFreezeXZ,
			FaceCameraFreezeYZ,
			FaceXUp,
			FaceXDown,
			FaceYUp,
			FaceYDown,
			FaceZUp,
			FaceZDown,
			FaceDirection,
			DontFaceAnything,
		}
		public const float SelfHitTimeDelay = 0.5f;
		public enum OnHitType
		{
			Displacement,
			StatusEffect,
			Damage,

			COUNT
		}
		public const int OnHitTypeCount = (int)OnHitType.COUNT;
		public enum StatusEffect
		{
			ANALYSED,
			STUNNED,
			BOUNTIFUL,
			BLITZKRIEGING,
			// TODO: add StatusEffects
			COUNT
		}
		public const int StatusEffectCount = (int)StatusEffect.COUNT;
		public enum MovementState
		{
			RampingUp,
			Continuous,
			Stopping,
			Stopped
		}
		[Flags]
		public enum ImpulseDirection
		{
			NONE = 0,
			TOP = 1,
			BOT = 2,
			LEFT = 4,
			RIGHT = 8
		}
		public enum StairType
		{
			NORMAL,
			RAMP,

			COUNT
		}
		public const int StairTypeCount = (int)StairType.COUNT;
		public enum QuirkTriggerType
		{
			AwarenessState,
			EntitiesOnSight,
			EntitiesHeard,
			EntitiesNear,
			SpellState,
			TargetNotNull,
			TimerReady,
			HasAttacked,
			TargetReached,
			Status,
			TargetStatus,
			OnHit,
			TargetIs,
		}
		public enum MonsterTimers
		{
			TimerA,
			TimerB,

			COUNT
		}
		public const int MonsterTimerCount = (int)MonsterTimers.COUNT;
		public enum Materials
		{
			Default,
			Cutout,
			Fade,
			Transparent,

			Sprite,
			SpriteLit,
			SpriteLitDS,

			ColoredDefault,
			ColoredTransparent,
			ColoredCutout,
			Background,
			COUNT
		}
		public const int MaterialCount = (int)Materials.COUNT;
		public enum ConfigType
		{
			STRING,
			INTEGER,
			FLOAT,
			BOOLEAN,
			VECTOR2,
			VECTOR3,
			ENUM,
			ENTITYLIST,
			PARTICLE_TEXTURE,
			SPELL_WEAPON,
		}
		public enum VFXPlanes
		{
			ONE,
			TWO,
			FOUR
		}
		public enum MeleeAttackType
		{
			Piercing,
			Slash
		}
		public enum CowardEnemySelection
		{
			Distance,
			Hearing,
			Sight,
		}
		public enum LEStats
		{
			Health,
			Soulness
		}
		public enum Comparison
		{
			LESS,
			LESSEQUAL,
			EQUAL,
			GREATEREQUAL,
			GREATER
		}
		public enum StatType
		{
			FIXED,
			PCT
		}
		public enum ChangeTargetType
		{
			Distance,
			Entity,
			Weakest,
			Strongest,

		}
		public enum ChangeTargetSelection
		{
			HeardFriends,
			HeardEnemies,
			HeardEntities,
			SeenFriends,
			SeenEnemies,
			SeenEntities,
		}
		public enum TargetIs
		{
			FRIEND,
			ENEMY,
			PROP,
			MONSTER,
			ODD,
		}
		public enum BlockProperty
		{
			Layer,
			Material,
			Length,
			Height,
			MicroHeight,
			BlockType,
			Rotation,
			StairState,
			VoidState,
			Prop,
			Monster,
			COUNT
		}
		public const int BlockPropertyCount = (int)BlockProperty.COUNT;

		public const int WorldSizeMax = 32000;
		public const int WorldSizeMin = 16;
		public const int WorldSizeDefault = 4000;
		public const int WorldStrucSize = 16;
		public const int WorldPilarMinID = 0;
		public const int WorldPilarMaxID = WorldSizeMax * WorldSizeMax - 1;
		public enum BiomeStat
		{
			Density,
			Temperature,
			Height,
			Soulness,
			Wealth,

			COUNT
		}
		public const int BiomeStatCount = (int)BiomeStat.COUNT;
		public enum WorldPerlins
		{
			Density,
			Temperature,
			Height,
			Soulness,
			Wealth,
			Bump,

			COUNT
		}
		public const int WorldPerlinCount = (int)WorldPerlins.COUNT;
		public const int BiomeStatMin = -2;
		public const int BiomeStatMax = 2;
		public const float WorldHeightRange = 32f;
		public const float WorldBumpRange = 6f;
		public enum MessageBoxType
		{
			OnlyCross,
			OnlyOk,
			CrossNOk,
			YesNo,
			YesNoNCross,
		}
		public const int CompactSpawnRadius = 2;
		public const int NormalSpawnRadius = 8;
		public const int LargeSpawnRadius = 32;
		public const ushort DefaultLastWaveChance = 500;
		public enum OddWeaponType
		{
			GreatSword,

			COUNT
		}
		public const int OddWeaponTypeCount = (int)OddWeaponType.COUNT;
		public enum OddAttackType
		{
			Normal1,
			Normal2,
			Normal3,
			Normal4,
			Heavy1,
			Heavy2,
			Dash,

			COUNT
		}
		public const int OddAttackTypeCount = (int)OddAttackType.COUNT;
		public static bool IsOddHeavyAttack(OddAttackType oddAttack) => oddAttack == OddAttackType.Heavy1 || oddAttack == OddAttackType.Heavy2;
		public static OddAttackType OddAttackTypeFromState(AI.ODD.OddState state)
		{
			switch(state)
			{
				case AI.ODD.OddState.Attacking1:
					return OddAttackType.Normal1;
				case AI.ODD.OddState.Attacking2:
					return OddAttackType.Normal2;
				case AI.ODD.OddState.Attacking3:
					return OddAttackType.Normal3;
				case AI.ODD.OddState.Attacking4:
					return OddAttackType.Normal4;
				case AI.ODD.OddState.AttackingHeavy1:
					return OddAttackType.Heavy1;
				case AI.ODD.OddState.AttackingHeavy2:
					return OddAttackType.Heavy2;
				case AI.ODD.OddState.DashAttack:
					return OddAttackType.Dash;
				default:
					return OddAttackType.COUNT;
			}
		}
		public enum ItemEffectType
		{
			StatUpDown,
			Consumable,

			COUNT
		}
		public const int ItemEffectTypeCount = (int)ItemEffectType.COUNT;
		public enum ItemCategory
		{
			Consumable,
			Resource,
			Equipment,
			Quest,

			COUNT
		}
		public const int ItemCategoryCount = (int)ItemCategory.COUNT;
		public enum ItemSubcategory
		{

			COUNT
		}
		public const int ItemSubcategoryCount = (int)ItemSubcategory.COUNT;
		public enum ItemConsumableType
		{
			None,
			Fast,
			Eating,

			COUNT
		}
		public const int ItemConsumableTypeCount = (int)ItemConsumableType.COUNT; 
		public enum ItemEquipSlot
		{

			COUNT
		}
		public const int ItemEquipSlotCount = (int)ItemEquipSlot.COUNT;
		public enum QuickItemSlot
		{
			SlotE,
			Slot1,
			Slot2,
			Slot3,
			Slot4,
			Slot5,
			Slot6,
			Slot7,
			Slot8,
			Slot9,

			COUNT
		}
		public const int QuickItemSlotCount = (int)QuickItemSlot.COUNT;
		public enum GameInputControl
		{
			Mouse,
			MouseLikeController,
			Control
		}
	}
}
