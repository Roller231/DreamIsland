using System.Collections;
using UnityEngine;

public class ButtonsMain : MonoBehaviour
{
    private bool flipFlop_Day_night;

    [SerializeField] private Material skybox_night;
    [SerializeField] private Material skybox_day;

    [SerializeField] private Animation changeTimeAnim;

    public void ChangeTime()
    {
        changeTimeAnim.gameObject.SetActive(true);
        changeTimeAnim.Play();

        StartCoroutine(PlayAnimChangeTime(1f));
    }

    IEnumerator PlayAnimChangeTime(float time)
    {
        yield return new WaitForSeconds(time);

        if (flipFlop_Day_night)
        {
            RenderSettings.skybox = skybox_day;
            // ≈сли нужно сразу применить изменени€
            DynamicGI.UpdateEnvironment();
            flipFlop_Day_night = !flipFlop_Day_night;

        }
        else
        {
            RenderSettings.skybox = skybox_night;
            // ≈сли нужно сразу применить изменени€
            DynamicGI.UpdateEnvironment();
            flipFlop_Day_night = !flipFlop_Day_night;
        }

        yield return new WaitForSeconds(time);


        changeTimeAnim.gameObject.SetActive(false);

    }
}
