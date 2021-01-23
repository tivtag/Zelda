
namespace Zelda.ItemCreator.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Zelda.Entities;
    using Zelda.Items;

    /// <summary>
    /// Responsible for finding all locations where an Item drops.
    /// </summary>
    public sealed class DropLocationFinder
    {
        /// <summary>
        /// Initializes a new instance of the DropLocationFinder class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides a mechanism to receive various game related services.
        /// </param>
        public DropLocationFinder( IZeldaServiceProvider serviceProvider )
        {
            if( serviceProvider == null )
            {
                throw new ArgumentNullException( nameof( serviceProvider ) );
            }

            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Returns a string that contains a description of the ingame locations
        /// where the given Item drops.
        /// </summary>
        /// <param name="item">
        /// The Item to search.
        /// </param>
        public string FindLocations( Item item )
        {
            if( item == null )
            {
                throw new ArgumentNullException( nameof( item ) );
            }

            var sb = new StringBuilder();
            sb.AppendLine( "Dropped by:" );

            foreach( (Enemy enemy, double dropChance) in this.GetAllEnemiesThatDrop( item ) )
            {
                string enemyName = enemy.Name;
                string dropChanceText = dropChance.ToString();

                sb.AppendFormat( "    {0} - {1}%\n", enemyName, dropChanceText );
            }

            string[] locations = GetAllMapLocationsOf( item )
                .ToArray();

            if( locations.Length > 0 )
            {
                sb.AppendLine( "\nFound in:" );
                
                foreach( string location in locations )
                {
                    sb.Append( location );
                }
            }

            return sb.ToString();
        }

        private IEnumerable<(Enemy Enemy, double DropChance)> GetAllEnemiesThatDrop( Item item )
        {
            return
                from enemy in this.GetAllEnemies()
                let loot = enemy.Lootable.Loot
                let relevantEntries = loot.Where( hatEntry => hatEntry.Data == item.Name )
                where relevantEntries.Count() > 0
                    from entry in relevantEntries
                let dropChance = Math.Round( (entry.Weight / loot.TotalWeight) * 100.0f, 2 )
                select (enemy, dropChance);
        }

        private IEnumerable<string> GetAllMapLocationsOf( Item item )
        {
            return
                from scene in this.GetAllScenes()
                let items = scene.Entities
                    .Where( e => e is MapItem )
                    .Cast<MapItem>()
                    .Where( m => m.ItemInstance.Item.Name == item.Name )
                    .ToArray()
                    
                where items.Any()
                let desc = items
                    .Aggregate( scene.Name, (a, m) => string.Format( "{0} {1}\r\n", a, m.Transform.Position.ToString() ) )

                select desc;
        }

        private IEnumerable<Enemy> GetAllEnemies()
        {
            return 
                from name in this.GetAllObjectNames()
                let enemy = this.serviceProvider.EntityTemplateManager.LoadEntity( name ) as Enemy
                where enemy != null
                select enemy;
        }

        private IEnumerable<ZeldaScene> GetAllScenes()
        {
            if( this.scenes == null )
            {
                this.scenes = Directory.EnumerateFiles( @"Content\Scenes\" )
                    .Select( fileName => Path.GetFileNameWithoutExtension( fileName ) )
                    .Select( fileName => ZeldaScene.Load( fileName, this.serviceProvider ) )
                    .ToArray();
            }

            return this.scenes;
        }

        private IEnumerable<string> GetAllObjectNames()
        {
            return Directory.EnumerateFiles( @"Content\Objects\" )
                .Select( fileName => Path.GetFileNameWithoutExtension( fileName ) );
        }

        private ZeldaScene[] scenes;
        private readonly IZeldaServiceProvider serviceProvider;
    }
}
