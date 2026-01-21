using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TaskDetail
{ 

    public SerializableDictionary<string, RequiredMaterial> requiredMaterials = new SerializableDictionary<string, RequiredMaterial>();

    /*
     * *
     */
    public void Clear()
    {
        requiredMaterials.Clear();
    }

    public void SetData(BuildableObjectInfoTable.SRequiredMaterial[] _requiredMaterials)
    {
        requiredMaterials.Clear();
        for (int i = 0; i < _requiredMaterials.Length; i++)
        {
            RequiredMaterial requiredMaterial = new RequiredMaterial();
            requiredMaterial.code = _requiredMaterials[i].code;
            requiredMaterial.requiredMaterialCount = _requiredMaterials[i].count;
            requiredMaterial.submittedMaterialCount = 0;
            requiredMaterials[_requiredMaterials[i].code] = requiredMaterial;
        }
    }
    public void AddRequiredMaterial(string code, int count)
    {
        if (!requiredMaterials.TryGetValue(code, out var material))
            return;

        int remaining = material.requiredMaterialCount - material.submittedMaterialCount;
        int addAmount = Mathf.Min(count, remaining);

        material.submittedMaterialCount += addAmount;
    }
    public bool CheckRequiredMaterial()
    {
        foreach (var material in requiredMaterials.Values)
        {
            if (material.submittedMaterialCount < material.requiredMaterialCount)
                return false;
        }
        return true;
    }
    public float GetProgressPercent()
    {
        float sumCount = 0;
        float submitCount = 0;
        foreach (var material in requiredMaterials.Values)
        {
            sumCount += material.requiredMaterialCount;
            submitCount += material.submittedMaterialCount;
        }
        
        return (submitCount / sumCount);
    }
}
