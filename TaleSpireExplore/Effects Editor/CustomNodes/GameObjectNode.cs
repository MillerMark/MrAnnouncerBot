using System;
using System.Linq;
using System.Windows.Forms;
using UnityEngine;

namespace TaleSpireExplore
{
	public class GameObjectNode : TreeNode
	{

		public GameObjectNode()
		{

		}
		public GameObject GameObject { get; set; }
	}
}
