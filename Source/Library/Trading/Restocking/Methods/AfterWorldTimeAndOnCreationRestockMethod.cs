// <copyright file="AfterWorldTimeAndOnCreationRestockMethod.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Trading.Restocking.AfterWorldTimeAndOnCreationRestockMethod class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Trading.Restocking
{
    /// <summary>
    /// Defines an <see cref="IRestockMethod"/> that composes the <see cref="OnMerchantCreatedRestockMethod"/> and 
    /// the <see cref="AfterWorldTimeRestockMethod"/>.
    /// </summary>
    public sealed class AfterWorldTimeAndOnCreationRestockMethod : IRestockMethod
    {
        /// <summary>
        /// Gets the IRestockMethod that restocks the MerchantItem when the Merchant is created.
        /// </summary>
        public OnMerchantCreatedRestockMethod OnCreationRestockMethod
        {
            get
            {
                return this.onCreatedMethod;
            }
        }

        /// <summary>
        /// Gets the IRestockMethod that restocks the MerchantItem after a certain amount of time.
        /// </summary>
        public AfterWorldTimeRestockMethod AfterWorldTimeRestockMethod
        {
            get
            {
                return this.afterWorldTimeMethod;
            }
        }

        /// <summary>
        /// Hooks this IRestockMode up with the given MerchantItem.
        /// </summary>
        /// <param name="merchantItem">
        /// The IMerchant this IRestockMode should hook up with.
        /// </param>
        public void Hook( MerchantItem merchantItem )
        {
            this.onCreatedMethod.Hook( merchantItem );
            this.afterWorldTimeMethod.Hook( merchantItem );
        }

        /// <summary>
        /// Unhooks this IRestockMode.
        /// </summary>
        public void Unhook()
        {
            this.onCreatedMethod.Unhook();
            this.afterWorldTimeMethod.Unhook();
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

            this.onCreatedMethod.Serialize( context );
            this.afterWorldTimeMethod.Serialize( context );
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

            this.onCreatedMethod.Deserialize( context );
            this.afterWorldTimeMethod.Deserialize( context );
        }

        /// <summary>
        /// Identifies the IRestockMethod that restocks the MerchantItem when the Merchant is created.
        /// </summary>
        private readonly OnMerchantCreatedRestockMethod onCreatedMethod = new OnMerchantCreatedRestockMethod();
        
        /// <summary>
        /// Identifies the IRestockMethod that restocks the MerchantItem after a certain amount of time.
        /// </summary>
        private readonly AfterWorldTimeRestockMethod afterWorldTimeMethod = new AfterWorldTimeRestockMethod();
    }
}