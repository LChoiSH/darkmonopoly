# Unity Test Framework 설정 완료

## ✅ 설정된 내용

### 1. GameScripts.asmdef
게임 코드 어셈블리 정의 (필요한 모든 패키지 참조 포함)
- VInspector
- Unity.Localization
- Unity.TextMeshPro
- Unity.Addressables

### 2. 테스트 어셈블리
- **EditModeTests**: 빠른 단위 테스트
- **PlayModeTests**: Unity 환경 통합 테스트

## 🎯 제대로 된 Unity Test Framework 사용

### NUnit Attributes
```csharp
[Test]              // 일반 테스트 메서드
[UnityTest]         // Coroutine 테스트 (IEnumerator 반환)
[TestCase(1, 2)]    // 파라미터화된 테스트
[SetUp]             // 각 테스트 전 실행
[TearDown]          // 각 테스트 후 실행
```

### Assert 메서드
```csharp
Assert.AreEqual(expected, actual);
Assert.AreNotEqual(expected, actual);
Assert.IsTrue(condition);
Assert.IsFalse(condition);
Assert.IsNull(obj);
Assert.IsNotNull(obj);
Assert.Greater(a, b);
Assert.Less(a, b);
Assert.IsInstanceOf<Type>(obj);
Assert.DoesNotThrow(() => {});
Assert.Throws<Exception>(() => {});
```

## 🚀 테스트 실행 방법

1. Unity 에디터 열기
2. `Window` → `General` → `Test Runner`
3. **EditMode** 탭:
   - 빠른 단위 테스트
   - `UnitStatTests` 확인
   - `Run All` 클릭
4. **PlayMode** 탭:
   - Unity 환경 테스트
   - `UnitTests` 확인
   - `Run All` 클릭

## 📂 현재 테스트

### EditMode Tests
- `UnitStatTests.cs` - UnitStat 클래스 단위 테스트
  - 기본값 테스트
  - 값 설정 테스트
  - 파라미터화된 테스트

### PlayMode Tests
- `UnitTests.cs` - Unit MonoBehaviour 통합 테스트
  - GameObject 생성 테스트
  - 상태 변경 테스트
  - Coroutine 테스트
  - 이벤트 테스트

## 💡 새 테스트 추가 방법

### EditMode 테스트 (빠른 로직 테스트)
```csharp
// Assets/Tests/EditMode/MyTests.cs
using NUnit.Framework;

public class MyTests
{
    [Test]
    public void MyTest()
    {
        // Arrange
        int expected = 5;

        // Act
        int actual = 2 + 3;

        // Assert
        Assert.AreEqual(expected, actual);
    }
}
```

### PlayMode 테스트 (Unity 환경 테스트)
```csharp
// Assets/Tests/PlayMode/MyGameTests.cs
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class MyGameTests
{
    [Test]
    public void GameObject_Creation_Works()
    {
        var go = new GameObject();
        Assert.IsNotNull(go);
        Object.DestroyImmediate(go);
    }

    [UnityTest]
    public IEnumerator Coroutine_Test()
    {
        yield return null;
        Assert.Pass();
    }
}
```

## 📖 참고 자료

- [Unity Test Framework 공식 문서](https://docs.unity3d.com/Packages/com.unity.test-framework@latest)
- [NUnit 문서](https://docs.nunit.org/)
- [NUnit Assertions](https://docs.nunit.org/articles/nunit/writing-tests/assertions/assertion-models/constraint.html)
