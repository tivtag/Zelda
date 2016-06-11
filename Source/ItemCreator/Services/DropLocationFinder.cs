
namespace Zelda.ItemCreator.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Zelda.Entities;
    using Zelda.Items;

    public class DropLocationFinder
    {
        public DropLocationFinder( IZeldaServiceProvider serviceProvider )
        {
            if( serviceProvider == null )
                throw new ArgumentNullException( "serviceProvider" );

            this.serviceProvider = serviceProvider;
        }

        public string FindLocations( Item item )
        {
            if( item == null )
                throw new ArgumentNullException( "item" );

            var sb = new StringBuilder();

            sb.AppendLine( "Dropped by:" );

            foreach( var dropEntry in this.GetAllEnemiesThatDrop( item ) )
            {
                string enemyName = dropEntry.Item1.Name;
                string dropChance = dropEntry.Item2.ToString();

                sb.AppendFormat( "    {0} - {1}%\n", enemyName, dropChance );
            }
            
            var locations = GetAllMapLocationsOf( item )
                .ToArray();

            if( locations.Length > 0 )
            {
                sb.AppendLine( "\nFound in:" );
                
                foreach( var location in locations )
                {
                    sb.Append( location );
                }
            }

            return sb.ToString();
        }

        private IEnumerable<Tuple<Enemy, double>> GetAllEnemiesThatDrop( Item item )
        {
            return
                from enemy in this.GetAllEnemies()
                let loot = enemy.Lootable.Loot
                let relevantEntries = loot.Where( hatEntry => hatEntry.Data == item.Name )
                where relevantEntries.Count() > 0
                    from entry in relevantEntries
                let dropChance = Math.Round( (entry.Weight / loot.TotalWeight) * 100.0f, 2 )
                select Tuple.Create( enemy, dropChance );
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
            return from name in this.GetAllObjectNames()
                    let enemy = this.serviceProvider.EntityTemplateManager.LoadEntity( name ) as Enemy
                    where enemy != null
                    select enemy;


            //if( this.enemies == null )
            //{
            //    this.enemies =
            //        ().ToArray();      
            //}

            //return this.enemies;
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
