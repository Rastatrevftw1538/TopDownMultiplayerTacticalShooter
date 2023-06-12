using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _progress;
    [SerializeField] public GameObject effectPrefab;


    [SerializeField] private float _bulletSpeed = 25f;

    // Start is called before the first frame update
    void Start()
    {
        _startPosition = transform.position = new Vector3(transform.position.x,transform.position.y,-1);
    }

    // Update is called once per frame
    void Update()
    {
        _progress += Time.deltaTime * _bulletSpeed;
        transform.position = Vector3.Lerp(_startPosition,_targetPosition,_progress);
    }

    public void SetTargetPosition(Vector3 targetPosition){
        _targetPosition = new Vector3(targetPosition.x,targetPosition.y,-1);
    }
    public void SetColor(Color color)
    {
        effectPrefab.GetComponent<ParticleSystem>().startColor = color;
    }
}
