using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyWaveSpawner : MonoBehaviour
{
    [Header("��������� ����")]
    public List<GameObject> enemies; // ������ ������
    public float startWaveTime = 5f; // ��������� ����� ����� �������
    public float minWaveTime = 1f;  // ����������� ����� ����� �������
    public int enemiesPerWave = 1;  // ��������� ���������� ������ �� �����
    public int wavesToIncreaseEnemies = 10; // ���������� ����, ����� �������� ������������� ����� ������
    public float waveTimeDecrement = 0.5f; // �� ������� ����������� ����� ����� ������ ���

    private float currentWaveTime; // ������� ����� ����� �������
    private int currentWave = 0;   // ������� ����� �����

    [Header("��������� ������")]
    public int health;
    public Button exitButton;
    public List<GameObject> heartsImage;

    void Start()
    {
        // ���������� ��������� ���������
        currentWaveTime = startWaveTime;

        // ���������, ��� ��� ����� ���������� ���������
        foreach (var enemy in enemies)
        {
            enemy.SetActive(false);
        }


    }

    public void StartWavesFunc()
    {
        // ������ ��������
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (true)
        {
            // ����� ����� ������� ����� �����
            yield return new WaitForSeconds(currentWaveTime);

            // ��������� ����� ������� �����
            currentWave++;

            // ����� ������
            for (int i = 0; i < enemiesPerWave; i++)
            {
                GameObject enemyToActivate = GetRandomInactiveEnemy();
                if (enemyToActivate != null)
                {
                    enemyToActivate.SetActive(true); // ������������ �����
                }
                else
                {
                    Debug.Log("��� ��������� ������ ��� ���������.");
                    yield break; // ����� �� ��������, ���� ��� ����� ������������
                }
            }

            // ��������� ����� ����� �������, ���� ��� ������ ������������
            if (currentWave % wavesToIncreaseEnemies == 0)
            {
                enemiesPerWave++; // ��������� ���������� ������
            }
            currentWaveTime = Mathf.Max(minWaveTime, currentWaveTime - waveTimeDecrement);
        }
    }

    private GameObject GetRandomInactiveEnemy()
    {
        // ������� ������ �� ���������� ������
        List<GameObject> inactiveEnemies = enemies.FindAll(enemy => !enemy.activeSelf);

        if (inactiveEnemies.Count > 0)
        {
            // ������� ���������� ����� �� ����������
            return inactiveEnemies[Random.Range(0, inactiveEnemies.Count)];
        }
        return null; // ������� null, ���� ��� ���������� ������
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
        heartColor.a = 0.3f; // ��������, ������ ������ ����������


        this.health -= health;
        if (this.health == 3)
        {
            heartColor.a = 1f; // ��������, ������ ������ ����������
            foreach (GameObject e in heartsImage)
            {

                e.GetComponent<Image>().color = heartColor;
            }
        }
        else if (this.health == 2)
        {
            heartColor.a = 0.3f; // ��������, ������ ������ ����������
            heartsImage[2].GetComponent<Image>().color = heartColor;
        }
        else if (this.health == 1)
        {
            heartColor.a = 0.3f; // ��������, ������ ������ ����������
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
