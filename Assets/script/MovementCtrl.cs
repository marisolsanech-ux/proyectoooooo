using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class MovementCtrl : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    private InputActionMap _inputActionMap;
    private InputAction _move;
    private InputAction _jump;

    [Header("Velocidad")]
    public float velocidadBase = 5f;
    public float multiplicadorSprint = 1.8f;
    public bool sprintHabilitado = true;

    [Header("Salto")]
    public float fuerzaSalto = 6f;
    public float gravedad = -20f;
    [Tooltip("Clip que se reproduce cada vez que el personaje salta")]
    public AudioClip sonidoSalto;
    [Range(0f, 1f)]
    public float volumenSalto = 1f;

    [Header("Plataformas moviles")]
    [Tooltip("Capa(s) que contienen las plataformas moviles, para el raycast de deteccion")]
    public LayerMask capaPlataformas = ~0;
    [Tooltip("Distancia extra hacia abajo para detectar la plataforma bajo los pies")]
    public float distanciaDeteccionPlataforma = 0.3f;

    [Header("Impulso externo (Jump Pads)")]
    [Tooltip("Que tan rapido se desvanece el impulso horizontal recibido de un Jump Pad")]
    public float friccionImpulso = 2f;

    private CharacterController _controller;
    private AudioSource _audioSource;
    private Vector3 _velocidadVertical;
    private bool _sprintPresionado;
    private MovingPlatform _plataformaActual;
    private Vector3 _impulsoExterno = Vector3.zero;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
    }

    void Start()
    {
        _inputActionMap = inputActionAsset.FindActionMap("Player");
        _move = _inputActionMap.FindAction("Move");

        _jump = _inputActionMap.FindAction("Jump");

        _inputActionMap.Enable();
    }

    void Update()
    {
        Vector2 joyStickMove = _move.ReadValue<Vector2>();
        Vector3 realMovement = new Vector3(joyStickMove.x, 0, joyStickMove.y);

        _sprintPresionado = sprintHabilitado &&
                             Keyboard.current != null &&
                             Keyboard.current.leftShiftKey.isPressed;

        float velocidadFinal = velocidadBase * (_sprintPresionado ? multiplicadorSprint : 1f);

        
        Vector3 movimientoHorizontal = (realMovement * velocidadFinal) + _impulsoExterno;

      
        _impulsoExterno = Vector3.MoveTowards(_impulsoExterno, Vector3.zero, friccionImpulso * Time.deltaTime);

        // --- Gravedad y salto ---
        bool enSuelo = _controller.isGrounded;

        if (enSuelo && _velocidadVertical.y < 0)
        {
            _velocidadVertical.y = -2f;
        }

        bool saltoPresionado = (_jump != null && _jump.WasPressedThisFrame()) ||
                                (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame);

        if (saltoPresionado && enSuelo)
        {
            _velocidadVertical.y = fuerzaSalto;
            ReproducirSonidoSalto();
        }

        _velocidadVertical.y += gravedad * Time.deltaTime;

      
        _plataformaActual = DetectarPlataformaMovil(enSuelo);

      
        Vector3 movimientoPlataforma = Vector3.zero;
        if (_plataformaActual != null)
        {
            movimientoPlataforma = _plataformaActual.DeltaMovimiento;
        }

       
        Vector3 movimientoTotal = (movimientoHorizontal + new Vector3(0, _velocidadVertical.y, 0)) * Time.deltaTime;
        _controller.Move(movimientoTotal + movimientoPlataforma);
    }

    private void ReproducirSonidoSalto()
    {
        if (sonidoSalto == null) return;

        _audioSource.PlayOneShot(sonidoSalto, volumenSalto);
    }

    private MovingPlatform DetectarPlataformaMovil(bool enSuelo)
    {
        if (!enSuelo) return null;

        Vector3 origen = transform.position + Vector3.up * 0.1f;
        float distancia = (_controller.height / 2f) + distanciaDeteccionPlataforma;

        if (Physics.Raycast(origen, Vector3.down, out RaycastHit hit, distancia, capaPlataformas, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.GetComponent<MovingPlatform>();
        }

        return null;
    }

 
    public void AplicarImpulso(Vector3 impulsoHorizontal, float impulsoVertical)
    {
        _impulsoExterno += impulsoHorizontal;
        _velocidadVertical.y = impulsoVertical;
    }
}
