using System.Collections;
using UnityEngine;

[AddComponentMenu("Spline/Spline Follow")]
public class SplineFollow : MonoBehaviour
{
    [SerializeField] SplineBaked splineToFollow;
    [SerializeField] bool faceSplineDirection;
    [SerializeField] float totalDuration = 1;

    Vector3[] movePositions;
    Coroutine movementCoroutine;

    void Start()
    {
        movePositions = splineToFollow.SplinePoints;
    }

    [ContextMenu("Move")]
    public void MoveInSpline()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }

        movementCoroutine = StartCoroutine(MoveCoroutine());
    }

    IEnumerator MoveCoroutine()
    {
        float transition = totalDuration / movePositions.Length;
        int id = 0;

        while (id < movePositions.Length)
        {
            float count = 0;
            transform.GetPositionAndRotation(out Vector3 start, out Quaternion startRot);
            while (count < transition)
            {
                count += Time.deltaTime;

                transform.position = Vector3.Lerp(start, movePositions[id], count / transition);
                if (faceSplineDirection)
                {
                    transform.rotation = Quaternion.Slerp(startRot, Quaternion.LookRotation((movePositions[id] - start).normalized), count / transition);
                }
                yield return null;
            }

            transform.position = movePositions[id];
            id += 1;
        }
    }
}
