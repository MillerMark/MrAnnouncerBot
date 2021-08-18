using System.Collections.Generic;
using UnityEngine;

namespace TaleSpireExploreDecompiles
{
	public class BezierPath
	{
		// Fields
		private const int SEGMENTS_PER_CURVE = 15;
		private const float MINIMUM_SQR_DISTANCE = 0.01f;
		private const float DIVISION_THRESHOLD = -0.97f;
		private List<Vector3> controlPoints = new List<Vector3>();
		private int curveCount;

		// Methods
		public Vector3 CalculateBezierPoint(int curveIndex, float t)
		{
			int num = curveIndex * 3;
			Vector3 vector = controlPoints[num];
			return CalculateBezierPoint(t, vector, controlPoints[num + 1], controlPoints[num + 2], controlPoints[num + 3]);
		}

		private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			float num = 1f - t;
			float num2 = t * t;
			float num3 = num * num;
			float num4 = num2 * t;
			return (Vector3)(((((num3 * num) * p0) + (((3f * num3) * t) * p1)) + (((3f * num) * num2) * p2)) + (num4 * p3));
		}

		private List<Vector3> FindDrawingPoints(int curveIndex)
		{
			List<Vector3> pointList = new List<Vector3> {
						CalculateBezierPoint(curveIndex, 0f),
						CalculateBezierPoint(curveIndex, 1f)
				};
			FindDrawingPoints(curveIndex, 0f, 1f, pointList, 1);
			return pointList;
		}

		private int FindDrawingPoints(int curveIndex, float t0, float t1, List<Vector3> pointList, int insertionIndex)
		{
			Vector3 vector = CalculateBezierPoint(curveIndex, t0);
			Vector3 vector2 = CalculateBezierPoint(curveIndex, t1);
			if ((vector - vector2).sqrMagnitude < 0.01f)
			{
				return 0;
			}
			float t = (t0 + t1) / 2f;
			Vector3 item = CalculateBezierPoint(curveIndex, t);
			if ((Vector3.Dot((vector - item).normalized, (vector2 - item).normalized) <= -0.97f) && (Mathf.Abs((float)(t - 0.5f)) >= 0.0001f))
			{
				return 0;
			}
			int num2 = 0 + FindDrawingPoints(curveIndex, t0, t, pointList, insertionIndex);
			pointList.Insert(insertionIndex + num2, item);
			num2++;
			return (num2 + FindDrawingPoints(curveIndex, t, t1, pointList, insertionIndex + num2));
		}

		public List<Vector3> GetControlPoints() =>
				controlPoints;

		public List<Vector3> GetDrawingPoints0()
		{
			List<Vector3> list = new List<Vector3>();
			int curveIndex = 0;
			while (curveIndex < curveCount)
			{
				if (curveIndex == 0)
				{
					list.Add(CalculateBezierPoint(curveIndex, 0f));
				}
				int num2 = 1;
				while (true)
				{
					if (num2 > 15)
					{
						curveIndex++;
						break;
					}
					float t = ((float)num2) / 15f;
					list.Add(CalculateBezierPoint(curveIndex, t));
					num2++;
				}
			}
			return list;
		}

		public List<Vector3> GetDrawingPoints1()
		{
			List<Vector3> list = new List<Vector3>();
			int num = 0;
			while (num < (controlPoints.Count - 3))
			{
				Vector3 vector = controlPoints[num];
				Vector3 vector2 = controlPoints[num + 1];
				Vector3 vector3 = controlPoints[num + 2];
				Vector3 vector4 = controlPoints[num + 3];
				if (num == 0)
				{
					list.Add(CalculateBezierPoint(0f, vector, vector2, vector3, vector4));
				}
				int num2 = 1;
				while (true)
				{
					if (num2 > 15)
					{
						num += 3;
						break;
					}
					float t = ((float)num2) / 15f;
					list.Add(CalculateBezierPoint(t, vector, vector2, vector3, vector4));
					num2++;
				}
			}
			return list;
		}

		public List<Vector3> GetDrawingPoints2()
		{
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < curveCount; i++)
			{
				List<Vector3> collection = FindDrawingPoints(i);
				if (i != 0)
				{
					collection.RemoveAt(0);
				}
				list.AddRange(collection);
			}
			return list;
		}

		public void Interpolate(List<Vector3> segmentPoints, float scale)
		{
			controlPoints.Clear();
			if (segmentPoints.Count >= 2)
			{
				for (int i = 0; i < segmentPoints.Count; i++)
				{
					if (i == 0)
					{
						Vector3 item = segmentPoints[i];
						Vector3 vector2 = segmentPoints[i + 1] - item;
						Vector3 vector3 = item + (scale * vector2);
						controlPoints.Add(item);
						controlPoints.Add(vector3);
					}
					else if (i == (segmentPoints.Count - 1))
					{
						Vector3 item = segmentPoints[i];
						Vector3 vector6 = item - segmentPoints[i - 1];
						Vector3 vector7 = item - (scale * vector6);
						controlPoints.Add(vector7);
						controlPoints.Add(item);
					}
					else
					{
						Vector3 vector8 = segmentPoints[i - 1];
						Vector3 item = segmentPoints[i];
						Vector3 vector10 = segmentPoints[i + 1];
						Vector3 normalized = (vector10 - vector8).normalized;
						Vector3 vector12 = item - ((scale * normalized) * (item - vector8).magnitude);
						Vector3 vector13 = item + ((scale * normalized) * (vector10 - item).magnitude);
						controlPoints.Add(vector12);
						controlPoints.Add(item);
						controlPoints.Add(vector13);
					}
				}
				curveCount = (controlPoints.Count - 1) / 3;
			}
		}

		public void SamplePoints(List<Vector3> sourcePoints, float minSqrDistance, float maxSqrDistance, float scale)
		{
			if (sourcePoints.Count >= 2)
			{
				Stack<Vector3> collection = new Stack<Vector3>();
				collection.Push(sourcePoints[0]);
				Vector3 item = sourcePoints[1];
				int num = 2;
				for (num = 2; num < sourcePoints.Count; num++)
				{
					if ((item - sourcePoints[num]).sqrMagnitude > minSqrDistance)
					{
						Vector3 vector5 = collection.Peek() - sourcePoints[num];
						if (vector5.sqrMagnitude > maxSqrDistance)
						{
							collection.Push(item);
						}
					}
					item = sourcePoints[num];
				}
				Vector3 vector2 = collection.Pop();
				Vector3 vector3 = collection.Peek();
				collection.Push(vector2 + ((vector3 - item).normalized * (((vector2 - vector3).magnitude - (item - vector2).magnitude) / 2f)));
				collection.Push(item);
				Interpolate(new List<Vector3>(collection), scale);
			}
		}

		public void SetControlPoints(List<Vector3> newControlPoints)
		{
			controlPoints.Clear();
			controlPoints.AddRange(newControlPoints);
			curveCount = (controlPoints.Count - 1) / 3;
		}
	}
}