// <copyright file="FirewallSpell.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Casting.Spells.FirewallSpell class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Casting.Spells
{
    using Atom.Math;
    using Zelda.Attacks;
    using Zelda.Entities;
    using Zelda.Entities.Behaviours;
    using Zelda.Talents.Magic;

    /// <summary>
    /// Represents a spell that casts a wall of fire next to the player.
    /// </summary>
    /// <remarks>
    /// The two possible setups are; where X is a Firewall Pillar and * the player.
    /// <para>
    /// X*X
    /// </para>
    /// <para>
    ///  X
    ///  *
    ///  X
    /// </para>
    /// </remarks>
    public sealed class FirewallSpell : PlayerSpell
    {
        /// <summary>
        /// Initializes a new instance of the FirewallSpell class.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that owns the new FirewallSpell.
        /// </param>
        /// <param name="castTime">
        /// The time it takes to cast the Firewall.
        /// </param>
        /// <param name="damageMethod">
        /// The damage method responsible for calculating the damage done by the Firewall.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public FirewallSpell( PlayerEntity player, float castTime, AttackDamageMethod damageMethod, IZeldaServiceProvider serviceProvider )
            : base( player, castTime, damageMethod )
        {
            this.firePillarSpell = new FirewallPillarSpell( player, damageMethod, serviceProvider ) {
                TimeBetweenPillarAttacks = 1.8f
            };
        }

        /// <summary>
        /// Fires this FirewallSpell.
        /// </summary>
        /// <param name="target">
        /// This parameter is not used.
        /// </param>
        /// <returns>
        /// true if it was used;
        /// otherwise false.
        /// </returns>
        public override bool Fire( Zelda.Entities.Components.Attackable target )
        {
            Vector2 position = player.Collision.Position;            
            Vector2 direction = player.Transform.Direction.ToVector();

            this.SpawnFirePillar( position + (direction * new Vector2( -24.0f, -24.0f )) );
            this.SpawnFirePillar( position + (direction * new Vector2( -8.0f, -8.0f )) );
            this.SpawnFirePillar( position + (direction * new Vector2( 8.0f, 8.0f )) );
            this.SpawnFirePillar( position + (direction * new Vector2( 24.0f, 24.0f )) );
            return true;
        }
        
        /// <summary>
        /// Spawns a single pillar of the Firewall.
        /// </summary>
        /// <param name="position">
        /// The spawn position.
        /// </param>
        private void SpawnFirePillar( Vector2 position )
        {
            this.firePillarSpell.SpawnFirePillarAt( position, player.FloorNumber, player.Scene );
        }

        /// <summary>
        /// The FirewallPillarSpell that is responsible for creating the pillars the Firewall is made of.
        /// </summary>
        private readonly FirewallPillarSpell firePillarSpell;

        /// <summary>
        /// Represents the spell that is responsible for creating the individual pillars of a Firewall.
        /// </summary>
        private sealed class FirewallPillarSpell : FirePillarSpell
        {
            /// <summary>
            /// Initializes a new instance of the FirewallPillarSpell class.
            /// </summary>
            /// <param name="player">
            /// The player that owns the new FirewallPillarSpell.
            /// </param>
            /// <param name="damageMethod">
            /// The damage method used to calculate the damage done by each individual pillar.
            /// </param>
            /// <param name="serviceProvider">
            /// Provides fast access to game-related servives.
            /// </param>
            public FirewallPillarSpell( PlayerEntity player, AttackDamageMethod damageMethod, IZeldaServiceProvider serviceProvider )
                : base( player, player.Statable, 0.0f, damageMethod, serviceProvider )
            {
            }

            /// <summary>
            /// Allows custom initialization logic for Fire Pillars created by this FirePillarSpell
            /// to be inserted.
            /// </summary>
            /// <param name="firePillar">
            /// The Fire Pillar that was created created.
            /// </param>
            protected override void SetupFirePillarEntity( DamageEffectEntity firePillar )
            {
                firePillar.Behaveable.Behaviour = new RemoveAfterAnimationEndedNtimesBehaviour( firePillar ) {
                    TimesAnimationHasToEnded = 3
                };

                firePillar.MeleeAttack.HitEffect = new PushBackHitEffect();
            }
        }

        private sealed class PushBackHitEffect : IAttackHitEffect
        {
            public void OnHit( Status.Statable user, Status.Statable target )
            {
                var entity = target.Owner as IMoveableEntity;

                if( entity != null )
                {
                    entity.Moveable.Push(entity.Moveable.Speed * FirewallTalent.PushFactor, entity.Transform.Direction.Invert());
                }
            }
        }
    }
}
