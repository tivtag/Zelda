// <copyright file="SceneChangeEvent.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Events.SceneChangeEvent class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Events
{
    using System;
    using System.ComponentModel;
    using Atom.Events;
    using Zelda.Entities;
    using Zelda.Entities.Spawning;

    /// <summary>
    /// Defines an <see cref="Event"/> that when triggered
    /// changes the current <see cref="ZeldaScene"/> shown in-game and
    /// spawns the PlayerEntity at a specified SpawnPoint.
    /// </summary>
    public class SceneChangeEvent : Event
    {
        /// <summary>
        /// Gets or sets a value indicating whether 
        /// the previous scene is cached, or completly destroyed.
        /// </summary>
        /// <value>
        /// The default value is true.
        /// </value>
        public bool CachePreviousScene
        {
            get
            {
                return this.cachePreviousScene;
            }

            set 
            {
                this.cachePreviousScene = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the <see cref="ZeldaScene"/> to change to.
        /// </summary>
        [Editor( typeof( Zelda.Design.SceneNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string SceneName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the spawn point to spawn the player at.
        /// </summary>
        public string SpawnPointName
        {
            get;
            set;
        }
                
        /// <summary>
        /// Gets a value indicating whether this SceneChangeEvent
        /// can be triggered by the specified <see cref="Object"/>.
        /// </summary>
        /// <param name="obj">
        /// The object that wishes to trigger this SceneChangeEvent.
        /// </param>
        /// <returns>
        /// true if the specified Object can trigger this SceneChangeEvent;
        /// otherwise false.
        /// </returns>
        public override bool CanBeTriggeredBy( object obj )
        {
            return obj is PlayerEntity;
        }

        /// <summary>
        /// Triggers this SceneChangeEvent.
        /// </summary>
        /// <param name="obj">
        /// The object that has triggered this SceneChangeEvent.
        /// </param>
        public override void Trigger( object obj )
        {
            var player = obj as PlayerEntity;
            if( player == null )
                return;

            this.ChangeScene( player );
        }

        /// <summary>
        /// Changes to the ZeldaScene specified by this SceneChangeEvent.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that wants to change his current scene.
        /// </param>
        protected void ChangeScene( PlayerEntity player )
        {
            // Change Scene
            var scene = player.IngameState.RequestSceneChange( this.SceneName, this.CachePreviousScene );

            // Find Spawn Point.
            var spawnPoint = scene.GetSpawnPoint( this.SpawnPointName );

            if( spawnPoint == null )
            {
                spawnPoint = scene.GetEntity<ISpawnPoint>();

                if( spawnPoint == null )
                {
                    throw new Atom.NotFoundException(
                        string.Format(
                            System.Globalization.CultureInfo.CurrentCulture,
                            Resources.Error_SpawnPointXOrAlternativeNotFound,
                            this.SpawnPointName ?? string.Empty
                        )
                    );
                }
            }

            // Spawn Player
            spawnPoint.Spawn( player );
        }
        
        /// <summary>
        /// Serializes this SceneChangeEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process occurs.
        /// </param>
        public override void Serialize( Atom.Events.IEventSerializationContext context )
        {
            base.Serialize( context );

            context.Write( this.SceneName ?? string.Empty );
            context.Write( this.SpawnPointName ?? string.Empty );
            context.Write( this.CachePreviousScene );
        }
        
        /// <summary>
        /// Deserializes this SceneChangeEvent event.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process occurs.
        /// </param>
        public override void Deserialize( Atom.Events.IEventDeserializationContext context )
        {
            // Read base.
            base.Deserialize( context );

            this.SceneName          = context.ReadString();
            this.SpawnPointName     = context.ReadString();
            this.CachePreviousScene = context.ReadBoolean();
        }
        
        /// <summary>
        /// The storage field of the <see cref="CachePreviousScene"/> property.
        /// </summary>
        private bool cachePreviousScene = true;
    }
}
