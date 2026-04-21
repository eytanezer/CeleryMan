using System.Collections;
using UnityEngine;
using DG.Tweening; // השורה הזו חובה כדי להשתמש ב-DOTween!

public class GrowingItem : MonoBehaviour
{
    [Header("Grow Settings")]
    [Tooltip("How long it takes to reach full size (seconds)")]
    private const float GrowDuration = 2f;

    [Header("Despawn Settings")]
    [Tooltip("How long the item will stay on the ground before disappearing if not collected")]
    private const float DespawnTime = 12f;

    private Vector3 _targetScale;

    private void Awake()
    {
        // שומרים את הגודל המקורי
        _targetScale = transform.localScale;
        
        // מתחילים מגודל אפס
        transform.localScale = Vector3.zero;
    }

    private void Start()
    {
        // הקסם של DOTween: גדילה בשורה אחת עם אפקט קפיצי
        transform.DOScale(_targetScale, GrowDuration)
            .SetEase(Ease.OutBack) // OutBack גורם לו לגדול טיפה מעבר לגודל המקורי ולחזור (קפיצי)
            .SetLink(gameObject)
            .OnComplete(() => 
            {
                // הפקודה הזו תרוץ אוטומטית כשהאנימציה תסתיים
                StartCoroutine(DespawnRoutine());
            });
    }

    private IEnumerator DespawnRoutine()
    {
        // מחכים את הזמן שהגדרנו
        yield return new WaitForSeconds(DespawnTime);

        // בונוס DOTween: במקום פשוט להרוס את החפץ, נכווץ אותו קודם!
        transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InBack) // הפעם מתכווץ פנימה עם תנופה
            .SetLink(gameObject)
            .OnComplete(() => 
            {
                // רק כשההתכווצות מסתיימת - נמחק את האובייקט מהזיכרון
                Destroy(gameObject);
            });
    }
}