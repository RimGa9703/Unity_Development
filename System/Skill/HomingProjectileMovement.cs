using UnityEngine;

public class HomingProjectileMovement : IProjectileMovement
{
    private Vector3 currentDirection;
    private float speed;
    private float turnSpeedDegreesPerSecond;

    public HomingProjectileMovement(Vector3 initialDirection, float speed, float turnSpeedDegreesPerSecond)
    {
        currentDirection = initialDirection.normalized;
        this.speed = speed;
        this.turnSpeedDegreesPerSecond = turnSpeedDegreesPerSecond;
    }

    public void Move(Transform projectileTransform, ISkillUser target, float deltaTime)
    {
        if (target != null && target.IsAlive == true)
        {
            Vector3 desiredDirection = (target.SkillTransform.position - projectileTransform.position).normalized;
            float maxRadiansDelta = turnSpeedDegreesPerSecond * Mathf.Deg2Rad * deltaTime;

            currentDirection = Vector3.RotateTowards(currentDirection, desiredDirection, maxRadiansDelta, 0f);
        }

        projectileTransform.position += currentDirection * speed * deltaTime;
        projectileTransform.forward = currentDirection;
    }
}
