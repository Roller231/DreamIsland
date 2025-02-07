using UnityEngine;

public class BoolletBig : MonoBehaviour
{
    [SerializeField] private AudioClip clipOnFalse;

    // Новый параметр для управления силой ветра
    [SerializeField] private float windStrength = 0.1f;

    // Время, через которое меняется направление ветра
    [SerializeField] private float windChangeInterval = 0.5f;

    private float currentWindForce;
    private float windChangeTimer;

    private void Start()
    {
        // Устанавливаем начальное значение силы ветра
        UpdateWindForce();
    }

    private void Update()
    {
        // Таймер для смены направления ветра
        windChangeTimer += Time.deltaTime;
        if (windChangeTimer >= windChangeInterval)
        {
            UpdateWindForce();
            windChangeTimer = 0;
        }

        // Применяем отклонение пули
        transform.position += transform.right * currentWindForce * Time.deltaTime;
    }

    private void UpdateWindForce()
    {
        // Рандомно задаем силу и направление ветра
        currentWindForce = Random.Range(-windStrength, windStrength);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().Die(false);
            Destroy(gameObject);
        }
        else
        {
            GameObject.Find("GameManager").GetComponent<AudioSource>().clip = clipOnFalse;
            GameObject.Find("GameManager").GetComponent<AudioSource>().Play();
            GameObject.Find("sling").GetComponent<EnemyWaveSpawner>().GetDamage(1);
            Destroy(gameObject);
        }
    }
}
