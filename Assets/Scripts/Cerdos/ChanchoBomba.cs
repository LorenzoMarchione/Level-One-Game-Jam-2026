using UnityEngine;

public class ChanchoBomba : MonoBehaviour
{
    [Header("Daño en área")]
    [SerializeField] private float radioExplosion = 2f;
    [SerializeField] private int danoExplosion = 999;

    [Header("Visualización de prueba")]
    [SerializeField] private bool mostrarRadioExplosion = true;

    private CerdoVolador cerdoVolador;

    private void Awake()
    {
        cerdoVolador = GetComponent<CerdoVolador>();
    }

    private void OnEnable()
    {
        if (cerdoVolador != null)
        {
            // Cuando este chancho bomba sea derribado, ejecuta la explosión.
            cerdoVolador.AlSerDerribado += Explotar;
        }
    }

    private void OnDisable()
    {
        if (cerdoVolador != null)
        {
            cerdoVolador.AlSerDerribado -= Explotar;
        }
    }

    private void Explotar(CerdoVolador cerdoDerribado, int puntosOtorgados)
    {
        Collider2D[] collidersDetectados = Physics2D.OverlapCircleAll(
            transform.position,
            radioExplosion
        );

        foreach (Collider2D colliderDetectado in collidersDetectados)
        {
            CerdoVolador cerdoCercano = colliderDetectado.GetComponentInParent<CerdoVolador>();

            if (cerdoCercano == null)
            {
                continue;
            }

            // Evita que el chancho bomba se dañe a sí mismo.
            if (cerdoCercano == cerdoVolador)
            {
                continue;
            }

            // Aplica daño masivo a los cerdos cercanos.
            cerdoCercano.RecibirDisparo(danoExplosion);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!mostrarRadioExplosion)
        {
            return;
        }

        Gizmos.DrawWireSphere(transform.position, radioExplosion);
    }
}