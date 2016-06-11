// <copyright file="TalentWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.TalentWindow class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI
{
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;
    using Atom.Xna.UI;
    using Atom.Xna.UI.Controls;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Talents;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Defines an IngameWindow that allows the user to interact with
    /// the Talent Tree of this character.
    /// </summary>
    internal sealed class TalentWindow : IngameWindow
    {
        #region [ Constants ]

        /// <summary>
        /// The color of the network's lines that connects the talents.
        /// </summary>
        private readonly Xna.Color
            ColorFollowUpNotFulfilled = Xna.Color.White,
            ColorReqFulfilledInner = UIColors.PositiveDark,
            ColorReqFulfilled = UIColors.PositiveLight,
            ColorReqNotFulfilledInner = UIColors.NegativeDark,
            ColorReqNotFulfilled = UIColors.NegativeLight;

        private readonly Xna.Color ColorSpriteReqNotFulfilled = Xna.Color.Gray;

        /// <summary>
        /// The color of the background rectangle.
        /// </summary>
        private readonly Xna.Color ColorBackground = new Xna.Color( 0, 0, 0, 225 );

        /// <summary>
        /// The coordinate constants of the rows.
        /// </summary>
        private const float RowUpY = 65.0f, RowMiddleY = 105.0f, RowDownY = 145.0f;

        /// <summary>
        /// The coordinate constants of the columns.
        /// </summary>
        private readonly float ColumnLeftX = 40.0f, ColumnMiddleX = 180.0f, ColumnRightX = 300.0f;

        private const float Depth = 0.001f;
        private const float TextDepth = 0.01f;
        private const float DepthInnerLine = Depth + 0.00001f;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the currently selected talent.
        /// </summary>
        private Talent SelectedTalent
        {
            get
            {
                return this.selectedTalent;
            }

            set
            {
                this.selectedTalent = value;
                this.RefreshInvestButtonVisablity();
                this.RefreshTalentAccessButtonVisability();
            }
        }

        /// <summary>
        /// Gets the level for which information about the selected talent should be shown.
        /// </summary>
        private int ShownSelectedTalentLevel
        {
            get
            {
                if( this.selectedTalent == null )
                    return -1;

                if( this.selectedTalent.Level == this.selectedTalent.MaximumLevel )
                    return this.selectedTalent.MaximumLevel;

                return this.selectedTalent.Level + ((this.buttonInvest.IsEnabled && this.buttonInvest.IsMouseOver) ? 1 : 0);
            }
        }

        #endregion

        #region [ Enums ]

        /// <summary>
        /// Enumerates the tags set for the Talent Access buttons.
        /// </summary>
        private enum TalentAccessButtonTag
        {
            /// <summary>
            /// The first talent requisite; displayed in the middle row.
            /// </summary>
            RequirementFirst,

            /// <summary>
            /// The second talent requisite; displayed in the upper row.
            /// </summary>
            RequirementSecond,

            /// <summary>
            /// The third talent requisite; displayed in the bottum row.
            /// </summary>
            RequirementThird,

            /// <summary>
            /// The first talent that follows the selected talent; displayed in the middle row.
            /// </summary>
            FollowingFirst,

            /// <summary>
            /// The second talent that follows the selected talent; displayed in the upper row.
            /// </summary>
            FollowingSecond,

            /// <summary>
            /// The third talent that follows the selected talent; displayed in the bottum row.
            /// </summary>
            FollowingThird
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TalentWindow"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        internal TalentWindow( IZeldaServiceProvider serviceProvider )
        {
            // Set Properties
            this.Size = serviceProvider.ViewSize;

            this.ColumnMiddleX = (int)(this.Size.X / 2);
            this.ColumnLeftX = this.ColumnMiddleX - 140;
            this.ColumnRightX = this.ColumnMiddleX + 120;

            // Load Content
            var spriteLoader = serviceProvider.SpriteLoader;

            // Create Buttons
            this.talentQuickAccessButtons = new SpriteButton[4];
            this.talentButtons = new DynamicSpriteButton[6];

            this.buttonInvest = new SpriteButton(
                "Button_InvestTalent",
                spriteLoader.LoadSprite( "Button_Invest_Default" ),
                spriteLoader.LoadSprite( "Button_Invest_Selected" )
            );

            SetupButtons();
        }

        /// <summary>
        /// Setups the buttons used by this TalentWindow.
        /// </summary>
        private void SetupButtons()
        {
            // Invest Button:
            buttonInvest.FloorNumber = this.FloorNumber;
            buttonInvest.RelativeDrawOrder = 0.0001f;
            buttonInvest.Position = new Vector2(
                (this.Width / 2.0f) - (buttonInvest.ClientArea.Width / 2.0f),
                this.Height - buttonInvest.SpriteDefault.Height
            );

            buttonInvest.Clicked += OnInvestButtonClicked;
            buttonInvest.IsEnabled = false;
            buttonInvest.IsVisible = false;

            // Quick Access buttons:
            this.talentQuickAccessButtons[0] = new SpriteButton( "Button_MeleeRootTalent" );
            this.talentQuickAccessButtons[1] = new SpriteButton( "Button_RangedRootTalent" );
            this.talentQuickAccessButtons[2] = new SpriteButton( "Button_MagicRootTalent" );
            this.talentQuickAccessButtons[3] = new SpriteButton( "Button_SupportRootTalent" );

            // Talent Access buttons:
            var talentButtonClickedHandler = new MouseInputEventHandler( OnTalentAccessButtonClicked );
            var talentButtonSpriteReceiver = new DynamicSpriteButtonSpriteReceiver( TalentAccessButton_SpriteReceiver );

            const int OffsetX = 21 / 2, OffsetY = 19 / 2;

            // 'Following' Buttons.
            this.talentButtons[0] = new DynamicSpriteButton( "TalentAccessButton_Follow1" );
            SetupTalentAccessButton(
                talentButtons[0],
                TalentAccessButtonTag.FollowingFirst,
                new Vector2( ColumnRightX - OffsetX, RowMiddleY - OffsetY ),
                talentButtonSpriteReceiver,
                talentButtonClickedHandler
            );

            this.talentButtons[1] = new DynamicSpriteButton( "TalentAccessButton_Follow2" );
            SetupTalentAccessButton(
                talentButtons[1],
                TalentAccessButtonTag.FollowingSecond,
                new Vector2( ColumnRightX - OffsetX, RowUpY - OffsetY ),
                talentButtonSpriteReceiver,
                talentButtonClickedHandler
            );

            this.talentButtons[2] = new DynamicSpriteButton( "TalentAccessButton_Follow3" );
            SetupTalentAccessButton(
                talentButtons[2],
                TalentAccessButtonTag.FollowingThird,
                new Vector2( ColumnRightX - OffsetX, RowDownY - OffsetY ),
                talentButtonSpriteReceiver,
                talentButtonClickedHandler
            );

            // 'Requirement' Buttons.
            this.talentButtons[3] = new DynamicSpriteButton( "TalentAccessButton_Req1" );
            SetupTalentAccessButton(
                talentButtons[3],
                TalentAccessButtonTag.RequirementFirst,
                new Vector2( ColumnLeftX - OffsetX, RowMiddleY - OffsetY ),
                talentButtonSpriteReceiver,
                talentButtonClickedHandler
            );

            this.talentButtons[4] = new DynamicSpriteButton( "TalentAccessButton_Req2" );
            SetupTalentAccessButton(
                talentButtons[4],
                TalentAccessButtonTag.RequirementSecond,
                new Vector2( ColumnLeftX - OffsetX, RowUpY - OffsetY ),
                talentButtonSpriteReceiver,
                talentButtonClickedHandler
            );

            this.talentButtons[5] = new DynamicSpriteButton( "TalentAccessButton_Req3" );
            SetupTalentAccessButton(
                talentButtons[5],
                TalentAccessButtonTag.RequirementThird,
                new Vector2( ColumnLeftX - 11, RowDownY - 11 ),
                talentButtonSpriteReceiver,
                talentButtonClickedHandler
            );
        }

        /// <summary>
        /// Setups the quick access buttons.
        /// </summary>
        private void SetupTalentQuickAccessButtons()
        {
            if( this.Player != null )
            {
                Vector2 position = new Vector2( 5.0f, 2.0f );
                TalentTree talentTree = this.Player.TalentTree;
                const float GabBetweenButtons = 2.0f;

                // Setup MeleeRoot quick access button
                SetupQuickAccessButton( talentQuickAccessButtons[0], talentTree.MeleeRoot, position );
                position.X += talentTree.MeleeRoot.Symbol.Width + GabBetweenButtons;

                // Setup RangedRoot quick access button
                SetupQuickAccessButton( talentQuickAccessButtons[1], talentTree.RangedRoot, position );
                position.X += talentTree.RangedRoot.Symbol.Width + GabBetweenButtons;

                // Setup SupportRoot quick access button
                SetupQuickAccessButton( talentQuickAccessButtons[2], talentTree.MagicRoot, position );
                position.X += talentTree.MagicRoot.Symbol.Width + GabBetweenButtons;

                // Setup SupportRoot quick access button
                SetupQuickAccessButton( talentQuickAccessButtons[3], talentTree.SupportRoot, position );
                position.X += talentTree.SupportRoot.Symbol.Width + GabBetweenButtons;
            }
        }

        /// <summary>
        /// Setups the given talent access button.
        /// </summary>
        /// <param name="button">The button to setup.</param>
        /// <param name="tag">The tag that uniquely identifies the button.</param>
        /// <param name="position">The position of the button.</param>
        /// <param name="spriteReceiver">The SpriteReceiver delegate that decides what sprite the button uses.</param>
        /// <param name="clickedHandler">The event handler that gets invoked when the button was pressed.</param>
        private void SetupTalentAccessButton(
            DynamicSpriteButton button,
            TalentAccessButtonTag tag,
            Vector2 position,
            DynamicSpriteButtonSpriteReceiver spriteReceiver,
            MouseInputEventHandler clickedHandler )
        {
            button.FloorNumber = this.FloorNumber;
            button.RelativeDrawOrder = 0.002f;

            button.Size = new Vector2( 22.0f, 22.0f );
            button.ColorSelected = Xna.Color.Silver;
            button.SpriteReceiver = spriteReceiver;
            button.Clicked += clickedHandler;
            button.IsEnabled = false;
            button.IsVisible = false;
            button.Position = position;
            button.Tag = tag;
        }

        /// <summary>
        /// Setups the given button to allow quick access to the given talent.
        /// </summary>
        /// <param name="button">The button to setup.</param>
        /// <param name="talent">The talent the quick access button leads to.</param>
        /// <param name="position">The position of the button.</param>
        private void SetupQuickAccessButton( SpriteButton button, Talent talent, Vector2 position )
        {
            Sprite sprite = talent.Symbol;

            button.SpriteDefault = sprite;
            button.SpriteSelected = sprite;

            button.FloorNumber = this.FloorNumber;
            button.RelativeDrawOrder = 0.002f;

            button.ColorSelected = Xna.Color.Silver;
            button.Clicked += this.OnTtalentQuickAccessButton_Clicked;
            button.Position = position;
            button.Tag = talent;
            button.IsVisible = false;
            button.IsEnabled = false;
        }

        #endregion

        #region [ Methods ]

        #region > Drawing <

        /// <summary>
        /// Called when this TalentWindow is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( ISpriteDrawContext drawContext )
        {
            var batch = drawContext.Batch;
            batch.DrawRect( this.ClientArea, ColorBackground );

            if( this.Player == null || this.selectedTalent == null )
                return;

            var culture = System.Globalization.CultureInfo.CurrentCulture;
            TalentTree tree = Player.TalentTree;
            Sprite sprite = null;

            // Draw Selected Talent:
            sprite = selectedTalent.Symbol;
            sprite.Draw(
                new Vector2( ColumnMiddleX - (sprite.Width / 2), RowMiddleY - (sprite.Height / 2) ),
                selectedTalent.FulfillsRequirements() ? Xna.Color.White : ColorSpriteReqNotFulfilled,
                0.002f,
                batch
            );

            // Draw Name
            this.fontLargeText.Draw(
                this.selectedTalent.LocalizedName,
                new Vector2( ColumnMiddleX - (int)(this.fontLargeText.MeasureString( selectedTalent.LocalizedName ).X / 2), 22.0f ),
                Xna.Color.Wheat,
                TextDepth,
                drawContext
            );

            // Draw Level
            string strTalentLevel = string.Format(
                culture,
                Resources.TalentLevelXOfY,
                this.ShownSelectedTalentLevel.ToString( culture ),
                this.selectedTalent.MaximumLevel.ToString( culture )
            );

            this.fontSmallText.Draw(
                strTalentLevel,
                new Vector2( ColumnMiddleX - (int)(this.fontSmallText.MeasureString( strTalentLevel ).X / 2), 39.0f ),
                Xna.Color.White,
                TextDepth,
                drawContext
            );

            if( selectedTalent.Type == TalentType.Active )
            {
                string strTalentType = Resources.ActiveSkill;
                this.fontSmallText.Draw(
                    strTalentType,
                    new Vector2( ColumnMiddleX - (int)(this.fontSmallText.MeasureString( strTalentType ).X / 2), 51.0f ),
                    Xna.Color.Gray,
                    TextDepth,
                    drawContext
                );
            }

            this.DrawDescription( drawContext );
            this.DrawFreeTalentPoints( drawContext );
            this.DrawLinesToFollowingTalents( drawContext );
            this.DrawLinesToRequiredTalents( drawContext );
        }

        /// <summary>
        /// Draws the number of free talent points the player currently has.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawFreeTalentPoints( ISpriteDrawContext drawContext )
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            int freeTalentPoints = this.Player.TalentTree.FreeTalentPoints;

            if( freeTalentPoints > 0 )
            {
                string strFreeTalentPoints;
                if( freeTalentPoints == 1 )
                {
                    strFreeTalentPoints = Resources.OneFreeTalentPoint;
                }
                else
                {
                    strFreeTalentPoints = string.Format(
                        culture,
                        Resources.XFreeTalentPoints,
                        freeTalentPoints.ToString( culture )
                    );
                }

                var drawPosition = new Vector2(
                    this.Width - (int)fontSmallText.MeasureString( strFreeTalentPoints ).X,
                    this.Height - fontSmallText.LineSpacing
                );

                this.fontSmallText.Draw(
                    strFreeTalentPoints,
                    drawPosition,
                    Xna.Color.White,
                    TextDepth,
                    drawContext
                );
            }
        }

        /// <summary>
        /// Draws the description of the currently selected Talent.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        private void DrawDescription( ISpriteDrawContext drawContext )
        {
            int descriptionTalentLevel = this.selectedTalent.Level == 0 ? 1 : this.ShownSelectedTalentLevel;

            string description = this.selectedTalent.GetDescriptionSafe( descriptionTalentLevel );
            var size = this.fontText.MeasureString( description );

            var font = size.X > 325.0f ? fontSmallText : fontText;

            font.Draw(
                description,
                new Vector2( ColumnMiddleX - (int)(font.MeasureString( description ).X / 2), 160.0f ),
                Xna.Color.White,
                TextDepth,
                drawContext
            );
        }

        /// <summary>
        /// Draws the lines that connect the current talent with the 
        /// talents that follow it.
        /// </summary>
        /// <param name="drawContext">
        /// The current ZeldaDrawContext.
        /// </param>
        private void DrawLinesToFollowingTalents( ISpriteDrawContext drawContext )
        {
            var batch = drawContext.Batch;
            var culture = System.Globalization.CultureInfo.CurrentCulture;

            // first; middle
            Talent followingTalent = this.selectedTalent.GetFollowing( 0 );
            int talentLevel = this.ShownSelectedTalentLevel;
            bool followUpDisallowed = !(SelectedTalent.FulfillsRequirements() && Player.TalentTree.FreeTalentPoints > 0);

            if( followingTalent != null )
            {
                TalentRequirement requirement = followingTalent.GetRequirement( selectedTalent );
                bool isFulfilled = requirement != null && requirement.IsFulfilledAt( talentLevel );

                batch.DrawRect(
                    new Xna.Rectangle( (int)ColumnMiddleX, (int)RowMiddleY, (int)(ColumnRightX - ColumnMiddleX), 3 ),
                    isFulfilled ? ColorReqFulfilled : (followUpDisallowed ? ColorReqNotFulfilled : ColorFollowUpNotFulfilled),
                    Depth
                );

                batch.DrawRect(
                    new Xna.Rectangle( (int)ColumnMiddleX, (int)RowMiddleY + 1, (int)(ColumnRightX - ColumnMiddleX), 1 ),
                    isFulfilled ? ColorReqFulfilledInner : (followUpDisallowed ? ColorReqNotFulfilledInner : ColorFollowUpNotFulfilled),
                    DepthInnerLine
                );

                if( !isFulfilled && requirement != null )
                {
                    fontText.Draw(
                        requirement.RequiredTalentLevel.ToString( culture ),
                        new Vector2( ColumnMiddleX + (int)((ColumnRightX - ColumnMiddleX) * 0.75f), RowMiddleY - fontText.LineSpacing ),
                        ColorFollowUpNotFulfilled,
                        TextDepth,
                        drawContext
                    );
                }
            }

            // second; upper
            followingTalent = this.selectedTalent.GetFollowing( 1 );

            if( followingTalent != null )
            {
                TalentRequirement requirement = followingTalent.GetRequirement( selectedTalent );
                bool isFulfilled = requirement != null && requirement.IsFulfilledAt( talentLevel );

                batch.DrawRect(
                    new Xna.Rectangle( (int)ColumnMiddleX + 2, (int)RowMiddleY - (int)(RowMiddleY - RowUpY), 3, (int)(RowMiddleY - RowUpY) ),
                    isFulfilled ? ColorReqFulfilled : (followUpDisallowed ? ColorReqNotFulfilled : ColorFollowUpNotFulfilled),
                    Depth
                );

                batch.DrawRect(
                    new Xna.Rectangle( (int)ColumnMiddleX + 2, (int)RowUpY, (int)(ColumnRightX - ColumnMiddleX), 3 ),
                    isFulfilled ? ColorReqFulfilled : (followUpDisallowed ? ColorReqNotFulfilled : ColorFollowUpNotFulfilled),
                    Depth
                );

                // Inner
                batch.DrawRect(
                    new Xna.Rectangle( (int)ColumnMiddleX + 3, (int)RowMiddleY - (int)(RowMiddleY - RowUpY), 1, (int)(RowMiddleY - RowUpY) ),
                    isFulfilled ? ColorReqFulfilledInner : (followUpDisallowed ? ColorReqNotFulfilledInner : ColorFollowUpNotFulfilled),
                    DepthInnerLine
                );

                batch.DrawRect(
                    new Xna.Rectangle( (int)ColumnMiddleX + 2, (int)RowUpY + 1, (int)(ColumnRightX - ColumnMiddleX), 1 ),
                    isFulfilled ? ColorReqFulfilledInner : (followUpDisallowed ? ColorReqNotFulfilledInner : ColorFollowUpNotFulfilled),
                    DepthInnerLine
                );

                if( !isFulfilled && requirement != null )
                {
                    this.fontText.Draw(
                        requirement.RequiredTalentLevel.ToString( culture ),
                        new Vector2( ColumnMiddleX + (int)((ColumnRightX - ColumnMiddleX) * 0.75f), RowUpY - fontText.LineSpacing ),
                        ColorFollowUpNotFulfilled,
                        TextDepth,
                        drawContext
                    );
                }
            }

            // third; bottum
            followingTalent = this.selectedTalent.GetFollowing( 2 );

            if( followingTalent != null )
            {
                TalentRequirement requirement = followingTalent.GetRequirement( selectedTalent );
                bool isFulfilled = requirement != null && requirement.IsFulfilledAt( talentLevel );

                batch.DrawRect(
                    new Rectangle( (int)ColumnMiddleX + 2, (int)RowMiddleY, 3, (int)(RowDownY - RowMiddleY) ),
                    isFulfilled ? ColorReqFulfilled : (followUpDisallowed ? ColorReqNotFulfilled : ColorFollowUpNotFulfilled),
                    Depth
                );

                batch.DrawRect(
                    new Rectangle( (int)ColumnMiddleX + 2, (int)RowDownY, (int)(ColumnRightX - ColumnMiddleX), 3 ),
                    isFulfilled ? ColorReqFulfilled : (followUpDisallowed ? ColorReqNotFulfilled : ColorFollowUpNotFulfilled),
                    Depth
                );

                // Inner
                batch.DrawRect(
                    new Rectangle( (int)ColumnMiddleX + 3, (int)RowMiddleY, 1, (int)(RowDownY - RowMiddleY) ),
                    isFulfilled ? ColorReqFulfilledInner : (followUpDisallowed ? ColorReqNotFulfilledInner : ColorFollowUpNotFulfilled),
                    DepthInnerLine
                );

                batch.DrawRect(
                    new Rectangle( (int)ColumnMiddleX + 2, (int)RowDownY + 1, (int)(ColumnRightX - ColumnMiddleX), 1 ),
                    isFulfilled ? ColorReqFulfilledInner : (followUpDisallowed ? ColorReqNotFulfilledInner : ColorFollowUpNotFulfilled),
                    DepthInnerLine
                );

                if( !isFulfilled && requirement != null )
                {
                    this.fontText.Draw(
                        requirement.RequiredTalentLevel.ToString( culture ),
                        new Vector2( ColumnMiddleX + (int)((ColumnRightX - ColumnMiddleX) * 0.75f), RowDownY - fontText.LineSpacing ),
                        ColorFollowUpNotFulfilled,
                        TextDepth,
                        drawContext
                    );
                }
            }
        }

        /// <summary>
        /// Draws the lines that connect the current talent with the 
        /// talents it requires.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawLinesToRequiredTalents( ISpriteDrawContext drawContext )
        {
            var batch = drawContext.Batch;
            TalentRequirement requirement = this.selectedTalent.GetRequirement( 0 );

            if( requirement != null )
            {
                batch.DrawRect(
                    new Rectangle( (int)ColumnLeftX, (int)RowMiddleY, (int)(ColumnMiddleX - ColumnLeftX), 3 ),
                    requirement.IsFulfilled ? ColorReqFulfilled : ColorReqNotFulfilled,
                    Depth
                );

                batch.DrawRect(
                    new Rectangle( (int)ColumnLeftX, (int)RowMiddleY + 1, (int)(ColumnMiddleX - ColumnLeftX), 1 ),
                    requirement.IsFulfilled ? ColorReqFulfilledInner : ColorReqNotFulfilledInner,
                    DepthInnerLine
                );
            }

            requirement = this.selectedTalent.GetRequirement( 1 );

            if( requirement != null )
            {
                batch.DrawRect(
                    new Rectangle( (int)ColumnMiddleX - 2, (int)RowUpY, 3, (int)(RowMiddleY - RowUpY) ),
                    requirement.IsFulfilled ? ColorReqFulfilled : ColorReqNotFulfilled,
                    Depth
                );

                batch.DrawRect(
                    new Rectangle( (int)ColumnLeftX, (int)RowUpY, (int)(ColumnMiddleX - ColumnLeftX - 2), 3 ),
                    requirement.IsFulfilled ? ColorReqFulfilled : ColorReqNotFulfilled,
                    Depth
                );

                batch.DrawRect(
                    new Rectangle( (int)ColumnMiddleX - 1, (int)RowUpY, 1, (int)(RowMiddleY - RowUpY) ),
                    requirement.IsFulfilled ? ColorReqFulfilledInner : ColorReqNotFulfilledInner,
                    DepthInnerLine
                );

                batch.DrawRect(
                    new Rectangle( (int)ColumnLeftX, (int)RowUpY + 1, (int)(ColumnMiddleX - ColumnLeftX - 2), 1 ),
                    requirement.IsFulfilled ? ColorReqFulfilledInner : ColorReqNotFulfilledInner,
                    DepthInnerLine
                );
            }
        }

        #endregion

        #region > Updating <

        /// <summary>
        /// Called when this TalentWindow is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
        }

        #endregion

        #region > State <

        /// <summary>
        /// Gets called when this <see cref="TalentWindow"/> is opening.
        /// </summary>
        protected override void Opening()
        {
            foreach( SpriteButton button in this.talentQuickAccessButtons )
            {
                button.ShowAndEnable();
            }

            if( this.SelectedTalent == null )
            {
                this.SelectedTalent = this.Player.TalentTree.MeleeRoot;
            }
            else
            {
                this.RefreshInvestButtonVisablity();
                this.RefreshTalentAccessButtonVisability();
            }
        }

        /// <summary>
        /// Gets called when this <see cref="TalentWindow"/> is closing.
        /// </summary>
        protected override void Closing()
        {
            this.buttonInvest.HideAndDisable();

            foreach( SpriteButton button in this.talentQuickAccessButtons )
            {
                button.HideAndDisable();
            }

            foreach( DynamicSpriteButton button in this.talentButtons )
            {
                button.HideAndDisable();
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
            userInterface.AddElements( this.talentQuickAccessButtons );
            userInterface.AddElements( this.talentButtons );
            userInterface.AddElement( this.buttonInvest );
        }

        /// <summary>
        /// Removes the child elements of this IngameWindow from the specified UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface object.
        /// </param>
        protected override void RemoveChildElementsFrom( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.RemoveElements( this.talentQuickAccessButtons );
            userInterface.RemoveElements( this.talentButtons );
            userInterface.RemoveElement( this.buttonInvest );
        }

        #endregion

        #region > Refresh <

        /// <summary>
        /// Refreshes the visablity state of the Invest button
        /// based on the player's number of free talent points and
        /// the level of the selected talent.
        /// </summary>
        private void RefreshInvestButtonVisablity()
        {
            if( this.ShouldShowInvestButton() )
            {
                this.buttonInvest.ShowAndEnable();
            }
            else
            {
                this.buttonInvest.HideAndDisable();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Talent Invest button should be visible.
        /// </summary>
        /// <returns>
        /// true if it should be visible;
        /// otherwise false.
        /// </returns>
        private bool ShouldShowInvestButton()
        {
            if( this.selectedTalent != null && this.selectedTalent.FulfillsRequirements() && this.selectedTalent.Level < this.selectedTalent.MaximumLevel && !Player.IsDead )
            {
                return this.Player.TalentTree.FreeTalentPoints > 0;
            }

            return false;
        }

        /// <summary>
        /// Refreshes the visability of the Talent Access buttons
        /// based on the currently selected talent.
        /// </summary>
        private void RefreshTalentAccessButtonVisability()
        {
            foreach( DynamicSpriteButton button in this.talentButtons )
            {
                if( this.selectedTalent == null )
                {
                    button.HideAndDisable();
                    continue;
                }

                TalentAccessButtonTag tag = (TalentAccessButtonTag)button.Tag;
                bool isVisible = false;

                switch( tag )
                {
                    case TalentAccessButtonTag.FollowingFirst:
                        isVisible = this.selectedTalent.GetFollowing( 0 ) != null;
                        break;

                    case TalentAccessButtonTag.FollowingSecond:
                        isVisible = this.selectedTalent.GetFollowing( 1 ) != null;
                        break;

                    case TalentAccessButtonTag.FollowingThird:
                        isVisible = this.selectedTalent.GetFollowing( 2 ) != null;
                        break;

                    case TalentAccessButtonTag.RequirementFirst:
                        isVisible = this.selectedTalent.GetRequirement( 0 ) != null;
                        break;

                    case TalentAccessButtonTag.RequirementSecond:
                        isVisible = this.selectedTalent.GetRequirement( 1 ) != null;
                        break;

                    case TalentAccessButtonTag.RequirementThird:
                        isVisible = this.selectedTalent.GetRequirement( 2 ) != null;
                        break;

                    default:
                        throw new System.NotImplementedException();
                }

                if( isVisible )
                {
                    button.ShowAndEnable();
                }
                else
                {
                    button.HideAndDisable();
                }
            }
        }

        #endregion

        /// <summary>
        /// Called when the PlayerEntity that owns this IngameWindow has changed.
        /// </summary>
        protected override void OnPlayerChanged()
        {
            this.selectedTalent = null;
            this.SetupTalentQuickAccessButtons();
        }

        #endregion

        #region [ Events ]

        /// <summary>
        /// Called when any of the talent quick access buttons has been clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        private void OnTtalentQuickAccessButton_Clicked(
            object sender,
            ref Microsoft.Xna.Framework.Input.MouseState mouseState,
            ref Microsoft.Xna.Framework.Input.MouseState oldMouseState )
        {
            if( mouseState.LeftButton == ButtonState.Pressed &&
                oldMouseState.LeftButton == ButtonState.Released )
            {
                SpriteButton button = (SpriteButton)sender;
                this.SelectedTalent = (Talent)button.Tag;
            }
        }

        /// <summary>
        /// Called when the user clicks on the invest button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        private void OnInvestButtonClicked(
            object sender,
            ref Microsoft.Xna.Framework.Input.MouseState mouseState,
            ref Microsoft.Xna.Framework.Input.MouseState oldMouseState )
        {
            if( mouseState.LeftButton == ButtonState.Pressed &&
                oldMouseState.LeftButton == ButtonState.Released )
            {
                if( this.selectedTalent != null && this.Player != null )
                {
                    if( this.Player.TalentTree.InvestInto( selectedTalent.GetType() ) )
                    {
                        RefreshInvestButtonVisablity();
                    }
                }
            }
        }

        /// <summary>
        /// Called when any of the talent access buttons has been clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        private void OnTalentAccessButtonClicked(
            object sender,
            ref Microsoft.Xna.Framework.Input.MouseState mouseState,
            ref Microsoft.Xna.Framework.Input.MouseState oldMouseState )
        {
            if( this.selectedTalent == null )
                return;

            if( mouseState.LeftButton == ButtonState.Pressed &&
                oldMouseState.LeftButton == ButtonState.Released )
            {
                DynamicSpriteButton button = (DynamicSpriteButton)sender;
                TalentAccessButtonTag tag = (TalentAccessButtonTag)button.Tag;

                Talent talent = null;

                switch( tag )
                {
                    case TalentAccessButtonTag.FollowingFirst:
                        talent = this.selectedTalent.GetFollowing( 0 );
                        break;

                    case TalentAccessButtonTag.FollowingSecond:
                        talent = this.selectedTalent.GetFollowing( 1 );
                        break;

                    case TalentAccessButtonTag.FollowingThird:
                        talent = this.selectedTalent.GetFollowing( 2 );
                        break;

                    case TalentAccessButtonTag.RequirementFirst:
                        {
                            TalentRequirement requirement = this.selectedTalent.GetRequirement( 0 );
                            if( requirement != null )
                                talent = requirement.RequiredTalent;
                        }
                        break;

                    case TalentAccessButtonTag.RequirementSecond:
                        {
                            TalentRequirement requirement = this.selectedTalent.GetRequirement( 1 );
                            if( requirement != null )
                                talent = requirement.RequiredTalent;
                        }
                        break;

                    case TalentAccessButtonTag.RequirementThird:
                        {
                            TalentRequirement requirement = this.selectedTalent.GetRequirement( 2 );
                            if( requirement != null )
                                talent = requirement.RequiredTalent;
                        }
                        break;

                    default:
                        throw new System.NotImplementedException();
                }

                if( talent != null )
                {
                    this.SelectedTalent = talent;
                }
            }
        }

        /// <summary>
        /// Called by the TalentAccess buttons when they wish to receiv their sprite data.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="spriteDefault">Will contain the default sprite.</param>
        /// <param name="spriteSelected">Will contain the selected sprite.</param>
        private void TalentAccessButton_SpriteReceiver(
            DynamicSpriteButton sender,
            out ISprite spriteDefault,
            out ISprite spriteSelected )
        {
            spriteDefault = spriteSelected = null;

            if( selectedTalent == null )
                return;

            Sprite sprite = null;
            Talent talent = null;
            TalentAccessButtonTag tag = (TalentAccessButtonTag)sender.Tag;

            switch( tag )
            {
                case TalentAccessButtonTag.FollowingFirst:
                    {
                        talent = selectedTalent.GetFollowing( 0 );
                    }
                    break;

                case TalentAccessButtonTag.FollowingSecond:
                    {
                        talent = selectedTalent.GetFollowing( 1 );
                    }
                    break;

                case TalentAccessButtonTag.FollowingThird:
                    {
                        talent = selectedTalent.GetFollowing( 2 );
                    }
                    break;

                case TalentAccessButtonTag.RequirementFirst:
                    {
                        TalentRequirement talentReq = selectedTalent.GetRequirement( 0 );
                        if( talentReq != null )
                        {
                            talent = talentReq.RequiredTalent;
                        }
                    }
                    break;

                case TalentAccessButtonTag.RequirementSecond:
                    {
                        TalentRequirement talentReq = selectedTalent.GetRequirement( 1 );
                        if( talentReq != null )
                            talent = talentReq.RequiredTalent;
                    }
                    break;

                case TalentAccessButtonTag.RequirementThird:
                    {
                        TalentRequirement talentReq = selectedTalent.GetRequirement( 2 );
                        if( talentReq != null )
                            talent = talentReq.RequiredTalent;
                    }
                    break;

                default:
                    throw new System.NotImplementedException();
            }

            if( talent != null )
            {
                sprite = talent.Symbol;
            }

            spriteDefault = spriteSelected = sprite;
            sender.ColorDefault = GetTalentSpriteColor( talent );
        }

        private Xna.Color GetTalentSpriteColor( Talent talent )
        {
            bool fullfilled = talent != null && talent.FulfillsRequirementsWith( this.selectedTalent, this.ShownSelectedTalentLevel );
            return fullfilled ? Xna.Color.White : ColorSpriteReqNotFulfilled;
        }

        #endregion

        #region [ Fields ]

        /// <summary> 
        /// Identifies the currently selected talent. 
        /// </summary>
        private Talent selectedTalent;

        /// <summary>
        /// IFonts used for text rendering.
        /// </summary>
        private readonly IFont
            fontLargeText = UIFonts.TahomaBold11,
            fontText = UIFonts.Tahoma10,
            fontSmallText = UIFonts.Tahoma7;

        /// <summary>
        /// The button that when clicked invests one talent point into the currently selected talent.
        /// </summary>
        private readonly SpriteButton buttonInvest;

        /// <summary>
        /// The buttons that provide quick access to the root talents (melee, ranged, magic, support).
        /// </summary>
        private readonly SpriteButton[] talentQuickAccessButtons;

        /// <summary>
        /// The buttons that provide access to the pre-requisite and following talents.
        /// </summary>
        private readonly DynamicSpriteButton[] talentButtons;

        #endregion
    }
}