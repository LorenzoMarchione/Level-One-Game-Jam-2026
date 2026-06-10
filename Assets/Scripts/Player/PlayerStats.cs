using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    //Munición
    public int maxAmmo = 6;

    //Disparo
    public float fireRate = 2f;      // disparos por segundo
    public float reloadTime = 1.5f;
    public int damage = 1;
    public float hitRadius = 2f;

    //Puntaje
    public float scoreMultiplier = 1f;
}