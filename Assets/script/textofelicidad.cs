using UnityEngine;
using UnityEngine.UI;

public class textofelicidad : MonoBehaviour
{
    public const string MensajeFinal = "Terminaste el recorrido";

    [Header("Texto de meta")]
    [Tooltip("Si queda vacio, se usa el componente Text de este mismo objeto")]
    public Text texto;

    public string mensaje = MensajeFinal;

    [Header("Pausa")]
    public bool pausarAlMostrar = true;
    public bool permitirReanudarConEspacio = true;

    private bool _mensajeVisible;
    private float _escalaAntesDePausar = 1f;

    void Awake()
    {
        if (texto == null)
        {
            texto = GetComponent<Text>();
        }

        CrearTextoSiHaceFalta();
        OcultarMensaje();
    }

    void Update()
    {
        if (_mensajeVisible && permitirReanudarConEspacio && Input.GetKeyDown(KeyCode.Space))
        {
            ReanudarJuego();
        }
    }

    public void MostrarMeta()
    {
        MostrarMensaje(mensaje);
    }

    public void MostrarMensaje(string textoAMostrar)
    {
        CrearTextoSiHaceFalta();
        if (texto == null) return;

        texto.text = textoAMostrar;
        texto.fontSize = 80;
        texto.color = new Color(0.2f, 1f, 0.05f, 1f);
        texto.alignment = TextAnchor.MiddleCenter;
        texto.enabled = true;
        AgregarSombra();
        CentrarTexto();
        _mensajeVisible = true;

        if (pausarAlMostrar)
        {
            _escalaAntesDePausar = Time.timeScale;
            Time.timeScale = 0f;
        }
    }

    public void ReanudarJuego()
    {
        Time.timeScale = _escalaAntesDePausar > 0f ? _escalaAntesDePausar : 1f;
        _mensajeVisible = false;
        OcultarMensaje();
    }

    public void QuitarMensaje()
    {
        ReanudarJuego();
    }

    private void OcultarMensaje()
    {
        if (texto != null)
        {
            texto.enabled = false;
        }
    }

    private void CrearTextoSiHaceFalta()
    {
        if (texto != null) return;

        GameObject canvasObject = new GameObject("Canvas Mensaje de Meta");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject textoObject = new GameObject("Texto Meta Alcanzada");
        textoObject.transform.SetParent(canvasObject.transform, false);
        texto = textoObject.AddComponent<Text>();
        texto.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        texto.fontSize = 77;
        texto.color = new Color(0.2f, 1f, 0.05f, 1f);
        texto.horizontalOverflow = HorizontalWrapMode.Overflow;
        texto.verticalOverflow = VerticalWrapMode.Overflow;

        RectTransform rectTransform = texto.rectTransform;
        rectTransform.sizeDelta = new Vector2(1100f, 160f);
    }

    private void AgregarSombra()
    {
        Shadow sombra = texto.GetComponent<Shadow>();
        if (sombra == null)
        {
            sombra = texto.gameObject.AddComponent<Shadow>();
        }

        sombra.effectColor = new Color(0f, 0f, 0f, 0.9f);
        sombra.effectDistance = new Vector2(3f, -3f);
    }

    private void CentrarTexto()
    {
        RectTransform rectTransform = texto.rectTransform;
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
    }
}
