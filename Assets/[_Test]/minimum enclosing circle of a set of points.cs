using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Delaunay : MonoBehaviour
{
    [SerializeField] Transform pointPrefab;
    public List<Transform> points;
    [SerializeField] int _pointAmount;
    [SerializeField] Vector2 _xLimits;
    [SerializeField] Vector2 _yLimits;
 
    public void CreateRandomPoints()
    {
        ClearPoints();
        for (int i = 0; i < _pointAmount; i++)
        {
            float x = Random.Range(_xLimits.x, _xLimits.y);
            float y = Random.Range(_yLimits.x, _yLimits.y);
            Transform newPoint = CreatePointAt(x, y);
            points.Add(newPoint);
        }
        Debug.Log("drew");
    }
 
    Transform CreatePointAt(float x, float y)
    {
        Transform pointTransform = Instantiate(pointPrefab, new Vector3(x, y, 0), Quaternion.identity);
        return pointTransform;
    }
 
    void ClearPoints()
    {
        foreach (Transform point in points)
        {
            Destroy(point.gameObject);
        }
        points.Clear();
    }
 
    public static Vector2 GetCircumcenter(Vector2 pointA, Vector2 pointB, Vector2 pointC)
    {
        LinearEquation lineAB = new LinearEquation(pointA, pointB);
        LinearEquation lineBC = new LinearEquation(pointB, pointC);
 
        Vector2 midPointAB = Vector2.Lerp(pointA, pointB, .5f);
        Vector2 midPointBC = Vector2.Lerp(pointB, pointC, .5f);
 
        LinearEquation perpendicularAB = lineAB.PerpendicularLineAt(midPointAB);
        LinearEquation perpendicularBC = lineBC.PerpendicularLineAt(midPointBC);
 
        Vector2 circumcenter = GetCrossingPoint(perpendicularAB, perpendicularBC);
 
        return circumcenter;
    }
 
    public static Circle Circumcircle(Vector2 pointA, Vector2 pointB, Vector2 pointC)
    {
        LinearEquation lineAB = new LinearEquation(pointA, pointB);
        LinearEquation lineBC = new LinearEquation(pointB, pointC);
 
        Vector2 midPointAB = Vector2.Lerp(pointA, pointB, .5f);
        Vector2 midPointBC = Vector2.Lerp(pointB, pointC, .5f);
 
        LinearEquation perpendicularAB = lineAB.PerpendicularLineAt(midPointAB);
        LinearEquation perpendicularBC = lineBC.PerpendicularLineAt(midPointBC);
 
        Vector2 circumcenter = GetCrossingPoint(perpendicularAB, perpendicularBC);
 
        float circumRadius = Vector2.Distance(circumcenter, pointA);
 
        return new Circle(circumcenter, circumRadius);
    }
 
 
    static Vector2 GetCrossingPoint(LinearEquation line1, LinearEquation line2)
    {
        float A1 = line1._A;
        float A2 = line2._A;
        float B1 = line1._B;
        float B2 = line2._B;
        float C1 = line1._C;
        float C2 = line2._C;
 
        //Cramer's rule
        float Determinant = A1 * B2 - A2 * B1;
        float DeterminantX = C1 * B2 - C2 * B1;
        float DeterminantY = A1 * C2 - A2 * C1;
 
        float x = DeterminantX / Determinant;
        float y = DeterminantY / Determinant;
 
        return new Vector2(x, y);
    }
 
    public List<Vector2> GetPointsPositions()
    {
        List<Vector2> pointList = new List<Vector2>();
        foreach (Transform t in points)
        {
            pointList.Add(t.position);
        }
        return pointList;
    }
 
    //not really efficient
    //try meggido's or welzl's algorhithm for efficiency?
    public Circle MinimumEnclosingCircle(Vector2[] points)
    {
        if (points.Length < 2)
        {
            Debug.LogError("need at least 2 points");
            return null;
        }
        //for 2 points return a circle with points at opposite sides
        if (points.Length == 2)
        {
            return TwoPointMinimumEnclosingCircle(points[0], points[1]);
        }
        //else try all mec's of 2 and 3 points and choose the one with smallest radius
        else
        {
            //arbitrarily large circle as starting "smallest  circle"
            Circle smallestCircle = new Circle(Vector2.zero, 10000000000000);
            Circle currentCircle;
            int amountOfPoints = points.Length;
            for (int i = 0; i < amountOfPoints; i++)
            {
                for (int j = i + 1; j < amountOfPoints; j++)
                {
                    //try 2 point circle
                    currentCircle = TwoPointMinimumEnclosingCircle(points[i], points[j]);
                    if (currentCircle.Radius < smallestCircle.Radius && IsEnclosingPoints(currentCircle, points))
                    {
                        smallestCircle = currentCircle;
                    }
 
                    //try 3 point circles
                    for (int k = j + 1; k < amountOfPoints; k++)
                    {
                        currentCircle = Circumcircle(points[i], points[j], points[k]);
                        if (currentCircle.Radius < smallestCircle.Radius && IsEnclosingPoints(currentCircle, points))
                        {
                            smallestCircle = currentCircle;
                        }
                    }
                }
            }
            return smallestCircle;
        }
    }
 
    Circle TwoPointMinimumEnclosingCircle(Vector2 pointA, Vector2 pointB)
    {
        Vector2 center = (pointA + pointB) / 2;
        float radius = Vector2.Distance(center, pointA);
        return new Circle(center, radius);
    }
 
 
    bool IsEnclosingPoints(Circle circle, Vector2[] points)
    {
        foreach (Vector2 point in points)
        {
            if (!IsPointInCircle(point, circle))
            {
                return false;
            }
        }
        return true;
    }
 
 
    // bool IsEnclosingPoints(Vector2[] points)
    // {
    //     foreach (Vector2 point in points)
    //     {
    //         if (IsPointInCircle(point, _circle))
    //         {
 
    //         }
    //         else
    //         {
    //             return false;
    //         }
    //     }
    //     return true;
    // }
 
 
    bool IsPointInCircle(Vector2 point, Circle circle)
    {
        float dist = Vector2.Distance(point, circle.Center);
        if (dist < circle.Radius || Mathf.Approximately(dist, circle.Radius))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
 
    // Circle Circumcircle(Vector2 pointA, Vector2 pointB, Vector2 pointC)
    // {
    //     Vector2 center = GetCircumcenter(pointA, pointB, pointC);
    //     float radius = Vector2.Distance(center, pointA);
    //     return new Circle(center, radius);
    // }
}

public class Circle
{
    public Vector2 _center;
    public float _radius;
 
    public float Radius
    {
        get
        {
            return _radius;
        }
        set
        {
            _radius = value;
        }
    }
 
    public Vector2 Center
    {
        get
        {
            return _center;
        }
        set
        {
            _center = value;
        }
    }
 
    public Circle(Vector2 center, float radius)
    {
        _center = center;
        _radius = radius;
    }
}
[System.Serializable]
public class LinearEquation
{
    public float _A;
    public float _B;
    public float _C;
 
    public LinearEquation() { }
 
    //Ax+By=C
    public LinearEquation(Vector2 pointA, Vector2 pointB)
    {
        float deltaX = pointB.x - pointA.x;
        float deltaY = pointB.y - pointA.y;
        _A = deltaY; //y2-y1
        _B = -deltaX; //x1-x2
        _C = _A * pointA.x + _B * pointA.y;
    }
 
    public LinearEquation PerpendicularLineAt(Vector3 point)
    {
        LinearEquation newLine = new LinearEquation();
 
        newLine._A = -_B;
        newLine._B = _A;
        newLine._C = newLine._A * point.x + newLine._B * point.y;
 
        return newLine;
    }
}