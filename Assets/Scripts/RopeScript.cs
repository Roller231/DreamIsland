using UnityEngine;

public class RopeScript : MonoBehaviour
{
    [Header("Slingshot Settings")]
    [SerializeField] private Transform slingOrigin;  // �����, ��� ������� ������ ������

    private Camera mainCamera;
    private bool isTouching = false;

    private void Start()
    {
        mainCamera = Camera.main;  // �������� �������� ������
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))  // ���������, ���� ������ ��� ��� �����
        {
            isTouching = true;
            RotateObjectToTouch();  // ������� ������� ��� ������ �������
        }
        else if (Input.GetMouseButton(0) && isTouching)  // ���� ��� ������ ��� ����� �� ������
        {
            RotateObjectToTouch();  // ���������� �� ��������� ������
        }
        else if (Input.GetMouseButtonUp(0))  // ����� �����/��� ��������
        {
            isTouching = false;
        }
    }

    private void RotateObjectToTouch()
    {
        // �������� ������� ���� �� ������
        Vector3 mousePosition = Input.mousePosition;

        // ����������� �������� ���������� � ������� � ������� ������
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        RaycastHit hit;

        // ���������� ��� � ��������� ���������
        if (Physics.Raycast(ray, out hit))
        {
            // �������� ����� ���������
            Vector3 targetPosition = hit.point;

            // ������� ������� � ������� ���������
            Vector3 direction = targetPosition - slingOrigin.position;

            // �������� ������� � ������ ���� ����
            Quaternion rotation = Quaternion.LookRotation(-direction);
            slingOrigin.rotation = rotation;  // ��������� �������� �� ���� ����
        }
    }
}
