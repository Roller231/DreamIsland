using UnityEngine;

public class RopeScript : MonoBehaviour
{
    [Header("Slingshot Settings")]
    [SerializeField] private Transform slingOrigin;  // Точка, где рогатка держит снаряд

    private Camera mainCamera;
    private bool isTouching = false;

    private void Start()
    {
        mainCamera = Camera.main;  // Получаем основную камеру
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))  // Проверяем, если нажата ЛКМ или палец
        {
            isTouching = true;
            RotateObjectToTouch();  // Поворот объекта при первом касании
        }
        else if (Input.GetMouseButton(0) && isTouching)  // Если ЛКМ зажата или палец на экране
        {
            RotateObjectToTouch();  // Следование за движением пальца
        }
        else if (Input.GetMouseButtonUp(0))  // Когда палец/ЛКМ отпущены
        {
            isTouching = false;
        }
    }

    private void RotateObjectToTouch()
    {
        // Получаем позицию мыши на экране
        Vector3 mousePosition = Input.mousePosition;

        // Преобразуем экранные координаты в мировые с помощью камеры
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        RaycastHit hit;

        // Отправляем луч и проверяем попадание
        if (Physics.Raycast(ray, out hit))
        {
            // Получаем точку попадания
            Vector3 targetPosition = hit.point;

            // Поворот объекта в сторону попадания
            Vector3 direction = targetPosition - slingOrigin.position;

            // Вращение объекта с учетом всех осей
            Quaternion rotation = Quaternion.LookRotation(-direction);
            slingOrigin.rotation = rotation;  // Применяем вращение по всем осям
        }
    }
}
