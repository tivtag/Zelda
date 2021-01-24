// <copyright file="PermanentDamageOverTimeAura.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Auras.PermanentDamageOverTimeAura class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Auras
{
    using System;
    using Zelda.Attacks;

    /// <summary>
    /// Defines a <see cref="PermanentAura"/> that applies DamageOverTime to its owning StatusObject.
    /// </summary>
    /// <remarks>
    /// DamageOverTime(DoT) StatusEffects never crit, nor can the individual ticks get resisted.
    /// </remarks>
    public class PermanentDamageOverTimeAura : PermanentAura
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the amount of damage this <see cref="PermanentDamageOverTimeAura"/> applies each tick. 
        /// </summary>
        public int DamageEachTick
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets how the <see cref="DamageEachTick"/> property
        /// should be interpreted.
        /// </summary>
        public StatusManipType ManipType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets what kind of power this <see cref="PermanentDamageOverTimeAura"/> manibulates.
        /// </summary>
        public LifeMana PowerType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time (in seconds) between two ticks.
        /// </summary>
        public float TickTime
        {
            get { return tickTime; }
            set
            {
                if( tickTime < 0.0f )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "value" );

                this.tickTime = value;
                ResetTick();
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PermanentDamageOverTimeAura"/> class.
        /// </summary>
        /// <param name="attacker">
        /// Identifies the Entity that has applied the new PermanentDamageOverTimeAura.
        /// </param>
        public PermanentDamageOverTimeAura( Zelda.Entities.IAttackableEntity attacker )
            : base()
        {
            this.attacker = attacker;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this <see cref="PermanentDamageOverTimeAura"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            tickTimeLeft -= updateContext.FrameTime;

            if( tickTimeLeft <= 0.0f )
            {
                OnTick();
                ResetTick();
            }
        }

        /// <summary>
        /// Resets the time until this <see cref="PermanentDamageOverTimeAura"/> ticks again.
        /// </summary>
        public void ResetTick()
        {
            tickTimeLeft = tickTime;
        }

        /// <summary>
        /// Applies the damage inflicates by this <see cref="PermanentDamageOverTimeAura"/>.
        /// </summary>
        private void OnTick()
        {
            var statable = this.AuraList.Owner;

            if( ManipType == StatusManipType.Fixed )
            {
                if( PowerType == LifeMana.Life )
                {
                    attackable.Attack(
                        this.attacker, 
                        new AttackDamageResult( DamageEachTick, AttackReceiveType.Hit ) 
                    );
                }
                else
                {
                    statable.LoseMana( new AttackDamageResult( DamageEachTick, AttackReceiveType.Hit ) );
                }
            }
            else
            {
                if( PowerType == LifeMana.Life )
                {
                    attackable.Attack(
                        this.attacker,
                        new AttackDamageResult(
                            (int)(statable.MaximumLife * DamageEachTick / 100.0f),
                            AttackReceiveType.Hit
                        ) 
                    );
                }
                else
                {
                    statable.LoseMana( 
                        new AttackDamageResult(
                            (int)(statable.MaximumMana * DamageEachTick / 100.0f),
                            AttackReceiveType.Hit
                        )
                    );
                }
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
        /// Creates a clone of this <see cref="PermanentDamageOverTimeAura"/>.
        /// </summary>
        /// <returns>The cloned Aura.</returns>
        public override Aura Clone()
        {
            throw new NotSupportedException();

            /*
            return new PermanentDamageOverTimeAura( this.attacker, GetClonedEffects() ) {
                Name                = this.Name,
                Symbol              = this.Symbol,
                IsVisible           = this.IsVisible,
                DebuffFlags         = this.DebuffFlags,
                DescriptionProvider = this.DescriptionProvider,
                DamageEachTick      = this.DamageEachTick,
                ManipType           = this.ManipType,
                PowerType           = this.PowerType,
                TickTime            = this.TickTime
            };
            */
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The time (in seconds) left until this PermanentDamageOverTimeAura ticks again.
        /// </summary>
        private float tickTimeLeft;

        /// <summary>
        /// The time (in seconds) between two ticks.
        /// </summary>
        private float tickTime;

        /// <summary>
        /// Identifies the Attackable component of the currently active AuraList.Owner.
        /// </summary>
        private Entities.Components.Attackable attackable;

        /// <summary>
        /// Identifies the Entity that has applied this PermanentDamageOverTimeAura.
        /// </summary>
        private readonly Zelda.Entities.IAttackableEntity attacker;

        #endregion
    }
}
