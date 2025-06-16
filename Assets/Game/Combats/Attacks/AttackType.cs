namespace Asce.Game.Combats
{
    [System.Flags]
    public enum AttackType : System.Int32
    {
        None = 0,

        Swipe = 1 << 1,
        Stab = 1 << 2,

        PointAtTarget = 1 << 11,
        Summon = 1 << 12,
        Throw = 1 << 13,
        Cast = 1 << 14,

        Archery = 1 << 21,
    }
}