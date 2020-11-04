//#define profiling
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

		/// <summary>
		/// Creates a new virtual 3D instance in the world and returns its index.
		/// </summary>
		public int AddInstance(PositionVelocityTime throwVector)
		{
			Virtual3dObject virtual3DObject = new Virtual3dObject() { Position = throwVector.Position, Velocity = throwVector.Velocity, Index = numInstancesCreated };
			if (Instances == null)
				Instances = new List<Virtual3dObject>();
			Instances.Add(virtual3DObject);
			numInstancesCreated++;
			return virtual3DObject.Index;
		}
	}
}