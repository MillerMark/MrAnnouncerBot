using Bounce.TaleSpire.AssetManagement;
using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Board
		{
			public static bool IsLoaded
			{
				get
				{
					return (CameraController.HasInstance &&
									BoardSessionManager.HasInstance &&
									BoardSessionManager.HasBoardAndIsInNominalState &&
									!BoardSessionManager.IsLoading);
				}
			}

			//public static void InstantiateCreature(string boardAssetId, Vector3 position)
			//{
			//	CreatureDataV1 cd = new CreatureDataV1();
			//	cd.Color = new Bounce.Unmanaged.Color888(20, 255, 20);
			//	cd.ExplicitlyHidden = false;
			//	cd.Flying = false;
			//	cd.Hp = new CreatureStat(110f, 120f);
			//	cd.Get = new Bounce.Unmanaged.NGuid(new Guid(boardAssetId));
			//	CreatureManager.CreateAndAddNewCreature(cd, position, Quaternion.identity);
			//	//BoardSessionManager.Board.AddAsset(PlaceableKind.Prop, new Bounce.Unmanaged.NGuid(), origin, angle, DataModel.ShaderState.DropInAnimationKind.Place);
			//	//BoardSessionManager.
			//}
		}
	}
}