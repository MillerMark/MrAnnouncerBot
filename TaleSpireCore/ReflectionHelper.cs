using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace TaleSpireCore
{
	public static class ReflectionHelper
	{
		public static void CallNonPublicMethod(Type type, string methodName, object instance = null, object[] parameters = null)
		{
			if (parameters == null)
				parameters = new object[] { };

			BindingFlags bindingFlags = BindingFlags.NonPublic;
			if (instance == null)
				bindingFlags |= BindingFlags.Static;
			else
				bindingFlags |= BindingFlags.Instance;

			MethodInfo methodInfo = type.GetMethod(methodName, bindingFlags);
			if (methodInfo == null)
			{
				MessageBox.Show($"Non-public method \"{methodName}\" not found in {type.Name}.", "Error!");
				return;
			}

			try
			{
				methodInfo.Invoke(instance, parameters);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception calling method!");
			}
		}

		static ReflectionHelper()
		{

		}
	}
}
