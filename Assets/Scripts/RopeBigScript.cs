using System.Collections;
using UnityEngine;
using UTeleApp;

public class RopeBigScript : MonoBehaviour
{
    [Header("Slingshot Settings")]
    [SerializeField] private Transform slingOrigin; // Начало рогатки
    [SerializeField] private Transform bulletOrigin; // Позиция появления снаряда
    [SerializeField] private GameObject projectilePrefab; // Префаб снаряда
    [SerializeField] private float minForce = 5f; // Минимальная сила выстрела
    [SerializeField] private float maxForce = 20f; // Максимальная сила выстрела
    [SerializeField] private AudioClip startRoping; // Звук натяжения
    [SerializeField] private AudioClip shoot; // Звук выстрела

    private Camera mainCamera;
    private bool isTouching = false;
    private float touchDuration = 0f; // Время натяжения
    private GameObject currentProjectile; // Текущий снаряд
    [HideInInspector] public Rigidbody projectileRigidbody;
    private Vector3 shootDirection; // Направление выстрела
    private AudioSource audioSource; // Источник звука
    private Animator animator;

    private void Start()
    {
        mainCamera = Camera.main; // Ссылка на основную камеру
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Начало нажатия
        {
            if (TryGetHitPoint(out shootDirection)) // Проверяем, попадает ли рейкаст в коллайдер
            {
                isTouching = true;
                touchDuration = 0f; // Сбрасываем время натяжения
                SpawnProjectile(); // Создаём снаряд
                RotateObjectToTouch();
                audioSource.clip = startRoping;
                audioSource.Play();
                animator.SetBool("touch", true);
            }
            else
            {
                // Удаляем текущий объект, если он уже был создан
                DestroyCurrentProjectile();
            }
        }
        else if (Input.GetMouseButton(0) && isTouching) // Удержание
        {
            touchDuration += Time.deltaTime; // Увеличиваем время натяжения
            RotateObjectToTouch();
            CalculateShootDirection(); // Обновляем направление
        }
        else if (Input.GetMouseButtonUp(0)) // Отпускание
        {
            if (isTouching)
            {
                isTouching = false;
                ShootProjectile(); // Выстрел
                animator.SetTrigger("shoot");
                audioSource.clip = shoot;
                audioSource.Play();
            }
            else
            {
                // Если не стреляет, удаляем текущий снаряд
                DestroyCurrentProjectile();
            }

            animator.SetBool("touch", false);
        }
    }

    private void SpawnProjectile()
    {
        // Создаём снаряд в начальной позиции
        currentProjectile = Instantiate(projectilePrefab, bulletOrigin.position, bulletOrigin.rotation);
        currentProjectile.transform.SetParent(bulletOrigin); // Привязываем снаряд к рогатке

        // Настраиваем Rigidbody снаряда
        projectileRigidbody = currentProjectile.GetComponent<Rigidbody>();
        if (projectileRigidbody != null)
        {
            projectileRigidbody.isKinematic = true; // Отключаем физику
            projectileRigidbody.useGravity = false; // Отключаем гравитацию
        }
    }

    private void ShootProjectile()
    {
        if (currentProjectile != null && projectileRigidbody != null && shootDirection != Vector3.zero)
        {
            // Отсоединяем снаряд
            currentProjectile.transform.SetParent(null);

            // Рассчитываем силу выстрела
            float force = Mathf.Lerp(minForce, maxForce, touchDuration / 2f);

            // Применяем силу к снаряду
            projectileRigidbody.isKinematic = false;
            projectileRigidbody.useGravity = true;
            projectileRigidbody.linearVelocity = Vector3.zero;
            projectileRigidbody.angularVelocity = Vector3.zero;
            projectileRigidbody.AddForce(shootDirection.normalized * force, ForceMode.Impulse);

            // Очищаем текущий снаряд
            currentProjectile = null;
        }
        else
        {
            DestroyCurrentProjectile(); // Удаляем объект, если выстрел невозможен
        }
    }

    private void CalculateShootDirection()
    {
        if (!TryGetHitPoint(out shootDirection))
        {
            shootDirection = Vector3.zero;
        }
    }

    private void RotateObjectToTouch()
    {
        Vector3 mousePosition = Input.mousePosition;

        // Преобразование позиции мыши в мировые координаты через луч
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "Bullet") return;

            // Инвертируем направление, чтобы рогатка смотрела в противоположную сторону
            Vector3 targetPosition = hit.point;
            Vector3 direction = targetPosition - slingOrigin.position; // Направление от рогатки к точке нажатия
            Quaternion rotation = Quaternion.LookRotation(-direction); // Инвертируем направление для поворота

            // Ограничиваем вращение только по осям X и Y, фиксируем Z
            Vector3 eulerRotation = rotation.eulerAngles;
            slingOrigin.rotation = Quaternion.Euler(0f, eulerRotation.y, 0f);
        }
    }

    private bool TryGetHitPoint(out Vector3 hitPoint)
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            hitPoint = hit.point - bulletOrigin.position;
            return true;
        }

        hitPoint = Vector3.zero;
        return false;
    }

    private void DestroyCurrentProjectile()
    {
        if (currentProjectile != null)
        {
            Destroy(currentProjectile);
            currentProjectile = null;
        }
    }
}
