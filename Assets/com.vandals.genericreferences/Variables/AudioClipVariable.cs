using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(menuName = "Variable/Audio Clip")]
public class AudioClipVariable : Variable<AudioClip>
{
    public void Execute(BaseEventData eventBseData)
    {
        //AudioManager.Instance.Play(value, value.music);
    }
}