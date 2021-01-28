// <copyright file="BaseCharacterWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.BaseCharacterWindow class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI
{
    using System.Globalization;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Microsoft.Xna.Framework.Graphics;
    using Zelda.Difficulties;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Represents the abstract base IngameWindow class for the <see cref="CharacterWindow"/> and <see cref="CharacterDetailsWindow"/>
    /// classes.
    /// </summary>
    internal abstract class BaseCharacterWindow : IngameWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterWindow"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related servicess.
        /// </param>
        /// <param name="offsetY">
        /// An optional offset applied to the window's position.
        /// </param>
        public BaseCharacterWindow( IZeldaServiceProvider serviceProvider, int offsetY = 0 )
        {
            // Set properties.
            this.Size = serviceProvider.ViewSize;
            this.CenterX = (int)this.Size.X / 2;

            // Load Content.
            this.spriteBackground = serviceProvider.SpriteLoader.LoadSprite( "StatusWindow" );

            this.leftSpritePosition = this.Size / 2 - this.spriteBackground.Size / 2 + new Point2( 0, offsetY );
            this.rightSpritePosition = this.leftSpritePosition + new Vector2( this.spriteBackground.Width, 0.0f );
            this.LineStartY = (int)rightSpritePosition.Y + 30;

            this.difficultyIndicatorDrawer = new DifficultyIndicatorDrawer( serviceProvider ) {
                Alignment = HorizontalAlignment.Right,
                Position = new Vector2( serviceProvider.ViewSize.X - 10.0f, 5.0f + offsetY )
            };
            this.difficultyIndicatorDrawer.LoadContent();
        }

        /// <summary>
        /// Draws the background of this BaseCharacterWindow. 
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected virtual void DrawBackground( ISpriteDrawContext drawContext )
        {
            this.spriteBackground.Draw( this.leftSpritePosition, drawContext.Batch );
        }

        /// <summary>
        /// Draws the basic background and statistics all BaseCharacterWindows share.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected virtual void DrawBasicStatistics( ISpriteDrawContext drawContext )
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            Status.ExtendedStatable statable = this.Player.Statable;

            // Draw Player's name:
            DrawText(
                fontLargeText,
                this.Player.Name,
                new Vector2( (int)(CenterX - (fontLargeText.MeasureString( this.Player.Name ).X / 2)), -3.0f ),
                Xna.Color.White,
                drawContext
            );

            // Draw Player's level and class tag:
            string strLevelClass = string.Format(
                culture,
                Resources.LevelXClassY,
                statable.Level.ToString( culture ),
                this.GetCharacterClassName()
            );

            DrawText(
                fontSmallText,
                strLevelClass,
                new Vector2( CenterX - (int)(fontSmallText.MeasureString( strLevelClass ).X / 2), 11.0f ),
                Xna.Color.White,
                drawContext
            );

            // Draw Player's experience:
            string strExp = string.Format(
                culture,
                "{0}/{1} {2}",
                 statable.Experience.ToString( culture ),
                 statable.ExperienceNeeded.ToString( culture ),
                 Resources.Experience
            );

            DrawText(
                fontText,
                strExp,
                new Vector2( CenterX - (int)(fontText.MeasureString( strExp ).X / 2), leftSpritePosition.Y + 3.0f ),
                Xna.Color.White,
                drawContext
            );

            // Draw Life:
            string strLife = string.Format(
                culture,
                "{0}/{1}",
                statable.Life.ToString( culture ),
                statable.MaximumLife.ToString( culture )
            );

            DrawText(
                fontText,
                strLife,
                this.leftSpritePosition + new Vector2( 5.0f, 3.0f ),
                new Xna.Color( 255, 99, 99 ),
                drawContext
            );

            // Draw Mana:
            string strMana = string.Format(
                culture,
                "{0}/{1}",
                statable.Mana.ToString( culture ),
                statable.MaximumMana.ToString( culture )
            );

            DrawText(
                fontText,
                strMana,
                this.rightSpritePosition + new Vector2( -4 - (int)fontText.MeasureString( strMana ).X, 3.0f ),
                new Xna.Color( 48, 104, 216 ),
                drawContext
            );

            // Draw difficulty indicator:
            this.difficultyIndicatorDrawer.Draw( this.Player.Profile.Difficulty, this.Player.Profile.Hardcore, drawContext );
        }

        /// <summary>
        /// Helper function that draws the given text.
        /// </summary>
        /// <param name="font">The font to draw the text with.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="position">The position to draw the text at.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="drawContext">The current ISpriteDrawContext</param>
        protected static void DrawText( IFont font, string text, Vector2 position, Xna.Color color, ISpriteDrawContext drawContext )
        {
            font.Draw(
                text,
                new Vector2( (int)position.X, (int)position.Y ),
                color,
                0.0f,
                Vector2.Zero,
                1.0f,
                SpriteEffects.None,
                0.01f,
                drawContext
            );
        }

        /// <summary>
        /// Gets the (localized) name of the character class of the player.
        /// </summary>
        private string GetCharacterClassName()
        {
            string className = this.Player.ClassName;

            if( !string.IsNullOrEmpty( className ) )
            {
                return className;
            }
            else
            {
                Talents.TalentTreeStatistics talentStats = this.Player.TalentTree.Statistics;

                return string.Format(
                    "Unknown Class ({0}/{1}/{2}/{3})",
                    talentStats.InvestedPointsMelee,
                    talentStats.InvestedPointsRanged,
                    talentStats.InvestedPointsMagic,
                    talentStats.InvestedPointsSupport
                );
            }
        }

        /// <summary>
        /// Called when this BaseCharacterWindow is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
        }

        /// <summary>
        /// The background sprite.
        /// </summary>
        private readonly Sprite spriteBackground;

        /// <summary>
        /// Cached ingame coordinate.
        /// </summary>
        protected readonly int CenterX;

        /// <summary>
        /// Cached ingame coordinate.
        /// </summary>
        protected int LineStartY, LineSpacing = 24;

        /// <summary>
        /// The upper positions of the background sprite.
        /// </summary>
        protected readonly Vector2 leftSpritePosition, rightSpritePosition;

        /// <summary>
        /// The fonts used in the CharacterWindow.
        /// </summary>
        protected readonly IFont
            fontLargeText = UIFonts.TahomaBold11,
            fontText = UIFonts.TahomaBold10,
            fontSmallText = UIFonts.Tahoma10,
            fontVerySmallText = UIFonts.Tahoma7;

        /// <summary>
        /// Implements a mechanism that allows the visualization of the difficulty the current
        /// character has choosen.
        /// </summary>
        private readonly DifficultyIndicatorDrawer difficultyIndicatorDrawer;
    }
}
