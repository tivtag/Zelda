// <copyright file="Visionable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Components.Visionable class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Components
{
    using System;
    using Atom.Diagnostics.Contracts;
    using Atom;
    using Atom.Components;
    using Atom.Math;

    /// <summary>
    /// Defines the <see cref="Component"/> that enables a <see cref="ZeldaEntity"/> 
    /// to have a vision of the world. This class can't be inherited.
    /// </summary>
    public sealed class Visionable : ZeldaComponent
    {
        #region [ Initialization ]

        /// <summary>
        /// Called when an IComponent has been removed or added to the <see cref="IEntity"/> that owns this IComponent.
        /// </summary>
        public override void InitializeBindings()
        {
            this.moveable = this.Owner.Components.Get<Moveable>();
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the range of the circular vision field.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Set: If the given value is negative.
        /// </exception>
        public int VisionRange
        {
            get
            {
                return this.visionRange; 
            }

            set 
            {
                if( value < 0 )
                    throw new ArgumentOutOfRangeException( "value" );

                visionRange = value;
            }
        }

        /// <summary>
        /// Gets or sets the range of the circular feeling field.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Set: If the given value is negative.
        /// </exception>
        public int FeelingRange
        {
            get
            {
                return this.feelingRange; 
            }
            
            set
            {
                if( value < 0 )
                    throw new ArgumentOutOfRangeException( "value" );

                this.feelingRange = value; 
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets whether the specified <see cref="ZeldaEntity"/> is within 
        /// the circular vision of this <see cref="Visionable"/> entity.
        /// </summary>
        /// <param name="entity">
        /// The entity to test against.
        /// </param>
        /// <returns>
        /// true if the given <paramref name="entity"/> is within 
        /// the circular vision field of this <see cref="Visionable"/> entity.
        /// </returns>
        public bool IsWithinCircularVision( ZeldaEntity entity )
        {
            Contract.Requires<ArgumentNullException>( entity != null );

            if( this.Owner.FloorNumber != entity.FloorNumber )
                return false;

            Circle circle = new Circle(
                this.Owner.Collision.Center,
                visionRange
            );

            return circle.Contains( entity.Collision.Center );
        }

        /// <summary>
        /// Gets whether the specified <see cref="ZeldaEntity"/> is within 
        /// the vision of this <see cref="Visionable"/> entity.
        /// </summary>
        /// <param name="entity">
        /// The entity to test against.
        /// </param>
        /// <returns>
        /// true if the given <paramref name="entity"/> is within 
        /// the circular vision field of this <see cref="Visionable"/> entity.
        /// </returns>
        public bool IsWithinVision( ZeldaEntity entity )
        {
            Contract.Requires<ArgumentNullException>( entity != null );

            if( this.Owner.FloorNumber != entity.FloorNumber )
                return false;

            Vector2 thisCenter  = this.Owner.Collision.Center;
            Vector2 otherCenter = entity.Collision.Center;

            // Test Vision Range first:
            Circle circle = new Circle (
                thisCenter,
                visionRange
            );

            if( !circle.Contains( otherCenter ) )
                return false;

            // Now test Feeling Range:
            circle.Radius = feelingRange;
            if( circle.Contains( otherCenter ) )
                return true;

            var actionLayer = this.Owner.ActionLayer;
            if( actionLayer == null )
                return false;

            int objTileX  = (int)otherCenter.X  / 16;
            int objTileY  = (int)otherCenter.Y  / 16;
            int thisTileX = (int)thisCenter.X / 16;
            int thisTileY = (int)thisCenter.Y / 16;

            // Now test whether this visionable Entity
            // actually looks into the direction of other entity
            bool sameX = objTileX == thisTileX;
            bool sameY = objTileY == thisTileY;

            if( !(sameX || sameY) )
                return false;

            Direction4 thisDir = this.Owner.Transform.Direction;
            int moveX = 0, moveY = 0;

            if( sameX )
            {
                if( objTileY > thisTileY )
                {
                    if( thisDir != Direction4.Down )
                        return false;
                    moveY = 1; // Direction4.Down;
                }
                else
                {
                    if( thisDir != Direction4.Up )
                        return false;
                    moveY = -1; // Direction4.Up;
                }
            }
            else
            {
                if( objTileX > thisTileX )
                {
                    if( thisDir != Direction4.Right )
                        return false;
                    moveX = 1; // Direction4.Right;
                }
                else
                {
                    if( thisDir != Direction4.Left )
                        return false;
                    moveX = -1;  // Direction4.Left;
                }
            }

            int visionRangeTile = this.visionRange / 16;

            // Now test whether the other entity stands in a straight line:
            for( int x = thisTileX, y = thisTileY, i = 0;
                 i < visionRangeTile;
                 ++i, x += moveX, y += moveY )
            {
                if( moveable.TileHandler.IsWalkable( actionLayer.GetTileAt( x, y ), moveable ) == false )
                    return false;

                if( x == objTileX && y == objTileY )
                    return true;
            }

            return false;
        }

        #region > Cloning <

        /// <summary>
        /// Setups the given Visionable component
        /// to be a clone of this Visionable component.
        /// </summary>
        /// <param name="clone">
        /// The Visionable component to setup as a clone of this Visionable component.
        /// </param>
        public void SetupClone( Visionable clone )
        {
            clone.feelingRange = this.feelingRange;
            clone.visionRange  = this.visionRange;
        }

        #endregion

        #region > Storage <

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int Version = 1;
            context.Write( Version );

            context.Write( this.visionRange );
            context.Write( this.feelingRange );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int Version = 1;
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, Version, this.GetType() );

            this.visionRange  = context.ReadInt32();
            this.feelingRange = context.ReadInt32();
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The range of the circular vision field.
        /// </summary>
        private int visionRange = 6 * 16;

        /// <summary>
        /// The range of the circular feeling field.
        /// </summary>  
        private int feelingRange = 3 * 16;

        /// <summary>
        /// Identifies the <see cref="Moveable"/> component of the <see cref="Visionable"/> entity.
        /// </summary>
        private Moveable moveable;

        #endregion
    }
}
