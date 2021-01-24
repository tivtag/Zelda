// <copyright file="ScrollingParticleEffectOverlay.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Overlays.ScrollingParticleEffectOverlay class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Overlays
{
    using Atom;
    using Atom.Math;
    using Atom.Xna.Particles;
    
    /// <summary>
    /// Defines an <see cref="ISceneOverlay"/> that contains a ParticleEffect.
    /// </summary>
    public sealed class ScrollingParticleEffectOverlay : ParticleEffectOverlay
    {
        /// <summary>
        /// Initializes a new instance of the ScrollingParticleEffectOverlay class.
        /// </summary>
        /// <param name="effect">
        /// The ParticleEffect to overlay with the Scene.
        /// </param>
        public ScrollingParticleEffectOverlay( ParticleEffect effect )
            : base( effect )
        {
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
        public override void AddedToScene( ZeldaScene scene )
        {
            scene.Camera.ScrollChanged += this.OnCameraScrollChanged;
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
        public override void RemovedFromScene( ZeldaScene scene )
        {
            scene.Camera.ScrollChanged -= this.OnCameraScrollChanged;
        }

        /// <summary>
        /// Gets called when the scroll property of the Camera has changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        private void OnCameraScrollChanged( object sender, ChangedValue<Vector2> e )
        {
            Vector2 change = e.OldValue - e.NewValue;

            const float ScrollFactorX = 1.0f;
            const float ScrollFactorY = 1.0f;
            
            var scrollValue = new Microsoft.Xna.Framework.Vector2(
               change.X / ScrollFactorX,
               change.Y / ScrollFactorY
            );

            this.Effect.MoveActiveParticles( scrollValue );
        }
    }
}
