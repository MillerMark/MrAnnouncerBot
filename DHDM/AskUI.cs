using System;
using System.Reflection;
using System.Linq;
using DndCore;

namespace DHDM
{
	public class AskUI
	{
		public string Caption { get; set; }
		public object Value { get; set; }
		public string MemberName { get; set; }
		public string MemberTypeName { get; set; }
		public Character Player { get; set; }
		public AskUI()
		{

		}

		public AskUI(AskValueEventArgs ea)
		{
			Caption = ea.Caption;
			Value = ea.Value;
			MemberName = ea.MemberName;
			MemberTypeName = ea.MemberTypeName;
			Player = ea.Player;
		}
		public void SetBooleanProperty(bool? isChecked)
		{
			if (!isChecked.HasValue)
				return;
			Value = isChecked.Value;
			Character.SetBoolProperty(Player, MemberName, (bool)Value);
		}

		public bool GetBooleanValue()
		{
			if (Player == null)
				return (bool)Value;

			PropertyInfo propertyInfo = Player.GetType().GetProperty(MemberName, BindingFlags.Public | BindingFlags.Instance);
			if (null != propertyInfo && propertyInfo.CanRead)
				Value = (bool)propertyInfo.GetValue(Player);
			else
			{
				FieldInfo fieldInfo = Player.GetType().GetField(MemberName, BindingFlags.Public | BindingFlags.Instance);
				if (null != fieldInfo)
					Value = (bool)fieldInfo.GetValue(Player);
			}

			return (bool)Value;
		}
	}
}
