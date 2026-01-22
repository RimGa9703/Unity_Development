# Unity FSM (ScriptableObject 기반 상태 머신)

이 폴더는 **Unity에서 ScriptableObject 기반으로 구성된 FSM(Finite State Machine)** 구현 코드입니다.

---

## 핵심 구조

이 FSM은 크게 **State / Action / Decision / Controller** 네 가지 요소로 구성됩니다.

### 1. FSM_State

각 상태(State)를 정의하는 ScriptableObject입니다.

* 상태 진입 조건(Entry Decision)
* 상태 유지 중 검사 조건(Update Decision)
* 상태에서 수행할 Action 목록
* FixedUpdate 사용 여부 설정 가능

```text
FSM_State
 ├─ Entry Decisions
 ├─ Update Decisions
 ├─ Actions[]
 └─ Option Flags (Hold / UpdateCheck / FixedUpdate)
```

**특징**

* 다수의 Decision을 AND 조건으로 평가
* NOT 조건 지원
* 상태 자체는 로직을 가지지 않고 “조합된 데이터” 역할만 수행

---

### 2. FSM_Action

상태에서 실제로 수행되는 **행동 단위 로직**입니다.

```csharp
public abstract class FSM_Action : ScriptableObject
{
    EntryAction()
    UpdateAction()
    FixedUpdateAction()
    ExitAction()
}
```

**특징**

* 상태 진입 / 업데이트 / 종료 시점이 명확히 분리됨
* 여러 Action을 하나의 State에 조합 가능
* UpdateAction의 반환값으로 상태 유지 여부를 제어

---

### 3. FSM_Decision

상태 전환 여부를 판단하는 **조건 로직 단위**입니다.

```csharp
public abstract class FSM_Decision : ScriptableObject
{
    bool Decide(StateController controller);
}
```

**특징**

* 상태 진입 및 유지 조건에 사용
* 상태와 완전히 분리되어 재사용 가능
* ScriptableObject 기반이므로 Inspector에서 조합 가능

---

### 4. StateController

FSM 전체를 관리하는 **실행 컨트롤러**입니다.

* 현재 상태 관리
* 최적 상태 탐색
* 상태 전환 처리
* Animator 파라미터 초기화 및 제어

```text
StateController
 ├─ CurrentState
 ├─ IdleState
 ├─ StateInfo List (우선순위 기반)
 └─ Animator 연동
```

**상태 전환 규칙**

1. 현재 상태의 Action / Update Decision 실패 시 상태 변경
2. Entry 조건을 만족하는 State 중 우선순위가 가장 높은 State 선택
3. 조건 만족 State가 없으면 Idle State로 복귀

---

## FSM 동작 흐름

```text
Update()
 ├─ 현재 상태가 Idle → Best State 탐색
 ├─ Action.UpdateAction 실행
 │    └─ false 반환 시 상태 변경
 ├─ Update Decision 검사
 │    └─ 실패 시 상태 변경
```

```text
상태 변경 시
 ├─ ExitAction 호출
 ├─ Animator 파라미터 초기화
 ├─ EntryAction 호출
```

---

## 설계 의도

* **상태 로직과 조건 로직 분리**
* if / switch 기반 상태 분기 제거
* 디자이너 친화적인 데이터 기반 FSM 구성
* ScriptableObject를 활용한 재사용성 강화

---

## 사용 예시 흐름

1. FSM_State ScriptableObject 생성
2. FSM_Action / FSM_Decision 구현 클래스 작성
3. StateController에 StateInfo 등록
4. 우선순위 및 조건 설정
5. 런타임에서 자동 상태 전환 체크

---

## 제한 사항 및 주의점

* 단일 StateController 기준 설계 (멀티 FSM 간 상호작용 고려 안 됨)
* 상태 전환 그래프 시각화 기능 없음
* Entry / Update 조건이 많아질 경우 디버깅 난이도 증가 가능

---