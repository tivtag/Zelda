// <copyright file="DamageOverTimeAura.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Auras.DamageOverTimeAura class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Auras
{
    using System;
    using Zelda.Attacks;
    using Zelda.Entities;

    /// <summary>
    /// Defines a <see cref="TimedAura"/> that applies damage over time.
    /// </summary>
    public class DamageOverTimeAura : TickingAura
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the amount of damage each tick of this DamageOverTimeAura does. 
        /// </summary>
        public int DamageEachTick
        {
            get { return this.damageEachTick;  }
            set { this.damageEachTick = value; }
        }

        /// <summary>
        /// Gets or sets the power type that is affected by this DamageOverTimeAura.
        /// </summary>
        public LifeMana PowerType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets status manipulation type that is used when applying the damage of this DamageOverTimeAura.
        /// </summary>
        public StatusManipType ManipulationType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="IAttackableEntity"/> that has applied this DamageOverTimeAura.
        /// </summary>
        public IAttackableEntity Attacker
        {
            get
            {
                return this.attacker;
            }

            set
            {
                this.attacker = value;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageOverTimeAura"/> class.
        /// </summary>
        /// <param name="totalTime">
        /// The time (in seconds) the new DamageOverTimeAura lasts.
        /// </param>
        /// <param name="tickTime">
        /// The time (in seconds) between two ticks.
        /// </param>
        public DamageOverTimeAura( float totalTime, float tickTime )
            : base( totalTime, tickTime )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageOverTimeAura"/> class.
        /// </summary>
        /// <param name="totalTime">
        /// The time (in seconds) the new DamageOverTimeAura lasts.
        /// </param>
        /// <param name="tickTime">
        /// The time (in seconds) between two ticks.
        /// </param>
        /// <param name="effect">
        /// The <see cref="StatusEffect"/> the new DamageOverTimeAura applies additionaly.
        /// </param>
        /// <param name="attacker">
        /// Identifies the Entity that has applied the new DamageOverTimeAura.
        /// </param>
        public DamageOverTimeAura( float totalTime, float tickTime, StatusEffect effect, IAttackableEntity attacker )
            : this( totalTime, tickTime, new StatusEffect[1] { effect }, attacker )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageOverTimeAura"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException"> 
        /// If <paramref name="effects"/> is null.
        /// </exception>
        /// <param name="totalTime">
        /// The time (in seconds) the new DamageOverTimeAura lasts.
        /// </param>
        /// <param name="tickTime">
        /// The time (in seconds) between two ticks.
        /// </param>
        /// <param name="effects">
        /// The list of <see cref="StatusEffect"/>s of the new DamageOverTimeAura.
        /// </param>
        /// <param name="attacker">
        /// Identifies the Entity that has applied the new DamageOverTimeAura.
        /// </param>
        public DamageOverTimeAura( float totalTime, float tickTime, StatusEffect[] effects, IAttackableEntity attacker )
            : base( totalTime, tickTime, effects )
        {
            this.attacker = attacker;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this DamageOverTimeAura has ticked.
        /// </summary>
        protected override void OnTicked()
        {
            if( this.PowerType == LifeMana.Life )
            {
                this.ApplyLifeDamage();
            }
            else
            {
                this.ApplyManaDamage();
            }           
        }

        /// <summary>
        /// Applies life damage to the owner of this DamageOverTimeAura.
        /// </summary>
        private void ApplyLifeDamage()
        {
            if( this.ManipulationType == StatusManipType.Fixed )
            {
                attackable.Attack(
                    this.attacker,
                    new AttackDamageResult( damageEachTick, AttackReceiveType.Hit )
                );
            }
            else
            {
                var statable = this.AuraList.Owner;
                int maximumLife = statable.MaximumLife;
                int damageDone = (int)(maximumLife * damageEachTick / 100.0f);

                attackable.Attack(
                    this.attacker,
                    new AttackDamageResult( damageDone, AttackReceiveType.Hit )
                );
            }
        }

        /// <summary>
        /// Applies mana damage to the owner of this DamageOverTimeAura.
        /// </summary>
        private void ApplyManaDamage()
        {
            var statable = this.AuraList.Owner;

            if( this.ManipulationType == StatusManipType.Fixed )
            {
                statable.LoseMana( damageEachTick );
            }
            else
            {
                int manaLost = (int)(statable.MaximumMana * damageEachTick / 100.0f);
                statable.LoseMana( manaLost );
            }
        }

        /// <summary>
        /// Called when this <see cref="Aura"/> got enabled.
        /// </summary>
        /// <param name="owner">
        /// The Statable that now owns this Aura.
        /// </param>
        protected override void OnEnabled( Statable owner )
        {
            var entity      = owner.Owner;
            this.attackable = entity.Components.Get<Entities.Components.Attackable>();
        }

        /// <summary>
        /// Called when this <see cref="Aura"/> got disabled.
        /// </summary>
        /// <param name="owner">
        /// The Statable that previously owned this Aura.
        /// </param>
        protected override void OnDisabled( Statable owner )
        {
            this.attackable = null;
        }

        /// <summary>
        /// Returns a clone of this <see cref="DamageOverTimeAura"/>.
        /// </summary>
        /// <returns>The cloned Aura.</returns>
        public override Aura Clone()
        {
            var clone = new DamageOverTimeAura( this.Cooldown.TotalTime, this.TickTime, this.GetClonedEffects(), this.attacker );
 
            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the specified DamageOverTimeAura to be a clone of this DamageOverTimeAura.
        /// </summary>
        /// <param name="clone">
        /// The DamageOverTimeAura to setup as a clone of this DamageOverTimeAura.
        /// </param>
        protected void SetupClone( DamageOverTimeAura clone )
        {
            clone.PowerType = this.PowerType;
            clone.damageEachTick = this.damageEachTick;

            base.SetupClone( clone );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The daamge that is applied when this DamageOverTimeAura ticks.
        /// </summary>
        private int damageEachTick;

        /// <summary>
        /// Identifies the Attackable component of the currently active AuraList.Owner.
        /// </summary>
        private Zelda.Entities.Components.Attackable attackable;

        /// <summary>
        /// Identifies the Entity that has applied this DamageOverTimeAura.
        /// </summary>
        private IAttackableEntity attacker;

        #endregion
    }
}
