using Vandals.LockedFrameRate;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    int count;
    bool stop;
    LockedFrameRateUpdate lockedFrameRateUpdate;
    LockedFrameRateUpdate lockedFrameRateUpdate2;

    void Start()
    {
        lockedFrameRateUpdate = LockedFrameRateUpdate.New(50);
        lockedFrameRateUpdate.OnUpdate += LockedUpdate;
        lockedFrameRateUpdate2 = LockedFrameRateUpdate.New(20);
        lockedFrameRateUpdate2.OnUpdate += (float dt) => { Debug.Log("AAAAAAAAAAAAAAAAAAAAA"); };
    }

    private void OnDestroy()
    {
        lockedFrameRateUpdate2.Dispose();
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
