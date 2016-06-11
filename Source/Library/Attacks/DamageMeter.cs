// <copyright file="DamageMeter.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Attacks.DamageMeter class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Attacks
{
    using Zelda.Entities.Components;

    /// <summary>
    /// Provides a mechanism to capture the damage done of an <see cref="Attackable"/> ZeldaEntity.
    /// This is a sealed class.
    /// </summary>
    public sealed class DamageMeter
    {
        /// <summary>
        /// Gets or sets the <see cref="Attackable"/> component of the
        /// ZeldaEntity to follow.
        /// </summary>
        public Attackable AttackerToFollow
        {
            get
            {
                return this.attacker;
            }

            set
            {
                if( this.attacker != null )
                    this.Unhook();

                this.attacker = value;

                if( this.attacker != null )
                    this.Hook();
            }
        }

        /// <summary>
        /// Gets the damage done  by the <see cref="AttackerToFollow"/> in seconds.
        /// </summary>
        public float DamagePerSecond
        {
            get
            {
                return this.totalTime != 0.0f ? this.totalDamageDone / this.totalTime : 0.0f;
            }
        }

        /// <summary>
        /// Resets this DamageMeter.
        /// </summary>
        public void Reset()
        {
            this.totalTime = 0.0f;
            this.totalDamageDone = 0;
        }

        /// <summary>
        /// Updates this DamageMeter.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( updateContext.IsPaused )
                return;

            this.totalTime += updateContext.FrameTime;
            this.UpdateReset( updateContext );
        }

        /// <summary>
        /// Updates the reseting logic of this DamageMeter.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void UpdateReset( ZeldaUpdateContext updateContext )
        {
            const float TimeElapsedWithoutAttackUntilReset = 10.0f;
            this.timeLeftResetCheck += updateContext.FrameTime;

            if( this.timeLeftResetCheck >= TimeElapsedWithoutAttackUntilReset )
            {
                if( this.oldTotalDamage == this.totalDamageDone )
                {
                    this.Reset();
                }
                else
                {
                    this.oldTotalDamage = this.totalDamageDone;
                }

                this.timeLeftResetCheck = 0.0f;
            }
        }

        /// <summary>
        /// Gets called when the Entity to follow has used an attack.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The AttackEventArgs that contain the event data.
        /// </param>
        private void OnAttackUsed( object sender, AttackEventArgs e )
        {
            if( IsSelfAttack( ref e ) )
                return;

            this.totalDamageDone += e.DamageResult.Damage;
        }

        /// <summary>
        /// Gets a value indicating whether the player has attacked himself.
        /// </summary>
        /// <remarks>
        /// This happens for example when using the 'Frustration' skill.
        /// </remarks>
        /// <param name="e">
        /// The AttackEventArgs that contain the event data.
        /// </param>
        /// <returns>
        /// Returns true if the attack was a self-inflicted attack;
        /// otherwise false.
        /// </returns>
        private static bool IsSelfAttack( ref AttackEventArgs e )
        {
            return e.Attacker != null && e.Attacker == e.Target;
        }

        /// <summary>
        /// Hooks up the connections with the current Attackable.
        /// </summary>
        private void Hook()
        {
            this.attacker.AttackHit += this.OnAttackUsed;
        }

        /// <summary>
        /// Un-hooks up the connections with the current Attackable.
        /// </summary>
        private void Unhook()
        {
            this.attacker.AttackHit -= this.OnAttackUsed;
        }

        /// <summary>
        /// The total time that has elapsed since the last reset.
        /// </summary>
        private float totalTime;

        /// <summary>
        /// The total damage done since the last reset.
        /// </summary>
        private int totalDamageDone;

        /// <summary>
        /// The time in seconds until the next reset check.
        /// </summary>
        private float timeLeftResetCheck;

        /// <summary>
        /// The damage done on the last reset check.
        /// </summary>
        private int oldTotalDamage;

        /// <summary>
        /// Identifies the <see cref="Attackable"/> component of the
        /// ZeldaEntity to follow.
        /// </summary>
        private Attackable attacker;
    }
}
