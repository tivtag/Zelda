// <copyright file="CraftingBottle.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Crafting.CraftingBottle class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Crafting
{
    using System.Collections.Generic;
    using System.Linq;
    using Zelda.Items;
    using Zelda.Items.Affixes;
    using Atom.Math;

    /// <summary>
    /// The Crafting Bottle resamples the magic cube of Diablo 2.
    /// This class can't be inherited.
    /// </summary>
    /// <remarks>
    /// The player may place any item in the cube and 
    /// then try to transform them into a different item.
    /// <see cref="Recipe"/>s descripe what items are needed to create another item.
    /// </remarks>
    public sealed class CraftingBottle : Zelda.Items.Inventory
    {
        /// <summary>
        /// The default size of the grid (in cell space) of the <see cref="CraftingBottle"/>.
        /// </summary>
        public const int BottleGridWidth = 6,
                         BottleGridHeight = 6;

        /// <summary>
        /// Gets the total number of rubies that all items in this CraftingBottle are worth.
        /// </summary>
        public long TotalRubiesWorth
        {
            get
            {
                return this.Items.Sum( item => (long)(item.ItemInstance.Count * item.Item.RubiesWorth) );
            }
        }

        /// <summary>
        /// Initializes a new instance of the CraftingBottle class.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that owns the new CraftingBottle.
        /// </param>
        internal CraftingBottle( Zelda.Entities.PlayerEntity player )
            : base( player, BottleGridWidth, BottleGridHeight )
        {
        }

        #region - Transform -

        /// <summary>
        /// To be a transformation to be sucessfull only items 
        /// needed for a particular recipe are allowed to be in the cube.
        /// </summary>
        /// <param name="recipeDatabase">
        /// The <see cref="RecipeDatabase"/> object.
        /// </param>
        /// <param name="itemManager">
        /// The <see cref="ItemManager"/> object.
        /// </param>
        public void Transform( RecipeDatabase recipeDatabase, ItemManager itemManager )
        {
            var items = this.Items;
            if( items.Count == 0 )
                return;

            Recipe recipe = FindWorkingRecipe( recipeDatabase );

            if( recipe != null )
            {
                this.TryTransformRecipe( recipe, itemManager );
            }
        }

        /// <summary>
        /// Finds the first Recipe that works with the current content of the bottle.
        /// </summary>
        /// <param name="recipeDatabase">
        /// The <see cref="RecipeDatabase"/> object.
        /// </param>
        /// <returns>
        /// The Recipe that would work; -or- otherwise null.
        /// </returns>
        public Recipe FindWorkingRecipe( RecipeDatabase recipeDatabase )
        {
            var items = this.Items;
            if( items.Count == 0 )
            {
                return null;
            }

            IEnumerable<Recipe> recipes = this.GetRecipes( recipeDatabase );

            foreach( Recipe recipe in recipes )
            {
                if( this.IsRecipeWorking( recipe ) )
                {
                    return recipe;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all <see cref="Recipe"/>s the items in this CraftingBottle
        /// relate to; sorted by importance.
        /// </summary>
        /// <param name="recipeDatabase">
        /// The <see cref="RecipeDatabase"/> object.
        /// </param>
        /// <returns>
        /// A new list containing the Recipes relating to the content of this CraftingBottle.
        /// </returns>
        private IEnumerable<Recipe> GetRecipes( RecipeDatabase recipeDatabase )
        {
            var recipes = this.GetRecipesUnsorted( recipeDatabase );

            recipes.Sort(
                ( x, y ) => {
                    int countX = x.RequiredCount;
                    int countY = y.RequiredCount;

                    return countY.CompareTo( countX );
                }
            );

            return recipes;
        }

        /// <summary>
        /// Gets all <see cref="Recipe"/>s the items in this CraftingBottle
        /// relate to.
        /// </summary>
        /// <param name="recipeDatabase">
        /// The <see cref="RecipeDatabase"/> object.
        /// </param>
        /// <returns>
        /// A new list containing the Recipes relating to the content of this CraftingBottle.
        /// </returns>
        private List<Recipe> GetRecipesUnsorted( RecipeDatabase recipeDatabase )
        {
            List<Recipe> recipes = new List<Recipe>();

            foreach( var item in this.Items )
            {
                string[] recipeNames = item.Item.Recipes;
                if( recipeNames == null )
                    continue;

                for( int i = 0; i < recipeNames.Length; ++i )
                {
                    Recipe recipe = recipeDatabase.Get( recipeNames[i] );
                    if( recipe == null )
                        continue;

                    if( !recipes.Contains( recipe ) )
                    {
                        recipes.Add( recipe );
                    }
                }
            }

            foreach( Recipe recipe in recipeDatabase.SpecialRecipes )
            {
                if( recipe.Handler.HasRequired( this ) )
                {
                    recipes.Add( recipe );
                }
            }

            return recipes;
        }

        /// <summary>
        /// Tries to transform the items in this CraftingBottle
        /// by using the specified <see cref="Recipe"/>.
        /// </summary>
        /// <param name="recipe">
        /// The related recipe.
        /// </param>
        /// <param name="itemManager">
        /// The ItemManager object that provides a mechanism to load and create new Items.
        /// </param>
        /// <returns>
        /// true if transformation was successful;
        /// otherwise false.
        /// </returns>
        private bool TryTransformRecipe( Recipe recipe, ItemManager itemManager )
        {
            if( !IsRecipeWorking( recipe ) )
            {
                return false;
            }

            // Todo: Maybe add a "Yes/No" check here.       
            var usedItems = this.RemoveRequired( recipe );
            this.CreateAndSpawnResult( recipe, usedItems, itemManager );
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the given Recipe can currently be used.
        /// </summary>
        /// <param name="recipe">
        /// The Recipe to check.
        /// </param>
        /// <returns>
        /// true if the given Recipe is working; -or- otherwise false.
        /// </returns>
        private bool IsRecipeWorking( Recipe recipe )
        {
            if( !recipe.IsAvailableTo( this.Owner ) )
                return false;

            if( !this.HasRequired( recipe ) )
                return false;

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether this CraftingBottle contains
        /// all items needed for the specified <see cref="Recipe"/>.
        /// </summary>
        /// <param name="recipe">
        /// The related recipe.
        /// </param>
        /// <returns>
        /// true if this CraftingBottle contains everything 
        /// needed for the specified <see cref="Recipe"/>;
        /// otherwise false.
        /// </returns>
        private bool HasRequired( Recipe recipe )
        {
            return recipe.Handler.HasRequired( this );
        }

        /// <summary>
        /// Removes the items needed for the specified <see cref="Recipe"/>.
        /// </summary>
        /// <param name="recipe">
        /// The related recipe.
        /// </param>
        /// <returns>
        /// The ItemInstances that have been modified.
        /// </returns>
        private IEnumerable<ItemInstance> RemoveRequired( Recipe recipe )
        {
            return recipe.Handler.RemoveRequired( this );
        }

        /// <summary>
        /// Spawns the items of a successfully used <see cref="Recipe"/>.
        /// </summary>
        /// <param name="recipe">
        /// The related recipe.
        /// </param>
        /// <param name="usedItems">
        /// The ItemInstances that have been modified when removing the needed items.
        /// </param>
        /// <param name="itemManager">
        /// The ItemManager that is required to load any Items related to the Recipe.
        /// </param>
        private void CreateAndSpawnResult( Recipe recipe, IEnumerable<ItemInstance> usedItems, ItemManager itemManager )
        {
            recipe.FullyLoad( itemManager );

            ItemInstance result = recipe.Handler.CreateResult( usedItems, this.Owner );
            this.SpawnResult( result );
        }

        /// <summary>
        /// Spawns the given ItemInstance, by inserting it into the Bottle.
        /// </summary>
        /// <remarks>
        /// If the bottle is full other places to insert the item are tried.
        /// </remarks>
        /// <param name="itemInstance">
        /// The instance of the Item to spawn.
        /// </param>
        private void SpawnResult( ItemInstance itemInstance )
        {
            if( itemInstance != null )
            {
                this.FailSafeInsert( itemInstance );
            }
        }

        #endregion

        #region - TransformItemsIntoRubies -

        /// <summary>
        /// Transforms all items currently in  this CraftingBottle into rubies.
        /// </summary>
        /// <returns>
        /// true if any item was transformed into rubies;
        /// otherwise false.
        /// </returns>
        public bool TransformItemsIntoRubies()
        {
            long totalRubies = this.TotalRubiesWorth;

            this.Owner.Statable.Rubies += totalRubies;
            this.Clear();

            return totalRubies > 0;
        }

        #endregion

        /// <summary>
        /// Handles the case of the user left-clicking on an item in the CraftingBottle
        /// while the Shift key is down.
        /// </summary>
        /// <remarks>
        /// The default behaviour is to move the item into the inventory.
        /// </remarks>
        /// <param name="item">The related item.</param>
        /// <param name="cellX">The original position of the <paramref name="item"/> (in cell-space).</param>
        protected override void SwapItemsOnShiftLeftClick( ItemInstance item, Point2 cell )
        {
            if( !this.Owner.Inventory.Insert( item ) )
            {
                this.InsertAt( item, cell );
            }
        }
    }
}
