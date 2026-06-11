using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public LayerMask pigLayer;
    public Transform mousePosition;
    public Crosshair crosshair;
    public Animator anim;
    public AudioSource shotSound;
    public AudioSource reloadSound;

    // Stats
    public PlayerStats stats;

    // Estado actual
    public int currentAmmo;
    private float nextShotTime;
    private bool isReloading;

    private void Start()
    {
        Cursor.visible = false;

        if (stats == null)
        {
            stats = new PlayerStats();
        }

        currentAmmo = stats.maxAmmo;
    }

    private void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartReload();
        }
    }

    void Shoot()
    {
        if (isReloading)
        {
            return;
        }

        if (Time.time < nextShotTime)
        {
            return;
        }

        if (currentAmmo <= 0)
        {
            Debug.Log("No bullets");
            StartReload();
            return;
        }

        if (anim != null)
        {
            anim.Play("Shot");
        }

        if (shotSound != null)
        {
            shotSound.Play();
        }

        if (crosshair != null)
        {
            StartCoroutine(crosshair.ShootEffect());
        }

        currentAmmo--;
        nextShotTime = Time.time + (1f / stats.fireRate);

        // CAMBIO IMPORTANTE:
        // Antes se dañaban todos los cerdos dentro del radio de disparo.
        // Ahora se revisa qué objetos hay en el radio y se elige uno solo:
        // el objetivo más cercano al centro de la mira.
        RevisarImpacto();

        Debug.Log("Shot");
    }

    private void RevisarImpacto()
    {
        if (mousePosition == null)
        {
            Debug.LogWarning("Falta asignar mousePosition en el Player.");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            mousePosition.position,
            stats.hitRadius,
            pigLayer
        );

        if (hits.Length == 0)
        {
            return;
        }

        Component objetivoElegido = null;
        float menorDistancia = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
            {
                continue;
            }

            // CAMBIO IMPORTANTE:
            // Ahora el disparo puede detectar recompensas además de cerdos.
            RecompensaEstadistica recompensa = hit.GetComponentInParent<RecompensaEstadistica>();
            CerdoVolador cerdo = hit.GetComponentInParent<CerdoVolador>();

            Component candidato = null;

            if (recompensa != null)
            {
                candidato = recompensa;
            }
            else if (cerdo != null && !cerdo.yaFinalizo)
            {
                candidato = cerdo;
            }

            if (candidato == null)
            {
                continue;
            }

            float distancia = Vector2.Distance(
                mousePosition.position,
                candidato.transform.position
            );

            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                objetivoElegido = candidato;
            }
        }

        if (objetivoElegido == null)
        {
            return;
        }

        // CAMBIO IMPORTANTE:
        // Si el objetivo elegido es una recompensa, se aplica la mejora.
        RecompensaEstadistica recompensaElegida = objetivoElegido as RecompensaEstadistica;

        if (recompensaElegida != null)
        {
            recompensaElegida.RecibirDisparo(this);
            return;
        }

        // CAMBIO IMPORTANTE:
        // Si el objetivo elegido es un cerdo, recibe daño normalmente.
        CerdoVolador cerdoElegido = objetivoElegido as CerdoVolador;

        if (cerdoElegido != null)
        {
            cerdoElegido.RecibirDisparo(stats.damage);
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (mousePosition == null || stats == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(mousePosition.position, stats.hitRadius);
    }

    void StartReload()
    {
        if (isReloading)
        {
            return;
        }

        if (currentAmmo == stats.maxAmmo)
        {
            return;
        }

        StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        if (reloadSound != null)
        {
            reloadSound.Play();
        }

        yield return new WaitForSeconds(stats.reloadTime);

        currentAmmo = stats.maxAmmo;
        isReloading = false;
        Debug.Log("Full reload");

        if (reloadSound != null)
        {
            reloadSound.Stop();
        }
    }
}