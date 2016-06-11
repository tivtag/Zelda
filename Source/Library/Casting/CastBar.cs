// <copyright file="CastBar.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Casting.CastBar class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Casting
{
    using System.Diagnostics;
    using Atom;
    using Zelda.Entities;
    using Zelda.Status;
    
    /// <summary>
    /// The CastBar holds the logic behind casting of spells.
    /// This class can't be inherited.
    /// </summary>
    /// <remarks>
    /// Magical attacks/Spells that aren't instant
    /// use the CastBar for their casting process.
    /// </remarks>
    public sealed class CastBar
    {
        #region [ Events ]

        /// <summary>
        /// Fired when the casting of a spell has started.
        /// </summary>
        public event RelaxedEventHandler<CastBar, Spell> Started;

        /// <summary>
        /// Fired when the casting of a spell has finished.
        /// </summary>
        public event RelaxedEventHandler<CastBar, Spell> Finished;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets a value indicating whether the caster that owns this CastBar
        /// is currently casting a spell.
        /// </summary>
        public bool IsCasting
        {
            get
            {
                return spell != null;
            }
        }

        /// <summary>
        /// Gets the time (in seconds) the currently casting Spell takes to execute;
        /// including the cast speed reduction modifier of the caster.
        /// </summary>
        public float CastTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the time left (in seconds) until the currently casting Spell executes.
        /// </summary>
        public float CastTimeLeft
        {
            get { return this.castTimeLeft; }
        }

        /// <summary>
        /// Gets a value indicating how far the cast process
        /// for the current Spell has gone.
        /// </summary>
        /// <remarks>
        /// This value is used to find the correct animation frame
        /// of the caster. Casting animations always have three frames.
        /// </remarks>
        public int CastIndex
        {
            get
            {
                float ratio = this.castTimeLeft / this.CastTime;
                return ratio <= 0.33f ? 2 : (ratio <= 0.66f ? 1 : 0);
            }
        }

        /// <summary>
        /// Gets the ZeldaEntity that owns this CastBar.
        /// </summary>
        public ZeldaEntity Caster
        {
            get { return this.caster; }
        }

        /// <summary>
        /// Gets the Statable component of the <see cref="Caster"/> that owns this CastBar.
        /// </summary>
        public Statable Statable
        {
            get { return this.statable; }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the CastBar class.
        /// </summary>
        /// <param name="entity">
        /// The ZeldaEntity that owns the new CastBar.
        /// </param>
        /// <exception cref="Atom.Components.ComponentNotFoundException">
        /// If the given <paramref name="entity"/> doesn't own the Statable component.
        /// </exception>
        internal CastBar( ZeldaEntity entity )
        {
            Debug.Assert( entity != null );

            this.statable = entity.Components.Find<Statable>();
            if( this.statable == null )
                throw new Atom.Components.ComponentNotFoundException( typeof( Statable ) );

            this.caster = entity;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Starts casting of the given <see cref="Spell"/>.
        /// </summary>
        /// <param name="spell">
        /// The Spell to cast.
        /// </param>
        /// <returns>
        /// Returns <see langword="true"/> if casting has started;
        /// or otherwise <see langword="false"/> if there is already another Spell
        /// under the process of casting.
        /// </returns>
        internal bool StartCast( Spell spell )
        {
            Debug.Assert( spell != null );
            if( this.spell != null )
                return false;

            this.spell = spell;
            this.Started.Raise( this, this.spell );

            this.CastTime     = spell.CastTime * statable.CastTimeModifier;
            this.castTimeLeft = this.CastTime;
            return true;
        }

        /// <summary>
        /// Updates this CastBar.
        /// </summary>
        /// <param name="frameTime">
        /// The time the last frame took to execute (in seconds).
        /// </param>
        public void Update( float frameTime )
        {
            if( this.IsCasting )
            {
                this.castTimeLeft -= frameTime;

                if( this.castTimeLeft <= 0.0f )
                {
                    this.ExecuteCast();
                }
            }
        }

        /// <summary>
        /// Executes the currently casting Spell.
        /// </summary>
        private void ExecuteCast()
        {
            this.Finished.Raise( this, this.spell );

            this.spell.Fire( null );
            this.spell = null;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The spell currently beeing cast.
        /// </summary>
        private Spell spell;

        /// <summary>
        /// The time in seconds left until the current
        /// Spell cast ends.
        /// </summary>
        private float castTimeLeft;

        /// <summary>
        /// Identifies the ZeldaEntity that owns this CastBar.
        /// </summary>
        private readonly ZeldaEntity caster;

        /// <summary>
        /// Identifies the Statable component of the ZeldaEntity that owns this CastBar.
        /// </summary>
        private readonly Statable statable;

        #endregion
    }
}
