// <copyright file="ItemViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ItemCreator.ItemViewModel class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.ItemCreator
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using Atom.Design;
    using Atom.Math;
    using Zelda.Items;

    /// <summary>
    /// Defines the view-model/property wrapper around the <see cref="Item"/> class.
    /// </summary>
    internal class ItemViewModel : BaseObjectPropertyWrapper, IObjectViewModel
    {
        #region [ Wrapped Properties ]

        [LocalizedCategory( "PropCate_Item" )]
        [LocalizedDisplayName( "PropDisp_Name" )]
        [LocalizedDescription( "PropDesc_Name" )]
        public string Name
        {
            get { return this.WrappedItem.Name; }
            set { this.WrappedItem.Name = value; }
        }

        [LocalizedCategory( "PropCate_Item" )]
        [LocalizedDisplayName( "PropDisp_LocalizedName" )]
        [LocalizedDescription( "PropDesc_LocalizedName" )]
        public string LocalizedName
        {
            get { return this.WrappedItem.LocalizedName; }
        }

        [LocalizedCategory( "PropCate_Item" )]
        [LocalizedDisplayName( "PropDisp_Description" )]
        [LocalizedDescription( "PropDesc_Description" )]
        public string Description
        {
            get { return this.WrappedItem.Description; }
            set { this.WrappedItem.Description = value; }
        }

        [LocalizedCategory( "PropCate_Item" )]
        [LocalizedDisplayName( "PropDisp_SpecialType" )]
        [LocalizedDescription( "PropDesc_SpecialType" )]
        public SpecialItemType SpecialType
        {
            get { return this.WrappedItem.SpecialType; }
            set { this.WrappedItem.SpecialType = value; }
        }

        [LocalizedCategory( "PropCate_Item" )]
        [LocalizedDisplayName( "PropDisp_LocalizedDescription" )]
        [LocalizedDescription( "PropDesc_LocalizedDescription" )]
        public string LocalizedDescription
        {
            get { return this.WrappedItem.LocalizedDescription; }
        }

        [LocalizedCategory( "PropCate_Item" )]
        [LocalizedDisplayName( "PropDisp_Level" )]
        [LocalizedDescription( "PropDesc_Level" )]
        [DefaultValue( 1 )]
        public int Level
        {
            get { return this.WrappedItem.Level; }
            set { this.WrappedItem.Level = value; }
        }

        [LocalizedCategory( "PropCate_Item" )]
        [LocalizedDisplayName( "PropDisp_StackSize" )]
        [LocalizedDescription( "PropDesc_StackSize" )]
        [DefaultValue( 1 )]
        public int StackSize
        {
            get { return this.WrappedItem.StackSize; }
            set { this.WrappedItem.StackSize = value; }
        }

        [LocalizedCategory( "PropCate_Item" )]
        [LocalizedDisplayName( "PropDisp_WidthInInventory" )]
        [LocalizedDescription( "PropDesc_WidthInInventory" )]
        [DefaultValue( 1 )]
        public int WidthInInventory
        {
            get { return this.WrappedItem.InventoryWidth; }
            set { this.WrappedItem.InventoryWidth = value; }
        }

        [LocalizedCategory( "PropCate_Item" )]
        [LocalizedDisplayName( "PropDisp_HeightInInventory" )]
        [LocalizedDescription( "PropDesc_HeightInInventory" )]
        [DefaultValue( 1 )]
        public int HeightInInventory
        {
            get { return this.WrappedItem.InventoryHeight; }
            set { this.WrappedItem.InventoryHeight = value; }
        }

        [LocalizedCategory( "PropCate_Item" )]
        [LocalizedDisplayName( "PropDisp_Recipes" )]
        [LocalizedDescription( "PropDesc_Recipes" )]
        public string[] Recipes
        {
            get { return this.WrappedItem.Recipes; }
            set { this.WrappedItem.Recipes = value; }
        }

        [LocalizedCategory( "PropCate_Item" )]
        [LocalizedDisplayName( "PropDisp_RubiesWorth" )]
        [LocalizedDescription( "PropDesc_RubiesWorth" )]
        [DefaultValue( 0 )]
        public int RupiesWorth
        {
            get { return this.WrappedItem.RubiesWorth; }
            set { this.WrappedItem.RubiesWorth = value; }
        }

        [LocalizedCategory( "PropCate_Item" )]
        [LocalizedDisplayName( "PropDisp_Quality" )]
        [LocalizedDescription( "PropDesc_Quality" )]
        public ItemQuality Quality
        {
            get { return this.WrappedItem.Quality; }
            set { this.WrappedItem.Quality = value; }
        }

        [LocalizedCategory( "PropCate_Item" )]
        [LocalizedDisplayName( "PropDisp_SoundOnPickupName" )]
        [LocalizedDescription( "PropDesc_SoundOnPickupName" )]
        public string SoundOnPickupName
        {
            get { return this.WrappedItem.SoundOnPickupName; }
            set { this.WrappedItem.SoundOnPickupName = value; }
        }

        [LocalizedCategory( "PropCate_Item" )]
        [LocalizedDisplayName( "PropDisp_SoundOnPickupVolume" )]
        [LocalizedDescription( "PropDesc_SoundOnPickupVolume" )]
        public FloatRange SoundOnPickupVolume
        {
            get { return this.WrappedItem.SoundOnPickupVolume; }
            set { this.WrappedItem.SoundOnPickupVolume = value; }
        }

        [LocalizedCategory( "PropCate_Item" )]
        [LocalizedDisplayName( "PropDisp_Sprite" )]
        [LocalizedDescription( "PropDesc_Sprite" )]
        [Editor( typeof( Zelda.Graphics.Design.SpriteAssetEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public object Sprite
        {
            get { return this.WrappedItem.Sprite; }
            set { this.WrappedItem.Sprite = (Atom.Xna.ISpriteAsset)value; }
        }

        [LocalizedCategory( "PropCate_Item" )]
        [LocalizedDisplayName( "PropDisp_SpriteColor" )]
        [LocalizedDescription( "PropDesc_SpriteColor" )]
        public Microsoft.Xna.Framework.Color SpriteColor
        {
            get { return this.WrappedItem.SpriteColor; }
            set { this.WrappedItem.SpriteColor = value; }
        }

        [LocalizedCategory( "PropCate_Item" )]
        [LocalizedDisplayName( "PropDisp_OnUseEffect" )]
        [LocalizedDescription( "PropDesc_OnUseEffect" )]
        [Editor( typeof( Zelda.ItemCreator.Design.OnUseEffectEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Zelda.Items.ItemUseEffect OnUseEffect
        {
            get { return this.WrappedItem.UseEffect; }
            set { this.WrappedItem.UseEffect = value; }
        }

        [LocalizedCategory( "PropCate_Item" )]
        [LocalizedDisplayName( "PropDisp_DropRequirement" )]
        [LocalizedDescription( "PropDesc_DropRequirement" )]
        [Editor( typeof( Zelda.ItemCreator.Design.DropRequirementEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Zelda.Core.Requirements.IRequirement DropRequirement
        {
            get { return this.WrappedItem.DropRequirement; }
            set { this.WrappedItem.DropRequirement = value; }
        }

        #endregion

        #region [ Wrapper ]

        /// <summary>
        /// Gets the Item this <see cref="ItemViewModel"/> wraps around.
        /// </summary>
        [Browsable( false )]
        public Item WrappedItem
        {
            get
            {
                return (Item)this.WrappedObject;
            }
        }

        /// <summary>
        /// Gets the <see cref="Type"/> this <see cref="ItemViewModel"/> wraps around.
        /// </summary>
        [Browsable( false )]
        public override Type WrappedType
        {
            get
            {
                return typeof( Item );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemViewModel"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game related services.
        /// </param>
        internal ItemViewModel( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Returns a clone of this <see cref="ItemViewModel"/>.
        /// </summary>
        /// <returns>The cloned IObjectPropertyWrapper.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new ItemViewModel( this.serviceProvider );
        }

        /// <summary>
        /// Gets the <see cref="IZeldaServiceProvider"/> which provides
        /// fast access to game-related services.
        /// </summary>
        protected IZeldaServiceProvider ServiceProvider
        {
            get
            {
                return this.serviceProvider;
            }
        }

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        #endregion

        #region [ ViewModel ]

        /// <summary>
        /// Saves this ItemViewModel.
        /// </summary>
        public virtual void Save()
        {
            string directory = this.GetDirectory();
            string fullPath = directory + this.GetFileName();
            Directory.CreateDirectory( directory );

            using( var stream = File.Create( fullPath ) )
            {
                var context = new Zelda.Saving.SerializationContext( new BinaryWriter( stream ), this.serviceProvider );
                this.WrappedItem.Serialize( context );
            }
        }
        
        /// <summary>
        /// Gets the full path of the directory in which
        /// the object is saved in.
        /// </summary>
        /// <returns></returns>
        private string GetDirectory()
        {
            return Path.Combine( AppDomain.CurrentDomain.BaseDirectory, this.GetRelativeDirectory() );
        }

        /// <summary>
        /// Gets the directory, relative to the game folder,
        /// in which the object is saved in.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetRelativeDirectory()
        {
            return @"Content\Items\";
        }

        /// <summary>
        /// Gets the filename under which the item is saved.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetFileName()
        {
            return this.Name + Zelda.Items.Item.Extension;
        }

        #endregion
    }
}