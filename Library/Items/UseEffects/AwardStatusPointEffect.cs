// <copyright file="AwardStatusPointEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.UseEffects.AwardStatusPointEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Items.UseEffects
{
    using System;

    /// <summary>
    /// Defines an <see cref="ItemUseEffect"/> that permanently increases 
    /// the number of avaiable status points of the PlayerEntity.
    /// This class can't be inherited
    /// </summary>
    public sealed class AwardStatusPointEffect : ItemUseEffect
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the number of points awarded by this <see cref="AwardStatusPointEffect"/>.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Set: If the given value is less than or equal to zero.
        /// </exception>
        public int PointsGained
        {
            get 
            {
                return this.pointsGained;
            }

            set
            {
                if( value <= 0 )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsZeroOrNegative, "value" );

                this.pointsGained = value;
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
            if( this.pointsGained == 1 )
            {
                return ItemResources.IUE_AwardsOneStatusPoint;
            }
            else
            {
                return string.Format(
                    System.Globalization.CultureInfo.CurrentCulture,
                    ItemResources.IUE_AwardsXStatusPoints,
                    pointsGained.ToString( System.Globalization.CultureInfo.CurrentCulture )
                );
            }
        }

        /// <summary>
        /// Gets a value that represents how much "Item Points" this <see cref="AwardStatusPointEffect"/> uses.
        /// </summary>
        public override double ItemBudgetUsed
        {
            get
            {
                return pointsGained * 25.0;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="AwardStatusPointEffect"/> class.
        /// </summary>
        public AwardStatusPointEffect()
        {
            this.DestroyItemOnUse = true;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Uses this <see cref="AwardStatusPointEffect"/>, increasing the number
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
            user.Statable.AddStatPoints( this.pointsGained );
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
            context.Write( this.pointsGained );
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
            this.pointsGained = context.ReadInt32();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The number of points awarded by the <see cref="AwardStatusPointEffect"/>.
        /// </summary>
        private int pointsGained = 1;

        #endregion
    }
}
