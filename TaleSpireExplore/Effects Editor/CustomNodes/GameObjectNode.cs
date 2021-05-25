using System;
using System.Linq;
using System.Windows.Forms;
using TaleSpireCore;
using UnityEngine;

namespace TaleSpireExplore
{
	public class GameObjectNode : TreeNode
	{
		public GameObjectNode()
		{

		}
		public CompositeEffect CompositeEffect { get; set; }
		public GameObject GameObject { get; set; }
	}
}
