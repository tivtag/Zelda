// <copyright file="Item.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Item class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Items
{
    using System;
    using Atom.Xna;
    using Zelda.Core.Requirements;
    using Zelda.Items.Affixes;
    using Zelda.Saving;
    using Atom.Math;

    /// <summary>
    /// The <see cref="Item"/> class describes the properties of a simple item. 
    /// </summary>
    /// <remarks>
    /// To create an actual instance of an Item one must use the <see cref="ItemInstance"/> class.
    /// </remarks>
    public class Item : Atom.IReadOnlyNameable, Zelda.Saving.ISaveable
    {
        #region [ Constants ]

        /// <summary>
        /// The extension any Item definition file uses. ".zitm"
        /// </summary>
        public const string Extension = ".zitm";

        #endregion

        #region [ Properties ]

        #region > Strings <

        /// <summary>
        /// Gets or sets the (unique) name of this <see cref="Item"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return this._name;
            }

            set
            {
                if( value == null )
                    throw new ArgumentNullException( "value" );

                this._name = value;
                this.LocalizedName = GetLocalizedName( value );
            }
        }

        /// <summary>
        /// Gets the localized name of the Item with the specified itemName.
        /// </summary>
        /// <param name="itemName">
        /// The name of the Item.
        /// </param>
        /// <returns>
        /// The localized name; or the original input name.
        /// </returns>
        internal static string GetLocalizedName( string itemName )
        {
            try
            {
                string str = ItemResources.ResourceManager.GetString( "IN_" + itemName );
                return str ?? itemName;
            }
            catch( InvalidOperationException )
            {
                return itemName;
            }
        }

        /// <summary>
        /// Gets the localized name of this <see cref="Item"/>.
        /// </summary>
        /// <remarks>
        /// The Localized Name is aquired by looking
        /// up the string "IN_X", where X is the <see cref="Name"/>,
        /// in the <see cref="ItemResources"/>.
        /// </remarks>
        public string LocalizedName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the description of this <see cref="Item"/>.
        /// </summary>
        public string Description
        {
            get { return this._description; }
            set
            {
                if( value == null )
                    throw new ArgumentNullException( "value" );

                this._description = value;

                try
                {
                    this.LocalizedDescription = ItemResources.ResourceManager.GetString( "ID_" + value );
                }
                catch( InvalidOperationException )
                {
                    this.LocalizedDescription = value;
                }
            }
        }

        /// <summary>
        /// Gets the localized description of this <see cref="Item"/>.
        /// </summary>
        /// <remarks>
        /// The Localized Description is aquired by looking
        /// up the string "ID_X", where X is the <see cref="Description"/>,
        /// in the <see cref="Zelda.Resources"/>.
        /// </remarks>
        public string LocalizedDescription
        {
            get
            {
                return this._localizedDescription;
            }

            private set
            {
                this._localizedDescription = value;

                // Set the LocalizedDescriptionSplit:
                if( value != null )
                {
                    if( this._localizedDescription.Contains( Environment.NewLine ) )
                    {
                        this.LocalizedDescriptionSplit = this._localizedDescription.Split(
                            new string[1] { Environment.NewLine }, StringSplitOptions.None
                        );
                    }
                    else
                    {
                        this.LocalizedDescriptionSplit = null;
                    }
                }
                else
                {
                    this.LocalizedDescriptionSplit = null;
                }
            }
        }

        /// <summary>
        /// Gets the localized description of this <see cref="Item"/>,
        /// splitting between new lines.
        /// </summary>
        /// <value>
        /// This value is only used if there actually is a NewLine string in the <see cref="LocalizedDescription"/>.
        /// </value>
        public string[] LocalizedDescriptionSplit
        {
            get;
            private set;
        }

        #endregion

        #region > Visualization <

        /// <summary>
        /// Gets or sets the <see cref="Atom.Xna.ISpriteAsset"/> that is used to display this Item.
        /// </summary>
        /// <value>
        /// This property may be null
        /// </value>
        public Atom.Xna.ISpriteAsset Sprite
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color the <see cref="Sprite"/> is tinted in.
        /// </summary>
        public Microsoft.Xna.Framework.Color SpriteColor
        {
            get
            {
                return this._spriteColor;
            }

            set
            {
                this._spriteColor = value;
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the level of this <see cref="Item"/>.
        /// </summary>
        public int Level
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="SpecialItemType"/> of this Item.
        /// </summary>
        public SpecialItemType SpecialType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the <see cref="ItemType"/> this <see cref="Item"/> represents.
        /// </summary>
        public virtual ItemType ItemType
        {
            get
            {
                return ItemType.Item;
            }
        }

        /// <summary>
        /// Gets or sets a value that represents how many rupies <see cref="Item"/>s of this type are worth.
        /// </summary>
        public int RubiesWorth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="ItemQuality"/> of this <see cref="Item"/>.
        /// </summary>
        /// <value>The default value is <see cref="ItemQuality.Common"/>.</value>
        public ItemQuality Quality
        {
            get
            {
                return this.quality;
            }

            set
            {
                this.quality = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of <see cref="Item"/>s of this type
        /// that can be placed ontop of each-other on a stack.
        /// </summary>
        /// <value>The default value is 1.</value>
        public int StackSize
        {
            get
            {
                return this.stackSize;
            }

            set
            {
                this.stackSize = value;
            }
        }

        public bool AllowedToVaryInPower
        {
            get
            {
                return this.StackSize == 1;
            }
        }

        /// <summary>
        /// Gets or sets the power difference allowed by this Item.
        /// </summary>
        /// <remarks>
        /// Only works with non-stackable items. 
        /// Formula:
        /// [Base Item] -> Apply suffixes -> Apply Power Difference -> Apply Gems (which each have their own power difference) -> [Final Item]
        /// </remarks>
        /// <value>The default value is -0.15f to +0.15f.</value>
        public FloatRange PossiblePowerDifference
        {
            get
            {
                return this.possiblePowerDifference;
            }

            set
            {
                this.possiblePowerDifference = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Item"/>s of this type can be stacked.
        /// </summary>
        public bool IsStackable
        {
            get
            {
                return this.StackSize > 1;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating what affixes this Item
        /// is allowed to have.
        /// </summary>
        public AffixType AllowedAffixes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the names of the recipes that contain
        /// this <see cref="Item"/> as a reagent. May be null.
        /// </summary>
        public string[] Recipes
        {
            get
            {
                return this._recipes;
            }

            set
            {
                if( value == null )
                    this._recipes = null;
                else
                    this._recipes = (string[])value.Clone();
            }
        }

        /// <summary>
        /// Gets or sets the effect that happens when this <see cref="Item"/> gets used.
        /// </summary>
        /// <value>The default value is null.</value>
        public ItemUseEffect UseEffect
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an <see cref="IRequirement"/> that states
        /// whether this Item can currently drop.
        /// </summary>
        public IRequirement DropRequirement
        {
            get;
            set;
        }

        #region > Sound <

        /// <summary>
        /// Gets or sets the name of the Sound File that 
        /// is played when the Item has been picked up.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        public string SoundOnPickupName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the (ready to be played) <see cref="Atom.Fmod.Sound"/> that 
        /// is played when the Item has been picked up.
        /// </summary>
        /// <remarks>
        /// This field is initialized when this <see cref="Item"/> gets loaden.
        /// </remarks>
        public Atom.Fmod.Sound SoundOnPickup
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the volume of the <see cref="SoundOnPickup"/>.
        /// </summary>
        /// <value>The default value is [1..1].</value>
        public FloatRange SoundOnPickupVolume
        {
            get
            {
                return this.soundOnPickupVolume;
            }

            set
            {
                this.soundOnPickupVolume = value;
            }
        }

        #endregion

        #region > Inventory <

        /// <summary>
        /// Gets or sets a value that represents the width of 
        /// this <see cref="Item"/> in the Inventory. (in tile-space)
        /// </summary>
        /// <value>
        /// The default value is 1.
        /// </value>
        public int InventoryWidth
        {
            get { return this._widthInInventory; }
            set { this._widthInInventory = value; }
        }

        /// <summary>
        /// Gets or sets a value that represents the height of 
        /// this <see cref="Item"/> in the Inventory. (in tile-space)
        /// </summary>
        /// <value>
        /// The default value is 1.
        /// </value>
        public int InventoryHeight
        {
            get { return this._heightInInventory; }
            set { this._heightInInventory = value; }
        }

        /// <summary>
        /// Gets a value that represents the size of this Item in the Inventory. (in tile-space)
        /// </summary>
        public Atom.Math.Point2 InventorySize
        {
            get
            {
                return new Atom.Math.Point2( this.InventoryWidth, this.InventoryHeight );
            }
        }

        #endregion

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets a random multiplicative power factor for new instances of this Item.
        /// </summary>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <returns>
        /// A new random multiplative power factor.
        /// </returns>
        public float GetPowerFactor( IRand rand )
        {
            if( this.AllowedToVaryInPower )
            {
                return 1.0f + this.possiblePowerDifference.GetRandomValue( rand );
            }
            else
            {
                return 1.0f;
            }
        }

        /// <summary>
        /// Modifies the power of this Item by the given factor.
        /// </summary>
        /// <param name="factor">
        /// The factor to modify the item by.
        /// </param>
        /// <returns>
        /// true if modifications were allowed; -or- otherwise false.
        /// </returns>
        public virtual bool ModifyPowerBy( float factor )
        {
            if( factor != 1.0f && this.AllowedToVaryInPower )
            {
                if( this.RubiesWorth > 1 )
                {
                    this.RubiesWorth = (int)(this.RubiesWorth * factor);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a new instance of this <see cref="Item"/> with a random power factor.
        /// </summary>
        /// <param name="rand">
        /// The random number generator used to generate the power factor.
        /// </param>
        /// <returns>A new <see cref="ItemInstance"/>.</returns>
        public ItemInstance CreateInstance( IRand rand )
        {
            return this.CreateInstance( this.GetPowerFactor( rand ) );
        }

        /// <summary>
        /// Creates a new instance of this <see cref="Item"/>.
        /// </summary>
        /// <param name="powerFactor">
        /// The factor by which the power of the new ItemInstance varies compared to this base Item.
        /// </param>
        /// <returns>A new <see cref="ItemInstance"/>.</returns>
        public virtual ItemInstance CreateInstance( float powerFactor = 1.0f )
        {
            return new ItemInstance( this, powerFactor );
        }

        /// <summary>
        /// Multiplies the <see cref="SpriteColor"/> of this Item by the specified multipliers.
        /// </summary>
        /// <param name="redMultiplier">The multiplier value for the red color component.</param>
        /// <param name="greenMultiplier">The multiplier value for the green color component.</param>
        /// <param name="blueMultiplier">The multiplier value for the blue color component.</param>
        internal void MultiplyColor( float redMultiplier, float greenMultiplier, float blueMultiplier )
        {
            this.MultiplyColor( redMultiplier, greenMultiplier, blueMultiplier, 1.0f );
        }

        /// <summary>
        /// Multiplies the <see cref="SpriteColor"/> of this Item by the specified multipliers.
        /// </summary>
        /// <param name="redMultiplier">The multiplier value for the red color component.</param>
        /// <param name="greenMultiplier">The multiplier value for the green color component.</param>
        /// <param name="blueMultiplier">The multiplier value for the blue color component.</param>
        /// <param name="alphaMultiplier">The multiplier value for the alpha color component.</param>
        internal virtual void MultiplyColor( float redMultiplier, float greenMultiplier, float blueMultiplier, float alphaMultiplier )
        {
            this.SpriteColor = this.SpriteColor.MultiplyBy( redMultiplier, greenMultiplier, blueMultiplier, alphaMultiplier );
        }

        /// <summary>
        /// Overriden to return the name of this Item.
        /// </summary>
        /// <returns>
        /// A System.String that represents the current Item.
        /// </returns>
        public override string ToString()
        {
            return this.Name ?? base.ToString();
        }

        #region > Cloning <

        /// <summary>
        /// Creates a clone of this <see cref="Item"/>.
        /// </summary>
        /// <returns>
        /// The cloned item.
        /// </returns>
        public virtual Item Clone()
        {
            var clone = new Item();

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the given Item to be a clone of this Item.
        /// </summary>
        /// <param name="clone">
        /// The Item to setup as a clone of this Item.
        /// </param>
        protected void SetupClone( Item clone )
        {
            clone._name = this._name;
            clone._localizedDescription = this._localizedDescription;
            clone.LocalizedName = this.LocalizedName;
            clone.LocalizedDescriptionSplit = this.LocalizedDescriptionSplit;

            clone.Level = this.Level;
            clone.SpecialType = this.SpecialType;
            clone.Recipes = this.Recipes;
            clone.DropRequirement = this.DropRequirement;
            clone.UseEffect = this.UseEffect;
            clone.RubiesWorth = this.RubiesWorth;
            clone.Sprite = this.Sprite;
            clone.SoundOnPickup = this.SoundOnPickup;
            clone.SoundOnPickupName = this.SoundOnPickupName;
            clone.soundOnPickupVolume = this.soundOnPickupVolume;

            clone._widthInInventory = this._widthInInventory;
            clone._heightInInventory = this._heightInInventory;
            clone.quality = this.quality;
            clone._spriteColor = this._spriteColor;
            clone.stackSize = this.stackSize;
        }

        #endregion

        #region > Storage <

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            // Write Global Header:
            context.Write( Atom.ReflectionExtensions.GetTypeName( this.GetType() ) );

            // Write Item Header:
            const int Version = 7;
            context.Write( Version );

            // Write Item Data:
            context.Write( this.Name ?? string.Empty );
            context.Write( this.Description ?? string.Empty );

            context.Write( this.Level );
            context.Write( (int)this.Quality );
            context.Write( this.RubiesWorth );
            context.Write( this.StackSize );
            context.Write( this.InventoryWidth );
            context.Write( this.InventoryHeight );
            context.Write( (int)this.AllowedAffixes );
            context.Write( (int)this.SpecialType );
            context.Write( this.possiblePowerDifference ); // New in v5

            context.Write( this.Sprite == null ? string.Empty : this.Sprite.Name );
            context.Write( _spriteColor );

            context.Write( this.SoundOnPickupName ?? string.Empty );
            context.Write( this.SoundOnPickupVolume ); // New in v6

            // Write Recipes:
            if( this._recipes != null )
            {
                // Remove any empty/null strings:
                var trimedRecipeList = new System.Collections.Generic.List<string>( this._recipes.Length );

                for( int i = 0; i < this._recipes.Length; ++i )
                {
                    string recipe = this._recipes[i];

                    if( !string.IsNullOrEmpty( recipe ) )
                        trimedRecipeList.Add( recipe );
                }

                this._recipes = trimedRecipeList.ToArray();

                context.Write( this._recipes.Length );
                for( int i = 0; i < this._recipes.Length; ++i )
                    context.Write( this._recipes[i] );
            }
            else
            {
                context.Write( (int)0 );
            }

            // Write OnUse Effect:
            if( this.UseEffect != null )
            {
                context.Write( true );
                this.UseEffect.Serialize( context );
            }
            else
            {
                context.Write( false );
            }

            context.WriteObject( this.DropRequirement );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            // Global Header has been readen.
            var serviceProvider = context.ServiceProvider;

            // Read Item Header:
            const int CurrentVersion = 7;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 3, CurrentVersion, "Zelda.Items.Item" );

            this.Name = context.ReadString();
            this.Description = context.ReadString();

            this.Level = context.ReadInt32();
            this.Quality = (ItemQuality)context.ReadInt32();
            this.RubiesWorth = context.ReadInt32();
            this.StackSize = context.ReadInt32();
            this.InventoryWidth = context.ReadInt32();
            this.InventoryHeight = context.ReadInt32();

            if( version >= 3 )
                this.AllowedAffixes = (AffixType)context.ReadInt32();

            if( version >= 4 )
                this.SpecialType = (SpecialItemType)context.ReadInt32();

            if( version >= 5 )
                this.possiblePowerDifference = context.ReadFloatRange();

            string spriteName = context.ReadString();
            #region - Load Sprite -

            if( spriteName.Length > 0 && serviceProvider != null )
            {
                try
                {
                    this.Sprite = serviceProvider.SpriteLoader.LoadAsset( spriteName );
                }
                catch( Microsoft.Xna.Framework.Content.ContentLoadException exc )
                {
                    var message = string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.Error_LoadingSpriteAssetXExceptionMessageY,
                        spriteName,
                        exc.Message
                    );

                    serviceProvider.Log.WriteLine();
                    serviceProvider.Log.WriteLine( message );
                }
            }

            #endregion

            this.SpriteColor = context.ReadColor();

            this.SoundOnPickupName = context.ReadString();
            if( version >= 7 )
                this.soundOnPickupVolume = context.ReadFloatRange();

            #region - Load Sound -

            if( this.SoundOnPickupName.Length != 0 && serviceProvider != null )
            {
                Atom.Fmod.AudioSystem audioSystem = serviceProvider.AudioSystem;

                if( audioSystem != null )
                {
                    try
                    {
                        this.SoundOnPickup = audioSystem.GetSample( this.SoundOnPickupName );

                        if( this.SoundOnPickup != null )
                            this.SoundOnPickup.LoadAsSample( false );
                    }
                    catch( Atom.Fmod.AudioException exc )
                    {
                        var message = string.Format(
                            System.Globalization.CultureInfo.CurrentCulture,
                            Resources.Error_LoadingSoundAssetXExceptionMessageY,
                            spriteName,
                            exc.Message
                        );

                        context.ServiceProvider.Log.WriteLine();
                        context.ServiceProvider.Log.WriteLine( message );
                    }
                }
            }

            #endregion

            int recipeCount = context.ReadInt32();
            if( recipeCount > 0 )
            {
                this._recipes = new string[recipeCount];
                for( int i = 0; i < this._recipes.Length; ++i )
                    this._recipes[i] = context.ReadString();
            }

            bool hasOnUseEffect = context.ReadBoolean();
            if( hasOnUseEffect )
            {
                string effectTypeName = context.ReadString();
                Type type = Type.GetType( effectTypeName );

                this.UseEffect = (ItemUseEffect)Activator.CreateInstance( type );
                this.UseEffect.Setup( serviceProvider );
                this.UseEffect.Deserialize( context );
            }

            this.DropRequirement = context.ReadObject<IRequirement>();
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Defines the storage field for the <see cref="Name"/> property.
        /// </summary>
        private string _name;

        /// <summary>
        /// Defines the storage field for the <see cref="Description"/> property.
        /// </summary>
        private string _description;

        /// <summary>
        /// Defines the storage field for the <see cref="LocalizedDescription"/> property.
        /// </summary>
        private string _localizedDescription;

        /// <summary>
        /// Defines the storage field for the <see cref="StackSize"/> property.
        /// </summary>
        private int stackSize = 1;

        /// <summary>
        /// [Base Item] -> Apply suffixes -> Apply Power Difference -> Apply Gems (which each have their own power difference) -> [Final Item]
        /// </summary>
        private FloatRange possiblePowerDifference = new FloatRange( -0.10f, 0.10f );

        /// <summary>
        /// The names of the recipes that contain the <see cref="Item"/> as a reagent. May be null.
        /// </summary>
        private string[] _recipes;

        /// <summary>
        /// Defines the storage field for the <see cref="Quality"/> property.
        /// </summary>
        private ItemQuality quality = ItemQuality.Common;

        /// <summary>
        /// Defines the storage field for the <see cref="InventoryWidth"/> property.
        /// </summary>
        private int _widthInInventory = 1;

        /// <summary>
        /// Defines the storage field for the <see cref="InventoryHeight"/> property.
        /// </summary>
        private int _heightInInventory = 1;

        /// <summary>
        /// Defines the storage field for the <see cref="SpriteColor"/> property.
        /// </summary>
        private Microsoft.Xna.Framework.Color _spriteColor = Microsoft.Xna.Framework.Color.White;

        /// <summary>
        /// Defines the storage field for the <see cref="SoundOnPickupVolume"/> property.
        /// </summary>
        private FloatRange soundOnPickupVolume = new FloatRange( 1.0f, 1.0f );

        #endregion
    }
}