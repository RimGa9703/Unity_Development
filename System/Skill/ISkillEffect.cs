public interface ISkillEffect
{
    void Execute(ISkillUser caster, ISkillUser target, SkillEffectData effectData);
}
