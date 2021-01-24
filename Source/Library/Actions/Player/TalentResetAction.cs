// <copyright file="StatResetAction.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Actions.StatResetAction class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Actions.Player
{
    using System.Globalization;

    /// <summary>
    /// Resets the stats of the current player.
    /// </summary>
    public sealed class TalentResetAction : ResetAction
    {
        /// <summary>
        /// Gets the number of talent points that would be
        /// returned to the player.
        /// </summary>
        private int TalentPoints
        {
            get
            {
                var talents = this.Player.TalentTree;
                return talents.Statistics.TotalInvestedPoints;
            }
        }

        /// <summary>
        /// Executes the actual resetting logic.
        /// </summary>
        protected override void ActuallyReset()
        {
            this.Player.TalentTree.Reset();
        }

        /// <summary>
        /// Gets a value indicating whether this IAction can be executed.
        /// </summary>
        /// <returns>
        /// true if this IAction can be executed;
        /// otherwise false.
        /// </returns>
        public override bool CanExecute()
        {
            if( this.TalentPoints > 0 )
            {
                return base.CanExecute();
            }

            return false;
        }

        /// <summary>
        /// Gets a localized description text of this IAction.
        /// </summary>
        /// <returns>
        /// The localized description of this IAction.
        /// </returns>
        public override string GetDescription()
        {
            int talents = this.TalentPoints;
            var culture = CultureInfo.CurrentCulture;

            return string.Format(
                culture,
                "Resets {0} talent points.\nCosts {1} rubies.",
                talents.ToString( culture ),
                this.GetRubyCost( this.Statable ).ToString( culture )
            );
        }
    }
}
