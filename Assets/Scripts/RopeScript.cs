using UnityEngine;

public class RopeScript : MonoBehaviour
{
    [Header("Slingshot Settings")]
    [SerializeField] private Transform slingOrigin; // ����� �������� �������
    [SerializeField] private Transform bulletOrigin; // ����� ������ �������
    [SerializeField] private GameObject projectilePrefab; // ������ �������
    [SerializeField] private float minForce = 5f; // ����������� ����
    [SerializeField] private float maxForce = 20f; // ������������ ����

    private Camera mainCamera;
    private bool isTouching = false;
    private float touchDuration = 0f; // ����� �������
    private GameObject currentProjectile; // ������� ������
    private Rigidbody projectileRigidbody;
    private Vector3 shootDirection; // ����������� ��������

    private Animator animator;

    private void Start()
    {
        mainCamera = Camera.main; // �������� �������� ������
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ������� ��� ��� �������
        {
            isTouching = true;
            touchDuration = 0f; // ���������� ������
            SpawnProjectile(); // ������ ������
            CalculateShootDirection(); // ������������ ����������� ��������
            RotateObjectToTouch();
            animator.SetBool("touch", true);
        }
        else if (Input.GetMouseButton(0) && isTouching) // ���������
        {
            touchDuration += Time.deltaTime; // ����������� ����� ���������
            RotateObjectToTouch();
            CalculateShootDirection(); // ��������� ����������� ��� ���������
        }
        else if (Input.GetMouseButtonUp(0)) // ����������
        {
            isTouching = false;
            ShootProjectile(); // ������������ ������
            animator.SetTrigger("shoot");
            animator.SetBool("touch", false);
        }
    }

    private void SpawnProjectile()
    {
        // ������ ������ � ����� ������
        currentProjectile = Instantiate(projectilePrefab, bulletOrigin.position, bulletOrigin.rotation);
        currentProjectile.transform.SetParent(bulletOrigin); // ������ ������ �������� �������� ����� ������

        // �������� Rigidbody � ����������� ���
        projectileRigidbody = currentProjectile.GetComponent<Rigidbody>();
        if (projectileRigidbody != null)
        {
            projectileRigidbody.isKinematic = true; // ������ ������ ����������� (���� �� ���������)
            projectileRigidbody.useGravity = false; // ��������� ����������
        }
    }

    private void ShootProjectile()
    {
        if (currentProjectile != null && projectileRigidbody != null)
        {
            // ����������� ������ �� ��������
            currentProjectile.transform.SetParent(null);

            // ������������ ���� ��������
            float force = Mathf.Lerp(minForce, maxForce, touchDuration / 2f); // ������������ �������� �� 2 ���

            // �������� ������ � ��������� ����
            projectileRigidbody.isKinematic = false;
            projectileRigidbody.useGravity = true;
            projectileRigidbody.linearVelocity = Vector3.zero; // �������� ������� ��������
            projectileRigidbody.angularVelocity = Vector3.zero; // �������� ������� ��������
            projectileRigidbody.AddForce(shootDirection.normalized * force, ForceMode.Impulse); // ��������� ����

            // ������� ������ ����� 3 �������
            Destroy(currentProjectile, 3f);

            // ���������� ������� ������
            currentProjectile = null;
        }
    }

    private void CalculateShootDirection()
    {
        Vector3 mousePosition = Input.mousePosition;

        // ����������� �������� ���������� � ������� � ������� ������
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // ���� ��� ����� � ������, ����������� - �� ������� � ����� ���������
            shootDirection = hit.point - bulletOrigin.position;
        }
        else
        {
            // ���� ��� ������ �� �����, �������� � ����������� ������
            shootDirection = ray.direction;
        }
    }

    private void RotateObjectToTouch()
    {
        Vector3 mousePosition = Input.mousePosition;

        // ����������� �������� ���������� � ������� � ������� ������
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;
            Vector3 direction = targetPosition - slingOrigin.position;
            Quaternion rotation = Quaternion.LookRotation(-direction);
            slingOrigin.rotation = rotation;
        }
    }
}
