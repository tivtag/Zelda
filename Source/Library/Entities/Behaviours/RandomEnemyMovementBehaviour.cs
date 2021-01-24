// <copyright file="RandomEnemyMovementBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.RandomEnemyMovementBehaviour class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Behaviours
{
    using System;
    
    /// <summary>
    /// Defines the default <see cref="RandomMovementBehaviour"/> for <see cref="Enemy"/> entities.
    /// </summary>
    /// <remarks>
    /// The difference is that the behaviour looks for the player and
    /// leaves the behaviour if it sees the Player.
    /// <para>
    /// An above standing behaviour may react to this by changing into a different behaviour, 
    /// such as the EnemyChasePlayerBehaviour.
    /// </para>
    /// </remarks>
    public class RandomEnemyMovementBehaviour : RandomMovementBehaviour
    {
        /// <summary>
        /// Gets the current state of this <see cref="RandomEnemyMovementBehaviour"/>.
        /// </summary>
        [System.ComponentModel.Browsable( false )]
        public State BehaviourState
        {
            get { return state; }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomEnemyMovementBehaviour"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        /// <exception cref="ArgumentNullException">If <paramref name="serviceProvider"/> is null.</exception>
        internal RandomEnemyMovementBehaviour( IZeldaServiceProvider serviceProvider )
            : base( serviceProvider )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomEnemyMovementBehaviour"/> class.
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
        public RandomEnemyMovementBehaviour( Enemy entity, IZeldaServiceProvider serviceProvider )
            : base( entity, serviceProvider )
        {
            this.enemy = entity;
        }
        
        /// <summary>
        /// Enters the <see cref="RandomEnemyMovementBehaviour"/>.
        /// </summary>
        public override void Enter()
        {
            if( this.IsActive )
                return;

            base.Enter();

            this.state              = State.Default;
            this.timeLeftVisioCheck = TimeBetweenVisioChecks;
        }

        /// <summary>
        /// Updates this <see cref="RandomEnemyMovementBehaviour"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            if( !this.IsActive )
                return;

            if( this.enemy.AgressionType != AggressionType.Neutral )
            {
                this.timeLeftVisioCheck -= updateContext.FrameTime;

                if( timeLeftVisioCheck <= 0.0f )
                {
                    this.CheckVision();
                    this.timeLeftVisioCheck = TimeBetweenVisioChecks;
                }
            }

            base.Update( updateContext );
        }

        /// <summary>
        /// Checks whether the player is in vision of the enemy controlled by this IEntityBehaviour.
        /// </summary>
        private void CheckVision()
        {
            ZeldaScene scene = this.enemy.Scene;
            if( scene == null )
                return;

            PlayerEntity player = scene.Player;
            if( player == null || player.IsDead )
                return;

            if( this.enemy.Visionable.IsWithinVision( player ) )
            {
                this.state = State.FoundPlayer;
                this.Leave();
            }
        }

        /// <summary>
        /// Resets this <see cref="RandomEnemyMovementBehaviour"/>.
        /// </summary>
        public override void Reset()
        {
            this.state              = State.Default;
            this.timeLeftVisioCheck = TimeBetweenVisioChecks;

            base.Reset();
        }

        /// <summary>
        /// Returns a clone of this <see cref="RandomEnemyMovementBehaviour"/>
        /// for the given ZeldaEntity.
        /// </summary>
        /// <param name="newOwner">The owner of the clone to create.</param>
        /// <returns>The cloned IEntityBehaviour.</returns>
        public override IEntityBehaviour Clone( ZeldaEntity newOwner )
        {
            var clone = new RandomEnemyMovementBehaviour( (Enemy)newOwner, this.ServiceProvider );

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

            const int CurrentVersion = 2;
            context.Write( CurrentVersion );
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

            const int CurrentVersion = 2;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, "RandomEnemyMovementBehaviour" );
        }
        
        /// <summary>
        /// The current state of the <see cref="RandomEnemyMovementBehaviour"/>.
        /// </summary>
        private State state = State.Default;

        /// <summary>
        /// The time that is left until the Enemy looks for the player again.
        /// </summary>
        private float timeLeftVisioCheck = TimeBetweenVisioChecks;

        /// <summary>
        /// The time between the tests that check whether the Enemy sees the Player.
        /// </summary>
        private const float TimeBetweenVisioChecks = 1.25f;

        /// <summary>
        /// Identifies the Enemy entity that this IEntityBehaviour controls.
        /// </summary>
        private readonly Enemy enemy;
        
        /// <summary>
        /// Enumerates the different states of the <see cref="RandomEnemyMovementBehaviour"/>.
        /// </summary>
        public enum State
        {
            /// <summary>
            /// The default state, the enemy is wandering around.
            /// </summary>
            Default,

            /// <summary>
            /// The enemy found the player.
            /// </summary>
            FoundPlayer
        }
    }
}
