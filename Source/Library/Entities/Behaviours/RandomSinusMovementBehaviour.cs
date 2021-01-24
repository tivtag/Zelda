// <copyright file="RandomSinusMovementBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.RandomSinusMovementBehaviour class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Behaviours
{
    using System;
    using System.ComponentModel;
    using Atom.Diagnostics.Contracts;
    using Atom.Math;
    using Zelda.Saving;

    /// <summary>
    /// Defines an <see cref="IEntityBehaviour"/> which
    /// </summary>
    public class RandomSinusMovementBehaviour : RandomMovementBehaviour
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSinusMovementBehaviour"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceProvider"/> is null.</exception>
        internal RandomSinusMovementBehaviour( IZeldaServiceProvider serviceProvider )
            : base( serviceProvider )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSinusMovementBehaviour"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity that is controlled by the new RandomSinusMovementBehaviour.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> or <paramref name="serviceProvider"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the given <paramref name="entity"/> doesn't own the <see cref="Components.Moveable"/> component.
        /// </exception>
        protected RandomSinusMovementBehaviour( ZeldaEntity entity, IZeldaServiceProvider serviceProvider )
            : base( entity, serviceProvider )
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void ChangeMovement()
        {
            base.ChangeMovement();

            this.start = this.Entity.Transform.Position;
            this.end = this.start + this.MovementDirection.ToVector() * this.waveLength.GetRandomValue( this.Rand );
            this.wavePosition = 0.0f;

            this.delta = end - start;
            this.normal = delta.Perpendicular;
            this.waveSpeed = this.waveSpeedRange.GetRandomValue( this.Rand );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateContext"></param>
        protected override void Move( ZeldaUpdateContext updateContext )
        {
            float wabble = (float)(Math.Sin( this.wavePosition ) * Math.Cos( this.wavePosition / 3.0f ));
            Vector2 wavePoint = start + delta * wavePosition + normal * wabble;

            this.Moveable.MoveTo( wavePoint, updateContext.FrameTime );

            this.wavePosition += (this.waveSpeed * updateContext.FrameTime);
        }

        /// <summary>
        /// Returns a clone of this <see cref="RandomSinusMovementBehaviour"/>
        /// for the given ZeldaEntity.
        /// </summary>
        /// <param name="newOwner">The owner of the clone to create.</param>
        /// <returns>The cloned IEntityBehaviour.</returns>
        public override IEntityBehaviour Clone( ZeldaEntity newOwner )
        {
            var clone = new RandomSinusMovementBehaviour( (Enemy)newOwner, this.ServiceProvider );

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            base.Serialize( context );

            context.WriteDefaultHeader();
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            base.Deserialize( context );

            context.ReadDefaultHeader( this.GetType() );
        }

        private Vector2 start, end;
        private Vector2 normal;
        private Vector2 delta;
        private float waveSpeed;

        private FloatRange waveSpeedRange = new FloatRange( 0.5f, 2.0f );
        private FloatRange waveLength = new FloatRange( 6.0f, 32.0f );
        private float wavePosition;
    }
}
