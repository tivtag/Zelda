// <copyright file="Statable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Statable class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    using System;
    using Atom;
    using Atom.Math;
    using Zelda.Attacks;
    using Zelda.Entities;
    using Zelda.Entities.Components;
    using Zelda.Status.Containers;
    using Zelda.Status.Damage.Containers;
    
    /// <summary>
    /// Defines the <see cref="ZeldaComponent"/> that holds
    /// the RPG-related status information of the Entity that owns it.
    /// </summary>
    public class Statable : ZeldaComponent, IZeldaSetupable
    {
        #region [ Events ]

        /// <summary>
        /// Fired when the <see cref="ZeldaEntity"/> has died.
        /// </summary>
        public event SimpleEventHandler<Statable> Died;

        /// <summary>
        /// Fired when the statable <see cref="ZeldaEntity"/> has been damaged.
        /// </summary>
        public event RelaxedEventHandler<Statable, AttackDamageResult> Damaged, DamagedMana;

        /// <summary>
        /// Fired when the power of the statable <see cref="ZeldaEntity"/> has been restored (partially).
        /// </summary>
        public event RelaxedEventHandler<Statable, AttackDamageResult> Restored, RestoredMana;

        #region > Combat Notifiers <

        /// <summary>
        /// Fired when the <see cref="ZeldaEntity"/> managed to hit something.
        /// </summary>
        public event SimpleEventHandler<Statable> MeleeHit, RangedHit, MagicHit;

        /// <summary>
        /// Fired when the <see cref="ZeldaEntity"/> hits something with a default attack.
        /// </summary>
        public event RelaxedEventHandler<Statable, CombatEventArgs> DefaultMeleeHit, DefaultRangedHit;

        /// <summary>
        /// Fired when the <see cref="ZeldaEntity"/> crits something with a default attack.
        /// </summary>
        public event RelaxedEventHandler<Statable, CombatEventArgs> DefaultMeleeCrit;

        /// <summary>
        /// Fired when the <see cref="ZeldaEntity"/> managed to critically hit something.
        /// </summary>
        public event SimpleEventHandler<Statable> MeleeCrit, RangedCrit, MagicCrit;

        /// <summary>
        /// Fired when the <see cref="ZeldaEntity"/> got hit.
        /// </summary>
        public event SimpleEventHandler<Statable> GotMeleeHit, GotRangedHit, GotMagicHit;

        /// <summary>
        /// Fired when the <see cref="ZeldaEntity"/> got critically hit.
        /// </summary>
        public event SimpleEventHandler<Statable> GotMeleeCrit, GotRangedCrit, GotMagicCrit;

        /// <summary>
        /// Fired when the <see cref="ZeldaEntity"/> blocked an attack.
        /// </summary>
        public event SimpleEventHandler<Statable> Blocked;

        /// <summary>
        /// Fired when one of the attacks of the <see cref="ZeldaEntity"/> got blocked.
        /// </summary>
        public event SimpleEventHandler<Statable> GotBlocked;

        #endregion

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets a value indicates whether
        /// this <see cref="Statable"/> is 'friendly' towards the player.
        /// </summary>
        /// <value>The default value is null.</value>
        public bool IsFriendly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the object that encapsulates the various
        /// damage done properties of the statable entity.
        /// </summary>
        public DamageDoneContainer DamageDone
        {
            get 
            {
                return this.damageDone;
            }
        }

        /// <summary>
        /// Gets the object that encapsulates the various
        /// damage taken properties.
        /// </summary>
        public DamageTakenContainer DamageTaken
        {
            get
            {
                return this.damageTaken;
            }
        }

        /// <summary>
        /// Gets the object that encapsulates various
        /// chance-to properties.
        /// </summary>
        public ChanceToContainer ChanceTo
        {
            get
            {
                return this.chanceTo;
            }
        }

        /// <summary>
        /// Gets the object that encapsulates various
        /// chance-to-be properties.
        /// </summary>
        public ChanceToBeContainer ChanceToBe
        {
            get
            {
                return this.chanceToBe;
            }
        }

        /// <summary>
        /// Gets the object that encapsulates the chances to resist 
        /// spells or attacks of a specific ElementSchool.
        /// </summary>
        public SpellResistanceContainer Resistances
        {
            get
            {
                return this.resistances;
            }
        }

        #region AuraList

        /// <summary>
        /// Gets the list of <see cref="Aura"/>s active on this <see cref="ZeldaEntity"/>.
        /// </summary>
        public AuraList AuraList
        {
            get 
            {
                return this.auraList; 
            }
        }

        #endregion

        #region Race

        /// <summary>
        /// Gets or sets the race of the <see cref="ZeldaEntity"/>.
        /// </summary>
        public RaceType Race
        {
            get
            {
                return this.race;
            }

            set 
            {
                this.race = value;
            }
        }

        #endregion

        #region Level

        /// <summary>
        /// Gets or sets the level of the status object.
        /// </summary>
        public virtual int Level
        {
            get
            { 
                return this.level;
            }

            set 
            {
                this.level = value;
            }
        }

        #endregion

        #region Armor

        /// <summary>
        /// Gets or sets the total armor of the statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public int Armor
        {
            get
            { 
                return this.armor;
            }

            protected set 
            { 
                this.armor = value;
            }
        }

        /// <summary>
        /// Gets or sets the base armor of the statable <see cref="ZeldaEntity"/>.
        /// </summary>
        public int BaseArmor
        {
            get
            {
                return this.baseArmor;
            }

            set
            {
                this.baseArmor = value;
                this.armor     = value;
            }
        }

        #endregion

        #region Attack Speed

        /// <summary>
        /// Gets or sets the melee attack speed (also called delay) of this status object.
        /// </summary>
        public float AttackSpeedMelee
        {
            get { return attackSpeedMelee; }
            set { attackSpeedMelee = value; }
        }

        /// <summary>
        /// Gets or sets the melee ranged speed (also called delay) of this status object.
        /// </summary>
        public float AttackSpeedRanged
        {
            get { return attackSpeedRanged; }
            set { attackSpeedRanged = value; }
        }

        /// <summary>
        /// Gets the modifier value that is applied to the cast-time
        /// of a spell to get the actual cast-time.
        /// </summary>
        public float CastTimeModifier
        {
            get { return this.castTimeModifier; }            
            set { this.castTimeModifier = value; }
        }

        #endregion

        #region CritModifier

        /// <summary>
        /// Gets or sets the value an attack is multiplied with when it crits. 
        /// </summary>
        public float CritModifierMelee
        {
            get 
            { 
                return this.damageDone.WithCritical.Melee;
            }
        }

        /// <summary>
        /// Gets or sets the value an attack is multiplied with when it crits. 
        /// </summary>
        public float CritModifierRanged
        {
            get
            {
                return this.damageDone.WithCritical.Ranged; 
            }
        }

        /// <summary>
        /// Gets or sets the value an attack is multiplied with when it crits. 
        /// </summary>
        public float CritModifierSpell
        {
            get
            { 
                return this.damageDone.WithCritical.Spell; 
            }
        }

        /// <summary>
        /// Gets the value a healing effect is multiplied with when it crits.
        /// </summary>
        public float CritModifierHeal
        {
            get 
            {
                return this.CritModifierMelee + 0.20f;
            }
        }
        
        /// <summary>
        /// Gets the value a block effect is multiplied with when it crits.
        /// </summary>
        public float CritModifierBlock
        {
            get
            {
                return (this.CritModifierMelee + CritModifierRanged) / 2.0f;
            }
        }

        #endregion

        #region DamageMelee

        /// <summary>
        /// Gets or sets the physcial damage this <see cref="ZeldaEntity"/> can do in melee-range. 
        /// </summary>
        public int DamageMeleeMin
        {
            get { return damageMeleeMin; }
            set { damageMeleeMin = value; }
        }

        /// <summary>
        /// Gets or sets the physcial damage this <see cref="ZeldaEntity"/> can do in melee-range. 
        /// </summary>
        public int DamageMeleeMax
        {
            get { return damageMeleeMax; }
            set { damageMeleeMax = value; }
        }

        #endregion

        #region DamageRanged

        /// <summary>
        /// Gets or sets the physcial damage this <see cref="ZeldaEntity"/> can do in ranged combat. 
        /// </summary>
        public int DamageRangedMin
        {
            get { return damageRangedMin; }
            set { damageRangedMin = value; }
        }

        /// <summary>
        /// Gets or sets the physcial damage this <see cref="ZeldaEntity"/> can do in ranged combat. 
        /// </summary>
        public int DamageRangedMax
        {
            get { return damageRangedMax; }
            set { damageRangedMax = value; }
        }

        #endregion
        
        /// <summary>
        /// Gets or sets the magical damage bonus this <see cref="ZeldaEntity"/> has. 
        /// </summary>
        public IntegerRange DamageMagic
        {
            get;
            set;
        }
        
        #region > Life <

        /// <summary>
        /// Gets or sets the current life of this <see cref="ZeldaEntity"/>.
        /// </summary>
        public int Life
        {
            get
            {
                return this.life;
            }

            set
            {
                this.life = value;

                if( this.life < 0 )
                {
                    this.life = 0;
                }
                else if( this.life > this.maximumLife )
                {
                    this.life = this.maximumLife;
                }
            }
        }

        /// <summary>
        /// Gets or sets the base value the <see cref="MaximumLife"/> value is based on.
        /// </summary>
        public int BaseMaximumLife
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum life of this <see cref="ZeldaEntity"/>.
        /// </summary>
        public int MaximumLife
        {
            get
            {
                return this.maximumLife;
            }

            set
            {
                this.maximumLife = value;

                if( this.life > this.maximumLife )
                {
                    this.life = this.maximumLife;
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount of life regenerated each regen-tick.
        /// </summary>
        public int LifeRegeneration
        {
            get
            {
                return this.lifeRegeneration;
            }

            set
            {
                this.lifeRegeneration = value;
            }
        }

        #endregion

        #region > Mana <

        /// <summary>
        /// Gets or sets the current mana of this <see cref="ZeldaEntity"/>.
        /// </summary>
        public int Mana
        {
            get
            {
                return this.mana;
            }

            set
            {
                this.mana = value;

                if( this.mana < 0 )
                {
                    this.mana = 0;
                }
                else if( this.mana > this.maximumMana )
                {
                    this.mana = this.maximumMana;
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum mana of this <see cref="ZeldaEntity"/>.
        /// </summary>
        public int MaximumMana
        {
            get
            {
                return this.maximumMana;
            }

            set
            {
                this.maximumMana = value;

                if( this.mana > this.maximumMana )
                {
                    this.mana = this.maximumMana;
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount of life regenerated each regen-tick.
        /// </summary>
        public int ManaRegeneration
        {
            get
            {
                return this.manaRegeneration;
            }

            set
            {
                this.manaRegeneration = value;
            }
        }

        #endregion

        #region > Blocking <

        /// <summary>
        ///  Gets or sets the value that states whether the <see cref="ZeldaEntity"/> can block attacks.
        /// </summary>
        public bool CanBlock 
        {
            get
            { 
                return this.canBlock;
            }

            set 
            {
                this.canBlock = value;
            } 
        }

        /// <summary>
        /// Gets or sets the value that stores the chance for the <see cref="ZeldaEntity"/> to block an attack.
        /// Only relevant if canBlock is true.
        /// </summary>
        public float ChanceToBlock
        {
            get 
            { 
                return this.chanceTo.Block;
            }
        }

        /// <summary>
        /// Gets or sets the value that stores the block 'power' of the <see cref="ZeldaEntity"/> - aka how much damage is blocked.
        /// Only relevant if canBlock is true.
        /// </summary>
        public int BlockValue
        {
            get 
            {
                return this.blockValue; 
            }

            set
            {
                this.blockValue = value; 
            }
        }

        #endregion
        
        /// <summary>
        /// Gets a value indicating whether the statable ZeldaEntity is dead.
        /// </summary>
        public bool IsDead
        {
            get 
            { 
                return this.life <= 0; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this Statable is invincible.
        /// </summary>
        public bool IsInvincible
        {
            get 
            { 
                return this.isInvincible; 
            }

            set 
            { 
                this.isInvincible = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this Statable is temporary invincible.
        /// </summary>
        public bool IsTempInvincible
        {
            get 
            { 
                return this.isTempInvincible;
            }
        }

        #endregion

        #region [ Initialization ]
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Statable"/> class.
        /// </summary>
        public Statable()
            : this( new DamageDoneContainer(), new ChanceToContainer() , new ChanceToBeContainer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Statable"/> class.
        /// </summary>
        /// <param name="damageDoneContainer">
        /// The DamageDoneContainer that should be injected into the new Statable component.
        /// </param>
        /// <param name="chanceToContainer">
        /// The ChanceToContainer that should be injected into the new Statable component.
        /// </param>
        /// <param name="chanceToBeContainer">
        /// The ChanceToBeContainer that should be injected into the new Statable component.
        /// </param>
        protected Statable( DamageDoneContainer damageDoneContainer, ChanceToContainer chanceToContainer, ChanceToBeContainer chanceToBeContainer )
        {
            this.chanceTo = chanceToContainer;
            this.damageDone = damageDoneContainer;
            this.chanceToBe = chanceToBeContainer;

            this.auraList = new AuraList( this );
            this.damageTaken = new DamageTakenContainer();
            this.resistances = new SpellResistanceContainer( this );

            this.damageDone.Initialize( this );
            this.damageTaken.Initialize( this );
            this.chanceTo.Initialize( this );
            this.chanceToBe.Initialize( this );
        }

        /// <summary>
        /// Setups this Statable component.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public virtual void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.rand = serviceProvider.Rand;

            this.resistances.Setup( serviceProvider );
        }

        #endregion

        #region [ Methods ]

        #region > Try <

        /// <summary>
        /// Rolls the dice; deciding whether an attack has hit or missed.
        /// </summary>
        /// <param name="target">
        /// The target of the attack.
        /// </param>
        /// <returns>
        /// true if this Statable has missed the specified <paramref name="target"/> Statable;
        /// otherwise false if it has hit.
        /// </returns>
        internal bool TryHit( Statable target )
        {
            float missChance = this.chanceTo.Miss - target.chanceToBe.Hit;
            float roll = this.rand.RandomRange( 0.0f, 100.0f );

            return roll <= missChance;
        }

        /// <summary>
        /// Rolls the dice; deciding whether an attack against this Statable was dodged.
        /// </summary>
        /// <param name="attacker">
        /// The Statable that is attacking this Statable.
        /// </param>
        /// <returns>
        /// true if this Statable has dodged the specified <paramref name="attacker"/> Statable;
        /// otherwise false.
        /// </returns>
        internal bool TryDodge( Statable attacker )
        {
            float dodgeChance = this.chanceTo.Dodge;
            float roll = this.rand.RandomRange( 0.0f, 100.0f );

            return roll <= dodgeChance;
        }

        /// <summary>
        /// Rolls the dice; deciding whether an attack against this Statable was parried.
        /// </summary>
        /// <param name="attacker">
        /// The Statable that is attacking this Statable.
        /// </param>
        /// <returns>
        /// true if this Statable has parried the specified <paramref name="attacker"/> Statable;
        /// otherwise false.
        /// </returns>
        internal bool TryParry( Statable attacker )
        {
            float parryChance = this.chanceTo.Parry;
            float roll = this.rand.RandomRange( 0.0f, 100.0f );

            return roll <= parryChance;
        }
        
        /// <summary>
        /// Rolls the dice; deciding whether an attack against
        /// the specified <paramref name="target"/> Statable has been a critical attack.
        /// </summary>
        /// <param name="target">
        /// The target of the attack.
        /// </param>
        /// <returns>
        /// true if the attack of this Statable has been a critical attack
        /// against the specified <paramref name="target"/> Statable;
        /// otherwise false.
        /// </returns>
        internal bool TryCrit( Statable target )
        {
            float critChance = this.chanceTo.Crit + target.chanceToBe.Crit;
            float roll = this.rand.RandomRange( 0.0f, 100.0f );

            return roll <= critChance;
        }

        /// <summary>
        /// Rolls the dice; deciding whether an attack against
        /// the specified <paramref name="target"/> Statable has been a critical attack.
        /// </summary>
        /// <param name="target">
        /// The target of the attack.
        /// </param>
        /// <param name="extraCritChance">
        /// The extra chance the attack will be a critical attack (in percent).
        /// </param>
        /// <returns>
        /// true if the attack of this Statable has been a critical attack
        /// against the specified <paramref name="target"/> Statable;
        /// otherwise false.
        /// </returns>
        internal bool TryCrit( Statable target, float extraCritChance )
        {
            float critChance = this.chanceTo.Crit + extraCritChance + target.chanceToBe.Crit;
            float roll = this.rand.RandomRange( 0.0f, 100.0f );

            return roll <= critChance;
        }

        #endregion

        #region > Updating <

        /// <summary>
        /// Updates this <see cref="Statable"/> component-
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        public override void Update( Atom.IUpdateContext updateContext )
        {
            var zeldaUpdateContext = (ZeldaUpdateContext)updateContext;

            auraList.Update( zeldaUpdateContext );
           
            UpdateRegenaration( zeldaUpdateContext );
            UpdateInvincible( zeldaUpdateContext );
        }

        /// <summary>
        /// Updates the IsInvincible state of the ZeldaEntity
        /// if isInvincibleTicking is true.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void UpdateInvincible( ZeldaUpdateContext updateContext )
        {
            if( isTempInvincible )
            {
                invincibleTick -= updateContext.FrameTime;

                if( invincibleTick <= 0.0f )
                {
                    isTempInvincible = false;
                    isInvincible     = false;
                }
            }
        }

        /// <summary>
        /// Regenerates the life/mana of the ZeldaEntity.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        private void UpdateRegenaration( ZeldaUpdateContext updateContext )
        {
            this.regenTick -= updateContext.FrameTime;

            if( this.regenTick <= 0.0f )
            {
                this.regenTick = RegenTime;

                if( !this.IsDead )
                {
                    this.Life += this.lifeRegeneration;
                    this.Mana += this.manaRegeneration;
                }
            }
        }

        #endregion

        #region > Get <
        
        /// <summary>
        /// Calculates the mitigation multiplier a physical attack of this statable
        /// against the given <paramref name="target"/>.
        /// </summary>
        /// <param name="target">
        /// The target of the physical attack.
        /// </param>
        /// <returns>
        /// The mitigation multiplier that simply gets applied to the damage.
        /// </returns>
        internal float GetPhysicalMitigationOf( Statable target )
        {
            return this.GetPhysicalMitigationOf( target.Armor, target.Level );
        }

        /// <summary>
        /// Calculates the mitigation multiplier a physical attack of this statable
        /// against a target with the given <paramref name="targetArmor"/> and <paramref name="targetLevel"/>.
        /// </summary>
        /// <param name="targetArmor">
        /// The armor of the target of the physical attack.
        /// </param>
        /// <param name="targetLevel">
        /// The level of the target of the physical attack.
        /// </param>
        /// <returns>
        /// The mitigation multiplier that simply gets applied to the damage.
        /// </returns>
        internal virtual float GetPhysicalMitigationOf( int targetArmor, int targetLevel )
        {
            float mitigation = StatusCalc.GetMitigationFromArmor( targetArmor, targetLevel );
            return 1.0f - (mitigation / 100.0f);
        }

        /// <summary>
        /// Gets the given percentage of the specified power type of the extended-statable ZeldaEntity.
        /// </summary>
        /// <param name="powerType">
        /// The power type.
        /// </param>
        /// <param name="percentage">
        /// The percentage to get; where 1 equals 100%.
        /// </param>
        /// <returns>The requested value.</returns>
        public int GetPercentageOf( LifeMana powerType, float percentage )
        {
            switch( powerType )
            {
                case LifeMana.Life:
                    return (int)(this.MaximumLife * percentage);

                case LifeMana.Mana:
                    return (int)(this.MaximumMana * percentage);

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Helpers method that gets how much of the given power type
        /// is needed to be restored before the statable ZeldaEntity
        /// is fully restored.
        /// </summary>
        /// <param name="powerType">
        /// The power type to query.
        /// </param>
        /// <returns>
        /// The amount needed to restore.
        /// </returns>
        internal int GetRestoreNeeded( LifeMana powerType )
        {
            switch( powerType )
            {
                case LifeMana.Life:
                    return this.MaximumLife - this.Life;

                case LifeMana.Mana:
                    return this.MaximumMana - this.Mana;

                default:
                    throw new NotSupportedException();
            }
        }

        #endregion

        #region > Lose <

        /// <summary>
        /// Reduces the life of the <see cref="ZeldaEntity"/> given the given
        /// <see cref="AttackDamageResult"/>.
        /// </summary>
        /// <param name="result">
        /// </param>
        public void LoseLife( AttackDamageResult result )
        {
            if( this.IsInvincible || this.life <= 0 )
                return;

            this.Life -= result.Damage;

            if( Damaged != null )
                Damaged( this, result );

            if( this.life <= 0 )
                OnDied();
        }

        /// <summary>
        /// Reduces the mana of the <see cref="ZeldaEntity"/> by the amount
        /// encapsulates by the given <see cref="AttackDamageResult"/>.
        /// </summary>
        /// <param name="result">
        /// The AttackDamageResult encapsulating the amount of mana the 
        /// entity should lose.
        /// </param>
        public void LoseMana( AttackDamageResult result )
        {
            this.Mana -= result.Damage;

            if( this.DamagedMana != null )
                this.DamagedMana( this, result );
        }

        /// <summary>
        /// Reduces the mana of the <see cref="ZeldaEntity"/> by the given amount .
        /// </summary>
        /// <param name="amount">
        /// The amount of mana</param>
        internal void LoseMana( int amount )
        {
            this.LoseMana( new AttackDamageResult( amount, AttackReceiveType.Hit ) );
        }

        #endregion

        #region > Restore <

        /// <summary>
        /// Restores the ZeldaEntity's Power by the amount specified 
        /// with the given <see cref="AttackDamageResult"/>.
        /// </summary>
        /// <param name="powerType">
        /// The power to restore.
        /// </param>
        /// <param name="result">
        /// The structure that encapsulates the amount restored.
        /// </param>
        public void Restore( LifeMana powerType, AttackDamageResult result )
        {
            switch( powerType )
            {
                case LifeMana.Life:
                    this.RestoreLife( result );
                    break;

                case LifeMana.Mana:
                    this.RestoreMana( result );
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Heals the ZeldaEntity by the by the amount specified 
        /// with the given <see cref="AttackDamageResult"/>.
        /// </summary>
        /// <param name="result">
        /// The structure that encapsulates the amount restored.
        /// </param>
        public void RestoreLife( AttackDamageResult result )
        {
            if( result.Damage < 0 )
                throw new ArgumentException( Zelda.Resources.Error_CantRestoreForZeroOrLessThanZero, "result" );
            
            this.Life += result.Damage;

            if( this.Restored != null )
                this.Restored( this, result );
        }

        /// <summary>
        /// Restores the ZeldaEntity's mana by the amount specified 
        /// with the given <see cref="AttackDamageResult"/>.
        /// </summary>
        /// <param name="result">
        /// The structure that encapsulates the amount restored.
        /// </param>
        public void RestoreMana( AttackDamageResult result )
        {
            if( result.Damage < 0 )
                throw new ArgumentException( Zelda.Resources.Error_CantRestoreForZeroOrLessThanZero, "result" );

            this.Mana += result.Damage;
            if( this.RestoredMana != null )
                this.RestoredMana( this, result );
        }

        /// <summary>
        /// Restores the ZeldaEntity's mana by the specified amount.
        /// </summary>
        /// <param name="amount">
        /// The amount of mana to restore.
        /// </param>
        public void RestoreMana( int amount )
        {
            this.RestoreMana( new AttackDamageResult( amount, AttackReceiveType.Hit ) );
        }

        /// <summary>
        /// Fully restores the life and mana of this statable ZeldaEntity.
        /// </summary>
        public void RestoreFully()
        {
            this.Life = this.MaximumLife;
            this.Mana = this.MaximumMana;
        }

        #endregion

        #region > Blocking <

        /// <summary>
        /// Helper method that returns whether an attack was blocked,
        /// based on the <see cref="ChanceToBlock"/> and <see cref="CanBlock"/> properties.
        /// </summary>
        /// <param name="attacker">
        /// The statable component of the Entity that is attacking this statable Entity.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <returns>
        /// True if the attack was blocked;
        /// otherwise false.
        /// </returns>
        public bool TryBlock( Statable attacker, RandMT rand )
        {
            if( this.canBlock )
            {
                if( rand.RandomRange( 0.0f, 100.0f ) <= this.ChanceToBlock )
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Calculates the damage done to the statable Entity, 
        /// after blocking the attack.
        /// </summary>
        /// <param name="damage">
        /// The input damage.
        /// </param>
        /// <param name="attacker">
        /// The statable component of the Entity that is attacking this statable Entity.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        /// <returns>
        /// The blocked output damage.
        /// </returns>
        public int HandleBlock( int damage, Statable attacker, RandMT rand )
        {
            int blockedDamage = rand.RandomRange( blockValue / 2, blockValue );

            // Crit Block
            if( rand.RandomRange( 0.0f, 100.0f ) <= this.ChanceTo.CritBlock )
            {
                blockedDamage = (int)(blockedDamage * this.CritModifierBlock);
            }

            int result = damage - blockedDamage;
            if( result <= 0 )
                result = 1;

            this.OnBlocked();
            attacker.OnGotBlocked();

            return result;
        }

        #endregion

        #region > Refresh <

        /// <summary>
        /// Refreshes the Movement Speed of the statable <see cref="ZeldaEntity"/>
        /// by taking into account the <see cref="MovementSpeedEffect"/>s currently active
        /// on the ZeldaEntity.
        /// </summary>
        /// <seealso cref="MovementSpeedEffect"/>
        internal virtual void Refresh_MovementSpeed()
        {
            var moveable = this.Owner.Components.Get<Zelda.Entities.Components.Moveable>();
            if( moveable == null )
                return;

            float addValue, mulValue, ratingValue;
            this.auraList.GetEffectValues( MovementSpeedEffect.IdentifierString, out addValue, out mulValue, out ratingValue );            
           
            moveable.Speed = StatusCalc.GetMovementSpeed( moveable.BaseSpeed, addValue, mulValue, ratingValue, this.Level );
        }

        /// <summary>
        /// Refreshes the total <see cref="Armor"/> of the statable <see cref="ZeldaEntity"/>
        /// by taking into account the <see cref="ArmorEffect"/>s currently active
        /// on the ZeldaEntity.
        /// </summary>
        /// <seealso cref="ArmorEffect"/>
        internal virtual void Refresh_TotalArmor()
        {
            float addValue, mulValue;
            this.auraList.GetEffectValues( ArmorEffect.IdentifierString, out addValue, out mulValue );

            this.armor = (int)((baseArmor + addValue) * mulValue);
            
            if( this.armor < 0 )
            {
                this.armor = 0;
            }
        }
        
        /// <summary>
        /// Refreshes the MaximumLife of this statable entity.
        /// </summary>
        internal virtual void Refresh_TotalLife()
        {
            float addValue, mulValue;
            this.AuraList.GetEffectValues( "Life", out addValue, out mulValue );

            this.MaximumLife = (int)((this.BaseMaximumLife + addValue ) * mulValue);
        }

        #endregion

        #region > Events <

        #region OnDied

        /// <summary>
        /// Triggers the <see cref="Died"/> event.
        /// </summary>
        protected void OnDied()
        {
            if( this.Died != null )
            {
                this.Died( this );
            }
        }

        #endregion

        #region OnHit

        /// <summary>
        /// Triggers the <see cref="MeleeHit"/> event.
        /// Should only be used from inside an <see cref="AttackDamageMethod"/>.
        /// </summary>
        public void OnMeleeHit()
        {
            if( this.MeleeHit != null )
                this.MeleeHit( this );
        }

        /// <summary>
        /// Triggers the <see cref="RangedHit"/> event.
        /// Should only be used from inside an <see cref="AttackDamageMethod"/>.
        /// </summary>
        public void OnRangedHit()
        {
            if( this.RangedHit != null )
                this.RangedHit( this );
        }

        /// <summary>
        /// Triggers the <see cref="MagicHit"/> event.
        /// Should only be used from inside an <see cref="AttackDamageMethod"/>.
        /// </summary>
        public void OnMagicHit()
        {
            if( this.MagicHit != null )
                this.MagicHit( this );
        }

        #endregion

        #region OnNormal

        /// <summary>
        /// Raises the <see cref="DefaultMeleeHit"/> event.
        /// Should only be used from inside an <see cref="AttackDamageMethod"/>.
        /// </summary>
        /// <param name="target">
        /// The target that got hit.
        /// </param>
        internal void OnNormalMeleeHit( Statable target )
        {
            if( this.DefaultMeleeHit != null )
                this.DefaultMeleeHit( this, new CombatEventArgs( this, target ) );

            if( this.MeleeHit != null )
                this.MeleeHit( this );
        }

        /// <summary>
        /// Raises the <see cref="DefaultMeleeCrit"/> and <see cref="MeleeCrit"/> events.
        /// Should only be used from inside an <see cref="AttackDamageMethod"/>.
        /// </summary>
        /// <param name="target">
        /// The target that got hit.
        /// </param>
        internal void OnNormalMeleeCrit( Statable target )
        {
            if( this.DefaultMeleeCrit != null )
                this.DefaultMeleeCrit( this, new CombatEventArgs( this, target ) );

            if( this.MeleeCrit != null )
                this.MeleeCrit( this );
        }

        /// <summary>
        /// Triggers the <see cref="DefaultRangedHit"/> event.
        /// Should only be used from inside an <see cref="AttackDamageMethod"/>.
        /// </summary>
        /// <param name="target">
        /// The target that got hit.
        /// </param>
        internal void OnNormalRangedHit( Statable target )
        {
            if( this.DefaultRangedHit != null )
                this.DefaultRangedHit( this, new CombatEventArgs( this, target ) );

            if( this.RangedHit != null )
                this.RangedHit( this );
        }

        #endregion

        #region OnCrit

        /// <summary>
        /// Triggers the <see cref="MeleeCrit"/> event.
        /// Should only be used from inside an <see cref="AttackDamageMethod"/>.
        /// </summary>
        public void OnMeleeCrit()
        {
            if( this.MeleeCrit != null )
                this.MeleeCrit( this );
        }

        /// <summary>
        /// Triggers the <see cref="RangedCrit"/> event.
        /// Should only be used from inside an <see cref="AttackDamageMethod"/>.
        /// </summary>
        public void OnRangedCrit()
        {
            if( this.RangedCrit != null )
                this.RangedCrit( this );
        }

        /// <summary>
        /// Triggers the <see cref="MagicCrit"/> event.
        /// Should only be used from inside an <see cref="AttackDamageMethod"/>.
        /// </summary>
        public void OnMagicCrit()
        {
            if( this.MagicCrit != null )
                this.MagicCrit( this );
        }

        #endregion

        #region OnGotHit

        /// <summary>
        /// Triggers the <see cref="GotMeleeHit"/> event.
        /// Should only be used from inside an <see cref="AttackDamageMethod"/>.
        /// </summary>
        public void OnGotMeleeHit()
        {
            if( this.GotMeleeHit != null )
                this.GotMeleeHit( this );
        }

        /// <summary>
        /// Triggers the <see cref="GotRangedHit"/> event.
        /// Should only be used from inside an <see cref="AttackDamageMethod"/>.
        /// </summary>
        public void OnGotRangedHit()
        {
            if( this.GotRangedHit != null )
                this.GotRangedHit( this );
        }

        /// <summary>
        /// Triggers the <see cref="GotMagicHit"/> event.
        /// Should only be used from inside an <see cref="AttackDamageMethod"/>.
        /// </summary>
        public void OnGotMagicHit()
        {
            if( this.GotMagicHit != null )
                this.GotMagicHit( this );
        }

        #endregion

        #region OnGotCrit

        /// <summary>
        /// Triggers the <see cref="GotMeleeCrit"/> event.
        /// Should only be used from inside an <see cref="AttackDamageMethod"/>.
        /// </summary>
        public void OnGotMeleeCrit()
        {
            if( this.GotMeleeCrit != null )
                this.GotMeleeCrit( this );
        }

        /// <summary>
        /// Triggers the <see cref="GotRangedCrit"/> event.
        /// Should only be used from inside an <see cref="AttackDamageMethod"/>.
        /// </summary>
        public void OnGotRangedCrit()
        {
            if( this.GotRangedCrit != null )
                this.GotRangedCrit( this );
        }

        /// <summary>
        /// Triggers the <see cref="GotMagicCrit"/> event.
        /// Should only be used from inside an <see cref="AttackDamageMethod"/>.
        /// </summary>
        public void OnGotMagicCrit()
        {
            if( this.GotMagicCrit != null )
                this.GotMagicCrit( this );
        }

        #endregion

        /// <summary>
        /// Triggers the <see cref="Blocked"/> event.
        /// </summary>
        private void OnBlocked()
        {
            if( this.Blocked != null )
            {
                this.Blocked( this );
            }
        }

        /// <summary>
        /// Triggers the <see cref="GotBlocked"/> event.
        /// </summary>
        private void OnGotBlocked()
        {
            if( this.GotBlocked != null )
            {
                this.GotBlocked( this );
            }
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Setups the given <see cref="Statable"/> component to be a clone of this <see cref="Statable"/> component.
        /// </summary>
        /// <param name="clone">
        /// The object to setup as a clone.
        /// </param>
        public void SetupClone( Statable clone )
        {
            clone.Armor     = this.Armor;
            clone.BaseArmor = this.BaseArmor;

            clone.race = this.race;
            clone.attackSpeedMelee = this.attackSpeedMelee;
            clone.attackSpeedRanged = this.attackSpeedRanged;

            clone.damageMeleeMax  = this.damageMeleeMax;
            clone.damageMeleeMin  = this.damageMeleeMin;
            clone.damageRangedMin = this.damageRangedMin;
            clone.damageRangedMax = this.damageRangedMax;
            clone.DamageMagic = this.DamageMagic;

            clone.isInvincible = this.isInvincible;
            clone.level = this.level;
            clone.maximumLife = clone.BaseMaximumLife = this.BaseMaximumLife;
            clone.maximumMana = this.maximumMana;
            clone.lifeRegeneration = this.lifeRegeneration;
            clone.manaRegeneration = this.manaRegeneration;
            clone.life = this.life;
            clone.mana = this.mana;

            this.chanceTo.SetupClone( clone.chanceTo );
            this.chanceToBe.SetupClone( clone.chanceToBe );
            this.damageDone.SetupClone( clone.damageDone );
            this.damageTaken.SetupClone( clone.damageTaken );
            this.resistances.SetupClone( clone.resistances );
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
        public virtual void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 9;
            context.Write( CurrentVersion );

            context.Write( this.Level );
            context.Write( (int)this.Race );

            context.Write( this.BaseMaximumLife );
            context.Write( this.LifeRegeneration );

            context.Write( this.MaximumMana );
            context.Write( this.ManaRegeneration );

            context.Write( this.BaseArmor );
            context.Write( this.IsInvincible );

            this.chanceTo.Serialize( context ); // New in V8.
            this.chanceToBe.Serialize( context ); // New in V9.

            context.Write( this.DamageMeleeMin );
            context.Write( this.DamageMeleeMax );
            context.Write( this.DamageRangedMin );
            context.Write( this.DamageRangedMax );
            context.Write( this.DamageMagic );

            context.Write( this.AttackSpeedMelee );
            context.Write( this.AttackSpeedRanged );

            this.resistances.Serialize( context );
            this.damageTaken.Serialize( context );
            this.damageDone.Serialize( context );

            context.Write( this.CanBlock );
            context.Write( this.BlockValue );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int CurrentVersion = 9;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.Level = context.ReadInt32();
            this.Race = (RaceType)context.ReadInt32();

            this.Life = this.MaximumLife = this.BaseMaximumLife = context.ReadInt32();
            this.LifeRegeneration         = context.ReadInt32();
            this.Mana = this.MaximumMana  = context.ReadInt32();
            this.ManaRegeneration         = context.ReadInt32();

            this.Armor = this.BaseArmor   = context.ReadInt32();
            this.IsInvincible             = context.ReadBoolean();

            this.chanceTo.Deserialize( context );
            this.chanceToBe.Deserialize( context );

            this.DamageMeleeMin = context.ReadInt32();
            this.DamageMeleeMax = context.ReadInt32();
            this.DamageRangedMin = context.ReadInt32();
            this.DamageRangedMax = context.ReadInt32();
            this.DamageMagic = context.ReadIntegerRange();

            this.AttackSpeedMelee  = context.ReadSingle();
            this.AttackSpeedRanged = context.ReadSingle();

            this.resistances.Deserialize( context );
            this.damageTaken.Deserialize( context );
            this.damageDone.Deserialize( context );
            
            this.CanBlock = context.ReadBoolean();
            this.BlockValue = context.ReadInt32();
        }

        #endregion

        #region > Misc <

        /// <summary>
        /// Makes the statable ZeldaEntity invincible for
        /// the given amount of <paramref name="time"/>.
        /// </summary>
        /// <param name="time">
        /// The time in seconds the statable ZeldaEntity should go invincible.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="time"/> is negative.
        /// </exception>
        public void MakeTempInvincible( float time )
        {
            if( time < 0.0f )
                throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "time" );

            this.isInvincible     = true;
            this.isTempInvincible = true;
            this.invincibleTick   = time;
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The race of the <see cref="ZeldaEntity"/>.
        /// </summary>
        private RaceType race = RaceType.Human;

        /// <summary>
        /// The list of <see cref="Aura"/>s active on the <see cref="ZeldaEntity"/>.
        /// </summary>
        private readonly AuraList auraList;

        /// <summary> 
        /// The final physcial damage the <see cref="ZeldaEntity"/> does in melee-range.
        /// </summary>
        private int damageMeleeMin, damageMeleeMax;

        /// <summary>
        /// The final physcial damage the <see cref="ZeldaEntity"/> does in ranged-combat.
        /// </summary>
        private int damageRangedMin, damageRangedMax;
        
        /// <summary>
        /// The attack speed (also called delay) of the status object.
        /// </summary>
        private float attackSpeedMelee = 2.5f, attackSpeedRanged = 3.0f;

        /// <summary>
        /// The modifier value that is applied to the cast-time
        /// of a spell to get the actual cast-time.
        /// </summary>
        private float castTimeModifier = 1.0f;

        /// <summary>
        /// The current life of the <see cref="ZeldaEntity"/>.</summary>
        private int life;
        /// <summary> The maximum life of the <see cref="ZeldaEntity"/>. </summary>
        private int maximumLife;
        /// <summary> The amount of life regenerated each regen-tick. </summary>
        private int lifeRegeneration;

        /// <summary> The current mana of the <see cref="ZeldaEntity"/>. </summary>
        private int mana;
        /// <summary> The maximum mana of the <see cref="ZeldaEntity"/>. </summary>
        private int maximumMana;
        /// <summary> The amount of mana regenerated each regen-tick. </summary>
        private int manaRegeneration;

        /// <summary>
        /// The time left until the object regenerates life/mana again.
        /// </summary>
        private float regenTick = RegenTime;

        /// <summary>
        /// The time between Regeneration Ticks.
        /// </summary>
        private const float RegenTime = 3.5f;

        /// <summary>
        /// The level of the statable <see cref="ZeldaEntity"/>.
        /// </summary>
        private int level = 1;

        /// <summary> 
        /// The total armor of the statable <see cref="ZeldaEntity"/>. 
        /// </summary>
        private int armor;

        /// <summary> 
        /// The base armor of the statable <see cref="ZeldaEntity"/>, not taking into account any modifiers.
        /// </summary>
        private int baseArmor;

        /// <summary>
        /// Encapsulates the various chance-to properties.
        /// </summary>
        private readonly ChanceToContainer chanceTo;

        /// <summary>
        /// Encapsulates the various chance-to-be properties.
        /// </summary>
        private readonly ChanceToBeContainer chanceToBe;

        /// <summary>
        /// Encapsulates the various kind of damage done properties.
        /// </summary>
        private readonly DamageDoneContainer damageDone;

        /// <summary>
        /// Encapsulates the various kind of damage taken properties.
        /// </summary>
        private readonly DamageTakenContainer damageTaken;
        
        /// <summary>
        /// Encapsulates the chances to resist spell or attacks of a specific ElementSchool.
        /// </summary>
        private readonly SpellResistanceContainer resistances;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private RandMT rand;

        #region > Blocking <

        /// <summary>
        /// States whether the <see cref="ZeldaEntity"/> can block attacks.
        /// </summary>
        private bool canBlock = false;

        /// <summary>
        /// Stores the block 'power' of the <see cref="ZeldaEntity"/> - aka how much damage is blocked.
        /// Only relevant if canBlock is true.
        /// </summary>
        private int blockValue = 0;

        #endregion

        #region > Invincability <

        /// <summary>
        /// States whether the object is invincible.
        /// </summary>
        private bool isInvincible;

        /// <summary>
        /// States whether the IsInvincible state of the object is only temporary.
        /// </summary>
        private bool isTempInvincible;

        /// <summary>
        /// Stores how much time is left until the object wents non invincible again.
        /// Only relevant if isInvincible and isInvincibleTicking are both true.
        /// </summary>
        private float invincibleTick;

        #endregion

        #endregion
    }
}