// <copyright file="UnlockableDoorTileBlockPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines Zelda.Editor.Object.PropertyWrappers.UnlockableDoorTileBlockPropertyWrapper class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Object.PropertyWrappers
{
    using System.ComponentModel;
    using Atom.Math;
    using Zelda.Entities;

    /// <summary>
    /// Defines the IObjectPropertyWrapper for the <see cref="UnlockableDoorTileBlock"/> type.
    /// </summary>
    internal sealed class UnlockableDoorTileBlockPropertyWrapper : EntityPropertyWrapper<UnlockableDoorTileBlock>
    {
        #region > Transform <

        [LocalizedDisplayName( "PropDisp_Position" )]
        [LocalizedCategory( "PropCate_Transform" )]
        [LocalizedDescriptionAttribute( "PropDesc_Position" )]
        public Vector2 Position
        {
            get 
            {
                return this.WrappedObject.Transform.Position;
            }

            set 
            {
                this.WrappedObject.Transform.Position = value;
            }
        }

        [LocalizedDisplayName( "PropDisp_PositionTile" )]
        [LocalizedCategory( "PropCate_Transform" )]
        [LocalizedDescriptionAttribute( "PropDesc_PositionTile" )]
        public Point2 PositionTile
        {
            get
            {
                return this.WrappedObject.Transform.PositionTile; 
            }
        }

        [LocalizedDisplayName( "PropDisp_FloorRelativity" )]
        [LocalizedCategory( "PropCate_Transform" )]
        [LocalizedDescriptionAttribute( "PropDesc_FloorRelativity" )]
        public EntityFloorRelativity FloorRelativity
        {
            get
            {
                return this.WrappedObject.FloorRelativity;
            }

            set
            {
                this.WrappedObject.FloorRelativity = value;
            }
        }

        #endregion

        #region > Settings <

        [LocalizedDisplayName( "PropDisp_IsSolid" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_TileBlock_IsSolid" )]
        public bool IsSolid
        {
            get 
            {
                return this.WrappedObject.IsSolid;
            }

            set
            {
                this.WrappedObject.IsSolid = value;
            }
        }

        [LocalizedDisplayName( "PropDisp_FloorNumber" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_FloorNumber" )]
        public int FloorNumber
        {
            get
            {
                return this.WrappedObject.FloorNumber; 
            }

            set 
            {
                this.WrappedObject.FloorNumber = value;
            }
        }

        [LocalizedDisplayName( "PropDisp_Sprite" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_Sprite" )]
        [System.ComponentModel.Editor( typeof( Zelda.Graphics.Design.SpriteNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string Sprite
        {
            get
            {
                return this.WrappedObject.DrawDataAndStrategy.SpriteGroup;
            }

            set
            {
                this.WrappedObject.DrawDataAndStrategy.SpriteGroup = value;
                this.WrappedObject.DrawDataAndStrategy.Load( serviceProvider );
            }
        }

        [LocalizedDisplayName( "PropDisp_UnlockableDoor_RequiredKeyItemName" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_UnlockableDoor_RequiredKeyItemName" )]
        [System.ComponentModel.Editor( typeof( Zelda.Items.Design.ItemNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string RequiredKeyItemName
        {
            get 
            {
                return this.WrappedObject.RequiredKeyName;
            }

            set 
            {
                this.WrappedObject.RequiredKeyName = value;
            }
        }

        [LocalizedDisplayName( "PropDisp_IsRemovingPersistanceOnUnlock" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_IsRemovingPersistanceOnUnlock" )]
        [DefaultValue( true )]
        public bool IsRemovingPersistanceOnUnlock
        {
            get
            {
                return this.WrappedObject.IsRemovingPersistanceOnUnlock;
            }

            set
            {
                this.WrappedObject.IsRemovingPersistanceOnUnlock = value;
            }
        }

        #endregion

        #region > Events <

        [LocalizedDisplayName( "PropDisp_UnlockableDoor_UnlockedEvent" )]
        [LocalizedCategory( "PropCate_Events" )]
        [LocalizedDescriptionAttribute( "PropDesc_UnlockableDoor_UnlockedEvent" )]
        [System.ComponentModel.Editor( typeof( Atom.Events.Design.EventCreationEditor), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Atom.Events.Event UnlockedEvent
        {
            get
            { 
                return this.WrappedObject.UnlockedEvent; 
            }

            set 
            {
                this.WrappedObject.UnlockedEvent = value; 
            }
        }

        [LocalizedDisplayName( "PropDisp_UnlockableDoor_NotUnlockedEvent" )]
        [LocalizedCategory( "PropCate_Events" )]
        [LocalizedDescriptionAttribute( "PropDesc_UnlockableDoor_NotUnlockedEvent" )]
        [System.ComponentModel.Editor( typeof( Atom.Events.Design.EventCreationEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Atom.Events.Event NotUnlockedEvent
        {
            get
            {
                return this.WrappedObject.NotUnlockedEvent;
            }

            set 
            {
                this.WrappedObject.NotUnlockedEvent = value; 
            }
        }

        #endregion

        /// <summary>
        /// Returns a clone of this <see cref="IObjectPropertyWrapper"/>.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new UnlockableDoorTileBlockPropertyWrapper( serviceProvider );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnlockableDoorTileBlockPropertyWrapper"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public UnlockableDoorTileBlockPropertyWrapper( IZeldaServiceProvider serviceProvider )
        {
            System.Diagnostics.Debug.Assert( serviceProvider != null );
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;
    }
}