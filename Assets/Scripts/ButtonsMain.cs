using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UTeleApp;

namespace Pinwheel.Jupiter
{
    public class ButtonsMain : MonoBehaviour
    {
        public bool flipFlop_Day_night;

        [Header("Change Time")]
        [SerializeField] private GameObject Remy_night;
        [SerializeField] private GameObject Remy_day;
        [SerializeField] private GameObject sun_light;
        [SerializeField] private JDayNightCycle ChangeScriptMaterial;
        [SerializeField] private Animation changeTimeAnim;
        [SerializeField] private Text init;

        [Header("Play Game")]
        [SerializeField] private GameObject PanelMain;
        [SerializeField] private GameObject PanelGameDay;
        [SerializeField] private GameObject slipobject;

        [Header("Camera Movement")]
        [SerializeField] private Transform cameraTransform;  // ������ �� ������
        [SerializeField] private float moveSpeed = 1f; // ����������� �������� ����������� ������ (�� ��������� 1)

        private Vector3 targetPosition;
        private Vector3 targetRotation;
        private bool isMovingCamera = false;
        private float moveDuration = 2f;
        private float moveStartTime;

        [Header("Monkey Games")]
        [SerializeField] EnemyWaveSpawner EnemyWaveSpawner;

        private void Start()
        {
            TelegramWebApp.Ready();
            init.text = GetUserIdFromInitData( TelegramWebApp.InitData).ToString();
        }

        public static long GetUserIdFromInitData(string initData)
        {
            try
            {
                // ���������� URL-������
                string decodedData = Uri.UnescapeDataString(initData);

                // ���� JSON-�����, ������������ � "user={"
                int userStartIndex = decodedData.IndexOf("user={") + 5; // 5 = ����� "user="
                if (userStartIndex == -1)
                {
                    Debug.LogError("User data not found in initData.");
                    return -1;
                }

                // ������� ����� JSON-�������
                int userEndIndex = decodedData.IndexOf('}', userStartIndex);
                if (userEndIndex == -1)
                {
                    Debug.LogError("Malformed initData string.");
                    return -1;
                }

                // ��������� JSON-������ (������: {"id":1008871802,...})
                string userJson = decodedData.Substring(userStartIndex, userEndIndex - userStartIndex + 1);

                // ���� "id" � JSON (������: "id":1008871802)
                string idKey = "\"id\":";
                int idStartIndex = userJson.IndexOf(idKey) + idKey.Length;

                if (idStartIndex == -1)
                {
                    Debug.LogError("ID not found in user JSON.");
                    return -1;
                }

                // ������� ����� �������� id
                int idEndIndex = userJson.IndexOfAny(new char[] { ',', '}' }, idStartIndex);
                if (idEndIndex == -1)
                {
                    Debug.LogError("Malformed user JSON string.");
                    return -1;
                }

                // ��������� ID � ������ ��� ��� long
                string idString = userJson.Substring(idStartIndex, idEndIndex - idStartIndex).Trim();
                return long.Parse(idString);
            }
            catch (Exception ex)
            {
                Debug.Log($"Error extracting User ID: {ex.Message}");
                return -1;
            }
        }

        public void ChangeTime()
        {
            changeTimeAnim.gameObject.SetActive(true);
            changeTimeAnim.Play();
            StartCoroutine(ChangeModelDelay());
            if (flipFlop_Day_night)
            {
                StartCoroutine(AnimateTimeChange(ChangeScriptMaterial.Time, 13.06f));
            }
            else
            {
                StartCoroutine(AnimateTimeChange(ChangeScriptMaterial.Time, 24f));
            }

            flipFlop_Day_night = !flipFlop_Day_night;
        }

        private IEnumerator AnimateTimeChange(float startTime, float targetTime)
        {
            float duration = 2f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                ChangeScriptMaterial.Time = Mathf.Lerp(startTime, targetTime, elapsedTime / duration);

                yield return null;
            }

            ChangeScriptMaterial.Time = targetTime;
        }

        private IEnumerator ChangeModelDelay()
        {
            yield return new WaitForSeconds(1.3f);
            if (!flipFlop_Day_night)
            {
                Remy_night.SetActive(false);
                Remy_day.SetActive(true);
                sun_light.SetActive(true);
            }
            else
            {
                Remy_night.SetActive(true);
                Remy_day.SetActive(false);
                sun_light.SetActive(false);
            }
            yield return new WaitForSeconds(0.8f);

            changeTimeAnim.gameObject.SetActive(false);
        }

        public void PlayButton()
        {
            if (!flipFlop_Day_night)
            {
                DayGame();
            }
            else
            {
                NightGame();
            }
        }

        public void DayGame()
        {


            PanelMain.SetActive(false);
            PanelGameDay.SetActive(true );

            slipobject.SetActive(true);
            EnemyWaveSpawner.StartWavesFunc();
            StartCameraMove(new Vector3(-6.72f, -5.75f, 1.96f), new Vector3(357.77f, 285.10f, 0f), 2f);
        }

        public void DayGameEXIT()
        {

            PanelMain.SetActive(true);
            PanelGameDay.SetActive(false);

            slipobject.SetActive(false);
            try
            {
                Destroy(slipobject.GetComponent<RopeScript>().projectileRigidbody.gameObject);
            }
            catch (Exception e) {
                }

            EnemyWaveSpawner.health = 3;
            EnemyWaveSpawner.GetDamage(0);
            StartCameraMove(new Vector3(-0.85f, -2.25f, -6.69f), new Vector3(357.774f, 0, 0), 2f);
        }

        public void NightGame()
        {
            // ����� ����� �������� ���������� ��� ������ ����
        }

        // ������� ��� ������ �������� ������
        private void StartCameraMove(Vector3 targetPos, Vector3 targetRot, float duration)
        {
            targetPosition = targetPos;
            targetRotation = targetRot;
            moveDuration = duration / moveSpeed;  // �������� �� moveSpeed ��� ����������� ��������
            moveStartTime = Time.time;
            isMovingCamera = true;
        }

        // ���������� ������
        private void Update()
        {
            if (isMovingCamera)
            {
                // ������� �����������
                float elapsedTime = Time.time - moveStartTime;
                if (elapsedTime < moveDuration)
                {
                    // ������� ����������� � ������� ������
                    cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPosition, elapsedTime / moveDuration);
                    cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, Quaternion.Euler(targetRotation), elapsedTime / moveDuration);
                }
                else
                {
                    // ������ �������� ����� �������
                    cameraTransform.localPosition = targetPosition;
                    cameraTransform.localRotation = Quaternion.Euler(targetRotation);
                    isMovingCamera = false;  // ��������� ��������
                }
            }
        }
    }
}
