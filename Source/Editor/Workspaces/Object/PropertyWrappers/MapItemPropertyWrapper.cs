// <copyright file="MapItemPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines Zelda.Editor.Object.PropertyWrappers.MapItemPropertyWrapper class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Object.PropertyWrappers
{
    using Atom.Math;
    using Zelda.Entities;

    /// <summary>
    /// Defines the IObjectPropertyWrapper for the <see cref="MapItem"/> type.
    /// </summary>
    internal sealed class MapItemPropertyWrapper : EntityPropertyWrapper<MapItem>
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
        public string MapItem
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
                    var item = this.serviceProvider.ItemManager.Load( value );
                    this.WrappedObject.ItemInstance = item.CreateInstance();
                }
            }
        }

        #endregion

        /// <summary>
        /// Returns a clone of this <see cref="IObjectPropertyWrapper"/>.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new MapItemPropertyWrapper( serviceProvider );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapItemPropertyWrapper"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public MapItemPropertyWrapper( IZeldaServiceProvider serviceProvider )
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
