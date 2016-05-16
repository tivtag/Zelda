// <copyright file="ItemSocketProperties.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.ItemSocketProperties class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items
{
    using System;
    using System.Collections.Generic;
    using Zelda.Status;

    /// <summary>
    /// Encapsulates the sockets properties of an <see cref="Equipment"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class ItemSocketProperties
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the list of actual <see cref="Socket"/>s of the Item.
        /// </summary>
        /// <value>Is null by default.</value>
        public Socket[] Sockets
        {
            get { return this.sockets; }
            set { this.sockets = value; }
        }
        
        /// <summary>
        /// Gets or sets the bonus provided by the Item if 
        /// all Sockets of the Equipment are socketed correctly. (gems-sockets colors match)
        /// </summary>
        /// <value>Is null by default.</value>
        public PermanentAura Bonus
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the number of <see cref="Socket"/>s the Item has.
        /// </summary>
        public int SocketCount
        {
            get
            {
                return this.Sockets != null ? this.Sockets.Length : 0;
            }
        }

        /// <summary>
        /// Gets the number of empty <see cref="Socket"/>s the Item has.
        /// </summary>
        public int EmptySocketCount
        {
            get
            {
                if( this.sockets == null )
                    return 0;

                int count = 0;

                for( int i = 0; i < this.sockets.Length; ++i )
                {
                    var socket = this.sockets[i];

                    if( socket.IsEmpty )
                    {
                        ++count;
                    }
                }

                return count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether any of the <see cref="Socket"/>s contains a <see cref="Gem"/>.
        /// </summary>
        public bool HasGems
        {
            get
            {
                if( this.sockets == null )
                    return false;

                for( int i = 0; i < this.sockets.Length; ++i )
                {
                    if( this.sockets[i].Gem != null )
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets an enumeration over the Gems the Item currently contains.
        /// </summary>
        public IEnumerable<GemInstance> Gems
        {
            get
            {
                if( this.sockets != null )
                {
                    for( int i = 0; i < this.sockets.Length; ++i )
                    {
                        var gem = this.sockets[i].Gem;

                        if( gem != null )
                            yield return gem;
                    }
                }
            }
        }

        /// <summary>
        /// Gets an enumeration over the set of StatusEffect that gets applied because of the Sockets of the Item.
        /// </summary>
        public IEnumerable<StatusEffect> MergedEffects
        {
            get
            {
                if( this.mergedEffect != null )
                {
                    for( int i = 0; i < this.mergedEffect.Effects.Count; ++i )
                    {
                        yield return this.mergedEffect.Effects[i];
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets the number of individual StatusEffects the MergedEffects consists of.
        /// </summary>
        public int MergedEffectCount
        {
            get
            {
                return this.mergedEffect != null ? this.mergedEffect.Effects.Count : 0;
            }
        }
                
        /// <summary>
        /// Gets an enumeration over the empty Sockets.
        /// </summary>
        public IEnumerable<Socket> EmptySockets
        {
            get
            {
                if( this.sockets != null )
                {
                    for( int i = 0; i < this.sockets.Length; ++i )
                    {
                        var socket = this.sockets[i];

                        if( socket.IsEmpty )
                            yield return socket;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the maximum number of sockets the Item is allowed to have.
        /// </summary>
        public int MaximumSocketCount
        {
            get
            {
                return this.equipment.InventoryWidth * this.equipment.InventoryHeight;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the ItemSocketProperties class.
        /// </summary>
        /// <param name="equipment">
        /// The Equipment whose sockets properties are descriped by the new Sockets instance.
        /// </param>
        internal ItemSocketProperties( Equipment equipment )
        {
            this.equipment = equipment;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Tries to insert the given GemInstance into the Socket at the given relative socket position.
        /// </summary>
        /// <param name="position">
        /// The relative socket position.
        /// </param>
        /// <param name="gemIntance">
        /// The GemInstance to insert.
        /// </param>
        /// <param name="statable">
        /// The Statable component of the PlayerEntity that wishes to insert a Gem into a Socket.
        /// </param>
        /// <returns>
        /// Returns true if the insert was succesful;
        /// otherwise false.
        /// </returns>
        public bool Insert( Atom.Math.Point2 position, GemInstance gemIntance, Statable statable )
        {
            var socket = this.GetSocketAt( position );
            if( socket == null )
                return false;

            if( !gemIntance.Gem.FulfillsRequirements( statable ) )
                return false;

            if( socket.Insert( gemIntance ) )
            {
                this.Refresh();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to add an additional Socket to the Item.
        /// </summary>
        /// <param name="socketColor">
        /// The color of the socket.
        /// </param>
        /// <returns>
        /// True if an additional Socket has been added; otherwise false.
        /// </returns>
        internal bool AddSocket( ElementalSchool socketColor )
        {
            if( this.CanHaveAdditionalSocket() )
            {
                var socket = new Socket( socketColor );

                if( this.sockets == null )
                {
                    this.sockets = new Socket[1];
                }
                else
                {
                    Array.Resize( ref this.sockets, this.sockets.Length + 1 );
                }

                this.sockets[this.sockets.Length - 1] = socket;
                this.RefreshBonus();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Item can have one additional Socket.
        /// </summary>
        /// <returns>
        /// Returns true if adding an additional Socket is allowed;
        /// otherwise false.
        /// </returns>
        internal bool CanHaveAdditionalSocket()
        {
            return this.SocketCount < this.MaximumSocketCount;
        }

        /// <summary>
        /// Applies the socket properties to the given ExtendedStatable.
        /// </summary>
        /// <param name="statable">
        /// The ExtendedStatable that is about to equip the item
        /// with these ItemSocketProperties.
        /// </param>
        internal void OnEquip( ExtendedStatable statable )
        {
            if( this.wearer != null )
                throw new System.InvalidOperationException();

            this.wearer = statable;

            if( this.mergedEffect != null && !this.mergedEffect.IsEnabled )
            {
                this.wearer.AuraList.Add( this.mergedEffect );
            }
        }

        /// <summary>
        /// Disables the socket properties from the given ExtendedStatable.
        /// </summary>
        internal void OnUnequip()
        {
            if( this.wearer == null )
                throw new System.InvalidOperationException();

            if( this.mergedEffect != null && this.mergedEffect.IsEnabled )
            {
                this.wearer.AuraList.Remove( this.mergedEffect );
            }

            this.wearer = null;
        }
        
        /// <summary>
        /// Get the <see cref="Socket"/> at the given zero-based index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the Socket to receive.
        /// </param>
        /// <returns>
        /// The Socket at the given index; or null.
        /// </returns>
        public Socket GetSocket( int index )
        {
            if( this.sockets == null )
                return null;

            if( index < 0 || index >= this.sockets.Length )
                return null;

            return this.sockets[index];
        }

        /// <summary>
        /// Gets the socket at the given two-dimensional position, relative to the Item's inventory size,
        /// </summary>
        /// <param name="position">
        /// The relative position.
        /// </param>
        /// <returns>
        /// The requested Socket; or null.
        /// </returns>
        private Socket GetSocketAt( Atom.Math.Point2 position )
        {
            int index = position.X + (position.Y * this.equipment.InventoryWidth);
            return this.GetSocket( index );
        }

        #region > Refresh <

        /// <summary>
        /// Refreshes the socket bonus and merged effect.
        /// </summary>
        private void Refresh()
        {
            this.RefreshBonus();
            this.RefreshMergedEffect();
        }

        /// <summary>
        /// Refreshes the socket <see cref="Bonus"/> given to the wearer of the item.
        /// </summary>
        private void RefreshBonus()
        {
            if( this.sockets == null || this.Bonus == null )
                return;

            this.isBonusFulfilled = true;

            for( int i = 0; i < this.sockets.Length; ++i )
            {
                if( !this.sockets[i].IsWellSocketted )
                {
                    this.isBonusFulfilled = false;
                    break;
                }
            }

            if( this.wearer != null )
            {
                if( this.isBonusFulfilled && !this.Bonus.IsEnabled )
                {
                    this.wearer.AuraList.Add( this.Bonus );
                }
                else if( !this.isBonusFulfilled && this.Bonus.IsEnabled )
                {
                    this.wearer.AuraList.Remove( this.Bonus );
                }
            }
        }

        /// <summary>
        /// Refreshes the mergedEffect Aura by completely rebuilding it.
        /// </summary>
        private void RefreshMergedEffect()
        {
            if( this.mergedEffect == null )
                this.mergedEffect = new PermanentAura();

            var auraList = this.mergedEffect.AuraList;
            if( auraList != null )
                auraList.Remove( this.mergedEffect );

            // Clear previous.
            this.mergedEffect.ClearEffects();

            // Merge Gems.
            foreach( var gem in this.Gems )
            {
                this.mergedEffect.MergeAdd( gem.Effects );
            }

            // Merge Bonus.
            if( this.isBonusFulfilled && this.Bonus != null )
            {
                this.mergedEffect.MergeAdd( this.Bonus.Effects );
            }

            this.mergedEffect.SortEffects();

            if( auraList != null )
                auraList.Add( this.mergedEffect );
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Creates a clone of this ItemSocketProperties instance.
        /// </summary>
        /// <returns>
        /// The cloned ItemSocketProperties instance.
        /// </returns>
        internal ItemSocketProperties Clone()
        {
            var clone = new ItemSocketProperties( this.equipment );
            this.SetupClone( clone );
            return clone;
        }

        /// <summary>
        /// Setups the given ItemSocketProperties instance to be a clone of this ItemSocketProperties instance.
        /// </summary>
        /// <param name="clone">
        /// The  ItemSocketProperties instance to setup as a clone of this ItemSocketProperties instance.
        /// </param>
        internal void SetupClone( ItemSocketProperties clone )
        {
            clone.Sockets = this.GetSocketClone();
            clone.Bonus = this.GetBonusClone();
        }

        /// <summary>
        /// Returns a clone of the Sockets of this ItemSocketProperties instance.
        /// </summary>
        /// <returns>
        /// The cloned array; can be null.
        /// </returns>
        private Socket[] GetSocketClone()
        {
            if( this.sockets != null )
            {
                var socketsClone = new Socket[this.sockets.Length];

                for( int i = 0; i < this.sockets.Length; ++i )
                {
                    socketsClone[i] = new Socket( this.sockets[i].Color );
                }

                return socketsClone;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a clone of the <see cref="Bonus"/> of this ItemSocketProperties instance.
        /// </summary>
        /// <returns>
        /// The cloned PermanentAura; can be null.
        /// </returns>
        private PermanentAura GetBonusClone()
        {
            return this.Bonus != null ? (PermanentAura)this.Bonus.Clone() : null;
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
        internal void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.Sockets.Length );
            for( int i = 0; i < this.Sockets.Length; ++i )
            {
                Socket socket = this.Sockets[i];
                context.Write( (int)socket.Color );
            }

            if( this.Bonus != null )
            {
                context.Write( true );
                this.Bonus.Serialize( context );
            }
            else
            {
                context.Write( false );
            }
        }

        /// <summary>
        /// Serializes the state of the sockets.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        internal void SerializeState( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.sockets.Length );

            foreach( Socket socket in this.sockets )
            {
                if( socket.Gem != null )
                {
                    context.Write( true );
                    socket.Gem.Serialize( context );
                }
                else
                {
                    context.Write( false );
                }
            }
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        internal void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            int socketCount = context.ReadInt32();
            this.Sockets = new Socket[socketCount];

            for( int i = 0; i < socketCount; ++i )
            {
                this.Sockets[i] = new Socket( (ElementalSchool)context.ReadInt32() );
            }

            bool hasSocketBonus = context.ReadBoolean();

            if( hasSocketBonus )
            {
                this.Bonus = new PermanentAura();
                this.Bonus.Deserialize( context );
            }
            else
            {
                this.Bonus = null;
            }
        }

        /// <summary>
        /// Deserializes the state of the individual sockets.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        internal void DeserializeState( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            int socketCount = context.ReadInt32();

            for( int i = 0; i < socketCount; ++i )
            {
                bool hasGem = context.ReadBoolean();

                if( hasGem )
                {
                    var itemType = (ItemType)context.ReadInt32();
                    var gemInstance = GemInstance.ReadGem( context );

                    this.sockets[i].Insert( gemInstance );
                }
            }

            if( this.HasGems )
            {
                this.Refresh();
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The Equipment whose sockets properties are descriped by this Sockets instance.
        /// </summary>
        private readonly Equipment equipment;

        /// <summary>
        /// The sockets of the Item.
        /// </summary>
        private Socket[] sockets;

        /// <summary>
        /// States whether the socket <see cref="Bonus"/> is fulfilled.
        /// </summary>
        private bool isBonusFulfilled;

        /// <summary>
        /// The ExtendedStatable that is currently wearing the item that owns this ItemSocketProperties instance.
        /// </summary>
        private ExtendedStatable wearer;

        /// <summary>
        /// The effect of the gems and the socket bonus merged into the PermanentAura.
        /// </summary>
        private PermanentAura mergedEffect;

        #endregion
    }
}