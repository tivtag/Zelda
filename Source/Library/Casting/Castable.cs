// <copyright file="Castable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Casting.Castable class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Casting
{
    using Zelda.Entities;
    using Zelda.Entities.Components;

    /// <summary>
    /// Defines a <see cref="ZeldaComponent"/> that gives
    /// the <see cref="ZeldaEntity"/> that owns the component
    /// the possibility to cast <see cref="Spell"/>s.
    /// This class can't be inherited.
    /// </summary>
    public sealed class Castable : ZeldaComponent
    {
        /// <summary>
        /// Gets the <see cref="CastBar"/> of the castable <see cref="ZeldaEntity"/>.
        /// </summary>
        public CastBar CastBar
        {
            get 
            {
                return this.castBar; 
            }
        }

        /// <summary>
        /// Called when this ZeldaComponent has been successfully attached to an Atom.Components.IEntity.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            this.castBar = new CastBar( this.Owner );
        }

        /// <summary>
        /// Updates this Castable component.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        public override void Update( Atom.IUpdateContext updateContext )
        {
            this.castBar.Update( updateContext.FrameTime );
        }

        /// <summary>
        /// The <see cref="CastBar"/> of the castable <see cref="ZeldaEntity"/>.
        /// </summary>
        private CastBar castBar;
    }
}
