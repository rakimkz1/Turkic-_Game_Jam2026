using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Максимальная длительность заморозки в секундах")]
    [SerializeField] private float maxDuration = 0.2f;

    [Tooltip("Множитель: скорость * intensity = длительность заморозки")]
    [SerializeField] private float intensity = 0.015f;

    [Tooltip("Time.timeScale во время заморозки (0 = полная остановка)")]
    [SerializeField, Range(0f, 1f)] private float frozenTimeScale = 0f;

    [Tooltip("Плавный выход из заморозки (сек). 0 = мгновенный")]
    [SerializeField] private float easeOutDuration = 0.05f;

    // -------------------------------------------------------
    // Публичные свойства
    // -------------------------------------------------------

    public bool IsActive { get; private set; }

    public float RemainingTime { get; private set; }

    private Coroutine _hitStopCoroutine;
    public void Trigger(float speed)
    {
        float duration = Mathf.Min(speed * intensity, maxDuration);

        // Если уже активен — перезапускаем только если новый удар сильнее
        if (IsActive && duration <= RemainingTime)
            return;

        if (_hitStopCoroutine != null)
            StopCoroutine(_hitStopCoroutine);

        _hitStopCoroutine = StartCoroutine(HitStopRoutine(duration));
    }

    public void Trigger(Vector2 velocity) => Trigger(velocity.magnitude);

    public void Trigger(Vector3 velocity) => Trigger(velocity.magnitude);

    public void Cancel()
    {
        if (_hitStopCoroutine != null)
            StopCoroutine(_hitStopCoroutine);

        RestoreTime();
    }


    private IEnumerator HitStopRoutine(float duration)
    {
        IsActive = true;
        RemainingTime = duration;

        // Заморозка
        Time.timeScale = frozenTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // Ждём в реальном времени (WaitForSecondsRealtime игнорирует timeScale)
        while (RemainingTime > 0f)
        {
            RemainingTime -= Time.unscaledDeltaTime;
            yield return null;
        }

        RemainingTime = 0f;

        // Плавный выход
        if (easeOutDuration > 0f)
        {
            float elapsed = 0f;
            while (elapsed < easeOutDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / easeOutDuration);
                Time.timeScale = Mathf.Lerp(frozenTimeScale, 1f, t);
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                yield return null;
            }
        }

        RestoreTime();
    }

    private void RestoreTime()
    {
        IsActive = false;
        RemainingTime = 0f;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        _hitStopCoroutine = null;
    }

    // Защита на случай уничтожения объекта
    private void OnDestroy()
    {
        if (IsActive)
            RestoreTime();
    }
}