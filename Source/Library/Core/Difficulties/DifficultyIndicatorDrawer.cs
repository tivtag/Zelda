// <copyright file="DifficultyIndicatorDrawer.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Difficulties.DifficultyIndicatorDrawer class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Difficulties
{
    using Atom.Math;
    using Atom.Xna;

    /// <summary>
    /// Implements a mechanism that draws a simple indicator for the difficulty of the game.
    /// </summary>
    public sealed class DifficultyIndicatorDrawer
    {
        /// <summary>
        /// Gets or sets the position the indicator is drawn at.
        /// </summary>
        public Vector2 Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="HorizontalAlignment"/> of the indicator.
        /// </summary>
        public HorizontalAlignment Alignment
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the DifficultyIndicatorDrawer class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public DifficultyIndicatorDrawer( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Loads the resources this DifficultyIndicatorDrawer uses.
        /// </summary>
        public void LoadContent()
        { 
            this.spriteSkull = this.serviceProvider.SpriteLoader.LoadSprite( "SmallSkeletonHead" );
        }

        /// <summary>
        /// Draws the indicator for the IDifficulty that is identified by the specified DifficultyId.
        /// </summary>
        /// <param name="difficulty">
        /// The DifficultyId that has been found.
        /// </param>
        /// <param name="hardcore">
        /// States whether hardcore modus is enabled.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        public void Draw( DifficultyId difficulty, bool hardcore, ISpriteDrawContext drawContext )
        {
            int skullCount = (int)difficulty;
            var color = hardcore ? Microsoft.Xna.Framework.Color.Red : Microsoft.Xna.Framework.Color.White;

            if( skullCount > 0 )
            {
                const int Gap = 3;
                int width = this.spriteSkull.Width;
                int totalWidth = (skullCount * (width + Gap)) - Gap;

                switch( this.Alignment )
                {
                    case HorizontalAlignment.Center:
                        {
                            Vector2 position = this.Position - new Vector2( totalWidth / 2, 0.0f );

                            for( int i = 0; i < skullCount; ++i )
                            {
                                this.spriteSkull.Draw( position, color, drawContext.Batch );
                                position.X += width + Gap;
                            }
                        }
                        break;

                    case HorizontalAlignment.Right:
                        {
                            Vector2 position = this.Position - new Vector2( totalWidth, 0.0f );

                            for( int i = 0; i < skullCount; ++i )
                            {
                                this.spriteSkull.Draw( position, color, drawContext.Batch );
                                position.X += width + Gap;
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// The Sprite that is used to visualize the difficulty.
        /// </summary>
        private Sprite spriteSkull;

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;
    }
}
