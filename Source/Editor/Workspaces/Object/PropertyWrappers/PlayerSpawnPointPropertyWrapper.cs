// <copyright file="PlayerSpawnPointPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines Zelda.Editor.Object.PropertyWrappers.PlayerSpawnPointPropertyWrapper class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Object.PropertyWrappers
{
    using System.ComponentModel;
    using Atom.Math;
    using Zelda.Entities.Spawning;

    /// <summary>
    /// Defines the IObjectPropertyWrapper for the <see cref="PlayerSpawnPoint"/> type.
    /// </summary>
    internal sealed class PlayerSpawnPointPropertyWrapper : EntityPropertyWrapper<PlayerSpawnPoint>
    {
        [LocalizedDisplayName( "PropDisp_Position" )]
        [LocalizedCategory( "PropCate_Transform" )]
        [LocalizedDescriptionAttribute( "PropDesc_Position" )]
        public Vector2 Position
        {
            get { return this.WrappedObject.Transform.Position; }
            set { this.WrappedObject.Transform.Position = value; }
        }

        [LocalizedDisplayName( "PropDisp_FloorNumber" )]
        [LocalizedCategory( "PropCate_Transform" )]
        [LocalizedDescriptionAttribute( "PropDesc_FloorNumber" )]
        [DefaultValue( 0 )]
        public int FloorNumber
        {
            get { return this.WrappedObject.FloorNumber; }
            set { this.WrappedObject.FloorNumber = value; }
        }

        [LocalizedDisplayName( "PropDisp_SpawnDirection" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_SpawnDirection" )]
        public Direction4 SpawnDirection
        {
            get { return this.WrappedObject.SpawnDirection; }
            set { this.WrappedObject.SpawnDirection = value; }
        }

        [LocalizedDisplayName( "PropDisp_SpawnArea" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_SpawnArea" )]
        public RectangleF SpawnArea
        {
            get { return this.WrappedObject.Collision.Rectangle; }
            set
            {
                this.WrappedObject.Transform.Position = value.Position;
                this.WrappedObject.Collision.Size     = value.Size;
            }
        }

        [LocalizedDisplayName( "PropDisp_SaveLocationAtSpawn" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_SaveLocationAtSpawn" )]
        [DefaultValue(false)]
        public bool SaveLocationAtSpawn
        {
            get { return this.WrappedObject.SaveLocationAtSpawn; }
            set { this.WrappedObject.SaveLocationAtSpawn = value; }
        }

        /// <summary>
        /// Returns a clone of this <see cref="IObjectPropertyWrapper"/>.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new PlayerSpawnPointPropertyWrapper( serviceProvider );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerSpawnPointPropertyWrapper"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public PlayerSpawnPointPropertyWrapper( IZeldaServiceProvider serviceProvider )
        {
            System.Diagnostics.Debug.Assert( serviceProvider != null );
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;
    }
}
