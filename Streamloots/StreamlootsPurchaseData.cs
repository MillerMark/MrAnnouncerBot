using System.Collections.Generic;
using System.Linq;

namespace Streamloots
{
	public class StreamlootsPurchaseData
	{
		public List<StreamlootsDataField> fields { get; set; }

		// This is the person receiving the action (if gifted)
		public string Recipient
		{
			get
			{
				return GetField("giftee");
			}
		}

		public int Quantity
		{
			get
			{
				return GetIntField("quantity");
			}
		}

		// This is the person doing the action (purchase or gifting)
		public string Username
		{
			get
			{
				return GetField("username");
			}
		}

		string GetField(string fieldName)
		{
			StreamlootsDataField field = fields.FirstOrDefault(f => f.name.Equals(fieldName));
			return (field != null) ? field.value : string.Empty;
		}

		int GetIntField(string fieldName)
		{
			string valueStr = GetField(fieldName);
			if (int.TryParse(valueStr, out int value))
				return value;
			return 0;
		}
	}
}
