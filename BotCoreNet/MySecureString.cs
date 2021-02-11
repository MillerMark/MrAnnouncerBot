using System;
using System.Linq;

namespace BotCore
{
	public class MySecureString
	{
		string value;
		public MySecureString(string initialValue)
		{
			value = initialValue;
		}
		public override string ToString()
		{
			return "This is a secure string. Call GetStr() to get the value.";
		}

		public string GetStr()
		{
			return value;
		}
	}
}
