// <copyright file="RecipeLoader.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Tools.RecipeProcessor.RecipeLoader class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Tools.RecipeProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.IO;
    using System.Xml;
    using Zelda.Crafting;
    using Zelda.Items;
    using Zelda.Status;

    /// <summary>
    /// Implements a mechanism that transforms the data stored in an XML format
    /// into an in-memory <see cref="Recipe"/> object.
    /// </summary>
    internal sealed class RecipeLoader
    {
        /// <summary>
        /// Initializes a new instance of the RecipeLoader class.
        /// </summary>
        /// <param name="itemManager">
        /// Implements a mechanism that allows loading of Item definition files
        /// into memory.
        /// </param>
        public RecipeLoader( ItemManager itemManager )
        {
            this.itemManager = itemManager;
        }

        /// <summary>
        /// Attempts to load the <see cref="Recipe"/> data stored in the specified XmlNode.
        /// </summary>
        /// <param name="dataNode">
        /// The input node.
        /// </param>
        /// <returns>
        /// The output recipe.
        /// </returns>
        public Recipe LoadFromXml( XmlNode dataNode )
        {
            try
            {
                return this.Parse( dataNode );
            }
            catch( Exception exc )
            {
                string message = string.Format(
                    "Error in '{0}':\n{1}",
                    this.recipeName ?? "?",
                    exc.Message
                );
                
                throw new InvalidDataException( message, exc.InnerException );
            }
        }

        /// <summary>
        /// Attempts to transform the specified XmlNode into an in-memory <see cref="Recipe"/>.
        /// </summary>
        /// <param name="dataNode">
        /// The XmlNode to parse.
        /// </param>
        /// <returns>
        /// The Recipe that has been parsed.
        /// </returns>
        private Recipe Parse( XmlNode dataNode )
        {
            XmlNode requiredNode = dataNode.ChildNodes[0];
            if( requiredNode == null )
                throw new InvalidDataException( "The required node is missing." );

            XmlNode resultNode = dataNode.ChildNodes[1];
            if( resultNode == null )
                throw new InvalidDataException( "The result node is missing." );

            XmlNode handlerNode = dataNode.ChildNodes[2];

            this.ParseRecipeName( dataNode );
            this.ParseRecipeDescription( dataNode );
            this.ParseRecipeHidden( dataNode );
            this.ParseRequiredItems( requiredNode );
            this.ParseResultingItems( resultNode );
            this.ParseCategory( dataNode );
            this.ParseHandler( handlerNode );
            this.ParseLevel( dataNode );

            return this.CreateRecipe();
        }

        /// <summary>
        /// Parses the specified XmlNode; attempting to read a value indicating the level of the recipe.
        /// </summary>
        /// <param name="dataNode">
        /// The root input node.
        /// </param>
        private void ParseLevel( XmlNode dataNode )
        {
            var attribute = dataNode.Attributes["level"];

            if( attribute != null )
            {
                this.level = int.Parse( attribute.Value );
            }
            else
            {
                this.level = GetRecipeLevel( this.resultingItem.Item );
            }
        }

        /// <summary>
        /// Parses the specified XmlNode; attempting to read a value indicating whether the recipe is hidden.
        /// </summary>
        /// <param name="dataNode">
        /// The root input node.
        /// </param>
        private void ParseRecipeHidden( XmlNode dataNode )
        {
            var attribute = dataNode.Attributes["hidden"];

            if( attribute != null )
            {
                this.isHidden = attribute.Value == "true";
            }
            else
            {
                this.isHidden = false;
            }
        }

        /// <summary>
        /// Parses/Calculates the RecipeCategory of the recipe beeing parsed currently.
        /// </summary>
        /// <param name="dataNode">
        /// The root input node.
        /// </param>
        private void ParseCategory( XmlNode dataNode )
        {
            if( this.recipeName.Contains( "Potion" ) )
            {
                this.category = RecipeCategory.Food;
                return;
            }

            this.category = GetRecipeCategory( this.resultingItem.Item );
        }

        /// <summary>
        /// Gets the RecipeCategory the given Item is associated with.
        /// </summary>
        /// <param name="item">
        /// The input item.
        /// </param>
        /// <returns>
        /// The RecipeCategory the given Item would have.
        /// </returns>
        private static RecipeCategory GetRecipeCategory( Item item )
        {
            if( item is Weapon )
            {
                return RecipeCategory.Weapons;
            }

            if( item is Gem )
            {
                return RecipeCategory.Jewelry;
            }

            var equipment = item as Equipment;

            if( equipment != null )
            {
                return ConvertSlotToCategory( equipment.Slot );
            }

            return RecipeCategory.Miscellaneous;
        }

        /// <summary>
        /// Transform an <see cref="EquipmentSlot"/> into a <see cref="RecipeCategory"/>.
        /// </summary>
        /// <param name="equipmentSlot">
        /// The input EquipmentSlot.
        /// </param>
        /// <returns>
        /// The transformed output RecipeCategory.
        /// </returns>
        private static RecipeCategory ConvertSlotToCategory( EquipmentSlot equipmentSlot )
        {
            switch( equipmentSlot )
            {
                case EquipmentSlot.ShieldHand:
                    return RecipeCategory.Shields;

                case EquipmentSlot.Necklace:
                case EquipmentSlot.Trinket:
                case EquipmentSlot.Relic:
                case EquipmentSlot.Ring:
                    return RecipeCategory.Jewelry;

                case EquipmentSlot.Chest:
                case EquipmentSlot.Cloak:
                case EquipmentSlot.Gloves:
                case EquipmentSlot.Head:
                case EquipmentSlot.Boots:
                case EquipmentSlot.Belt:
                    return RecipeCategory.Armor;

                case EquipmentSlot.Staff:
                    return RecipeCategory.Weapons;

                default:
                    return RecipeCategory.Miscellaneous;
            }
        }

        /// <summary>
        /// Creates a new instance of the Recipe class by combining the parsed data.
        /// </summary>
        /// <returns>
        /// The newly created Recipe.
        /// </returns>
        private Recipe CreateRecipe()
        {
            var recipe = new Recipe(
                this.recipeName,
                this.description,
                this.category,
                this.level,
                this.isHidden,
                this.requiredItems,
                this.resultingItem,
                this.handler
            );

            recipe.FullyLoad( this.itemManager );
            return recipe;
        }

        /// <summary>
        /// Calculates the level of the recipe.
        /// </summary>
        /// <param name="resultingItem">
        /// The item the recipe creates.
        /// </param>
        /// <returns>
        /// The average item level.
        /// </returns>
        private static int GetRecipeLevel( Item resultingItem )
        {
            return resultingItem.Level - 3;
        }

        /// <summary>
        /// Parses the specified XmlNode; attempting to read the name of the recipe.
        /// </summary>
        /// <param name="dataNode">
        /// The root input node.
        /// </param>
        private void ParseRecipeName( XmlNode dataNode )
        {
            this.recipeName = dataNode.Attributes["name"].Value;

            Console.WriteLine( this.recipeName + " ..." );
        }

        /// <summary>
        /// Parses the specified XmlNode; attempting to read the description of the recipe.
        /// </summary>
        /// <param name="dataNode">
        /// The root input node.
        /// </param>
        private void ParseRecipeDescription( XmlNode dataNode )
        {
            var attribute = dataNode.Attributes["desc"];

            if( attribute != null )
            {
                this.description = attribute.Value;
            }
            else
            {
                this.description = string.Empty;
            }
        }

        /// <summary>
        /// Parses the specified XmlNode; attempting to read the items required by the recipe.
        /// </summary>
        /// <param name="requiredNode">
        /// The node that contains the items required by the recipe.
        /// </param>
        private void ParseRequiredItems( XmlNode requiredNode )
        {
            this.requiredItems.Clear();

            foreach( XmlNode itemNode in requiredNode.ChildNodes )
            {
                var nameAtri = itemNode.Attributes[0];
                var amountAtri = itemNode.Attributes[1];
                var allowsAffixesAtri = itemNode.Attributes["allowsAffixes"];

                string name = nameAtri.Value;
                int amount = int.Parse( amountAtri.Value, CultureInfo.InvariantCulture );
                bool allowsAffixes = allowsAffixesAtri != null ? bool.Parse( allowsAffixesAtri.Value ) : false;
                IRecipeItemMatcher matcher = null;

                XmlNode subNode = itemNode.FirstChild;
                if( subNode != null )
                {
                    matcher = ParseItemMatcher( subNode );
                }

                var item = new RequiredRecipeItem( name, amount, allowsAffixes, matcher );
                requiredItems.Add( item );
            }
        }

        private IRecipeItemMatcher ParseItemMatcher( XmlNode subNode )
        {
            if( subNode.Name == "GemMatch" )
            {
                ElementalSchool gemColor;
                string gemColorStr = subNode.Attributes["color"].Value;

                if( !Enum.TryParse<ElementalSchool>( gemColorStr, out gemColor ) )
                {
                    throw new InvalidDataException( "Unknown gem color '" + gemColorStr + "'." );
                }

                return new GemItemMatcher() { GemColor = gemColor };
            }

            throw new InvalidDataException( "Unknown matcher'" + subNode.Name );
        }

        /// <summary>
        /// Parses the specified XmlNode; attempting to read the items that are created by the recipe.
        /// </summary>
        /// <param name="resultNode">
        /// The node that contains the items that are created by the recipe.
        /// </param>
        private void ParseResultingItems( XmlNode resultNode )
        {
            this.resultingItem = null;
            XmlNode itemNode = resultNode.ChildNodes[0];

            if( itemNode == null || resultNode.ChildNodes.Count > 1 )
            {
                throw new InvalidDataException( "There must be one and -only- one result in a recipe." );
            }

            var nameAtri = itemNode.Attributes[0];
            var amountAtri = itemNode.Attributes[1];
            var appliesAffixesOfAtri = itemNode.Attributes["applyAffixesOf"];

            string name = nameAtri.Value;
            int amount = int.Parse( amountAtri.Value, CultureInfo.InvariantCulture );
            string appliesAffixesOf = appliesAffixesOfAtri != null ? appliesAffixesOfAtri.Value : null;

            this.resultingItem = new ResultRecipeItem( name, amount, appliesAffixesOf );
            this.resultingItem.FullyLoad( this.itemManager );
        }

        private void ParseHandler( XmlNode handlerNode )
        {
            if( handlerNode == null )
            {
                this.handler = new NormalRecipeHandler();
            }
            else
            {
                XmlNode handlerDataNode = handlerNode.FirstChild;

                switch( handlerDataNode.Name )
                {
                    case "TransformGems":
                        this.handler = ParseTransformGemsHandler( handlerDataNode );
                        break;

                    default:
                        this.handler = null;
                        throw new InvalidDataException( "Unknown handler type '" + handlerNode.Name + "'." );
                }
            }
        }

        private IRecipeHandler ParseTransformGemsHandler( XmlNode handlerDataNode )
        {
            ElementalSchool gemColor;
            string gemColorString = handlerDataNode.Attributes["color"].Value;

            if( !Enum.TryParse<ElementalSchool>( gemColorString, out gemColor ) )
            {
                throw new InvalidDataException( "Unknown gem color '" + gemColorString + "'." );
            }

            return new TransformGemsRecipeHandler() {
                GemColor = gemColor
            };
        }

        /// <summary>
        /// Stores the name of the Recipe that is currently beeing parsed.
        /// </summary>
        private string recipeName;

        /// <summary>
        /// The (optional) description of the Recipe.
        /// </summary>
        private string description;

        /// <summary>
        /// The level of the item. The levle of the player must be within range of the recipe level to use the recipe.
        /// </summary>
        private int level;

        /// <summary>
        /// Stores the items that are required by the Recipe that is currently beeing parsed.
        /// </summary>
        private readonly IList<RequiredRecipeItem> requiredItems = new List<RequiredRecipeItem>();

        /// <summary>
        /// Stores the item that is generated by the Recipe that is currently beeing parsed.
        /// </summary>
        private ResultRecipeItem resultingItem;

        /// <summary>
        /// Stores the category of the Recipe that is currently beeing parsed.
        /// </summary>
        private RecipeCategory category;

        /// <summary>
        /// Stores a value indicating whether the Recipe that is currently beeing parsed
        /// is hidden from the user interface.
        /// </summary>
        private bool isHidden;

        /// <summary>
        /// The object that is responsible for using the Recipe.
        /// </summary>
        private IRecipeHandler handler;

        /// <summary>
        /// Implements a mechanism that allows loading of Item definition files
        /// into memory.
        /// </summary>
        private readonly ItemManager itemManager;
    }
}
