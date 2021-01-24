// <copyright file="RestoreLifeEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.UseEffects.RestoreLifeEffect class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.UseEffects
{
    using System;
    using Atom.Math;
    using Zelda.Attacks;
    using Zelda.Status;
    
    /// <summary>
    /// Defines an <see cref="ItemUseEffect"/> that heals the user.
    /// This class can't be inherited.
    /// </summary>
    [Obsolete]
    public sealed class RestoreLifeEffect : ItemUseEffect, IRestoreEffect
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the minimum amount of life healed by this RestoreLifeEffect.
        /// </summary>
        public int LifeRestoredMinimum
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum amount of life healed by this RestoreLifeEffect.
        /// </summary>
        public int LifeRestoredMaximum
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the amount of life healed on average.
        /// </summary>
        public int AverageLifeRestored
        {
            get { return (this.LifeRestoredMinimum + this.LifeRestoredMaximum) / 2; }
        }

        /// <summary>
        /// Gets a value indicating how many 'Item Points' this ItemUseEffect effect uses.
        /// </summary>
        public override double ItemBudgetUsed
        {
            get
            {
                // 1 Item Point per 85 Life
                return AverageLifeRestored / 85.0;
            }
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
            return string.Format( 
                System.Globalization.CultureInfo.CurrentCulture,
                Resources.OnUseRestoresXToYLife,
                this.LifeRestoredMinimum.ToString( System.Globalization.CultureInfo.CurrentCulture ),
                this.LifeRestoredMaximum.ToString( System.Globalization.CultureInfo.CurrentCulture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RestoreLifeEffect"/> class.
        /// </summary>
        public RestoreLifeEffect()
        {
        }

        /// <summary>
        /// Setups this <see cref="RestoreLifeEffect"/>.
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
        /// Uses this <see cref="ItemUseEffect"/>.
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
                var statable = user.Statable;

                int amountRestored = rand.RandomRange( this.LifeRestoredMinimum, this.LifeRestoredMaximum );
                bool wasCrit = rand.RandomRange( 0.0f, 100.0f ) <= statable.ChanceTo.CritHeal;

                if( wasCrit )
                {
                    statable.RestoreLife(
                        new AttackDamageResult(
                            (int)(amountRestored * statable.CritModifierHeal * statable.LifePotionEffectiviness),
                            AttackReceiveType.Crit
                        )
                    );
                }
                else
                {
                    statable.RestoreLife(
                        new AttackDamageResult(
                            (int)(amountRestored * statable.LifePotionEffectiviness),
                            AttackReceiveType.Hit
                        )
                    );
                }

                this.ResetCooldown();
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
            switch( powerType )
            {
                case LifeMana.Life:
                    return this.AverageLifeRestored;

                case LifeMana.Mana:
                    return 0;

                default:
                    throw new NotSupportedException();
            }
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

            // Write Header:
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            // Write Data:
            context.Write( this.LifeRestoredMinimum );
            context.Write( this.LifeRestoredMaximum );
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

            // Read Header:
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            // Read Data:
            this.LifeRestoredMinimum = context.ReadInt32();
            this.LifeRestoredMaximum = context.ReadInt32();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// A random number generator.
        /// </summary>
        private Atom.Math.RandMT rand;

        #endregion
    }
}
