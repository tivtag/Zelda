// <copyright file="TravellingSalesmanBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.TravellingSalesmanBehaviour class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Behaviours
{
    using System;
    using System.ComponentModel;
    using Zelda.Saving;
    using Zelda.Waypoints;

    /// <summary>
    /// I
    /// </summary>
    public sealed class TravellingSalesmanBehaviour : IEntityBehaviour
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="IEntityBehaviour"/> is currently active.
        /// </summary>
        public bool IsActive
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the route the salesman travels on.
        /// </summary>
        [Editor( typeof( Zelda.Waypoints.Design.ZeldaPathEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public ZeldaPath Route
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether when the entity has reached the end of the
        /// path it should start over at the first Waypoint.
        /// </summary>
        public bool IsLooping
        {
            get
            {
                return this.waypointPathFollower.IsLooping;
            }

            set
            {
                this.waypointPathFollower.IsLooping = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the TravellingSalesmanBehaviour class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal TravellingSalesmanBehaviour( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Initializes a new instance of the TravellingSalesmanBehaviour class.
        /// </summary>
        /// <param name="entity">
        /// The entity that is controlled by the new TravellingSalesmanBehaviour.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        private TravellingSalesmanBehaviour( ZeldaEntity entity, IZeldaServiceProvider serviceProvider )
            : this( serviceProvider )
        {
            if( entity != null )
            {
                this.entity = entity as IMoveableEntity;

                if( this.entity == null )
                {
                    throw new ArgumentException( "The entity is required to be an IMoveableEntity." );
                }

                this.turnToPlayerBehaviour = new TurnToPlayerOnSightBehaviour( entity, serviceProvider.Rand );
                this.turnToPlayerBehaviour.InVisionChanged += this.OnSalesmanSeesPlayerStateChanged;
            }
        }

        /// <summary>
        /// Updates this TravellingSalesmanBehaviour.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( !this.IsActive )
                return;

            this.turnToPlayerBehaviour.Update( updateContext );
            this.waypointPathFollower.Update( updateContext );
        }

        /// <summary>
        /// Called when an entity enters this <see cref="IEntityBehaviour"/>.
        /// </summary>
        public void Enter()
        {
            if( this.IsActive )
                return;

            this.IsActive = true;
            this.waypointPathFollower.Setup( this.entity, this.Route );
            this.turnToPlayerBehaviour.Enter();
        }

        /// <summary>
        /// Called when an entity leaves this <see cref="IEntityBehaviour"/>.
        /// </summary>
        /// <exception cref="System.InvalidOperationException"> 
        /// If this <see cref="IEntityBehaviour"/> is currently not active.
        /// </exception>
        public void Leave()
        {
            if( !this.IsActive )
            {
                throw new System.InvalidOperationException();
            }

            this.IsActive = false;
            this.turnToPlayerBehaviour.Leave();
        }

        /// <summary>
        /// Reset this <see cref="IEntityBehaviour"/> to its original state.
        /// </summary>
        public void Reset()
        {
            this.turnToPlayerBehaviour.Reset();
            this.waypointPathFollower.IsActive = true;
        }

        /// <summary>
        /// Called when the salesman entity sees or doesn't see the player anymore.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The EventArgs that contain the event data.
        /// </param>
        private void OnSalesmanSeesPlayerStateChanged( object sender, EventArgs e )
        {
            if( this.turnToPlayerBehaviour.InVision )
            {
                this.waypointPathFollower.IsActive = false;
            }
            else
            {
                this.waypointPathFollower.IsActive = true;
            }
        }

        /// <summary>
        /// Returns a clone of this <see cref="IEntityBehaviour"/> for the given <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="newOwner">
        /// The owner of the cloned IEntityBehaviour.
        /// </param>
        /// <returns>
        /// The cloned IEntityBehaviour.
        /// </returns>
        public IEntityBehaviour Clone( ZeldaEntity newOwner )
        {
            var clone = new TravellingSalesmanBehaviour( newOwner, this.serviceProvider );

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the specified TravellingSalesmanBehaviour to be a clone of this TravellingSalesmanBehaviour.
        /// </summary>
        /// <param name="clone">
        /// The TravellingSalesmanBehaviour to setup as a clone of this TravellingSalesmanBehaviour.
        /// </param>
        private void SetupClone( TravellingSalesmanBehaviour clone )
        {
            clone.Route = this.Route;
            clone.IsLooping = this.IsLooping;
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Saving.IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();

            context.Write( this.Route != null ? this.Route.Name : string.Empty );
            context.Write( this.IsLooping );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Saving.IZeldaDeserializationContext context )
        {
            var sceneContext = (ISceneDeserializationContext)context;
            context.ReadDefaultHeader( this.GetType() );
            
            string routeName = context.ReadString();

            if( routeName.Length > 0 )
            {
                this.Route = sceneContext.Scene.WaypointMap.GetPath( routeName );
            }

            this.IsLooping = context.ReadBoolean();
        }

        /// <summary>
        /// Responsible for following the Route.
        /// </summary>
        private readonly WaypointPathFollower waypointPathFollower = new WaypointPathFollower();

        /// <summary>
        /// The sub behaviour of this TravellingSalesmanBehaviour that makes the salesman stop and turn
        /// to the player.
        /// </summary>
        private readonly TurnToPlayerOnSightBehaviour turnToPlayerBehaviour;

        /// <summary>
        /// Represents the entity that is controlled by this TravellingSalesmanBehaviour.
        /// </summary>
        private readonly IMoveableEntity entity;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;
    }
}
