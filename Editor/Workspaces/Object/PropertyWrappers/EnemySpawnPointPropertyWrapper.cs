// <copyright file="EnemySpawnPointPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines Zelda.Editor.Object.PropertyWrappers.EnemySpawnPointPropertyWrapper class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Object.PropertyWrappers
{
    using System.Collections.Generic;
    using Atom.Math;
    using Zelda.Entities.Spawning;
    
    /// <summary>
    /// Defines the IObjectPropertyWrapper for EnemySpawnPoints.
    /// </summary>
    internal sealed class EnemySpawnPointPropertyWrapper : EntityPropertyWrapper<EnemySpawnPoint>
    {
        #region > Transform <

        [LocalizedDisplayName( "PropDisp_Position" )]
        [LocalizedCategory( "PropCate_Transform" )]
        [LocalizedDescriptionAttribute( "PropDesc_Position" )]
        public Vector2 Position
        {
            get { return this.WrappedObject.Transform.Position; }
            set { this.WrappedObject.Transform.Position = value; }
        }

        #endregion

        #region > Settings <

        [LocalizedDisplayName( "PropDisp_FloorNumber" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_FloorNumber" )]
        public int FloorNumber
        {
            get { return this.WrappedObject.FloorNumber; }
            set { this.WrappedObject.FloorNumber = value; }
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

        [LocalizedDisplayName( "PropDisp_RespawnGroups" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_RespawnGroups" )]
        [System.ComponentModel.Editor( typeof( Zelda.Entities.Spawning.Design.EnemyRespawnGroupListEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public List<IEnemyRespawnGroup> RespawnGroups
        {
            get { return this.WrappedObject.RespawnGroups; }
        }

        #endregion

        /// <summary>
        /// Returns a clone of this <see cref="IObjectPropertyWrapper"/>.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new EnemySpawnPointPropertyWrapper( serviceProvider );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemySpawnPointPropertyWrapper"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public EnemySpawnPointPropertyWrapper( IZeldaServiceProvider serviceProvider )
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
