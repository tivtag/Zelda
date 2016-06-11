// <copyright file="MusicFadeIner.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Audio.MusicFadeIner class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Audio
{
    using Atom.Fmod;

    /// <summary>
    /// Encapsulates the process of fading-in a Channel.
    /// </summary>
    public sealed class MusicFadeIner
    {
        /// <summary>
        /// Initializes a new instance of the MusicFadeIner class.
        /// </summary>
        /// <param name="channel">
        /// The channel to fade-in. Can be null.
        /// </param>
        /// <param name="maximumVolumneTime">
        /// The time in seconds at which the channel should be completely faded in.
        /// </param>
        public MusicFadeIner( Channel channel, float maximumVolumneTime )
        {
            this.channel = channel;
            this.maximumVolumneTime = maximumVolumneTime;
        }

        /// <summary>
        /// Updates this MusicFadeIner.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            if( this.hasFaded || this.channel == null )
                return;

            this.tickTime += updateContext.FrameTime;
            this.ApplyNewVolumne();
        }

        /// <summary>
        /// Applies the new volumne value to the channel.
        /// </summary>
        private void ApplyNewVolumne()
        {
            const float MaximumVolumne = 1.0f;
            float volumne = tickTime / maximumVolumneTime;

            if( volumne >= MaximumVolumne )
            {
                this.channel.Volume = MaximumVolumne;
                this.hasFaded = true;
            }
            else
            {
                this.channel.Volume = volumne;
            }
        }

        /// <summary>
        /// The time that has passed in seconds.
        /// </summary>
        private float tickTime;

        /// <summary>
        /// States whether the MusicFadeIner has completed the fading process.
        /// </summary>
        private bool hasFaded;

        /// <summary>
        /// The channel to fade-in.
        /// </summary>
        private readonly Channel channel;

        /// <summary>
        /// The time in seconds at which the channel should have the MaximumVolumne.
        /// </summary>
        private readonly float maximumVolumneTime;
    }
}
