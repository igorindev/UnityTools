using System.Collections.Generic;
using UnityEngine;

namespace Vandals.LockedFrameRate
{
    public class LockedFrameRateUpdateManager : MonoBehaviour
    {
        readonly List<LockedFrameRateUpdate> lockedFrameRateUpdates = new List<LockedFrameRateUpdate>();

        internal void Add(LockedFrameRateUpdate lockedFrameRateUpdate)
        {
            lockedFrameRateUpdates.Add(lockedFrameRateUpdate);
            enabled = true;
        }

        internal void Remove(LockedFrameRateUpdate lockedFrameRateUpdate)
        {
            lockedFrameRateUpdates.Remove(lockedFrameRateUpdate);

            if (lockedFrameRateUpdates.Count == 0)
            {
                enabled = false;
            }
        }

        void Update()
        {
            foreach (LockedFrameRateUpdate item in lockedFrameRateUpdates)
            {
                item.Update(Time.deltaTime);
            }
        }
    }
}