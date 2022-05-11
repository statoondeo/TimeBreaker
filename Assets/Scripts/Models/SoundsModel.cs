using UnityEngine;

[CreateAssetMenu(menuName = "New Sounds Model", fileName = "New Sounds Model")]
public class SoundsModel : ScriptableObject
{
	[Header("Volumes sonores")]
	[Range(0.0f, 1.0f)] public float GlobalVolume = 1.0f;
	[Range(0.0f, 1.0f)] public float MusicsVolume = 0.25f;
	[Range(0.0f, 1.0f)] public float SoundsVolume = 0.5f;

	[Header("Musiques")]
	public AudioClip BusyBeat;
	public AudioClip DigitalStroll;
	public AudioClip MileHigh;
	public AudioClip SadPast;
	public AudioClip SpaceDifficulties;
	public AudioClip SpaceUtopia;
	public AudioClip SpringDay;
	public AudioClip Stinger;
	public AudioClip StormChasers;
	public AudioClip SubterraneanMonster;
	public AudioClip SwampChase;
	public AudioClip TractorPull;
	public AudioClip WhichWayIsUp;

	[Header("Sons")]
	public AudioClip Bonus;

	public AudioClip Collision1;
	public AudioClip Collision2;
	public AudioClip Collision3;
	public AudioClip Collision4;
	public AudioClip Collision5;

	public AudioClip Explosion1;
	public AudioClip Explosion2;
	public AudioClip Explosion3;

	public AudioClip PowerUp;
	public AudioClip Regen;

	public AudioClip CountdownAlert;
	public AudioClip CountdownFinal;

	public AudioClip Fail;
	public AudioClip Victory;

	public AudioClip ShieldStartBonus;
	public AudioClip ShieldLoopBonus;
	public AudioClip ShieldEndBonus;
	public AudioClip TimeBonus;
	public AudioClip NukeBonus;

	public AudioClip Star;
	public AudioClip Machine4;
}
