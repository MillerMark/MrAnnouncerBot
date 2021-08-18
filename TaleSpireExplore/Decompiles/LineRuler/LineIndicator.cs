using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace TaleSpireExploreDecompiles
{
	public class LineIndicator : MonoBehaviour
	{
		// Fields
		private const int _BUFFER_SIZE = 0x400;
		private readonly BezierPath _path = new BezierPath();
		private readonly List<Vector3> _verts = new List<Vector3>(0x400);
		private readonly List<int> _tris = new List<int>(0x800);
		private Mesh _mesh;
		[SerializeField]
		private AnimationCurve _stretchCurve = new AnimationCurve();
		[SerializeField]
		private List<ControlPoint> _controlPoints = new List<ControlPoint>();
		[SerializeField]
		private Material _mat;
		private readonly List<Transform> _transformList = new List<Transform>();
		private readonly List<Vector3> _points = new List<Vector3>();
		private const float _FOLD_RESPONSIVENESS = 12f;
		private const float _STIP_THICKNESS = 0.1f;

		// Methods
		private void Awake()
		{
			for (int i = 0; i < 0x400; i++)
			{
				_verts.Add(Vector3.zero);
			}
			for (int j = 0; j < 0x2aa; j += 2)
			{
				_tris.Add(j);
				_tris.Add(j + 2);
				_tris.Add(j + 1);
				_tris.Add(j + 1);
				_tris.Add(j + 2);
				_tris.Add(j + 3);
			}
			_mesh = new Mesh();
			_mesh.vertices = _verts.ToArray();
			_mesh.triangles = _tris.ToArray();
			_mesh.RecalculateBounds();
			MeshFilter filter = base.gameObject.AddComponent<MeshFilter>();
			filter.sharedMesh = _mesh;
			base.gameObject.AddComponent<MeshRenderer>().sharedMaterial = _mat;
		}

		public void SetTransformPoints(List<Transform> transforms)
		{
			_transformList.Clear();
			_transformList.AddRange(transforms);
			SetTransformPointsCommon();
		}

		public void SetTransformPoints(Transform[] transforms)
		{
			_transformList.Clear();
			_transformList.AddRange(transforms);
			SetTransformPointsCommon();
		}

		private void SetTransformPointsCommon()
		{
			_controlPoints.Clear();
			for (int i = 0; i < _transformList.Count; i++)
			{
				Vector3 localPosition = _transformList[i].localPosition;
				ControlPoint item = new ControlPoint
				{
					point = localPosition,
					control01 = localPosition,
					control02 = localPosition
				};
				_controlPoints.Add(item);
			}
		}

		public void ShakeLines()
		{
			for (int i = 0; i < _transformList.Count; i++)
			{
				Vector3 localPosition = _transformList[i].localPosition;
				if (_controlPoints.Count > i)
				{
					ControlPoint point = new ControlPoint
					{
						point = localPosition,
						control01 = localPosition + new Vector3((UnityEngine.Random.value - 0.5f) * 2f, 0f, (UnityEngine.Random.value - 0.5f) * 0.4f),
						control02 = localPosition + new Vector3((UnityEngine.Random.value - 0.5f) * 2f, 0f, (UnityEngine.Random.value - 0.5f) * 0.4f)
					};
					_controlPoints[i] = point;
				}
			}
		}

		private void Update()
		{
			_points.Clear();
			for (int i = 0; i < _controlPoints.Count; i++)
			{
				Vector3 localPosition = _transformList[i].localPosition;
				ControlPoint point = new ControlPoint
				{
					point = localPosition,
					control01 = Vector3.Lerp(_controlPoints[i].control01, localPosition, Time.deltaTime * 12f),
					control02 = (_controlPoints[i].control02 == Vector3.zero) ? localPosition : Vector3.Lerp(_controlPoints[i].control02, localPosition, Time.deltaTime * 12f)
				};
				_controlPoints[i] = point;
				if (i != 0)
				{
					_points.Add(_controlPoints[i].control01);
				}
				_points.Add(_controlPoints[i].point);
				if (i != (_controlPoints.Count - 1))
				{
					_points.Add(_controlPoints[i].control02);
				}
			}
			_path.SetControlPoints(_points);
			float x = 0f;
			for (int j = 1; j < _points.Count; j++)
			{
				x += Vector3.SqrMagnitude(_points[j] - _points[j - 1]);
			}
			x = Mathf.Lerp(0.1f, 0.02f, _stretchCurve.Evaluate(x * 0.05f));
			Matrix4x4 matrixx = new Matrix4x4();
			Vector3 forward = ActiveCameraManager.ActiveCamera.transform.forward;
			List<Vector3> list = _path.GetDrawingPoints1();
			int num2 = 0;
			for (int k = 0; k < (_verts.Count - 1); k += 2)
			{
				if (list.Count <= num2)
				{
					_verts[k] = Vector3.zero;
					_verts[k + 1] = Vector3.zero;
				}
				else
				{
					if (list.Count == (num2 + 1))
					{
						matrixx.SetTRS(list[num2], matrixx.rotation, Vector3.one);
					}
					else
					{
						Vector3 vector4 = Vector3.Cross(Vector3.Cross(forward, (list[num2] - list[num2 + 1]).normalized).normalized, Vector3.up);
						if (vector4.normalized == Vector3.zero)
						{
							vector4 = -forward;
						}
						matrixx.SetTRS(list[num2], Quaternion.LookRotation(vector4.normalized), Vector3.one);
					}
					_verts[k] = matrixx.MultiplyPoint3x4(new Vector3(-x, 0f, 0f));
					_verts[k + 1] = matrixx.MultiplyPoint3x4(new Vector3(x, 0f, 0f));
				}
				num2++;
			}
			int num3 = 0;
			for (int m = 0; m < _tris.Count; m += 6)
			{
				if (((int)(num3 * 0.5f)) >= (list.Count - 1))
				{
					_tris[m + 1] = 0;
					_tris[m + 2] = 0;
					_tris[m + 3] = 0;
					_tris[m + 4] = 0;
					_tris[m + 5] = 0;
				}
				else
				{
					_tris[m] = num3;
					_tris[m + 1] = num3 + 2;
					_tris[m + 2] = num3 + 1;
					_tris[m + 3] = num3 + 1;
					_tris[m + 4] = num3 + 2;
					_tris[m + 5] = num3 + 3;
				}
				num3 += 2;
			}
			_mesh.SetVertices(_verts);
			_mesh.SetTriangles(_tris, 0);
			_mesh.RecalculateBounds();
		}

		// Nested Types
		[Serializable, StructLayout(LayoutKind.Sequential)]
		private struct ControlPoint
		{
			public Vector3 control01;
			public Vector3 point;
			public Vector3 control02;
		}
	}
}
