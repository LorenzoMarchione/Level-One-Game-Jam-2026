using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
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
            return;
        }

        currentAmmo --;
        nextShotTime = Time.time + (1f / stats.fireRate);
        Debug.Log("Shot");
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
        yield return new WaitForSeconds(stats.reloadTime);
        currentAmmo = stats.maxAmmo;
        isReloading = false;
        Debug.Log("Full reload");
    }

}

