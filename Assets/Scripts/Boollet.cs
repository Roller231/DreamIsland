using UnityEngine;

public class Boollet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().Die();
            Destroy(gameObject);
        }
    }
}
