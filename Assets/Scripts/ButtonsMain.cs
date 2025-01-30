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
                // Смена на день
                StartCoroutine(AnimateTimeChange(ChangeScriptMaterial.Time, 13.06f));

            }
            else
            {
                // Смена на ночь
                StartCoroutine(AnimateTimeChange(ChangeScriptMaterial.Time, 24f));

            }

            flipFlop_Day_night = !flipFlop_Day_night;
        }

        private IEnumerator AnimateTimeChange(float startTime, float targetTime)
        {
            float duration = 2f; // Время, за которое происходит смена времени (в секундах)
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                ChangeScriptMaterial.Time = Mathf.Lerp(startTime, targetTime, elapsedTime / duration);

                yield return null; // Ждем следующий кадр
            }

            ChangeScriptMaterial.Time = targetTime; // Убедимся, что значение точно равно конечному
            
        }

        private IEnumerator ChangeModelDelay()
        {

            yield return new WaitForSeconds(1.3f);
            if (!flipFlop_Day_night)
            {
                // Смена на день
                Remy_night.SetActive(false);
                Remy_day.SetActive(true);
                sun_light.SetActive(true);
            }
            else
            {
                // Смена на ночь
                Remy_night.SetActive(true);
                Remy_day.SetActive(false);
                sun_light.SetActive(false);

            }
            yield return new WaitForSeconds(0.8f);

            changeTimeAnim.gameObject.SetActive(false);

        }
    }
}