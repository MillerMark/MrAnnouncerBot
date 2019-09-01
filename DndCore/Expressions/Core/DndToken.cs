using System;
using System.Linq;

namespace DndCore
{
	public class DndToken
	{
		public virtual string Name { get; set; }

		public virtual bool Handles(string tokenName)
		{
			return tokenName == Name;
		}

		public DndToken()
		{

		}
	}
}

