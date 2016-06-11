// <copyright file="CounterAttackSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Melee.CounterAttackSkill class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Skills.Melee
{
    using Zelda.Attacks;
    using Zelda.Attacks.Melee;
    using Zelda.Talents.Melee;

    /// <summary>
    /// The Counter Attack skill gets useable after killing
    /// </summary>
    internal sealed class CounterAttackSkill : PlayerAttackSkill<CounterAttackTalent, PlayerMeleeAttack>
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="Skill"/> is currently inactive;
        /// and as such unuseable.
        /// </summary>
        public override bool IsInactive
        {
            get 
            {
                return this.useableTimeCounter <= 0.0f; 
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CounterAttackSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The CounterAttackTalent that 'learns' the player the new CounterAttackSkill.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal CounterAttackSkill( CounterAttackTalent talent, IZeldaServiceProvider serviceProvider )
            : base( talent, CounterAttackTalent.Cooldown )
        {
            this.method = new CounterAttackDamageMethod();
            this.Attack = new PlayerMeleeAttack( this.Player, this.method, this.Cooldown ) {
                IsPushing = true
            };

            this.method.Setup( serviceProvider );
            this.Attack.Setup( serviceProvider );
        }

        /// <summary>
        /// Initializes this CounterAttackSkill.
        /// </summary>
        public override void Initialize()
        {
            this.Player.Attackable.Attacked += this.OnPlayerAttacked;
        }

        /// <summary>
        /// Uninitializes this CounterAttackSkill.
        /// </summary>
        public override void Uninitialize()
        {
            this.Player.Attackable.Attacked -= this.OnPlayerAttacked;
        }
        
        /// <summary>
        /// Refreshes the power of this CounterAttackSkill based on the talents of the player.
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            this.method.SetValues( this.Talent.DamageIncreaseMultiplier );
        }

        /// <summary>
        /// Called when this CounterAttackSkill has been succesfully fired.
        /// </summary>
        protected override void OnFired()
        {
            this.useableTimeCounter = 0.0f;
        }

        /// <summary>
        /// Updates this CounterAttackSkill.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            if( this.useableTimeCounter > 0.0f )
                this.useableTimeCounter -= updateContext.FrameTime;

            this.Attack.Update( updateContext );
        }

        /// <summary>
        /// Called when the player was attacked by an enemy.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="Zelda.Entities.Components.AttackEventArgs"/> that contain the event data.
        /// </param>
        private void OnPlayerAttacked( object sender, Zelda.Entities.Components.AttackEventArgs e )
        {
            if( e.DamageResult.AttackReceiveType == AttackReceiveType.Parry )
            {
                this.useableTimeCounter = CounterAttackTalent.UseableTime;
            }
        }
        
        /// <summary>
        /// Stores the time the Counter Attack ability is useable.
        /// </summary>
        private float useableTimeCounter;

        /// <summary>
        /// The method that calcualtes the damage the CounterAttack does.
        /// </summary>
        private readonly CounterAttackDamageMethod method;
    }
}
