// <copyright file="TemporaryStatusEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.UseEffects.TemporaryStatusEffect class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.UseEffects
{
    using Zelda.Status;

    /// <summary>
    /// Defines an <see cref="ItemUseEffect"/> that temporarily enables
    /// one or multiple StatusEffects on the user.
    /// </summary>
    public sealed class TemporaryStatusEffect : ItemUseEffect
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="TimedAura"/> that gets attached to the player
        /// when he uses this ItemUseEffect.
        /// </summary>
        public TimedAura Aura
        {
            get
            {
                return this.aura;
            }
        }

        /// <summary>
        /// Gets a value that represents how much "Item Points" this <see cref="ItemUseEffect"/> uses.
        /// </summary>
        public override double ItemBudgetUsed
        {
            get 
            {
                return 0.0f;
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
            var culture = System.Globalization.CultureInfo.CurrentCulture;

            return string.Format(
                culture,
                Resources.TempStatusUseEffectDescription,
                this.aura.GetEffectDescription( statable ),
                this.aura.Duration.ToString( culture )
            );
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryStatusEffect"/> class.
        /// </summary>
        public TemporaryStatusEffect()
        {
            this.aura = new TimedAura() {
                IsVisible = true
            };
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Uses this TempStatusEffect.
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
                this.aura.ResetDuration();
                this.ResetCooldown();

                if( this.aura.AuraList == null )
                    user.Statable.AuraList.Add( this.aura );
                return true;
            }

            return false;
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
            this.aura.Serialize( context );
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
            int version = context.ReadInt32();
            const int CurrentVersion = 1;
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            // Read Data:
            this.aura.Deserialize( context );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The TimedAura that holds the ArmorEffect that gets applied by this TempStatusEffect.
        /// </summary>
        private readonly TimedAura aura;

        #endregion
    }
}
