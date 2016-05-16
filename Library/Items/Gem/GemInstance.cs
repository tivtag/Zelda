// <copyright file="GemInstance.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.GemInstance class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items
{   
    using Zelda.Status;
    using Zelda.Saving;

    /// <summary>
    /// Represents an <see cref="ItemInstance"/> that stores <see cref="Gem"/>s.
    /// </summary>
    public sealed class GemInstance : ItemInstance
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="Gem"/> that is encapsulated by this <see cref="GemInstance"/>.
        /// </summary>
        public Gem Gem
        {
            get 
            { 
                return (Gem)this.Item;
            }
        }

        /// <summary>
        /// Gets the <see cref="Gem"/> template that is encapsulated by this <see cref="GemInstance"/>.
        /// </summary>
        public Gem BaseGem
        {
            get
            {
                return (Gem)this.BaseItem;
            }
        }

        /// <summary>
        /// Gets the 'color' of this Gem.
        /// </summary>
        public ElementalSchool Color
        {
            get 
            { 
                return this.Gem.GemColor; 
            }
        }

        /// <summary>
        /// Gets the list of StatusEffects this GemInstance applies when socketed into an Item.
        /// </summary>
        public System.Collections.Generic.IEnumerable<StatusEffect> Effects
        {
            get
            {
                return this.Gem.EffectAura.Effects;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="GemInstance"/> class.
        /// </summary>
        /// <param name="gem">
        /// The underlying <see cref="Gem"/>.
        /// </param>
        /// <param name="powerFactor">
        /// The factor by which the power of the new ItemInstance varies compared to this base Item.
        /// </param>
        internal GemInstance( Gem gem, float powerFactor )
            : base( gem, powerFactor )
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            context.Write( (int)ItemType.Gem );

            context.WriteDefaultHeader(); // New in Safe File Version 3
            context.Write( this.BaseItem.Name );
            context.Write( this.Count );
            context.Write( this.PowerFactor );
        }

        /// <summary>
        /// Deserializes the data required to create a <see cref="GemInstance"/>.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <returns>
        /// A new GemInstance object.
        /// </returns>
        internal static GemInstance ReadGem( Zelda.Saving.IZeldaDeserializationContext context )
        {
            int version;

            if( context.Version >= 3 )
            {
                context.ReadDefaultHeader( "GemInstance.ReadGem" );
                version = 1;
            }
            else
            {
                version = 0;
            }

            // The typename has been readen at this point.
            var itemManager = context.ServiceProvider.ItemManager;
            
            string itemName = context.ReadString();
            int itemCount = context.ReadInt32();

            float powerFactor;

            if( version >= 1 )
            {
                powerFactor = context.ReadSingle();
            }
            else
            {
                powerFactor = 1.0f;
            }

            Gem gem = (Gem)itemManager.Get( itemName );
            if( gem == null )
                return null;

            return new GemInstance( gem, powerFactor ) {
                Count = itemCount
            };
        }

        #endregion
    }
}