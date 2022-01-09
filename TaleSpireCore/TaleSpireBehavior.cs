using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class TaleSpireBehavior : MonoBehaviour
	{
		public event EventHandler<object> StateChanged;

		public string OwnerID { get; set; }
		public void OwnerCreated(string ownerID)
		{
			Talespire.Log.Warning($"OwnerCreated/ownerID - {ownerID}");
			OwnerID = ownerID;
		}


		protected virtual void OnStateChanged(object data)
		{
			Talespire.Log.Warning($"StateChanged?.Invoke(this, data);");
			if (StateChanged == null)
			{
				Talespire.Log.Error($"StateChanged is null!");
				return;
			}

			Talespire.Log.Debug($"StateChanged.GetInvocationList().Length = {StateChanged.GetInvocationList().Length}");

			StateChanged?.Invoke(this, data);
		}

		internal virtual void Initialize(string scriptData)
		{

		}

		internal virtual void OwnerSelected()
		{

		}

		public TaleSpireBehavior()
		{
			
		}
	}
}
