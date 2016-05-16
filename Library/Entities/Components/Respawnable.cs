// <copyright file="Respawnable.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Components.Respawnable class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Components
{
    using System;
    using System.Diagnostics;
    using Atom;
    using Atom.Math;
    
    /// <summary>
    /// Defines a <see cref="ZeldaComponent"/> that allows
    /// a <see cref="ZeldaEntity"/> to be respawned.
    /// This class can't be inherited.
    /// </summary>
    public sealed class Respawnable : ZeldaComponent
    {
        #region [ Events ]

        /// <summary>
        /// Fired when the respawnable ZeldaEntity wants to be respawned.
        /// </summary>
        public event SimpleEventHandler<Respawnable> RespawnNeeded;

        /// <summary>
        /// Fired when the respawnable ZeldaEntity has been respawned.
        /// </summary>
        public event SimpleEventHandler<Respawnable> Respawned;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the time in seconds it takes for the ZeldaEntity to respawn.
        /// </summary>
        public float RespawnTime
        {
            get { return respawnTime; }
            set { respawnTime = value; }
        }

        #endregion
        
        /// <summary>
        /// Notifies the respawnable ZeldaEntity that 
        /// a respawn is needed.
        /// </summary>
        public void NotifyRespawnNeeded()
        {
            if( this.RespawnNeeded != null )
            {
                this.RespawnNeeded( this );
            }
        }

        /// <summary>
        /// Gets wgetger the respawnable <see cref="ZeldaEntity"/>
        /// can spawn at the given position without any problems.
        /// </summary>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        /// <param name="position">
        /// The respawn position.
        /// </param>
        /// <param name="floorNumber">
        /// The number of the floor to spawn at.
        /// </param>
        /// <param name="isInitialSpawn">
        /// States whether the respawn is going to be the initial spawn of the ZeldaEntity.
        /// </param>
        /// <returns>
        /// true if the respawnable ZeldaEntity can respawn at the given position;
        /// otherwise false.
        /// </returns>
        public bool CanRespawnAt( ZeldaScene scene, Vector2 position, int floorNumber, bool isInitialSpawn )
        {
            if( isInitialSpawn )
                return true;

            // Don't allow spawning of the ZeldaEntity
            // if the player could see it pop up.
            if( scene.Camera.IsInVision( this.Owner ) )
                return false;
           
            var collision = this.Owner.Collision;

            // Nor allow creation if an object is placed on it.
            Rectangle area = new Rectangle(
                (int)(position.X + collision.Offset.X),
                (int)(position.Y + collision.Offset.Y),
                (int)collision.Width,
                (int)collision.Height
            );

            if( scene.HasSolidEntityAt( area, floorNumber ) )
                return false;

            return true;
        }

        /// <summary>
        /// Respawns the respawnable <see cref="ZeldaEntity"/>.
        /// </summary>    
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        /// <param name="position">
        /// The respawn position.
        /// </param>
        /// <param name="floorNumber">
        /// The number of the floor to spawn at.
        /// </param>
        public void Respawn( ZeldaScene scene, Vector2 position, int floorNumber )
        {
            Debug.Assert( scene != null );

            // Set position
            this.Owner.Transform.Position = position;
            this.Owner.FloorNumber        = floorNumber;

            // Add to scene
            if( this.Owner.Scene != null )
            {
                if( this.Owner.Scene != scene )
                {
                    this.Owner.RemoveFromScene();
                    this.Owner.AddToScene( scene );
                }
            }
            else
            {
                this.Owner.AddToScene( scene );
            }

            // Notify
            if( this.Respawned != null )
            {
                this.Respawned( this );
            }
        }
        
        /// <summary>
        /// Setups the given <see cref="Respawnable"/> component
        /// to be a clone of this <see cref="Respawnable"/> component.
        /// </summary>
        /// <param name="clone">
        /// The <see cref="Respawnable"/> component to setup as a clone.
        /// </param>
        public void SetupClone( Respawnable clone )
        {
            clone.respawnTime = this.respawnTime;
        }
                
        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int Version = 1;
            context.Write( Version );

            context.Write( this.respawnTime );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int Version = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, Version, this.GetType() );

            this.respawnTime  = context.ReadSingle();
        }
        
        /// <summary>
        /// The time in seconds it takes for the ZeldaEntity to respawn.
        /// </summary>
        private float respawnTime = 180.0f;
    }
}
