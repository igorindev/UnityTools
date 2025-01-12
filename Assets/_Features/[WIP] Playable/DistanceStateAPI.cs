using System;
using UnityEngine;

public class DistanceStateAPI : MonoBehaviour
{
    public enum CascadeUpdateRate : byte
    {
        Zero = 0,
        Total = 1,
        Half = 2,
        Quarter = 4,
        Eight = 8,
    }

    [Serializable]
    public struct DistanceCascade
    {
        [SerializeField] private float _distanceUnits;
        [SerializeField] private int _updateRate;

        public float DistanceUnitsSqr => _distanceUnits * _distanceUnits;
        public int UpdateRate => (int)_updateRate;

        public DistanceCascade(float distance, int updateRate)
        {
            _distanceUnits = distance;
            _updateRate = updateRate;
        }
    }

    [SerializeField] private DistanceCascade[] _distanceCascades = new[] { new DistanceCascade(100, 1) };
    [SerializeField] private Transform _targetTransform;


    public static Action<float, float, Func<Vector3, int>> OnUpdate;

    public Vector3 Target => _targetTransform.position;

    public void Initialize(Transform target)
    {
        _targetTransform = target;
    }

    public void Update()
    {
        float deltaTime = Time.deltaTime;
        float frameCount = Time.frameCount;

        OnUpdate?.Invoke(deltaTime, frameCount, GetDistanceCascadeUpdateRate);
    }

    public int GetDistanceCascadeUpdateRate(Vector3 worldPosition)
    {
        if (_distanceCascades.Length == 0)
        {
            return 1;
        }

        for (int i = 0; i < _distanceCascades.Length - 1; i++)
        {
            if ((Target - worldPosition).sqrMagnitude <= _distanceCascades[i].DistanceUnitsSqr)
            {
                return _distanceCascades[i].UpdateRate;
            }
        }

        return 0;
    }
}
