using System.Collections.Generic;
using UnityEngine;

public enum ParticleType
{
    WARRIOR_ATTACK,
    FORCE_AREA,
    WARRIOR_SKILL,
    MELEE_BOSS_GROUND_HIT,
}

[System.Serializable]
public struct ParticlePrefabData
{
    public ParticleType type;
    public GameObject prefab;
}

public class ParticlePool : Pool<ParticleWrapper>
{
    [SerializeField] private ParticlePrefabData[] particlePrefabs;
    [SerializeField] private ParticleType defaultParticleType = ParticleType.WARRIOR_ATTACK;

    private Dictionary<ParticleType, GameObject> prefabDictionary;


    public override GameObject Prefab => Get(defaultParticleType).gameObject;

    protected override void Awake()
    {
        base.Awake();
        InitializePrefabDictionary();
    }

    private void InitializePrefabDictionary()
    {
        prefabDictionary = new Dictionary<ParticleType, GameObject>();

        foreach (var data in particlePrefabs)
        {
            if(data.prefab != null)
            {
                prefabDictionary[data.type] = data.prefab;
            }
        }
    }


    public ParticleWrapper Get(ParticleType particleType)
    {
        if (!prefabDictionary.ContainsKey(particleType))
            return null;

        foreach (var child in GetChildList(true))
        {
            if(child.gameObject.activeSelf == false && child.ParticleType == particleType)
            {
                child.gameObject.SetActive(true);
                return child;
            }
        }

        var prefab = prefabDictionary[particleType];
        var newParticle = Instantiate(prefab, Vector3.zero, Quaternion.identity, pool).GetComponent<ParticleWrapper>();
        newParticle.ParticleType = particleType;

        return newParticle;
    }

    public ParticleWrapper Get(ParticleType particleType, Vector3 pos, Quaternion rotation)
    {
        var particle = Get(particleType);
        if(particle != null)
        {
            particle.transform.position = pos;
            particle.transform.rotation = rotation;
        }

        return particle;
    }

    public ParticleWrapper Get(ParticleType particleType, Vector3 pos)
    {
        return Get(particleType, pos, Quaternion.identity);
    }

    public override void Return(ParticleWrapper obj)
    {
        if(obj != null)
        {
            obj.Stop();
            obj.Clear();
            base.Return(obj);
        }
    }

}
