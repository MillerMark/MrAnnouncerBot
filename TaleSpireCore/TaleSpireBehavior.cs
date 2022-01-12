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
			if (Guard.IsNull(StateChanged, "StateChanged")) return;

			StateChanged?.Invoke(this, data);
		}

		internal virtual void Initialize(string scriptData)
		{

		}

		// TODO: Maybe delete this because it's no longer used?
		internal virtual void OwnerSelected()
		{

		}

		public TaleSpireBehavior()
		{
			
		}
	}
}
