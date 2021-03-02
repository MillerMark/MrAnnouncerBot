using System;
using System.Linq;

namespace DHDM
{
	public delegate void DigitChangedEventHandler(object sender, DigitChangedEventArgs ea);
	public delegate void ViewerDieRollEndsEventHandler(object sender, ViewerDieRollStoppedEventArgs ea);
	public delegate void ViewerDieRollStartsEventHandler(object sender, ViewerDieRollStartedEventArgs ea);
}
