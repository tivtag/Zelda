// <copyright file="SharedChestHasSpecificTierRestockRequirement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Trading.Restocking.SharedChestHasSpecificTierRestockRequirement class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Trading.Restocking
{
    using Atom;
    using Zelda.Items;
    using Zelda.Saving;

    /// <summary>
    /// Implements an <see cref="IRestockRequirement"/> that requires the <see cref="SharedChest"/>
    /// to have a specific <see cref="SharedChestTier"/>.
    /// </summary>
    public sealed class SharedChestHasSpecificTierRestockRequirement : IRestockRequirement, IZeldaSetupable
    {
        /// <summary>
        /// Gets or sets the <see cref="SharedChestTier"/> the <see cref="SharedChest"/> is expected to have
        /// for the MerchantItem to be restocked.
        /// </summary>
        public SharedChestTier ExpectedTier 
        {
            get;
            set;
        }

        /// <summary>
        /// Setups this SharedChestHasSpecificTierRestockRequirement.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.chestProvider = serviceProvider.TryGetObjectProvider<SharedChest>();
        }

        /// <summary>
        /// Gets a value indicating whether this IRestockRequirement has been fulfilled.
        /// </summary>
        /// <param name="item">
        /// The MerchantItem that is about to be restock.
        /// </param>
        /// <returns>
        /// true if the restocking process is allowed to continue;
        /// otherwise false.
        /// </returns>
        public bool IsFulfilled( MerchantItem item )
        {
            if( this.chestProvider == null )
            {
                return false;
            }

            SharedChest chest = this.chestProvider.TryResolve();
            return chest != null && chest.Tier == this.ExpectedTier;
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();
            context.Write( (int)this.ExpectedTier );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );
            this.ExpectedTier = (SharedChestTier)context.ReadInt32();
        }

        /// <summary>
        /// Provides access to the <see cref="SharedChest"/>.
        /// </summary>
        private IObjectProvider<SharedChest> chestProvider;
    }
}
