using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DHDM
{
	public class PlayerRollCheckBox : CheckBox
	{
		public UIElement DependantUI { get; set; }
		public int PlayerId { get; set; }
		public RadioButton RbDisadvantage { get; set; }
		public RadioButton RbAdvantage { get; set; }
		public RadioButton RbNormal { get; set; }
		public TextBox TbxInspiration { get; set; }
		public PlayerRollCheckBox()
		{

		}
	}
}
