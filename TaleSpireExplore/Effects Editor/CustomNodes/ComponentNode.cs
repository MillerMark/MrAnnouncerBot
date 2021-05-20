using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;

namespace TaleSpireExplore
{
	public class ComponentNode : TreeNode
	{
		public UnityEngine.Component Component { get; set; }
		public Type Type { get; set; }
		public ComponentNode()
		{

		}
	}
}
