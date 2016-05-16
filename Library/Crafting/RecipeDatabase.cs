// <copyright file="RecipeDatabase.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Crafting.RecipeDatabase class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Crafting
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using Zelda.Saving;

    /// <summary>
    /// Manages all of <see cref="Recipe"/> that are part of the game.
    /// </summary>
    /// <remarks>
    /// The RecipeDatabase is originally stored in a human-editable XML format.
    /// This format gets converted into a binary format when compiling the game-data.
    /// The binary database is stored in the Content\\RecipeDatabase.zrdb file.
    /// </remarks>
    public sealed class RecipeDatabase : IEnumerable<Recipe>, ISaveable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the recipes that don't have the default <see cref="NormalRecipeHandler"/>.
        /// </summary>
        public IEnumerable<Recipe> SpecialRecipes
        {
            get
            {
                return this.specialRecipes;
            }
        }

        /// <summary>
        /// Gets the number of Recipes in this RecipeDatabase.
        /// </summary>
        public int Count
        {
            get
            {
                return this.recipes.Count; 
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets the recipes that are visible at the given level in the given category.
        /// </summary>
        /// <param name="level">
        /// The level of the player.
        /// </param>
        /// <param name="category">
        /// The RecipeCategory to filter for.
        /// </param>
        /// <returns>
        /// The visible recipes.
        /// </returns>
        public IEnumerable<Recipe> GetVisibleRecipes( int level, RecipeCategory category )
        {
            var recipes =
                from recipe in this.GetRecipesInCategory( category )
                where recipe.IsVisibleAt( level )
                orderby recipe.Level descending, recipe.Name
                select recipe;

            return recipes;
        }
        
        /// <summary>
        /// Gets a value indicating whether the player learns any new recipes at the given player level.
        /// </summary>
        /// <param name="playerLevel">
        /// The level of the player.
        /// </param>
        /// <returns>
        /// true if the player would learn at least one new visible recipe; -or- otherwise false.
        /// </returns>
        public bool LearnsNewRecipesAt( int playerLevel )
        {
            int oldPlayerLevel = playerLevel - 1;
            return this.GetVisibleRecipesCountAt( playerLevel ) != this.GetVisibleRecipesCountAt( oldPlayerLevel );
        }

        /// <summary>
        /// Gets the number of recipes that are visibile at the given level.
        /// </summary>
        /// <param name="level">
        /// The player level to check.
        /// </param>
        /// <returns>
        /// The number of visible recipes.
        /// </returns>
        private int GetVisibleRecipesCountAt( int level )
        {
            return this.recipes.Values.Count( recipe => recipe.IsVisibleAt( level ) );
        }

        /// <summary>
        /// Gets all of the Recipes that are in the given RecipeCategory.
        /// </summary>
        /// <param name="category">
        /// The RecipeCategory to filter for.
        /// </param>
        /// <returns>
        /// The Recipes associated with the specified RecipeCategory.
        /// </returns>
        private IEnumerable<Recipe> GetRecipesInCategory( RecipeCategory category )
        {
            if( category == RecipeCategory.All )
            {
                return this.recipes.Values;
            }

            return this.recipes.Values.Where( r => r.Category == category );
        }

        /// <summary>
        /// Tries to get the <see cref="Recipe"/> with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the recipe to get.
        /// </param>
        /// <returns>
        /// The <see cref="Recipe"/>; or null if not found.
        /// </returns>
        public Recipe Get( string name )
        {
            Recipe recipe = null;

            this.recipes.TryGetValue( name, out recipe );

            return recipe;
        }

        /// <summary>
        /// Adds the given Recipe to this RecipeDatabase.
        /// </summary>
        /// <param name="recipe">
        /// The Recipe to add.
        /// </param>
        public void Add( Recipe recipe )
        {
            Contract.Requires<ArgumentNullException>( recipe != null );

            this.recipes.Add( recipe.Name, recipe );
        }

        /// <summary>
        /// Gets an enumerator that iterates over the <see cref="Recipe"/>s
        /// in this RecipeDatabase.
        /// </summary>
        /// <returns>
        /// A new enumerator.
        /// </returns>
        public IEnumerator<Recipe> GetEnumerator()
        {
            return this.recipes.Values.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that iterates over the <see cref="Recipe"/>s
        /// in this RecipeDatabase.
        /// </summary>
        /// <returns>
        /// A new enumerator.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #region > Storage <

        /// <summary>
        /// Loads this RecipeDatabase by reading the Content\\RecipeDatabase.zrdb file.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides a mechanism for accessing game-related services.
        /// </param>
        public void Load( IZeldaServiceProvider serviceProvider )
        {
            this.recipes.Clear();

            using( var stream = File.OpenRead( "Content\\RecipeDatabase.zrdb" ) )
            {
                this.Deserialize( new Zelda.Saving.DeserializationContext(stream, serviceProvider) );
            }

            this.specialRecipes = this.recipes.Values
                .Where( recipe => !(recipe.Handler is NormalRecipeHandler) )
                .ToArray();
        }

        /// <summary>
        /// Serializes this RecipeDatabase.
        /// </summary>
        /// <param name="context">
        /// The ISerializationContext to which is written.
        /// </param>
        public void Serialize( IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();
            context.Write( this.recipes.Count );

            foreach( var recipe in this.recipes.Values )
            {
                Recipe.Serialize( recipe, context );
            }
        }

        /// <summary>
        /// Deserializes this RecipeDatabase.
        /// </summary>
        /// <param name="context">
        /// The IDeserializationContext from which is read.
        /// </param>
        public void Deserialize( IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );
            int recipeCount = context.ReadInt32();

            for( int i = 0; i < recipeCount; ++i )
            {
                var recipe = Recipe.Deserialize( context );
                this.recipes.Add( recipe.Name, recipe );
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The recipes that don't have the default <see cref="NormalRecipeHandler"/>.
        /// </summary>
        private Recipe[] specialRecipes;

        /// <summary>
        /// The Recipes this RecipeDatabase contains; sorted by their name.
        /// </summary>
        private readonly Dictionary<string, Recipe> recipes = new Dictionary<string, Recipe>();

        #endregion
    }
}