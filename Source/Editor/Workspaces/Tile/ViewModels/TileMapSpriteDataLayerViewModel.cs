// <copyright file="TileMapSpriteDataLayerViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Tile.ViewModels.TileMapSpriteDataLayerViewModel class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Tile.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Input;
    using Atom.Scene.Tiles;
    using Atom.Wpf;
    using Atom.Xna;

    /// <summary>
    /// Defines the ViewModel that defines the bindable properties and commands 
    /// of a <see cref="TileMapSpriteDataLayer"/> to provide them to the View (WPF).
    /// </summary>
    public class TileMapSpriteDataLayerViewModel : TileMapDataLayerViewModel, IViewModel<TileMapSpriteDataLayer>
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="TileMapSpriteDataLayerViewModel"/> class.
        /// </summary>
        /// <param name="layer">
        /// The layer the new ViewModel wraps around.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game services.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="layer"/> or <paramref name="serviceProvider"/> is null.
        /// </exception>
        public TileMapSpriteDataLayerViewModel( TileMapSpriteDataLayer layer, TileMapFloorViewModel floor, IZeldaServiceProvider serviceProvider )
            : base( layer, floor, serviceProvider )
        {
            #region Verify Arguments

            if( layer == null )
                throw new ArgumentNullException( "layer" );

            if( serviceProvider == null )
                throw new ArgumentNullException( "serviceProvider" );

            #endregion

            if( layer.Sheet != null )
                this.sheet = new SpriteSheetViewModel( layer.Sheet );

            // Commands:
            this.SetSheet = new SetSheetCommand( this );
            this.Fill     = new FillCommand( this );
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Receives the model this <see cref="TileMapSpriteDataLayerViewModel"/> wraps around.
        /// </summary>
        public new TileMapSpriteDataLayer Model
        {
            get
            { 
                return (TileMapSpriteDataLayer)base.Model;
            }
        }

        /// <summary>
        /// Receives the <see cref="SpriteSheet"/> of the TileMapSpriteDataLayer.
        /// </summary>
        public SpriteSheetViewModel Sheet
        {
            get 
            { 
                return this.sheet;
            }

            private set
            {
                if( value == this.Sheet )
                    return;

                this.sheet = value;

                if( value != null )
                    this.Model.Sheet = value.Model;
                else
                    this.Model.Sheet = null;

                // We need to notify the Scene
                // that its Sheets have changed. (code smell? >_>)
                var scene = this.Floor.Map.Scene.Model;
                scene.RefreshSpriteSheetsToUpdate();

                this.OnPropertyChanged( "Sheet" );
            }
        }

        /// <summary>
        /// Receives the ICommand that when executed
        /// asks the user to select a SpriteSheet for the TileMapSpriteDataLayer.
        /// </summary>
        public ICommand SetSheet
        {
            get;
            private set;
        }

        /// <summary>
        /// Receives the ICommand that when executed
        /// sets all tiles of the TileMapSpriteDataLayer to the selected Sprite.
        /// </summary>
        public ICommand Fill
        {
            get;
            private set;
        }
        
        #endregion

        #region [ Methods ]

        #endregion

        #region [ Commands ]

        /// <summary>
        /// Defines the ICommand that when executed
        /// sets all tiles of the TileMapSpriteDataLayer to the selected Sprite.
        /// </summary>
        private sealed class FillCommand : ViewModelCommand<TileMapSpriteDataLayerViewModel, TileMapSpriteDataLayer>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FillCommand"/> class.
            /// </summary>
            /// <param name="viewModel">
            /// The ViewModel that owns the new ICommand.
            /// </param>
            public FillCommand( TileMapSpriteDataLayerViewModel viewModel )
                : base( viewModel )
            {
                viewModel.PropertyChanged += (sender, e) => this.OnCanExecuteChanged(); ;
            }

            /// <summary>
            /// Receives a value that indicates whether this ICommand can currently execute.
            /// </summary>
            /// <param name="parameter"></param>
            /// <returns></returns>
            public override bool CanExecute( object parameter )
            {
                return this.ViewModel.Sheet != null;
            }

            /// <summary>
            /// Executes this ICommand.
            /// </summary>
            /// <param name="parameter"></param>
            public override void Execute( object parameter )
            {
                if( !CanExecute( parameter ) )
                    return;

                var sheet = this.ViewModel.Sheet;
                Debug.Assert( sheet != null );

                var sprite = sheet.SelectedSprite;
                if( sprite.Index == TileMap.InvalidTile )
                    return;

                string message = string.Format( CultureInfo.CurrentCulture,
                    Properties.Resources.Question_ReallyFillLayerXOfFloorYWithSpriteZIndexW,
                    this.ViewModel.Name, this.Model.Floor.FloorNumber.ToString( CultureInfo.CurrentCulture ),
                    sprite.Sprite == null ? Properties.Resources.NullSprite : sprite.Sprite.Name,
                    sprite.Index.ToString( CultureInfo.CurrentCulture )
                );

                if( QuestionMessageBox.Show( message ) )
                {
                    this.Model.Fill( sprite.Index );
                }          
            }
        }

        /// <summary>
        /// Defines the ICommand that when executed
        /// asks the user to select a SpriteSheet for the TileMapSpriteDataLayer.
        /// </summary>
        private sealed class SetSheetCommand : ViewModelCommand<TileMapSpriteDataLayerViewModel, TileMapSpriteDataLayer>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SetSheetCommand"/> class.
            /// </summary>
            /// <param name="viewModel">
            /// The ViewModel that owns the new ICommand.
            /// </param>
            public SetSheetCommand( TileMapSpriteDataLayerViewModel viewModel )
                : base( viewModel )
            {
            }

            /// <summary>
            /// Executes this ICommand.
            /// </summary>
            /// <param name="parameter"></param>
            public override void Execute( object parameter )
            {
                var appPath = AppDomain.CurrentDomain.BaseDirectory;

                var dialog = new Microsoft.Win32.OpenFileDialog()
                {
                    Title            = Properties.Resources.DialogTitle_SetSheet,
                    Filter           = Properties.Resources.Filter_SpriteSheet,
                    InitialDirectory = System.IO.Path.Combine( appPath, @"Content\Sheets\" ),
                    RestoreDirectory = true,
                    CheckFileExists  = true,
                    ValidateNames    = true
                };

                System.IO.Directory.CreateDirectory( dialog.InitialDirectory );

                if( dialog.ShowDialog() == true )
                {
                    var loader = this.ViewModel.ServiceProvider.SpriteSheetLoader;

                    try
                    {
                        string asset = System.IO.Path.GetFileNameWithoutExtension( dialog.FileName );   

                        var sheet = loader.LoadSpriteSheet( asset );
                        this.ViewModel.Sheet = new SpriteSheetViewModel( sheet );
                    }
                    catch( Exception exc )
                    {
                        System.Windows.MessageBox.Show( exc.Message, Atom.ErrorStrings.Error,
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error );
                    }
                }
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Intentifies the viewmodel of the SpriteSheet of the TileMapSpriteDataLayer.
        /// </summary>
        private SpriteSheetViewModel sheet;

        #endregion
    }
}
