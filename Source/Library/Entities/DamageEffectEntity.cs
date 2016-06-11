// <copyright file="DamageEffectEntity.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.DamageEffectEntity class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities
{
    using Zelda.Attacks;
    using Zelda.Entities.Components;
    using Zelda.Status;

    /// <summary>
    /// Represents an object that deals damage to other entities
    /// but its status is based on its creator.
    /// </summary>
    public class DamageEffectEntity : ZeldaEntity
    {
        /// <summary>
        /// Gets the Behaveable component that owns this DamageEffectEntity.
        /// </summary>
        public Behaveable Behaveable
        {
            get
            {
                return this.behaveable;
            }
        }

        /// <summary>
        /// Gets the MeleeAttack this DamageEffectEntity uses to deal damage.
        /// </summary>
        public MeleeAttack MeleeAttack
        {
            get
            {
                return this.meleeAttack;
            }
        }

        /// <summary>
        /// Gets or sets the Statable component of the entity that has created this DamageEffectEntity.
        /// </summary>
        public Statable Creator
        {
            get
            {
                return this.creator;
            }

            set
            {
                this.creator = value;
                this.meleeAttack.Owner = value.Owner;
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the DamageEffectEntity class.
        /// </summary>
        public DamageEffectEntity()
            : base( 4 )
        {
            this.behaveable = new Behaveable();
            this.Components.Add( this.behaveable );
        }

        /// <summary>
        /// Updates this DamageEffectEntity.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            this.UpdateAttack( updateContext );

            base.Update( updateContext );
        }

        /// <summary>
        /// Updates the attack logic of this Projectile.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void UpdateAttack( ZeldaUpdateContext updateContext )
        {
            this.meleeAttack.Update( updateContext );
            this.TestCollision();
        }

        /// <summary>
        /// Tests for collesion against enemy objects.
        /// </summary>
        protected virtual void TestCollision()
        {
            if( !this.meleeAttack.IsReady || this.creator == null )
                return;

            var rectangle = this.Collision.Rectangle;
            var targets = this.Scene.VisibleEntities;

            for( int i = 0; i < targets.Count; ++i )
            {
                ZeldaEntity target = targets[i];

                if( this.FloorNumber == target.FloorNumber )
                {
                    if( target.Collision.Intersects( ref rectangle ) )
                    {
                        this.TryAttack( target );
                    }
                }
            }
        }

        /// <summary>
        /// Tries to attack the specified target.
        /// </summary>
        /// <param name="target">
        /// The entity that got attacked.
        /// </param>
        protected void TryAttack( ZeldaEntity target )
        {
            var attackable = target.Components.Get<Attackable>();

            if( attackable != null )
            {
                var statable = attackable.Statable;

                if( statable != null )
                {
                    if( creator.IsFriendly != statable.IsFriendly )
                    {
                        if( this.meleeAttack.Fire( attackable ) )
                        {
                            this.OnHitTarget( target );
                        }
                    }
                }
                else
                {
                    if( this.meleeAttack.Fire( attackable ) )
                    {
                        this.OnHitTarget( target );
                    }
                }
            }
        }

        /// <summary>
        /// Called when this DamageEffectEntity has attacked an Entity.
        /// </summary>
        /// <param name="target">
        /// The entity that got hit.
        /// </param>
        private void OnHitTarget( ZeldaEntity target )
        {
        }

        /// <summary>
        /// Returns a clone of this DamageEffectEntity.
        /// </summary>
        /// <returns>
        /// The cloned ZeldaEntity.
        /// </returns>
        public override ZeldaEntity Clone()
        {
            var clone = new DamageEffectEntity();

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the specified DamageEffectEntity to be a clone of this DamageEffectEntity.
        /// </summary>
        /// <param name="clone">
        /// The DamageEffectEntity to setup as a clone of this DamageEffectEntity.
        /// </param>
        protected void SetupClone( DamageEffectEntity clone )
        {
            base.SetupClone( clone );

            clone.creator = this.creator;
            this.meleeAttack.SetupClone( clone.meleeAttack );
            this.behaveable.SetupClone( clone.behaveable );
        }

        /// <summary>
        /// The Statable component of the Entity that has created this DamageEffectEntity.
        /// </summary>
        private Statable creator;

        /// <summary>
        /// The melee attack that gets executed when this DamageEffectEntity hits an enemy.
        /// </summary>
        private readonly MeleeAttack meleeAttack = new MeleeAttack( null, null );

        /// <summary>
        /// Identifies the Behaveable component that controls this DamageEffectEntity.
        /// </summary>
        private readonly Behaveable behaveable;
    }
}
