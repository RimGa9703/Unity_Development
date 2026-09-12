using UnityEngine;

public interface ISkillUser
{
    Transform SkillTransform { get; }
    bool IsAlive { get; }
    void OnSkillUsed(SkillData skillData);
}
