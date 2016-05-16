// <copyright file="FixedManaRegenerationAura.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Auras.FixedManaRegenerationAura class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status.Auras
{
    using System;

    /// <summary>
    /// Defines a <see cref="TimedAura"/> that regenerates mana over time.
    /// This is a sealed class.
    /// </summary>
    public sealed class FixedManaRegenerationAura : TickingAura
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the amount of damage each tick of this FixedManaRegenerationAura does. 
        /// </summary>
        public int ManaEachTick
        {
            get;
            set;
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedManaRegenerationAura"/> class.
        /// </summary>
        /// <param name="totalTime">
        /// The time (in seconds) the new FixedManaRegenerationAura lasts.
        /// </param>
        /// <param name="tickTime">
        /// The time (in seconds) between two ticks.
        /// </param>
        public FixedManaRegenerationAura( float totalTime, float tickTime )
            : base( totalTime, tickTime )
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this FixedManaRegenerationAura is ticking.
        /// </summary>
        protected override void OnTicked()
        {
            this.statable.RestoreMana( this.ManaEachTick ); 
        }

        /// <summary>
        /// Called when this <see cref="Aura"/> got enabled.
        /// </summary>
        /// <param name="owner">
        /// The Statable that now owns this Aura.
        /// </param>
        protected override void OnEnabled( Statable owner )
        {
            this.statable = owner;
        }

        /// <summary>
        /// Called when this <see cref="Aura"/> got disabled.
        /// </summary>
        /// <param name="owner">
        /// The Statable that previously owned this Aura.
        /// </param>
        protected override void OnDisabled( Statable owner )
        {
            this.statable = null;
        }

        /// <summary>
        /// Returns a clone of this <see cref="FixedManaRegenerationAura"/>.
        /// </summary>
        /// <returns>The cloned Aura.</returns>
        public override Aura Clone()
        {
            return new FixedManaRegenerationAura( this.Cooldown.TotalTime, this.TickTime ) {
                Name                = this.Name,
                Symbol              = this.Symbol,
                IsVisible           = this.IsVisible,
                DebuffFlags         = this.DebuffFlags,
                DescriptionProvider = this.DescriptionProvider,

                ManaEachTick = this.ManaEachTick,
            };
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the Statable component of the currently active AuraList.Owner.
        /// </summary>
        private Statable statable;

        #endregion
    }
}
