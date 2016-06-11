// <copyright file="Spell.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Casting.Spell class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Casting
{
    using Zelda.Attacks;
    using Zelda.Entities;

    /// <summary>
    /// Represents the base class of all magical <see cref="Attack"/>s.
    /// </summary>
    public abstract class Spell : Attack
    {
        /// <summary>
        /// Gets or sets the time (in seconds) this Spell takes to cast;
        /// before applying any cast-time modifiers.
        /// </summary>
        public float CastTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this Spell casts instantly.
        /// </summary>
        public bool IsInstant
        {
            get 
            { 
                return this.CastTime == 0;
            }
        }

        /// <summary>
        /// Gets the <see cref="Castable"/> component of the ZeldaEntity that owns this Spell.
        /// </summary>
        public Castable Castable
        {
            get
            {
                return this.castable;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this Spell can currently be cast.
        /// </summary>
        /// <remarks>
        /// This property is the ultimate castable check.
        /// </remarks>
        protected virtual bool IsCastable
        {
            get
            {
                return this.IsReady && this.IsUseable;
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Spell"/> class.
        /// </summary>
        /// <param name="owner">
        /// The entity that owns the new Spell.
        /// </param>
        /// <param name="castTime">
        /// The time it takes for the new Spell to cast.
        /// </param>
        /// <param name="method">
        /// The AttackDamageMethod that calculates the damage the new Spell does. 
        /// </param>
        /// <exception cref="Atom.Components.ComponentNotFoundException">
        /// If the given <see cref="ZeldaEntity"/> doesn't contain the <see cref="Zelda.Status.Statable"/> 
        /// and/or <see cref="Castable"/> component.
        /// </exception>
        protected Spell( ZeldaEntity owner, float castTime, AttackDamageMethod method )
            : base( owner, method )
        {
            this.castable = owner.Components.Get<Castable>();
            this.CastTime = castTime;

            if( castTime != 0.0f && this.castable == null )
            {
                throw new Atom.Components.ComponentNotFoundException( typeof( Castable ) );
            }
        }

        /// <summary>
        /// Starts to cast this Spell.
        /// </summary>
        /// <returns>
        /// Returns <see langword="true"/> if casting of this Spell has begun;
        /// otherwise <see langword="false"/>.
        /// </returns>
        public bool Cast()
        {
            if( !this.IsCastable )
                return false;

            if( this.IsInstant )
            {
                if( this.Fire( null ) )
                {
                    this.OnFired();
                    return true;
                }
            }
            else
            {
                if( this.castable.CastBar.StartCast( this ) )
                {
                    this.OnFired();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Identifies the Castable component of the ZeldaEntity that owns this Spell.
        /// </summary>
        private readonly Castable castable;
    }
}
