using System.Collections.Generic;
using System.Linq;

namespace Streamloots
{
	public class StreamlootsCardData
	{
		public string cardName { get; set; }

		public List<StreamlootsDataField> fields { get; set; }

		public string GetField(string fieldName)
		{
			StreamlootsDataField field = fields.FirstOrDefault(f => f.name.Equals(fieldName));
			return (field != null) ? field.value : string.Empty;
		}

		public string Target
		{
			get
			{
				return GetField("target");
			}
		}

		public string Username
		{
			get
			{
				return GetField("username");
			}
		}
	}
}
