using System.Collections.Generic;
using UnityEngine;

public class RandomObjectActivator : MonoBehaviour
{
    public List<GameObject> objectsToActivate; // Список объектов
    public LayerMask garbageLayer; // Слой "garbage"
    public float activationInterval = 3f; // Интервал в секундах
    private HashSet<GameObject> activeObjects = new HashSet<GameObject>(); // Активные объекты

    private void Start()
    {
        InvokeRepeating(nameof(ActivateRandomObject), 0f, activationInterval);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ЛКМ
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, garbageLayer))
            {

                DeactivateObject(hit.collider.gameObject.GetComponentInParent<Animation>().gameObject);
            }
        }
    }

    private void ActivateRandomObject()
    {
        List<GameObject> inactiveObjects = objectsToActivate.FindAll(obj => !activeObjects.Contains(obj));

        if (inactiveObjects.Count > 0)
        {
            GameObject selectedObject = inactiveObjects[Random.Range(0, inactiveObjects.Count)];
            selectedObject.SetActive(true);
            activeObjects.Add(selectedObject);
        }
    }

    private void DeactivateObject(GameObject obj)
    {
        if (activeObjects.Contains(obj))
        {
            Debug.Log("RAY");

            obj.SetActive(false);
            activeObjects.Remove(obj);
        }
    }
}
