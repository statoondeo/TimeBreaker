using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    [SerializeField] private Vector3 Rotation;
    [SerializeField] private Transform Target;

	private void Update() => Target.Rotate(Rotation * Time.deltaTime);
}
