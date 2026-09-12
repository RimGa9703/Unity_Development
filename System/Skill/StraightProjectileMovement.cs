using UnityEngine;

public class StraightProjectileMovement : IProjectileMovement
{
    private Vector3 direction;
    private float speed;

    public StraightProjectileMovement(Vector3 direction, float speed)
    {
        this.direction = direction.normalized;
        this.speed = speed;
    }

    public void Move(Transform projectileTransform, ISkillUser target, float deltaTime)
    {
        projectileTransform.position += direction * speed * deltaTime;
    }
}
