using System;
using System.Linq;
using UnityEngine;

namespace TaleSpireExplore
{
	public interface IScriptEditor
	{
		void InitializeInstance(MonoBehaviour script);
		string LastSerializedData { get; set; }
	}
}
