// <copyright file="ShieldWallSkill.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Melee.ShieldWallSkill class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Skills.Melee
{
    using Zelda.Status;
    using Zelda.Status.Damage;
    using Zelda.Talents.Melee;

    /// <summary>
    /// Shield Wall is an active ability that when
    /// used reduces damage taken by all sources 
    /// by 20.0% per level for a total of 60%.
    /// <para>
    /// Shield Wall also reduces damage done and the movement speed of the Player
    /// as a penality for using the Skill.
    /// Shield Wall doesn't cost Mana and is active until deactivated.
    /// </para>
    /// </summary>
    internal sealed class ShieldWallSkill : PlayerSkill
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the description of this ShieldWallSkill.
        /// </summary>
        public override string Description
        {
            get
            {
                return this.talent.Description;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this ShieldWallSkill is currently useable.
        /// </summary>
        public override bool IsUseable
        {
            get
            {
                return this.Cooldown.IsReady;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this ShieldWallSkill is only limited by its own cooldown
        /// and not such things as location/mana cost/etc.
        /// </summary>
        public override bool IsOnlyLimitedByCooldown
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="ShieldWallSkill"/> class.
        /// </summary>
        /// <param name="talent">
        /// The ShieldWallTalent that 'learns' the player the new ShieldWallSkill.
        /// </param>
        internal ShieldWallSkill( ShieldWallTalent talent )
            : base( talent.LocalizedName, new Cooldown( ShieldWallTalent.Cooldown ), talent.Symbol, talent.Owner )
        {
            this.talent = talent;
            this.effectDamageTaken = new DamageTakenFromSchoolEffect() { DamageSchool = DamageSchool.Physical };
            this.effectDamageDone  = new DamageDoneWithSchoolEffect() { 
                DamageSchool = DamageSchool.All,
                Value = ShieldWallTalent.DamageDoneReducement
            };

            this.effectMoveSpeed = new MovementSpeedEffect( ShieldWallTalent.MovementSpeedReducement, StatusManipType.Percental );

            this.aura = new PermanentAura( new StatusEffect[3] { effectDamageTaken, effectDamageDone, effectMoveSpeed } ) {
                DescriptionProvider = talent,
                Name                = talent.LocalizedName,
                IsVisible           = true,
                Symbol              = talent.Symbol
            };
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Refreshes the strength of this ShieldWallSkill based on the Talents the player has choosen.
        /// </summary>
        public override void RefreshDataFromTalents()
        {
            bool removed = this.AuraList.Remove( aura );

            effectDamageTaken.Value = talent.DamageTakenReducement;

            if( removed )
                this.AuraList.Add( aura );
        }

        /// <summary>
        /// Uses this ShieldWallSkill.
        /// </summary>
        /// <returns>
        /// true if this ShieldWallSkill has been used;
        /// otherwise false.
        /// </returns>
        protected override bool Fire()
        {
            if( this.aura.IsEnabled )
            {
                this.AuraList.Remove( this.aura );
            }
            else
            {
                this.AuraList.Add( aura );
            }

            this.Cooldown.Reset();
            return true;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the ShieldWallTalent that modifies the strength of this ShieldWallSkill.
        /// </summary>
        private readonly ShieldWallTalent talent;

        /// <summary>
        /// The DamageTakenModEffect that is applied by this ShieldWallSkill.
        /// </summary>
        private readonly DamageTakenFromSchoolEffect effectDamageTaken;

        /// <summary>
        /// The DamageDoneModEffect that is applied by this ShieldWallSkill.
        /// </summary>
        private readonly DamageDoneWithSchoolEffect effectDamageDone;

        /// <summary>
        /// The MovementSpeedEffect that is applied by this ShieldWallSkill.
        /// </summary>
        private readonly MovementSpeedEffect effectMoveSpeed;

        /// <summary>
        /// The PermanentAura that holds the StatusEffects this ShieldWallSkill provides.
        /// </summary>
        private readonly PermanentAura aura;

        #endregion
    }
}
