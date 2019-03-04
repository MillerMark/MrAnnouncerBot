using System;
using System.Collections.Generic;
using System.Linq;

namespace DndUI
{
	public static class FocusHelper
	{
		public delegate void FocusedControlsChangedHandler(object sender, FocusedControlsChangedEventArgs ea);
		public static event FocusedControlsChangedHandler FocusedControlsChanged;
		public static void OnFocusedControlsChanged()
		{
			if (clearingInternally)
				return;

			FocusedControlsChanged?.Invoke(null, new FocusedControlsChangedEventArgs(ActiveStatBoxes, DeactivatedStatBoxes));

			deactivatedStatBoxes.Clear();
		}

		static bool clearingInternally;
		public static void ClearAll(List<StatBox> activeStatBoxes)
		{
			clearingInternally = true;
			try
			{
				foreach (StatBox statBox in activeStatBoxes)
				{
					statBox.StatBoxState = StatBoxState.DisplayOnly;
				}
			}
			finally
			{
				clearingInternally = false;
			}
		}



		static List<StatBox> activeStatBoxes = new List<StatBox>();
		static List<StatBox> deactivatedStatBoxes = new List<StatBox>();
		

		public static List<StatBox> ActiveStatBoxes { get => activeStatBoxes; private set => activeStatBoxes = value; }
		public static List<StatBox> DeactivatedStatBoxes { get => deactivatedStatBoxes; private set => deactivatedStatBoxes = value; }

		public static void ClearActiveStatBoxes()
		{
			ClearAll(activeStatBoxes);
			
			if (activeStatBoxes.Count > 0)
			{
				deactivatedStatBoxes.AddRange(activeStatBoxes);
				activeStatBoxes.Clear();
				OnFocusedControlsChanged();
			}
		}

		public static void Add(StatBox statBox)
		{
			activeStatBoxes.Add(statBox);
			OnFocusedControlsChanged();
		}
		public static void Remove(StatBox statBox)
		{
			if (clearingInternally)
				return;
			activeStatBoxes.Remove(statBox);
			OnFocusedControlsChanged();
		}


		static FocusHelper()
		{

		}
	}
}
