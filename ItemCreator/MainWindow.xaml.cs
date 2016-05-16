using Zelda.ItemCreator.Services;
// <copyright file="MainWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.ItemCreator.MainWindow class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.ItemCreator
{
    using System;
    using System.Windows;
    using Zelda.Items;
    using Zelda.Items.Sets;
    using Zelda.Saving;
    using System.Linq;
    using System.IO;

    /// <summary>
    /// Represents the main window of the Item Creator tool.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        /// <summary>
        /// Gets or sets the IItemViewModel that is currently beeing edited.
        /// </summary>
        public IObjectViewModel Current
        {
            get 
            { 
                return propertyGrid.SelectedObject as IObjectViewModel;
            }

            set 
            { 
                propertyGrid.SelectedObject = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="IZeldaServiceProvider"/> object
        /// which provides fast access to game-related services.
        /// </summary>
        public IZeldaServiceProvider ServiceProvider
        {
            get 
            {
                return App.Current;
            }
        }

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            // Startup Xna:
            this.xnaThread          = new System.Threading.Thread( RunXna );
            this.xnaThread.Priority = System.Threading.ThreadPriority.Lowest;
            this.xnaThread.Start();

            InitializeComponent();
            
            this.dropLocationFinder = new DropLocationFinder( App.Current );

            // Setup events:
            this.Closing += OnMainWindowClosing;
        }

        /// <summary>
        /// Starts up and initializes the XNA game loop.
        /// </summary>
        private void RunXna()
        {
            this.xnaApp = new XnaApp();
            App.Current.XnaApp = this.xnaApp;

            this.xnaApp.Run();
        }

        /// <summary>
        /// Gets called when the MainWindow is closing.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The CancelEventArgs that contains the event data.
        /// </param>
        private void OnMainWindowClosing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            lock( xnaApp )
            {
                this.xnaApp.Exit();
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Opens the .zitm Item of the given fileName.
        /// </summary>
        /// <param name="fileName">
        /// The full path of the file to open.
        /// </param>
        private void Open( string fileName )
        {
            var obj = Load( fileName );
            this.Current = CreateViewModel( obj );
        }

        /// <summary>
        /// Gets an <see cref="IItemViewModel"/> for the given Object.
        /// </summary>
        /// <param name="obj">The item to get a wrapper for.</param>
        /// <returns>The new IObjectPropertyWrapper.</returns>
        private IObjectViewModel CreateViewModel( object obj )
        {
            var type = obj.GetType();
            IObjectViewModel viewModel = null;

            if( type == typeof( Item ) )
                viewModel = new ItemViewModel( this.ServiceProvider );
            else if( type == typeof( Equipment ) )
                viewModel = new EquipmentViewModel( this.ServiceProvider );
            else if( type == typeof( Weapon ) )
                viewModel = new WeaponViewModel( this.ServiceProvider );
            else if( type == typeof( Gem ) )
                viewModel = new GemViewModel( this.ServiceProvider );
            else if( type == typeof( Set ) )
                viewModel = new SetViewModel( this.ServiceProvider );
            else
                throw new NotImplementedException( "Item type not implemented for loading: " + type.ToString() );

            viewModel.WrappedObject = obj;
            return viewModel;
        }

        /// <summary>
        /// Loads the item from the item definition file at the given file path.
        /// </summary>
        /// <param name="fileName">The full file path.</param>
        /// <returns>The item that has been loaden.</returns>
        private object Load( string fileName )
        {
            try
            {
                using( var reader = new System.IO.BinaryReader( System.IO.File.OpenRead( fileName ) ) )
                {
                    var typeName = reader.ReadString();
                    var type     = Type.GetType( typeName );
                    if( type == null )
                        return null;

                    var saveable = (ISaveable)Activator.CreateInstance( type );

                    var context = new Zelda.Saving.DeserializationContext( reader, this.ServiceProvider );
                    saveable.Deserialize( context );

                    return saveable;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets called when the user clicks on the "New Item" MenuItem.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void MenuItem_NewItem_Click( object sender, RoutedEventArgs e )
        {
            var wrapper = new ItemViewModel( this.ServiceProvider );

            wrapper.WrappedObject = new Zelda.Items.Item() {
                Name = "NewItem"
            };

            this.Current = wrapper;
        }

        /// <summary>
        /// Gets called when the user clicks on the "New Equipment" MenuItem.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void MenuItem_NewEquipment_Click( object sender, RoutedEventArgs e )
        {
            var wrapper = new EquipmentViewModel( this.ServiceProvider );

            wrapper.WrappedObject = new Zelda.Items.Equipment() {
                Name = "NewEquipment"
            };

            this.Current = wrapper;
        }

        /// <summary>
        /// Gets called when the user clicks on the "New Weapon" MenuItem.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void MenuItem_NewWeapon_Click( object sender, RoutedEventArgs e )
        {
            var wrapper = new WeaponViewModel( this.ServiceProvider );

            wrapper.WrappedObject = new Zelda.Items.Weapon() {
                Name = "NewWeapon"
            };

            this.Current = wrapper;
        }

        /// <summary>
        /// Gets called when the user clicks on the "New Gem" MenuItem.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void MenuItem_NewGem_Click( object sender, RoutedEventArgs e )
        {
            var wrapper = new GemViewModel( this.ServiceProvider );

            wrapper.WrappedObject = new Zelda.Items.Gem() {
                Name = "NewGem"
            };

            this.Current = wrapper;
        }

        /// <summary>
        /// Gets called when the user clicks on the "New Set" MenuItem.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void MenuItem_NewSet_Click( object sender, RoutedEventArgs e )
        {
            var wrapper = new SetViewModel( this.ServiceProvider );

            wrapper.WrappedObject = new Zelda.Items.Sets.Set() {
                Name = "NewSet"
            };

            this.Current = wrapper;
        }

        /// <summary>
        /// Gets called when the user clicks on the "Open" MenuItem.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void MenuItem_Open_Click( object sender, RoutedEventArgs e )
        {
            var dialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter           = Properties.Resources.Filter_Item,
                InitialDirectory = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"Content\Items\" ),
                RestoreDirectory = true
            };

            System.IO.Directory.CreateDirectory( dialog.InitialDirectory );

            if( dialog.ShowDialog() == true )
            {
                try
                {
                    Open( dialog.FileName );
                }
                catch( Exception exc )
                {
                    MessageBox.Show( exc.ToString(), Atom.ErrorStrings.Error, MessageBoxButton.OK, MessageBoxImage.Error );
                }
            }
        }

        /// <summary>
        /// Gets called when the user clicks on the "Save" MenuItem.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void MenuItem_Save_Click( object sender, RoutedEventArgs e )
        {
            var viewModel = this.Current;
            if( viewModel == null )
                return;

            try
            {
                viewModel.Save();                

                // Notify user.
                string message = string.Format( 
                    System.Globalization.CultureInfo.CurrentCulture,
                    Properties.Resources.Info_ItemXSavedSuccessfully,
                    ((Atom.IReadOnlyNameable)viewModel).Name
                );

                MessageBox.Show(
                    message,
                    string.Empty, 
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch( Exception exc )
            {                    
                // Notify user.
                string message = exc.ToString();

                this.ServiceProvider.Log.WriteLine( message );
                MessageBox.Show(
                    message,
                    Atom.ErrorStrings.Error,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Gets called when the user clicks on the "Resave All" MenuItem of the Tools MenuItem.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void OnMenuItemToolsResaveAllClicked( object sender, RoutedEventArgs e )
        {
            var files = System.IO.Directory.GetFiles( "Content/Items/", "*", System.IO.SearchOption.AllDirectories );

            foreach( var filePath in files )
            {
                var obj = this.Load( filePath );
                var viewModel = this.CreateViewModel( obj );

                viewModel.Save();
            }

            // Notify user.
            MessageBox.Show(
                Properties.Resources.Info_ResavedAllItems,
                string.Empty, 
                MessageBoxButton.OK, 
                MessageBoxImage.Information
            );
        }

        /// <summary>
        /// Gets called when the user clicks on the "List Drop Locations" MenuItem of the Tools MenuItem.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void OnMenuItemToolsListDropLocationsClicked( object sender, RoutedEventArgs e )
        {
            if( this.Current == null )
                return;

            var item = this.Current.WrappedObject as Item;
            if( item == null )
                return;

            // Notify user.
            MessageBox.Show(
                dropLocationFinder.FindLocations( item ),
                item.Name,
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        /// <summary>
        /// Gets called when the user clicks on the "Resave All" MenuItem of the Tools MenuItem.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void OnMenuItemToolsModifyItemCostClicked( object sender, RoutedEventArgs e )
        {
            if( !Atom.Wpf.QuestionMessageBox.Show( "Do you really want to modify the amount of rubies an item is worth?" ) )
                return;

            var files = System.IO.Directory.GetFiles( "Content/Items/", "*", System.IO.SearchOption.AllDirectories );

            foreach( var filePath in files )
            {
                var item = this.Load( filePath ) as Item;

                if( item != null )
                {
                    int currentWorth = item.RubiesWorth;
                    if( currentWorth == 0 )
                        continue;

                    int newWorth = currentWorth;

                    if( currentWorth <= 8 )
                    {
                        newWorth = currentWorth;

                        if( currentWorth >= 5 )
                        {
                            newWorth -= 1;
                        }
                    }
                    else
                    {
                        double factor = 4.0;

                        if( currentWorth <= 15 )
                        {
                            factor = 2.0;
                        }

                        newWorth = (int)Math.Round( (double)currentWorth / factor );
                    }

                    Console.WriteLine( "{0,-40} | {1,10} --> {2}", item.Name, currentWorth, newWorth );
                    item.RubiesWorth = newWorth;

                    var viewModel = this.CreateViewModel( item );
                    viewModel.Save();
                }
            }

            //  Notify user.
            MessageBox.Show(
                Properties.Resources.Info_ResavedAllItems,
                string.Empty, 
                MessageBoxButton.OK, 
                MessageBoxImage.Information
            );
        }

        private void OnMenuItemToolsShowGemsAllClicked( object sender, RoutedEventArgs e )
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            if( dialog.ShowDialog() != true )
                return;

            var groupedGems =
                System.IO.Directory.GetFiles( "Content/Items/", "*", System.IO.SearchOption.TopDirectoryOnly )
                    .Select( fileName => this.Load( fileName ) as Gem )
                    .Where( gem => gem != null )
                    .GroupBy( gem => gem.GemColor )
                    .ToArray();
   
            using( StreamWriter writer = new StreamWriter( dialog.FileName ) )
            {
                foreach( var group in groupedGems )
                {
                    var gems = group
                        .OrderBy( gem => gem.Level )
                        .ToArray();

                    writer.WriteLine( group.Key.ToString() );

                    int lastLevel = -1;

                    foreach( var gem in gems )
                    {
                        if( lastLevel != gem.Level )
                        {
                            lastLevel = gem.Level;
                            writer.WriteLine( gem.Level );
                        }

                        writer.WriteLine( "  {0}", gem.Name );
                    }

                    writer.WriteLine();
                }
            }
        }

        /// <summary>
        /// Gets called when the user clicks on the "Exit" MenuItem.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void OnMenuItemExitClicked( object sender, RoutedEventArgs e )
        {
            App.Current.Shutdown();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The Xna application object.
        /// </summary>
        private XnaApp xnaApp;

        /// <summary>
        /// The thread the XNA logic runs in.
        /// </summary>
        private readonly System.Threading.Thread xnaThread;
        private readonly DropLocationFinder dropLocationFinder;

        #endregion
    }
}
