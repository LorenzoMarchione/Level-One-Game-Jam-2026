using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour
{
    Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    } 

    private void Update()
    {
        transform. position = Input.mousePosition;
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ShootEffect());
        }
    }

    ShootEffect()
    {
        transform.localScale = originalScale * 1.2f;
        yield return new WaitForSeconds(0.05f);
        transform.localScale = originalScale;
    }

}