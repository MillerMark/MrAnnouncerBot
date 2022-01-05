using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireCore
{
	public class TaleSpireBehavior : MonoBehaviour
	{
		public string OwnerID { get; set; }
		public void OwnerCreated(string ownerID)
		{
			Talespire.Log.Warning($"OwnerCreated/ownerID - {ownerID}");
			OwnerID = ownerID;
		}

		internal virtual void Initialize(string scriptData)
		{
			
		}

		public TaleSpireBehavior()
		{

		}
	}
}
