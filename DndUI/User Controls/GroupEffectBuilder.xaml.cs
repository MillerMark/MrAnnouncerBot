using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DndUI
{
	/// <summary>
	/// Interaction logic for GroupEffectBuilder.xaml
	/// </summary>
	public partial class GroupEffectBuilder : UserControl
	{
		public GroupEffectBuilder()
		{
			InitializeComponent();
		}

		TimeLineControl.TimeLineData timeLineData;
		private void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			if (timeLineData == null)
			{
				timeLineData = new TimeLineControl.TimeLineData();
				tlEffects.ItemsSource = timeLineData.Entries;
			}

			string entryName = "Effect" + timeLineData.Entries.Count;

			timeLineData.AddEntry(TimeSpan.Zero, TimeSpan.FromSeconds(1), entryName, new EffectEntry(DndCore.EffectKind.Animation, entryName));
		}

		private void BtnDelete_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
