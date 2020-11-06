﻿//#define profiling
using System;
using System.Linq;
using System.Collections.Generic;

namespace DHDM
{
	public class World3d
	{
		int numInstancesCreated;
		public List<Virtual3dObject> Instances { get; set; }
		public World3d()
		{

		}

		public void Update(DateTime time)
		{
			if (Instances == null)
				return;
			lock (Instances)
				foreach (Virtual3dObject virtual3DObject in Instances)
				{
					virtual3DObject.Update(time);
				}
		}

		/// <summary>
		/// Creates a new virtual 3D instance in the world and returns its index.
		/// </summary>
		public int AddInstance(PositionVelocityTime throwVector)
		{
			Virtual3dObject virtual3DObject = new Virtual3dObject() { StartPosition = throwVector.Position, InitialVelocity = throwVector.LeapVelocity.ToMetersPerSecond(), Index = numInstancesCreated };
			if (Instances == null)
				Instances = new List<Virtual3dObject>();
			lock (Instances)
				Instances.Add(virtual3DObject);
			numInstancesCreated++;
			return virtual3DObject.Index;
		}

		public void RemoveInstances(List<Virtual3dObject> instancesToRemove)
		{
			if (Instances == null)
				return;
			lock (Instances)
				foreach (Virtual3dObject virtual3DObject in instancesToRemove)
				{
					Instances.Remove(virtual3DObject);
				}
		}
	}
}