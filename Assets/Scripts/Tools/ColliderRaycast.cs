using UnityEngine;

//[RequireComponent(typeof(Collider2D))]
public class ColliderRaycast : MonoBehaviour, ICanvasRaycastFilter
{
    private Collider2D Collider;

    private void Awake() => Collider = GetComponent<Collider2D>();

    public bool IsRaycastLocationValid(Vector2 screenPosition, Camera eventCamera) => Collider.OverlapPoint(eventCamera.ScreenToWorldPoint(screenPosition));
}