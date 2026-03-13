using UnityEngine;

public class CameraRenderer : MonoBehaviour
{
    public Material camNotFoundMaterial;

    private WebCamTexture webcam;
    private Renderer planeRenderer;

    void Start()
    {
        // Берём Renderer плоскости
        planeRenderer = GetComponent<Renderer>();

        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            // Создаём поток с камеры по умолчанию
            webcam = new WebCamTexture(devices[0].name);

            // Запускаем камеру
            webcam.Play();

            // Назначаем поток как текстуру на материал плоскости
            planeRenderer.material.mainTexture = webcam;
        }
        else
        {
            planeRenderer.material = camNotFoundMaterial;
        }
    }

    void Update()
    {
        // WebCamTexture обновляется автоматически,
        // поэтому в Update ничего делать не нужно.
        // Но если хочешь — можно проверять, работает ли камера:
        if (webcam != null && !webcam.isPlaying)
        {
            webcam.Play();
        }
    }
}
