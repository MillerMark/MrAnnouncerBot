using System;
using System.Linq;

namespace DndCore
{
	/// <summary>
	/// Attribute for properties whose values can only be determined at runtime by the user.
	/// </summary>
	public class AskAttribute : Attribute
	{
		public string Caption { get; set; }

		public AskAttribute()
		{

		}

		public AskAttribute(string caption)
		{
			Caption = caption;
		}
	}
}