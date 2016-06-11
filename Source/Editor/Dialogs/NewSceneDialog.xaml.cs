// <copyright file="NewSceneDialog.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Dialogs.NewSceneDialog class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Dialogs
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Asks the user to enter the properties needed to create a new Scene.
    /// </summary>
    sealed partial class NewSceneDialog : Window
    {
        #region [ Constants ]

        /// <summary>
        /// The minimum number of tiles a scene must have. 
        /// </summary>
        private const int MinimumWidth  = 50, MinimumHeight = 50;

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="NewSceneDialog"/> class.
        /// </summary>
        public NewSceneDialog()
        {
            InitializeComponent();

            textBox_Width.Text  = MinimumWidth.ToString( CultureInfo.CurrentCulture );
            textBox_Height.Text = MinimumHeight.ToString( CultureInfo.CurrentCulture );
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the scene name which has been selected by the user in the <see cref="NewSceneDialog"/>.
        /// </summary>
        public string SelectedName
        {
            get 
            { 
                return this.textBox_Name.Text; 
            }
        }

        /// <summary>
        /// Gets the scene width (in tile-space) which has been selected by the user in the <see cref="NewSceneDialog"/>.
        /// </summary>
        public int SelectedWidth
        {
            get 
            {
                return int.Parse( this.textBox_Width.Text, CultureInfo.CurrentCulture );
            }
        }

        /// <summary>
        /// Gets the scene height (in tile-space) which has been selected by the user in the <see cref="NewSceneDialog"/>.
        /// </summary>
        public int SelectedHeight
        {
            get 
            {
                return int.Parse( this.textBox_Height.Text, CultureInfo.CurrentCulture );
            }
        }

        /// <summary>
        /// Gets the number of initial floors which has been selected by the user in the <see cref="NewSceneDialog"/>.
        /// </summary>
        public int SelectedFloorCount
        {
            get
            {
                return int.Parse( this.textBox_FloorCount.Text, CultureInfo.CurrentCulture );
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Gets called when the user presses on the 'OK-Button'.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contain the event data.</param>
        private void OnOkButtonClicked( object sender, RoutedEventArgs e )
        {
            // Validate user input:
            if( textBox_Name.Text.Length == 0 || 
                textBox_Width.Text.Length == 0 || 
                textBox_Height.Text.Length == 0 || 
                textBox_FloorCount.Text.Length == 0 )
            {                
                MessageBox.Show(
                    Properties.Resources.Info_AllFieldsRequired,
                    Atom.ErrorStrings.Information,
                    MessageBoxButton.OK,
                    MessageBoxImage.None 
                );

                return;
            }

            int width = this.SelectedWidth;
            if( width < MinimumWidth )
            {
                string message = string.Format(
                    CultureInfo.CurrentCulture,
                    Properties.Resources.Info_WidthMustBeGreaterThanX,
                    (MinimumHeight - 1).ToString( CultureInfo.CurrentCulture )
                );

                MessageBox.Show( message, Atom.ErrorStrings.Information, MessageBoxButton.OK, MessageBoxImage.None );
                return;
            }

            int height = this.SelectedHeight;
            if( height < MinimumHeight )
            {
                string message = string.Format(
                    CultureInfo.CurrentCulture,
                    Properties.Resources.Info_HeightMustBeGreaterThanX,
                    (MinimumHeight - 1).ToString( CultureInfo.CurrentCulture )
                );

                MessageBox.Show( message, Atom.ErrorStrings.Information, MessageBoxButton.OK, MessageBoxImage.None );
                return;
            }

            this.DialogResult = true;
        }

        /// <summary>
        /// Gets called when the user presses on the 'Cancel-Button'.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contain the event data.</param>
        private void OnCancelButtonClicked( object sender, RoutedEventArgs e )
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// Moves focus from the control that invoked the event
        /// to the next control if the user pressed Enter.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The KeyEventArgs that contain the event data.</param>
        private void OnTextBoxKeyDown( object sender, System.Windows.Input.KeyEventArgs e )
        {
            if( e.Key == System.Windows.Input.Key.Enter )
            {
                var control = (System.Windows.Controls.Control)sender;
                control.MoveFocus( new TraversalRequest( FocusNavigationDirection.Next ) );
            }
        }

        #endregion
    }
}