using System;
using System.Linq;
using System.Collections.Generic;
using GoogleHelper;

namespace DndCore
{
	[SheetName("DnD")]
	[TabName("Magic")]
	public class MagicItem
	{
		string parameterStr;
		string name;

		List<string> parameters;
		public List<string> Parameters
		{
			get
			{
				if (parameters == null)
				{
					string[] parts = ParameterStr.Split(',');
					parameters = parts.Select(x => x.Trim()).ToList();
				}
				return parameters;
			}
		}
		

		public string ParameterStr
		{
			get
			{
				if (parameterStr == null)
					GetNameAndParametersFromSignature(out name, out parameterStr);
				return parameterStr;
			}

		}
		public string Name
		{
			get
			{
				if (name == null)
					GetNameAndParametersFromSignature(out name, out parameterStr);
				return name;
			}
		}

		void GetNameAndParametersFromSignature(out string name, out string parameterStr)
		{
			parameterStr = signature.EverythingBetweenNarrow("(", ")");
			if (signature.Contains("("))
				name = signature.EverythingBefore("(");
			else
				name = signature;
		}


		[Column("name")]
		public string signature { get; set; }

		[Column]
		public string description { get; set; }

		[Column]
		public string duration { get; set; }

		[Column]
		public string onReceived { get; set; }

		[Column]
		public string onExpire { get; set; }

		[Column]
		public string onOwnerStartsTurn { get; set; }

		[Column]
		public string onRecipientStartsTurn { get; set; }

		[Column]
		public string onCasting { get; set; }

		[Column]
		public string onGetAttackAbility { get; set; }

		[Column]
		public string onCast { get; set; }

		[Column]
		public string onPlayerPreparesAttack { get; set; }

		[Column]
		public string onPlayerAttacks { get; set; }

		[Column]
		public string onDieRollStopped { get; set; }

		[Column]
		public string onPlayerHitsTarget { get; set; }

		[Column]
		public string onDispel { get; set; }

		public MagicItem()
		{

		}
	}
}
