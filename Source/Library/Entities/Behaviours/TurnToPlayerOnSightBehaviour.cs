// <copyright file="TurnToPlayerOnSightBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.TurnToPlayerOnSightBehaviour class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Behaviours
{
    using System;
    using System.ComponentModel;
    using Atom.Diagnostics.Contracts;
    using Atom;
    using Atom.Math;
    
    /// <summary>
    /// Implements a simple <see cref="IEntityBehaviour"/>
    /// that turns the entity (using the Transform.Direction) towards the player.
    /// </summary>
    /// <remarks>
    /// The controlled entity is required to have 
    /// the <see cref="Components.Visionable"/> component.
    /// </remarks>
    public class TurnToPlayerOnSightBehaviour : IEntityBehaviour
    {
        #region [ Constants ]
        
        /// <summary>
        /// The time between two vision checks.
        /// </summary>
        private static readonly FloatRange CheckTime = new FloatRange( 5.0f, 7.0f );

        /// <summary>
        /// The time between two vision checks; when the player was in vision.
        /// </summary>
        private static readonly FloatRange CheckTimeInVision = new FloatRange( 0.2f, 1.0f );

        #endregion

        #region [ Events ]

        /// <summary>
        /// Raised when the <see cref="InVision"/> property has changed.
        /// </summary>
        public event EventHandler InVisionChanged;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets a value indicating whether this <see cref="IEntityBehaviour"/> is currently active.
        /// </summary>
        /// <value>The default value is false.</value>
        [Browsable(false)]
        public bool IsActive
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the player is currently in vision of the entity. 
        /// </summary>
        [Browsable( false )]
        public bool InVision 
        {
            get
            {
                return this._inVision;
            }

            private set
            {
                if( value == this._inVision )
                    return;

                this._inVision = value;
                this.InVisionChanged.Raise( this );
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TurnToPlayerOnSightBehaviour"/> class.
        /// </summary>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        internal TurnToPlayerOnSightBehaviour( RandMT rand )
        {
            Contract.Requires<ArgumentNullException>( rand != null );

            this.rand = rand;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TurnToPlayerOnSightBehaviour"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity that is controlled by the new TurnToPlayerOnSightBehaviour.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the given <paramref name="entity"/> doesn't own the <see cref="Components.Visionable"/> component.
        /// </exception>
        public TurnToPlayerOnSightBehaviour( ZeldaEntity entity, RandMT rand )
        {
            Contract.Requires<ArgumentNullException>( entity != null );
            Contract.Requires<ArgumentNullException>( rand != null );

            this.rand = rand;
            this.entity     = entity;
            this.visionable = entity.Components.Get<Components.Visionable>();

            if( visionable == null )
            {
                throw new ArgumentException(
                    string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.Error_EntityIsRequiredToHaveComponentX,
                        "Visionable"
                    ),
                    "entity"
               );
            }
        }

        #endregion

        #region [ Methods ]
        
        /// <summary>
        /// Updates this <see cref="TurnToPlayerOnSightBehaviour"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        public virtual void Update( ZeldaUpdateContext updateContext )
        {
            var scene = this.entity.Scene;
            if( !this.IsActive || scene == null )
                return;

            this.timeLeft -= updateContext.FrameTime;

            if( this.timeLeft <= 0.0f )
            {
                var player = scene.Player;
                if( player == null )
                    return;

                this.InVision = this.visionable.IsWithinCircularVision( player );

                if( this.InVision )
                {
                    this.TurnTo( player );

                    this.wasInVision = true;
                    this.timeLeft = CheckTimeInVision.GetRandomValue( this.rand );
                }
                else
                {
                    if( wasInVision )
                    {
                        this.entity.Transform.Direction = initialDirection;
                        this.wasInVision = false;
                    }

                    this.timeLeft = CheckTime.GetRandomValue( this.rand );
                }
            }
        }

        /// <summary>
        /// Turns the entity towards the specified PlayerEntity.
        /// </summary>
        /// <param name="player">
        /// The player to turn to.
        /// </param>
        private void TurnTo( PlayerEntity player )
        {
            var delta        = player.Collision.Center - entity.Collision.Center;
            var newDirection = delta.ToDirection4();
            this.entity.Transform.Direction = newDirection;
        }

        #region > State <

        /// <summary>
        /// Called when an entity enters this <see cref="IEntityBehaviour"/>.
        /// </summary>
        public virtual void Enter()
        {
            this.IsActive         = true;
            this.initialDirection = entity.Transform.Direction;
        }

        /// <summary>
        /// Called when an entity leaves this <see cref="IEntityBehaviour"/>.
        /// </summary>
        public virtual void Leave()
        {
            this.IsActive = false;
        }

        /// <summary>
        /// Reset this <see cref="IEntityBehaviour"/> to its original state.
        /// </summary>
        public virtual void Reset()
        {
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
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Returns a clone of this <see cref="TurnToPlayerOnSightBehaviour"/> for the given <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="newOwner">The owner of the cloned IEntityBehaviour.</param>
        /// <returns>The cloned IEntityBehaviour.</returns>
        public virtual IEntityBehaviour Clone( ZeldaEntity newOwner )
        {
            return new TurnToPlayerOnSightBehaviour( newOwner, this.rand );
        }

        #endregion

        #endregion

        #region [ Fields ]
        
        /// <summary>
        /// The direction the entity initialy turned into.
        /// </summary>
        private Direction4 initialDirection;

        /// <summary>
        /// States whether the player was seen by the controlled entity last frame.
        /// </summary>
        private bool wasInVision;

        /// <summary>
        /// The time left until an vision check is done again.
        /// </summary>
        private float timeLeft;

        /// <summary>
        /// The entity controlled by this TurnToPlayerOnSightBehaviour.
        /// </summary>
        private readonly ZeldaEntity entity;

        /// <summary>
        /// Identifies the Visionable component of the Entity.
        /// </summary>
        private readonly Components.Visionable visionable;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly RandMT rand;

        /// <summary>
        /// Represents the storage field of the <see cref="InVision"/> property.
        /// </summary>
        private bool _inVision;

        #endregion
    }
}
