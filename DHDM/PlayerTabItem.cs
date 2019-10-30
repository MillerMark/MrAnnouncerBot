using DndUI;
using System;
using System.Linq;
using System.Windows.Controls;

namespace DHDM
{
	public class PlayerTabItem : TabItem
	{
		public int PlayerId { get; set; }
		public CharacterSheets CharacterSheets { get; set; }
		public ListBox StateList { get; set; }
		public PlayerTabItem()
		{

		}
	}
}
