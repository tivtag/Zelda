// <copyright file="ValidationHelper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the static Zelda.NpcCreator.ValidationHelper class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.NpcCreator
{
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// Contains methods that help in validating Npc data.
    /// </summary>
    internal static class ValidationHelper
    {
        /// <summary>
        /// Initializes static members of the ValidationHelper class.
        /// </summary>
        static ValidationHelper()
        {
            itemNames = Directory.GetFiles( "..\\..\\Compiled\\Release\\Content\\Items\\" );

            for( int i = 0; i < itemNames.Length; ++i )
            {
                itemNames[i] = Path.GetFileNameWithoutExtension( itemNames[i] );
            }
        }

        /// <summary>
        /// Validates a <see cref="Zelda.Items.LootTable"/>.
        /// </summary>
        /// <param name="loot">
        /// The Zelda.Items.LootTable to validate.
        /// </param>
        /// <returns>
        /// true if the loot is valid;
        /// otherwise fale.
        /// </returns>
        public static bool ValidateLoot( Zelda.Items.LootTable loot )
        {
            foreach( var item in loot )
            {
                string itemName = item.Data;

                // Verify that the item exists.
                if( !itemNames.Contains( itemName ) )
                {
                    string message = string.Format( 
                        CultureInfo.CurrentCulture,
                        Properties.Resources.Error_TheLootTableContainsInvalidItemX,
                        itemName
                    );

                    MessageBox.Show(
                        message,
                        Atom.ErrorStrings.Error,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Stores the names that uniquely identify all known Items.
        /// </summary>
        private static string[] itemNames;
    }
}
