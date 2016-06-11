// <copyright file="SetItem.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Sets.SetItem class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Sets
{
    using System.ComponentModel;

    /// <summary>
    /// Represents an item that is part of an <see cref="ISet"/>.
    /// </summary>
    public sealed class SetItem : ISetItem
    {
        /// <summary>
        /// Gets or sets the name of this SetItem.
        /// </summary>
        [Editor( typeof( Zelda.Items.Design.ItemNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string Name
        {
            get
            { 
                return this._itemName; 
            }

            set
            {
                this._itemName = value;
                this.LocalizedName = Item.GetLocalizedName( value );
            }
        }

        /// <summary>
        /// Gets the localized name of this SetItem.
        /// </summary>
        public string LocalizedName
        {
            get;
            private set;
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

            context.Write( this.Name ?? string.Empty );
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

            this.Name = context.ReadString();
        }

        /// <summary>
        /// The storage field of the <see cref="Name"/> property.
        /// </summary>
        private string _itemName;
    }
}
