# Dark Monopoly

Unity 기반 보드게임 프로젝트

## 프로젝트 개요

카드 시스템과 유닛 시스템을 결합한 보드게임
- 카드 드래그 앤 드롭으로 유닛 공격 및 이동
- 마나 시스템 기반 카드 사용
- 모디파이어 시스템으로 확장 가능한 전투 시스템

## 주요 시스템

### UnitSystem (`Assets/Scripts/UnitSystem/`)

유닛의 핵심 기능을 컴포넌트로 분리한 구조:

#### Unit (메인 컴포넌트)
- `UnitStat`: 유닛의 기본 스탯 (damage, maxHp, moveSpeed)
- Attacker, Defender, Mover 컴포넌트 통합 관리

#### Attacker
- 공격 기능 담당
- `IHitModifier`: 공격 시 적용되는 수정자 리스트
- `Attack(Defender)`: Hit 시스템 기반 공격

#### Defender
- 방어/체력 관리
- `CurrentHp`, `MaxHp`, `HpRatio`
- `IHitModifier`: 방어 시 적용되는 수정자
- `IHealModifier`: 회복 시 적용되는 수정자
- 이벤트: `onCurrentHpChanged`, `onMaxHpChanged`, `onDeath`
- Debug: ContextMenu "Debug HP Info"

#### Mover
- 이동 기능 담당
- `MoveTo(Vector3)`, `Stop()`
- 코루틴 기반 부드러운 이동

#### Hit / Heal 시스템
```csharp
public struct Hit
{
    public Attacker attacker;
    public Defender target;
    public int baseDamage;
    public int finalDamage;
    public List<Action> postCallbacks;
}

public struct Heal
{
    public Unit healer;
    public Defender target;
    public int baseAmount;
    public int finalAmount;
    public List<Action> postCallbacks;
}
```

#### Modifier Pattern
- `IHitModifier.Apply(Hit)`: 데미지 수정
- `IHealModifier.Apply(Heal)`: 회복량 수정
- 확장 가능한 버프/디버프 시스템

---

### CardSystem (`Assets/Scripts/CardSystem/`)

카드 기반 게임플레이 시스템:

#### Card
```csharp
public class Card
{
    public int baseCost;           // 원래 코스트
    private int costModifier = 0;  // 코스트 증감값
    public int Cost => Mathf.Max(0, baseCost + costModifier);

    public bool needsTarget;       // 타겟 필요 여부
    public List<CardEffectPair> pairs;

    public Action<int> onCostChanged;  // Cost 변경 이벤트
}
```

**Cost 시스템**:
- `baseCost`: Inspector에 노출되는 기본 코스트
- `costModifier`: 버프/디버프로 변경되는 값
- `Cost`: 실제 사용 코스트 (최소 0)
- Cost 변경 시 `onCostChanged` 이벤트로 UI 자동 업데이트

#### CardEffectRegistry
- 카드 효과를 중앙에서 관리
- `CardEffectCategory.Move`: 이동
- `CardEffectCategory.Attack`: 공격 (타겟 필요)

#### CardUI
- 카드 UI 표시
- `manaCostText`: Cost 자동 동기화 (onCostChanged 구독)
- 이벤트 구독/해제로 메모리 누수 방지

---

### UI 시스템 (`Assets/Scripts/UI/`)

#### HandCardDragController
카드 드래그 앤 드롭 처리:

**드래그 시작 (OnBeginDrag)**:
- 마나 체크: 마나 부족 시 드래그 불가
- Placeholder 생성: RectTransform 설정 복사 (anchorMin, anchorMax, sizeDelta, pivot)
- DragLayer로 이동

**드래그 종료 (OnEndDrag)**:
- `needsTarget = true` 카드: 보드 raycast 성공 시만 사용
- `needsTarget = false` 카드: CardWrap 밖에 드롭만 하면 사용

**주요 기능**:
- P키로 Unity Editor 일시정지 (디버깅용)
- 드래그 중 부드러운 카드 이동 (Placeholder로 자리 유지)

#### CardCanvas
- 카드 사용 처리 및 타겟 찾기
- `FindUnitAtPosition()`: 드롭 위치에서 유닛 찾기
- 마나 소비 후 카드 효과 실행

---

## 주요 설계 패턴

### 1. Component-Based Architecture
- Unit = Attacker + Defender + Mover + Stat
- 각 기능을 독립적인 컴포넌트로 분리
- 필요한 기능만 조합 가능

### 2. Modifier Pattern
- IHitModifier, IHealModifier
- 전투/회복 시스템 확장 가능
- 버프/디버프 시스템 기반

### 3. Event-Driven
- `onCostChanged`, `onCurrentHpChanged`, `onDeath` 등
- UI와 로직의 자동 동기화
- 느슨한 결합

### 4. Data-Driven Design
- `needsTarget` 필드로 카드 타입 결정
- CardEffectCategory로 효과 분류
- Inspector에서 설정 가능

---

## 최근 주요 변경사항

### 2025-01 (최근)

**Card Cost 시스템 개선**:
- `baseCost + costModifier` 구조로 변경
- Cost 변경 시 UI 자동 업데이트
- 최소값 0 보장

**카드 드래그 개선**:
- Placeholder 크기 문제 해결 (RectTransform 직접 복사)
- needsTarget 여부에 따른 다른 드롭 조건
- 마나 부족 시 드래그 불가

**디버그 기능**:
- P키로 Editor 일시정지
- Defender ContextMenu: "Debug HP Info"

**UnitSystem 완성**:
- Hit/Heal 시스템 구현
- Modifier 패턴 적용
- 이벤트 기반 UI 연동

---

## TODO / Future Work

### 단기 (구현 예정)
- [ ] FindTileAtPosition() - 타일 타겟 카드용 (CardCanvas.cs:156)
- [ ] Card.DescriptionText() 구현
- [ ] 주석 처리된 코드 정리

### 장기 (고려 중)
- [ ] handCards를 List로 변경 (순서 보장 필요 시)
- [ ] 카드 애니메이션 개선
- [ ] 추가 CardEffectCategory 구현

---

## 파일 구조

```
Assets/Scripts/
├── UnitSystem/
│   ├── Unit.cs                 # 메인 유닛 컴포넌트
│   ├── UnitStat.cs            # 스탯 데이터
│   ├── Attacker.cs            # 공격 기능
│   ├── Defender.cs            # 방어/체력 관리
│   ├── Mover.cs               # 이동 기능
│   ├── Hit.cs                 # 공격 구조체
│   ├── Heal.cs                # 회복 구조체
│   ├── IHitModifier.cs        # 공격/방어 수정자
│   ├── IHealModifier.cs       # 회복 수정자
│   ├── UnitManager.cs         # 유닛 관리
│   └── UnitFactory.cs         # 유닛 생성
│
├── CardSystem/
│   ├── Card.cs                # 카드 데이터 및 로직
│   ├── CardController.cs      # 카드 관리
│   ├── CardEffectRegistry.cs  # 효과 등록/실행
│   └── EffectArgs.cs          # 효과 인자
│
└── UI/
    ├── CardUI.cs              # 카드 UI 표시
    ├── CardCanvas.cs          # 카드 사용 처리
    └── HandCardDragController.cs  # 드래그 앤 드롭
```

---

## 개발 노트

### 설계 의도

**왜 baseCost + costModifier?**
- 버프/디버프가 사라져도 원래 값으로 복귀 가능
- 수치상 음수지만 UI는 0으로 표시 (Mathf.Max)

**왜 LayoutElement가 아닌 RectTransform?**
- LayoutElement는 "요청"만 함
- HLG가 실제 크기를 다르게 계산할 수 있음
- RectTransform 직접 복사가 정확함

**왜 needsTarget 필드?**
- CardEffectCategory는 무수히 확장됨
- 데이터 기반이 더 유연함
- 나중에 Tile 타겟도 추가 예정

---

## 참고 사항

- Unity 버전: 2022.3.49f1
- TextMeshPro 사용
- Unity EventSystem 기반 드래그
