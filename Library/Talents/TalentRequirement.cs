// <copyright file="TalentRequirement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Talents.TalentRequirement class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Talents
{
    using System.Diagnostics;

    /// <summary>
    /// Defines a single requirement for a talent.
    /// </summary>
    public sealed class TalentRequirement
    {
        /// <summary>
        /// The Talent that is required.
        /// </summary>
        public readonly Talent RequiredTalent;

        /// <summary>
        /// The minimum level the player must have of the required Talent.
        /// </summary>
        public readonly int RequiredTalentLevel;

        /// <summary>
        /// Gets a value indicating whether this TalentRequirement is fulfilled.
        /// </summary>
        public bool IsFulfilled
        {
            get 
            {
                return this.RequiredTalent.Level >= this.RequiredTalentLevel;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TalentRequirement"/> class.
        /// </summary>
        /// <param name="requiredTalent">
        /// The Talent that is required.
        /// </param>
        /// <param name="requiredTalentLevel">
        /// The minimum level the player must have of the required Talent.
        /// </param>
        internal TalentRequirement( Talent requiredTalent, int requiredTalentLevel )
        {
            Debug.Assert( requiredTalent != null );
            Debug.Assert( RequiredTalentLevel <= requiredTalent.MaximumLevel );

            this.RequiredTalent      = requiredTalent;
            this.RequiredTalentLevel = requiredTalentLevel;
        }

        /// <summary>
        /// Gets a value indicating whether the talent requirement
        /// would be fulfilled at the specified talentLevel.
        /// </summary>
        /// <param name="talentLevel">
        /// The level of the talent.
        /// </param>
        /// <returns>
        /// true if it is fulfilled;
        /// -or- otherwise false.
        /// </returns>
        public bool IsFulfilledAt( int talentLevel )
        {
            return talentLevel >= this.RequiredTalentLevel;
        }
    }
}
