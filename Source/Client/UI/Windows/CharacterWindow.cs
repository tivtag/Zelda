// <copyright file="CharacterWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.CharacterWindow class.
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
    using Atom.Xna.UI;
    using Atom.Xna.UI.Tooltips;
    using Zelda.Status;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Allows the player to see basic character status information and invest status points.
    /// </summary>
    internal class CharacterWindow : BaseCharacterWindow
    {
        /// <summary>
        /// The coordinate constants used by the CharacterWindow.
        /// </summary>
        protected readonly int CenterStatNamesX, CenterStatValuesX;

        /// <summary>
        /// The color constants used by the CharacterWindow.
        /// </summary>
        private static readonly Xna.Color ColorStatName = Xna.Color.Wheat, ColorStatValue = Xna.Color.White;

        /// <summary>
        /// Gets or sets a value indicating whether the value tooltips are shown. E.g. "12 + 3"
        /// </summary>
        protected bool ShowsStatValueTooltips { get; set; }
        
        protected int StatUpCost
        {
            // get { return statUpCost; } // Unused
            set { statUpCost = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterWindow"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related servicess.
        /// </param>
        internal CharacterWindow( IZeldaServiceProvider serviceProvider )
            : base( serviceProvider, 0 /* offsetY */ )
        {
            this.toolTipDrawElement          = new ToolTipDrawElement( serviceProvider.ViewSize );
            this.statValueToolTipDrawElement = new StatValueToolTipDrawElement( this );

            CenterStatNamesX = (int)leftSpritePosition.X + 55;
            CenterStatValuesX = CenterStatNamesX + 70;
            ShowsStatValueTooltips = true;

            this.SetupStatUpButtons();
            this.SetupStatTooltips();
            this.SetupStatValueTooltips();
        }

        /// <summary>
        /// Setups the StatUp buttons, called once on creation of the window.
        /// </summary>
        private void SetupStatUpButtons()
        {
            Vector2 buttonPosition = new Vector2( CenterStatValuesX + 30, LineStartY + 2 );

            foreach( StatUpButton button in this.statUpButtons )
            {
                button.Text           = "+";
                button.Font           = fontText;
                button.Position       = buttonPosition;
                button.ColorSelected  = Xna.Color.OrangeRed;
                button.FloorNumber    = this.FloorNumber;      
                button.Clicked       += this.OnStatUpButtonClicked;
                button.MouseEntering += this.OnStatUpButtonMouseEntering;
                button.MouseLeaving  += this.OnStatUpButtonMouseLeaving;

                buttonPosition.Y += LineSpacing;
            }
        }

        /// <summary>
        /// Setups and creates the stat tooltips.
        /// </summary>
        private void SetupStatTooltips()
        {
            var blockSplitter = new Zelda.UI.TextBlockSplitter( fontText, 260 );

            int i = 0;
            const float Delta = 25.0f;
            float baseY = LineStartY - Delta - 2;

            this.statTooltips[i++] = this.CreateStatTooltip( Resources.Tooltip_Strength, baseY + Delta * i, blockSplitter );
            this.statTooltips[i++] = this.CreateStatTooltip( Resources.Tooltip_Dexterity, baseY + Delta * i, blockSplitter );
            this.statTooltips[i++] = this.CreateStatTooltip( Resources.Tooltip_Vitality, baseY + Delta * i, blockSplitter );
            this.statTooltips[i++] = this.CreateStatTooltip( Resources.Tooltip_Agility, baseY + Delta * i, blockSplitter );
            this.statTooltips[i++] = this.CreateStatTooltip( Resources.Tooltip_Intelligence, baseY + Delta * i, blockSplitter );
            this.statTooltips[i++] = this.CreateStatTooltip( Resources.Tooltip_Luck, baseY + Delta * i, blockSplitter );
        }

        /// <summary>
        /// Creates a new TextTooltip that is shown when the player moves over one of the stat names; Strength, Dexterity, ..
        /// </summary>
        /// <param name="tooltipText">
        /// The text to show when the player moves into the tooltip area.
        /// </param>
        /// <param name="positionY">
        /// The position of the tooltip area on the y-axis.
        /// </param>
        /// <param name="blockSplitter">
        /// The ITextBlockSplitter that should be used to split the given tooltipText into multiple lines.
        /// </param>
        /// <returns>
        /// The newly created TextTooltip.
        /// </returns>
        private TextTooltip CreateStatTooltip( string tooltipText, float positionY, ITextBlockSplitter blockSplitter )
        {
            return new TextTooltip( this.toolTipDrawElement )  {
                Text = new Text( this.fontText, tooltipText, TextAlign.Center, Xna.Color.White, blockSplitter ) {
                    LayerDepth = 0.1f
                },

                FloorNumber = 3,
                Position    = new Vector2( this.leftSpritePosition.X + 15, positionY ),
                Size        = new Vector2( 85.0f, 16.0f )
            };
        }

        /// <summary>
        /// Setups and creates the stat tooltips.
        /// </summary>
        private void SetupStatValueTooltips()
        {
            var blockSplitter = new Zelda.UI.TextBlockSplitter( fontText, 260 );

            int i = 0;
            const float Delta = 25.0f;
            float baseY = LineStartY - Delta - 2;

            this.statValueTooltips[i++] = this.CreateStatValueTooltip( Stat.Strength, baseY + Delta * i, blockSplitter );
            this.statValueTooltips[i++] = this.CreateStatValueTooltip( Stat.Dexterity, baseY + Delta * i, blockSplitter );
            this.statValueTooltips[i++] = this.CreateStatValueTooltip( Stat.Vitality, baseY + Delta * i, blockSplitter );
            this.statValueTooltips[i++] = this.CreateStatValueTooltip( Stat.Agility, baseY + Delta * i, blockSplitter );
            this.statValueTooltips[i++] = this.CreateStatValueTooltip( Stat.Intelligence, baseY + Delta * i, blockSplitter );
            this.statValueTooltips[i++] = this.CreateStatValueTooltip( Stat.Luck, baseY + Delta * i, blockSplitter ); 
        }

        /// <summary>
        /// Creates a new TextTooltip that is shown when the player moves over one of the stat values.
        /// </summary>
        /// <param name="stat">
        /// The stat value for which extra information should be displayed when the player moves his mouse
        /// over the tooltip area.
        /// </param>
        /// <param name="positionY">
        /// The position of the tooltip area on the y-axis.
        /// </param>
        /// <param name="blockSplitter">
        /// The ITextBlockSplitter that should be used to split the given tooltipText into multiple lines.
        /// </param>
        /// <returns>
        /// The newly created TextTooltip.
        /// </returns>
        private TextTooltip CreateStatValueTooltip( Stat stat, float positionY, TextBlockSplitter blockSplitter )
        {
            return new TextTooltip( this.statValueToolTipDrawElement ) {
                Text = new Text( fontText, TextAlign.Center, Xna.Color.White, blockSplitter ) {
                    LayerDepth = 0.1f
                },

                FloorNumber = 3,
                Position    = new Vector2( this.leftSpritePosition.X + 105, positionY ),
                Size        = new Vector2( 40.0f, 15.0f ),
                Tag         = stat,
                IsVisible = false,
                IsEnabled = false
            };
        }
        
        /// <summary>
        /// Called when this <see cref="CharacterWindow"/> is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        {
            if( this.Player == null )
            {
                return;
            }

            this.DrawBackground( drawContext );
            this.DrawBasicStatistics( drawContext );
            this.DrawStatIndicators( drawContext );
            this.DrawDetailedInfos( drawContext );
        }

        /// <summary>
        /// Draws the detailed character information.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext
        /// </param>
        private void DrawDetailedInfos( ISpriteDrawContext drawContext )
        {
            ExtendedStatable statable = this.Player.Statable;
            CultureInfo culture  = CultureInfo.CurrentCulture;
            float y = LineStartY - 6;
            int baseX = (int)this.rightSpritePosition.X - 14;

            // Melee Damage:
            string stringMeleeDamage = GetMeleeDamageInfoString( statable );

            DrawText( 
                fontVerySmallText,
                stringMeleeDamage,
                new Vector2( baseX - (int)fontVerySmallText.MeasureString( stringMeleeDamage ).X, y ),
                Xna.Color.White,
                drawContext
            );

            y += fontVerySmallText.LineSpacing;

            // Ranged Damage:
            string stringRangedDamage = GetRangedDamageInfoString( statable );

            DrawText( 
                fontVerySmallText,
                stringRangedDamage,
                new Vector2( baseX- (int)fontVerySmallText.MeasureString( stringRangedDamage ).X, y ),
                Xna.Color.White,
                drawContext 
            );

            y += fontVerySmallText.LineSpacing;

            // Draw Magic Damage Info:      
            string strMagicDmg = GetSpellDamageInfoString( statable );

            DrawText(
                fontVerySmallText,
                strMagicDmg,
                new Vector2( baseX - (int)fontVerySmallText.MeasureString( strMagicDmg ).X, y ),
                Xna.Color.White,
                drawContext
            );

            y += fontVerySmallText.LineSpacing + 5;

            // Draw Crit Info:
            string strCrit = string.Format( 
                culture, 
                Resources.CritX,
                System.Math.Round( statable.ChanceTo.Crit, 2 ).ToString( culture )
            );

            DrawText( 
                fontVerySmallText,
                strCrit,
                new Vector2( baseX - (int)fontVerySmallText.MeasureString( strCrit ).X, y ),
                Xna.Color.White,
                drawContext 
            );
            y += fontVerySmallText.LineSpacing;

            // Draw Hit Info:
            string strHit = string.Format( 
                culture,
                Resources.HitX,
                System.Math.Round( 100.0f - statable.ChanceTo.Miss, 2 ).ToString( culture ) 
            );

            DrawText( 
                fontVerySmallText, 
                strHit,
                new Vector2( baseX - (int)fontVerySmallText.MeasureString( strHit ).X, y ),
                Xna.Color.White,
                drawContext
            );
            y += fontVerySmallText.LineSpacing;

            // Draw Dodge Info:
            string strDodge = string.Format(
                culture,
                Resources.DodgeX,
                System.Math.Round( statable.ChanceTo.Dodge, 2 ).ToString( culture )
            );

            DrawText(
                fontVerySmallText,
                strDodge,
                new Vector2( baseX - (int)fontVerySmallText.MeasureString( strDodge ).X, y ),
                Xna.Color.White,
                drawContext
            );
            y += fontVerySmallText.LineSpacing;

            // Draw Parry Info:
            string strParry = string.Format(
                culture,
                Resources.ParryX,
                System.Math.Round( statable.ChanceTo.Parry, 2 ).ToString( culture )
            );

            DrawText(
                fontVerySmallText,
                strParry,
                new Vector2( baseX - (int)fontVerySmallText.MeasureString( strParry ).X, y ),
                Xna.Color.White,
                drawContext
            );

            y += fontVerySmallText.LineSpacing;
            y += fontVerySmallText.LineSpacing - 7;

            // Draw Armor Info:
            string strArmor = string.Format(
                culture,
                Resources.ArmorXMitigationY,
                statable.Armor.ToString( culture ),
                System.Math.Round( StatusCalc.GetMitigationFromArmor( statable.Armor, statable.Level ), 1 ).ToString( culture )
            );

            DrawText( 
                fontVerySmallText,
                strArmor,
                new Vector2( baseX - (int)fontVerySmallText.MeasureString( strArmor ).X, y ),
                Xna.Color.White,
                drawContext
            );

            y += fontVerySmallText.LineSpacing;

            // Draw Block Info:
            if( statable.CanBlock )
            {
                string strBlock = string.Format(
                    culture,
                    Resources.InfoBlocking,
                    System.Math.Round( statable.ChanceToBlock, 2 ).ToString( culture ),
                    statable.BlockValue.ToString( culture )
                );

                DrawText(
                    fontVerySmallText,
                    strBlock,
                    new Vector2( baseX - (int)fontVerySmallText.MeasureString( strBlock ).X, y ),
                    Xna.Color.White,
                    drawContext
                );
            }

            y += fontVerySmallText.LineSpacing;

            // Draw MF Info:
            string strMF = string.Format( 
                culture,
                Resources.MagicFindX,
                System.Math.Round( (statable.MagicFind * 100.0f) - 100.0f ).ToString( culture ) 
            );

            DrawText( 
                fontVerySmallText, 
                strMF,
                new Vector2( baseX - (int)fontVerySmallText.MeasureString( strMF ).X, y ),
                Xna.Color.White,
                drawContext
            );

            y += fontVerySmallText.LineSpacing;

            // Draw Life/Mana Regen Info:
            string strRegen = string.Format( 
                culture, 
                Resources.LifeXManaYRegen,
                statable.LifeRegeneration.ToString( culture ),
                statable.ManaRegeneration.ToString( culture )
            );

            DrawText( 
                fontVerySmallText,
                strRegen,
                new Vector2( baseX - (int)fontVerySmallText.MeasureString( strRegen ).X, y ),
                Xna.Color.White,
                drawContext
            );

            y += fontVerySmallText.LineSpacing + 1;

            // Draw Free Stat Count:
            if( statable.FreeStatPoints > 0 || statUpCost < 0 )
            {
                string strFreeStats;

                if( this.statUpCost > 0 )
                {
                    strFreeStats = string.Format(
                        culture,
                        "[{0} -{1} {2}]",
                        statable.FreeStatPoints.ToString( culture ),
                        this.statUpCost.ToString(culture),
                        statable.FreeStatPoints == 1 ? Resources.FreeStatPoint : Resources.FreeStatPoints
                    );
                }
                else if( this.statUpCost < 0 )
                {
                    strFreeStats = string.Format(
                        culture,
                        "[{0} +{1} {2}]",
                        statable.FreeStatPoints.ToString( culture ),
                        (-this.statUpCost).ToString( culture ),
                        statable.FreeStatPoints == 1 ? Resources.FreeStatPoint : Resources.FreeStatPoints
                    );
                }
                else
                {
                    strFreeStats = string.Format(
                        culture,
                        "[{0} {1}]",
                        statable.FreeStatPoints.ToString( culture ),
                        statable.FreeStatPoints == 1 ? Resources.FreeStatPoint : Resources.FreeStatPoints
                    );
                }

                var drawPosition = new Vector2(
                    baseX + 1 - (int)fontVerySmallText.MeasureString( strFreeStats ).X,
                    y
                );

                DrawText( 
                    fontVerySmallText,
                    strFreeStats,
                    drawPosition,
                    Xna.Color.White,
                    drawContext
                );
            }
        }

        /// <summary>
        /// Gets the ranged damage information for the given ExtendedStatable.
        /// </summary>
        /// <param name="statable">
        /// The ExtendedStatable to inspect.
        /// </param>
        /// <returns>
        /// An informative string.
        /// </returns>
        private static string GetRangedDamageInfoString( ExtendedStatable statable )
        {
            int minimum = statable.DamageRangedMin;
            minimum = statable.DamageDone.WithSource.ApplyFixedRanged( minimum );
            minimum = statable.DamageDone.WithSource.ApplyRanged( minimum );

            int maximum = statable.DamageRangedMax;
            maximum = statable.DamageDone.WithSource.ApplyFixedRanged( maximum );
            maximum = statable.DamageDone.WithSource.ApplyRanged( maximum );

            CultureInfo culture = CultureInfo.CurrentCulture;
            return string.Format(
                culture,
                Resources.RangedDamageXToYAtSpeedZ,
                minimum.ToString( culture ),
                maximum.ToString( culture ),
                GetRangedAttackSpeedString( statable )
            );
        }

        /// <summary>
        /// Gets the ranged attack speed information for the given ExtendedStatable.
        /// </summary>
        /// <param name="statable">
        /// The ExtendedStatable to inspect.
        /// </param>
        /// <returns>
        /// An informative string.
        /// </returns>
        private static string GetRangedAttackSpeedString( ExtendedStatable statable )
        {
            double rangedAttackSpeed = System.Math.Round( statable.AttackSpeedRanged, 2 );

            CultureInfo culture = CultureInfo.CurrentCulture;
            string stringRangedAttackSpeed = rangedAttackSpeed.ToString( culture );

            if( stringRangedAttackSpeed.Length == 1 )
            {
                stringRangedAttackSpeed += culture.NumberFormat.NumberDecimalSeparator + "00";
            }
            else if( stringRangedAttackSpeed.Length == 3 )
            {
                stringRangedAttackSpeed += "0";
            }

            return stringRangedAttackSpeed;
        }

        /// <summary>
        /// Gets the melee damage information for the given ExtendedStatable.
        /// </summary>
        /// <param name="statable">
        /// The ExtendedStatable to inspect.
        /// </param>
        /// <returns>
        /// An informative string.
        /// </returns>
        private static string GetMeleeDamageInfoString( ExtendedStatable statable )
        {
            int minimum = statable.DamageMeleeMin;
            minimum = statable.DamageDone.WithSource.ApplyFixedMelee( minimum );
            minimum = statable.DamageDone.WithSource.ApplyMelee( minimum );

            int maximum = statable.DamageMeleeMax;
            maximum = statable.DamageDone.WithSource.ApplyFixedMelee( maximum );
            maximum = statable.DamageDone.WithSource.ApplyMelee( maximum );

            CultureInfo culture = CultureInfo.CurrentCulture;
            return string.Format(
                culture,
                Resources.MeleeDamageXToYAtSpeedZ,
                minimum.ToString( culture ),
                maximum.ToString( culture ),
                GetMeleeAttackSpeedString( statable )
            );
        }

        /// <summary>
        /// Gets the melee attack speed information for the given ExtendedStatable.
        /// </summary>
        /// <param name="statable">
        /// The ExtendedStatable to inspect.
        /// </param>
        /// <returns>
        /// An informative string.
        /// </returns>
        private static string GetMeleeAttackSpeedString( ExtendedStatable statable )
        {
            double meleeAttackSpeed = System.Math.Round( statable.AttackSpeedMelee, 2 );

            CultureInfo culture = CultureInfo.CurrentCulture;
            string stringMeleeAttackSpeed = meleeAttackSpeed.ToString( culture );

            if( stringMeleeAttackSpeed.Length == 1 )
            {
                stringMeleeAttackSpeed += culture.NumberFormat.NumberDecimalSeparator + "00";
            }
            else if( stringMeleeAttackSpeed.Length == 3 )
            {
                stringMeleeAttackSpeed += "0";
            }

            return stringMeleeAttackSpeed;
        }

        /// <summary>
        /// Gets the spell damage information for the given ExtendedStatable.
        /// </summary>
        /// <param name="statable">
        /// The ExtendedStatable to inspect.
        /// </param>
        /// <returns>
        /// An informative string.
        /// </returns>
        private static string GetSpellDamageInfoString( ExtendedStatable statable )
        {
            IntegerRange spellPower = statable.SpellPower.GetDamageRange( ElementalSchool.All );
            int minimum = spellPower.Minimum;
            minimum = statable.DamageDone.WithSource.ApplyFixedSpell( minimum );
            minimum = statable.DamageDone.WithSource.ApplySpell( minimum );

            int maximum = spellPower.Maximum;
            maximum = statable.DamageDone.WithSource.ApplyFixedSpell( maximum );
            maximum = statable.DamageDone.WithSource.ApplySpell( maximum );

            CultureInfo culture = CultureInfo.CurrentCulture;
            return string.Format(
                culture,
                Resources.MagicDamageXToYAtCastSpeedZ,
                minimum.ToString( culture ),
                maximum.ToString( culture ),
                GetCastSpeedString( statable )
            );
        }

        /// <summary>
        /// Gets the cast speed information for the given ExtendedStatable.
        /// </summary>
        /// <param name="statable">
        /// The ExtendedStatable to inspect.
        /// </param>
        /// <returns>
        /// An informative string.
        /// </returns>
        private static string GetCastSpeedString( ExtendedStatable statable )
        {
            double castSpeed = System.Math.Round( statable.CastTimeModifier, 2 );

            CultureInfo culture = CultureInfo.CurrentCulture;
            string stringCastSpeed = castSpeed.ToString( culture );

            if( stringCastSpeed.Length == 1 )
            {
                stringCastSpeed += culture.NumberFormat.NumberDecimalSeparator + "00";
            }
            else if( stringCastSpeed.Length == 3 )
            {
                stringCastSpeed += "0";
            }

            return stringCastSpeed;
        }

        /// <summary>
        /// Draws the strings that indicate how much of each <see cref="Stat"/> the player has.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawStatIndicators( ISpriteDrawContext drawContext )
        {
            CultureInfo culture  = System.Globalization.CultureInfo.CurrentCulture;
            ExtendedStatable statable = this.Player.Statable;

            int statLineStartY = LineStartY + 2;

            // Draw Strength Indicator:
            DrawText( 
                fontText, 
                LocalizedEnums.Get( Stat.Strength ),
                new Vector2( CenterStatNamesX - (int)(fontText.MeasureString( LocalizedEnums.Get( Stat.Strength ) ).X / 2), statLineStartY ),
                ColorStatName,
                drawContext 
            );

            string strStrength = statable.Strength.ToString( culture );
            DrawText(
                fontText,
                strStrength,
                new Vector2( CenterStatValuesX - (int)(fontText.MeasureString( strStrength ).X / 2), statLineStartY ),
                ColorStatValue,
                drawContext
            );

            // Draw Dexterity Indicator:
            DrawText( 
                fontText, 
                LocalizedEnums.Get( Stat.Dexterity ),
                new Vector2( CenterStatNamesX - (int)(fontText.MeasureString( LocalizedEnums.Get( Stat.Dexterity ) ).X / 2), statLineStartY + LineSpacing ),
                ColorStatName,
                drawContext 
            );

            string strDexterity = statable.Dexterity.ToString( culture );
            DrawText( 
                fontText,
                strDexterity,
                new Vector2( CenterStatValuesX - (int)(fontText.MeasureString( strDexterity ).X / 2), statLineStartY + LineSpacing ),
                ColorStatValue,
                drawContext
            );

            // Draw Vitality Indicator:
            DrawText( 
                fontText,
                LocalizedEnums.Get( Stat.Vitality ),
                new Vector2( CenterStatNamesX - (int)(fontText.MeasureString( LocalizedEnums.Get( Stat.Vitality ) ).X / 2), statLineStartY + (2 * LineSpacing) ),
                ColorStatName,
                drawContext
            );

            string strVitality = statable.Vitality.ToString( culture );
            DrawText(
                fontText, 
                strVitality,
                new Vector2( CenterStatValuesX - (int)(fontText.MeasureString( strVitality ).X / 2), statLineStartY + (2 * LineSpacing) ),
                ColorStatValue,
                drawContext
            );

            // Draw Agility Indicator:
            DrawText( 
                fontText,
                LocalizedEnums.Get( Stat.Agility ),
                new Vector2( CenterStatNamesX - (int)(fontText.MeasureString( LocalizedEnums.Get( Stat.Agility ) ).X / 2), statLineStartY + (3 * LineSpacing) ),
                ColorStatName,
                drawContext 
            );

            string strAgility = statable.Agility.ToString( culture );
            DrawText( 
                fontText,
                strAgility,
                new Vector2( CenterStatValuesX - (int)(fontText.MeasureString( strAgility ).X / 2), statLineStartY + (3 * LineSpacing) ),
                ColorStatValue,
                drawContext
            );

            // Draw Intelligence Indicator:
            DrawText( 
                fontText, 
                LocalizedEnums.Get( Stat.Intelligence ),
                new Vector2( CenterStatNamesX - (int)(fontText.MeasureString( LocalizedEnums.Get( Stat.Intelligence ) ).X / 2), statLineStartY + (4 * LineSpacing) ),
                ColorStatName,
                drawContext 
            );

            string strIntelligence = statable.Intelligence.ToString( culture );
            DrawText( 
                fontText,
                strIntelligence,
                new Vector2( CenterStatValuesX - (int)(fontText.MeasureString( strIntelligence ).X / 2), statLineStartY + (4 * LineSpacing) ),
                ColorStatValue,
                drawContext
            );

            // Draw Luck Indicator:
            DrawText( 
                fontText, 
                LocalizedEnums.Get( Stat.Luck ),
                new Vector2( CenterStatNamesX - (int)(fontText.MeasureString( LocalizedEnums.Get( Stat.Luck ) ).X / 2), statLineStartY + (5 * LineSpacing) ),
                ColorStatName,
                drawContext 
            );

            string strLuck = statable.Luck.ToString( culture );
            DrawText(
                fontText, 
                strLuck,
                new Vector2( CenterStatValuesX - (int)(fontText.MeasureString( strLuck ).X / 2), statLineStartY + (5 * LineSpacing) ),
                ColorStatValue,
                drawContext 
            );
        }
        
        /// <summary>
        /// Called when the mouse is entering the client area of any of the stat-up buttons.
        /// </summary>
        /// <param name="element">
        /// The stat-up button that the mouse has entered.
        /// </param>
        private void OnStatUpButtonMouseEntering( UIElement element )
        {
            var button = (StatUpButton)element; 
            this.CaptureStatUpCost( button );
        }

        /// <summary>
        /// Stores the number of points that are required to invest into the Stat
        /// that the specified StatUpButton is responsible for.
        /// </summary>
        /// <param name="button">
        /// The button whose related stat-up cost will be captured.
        /// </param>
        private void CaptureStatUpCost( StatUpButton button )
        {
            if( button.IsVisible )
            {
                this.statUpCost = this.Player.Statable.GetPointsRequiredForStat( button.Stat );
            }
            else
            {
                // Display nothing if the button isn't even clickable
                this.statUpCost = 0;
            }
        }

        /// <summary>
        /// Resets the captured stat-up cost.
        /// </summary>
        protected void ResetStatUpCost()
        {
            this.statUpCost = 0;
        }

        /// <summary>
        /// Called when the mouse is leaving the client area of any of the stat-up buttons.
        /// </summary>
        /// <param name="element">
        /// The stat-up button that the mouse has left.
        /// </param>
        protected void OnStatUpButtonMouseLeaving( UIElement element )
        {
            this.ResetStatUpCost();
        }

        /// <summary>
        /// Called when one of the StatUp buttons gets clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        private void OnStatUpButtonClicked(
            object sender,
            ref Microsoft.Xna.Framework.Input.MouseState mouseState,
            ref Microsoft.Xna.Framework.Input.MouseState oldMouseState )
        {
            if( mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
                oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released )
            {
                var button = (StatUpButton)sender;

                if( this.Player.Statable.InvestInStat( button.Stat ) )
                {
                    OnStatChanged();
                    this.CaptureStatUpCost( button );
                }
                else
                {
                    this.ResetStatUpCost();
                }
            }
        }

        protected virtual void OnStatChanged()
        {
            // The player may just have invested into a specific
            // stat to make an equipment that turned red
            // equipable again -> recheck requirements! :)
            this.Player.Equipment.CheckItemRequirements();
            this.RefreshStatUpButtonVisability();
        }
        
        /// <summary>
        /// Refreshes what StatUp buttons are visible.
        /// </summary>
        private void RefreshStatUpButtonVisability()
        {
            foreach( StatUpButton button in this.statUpButtons )
            {
                if( this.Player != null && Player.Statable.CanInvestInStat( button.Stat ) && !Player.IsDead )
                {
                    button.ShowAndEnable();
                }
                else
                {
                    button.HideAndDisable();
                }
            }
        }

        /// <summary>
        /// Adds the child elements of this CharacterWindow to the given UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void AddChildElementsTo( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.AddElements( this.statUpButtons );
            userInterface.AddElements( this.statTooltips );
            userInterface.AddElements( this.statValueTooltips );

            userInterface.AddElement( this.toolTipDrawElement );
            userInterface.AddElement( this.statValueToolTipDrawElement );
        }

        /// <summary>
        /// Removes the child elements of this CharacterWindow from the given UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void RemoveChildElementsFrom( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.RemoveElements( this.statUpButtons );
            userInterface.RemoveElements( this.statTooltips );
            userInterface.RemoveElements( this.statValueTooltips );

            userInterface.RemoveElement( this.toolTipDrawElement );
            userInterface.RemoveElement( this.statValueToolTipDrawElement );
        }

        /// <summary>
        /// Gets called when this CharacterWindow is opening.
        /// </summary>
        protected override void Opening()
        {
            this.RefreshStatUpButtonVisability();

            foreach( TextTooltip tooltip in this.statTooltips )
            {
                tooltip.ShowAndEnable();
            }

            if( ShowsStatValueTooltips )
            {
                foreach( TextTooltip tooltip in this.statValueTooltips )
                {
                    tooltip.ShowAndEnable();
                }

                this.statValueToolTipDrawElement.ShowAndEnable();
            }

            this.toolTipDrawElement.ShowAndEnable();
        }

        /// <summary>
        /// Gets called when this CharacterWindow is closing.
        /// </summary>
        protected override void Closing()
        {
            foreach( StatUpButton button in this.statUpButtons )
            {
                button.HideAndDisable();
            }

            foreach( TextTooltip tooltip in this.statTooltips )
            {
                tooltip.HideAndDisable();
            }

            foreach( TextTooltip tooltip in this.statValueTooltips )
            {
                tooltip.HideAndDisable();
            }

            this.toolTipDrawElement.HideAndDisable();
            this.statValueToolTipDrawElement.HideAndDisable();
        }

        /// <summary>
        /// The number of status points that are required to invest one point
        /// into the stat that the player is hovering the invest button on.
        /// </summary>
        private int statUpCost;

        /// <summary>
        /// Enumerates the StatUpButton shown in the CharacterWindow.
        /// </summary>
        private readonly StatUpButton[] statUpButtons = new StatUpButton[6] {
            new StatUpButton( Stat.Strength     ),
            new StatUpButton( Stat.Dexterity    ),
            new StatUpButton( Stat.Vitality     ),
            new StatUpButton( Stat.Agility      ),
            new StatUpButton( Stat.Intelligence ),
            new StatUpButton( Stat.Luck         )
        };

        /// <summary>
        /// Enumerates the tooltips shown in the CharacterWindow.
        /// </summary>
        private readonly TextTooltip[] statTooltips = new TextTooltip[6];

        /// <summary>
        /// Enumerates the tooltips shown in the CharacterWindow.
        /// </summary>
        private readonly TextTooltip[] statValueTooltips = new TextTooltip[6];

        /// <summary>
        /// Defines the IToolTipDrawElement which draws the default tooltips shown in this CharacterWindow.
        /// </summary>
        private readonly ToolTipDrawElement toolTipDrawElement;

        /// <summary>
        /// Defines the IToolTipDrawElement which draws the tooltips shown
        /// when the player moves the mouse over a stat value in this CharacterWindow.
        /// </summary>
        private readonly StatValueToolTipDrawElement statValueToolTipDrawElement;

        /// <summary>
        /// Defines the class of buttons that
        /// when pressed increase a Stat of the player
        /// by one.
        /// </summary>
        protected sealed class StatUpButton : Atom.Xna.UI.Controls.TextButton
        {
            /// <summary>
            /// The Stat that is manipulated by this StatUpButton.
            /// </summary>
            public readonly Stat Stat;

            /// <summary>
            /// Initializes a new instance of the StatUpButton class.
            /// </summary>
            /// <param name="stat">
            /// The Stat that is manipulated by the new StatUpButton.
            /// </param>
            public StatUpButton( Stat stat )
                : base( "Button_StatUp_" + stat.ToString() )
            {
                this.Stat = stat;
                this.RelativeDrawOrder = 0.1f;
                this.IsVisible = false;
                this.IsEnabled = false;
            }
        }

        /// <summary>
        /// Defines the UIElement that draws the Tooltips shown in the Character Window.
        /// </summary>
        private class ToolTipDrawElement : TextFieldToolTipDrawElement
        {
            /// <summary>
            /// Initializes a new instance of the ToolTipDrawElement class.
            /// </summary>
            /// <param name="viewSize">
            /// The original size of the game-window (before it gets rescaled).
            /// </param>
            public ToolTipDrawElement( Point2 viewSize )
                : base()
            {
                this.IsVisible   = false;
                this.IsEnabled   = false;
                this.FloorNumber = 3;
                this.RelativeDrawOrder = 0.1f;

                this.Position = new Vector2( viewSize.X / 2, viewSize.Y / 2 );
            }

            /// <summary>
            /// Called when this ToolTipDrawElement is drawing itself.
            /// </summary>
            /// <param name="drawContext">
            /// The current ISpriteDrawContext.
            /// </param>
            protected override void OnDraw( ISpriteDrawContext drawContext )
            {
                if( this.Tooltip == null )
                {
                    return;
                }

                var zeldaDrawContext = (ZeldaDrawContext)drawContext;

                // Draw background.
                Xna.Rectangle area = this.ClientArea.ToXna();

                int borderWidth  = (int)(area.Width * 0.25f);
                int borderHeight = (int)(area.Height * 0.2f);

                area.X      -= borderWidth / 2;
                area.Y      -= borderHeight / 2;
                area.Width  += borderWidth;
                area.Height += borderHeight;

                zeldaDrawContext.Batch.DrawRect( area, Xna.Color.Black );

                // Draw text.
                base.OnDraw( drawContext );
            }
        }

        /// <summary>
        /// Defines the UIElement that draws the Tooltips 
        /// shown when the player mouse-overs one of the stat values
        /// in the Character Window.
        /// </summary>
        private sealed class StatValueToolTipDrawElement : TextFieldToolTipDrawElement
        {
            /// <summary>
            /// Initializes a new instance of the StatValueToolTipDrawElement class.
            /// </summary>
            /// <param name="window">
            /// The CharacterWindow that owns the new StatValueToolTipDrawElement.
            /// </param>
            public StatValueToolTipDrawElement( CharacterWindow window )
                : base()
            {
                this.window = window;

                this.IsVisible   = false;
                this.IsEnabled   = false;

                this.FloorNumber       = 3;
                this.RelativeDrawOrder = 0.1f;
            }

            /// <summary>
            /// Called when this StatValueToolTipDrawElement is drawing itself.
            /// </summary>
            /// <param name="drawContext">
            /// The current ISpriteDrawContext.
            /// </param>
            protected override void OnDraw( ISpriteDrawContext drawContext )
            {
                if( this.Tooltip == null )
                    return;

                var zeldaDrawContext = (ZeldaDrawContext)drawContext;

                // Draw background.
                Xna.Rectangle area = this.ClientArea.ToXna();

                const int BorderWidth = 4;
                area.X      -= BorderWidth / 2;
                area.Width  += BorderWidth;

                zeldaDrawContext.Batch.DrawRect( area, Xna.Color.Black );

                // Draw text.
                base.OnDraw( drawContext );
            }

            /// <summary>
            /// Called when this StatValueToolTipDrawElement is updating itself.
            /// </summary>
            /// <param name="updateContext">
            /// The current IUpdateContext.
            /// </param>
            protected override void OnUpdate( Atom.IUpdateContext updateContext )
            {
                // Update position.
                this.Position = this.Owner.MousePosition - new Vector2( 0.0f, 18.0f );
         
                base.OnUpdate( updateContext );
            }

            /// <summary>
            /// Gets called when the TextTooltip this StatValueToolTipDrawElement is visualizing has changed.
            /// </summary>
            protected override void OnTooltipChanged()
            {
                if( this.Tooltip != null )
                {
                    var stat     = (Stat)this.Tooltip.Tag;
                    ExtendedStatable statable = window.Player.Statable;

                    int baseValue  = statable.GetBaseStat( stat );
                    int totalValue = statable.GetStat( stat );
                    int deltaValue = totalValue - baseValue;

                    // Update string.
                    this.Tooltip.Text.TextString = string.Format(
                       CultureInfo.CurrentCulture,
                       "{0} + {1} = {2}",
                       baseValue.ToString( CultureInfo.CurrentCulture ),
                       deltaValue.ToString( CultureInfo.CurrentCulture ),
                       totalValue.ToString( CultureInfo.CurrentCulture )
                   );                        
                }
            }

            /// <summary>
            /// Identifies the CharacterWindow that owns this StatValueToolTipDrawElement.
            /// </summary>
            private readonly CharacterWindow window;
        }
    }
}
