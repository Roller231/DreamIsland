using System.Collections;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;


namespace Pinwheel.Jupiter
{
    public class ButtonsMain : MonoBehaviour
    {
        public bool flipFlop_Day_night;

        [SerializeField] private GameObject Remy_night;
        [SerializeField] private GameObject Remy_day;
        [SerializeField] private GameObject sun_light;

        [SerializeField] private JDayNightCycle ChangeScriptMaterial;

        [SerializeField] private Animation changeTimeAnim;

        public void ChangeTime()
        {
            changeTimeAnim.gameObject.SetActive(true);
            changeTimeAnim.Play();
            StartCoroutine(ChangeModelDelay());
            if (flipFlop_Day_night)
            {
                // ����� �� ����
                StartCoroutine(AnimateTimeChange(ChangeScriptMaterial.Time, 13.06f));

            }
            else
            {
                // ����� �� ����
                StartCoroutine(AnimateTimeChange(ChangeScriptMaterial.Time, 24f));

            }

            flipFlop_Day_night = !flipFlop_Day_night;
        }

        private IEnumerator AnimateTimeChange(float startTime, float targetTime)
        {
            float duration = 2f; // �����, �� ������� ���������� ����� ������� (� ��������)
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                ChangeScriptMaterial.Time = Mathf.Lerp(startTime, targetTime, elapsedTime / duration);

                yield return null; // ���� ��������� ����
            }

            ChangeScriptMaterial.Time = targetTime; // ��������, ��� �������� ����� ����� ���������
            
        }

        private IEnumerator ChangeModelDelay()
        {

            yield return new WaitForSeconds(1.3f);
            if (!flipFlop_Day_night)
            {
                // ����� �� ����
                Remy_night.SetActive(false);
                Remy_day.SetActive(true);
                sun_light.SetActive(true);
            }
            else
            {
                // ����� �� ����
                Remy_night.SetActive(true);
                Remy_day.SetActive(false);
                sun_light.SetActive(false);

            }
            yield return new WaitForSeconds(0.8f);

            changeTimeAnim.gameObject.SetActive(false);

        }
    }
}