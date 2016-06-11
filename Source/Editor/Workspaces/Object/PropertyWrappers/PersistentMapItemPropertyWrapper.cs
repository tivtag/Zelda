// <copyright file="PersistentMapItemPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines Zelda.Editor.Object.PropertyWrappers.PersistentMapItemPropertyWrapper class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Editor.Object.PropertyWrappers
{
    using Atom.Math;
    using Zelda.Entities;
    using System.ComponentModel;

    /// <summary>
    /// Defines the IObjectPropertyWrapper for the <see cref="PersistentMapItem"/> type.
    /// </summary>
    internal sealed class PersistentMapItemPropertyWrapper : EntityPropertyWrapper<PersistentMapItem>
    {
        #region > Transform <

        [LocalizedDisplayName( "PropDisp_Position" )]
        [LocalizedCategory( "PropCate_Transform" )]
        [LocalizedDescriptionAttribute( "PropDesc_Position" )]
        public Vector2 Position
        {
            get { return this.WrappedObject.Transform.Position; }
            set { this.WrappedObject.Transform.Position = value; }
        }

        [LocalizedDisplayName( "PropDisp_FloorNumber" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_FloorNumber" )]
        public int FloorNumber
        {
            get { return this.WrappedObject.FloorNumber; }
            set { this.WrappedObject.FloorNumber = value; }
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

        #region > Collision <

        [LocalizedDisplayName( "PropDisp_CollisionRectangle" )]
        [LocalizedCategory( "PropCate_Collision" )]
        [LocalizedDescriptionAttribute( "PropDesc_CollisionRectangle" )]
        public RectangleF CollisionRectangle
        {
            get { return this.WrappedObject.Collision.Rectangle; }
        }

        #endregion

        #region > Settings <

        [LocalizedDisplayName( "PropDisp_MapItem" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_MapItem" )]
        [System.ComponentModel.Editor( typeof( Zelda.Items.Design.ItemNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string Sprite
        {
            get
            {
                return this.WrappedObject.ItemInstance == null ? string.Empty : this.WrappedObject.ItemInstance.Item.Name;
            }
            set
            {
                if( string.IsNullOrEmpty( value ) )
                {
                    this.WrappedObject.ItemInstance = null;
                }
                else
                {
                    var item      = serviceProvider.ItemManager.Load( value );
                    this.WrappedObject.ItemInstance = item.CreateInstance();
                }
            }
        }

        [LocalizedDisplayName( "PropDisp_IsRemovingPersistanceOnPickUp" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_IsRemovingPersistanceOnPickUp" )]
        [DefaultValue(true)]
        public bool IsRemovingPersistanceOnPickUp
        {
            get
            {
                return this.WrappedObject.IsRemovingPersistanceOnPickUp;
            }
            set
            {
                this.WrappedObject.IsRemovingPersistanceOnPickUp = value;
            }
        }

        #endregion

        /// <summary>
        /// Returns a clone of this <see cref="IObjectPropertyWrapper"/>.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new PersistentMapItemPropertyWrapper( serviceProvider );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentMapItemPropertyWrapper"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public PersistentMapItemPropertyWrapper( IZeldaServiceProvider serviceProvider )
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
