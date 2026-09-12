[System.Serializable]
public class SkillEffectData
{
    public SkillEffectType effectType;
    public float value;
    public float duration;
}

public enum SkillEffectType
{
    Damage,
    Heal,
    Buff,
    Debuff
}
