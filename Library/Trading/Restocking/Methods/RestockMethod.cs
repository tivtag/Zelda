// <copyright file="RestockMethod.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Trading.Restocking.RestockMethod class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Trading.Restocking
{
    using System;
    using System.ComponentModel;
    using Zelda.Saving;

    /// <summary>
    /// Defines a base implementation of the IRestockMethod interface.
    /// </summary>
    public abstract class RestockMethod : IRestockMethod
    {
        /// <summary>
        /// Gets or sets the <see cref="IRestocker"/> this RestockMethod uses.
        /// </summary>
        [Editor( typeof( Zelda.Trading.Restocking.Design.RestockerEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public IRestocker Restocker
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="IRestockRequirement"/> that must be fulfilled for this RestockMethod to
        /// restock the MerchantItem.
        /// </summary>
        [Editor( typeof( Zelda.Trading.Restocking.Design.RestockRequirementEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public IRestockRequirement RestockRequirement
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="MerchantItem"/> this RestockMethod has been
        /// hooked up with.
        /// </summary>
        protected MerchantItem MerchantItem
        {
            get
            {
                return this.merchantItem;
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
            if( this.merchantItem != null )
                throw new InvalidOperationException( "This IRestockMethod is already hooked up." );

            this.merchantItem = merchantItem;
            this.OnHooked( merchantItem );
        }

        /// <summary>
        /// Unhooks this IRestockMode.
        /// </summary>
        public void Unhook()
        {
            this.VerifyHookedUp();

            this.OnUnhooked( this.merchantItem );
            this.merchantItem = null;
        }

        /// <summary>
        /// Called when this RestockMethod has been hooked up with the given MerchantItem.
        /// </summary>
        /// <param name="merchantItem">
        /// The related MerchantItem.
        /// </param>
        protected abstract void OnHooked( MerchantItem merchantItem );
        
        /// <summary>
        /// Called when this RestockMethod has been unhooked from the given MerchantItem.
        /// </summary>
        /// <param name="merchantItem">
        /// The related MerchantItem.
        /// </param>
        protected abstract void OnUnhooked( MerchantItem merchantItem );

        /// <summary>
        /// Restocks the MerchantItem this RestockMethod has been hooked up with;
        /// using the set <see cref="Restocker"/>.
        /// </summary>
        protected void Restock()
        {
            this.VerifyHookedUp();

            if( this.Restocker != null )
            {
                if( this.RestockRequirement != null && !this.RestockRequirement.IsFulfilled( this.merchantItem ) )
                {
                    return;
                }

                this.Restocker.Restock( this.merchantItem ); 
            }
        }
        
        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 2;
            context.Write( CurrentVersion );

            context.WriteObject( this.Restocker );
            context.WriteObject( this.RestockRequirement );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 2;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, typeof( RestockMethod ) );

            this.Restocker = context.ReadObject<IRestocker>();

            if( version >= 2 )
            {
                this.RestockRequirement = context.ReadObject<IRestockRequirement>();
            }
        }

        /// <summary>
        /// Verifies that this RestockMethod is currently hooked up.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        private void VerifyHookedUp()
        {
            if( this.merchantItem == null )
                throw new InvalidOperationException( "This IRestockMethod is currently not hooked up." );
        }

        /// <summary>
        /// Identifies the MerchantItem this RestockMethod is currently hooked on.
        /// </summary>
        private MerchantItem merchantItem;
    }
}
