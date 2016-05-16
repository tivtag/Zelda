// <copyright file="DoesntHaveItemsRequirement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.DropRequirements.DoesntHaveItemsRequirement class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Items.DropRequirements
{
    using Zelda.Core.Requirements;

    /// <summary>
    /// Represents an <see cref="IRequirement"/> that requires the player
    /// to not have the specified <see cref="Item"/>s in his inventory or crafting bottle.
    /// </summary>
    public sealed class DoesntHaveItemsRequirement : IRequirement
    {
        /// <summary>
        /// Gets or sets the names that uniquely idenfifies the <see cref="Item"/>s
        /// that the player is not allowed to have.
        /// </summary>
        public string[] ItemNames
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the given PlayerEntity
        /// fulfills the requirements as specified by this IItemDropRequirement.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that wishes an Item to be dropped.
        /// </param>
        /// <returns>
        /// Returns true if the given PlayerEntity fulfills the specified requirement;
        /// or otherwise false.
        /// </returns>
        public bool IsFulfilledBy( Zelda.Entities.PlayerEntity player )
        {
            if( this.ItemNames == null )
                return true;

            foreach( var itemName in this.ItemNames )
            {
                if( string.IsNullOrEmpty( itemName ) )
                    continue;

                if( HasItem( itemName, player ) )
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets whether the given PlayerEntity has the Item with the given name.
        /// </summary>
        /// <param name="itemName">
        /// The name of the item to check.
        /// </param>
        /// <param name="player">
        /// The PlayerEntity that wishes an Item to be dropped.
        /// </param>
        /// <returns>
        /// Returns true if the given PlayerEntity has the item with the given name;
        /// or otherwise false.
        /// </returns>
        private static bool HasItem( string itemName, Zelda.Entities.PlayerEntity player )
        {
            return player.Inventory.Contains( itemName ) ||
                   player.CraftingBottle.Contains( itemName );
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            if( this.ItemNames != null )
            {
                context.Write( this.ItemNames.Length );

                foreach( var item in this.ItemNames )
                {
                    context.Write( item ?? string.Empty );
                }
            }
            else
            {
                context.Write( (int)0 );
            }
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            int itemsLength = context.ReadInt32();

            if( itemsLength > 0 )
            {
                this.ItemNames = new string[itemsLength];

                for( int i = 0; i < itemsLength; ++i )
                {
                    this.ItemNames[i] = context.ReadString();
                }
            }
            else
            {
                this.ItemNames = null;
            }
        }
    }
}
