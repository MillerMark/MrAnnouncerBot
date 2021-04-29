using System;
using System.Linq;

namespace TaleSpireExploreDecompiles
{


  [Serializable]
	public class EffectDecompile
	{
		// Fields
		public string _effectName;
		public VisualEffect _prefab;
		private SpawnFactory<VisualEffectDecompile> _effects;

		// Methods
		public VisualEffect GetEffect()
		{
			if (this._effects == null)
				this._effects = new SpawnFactory<VisualEffect>(this._prefab, null, 0, true, null);
			return this._effects.HireItem(false);
		}
	}
}
