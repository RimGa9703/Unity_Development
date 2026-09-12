using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    private IProjectileMovement movement;
    private ISkillUser caster;
    private ISkillUser target;
    private SkillData skillData;
    private System.Action<ISkillUser, ISkillUser, SkillData> onHitCallback;
    private float lifetimeRemaining;

    public void Initialize(
        IProjectileMovement movement,
        ISkillUser caster,
        ISkillUser target,
        SkillData skillData,
        float lifetime,
        System.Action<ISkillUser, ISkillUser, SkillData> onHitCallback)
    {
        this.movement = movement;
        this.caster = caster;
        this.target = target;
        this.skillData = skillData;
        this.lifetimeRemaining = lifetime;
        this.onHitCallback = onHitCallback;
    }

    private void Update()
    {
        lifetimeRemaining -= Time.deltaTime;

        if (lifetimeRemaining <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        movement.Move(transform, target, Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        ISkillUser hitUser = other.GetComponent<ISkillUser>();

        if (hitUser == null)
        {
            return;
        }

        if (ReferenceEquals(hitUser, caster) == true)
        {
            return;
        }

        if (onHitCallback != null)
        {
            onHitCallback.Invoke(caster, hitUser, skillData);
        }

        Destroy(gameObject);
    }
}
