using UnityEngine;

public class FitWorkground : MonoBehaviour
{
    private Camera MainCamera;
	private Vector2 TargetSize = new Vector2(5.0f, 10.0f);

	private float OperandProduct;
	private float InitialSize;

	private void Awake()
    {
        MainCamera = GetComponent<Camera>();
		InitialSize = MainCamera.orthographicSize;
		OperandProduct = TargetSize.x * TargetSize.y;
		FitOrthographicSize();
	}

	private void FitOrthographicSize()
	{
		Vector3 bottomLeft = MainCamera.ViewportToWorldPoint(Vector3.zero);
		Vector3 topRight = MainCamera.ViewportToWorldPoint(new Vector3(MainCamera.rect.width, MainCamera.rect.height));
		Vector3 screenSize = topRight - bottomLeft;

		MainCamera.orthographicSize = Mathf.Max(screenSize.y * OperandProduct / screenSize.x * 0.05f, InitialSize);
	}
}
