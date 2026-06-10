using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour
{
    Vector3 originalScale;
    Camera mainCamera;

    private void Start()
    {
        originalScale = transform.localScale;
        mainCamera = Camera.main;
    } 

    private void Update()
    {
        if(mainCamera == null)
        {
            Debug.Log("camara no se a detectado");
            return;
        }
        Vector2 cursorPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = cursorPos;
        
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ShootEffect());
        }
    }

    public IEnumerator ShootEffect()
    {
        transform.localScale = originalScale * 1.2f;
        yield return new WaitForSeconds(0.05f);
        transform.localScale = originalScale;
    }
}