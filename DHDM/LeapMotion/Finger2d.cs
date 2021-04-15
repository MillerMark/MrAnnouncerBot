//#define profiling
using Leap;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public class FingerBone
	{
		public ScaledPoint Center { get; set; }
		public double ScaledLength { get; set; }
		public DndCore.Vector Direction{ get; set; }
		public FingerBone(ScaledPoint center, double scaledLength, DndCore.Vector direction)
		{
			Center = center;
			ScaledLength = scaledLength;
			Direction = direction;
		}
	}

	public class Finger2d
	{
		public ScaledPoint TipPosition { get; set; }
		public FingerBone DistalPhalange { get; set; }
		public FingerBone IntermediatePhalange { get; set; }
		public FingerBone ProximalPhalange { get; set; }
		public Finger2d()
		{

		}

		// ![](B0D2EA3807AA73CED7E27A1205328BF7.png;;;0.01721,0.01721)
		FingerBone GetBone(Finger finger, Bone.BoneType boneType)
		{
			Bone bone = finger.Bone(boneType);
			ScaledPoint centerPt = LeapCalibrator.ToScaledPoint(bone.Center);
			return new FingerBone(centerPt, bone.Length * centerPt.Scale, new DndCore.Vector(bone.Direction.x, bone.Direction.y));
		}

		public Finger2d(Finger finger)
		{
			DistalPhalange = GetBone(finger, Bone.BoneType.TYPE_DISTAL);
			IntermediatePhalange = GetBone(finger, Bone.BoneType.TYPE_INTERMEDIATE);
			ProximalPhalange = GetBone(finger, Bone.BoneType.TYPE_PROXIMAL);
			TipPosition = LeapCalibrator.ToScaledPoint(finger.TipPosition);
		}
	}
}