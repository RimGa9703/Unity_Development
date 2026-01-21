using System.Text;

/// <summary>
/// string 조합이 필요할때 사용 하기 위한 전역 클래스
/// 사용법 :
/// StringMaker.Clear();
/// StringMaker.stringBuilder.Append("[ffca6c]Lv [ffffff]");
/// StringMaker.stringBuilder.Append(heroInfo.level);
/// heroLevel.text = StringMaker.stringBuilder.ToString();
/// </summary>

static public class StringMaker
{
    static public StringBuilder stringBuilder = new StringBuilder();

    static public void Clear()
    {
        stringBuilder.Remove(0, stringBuilder.Length);
    }
}
