// <copyright file="MagicSpellDamageMethod.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.MagicSpellDamageMethod class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Attacks.Methods
{
    using Zelda.Saving;
    using Zelda.Status;
    using Zelda.Status.Damage;

    /// <summary>
    /// Defines an AttackDamageMethod that deals MagicMin/MagicMax of damage of a specific spell school.
    /// Warning: This does not use the usual DamageSchoolContainer of ExtendedStatables!
    /// </summary>
    public sealed class MagicSpellDamageMethod : AttackDamageMethod
    {
        /// <summary>
        /// Stores type information about the damage inflicted by the MagicSpellDamageMethod.
        /// </summary>
        private DamageTypeInfo DamageTypeInfo = DamageTypeInfo.CreateMagical(
            DamageSource.Spell,
            ElementalSchool.Fire
        );

        /// <summary>
        /// Gets or sets the Element of the damage.
        /// </summary>
        public ElementalSchool Element
        {
            get
            {
                return this.DamageTypeInfo.Element;
            }

            set
            {
                this.DamageTypeInfo = DamageTypeInfo.WithElement( value );
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the damaage can be dodged.
        /// </summary>
        /// <value>
        /// The default value is false.
        /// </value>
        public bool Dodgable 
        {
            get; 
            set; 
        }
        
        /// <summary>
        /// Calculates the damage done by the <paramref name="user"/> on the <paramref name="target"/>
        /// using this <see cref="AttackDamageMethod"/>.
        /// </summary>
        /// <param name="user">The user of the attack.</param>
        /// <param name="target">The target of the attack.</param>
        /// <returns>The calculated result.</returns>
        public override AttackDamageResult GetDamageDone( Zelda.Status.Statable user, Zelda.Status.Statable target )
        {
            if( target.Resistances.TryResist( DamageTypeInfo.Element ) )
                return AttackDamageResult.CreateResisted( DamageTypeInfo );

            if( this.Dodgable && target.TryDodge( user ) )
                return AttackDamageResult.CreateDodged( DamageTypeInfo );

            int damage = user.DamageMagic.GetRandomValue( this.rand );

            // Apply fixed modifiers:
            damage = target.DamageTaken.ApplyFixed( damage, DamageTypeInfo );

            // Apply multipliers:
            damage = target.DamageTaken.Apply( damage, DamageTypeInfo );

            // Critical:
            bool isCrit = user.TryCrit( target );
            if( isCrit )
                damage = (int)(damage * user.CritModifierSpell);

            return AttackDamageResult.Create( damage, isCrit, DamageTypeInfo );
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            base.Serialize( context );
            context.WriteDefaultHeader();

            // Data
            context.Write( (byte)this.Element );
            context.Write( this.Dodgable );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( IZeldaDeserializationContext context )
        {
            base.Deserialize( context );
            context.ReadDefaultHeader( typeof( MagicSpellDamageMethod ) );

            // Data
            this.Element = (ElementalSchool)context.ReadByte();
            this.Dodgable = context.ReadBoolean();
        }
    }
}
