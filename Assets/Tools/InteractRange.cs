using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractRange
{
    public static class Angles
    {
        /// <summary>
        /// Return true if the target is in distance and the direction passed is in the angle
        /// </summary>
        /// <param name="me"></param>
        /// <param name="target"></param>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="interactAngle"></param>
        /// <param name="interactDistance"></param>
        /// <returns></returns>
        public static bool InRange(Vector3 me, Vector3 target, Vector3 origin, Vector3 direction , float interactAngle, float interactDistance)
        {
            if (Vector3.Distance(me, target) <= interactDistance)
            {
                float cosAngle = Vector3.Dot((target - origin).normalized, direction);
                float angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;

                if (angle < interactAngle)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Return true if the target is inside the angle
        /// </summary>
        /// <param name="targetPos"></param>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <param name="interactAngle"></param>
        /// <returns></returns>
        public static bool TargetDirection(Vector3 targetPos, Vector3 origin, Vector3 direction, float interactAngle)
        {
            float cosAngle = Vector3.Dot((targetPos - origin).normalized, direction);
            float angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;

            if (angle < interactAngle)
            {
                return true;
            }
            return false;
        }
    }
}



