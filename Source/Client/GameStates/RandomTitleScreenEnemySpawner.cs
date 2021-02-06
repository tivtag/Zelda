// <copyright file="RandomTitleScreenEnemySpawner.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.GameStates.RandomTitleScreenEnemySpawner class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.GameStates
{
    using Zelda.Entities;
    using Zelda.Entities.Components;
    using Zelda.UI;

    /// <summary>
    /// Implements the spawning of random enemies in the title and character screens.
    /// </summary>
    internal sealed class RandomTitleScreenEnemySpawner
    {
        public void SpawnRandomEnemy( ZeldaGame game, ZeldaUserInterface userInterface, ZeldaScene scene )
        {
            Atom.Collections.Hat<string> templateHat = new Atom.Collections.Hat<string>( game.Rand );
            templateHat.Insert( "Skeleton", 15.0f );
            templateHat.Insert( "Spider_Small_45", 5.0f );
            templateHat.Insert( "Skeleton_Red", 4.0f );
            templateHat.Insert( "SkeletonHead", 4.0f );
            templateHat.Insert( "Ghost", 3.0f );
            templateHat.Insert( "Boss_RudrasEye", 0.25f );

            ZeldaEntity entity = game.EntityTemplateManager
                .GetTemplate( templateHat.Get() )
                .CreateInstance();

            entity.Transform.Position = scene.Camera.Scroll + userInterface.MousePosition - (entity.Collision.Size / 2);

            Moveable moveable = entity.Components.Get<Moveable>();

            if( moveable.IsWalkableAt( entity.Transform.Position, scene.Map.Floors[0].ActionLayer ) &&
                moveable.IsWalkableAt( entity.Transform.Position + (entity.Collision.Size / 2), scene.Map.Floors[0].ActionLayer ) )
            {
                scene.Add( entity );
            }
        }
    }
}
