using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("Basic Info")]
    public string skillName = "Skill Name";
    public string description = "Skill Description";
    public Sprite icon;

    [Header("Cooldown")]
    public float cooldownTime = 5f;

    [Header("Range")]
    public float castRange = 5f;

    [Header("Casting")]
    public float castTime = 0f;

    [Header("Sound")]
    public AudioClip castSound;
}
