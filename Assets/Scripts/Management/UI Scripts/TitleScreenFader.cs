using System.Collections;
using UnityEngine;

namespace Management.UI_Scripts
{
    public class TitleScreenFader :  MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public float fadeOutDuration = 1f;
        public float fadeInDuration = 1f;
        
        public void StartFade(int mode)
        {
            if (mode == 0) StartCoroutine(FadeOut());
            else if (mode == 1)
            {
                gameObject.SetActive(true);
                StartCoroutine(FadeIn());
            }
        }

        private IEnumerator FadeOut()
        {
            float startAlpha = 1;
            
            for (float t = 0; t < fadeOutDuration; t += Time.deltaTime)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t / fadeOutDuration);
                yield return null;
            }
            
            canvasGroup.alpha = 0; // Ensure it's fully transparent at the end
            gameObject.SetActive(false);
        }
        
        private IEnumerator FadeIn()
        {
            float startAlpha = 0;
            
            for (float t = 0; t < fadeInDuration; t += Time.deltaTime)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 1, t / fadeInDuration);
                yield return null;
            }
            
            canvasGroup.alpha = 1; // Ensure it's fully transparent at the end
        }
    }
}