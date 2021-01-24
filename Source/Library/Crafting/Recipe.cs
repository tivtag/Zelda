// <copyright file="Recipe.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Crafting.Recipe class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Crafting
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using Zelda.Saving;

    /// <summary>
    /// A Recipe descripes what items are required 
    /// to create another item using the magic <see cref="CraftingBottle"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class Recipe
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the name that uniquely identifies this <see cref="Recipe"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets the (optional and localized) description of this <see cref="Recipe"/>.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }
        }
        
        /// <summary>
        /// Gets the category this Recipe is sorted under.
        /// </summary>
        public RecipeCategory Category
        {
            get
            {
                return this.category;
            }
        }

        /// <summary>
        /// Gets the object that is responsible for controlling the usage of this Recipe.
        /// </summary>
        public IRecipeHandler Handler
        {
            get
            {
                return this.handler;
            }
        }

        /// <summary>
        /// Gets the level of this Recipe.
        /// </summary>
        public int Level
        {
            get
            {
                return this.level;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this Recipe is not shown
        /// in the Recipes UI.
        /// </summary>
        public bool IsHidden
        {
            get
            {
                return this.isHidden;
            }
        }

        /// <summary>
        /// Gets the number of individual parts this Recipe requires.
        /// </summary>
        public int RequiredCount
        {
            get
            {
                return this.requiredItems.Count;
            }
        }

        /// <summary>
        /// Gets the list of items which are needed 
        /// to craft the <see cref="Result"/> of this Recipe.
        /// </summary>
        public ReadOnlyCollection<RequiredRecipeItem> Required
        {
            get 
            {
                return this.requiredItems;
            }
        }
        
        /// <summary>
        /// Gets the ites which is crafted via this <see cref="Recipe"/>.
        /// </summary>
        public ResultRecipeItem Result
        {
            get 
            {
                return this.result;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the Recipe class.
        /// </summary>
        /// <param name="name">
        /// The name that uniquely identifies the new Recipe.
        /// </param>
        /// <param name="description">
        /// The (optional and localized) description of the new Recipe.
        /// </param>
        /// <param name="category">
        /// The category of the new Recipe.
        /// </param>
        /// <param name="level">
        /// The level of the new Recipe.
        /// </param>
        /// <param name="isHidden">
        /// States whether the new Recipe is hidden from the recipes UI.
        /// </param>
        /// <param name="requiredItems">
        /// The items that are needed to create the <paramref name="result"/> of the new Recipe.
        /// </param>
        /// <param name="result">
        /// The result of the new Recipe.
        /// </param>
        /// <param name="handler">
        /// The object responsible for applying/using the new Recipe.
        /// </param>
        public Recipe( 
            string name,
            string description,
            RecipeCategory category,
            int level, 
            bool isHidden,  
            IList<RequiredRecipeItem> requiredItems,
            ResultRecipeItem result,
            IRecipeHandler handler )
        {
            this.name = name;
            this.description = description;
            this.category = category;
            this.level = level;
            this.isHidden = isHidden;
            this.requiredItems = new ReadOnlyCollection<RequiredRecipeItem>( requiredItems.ToArray() );
            this.result = result;
            this.handler = handler;
            this.handler.Recipe = this;
        }

        #endregion

        #region [ Methods ]
        
        /// <summary>
        /// Gets a value indicating whether a player with the specified level is able to see this Recipe.
        /// </summary>
        /// <param name="level">
        /// The level of the player that wishes to use the Recipe.
        /// </param>
        /// <returns>
        /// true if the recipe is visible to a player at the specified level;
        /// otherwise false.
        /// </returns>
        public bool IsVisibleAt( int level )
        {
            return !this.isHidden && this.IsAvailableAt( level );
        }

        /// <summary>
        /// Gets a value indicating whether the specified PlayerEntity is able
        /// to use this Recipe.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that wishes to use the Recipe.
        /// </param>
        /// <returns>
        /// true if the recipe is available to the specified player;
        /// otherwise false.
        /// </returns>
        public bool IsAvailableTo( Entities.PlayerEntity player )
        {
            return this.IsAvailableAt( player.Statable.Level );
        }

        /// <summary>
        /// Gets a value indicating whether a player with the specified level is able to use this Recipe.
        /// </summary>
        /// <param name="level">
        /// The level of the player that wishes to use the Recipe.
        /// </param>
        /// <returns>
        /// true if the recipe is available to a player at the specified level;
        /// otherwise false.
        /// </returns>
        public bool IsAvailableAt( int level )
        {
            return this.level <= level;
        }

        /// <summary>
        /// Fully loads, if required, the item data this Recipe uses into memory.
        /// </summary>
        /// <param name="itemManager">
        /// Implements a mechanism that allows loading of Item definition files
        /// into memory.
        /// </param>
        public void FullyLoad( Items.ItemManager itemManager )
        {
            if( !this.isFullyLoaded )
            {
                try
                {
                    this.ActuallyFullyLoad( itemManager );
                }
                catch( Exception exc )
                {
                    throw new Exception(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            "An exception has occurred while trying to fully load the Recipe '{0}'.",
                            this.Name
                        ),
                        exc
                    );
                }
                
                this.isFullyLoaded = true;
            }
        }

        /// <summary>
        /// Fully loads the item data this Recipe uses into memory.
        /// </summary>
        /// <param name="itemManager">
        /// Implements a mechanism that allows loading of Item definition files
        /// into memory.
        /// </param>
        private void ActuallyFullyLoad( Items.ItemManager itemManager )
        {
            foreach( var recipeItem in this.requiredItems )
            {
                recipeItem.FullyLoad( itemManager );
            }

            this.result.FullyLoad( itemManager );
        }

        /// <summary>
        /// Serializes/Writes the data of the given Recipe into the given binary stream.
        /// </summary>
        /// <param name="recipe">
        /// The Recipe to serialize.
        /// </param>
        /// <param name="context">
        /// The IDeserializationContext to which is written.
        /// </param>
        internal static void Serialize( Recipe recipe, IZeldaSerializationContext context )
        {
            context.Write(recipe.name);
            context.Write(recipe.description);
            context.Write( (byte)recipe.category );
            context.Write( recipe.level );
            context.Write( recipe.isHidden );
            context.WriteObject( recipe.Handler );

            context.Write( recipe.requiredItems.Count );
            foreach( var item in recipe.requiredItems )
            {
                RequiredRecipeItem.Serialize( item, context );
            }

            ResultRecipeItem.Serialize( recipe.result, context );
        }

        /// <summary>
        /// Deserializes/Reads the data stored in the given binary stream
        /// to create a new Recipe.
        /// </summary>
        /// <param name="context">
        /// The IDeserializationContext from which is read.
        /// </param>
        /// <returns>
        /// The deserializes Recipe.
        /// </returns>
        internal static Recipe Deserialize( IZeldaDeserializationContext context )
        {
            string name   = context.ReadString();
            string description = context.ReadString();

            var category  = (RecipeCategory)context.ReadByte();
            int level     = context.ReadInt32();
            bool isHidden = context.ReadBoolean();
            var handler   = context.ReadObject<IRecipeHandler>();
            
            // Read items needed.
            int itemsNeededCount = context.ReadInt32();
            var itemsNeeded      = new List<RequiredRecipeItem>( itemsNeededCount );

            for( int i = 0; i < itemsNeededCount; ++i )
            {
                var item = RequiredRecipeItem.Deserialize( context );
                itemsNeeded.Add( item );
            }

            var result = ResultRecipeItem.Deserialize( context );
            return new Recipe( name, description, category, level, isHidden, itemsNeeded, result, handler );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The name that uniquely identifies this <see cref="Recipe"/>. This string is not localized.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The (optional and localized) description of this <see cref="Recipe"/>.
        /// </summary>
        private readonly string description;

        /// <summary>
        /// The category this Recipe is sorted under.
        /// </summary>
        private readonly RecipeCategory category;

        /// <summary>
        /// The level of this Recipe.
        /// </summary>
        private readonly int level;

        /// <summary>
        /// The items needed to craft the result.
        /// </summary>
        private readonly ReadOnlyCollection<RequiredRecipeItem> requiredItems;

        /// <summary>
        /// The resulting item.
        /// </summary>
        private readonly ResultRecipeItem result;

        /// <summary>
        /// States whether this Recipe has been fully loaded.
        /// </summary>
        private bool isFullyLoaded;

        /// <summary>
        /// States whether this Recipe is not shown in the Recipes UI.
        /// </summary>
        private readonly bool isHidden;

        /// <summary>
        /// The object responsible for applying/using this Recipe.
        /// </summary>
        private readonly IRecipeHandler handler;

        #endregion
    }
}
