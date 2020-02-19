using System;
using System.Linq;

namespace MapCore
{
	/// <summary>
	/// A target with **half cover** has a +2 bonus to AC and Dexterity saving throws. A target has 
	/// half cover if an obstacle blocks at least half of its body. The obstacle might be a low 
	/// wall, a large piece of furniture, a narrow tree trunk, or a creature, whether that creature 
	/// is an enemy or a friend.
	/// 
	/// A target with **three-quarters cover** has a +5 bonus to AC and Dexterity saving throws. A 
	/// target has three-quarters cover if about three-quarters of it is covered by an obstacle.
	/// The obstacle might be a portcullis, an arrow slit, or a thick tree trunk.
	/// 
	/// A target with **total cover** can't be targeted directly by an attack or a spell, although 
	/// some spells can reach such a target by including it in an area of effect. A target has 
	/// total cover if it is completely concealed by an obstacle.
	/// </summary>
	public enum Cover
	{
		None,

		/// <summary>
		/// +2 bonus to AC and Dexterity saving throws
		/// </summary>
		OneHalf,

		/// <summary>
		/// +5 bonus to AC and Dexterity saving throws
		/// </summary>
		ThreeQuarters,

		/// <summary>
		/// Can't be targeted **directly** by an attack or a spell
		/// </summary>
		Total
	}
}
