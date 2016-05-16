// <copyright file="TileBlockPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines Zelda.Editor.Object.PropertyWrappers.TileBlockPropertyWrapper class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Editor.Object.PropertyWrappers
{
    using Atom.Math;
    using Zelda.Entities;

    /// <summary>
    /// Defines the IObjectPropertyWrapper for the <see cref="TileBlock"/> type.
    /// </summary>
    internal sealed class TileBlockPropertyWrapper : EntityPropertyWrapper<TileBlock>
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

        [LocalizedDisplayName( "PropDisp_PositionTile" )]
        [LocalizedCategory( "PropCate_Transform" )]
        [LocalizedDescriptionAttribute( "PropDesc_PositionTile" )]
        public Point2 PositionTile
        {
            get { return this.WrappedObject.Transform.PositionTile; }
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
            get { return this.WrappedObject.IsSolid; }
            set { this.WrappedObject.IsSolid = value; }
        }

        [LocalizedDisplayName( "PropDisp_Sprite" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_Sprite" )]
        [System.ComponentModel.Editor( typeof( Zelda.Graphics.Design.SpriteEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Atom.Xna.Sprite Sprite
        {
            get
            {
                return this.WrappedObject.DrawDataAndStrategy.Sprite;
            }

            set
            {
                this.WrappedObject.DrawDataAndStrategy.SpriteGroup = value != null ? value.Name : string.Empty;
                this.WrappedObject.DrawDataAndStrategy.Load( serviceProvider );
            }
        }

        #endregion

        /// <summary>
        /// Returns a clone of this <see cref="IObjectPropertyWrapper"/>.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new TileBlockPropertyWrapper( serviceProvider );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TileBlockPropertyWrapper"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public TileBlockPropertyWrapper( IZeldaServiceProvider serviceProvider )
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
