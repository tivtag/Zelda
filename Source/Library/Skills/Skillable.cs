// <copyright file="Skillable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Skills.Skillable class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Skills
{
    using System;
    using System.Collections.Generic;
    using Atom.Diagnostics.Contracts;
    using Atom;
    using Zelda.Entities.Components;

    /// <summary>
    /// Encapsulates the <see cref="Skill"/>s management of an entity. 
    /// </summary>
    public sealed class Skillable : ZeldaComponent, IEnumerable<Skill>
    {
        /// <summary>
        /// Raised when the skillable Entity has learned a new Skill.
        /// </summary>
        public event RelaxedEventHandler<Skill> Learned;

        /// <summary>
        /// Raised when the skillable Entity has unlearned a Skill.
        /// </summary>
        public event RelaxedEventHandler<Skill> Unlearned;

        /// <summary>
        /// Receives a value that indicates whether this PlayerEntity 
        /// has learned the given <see cref="Skill"/>.
        /// </summary>
        /// <param name="skill">The skill to look for.</param>
        /// <returns>
        /// true if this PlayerEntity has already learned the given Skill;
        /// otherwise false.
        /// </returns>
        public bool Has( Skill skill )
        {
            return this.skills.Contains( skill );
        }

        /// <summary>
        /// Tries to receive the <see cref="Skill"/> of the specified type.
        /// </summary>
        /// <param name="type">
        /// The type of the skill to get.
        /// </param>
        /// <returns>
        /// The Skill; or null if the Player doesn't own that Skill.
        /// </returns>
        internal Skill Get( Type type )
        {
            for( int i = 0; i < skills.Count; ++i )
            {
                var skill = skills[i];

                if( skill.GetType().Equals( type ) )
                {
                    return skill;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds the given <see cref="Skill"/> to the list of skills the player has learned.
        /// </summary>
        /// <param name="skill">
        /// The skill to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="skill"/> is null.
        /// </exception>
        internal void Learn( Skill skill )
        {
            Contract.Requires<ArgumentNullException>( skill != null );

            if( this.Has( skill ) )
            {
                return;
            }

            this.skills.Add( skill );
            this.Learned.Raise( this, skill );
        }

        /// <summary>
        /// Removes the given <see cref="Skill"/> from the list of skills the player has learned.
        /// </summary>
        /// <param name="skill">
        /// The skill to remove.
        /// </param>
        internal void Unlearn( Skill skill )
        {
            Contract.Requires<ArgumentNullException>( skill != null );

            if( this.skills.Remove( skill ) )
            {
                this.Unlearned.Raise( this, skill );
            }
        }

        /// <summary>
        /// Updates this Skillable component.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        public override void Update( IUpdateContext updateContext )
        {
            var zeldaUpdateContext = (ZeldaUpdateContext)updateContext;

            for( int i = 0; i < skills.Count; ++i )
            {
                skills[i].Update( zeldaUpdateContext );
            }
        }
       
        /// <summary>
        /// Gets an enumerator that iterates over the Skills that the entity has learned.
        /// </summary>
        /// <returns>
        /// The Skills that the entity has learned.
        /// </returns>
        public IEnumerator<Skill> GetEnumerator()
        {
            return this.skills.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that iterates over the Skills that the entity has learned.
        /// </summary>
        /// <returns>
        /// The Skills that the entity has learned.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Lists the <see cref="Skill"/>s the PlayerEntity has aquired.
        /// </summary>
        private readonly List<Skill> skills = new List<Skill>( 10 );
    }
}