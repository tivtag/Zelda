// <copyright file="QuestLogWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.QuestLogWindow class.
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
    using Atom.Xna.UI.Controls;
    using Microsoft.Xna.Framework.Input;
    using Zelda.Quests;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Defines an IngameWindow that visualizes the QuestLog of the player.
    /// </summary>
    internal sealed class QuestLogWindow : IngameWindow
    {
        #region [ Constants ]

        /// <summary>
        /// The maximum number of quests shown at a time in the Quest List.
        /// </summary>
        private const int QuestListLength = 8;

        /// <summary>
        /// Position constant(s) used in this QuestLogWindow.
        /// </summary>
        private const int QuestListOffsetX = 30, QuestListOffsetY = 40, QuestListEntrySize = 22;

        /// <summary>
        /// The color of the background rectangle.
        /// </summary>
        private readonly Xna.Color ColorBackground = new Xna.Color( 0, 0, 0, 155 );

        /// <summary>
        /// The color of a quest field.
        /// </summary>
        private readonly Xna.Color ColorQuestField = new Xna.Color( 0, 0, 0, 55 );

        /// <summary>
        /// The color of the main quest indicators.
        /// </summary>
        private readonly Xna.Color ColorMainQuest = UIColors.NegativeLight;

        /// <summary>
        /// The color of a selected quest field.
        /// </summary>
        private readonly Xna.Color ColorSelectedQuestField = new Xna.Color( 100, 0, 0, 155 );

        #endregion

        #region [ Enums ]

        /// <summary>
        /// Enumerates the different possible states the QuestLogWindow can be in.
        /// </summary>
        private enum WindowState
        {
            /// <summary>
            /// In this state the QuestLogWindow shows the currently active quests.
            /// </summary>
            QuestList,

            /// <summary>
            /// In this state the QuestLogWindow shows the currently selected quest.
            /// </summary>
            SelectedQuest
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="QuestLog"/> that gets visualized by this QuestLogWindow.
        /// </summary>
        public QuestLog QuestLog
        {
            get;
            set;
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the QuestLogWindow class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public QuestLogWindow( IZeldaServiceProvider serviceProvider )
        {
            this.Size = serviceProvider.ViewSize;

            var textSplitter = new TextBlockSplitter( this.fontQuestState, (int)this.Width - 40 );
            var text = new PartiallyShownText( this.fontQuestState, TextAlign.Left, Xna.Color.White, textSplitter ) {
                TextBlockWidth = (int)this.Width - 60,
                LinesShown     = 6
            };

            this.textFieldQuestDetails = new TextField( "QuestLogWindow_QuestDetails_TextField" ) {
                Position          = new Vector2( 20.0f, 60.0f ),
                FloorNumber       = this.FloorNumber + 1,
                RelativeDrawOrder = 0.0f,
                Text = text
            };

            // Back Button
            this.backButton = new NavButton( "BackButton", serviceProvider ) {
                Position = new Vector2( 3, serviceProvider.ViewSize.Y - 23 ),
                ButtonMode = NavButton.Mode.Back
            };

            backButton.Clicked += OnBackButtonClicked;

            this.spriteSkull = serviceProvider.SpriteLoader.LoadSprite( "SmallSkeletonHead" );
        }

        #endregion

        #region [ Methods ]

        #region > Drawing <

        /// <summary>
        /// Called when this QuestLogWindow is drawing itself.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        protected override void OnDraw( Atom.Xna.ISpriteDrawContext drawContext )
        {
            var zeldaDrawContext = (ZeldaDrawContext)drawContext;
            var batch            = zeldaDrawContext.Batch;

            // Draw Background
            batch.DrawRect( this.ClientArea, ColorBackground );

            // Draw Quest Log String Background
            batch.DrawRect(
                new Xna.Rectangle( 0, 0, (int)this.Width, 20 ),
                ColorBackground,
                0.001f
            );

            // Draw Quest Log String
            this.fontTitle.Draw(
                Resources.QuestLog, 
                new Vector2( this.Width / 2.0f, 0.0f ),
                TextAlign.Center, 
                Xna.Color.White,
                0.002f,
                drawContext
            );

            if( state == WindowState.QuestList )
                this.Draw_QuestList( zeldaDrawContext );
            else // if( state == WindowState.SelectedQuest )
                this.Draw_SelectedQuest( zeldaDrawContext );
        }

        /// <summary>
        /// Draws this QuestLogWindow, showing the quest list.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void Draw_QuestList( ZeldaDrawContext drawContext )
        {
            if( this.QuestLog == null )
                return;

            var batch = drawContext.Batch;

            for( int index = 0, offsetY = QuestListOffsetY;
                 index < QuestListLength && index < this.QuestLog.ActiveQuestCount;
                 ++index, offsetY += QuestListEntrySize
               )
            {
                Quest quest = this.QuestLog.GetActiveQuest( index );
                if( quest == null )
                    continue;

                if( index == mouseHoveredQuestIndex )
                    batch.DrawRect( new Rectangle( QuestListOffsetX, offsetY, (int)this.Width - 60, QuestListEntrySize ), ColorSelectedQuestField );
                else if( index % 2 == 0 )
                    batch.DrawRect( new Rectangle( QuestListOffsetX, offsetY, (int)this.Width - 60, QuestListEntrySize ), ColorQuestField );

                // Draw Quest Name
                string questName = quest.LocalizedName ?? quest.Name;
                this.fontQuestListEntry.Draw(
                    questName,
                    new Vector2( QuestListOffsetX + (fontQuestListEntry.LineSpacing / 2) + 1, offsetY + 2 ),
                    Quest.GetQuestColor( quest, this.QuestLog.Owner ),
                    0.2f,
                    drawContext
                );

                if( quest.QuestType == QuestType.Main )
                {
                    spriteSkull.Draw(
                         new Vector2( this.Width - QuestListOffsetX - 55, offsetY + 6 ),
                        0.19f,
                        batch
                    );
                }

                this.DrawQuestState( quest, offsetY, drawContext );
            }
        }
        
        /// <summary>
        /// Draws the state of the quest in the quest list.
        /// </summary>
        /// <param name="quest">The quest whose state should be drawn.</param>
        /// <param name="offsetY">The drawing offset on the y-axis.</param>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawQuestState( Quest quest, int offsetY, ISpriteDrawContext drawContext )
        {
            int state = (int)(quest.State * 100);      

            this.fontQuestState.Draw(
                GetQuestStateString( quest, state ),
                new Vector2( this.Width - QuestListOffsetX - 3, offsetY + 3 ),
                TextAlign.Right,
                state >= 100 ? Xna.Color.WhiteSmoke : Xna.Color.White,
                0.3f,
                drawContext
           );
        }

        /// <summary>
        /// Gets a string that represents the current state of the specified Quest.
        /// </summary>
        /// <param name="quest">The quest to investigate.</param>
        /// <param name="state">The state of the speciefied Quest; as a value from 0 to 100%.</param>
        /// <returns>
        /// A short human-readable string that descripes the state of the specified Quest.
        /// </returns>
        private static string GetQuestStateString( Quest quest, int state )
        {
            if( ShouldHideQuestState( quest, state ) )
            {
                return "? %";
            }
            else
            {
                return state.ToString( System.Globalization.CultureInfo.CurrentCulture ) + "%";
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified Quest should hide its quest state.
        /// </summary>
        /// <param name="quest">The quest to investigate.</param>
        /// <param name="state">The state of the speciefied Quest; as a value from 0 to 100%.</param>
        /// <returns>
        /// true if the true quest state should be hidden from the player;
        /// otherwise false.
        /// </returns>
        private static bool ShouldHideQuestState( Quest quest, int state )
        {
            if( quest.IsStateHidden )
            {
                return !(quest.GoalCount > 0 && state == 100);
            }

            return false;
        }

        /// <summary>
        /// Draws this QuestLogWindow, showing the currently selected quest.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void Draw_SelectedQuest( ISpriteDrawContext drawContext )
        {
            if( selectedQuest == null )
                return;

            // Draw Quest Type
            if( selectedQuest.QuestType == QuestType.Main )
            {
                UIFonts.TahomaBold10.Draw(
                    "Main Quest",
                    new Vector2( this.Width / 2.0f, 23.0f ),
                    TextAlign.Center,
                    ColorMainQuest,
                    0.2f,
                    drawContext
                );
            }

            // Draw Quest Name
            this.fontQuestListEntry.Draw(
                this.selectedQuest.LocalizedName ?? selectedQuest.Name,
                new Vector2( this.Width / 2.0f, 39.0f ),
                TextAlign.Center,
                Quest.GetQuestColor( selectedQuest, this.QuestLog.Owner ),
                0.2f,
                drawContext
            );

            this.DrawSelectedQuestState( drawContext );
        }

        /// <summary>
        /// Draws the current state of the selected quest in detail.
        /// </summary>
        /// <param name="drawContext">
        /// The current ISpriteDrawContext.
        /// </param>
        private void DrawSelectedQuestState( ISpriteDrawContext drawContext )
        {
            if( this.selectedQuest.IsStateHidden )
                return;

            float offsetY = 150.0f;
            int goalIndex = 1;

            foreach( var goal in this.selectedQuest.Goals )
            {
                string description = goal.StateDescription;
                if( description == null )
                    continue;

                description = goalIndex.ToString( CultureInfo.CurrentCulture ) + ".) " + description;

                this.fontQuestGoalState.Draw(
                    description,
                    new Vector2( 20.0f, offsetY ),
                    TextAlign.Left,
                    Xna.Color.White,
                    0.2f,
                    drawContext
                );

                offsetY += fontQuestGoalState.LineSpacing;
                ++goalIndex;
            }
        }

        #endregion

        #region > Updating <

        /// <summary>
        /// Called when this QuestLogWindow is updating itself.
        /// </summary>
        /// <param name="updateContext">
        /// The current IUpdateContext.
        /// </param>
        protected override void OnUpdate( Atom.IUpdateContext updateContext )
        {
        }

        #endregion

        #region > Input <

        /// <summary>
        /// Handles mouse input related to this QuestLogWindow.
        /// </summary>
        /// <param name="mouseState">
        /// The current state of the mouse.
        /// </param>
        /// <param name="oldMouseState">
        /// The state of the mouse one frame ago.
        /// </param>
        protected override void HandleMouseInput( 
            ref Microsoft.Xna.Framework.Input.MouseState mouseState, 
            ref Microsoft.Xna.Framework.Input.MouseState oldMouseState )
        {
            if( !this.IsEnabled )
                return;

            int x = mouseState.X;
            int y = mouseState.Y;

            var rectangle = new Atom.Math.Rectangle( 
                QuestListOffsetX,
                QuestListOffsetY,
                (int)this.Width - (QuestListOffsetX * 2), 
                QuestListLength * 25
            );

            if( rectangle.Contains( x, y ) )
            {
                mouseHoveredQuestIndex = (y - QuestListOffsetY) / QuestListEntrySize;

                if( this.state == WindowState.QuestList )
                {
                    if( mouseState.LeftButton == ButtonState.Pressed &&
                        oldMouseState.LeftButton == ButtonState.Released )
                    {
                        int questIndex = mouseHoveredQuestIndex;

                        if( questIndex >= 0 && questIndex < this.QuestLog.ActiveQuestCount )
                        {
                            ChangeSelectedQuest( this.QuestLog.GetActiveQuest( questIndex ) );
                            ChangeState( WindowState.SelectedQuest );
                        }
                    }
                }
            }
            else
            {
                mouseHoveredQuestIndex = -1;
            }
        }

        /// <summary>
        /// Called every frame when this Atom.Xna.UI.UIElement is focused by its owning Atom.Xna.UI.UserInterface.
        /// </summary>
        /// <param name="keyState">The state of the Microsoft.Xna.Framework.Input.Keyboard.</param>
        /// <param name="oldKeyState">The state of the Microsoft.Xna.Framework.Input.Keyboard one frame ago.</param>
        protected override void HandleKeyInput( ref KeyboardState keyState, ref KeyboardState oldKeyState )
        {
            if( keyState.IsKeyDown( Keys.Delete ) )
            {
                ChangeState( WindowState.QuestList );
            }

            base.HandleKeyInput( ref keyState, ref oldKeyState );
        }

        /// <summary>
        /// Sets the currently selectedQuest to the given quest. 
        /// </summary>
        /// <param name="quest">
        /// The Quest to set. Can be null.
        /// </param>
        private void ChangeSelectedQuest( Quest quest )
        {
            if( quest != null )
            {
                textFieldQuestDetails.Text.TextString = quest.LocalizedTextDescription;
            }
            else
            {
                textFieldQuestDetails.Text.TextString = null;
            }

            this.selectedQuest = quest;
        }

        /// <summary>
        /// Called when the user clicks on the Back-button in the quest details.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">
        /// The current state of the mouse.
        /// </param>
        /// <param name="oldMouseState">
        /// The state of the mouse one frame ago.
        /// </param>
        private void OnBackButtonClicked( object sender, ref MouseState mouseState, ref MouseState oldMouseState )
        {
            ChangeState( WindowState.QuestList );
        }

        #endregion

        #region > State <
        
        /// <summary>
        /// Adds the child elements of this IngameWindow to the specified UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface object.
        /// </param>
        protected override void AddChildElementsTo( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.AddElement( this.backButton );
            userInterface.AddElement( this.textFieldQuestDetails );
        }

        /// <summary>
        /// Removes the child elements of this IngameWindow from the specified UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface object.
        /// </param>
        protected override void RemoveChildElementsFrom( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.RemoveElement( this.backButton );
            userInterface.RemoveElement( this.textFieldQuestDetails );
        }

        /// <summary>
        /// Changes the current state of this <see cref="QuestLogWindow"/>.
        /// </summary>
        /// <param name="newState">
        /// The state to change to.
        /// </param>
        private void ChangeState( WindowState newState )
        {
            switch( newState )
            {
                case WindowState.QuestList:
                    this.backButton.HideAndDisable();
                    this.textFieldQuestDetails.HideAndDisable();
                    break;

                case WindowState.SelectedQuest:
                    this.backButton.ShowAndEnable();
                    this.textFieldQuestDetails.ShowAndEnable();      
                    break;

                default:
                    break;
            }

            this.state = newState;
        }

        /// <summary>
        /// Called when this QuestLogWindow is opening.
        /// </summary>
        protected override void Opening()
        {
            this.ChangeState( WindowState.QuestList );
        }

        /// <summary>
        /// Called when this QuestLogWindow is closing.
        /// </summary>
        protected override void Closing()
        {
            this.textFieldQuestDetails.HideAndDisable();
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The index of the currently mouse-hovered quest.
        /// </summary>
        private int mouseHoveredQuestIndex = -1;

        /// <summary>
        /// The currently selected Quest.
        /// </summary>
        private Quest selectedQuest;

        /// <summary>
        /// The current state of this QuestLogWindow.
        /// </summary>
        private WindowState state = WindowState.QuestList;

        /// <summary>
        /// The sprite used for the main quest indicator.
        /// </summary>
        private readonly Sprite spriteSkull;

        /// <summary>
        /// The TextField that is used to show the details about this Quest.
        /// </summary>
        private readonly TextField textFieldQuestDetails;

        /// <summary>
        /// Identifies the IFont(s) that is used in this IngameWindow.
        /// </summary>
        private readonly IFont
            fontTitle          = UIFonts.TahomaBold11,
            fontQuestListEntry = UIFonts.VerdanaBold11,
            fontQuestState     = UIFonts.TahomaBold10,
            fontQuestGoalState = UIFonts.Tahoma10;
        private NavButton backButton;

        #endregion
    }
}