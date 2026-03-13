using Leap;
using UnityEngine;

public class HandFollower : MonoBehaviour
{
    public LeapServiceProvider provider;

    public Transform cameraPlane;   // Плоскость с камерой
    public Transform ruler;         // Линейка
    private Animator anim;          // Animator ручки

    private int _handAxisIndex = 2; // 0 - X, 1 - Y, 2 - Z (ось для масштабирования)

    private bool _isPinching;
    private Vector3 _startPinchLocation;
    private Vector3 _startPinchScale;

    private float _leftEdge;
    private float _rightEdge;

    // Константы
    private const float MinScale = 0.2f;
    private const float MaxScale = 7f;
    private const float Aspect = 9f / 16f;
    private const float ScaleMultiplier = 20f;

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

        _leftEdge = ruler.position.x - length * 0.5f;
        _rightEdge = ruler.position.x + length * 0.5f;

        MoveKnob(Mathf.Abs(cameraPlane.localScale.x));
    }

    void Update()
    {
        if (!provider) return;

        Frame frame = provider.CurrentFrame;
        if (frame == null || frame.Hands.Count == 0) return;

        var hand = frame.Hands[0];
        bool pinchNow = hand.PinchStrength > 0.8f;

        if (pinchNow && !_isPinching)   // Pinch начался
        {
            _isPinching = true;
            _startPinchLocation = hand.PalmPosition;
            _startPinchScale = cameraPlane.localScale;

            // Передаём состояние pinch в Animator
            anim.SetBool("pinch", true);
        }
        else if (!pinchNow && _isPinching) // Pinch закончился
        {
            _isPinching = false;
            anim.SetBool("pinch", false);
        }

        if (!_isPinching) return;

        var newScaleX = GetNewScale(ref hand.PalmPosition);
        
        ScalePlane(newScaleX);

        MoveKnob(Mathf.Abs(newScaleX));
    }

    private float GetNewScale(ref Vector3 handLocation)
    {
        // --- МАСШТАБИРОВАНИЕ ПЛОСКОСТИ ---
        float delta = (handLocation[_handAxisIndex] - _startPinchLocation[_handAxisIndex]) * ScaleMultiplier;
        return Mathf.Clamp(Mathf.Abs(_startPinchScale.x) + delta, MinScale, MaxScale);
    }

    private void ScalePlane(float scaleX)
    {
        cameraPlane.localScale = new Vector3(
            -scaleX,
            1f,
            scaleX * Aspect
        );
    }

    private void MoveKnob(float scaleX)
    {
        // --- ДВИЖЕНИЕ РУЧКИ ПО ЛИНЕЙКЕ ---
        float t = (scaleX - MinScale) / (MaxScale - MinScale); // нормализуем 0..1
        float xPos = Mathf.Lerp(_leftEdge, _rightEdge, t);

        transform.position = new Vector3(
            xPos,
            transform.position.y,
            transform.position.z
        );
    }
}
