using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;

namespace TaleSpireCore
{
	public class MiniGrouperData
	{
		[DefaultValue(false)]
		public bool Hidden { get; set; }

		[DefaultValue(false)]
		public bool Flying { get; set; }

		[DefaultValue(FormationStyle.FreeForm)]
		public FormationStyle FormationStyle { get; set; } = FormationStyle.FreeForm;

		[DefaultValue(1)]
		public int ColumnRadius { get; set; } = 1;

		[DefaultValue(GroupMovementMode.Formation)]
		public GroupMovementMode Movement { get; set; } = GroupMovementMode.Formation;

		[DefaultValue(LookTowardMode.Movement)]
		public LookTowardMode Look { get; set; } = LookTowardMode.Movement;

		[DefaultValue(-1)]
		public int BaseIndex { get; set; } = -1;

		[DefaultValue(0)]
		public int RingHue { get; set; }

		public List<string> Members { get; set; } = new List<string>();

		[DefaultValue(0)]
		public int Spacing { get; set; }

		public MiniGrouperData()
		{

		}
	}
}
