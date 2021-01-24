// <copyright file="HeartBar.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.HeartBar class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI
{
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.UI;
    using Zelda.Entities;

    /// <summary>
    /// Visualizes the health status of the player using
    /// hearts, just like in any other Zelda Game.
    /// </summary>
    /// <remarks>
    /// The difference is that the Player isn't going to gain any hearts,
    /// but starts with 20.
    /// <para>
    /// 20 hearts shown = 100% life
    /// 10 hearts shown =  50% life
    ///  0 hearts shown =   0% life
    /// </para>
    /// </remarks>
    internal sealed class HeartBar : UIElement
    {
        #region [ Constants ]

        /// <summary>
        /// The time the heart animation stays in one state before changing. (little-big-little-.. tick)
        /// </summary>
        private const float HeartTickTime = 2.5f;

        /// <summary>
        /// The number of hearts used to display the player's life/maximum life ratio.
        /// </summary>
        private const int HeartCount = 20;

        #endregion

        #region [ Properties ]

        /// <summary> 
        /// Gets or sets the PlayerEntity whos Life State
        /// is visualized by the <see cref="HeartBar"/>. 
        /// </summary>
        public PlayerEntity Player
        { 
            get; 
            set; 
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="HeartBar"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides access to game-related services.
        /// </param>
        internal HeartBar( IZeldaServiceProvider serviceProvider )
            : base( "HeartBar" )
        {
            this.Position = new Vector2( 5.0f, 5.0f );

            var spriteLoader = serviceProvider.SpriteLoader;
            this.spriteHeart_Small0of4 = spriteLoader.LoadSprite( "Heart_Small0of4" );
            this.spriteHeart_Small1of4 = spriteLoader.LoadSprite( "Heart_Small1of4" );
            this.spriteHeart_Small2of4 = spriteLoader.LoadSprite( "Heart_Small2of4" );
            this.spriteHeart_Small3of4 = spriteLoader.LoadSprite( "Heart_Small3of4" );
            this.spriteHeart_Small4of4 = spriteLoader.LoadSprite( "Heart_Small4of4" );

            this.spriteHeart_Big1of4 = spriteLoader.LoadSprite( "Heart_Big1of4" );
            this.spriteHeart_Big2of4 = spriteLoader.LoadSprite( "Heart_Big2of4" );
            this.spriteHeart_Big3of4 = spriteLoader.LoadSprite( "Heart_Big3of4" );
            this.spriteHeart_Big4of4 = spriteLoader.LoadSprite( "Heart_Big4of4" );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this UIElement is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        {
            if( this.Player == null )
                return;

            var batch    = drawContext.Batch;
            var statable = this.Player.Statable;

            float factor = statable.MaximumLife == 0.0f ? 0.0f : (float)statable.Life / (float)statable.MaximumLife;
            float hearts = factor * (float)HeartCount;

            int fullHearts    = (int)hearts;
            float extraHearts = hearts - fullHearts;

            int x = (int)this.Position.X, y = (int)this.Position.Y;
            int heartOffsetX = 10, heartOffsetY = spriteHeart_Big4of4.Height;
            int i = 0, offsetX = 0;

            #region if( numberOfFullHearts == 0 )

            if( fullHearts == 0 )
            {
                if( lastHeartTick < HeartTickTime )
                {
                    Sprite sprite = 
                        extraHearts == 0.0f  ? spriteHeart_Small0of4 :
                        extraHearts <= 0.25f ? spriteHeart_Small1of4 :
                        extraHearts <= 0.5f  ? spriteHeart_Small2of4 :
                        extraHearts <= 0.8f  ? spriteHeart_Small3of4 :
                                               spriteHeart_Small4of4;

                    sprite.Draw( new Vector2( x, y ), batch );
                }
                else if( lastHeartTick < HeartTickTime * 2.0f )
                {
                    if( extraHearts == 0.0f )
                    {
                        spriteHeart_Small0of4.Draw( new Vector2( x, y ), batch );
                    }
                    else
                    {
                        Sprite sprite =
                         extraHearts <= 0.25f ? spriteHeart_Big1of4 :
                         extraHearts <= 0.5f  ? spriteHeart_Big2of4 :
                         extraHearts <= 0.8f  ? spriteHeart_Big3of4 :
                                                spriteHeart_Big4of4;

                        sprite.Draw( new Vector2( x, y - 2 ), batch );
                    }
                }
                else
                {
                    Sprite sprite =
                       extraHearts == 0.0f  ? spriteHeart_Small0of4 :
                       extraHearts <= 0.25f ? spriteHeart_Small1of4 :
                       extraHearts <= 0.5f  ? spriteHeart_Small2of4 :
                       extraHearts <= 0.8f  ? spriteHeart_Small3of4 :
                                              spriteHeart_Small4of4;

                    sprite.Draw( new Vector2( x, y ), batch );
                    lastHeartTick = 0.0f;
                }

                offsetX += 10;
                ++i;
            }

            #endregion

            #region else
            else
            {
                int fullHeartsToRender = (extraHearts == 0.0f) ? fullHearts - 1 : fullHearts;

                for(; i < fullHeartsToRender; ++i )
                {
                    if( i % 10 == 0 && i != 0 )
                    {
                        y += heartOffsetY;
                        offsetX = 0;
                    }

                    spriteHeart_Small4of4.Draw( new Vector2( x + offsetX, y ), batch );
                    offsetX += heartOffsetX;
                }

                if( i % 10 == 0 && i != 0 )
                {
                    y += heartOffsetY;
                    offsetX = 0;
                }

                if( lastHeartTick < HeartTickTime )
                {
                    Sprite sprite =
                    extraHearts == 0.0f  ? spriteHeart_Small4of4 :
                    extraHearts <= 0.25f ? spriteHeart_Small1of4 :
                    extraHearts <= 0.5f  ? spriteHeart_Small2of4 :
                    extraHearts <= 0.8f  ? spriteHeart_Small3of4 :
                                           spriteHeart_Small4of4;

                    sprite.Draw( new Vector2( x + offsetX, y ), batch );
                }
                else if( lastHeartTick < HeartTickTime * 2.0f )
                {
                    Sprite sprite =
                        extraHearts == 0.0f  ? spriteHeart_Big4of4 :
                        extraHearts <= 0.25f ? spriteHeart_Big1of4 :
                        extraHearts <= 0.5f  ? spriteHeart_Big2of4 :
                        extraHearts <= 0.8f  ? spriteHeart_Big3of4 :
                                               spriteHeart_Big4of4;

                    sprite.Draw( new Vector2( x + offsetX, y - 2 ), batch );
                }
                else
                {
                    Sprite sprite =
                        extraHearts == 0.0f  ? spriteHeart_Small4of4 :
                        extraHearts <= 0.25f ? spriteHeart_Small1of4 :
                        extraHearts <= 0.5f  ? spriteHeart_Small2of4 :
                        extraHearts <= 0.8f  ? spriteHeart_Small3of4 :
                                               spriteHeart_Small4of4;

                    sprite.Draw( new Vector2( x + offsetX, y ), batch );
                    lastHeartTick = 0;
                }

                offsetX += heartOffsetX;
                ++i;
            }

            #endregion

            // Now draw the empty hearts.
            for(; i < HeartCount; ++i )
            {
                if( i % 10 == 0 )
                {
                    y += heartOffsetY;
                    offsetX = 0;
                }

                spriteHeart_Small0of4.Draw( new Vector2( x + offsetX, y ), batch );
                offsetX += heartOffsetX;
            }
        }

        /// <summary>
        /// Called when this UIElement is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
            lastHeartTick += updateContext.FrameTime;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Current animation tick of the last heart.
        /// </summary>
        private float lastHeartTick;

        /// <summary>
        /// The big heart sprites.
        /// </summary>
        private readonly Sprite 
            spriteHeart_Big1of4, spriteHeart_Big2of4,
            spriteHeart_Big3of4, spriteHeart_Big4of4;

        /// <summary>
        /// The small heart sprites.
        /// </summary>
        private readonly Sprite 
            spriteHeart_Small0of4, spriteHeart_Small1of4,
            spriteHeart_Small2of4, spriteHeart_Small3of4,
            spriteHeart_Small4of4;

        #endregion
    }
}