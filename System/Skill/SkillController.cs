using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    [SerializeField]
    private List<SkillData> initialSkills;

    [Header("Resource")]
    [SerializeField]
    private float maxResource = 100f;

    [SerializeField]
    private float currentResource;

    [SerializeField]
    private float resourceRegenPerSecond = 0f;

    public float CurrentResource => currentResource;
    public float MaxResource => maxResource;

    private List<SkillInstance> skillInstances = new List<SkillInstance>();
    private ISkillUser owner;
    private Dictionary<SkillEffectType, ISkillEffect> effectExecutors;

    private void Awake()
    {
        owner = GetComponent<ISkillUser>();

        if (owner == null)
        {
            Debug.LogError(gameObject.name + "에 ISkillUser를 구현한 컴포넌트가 없습니다.");
        }

        currentResource = maxResource;

        InitEffectExecutors();
        InitSkills();
    }

    private void InitEffectExecutors()
    {
        effectExecutors = new Dictionary<SkillEffectType, ISkillEffect>();
        effectExecutors.Add(SkillEffectType.Damage, new DamageSkillEffect());
        // TODO: Heal, Buff, Debuff 실행기 추가
    }

    private void InitSkills()
    {
        for (int i = 0; i < initialSkills.Count; i++)
        {
            SkillInstance instance = new SkillInstance(initialSkills[i]);
            skillInstances.Add(instance);
        }
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        TickSkills(deltaTime);
        RegenResource(deltaTime);
    }

    private void TickSkills(float deltaTime)
    {
        for (int i = 0; i < skillInstances.Count; i++)
        {
            skillInstances[i].Tick(deltaTime);
        }
    }

    private void RegenResource(float deltaTime)
    {
        if (resourceRegenPerSecond <= 0f)
        {
            return;
        }

        currentResource += resourceRegenPerSecond * deltaTime;

        if (currentResource > maxResource)
        {
            currentResource = maxResource;
        }
    }

    public bool TryUseSkill(int skillIndex, ISkillUser target)
    {
        if (skillIndex < 0 || skillIndex >= skillInstances.Count)
        {
            return false;
        }

        if (owner.IsAlive == false)
        {
            return false;
        }

        SkillInstance instance = skillInstances[skillIndex];

        if (instance.IsReady() == false)
        {
            return false;
        }

        if (HasEnoughResource(instance.Data.resourceCost) == false)
        {
            return false;
        }

        ConsumeResource(instance.Data.resourceCost);
        ExecuteSkill(instance, target);
        instance.TriggerCooldown();
        owner.OnSkillUsed(instance.Data);

        return true;
    }

    private bool HasEnoughResource(float cost)
    {
        if (currentResource >= cost)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ConsumeResource(float cost)
    {
        currentResource -= cost;

        if (currentResource < 0f)
        {
            currentResource = 0f;
        }
    }

    private void ExecuteSkill(SkillInstance instance, ISkillUser target)
    {
        if (instance.Data.activationType == SkillActivationType.Instant)
        {
            ApplySkillEffects(owner, instance.Data, target);
        }
        else if (instance.Data.activationType == SkillActivationType.Projectile)
        {
            SpawnProjectile(instance.Data, target);
        }
        else if (instance.Data.activationType == SkillActivationType.Area)
        {
            SpawnAreaZone(instance.Data, target);
        }
    }

    private void ApplySkillEffects(ISkillUser caster, SkillData skillData, ISkillUser target)
    {
        List<SkillEffectData> effects = skillData.effects;

        for (int i = 0; i < effects.Count; i++)
        {
            SkillEffectType effectType = effects[i].effectType;

            if (effectExecutors.ContainsKey(effectType) == true)
            {
                effectExecutors[effectType].Execute(caster, target, effects[i]);
            }
        }
    }

    private void SpawnProjectile(SkillData skillData, ISkillUser target)
    {
        if (skillData.projectilePrefab == null)
        {
            Debug.LogError(skillData.skillName + " 스킬에 프로젝타일 프리팹이 설정되지 않았습니다.");
            return;
        }

        Vector3 spawnPosition = owner.SkillTransform.position;
        Vector3 initialDirection = owner.SkillTransform.forward;

        if (target != null)
        {
            initialDirection = (target.SkillTransform.position - spawnPosition).normalized;
        }

        GameObject projectileObject = Instantiate(skillData.projectilePrefab, spawnPosition, Quaternion.LookRotation(initialDirection));
        Projectile projectile = projectileObject.GetComponent<Projectile>();

        if (projectile == null)
        {
            Debug.LogError(skillData.projectilePrefab.name + "에 Projectile 컴포넌트가 없습니다.");
            Destroy(projectileObject);
            return;
        }

        IProjectileMovement movement = CreateMovement(skillData, initialDirection);
        projectile.Initialize(movement, owner, target, skillData, skillData.projectileLifetime, OnEffectHit);
    }

    private IProjectileMovement CreateMovement(SkillData skillData, Vector3 initialDirection)
    {
        if (skillData.movementType == ProjectileMovementType.Homing)
        {
            return new HomingProjectileMovement(initialDirection, skillData.projectileSpeed, skillData.homingTurnSpeed);
        }
        else
        {
            return new StraightProjectileMovement(initialDirection, skillData.projectileSpeed);
        }
    }

    private void SpawnAreaZone(SkillData skillData, ISkillUser target)
    {
        if (skillData.areaEffectPrefab == null)
        {
            Debug.LogError(skillData.skillName + " 스킬에 장판 이펙트 프리팹이 설정되지 않았습니다.");
            return;
        }

        Vector3 spawnPosition;

        if (skillData.areaSpawnPositionType == AreaSpawnPositionType.SelfCentered)
        {
            spawnPosition = owner.SkillTransform.position;
        }
        else
        {
            if (target != null)
            {
                spawnPosition = target.SkillTransform.position;
            }
            else
            {
                spawnPosition = owner.SkillTransform.position;
            }
        }

        GameObject areaObject = Instantiate(skillData.areaEffectPrefab, spawnPosition, Quaternion.identity);
        AreaEffectZone zone = areaObject.GetComponent<AreaEffectZone>();

        if (zone == null)
        {
            Debug.LogError(skillData.areaEffectPrefab.name + "에 AreaEffectZone 컴포넌트가 없습니다.");
            Destroy(areaObject);
            return;
        }

        zone.Initialize(
            owner,
            skillData,
            skillData.areaRadius,
            skillData.areaDurationType,
            skillData.areaDuration,
            skillData.areaTickInterval,
            OnEffectHit);
    }

    private void OnEffectHit(ISkillUser caster, ISkillUser hitTarget, SkillData skillData)
    {
        ApplySkillEffects(caster, skillData, hitTarget);
    }

    public float GetRemainingCooldown(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= skillInstances.Count)
        {
            return 0f;
        }

        return skillInstances[skillIndex].RemainingCooldown;
    }
}
