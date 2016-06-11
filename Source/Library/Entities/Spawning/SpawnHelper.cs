// <copyright file="SpawnHelper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the static Zelda.Entities.Spawning.SpawnHelper utility class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Spawning
{
    using Zelda.Entities.Drawing;

    /// <summary>
    /// Contains static spawning-related utility methods.
    /// </summary>
    public static class SpawnHelper
    {
        /// <summary>
        /// Adds an <see cref="Zelda.Graphics.Tinting.BlendInColorTint"/> to the specified entity
        /// if it uses a <see cref="ITintedDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="entity">
        /// The entity that should be blended-in.
        /// </param>
        public static void AddBlendInColorTint( ZeldaEntity entity )
        {
            var tintedDrawData = entity.DrawDataAndStrategy as ITintedDrawDataAndStrategy;

            if( tintedDrawData != null )
            {
                var blendInTint = new Zelda.Graphics.Tinting.BlendInColorTint() {
                    TotalTime = SpawnConstants.SpawnFadeInTime
                };

                blendInTint.ReachedFullEffect += ( sender, e ) => {
                    tintedDrawData.TintList.Remove( blendInTint );
                };

                tintedDrawData.TintList.Add( blendInTint );
            }
        }

        /// <summary>
        /// Despawns the given ZeldaEntity.
        /// </summary>
        /// <param name="entity">
        /// The entity to despawn.
        /// </param>
        /// <param name="fadeOutTime">
        /// The time it takes for the entity to fade-out before it completly gets despawned.
        /// </param>
        public static void Despawn( ZeldaEntity entity, float fadeOutTime = SpawnConstants.DespawnFadeOutTime )
        {
            var tintedDrawData = entity.DrawDataAndStrategy as ITintedDrawDataAndStrategy;

            if( tintedDrawData != null && fadeOutTime > 0.0f )
            {
                // Blend the enemy out and then remove it.
                var blendOutTint = new Zelda.Graphics.Tinting.BlendOutColorTint()  {
                    TotalTime = fadeOutTime
                };

                blendOutTint.ReachedFullEffect += ( sender, e ) => {
                    tintedDrawData.TintList.Remove( blendOutTint );

                    if( entity.Scene != null )
                    {
                        entity.RemoveFromScene();
                    }
                };

                tintedDrawData.TintList.Add( blendOutTint );
            }
            else
            {
                // Remove the enemy Deferredly.
                if( entity.Scene != null )
                {
                    entity.RemoveFromScene();
                }
            }
        }

        /// <summary>
        /// Spawns entities of type [templateName] at the spawn point [spawnPointName] [count]-times in the given [scene].
        /// </summary>
        /// <param name="templateName">
        /// The name of the entity template.
        /// </param>
        /// <param name="count">
        /// The number of times the entity should be spawned.
        /// </param>
        /// <param name="spawnPoint">
        /// The spawn point at which the entities will be spawned.
        /// </param>
        /// <param name="entityTemplateManager">
        /// Creates the template entity for which the spawned entities will be based on.
        /// </param>
        public static void Spawn( string templateName, int count, ISpawnPoint spawnPoint, EntityTemplateManager entityTemplateManager )
        {
            if( spawnPoint == null )
                return;

            IEntityTemplate template = entityTemplateManager.GetTemplate( templateName );
            if( template == null )
                return;

            for( int i = 0; i < count; ++i )
            {
                ZeldaEntity entity = template.CreateInstance();

                AddBlendInColorTint( entity );
                spawnPoint.Spawn( entity );
            }
        }
    }
}
