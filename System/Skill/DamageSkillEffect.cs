using UnityEngine;

public class DamageSkillEffect : ISkillEffect
{
    public void Execute(ISkillUser caster, ISkillUser target, SkillEffectData effectData)
    {
        if (target == null)
        {
            return;
        }

        if (target.IsAlive == false)
        {
            return;
        }

        // TODO: 실제 데미지 적용 로직 연결 (예: target의 HP 컴포넌트 접근)
        Debug.Log(target.SkillTransform.name + "에게 " + effectData.value + " 데미지");
    }
}
