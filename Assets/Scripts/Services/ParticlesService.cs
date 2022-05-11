using System.Collections.Generic;
using UnityEngine;

public class ParticlesService : IService
{
	private readonly Dictionary<Particles, ObjectPool<ParticleSystem>> ParticlesPools;
	private readonly ParticlesModel ParticlesModel;

	public ParticlesService(ParticlesModel particlesModel)
	{
		ParticlesPools = new Dictionary<Particles, ObjectPool<ParticleSystem>>();
		ParticlesModel = particlesModel;
	}

	public void Reset()
	{
		ParticlesParam particlesParam;
		for (int i = 0, nbItems = ParticlesModel.ParticlesParams.Length; i < nbItems; i++)
		{
			particlesParam = ParticlesModel.ParticlesParams[i];
			ParticlesPools[particlesParam.ParticleId] = new ObjectPool<ParticleSystem>(() => GameManager.Instantiate(particlesParam.Particles, GameManager.Instance.transform), particlesParam.PoolSize);
		}
	}

	public ParticleSystem Get(Particles particles) => Get(particles, Vector3.zero);

	public ParticleSystem Get(Particles particles, Vector3 position) => Get(particles, position, Quaternion.identity);

	public ParticleSystem Get(Particles particles, Vector3 position, Quaternion rotation) => ParticlesPools[particles].Get(position, rotation);
}
