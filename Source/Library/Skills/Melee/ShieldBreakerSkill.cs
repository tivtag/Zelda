// <copyright file="ShieldBreakerSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Melee.ShieldBreakerSkill class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Skills.Melee
{
    using Zelda.Attacks.Melee;
    using Zelda.Status;
    using Zelda.Talents.Melee;

    /// <summary>
    /// You build up strength in your swords arm after blocking an attack.
    /// Unleashes MeleeDmg + BlockValue * ({0} * Block Points)% damage. 
    /// 1 Block Point is awarded for each block; lasting 12 seconds. 
    /// 5 Block Points maximum.
    /// </summary>
    internal sealed class ShieldBreakerSkill : PlayerAttackSkill<ShieldBreakerTalent, PlayerMeleeAttack>
    {
        /// <summary>
        /// Gets a value indicating whether this ShieldBreakerSkill is only limited by its own cooldown
        /// and not such things as location/mana cost/etc.
        /// </summary>
        public override bool IsOnlyLimitedByCooldown
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this ShieldBlockSkill is currently useable.
        /// </summary>
        public override bool IsUseable
        {
            get
            {
                return base.IsUseable && this.Player.Statable.CanBlock && !this.Player.Moveable.IsSwimming;
                ;
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether this ShieldBreakerSkill is currently in-active and
        /// as such unuseable.
        /// </summary>
        public override bool IsInactive
        {
            get
            {
                return (this.Player.Statable.CanBlock && !this.Player.Moveable.IsSwimming) && this.blockPoints == 0;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShieldBreakerSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The talent that 'learns' the player the Skill.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal ShieldBreakerSkill( ShieldBreakerTalent talent, IZeldaServiceProvider serviceProvider )
            : base( talent, ShieldBreakerTalent.Cooldown )
        {
            this.Cost = ManaCost.PercentageOfBase( ShieldBreakerTalent.ManaNeededPoBM );

            this.method = new ShieldBreakerDamageMethod();
            this.method.Setup( serviceProvider );

            this.Attack = new PlayerMeleeAttack( this.Player, method, this.Cooldown );
            this.Attack.Setup( serviceProvider );

            this.blockPointAura = new TimedAura( ShieldBreakerTalent.BlockPointDuration ) {
                IsVisible = true,
            };                 

            // Sprites
            var spriteLoader = serviceProvider.SpriteLoader;
            this.spriteSymbol_1BP = spriteLoader.LoadSprite( "Symbol_ShieldBreaker_1BP" );
            this.spriteSymbol_2BP = spriteLoader.LoadSprite( "Symbol_ShieldBreaker_2BP" );
            this.spriteSymbol_3BP = spriteLoader.LoadSprite( "Symbol_ShieldBreaker_3BP" );
            this.spriteSymbol_4BP = spriteLoader.LoadSprite( "Symbol_ShieldBreaker_4BP" );
            this.spriteSymbol_5BP = spriteLoader.LoadSprite( "Symbol_ShieldBreaker_5BP" );
        }

        /// <summary>
        /// Initializes this ShieldBreakerSkill.
        /// </summary>
        public override void Initialize()
        {
            this.Statable.Blocked += this.OnPlayerBlocked;
        }

        /// <summary>
        /// Uninitializes this ShieldBreakerSkill.
        /// </summary>
        public override void Uninitialize()
        {
            this.Statable.Blocked -= this.OnPlayerBlocked;
        }

        /// <summary>
        /// Called when this Skill has been succesfully fired.
        /// </summary>
        protected override void OnFired()
        {
            this.blockPoints = 0;
            this.AuraList.Remove( this.blockPointAura );
        }

        /// <summary>
        /// Updates this ShieldBreakerSkill.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            if( this.blockPointsTimeRemaining > 0.0f )
            {
                this.blockPointsTimeRemaining -= updateContext.FrameTime;
            }
            else
            {
                this.blockPoints = 0;
            }

            this.Attack.Update( updateContext );
        }

        /// <summary>
        /// Refreshes the power of this ShieldBreakerSkill based on the talents of the player.
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            this.multiplierPerPoint = this.Talent.BlockValueMultiplierPerBlockPoint;
            this.RefreshBlockValueToDamageMultiplier();
        }

        /// <summary>
        /// Refreshes the multiplier value that converts BlockValue into Damage. 
        /// </summary>
        private void RefreshBlockValueToDamageMultiplier()
        {
            float multiplier = 1.0f + (this.blockPoints * this.multiplierPerPoint);
            this.method.SetValues( multiplier );
        }

        /// <summary>
        /// Called when the player has blocked an attack.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnPlayerBlocked( Statable sender )
        {
            if( this.blockPoints < ShieldBreakerTalent.MaximumBlockPoints )
            {
                ++this.blockPoints;
                this.RefreshBlockValueToDamageMultiplier();
            }

            this.blockPointsTimeRemaining = ShieldBreakerTalent.BlockPointDuration;
            this.ResetAura();
        }

        /// <summary>
        /// Resets the TimedAura to display how many block points the player has.
        /// </summary>
        private void ResetAura()
        {
            this.blockPointAura.ResetDuration();

            if( !this.blockPointAura.IsEnabled )
            {
                this.AuraList.Add( this.blockPointAura );
            }

            this.blockPointAura.Symbol = this.GetAuraSymbol();
        }

        /// <summary>
        /// Gets the Sprite that is used to visualize the current number of blockPoints.
        /// </summary>
        /// <returns>The requested Sprite.</returns>
        private Atom.Xna.Sprite GetAuraSymbol()
        {
            switch( this.blockPoints )
            {
                case 1:
                    return this.spriteSymbol_1BP;

                case 2:
                    return this.spriteSymbol_2BP;

                case 3:
                    return this.spriteSymbol_3BP;

                case 4:
                    return this.spriteSymbol_4BP;

                case 5:
                    return this.spriteSymbol_5BP;

                case 0:
                default:
                    return null;
            }
        }

        /// <summary>
        /// Caches the current multiplier value of the ShieldBreakerTalent.
        /// </summary>
        private float multiplierPerPoint;

        /// <summary>
        /// The number of block points the player has managed to aquire.
        /// </summary>
        private int blockPoints;

        /// <summary>
        /// The time left until the blockPoints vanish.
        /// </summary>
        private float blockPointsTimeRemaining;

        /// <summary>
        /// The aura responsible for visualizing how many block points the player has.
        /// </summary>
        private readonly TimedAura blockPointAura;
        
        /// <summary>
        /// The DamageMethod used to calculate the damage done by this ShieldBreakerSkill.
        /// </summary>
        private readonly ShieldBreakerDamageMethod method;

        /// <summary>
        /// The sprites used to visualize how many block points the player currently has.
        /// </summary>
        private readonly Atom.Xna.Sprite 
            spriteSymbol_1BP,
            spriteSymbol_2BP,
            spriteSymbol_3BP,
            spriteSymbol_4BP,
            spriteSymbol_5BP;
    }
}
