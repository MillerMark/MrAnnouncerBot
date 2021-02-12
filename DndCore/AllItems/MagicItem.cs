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

		public void TriggerDispel(Magic magic)
		{
			TriggerEvent(magic, onDispel);
		}

		public void TriggerReceived(Magic magic)
		{
			TriggerEvent(magic, onReceived);
		}

		public void TriggerExpire(Magic magic)
		{
			TriggerEvent(magic, onExpire);
		}

		public void TriggerSenderStartsTurn(Magic magic)
		{
			TriggerEvent(magic, onSenderStartsTurn);
		}

		public void TriggerRecipientStartsTurn(Magic magic)
		{
			TriggerEvent(magic, onRecipientStartsTurn);
		}

		public void TriggerRecipientAttacks(Magic magic)
		{
			TriggerEvent(magic, onRecipientAttacks);
		}

		public void TriggerRecipientPreparesAttack(Magic magic)
		{
			TriggerEvent(magic, onRecipientPreparesAttack);
		}

		public void TriggerRecipientSaves(Magic magic)
		{
			TriggerEvent(magic, onRecipientSaves);
		}

		public void TriggerRecipientHitsTarget(Magic magic)
		{
			TriggerEvent(magic, onRecipientHitsTarget);
		}
		
		public void TriggerDieRollStopped(Magic magic, DiceStoppedRollingData dice)
		{
			TriggerEvent(magic, onDieRollStopped, dice);
		}

		private void TriggerEvent(Magic magic, string eventCode, DiceStoppedRollingData dice = null)
		{
			if (string.IsNullOrWhiteSpace(eventCode))
				return;
			Expressions.Do(eventCode, magic.Caster as Character, Target.FromMagic(magic), magic.CastedSpell, dice, magic);
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
		public string onSenderStartsTurn { get; set; }

		
		[Column]
		public string onRecipientStartsTurn { get; set; }

		
		[Column]
		public string onRecipientPreparesAttack { get; set; }


		[Column]
		public string onRecipientAttacks { get; set; }

		
		[Column]
		public string onRecipientSaves { get; set; }

		[Column]
		public string onRecipientChecksSkill { get; set; }


		[Column]
		public string onDieRollStopped { get; set; }

		
		[Column]
		public string onRecipientHitsTarget { get; set; }

		
		[Column]
		public string onDispel { get; set; }

		public MagicItem()
		{

		}
	}
}
