// <copyright file="IoC.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.IoC class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using Atom;

    /// <summary>
    /// Provides global access to the Inversion of Control container.
    /// </summary>
    public static class IoC
    {
        /// <summary>
        /// Gets or sets the <see cref="IZeldaServiceProvider"/> that is used
        /// to resolve dependencies.
        /// </summary>
        public static IZeldaServiceProvider Provider
        {
            get;
            set;
        }

        /// <summary>
        /// Attempts to resolve the object of the specified type.
        /// </summary>
        /// <typeparam name="TService">
        /// The type of the service.
        /// </typeparam>
        /// <returns>
        /// The requested service; or null.
        /// </returns>
        public static TService Resolve<TService>()
            where TService : class
        {
            return Provider.GetService<TService>();
        }
    }
}
