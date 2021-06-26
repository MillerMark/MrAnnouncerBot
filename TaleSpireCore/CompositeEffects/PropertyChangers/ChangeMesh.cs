using System;
using System.Linq;

namespace TaleSpireCore
{
	public class ChangeMesh : BasePropertyChanger
	{
		public ChangeMesh()
		{
		}

		public ChangeMesh(string propertyName, string valueStr) : base(propertyName, valueStr)
		{
		}


		protected override object ParseValue()
		{
			return Talespire.Mesh.Get(Value);
		}

		public void SetValue(string meshName)
		{
			Value = meshName;
		}
	}
}
