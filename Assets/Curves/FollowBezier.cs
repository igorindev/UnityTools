﻿using System.Collections;
using UnityEngine;

public class FollowBezier : MonoBehaviour
{
    [SerializeField] Transform[] routes;
    [SerializeField] float speedModifier;
    [SerializeField] bool loop;
    
    int routeToGo = 0;
    float tParam = 0f;
    bool coroutineAllowed = true;
    Vector3 objPos;

    Coroutine current;

    public void StartFollow()
    {
        routeToGo = 0;
        coroutineAllowed = true;

        if (current != null)
        {
            StopCoroutine(current);
        }

        Follow();
    }

    void Follow()
    {
        if (coroutineAllowed)
        {
            current = StartCoroutine(GoByTheRoute(routeToGo));
        }
    }

    IEnumerator GoByTheRoute(int routeNumber)
    {
        coroutineAllowed = false;

        Vector3 p0 = routes[routeNumber].GetChild(0).position;
        Vector3 p1 = routes[routeNumber].GetChild(1).position;
        Vector3 p2 = routes[routeNumber].GetChild(2).position;
        Vector3 p3 = routes[routeNumber].GetChild(3).position;

        while (tParam < 1)
        {
            tParam += Time.deltaTime * speedModifier;

            objPos = Mathf.Pow(1 - tParam, 3) * p0 +
            3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 +
            3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 +
            Mathf.Pow(tParam, 3) * p3;

            transform.position = objPos;
            yield return new WaitForEndOfFrame();
        }

        tParam = 0f;
        routeToGo += 1;
        if (routeToGo > routes.Length - 1)
        {
            routeToGo = 0;
        }

        coroutineAllowed = true;
        if (loop)
        {
            Follow();
        }
    }
}
