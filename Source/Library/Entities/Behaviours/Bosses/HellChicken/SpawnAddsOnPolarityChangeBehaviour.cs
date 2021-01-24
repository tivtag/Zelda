// <copyright file="SpawnAddsOnPolarityChangeBehaviour.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Behaviours.Bosses.HellChicken.SpawnAddsOnPolarityChangeBehaviour class.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Behaviours.Bosses.HellChicken
{
    using System;
    using System.Linq;
    using Atom.Math;
    using Zelda.Entities.Spawning;

    /// <summary>
    /// Defines the behaviour that spawns additional adds when the Chicken Hell
    /// boss changes polarity.
    /// </summary>
    internal sealed class SpawnAddsOnPolarityChangeBehaviour
    {
        /// <summary>
        /// The template name of the spider adds to spawn.
        /// </summary>
        private const string SpiderTemplateName = "Spider_Small_45";

        /// <summary>
        /// States how many adds might spawn when the polarity changes.
        /// </summary>
        private static readonly IntegerRange AddsToSpawn = new IntegerRange( 1, 4 );

        /// <summary>
        /// Initializes a new instance of the SpawnAddsOnPolarityChangeBehaviour class.
        /// </summary>
        /// <param name="polarityBehaviour">
        /// The PolarityBehaviour to hook onto.
        /// </param>
        /// <param name="boss">
        /// The boss that is controlled by this SpawnAddsOnPolarityChangeBehaviour.
        /// </param>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related servies.
        /// </param>
        public SpawnAddsOnPolarityChangeBehaviour( PolarityBehaviour polarityBehaviour, Enemy boss, IZeldaServiceProvider serviceProvider )
        {
            this.boss = boss;
            this.rand = serviceProvider.Rand;
            this.spiderTemplate = serviceProvider.EntityTemplateManager.GetEntity( SpiderTemplateName );

            // Hook events.
            polarityBehaviour.PolarityChanged += this.OnPolarityChanged;
        }

        /// <summary>
        /// Despawns all adds that have been spawned by the boss.
        /// </summary>
        internal void DespawnAll()
        {
            var scene = this.boss.Scene;
            if( scene == null )
                return;

            var adds = scene.Entities.Where(
                entity => entity is Enemy && entity != this.boss
            ).ToList();

            foreach( var add in adds )
            {
                Zelda.Entities.Spawning.SpawnHelper.Despawn( add );
            }
        }

        /// <summary>
        /// Called when the polarity of the boss has changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The EventArgs that contain the event data.</param>
        private void OnPolarityChanged( object sender, EventArgs e )
        {
            int addsToSpawn = AddsToSpawn.GetRandomValue( this.rand );

            for( int i = 0; i < addsToSpawn; ++i )
            {
                this.SpawnAdd( this.spiderTemplate );
            }
        }

        /// <summary>
        /// Spawns an add under the boss using the specified EntityTemplate.
        /// </summary>
        /// <param name="template">
        /// The template of the add to spawn.
        /// </param>
        private void SpawnAdd( ZeldaEntity template )
        {
            var scene = this.boss.Scene;
            if( scene == null )
                return;

            var add = template.Clone();
            add.Transform.Position = this.boss.Collision.Center;

            SpawnHelper.AddBlendInColorTint( add );
            scene.Add( add );
        }

        /// <summary>
        /// The boss that is controlled by this SpawnAddsOnPolarityChangeBehaviour.
        /// </summary>
        private readonly Enemy boss;

        /// <summary>
        /// The template that is used to create spider adds.
        /// </summary>
        private readonly ZeldaEntity spiderTemplate;

        /// <summary>
        /// A random number generator.
        /// </summary>
        private readonly RandMT rand;
    }
}
