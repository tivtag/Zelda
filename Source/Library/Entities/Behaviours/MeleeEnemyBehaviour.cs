// <copyright file="MeleeEnemyBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.MeleeEnemyBehaviour class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities.Behaviours
{
    using System;
    using Atom.Diagnostics.Contracts;
    using Atom;
    using Zelda.Entities.Components;

    /// <summary>
    /// Represents the default <see cref="IEntityBehaviour"/> for melee Enemies.
    /// </summary>
    public class MeleeEnemyBehaviour : IEntityBehaviour, ISubEntityBehaviourContainer
    {
        #region [ Events ]

        /// <summary>
        /// Called when the state of this MeleeEnemyBehaviour has changed.
        /// </summary>
        public event EventHandler StateChanged;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets a value indicating whether this MeleeEnemyBehaviour
        /// is currently active.
        /// </summary>
        [System.ComponentModel.Browsable( false )]
        public bool IsActive
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current <see cref="BehaviourState"/> of this MeleeEnemyBehaviour.
        /// </summary>
        public BehaviourState State
        {
            get { return this.state; }
        }

        /// <summary>
        /// Gets the behaviour that controlls the random wandering of the Enemy.
        /// </summary>
        public RandomEnemyMovementBehaviour RandomMovementBehaviour
        {
            get { return this.randomMovementBehaviour; }
        }

        /// <summary>
        /// Gets the behaviour that controlls the chasing of the player.
        /// </summary>
        public EnemyChasePlayerBehaviour ChasePlayerBehaviour
        {
            get { return this.chasePlayerBehaviour; }
        }

        #endregion 

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MeleeEnemyBehaviour"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceProvider"/> is null.</exception>
        internal MeleeEnemyBehaviour( IZeldaServiceProvider serviceProvider )
        {
            Contract.Requires<ArgumentNullException>( serviceProvider != null );

            this.serviceProvider = serviceProvider;

            this.randomMovementBehaviour = new RandomEnemyMovementBehaviour( serviceProvider );
            this.chasePlayerBehaviour    = new EnemyChasePlayerBehaviour();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeleeEnemyBehaviour"/> class.
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
        protected MeleeEnemyBehaviour( Enemy enemy, IZeldaServiceProvider serviceProvider )
        {
            Contract.Requires<ArgumentNullException>( enemy != null );
            Contract.Requires<ArgumentNullException>( serviceProvider != null );

            this.enemy           = enemy;
            this.serviceProvider = serviceProvider;

            this.randomMovementBehaviour = new RandomEnemyMovementBehaviour( enemy, serviceProvider );
            this.chasePlayerBehaviour    = new EnemyChasePlayerBehaviour( enemy );

            this.eventHandlerEnemyAttacked = new Atom.RelaxedEventHandler<AttackEventArgs>( OnEnemyAttacked );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this MeleeEnemyBehaviour.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public virtual void Update( ZeldaUpdateContext updateContext )
        {
            if( !this.IsActive )
                return;

            if( this.activeSubBehaviour != null )
                this.activeSubBehaviour.Update( updateContext );

            if( this.state == BehaviourState.Wandering )
            {
                if( this.randomMovementBehaviour.BehaviourState ==
                    RandomEnemyMovementBehaviour.State.FoundPlayer )
                {
                    this.SetState( BehaviourState.ChasingPlayer );
                }
            }
            else
            {
                if( this.chasePlayerBehaviour.BehaviourState ==
                    EnemyChasePlayerBehaviour.State.Returned )
                {
                    this.SetState( BehaviourState.Wandering );
                }
            }
        }
        
        /// <summary>
        /// Gets called when the enemy has been attacked (by the player).
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The AttackedEventArgs that contains the event data.</param>
        private void OnEnemyAttacked( object sender, AttackEventArgs e )
        {
            if( state == BehaviourState.Wandering )
            {
                // Right now only players can attack enemies.
                var player = e.Attacker as PlayerEntity;

                if( player != null )
                {
                    // In-case the player died after casting his attack.
                    if( !player.IsDead )
                    {
                        SetState( BehaviourState.ChasingPlayer );
                    }
                }
            }
        }

        /// <summary>
        /// Enters this <see cref="MeleeEnemyBehaviour"/>.
        /// </summary>
        public virtual void Enter()
        {
            if( this.IsActive )
                return;

            // Register events:
            this.enemy.Attackable.Attacked += eventHandlerEnemyAttacked;

            this.SetState( BehaviourState.Wandering );
            this.IsActive = true;
        }

        /// <summary>
        /// Leaves this <see cref="MeleeEnemyBehaviour"/>.
        /// </summary>
        /// <exception cref="System.InvalidOperationException"> 
        /// If this <see cref="IEntityBehaviour"/> is currently not active.
        /// </exception>
        public virtual void Leave()
        {
            if( !this.IsActive )
                throw new InvalidOperationException( Resources.Error_CantLeaveEntityBehaviourItIsNotActive );

            if( this.activeSubBehaviour != null )
            {
                if( this.activeSubBehaviour.IsActive )
                    this.activeSubBehaviour.Leave();

                this.activeSubBehaviour = null;
            }

            // Unregister events:
            this.enemy.Attackable.Attacked -= eventHandlerEnemyAttacked;

            this.IsActive = false;
        }

        /// <summary>
        /// Resets this <see cref="MeleeEnemyBehaviour"/>.
        /// </summary>
        public void Reset()
        {
            this.SetState( BehaviourState.Wandering );

            this.randomMovementBehaviour.Reset();
            this.chasePlayerBehaviour.Reset();
        }

        /// <summary>
        /// Sets the state of this <see cref="MeleeEnemyBehaviour"/>.
        /// </summary>
        /// <param name="newState">The state to set.</param>
        private void SetState( BehaviourState newState )
        {
            IEntityBehaviour newBehaviour = this.GetSubBehaviour( newState );

            if( newBehaviour != null )
            {
                if( newBehaviour != this.activeSubBehaviour )
                {
                    if( this.activeSubBehaviour != null )
                        this.activeSubBehaviour.Leave();

                    this.activeSubBehaviour = newBehaviour;
                    this.activeSubBehaviour.Enter();
                }
            }

            var oldState = this.state;
            this.state = newState; 

            this.OnStateChangedPrivate( oldState, newState );
        }

        /// <summary>
        /// Called when the BehaviourState of this MeleeENemyBehaviour has changed.
        /// </summary>
        /// <param name="oldState">
        /// The old BehaviourState.
        /// </param>
        /// <param name="newState">
        /// The new BehaviourState.
        /// </param>
        private void OnStateChangedPrivate( BehaviourState oldState, BehaviourState newState )
        {
            this.StateChanged.Raise( this );
            this.OnStateChanged( oldState, newState );
        }

        /// <summary>
        /// Called when the BehaviourState of this MeleeENemyBehaviour has changed.
        /// </summary>
        /// <param name="oldState">
        /// The old BehaviourState.
        /// </param>
        /// <param name="newState">
        /// The new BehaviourState.
        /// </param>
        protected virtual void OnStateChanged( BehaviourState oldState, BehaviourState newState )
        {
        }

        /// <summary>
        /// Gets the sub-<see cref="IEntityBehaviour"/> associated with the given BehaviourState.
        /// </summary>
        /// <param name="state">
        /// The input BehaviourState.
        /// </param>
        /// <returns>
        /// The IEntityBehaviour that relates to the given BehaviourState.
        /// </returns>
        private IEntityBehaviour GetSubBehaviour( BehaviourState state )
        {
            switch( state )
            {
                case BehaviourState.Wandering:
                    return this.randomMovementBehaviour;

                case BehaviourState.ChasingPlayer:
                    return this.chasePlayerBehaviour;

                default:
                    return null;
            }
        }
        
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

            this.randomMovementBehaviour.Serialize( context );
            this.chasePlayerBehaviour.Serialize( context );
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
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.randomMovementBehaviour.Deserialize( context );
            this.chasePlayerBehaviour.Deserialize( context );
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Returns a clone of this <see cref="MeleeEnemyBehaviour"/>.
        /// </summary>
        /// <param name="newOwner">
        /// The Enemy entity that wants to get controlled by the cloned MeleeEnemyBehaviour.
        /// </param>
        /// <returns>The cloned IEntityBehaviour.</returns>
        public virtual IEntityBehaviour Clone( ZeldaEntity newOwner )
        {
            var clone = new MeleeEnemyBehaviour( (Enemy)newOwner, this.serviceProvider );

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the given <see cref="MeleeEnemyBehaviour"/> to be a clone of this <see cref="MeleeEnemyBehaviour"/>.
        /// </summary>
        /// <param name="clone">
        /// The <see cref="MeleeEnemyBehaviour"/> to setup aas clone of this <see cref="MeleeEnemyBehaviour"/>.
        /// </param>
        protected void SetupClone( MeleeEnemyBehaviour clone )
        {
            this.randomMovementBehaviour.SetupClone( clone.randomMovementBehaviour );
            this.chasePlayerBehaviour.SetupClone( clone.chasePlayerBehaviour );
        }

        #endregion

        #region > Misc <

        /// <summary>
        /// Tries to get the sub-<see cref="IEntityBehaviour"/> of this <see cref="MeleeEnemyBehaviour"/>
        /// that has the given type.
        /// </summary>
        /// <remarks>
        /// Valid types are: 
        /// <see cref="RandomMovementBehaviour"/>, 
        /// <see cref="RandomEnemyMovementBehaviour"/> and 
        /// <see cref="EnemyChasePlayerBehaviour"/>.
        /// </remarks>
        /// <param name="type">
        /// The type of the sub IEntityBehaviour to get.
        /// </param>
        /// <returns>The found sub-IEntityBehaviour or null.</returns>
        public virtual IEntityBehaviour GetSubBehaviour( Type type )
        {
            if( type == typeof( RandomMovementBehaviour )  ||
                type == typeof( RandomEnemyMovementBehaviour ) )
                return randomMovementBehaviour;

            if( type == typeof( EnemyChasePlayerBehaviour ) )
                return chasePlayerBehaviour;

            return null;
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The enemy that is controlled by this DefaultMeleeEnemyBehaviour.
        /// </summary>
        private readonly Enemy enemy;

        /// <summary>
        /// The current state of this DefaultMeleeEnemyBehaviour.
        /// </summary>
        private BehaviourState state = BehaviourState.Wandering;

        /// <summary>
        /// The currently active sub behaviour.
        /// </summary>
        private IEntityBehaviour activeSubBehaviour;

        /// <summary>
        /// The behaviour that controlls the random wandering of the Enemy.
        /// </summary>
        private readonly RandomEnemyMovementBehaviour randomMovementBehaviour;

        /// <summary>
        /// The behaviour that controlls the chasing of the player.
        /// </summary>
        private readonly EnemyChasePlayerBehaviour chasePlayerBehaviour;

        /// <summary>
        /// The event handler that gets invoked when the enemy gets attacked (by the player).
        /// </summary>
        private readonly Atom.RelaxedEventHandler<AttackEventArgs> eventHandlerEnemyAttacked;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        #endregion

        #region [ Enums ]

        /// <summary>
        /// Enumerates the different states of the <see cref="MeleeEnemyBehaviour"/>.
        /// </summary>
        public enum BehaviourState
        {
            /// <summary>
            /// The Enemy is randomly wandering around.
            /// </summary>
            Wandering,

            /// <summary>
            /// The Enemy is chasing the player.
            /// </summary>
            ChasingPlayer
        }

        #endregion
    }
}