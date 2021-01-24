// <copyright file="ParticleEffectOverlay.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Overlays.ParticleEffectOverlay class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Overlays
{
    using System;
    using Atom.Xna.Particles;

    /// <summary>
    /// Defines an <see cref="ISceneOverlay"/> that contains a ParticleEffect.
    /// </summary>
    public class ParticleEffectOverlay : ISceneOverlay
    {
        /// <summary>
        /// Gets the <see cref="ParticleEffect"/> that is associated with this ParticleEffectOverlay.
        /// </summary>
        public ParticleEffect Effect
        {
            get 
            {
                return this.effect;
            }
        }

        /// <summary>
        /// Initializes a new instance of the ParticleEffectOverlay class.
        /// </summary>
        /// <param name="effect">
        /// The ParticleEffect to overlay with the Scene.
        /// </param>
        public ParticleEffectOverlay( ParticleEffect effect )
        {
            if( effect == null )
                throw new ArgumentNullException( "effect" );

            this.effect = effect;
        }

        /// <summary>
        /// Updates this ScrollingParticleEffectOverlay.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            this.effect.Update( updateContext );
        }

        /// <summary>
        /// Draws this ScrollingParticleEffectOverlay.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public void Draw( ZeldaDrawContext drawContext )
        {
            this.effect.Render();
        }

        /// <summary>
        /// Gets called when this ISceneOverlay has been added to the given <see cref="ZeldaScene"/>.
        /// </summary>
        /// <remarks>
        /// This method should not directly be called from user-code.
        /// </remarks>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        public virtual void AddedToScene( ZeldaScene scene )
        {
        }

        /// <summary>
        /// Gets called when this ISceneOverlay has been removed from the given <see cref="ZeldaScene"/>.
        /// </summary>
        /// <remarks>
        /// This method should not directly be called from user-code.
        /// </remarks>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        public virtual void RemovedFromScene( ZeldaScene scene )
        {
        }

        /// <summary>
        /// The <see cref="ParticleEffect"/> that is associated with this ParticleEffectOverlay.
        /// </summary>
        private readonly ParticleEffect effect;
    }
}
