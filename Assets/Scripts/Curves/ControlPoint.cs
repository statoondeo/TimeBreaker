using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif

public class ControlPoint : MonoBehaviour
{
	public float Power = 1.0f;
	public Vector3 CheckPoint => transform.position;
	public Vector3 ControlPointA => transform.position - transform.right * Power;
	public Vector3 ControlPointB => transform.position + transform.right * Power;

	#if UNITY_EDITOR

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawLine(ControlPointA, ControlPointB);
		Gizmos.DrawIcon(CheckPoint, "sv_icon_dot11_pix16_gizmo");
		Gizmos.DrawIcon(ControlPointA, "sv_icon_dot3_pix16_gizmo");
		Gizmos.DrawIcon(ControlPointB, "sv_icon_dot3_pix16_gizmo");
	}

	#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(ControlPoint))]
public class ControlPointEditor : Editor
{
	private void OnSceneGUI()
	{
		ControlPoint controlPoint = target as ControlPoint;
		Handles.color = Color.green;
		EditorGUI.BeginChangeCheck();
		Vector3 newPower = Handles.DoScaleHandle(new Vector3(controlPoint.Power, 1.0f, 1.0f), controlPoint.ControlPointB, controlPoint.transform.rotation, 1.0f);
		Vector3 newRotation = Handles.DoPositionHandle(controlPoint.ControlPointA, controlPoint.transform.rotation);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(target, "Update ControlPoint");
			controlPoint.Power = newPower.x;
			controlPoint.transform.rotation = Quaternion.FromToRotation(controlPoint.ControlPointA, newRotation);
		}
	}
}

#endif