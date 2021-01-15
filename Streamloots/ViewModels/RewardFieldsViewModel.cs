namespace Streamloots
{
	public class RewardFieldsViewModel
	{
		public string message { get; set; }
		public string imageUrl { get; set; }
		public bool deactivated { get; set; }
		public bool ttsEnabled { get; set; }
		public bool muteSound { get; set; }
		public string type { get; set; }  // ALERT, ...?

		public RewardFieldsViewModel()
		{
		}
	}
}
