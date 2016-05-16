// <copyright file="BlockTriggerPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines Zelda.Editor.Object.PropertyWrappers.BlockTriggerPropertyWrapper class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Object.PropertyWrappers
{
    using Atom.Math;
    using Zelda.Editor.Design;
    using Zelda.Entities;

    /// <summary>
    /// Defines the IObjectPropertyWrapper for the <see cref="BlockTrigger"/> type.
    /// </summary>
    internal sealed class BlockTriggerPropertyWrapper : EntityPropertyWrapper<BlockTrigger>
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

        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_BlockTrigger_Event" )]
        [System.ComponentModel.Editor( typeof( Atom.Events.Design.EventCreationEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Atom.Events.Event Event
        {
            get { return this.WrappedObject.Event; }
            set { this.WrappedObject.Event = value; }
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
        
        #endregion

        /// <summary>
        /// Returns a clone of this <see cref="IObjectPropertyWrapper"/>.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new BlockTriggerPropertyWrapper( serviceProvider );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockTriggerPropertyWrapper"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public BlockTriggerPropertyWrapper( IZeldaServiceProvider serviceProvider )
        {
            System.Diagnostics.Debug.Assert( serviceProvider != null );
            this.serviceProvider = serviceProvider;

            this.drawStrategyWrapper = new DrawStrategyEntityPropertyWrapper( this );
        }
        
        private readonly DrawStrategyEntityPropertyWrapper drawStrategyWrapper;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;
    }
}
