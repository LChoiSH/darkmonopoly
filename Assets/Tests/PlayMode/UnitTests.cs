using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnitSystem;

namespace Tests.PlayMode
{
    /// <summary>
    /// Unit 클래스에 대한 Play Mode 테스트
    /// Unity 환경에서 실행되는 통합 테스트
    /// </summary>
    public class UnitTests
    {
        private GameObject unitGameObject;
        private Unit unit;
        private GameObject unitManagerObject;

        [SetUp]
        public void SetUp()
        {
            // 테스트 중 예외 로그 무시 (HpBar prefab이 없어서 발생하는 에러)
            UnityEngine.TestTools.LogAssert.ignoreFailingMessages = true;

            // UnitManager 설정 (Unit이 OnEnable에서 필요로 함)
            unitManagerObject = new GameObject("UnitManager");
            var unitManager = unitManagerObject.AddComponent<UnitManager>();

            // 각 테스트 전에 새로운 Unit GameObject 생성
            unitGameObject = new GameObject("TestUnit");
            unit = unitGameObject.AddComponent<Unit>();
        }

        [TearDown]
        public void TearDown()
        {
            // 로그 무시 설정 복원
            UnityEngine.TestTools.LogAssert.ignoreFailingMessages = false;

            // 각 테스트 후 GameObject 정리
            if (unitGameObject != null)
            {
                Object.DestroyImmediate(unitGameObject);
            }
            if (unitManagerObject != null)
            {
                Object.DestroyImmediate(unitManagerObject);
            }
        }

        [Test]
        public void Unit_CreatedWithComponent_IsNotNull()
        {
            // Assert
            Assert.IsNotNull(unit, "Unit 컴포넌트가 생성되어야 합니다");
        }

        [Test]
        public void Unit_InitialState_IsIdle()
        {
            // Assert
            Assert.AreEqual(UnitState.Idle, unit.State, "초기 상태는 Idle이어야 합니다");
        }

        [Test]
        public void Unit_SetState_ChangesState()
        {
            // Arrange
            UnitState expectedState = UnitState.Attack;

            // Act
            unit.SetState(expectedState);

            // Assert
            Assert.AreEqual(expectedState, unit.State, "상태가 변경되어야 합니다");
        }

        [Test]
        public void Unit_Stat_IsNotNull()
        {
            // Assert
            Assert.IsNotNull(unit.Stat, "Stat이 초기화되어 있어야 합니다");
        }

        [Test]
        public void Unit_Stat_IsInstanceOfUnitStat()
        {
            // Assert
            Assert.IsInstanceOf<UnitStat>(unit.Stat);
        }

        [UnityTest]
        public IEnumerator Unit_DeathAfterSeconds_ChangesStateToDeath()
        {
            // Act
            unit.StartCoroutine(unit.DeathAfterSeconds(0.1f));

            // Wait for coroutine to start
            yield return null;

            // Assert
            Assert.AreEqual(UnitState.Death, unit.State, "DeathAfterSeconds 호출 후 상태가 Death로 변경되어야 합니다");
        }

        [Test]
        public void Unit_GameObject_HasCorrectName()
        {
            // Assert
            Assert.AreEqual("TestUnit", unitGameObject.name);
        }

        [UnityTest]
        public IEnumerator Unit_DeathSequence_InvokesOnDeathEvent()
        {
            // Arrange
            bool eventInvoked = false;
            unit.onDeath += (u) => eventInvoked = true;

            // Act
            unit.StartCoroutine(unit.DeathAfterSeconds(0.1f));
            yield return null;

            // Assert
            Assert.IsTrue(eventInvoked, "onDeath 이벤트가 발생해야 합니다");
        }
    }
}
