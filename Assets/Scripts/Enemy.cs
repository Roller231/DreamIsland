using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private AudioClip clipOnDie;
    [SerializeField] private AudioClip clipOnFalse;
    [SerializeField] private AudioClip clipMonkey;

    private GameManager gameManager;

    private void Start()
    {

    }

    private void OnEnable()
    {
        StartCoroutine(goBack());
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        gameManager.GetComponent<AudioSource>().clip = clipMonkey;
        gameManager.GetComponent<AudioSource>().Play();
    }

    public void Die()
    {
        gameManager.monkeyCount++;
        gameManager.GetComponent<AudioSource>().clip = clipOnDie;
        gameManager.GetComponent<AudioSource>().Play();
        gameObject.SetActive(false);
    }

    private IEnumerator goBack()
    {
        yield return new WaitForSeconds(5f);

        gameManager.GetComponent<AudioSource>().clip = clipOnFalse;
        gameManager.GetComponent<AudioSource>().Play();
        GameObject.Find("sling").GetComponent<EnemyWaveSpawner>().GetDamage(1);
        gameObject.SetActive(false );

    }

    
}
