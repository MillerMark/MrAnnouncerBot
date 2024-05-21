using System;

namespace Imaging
{
    public enum ObsFramePropertyAttribute
    {
        None = 0,
        X,
        Y,
        Scale,
        Rotation,
        Opacity
    }

    public class ObsTransformEdit : BaseObsTransform
	{
		public double DeltaX { get; set; }
		public double DeltaY { get; set; }
		public double DeltaRotation { get; set; }
		public double DeltaScale { get; set; } = 1;
		public double DeltaOpacity { get; set; } = 1;

		public ObsTransformEdit()
		{

		}

		public double GetX()
		{
			return Origin.X + DeltaX;
		}

		public double GetY()
		{
			return Origin.Y + DeltaY;
		}

		public double GetRotation()
		{
			return Rotation + DeltaRotation;
		}

		public double GetOpacity()
		{
			return Opacity * DeltaOpacity;
		}

		public double GetScale()
		{
			return Scale * DeltaScale;
		}

        public bool Matches(ObsFramePropertyAttribute attribute, double comparisonValue)
        {
            switch (attribute)
            {
                case ObsFramePropertyAttribute.X:
                    return comparisonValue == GetX();
                case ObsFramePropertyAttribute.Y:
                    return comparisonValue == GetY();
                case ObsFramePropertyAttribute.Scale:
                    return comparisonValue == GetScale();
                case ObsFramePropertyAttribute.Rotation:
                    return comparisonValue == GetRotation();
                case ObsFramePropertyAttribute.Opacity:
                    return comparisonValue == GetOpacity();
                default:
                    return false;
            }
        }
    }
}
