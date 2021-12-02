using System;
using System.Linq;
using System.Collections.Generic;

namespace TaleSpireCore
{
	public static partial class Talespire
	{
		public static class Rulers
		{

			public static int GetLineRulerCount()
			{
				List<LineRulerMode> allLineRulers = GetAllLineRulers();
				if (allLineRulers == null)
					return 0;
				return allLineRulers.Count;
			}

			public static List<LineRulerMode> GetAllLineRulers()
			{
				return Components.GetAll<LineRulerMode>().Cast<LineRulerMode>().ToList();
			}

			/* 
				for (int i = 0; i < lineRulers.Length; i++)
				{
					LineRulerMode LineRulerMode = lineRulers[i] as LineRulerMode;

					if (LineRulerMode != null)
					{
						List<Transform> _handles = ReflectionHelper.GetNonPublicField<List<Transform>>(LineRulerMode, "_handles");
						if (_handles != null)
						{
							tbxScratch.Text += $"{_handles.Count} _handles found!\n";
							for (int j = 0; j < _handles.Count; j++)
								AddEffect($"RulerFlames{j}", "MediumFire", new Vector3(_handles[j].position.x, _handles[j].position.y, _handles[j].position.z));
						}
						else
							tbxScratch.Text += $"_handles NOT found!\n";
					}
				} */
		}
	}
}