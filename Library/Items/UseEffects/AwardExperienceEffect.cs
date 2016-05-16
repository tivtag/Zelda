// <copyright file="AwardExperienceEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.UseEffects.AwardExperienceEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Items.UseEffects
{
    using System;

    /// <summary>
    /// Defines an <see cref="ItemUseEffect"/> that awards the user with experience.
    /// This class can't be inherited.
    /// </summary>
    public sealed class AwardExperienceEffect : ItemUseEffect
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the experience this <see cref="AwardExperienceEffect"/> awards.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Set: If the given value is less than or equal to zero.
        /// </exception>
        public int ExperienceGained
        {
            get
            {
                return this.experience;
            }

            set
            {
                if( value <= 0 )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsZeroOrNegative, "value" );

                this.experience = value;
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
                ItemResources.IUE_AwardsXExperience,
                experience.ToString( System.Globalization.CultureInfo.CurrentCulture )
            );
        }

        /// <summary>
        /// Gets a value that represents how much "Item Points" this <see cref="AwardExperienceEffect"/> uses.
        /// </summary>
        public override double ItemBudgetUsed
        {
            get
            {
                return experience * 0.25;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="AwardExperienceEffect"/> class.
        /// </summary>
        public AwardExperienceEffect()
        {
            this.DestroyItemOnUse = true;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Uses this <see cref="AwardExperienceEffect"/>, increasing the number
        /// of Free Status Points of the given PlayerEntity.
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
            user.Statable.AddExperienceModified( this.experience );
            return true;
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
            context.Write( this.experience );
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
            this.experience = context.ReadInt32();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The amount of experience this <see cref="AwardExperienceEffect"/> awards.
        /// </summary>
        private int experience = 1;

        #endregion
    }
}
