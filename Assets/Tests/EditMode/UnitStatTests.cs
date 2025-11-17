using NUnit.Framework;
using UnitSystem;

namespace Tests.EditMode
{
    /// <summary>
    /// UnitStat 클래스에 대한 Edit Mode 테스트
    /// NUnit Framework를 사용한 표준 단위 테스트
    /// </summary>
    public class UnitStatTests
    {
        [Test]
        public void UnitStat_DefaultValues_AreZero()
        {
            // Arrange & Act
            var stat = new UnitStat();

            // Assert
            Assert.AreEqual(0, stat.damage, "기본 damage는 0이어야 합니다");
            Assert.AreEqual(0, stat.maxHp, "기본 maxHp는 0이어야 합니다");
            Assert.AreEqual(0f, stat.moveSpeed, "기본 moveSpeed는 0이어야 합니다");
        }

        [Test]
        public void UnitStat_SetDamage_StoresCorrectly()
        {
            // Arrange
            var stat = new UnitStat();
            int expectedDamage = 10;

            // Act
            stat.damage = expectedDamage;

            // Assert
            Assert.AreEqual(expectedDamage, stat.damage);
        }

        [Test]
        public void UnitStat_SetMaxHp_StoresCorrectly()
        {
            // Arrange
            var stat = new UnitStat();
            int expectedHp = 100;

            // Act
            stat.maxHp = expectedHp;

            // Assert
            Assert.AreEqual(expectedHp, stat.maxHp);
        }

        [Test]
        public void UnitStat_SetMoveSpeed_StoresCorrectly()
        {
            // Arrange
            var stat = new UnitStat();
            float expectedSpeed = 5.5f;

            // Act
            stat.moveSpeed = expectedSpeed;

            // Assert
            Assert.AreEqual(expectedSpeed, stat.moveSpeed, 0.001f);
        }

        [TestCase(10, 100, 5.5f)]
        [TestCase(0, 0, 0f)]
        [TestCase(-10, -100, -5.5f)]
        public void UnitStat_MultipleValues_CanBeSet(int damage, int maxHp, float moveSpeed)
        {
            // Arrange
            var stat = new UnitStat();

            // Act
            stat.damage = damage;
            stat.maxHp = maxHp;
            stat.moveSpeed = moveSpeed;

            // Assert
            Assert.AreEqual(damage, stat.damage);
            Assert.AreEqual(maxHp, stat.maxHp);
            Assert.AreEqual(moveSpeed, stat.moveSpeed, 0.001f);
        }

        [Test]
        public void UnitStat_DamageCanBeNegative()
        {
            // Arrange
            var stat = new UnitStat();

            // Act
            stat.damage = -10;

            // Assert
            Assert.Less(stat.damage, 0, "Damage can be negative");
        }

        [Test]
        public void UnitStat_MaxHpCanBePositive()
        {
            // Arrange
            var stat = new UnitStat();

            // Act
            stat.maxHp = 100;

            // Assert
            Assert.Greater(stat.maxHp, 0, "MaxHp should be positive");
        }
    }
}
