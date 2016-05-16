// <copyright file="RecipeDatabaseBuilder.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Tools.RecipeProcessor.RecipeDatabaseBuilder class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Tools.RecipeProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using Zelda.Crafting;
    using Zelda.Items;

    /// <summary>
    /// Implements a mechanism that creates a <see cref="RecipeDatabase"/> by loading all of the
    /// <see cref="Recipe"/>s descriped in the XML files in Recipes folder.
    /// </summary>
    internal sealed class RecipeDatabaseBuilder
    {
        /// <summary>
        /// Initializes a new instance of the RecipeDatabaseBuilder class.
        /// </summary>
        /// <param name="itemManager">
        /// Implements a mechanism that allows loading of Item definition files
        /// into memory.
        /// </param>
        public RecipeDatabaseBuilder( ItemManager itemManager )
        {
            this.loader = new RecipeLoader( itemManager );
            this.verifier = new RecipeVerifier( itemManager );
        }

        /// <summary>
        /// Builds the <see cref="RecipeDatabase"/> by loading the recipes stored
        /// in the XML files.
        /// </summary>
        public RecipeDatabase Build()
        {
            this.LoadRecipesFromXml(
                new string[] {
                    "Armors.xml",
                    "Bags.xml",
                    "Belts.xml",
                    "Boots.xml",
                    "Misc.xml",
                    "Potions.xml",
                    "Relics.xml",
                    "Shields.xml",
                    "Gems.xml",
                    "Trinkets.xml",
                    "Helmets.xml",
                    "Weapons.xml"
                }
            );

            Console.WriteLine( "========================" );
            Console.WriteLine( "Reading of " + this.database.Count + " recipes done.\n" );

            return this.database;
        }

        /// <summary>
        /// Loads the recipes the XmlDocuments with the given documentNames contains into the RecipeDatabase.
        /// </summary>
        /// <param name="documentNames">
        /// The names of the xml recipe files.
        /// </param>
        private void LoadRecipesFromXml( IEnumerable<string> documents )
        {
            foreach( var document in documents )
            {
                this.LoadRecipesFromXml( document );
            }
        }

        /// <summary>
        /// Loads the recipes the XmlDocument with the given documentName contains into the RecipeDatabase.
        /// </summary>
        /// <param name="documentName">
        /// The name of the xml recipe file.
        /// </param>
        private void LoadRecipesFromXml( string documentName )
        {
            XmlDocument document = LoadXml( documentName );
            this.LoadRecipesFromXml( document );
        }
        
        /// <summary>
        /// Loads and returns the recipe XmlDocument with the given documentName.
        /// </summary>
        /// <param name="documentName">
        /// The name of the xml recipe file.
        /// </param>
        /// <returns>
        /// The loaded XmlDocument.
        /// </returns>
        private static XmlDocument LoadXml( string documentName )
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = string.Format( "{0}.Recipes.{1}", assembly.GetName().Name, documentName );

            using( var stream = assembly.GetManifestResourceStream( resourceName ) )
            {
                var document = new XmlDocument();
                document.Load( stream );

                return document;
            }
        }

        /// <summary>
        /// Loads the recipes the specified XmlDocument contains into the RecipeDatabase.
        /// </summary>
        /// <param name="document">
        /// The XmlDocument that should be read.
        /// </param>
        private void LoadRecipesFromXml( XmlDocument document )
        {
            // Access the <Recipes> ... </Recipes> node.
            XmlNode recipesNode = document.DocumentElement;

            foreach( XmlNode recipeNode in recipesNode.ChildNodes )
            {
                try
                {
                    this.LoadRecipeFromXml( recipeNode );
                }
                catch( Exception exc )
                {
                    HandleError( exc );
                    continue;
                }
            }
        }

        /// <summary>
        /// Loads, verifies and adds the Recipe stored the specified XmlNode.
        /// </summary>
        /// <param name="recipeNode">
        /// The input node that contains the Recipe data.
        /// </param>
        private void LoadRecipeFromXml( XmlNode recipeNode )
        {
            Recipe recipe = this.loader.LoadFromXml( recipeNode );

            this.verifier.Verify( recipe );
            this.database.Add( recipe );
        }

        /// <summary>
        /// Handles an error that has occurred while loading and verifying a recipe.
        /// </summary>
        /// <param name="exc">
        /// The exception that has occurred.
        /// </param>
        private static void HandleError( Exception exc )
        {
            Console.WriteLine();
            Console.WriteLine( exc.Message );
            Console.WriteLine();

            if( exc.InnerException != null )
            {
                Console.WriteLine( exc.InnerException.Message );
            }

            Console.WriteLine();
            Console.ReadKey();
        }

        /// <summary>
        /// Implements a mechanism that transforms the data stored in an XML format
        /// into an in-memory <see cref="Recipe"/> object.
        /// </summary>
        private readonly RecipeLoader loader;
        
        /// <summary>
        /// Implements a mechanism that verifies the consistency of <see cref="RecipeItem"/>s.
        /// </summary>
        private readonly RecipeVerifier verifier;

        /// <summary>
        /// Represents the RecipeDatabase this RecipeDatabaseBuilder has build so far.
        /// </summary>
        private readonly RecipeDatabase database = new RecipeDatabase();
    }
}
