using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class GoalTrigger : MonoBehaviour
{
    [Header("Tipo de objeto")]
    [Tooltip("Activa esta opcion en los cubos que sean estrellas")]
    public bool esEstrella;

    [Header("Mensaje")]
    public string mensaje = "¡Meta alcanzada!";
    public string mensajeTresEstrellas = textofelicidad.MensajeFinal;

    [Header("UI opcional")]
    [Tooltip("Arrastra aqui un Text de la UI (opcional) para mostrar el mensaje en pantalla")]
    public Text textoUI;

    [Tooltip("Arrastra aqui el Text que mostrara cuantas estrellas se han obtenido")]
    public Text contadorEstrellasUI;

    private static int _estrellasObtenidas;
    private static Text _contadorAutomatico;
    private const int TotalEstrellas = 3;
    private bool _objetoActivado;

    private bool _metaAlcanzada;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ReiniciarContador()
    {
        _estrellasObtenidas = 0;
    }

    void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    void Start()
    {
        if (esEstrella)
        {
            CrearContadorSiHaceFalta();
            ActualizarContador();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (esEstrella)
        {
            RecogerEstrella(other);
            return;
        }

        if (_metaAlcanzada) return; // evita que se dispare mas de una vez

        if (other.GetComponent<MovementCtrl>() == null) return;

        _metaAlcanzada = true;
        Debug.Log(mensaje);

        if (textoUI != null)
        {
            textoUI.text = mensaje;
            textoUI.gameObject.SetActive(true);
        }

    }

    private void RecogerEstrella(Collider other)
    {
        if (_objetoActivado) return;

        if (other.GetComponent<MovementCtrl>() == null) return;

        _objetoActivado = true;
        _estrellasObtenidas++;

        ActualizarContador();

        if (_estrellasObtenidas >= TotalEstrellas)
        {
            MostrarMensajeFinal();
        }

        Destroy(gameObject);
    }

    private void MostrarMensajeFinal()
    {
        textofelicidad textoDeMeta = FindObjectOfType<textofelicidad>();
        if (textoDeMeta == null)
        {
            GameObject objetoMensaje = new GameObject("Mensaje de Tres Estrellas");
            textoDeMeta = objetoMensaje.AddComponent<textofelicidad>();
        }

        textoDeMeta.MostrarMensaje(textofelicidad.MensajeFinal);
    }

    private void ActualizarContador()
    {
        Text contador = contadorEstrellasUI != null ? contadorEstrellasUI : _contadorAutomatico;
        if (contador == null) return;

        contador.text = "Estrellas: " + _estrellasObtenidas;
        contador.gameObject.SetActive(true);
    }

    private void CrearContadorSiHaceFalta()
    {
        if (contadorEstrellasUI != null || _contadorAutomatico != null) return;

        GameObject canvasObject = new GameObject("Contador de Estrellas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject textoObject = new GameObject("Texto de Estrellas");
        textoObject.transform.SetParent(canvasObject.transform, false);
        _contadorAutomatico = textoObject.AddComponent<Text>();
        _contadorAutomatico.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _contadorAutomatico.fontSize = 20;
        _contadorAutomatico.alignment = TextAnchor.UpperRight;
        _contadorAutomatico.color = Color.white;

        RectTransform rectTransform = _contadorAutomatico.rectTransform;
        rectTransform.anchorMin = new Vector2(1f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(1f, 1f);
        rectTransform.anchoredPosition = new Vector2(-15f, -15f);
        rectTransform.sizeDelta = new Vector2(170f, 32f);
    }
}