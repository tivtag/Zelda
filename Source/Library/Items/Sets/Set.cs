// <copyright file="Set.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Sets.Set class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Sets
{
    using System;
    using System.Collections.Generic;
    using Atom;
    using Zelda.Saving;
    
    /// <summary>
    /// Represents a set of <see cref="ISetItem"/>s that when equipped
    /// together provide an <see cref="ISetBonus"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class Set : ISet
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name that uniquely identifies
        /// this Set.
        /// </summary>
        public string Name
        {
            get
            {
                return this._name;
            }

            set
            {
                if( value == null )
                    throw new ArgumentNullException( "value" );

                this._name = value;

                try
                {
                    this.LocalizedName = ItemResources.ResourceManager.GetString( "SN_" + value );
                    if( this.LocalizedName == null )
                        this.LocalizedName = value;
                }
                catch( InvalidOperationException )
                {
                    this.LocalizedName = value;
                }
            }
        }

        /// <summary>
        /// Gets the localized name of this <see cref="Item"/>.
        /// </summary>
        /// <remarks>
        /// The Localized Name is aquired by looking
        /// up the string "SN_X", where X is the <see cref="Name"/>,
        /// in the <see cref="ItemResources"/>.
        /// </remarks>
        public string LocalizedName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the list of <see cref="ISetItem"/> that are part of this ISet.
        /// </summary>
        public IList<ISetItem> Items
        {
            get 
            {
                return this.items;
            }
        }

        /// <summary>
        /// Gets the <see cref="ISetBonus"/> this ISet provides when all Items of this ISet
        /// are equipped.
        /// </summary>
        public ISetBonus Bonus
        {
            get
            {
                return this.bonus;
            }
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
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 2;
            context.Write( CurrentVersion );

            context.Write( this.Name ?? string.Empty );
            context.Write( this.Items.Count );            
            foreach( ISetItem item in this.Items )
            {
                context.WriteObject( item );
            }

            context.Write( bonus.GetType().GetTypeName() );
            bonus.Serialize( context );
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
            const int CurrentVersion = 2;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.Name = context.ReadString();
            int itemCount = context.ReadInt32();
            this.items.Capacity = itemCount;

            for( int i = 0; i < itemCount; ++i )
            {
                var setItem = context.ReadObject<ISetItem>();

                if( setItem != null )
                {
                    this.items.Add( setItem );
                }
            }
            
            string bonusTypeName = context.ReadString();
            bonus.Deserialize( context );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The storage field of the <see cref="Name"/> property.
        /// </summary>
        private string _name;

        /// <summary>
        /// The SetBonus provided by this Set when all ISetItems have been equipped.
        /// </summary>
        private readonly ISetBonus bonus = new StatusSetBonus();

        /// <summary>
        /// The list of <see cref="ISetItem"/>s this Set consists of.
        /// </summary>
        private readonly List<ISetItem> items = new List<ISetItem>();

        #endregion
    }
}
