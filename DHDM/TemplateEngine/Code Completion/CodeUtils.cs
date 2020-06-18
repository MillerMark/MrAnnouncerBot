//#define profiling
using System;
using System.Linq;

namespace DHDM
{
	public static class CodeUtils
	{
		public static bool IsIdentifierCharacter(char character)
		{
			return char.IsLetterOrDigit(character) || character == '_';
		}
	}
}
