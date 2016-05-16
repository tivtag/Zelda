// <copyright file="ItemUseEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.ItemUseEffect classs.
// </summary>
// <author>
//     Paul Ennemoser (Tick)

namespace Zelda.Items
{    
    using System.ComponentModel;
    using Zelda.Items.UseEffects;
    
    /// <summary>
    /// An <see cref="ItemUseEffect"/> is an effect that gets triggered
    /// when the <see cref="Item"/> that owns the StatusEffects gets used.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public abstract class ItemUseEffect : IZeldaSetupable, IItemUseEffect
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value indicating whether
        /// the underlying item gets destroyed once this <see cref="ItemUseEffect"/> has been used.
        /// </summary>
        public bool DestroyItemOnUse
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Cooldown"/> of this <see cref="ItemUseEffect"/>.
        /// </summary>
        /// <remarks>
        /// Cooldowns of ItemUseEffects are often -shared- cooldowns.
        /// </remarks>
        public Cooldown Cooldown
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value that represents how much "Item Points" this <see cref="ItemUseEffect"/> uses.
        /// </summary>
        public virtual double ItemBudgetUsed
        {
            get
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this ItemUseEffect is ready to be used
        /// based on its <see cref="Cooldown"/>.
        /// </summary>
        [Browsable(false )]
        public bool IsReady
        {
            get
            {
                if( this.Cooldown != null )
                {
                    return this.Cooldown.IsReady;
                }

                return true;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Setups this <see cref="ItemUseEffect"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public virtual void Setup( IZeldaServiceProvider serviceProvider )
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets a short localised description of this <see cref="ItemUseEffect"/>.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this ItemUseEffect.
        /// </param>
        /// <returns>
        /// The localized description string.
        /// </returns>
        public abstract string GetDescription( Zelda.Status.Statable statable );

        /// <summary>
        /// Uses this <see cref="ItemUseEffect"/>.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that wishes to use this ItemUseEffect.
        /// </param>
        /// <returns>
        /// true if this ItemUseEffect has been used;
        /// otherwise false.
        /// </returns>
        public abstract bool Use( Zelda.Entities.PlayerEntity user );

        /// <summary>
        /// Returns a value that indicates whether the given PlayerEntity
        /// can use this <see cref="ItemUseEffect"/> at this moment.
        /// </summary>
        /// <param name="user">The PlayerEntity that wishes to use this ItemUseEffect.</param>
        /// <returns>
        /// true if this ItemUseEffect can been used;
        /// otherwise false.
        /// </returns>
        public virtual bool IsFulfilledBy( Zelda.Entities.PlayerEntity user )
        {
            return this.IsReady;
        }

        /// <summary>
        /// Helper method that resets the <see cref="Cooldown"/> on this ItemUseEffect.
        /// </summary>
        protected void ResetCooldown()
        {
            if( this.Cooldown != null )
            {
                this.Cooldown.Reset();
            }
        }

        #region > Storage <

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            context.Write( Atom.ReflectionExtensions.GetTypeName( this.GetType() ) );

            // Write Cooldown Information:
            if( this.Cooldown != null )
            {
                context.Write( true );
                context.Write( this.Cooldown.IsShared );
                context.Write( this.Cooldown.Id );
                context.Write( this.Cooldown.TotalTime );
            }
            else
            {
                context.Write( false );
            }

            context.Write( this.DestroyItemOnUse );
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
            // The type Name has been readen at this point.
            bool hasCooldown = context.ReadBoolean();

            if( hasCooldown )
            {
                bool isShared = context.ReadBoolean();
                int id        = context.ReadInt32();
                float time    = context.ReadSingle();

                if( isShared )
                {
                    var sharedCooldownMap = GetSharedCooldowns( context );

                    if( sharedCooldownMap != null )
                    {
                        Cooldown cooldown;

                        if( sharedCooldownMap.TryGetValue( id, out cooldown ) )
                        {
                            this.Cooldown = cooldown;
                        }
                        else
                        {
                            this.Cooldown = new Cooldown( id, time, true );
                            sharedCooldownMap.Add( id, this.Cooldown );
                        }
                    }
                    else
                    {
                        this.Cooldown = new Cooldown( id, time, true );
                    }
                }
                else
                {
                    this.Cooldown = new Cooldown( time );
                }
            }

            this.DestroyItemOnUse = context.ReadBoolean();
        }

        /// <summary>
        /// Gets the <see cref="SharedCooldownMap"/> associated with the given IItemDeserializationContext.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <returns>
        /// The SharedCooldownMap to use.
        /// </returns>
        private static SharedCooldownMap GetSharedCooldowns( Zelda.Saving.IZeldaDeserializationContext context )
        {
            var serviceProvider = context.ServiceProvider;
            if( serviceProvider == null )
                return null;

            var itemManager = serviceProvider.ItemManager;
            if( itemManager == null )
                return null;

            var statable = itemManager.Statable;
            if( statable == null )
                return null;

            return statable.SharedCooldowns;
        }

        #endregion

        #endregion
    }
}