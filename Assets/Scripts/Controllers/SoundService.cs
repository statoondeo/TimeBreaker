using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public partial class SoundService
{
	private readonly SoundsModel SoundsModel;

	private readonly AudioSource MainMusicSource;
	private readonly AudioSource EffectSource;

	private readonly Dictionary<Clips, AudioClip> SoundsAtlas = new Dictionary<Clips, AudioClip>();
	private readonly Dictionary<Musics, AudioClip> MusicsAtlas = new Dictionary<Musics, AudioClip>();

	private readonly Musics[] MusicsShuffled;
	private int CurrentMusicIndex;
	private readonly int NbMusics;

	public SoundService(SoundsModel soundsModel)
	{
		SoundsModel = soundsModel;

		MainMusicSource = GameManager.Instance.gameObject.AddComponent<AudioSource>();
		MainMusicSource.loop = true;
		MainMusicSource.playOnAwake = false;

		EffectSource = GameManager.Instance.gameObject.AddComponent<AudioSource>();
		EffectSource.playOnAwake = false;
		LoadAtlas();

		MusicsShuffled = (MusicsAtlas.Keys).OrderBy(x => Random.value).ToArray();
		CurrentMusicIndex = 0;
		NbMusics = MusicsShuffled.Length;

		MainMusicSource.Stop();
	}

	public void SetVolumes()
	{
		MainMusicSource.volume = GameManager.Instance.OptionsService.GlobalVolume * GameManager.Instance.OptionsService.MusicVolume / 10000.0f;
		EffectSource.volume = GameManager.Instance.OptionsService.GlobalVolume * GameManager.Instance.OptionsService.EffectVolume / 10000.0f;
	}

	private void LoadAtlas()
	{
		if (null != SoundsModel.BusyBeat) MusicsAtlas.Add(Musics.BusyBeat, SoundsModel.BusyBeat);
		if (null != SoundsModel.SpaceDifficulties) MusicsAtlas.Add(Musics.SpaceDifficulties, SoundsModel.SpaceDifficulties);
		if (null != SoundsModel.SpaceUtopia) MusicsAtlas.Add(Musics.SpaceUtopia, SoundsModel.SpaceUtopia);
		if (null != SoundsModel.SubterraneanMonster) MusicsAtlas.Add(Musics.SubterraneanMonster, SoundsModel.SubterraneanMonster);
		if (null != SoundsModel.SwampChase) MusicsAtlas.Add(Musics.SwampChase, SoundsModel.SwampChase);
		if (null != SoundsModel.DigitalStroll) MusicsAtlas.Add(Musics.DigitalStroll, SoundsModel.DigitalStroll);
		if (null != SoundsModel.MileHigh) MusicsAtlas.Add(Musics.MileHigh, SoundsModel.MileHigh);
		if (null != SoundsModel.SadPast) MusicsAtlas.Add(Musics.SadPast, SoundsModel.SadPast);
		if (null != SoundsModel.SpringDay) MusicsAtlas.Add(Musics.SpringDay, SoundsModel.SpringDay);
		if (null != SoundsModel.Stinger) MusicsAtlas.Add(Musics.Stinger, SoundsModel.Stinger);
		if (null != SoundsModel.StormChasers) MusicsAtlas.Add(Musics.StormChasers, SoundsModel.StormChasers);
		if (null != SoundsModel.TractorPull) MusicsAtlas.Add(Musics.TractorPull, SoundsModel.TractorPull);
		if (null != SoundsModel.WhichWayIsUp) MusicsAtlas.Add(Musics.WhichWayIsUp, SoundsModel.WhichWayIsUp);

		SoundsAtlas.Add(Clips.Bonus, SoundsModel.Bonus);
		SoundsAtlas.Add(Clips.Collision1, SoundsModel.Collision1);
		SoundsAtlas.Add(Clips.Collision2, SoundsModel.Collision2);
		SoundsAtlas.Add(Clips.Collision3, SoundsModel.Collision3);
		SoundsAtlas.Add(Clips.Collision4, SoundsModel.Collision4);
		SoundsAtlas.Add(Clips.Collision5, SoundsModel.Collision5);
		SoundsAtlas.Add(Clips.Explosion1, SoundsModel.Explosion1);
		SoundsAtlas.Add(Clips.Explosion2, SoundsModel.Explosion2);
		SoundsAtlas.Add(Clips.Explosion3, SoundsModel.Explosion3);
		SoundsAtlas.Add(Clips.PowerUp, SoundsModel.PowerUp);
		SoundsAtlas.Add(Clips.Regen, SoundsModel.Regen);
		SoundsAtlas.Add(Clips.CountdownAlert, SoundsModel.CountdownAlert);
		SoundsAtlas.Add(Clips.CountdownFinal, SoundsModel.CountdownFinal);
		SoundsAtlas.Add(Clips.Fail, SoundsModel.Fail);
		SoundsAtlas.Add(Clips.Victory, SoundsModel.Victory);
		SoundsAtlas.Add(Clips.ShieldStartBonus, SoundsModel.ShieldStartBonus);
		SoundsAtlas.Add(Clips.ShieldLoopBonus, SoundsModel.ShieldLoopBonus);
		SoundsAtlas.Add(Clips.ShieldEndBonus, SoundsModel.ShieldEndBonus);
		SoundsAtlas.Add(Clips.TimeBonus, SoundsModel.TimeBonus);
		SoundsAtlas.Add(Clips.NukeBonus, SoundsModel.NukeBonus);
		SoundsAtlas.Add(Clips.Star, SoundsModel.Star);
		SoundsAtlas.Add(Clips.Machine4, SoundsModel.Machine4);
	}

	public Musics GetNextMusic()
	{
		Musics music = MusicsShuffled[CurrentMusicIndex];
		CurrentMusicIndex = (CurrentMusicIndex + 1) % NbMusics;
		return (music);
	}

	public bool IsMusicPlaying() => MainMusicSource.isPlaying;

	public void StopMusic()
	{
		if (!MainMusicSource.isPlaying) return;
		MainMusicSource.Stop();
	}

	public void Play(Musics music, float time = 0.0f)
	{
		MainMusicSource.clip = MusicsAtlas[music];
		MainMusicSource.time = time;
		MainMusicSource.Play();
	}

	public void Play(Clips clip) => EffectSource.PlayOneShot(SoundsAtlas[clip]);
}
