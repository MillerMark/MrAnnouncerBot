using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace DHDM
{
	public class RelayCommand : ICommand
	{
		private readonly Action<object> execute;
		private readonly Func<object, bool> canExecute;

		public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
		{
			this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
			this.canExecute = canExecute;
		}

		[DebuggerStepThrough]
		public bool CanExecute(object parameter) => canExecute == null || canExecute(parameter);

		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}

		public void Execute(object parameter) => execute(parameter);

	}
}
