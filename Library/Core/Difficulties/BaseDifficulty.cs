// <copyright file="BaseDifficulty.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Difficulties.BaseDifficulty class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Difficulties
{
    using Zelda.Status;
    using Zelda.Entities;
    
    /// <summary>
    /// Implements the base functionality of an <see cref="IDifficulty"/> object.
    /// </summary>
    public abstract class BaseDifficulty : IDifficulty
    {
        /// <summary>
        /// Gets the ID that uniquely identifies this IDifficulty.
        /// </summary>
        public abstract DifficultyId Id
        {
            get;
        }

        /// <summary>
        /// Gets the (localized) name of this IDifficulty.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Applies this IDifficulty to the specified Enemy.
        /// </summary>
        /// <param name="enemy">
        /// The enemy to apply this IDifficulty on.
        /// </param>
        public void ApplyOn( Enemy enemy )
        {
            var auraList = enemy.Statable.AuraList;

            for( int i = 0; i < this.StatusEffects.Length; ++i )
            {
                auraList.AddEffect( this.StatusEffects[i] );
            }
        }

        /// <summary>
        /// Called when this IDifficulty has been choosen or given up.
        /// </summary>
        /// <param name="changeType">
        /// States whether this IDifficulty has been choosen or given up.
        /// </param>
        public void OnChange( ChangeType changeType )
        {
            if( changeType == ChangeType.To )
            {
                this.EnsureUseable();
            }
        }

        /// <summary>
        /// Ensures that this BaseDifficulty can be applied.
        /// </summary>
        private void EnsureUseable()
        {
            if( this.StatusEffects != null )
                return;

            this.StatusEffects = this.CreateStatusEffects();
        }

        /// <summary>
        /// Creates the StatusValueEffects that are applied to enemies.
        /// </summary>
        /// <returns>
        /// The newly created StatusValueEffects.
        /// </returns>
        protected abstract StatusValueEffect[] CreateStatusEffects();

        /// <summary>
        /// The StatusEffects that are applied to enemies for this difficulty.
        /// </summary>
        private StatusValueEffect[] StatusEffects;
    }
}
