using System;
using System.Linq;

namespace Streamloots
{
	public class AlertViewModel
	{
		public bool deactivated { get; set; }
		public string imageUrl { get; set; }  // "https://static.streamloots.com/99d63dc9-60b1-4376-aea9-08592f04781d/loots/chest.png"
		public string message { get; set; }  // "¡{{username}} has purchased {{quantity}} chest!"
		public bool muteSound { get; set; }
		public string type { get; set; }  // "ALERT"

		static AlertViewModel()
		{

		}
	}
}
