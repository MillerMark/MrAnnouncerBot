using System;
using DndCore;
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
using System.Windows.Shapes;

namespace DHDM
{
	/// <summary>
	/// Interaction logic for FrmAsk.xaml
	/// </summary>
	public partial class FrmAsk : Window
	{
		public int AnswerInt { get; set; }

		public FrmAsk()
		{
			InitializeComponent();
		}

		private static void AnswerButton_Click(object sender, RoutedEventArgs e)
		{
			if (!(sender is AnswerButton answerButton))
				return;

			FrameworkElement parent = answerButton.Parent as FrameworkElement;
			while (parent != null && !(parent is FrmAsk))
			{
				parent = parent.Parent as FrameworkElement;
			}

			if (parent is FrmAsk frmAsk)
			{
				frmAsk.AnswerInt = answerButton.AnswerInt;
				frmAsk.Close();
			}

			//answerButton
			// AnswerInt
		}
		public static void TryAnswer(int value)
		{
			if (frmAsk == null)
				return;
			frmAsk.AnswerInt = value;
			frmAsk.CloseGracefully();
		}
		void CloseGracefully()
		{
			Dispatcher.Invoke(() =>
			{
				frmAsk.Close();
			});
		}

		static FrmAsk frmAsk;

		public static int Ask(string question, List<string> answers, Window owner)
		{
			frmAsk = new FrmAsk();
			frmAsk.Owner = owner;
			frmAsk.AnswerInt = 0;
			char[] quotes = { '"', ' ' };
			frmAsk.tbQuestion.Text = question.Trim(quotes);
			foreach (string answer in answers)
			{
				string trimmedAnswer = answer.Trim(quotes);
				int result = 0;
				int colonIndex = trimmedAnswer.IndexOf(":");
				if (colonIndex > 0)
				{
					string resultStr = trimmedAnswer.EverythingBefore(":");
					int.TryParse(resultStr, out result);
					string answerText = trimmedAnswer.EverythingAfter(":");
					AnswerButton button = new AnswerButton();
					button.AnswerInt = result;
					button.Content = answerText;
					button.Margin = new Thickness(4);
					button.Click += AnswerButton_Click;
					frmAsk.spAnswers.Children.Add(button);
				}
			}
			frmAsk.ShowDialog();
			return frmAsk.AnswerInt;
		}
	}
}
