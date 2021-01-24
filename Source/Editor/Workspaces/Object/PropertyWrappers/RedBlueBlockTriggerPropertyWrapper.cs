// <copyright file="RedBlueBlockTriggerPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines Zelda.Editor.Object.PropertyWrappers.RedBlueBlockTriggerPropertyWrapper class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Object.PropertyWrappers
{
    using Atom.Math;
    using Zelda.Entities;

    /// <summary>
    /// Defines the IObjectPropertyWrapper for the <see cref="RedBlueBlockTrigger"/> type.
    /// </summary>
    internal sealed class RedBlueBlockTriggerPropertyWrapper : EntityPropertyWrapper<RedBlueBlockTrigger>
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

        [LocalizedDisplayName( "PropDisp_SwitchEvent" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_RedBlueBlockTrigger_SwitchEvent" )]
        [System.ComponentModel.Editor( typeof( Atom.Events.Design.EventCreationEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Atom.Events.Event SwitchEvent
        {
            get { return this.WrappedObject.SwitchEvent; }
            set { this.WrappedObject.SwitchEvent = (Atom.Events.DualSwitchEvent)value; }
        }

        [LocalizedDisplayName( "PropDisp_SpriteOn" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_RedBlueBlockTrigger_SpriteOn" )]
        [System.ComponentModel.Editor( typeof( Zelda.Graphics.Design.SpriteEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Atom.Xna.Sprite SpriteOn
        {
            get
            {
                return this.WrappedObject.SpriteOn;
            }

            set
            {
                this.WrappedObject.SpriteOn = value;
            }
        }

        [LocalizedDisplayName( "PropDisp_SpriteOff" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_RedBlueBlockTrigger_SpriteOff" )]
        [System.ComponentModel.Editor( typeof( Zelda.Graphics.Design.SpriteEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Atom.Xna.Sprite SpriteOff
        {
            get
            {
                return this.WrappedObject.SpriteOff;
            }

            set
            {            
                this.WrappedObject.SpriteOff = value;
            }
        }

        #endregion

        /// <summary>
        /// Returns a clone of this <see cref="IObjectPropertyWrapper"/>.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new RedBlueBlockTriggerPropertyWrapper( serviceProvider );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlueBlockTriggerPropertyWrapper"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public RedBlueBlockTriggerPropertyWrapper( IZeldaServiceProvider serviceProvider )
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
