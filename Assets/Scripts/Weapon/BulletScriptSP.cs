using UnityEngine;

public class BulletScriptSP : MonoBehaviour
{
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _progress;
    public GameObject effectPrefab;
    public GameObject onBeatEffectPrefab;


    [SerializeField] private float _bulletSpeed = 25f;

    // Start is called before the first frame update
    void Start()
    {
        _startPosition = transform.position;
        Invoke(nameof(DestroySelf), 0.5f);
        //Debug.LogError("this bullet wants to go to: " + _targetPosition);
    }

    // Update is called once per frame
    [HideInInspector] public bool targetSet = false;
    void FixedUpdate()
    {
        if (!targetSet) return;
        _progress += _bulletSpeed * Time.timeScale * Time.fixedDeltaTime;
        //transform.position = Vector3.Lerp(_startPosition,_targetPosition,_progress);
        transform.position = Vector3.MoveTowards(_startPosition, _targetPosition, _progress);
    }

    public void SetTargetPosition(Vector3 targetPosition){
        _targetPosition = targetPosition;
        targetSet = true;
    }

    public void SetColor(Color color)
    {
        ParticleSystem particleSystem = effectPrefab.GetComponent<ParticleSystem>();
        var main = particleSystem.main;
        main.startColor = color;
    }

    public void SetTrailColor(Color color1, Color color2)
    {
        TrailRenderer trail = this.GetComponent<TrailRenderer>();
        if (!trail) return;

        //SET COLORS
        var gradient = new Gradient();

        var colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(color1, 0.5f);
        colors[1] = new GradientColorKey(color2, 1.0f);

        //SET ALPHAS
        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(0.0f, 1.0f);

        trail.colorGradient.SetKeys(colors, alphas);

        Debug.LogError("doin a thang");
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
