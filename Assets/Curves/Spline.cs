using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(LineRenderer))]
public class Spline : MonoBehaviour
{
	public Transform toMove;
	public List<GameObject> controlPoints = new List<GameObject>();
	public Color color = Color.white;
	public float width = 0.2f;
	public float speedModifier = 1;
	public int numberOfPoints = 20;
	public bool faceSplineDirection;
	LineRenderer lineRenderer;

	Vector3[] movePositions;

	[ContextMenu("Move")]
	public void MoveInSpline()
    {
		StartCoroutine(Go());
	}

	IEnumerator Go() 
	{
		int id = 0;
        while (id < movePositions.Length)
		{
			float count = 0;
			Vector3 start = toMove.position;
			Quaternion startRot = toMove.rotation;
			while (count < 1)
			{
				count += Time.deltaTime * speedModifier;

				toMove.position = Vector3.Lerp(start, movePositions[id], count);
                if (faceSplineDirection)
                {
					toMove.rotation = Quaternion.Slerp(startRot, Quaternion.LookRotation((movePositions[id] - start).normalized), count);
				}
				yield return null;
			}

			toMove.position = movePositions[id];
			id += 1;
		}
	}

	void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.useWorldSpace = true;
		lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
	}

	void Update()
	{
		if (null == lineRenderer || controlPoints == null || controlPoints.Count < 3)
		{
			return; // not enough points specified
		}

		// update line renderer
		lineRenderer.startColor = color;
		lineRenderer.endColor = color;
		lineRenderer.startWidth = width;
		lineRenderer.endWidth = width;

		if (numberOfPoints < 2)
		{
			numberOfPoints = 2;
		}

		lineRenderer.positionCount = numberOfPoints * (controlPoints.Count - 2);
		movePositions = new Vector3[numberOfPoints * (controlPoints.Count - 2)];

		Vector3 p0, p1, p2;
		for (int j = 0; j < controlPoints.Count - 2; j++)
		{
			// check control points
			if (controlPoints[j] == null || controlPoints[j + 1] == null || controlPoints[j + 2] == null)
			{
				return;
			}
			// determine control points of segment
			p0 = 0.5f * (controlPoints[j].transform.position
			+ controlPoints[j + 1].transform.position);
			p1 = controlPoints[j + 1].transform.position;
			p2 = 0.5f * (controlPoints[j + 1].transform.position
			+ controlPoints[j + 2].transform.position);

			// set points of quadratic Bezier curve
			Vector3 position;
			float t;
			float pointStep = 1.0f / numberOfPoints;
			if (j == controlPoints.Count - 3)
			{
				pointStep = 1.0f / (numberOfPoints - 1.0f);
				// last point of last segment should reach p2
			}
			for (int i = 0; i < numberOfPoints; i++)
			{
				t = i * pointStep;
				position = (1.0f - t) * (1.0f - t) * p0 + 2.0f * (1.0f - t) * t * p1 + t * t * p2;
				lineRenderer.SetPosition(i + j * numberOfPoints, position);
				movePositions[i + j * numberOfPoints] = position;
			}
		}
	}
}