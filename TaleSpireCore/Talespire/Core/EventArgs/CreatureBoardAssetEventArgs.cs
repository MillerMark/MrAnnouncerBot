using System;
using System.Linq;

namespace TaleSpireCore
{
	public delegate void CreatureBoardAssetEventHandler(object sender, CreatureBoardAssetEventArgs ea);
	public class CreatureBoardAssetEventArgs : EventArgs
	{
		public CreatureBoardAsset Mini { get; set; }
		public CreatureBoardAssetEventArgs()
		{

		}
		public void SetMini(CreatureBoardAsset selectedMini)
		{
			Mini = selectedMini;
		}
	}
}