using UnityEngine;

public class AreaEffectZone : MonoBehaviour
{
    private ISkillUser caster;
    private SkillData skillData;
    private float radius;
    private AreaDurationType durationType;
    private float remainingDuration;
    private float tickInterval;
    private float tickTimer;
    private System.Action<ISkillUser, ISkillUser, SkillData> onHitCallback;

    public void Initialize(
        ISkillUser caster,
        SkillData skillData,
        float radius,
        AreaDurationType durationType,
        float duration,
        float tickInterval,
        System.Action<ISkillUser, ISkillUser, SkillData> onHitCallback)
    {
        this.caster = caster;
        this.skillData = skillData;
        this.radius = radius;
        this.durationType = durationType;
        this.remainingDuration = duration;
        this.tickInterval = tickInterval;
        this.tickTimer = 0f;
        this.onHitCallback = onHitCallback;

        if (durationType == AreaDurationType.Instant)
        {
            ApplyEffectToTargetsInRange();
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (durationType != AreaDurationType.Persistent)
        {
            return;
        }

        remainingDuration -= Time.deltaTime;
        tickTimer -= Time.deltaTime;

        if (tickTimer <= 0f)
        {
            ApplyEffectToTargetsInRange();
            tickTimer = tickInterval;
        }

        if (remainingDuration <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void ApplyEffectToTargetsInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            ISkillUser hitUser = hitColliders[i].GetComponent<ISkillUser>();

            if (hitUser == null)
            {
                continue;
            }

            if (ReferenceEquals(hitUser, caster) == true)
            {
                continue;
            }

            if (hitUser.IsAlive == false)
            {
                continue;
            }

            if (onHitCallback != null)
            {
                onHitCallback.Invoke(caster, hitUser, skillData);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
