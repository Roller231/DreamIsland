using UnityEngine;

public class Boollet : MonoBehaviour
{
    [SerializeField] private AudioClip clipOnFalse;


    private void OnCollisionEnter(Collision collision)
    {


        if(collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().Die();
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
