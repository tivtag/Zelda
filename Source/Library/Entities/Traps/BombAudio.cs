// <copyright file="BombAudio.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.BombAudio class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Entities
{
    using Atom.Fmod;
    using Atom.Math;

    /// <summary>
    /// Implements the selection of the bomb explosion sound.
    /// </summary>
    public sealed class BombAudio
    {
        /// <summary>
        /// Initializes a new instance of the BombAudio class.
        /// </summary>
        /// <param name="audioSystem">
        /// The AudioSystem that plays and receives the sound samples.
        /// </param>
        /// <param name="rand">
        /// A random number generator.
        /// </param>
        public BombAudio( AudioSystem audioSystem, IRand rand )
        {
            this.bombExplosionSoundA = audioSystem.GetSample( "Bomb_Blow.ogg" );
            this.bombExplosionSoundB = audioSystem.GetSample( "Bomb_Blow_2.ogg" );

            Atom.Fmod.Native.MODE mode =
                Atom.Fmod.Native.MODE._3D |
                Atom.Fmod.Native.MODE._3D_LINEARROLLOFF;

            if( bombExplosionSoundA != null )
            {
                this.bombExplosionSoundA.Load( mode );
            }

            if( bombExplosionSoundB != null )
            {
                this.bombExplosionSoundB.Load( mode );
            }

            this.rand = rand;
        }

        /// <summary>
        /// Gets a random bomb explosion Sound.
        /// </summary>
        public Sound GetRandomSound()
        {
            return this.rand.RandomBoolean ? this.bombExplosionSoundA : this.bombExplosionSoundB;
        }

        private readonly Sound bombExplosionSoundB;
        private readonly Sound bombExplosionSoundA;
        private readonly IRand rand;
    }
}