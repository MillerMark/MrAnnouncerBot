//#define profiling
using System;
using System.Linq;
using DndCore;
using TaleSpireCore;

namespace DHDM
{
	public static class TargetManager
	{
		public static void TargetPoint(ApiResponse response)
		{
			VectorDto vector = response.GetData<VectorDto>();
			if (vector == null)
				return;

			Targeting.SetPoint(new Vector(vector.x, vector.y, vector.z));
		}

		public static CharacterPositions GetAllCreaturesInVolume()
		{
			VectorDto volumeCenter = Targeting.TargetPoint.ToVectorDto();
			string shapeName = Targeting.ExpectedTargetDetails.Shape.ToString();
			CharacterPositions allTargetsInVolume = TaleSpireClient.GetAllTargetsInVolume(volumeCenter, shapeName, Targeting.ExpectedTargetDetails.Dimensions);
			return allTargetsInVolume;
		}

	}
}


