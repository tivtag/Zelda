// <copyright file="RecipeDocumentationWriter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Tools.RecipeProcessor.RecipeDocumentationWriter class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Tools.RecipeProcessor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Zelda.Crafting;
    using Zelda.Items;

    /// <summary>
    /// Writes a plain-text documentation about all of the recipes in a RecipeDatabase.
    /// </summary>
    internal sealed class RecipeDocumentationWriter
    {
        /// <summary>
        /// Initializes a new instance of the RecipeDocumentationWriter class.
        /// </summary>
        /// <param name="itemManager">
        /// Implements a mechanism that allows loading of Item definition files
        /// into memory.
        /// </param>
        public RecipeDocumentationWriter( ItemManager itemManager )
        {
            this.itemManager = itemManager;
        }

        /// <summary>
        /// Creates the documentation for the given RecipeDatabase.
        /// </summary>
        /// <param name="recipes">
        /// The recipes to process.
        /// </param>
        public void Write( RecipeDatabase database, string fileName = "..\\..\\Compiled\\Release\\Recipes.txt" )
        {
            Console.Write( "Writing Documentation to '{0}' ... ", fileName );
            IEnumerable<Recipe> recipes = GetSortedRecipes( database );

            using( var stream = new FileStream( fileName, FileMode.Create, FileAccess.Write ) )
            {
                using( var writer = new StreamWriter( stream, Encoding.Unicode ) )
                {
                    Write( recipes, writer );
                }
            }

            Console.WriteLine( "done!" );
        }

        /// <summary>
        /// Writes the documentation about the specified recipes to the specified StreamWriter.
        /// </summary>
        /// <param name="recipes">
        /// The recipes to write down.
        /// </param>
        /// <param name="writer">
        /// The StreamWriter to write with.
        /// </param>
        private static void Write( IEnumerable<Recipe> recipes, StreamWriter writer )
        {
            writer.WriteLine( "*********************************" );
            writer.WriteLine( "The Legend of Zelda - Black Crown" );
            writer.WriteLine( "RECIPES RECIPES RECIPES" );
            writer.WriteLine();
            writer.WriteLine( "Use the Magic Crafting Bottle to mix your items!" );
            writer.WriteLine( "*********************************" );

            RecipeCategory lastCategory = RecipeCategory.Armor;
            WriteCategoryInfo( lastCategory, writer );

            foreach( var recipe in recipes )
            {
                var item      = recipe.Result.Item;
                var category = recipe.Category;

                if( category != lastCategory )
                {
                    WriteCategoryInfo( category, writer );
                    lastCategory = category;
                }

                WriteRecipe( recipe, writer );
            }
        }
        
        /// <summary>
        /// Writes the header for the given RecipeCategory.
        /// </summary>
        /// <param name="slot">
        /// The EquipmentSlot to write.
        /// </param>
        /// <param name="writer">
        /// The StreamWriter to use.
        /// </param>
        private static void WriteCategoryInfo(RecipeCategory slot, StreamWriter writer)
        {
            string categoryString = Zelda.LocalizedEnums.Get( slot );

            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine( "-----------------" );
            writer.WriteLine( categoryString );
            writer.WriteLine( "-----------------" );
        }
        
        /// <summary>
        /// Gets the Recipes of the specified RecipeDatabase in sorted order.
        /// </summary>
        /// <param name="database">
        /// The input database.
        /// </param>
        /// <returns>
        /// The sorted recipes of the input database.
        /// </returns>
        private IEnumerable<Recipe> GetSortedRecipes( RecipeDatabase database )
        {
            List<Recipe> recipes = database.ToList();
            SortRecipes( recipes );

            return recipes;
        }

        /// <summary>
        /// Sorts the given List of ActualRecipe.
        /// </summary>
        /// <param name="recipes">
        /// The recipes to process.
        /// </param>
        private static void SortRecipes( List<Recipe> recipes )
        {
            recipes.Sort( (x, y) => {
                if (x.Category == y.Category)
                {
                    var itemX = x.Result.Item;
                    var itemY = y.Result.Item;
                    return itemX.LocalizedName.CompareTo(itemY.LocalizedName);
                }
                else
                {
                    return x.Category.ToString().CompareTo(y.Category.ToString());
                }
            } );
        }

        /// <summary>
        /// Writes the documentation for the given <see cref="ActualRecipe"/>.
        /// </summary>
        /// <param name="recipe">
        /// The recipe to document.
        /// </param>
        /// <param name="writer">
        /// The StreamWriter to use.
        /// </param>
        private static void WriteRecipe( Recipe recipe, StreamWriter writer )
        {
            writer.WriteLine();
            
            int amount = recipe.Result.Amount;
            string name = recipe.Result.Item.LocalizedName;

            if( amount == 1 )
            {
                writer.WriteLine( "{0}", name );
            }
            else
            {
                writer.WriteLine( "{0}x {1}", amount, name );
            }

            if( !string.IsNullOrEmpty( recipe.Description ) )
            {
                writer.WriteLine( recipe.Description );
            }

            for( int i = 0; i < recipe.Required.Count; ++i )
            {
                var requiredItem = recipe.Required[i];
                writer.Write( "{0}x {1}", requiredItem.Amount, requiredItem.Item.LocalizedName );

                if( (i + 1) < recipe.Required.Count )
                    writer.Write( " + " );
            }

            writer.WriteLine();
        }

        /// <summary>
        /// Implements a mechanism that allows loading of Item definition files
        /// into memory.
        /// </summary>
        private readonly ItemManager itemManager;
    }
}
