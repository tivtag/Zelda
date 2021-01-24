// <copyright file="DesignTime.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the static Zelda.Design.DesignTime singleton class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Design
{
    using Atom;

    /// <summary>
    /// Provides access to design-time-only services.
    /// </summary>
    /// <remarks>
    /// This class should only be used by design-time components;
    /// such as UITypeEditors.
    /// </remarks>
    public static class DesignTime
    {
        /// <summary>
        /// Gets an object that provides fast access to game-related services.
        /// </summary>
        public static IZeldaServiceProvider Services
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the <see cref="DesignTime"/> singleton.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public static void Initialize( IZeldaServiceProvider serviceProvider )
        {
            DesignTime.Services = serviceProvider;
        }

        /// <summary>
        /// Gets the service of the specified type <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">
        /// The type of service to get.
        /// </typeparam>
        /// <returns>
        /// The requested service;
        /// or null if no such service was registered.
        /// </returns>
        public static TService GetService<TService>()
             where TService : class
        {
            return Services.GetService<TService>();
        }
    }
}
