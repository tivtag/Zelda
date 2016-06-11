// <copyright file="Program.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Tools.RecipeProcessor.Program class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Tools.RecipeProcessor
{
    using System;
    using System.IO;
    using Zelda.Crafting;
    using Zelda.Items;

    /// <summary>
    /// The RecipeProcessor program converts the RecipesDatabase.xml into
    /// a binary format, which can be loaden from the harddisc.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The starting point of the RecipeProcessor program.
        /// </summary>
        public static void Main()
        {
            (new Program()).Process();
        }
        
        /// <summary>
        /// Initializes a new instance of the Program class.
        /// </summary>
        private Program()
        {
            this.itemManager = new ItemManager( null ) {
                Path = "..\\..\\Compiled\\Release\\Content\\Items\\"
            };
        }

        /// <summary>
        /// Processes the RecipeDatabase from Xml into binary format.
        /// </summary>
        private void Process()
        {
            // Build the RecipeDatabase by loading the recipes
            // descriped in the XML files stored in the Recipes folder.
            // 
            // The Recipes are also verified for consistency.
            RecipeDatabase database = (new RecipeDatabaseBuilder( this.itemManager )).Build();
            
            // Save the database in binary format to be loaded in the actual game.
            SerializeToBinary( database );

            // Write a plain-text documentation for the player.
            (new RecipeDocumentationWriter( this.itemManager )).Write( database );

            Console.WriteLine( "\nProcessing complete." );
            Console.WriteLine( "========================\n" );
            Console.ReadKey();
        }

        /// <summary>
        /// Saves the given RecipeDatabase into binary format.
        /// </summary>
        /// <param name="database">
        /// The RecipeDatabase to process.
        /// </param>
        private static void SerializeToBinary( RecipeDatabase database )
        {
            using( var stream = File.Create( "Content\\RecipeDatabase.zrdb" ) )
            {
                database.Serialize( new Zelda.Saving.SerializationContext( stream, null ) );
            }
        }

        /// Implements a mechanism that allows loading of Item definition files
        /// into memory.
        private readonly ItemManager itemManager;
    }
}
