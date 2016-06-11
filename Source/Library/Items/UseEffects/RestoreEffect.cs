// <copyright file="RestoreEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.UseEffects.RestoreEffect class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.UseEffects
{
    using System;
    using System.Text;
    using Atom.Math;
    using Zelda.Status;
    
    /// <summary>
    /// Defines an <see cref="ItemUseEffect"/> that restores the Life, the Mana
    /// or both of the user.
    /// </summary>
    public sealed class RestoreEffect : ItemUseEffect, IRestoreEffect
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the PowerType restored by this RestoreEffect.
        /// </summary>
        public LifeMana PowerType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating how the Amount restored should be interpreted.
        /// </summary>
        public StatusManipType ManipulationType
        {
            get
            {
                return this.valueType;
            }

            set
            {
                if( value == StatusManipType.Rating )
                    throw new ArgumentException( Resources.Error_StatusManipTypeRatingInvalid, "value" );

                this.valueType = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum amount restored by this RestoreEffect.
        /// </summary>
        /// <seealso cref="ValueType"/>
        public int MinimumAmount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum amount restored by this RestoreEffect.
        /// </summary>
        /// <seealso cref="ValueType"/>
        public int MaximumAmount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the average amount restored by this RestoreEffect.
        /// </summary>
        public int AverageAmount
        {
            get
            { 
                return (this.MinimumAmount + this.MaximumAmount) / 2; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the restore effect can crit,
        /// restoring twice the amount.
        /// </summary>
        public bool CanCrit
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a short localised description of this <see cref="ItemUseEffect"/>.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this ItemUseEffect.
        /// </param>
        /// <returns>
        /// The localized description string.
        /// </returns>
        public override string GetDescription( Zelda.Status.Statable statable )
        {
            var sb = new StringBuilder();

            switch( this.ManipulationType )
            {
                case StatusManipType.Fixed:
                    break;

                case StatusManipType.Percental:
                    break;

                default:
                    throw new NotSupportedException();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets a value that represents how much "Item Points" this RestoreEffect uses.
        /// </summary>
        public override double ItemBudgetUsed
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region [ Initialization ]
        
        /// <summary>
        /// Setups this <see cref="RestoreEffect"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provide fast access to game-related services.
        /// </param>
        public override void Setup( IZeldaServiceProvider serviceProvider )
        {
            if( serviceProvider != null )
            {
                this.rand = serviceProvider.Rand;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Uses this RestoreEffect.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that wishes to use this ItemUseEffect.
        /// </param>
        /// <returns>
        /// true if this ItemUseEffect has been used;
        /// otherwise false.
        /// </returns>
        public override bool Use( Zelda.Entities.PlayerEntity user )
        {
            if( this.IsFulfilledBy( user ) )
            {
                this.Restore( this.PowerType, user.Statable );
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the average amount restored of the given power type.
        /// </summary>
        /// <param name="powerType">
        /// The power type.
        /// </param>
        /// <param name="user">
        /// The statable component of the Entity that wants to use this IRestoreEffect.
        /// </param>
        /// <returns>
        /// The average amount restored.
        /// </returns>
        public int GetAverageAmountRestored( LifeMana powerType, Statable user )
        {
            if( this.ManipulationType == StatusManipType.Fixed )
            {
                switch( powerType )
                {
                    case LifeMana.Life:
                        if( this.PowerType == LifeMana.Mana )
                            return 0;
                        return this.AverageAmount;

                    case LifeMana.Mana:
                        if( this.PowerType == LifeMana.Life )
                            return 0;
                        return this.AverageAmount;

                    default:
                        throw new NotSupportedException();
                }
            }
            else
            {
                switch( powerType )
                {
                    case LifeMana.Life:
                        if( this.PowerType == LifeMana.Mana )
                            return 0;
                        return user.GetPercentageOf( LifeMana.Life, this.AverageAmount / 100.0f );

                    case LifeMana.Mana:
                        if( this.PowerType == LifeMana.Life )
                            return 0;
                        return user.GetPercentageOf( LifeMana.Mana, this.AverageAmount / 100.0f );

                    default:
                        throw new NotSupportedException();
                }
            }
        }

        /// <summary>
        /// Restores the life or mana of the given user.
        /// </summary>
        /// <param name="powerType">
        /// The power to restore; Life or Mana.
        /// </param>
        /// <param name="statable">
        /// The ExtendedStatable component of the ZeldaEntity to restore.
        /// </param>
        private void Restore( LifeMana powerType, ExtendedStatable statable )
        {
            int amount = this.rand.RandomRange( this.MinimumAmount, this.MaximumAmount );

            bool wasCrit = false;

            if( this.CanCrit )
            {
                if( this.rand.RandomRange( 0.0f, 100.0f ) <= statable.ChanceTo.CritHeal )
                {
                    amount = (int)(amount * statable.CritModifierHeal);
                    wasCrit = true;
                }
            }

            if( this.ManipulationType == StatusManipType.Percental )
                amount = statable.GetPercentageOf( powerType, amount  / 100.0f );

            var result = new Zelda.Attacks.AttackDamageResult(
                amount,
                wasCrit ? Zelda.Attacks.AttackReceiveType.Crit : Zelda.Attacks.AttackReceiveType.Hit
            );

            statable.Restore( powerType, result );
        }

        #region > Storage <

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

            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.CanCrit );
            context.Write( this.MinimumAmount );
            context.Write( this.MaximumAmount );
            context.Write( (byte)this.PowerType );
            context.Write( (byte)this.ManipulationType );
        }
        
        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            base.Deserialize( context );

            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.CanCrit       = context.ReadBoolean();
            this.MinimumAmount = context.ReadInt32();
            this.MaximumAmount = context.ReadInt32();
            this.PowerType        = (LifeMana)context.ReadByte();
            this.ManipulationType = (StatusManipType)context.ReadByte();
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Represents the storage field of the <see cref="ValueType"/> property.
        /// </summary>
        private StatusManipType valueType;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private IRand rand;

        #endregion
    } 
}