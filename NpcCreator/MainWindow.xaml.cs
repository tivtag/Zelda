// <copyright file="MainWindow.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.NpcCreator.MainWindow class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.NpcCreator
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Forms;
    using Zelda.Entities;

    /// <summary>
    /// Defines the main window of the Npc Creator tool.
    /// </summary>
    /// <remarks>
    /// The Npc Creator allows the creation and edition of <see cref="Zelda.Entities.ZeldaEntity"/>s.
    /// </remarks>
    public sealed partial class MainWindow : Window
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            InitializeCustomComponents();

            // Startup Xna:
            this.xnaThread = new System.Threading.Thread( RunXna );
            this.xnaThread.Priority = System.Threading.ThreadPriority.Lowest;
            this.xnaThread.Start();

            // Setup events:
            this.Closing += new System.ComponentModel.CancelEventHandler( OnMainWindowClosing );
        }

        /// <summary>
        /// Initializes custom UI components of the NPC Creator tool.
        /// </summary>
        private void InitializeCustomComponents()
        {
            var columnName = new DataGridViewTextBoxColumn() {
                Name         = "ItemName",
                HeaderText   = Properties.Resources.DataGridColumn_ItemName,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            var columnDropChance = new DataGridViewTextBoxColumn() {
                Name       = "ChanceToDropColumn",
                HeaderText = Properties.Resources.DataGridColumn_ChanceToDrop,
                Width      = 130,
                ValueType  = typeof( int )
            };

            var columnIsMagicFindAffected = new DataGridViewCheckBoxColumn() {
                Name       = "MagicFindAffectedColumn",
                HeaderText = Properties.Resources.DataGridColumn_MagicFindWorks,
                Width      = 60
            };

            var columnDropChanceInPercent = new DataGridViewTextBoxColumn() {
                Name       = "ChanceToDropInPercentColumn",
                HeaderText = Properties.Resources.DataGridColumn_ChanceToDropInPercent,
                ReadOnly   = true,
                ValueType  = typeof( float )
            };

            this.dataGridLoot.Columns.AddRange(
                new DataGridViewColumn[] {
                    columnName,
                    columnDropChance,
                    columnIsMagicFindAffected,
                    columnDropChanceInPercent
                }
            );

            this.dataGridLoot.CellValueChanged += new DataGridViewCellEventHandler( OnLootDataGridCellValueChanged );
        }       

        /// <summary>
        /// Starts up and initializes the XNA game loop.
        /// </summary>
        /// <param name="obj">The handle of the control xna should draw into.</param>
        private void RunXna()
        {
            this.xnaApp = new XnaApp( App.Current );

            App.Current.XnaApp = this.xnaApp;
            App.Current.LoadDefaults();

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

        #region [ Properties ]

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

        /// <summary>
        /// Gets the data grid that holds the loot information about the current NPC.
        /// </summary>
        public DataGridView DataGridLoot
        {
            get
            {
                return this.dataGridLoot;
            }
        }

        #endregion

        #region [ Methods ]
        
        /// <summary>
        /// Opens the Npc definition file at the given path.
        /// </summary>
        /// <param name="fileName">
        /// The name and path of the file to open.
        /// </param>
        private void Open( string fileName )
        {
            // Read object
            var entity = this.Load( fileName );

            // Find wrapper
            var wrapperFactory = App.Current.PropertyWrapperFactory;
            var wrapper        = (INpcPropertyWrapper)wrapperFactory.ReceiveWrapper( entity );

            this.SetCurrent( wrapper );
        }

        /// <summary>
        /// Loads the ZeldaEntity from the file with the given fileName.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file to load.
        /// </param>
        /// <returns>
        /// The loaded ZeldaEntity.
        /// </returns>
        private ZeldaEntity Load( string fileName )
        {
            using( BinaryReader reader = new BinaryReader( File.OpenRead( fileName ) ) )
            {
                // Receive type
                string typeName = reader.ReadString();
                Type   type     = Type.GetType( typeName );

                // Receive object-reader
                var entityReader = this.ServiceProvider.EntityReaderWriterManager.Get( type );

                // Read object
                var entity = entityReader.Create( string.Empty );

                var context = new Zelda.Saving.DeserializationContext( reader, this.ServiceProvider );
                entityReader.Deserialize( entity, context );

                return entity;
            }
        }

        /// <summary>
        /// Sets the currently active object.
        /// </summary>
        /// <param name="wrapper">
        /// The wrapper around the object.
        /// </param>
        private void SetCurrent( INpcPropertyWrapper wrapper )
        {
            propertyGrid.SelectedObject = wrapper;

            if( wrapper.HasLoot )
            {
                this.tabItem_Loot.Visibility = Visibility.Visible;
            }
            else
            {
                this.tabItem_Loot.Visibility = Visibility.Collapsed;
            }

            wrapper.SetupView( this );
        }

        /// <summary>
        /// Refreshes the "Chance to Find in %" column of the loot data grid.
        /// </summary>
        private void RefreshLootChanceToFindInPercent()
        {
            var culture       = System.Globalization.CultureInfo.CurrentCulture;
            float totalChance = 0.0f;

            foreach( DataGridViewRow row in dataGridLoot.Rows )
            {
                if( row.Cells[1].Value != null )
                {
                    totalChance += (int)row.Cells[1].Value;
                }
            }

            foreach( DataGridViewRow row in dataGridLoot.Rows )
            {
                if( row.Cells[1].Value != null )
                {
                    int chance = (int)row.Cells[1].Value;

                    if( totalChance != 0 )
                    {
                        row.Cells[3].Value = (chance / totalChance) * 100.0f;
                    }
                    else
                    {
                        row.Cells[3].Value = 0.0f;
                    }
                }
            }
        }
        
        /// <summary>
        /// Called when a cell value in the Loot DataGrid has changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The DataGridViewCellEventArgs that contains the event data.</param>
        private void OnLootDataGridCellValueChanged( object sender, DataGridViewCellEventArgs e )
        {
            // If "chance to find"-value has changed:
            if( e.ColumnIndex == 1 )
            {
                this.RefreshLootChanceToFindInPercent();
            }
        }

        /// <summary>
        /// Gets called when the user clicks on the "New Enemy" MenuItem.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void MenuItem_NewEnemy_Click( object sender, RoutedEventArgs e )
        {
            var enemy = new Zelda.Entities.Enemy();

            enemy.Setup( this.ServiceProvider );
            enemy.Collision.Size = new Atom.Math.Vector2( 16.0f, 16.0f );
            enemy.MeleeAttack.DamageMethod = new Zelda.Attacks.Melee.DefaultMeleeDamageMethod();
            enemy.MeleeAttack.DamageMethod.Setup( this.ServiceProvider );

            var wrapper = new EnemyPropertyWrapper( this.ServiceProvider ) {
                WrappedObject = enemy
            };

            this.SetCurrent( wrapper );
        }

        /// <summary>
        /// Gets called when the user clicks on the "New Plant" MenuItem.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void MenuItem_NewPlant_Click( object sender, RoutedEventArgs e )
        {
            var plant = new Zelda.Entities.Plant();

            plant.Setup( this.ServiceProvider );
            plant.Collision.Size = new Atom.Math.Vector2( 16.0f, 16.0f );

            var wrapper = new PlantPropertyWrapper( this.ServiceProvider ) {
                WrappedObject = plant
            };

            this.SetCurrent( wrapper );
        }

        /// <summary>
        /// Gets called when the user clicks on the "New Npc" MenuItem.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void MenuItem_NewNpc_Click( object sender, RoutedEventArgs e )
        {
            var npc = new Zelda.Entities.FriendlyNpc();

            npc.Collision.Size = new Atom.Math.Vector2( 16.0f, 16.0f );

            var wrapper = new FriendlyNpcPropertyWrapper( this.ServiceProvider ) {
                WrappedObject = npc
            };

            this.SetCurrent( wrapper );
        }

        /// <summary>
        /// Gets called when the user clicks on the "New Merchant" MenuItem.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void MenuItem_NewMerchant_Click( object sender, RoutedEventArgs e )
        {
            var merchant = new Zelda.Trading.Merchant();
            merchant.Collision.Size = new Atom.Math.Vector2( 16.0f, 16.0f );

            var wrapper = new MerchantPropertyWrapper( this.ServiceProvider )
            {
                WrappedObject = merchant
            };

            this.SetCurrent( wrapper );
        }

        /// <summary>
        /// Gets called when the user clicks on the "Open" MenuItem.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void MenuItem_Open_Click( object sender, RoutedEventArgs e )
        {
            var dialog = new Microsoft.Win32.OpenFileDialog() {
                Title = Properties.Resources.DialogTitle_SelectNpcFile,
                InitialDirectory = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"Content\Objects\" ),
                RestoreDirectory = true,
                CheckPathExists  = true,
                CheckFileExists  = true
            };

            Directory.CreateDirectory( dialog.InitialDirectory );

            if( dialog.ShowDialog() == true )
            {
                try
                {
                    this.Open( dialog.FileName );                    
                }
                catch( Exception exc )
                {
                   System.Windows.MessageBox.Show(
                       exc.ToString(), 
                       Atom.ErrorStrings.Error, 
                       MessageBoxButton.OK, 
                       MessageBoxImage.Error
                   );
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
            var wrapper = propertyGrid.SelectedObject as INpcPropertyWrapper;
            if( wrapper == null )
                return;

            // Apply
            wrapper.ApplyData( this );

            // Ensure
            if( !wrapper.Ensure() )
                return;

            this.Save( (ZeldaEntity)wrapper.WrappedObject );
            System.Windows.MessageBox.Show( Properties.Resources.Info_SaveSuccessful );
        }

        /// <summary>
        /// Saves the given ZeldaEntity.
        /// </summary>
        /// <param name="entity">
        /// The ZeldaEntity to save.
        /// </param>
        private void Save( ZeldaEntity entity )
        {
            var readerWriter = this.ServiceProvider.EntityReaderWriterManager.Get( entity.GetType() );

            // Find path
            string basePath = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Content/Objects/" );
            Directory.CreateDirectory( basePath );

            string fileName = entity.Name;
            string path     = System.IO.Path.Combine( basePath, fileName );

            // Save
            using( var writer = new BinaryWriter( 
                    new FileStream( path, FileMode.Create, FileAccess.Write, FileShare.None ) ) 
                 )
            {
                var context = new Zelda.Saving.SerializationContext( writer, this.ServiceProvider );

                context.Write( Atom.ReflectionExtensions.GetTypeName( entity.GetType() ) );
                readerWriter.Serialize( entity, context );
            }
        }

        /// <summary>
        /// Gets called when the user clicks on the "Tools -> Resave All" MenuItem.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void MenuItem_Tools_ResaveAll_Click( object sender, RoutedEventArgs e )
        {
            var files = System.IO.Directory.GetFiles( "Content/Objects/" );

            foreach( var filePath in files )
            {
                var entity = this.Load( filePath );
                this.Save( entity );
            }

            // Notify user.
            System.Windows.MessageBox.Show(
                Properties.Resources.Info_ResavedAllEntities,
                string.Empty,
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        /// <summary>
        /// Gets called when the user clicks on the "Exit" MenuItem.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        private void MenuItem_Exit_Click( object sender, RoutedEventArgs e )
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

        #endregion
    }
}
