using System;
using System.Linq;

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
		}
	}
}