//#define profiling
using System;
using System.Linq;

namespace AvalonEdit
{
	public static class CodeUtils
	{
		public static bool IsIdentifierCharacter(char character)
		{
			return char.IsLetterOrDigit(character) || character == '_';
		}
	}
}
