// <copyright file="Transformable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Components.ZeldaTransform class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Components
{
    using System.ComponentModel;
    using Atom.Components.Transform;
    using Atom.Math;

    /// <summary>
    /// Defines the component that represents 
    /// the transformation of the entity that owns the <see cref="ZeldaTransform"/> component.
    /// This class can't be inherited.
    /// </summary>
    public sealed class ZeldaTransform : HierarchicalTransform2
    {
        #region [ Events ]

        /// <summary>
        /// Fired when the <see cref="Direction"/> property of this <see cref="ZeldaTransform"/> has changed.
        /// </summary>
        public event Atom.RelaxedEventHandler<Atom.ChangedValue<Direction4>> DirectionChanged;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the direction the Entity is facing.
        /// </summary>
        /// <value>The default value is <see cref="Direction4.Down"/>.</value>
        [DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
        public Direction4 Direction
        {
            get
            {
                return this.direction;
            }

            set
            {
                if( value == this.direction )
                    return;

                var oldValue = direction;
                this.direction = value;

                if( this.DirectionChanged != null )
                    this.DirectionChanged( this, new Atom.ChangedValue<Direction4>( oldValue, value ) );
            }
        }

        /// <summary>
        /// Gets the direction the Entity was facing last frame.
        /// </summary>
        [DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
        public Direction4 OldDirection
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the position of the transformable ZeldaEntity in tile-space.
        /// </summary>
        public Point2 PositionTile
        {
            get
            {
                var position = this.Position;

                Point2 tile;
                tile.X = (int)((position.X + 0.5f) / 16);
                tile.Y = (int)((position.Y + 0.5f) / 16);

                return tile;
            }
        }
        
        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this <see cref="ZeldaTransform"/> component.
        /// </summary>
        /// <param name="updateContext">
        /// The current Atom.IUpdateContext.
        /// </param>
        public override void Update( Atom.IUpdateContext updateContext )
        {
            this.OldDirection = this.Direction;
             
            // Not required to be called, Component.Update does nothing!
            // base.Update( updateContext );
        }        
        
        /// <summary>
        /// Returns whether the ZeldaEntity with the given <see cref="ZeldaTransform"/>
        /// is facing the ZeldaEntity with this <see cref="ZeldaTransform"/>.
        /// </summary>
        /// <param name="other">
        /// The other ZeldaTransform.
        /// </param>
        /// <returns>
        /// Returns true if the given ZeldaTransform is facing this ZeldaTransform;
        /// otherwise false.
        /// </returns>
        public bool IsFacing( ZeldaTransform other )
        {
            var delta = other.Position - this.Position;
            var direction = delta.ToDirection4();

            return direction.Invert() == other.Direction;
        }

        /// <summary>
        /// Returns whether this transformable ZeldaEntity is facing the specified location
        /// </summary>
        /// <param name="location">
        /// The location to check.
        /// </param>
        /// <returns>
        /// Returns true if this transformable ZeldaEntity is facing
        /// the specified location;
        /// otherwise false.
        /// </returns>
        public bool IsFacing( Vector2 location )
        {
            var delta = location - this.Position;
            var direction = delta.ToDirection4();
            var expectedDirection = direction.Invert();

            return expectedDirection == this.Direction;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Represents the storage field for the <see cref="Direction"/> property.
        /// </summary>
        private Direction4 direction = Direction4.Down;

        #endregion
    }
}
