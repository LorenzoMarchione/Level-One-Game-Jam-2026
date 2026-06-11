using UnityEngine;

public class RecompensaEstadistica : MonoBehaviour
{
    public enum TipoRecompensa
    {
        VelocidadDeDisparo,
        TamanoDeMira,
        TiempoDeRecarga,
        Dano
    }

    [Header("Tipo de recompensa")]
    [SerializeField] private TipoRecompensa tipoRecompensa;

    [Header("Valores de mejora")]
    [SerializeField] private float aumentoVelocidadDisparo = 0.5f;
    [SerializeField] private float aumentoRadioMira = 0.15f;
    [SerializeField] private float multiplicadorVisualMira = 1.15f;
    [SerializeField] private float reduccionTiempoRecarga = 0.2f;
    [SerializeField] private int aumentoDano = 1;

    [Header("Límites")]
    [SerializeField] private float tiempoRecargaMinimo = 0.3f;
    [SerializeField] private float radioMiraMaximo = 2.5f;

    [Header("Desaparición")]
    [SerializeField] private float tiempoAntesDeDesaparecer = 8f;

    private bool yaFueRecolectada;

    private void Start()
    {
        if (tiempoAntesDeDesaparecer > 0f)
        {
            Destroy(gameObject, tiempoAntesDeDesaparecer);
        }
    }

    public void RecibirDisparo(Player jugador)
    {
        if (yaFueRecolectada)
        {
            return;
        }

        if (jugador == null || jugador.stats == null)
        {
            Debug.LogWarning("La recompensa no encontró al jugador o sus estadísticas.");
            return;
        }

        yaFueRecolectada = true;

        AplicarMejora(jugador);

        Destroy(gameObject);
    }

    private void AplicarMejora(Player jugador)
    {
        switch (tipoRecompensa)
        {
            case TipoRecompensa.VelocidadDeDisparo:
                jugador.stats.fireRate += aumentoVelocidadDisparo;
                Debug.Log("Mejora obtenida: velocidad de disparo +" + aumentoVelocidadDisparo);
                break;

            case TipoRecompensa.TamanoDeMira:
                jugador.stats.hitRadius = Mathf.Min(
                    jugador.stats.hitRadius + aumentoRadioMira,
                    radioMiraMaximo
                );

                if (jugador.crosshair != null)
                {
                    jugador.crosshair.AumentarTamano(multiplicadorVisualMira);
                }

                Debug.Log("Mejora obtenida: tamaño de mira +" + aumentoRadioMira);
                break;

            case TipoRecompensa.TiempoDeRecarga:
                jugador.stats.reloadTime = Mathf.Max(
                    tiempoRecargaMinimo,
                    jugador.stats.reloadTime - reduccionTiempoRecarga
                );

                Debug.Log("Mejora obtenida: recarga más rápida -" + reduccionTiempoRecarga);
                break;

            case TipoRecompensa.Dano:
                jugador.stats.damage += aumentoDano;
                Debug.Log("Mejora obtenida: daño +" + aumentoDano);
                break;
        }
    }
}