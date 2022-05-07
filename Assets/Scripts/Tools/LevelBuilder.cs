using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
	public string LevelName;
	public string BallModelPath;
	public string PaddleModelPath;
	public SplineDrawer SplineDrawer;
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelBuilder))]
public class LevelBuilderEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if (GUILayout.Button("Load Level")) LoadLevel();
		if (GUILayout.Button("Clear Level")) Clearlevel();
		if (GUILayout.Button("Save Level")) SaveLevel();
	}

	private void SaveLevel()
	{
		LevelBuilder levelBuilder = target as LevelBuilder;
		string path = EditorUtility.SaveFilePanel("Save Level", "Assets/Resources/Models/Levels/", levelBuilder.LevelName, "asset");
		if (string.IsNullOrEmpty(path)) return;

		path = FileUtil.GetProjectRelativePath(path);

		LevelModel asset = ScriptableObject.CreateInstance<LevelModel>();

		asset.Name = levelBuilder.LevelName;
		asset.SuccessTimer = 120000;
		asset.BallModel = new BrickInLevelModel() { BrickPrefab = levelBuilder.BallModelPath, SetupCurve = new SetupCurveParameter() { Delay = 0.0f, HasParameter = true, InitialRotation = Quaternion.Euler(0.0f, 0.0f, 270f), InitialtPosition = new Vector3(0.0f, 6.0f, 0.0f), Order = 0, TargetPosition = new Vector3(0.0f, -1.0f, 0.0f), TargetRotation = Quaternion.Euler(0.0f, 0.0f, 270.0f), Timer = 4.0f, Tweening = Tweening.Method.QuintOut, Spline = levelBuilder.SplineDrawer } };
		asset.PaddleModel = new BrickInLevelModel() { BrickPrefab = levelBuilder.PaddleModelPath, SetupCurve = new SetupCurveParameter() { Delay = 0.0f, HasParameter = true, InitialRotation = Quaternion.Euler(0.0f, 0.0f, 270f), InitialtPosition = new Vector3(0.0f, 6.0f, 0.0f), Order = 0, TargetPosition = new Vector3(0.0f, -3.0f, 0.0f), TargetRotation = Quaternion.Euler(0.0f, 0.0f, 270.0f), Timer = 4.0f, Tweening = Tweening.Method.QuintOut, Spline = levelBuilder.SplineDrawer } };
		asset.BrickModels = new List<BrickInLevelModel>();

		for (int i = 0, nbItems = levelBuilder.transform.childCount; i < nbItems; i++)
		{
			GameObject go = levelBuilder.transform.GetChild(i).gameObject;
			asset.BrickModels.Add(new BrickInLevelModel() { BrickPrefab = GetResourcesRelativePath(go), SetupCurve = new SetupCurveParameter() { Delay = 0.0f, HasParameter = true, InitialRotation = Quaternion.Euler(0.0f, 0.0f, 270f), InitialtPosition = new Vector3(go.transform.position.x, 6.0f, 0.0f), Order = 0, TargetPosition = new Vector3(go.transform.position.x, go.transform.position.y, 0.0f), TargetRotation = Quaternion.Euler(0.0f, 0.0f, 270.0f), Timer = 4.0f, Tweening = Tweening.Method.QuintOut, Spline = levelBuilder.SplineDrawer } });
		}

		AssetDatabase.CreateAsset(asset, path);
		AssetDatabase.SaveAssets();
	}

	private void Clearlevel()
	{
		LevelBuilder levelBuilder = target as LevelBuilder;
		for (int i = levelBuilder.transform.childCount - 1; i >= 0; i--) DestroyImmediate(levelBuilder.transform.GetChild(i).gameObject);
	}

	private void LoadLevel()
	{
		LevelBuilder levelBuilder = target as LevelBuilder;
		string path = EditorUtility.OpenFilePanel("Load Level", "Assets/Resources/Models/Levels/", "asset");
		if (string.IsNullOrWhiteSpace(path)) return;

		Clearlevel();
		LevelModel asset = Instantiate(Resources.Load<LevelModel>(GetRelativePath(path)));

		levelBuilder.LevelName = asset.Name;

		BrickInLevelModel brick;
		GameObject go;
		for (int i = 0, nbItems = asset.BrickModels.Count; i < nbItems; i++)
		{
			brick = asset.BrickModels[i];
			go = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>(GetRelativePath(brick.BrickPrefab)), levelBuilder.transform) as GameObject;
			go.transform.position = brick.SetupCurve.TargetPosition;
		}
	}

	private string GetResourcesRelativePath(GameObject go)
	{
		return (GetRelativePath(AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(go))));
	}

	private string GetRelativePath(string fullPath)
	{
		string[] assetPathesList = fullPath.Split(new String[] { "Resources/" }, StringSplitOptions.RemoveEmptyEntries);
		return ((assetPathesList.Length == 0 ? string.Empty : assetPathesList[assetPathesList.Length - 1]).Replace(".asset", string.Empty).Replace(".prefab", string.Empty));
	}
}
#endif