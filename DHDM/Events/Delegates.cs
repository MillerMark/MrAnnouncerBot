using System;
using System.Linq;

namespace DHDM
{
	public delegate void ViewerDieRollEndsEventHandler(object sender, ViewerDieRollStoppedEventArgs ea);
	public delegate void ViewerDieRollStartsEventHandler(object sender, ViewerDieRollStartedEventArgs ea);
}
