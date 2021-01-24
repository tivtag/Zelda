// <copyright file="Gem.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Gem class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items
{
    using System;
    using Zelda.Status;

    /// <summary>
    /// A Gem is an <see cref="Item"/> that can be placed into a <see cref="Socket"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class Gem : Item
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="ItemType"/> this <see cref="Gem"/> represents.
        /// </summary>
        public override ItemType ItemType
        {
            get 
            { 
                return ItemType.Gem;
            }
        }

        /// <summary>
        /// Gets the status effects this <see cref="Gem"/> provides to the one that sockets it, if any.
        /// </summary>
        public PermanentAura EffectAura
        {
            get 
            { 
                return this.effects; 
            }
        }

        /// <summary>
        /// Gets or sets the 'color' of this <see cref="Gem"/>.
        /// </summary>
        /// <remarks>
        /// Remember the (gem-socket color relation!)
        /// </remarks>
        public ElementalSchool GemColor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the level required to use this <see cref="Gem"/>.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Set: If the given value is negative.
        /// </exception>
        public int RequiredLevel
        {
            get 
            {
                return this.requiredLevel; 
            }

            set 
            {
                if( value < 0 )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "value" );

                this.requiredLevel = value;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Modifies the power of this Gem by the given factor.
        /// </summary>
        /// <param name="factor">
        /// The factor to modify the item by.
        /// </param>
        /// <returns>
        /// true if modifications were allowed; -or- otherwise false.
        /// </returns>
        public override bool ModifyPowerBy( float factor )
        {
            if( base.ModifyPowerBy( factor ) )
            {
                effects.ModifyPowerBy( factor );
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a new instance of this <see cref="Gem"/>.
        /// </summary>
        /// <param name="powerFactor">
        /// The factor by which the power of the new ItemInstance varies compared to this base Item.
        /// </param>
        /// <returns>A new <see cref="GemInstance"/>.</returns>
        public override ItemInstance CreateInstance( float powerFactor )
        {
            return new GemInstance( this, powerFactor );
        }

        /// <summary>
        /// Gets a value indicating whether the given Statable component
        /// fulfills the requirements to socket this Gem.
        /// </summary>      
        /// <param name="statable">
        /// The <see cref="Statable"/> component of the ZeldaEntity
        /// that wishes to socket an instance of this <see cref="Gem"/>.
        /// </param>
        /// <returns>
        /// true if the Statable would be able to socket an instance of this <see cref="Gem"/>;
        /// otherwise false.
        /// </returns>
        internal bool FulfillsRequirements( Statable statable )
        {
            return statable.Level >= this.requiredLevel;
        }

        #region > Cloning <

        /// <summary>
        /// Creates a clone of this <see cref="Gem"/>.
        /// </summary>
        /// <returns>
        /// The cloned item.
        /// </returns>
        public override Item Clone()
        {
            var clone = new Gem();

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the given Gem to be a clone of this Gem.
        /// </summary>
        /// <param name="clone">
        /// The Gem to setup as a clone of this Gem.
        /// </param>
        private void SetupClone( Gem clone )
        {
            base.SetupClone( clone );

            clone.GemColor = this.GemColor;
            clone.requiredLevel = this.requiredLevel;

            this.effects.SetupClone( clone.effects );
        }

        #endregion

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
            // Write Item Data:
            base.Serialize( context );

            // Write Gem Header:
            const int Version = 1;
            context.Write( Version );

            // Write Gem Data:
            context.Write( (int)this.GemColor );
            context.Write( this.RequiredLevel );

            this.effects.Serialize( context );
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
            // Read Item Header/Data:
            base.Deserialize( context );

            // Read Gem Header:
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            // Read Gem Data:
            this.GemColor      = (ElementalSchool)context.ReadInt32();
            this.RequiredLevel = context.ReadInt32();

            this.effects.Deserialize( context );
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The StatusEffects this <see cref="Gem"/> provides to the one that sockets it, if any.
        /// </summary>
        private readonly PermanentAura effects = new PermanentAura();

        /// <summary>
        /// The storage field for the <see cref="RequiredLevel"/> property.
        /// </summary>
        private int requiredLevel;

        #endregion
    }
}
