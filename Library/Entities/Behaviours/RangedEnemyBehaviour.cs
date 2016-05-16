// <copyright file="RangedEnemyBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.RangedEnemyBehaviour class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Behaviours
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using Atom;
    using Zelda.Attacks.Ranged;
    using Zelda.Entities.Components;
    
    /// <summary>
    /// Represents the default <see cref="IEntityBehaviour"/> for ranged Enemies.
    /// </summary>
    public class RangedEnemyBehaviour : IRangedEnemyBehaviour, ISubEntityBehaviourContainer
    {
        #region [ Events ]

        /// <summary>
        /// Called when the state of this RangedEnemyBehaviour has changed.
        /// </summary>
        public event EventHandler StateChanged;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets a value indicating whether this RangedEnemyBehaviour
        /// is currently active.
        /// </summary>
        [System.ComponentModel.Browsable( false )]
        public bool IsActive
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current <see cref="BehaviourState"/> of this RangedEnemyBehaviour.
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

        /// <summary>
        /// Gets the <see cref="RangedAttack"/> associated with this RangedEnemyBehaviour.
        /// </summary>
        [Browsable(false)]
        public RangedAttack RangedAttack
        {
            get
            {
                return this.rangedAttack;
            }
        }

        /// <summary>
        /// Gets the <see cref="AttackSettings"/> associated with this RangedEnemyBehaviour.
        /// </summary>
        public RangedEnemyAttackSettings AttackSettings
        {
            get { return this.attackSettings; }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="RangedEnemyBehaviour"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal RangedEnemyBehaviour( IZeldaServiceProvider serviceProvider )
        {
            Contract.Requires<ArgumentNullException>( serviceProvider != null );

            this.serviceProvider = serviceProvider;

            this.attackSettings = new RangedEnemyAttackSettings( serviceProvider );
            this.randomMovementBehaviour = new RandomEnemyMovementBehaviour( serviceProvider );
            this.chasePlayerBehaviour    = new EnemyChasePlayerBehaviour();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangedEnemyBehaviour"/> class.
        /// </summary>
        /// <param name="enemy">
        /// The entity that is controlled by the new RangedEnemyBehaviour.
        /// </param>
        /// <param name="attackSettings">
        /// The RangedEnemyAttackSettings of the Enemy.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        protected RangedEnemyBehaviour( Enemy enemy, RangedEnemyAttackSettings attackSettings, IZeldaServiceProvider serviceProvider )
        {
            Contract.Requires<ArgumentNullException>( enemy != null );
            Contract.Requires<ArgumentNullException>( attackSettings != null );
            Contract.Requires<ArgumentNullException>( serviceProvider != null );

            this.enemy = enemy;
            this.moveable = enemy.Moveable;
            this.attackSettings  = attackSettings;
            this.serviceProvider = serviceProvider;
            
            // Attack.
            this.rangedAttack = new RangedAttack( enemy, attackSettings.DamageMethod );
            this.rangedAttack.Setup( serviceProvider );

            this.ApplyAttackSettings(  serviceProvider );

            // Sub Behaviours.
            this.randomMovementBehaviour = new RandomEnemyMovementBehaviour( enemy, serviceProvider );
            this.chasePlayerBehaviour    = new EnemyChasePlayerBehaviour( enemy );
        }

        /// <summary>
        /// Applies the Attack Settings to this RangedEnemyBehaviour.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        private void ApplyAttackSettings( IZeldaServiceProvider serviceProvider )
        {
            if( this.attackSettings.ProjectileSprites == null )
                this.attackSettings.LoadProjectileSprites( serviceProvider.SpriteLoader );

            this.rangedAttack.Settings.Speed = this.attackSettings.ProjectileSpeed;
            this.rangedAttack.Settings.Sprites = this.attackSettings.ProjectileSprites;
            this.rangedAttack.HitSettings = this.attackSettings.HitSettings;
        }

        #endregion

        #region [ Methods ]

        #region > Updating <

        /// <summary>
        /// Updates this RangedEnemyBehaviour.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public virtual void Update( ZeldaUpdateContext updateContext )
        {
            if( !this.IsActive )
                return;

            this.UpdateSubBehaviour( updateContext );
            this.UpdateRangedAttack( updateContext );
            this.UpdateMovementRestriction( updateContext );
        }

        /// <summary>
        /// Updates the current sub-behaviour.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void UpdateSubBehaviour( ZeldaUpdateContext updateContext )
        {
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
        /// Updates the ranged attacking logic.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void UpdateRangedAttack( ZeldaUpdateContext updateContext )
        {
            if( this.ShouldFireProjectile() )
            {
                this.timeUntilNextRangedAttack -= updateContext.FrameTime;

                if( this.timeUntilNextRangedAttack <= 0.0f )
                {
                    this.FireProjectile();
                    this.timeUntilNextRangedAttack = this.GetExtraTimeBetweenAttacks();
                }
            }

            this.rangedAttack.Update( updateContext );
        }

        /// <summary>
        /// Gets the time (in seconds) before the next attack will be launched.
        /// </summary>
        /// <returns>
        /// The time in seconds.
        /// </returns>
        private float GetExtraTimeBetweenAttacks()
        {
            float time = this.attackSettings.ExtraTimeBetweenAttacks.GetRandomValue( this.serviceProvider.Rand );

            if( this.state == BehaviourState.ChasingPlayer )
            {
                // The enemy attacks more often while chasing the player.
                const float TimeModifier = 0.8f;
                return time * TimeModifier;
            }
            else 
            {
                return time;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Enemy should fire a Projectile.
        /// </summary>
        /// <returns></returns>
        private bool ShouldFireProjectile()
        {
            return this.rangedAttack.IsReady;
        }

        /// <summary>
        /// Fires a Projectile.
        /// </summary>
        private void FireProjectile()
        {           
            this.rangedAttack.Fire( null );

            if( attackSettings.TimeUnmoveableAfterAttack > 0.0f && this.moveable.CanMove )
            {
                this.moveable.CanMove = false;
                this.timeLeftUnableToMove = attackSettings.TimeUnmoveableAfterAttack;
                this.isMovementRestricted = true;
            }
        }

        /// <summary>
        /// Updates the movement restriction logic.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void UpdateMovementRestriction( ZeldaUpdateContext updateContext )
        {
            if( this.isMovementRestricted )
            {
                this.timeLeftUnableToMove -= updateContext.FrameTime;

                if( timeLeftUnableToMove <= 0.0f )
                {
                    this.moveable.CanMove = true;
                    this.isMovementRestricted = false;
                }
            }
        }        

        #endregion

        #region > Events <

        /// <summary>
        /// Gets called when the enemy has been attacked (by the player).
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The AttackedEventArgs that contains the event data.</param>
        private void OnEnemyAttacked( object sender, AttackEventArgs e )
        {
            // following line is not needed because enemies currently can only be attacked by the Player
            //// if( e.Attacker == enemy.Scene.Player )

            if( state == BehaviourState.Wandering )
            {
                this.SetState( BehaviourState.ChasingPlayer );
            }
        }

        #endregion

        #region > State <

        /// <summary>
        /// Enters this <see cref="RangedEnemyBehaviour"/>.
        /// </summary>
        public void Enter()
        {
            if( IsActive )
                return;

            // Register events:
            this.enemy.Attackable.Attacked += this.OnEnemyAttacked;

            this.SetState( BehaviourState.Wandering );
            this.IsActive = true;
        }

        /// <summary>
        /// Leaves this <see cref="RangedEnemyBehaviour"/>.
        /// </summary>
        /// <exception cref="System.InvalidOperationException"> 
        /// If this <see cref="IEntityBehaviour"/> is currently not active.
        /// </exception>
        public void Leave()
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
            this.enemy.Attackable.Attacked -= this.OnEnemyAttacked;

            this.IsActive = false;
        }

        /// <summary>
        /// Resets this <see cref="RangedEnemyBehaviour"/>.
        /// </summary>
        public void Reset()
        {
            this.SetState( BehaviourState.Wandering );

            this.randomMovementBehaviour.Reset();
            this.chasePlayerBehaviour.Reset();
        }

        /// <summary>
        /// Sets the state of this <see cref="RangedEnemyBehaviour"/>.
        /// </summary>
        /// <param name="newState">The state to set.</param>
        private void SetState( BehaviourState newState )
        {
            IEntityBehaviour newBehaviour = this.GetSubBehaviour( newState );

            // Set:
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
           
            this.state = newState;
            this.StateChanged.Raise( this );
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

            this.attackSettings.Serialize( context );
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
            const int Version = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, Version, this.GetType() );

            this.attackSettings.Deserialize( context );
            this.randomMovementBehaviour.Deserialize( context );
            this.chasePlayerBehaviour.Deserialize( context );
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Returns a clone of this <see cref="RangedEnemyBehaviour"/>.
        /// </summary>
        /// <param name="newOwner">
        /// The Enemy entity that wants to get controlled by the cloned RangedEnemyBehaviour.
        /// </param>
        /// <returns>The cloned IEntityBehaviour.</returns>
        public virtual IEntityBehaviour Clone( ZeldaEntity newOwner )
        {
            var clone = new RangedEnemyBehaviour( (Enemy)newOwner, this.attackSettings, this.serviceProvider );

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the given <see cref="RangedEnemyBehaviour"/> to be a clone of this <see cref="RangedEnemyBehaviour"/>.
        /// </summary>
        /// <param name="clone">
        /// The <see cref="RangedEnemyBehaviour"/> to setup aas clone of this <see cref="RangedEnemyBehaviour"/>.
        /// </param>
        protected void SetupClone( RangedEnemyBehaviour clone )
        {
            this.randomMovementBehaviour.SetupClone( clone.randomMovementBehaviour );
            this.chasePlayerBehaviour.SetupClone( clone.chasePlayerBehaviour );
        }

        #endregion

        #region > Misc <

        /// <summary>
        /// Tries to get the sub-<see cref="IEntityBehaviour"/> of this <see cref="RangedEnemyBehaviour"/>
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
        /// The time until the next ranged attack is fired.
        /// </summary>
        private float timeUntilNextRangedAttack;

        /// <summary>
        /// The RangedAttack of the ranged Enemy.
        /// </summary>
        private readonly RangedAttack rangedAttack;

        /// <summary>
        /// The <see cref="RangedEnemyAttackSettings"/> associated with this RangedEnemyBehaviour.
        /// </summary>
        private readonly RangedEnemyAttackSettings attackSettings;

        /// <summary>
        /// The enemy that is controlled by this DefaultRangedEnemyBehaviour.
        /// </summary>
        private readonly Enemy enemy;

        /// <summary>
        /// Identifies the Moveable component of the Enemy.
        /// </summary>
        private readonly Moveable moveable;
        
        /// <summary>
        /// The time (in seconds) the enemy is unable to move.
        /// </summary>
        private float timeLeftUnableToMove;

        /// <summary>
        /// States whether the enemy's movement is currently restricted.
        /// </summary>
        private bool isMovementRestricted;

        /// <summary>
        /// The current state of this DefaultRangedEnemyBehaviour.
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
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        #endregion

        #region [ Enums ]

        /// <summary>
        /// Enumerates the different states of the <see cref="RangedEnemyBehaviour"/>.
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