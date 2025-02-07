using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyWaveSpawner : MonoBehaviour
{
    [Header("Настройки волн")]
    public List<GameObject> enemies; // Список врагов
    public float startWaveTime = 5f; // Начальное время между волнами
    public float minWaveTime = 1f;  // Минимальное время между волнами
    public int enemiesPerWave = 1;  // Начальное количество врагов за волну
    public int wavesToIncreaseEnemies = 10; // Количество волн, после которого увеличивается число врагов
    public float waveTimeDecrement = 0.5f; // На сколько уменьшается время волны каждый раз

    private float currentWaveTime; // Текущее время между волнами
    private int currentWave = 0;   // Текущий номер волны

    [Header("Настройка жизней")]
    public int health;
    public Button exitButton;
    public List<GameObject> heartsImage;

    void Start()
    {
        // Установить начальные параметры
        currentWaveTime = startWaveTime;

        // Убедиться, что все враги изначально неактивны
        foreach (var enemy in enemies)
        {
            enemy.SetActive(false);
        }


    }

    public void StartWavesFunc()
    {
        // Запуск спаунера
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (true)
        {
            // Ждать перед началом новой волны
            yield return new WaitForSeconds(currentWaveTime);

            // Увеличить номер текущей волны
            currentWave++;

            // Спавн врагов
            for (int i = 0; i < enemiesPerWave; i++)
            {
                GameObject enemyToActivate = GetRandomInactiveEnemy();
                if (enemyToActivate != null)
                {
                    enemyToActivate.SetActive(true); // Активировать врага
                }
                else
                {
                    Debug.Log("Нет доступных врагов для активации.");
                    yield break; // Выход из корутины, если все враги активированы
                }
            }

            // Уменьшить время между волнами, если оно больше минимального
            if (currentWave % wavesToIncreaseEnemies == 0)
            {
                enemiesPerWave++; // Увеличить количество врагов
            }
            currentWaveTime = Mathf.Max(minWaveTime, currentWaveTime - waveTimeDecrement);
        }
    }

    private GameObject GetRandomInactiveEnemy()
    {
        // Создать список из неактивных врагов
        List<GameObject> inactiveEnemies = enemies.FindAll(enemy => !enemy.activeSelf);

        if (inactiveEnemies.Count > 0)
        {
            // Выбрать случайного врага из неактивных
            return inactiveEnemies[Random.Range(0, inactiveEnemies.Count)];
        }
        return null; // Вернуть null, если нет неактивных врагов
    }

    public void ExitGame()
    {
        foreach(GameObject e in enemies)
        {
            e.SetActive(false);
        }

        currentWave = 0;
        currentWaveTime = startWaveTime;
        enemiesPerWave = 1;
    }

    public void GetDamage(int health)
    {
        Color heartColor = heartsImage[0].GetComponent<Image>().color;
        heartColor.a = 0.3f; // Например, делаем сердце прозрачным


        this.health -= health;
        if (this.health == 3)
        {
            heartColor.a = 1f; // Например, делаем сердце прозрачным
            foreach (GameObject e in heartsImage)
            {

                e.GetComponent<Image>().color = heartColor;
            }
        }
        else if (this.health == 2)
        {
            heartColor.a = 0.3f; // Например, делаем сердце прозрачным
            heartsImage[2].GetComponent<Image>().color = heartColor;
        }
        else if (this.health == 1)
        {
            heartColor.a = 0.3f; // Например, делаем сердце прозрачным
            heartsImage[1].GetComponent<Image>().color = heartColor;
        }
        if (this.health <= 0)
        {
            heartsImage[0].GetComponent<Image>().color = heartColor;
            StopAllCoroutines();
            exitButton.onClick.Invoke();
        }
    }
}
