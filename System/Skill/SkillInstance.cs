public class SkillInstance
{
    public SkillData Data { get; private set; }
    public float RemainingCooldown { get; private set; }

    public SkillInstance(SkillData data)
    {
        Data = data;
        RemainingCooldown = 0f;
    }

    public bool IsReady()
    {
        if (RemainingCooldown <= 0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Tick(float deltaTime)
    {
        if (RemainingCooldown > 0f)
        {
            RemainingCooldown -= deltaTime;

            if (RemainingCooldown < 0f)
            {
                RemainingCooldown = 0f;
            }
        }
    }

    public void TriggerCooldown()
    {
        RemainingCooldown = Data.cooldownDuration;
    }
}
