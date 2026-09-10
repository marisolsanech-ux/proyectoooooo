using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Puntos de recorrido")]
    public Transform puntoA;
    public Transform puntoB;

    [Header("Velocidad")]
    public float velocidad = 2f;

    [Tooltip("Distancia minima para considerar que llego al punto destino")]
    public float umbralLlegada = 0.05f;


    public Vector3 DeltaMovimiento { get; private set; }

    private Rigidbody _rb;
    private Vector3 _destino;
    private Vector3 _posicionAnterior;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;   // se mueve por script, no por fuerzas fisicas
        _rb.useGravity = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate; // movimiento mas suave
    }

    void Start()
    {
        if (puntoA == null || puntoB == null)
        {
            Debug.LogWarning($"[{name}] MovingPlatform: asigna puntoA y puntoB en el Inspector.");
            enabled = false;
            return;
        }

        _rb.position = puntoA.position;
        _destino = puntoB.position;
        _posicionAnterior = _rb.position;
    }

    void FixedUpdate()
    {
        _posicionAnterior = _rb.position;

        Vector3 nuevaPosicion = Vector3.MoveTowards(_rb.position, _destino, velocidad * Time.fixedDeltaTime);
        _rb.MovePosition(nuevaPosicion);

        if (Vector3.Distance(nuevaPosicion, _destino) <= umbralLlegada)
        {
            _destino = (_destino == puntoA.position) ? puntoB.position : puntoA.position;
        }

        DeltaMovimiento = nuevaPosicion - _posicionAnterior;
    }

    void OnDrawGizmosSelected()
    {
        if (puntoA == null || puntoB == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(puntoA.position, puntoB.position);
        Gizmos.DrawWireSphere(puntoA.position, 0.3f);
        Gizmos.DrawWireSphere(puntoB.position, 0.3f);
    }
}