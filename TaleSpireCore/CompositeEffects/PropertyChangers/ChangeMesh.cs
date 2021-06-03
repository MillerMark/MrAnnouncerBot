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


		public override object GetValue()
		{
			return Talespire.Mesh.Get(Value);
		}

		public void SetValue(string meshName)
		{
			Value = meshName;
		}
	}
}
