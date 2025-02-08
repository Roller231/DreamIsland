using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public Transform slingshotOrigin; // ����� �������
    public LineRenderer rubberBandLeft; // ����� �������
    public LineRenderer rubberBandRight; // ������ �������
    public Transform leftAnchor; // ����� ��������� ������� �����
    public Transform rightAnchor; // ����� ��������� ������� ������
    public Transform projectilePrefab; // ������ �������
    public float maxStretch = 5f; // ������������ ���������
    public float releaseForce = 50f; // ���� ��������
    public LayerMask planeLayer; // ���� ��� ����������� ���������

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
        // ������� ��� ����������� �������
        DestroyProjectileAndReset();
    }

    private void StartDrag()
    {
        Vector3 mousePosition = GetMouseWorldPosition();

        // ������ ���������� � ������ �������
        isDragging = true;
        Transform projectileInstance = Instantiate(projectilePrefab, slingshotOrigin.position, Quaternion.identity);
        currentProjectile = projectileInstance.GetComponent<Rigidbody>();
        currentProjectile.isKinematic = true;
    }

    private void Drag()
    {
        if (currentProjectile == null) return;

        Vector3 mousePosition = GetMouseWorldPosition();

        // ������������ ���������
        Vector3 dragVector = slingshotOrigin.position - mousePosition; // ������ ���������
        float dragDistance = Mathf.Clamp(dragVector.magnitude, 0, maxStretch);
        Vector3 dragDirection = dragVector.normalized;

        Vector3 projectilePosition = slingshotOrigin.position - dragDirection * dragDistance;
        currentProjectile.transform.position = projectilePosition;

        // ��������� �������
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
        // ������� ������, ���� �� ����������
        if (currentProjectile != null)
        {
            Destroy(currentProjectile.gameObject);
            currentProjectile = null;
        }

        // ���������� �������
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

        // ���������� ��������� �� ������ �������
        Plane plane = new Plane(Vector3.up, slingshotOrigin.position); // ��������� �� ������ �������
        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance); // ���������� ����� �����������
        }

        return slingshotOrigin.position; // ���� ����������� ���, ���������� ������� �������
    }
}
