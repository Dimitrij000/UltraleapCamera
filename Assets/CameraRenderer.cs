using UnityEngine;

public class CameraRenderer : MonoBehaviour
{
    private WebCamTexture webcam;
    private Renderer planeRenderer;

    void Start()
    {
        // Берём Renderer плоскости
        planeRenderer = GetComponent<Renderer>();

        // Создаём поток с камеры по умолчанию
        webcam = new WebCamTexture("USB Video Device");

        // Запускаем камеру
        webcam.Play();

        // Назначаем поток как текстуру на материал плоскости
        planeRenderer.material.mainTexture = webcam;
    }

    void Update()
    {
        // WebCamTexture обновляется автоматически,
        // поэтому в Update ничего делать не нужно.
        // Но если хочешь — можно проверять, работает ли камера:
        if (!webcam.isPlaying)
        {
            webcam.Play();
        }
    }
}
