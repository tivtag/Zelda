// <copyright file="LightPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines Zelda.Editor.Object.PropertyWrappers.LightPropertyWrapper class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Object.PropertyWrappers
{
    using System.ComponentModel;
    using Atom.Math;
    using Atom.Xna;
    using Zelda.Entities;

    /// <summary>
    /// Defines the IObjectPropertyWrapper for the <see cref="Light"/> type.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    internal sealed class LightPropertyWrapper : EntityPropertyWrapper<Light>
    {
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

        [LocalizedDisplayName( "PropDisp_Scale" )]
        [LocalizedCategory( "PropCate_Transform" )]
        [LocalizedDescriptionAttribute( "PropDesc_Scale" )]
        public Vector2 Scale
        {
            get 
            {
                return this.WrappedObject.Transform.Scale;
            }

            set 
            {
                this.WrappedObject.Transform.Scale = value;
            }
        }

        [LocalizedDisplayName( "PropDisp_Rotation" )]
        [LocalizedCategory( "PropCate_Transform" )]
        [LocalizedDescriptionAttribute( "PropDesc_RotationDegree" )]
        public double Rotation
        {
            get 
            { 
                return System.Math.Round( MathUtilities.ToDegrees( (double)this.WrappedObject.Transform.Rotation ) );
            }
            set 
            {
                this.WrappedObject.Transform.Rotation = (float)MathUtilities.ToRadians( value );
            }
        }

        [LocalizedDisplayName( "PropDisp_CollisionRectangle" )]
        [LocalizedCategory( "PropCate_Collision" )]
        [LocalizedDescriptionAttribute( "PropDesc_CollisionRectangle" )]
        public RectangleF CollisionRectangle
        {
            get 
            {
                return this.WrappedObject.Collision.Rectangle;
            }
        }
        
        [LocalizedDisplayName( "PropDisp_Color" )]        
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_LightColor" )]
        public Microsoft.Xna.Framework.Color Color
        {
            get 
            {
                return this.WrappedObject.Color; 
            }

            set 
            {
                this.WrappedObject.Color = value;
            }
        }
        
        [LocalizedDisplayName( "PropDisp_Sprite" )]        
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_LightSprite" )]
        [System.ComponentModel.Editor( typeof( Zelda.Graphics.Design.SpriteEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public ISprite Sprite
        {
            get
            {
                return this.WrappedObject.Sprite;
            }

            set
            {
                this.WrappedObject.Sprite = value;
            }
        }
        /// <summary>
        /// Returns a clone of this <see cref="IObjectPropertyWrapper"/>.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new LightPropertyWrapper( serviceProvider );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightPropertyWrapper"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public LightPropertyWrapper( IZeldaServiceProvider serviceProvider )
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
