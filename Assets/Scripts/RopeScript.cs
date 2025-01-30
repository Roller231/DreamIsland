using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RopeScript : MonoBehaviour
{
    [Header("Rope Settings")]
    [SerializeField] private Transform TransPoint1;
    [SerializeField] private Transform TransPoint2;
    [SerializeField] private Transform BallPrefab;

    private LineRenderer _lineRenderer;
    private Transform _newBall;

    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && _newBall == null)
        {
            _newBall = Instantiate(BallPrefab, Vector3.zero, Quaternion.identity);
        }

        if (_newBall)
        {
            // Add 3rd Point in the Middle..
            if (_lineRenderer.positionCount < 3)
            {
                _lineRenderer.positionCount = 3;
            }

            Vector3 newPos = _newBall.position;
            newPos.z = -0.55f;
            newPos.y = -0.6f;
            _lineRenderer.SetPosition(1, newPos);
        }

        if (TransPoint1 && TransPoint2)
        {
            _lineRenderer.SetPosition(0, TransPoint1.position);
            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, TransPoint2.position);
        }
    }
}
