using System;
using System.Linq;

namespace MapCore
{
	public enum EndCapKind
	{
		None,
		Left,
		Top,
		Right,
		Bottom,
		TopLeftCorner,
		TopRightCorner,
		BottomRightCorner,
		BottomLeftCorner,
		LeftTee,
		TopTee,
		RightTee,
		BottomTee,
		FourWayIntersection
	}
}

