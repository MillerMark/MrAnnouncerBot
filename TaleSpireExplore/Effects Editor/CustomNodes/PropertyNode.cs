using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using UnityEngine;

namespace TaleSpireExplore
{
	public class PropertyNode : TreeNode
	{
		public string PropertyName { get; set; }
		public PropertyInfo PropertyInfo { get; set; }
		public FieldInfo FieldInfo { get; set; }
		public object ValueInstance { get; set; }
		public object FieldValue => FieldInfo?.GetValue(ParentInstance);
		public object PropertyValue => PropertyInfo?.GetValue(ParentInstance);
		public object ParentInstance { get; set; }
		public bool IsDisabled { get; set; }

		public PropertyNode()
		{

		}
	}
}
