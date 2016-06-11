// <copyright file="FogOfWarStatus.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.FogOfWarStatus class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda
{
    using System.Diagnostics;
    using Atom.Math;

    /// <summary>
    /// Stores the visability state of the Fog of War for a single ZeldaScene.
    /// This class can't be inherited.
    /// </summary>
    /// <remarks>
    /// The Fog of War is used to hide undiscovered places on the Mini Map.
    /// This is done by splitting the scene into a grid of size 32x32.
    /// Each cell in this boolean grid states whether the specific position
    /// has been uncovered by the player.
    /// </remarks>
    public sealed class FogOfWarStatus : Zelda.Saving.ISaveable
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the visability state at the given position (in FoW-space).
        /// </summary>
        /// <param name="x">
        /// The position on the x-axis (in FoW-space).
        /// </param>
        /// <param name="y">
        /// The position on the y-axis (in FoW-space).
        /// </param>
        /// <returns>
        /// true if the player has uncovered the area at the given position;
        /// otherwise false.
        /// </returns>
        public bool this[int x, int y]
        {
            get 
            { 
                return this.state[x, y]; 
            }
        }

        /// <summary>
        /// Gets the size of the visability storage used by this FogOfWarStatus.
        /// </summary>
        public int Size
        {
            get 
            { 
                return 32; 
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Uncovers the fog of war at the given <paramref name="position"/>.
        /// </summary>
        /// <param name="position">
        /// The position in world-space.
        /// </param>
        /// <param name="scene">
        /// The related ZeldaScene.
        /// </param>
        internal void Uncover( Vector2 position, ZeldaScene scene )
        {
            // Convert from world-space into scene-space.
            float projectedX = position.X / scene.WidthInPixels;
            float projectedY = position.Y / scene.HeightInPixels;

            // Convert from scene-space into FoW-space.
            int x = (int)(projectedX * this.Size);
            int y = (int)(projectedY * this.Size);

            if( x < 0 || x >= this.Size )
                return;
            if( y < 0 || y >= this.Size )
                return;

            this.state[x, y] = true;
        }

        #region > Stroage <

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            context.Write( this.Size );

            // Write visability states :>
            for( int x = 0; x < this.Size; ++x )
            {
                for( int y = 0; y < this.Size; ++y )
                {
                    context.Write( this.state[x, y] );
                }
            }
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
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            int size = context.ReadInt32();
            Debug.Assert( size == this.Size, "Size mismatch." );

            // Read visability states (:
            for( int x = 0; x < this.Size; ++x )
            {
                for( int y = 0; y < this.Size; ++y )
                {
                    this.state[x, y] = context.ReadBoolean();
                }
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The array of visability flags.
        /// </summary>
        private readonly bool[,] state = new bool[32, 32];

        #endregion
    }
}
