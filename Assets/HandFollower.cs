using Leap;
using UnityEngine;
using UnityEngine.UIElements;

public class HandFollower : MonoBehaviour
{
    public LeapServiceProvider provider;

    public Transform cameraPlane;   // Плоскость с камерой
    public Transform ruler;         // Линейка
    private Animator anim;          // Animator ручки

    private bool isPinching;
    private float initialPinchZ;
    private Vector3 initialScale;

    private float leftEdge;
    private float rightEdge;

    // Константы
    private const float minScale = 0.1f;
    private const float maxScale = 10f;
    private const float aspect = 9f / 16f;
    private const float scaleMultiplier = 20f;

    void Start()
    {
        // Leap provider
        if (!provider)
            provider = FindObjectOfType<LeapServiceProvider>();

        // Animator
        anim = GetComponent<Animator>();

        // Вычисляем границы линейки
        Renderer r = ruler.GetComponent<Renderer>();
        float length = r.bounds.size.x;

        leftEdge = ruler.position.x - length * 0.5f;
        rightEdge = ruler.position.x + length * 0.5f;

        MoveKnob(cameraPlane.localScale.x);
    }

    void Update()
    {
        if (!provider) return;

        Frame frame = provider.CurrentFrame;
        if (frame == null || frame.Hands.Count == 0) return;

        Hand hand = frame.Hands[0];
        bool pinchNow = hand.PinchStrength > 0.8f;

        // Pinch начался
        if (pinchNow && !isPinching)
        {
            isPinching = true;
            initialPinchZ = hand.PalmPosition.z;
            initialScale = cameraPlane.localScale;

            // Передаём состояние pinch в Animator
            anim.SetBool("pinch", true);
        }
        else if (!pinchNow && isPinching) // Pinch закончился
        {
            isPinching = false;
            anim.SetBool("pinch", false);
        }

        if (!isPinching) return;

        // --- МАСШТАБИРОВАНИЕ ПЛОСКОСТИ ---
        float delta = (hand.PalmPosition.z - initialPinchZ) * scaleMultiplier;
        float newScaleX = Mathf.Clamp(initialScale.x + delta, minScale, maxScale);

        cameraPlane.localScale = new Vector3(
            newScaleX,
            1f,
            newScaleX * aspect
        );

        MoveKnob(newScaleX);
    }

    private void MoveKnob(float scaleX)
    {
        // --- ДВИЖЕНИЕ РУЧКИ ПО ЛИНЕЙКЕ ---
        float t = (scaleX - minScale) / (maxScale - minScale); // нормализуем 0..1
        float xPos = Mathf.Lerp(leftEdge, rightEdge, t);

        transform.position = new Vector3(
            xPos,
            transform.position.y,
            transform.position.z
        );
    }
}
