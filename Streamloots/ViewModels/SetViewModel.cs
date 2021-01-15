using System;
using System.Linq;
using System.Collections.Generic;

namespace Streamloots
{
	public class SetViewModel
	{
		public string _id { get; set; }
		public bool craftableCards { get; set; }
		public DateTime createdAt { get; set; }
		public bool Default { get; set; }
		public string imageUrl { get; set; }
		public List<SetViewModel> loots { get; set; }
		public DateTime modifiedAt { get; set; }
		public string name { get; set; }
		public string ownerId { get; set; }
		public string pageId { get; set; }
		public bool paused { get; set; }
		public string pauseReason { get; set; }
		public bool published { get; set; }
		public DateTime publishedAt { get; set; }
		public List<RarityViewModel> rarities { get; set; }
		public DateTime resumeAt { get; set; }
		public List<string> tags { get; set; }
		public DateTime unpublishedAt { get; set; }

		public SetViewModel()
		{

		}
	}
}
