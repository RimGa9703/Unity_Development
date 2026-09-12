using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Skills/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("Basic Info")]
    public string skillId;
    public string skillName;
    public Sprite icon;

    [Header("Cooldown")]
    public float cooldownDuration;

    [Header("Cost")]
    public float resourceCost;

    [Header("Activation")]
    public SkillActivationType activationType;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float projectileLifetime = 5f;
    public ProjectileMovementType movementType;
    public float homingTurnSpeed = 180f;

    [Header("Area Settings")]
    public GameObject areaEffectPrefab;
    public float areaRadius = 3f;
    public AreaSpawnPositionType areaSpawnPositionType;
    public AreaDurationType areaDurationType;
    public float areaDuration = 3f;
    public float areaTickInterval = 1f;

    [Header("Effects")]
    public List<SkillEffectData> effects;
}
