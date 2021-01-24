// <copyright file="RecipeItem.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Crafting.RecipeItem class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Crafting
{
    using Zelda.Items;

    /// <summary>
    /// Represents a single item used in a <see cref="Recipe"/>.
    /// </summary>
    public class RecipeItem : IRecipeItem
    {
        /// <summary>
        /// Gets the name that uniquely identifies the Item.
        /// </summary>
        public string Name
        {
            get 
            { 
                return this.name; 
            }
        }

        /// <summary>
        /// Gets a value that represents the number of Items
        /// required.
        /// </summary>
        public int Amount
        {
            get 
            { 
                return this.amount;
            }
        }

        /// <summary>
        /// Gets the actual Item this RecipeItem is based on.
        /// Is null until this RecipeItem is <see cref="FullyLoad"/>ed.
        /// </summary>
        public Item Item
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeItem"/> class.
        /// </summary>
        /// <param name="name">
        /// The name that uniquely identifies the Item.
        /// </param>
        /// <param name="amount">
        /// The number of items needed.
        /// </param>
        public RecipeItem( string name, int amount )
        {
            this.name   = name;
            this.amount = amount;
        }

        /// <summary>
        /// Fully loads the actual item data of this RecipeItem into memory.
        /// </summary>
        /// <param name="itemManager">
        /// Implements a mechanism that allows loading of Item definition files
        /// into memory.
        /// </param>
        public void FullyLoad( Items.ItemManager itemManager )
        {
            if( this.Item == null )
            {
                this.Item = itemManager.Get( this.Name );
            }
        }

        /// <summary>
        /// The name that uniquely identifies the Item.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The number of items needed.
        /// </summary>
        private readonly int amount;
    }
}
