// <copyright file="HeadshotSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Ranged.HeadshotSkill class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Skills.Ranged
{
    using Zelda.Attacks.Ranged;
    using Zelda.Entities.Projectiles;
    using Zelda.Status;
    using Zelda.Talents.Ranged;
    
    /// <summary>
    /// Headshot is a swift ranged instant attack that can only be used
    /// after getting a critical ranged attack.
    /// <para>
    /// You aim for the head after getting a critical attack,
    /// firing a swift partially armor-piercing arrow that 
    /// has an improved chance to crit and pierce of 10/20/30%.
    /// </para>
    /// </summary>
    internal sealed class HeadshotSkill : PlayerAttackSkill<HeadshotTalent, RangedPlayerAttack>
    {
        /// <summary>
        /// Gets a value indicating whether this HeadshotSkill is currently in-active.
        /// </summary>
        public override bool IsInactive
        {
            get
            {
                return this.timeLeftUseable <= 0.0f;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadshotSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The talent that 'learns' the player the Skill.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal HeadshotSkill( HeadshotTalent talent, IZeldaServiceProvider serviceProvider )
            : base( talent, HeadshotTalent.Cooldown )
        {
            this.Cost = ManaCost.PercentageOfBase( HeadshotTalent.ManaNeededPoBM );

            // Setup attack:
            this.method = new HeadshotDamageMethod();
            this.method.Setup( serviceProvider );

            this.Attack = new RangedPlayerAttack( this.Player, method, this.Cooldown );
            this.Attack.Settings.Speed = new Atom.Math.IntegerRange(
                (int)(ProjectileSettings.DefaultSpeed.Minimum * HeadshotTalent.ArrowSpeedMultiplier),
                (int)(ProjectileSettings.DefaultSpeed.Maximum * HeadshotTalent.ArrowSpeedMultiplier)
            );

            this.Attack.Setup( serviceProvider );

            var spriteLoader = serviceProvider.SpriteLoader;
            this.Attack.Settings.SetSprites(
                spriteLoader.LoadSprite( "Arrow_Steel_Up" ),
                spriteLoader.LoadSprite( "Arrow_Steel_Down" ),
                spriteLoader.LoadSprite( "Arrow_Steel_Left" ),
                spriteLoader.LoadSprite( "Arrow_Steel_Right" )
            );
        }

        /// <summary>
        /// Initializes this HeadshotSkill.
        /// </summary>
        public override void Initialize()
        {
            this.Statable.RangedCrit += this.OnRangedCrit;
        }

        /// <summary>
        /// Uninitializes this HeadshotSkill.
        /// </summary>
        public override void Uninitialize()
        {
            this.Statable.RangedCrit -= this.OnRangedCrit;
        }

        /// <summary>
        /// Refreshes the power of this HeadshotSkill based on the HeadshotTalent.
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            this.Attack.Settings.AdditionalPiercingChance = this.Talent.AdditionalPiercingChance;
            this.method.SetValues( this.Talent.CritChanceIncrease, HeadshotTalent.ArmorPiercingModifier );
        }
        
        /// <summary>
        /// Updates the HeadshotSkill.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            if( this.timeLeftUseable > 0.0f )
            {
                this.timeLeftUseable -= updateContext.FrameTime;
            }

            this.Attack.Update( updateContext );
        }

        /// <summary>
        /// Gets called when the player got a ranged critical attack.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnRangedCrit( Zelda.Status.Statable sender )
        {
            this.timeLeftUseable = HeadshotTalent.TimeUseableAfterCrit;
        }

        /// <summary>
        /// The time in seconds left the HeadshotSkill is useable for.
        /// </summary>
        private float timeLeftUseable;

        /// <summary>
        /// The AttackDamageMethod that is used to calculate the damage done by the headshots.
        /// </summary>
        private readonly HeadshotDamageMethod method;
    }
}
