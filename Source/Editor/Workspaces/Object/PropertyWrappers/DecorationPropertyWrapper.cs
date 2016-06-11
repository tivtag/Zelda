// <copyright file="DecorationPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines Zelda.Editor.Object.PropertyWrappers.DecorationPropertyWrapper class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Object.PropertyWrappers
{
    using System;
    using System.ComponentModel;
    using Atom.Math;
    using Zelda.Design;
    using Zelda.Entities;
    using Zelda.Editor.Design;

    // <summary>
    /// Defines the IObjectPropertyWrapper for Decoration entities.
    /// </summary>
    internal sealed class DecorationPropertyWrapper : EntityPropertyWrapper<Decoration>
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

        [LocalizedDisplayName( "PropDisp_FloorNumber" )]
        [LocalizedCategory( "PropCate_Transform" )]
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

        [LocalizedDisplayName( "PropDisp_CollisionArea" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_CollisionArea" )]
        public RectangleF CollisionArea
        {
            get
            {
                return this.WrappedObject.Collision.Rectangle;
            }
        }

        [LocalizedCategory( "PropCate_Settings" )]
        public Vector2 CollisionSize
        {
            get
            {
                return this.WrappedObject.Collision.Size;
            }

            set
            {
                this.WrappedObject.Collision.Size = value;
            }
        }

        [LocalizedCategory( "PropCate_Settings" )]
        public Vector2 CollisionOffset
        {
            get
            {
                return this.WrappedObject.Collision.Offset;
            }

            set
            {
                this.WrappedObject.Collision.Offset = value;
            }
        }
                
        [LocalizedCategory( "PropCate_Visuals" )]
        [LocalizedDisplayName( "PropDisp_DrawDataAndStrategy" )]
        [LocalizedDescriptionAttribute( "PropDesc_DrawDataAndStrategy" )]   
        public DrawStrategyEntityPropertyWrapper DrawStrategy
        {
            get
            {
                return this.drawStrategyWrapper;
            }
        }

        /// <summary>
        /// Initializes a new instance of the DecorationPropertyWrapper class.
        /// </summary>
        public DecorationPropertyWrapper()
        {
            this.drawStrategyWrapper = new DrawStrategyEntityPropertyWrapper( this );
        }

        /// <summary>
        /// Returns a clone of this <see cref="IObjectPropertyWrapper"/>.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new DecorationPropertyWrapper();
        }

        private readonly DrawStrategyEntityPropertyWrapper drawStrategyWrapper;
    }
}
