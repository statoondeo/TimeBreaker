using UnityEngine;

[CreateAssetMenu()]
public class LootModel : ScriptableObject
{
	public float LootRate = 0.25f;
	public int PreLoad = 6;
	public float RechargeTime = 10.0f;
	public LootParam[] Loots;
}
