// <copyright file="SavePoint.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Saving.SavePoint structure.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Saving
{
    /// <summary>
    /// Defines the properties of a Save Point in the world.
    /// </summary>
    public struct SavePoint
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the name that uniquely identifies the Scene 
        /// in which the player has saved.
        /// </summary>
        public string Scene
        {
            get 
            {
                return this.scene; 
            }
        }

        /// <summary>
        /// Gets the name that uniquely identifies the Spawn Point in the Scene.
        /// </summary>
        public string SpawnPoint
        {
            get { return spawnPoint; }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="SavePoint"/> struct.
        /// </summary>
        /// <param name="sceneName">
        /// The name of the scene.
        /// </param>
        /// <param name="spawnPointName">
        /// The name of the spawn point.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="sceneName"/> or <paramref name="spawnPointName"/> is null.
        /// </exception>
        public SavePoint( string sceneName, string spawnPointName )
        {
            if( sceneName == null )
                throw new System.ArgumentNullException( "sceneName" );

            if( spawnPointName == null )
                throw new System.ArgumentNullException( "spawnPointName" );

            this.scene      = sceneName;
            this.spawnPoint = spawnPointName;
        }

        #endregion 

        #region [ Fields ]

        /// <summary>
        /// The name of the scene.
        /// </summary>
        private readonly string scene;

        /// <summary>
        /// The name of the spawn point.
        /// </summary>
        private readonly string spawnPoint;

        #endregion
    }
}
