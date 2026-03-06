using UnityEngine;
using Leap;

public class HandFollower : MonoBehaviour
{
    public LeapServiceProvider provider;

    void Start()
    {
        // Если провайдер не назначен вручную — ищем его в сцене
        if (provider == null)
            provider = FindObjectOfType<LeapServiceProvider>();
    }

    void Update()
    {
        if (provider == null) return;

        // Получаем текущий кадр с данными рук
        Frame frame = provider.CurrentFrame;
        if (frame == null || frame.Hands.Count == 0) return;

        // Берём первую руку
        Hand hand = frame.Hands[0];
        if (!hand.IsPinching()) return;

        // Позиция ладони в Unity-координатах
        Vector3 palm = new Vector3(
            hand.PalmPosition.x,
            hand.PalmPosition.y,
            hand.PalmPosition.z
        );

        // Двигаем объект полностью за рукой
        transform.position = palm;
    }
}
