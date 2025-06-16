using Asce.Managers.Utils;
using UnityEngine;

namespace Asce.Game.Combats
{
    /// <summary>
    ///     Provides extension methods for working with <see cref="AttackType"/> flags.
    ///     Includes categorization (melee, ranged, magic), state checks, and bitwise utilities.
    /// </summary>
    public static class AttackTypeExtension
    {
        /// <summary>
        ///     Represents all melee-based attack types.
        /// </summary>
        public static readonly AttackType meleeMask = AttackType.Swipe | AttackType.Stab;

        /// <summary>
        ///     Represents all ranged-based attack types.
        /// </summary>
        public static readonly AttackType rangedMask = AttackType.Throw | AttackType.Archery;

        /// <summary>
        ///     Represents all magic-based attack types.
        /// </summary>
        public static readonly AttackType magicMask = AttackType.Cast | AttackType.Summon | AttackType.PointAtTarget;

        /// <summary>
        ///     Checks whether the attack type includes any melee attack.
        /// </summary>
        public static bool HasMeleeAttack(this AttackType type) => type.HasAttack(meleeMask);

        /// <summary>
        ///     Checks whether the attack type includes any ranged attack.
        /// </summary>
        public static bool HasRangedAttack(this AttackType type) => type.HasAttack(rangedMask);

        /// <summary>
        ///     Checks whether the attack type includes any magic attack.
        /// </summary>
        public static bool HasMagicAttack(this AttackType type) => type.HasAttack(magicMask);

        /// <summary>
        ///     Checks whether the attack type contains only melee attacks (and nothing else).
        /// </summary>
        public static bool IsOnlyMelee(this AttackType type) => type.IsOnly(meleeMask);

        /// <summary>
        ///     Checks whether the attack type contains only ranged attacks (and nothing else).
        /// </summary>
        public static bool IsOnlyRanged(this AttackType type) => type.IsOnly(rangedMask);

        /// <summary>
        ///     Checks whether the attack type contains only magic attacks (and nothing else).
        /// </summary>
        public static bool IsOnlyMagic(this AttackType type) => type.IsOnly(magicMask);

        /// <summary>
        ///     Determines whether the given attack type can be performed while crawling.
        ///     <br/>
        ///     Only pure melee attacks are allowed while crawling.
        /// </summary>
        public static bool CanAttackWhenCrawling(this AttackType type)
        {
            // Limit crawling attacks to only melee types (Swipe, Stab)
            return type.IsOnly(AttackType.Swipe | AttackType.Stab);
        }

        /// <summary>
        ///     Adds the specified <paramref name="addType"/> to the existing <paramref name="type"/> flags.
        /// </summary>
        /// <param name="type"> The reference to the current attack type. </param>
        /// <param name="addType"> The attack type(s) to add. </param>
        /// <returns> Returns the updated <see cref="AttackType"/> with added flag(s).</returns>
        public static AttackType Add(ref this AttackType type, AttackType addType)
        {
            type |= addType;
            return type;
        }

        /// <summary>
        ///     Removes the specified <paramref name="removeType"/> from the existing <paramref name="type"/> flags.
        /// </summary>
        /// <param name="type"> The reference to the current attack type. </param>
        /// <param name="removeType"> The attack type(s) to remove. </param>
        /// <returns> Returns the updated <see cref="AttackType"/> without the removed flag(s). </returns>
        public static AttackType Remove(ref this AttackType type, AttackType removeType)
        {
            type &= ~removeType;
            return type;
        }

        /// <summary>
        ///     Checks whether the attack type contains a specific <paramref name="checkType"/> flag.
        /// </summary>
        public static bool Contains(this AttackType type, AttackType checkType)
        {
            return (type & checkType) != 0;
        }

        /// <summary>
        ///     Converts a single-flag <see cref="AttackType"/> into its corresponding zero-based integer index (bit position).
        /// </summary>
        /// <returns>
        ///     Returns the bit index (0-based) if <paramref name="type"/> is a single flag.
        ///     <br/>
        ///     Returns 0 if <see cref="AttackType.None"/>.
        ///     <br/>
        ///     Returns -1 if multiple flags are set (invalid for this usage).
        /// </returns>
        public static int ToIntValue(this AttackType type)
        {
            if (type == AttackType.None) return 0;

            int raw = (int)type;

            // Check if only one bit is set using bitwise trick: n & (n - 1) == 0
            if ((raw & (raw - 1)) != 0)
            {
                Debug.LogError($"[{"ToIntValue".ColorWrap(Color.green)}] '{type}' is not a single flag (multiple flags set).");
                return -1;
            }

            // Compute the bit index (log2) of the flag
            return Mathf.RoundToInt(Mathf.Log(raw, 2));
        }

        /// <summary>
        ///     Checks if the <paramref name="type"/> includes at least one flag from the given <paramref name="mask"/>.
        /// </summary>
        private static bool HasAttack(this AttackType type, AttackType mask)
        {
            return (type & mask) != 0;
        }

        /// <summary>
        ///     Checks if the <paramref name="type"/> contains only flags from the <paramref name="mask"/>,
        ///     and is not <see cref="AttackType.None"/>.
        /// </summary>
        private static bool IsOnly(this AttackType type, AttackType mask)
        {
            // Ensure type contains only the mask bits (no others), and not None
            return (type & ~mask) == 0 && type != AttackType.None;
        }
    }
}