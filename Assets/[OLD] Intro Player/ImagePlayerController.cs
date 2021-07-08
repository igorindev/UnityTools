using System.Collections;
using UnityEngine;

public class ImagePlayerController : MonoBehaviour
{
    CanvasGroup canvasGroup;
    [SerializeField] CanvasGroup[] spriteClips;

    [SerializeField] float fadeTime = 0.4f;
    [SerializeField] float imageDuration = 3;
    int actual = 0;

    void Start()
    {
        actual = 0;
        canvasGroup = spriteClips[0];
    }

    public void Initialize()
    {
        actual = 0;
        canvasGroup.alpha = 0;
        StartCoroutine(Play());
    }

    void PlayNext()
    {
        //Troca o sprite
        canvasGroup = spriteClips[actual];
        StartCoroutine(Play());
    }
    IEnumerator Play()
    {
        float counter = 0;

        while (counter < fadeTime)
        {
            counter += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, counter / fadeTime);

            yield return null;
        }
        canvasGroup.alpha = 1;
        yield return new WaitForSeconds(imageDuration);

        counter = 0;
        while (counter < fadeTime)
        {
            counter += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, counter / fadeTime);

            yield return null;
        }
        canvasGroup.alpha = 0;
        actual += 1;

        if (actual < spriteClips.Length)
        {
            PlayNext();
        }
    }
}
