using System;
using UnityEngine;

public abstract class DistanceObjectComponent : MonoBehaviour
{
    public virtual Vector3 WorldPosition => transform.position;

    private float _accumulatedDeltaTime = 0;

    private void OnEnable()
    {
        DistanceStateAPI.OnUpdate += UpdateDeltaTime;
    }

    private void OnDisable()
    {
        DistanceStateAPI.OnUpdate -= UpdateDeltaTime;
    }

    public void UpdateDeltaTime(float deltaTime, float frameCount, Func<Vector3, int> getDistanceCascadeUpdateRate)
    {
        _accumulatedDeltaTime += deltaTime;

        if (frameCount % getDistanceCascadeUpdateRate(WorldPosition) == 0)
        {
            Tick(_accumulatedDeltaTime);
            _accumulatedDeltaTime = 0;
        }
    }

    public abstract void Tick(float accumulatedDeltaTime);
}
