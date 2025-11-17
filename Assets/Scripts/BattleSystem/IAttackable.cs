namespace BattleSystem
{
    public interface IAttackable
    {
        public int Hp { get; }
        public int CurrentHp { get; }
        public int Damaged(int damage);
    }
}