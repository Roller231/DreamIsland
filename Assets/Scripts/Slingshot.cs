using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public Transform slingshotOrigin; // Центр рогатки
    public LineRenderer rubberBandLeft; // Левая резинка
    public LineRenderer rubberBandRight; // Правая резинка
    public Transform leftAnchor; // Точка крепления резинки слева
    public Transform rightAnchor; // Точка крепления резинки справа
    public Transform projectilePrefab; // Префаб снаряда
    public float maxStretch = 5f; // Максимальное натяжение
    public float releaseForce = 50f; // Сила выстрела
    public LayerMask planeLayer; // Слой для определения плоскости

    private Rigidbody currentProjectile;
    private bool isDragging = false;

    void Start()
    {
        ResetRubberBands();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDrag();
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Drag();
        }

        if (isDragging && Input.GetMouseButtonUp(0))
        {
            Release();
        }
    }

    private void OnDisable()
    {
        // Очистка при деактивации объекта
        DestroyProjectileAndReset();
    }

    private void StartDrag()
    {
        Vector3 mousePosition = GetMouseWorldPosition();

        // Снаряд появляется в центре рогатки
        isDragging = true;
        Transform projectileInstance = Instantiate(projectilePrefab, slingshotOrigin.position, Quaternion.identity);
        currentProjectile = projectileInstance.GetComponent<Rigidbody>();
        currentProjectile.isKinematic = true;
    }

    private void Drag()
    {
        if (currentProjectile == null) return;

        Vector3 mousePosition = GetMouseWorldPosition();

        // Ограничиваем натяжение
        Vector3 dragVector = slingshotOrigin.position - mousePosition; // Вектор натяжения
        float dragDistance = Mathf.Clamp(dragVector.magnitude, 0, maxStretch);
        Vector3 dragDirection = dragVector.normalized;

        Vector3 projectilePosition = slingshotOrigin.position - dragDirection * dragDistance;
        currentProjectile.transform.position = projectilePosition;

        // Обновляем резинки
        UpdateRubberBands(projectilePosition);
    }

    private void Release()
    {
        isDragging = false;
        ResetRubberBands();

        if (currentProjectile != null)
        {
            currentProjectile.isKinematic = false;

            Vector3 releaseDirection = (slingshotOrigin.position - currentProjectile.position).normalized;
            float stretchDistance = Vector3.Distance(slingshotOrigin.position, currentProjectile.position);
            currentProjectile.AddForce(releaseDirection * stretchDistance * releaseForce, ForceMode.Impulse);

            currentProjectile = null;
        }
    }

    private void DestroyProjectileAndReset()
    {
        // Удаляем снаряд, если он существует
        if (currentProjectile != null)
        {
            Destroy(currentProjectile.gameObject);
            currentProjectile = null;
        }

        // Сбрасываем резинки
        ResetRubberBands();
    }

    private void UpdateRubberBands(Vector3 projectilePosition)
    {
        rubberBandLeft.SetPosition(0, leftAnchor.position);
        rubberBandLeft.SetPosition(1, projectilePosition);

        rubberBandRight.SetPosition(0, rightAnchor.position);
        rubberBandRight.SetPosition(1, projectilePosition);
    }

    private void ResetRubberBands()
    {
        rubberBandLeft.SetPosition(0, leftAnchor.position);
        rubberBandLeft.SetPosition(1, leftAnchor.position);

        rubberBandRight.SetPosition(0, rightAnchor.position);
        rubberBandRight.SetPosition(1, rightAnchor.position);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Используем плоскость на уровне рогатки
        Plane plane = new Plane(Vector3.up, slingshotOrigin.position); // Плоскость на уровне рогатки
        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance); // Возвращаем точку пересечения
        }

        return slingshotOrigin.position; // Если пересечения нет, возвращаем позицию рогатки
    }
}
