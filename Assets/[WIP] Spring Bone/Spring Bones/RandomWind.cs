using UnityEngine;
using System.Collections;

namespace SpringSystem
{
    public class RandomWind : MonoBehaviour
    {
        SpringBone[] springBones;
        public bool isWindActive = false;

        public float threshold = 0.5f;  
        public float interval = 5.0f;   
        public float windPower = 1.0f;  
        public float gravity = 0.98f;	
        public bool isGUI = true;
        bool isMinus = false;

        void Start()
        {
            springBones = GetComponent<SpringManager>().springBones;
            StartCoroutine(RandomChange());
        }

        void Update()
        {
            Vector3 force;
            if (isWindActive)
            {
                if (isMinus)
                {
                    force = new Vector3(Mathf.PerlinNoise(Time.time, 0.0f) * windPower * -0.001f, gravity * -0.001f, 0);
                }
                else
                {
                    force = new Vector3(Mathf.PerlinNoise(Time.time, 0.0f) * windPower * 0.001f, gravity * -0.001f, 0);
                }

                for (int i = 0; i < springBones.Length; i++)
                {
                    springBones[i].springForce = force;
                }
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

        IEnumerator RandomChange()
        {
            // 無限ループ開始.
            while (true)
            {
                //ランダム判定用シード発生.
                float _seed = Random.Range(0.0f, 1.0f);

                if (_seed > threshold)
                {
                    //_seedがthreshold以上の時、符号を反転する.
                    isMinus = true;
                }
                else
                {
                    isMinus = false;
                }

                // 次の判定までインターバルを置く.
                yield return new WaitForSeconds(interval);
            }
        }
    }
}