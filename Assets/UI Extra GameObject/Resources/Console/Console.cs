using UnityEngine;
using UnityEngine.UI;

namespace DebugStuff
{
    public class Console : MonoBehaviour
    {
        //#if !UNITY_EDITOR
        [SerializeField] Text text;
        static string myLog = "";
        private string output;
        private string stack;

        bool open = true;

        void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        void OnDisable()
        {
            Clear();
            Application.logMessageReceived -= Log;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                text.gameObject.SetActive(open = !open);
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                Time.timeScale = 1;
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                Time.timeScale = 0;
            }
            else if (Input.GetKeyDown(KeyCode.F4))
            {
                Clear();
            }
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            stack = stackTrace;
            output = logString + "\n" + stack;

            if (type == LogType.Error)
            {
                output = "<color=red>" + output + "</color>";
            }
            else if (type == LogType.Warning)
            {
                output = "<color=yellow>" + output + "</color>";
            }

            myLog = output + "\n" + myLog;
            if (myLog.Length > 5000)
            {
                myLog = myLog.Substring(0, 4000);
            }

            text.text = myLog;
        }

        [ContextMenu("Clear")]
        void Clear()
        {
            myLog = text.text = "";
        }

        [ContextMenu("Normal")]
        void DebugNormal()
        {
            Debug.Log("TestNormal");
        }
        [ContextMenu("Error")]
        void DebugError()
        {
            Debug.LogError("TestError");
        }
        [ContextMenu("Warning")]
        void DebugWarning()
        {
            Debug.LogWarning("TestError");
        }
    }
}
