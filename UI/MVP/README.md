# Unity UI MVP Pattern

Unity 프로젝트에서 UI를 구조적으로 관리하기 위한 MVP(Model-View-Presenter) 패턴 구현입니다.  
View와 로직을 철저히 분리하여 유지보수성과 확장성을 높이는 것을 목표로 설계했습니다.

---

## 설계 목표

- View는 화면 표시와 이벤트 발생만 담당, 로직을 갖지 않음
- Presenter는 View를 인터페이스로만 참조하여 구현에 독립적
- 공통 라이프사이클을 베이스 클래스로 추상화하여 반복 코드 제거
- 하위 클래스에서 Presenter를 자유롭게 조합할 수 있는 유연한 구조

---

## 구조

```
📁 MVP
 ├── IMVPView.cs               # View 기본 인터페이스
 ├── IMVPPresenter.cs          # Presenter 기본 인터페이스
 ├── BaseMVPView.cs            # View 공통 베이스 클래스 (MonoBehaviour)
 ├── BaseMVPPresenter.cs       # Presenter 공통 베이스 클래스 (순수 C#)
 ├── IWindowView.cs            # 팝업/윈도우용 View 인터페이스 (확인/취소 이벤트)
 ├── BaseWindowPresenter.cs    # 팝업/윈도우용 Presenter 베이스
 ├── BasePageView.cs           # 팝업/윈도우용 View 베이스 (프리팹에 부착)
 └── UIMVPWindowBase.cs        # Presenter 생성 및 흐름 제어 베이스 (MonoBehaviour)
```

---

## 프리팹 구성

하나의 프리팹에 두 컴포넌트를 함께 부착하는 방식으로 사용합니다.

```
📦 ItemPopup.prefab
 ├── ItemWindow.cs        ← UIMVPWindowBase 상속 (Presenter 생성 및 흐름 제어)
 └── ItemPopupView.cs     ← BasePageView 상속 (순수 View)
      ├── Text (ItemName)
      ├── Button (Confirm)
      └── Button (Cancel)
```

---

## 라이프사이클

```
Init()     이벤트 구독 및 초기화
Open()     View 표시
Refresh()  데이터 갱신
Close()    View 숨김
Free()     이벤트 해제 및 리소스 정리
```

---

## 데이터 흐름

```
유저 입력
   ↓
BasePageView (이벤트 발생)
   ↓
BaseWindowPresenter (이벤트 구독 → 로직 처리)
   ↓
View에 결과 전달 (Refresh)
```

---

## 사용 방법

**1. View 구현 - BasePageView 상속, 프리팹에 부착**

```csharp
public class ItemPopupView : BasePageView
{
    [SerializeField] private TextMeshProUGUI itemNameText;

    public override void Refresh()
    {
        // 데이터 갱신 시 UI 업데이트
    }
}
```

**2. Presenter 구현 - BaseWindowPresenter 상속, 순수 C# 클래스**

```csharp
public class ItemPopupPresenter : BaseWindowPresenter
{
    private readonly GameData model;

    public ItemPopupPresenter(IWindowView view, GameData model, UnityAction onConfirm, UnityAction onCancel)
        : base(view, onConfirm, onCancel)
    {
        this.model = model;
    }

    public override void Refresh()
    {
        View.Refresh();
    }
}
```

**3. UIMVPWindowBase 상속 - 같은 프리팹에 부착, Presenter 생성 및 흐름 제어**

```csharp
public class ItemWindow : UIMVPWindowBase
{
    // windowView는 같은 프리팹에 부착된 ItemPopupView를 Inspector에서 참조
    private ItemPopupPresenter presenter;
    private GameData gameData;

    public override void Init()
    {
        presenter = new ItemPopupPresenter(windowView, gameData, OnConfirm, OnCancel);
        presenter.Init();
    }

    public override void Open() => presenter.Open();
    public override void Close() => presenter.Close();
    public override void Refresh() => presenter.Refresh();
    public override void Free() => presenter.Free();

    private void OnConfirm() { }
    private void OnCancel() { }
}
```

---

## 핵심 설계 원칙

**View는 Presenter를 모른다**  
View는 버튼 클릭을 이벤트로 발생시킬 뿐, 누가 구독하는지 알지 못합니다.

**Presenter는 View를 인터페이스로만 참조한다**  
`IWindowView`를 통해서만 View에 접근하므로 View 구현이 바뀌어도 Presenter 코드는 변경이 없습니다.

**Presenter 생성은 외부(UIMVPWindowBase 하위 클래스)에서 담당한다**  
각 Window 클래스에서 필요한 Presenter를 자유롭게 조합할 수 있습니다.

---

## 확장 방법

새로운 UI가 필요할 때 아래 세 가지만 추가하면 됩니다.

1. `IXxxView` - 필요한 이벤트와 메서드 정의
2. `XxxView : BasePageView` - 프리팹에 부착할 View 구현
3. `XxxPresenter : BaseWindowPresenter` - 로직 처리 Presenter 구현