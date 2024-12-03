using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{

    [SerializeField] private Camera followCamera;
    Vector2 viewportHalfSize;
    float leftBoundaryLimit;
    float rightBoundaryLimit;
    float bottomBoundaryLimit;

    [SerializeField] Tilemap tileMap;
    [SerializeField] Transform target;
    [SerializeField] Vector2 offset;
    [SerializeField] float smoothing = 5f;

    Vector3 shakeOffset = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        tileMap.CompressBounds();
        CalculateCameraBoundaries();
    }

    private void CalculateCameraBoundaries()
    {
        viewportHalfSize = new Vector2(followCamera.orthographicSize * followCamera.aspect, followCamera.orthographicSize);

        leftBoundaryLimit = tileMap.transform.position.x + tileMap.cellBounds.min.x + viewportHalfSize.x;
        rightBoundaryLimit = tileMap.transform.position.x + tileMap.cellBounds.max.x - viewportHalfSize.x;
        bottomBoundaryLimit = tileMap.transform.position.y + tileMap.cellBounds.min.y + viewportHalfSize.y;
    }

    public void LateUpdate()
    {
        Vector3 desiredPosition = target.position + new Vector3(offset.x, offset.y, transform.position.z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, 1 - Mathf.Exp(-smoothing * Time.deltaTime));

        smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, leftBoundaryLimit, rightBoundaryLimit);
        smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, bottomBoundaryLimit, smoothedPosition.y);

        transform.position = smoothedPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shake(float intensity, float duration)
    {
        StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            shakeOffset = Random.insideUnitCircle * intensity;
            elapsed += Time.deltaTime;
            yield return null;
        }
        shakeOffset = Vector3.zero;
    }
}
