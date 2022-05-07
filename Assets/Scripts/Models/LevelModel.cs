using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Level", fileName = "New Level")]
public class LevelModel : ScriptableObject
{
    public string Id;
    public string Name;

    public int Number;
    public float BestTimer;
    public float GoldTimer;
    public float SilverTimer;
    public float BronzeTimer;
    public float SuccessTimer;

    public BrickInLevelModel BallModel;
    public BrickInLevelModel PaddleModel;
    public List<BrickInLevelModel> BrickModels = new List<BrickInLevelModel>();
}
