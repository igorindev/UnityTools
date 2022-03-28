//This code is based on DitzelGames tutorial https://www.youtube.com/watch?v=7noMEjDJ-_Q

using System.Collections;
using UnityEngine;

namespace TPSCameraController
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] Vector3 amount = new Vector3(1f, 1f, 0f);
        [SerializeField] float duration = 1f;
        [SerializeField] float speed = 10f;
        [SerializeField] AnimationCurve curve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        [SerializeField] bool deltaMovement = true;

        Camera playerCamera;
        float time = 0;
        Vector3 lastPos;
        Vector3 nextPos;
        float lastFOV;
        float nextFOV;

        Coroutine shake;
        bool isShaking;

        void Awake()
        {
            playerCamera = GetComponent<Camera>();
        }

        [ContextMenu("Play")]
        public void Shake()
        {
            ShakeCamera();
        }
        public void Shake(Vector3 direction)
        {
            amount = direction;

            ShakeCamera();
        }
        public void Shake(float _duration = 1f, float _speed = 10f, Vector3? _amount = null, Camera _camera = null, bool _deltaMovement = true, AnimationCurve _curve = null)
        {
            duration = _duration;
            speed = _speed;

            if (_amount != null)
            {
                amount = (Vector3)_amount;
            }

            if (_curve != null)
            {
                curve = _curve;
            }

            deltaMovement = _deltaMovement;

            ShakeCamera();
        }

        /// <summary>
        /// Execute Shake
        /// </summary>
        void ShakeCamera()
        {
            if (!isShaking)
            {
                ResetCam();
                time = duration;

                if (shake != null)
                {
                    StopCoroutine(shake);
                }

                shake = StartCoroutine(ShakeCoroutine());
            }
        }

        /// <summary>
        /// Reset camera Settings
        /// </summary>
        void ResetCam()
        {
            playerCamera.transform.Translate(deltaMovement ? -lastPos : Vector3.zero);
            playerCamera.fieldOfView -= lastFOV;

            //Clear values
            lastPos = nextPos = Vector3.zero;
            lastFOV = nextFOV = 0f;
        }

        IEnumerator ShakeCoroutine()
        {
            isShaking = true;

            while (time > 0)
            {
                time -= Time.deltaTime;

                nextPos = (Mathf.PerlinNoise(time * speed, time * 2 * speed) - 0.5f) * transform.right * amount.x * curve.Evaluate(1f - time / duration) +
                          (Mathf.PerlinNoise(time * 2 * speed, time * speed) - 0.5f) * transform.up * amount.y * curve.Evaluate(1f - time / duration);
                nextFOV = (Mathf.PerlinNoise(time * 2 * speed, time * 2 * speed) - 0.5f) * amount.z * curve.Evaluate(1f - time / duration);

                playerCamera.fieldOfView += (nextFOV - lastFOV);
                playerCamera.transform.Translate(deltaMovement ? (nextPos - lastPos) : nextPos);

                lastPos = nextPos;
                lastFOV = nextFOV;

                yield return new WaitForEndOfFrame();
            }

            ResetCam();
            isShaking = false;
        }
    }
}