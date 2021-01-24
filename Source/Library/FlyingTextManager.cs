// <copyright file="FlyingTextManager.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.FlyingTextManager class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using System.Globalization;
    using Atom.Collections.Pooling;
    using Atom.Math;
    using Atom.Xna.Fonts;
    using Microsoft.Xna.Framework.Graphics;
    using Xna = Microsoft.Xna.Framework;
    using Atom;

    /// <summary>
    /// Manages the creation, drawing and updating of <see cref="FlyingText"/> instances.
    /// This class can't be inherited.
    /// </summary>
    public sealed class FlyingTextManager
    {
        private static Xna.Color ColorNegative = UI.UIColors.NegativeBright;
        private static Xna.Color ColorNegativeMitigated = new Xna.Color( 255, 84, 84, 255 );

        private static Xna.Color ColorNegativeCrit = UI.UIColors.NegativeLight;
        private static Xna.Color ColorNegativeCritMitigated = new Xna.Color( 200, 42, 42, 255 );

        private static Xna.Color ColorPositive = new Xna.Color( 255, 255, 255 );
        private static Xna.Color ColorPositiveCrit = new Xna.Color( 255, 255, 255 );

        private static readonly Xna.Color ColorInfo = new Xna.Color( 245, 245, 245, 150 );
        private const float ScaleCrit = 1.025f;

        /// <summary>
        /// Gets or sets a value indicating whether this FlyingTextManager is actually
        /// drawing anything.
        /// </summary>
        /// <value>The default value is true.</value>
        public bool IsVisible
        {
            get;
            set;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FlyingTextManager"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public FlyingTextManager( IZeldaServiceProvider serviceProvider )
        {
            this.rand = serviceProvider.Rand;
            this.IsVisible = true;
        }

        /// <summary>
        /// Loads the content required by this FlyingTextManager.
        /// </summary>
        public void LoadContent()
        {
            fontCrit = Zelda.UI.UIFonts.TahomaBold11;
            defaultFont = Zelda.UI.UIFonts.TahomaBold10;
            smallFont = Zelda.UI.UIFonts.TahomaBold8;
        }

        /// <summary>
        /// Unloads all used content.
        /// </summary>
        public void UnloadContent()
        {
            this.pool.Clear();
        }

        /// <summary>
        /// Returns an uninitialized <see cref="FlyingText"/> object that is ready to be used.
        /// </summary>
        /// <returns>
        /// The FlyingText.
        /// </returns>
        public FlyingText Create()
        {
            PoolNode<FlyingText> node = pool.Get();
            FlyingText flyingText = node.Item;

            flyingText.Manager = this;
            flyingText.PoolNode = node;

            return flyingText;
        }

        /// <summary>
        /// Updates the <see cref="FlyingTextManager"/> and all active <see cref="FlyingText"/> instances.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            float frameTime = updateContext.FrameTime;

            foreach( FlyingText text in this.pool )
            {
                text.Update( frameTime );
            }
        }

        /// <summary>
        /// Draws all active <see cref="FlyingText"/> instances.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        public void Draw( ZeldaDrawContext drawContext )
        {
            if( this.IsVisible && this.pool.ActiveCount > 0 )
            {
                foreach( FlyingText text in this.pool )
                {
                    text.Draw( drawContext );
                }
            }
        }

        /// <summary>
        /// Returns the given <see cref="FlyingText"/> to the pool,
        /// marking it non-active.
        /// </summary>
        /// <param name="text">
        /// The FlyingText instance to return.
        /// </param>
        internal void Return( FlyingText text )
        {
            this.pool.Return( text.PoolNode );
        }

        /// <summary>
        /// Removes all FlyingText instances from this FlyingTextManager.
        /// </summary>
        public void Clear()
        {
            this.pool.Clear();
        }
                
        /// <summary>
        /// Fires a FlyingText with default settings.
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        /// <param name="color">The color of the FlyingText.</param>
        /// <param name="text">The text string of the FlyingText.</param>
        private void FireDefault( Vector2 position, Xna.Color color, string text )
        {
            FireDefault( position, color, text, defaultFont, 1.0f );
        }

        /// <summary>
        /// Fires a FlyingText with default settings.
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        /// <param name="color">The color of the FlyingText.</param>
        /// <param name="text">The text string of the FlyingText.</param>
        /// <param name="font">The font of the FlyingText.</param>
        /// <param name="scale">The scaling factor of the FlyingText.</param>
        private void FireDefault(
            Vector2 position,
            Xna.Color color,
            string text,
            IFont font,
            float scale )
        {
            FlyingText flyingText = Create();
            flyingText.Text = text;

            flyingText.Position = position - (font.MeasureString( text ) * (scale / 2.0f));
            flyingText.Font = font;
            flyingText.Time = 3.4f;
            flyingText.Color = color;
            flyingText.Scale = scale;

            const float Speed = 34.0f;

            float angle = Atom.Math.RandRangeExtensions.RandomRange( rand, Constants.Pi, Constants.TwoPi );
            flyingText.Velocity = new Vector2(
                (float)System.Math.Cos( angle ) * Speed,
                (float)System.Math.Sin( angle ) * Speed );

            flyingText.ResetTime();
        }

        /// <summary>
        /// Fires a FlyingText with default settings.
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        /// <param name="color">The color of the FlyingText.</param>
        /// <param name="text">The text string of the FlyingText.</param>
        /// <param name="font">The font to use.</param>
        private void FireHorizontal( Vector2 position, Xna.Color color, string text, IFont font )
        {
            FlyingText flyingText = Create();
            flyingText.Text = text;

            flyingText.Position = position - (font.MeasureString( text ) / 2.0f);
            flyingText.Font = font;
            flyingText.Time = 3.4f;
            flyingText.Color = color;
            flyingText.Scale = 1.0f;

            const float Speed = 34.0f;

            flyingText.Velocity = new Vector2(
                (float)System.Math.Cos( 0.0f ) * Speed,
                (float)System.Math.Sin( 0.0f ) * Speed );

            flyingText.ResetTime();
        }

        private FlyingText FireVertical(
            Vector2 position,
            Xna.Color color,
            string text,
            IFont font,
            float scale )
        {
            return this.FireVertical( position, color, text, font, scale, new Vector2( 34.0f, 34.0f ) );
        }

        /// <summary>
        /// Fires a FlyingText with default settings.
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        /// <param name="color">The color of the FlyingText.</param>
        /// <param name="text">The text string of the FlyingText.</param>
        /// <param name="font">The font of the FlyingText.</param>
        /// <param name="scale">The scaling factor.</param>
        /// <param name="speed">The traveling speed.</param>
        /// <returns>The FlyingText that has been fired.</returns>
        private FlyingText FireVertical(
            Vector2 position,
            Xna.Color color,
            string text,
            IFont font,
            float scale,
            Vector2 speed )
        {
            FlyingText flyingText = Create();
            flyingText.Text = text;

            flyingText.Position = position - (font.MeasureString( text ) * (scale / 2.0f));
            flyingText.Font = font;
            flyingText.Time = 3.4f;
            flyingText.Color = color;
            flyingText.Scale = scale;
            flyingText.Velocity = VerticalDirection * speed;

            flyingText.ResetTime();

            return flyingText;
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        /// <param name="amount">The input value.</param>
        internal void FireGainedExperience( Vector2 position, long amount )
        {
            string str = amount.ToString( Culture ) + ' ' + StringExperience;
            this.FireHorizontal( position, ColorInfo, str, smallFont );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        public void FireGameSaved( Vector2 position )
        {
            FireVertical(
                position,
                Xna.Color.White,
                Resources.GameSaved,
                defaultFont,
                1.0f
            );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        /// <param name="text">The text to show.</param>
        public void FireScreenshotTaken( Vector2 position, string text )
        {
            FireVertical(
                position,
                Xna.Color.White,
                text,
                smallFont,
                1.0f
            );
        }
        
        /// <summary>
        /// Fires a <see cref="FlyingText"/>.
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        internal void FireLevelUp( Vector2 position )
        {
            this.FireVertical( position, Xna.Color.Gold, StringLevelUp, defaultFont, 1.0f );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        /// <param name="amount">The input value.</param>
        internal void FireRestoredLife( Vector2 position, int amount )
        {
            string str = amount.ToString( Culture );
            this.FireVertical( position, Xna.Color.LawnGreen, str, defaultFont, 1.0f );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        /// <param name="amount">The input value.</param>
        internal void FireRestoredLifeCrit( Vector2 position, int amount )
        {
            string str = amount.ToString( Culture );
            this.FireVertical( position, Xna.Color.LawnGreen, str, fontCrit, 1.15f );
        }
        
        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        /// <param name="amount">The input value.</param>
        internal void FireRestoredMana( Vector2 position, int amount )
        {
            string str = amount.ToString( Culture );
            this.FireVertical( position, new Xna.Color( 48, 104, 216 ), str, defaultFont, 1.0f );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        internal void FireAttackOnPlayerMissed( Vector2 position )
        {
            this.FireDefault( position, ColorNegative, StringMiss );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        /// <param name="amount">The input value.</param>
        /// <param name="wasBlocked">States whether the attack was mitigated.</param>
        internal void FireAttackOnPlayerHit( Vector2 position, int amount, bool wasBlocked )
        {
            this.FireDefault( position, wasBlocked ? ColorNegativeMitigated : ColorNegative, amount.ToString( Culture ) );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        internal void FireAttackOnPlayerDodged( Vector2 position )
        {
            this.FireDefault( position, ColorPositive, StringDodge );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        internal void FireAttackOnPlayerParried( Vector2 position )
        {
            this.FireDefault( position, ColorPositive, StringParry );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        internal void FireAttackOnPlayerResisted( Vector2 position )
        {
            this.FireDefault( position, Xna.Color.Violet, StringResisted );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        /// <param name="amount">The input value.</param>
        internal void FireAttackOnPlayerPartiallyResisted( Vector2 position, int amount )
        {
            this.FireDefault( position, Xna.Color.Violet, amount.ToString( Culture ) );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        /// <param name="amount">The input value.</param>
        /// <param name="wasBlocked">States whether the attack was mitigated.</param>
        internal void FireAttackOnPlayerCrit( Vector2 position, int amount, bool wasBlocked )
        {
            this.FireDefault( position, wasBlocked ? ColorNegativeCritMitigated : ColorNegativeCrit, amount.ToString( Culture ), fontCrit, 1.0f );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        internal void FireAttackOnEnemyMissed( Vector2 position )
        {
            this.FireDefault( position, ColorNegative, StringMiss );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        /// <param name="amount">The input value.</param>
        internal void FireAttackOnEnemyHit( Vector2 position, int amount )
        {
            this.FireDefault( position, ColorPositive, amount.ToString( Culture ) );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        internal void FireAttackOnEnemyDodged( Vector2 position )
        {
            this.FireDefault( position, ColorNegative, StringDodge );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        internal void FireAttackOnEnemyParried( Vector2 position )
        {
            this.FireDefault( position, ColorNegative, StringParry );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        internal void FireAttackOnEnemyResisted( Vector2 position )
        {
            this.FireDefault( position, ColorNegativeCrit, StringResisted, smallFont, 1.0f );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        /// <param name="amount">The input value.</param>
        internal void FireAttackOnEnemyPartiallyResisted( Vector2 position, int amount )
        {
            this.FireDefault( position, Xna.Color.LightSteelBlue, amount.ToString( Culture ) );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        /// <param name="amount">The input value.</param>
        internal void FireAttackOnEnemyCrit( Vector2 position, int amount )
        {
            this.FireDefault( position, ColorPositiveCrit, amount.ToString( Culture ), fontCrit, ScaleCrit );
        }
        
        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="questName">The name of the quest.</param>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        internal void FireQuestAccepted( string questName, Vector2 position )
        {
            string text = string.Format(
               Culture,
                Resources.AcceptedQuestX,
                questName
            );

            this.FireVertical( position, Xna.Color.White, text, defaultFont, 1.0f, new Vector2( 0.0f, 25.0f ) );
        }

        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="questName">The name of the quest.</param>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        internal void FireQuestAccomplished( string questName, Vector2 position )
        {
            string text = string.Format(
                Culture,
                Resources.QuestXComplete,
                questName
            );

            this.FireVertical( position, Xna.Color.WhiteSmoke, text, defaultFont, 1.0f, new Vector2( 0.0f, 25.0f ) );
        }
        
        /// <summary>
        /// Fires a <see cref="FlyingText"/>.  
        /// </summary>
        /// <param name="factionName">The name of the faction.</param>
        /// <param name="reputationLevel">The new level of reputation.</param>
        /// <param name="position">The position to spawn the FlyingText at.</param>
        internal void FireReputationLevelChanged( string factionName, Zelda.Factions.ReputationLevel reputationLevel, Vector2 position )
        {
            string text = string.Format(
                Culture,
                Resources.ReputationLevelChangedWithXToY,
                factionName,
                LocalizedEnums.Get( reputationLevel )
            );

            this.FireVertical( position, Xna.Color.GhostWhite, text, defaultFont, 1.0f, new Vector2( 0.0f, 25.0f ) );
        }
        
        /// <summary>
        /// A pool of FlyingText instances.
        /// </summary>
        private readonly Pool<FlyingText> pool = Pool<FlyingText>.Create( 55, () => new FlyingText() );

        /// <summary>
        /// A Random Number Generator.
        /// </summary>
        private readonly Atom.Math.RandMT rand;

        /// <summary>
        /// The font resources.
        /// </summary>
        private IFont defaultFont, fontCrit, smallFont;

        /// <summary>
        /// Identifies the culture used to convert values to strings.
        /// </summary>
        private static readonly CultureInfo Culture = CultureInfo.CurrentCulture;

        /// <summary>
        /// Direction flying text are heading to.
        /// </summary>
        private static readonly Vector2 VerticalDirection = new Vector2( (float)System.Math.Cos( -Constants.PiOver2 ), (float)System.Math.Sin( -Constants.PiOver2 ) );

        /// <summary>
        /// The 'Miss' string cached for fast lockup.
        /// </summary>
        private static readonly string StringMiss = Resources.Miss;

        /// <summary>
        /// The 'Level Up' string cached for fast lockup.
        /// </summary>
        private static readonly string StringLevelUp = Resources.LevelUp;

        /// <summary>
        /// The 'Dodge' string cached for fast lockup.
        /// </summary>
        private static readonly string StringDodge = Resources.Dodge;

        /// <summary>
        /// The 'Parry' string cached for fast lockup.
        /// </summary>
        private static readonly string StringParry = Resources.Parry;

        /// <summary>
        /// The 'Resisted' string cached for fast lockup.
        /// </summary>
        private static readonly string StringResisted = Resources.Resisted;

        /// <summary>
        /// The 'Experience' string cached for fast lockup.
        /// </summary>
        private static readonly string StringExperience = Resources.Experience;
    }
}
