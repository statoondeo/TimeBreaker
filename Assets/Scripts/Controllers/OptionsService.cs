using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OptionsService
{
	private readonly UniversalRenderPipelineAsset PipelineAsset;

	public OptionsService(float globalVolume, float musicsVolume, float soundsVolume)
	{
		PipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

		PostProcessing = PlayerPrefs.GetInt("PostProcessing", 0) == 1;
		GraphicQuality = PlayerPrefs.GetInt("Quality", 1);

		GlobalVolume = PlayerPrefs.GetInt("Global", (int)(globalVolume * 100));
		MusicVolume = PlayerPrefs.GetInt("Music", (int)(musicsVolume * 100));
		EffectVolume = PlayerPrefs.GetInt("Effect", (int)(soundsVolume * 100));
	}

	public event Action OnOptionsChanged;

	private int _PostProcessing;
	public bool PostProcessing
	{
		get => _PostProcessing == 1;
		set
		{
			int newValue = value ? 1 : 0;
			if (_PostProcessing != newValue)
			{
				_PostProcessing = newValue;
				PlayerPrefs.SetInt("PostProcessing", _PostProcessing);
				OnOptionsChanged?.Invoke();
			}
		}
	}

	public int _Quality;
	public int GraphicQuality
	{
		get => _Quality;
		set
		{
			if (_Quality != value)
			{
				_Quality = value;
				switch(_Quality)
				{
					case 0:
						PipelineAsset.renderScale = 0.25f;
						PipelineAsset.msaaSampleCount = 0;
						break;

					case 1:
						PipelineAsset.renderScale = .75f;
						PipelineAsset.msaaSampleCount = 2;
						break;

					case 2:
						PipelineAsset.renderScale = 1.25f;
						PipelineAsset.msaaSampleCount = 4;
						break;

					case 3:
						PipelineAsset.renderScale = 2.0f;
						PipelineAsset.msaaSampleCount = 8;
						break;
				}
				PlayerPrefs.SetInt("Quality", _Quality);
				OnOptionsChanged?.Invoke();
			}
		}
	}

	public int _Global;
	public int GlobalVolume
	{
		get => _Global;
		set
		{
			if (_Global != value)
			{
				_Global = value;
				PlayerPrefs.SetInt("Global", _Global);
				OnOptionsChanged?.Invoke();
			}
		}
	}

	public int _Music;
	public int MusicVolume
	{
		get => _Music;
		set
		{
			if (_Music != value)
			{
				_Music = value;
				PlayerPrefs.SetInt("Music", _Music);
				OnOptionsChanged?.Invoke();
			}
		}
	}

	public int _Effect;
	public int EffectVolume
	{
		get => _Effect;
		set
		{
			if (_Effect != value)
			{
				_Effect = value;
				PlayerPrefs.SetInt("Effect", _Effect);
				OnOptionsChanged?.Invoke();
			}
		}
	}
}
