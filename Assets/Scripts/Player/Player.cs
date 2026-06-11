using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

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
        currentAmmo = stats.maxAmmo;
    }

    private void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if  (Input.GetMouseButtonDown(0))
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
        if  (isReloading)
        return;

        if (Time.time < nextShotTime)
        return;

        if (currentAmmo <= 0)
        {
            Debug.Log("No bullets");
            StartReload();
            return;
        }
        anim.Play("Shot");
        shotSound.Play();
        currentAmmo --;
        nextShotTime = Time.time + (1f / stats.fireRate);
        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePosition.position, stats.hitRadius, pigLayer);
        foreach(Collider2D hit in hits) 
        {
            if (hit != null)
            {
                CerdoVolador pig = hit.GetComponent<CerdoVolador>();
                if (pig != null)
                {
                    pig.RecibirDisparo(stats.damage);
                }
            }
        }


        Debug.Log("Shot");
    }


    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(mousePosition.position, stats.hitRadius);
    }

    void StartReload()
    {
        if  (isReloading)
        return;

        if (currentAmmo == stats.maxAmmo)
        return;

        StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        reloadSound.Play();
        yield return new WaitForSeconds(stats.reloadTime);
        currentAmmo = stats.maxAmmo;
        isReloading = false;
        Debug.Log("Full reload");
        reloadSound.Stop();
    }

}

