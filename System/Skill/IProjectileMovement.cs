using UnityEngine;

public interface IProjectileMovement
{
    void Move(Transform projectileTransform, ISkillUser target, float deltaTime);
}
