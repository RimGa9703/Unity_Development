using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameItem : GameData
{
    public string code;
    public string iconCode;
    public int count;
    public int maxStackCount;

    public GameItem CloneWithCount(int count)
    {
        return new GameItem { code = this.code, iconCode = this.iconCode, count = count ,maxStackCount = this.maxStackCount};
    }
}
