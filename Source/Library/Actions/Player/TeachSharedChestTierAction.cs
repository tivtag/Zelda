// <copyright file="TeachSharedChestTierAction.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Actions.Player.TeachSharedChestTierAction class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Actions.Player
{
    using System;
    using System.ComponentModel;
    using Atom.Storage;
    using Zelda.Items;

    /// <summary>
    /// Implements an action that teaches the player a new Shared Chest Tier,
    /// unless he has already learned it.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public sealed class TeachSharedChestTierAction : BasePlayerAction
    {
        /// <summary>
        /// Gets or sets the <see cref="SharedChestTier"/> that is required for this action
        /// to have effect and upgrade the current tier of the players shared chest.
        /// </summary>
        public SharedChestTier UpgradeFrom
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets a value indicating whether this TeachSharedChestTierAction can be executed.
        /// </summary>
        /// <returns>
        /// true if this TeachSharedChestTierAction can be executed;
        /// otherwise false.
        /// </returns>
        public override bool CanExecute()
        {
            var currentTier = this.Player.SharedChest.Tier;
            if( currentTier == SharedChestTier.Tier4 )
                return false;

            return currentTier == this.UpgradeFrom;
        }

        /// <summary>
        /// Executes this TeachSharedChestTierAction.
        /// </summary>
        public override void Execute()
        {
            if( this.CanExecute() )
            {
                var player = this.Player;
                player.SharedChest = SharedChest.UpgradeTier( player.SharedChest );
            }
        }

        /// <summary>
        /// Deferredly undoes this IAction.
        /// </summary>
        public override void Dexecute()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets a localized description text of this TeachSharedChestTierAction.
        /// </summary>
        /// <returns>
        /// The localized description of this TeachSharedChestTierAction.
        /// </returns>
        public override string GetDescription()
        {
            return string.Format(
                System.Globalization.CultureInfo.CurrentCulture,
                ActionResources.UpgradesSizeOfSharedChestFromTierAToTierB,
                ((int)this.UpgradeFrom + 1).ToString( System.Globalization.CultureInfo.CurrentCulture ),
                ((int)this.UpgradeFrom + 2).ToString( System.Globalization.CultureInfo.CurrentCulture )
            );
        }
        
        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( ISerializationContext context )
        {
            base.Serialize( context );

            context.Write( (int)this.UpgradeFrom );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( IDeserializationContext context )
        {
            base.Deserialize( context );

            this.UpgradeFrom = (SharedChestTier)context.ReadInt32();
        }
    }
}
