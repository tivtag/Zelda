// <copyright file="MapItemPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines Zelda.Editor.Object.PropertyWrappers.MapItemPropertyWrapper class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Object.PropertyWrappers
{
    using System.ComponentModel;
    using Atom;
    using Atom.Math;
    using Zelda.Entities;
    using System;
    using Zelda.Design;

    /// <summary>
    /// Defines the IObjectPropertyWrapper for the <see cref="UseableActionEntity"/> type.
    /// </summary>
    internal sealed class UseableActionEntityPropertyWrapper : EntityPropertyWrapper<UseableActionEntity>
    {
        [LocalizedDisplayName( "PropDisp_Position" )]
        [LocalizedCategory( "PropCate_Transform" )]
        [LocalizedDescriptionAttribute( "PropDesc_Position" )]
        public Vector2 Position
        {
            get { return this.WrappedObject.Transform.Position; }
            set { this.WrappedObject.Transform.Position = value; }
        }

        [LocalizedDisplayName( "PropDisp_Offset" )]
        [LocalizedCategory( "PropCate_Transform" )]
        [LocalizedDescriptionAttribute( "PropDesc_Offset" )]
        public Vector2 Offset
        {
            get { return this.WrappedObject.Collision.Offset; }
            set { this.WrappedObject.Collision.Offset = value; }
        }
        
        [LocalizedDisplayName( "PropDisp_UseArea" )]
        [LocalizedCategory( "PropCate_Transform" )]
        [LocalizedDescriptionAttribute( "PropDesc_UseArea" )]
        public RectangleF UseArea
        {
            get
            {
                return this.WrappedObject.Collision.Rectangle;
            }

            set
            {
                this.WrappedObject.Transform.Position = value.Position;
                this.WrappedObject.Collision.Size = value.Size;
            }
        }

        [LocalizedDisplayName( "PropDisp_FloorNumber" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_FloorNumber" )]
        public int FloorNumber
        {
            get { return this.WrappedObject.FloorNumber; }
            set { this.WrappedObject.FloorNumber = value; }
        }

        [LocalizedDisplayName( "PropDisp_Direction" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_Direction" )]
        public Direction4 Direction
        {
            get { return this.WrappedObject.Transform.Direction; }
            set { this.WrappedObject.Transform.Direction = value; }
        }

        [LocalizedDisplayName( "PropDisp_HasToFaceToUse" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_HasToFaceToUse" )]
        [DefaultValue(false)]
        public bool HasToFaceToUse
        {
            get { return this.WrappedObject.HasToFace; }
            set { this.WrappedObject.HasToFace = value; }
        }

        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_UseAction" )]
        [Editor( typeof( Zelda.Actions.Design.ActionEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public IAction Action
        {
            get
            {
                return this.WrappedObject.Action;
            }

            set
            {
                this.WrappedObject.Action = (IAction)value;
            }
        }

        [LocalizedCategory( "PropCate_Visuals" )]
        [LocalizedDisplayName( "PropDisp_DrawDataAndStrategyType" )]
        [LocalizedDescriptionAttribute( "PropDesc_DrawDataAndStrategyType" )]        
        [Editor( typeof( Zelda.Entities.Drawing.Design.DrawDataAndStrategyTypeEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Type DrawDataAndStrategyType
        {
            get
            {
                return this.WrappedObject.DrawDataAndStrategy != null ? 
                    this.WrappedObject.DrawDataAndStrategy.GetType() : null;
            }

            set
            {
                if( value == null )
                {
                    this.WrappedObject.DrawDataAndStrategy = null;
                }
                else
                {
                    Entities.Drawing.DrawStrategyManager strategyManager = DesignTime.Services.DrawStrategyManager;
                    this.WrappedObject.DrawDataAndStrategy = strategyManager.GetStrategyClone( value, this.WrappedObject );
                }
            }
        }

        [LocalizedDisplayName( "PropDisp_LoadDrawDataAndStrategyToggle" )]
        [LocalizedDescriptionAttribute( "PropDesc_LoadDrawDataAndStrategyToggle" )] 
        [LocalizedCategory( "PropCate_Visuals" )]
        [Editor( typeof( Atom.Design.ToggleEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public object LoadDrawDataAndStrategyToggle
        {
            get
            {
                return true;
            }

            set
            {
                if( this.WrappedObject.DrawDataAndStrategy != null )
                {
                    this.WrappedObject.DrawDataAndStrategy.Load( DesignTime.Services );
                }
            }
        }

        [LocalizedCategory( "PropCate_Visuals" )]
        [LocalizedDisplayName( "PropDisp_DrawDataAndStrategy" )]
        [LocalizedDescriptionAttribute( "PropDesc_DrawDataAndStrategy" )] 
        public Zelda.Entities.Drawing.IDrawDataAndStrategy DrawDataAndStrategy
        {
            get
            {
                return this.WrappedObject.DrawDataAndStrategy;
            }
        }

        /// <summary>
        /// Returns a clone of this <see cref="UseableActionEntityPropertyWrapper"/>.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new UseableActionEntityPropertyWrapper( serviceProvider );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UseableActionEntityPropertyWrapper"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public UseableActionEntityPropertyWrapper( IZeldaServiceProvider serviceProvider )
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
