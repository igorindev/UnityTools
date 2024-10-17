using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSubsystemQA : MonoBehaviour
{
    [ContextMenu("Test Audio Reset")]
    void Test()
    {
        UnityEngine.AudioSettings.Reset(UnityEngine.AudioSettings.GetConfiguration());
    }
}
