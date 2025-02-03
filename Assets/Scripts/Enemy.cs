using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private AudioClip clipOnDie;
    [SerializeField] private AudioClip clipOnFalse;


    private void OnEnable()
    {
        StartCoroutine(goBack());
    }

    public void Die()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().monkeyCount++;
        GameObject.Find("GameManager").GetComponent<AudioSource>().clip = clipOnDie;
        GameObject.Find("GameManager").GetComponent<AudioSource>().Play();
        gameObject.SetActive(false);
    }

    private IEnumerator goBack()
    {
        yield return new WaitForSeconds(5f);

        GameObject.Find("GameManager").GetComponent<AudioSource>().clip = clipOnFalse;
        GameObject.Find("GameManager").GetComponent<AudioSource>().Play();
        GameObject.Find("sling").GetComponent<EnemyWaveSpawner>().GetDamage(1);
        gameObject.SetActive(false );

    }
}
