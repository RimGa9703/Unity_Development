public class Singleton<T> where T : Singleton<T>, new()
{
    private static T instance;
    private static readonly object lockObj = new object();

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = new T();
                        instance.OnInit();
                    }
                }
            }
            return instance;
        }
    }

    public static void Clear()
    {
        instance = null;
    }

    // 선택적으로 자식 클래스에서 초기화 로직을 정의할 수 있게 virtual 메서드 제공
    protected virtual void OnInit() { }
}