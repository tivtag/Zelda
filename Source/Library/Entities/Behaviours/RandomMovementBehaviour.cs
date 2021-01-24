// <copyright file="RandomMovementBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.RandomMovementBehaviour class.
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

    /// <summary>
    /// Defines an <see cref="IEntityBehaviour"/> which
    /// lets the Entity move randomly around the Scene.
    /// </summary>
    public class RandomMovementBehaviour : IEntityBehaviour
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomMovementBehaviour"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceProvider"/> is null.</exception>
        internal RandomMovementBehaviour( IZeldaServiceProvider serviceProvider )
        {
            Contract.Requires<ArgumentNullException>( serviceProvider != null );

            this.serviceProvider = serviceProvider;
            this.rand            = serviceProvider.Rand;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomMovementBehaviour"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity that is controlled by the new RandomMovementBehaviour.
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
        protected RandomMovementBehaviour( ZeldaEntity entity, IZeldaServiceProvider serviceProvider )
            : this( serviceProvider )
        {
            Contract.Requires<ArgumentNullException>( entity != null );

            this.entity = entity;
            this.moveable = entity.Components.Get<Components.Moveable>();

            if( moveable == null )
            {
                throw new ArgumentException(
                    string.Format( 
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.Error_EntityIsRequiredToHaveComponentX,
                        "Moveable"
                    ), 
                    "entity"
               );
            }
        }

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
        /// Gets or sets the minimum value the movementTimeLeft variable may get populated with.
        /// </summary>
        /// <exception cref="ArgumentException"> If the given value is greater than the MovementTimeMaximum. </exception>
        [DefaultValue( 5.0f )]
        public float MovementTimeMinimum
        {
            get 
            {
                return this.movementTimeMinimum;
            }

            set
            {
                if( value > movementTimeMaximum )
                    throw new ArgumentException( Resources.Error_ValueMustBeLessOrEqualMaximum, "value" );

                this.movementTimeMinimum = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum value the movementTimeLeft variable may get populated with.
        /// </summary>
        /// <exception cref="ArgumentException"> If the given value is less than the MovementTimeMinimum. </exception>
        [DefaultValue(25.0f)]
        public float MovementTimeMaximum
        {
            get 
            {
                return this.movementTimeMaximum; 
            }
            
            set
            {
                if( value < this.movementTimeMinimum )
                    throw new ArgumentException( Resources.Error_ValueMustBeGreaterOrEqualMinimum, "value" );

                this.movementTimeMaximum = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the chance in % for the entity to not move at all.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// If the given value is less than zero.
        /// </exception>
        [DefaultValue( 20.0f )]
        public float ChanceToNotMove
        {
            get
            {
                return this.chanceToNotMove;
            }
            
            set
            {
                if( value < 0.0f )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "value" );

                this.chanceToNotMove = value;
            }
        }

        /// <summary>
        /// Gets the direction the entity is currently is moving towards.
        /// </summary>
        protected Direction4 MovementDirection
        {
            get
            {
                return this.movementDirection;
            }
        }

        /// <summary>
        /// Gets the moveable component of the entity.
        /// </summary>
        protected Components.Moveable Moveable
        {
            get
            {
                return this.moveable;
            }
        }
        
        /// <summary>
        /// Gets the entity that is moved by this behaviour.
        /// </summary>
        protected ZeldaEntity Entity
        {
            get
            {
                return this.entity;
            }
        }

        /// <summary>
        /// Gets the <see cref="IZeldaServiceProvider"/> object
        /// which provides fast access to game-related services.
        /// </summary>
        protected IZeldaServiceProvider ServiceProvider
        {
            get
            {
                return this.serviceProvider; 
            }
        }

        /// <summary>
        /// Gets a random number generator.
        /// </summary>
        protected IRand Rand
        {
            get
            {
                return this.rand;
            }
        }

        #endregion

        #region [ Methods ]
        
        /// <summary>
        /// Updates this <see cref="RandomMovementBehaviour"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        public virtual void Update( ZeldaUpdateContext updateContext )
        {
            if( !this.IsActive || ! this.moveable.CanMove )
                return;

            this.movementTimeLeft -= updateContext.FrameTime;

            if( this.movementTimeLeft <= 0.0f )
            {
                this.RandomiseMovementTime();
                this.ChangeMovement();
            }

            this.Move( updateContext );
        }

        /// <summary>
        /// Moves the entity.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected virtual void Move( ZeldaUpdateContext updateContext )
        {
            this.moveable.MoveDir( this.movementDirection, updateContext.FrameTime );
        }

        /// <summary>
        /// Gets called when the Entity collides with the TileMap.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnMapCollisionOccurred( Zelda.Entities.Components.Moveable sender )
        {
            this.ChangeMovement();
        }

        /// <summary>
        /// Fills the movementTimeLeft field with a random value of the interval [movementTimeMin,movementTimeMax].
        /// </summary>
        private void RandomiseMovementTime()
        {
            this.movementTimeLeft = this.rand.RandomRange( this.movementTimeMinimum, this.movementTimeMaximum );
        }

        /// <summary>
        /// Changes the movement direction of the object randomly.
        /// </summary>
        protected virtual void ChangeMovement()
        {
            if( rand.RandomRange( 0.0f, 100.0f ) < this.chanceToNotMove )
            {
                if( this.movementDirection != Direction4.None )
                {
                    this.movementDirection = Direction4.None;
                    return;
                }
            }

            this.movementDirection = rand.RandomActualDirection4But( this.movementDirection );
        }

        #region > State <

        /// <summary>
        /// Called when an entity enters this <see cref="IEntityBehaviour"/>.
        /// </summary>
        public virtual void Enter()
        {
            if( this.IsActive )
                return;

            // Register events:
            moveable.MapCollisionOccurred += this.OnMapCollisionOccurred;
            this.IsActive = true;
        }

        /// <summary>
        /// Called when an entity leaves this <see cref="IEntityBehaviour"/>.
        /// </summary>
        public virtual void Leave()
        {
            if( !IsActive )
                return;

            // Register events:
            moveable.MapCollisionOccurred -= this.OnMapCollisionOccurred;

            this.IsActive = false;
        }

        /// <summary>
        /// Reset this <see cref="IEntityBehaviour"/> to its original state.
        /// </summary>
        public virtual void Reset()
        {
            RandomiseMovementTime();
            ChangeMovement();
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
        public virtual void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.movementTimeMinimum );
            context.Write( this.movementTimeMaximum );
            context.Write( this.chanceToNotMove );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, "RandomMovementBehaviour" );

            this.movementTimeMinimum = context.ReadSingle();
            this.movementTimeMaximum = context.ReadSingle();
            this.chanceToNotMove     = context.ReadSingle();
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Returns a clone of this <see cref="RandomMovementBehaviour"/> for the given <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="newOwner">The owner of the cloned IEntityBehaviour.</param>
        /// <returns>The cloned IEntityBehaviour.</returns>
        public virtual IEntityBehaviour Clone( ZeldaEntity newOwner )
        {
            var clone = new RandomMovementBehaviour( newOwner, serviceProvider );

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the given <see cref="RandomMovementBehaviour"/> to be a clone of this RandomMovementBehaviour.
        /// </summary>
        /// <param name="clone">
        /// The RandomMovementBehaviour to setup as a clone of this RandomMovementBehaviour.
        /// </param>
        public void SetupClone( RandomMovementBehaviour clone )
        {
            clone.movementTimeMinimum = this.movementTimeMinimum;
            clone.movementTimeMaximum = this.movementTimeMaximum;
            clone.chanceToNotMove     = this.chanceToNotMove;
        }

        #endregion

        #endregion

        #region [ Fields ]
        
        /// <summary>
        /// The direction the entity currently moves.
        /// </summary>
        private Direction4 movementDirection;

        /// <summary>
        /// Stores the time left until the object changes direction.
        /// </summary>
        private float movementTimeLeft;

        /// <summary>
        /// The range from which the <see cref="movementTimeLeft"/> variable is populated with.
        /// </summary>
        private float movementTimeMinimum = 5.0f, movementTimeMaximum = 25.0f;

        /// <summary>
        /// The chance for the entity to not move at all.
        /// </summary>
        private float chanceToNotMove = 20.0f;

        /// <summary>
        /// Identifies the Moveable component of the Entity.
        /// </summary>
        private readonly Components.Moveable moveable;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly RandMT rand;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        /// <summary>
        /// The entity that is moved by this RandomMovementBehaviour.
        /// </summary>
        private readonly ZeldaEntity entity;

        #endregion
    }
}
