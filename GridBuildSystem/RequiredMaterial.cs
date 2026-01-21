public class RequiredMaterial
{
    public string code;
    public string iconCode;
    public int requiredMaterialCount;
    public int submittedMaterialCount;

    public RequiredMaterial CloneWithCount(int count)
    {
        return new RequiredMaterial { code = this.code, iconCode = this.iconCode, requiredMaterialCount = count };
    }
}