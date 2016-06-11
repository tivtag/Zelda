// <copyright file="RecipeVerifier.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Tools.RecipeProcessor.RecipeVerifier class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Tools.RecipeProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Zelda.Crafting;
    using Zelda.Items;

    /// <summary>
    /// Implements a mechanism that verifies the consistency of <see cref="RecipeItem"/>s.
    /// </summary>
    internal sealed class RecipeVerifier
    {
        /// <summary>
        /// Initializes a new instance of the RecipeVerifier class.
        /// </summary>
        /// <param name="itemManager">
        /// Implements a mechanism that allows loading of Item definition files
        /// into memory.
        /// </param>
        public RecipeVerifier( ItemManager itemManager )
        {
            this.itemManager = itemManager;
        }

        /// <summary>
        /// Verifies the consistency of the given Recipe.
        /// </summary>
        /// <param name="recipe">
        /// The Recipe to verify.
        /// </param>
        public void Verify( Recipe recipe )
        {
            this.VerifyRequiredItems( recipe );
            this.VerifyResultingItems( recipe );
        }
                
        /// <summary>
        /// Verifies that the required items of the specified Recipe are consistent.
        /// </summary>
        /// <param name="recipe">
        /// The Recipe to verify.
        /// </param>
        private void VerifyRequiredItems( Recipe recipe )
        {
            foreach( var recipeItem in recipe.Required )
            {
                this.VerifyRequiredItems( recipeItem, recipe );
            }
        }

        /// <summary>
        /// Verifies that the specified required item of the specified Recipe are consistent.
        /// </summary>
        /// <param name="recipeItem">
        /// The NeededRecipeItem to verify.
        /// </param>
        /// <param name="recipe">
        /// The Recipe to verify.
        /// </param>
        private void VerifyRequiredItems( RequiredRecipeItem recipeItem, Recipe recipe )
        {
            Item item = recipeItem.Item;

            if( recipe.Handler is NormalRecipeHandler )
            {
                if (item.Recipes == null || !item.Recipes.Contains(recipe.Name))
                {
                    throw new InvalidDataException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            " The Item '{0}' doesn't contain the Recipe '{1}' in its recipe list. Add it!",
                            recipeItem.Name,
                            recipe.Name
                        )
                    );
                }
            }

            if( recipeItem.AllowsAffixed )
            {
                if( item.AllowedAffixes == Zelda.Items.Affixes.AffixType.None )
                {
                    throw new InvalidDataException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "The Item '{0}' doesn't allow any affixes. But the recipe '{1}' says that affixed items are allowed.",
                            recipeItem.Name,
                            recipe.Name
                        )
                    );
                }
            }
        }

        /// <summary>
        /// Verifies that the item level of the specified Item has been set.
        /// </summary>
        /// <param name="item">
        /// The Item to verify.
        /// </param>
        private void VerifyItemLevel( Item item )
        {
            if( item.Level <= 0 )
            {
                throw new InvalidDataException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "  Error: The Item '{0}' has an invalid item level of '{1}'.",
                        item.Name,
                        item.Level
                    )
                );
            }
        }

        /// <summary>
        /// Verifies that the resulting items of the specified Recipe are consistent.
        /// </summary>
        /// <param name="recipe">
        /// The Recipe to verify.
        /// </param>
        private void VerifyResultingItems( Recipe recipe )
        {
            this.VerifyItemLevel( recipe.Result.Item );
            this.VerifyAffixTransportation( recipe.Result, recipe );
        }

        /// <summary>
        /// Verifies that the AppliesAffixesOf attribute has been correctly set on the specified <see cref="ResultRecipeItem"/>.
        /// </summary>
        /// <param name="recipeItem">
        /// The ResultRecipeItem to verify.
        /// </param>
        /// <param name="recipe">
        /// The Recipe to verify.
        /// </param>
        private void VerifyAffixTransportation( ResultRecipeItem recipeItem, Recipe recipe )
        {
            if( recipeItem.AppliesAffixesOf != null )
            {
                bool hasInNeededItems = false;

                foreach( var neededRecipeItem in recipe.Required )
                {
                    if( neededRecipeItem.Name.Equals( recipeItem.AppliesAffixesOf, StringComparison.Ordinal ) )
                    {
                        hasInNeededItems = true;
                        break;
                    }
                }

                if( !hasInNeededItems )
                {
                    throw new InvalidDataException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                             "  Error: The item '{0}' whose affixes should be copied over to the resulting item '{1}' doesn't exist in the list of needed items.",
                             recipeItem.AppliesAffixesOf,
                             recipeItem.Name
                        )
                    );
                }
            }
        }

        /// <summary>
        /// Implements a mechanism that allows loading of Item definition files
        /// into memory.
        /// </summary>
        private readonly ItemManager itemManager;
    }
}
