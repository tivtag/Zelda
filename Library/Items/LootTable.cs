// <copyright file="LootTable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.LootTable class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items
{
    using System;
    
    /// <summary>
    /// The LootTable is internally a Hat of item names.
    /// This class can't be inherited.
    /// </summary>
    public sealed class LootTable : Atom.Collections.Hat<string>
    {
        #region [ Properties / Constants ]

        /// <summary>
        /// The Id of items that are affected by Magic Find.
        /// </summary>
        public const int ItemIdAffectedByMagicFind = 1;

        /// <summary>
        /// Gets or sets the Magic Find Modifier set on the <see cref="LootTable"/>.
        /// </summary>
        public float MagicFindModifier
        {
            get
            {
                return this.magicFindModifier;
            }

            set
            {
                if( this.magicFindModifier == value )
                    return;

                this.SetWeightModifier( ItemIdAffectedByMagicFind, value );
                this.magicFindModifier = value;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="LootTable"/> class.
        /// </summary>
        /// <param name="itemManager">The ItemManager object.</param>
        /// <param name="rand">A random number generator.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="itemManager"/> or <paramref name="rand"/> is null.
        /// </exception>
        internal LootTable( ItemManager itemManager, Atom.Math.RandMT rand )
            : base( rand )
        {
            if( itemManager == null )
                throw new ArgumentNullException( "itemManager" );

            this.itemManager = itemManager;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets a random <see cref="ItemInstance"/> that is in the LootList.
        /// </summary>
        /// <returns>The ItemInstance - can be null.</returns>
        public new Item Get()
        {
            string itemName = base.Get();
            if( itemName == null )
                return null;

            Item item = this.itemManager.Get( itemName );
            if( item == null )
                return null;

            return item;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The magic find value set for this <see cref="LootTable"/>.
        /// </summary>
        private float magicFindModifier = 1.0f;

        /// <summary>
        /// Identifies the <see cref="ItemManager"/> object.
        /// </summary>
        private readonly ItemManager itemManager;

        #endregion
    }
}
