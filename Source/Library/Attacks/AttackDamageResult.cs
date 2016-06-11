// <copyright file="AttackDamageResult.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.AttackDamageResult structure.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Attacks
{
    using System.Diagnostics;
    using Zelda.Status.Damage;
    
    /// <summary>
    /// Encapsulates the damage result of an attack.
    /// </summary>
    public struct AttackDamageResult : System.IEquatable<AttackDamageResult>
    {
        #region [ Fields ]

        /// <summary>
        /// The amount of damage that was done.
        /// </summary>
        public readonly int Damage;

        /// <summary>
        /// States how the damage was received.
        /// </summary>
        public readonly AttackReceiveType AttackReceiveType;

        /// <summary>
        /// States whether the attack was blocked.
        /// </summary>
        public readonly bool WasBlocked;

        /// <summary>
        /// Descripes the exact type of the damage that has been inflicted.
        /// </summary>
        public readonly DamageTypeInfo DamageTypeInfo;

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="AttackDamageResult"/> structure.
        /// </summary>
        /// <param name="damage">
        /// The amount of damage that was done.
        /// </param>
        /// <param name="attackReceiveType">
        /// States how the attack was received.
        /// </param>
        public AttackDamageResult( int damage, AttackReceiveType attackReceiveType )
        {
            this.Damage = damage;
            this.AttackReceiveType = attackReceiveType;
            this.DamageTypeInfo =  null;
            this.WasBlocked = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttackDamageResult"/> structure.
        /// </summary>
        /// <param name="damage">
        /// The amount of damage that was done.
        /// </param>
        /// <param name="attackReceiveType">
        /// States how the attack was received.
        /// </param>
        /// <param name="damageTypeInfo">
        /// Descripes the exact type of the damage that has been inflicted.
        /// </param>
        private AttackDamageResult( int damage, AttackReceiveType attackReceiveType, DamageTypeInfo damageTypeInfo )
        {
            Debug.Assert( damageTypeInfo != null );

            this.Damage = damage;
            this.AttackReceiveType = attackReceiveType;
            this.DamageTypeInfo = damageTypeInfo;
            this.WasBlocked = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttackDamageResult"/> structure.
        /// </summary>
        /// <param name="damage">
        /// The amount of damage that was done.
        /// </param>
        /// <param name="attackReceiveType">
        /// States how the attack was received.
        /// </param>
        /// <param name="wasBlocked">
        /// States whether the attack was blocked.
        /// </param>
        /// <param name="damageTypeInfo">
        /// Descripes the exact type of the damage that has been inflicted.
        /// </param>
        private AttackDamageResult( int damage, AttackReceiveType attackReceiveType, bool wasBlocked, DamageTypeInfo damageTypeInfo )
        {
            Debug.Assert( damageTypeInfo != null );

            this.Damage = damage;
            this.AttackReceiveType = attackReceiveType;
            this.DamageTypeInfo = damageTypeInfo;
            this.WasBlocked = wasBlocked;
        }

        /// <summary>
        /// Creates a new <see cref="AttackDamageResult"/> that represents
        /// a fully resisted attack.
        /// </summary>
        /// <param name="damageTypeInfo">
        /// Descripes the exact type of the damage that might have been inflicted.
        /// </param>
        /// <returns>
        /// A new AttackDamageResult instance.
        /// </returns>
        internal static AttackDamageResult CreateResisted( DamageTypeInfo damageTypeInfo )
        {
            return new AttackDamageResult( 0, AttackReceiveType.Resisted, damageTypeInfo );
        }
        
        /// <summary>
        /// Creates a new <see cref="AttackDamageResult"/> that represents
        /// an attack that has missed.
        /// </summary>
        /// <param name="damageTypeInfo">
        /// Descripes the exact type of the damage that might have been inflicted.
        /// </param>
        /// <returns>
        /// A new AttackDamageResult instance.
        /// </returns>
        internal static AttackDamageResult CreateMissed( DamageTypeInfo damageTypeInfo )
        {
            return new AttackDamageResult( 0, AttackReceiveType.Miss, damageTypeInfo );
        }

        /// <summary>
        /// Creates a new <see cref="AttackDamageResult"/> that represents
        /// an attack that has been parried.
        /// </summary>
        /// <param name="damageTypeInfo">
        /// Descripes the exact type of the damage that might have been inflicted.
        /// </param>
        /// <returns>
        /// A new AttackDamageResult instance.
        /// </returns>
        internal static AttackDamageResult CreateParried( DamageTypeInfo damageTypeInfo )
        {
            return new AttackDamageResult( 0, AttackReceiveType.Parry, damageTypeInfo );
        }

        /// <summary>
        /// Creates a new <see cref="AttackDamageResult"/> that represents
        /// an attack that has been dodged.
        /// </summary>
        /// <param name="damageTypeInfo">
        /// Descripes the exact type of the damage that might have been inflicted.
        /// </param>
        /// <returns>
        /// A new AttackDamageResult instance.
        /// </returns>
        internal static AttackDamageResult CreateDodged( DamageTypeInfo damageTypeInfo )
        {
            return new AttackDamageResult( 0, AttackReceiveType.Dodge, damageTypeInfo );
        }

        /// <summary>
        /// Creates a new <see cref="AttackDamageResult"/> that represents the normal outcome
        /// of an arabitary attack.
        /// </summary>
        /// <param name="damage">
        /// The damage that has been inflicted.
        /// </param>
        /// <param name="isCrit">
        /// States whether the attack has critted.
        /// </param>
        /// <param name="damageTypeInfo">
        /// Descripes the exact type of the damage that has been inflicted.
        /// </param>
        /// <returns>
        /// A new AttackDamageResult instance.
        /// </returns>
        internal static AttackDamageResult Create( int damage, bool isCrit, DamageTypeInfo damageTypeInfo )
        {
            return new AttackDamageResult( damage, isCrit ? AttackReceiveType.Crit : AttackReceiveType.Hit, damageTypeInfo );
        }


        /// <summary>
        /// Creates a new <see cref="AttackDamageResult"/> that represents the normal outcome
        /// of an arabitary attack.
        /// </summary>
        /// <param name="damage">
        /// The damage that has been inflicted.
        /// </param>
        /// <param name="isCrit">
        /// States whether the attack has critted.
        /// </param>
        /// <param name="wasBlocked">
        /// States whether the attack was blocked.
        /// </param>
        /// <param name="damageTypeInfo">
        /// Descripes the exact type of the damage that has been inflicted.
        /// </param>
        /// <returns>
        /// A new AttackDamageResult instance.
        /// </returns>
        internal static AttackDamageResult Create( int damage, bool isCrit, bool wasBlocked, DamageTypeInfo damageTypeInfo )
        {
            return new AttackDamageResult( damage, isCrit ? AttackReceiveType.Crit : AttackReceiveType.Hit, wasBlocked, damageTypeInfo );
        }

        /// <summary>
        /// Creates a new <see cref="AttackDamageResult"/> that represents the outcome
        /// of damage-over-time effect.
        /// </summary>
        /// <param name="damage">
        /// The damage that has been inflicted.
        /// </param>
        /// <param name="damageTypeInfo">
        /// Descripes the exact type of the damage that has been inflicted.
        /// </param>
        /// <returns>
        /// A new AttackDamageResult instance.
        /// </returns>
        internal static AttackDamageResult CreateDamageOverTime( int damage, DamageTypeInfo damageTypeInfo )
        {
            return new AttackDamageResult( damage, AttackReceiveType.Hit, damageTypeInfo );
        }

        /// <summary>
        /// Creates a new <see cref="AttackDamageResult"/> that represents
        /// a healing effect.
        /// </summary>
        /// <param name="amount">
        /// The amount that has been read.
        /// </param>
        /// <returns>
        /// A new AttackDamageResult instance.
        /// </returns>
        internal static AttackDamageResult CreateHealed( int amount )
        {
            return new AttackDamageResult( amount, AttackReceiveType.Hit );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Returns whether the given Object is equal to this <see cref="AttackDamageResult"/>.
        /// </summary>
        /// <param name="obj">The object to compare this AttackDamageResult instance with.</param>
        /// <returns>true if they are equal; otherwise false.</returns>
        public override bool Equals( object obj )
        {
            if( !(obj is AttackDamageResult) )
                return false;

            AttackDamageResult other = (AttackDamageResult)obj;
            return other.DamageTypeInfo == this.DamageTypeInfo && 
                   other.Damage == this.Damage && 
                   other.AttackReceiveType == this.AttackReceiveType;
        }

        /// <summary>
        /// Returns whether the given <see cref="AttackDamageResult"/> is equal to this <see cref="AttackDamageResult"/>.
        /// </summary>
        /// <param name="other">The AttackDamageResult to compare this AttackDamageResult instance with.</param>
        /// <returns>true if they are equal; otherwise false.</returns>
        public bool Equals( AttackDamageResult other )
        {
            return other.DamageTypeInfo == this.DamageTypeInfo && 
                   other.Damage == this.Damage && 
                   other.AttackReceiveType == this.AttackReceiveType;
        }

        /// <summary>
        /// Returns whether the given <see cref="AttackDamageResult"/> instances are equal.
        /// </summary>
        /// <param name="left">The AttackDamageResult instance on the left side.</param>
        /// <param name="right">The AttackDamageResult instance on the right side.</param>
        /// <returns>true if the given AttackDamageResult instances are equal; otherwise false.</returns>
        public static bool operator ==( AttackDamageResult left, AttackDamageResult right )
        {
            return left.DamageTypeInfo == right.DamageTypeInfo && 
                   left.Damage == right.Damage && 
                   left.AttackReceiveType == right.AttackReceiveType;
        }

        /// <summary>
        /// Returns whether the given <see cref="AttackDamageResult"/> instances are inequal.
        /// </summary>
        /// <param name="left">The AttackDamageResult instance on the left side.</param>
        /// <param name="right">The AttackDamageResult instance on the right side.</param>
        /// <returns>true if the given AttackDamageResult instances are inequal; otherwise false.</returns>
        public static bool operator !=( AttackDamageResult left, AttackDamageResult right )
        {
            return left.DamageTypeInfo != right.DamageTypeInfo ||
                   left.Damage != right.Damage || left.AttackReceiveType != right.AttackReceiveType;
        }

        /// <summary>
        /// Returns the hash code of this <see cref="AttackDamageResult"/> instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return this.DamageTypeInfo.GetHashCode() ^ this.Damage.GetHashCode() ^ this.AttackReceiveType.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of this <see cref="AttackDamageResult"/> instance.
        /// </summary>
        /// <returns>A string representation of this <see cref="AttackDamageResult"/> instance.</returns>
        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();

            sb.Append( this.Damage.ToString( System.Globalization.CultureInfo.CurrentCulture ) );
            sb.Append( ' ' );
            sb.AppendLine( this.AttackReceiveType.ToString() );
            sb.Append( this.DamageTypeInfo.ToString() );

            return sb.ToString();
        }

        #endregion
    }
}
