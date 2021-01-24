// <copyright file="CharacterDetailWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.CharacterDetailsWindow class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.UI
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI.Tooltips;
    using Zelda.Factions;
    using Zelda.Status;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Allows the player to see detailed character status information.
    /// </summary>
    internal sealed class CharacterDetailsWindow : BaseCharacterWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterDetailsWindow"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related servicess.
        /// </param>
        internal CharacterDetailsWindow( IZeldaServiceProvider serviceProvider )
            : base( serviceProvider )
        {
            this.SetupTooltips( serviceProvider );
        }

        /// <summary>
        /// Setups the tooltips of this CharacterDetailsWindow.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related servicess.
        /// </param>
        private void SetupTooltips( IZeldaServiceProvider serviceProvider )
        {
            float X = leftSpritePosition.X + 20;
            float X2 = X + 170.0f;
            float DefaultY = leftSpritePosition.Y + 30;
            float y = DefaultY;

            this.AddInlineTooltip(
                Resources.SpellPower,
                new Vector2( X, y ),
                this.DrawSpellPowerTooltip
            );

            this.AddInlineTooltip(
                Resources.CriticalBonus,
                new Vector2( X, y += 20.0f ),
                this.DrawCriticalBonusTooltip
            );

            this.AddInlineTooltip(
                Resources.ArmorIgnore,
                new Vector2( X, y += 20.0f ),
                this.DrawArmorIgnoreTooltip
            );

            this.AddInlineTooltip(
                 Resources.PierceChance,
                 new Vector2( X, y += 20.0f ),
                 this.DrawPiercingChanceTooltip
             );

            this.AddInlineTooltip(
                Resources.PushingForce,
                new Vector2( X, y += 20.0f ),
                this.DrawPushingForceTooltip
            );

            this.AddInlineTooltip(
                Resources.MovementSpeed,
                new Vector2( X, y += 20.0f ),
                this.DrawMovementSpeedTooltip
            );

            this.AddInlineTooltip(
                Resources.Reputation,
                new Vector2( X, y += 20.0f ),
                this.DrawReputationTooltip
            );

            y = DefaultY;
            this.AddInlineTooltip(
                Resources.ChanceTo,
                new Vector2( X2, y ),
                this.DrawChanceToTooltip
            );

            this.AddInlineTooltip(
                Resources.ChanceToBe,
                new Vector2( X2, y += 20.0f ),
                this.DrawChanceToBeTooltip
            );

            this.AddInlineTooltip(
                Resources.Resistance,
                new Vector2( X2, y += 20.0f ),
                this.DrawResistanceTooltip
            );

            this.AddInlineTooltip(
                Resources.Block,
                new Vector2( X2, y += 20.0f ),
                this.DrawBlockTooltip
            );

            this.AddInlineTooltip(
                Resources.PotionEffectiveness,
                new Vector2( X2, y += 20.0f ),
                this.DrawPotionEffectivenessTooltip
            );

            this.AddInlineTooltip(
                Resources.Miscellaneous,
                new Vector2( X2, y += 20 ),
                this.DrawMiscellaneousTooltip
            );

            this.AddInlineTooltip(
                Resources.Statistics,
                new Vector2( X2, y += 20.0f ),
                this.DrawStatisticsTooltip
            );
        }

        /// <summary>
        /// Adds a new InlineTooltip to this CharacterDetailsWindow.
        /// </summary>
        /// <param name="header">
        /// The text that should be displayed within the box.
        /// </param>
        /// <param name="position">
        /// The position of the box.
        /// </param>
        /// <param name="drawAction">
        /// The action that should be executed when the players moves the mouse
        /// over the header text.
        /// </param>
        private void AddInlineTooltip( string header, Vector2 position, Action<Tooltip, ISpriteDrawContext> drawAction )
        {
            this.inlineTooltips.Add(
                new InlineTooltip( header, this.fontVerySmallText, drawAction ) {
                    Position = position
                }
            );
        }

        /// <summary>
        /// Draws the 'Potion Effectiveness' tooltip.
        /// </summary>
        /// <param name="tooltip">
        /// The tooltip that is active.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawMovementSpeedTooltip( Tooltip tooltip, ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;

            // Measure
            Point2 position = this.Owner.MousePosition;
            Point2 tooltipSize = new Point2( 50, this.fontVerySmallText.LineSpacing + 2 );
            Rectangle area = new Rectangle( position.X, position.Y, tooltipSize.X, tooltipSize.Y );

            // Background            
            DrawTooltipBackground( area, zeldaDrawContext );

            // Text
            var moveable = this.Player.Moveable;

            this.fontVerySmallText.Draw(
                Math.Round( moveable.Speed, 2 ).ToString(),
                new Vector2( position.X + tooltipSize.X / 2, position.Y + 2 ),
                TextAlign.Center,
                Xna.Color.White,
                0.6f,
                drawContext
            );
        }

        /// <summary>
        /// Draws the 'Critical Bonus' tooltip.
        /// </summary>
        /// <param name="tooltip">
        /// The tooltip that is active.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawCriticalBonusTooltip( Tooltip tooltip, ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;

            // Measure
            Point2 position = this.Owner.MousePosition;
            Point2 tooltipSize = new Point2( 80, 5 * (this.fontVerySmallText.LineSpacing + 2) );
            Rectangle area = new Rectangle( position.X, position.Y, tooltipSize.X, tooltipSize.Y );

            position.Y += 2;
            Vector2 left = new Vector2( position.X + 3.0f, position.Y );
            Vector2 right = new Vector2( position.X + tooltipSize.X - 2.0f, position.Y );

            // Background            
            DrawTooltipBackground( area, zeldaDrawContext );

            // Text
            var container = this.Player.Statable.DamageDone.WithCritical;

            // Melee
            this.fontVerySmallText.Draw(
                Resources.Melee + ":",
                left,
                Xna.Color.White,
                0.6f,
                drawContext
            );

            this.fontVerySmallText.Draw(
                ConvertModifierToString( container.Melee, 0 ) + "%",
                right,
                TextAlign.Right,
                Xna.Color.White,
                0.6f,
                drawContext
            );

            // Ranged
            this.fontVerySmallText.Draw(
                Resources.Ranged + ":",
                left + new Vector2( 0.0f, fontVerySmallText.LineSpacing + 2.0f ),
                Xna.Color.White,
                0.6f,
                drawContext
            );

            this.fontVerySmallText.Draw(
                ConvertModifierToString( container.Ranged, 0 ) + "%",
                right + new Vector2( 0.0f, fontVerySmallText.LineSpacing + 2.0f ),
                TextAlign.Right,
                Xna.Color.White,
                0.6f,
                drawContext
            );

            // Spell
            this.fontVerySmallText.Draw(
                Resources.Spell + ":",
                left + new Vector2( 0.0f, (2.0f * fontVerySmallText.LineSpacing) + 4.0f ),
                Xna.Color.White,
                0.6f,
                drawContext
            );

            this.fontVerySmallText.Draw(
                ConvertModifierToString( container.Spell, 0 ) + "%",
                right + new Vector2( 0.0f, (2.0f * fontVerySmallText.LineSpacing) + 4.0f ),
                TextAlign.Right,
                Xna.Color.White,
                0.6f,
                drawContext
            );

            // Heal
            this.fontVerySmallText.Draw(
                Resources.Heal + ":",
                left + new Vector2( 0.0f, (3.0f * fontVerySmallText.LineSpacing) + 6.0f ),
                Xna.Color.White,
                0.6f,
                drawContext
            );

            this.fontVerySmallText.Draw(
                ConvertModifierToString( Player.Statable.CritModifierHeal, 0 ) + "%",
                right + new Vector2( 0.0f, (3.0f * fontVerySmallText.LineSpacing) + 6.0f ),
                TextAlign.Right,
                Xna.Color.White,
                0.6f,
                drawContext
            );

            // Block
            this.fontVerySmallText.Draw(
                Resources.Block + ":",
                left + new Vector2( 0.0f, (4.0f * fontVerySmallText.LineSpacing) + 8.0f ),
                Xna.Color.White,
                0.6f,
                drawContext
            );

            this.fontVerySmallText.Draw(
                ConvertModifierToString( Player.Statable.CritModifierBlock, 0 ) + "%",
                right + new Vector2( 0.0f, (4.0f * fontVerySmallText.LineSpacing) + 8.0f ),
                TextAlign.Right,
                Xna.Color.White,
                0.6f,
                drawContext
            );
        }

        /// <summary>
        /// Draws the 'Potion Effectiveness' tooltip.
        /// </summary>
        /// <param name="tooltip">
        /// The tooltip that is active.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawPotionEffectivenessTooltip( Tooltip tooltip, ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;

            // Measure
            Point2 position = this.Owner.MousePosition;
            Point2 tooltipSize = new Point2( 80, (2 * this.fontVerySmallText.LineSpacing) + 4 );
            Rectangle area = new Rectangle( position.X, position.Y, tooltipSize.X, tooltipSize.Y );

            position.Y += 2;
            Vector2 left = new Vector2( position.X + 3.0f, position.Y );
            Vector2 right = new Vector2( position.X + tooltipSize.X - 2.0f, position.Y );

            // Background            
            DrawTooltipBackground( area, zeldaDrawContext );

            // Text
            var container = this.Player.Statable;

            // Life
            this.fontVerySmallText.Draw(
                Resources.Life + ":",
                left,
                Xna.Color.White,
                0.6f,
                drawContext
            );

            this.fontVerySmallText.Draw(
                ConvertModifierToString( 1.0f + container.LifePotionEffectiviness ) + "%",
                right,
                TextAlign.Right,
                Xna.Color.White,
                0.6f,
                drawContext
            );

            // Mana
            this.fontVerySmallText.Draw(
                Resources.Mana + ":",
                left + new Vector2( 0.0f, fontVerySmallText.LineSpacing + 2.0f ),
                Xna.Color.White,
                0.6f,
                drawContext
            );

            this.fontVerySmallText.Draw(
                ConvertModifierToString( 1.0f + container.ManaPotionEffectiviness ) + "%",
                right + new Vector2( 0.0f, fontVerySmallText.LineSpacing + 2.0f ),
                TextAlign.Right,
                Xna.Color.White,
                0.6f,
                drawContext
            );
        }

        /// <summary>
        /// Draws the 'Armor Ignore' tooltip.
        /// </summary>
        /// <param name="tooltip">
        /// The tooltip that is active.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawArmorIgnoreTooltip( Tooltip tooltip, ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;

            // Measure
            Point2 position = this.Owner.MousePosition;
            Point2 tooltipSize = new Point2( 70, this.fontVerySmallText.LineSpacing + 2 );
            Rectangle area = new Rectangle( position.X, position.Y, tooltipSize.X, tooltipSize.Y );

            position.Y += 2;
            Vector2 center = new Vector2( position.X + tooltipSize.X / 2.0f + 1, position.Y );

            // Background            
            DrawTooltipBackground( area, zeldaDrawContext );

            // Text
            var container = this.Player.Statable;
            double percentage = Math.Floor( 100.0f - container.ArmorIgnoreMultiplier * 100.0f );

            this.fontVerySmallText.Draw(
                string.Format(
                    "{0} {1}%",
                    container.ArmorIgnore.ToString( CultureInfo.CurrentCulture ),
                    percentage >= 0.0f ?
                        "+ " + percentage.ToString( CultureInfo.CurrentCulture ) :
                        "- " + (-percentage).ToString( CultureInfo.CurrentCulture )
                ),
                center,
                TextAlign.Center,
                Xna.Color.White,
                0.6f,
                drawContext
            );
        }

        /// <summary>
        /// Draws the 'Pushing Force' tooltip.
        /// </summary>
        /// <param name="tooltip">
        /// The tooltip that is active.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawPushingForceTooltip( Tooltip tooltip, ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;

            // Measure
            Point2 position = this.Owner.MousePosition;
            Point2 tooltipSize = new Point2( 70, this.fontVerySmallText.LineSpacing + 2 );
            Rectangle area = new Rectangle( position.X, position.Y, tooltipSize.X, tooltipSize.Y );

            position.Y += 2;
            Vector2 center = new Vector2( position.X + tooltipSize.X / 2.0f + 1, position.Y );

            // Background            
            DrawTooltipBackground( area, zeldaDrawContext );

            // Text
            var container = this.Player.Statable;

            float percentage = ConvertToPercentage( container.PushingForceMultiplicative, 1 );

            this.fontVerySmallText.Draw(
                string.Format(
                    "{0} {1}%",
                    container.PushingForceAdditive.ToString( CultureInfo.CurrentCulture ),
                    percentage >= 0.0f ?
                        "+ " + percentage.ToString( CultureInfo.CurrentCulture ) :
                        "- " + (-percentage).ToString( CultureInfo.CurrentCulture )
                ),
                center,
                TextAlign.Center,
                Xna.Color.White,
                0.6f,
                drawContext
            );
        }

        /// <summary>
        /// Draws the 'Miscellaneous' tooltip.
        /// </summary>
        /// <param name="tooltip">
        /// The tooltip that is active.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawMiscellaneousTooltip( Tooltip tooltip, ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;

            // Measure
            Point2 position = this.Owner.MousePosition;
            Point2 tooltipSize = new Point2( 135, (this.fontVerySmallText.LineSpacing * 2) + 2 );
            Rectangle area = new Rectangle( position.X, position.Y, tooltipSize.X, tooltipSize.Y );

            position.Y += 2;

            // Background            
            DrawTooltipBackground( area, zeldaDrawContext );
            area.Y += 2;

            // Text
            var container = this.Player.Statable;

            // Light Radius
            float radius = (float)Math.Round( this.Player.Latern.ExtraLightRadius, 1 );

            DrawText(
                Resources.LightRadius,
                string.Format(
                    CultureInfo.CurrentCulture,
                    radius >= 0 ? "+{0}" : "{0}",
                    radius.ToString( CultureInfo.CurrentCulture )
                ),
                new Vector2( area.X + 3.0f, area.Y ),
                new Vector2( area.X + tooltipSize.X - 2.0f, area.Y ),
                radius < 0 ? UIColors.NegativeLight : Xna.Color.White,
                drawContext
            );

            area.Y += this.fontVerySmallText.LineSpacing;

            // Experience gained
            float percentage = ConvertToPercentage( container.ExperienceGainedModifier, 1 );

            DrawText(
                Resources.ExperienceGained,
                string.Format(
                    "{0} {1}%",
                    container.FixedExperienceGainedModifier.ToString( CultureInfo.CurrentCulture ),
                    percentage >= 0.0f ?
                        "+ " + percentage.ToString( CultureInfo.CurrentCulture ) :
                        "- " + (-percentage).ToString( CultureInfo.CurrentCulture )
                ),
                new Vector2( area.X + 3.0f, area.Y ),
                new Vector2( area.X + tooltipSize.X - 2.0f, area.Y ),
                drawContext
            );
        }

        /// <summary>
        /// Draws the 'Ranged Piercing Chance' tooltip.
        /// </summary>
        /// <param name="tooltip">
        /// The tooltip that is active.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawPiercingChanceTooltip( Tooltip tooltip, ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;

            // Measure
            Point2 position = this.Owner.MousePosition;
            Point2 tooltipSize = new Point2( 50, this.fontVerySmallText.LineSpacing + 2 );
            Rectangle area = new Rectangle( position.X, position.Y, tooltipSize.X, tooltipSize.Y );

            position.Y += 2;
            Vector2 center = new Vector2( position.X + tooltipSize.X / 2.0f + 1, position.Y );

            // Background            
            DrawTooltipBackground( area, zeldaDrawContext );

            // Text
            var container = this.Player.Statable;

            float percentage = (float)Math.Round( (double)container.ChanceTo.Pierce, 1 );

            this.fontVerySmallText.Draw(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "{0}%",
                     percentage.ToString( CultureInfo.CurrentCulture )
                ),
                center,
                TextAlign.Center,
                Xna.Color.White,
                0.6f,
                drawContext
            );
        }

        /// <summary>
        /// Draws the 'Spell Power' tooltip.
        /// </summary>
        /// <param name="tooltip">
        /// The tooltip that is active.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawSpellPowerTooltip( Tooltip tooltip, ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;

            // Measure
            Point2 position = this.Owner.MousePosition;
            Point2 tooltipSize = new Point2( 110, 85 );
            Rectangle area = new Rectangle( position.X, position.Y, tooltipSize.X, tooltipSize.Y );

            position.Y += 2;

            // Background            
            DrawTooltipBackground( area, zeldaDrawContext );

            // Resistances
            this.DrawSpellPower(
                ElementalSchool.Fire,
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );

            position.Y += this.fontVerySmallText.LineSpacing + 2;

            this.DrawSpellPower(
                ElementalSchool.Water,
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );

            position.Y += this.fontVerySmallText.LineSpacing + 2;

            this.DrawSpellPower(
                ElementalSchool.Light,
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );

            position.Y += this.fontVerySmallText.LineSpacing + 2;

            this.DrawSpellPower(
                ElementalSchool.Shadow,
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );

            position.Y += this.fontVerySmallText.LineSpacing + 2;

            this.DrawSpellPower(
                ElementalSchool.Nature,
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );

            position.Y += this.fontVerySmallText.LineSpacing + 2;

            this.fontVerySmallText.Draw(
               Resources.Penetration + ":",
               new Vector2( position.X + 3.0f, position.Y ),
               Xna.Color.White,
               0.6f,
               drawContext
           );

            var container = this.Player.Statable.ChanceToBe;

            this.fontVerySmallText.Draw(
                Math.Round( container.Resisted, 2 ).ToString( CultureInfo.CurrentCulture ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                TextAlign.Right,
                Xna.Color.White,
                0.6f,
                drawContext
            );
        }

        /// <summary>
        /// Draws a sub part of the Spell Power Tooltip, detailing the restistance of the specified ElementalSchool.
        /// </summary>
        /// <param name="element">
        /// The resistance type to draw.
        /// </param>
        /// <param name="left">
        /// The position of the header text on the left.
        /// </param>
        /// <param name="right">
        /// The position of the value text on the right.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawSpellPower( ElementalSchool element, Vector2 left, Vector2 right, ISpriteDrawContext drawContext )
        {
            var container = this.Player.Statable.SpellPower;
            IntegerRange range = container.GetDamageRange( element );

            this.DrawText(
                LocalizedEnums.Get( element ),
                string.Format(
                    CultureInfo.CurrentCulture,
                    "{0} - {1}",
                    range.Minimum.ToString( CultureInfo.CurrentCulture ),
                    range.Maximum.ToString( CultureInfo.CurrentCulture )
                ),
                left,
                right,
                drawContext
            );
        }

        /// <summary>
        /// Draws the 'Statistics' tooltip.
        /// </summary>
        /// <param name="tooltip">
        /// The tooltip that is active.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawStatisticsTooltip( Tooltip tooltip, ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;

            // Measure
            string killCountStr = Player.Statistics.KillCount.ToString();
            int killCountStrWidth = (int)fontVerySmallText.MeasureStringWidth( killCountStr );

            Point2 position = this.Owner.MousePosition;
            Point2 tooltipSize = new Point2( 80 + killCountStrWidth, (3 * this.fontVerySmallText.LineSpacing) + 6 );
            Rectangle area = CreateTooltipArea( zeldaDrawContext, position, tooltipSize );

            // Background            
            DrawTooltipBackground( area, zeldaDrawContext );
            area.Y += 2;

            // Kill Count
            this.DrawText(
                Resources.KillCount,
                killCountStr,
                new Vector2( area.X + 3.0f, area.Y ),
                new Vector2( area.X + tooltipSize.X - 2.0f, area.Y ),
                drawContext
            );

            area.Y += this.fontVerySmallText.LineSpacing + 2;

            // Death Count
            this.DrawText(
                Resources.DeathCount,
                Player.Statistics.DeathCount.ToString(),
                new Vector2( area.X + 3.0f, area.Y ),
                new Vector2( area.X + tooltipSize.X - 2.0f, area.Y ),
                Player.Statistics.DeathCount > 0 ? UIColors.NegativeLight : Xna.Color.White,
                drawContext
            );

            area.Y += this.fontVerySmallText.LineSpacing + 2;

            // Game Time
            double totalHours = Player.Statistics.GameTime.TotalHours;
            string gameTime = totalHours < 1.0 ? (Math.Round( totalHours * 60.0, 1 ) + "m") : (Math.Round( totalHours, 1 ) + "h");

            DrawText(
                Resources.GameTime,
                gameTime,
                new Vector2( area.X + 3.0f, area.Y ),
                new Vector2( area.X + tooltipSize.X - 2.0f, area.Y ),
                drawContext
            );
        }

        private static Rectangle CreateTooltipArea( ZeldaDrawContext zeldaDrawContext, Point2 position, Point2 tooltipSize )
        {
            Rectangle area = new Rectangle( position.X, position.Y, tooltipSize.X, tooltipSize.Y );

            // Ensure that the area (including the border) is inside the view window
            if( (area.Right + 1) > zeldaDrawContext.Camera.ViewSize.X )
            {
                area.X = zeldaDrawContext.Camera.ViewSize.X - area.Width - 1;
            }

            return area;
        }


        /// <summary>
        /// Draws the 'Resistance' tooltip.
        /// </summary>
        /// <param name="tooltip">
        /// The tooltip that is active.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawResistanceTooltip( Tooltip tooltip, ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;

            // Measure
            Point2 position = this.Owner.MousePosition;
            Point2 tooltipSize = new Point2( 80, 70 );
            Rectangle area = new Rectangle( position.X, position.Y, tooltipSize.X, tooltipSize.Y );

            position.Y += 2;

            // Background            
            DrawTooltipBackground( area, zeldaDrawContext );

            // Resistances
            this.DrawResistance(
                ElementalSchool.Fire,
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );

            position.Y += this.fontVerySmallText.LineSpacing + 2;

            this.DrawResistance(
                ElementalSchool.Water,
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );

            position.Y += this.fontVerySmallText.LineSpacing + 2;

            this.DrawResistance(
                ElementalSchool.Light,
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );

            position.Y += this.fontVerySmallText.LineSpacing + 2;

            this.DrawResistance(
                ElementalSchool.Shadow,
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );

            position.Y += this.fontVerySmallText.LineSpacing + 2;

            this.DrawResistance(
                ElementalSchool.Nature,
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );

            position.Y += this.fontVerySmallText.LineSpacing + 2;
        }

        /// <summary>
        /// Draws a sub part of the Resistance Tooltip, detailing the restistance of the specified ElementalSchool.
        /// </summary>
        /// <param name="element">
        /// The resistance type to draw.
        /// </param>
        /// <param name="left">
        /// The position of the header text on the left.
        /// </param>
        /// <param name="right">
        /// The position of the value text on the right.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawResistance( ElementalSchool element, Vector2 left, Vector2 right, ISpriteDrawContext drawContext )
        {
            this.DrawTextAsPercent(
                LocalizedEnums.Get( element ),
                this.Player.Statable.Resistances.Get( element ),
                left,
                right,
                drawContext
            );
        }

        /// <summary>
        /// Draws the 'Chance to' tooltip.
        /// </summary>
        /// <param name="tooltip">
        /// The tooltip that is active.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawChanceToTooltip( Tooltip tooltip, ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;
            var statable = this.Player.Statable;
            var container = statable.ChanceTo;

            // Measure
            Point2 position = this.Owner.MousePosition;
            Point2 tooltipSize = new Point2( 100, 5 * (this.fontVerySmallText.LineSpacing + 2) );
            Rectangle area = new Rectangle( position.X, position.Y, tooltipSize.X, tooltipSize.Y );

            position.Y += 2;

            // Background            
            DrawTooltipBackground( area, zeldaDrawContext );

            // Crit
            this.DrawTextAsPercent(
                Resources.Crit,
                container.Crit,
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );

            position.Y += this.fontVerySmallText.LineSpacing + 2;

            // Hit
            this.DrawTextAsPercent(
                Resources.Hit,
                100.0f - container.Miss,
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );

            position.Y += this.fontVerySmallText.LineSpacing + 2;

            // Dodge
            this.DrawTextAsPercent(
                Resources.Dodge,
                container.Dodge,
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );

            position.Y += this.fontVerySmallText.LineSpacing + 2;

            // Parry
            this.DrawTextAsPercent(
                Resources.Parry,
                container.Parry,
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );

            position.Y += this.fontVerySmallText.LineSpacing + 2;

            // Crit Heal
            this.DrawTextAsPercent(
                Resources.CritHeal,
                container.CritHeal,
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );

            position.Y += this.fontVerySmallText.LineSpacing + 2;
        }

        /// <summary>
        /// Draws the 'Chance to Be' tooltip.
        /// </summary>
        /// <param name="tooltip">
        /// The tooltip that is active.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawChanceToBeTooltip( Tooltip tooltip, ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;
            var container = this.Player.Statable.ChanceToBe;

            // Measure
            Point2 position = this.Owner.MousePosition;
            Point2 tooltipSize = new Point2( 100, 42 );
            Rectangle area = new Rectangle( position.X, position.Y, tooltipSize.X, tooltipSize.Y );

            position.Y += 2;

            // Background            
            DrawTooltipBackground( area, zeldaDrawContext );

            // To Be
            this.DrawTextAsPercent(
                Resources.Crit,
                container.Crit,
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );

            position.Y += this.fontVerySmallText.LineSpacing + 2;

            this.DrawTextAsPercent(
                Resources.Hit,
                container.Hit,
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );

            position.Y += this.fontVerySmallText.LineSpacing + 2;

            this.DrawTextAsPercent(
                Resources.Resisted,
                container.Resisted,
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );

            position.Y += this.fontVerySmallText.LineSpacing + 2;
        }

        /// <summary>
        /// Draws the 'Block' tooltip.
        /// </summary>
        /// <param name="tooltip">
        /// The tooltip that is active.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawBlockTooltip( Tooltip tooltip, ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;
            var container = this.Player.Statable;

            // Measure
            Point2 position = this.Owner.MousePosition;
            Point2 tooltipSize = new Point2( 105, (this.fontVerySmallText.LineSpacing * (container.CanBlock ? 3 : 2)) + (container.CanBlock ? 6 : 4) );
            Rectangle area = new Rectangle( position.X, position.Y, tooltipSize.X, tooltipSize.Y );

            position.Y += 2;

            // Background            
            DrawTooltipBackground( area, zeldaDrawContext );

            // Chance
            if( container.CanBlock )
            {
                this.DrawTextAsPercent(
                    Resources.BlockChance,
                    container.ChanceToBlock,
                    new Vector2( position.X + 3.0f, position.Y ),
                    new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                    drawContext
                );

                position.Y += this.fontVerySmallText.LineSpacing + 2;

                // Crit Block
                this.DrawTextAsPercent(
                    Resources.CritBlock,
                    container.ChanceTo.CritBlock,
                    new Vector2( position.X + 3.0f, position.Y ),
                    new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                    Player.Statable.CanBlock ? Xna.Color.White : UIColors.NegativeLight,
                    drawContext
                );
            }
            else
            {
                this.DrawText(
                    Resources.BlockChance,
                    "no block",
                    new Vector2( position.X + 3.0f, position.Y ),
                    new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                    UIColors.NegativeLight,
                    drawContext
                );
            }

            position.Y += this.fontVerySmallText.LineSpacing + 2;

            // Value
            this.DrawText(
                Resources.BlockValue,
                container.BlockValue.ToString(),
                new Vector2( position.X + 3.0f, position.Y ),
                new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                drawContext
            );
        }

        /// <summary>
        /// Draws the 'Block' tooltip.
        /// </summary>
        /// <param name="tooltip">
        /// The tooltip that is active.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawReputationTooltip( Tooltip tooltip, ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;
            ICollection<FactionState> container = this.Player.FactionStates.KnownStates;
            bool empty = container.Count == 0;

            // Measure
            Point2 position = this.Owner.MousePosition;
            Point2 tooltipSize = new Point2( empty ? 105 : 150, (this.fontVerySmallText.LineSpacing * (empty ? 1 : container.Count)) + 3 );
            Rectangle area = new Rectangle( position.X, position.Y, tooltipSize.X, tooltipSize.Y );

            position.Y += 2;

            // Background            
            DrawTooltipBackground( area, zeldaDrawContext );

            if( empty )
            {
                this.DrawText(
                    "No one knows you.. :(",
                    new Vector2( position.X + 3.0f, position.Y ),
                    new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                    UIColors.NegativeLight,
                    drawContext
                );
            }
            else
            {
                foreach( FactionState state in container )
                {
                    int start, end;
                    Faction.GetReputationValues( state.ReputationLevel, out start, out end );

                    this.DrawText(
                        state.Faction.LocalizedName,
                        string.Format( "{0} ({1}/{2})", LocalizedEnums.Get( state.ReputationLevel ), state.Reputation, end ),
                        new Vector2( position.X + 3.0f, position.Y ),
                        new Vector2( position.X + tooltipSize.X - 2.0f, position.Y ),
                        drawContext
                    );
                    position.Y += this.fontVerySmallText.LineSpacing + 2;
                }
            }
        }

        /// <summary>
        /// Draws a labeled value plus a '%' sign at the specified positions.
        /// </summary>
        /// <param name="label">
        /// The label of the value. E.g.: "Armor"
        /// </param>
        /// <param name="value">
        /// The value that will be alligned to the right. E.g.: "1000.0"
        /// </param>
        /// <param name="left">
        /// The position of the label.
        /// </param>
        /// <param name="right">
        /// The position of the value.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawTextAsPercent( string label, double value, Vector2 left, Vector2 right, ISpriteDrawContext drawContext )
        {
            value = Math.Round( value, 2 );
            string valueString = value.ToString( CultureInfo.CurrentCulture ) + "%";

            this.DrawText( label, valueString, left, right, drawContext );
        }

        /// <summary>
        /// Draws a labeled value plus a '%' sign at the specified positions.
        /// </summary>
        /// <param name="label">
        /// The label of the value. E.g.: "Armor"
        /// </param>
        /// <param name="value">
        /// The value that will be alligned to the right. E.g.: "1000.0"
        /// </param>
        /// <param name="left">
        /// The position of the label.
        /// </param>
        /// <param name="right">
        /// The position of the value.
        /// </param>
        /// <param name="valueColor">
        /// The color of the value text.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawTextAsPercent( string label, double value, Vector2 left, Vector2 right, Xna.Color valueColor, ISpriteDrawContext drawContext )
        {
            value = Math.Round( value, 2 );
            string valueString = value.ToString( CultureInfo.CurrentCulture ) + "%";

            this.DrawText( label, valueString, left, right, valueColor, drawContext );
        }

        /// <summary>
        /// Draws a labeled text at the specified positions.
        /// </summary>
        /// <param name="label">
        /// The label of the value. E.g.: "Armor"
        /// </param>
        /// <param name="value">
        /// The value that will be alligned to the right. E.g.: "1000"
        /// </param>
        /// <param name="left">
        /// The position of the label.
        /// </param>
        /// <param name="right">
        /// The position of the value.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawText( string label, string value, Vector2 left, Vector2 right, ISpriteDrawContext drawContext )
        {
            DrawText( label, value, left, right, Xna.Color.White, drawContext );
        }

        /// <summary>
        /// Draws a labeled text at the specified positions.
        /// </summary>
        /// <param name="label">
        /// The label of the value. E.g.: "Armor"
        /// </param>
        /// <param name="value">
        /// The value that will be alligned to the right. E.g.: "1000"
        /// </param>
        /// <param name="left">
        /// The position of the label.
        /// </param>
        /// <param name="right">
        /// The position of the value.
        /// </param>
        /// <param name="valueColor">
        /// The color the value text will be tinted in.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawText( string label, string value, Vector2 left, Vector2 right, Xna.Color valueColor, ISpriteDrawContext drawContext )
        {
            this.fontVerySmallText.Draw(
               label + ":",
               left,
               Xna.Color.White,
               0.6f,
               drawContext
            );

            this.fontVerySmallText.Draw(
                value,
                right,
                TextAlign.Right,
                valueColor,
                0.6f,
                drawContext
            );
        }

        /// <summary>
        /// Draws an unlabeled text at the specified positions.
        /// </summary>
        /// <param name="value">
        /// The value that will be alligned to the right. E.g.: "1000"
        /// </param>
        /// <param name="left">
        /// The position of the label.
        /// </param>
        /// <param name="right">
        /// The position of the value.
        /// </param>
        /// <param name="valueColor">
        /// The color the value text will be tinted in.
        /// </param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawText( string value, Vector2 left, Vector2 right, Xna.Color valueColor, ISpriteDrawContext drawContext )
        {
            this.fontVerySmallText.Draw(
                value,
                new Vector2( (left.X + right.X) / 2.0f, right.Y ),
                TextAlign.Center,
                valueColor,
                0.6f,
                drawContext
            );
        }

        /// <summary>
        /// Draws the background of an InlineTooltip.
        /// </summary>
        /// <param name="area">
        /// the area the tooltip is taking up.
        /// </param>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        private static void DrawTooltipBackground( Rectangle area, ZeldaDrawContext drawContext )
        {
            drawContext.Batch.DrawRect(
                area,
                Xna.Color.Black,
                0.5f
            );

            drawContext.Batch.DrawLineRect(
                area,
                Xna.Color.White,
                1,
                0.55f
            );
        }

        /// <summary>
        /// Converts the specified value multiplier into a percentage string.
        /// </summary>
        /// <param name="modifier">
        /// The input modifier.
        /// </param>
        /// <param name="digits">
        /// The number of significant digits to keep.
        /// </param>
        /// <returns>
        /// The converted output value.
        /// </returns>
        private static string ConvertModifierToString( float modifier, int digits = 2 )
        {
            return ConvertToPercentage( modifier, digits ).ToString( CultureInfo.CurrentCulture );
        }

        /// <summary>
        /// Converts the specified value multiplier into a percentage.
        /// </summary>
        /// <param name="modifier">
        /// The input modifier.
        /// </param>
        /// <param name="digits">
        /// The number of significant digits to keep.
        /// </param>
        /// <returns>
        /// The converted output value.
        /// </returns>
        private static float ConvertToPercentage( float modifier, int digits )
        {
            float increaseInPercent = (modifier * 100.0f) - 100.0f;
            return (float)Math.Round( increaseInPercent, digits );
        }

        /// <summary>
        /// Called when this CharacterDetailsWindow is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        {
            if( this.Player == null )
                return;

            this.DrawBackground( drawContext );
            this.DrawBasicStatistics( drawContext );
        }

        /// <summary>
        /// Called when the IsEnabled state of this CharacterDetailsWindow has changed.
        /// </summary>
        protected override void OnIsEnabledChanged()
        {
            foreach( var tooltip in this.inlineTooltips )
            {
                tooltip.IsEnabled = this.IsEnabled;
            }
        }

        /// <summary>
        /// Called when the IsVisible state of this CharacterDetailsWindow has changed.
        /// </summary>
        protected override void OnIsVisibleChanged()
        {
            foreach( var tooltip in this.inlineTooltips )
            {
                tooltip.IsVisible = this.IsVisible;
            }
        }

        /// <summary>
        /// Adds the child elements of this IngameWindow to the specified UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface object.
        /// </param>
        protected override void AddChildElementsTo( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.AddElements( this.inlineTooltips );
        }

        /// <summary>
        /// Removes the child elements of this IngameWindow from the specified UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface object.
        /// </param>
        protected override void RemoveChildElementsFrom( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.RemoveElements( this.inlineTooltips );
        }

        /// <summary>
        /// The InlineTooltip this CharacterDetailsWindow contains.
        /// </summary>
        private readonly List<InlineTooltip> inlineTooltips = new List<InlineTooltip>();

        /// <summary>
        /// Represents a Tooltip that renders a header text inline and when hoverer over
        /// draws the actual tooltip text.
        /// </summary>
        private sealed class InlineTooltip : Tooltip
        {
            /// <summary>
            /// Initializes a new instance of the InlineTooltip class.
            /// </summary>
            /// <param name="header">
            /// The text that represents the content of the tooltip.
            /// </param>
            /// <param name="font">
            /// The font with which the header text should be drawn.
            /// </param>
            /// <param name="drawAction">
            /// The action to execute when drawing the actual tooltip.
            /// </param>
            public InlineTooltip( string header, IFont font, Action<Tooltip, ISpriteDrawContext> drawAction )
                : base( new LambdaTooltipDrawElement( drawAction ) )
            {
                this.header = header;
                this.font = font;
                this.Size = font.MeasureString( header ) + new Vector2( 6.0f, 0.0f );
                this.FloorNumber = 6;
            }

            /// <summary>
            /// Called when this InlineTooltip is drawing itself.
            /// </summary>
            /// <param name="drawContext">
            /// The current ISpriteDrawContext.
            /// </param>
            protected override void OnDraw( ISpriteDrawContext drawContext )
            {
                var zeldaDrawContext = (ZeldaDrawContext)drawContext;

                if( !this.IsMouseOver )
                {
                    drawContext.Batch.DrawLineRect(
                        (Rectangle)this.ClientArea,
                        Xna.Color.White,
                        1,
                        0.05f
                    );
                }

                this.font.Draw(
                    this.header,
                    this.Position + new Vector2( this.Width / 2.0f + 1, 1.0f ),
                    TextAlign.Center,
                    Xna.Color.White,
                    0.1f,
                    drawContext
                );

                base.OnDraw( drawContext );
            }

            /// <summary>
            /// The text that represents the content of the tooltip.
            /// </summary>
            private readonly string header;

            /// <summary>
            /// The font with which the header text is drawn.
            /// </summary>
            private readonly IFont font;
        }
    }
}
