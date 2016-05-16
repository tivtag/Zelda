// <copyright file="EntityTemplate.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.EntityTemplate class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities
{
    using Atom;
    using Zelda.Difficulties;

    /// <summary>
    /// Represents a template that can be used to create new instances of a specific <see cref="ZeldaEntity"/>.
    /// </summary>
    public sealed class EntityTemplate : IEntityTemplate
    {
        /// <summary>
        /// Gets the name that uniquely identifies this EntityTemplate.
        /// </summary>
        public string Name
        {
            get
            {
                return this.template.Name;
            }
        }

        /// <summary>
        /// Gets the localized name of the templated entity.
        /// </summary>
        public string LocalizedName
        {
            get 
            {
                var localized = this.template as IReadOnlyLocalizedNameable;
                return localized != null ? localized.LocalizedName : this.template.Name;
            }
        }

        /// <summary>
        /// Initializes a new instance of the EntityTemplate class.
        /// </summary>
        /// <param name="template">
        /// The actual entity on which instances of the new EntityTemplate are based on.
        /// </param>
        /// <param name="serviceProvider">
        /// Allows access to various game-related services.
        /// </param>
        internal EntityTemplate( ZeldaEntity template, IZeldaServiceProvider serviceProvider )
        {
            this.template = template;
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates an instance of this <see cref="EntityTemplate"/>.
        /// </summary>
        /// <returns>
        /// The newly created ZeldaEntity.
        /// </returns>
        public ZeldaEntity CreateInstance()
        {
            var entity = this.template.Clone();
            GameDifficulty.ApplyOn( entity );
            
            var setupable = entity as IZeldaSetupable;
            if( setupable != null )
                setupable.Setup( this.serviceProvider );

            return entity;
        }

        /// <summary>
        /// The actual entity on which instances of this EntityTemplate are based on.
        /// </summary>
        private readonly ZeldaEntity template;

        /// <summary>
        /// Allows access to various game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;
    }
}
