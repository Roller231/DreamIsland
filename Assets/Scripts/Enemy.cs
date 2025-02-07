using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private AudioClip clipOnDie;
    [SerializeField] private AudioClip clipOnFalse;
    [SerializeField] private AudioClip clipMonkey;
    [SerializeField] private float timeToBack;

    private GameManager gameManager;

    private void Start()
    {

    }

    private void OnEnable()
    {
        StartCoroutine(goBack());
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GetComponent<Animator>().SetTrigger("Start");
        gameManager.GetComponent<AudioSource>().clip = clipMonkey;
        gameManager.GetComponent<AudioSource>().Play();
    }

    public void Die(bool game)
    {
        if(game) gameManager.monkeyCount++;
        else gameManager.pirateCount++;
        gameManager.GetComponent<AudioSource>().clip = clipOnDie;
        gameManager.GetComponent<AudioSource>().Play();
        //if(set_or_anim) gameObject.SetActive(false);
        GetComponent<Animator>().SetTrigger("Die");
        StartCoroutine(activationSet());


    }

    IEnumerator activationSet()
    {
        yield return new WaitForSeconds(0.1f);

        gameObject.SetActive(false );
    }

    private IEnumerator goBack()
    {
        yield return new WaitForSeconds(timeToBack);

        gameManager.GetComponent<AudioSource>().clip = clipOnFalse;
        gameManager.GetComponent<AudioSource>().Play();
        GameObject.Find("sling").GetComponent<EnemyWaveSpawner>().GetDamage(1);
        gameObject.SetActive(false );

    }

    
}
