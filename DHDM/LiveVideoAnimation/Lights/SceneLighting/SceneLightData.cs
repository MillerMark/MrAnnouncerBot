using System;
using System.Linq;
using System.Collections.Generic;
using SheetsPersist;
using Imaging;

namespace DHDM
{
	[Document("Live Video Animation")]
	[Sheet("Lights")]
	public class SceneLightData
	{
		public string LeftLight { get; set; }
		public string CenterLight { get; set; }
		public string RightLight { get; set; }
		public string SceneName { get; set; }

		public void SetLightsNow()
		{
			DmxLight.Left.SetColor(LeftLight);
			DmxLight.Center.SetColor(CenterLight);
			DmxLight.Right.SetColor(RightLight);
		}

		public SceneLightData()
		{

		}
	}
}
