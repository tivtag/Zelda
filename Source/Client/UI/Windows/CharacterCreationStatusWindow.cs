
namespace Zelda.UI
{
    using Atom.Math;
    using Atom.Xna.UI;
    using Zelda.Status;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a slight modification of the <see cref="CharacterWindow"/> used while creating the character.
    /// </summary>
    internal sealed class CharacterCreationStatusWindow : CharacterWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterCreationStatusWindow"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related servicess.
        /// </param>
        internal CharacterCreationStatusWindow( IZeldaServiceProvider serviceProvider )
            : base( serviceProvider )
        {
            this.ShowsStatValueTooltips = false;
            SetupStatDownButtons();
        }

        /// <summary>
        /// Setups the Stat-Down buttons, called once on creation of the window.
        /// </summary>
        private void SetupStatDownButtons()
        {
            Vector2 buttonPosition = new Vector2( CenterStatValuesX + 46, LineStartY + 2 );

            foreach( var button in this.statDownButtons )
            {
                button.Text = "-";
                button.Font = fontText;
                button.Position = buttonPosition;
                button.ColorSelected = Xna.Color.LightGreen;
                button.FloorNumber = this.FloorNumber;
                button.Clicked += this.OnStatDownButtonClicked;
                button.MouseEntering += this.OnStatDownButtonMouseEntering;
                button.MouseLeaving += this.OnStatUpButtonMouseLeaving;

                buttonPosition.Y += LineSpacing;
            }
        }

        /// <summary>
        /// Called when the mouse is entering the client area of any of the stat-up buttons.
        /// </summary>
        /// <param name="element">
        /// The stat-up button that the mouse has entered.
        /// </param>
        private void OnStatDownButtonMouseEntering( UIElement element )
        {
            var button = (StatUpButton)element;
            this.CaptureStatDownCost( button );
        }

        /// <summary>
        /// Stores the number of points that are required to undo the investment into the Stat
        /// that the specified StatUpButton is responsible for.
        /// </summary>
        /// <param name="button">
        /// The button whose related stat-down cost will be captured.
        /// </param>
        private void CaptureStatDownCost( StatUpButton button )
        {
            if( button.IsVisible )
            {
                this.StatUpCost = -this.Player.Statable.GetPointsGainedByDecreasingStat( button.Stat );
            }
            else
            {
                this.StatUpCost = 0;
            }
        }

        /// <summary>
        /// Called when one of the stat-down buttons gets clicked.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="mouseState">The current state of the mouse.</param>
        /// <param name="oldMouseState">The state of the mouse one frame ago.</param>
        private void OnStatDownButtonClicked(
            object sender,
            ref Microsoft.Xna.Framework.Input.MouseState mouseState,
            ref Microsoft.Xna.Framework.Input.MouseState oldMouseState )
        {
            if( mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
                oldMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released )
            {
                var button = (StatUpButton)sender;

                if( this.Player.Statable.UndoInvestmentInStat( button.Stat ) )
                {
                    OnStatChanged();
                    this.CaptureStatDownCost( button );
                }
                else
                {
                    this.ResetStatUpCost();
                }
            }
        }

        protected override void DrawBackground( Atom.Xna.ISpriteDrawContext drawContext )
        {
            // no op
            drawContext.Batch.DrawRect( new Rectangle( 3, 43, (int)this.Size.X - 7, (int)this.Size.Y - 74 ), new Xna.Color( 255, 255, 255, 15 ) );
            drawContext.Batch.DrawLineRect( new Rectangle( 3, 43, (int)this.Size.X - 7, (int)this.Size.Y - 74 ), new Xna.Color( 255, 255, 255, 25 ) );
        }
        
        protected override void DrawBasicStatistics( Atom.Xna.ISpriteDrawContext drawContext )
        {
            // no op
        }

        /// <summary>
        /// Refreshes what StatDown buttons are visible.
        /// </summary>
        private void RefreshStatDownButtonVisability()
        {
            foreach( StatUpButton button in this.statDownButtons )
            {
                if( this.Player != null && Player.Statable.GetStat( button.Stat ) > 1 )
                {
                    button.ShowAndEnable();
                }
                else
                {
                    button.HideAndDisable();
                }
            }
        }

        protected override void OnStatChanged()
        {
            this.RefreshStatDownButtonVisability();
            base.OnStatChanged();
        }

        /// <summary>    
        /// Gets called when this CharacterCreationStatusWindow is opening.
        /// </summary>
        protected override void Opening()
        {
            this.RefreshStatDownButtonVisability();
            base.Opening();
        }

        /// <summary>
        /// Gets called when this CharacterCreationStatusWindow is closing.
        /// </summary>
        protected override void Closing()
        {
            foreach( var button in this.statDownButtons )
            {
                button.HideAndDisable();
            }

            base.Closing();
        }

        /// <summary>
        /// Adds the child elements of this CharacterCreationStatusWindow to the given UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void AddChildElementsTo( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.AddElements( this.statDownButtons );
            base.AddChildElementsTo( userInterface );
        }

        /// <summary>
        /// Removes the child elements of this CharacterCreationStatusWindow from the given UserInterface.
        /// </summary>
        /// <param name="userInterface">
        /// The related UserInterface.
        /// </param>
        protected override void RemoveChildElementsFrom( Atom.Xna.UI.UserInterface userInterface )
        {
            userInterface.RemoveElements( this.statDownButtons );
            base.RemoveChildElementsFrom( userInterface );
        }
        /// <summary>
        /// Enumerates the StatUpButton for point reduction shown in the CharacterCreationStatusWindow.
        /// </summary>
        private readonly StatUpButton[] statDownButtons = new StatUpButton[6] {            
            new StatUpButton( Stat.Strength     ),
            new StatUpButton( Stat.Dexterity    ),
            new StatUpButton( Stat.Vitality     ),
            new StatUpButton( Stat.Agility      ),
            new StatUpButton( Stat.Intelligence ),
            new StatUpButton( Stat.Luck         )
        };
    }
}
