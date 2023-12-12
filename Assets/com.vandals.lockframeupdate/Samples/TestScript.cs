using UnityEngine;

public class TestScript : MonoBehaviour
{
    int count;
    bool stop;
    LockedFrameRateUpdate lockedFrameRateUpdate;

    void Start()
    {
        lockedFrameRateUpdate = LockedFrameRateUpdate.New(50);
        lockedFrameRateUpdate.OnUpdate += LockedUpdate;
    }

    private void LockedUpdate(float obj)
    {
        count += 1; 
        Debug.Log("LockedFrameRateUpdate");

        if (count == 100)
        {
            stop = true;
            lockedFrameRateUpdate.OnUpdate = null;
        }
    }

    private void Update()
    {
        if (stop) return;

        Debug.Log("Update");
    }

    private void FixedUpdate()
    {
        if (stop) return;

        Debug.Log("FixedUpdate");
    }
}
