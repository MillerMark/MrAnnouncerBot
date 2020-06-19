//#define profiling
using System;
using System.Linq;
using System.Windows.Input;

namespace DHDM
{
	public static class Modifiers
	{
		public static bool ShiftDown
		{
			get
			{
				return (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
			}
		}

		public static bool AltDown
		{
			get
			{
				return Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);
			}
		}

		public static bool CtrlDown
		{
			get
			{
				return Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
			}
		}


		public static bool AnyModifierDown
		{
			get
			{
				return CtrlDown || AltDown || ShiftDown;
			}
		}


		public static bool NoModifiersDown
		{
			get
			{
				return !AnyModifierDown;
			}
		}
		static KeyboardModifiers GetModifiers()
		{
			KeyboardModifiers result = KeyboardModifiers.None;
			if (Modifiers.AltDown)
				result |= KeyboardModifiers.Alt;
			if (Modifiers.CtrlDown)
				result |= KeyboardModifiers.Ctrl;
			if (Modifiers.ShiftDown)
				result |= KeyboardModifiers.Shift;
			return result;
		}
		public static KeyboardModifiers Active => GetModifiers();
	}
}
