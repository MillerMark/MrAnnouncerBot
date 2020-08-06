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
using DndCore;
using System.Windows.Shapes;

namespace DHDM
{
	/// <summary>
	/// Interaction logic for FrmSelectInGameCreature.xaml
	/// </summary>
	public partial class FrmSelectInGameCreature : Window
	{
		public List<Character> AllPlayers = new List<Character>();
		public FrmSelectInGameCreature()
		{
			InitializeComponent();
		}
		public void SetDataSources(List<InGameCreature> creatures, List<Character> players)
		{
			foreach (InGameCreature inGameCreature in creatures)
				inGameCreature.OnScreen = false;

			if (players != null)
				foreach (Character character in players)
					character.IsSelected = false;

			lbInGameCreatures.ItemsSource = creatures;
			lbPlayers.ItemsSource = players;
		}

		private void btnOkay_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
