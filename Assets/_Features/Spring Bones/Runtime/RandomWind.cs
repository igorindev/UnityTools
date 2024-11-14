using UnityEngine;
using System.Collections;

namespace SpringSystem
{
    public class RandomWind : MonoBehaviour
    {
        SpringManager springManager;

        [SerializeField] bool isWindActive = false;

        [SerializeField] float threshold = 0.5f;  
        [SerializeField] float interval = 5.0f;

        [SerializeField] float windPower = 1.0f;
        [SerializeField] float maxWindSpeed = 10f;

        [SerializeField] float gravity = 0.98f;
        [SerializeField] bool isGUI = true;

        private bool isMinus = false;
        private Vector3 windSpeed;
        private float windMagnitude;
        private Vector3 currentForce;

        void Start()
        {
            springManager = GetComponent<SpringManager>();
            StartCoroutine(RandomChange());
        }

        void Update()
        {

            if (isWindActive)
            {
                var windAcceleration = new Vector3(Mathf.PerlinNoise(Time.time, 0.0f) * windPower * (isMinus ? -0.001f : 0.001f), gravity * -0.001f, 0);

                windSpeed = windSpeed + windAcceleration * Time.deltaTime;
                windSpeed = Vector3.ClampMagnitude(windSpeed, maxWindSpeed);
                windMagnitude = windSpeed.magnitude;
                springManager.ApplyForce(windSpeed);
            }
        }

        private IEnumerator RandomChange()
        {
            while (true)
            {
                float _seed = Random.value;
                if (_seed > threshold)
                {
                    isMinus = true;
                }
                else
                {
                    isMinus = false;
                }

                yield return new WaitForSeconds(interval);
            }
        }

        void OnGUI()
        {
            if (isGUI)
            {
                Rect rect1 = new Rect(10, Screen.height - 40, 400, 30);
                isWindActive = GUI.Toggle(rect1, isWindActive, "Random Wind");
            }
        }
    }
}
