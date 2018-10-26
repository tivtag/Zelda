// <copyright file="RazorMovementBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.RazorMovementBehaviour class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Behaviours
{
    using System;
    using System.Diagnostics;
    using Atom.Diagnostics.Contracts;
    using Atom;
    using Atom.Events;
    using Atom.Math;
    using Zelda.Events;
    
    /// <summary>
    /// The movement behaviour of a 'Razor' enemy entity is unique.
    /// </summary>
    public sealed class RazorMovementBehaviour : IEntityBehaviour
    {
        #region [ Properties ]

        /// <summary>
        /// Gets a value indicating whether this <see cref="RazorMovementBehaviour"/>
        /// is currently active.
        /// </summary>
        [System.ComponentModel.Browsable( false )]
        public bool IsActive
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the <see cref="RazorBehaviourType"/> of this RazorMovementBehaviour,
        /// which states how the razor entity moves or gets triggered.
        /// </summary>
        public RazorBehaviourType BehaviourType
        {
            get 
            {
                return this.behaviourType;
            }

            set
            {
                if( this.BehaviourType == value )
                    return;

                // Reset old:
                switch( this.behaviourType )
                {
                    case RazorBehaviourType.Triggered:
                        RemoveEvents( this.razor.Scene );
                        break;

                    default:
                        break;
                }

                // Apply new:
                switch( value )
                {
                    case RazorBehaviourType.Triggered:
                        CreateEvents();

                        if( razor.Scene != null )
                            AddEvents( razor.Scene );
                        break;

                    default:
                        break;
                }

                this.behaviourType = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of bounces
        /// that may occur before the razor entity stops to bounce.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Set: If the given value is negative.
        /// </exception>
        public int MaximumBounceCount
        {
            get 
            {
                return this.maximumBounceCount;
            }

            set
            {
                if( value < 0 )
                    throw new ArgumentOutOfRangeException( "value" );

                this.maximumBounceCount = value;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorMovementBehaviour"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal RazorMovementBehaviour( IZeldaServiceProvider serviceProvider )
        {
            Contract.Requires<ArgumentNullException>( serviceProvider != null );

            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorMovementBehaviour"/> class.
        /// </summary>
        /// <param name="enemy">
        /// The entity that is controlled by the new MeleeEnemyBehaviour.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="enemy"/> or <paramref name="serviceProvider"/> is null.
        /// </exception>
        public RazorMovementBehaviour( Enemy enemy, IZeldaServiceProvider serviceProvider )
        {
            Contract.Requires<ArgumentNullException>( enemy != null );
            Contract.Requires<ArgumentNullException>( serviceProvider != null );

            this.razor           = enemy;
            this.serviceProvider = serviceProvider;

            // Register events:
            enemy.Added   += OnRazorAddedToScene;
            enemy.Removed += OnRazorRemovedFromScene;
            enemy.Moveable.MapCollisionOccurred += this.OnEnemyMapCollisionOccurred;
        }

        #endregion

        #region [ Methods ]

        #region > Logic <

        /// <summary>
        /// Updates this RazorMovementBehaviour.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( isMoving )
            {
                FindCollision();
                
                razor.Moveable.MoveDir( direction, updateContext.FrameTime );
            }
        }

        /// <summary>
        /// Tries to detect any collision that occurs between
        /// the razor entity and any other solid entity in the ZeldaScene.
        /// </summary>
        private void FindCollision()
        {
            // collisionTestTick -= frameTime;
            // if( collisionTestTick > 0.0f )
            // {
            //    return;
            // }

            bool invertDir = this.razor.Scene.Contains( 
                (Rectangle)this.razor.Collision.Rectangle,
                ( item ) => {
                    var component = item as Atom.Components.IComponent;

                    if( component != null )
                    {
                        var entity = (ZeldaEntity)component.Owner;

                        return entity != razor &&
                               entity.Collision.IsSolid && 
                               entity.FloorNumber == razor.FloorNumber;
                    }

                    return false;
                }

            );

            if( invertDir )
            {
                // collisionTestTick = collisionTestTickMax;
                this.InvertDirection();
            }
        }

        /// <summary>
        /// Tells this RazorMovementBehaviour to fire
        /// the controlled razor entity.
        /// </summary>
        public void FireRazor()
        {
            if( this.isMoving )
                return;

            this.direction = this.razor.Transform.Direction;
            this.isMoving = true;
            this.bounceCount = 0;

            if( this.behaviourType == RazorBehaviourType.Triggered )
            {
                this.trigger.IsActive = false;
            }
        }

        /// <summary>
        /// Stops the movement of the razor entity.
        /// </summary>
        private void StopRazor()
        {
            this.isMoving = false;

            if( this.behaviourType == RazorBehaviourType.Triggered )
            {
                this.trigger.IsActive = true;
            }
        }

        /// <summary>
        /// Inverses the movement direction of the razor.
        /// </summary>
        private void InvertDirection()
        {
            this.razor.Transform.Direction = this.direction = this.direction.Invert();
        }

        /// <summary>
        /// Changes the movement of the razor.
        /// </summary>
        private void ChangeMovementOnHit()
        {
            ++this.bounceCount;
            this.InvertDirection();

            if( this.BehaviourType == RazorBehaviourType.Triggered )
            {
                if( this.bounceCount >= this.maximumBounceCount )
                {
                    this.StopRazor();
                }
            }
        }
        
        /// <summary>
        /// Gets called when the entity that gets controlled by
        /// this RazorMovementBehaviour has collided with the TileMap.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnEnemyMapCollisionOccurred( Zelda.Entities.Components.Moveable sender )
        {
            this.ChangeMovementOnHit();
        }

        #endregion

        #region > State <

        /// <summary>
        /// Called when the ZeldaEntity that owns this IEntityBehaviour
        /// wishes to enter this RazorMovementBehaviour.
        /// </summary>
        public void Enter()
        {
            if( this.IsActive )
                return;

            this.IsActive = true;
        }

        /// <summary>
        /// Called when the ZeldaEntity that owns this IEntityBehaviour
        /// wishes to leave this RazorMovementBehaviour.
        /// </summary>
        public void Leave()
        {
            if( !this.IsActive )
                return;

            this.IsActive = false;
        }

        /// <summary>
        /// Resets the state of this RazorMovementBehaviour.
        /// </summary>
        public void Reset()
        {
            isMoving    = false;
            bounceCount = 0;
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
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( (int)this.BehaviourType );
            context.Write( this.maximumBounceCount );

            if( this.BehaviourType == RazorBehaviourType.Triggered )
            {
                Debug.Assert( trigger != null );

                var triggerArea = trigger.Area;
                context.Write( triggerArea.X );
                context.Write( triggerArea.Y );
                context.Write( triggerArea.Width );
                context.Write( triggerArea.Height );

                context.Write( trigger.FloorNumber );
            }
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
            // Header
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            // Data
            this.BehaviourType      = (RazorBehaviourType)context.ReadInt32();
            this.maximumBounceCount = context.ReadInt32();

            if( this.BehaviourType == RazorBehaviourType.Triggered )
            {
                Debug.Assert( trigger != null );

                int areaX      = context.ReadInt32();
                int areaY      = context.ReadInt32();
                int areaWidth  = context.ReadInt32();
                int areaHeight = context.ReadInt32();
                trigger.Area = new Rectangle( areaX, areaY, areaWidth, areaHeight );

                trigger.FloorNumber = context.ReadInt32();
            }
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Returns a clone of this RazorMovementBehaviour.
        /// </summary>
        /// <param name="newOwner">
        /// The owner of the cloned IEntityBehaviour.
        /// </param>
        /// <returns>
        /// The cloned IEntityBehaviour.
        /// </returns>
        public IEntityBehaviour Clone( ZeldaEntity newOwner )
        {
            var clone = new RazorMovementBehaviour( (Enemy)newOwner, serviceProvider );

            SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the given <see cref="RazorMovementBehaviour"/> to be a clone of this RazorMovementBehaviour.
        /// </summary>
        /// <param name="clone">The RazorMovementBehaviour to setup as a clone of this RazorMovementBehaviour.</param>
        private void SetupClone( RazorMovementBehaviour clone )
        {
            clone.direction          = direction;
            clone.BehaviourType      = behaviourType;
            clone.maximumBounceCount = maximumBounceCount;
        }

        #endregion

        #region > Organization <

        /// <summary>
        /// Gets called when the razor Enemy entity has been added
        /// to a ZeldaScene.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        private void OnRazorAddedToScene( object sender, ZeldaScene scene )
        {
            if( behaviourType == RazorBehaviourType.Triggered )
                AddEvents( scene );
        }

        /// <summary>
        /// Gets called when the razor Enemy entity has been removed
        /// from a ZeldaScene.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        private void OnRazorRemovedFromScene( object sender, ZeldaScene scene )
        {
            if( behaviourType == RazorBehaviourType.Triggered )
                RemoveEvents( scene );
        }

        /// <summary>
        /// Creates the Event and EventTrigger that are used
        /// in the <see cref="RazorBehaviourType.Triggered"/>.
        /// </summary>
        private void CreateEvents()
        {
            this.triggerEvent = new RazorTriggerEvent( this ) {
                Name = "RazorEvent_" + ZeldaEventManager.GetEventNameExtension()
            };

            this.trigger = new ExternalTileAreaEventTrigger() {
                Name  = "RazorEventTrigger_" + ZeldaEventManager.GetTriggerNameExtension(),
                Event = triggerEvent
            };
        }

        /// <summary>
        /// Adds the event and eventTrigger used by this RazorMovementBehaviour
        /// in the <see cref="RazorBehaviourType.Triggered"/> 
        /// to the given scene.
        /// </summary>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        private void AddEvents( ZeldaScene scene )
        {
            Debug.Assert( this.behaviourType == RazorBehaviourType.Triggered );
            Debug.Assert( scene != null );
            Debug.Assert( triggerEvent != null );
            Debug.Assert( trigger != null );

            scene.EventManager.Add( this.triggerEvent );
            scene.EventManager.Add( this.trigger      );
        }

        /// <summary>
        /// Removes the event and eventTrigger used by this RazorMovementBehaviour
        /// in the <see cref="RazorBehaviourType.Triggered"/> from the given ZeldaScene.
        /// </summary>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        private void RemoveEvents( ZeldaScene scene )
        {
            Debug.Assert( this.behaviourType == RazorBehaviourType.Triggered );

            if( scene != null )
            {
                if( triggerEvent != null )
                {
                    scene.EventManager.RemoveEvent( triggerEvent.Name );
                    triggerEvent = null;
                }

                if( trigger != null )
                {
                    scene.EventManager.RemoveTrigger( trigger.Name );
                    trigger = null;
                }
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// States the maximum number of bounces if in Triggered Mode.
        /// </summary>
        private int maximumBounceCount;

        /// <summary>
        /// States the direction the razor entity starts moving once triggered.
        /// </summary>
        private Direction4 direction;

        /// <summary>
        /// States the <see cref="RazorBehaviourType"/> this RazorMovementBehaviour uses.
        /// </summary>
        private RazorBehaviourType behaviourType = RazorBehaviourType.Always;

        /// <summary>
        /// Indicates whether this RazorMovementBehaviour makes the razor entity to move currently.
        /// </summary>
        private bool isMoving;

        /// <summary>
        /// The number of times the razor entity changed direction since it was fired.
        /// </summary>
        private int bounceCount;

        /// <summary>
        /// The TileAreaEventTrigger that triggers that is used to trigger
        /// this RazorMovementBehaviour.
        /// Only used in <see cref="RazorBehaviourType.Triggered"/> mode.
        /// </summary>
        private ExternalTileAreaEventTrigger trigger;

        /// <summary>
        /// The RazorTriggerEvent that fires this RazorMovementBehaviour when triggered.
        /// Only used in <see cref="RazorBehaviourType.Triggered"/> mode.
        /// </summary>
        private RazorTriggerEvent triggerEvent;

        /// <summary>
        /// The Enemy entity that is controlled by this RazorMovementBehaviour.
        /// </summary>
        private readonly Enemy razor;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        #endregion

        #region enum RazorBehaviourType

        /// <summary>
        /// States the different movement behaviours supported by the <see cref="RazorMovementBehaviour"/>.
        /// </summary>
        public enum RazorBehaviourType
        {
            /// <summary>
            /// No specific movement behaviour has been set.
            /// </summary>
            None = 0,

            /// <summary>
            /// The razor is always bouncing back and fourth.
            /// </summary>
            Always,

            /// <summary>
            /// The razor bounces once triggered, then waits until it's triggered again.
            /// </summary>
            Triggered,

            /// <summary>
            /// The razor bounces once triggered, then waits until it's manually triggered again.
            /// </summary>
            ManuallyTriggered
        }

        #endregion 

        #region class RazorTriggerEvent

        /// <summary>
        /// Defines the <see cref="Event"/> that gets triggered
        /// for a Enemy entity that uses the <see cref="RazorMovementBehaviour"/>
        /// that is sets to have a <see cref="RazorBehaviourType"/> of <see cref="RazorBehaviourType.Triggered"/>.
        /// </summary>
        private sealed class RazorTriggerEvent : ExternalEvent
        {
            /// <summary>
            /// Initializes a new instance of the RazorTriggerEvent class.
            /// </summary>
            /// <param name="razorBehaviour">
            /// The related RazorMovementBehaviour.
            /// </param>
            public RazorTriggerEvent( RazorMovementBehaviour razorBehaviour )
            {
                Debug.Assert( razorBehaviour != null );
                this.razorBehaviour = razorBehaviour;
            }

            /// <summary>
            /// Triggers this RazorTriggerEvent.
            /// </summary>
            /// <param name="obj">
            /// The object that has triggered the RazorTriggerEvent.
            /// </param>
            public override void Trigger( object obj )
            {
                razorBehaviour.FireRazor();
            }

            /// <summary>
            /// Identifies the related RazorMovementBehaviour.
            /// </summary>
            private readonly RazorMovementBehaviour razorBehaviour;
        }

        #endregion
    }
}
