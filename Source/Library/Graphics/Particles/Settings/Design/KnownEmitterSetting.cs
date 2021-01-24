// <copyright file="KnownEmitterSetting.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Graphics.Particles.Settings.Design.KnownEmitterSetting class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Graphics.Particles.Settings.Design
{
    using System;

    /// <summary>
    /// Provides access to all known IEmitterSettings.
    /// </summary>
    public static class KnownEmitterSetting
    {
        /// <summary>
        /// Gets the types of all known IEmitterSettings.
        /// </summary>
        /// <remarks>
        /// The returned array should not be modified.
        /// </remarks>
        public static Type[] Types
        {
            get
            {
                return KnownEmitterSetting.types;
            }
        }

        /// <summary>
        /// The IEmitterSettings that are known to the design-time system.
        /// </summary>
        private static readonly Type[] types = new Type[4] {
            typeof( EmitterSettings ),
            typeof( RandomFromListEmitterSettings ),
            typeof( DefaultRainEmitterSettings ),
            typeof( DefaultSnowEmitterSettings )
        };
    }
}
