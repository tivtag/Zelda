// <copyright file="SpawnEntityEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.SpawnEntityEvent class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Events
{
    using System;
    using Zelda.Entities;
    using Atom;

    /// <summary>
    /// Defines an event that when triggered spawns a <see cref="ZeldaEntity"/>
    /// defined with a specified entity template at a specified <see cref="Zelda.Entities.Spawning.ISpawnPoint"/>.
    /// </summary>
    public class SpawnEntityEvent : ZeldaEvent, IZeldaSetupable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name that uniquely identifies
        /// the entity template that is uses to create the
        /// entity that gets spawned by this SpawnEntityEvent.
        /// </summary>
        public string EntityTemplateName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name that uniquely identifies
        /// the spawn point at which the entity should spawn.
        /// </summary>
        public string SpawnPointName
        {
            get;
            set;
        }        

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Triggers this SpawnEntityEvent.
        /// </summary>
        /// <param name="obj">
        /// The object that has triggered this SpawnEntityEvent.
        /// </param>
        public override void Trigger( object obj )
        {
            var scene = this.Scene;
            if( scene == null )
                return;

            var entity     = entityTemplateManager.GetEntity( this.EntityTemplateName );
            var spawnPoint = scene.GetSpawnPoint( this.SpawnPointName );
                        
            spawnPoint.Spawn( entity );
        }

        /// <summary>
        /// Setups this SpawnEntityEvent.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
           this.entityTemplateManager = serviceProvider.EntityTemplateManager;
        }

        /// <summary>
        /// Serializes this SpawnEntityEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process occurs.
        /// </param>
        public override void Serialize( Atom.Events.IEventSerializationContext context )
        {
            base.Serialize( context );

            // Header
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            // Data
            context.Write( this.SpawnPointName     ?? string.Empty );
            context.Write( this.EntityTemplateName ?? string.Empty );
        }

        /// <summary>
        /// Deserializes this SpawnEntityEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process occurs.
        /// </param>
        public override void Deserialize( Atom.Events.IEventDeserializationContext context )
        {
            base.Deserialize( context );

            // Header
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            this.SpawnPointName = context.ReadString();
            this.EntityTemplateName = context.ReadString();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The <see cref="EntityTemplateManager"/> object. 
        /// </summary>
        private EntityTemplateManager entityTemplateManager;

        #endregion
    }
}
