// <copyright file="Collideable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Components.ZeldaCollision class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Components
{
    using Atom.Components;
    using Atom.Components.Collision;
    using Atom.Math;

    /// <summary>
    /// Defines the <see cref="Component"/> that is responsible 
    /// for descriping the collision area of the <see cref="Entity"/>.
    /// </summary>
    public sealed class ZeldaCollision : StaticCollision2
    {
        #region [ Properties ]
        
        /// <summary>
        /// Gets or sets a value indicating whether the ZeldaEntity is a solid object.
        /// </summary>
        /// <value>
        /// The default value is true.
        /// </value>
        public bool IsSolid
        {
            get 
            {
                return this.isSolid;
            }

            set
            {
                this.isSolid = value;
            }
        }

        #endregion

        #region [ Storage ]

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 2;
            context.Write( CurrentVersion );

            context.Write( this.Offset );
            context.Write( this.Size );
            context.Write( this.isSolid );
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
            const int CurrentVersion = 2;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, this.GetType() );

            Vector2 offset = context.ReadVector2();
            Vector2 size = context.ReadVector2();            
            this.Set( offset, size );

            if( version >= 2 )
            {
                this.isSolid = context.ReadBoolean();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Returns whether the <see cref="Collision2.Rectangle"/> of the given <see cref="ZeldaCollision"/> component
        /// intersects with the <see cref="Collision2.Rectangle"/> of this <see cref="ZeldaCollision"/> component.
        /// </summary>
        /// <remarks>
        /// This method provides an unstrict intersection test;
        /// as such the collision rectangles are considered to intersect even with a small margin of error.
        /// </remarks>
        /// <param name="collision">
        /// The collision component to test against.
        /// </param>
        /// <returns>
        /// true if the rectangles intersect;
        /// otherwise false.
        /// </returns>
        public bool IntersectsUnstrict( ZeldaCollision collision )
        {
            var rectangle = collision.Rectangle;
            
            // Modify this value to fit ya needs~
            rectangle.X      -= 2.6f;
            rectangle.Y      -= 2.5f;
            rectangle.Width  += 5.5f;
            rectangle.Height += 6.5f;

            return this.Intersects( ref rectangle );
        }

        /// <summary>
        /// Returns whether the <see cref="Collision2.Rectangle"/> of the given <see cref="ZeldaCollision"/> component
        /// intersects with the <see cref="Collision2.Rectangle"/> of this <see cref="ZeldaCollision"/> component.
        /// </summary>
        /// <remarks>
        /// This method provides an unstrict intersection test;
        /// as such the collision rectangles are considered to intersect even with a small margin of error.
        /// </remarks>
        /// <param name="collision">
        /// The collision component to test against.
        /// </param>
        /// <returns>
        /// true if the rectangles intersect;
        /// otherwise false.
        /// </returns>
        public bool IntersectsUnstrict2px( ZeldaCollision collision )
        {
            var rectangle = collision.Rectangle;

            // Modify this value to fit ya needs~
            rectangle.X      -= 1.0f;
            rectangle.Y      -= 1.0f;
            rectangle.Width  += 2.0f;
            rectangle.Height += 2.0f;

            return this.Intersects( ref rectangle );
        }

        /// <summary>
        /// Returns whether the <see cref="Collision2.Rectangle"/> of the given <see cref="ZeldaCollision"/> component
        /// intersects with the <see cref="Collision2.Rectangle"/> of this <see cref="ZeldaCollision"/> component.
        /// </summary>
        /// <param name="collision">
        /// The collision component to test against.
        /// </param>
        /// <param name="tolerance">
        /// The tolerance to allow.
        /// </param>
        /// <returns>
        /// true if the rectangles intersect;
        /// otherwise false.
        /// </returns>
        public bool IntersectsUnstrict( ZeldaCollision collision, float tolerance )
        {
            var rectangle = collision.Rectangle;
            float halfTolerance = tolerance / 2.0f;

            // Modify this value to fit ya needs~
            rectangle.X      -= halfTolerance;
            rectangle.Y      -= halfTolerance;
            rectangle.Width  += tolerance;
            rectangle.Height += tolerance;

            return this.Intersects( ref rectangle );
        }

        /// <summary>
        /// Returns whether the <see cref="Collision2.Rectangle"/> of the given <see cref="ZeldaCollision"/> component
        /// intersects with the given Circle.
        /// </summary>
        /// <param name="circle">
        /// The circle to test against.
        /// </param>
        /// <returns>
        /// true if the circle intersects;
        /// otherwise false.
        /// </returns>
        internal bool Intersects( ref Atom.Math.Circle circle )
        {
            return this.Rectangle.Intersects( circle );
        }

        /// <summary>
        /// Setups the given ZeldaCollision component to be a clone of this ZeldaCollision.
        /// </summary>
        /// <param name="clone">
        /// The ZeldaCollision component to setup as a clone of this ZeldaCollision.
        /// </param>
        public void SetupClone( ZeldaCollision clone )
        {
            clone.isSolid = this.isSolid;
            clone.Set( this.Offset, this.Size );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the storage field of the IsSolid property.
        /// </summary>
        private bool isSolid = true;

        #endregion
    }
}
