// <copyright file="ProjectileSpritesHelper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the static Zelda.Entities.Projectiles.Drawing.ProjectileSpritesHelper class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Entities.Projectiles.Drawing
{
    using Atom.Xna;
    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// Static helper class that provides a mechanism to load an appropriate <see cref="IProjectileSprites"/>.
    /// </summary>
    public static class ProjectileSpritesHelper
    {
        /// <summary>
        /// Tries to load the appropriate <see cref="IProjectileSprites"/> for the given <paramref name="spriteGroup"/>.
        /// </summary>
        /// <param name="spriteGroup">
        /// The sprite group to load.
        /// </param>
        /// <param name="spriteLoader">
        /// Provides a mechanism to load ISprites.
        /// </param>
        /// <returns>
        /// The loaded IProjectileSprites.
        /// </returns>
        public static IProjectileSprites Load( string spriteGroup, ISpriteLoader spriteLoader )
        {
            if( string.IsNullOrEmpty( spriteGroup ) )
            {
                return null;
            }
            else
            {
                try
                {
                    return TryLoadAsSingleAnimatedProjectileSprites( spriteGroup, spriteLoader );
                }
                catch( System.IO.FileNotFoundException )
                {
                    try
                    {
                        return TryLoadAsAnimatedProjectileSprites( spriteGroup, spriteLoader );
                    }
                    catch( System.IO.FileNotFoundException )
                    {
                        return TryLoadAsProjectilesSprites( spriteGroup, spriteLoader );
                    }
                }
            }
        }

        /// <summary>
        /// Tries to load the <see cref="ProjectileSprites"/> as <see cref="SingleAnimatedProjectileSprites"/>
        /// using the given <paramref name="spriteGroup"/>.
        /// </summary>
        /// <param name="spriteGroup">
        /// The sprite group to load.
        /// </param>
        /// <param name="spriteLoader">
        /// Provides a mechanism to load ISprites.
        /// </param>
        /// <returns>
        /// The loaded IProjectileSprites.
        /// </returns>
        private static IProjectileSprites TryLoadAsSingleAnimatedProjectileSprites( string spriteGroup, ISpriteLoader spriteLoader )
        {
            var spriteTemplate = spriteLoader.LoadAnimatedSprite( spriteGroup );
            return new SingleAnimatedProjectileSprites( spriteTemplate );
        }

        /// <summary>
        /// Tries to load the <see cref="ProjectileSprites"/> as <see cref="AnimatedProjectileSprites"/>
        /// using the given <paramref name="spriteGroup"/>.
        /// </summary>
        /// <param name="spriteGroup">
        /// The sprite group to load.
        /// </param>
        /// <param name="spriteLoader">
        /// Provides a mechanism to load ISprites.
        /// </param>
        /// <returns>
        /// The loaded IProjectileSprites.
        /// </returns>
        private static IProjectileSprites TryLoadAsAnimatedProjectileSprites( string spriteGroup, ISpriteLoader spriteLoader )
        {
            var spriteTemplateLeft = spriteLoader.LoadAnimatedSprite( spriteGroup + "_Left" );
            var spriteTemplateRight = spriteLoader.LoadAnimatedSprite( spriteGroup + "_Right" );
            var spriteTemplateUp = spriteLoader.LoadAnimatedSprite( spriteGroup + "_Up" );
            var spriteTemplateDown = spriteLoader.LoadAnimatedSprite( spriteGroup + "_Down" );

            return new AnimatedProjectileSprites(
                spriteTemplateLeft,
                spriteTemplateRight,
                spriteTemplateUp,
                spriteTemplateDown
            );
        }

        /// <summary>
        /// Tries to load the <see cref="ProjectileSprites"/> as <see cref="ProjectileSprites"/>
        /// using the given <paramref name="spriteGroup"/>.
        /// </summary>
        /// <param name="spriteGroup">
        /// The sprite group to load.
        /// </param>
        /// <param name="spriteLoader">
        /// Provides a mechanism to load ISprites.
        /// </param>
        /// <returns>
        /// The loaded IProjectileSprites.
        /// </returns>
        private static IProjectileSprites TryLoadAsProjectilesSprites( string spriteGroup, ISpriteLoader spriteLoader )
        {
            var spriteLeft = spriteLoader.LoadSprite( spriteGroup + "_Left" );
            var spriteRight = spriteLoader.LoadSprite( spriteGroup + "_Right" );
            var spriteUp = spriteLoader.LoadSprite( spriteGroup + "_Up" );
            var spriteDown = spriteLoader.LoadSprite( spriteGroup + "_Down" );

            return new ProjectileSprites(
                spriteLeft,
                spriteRight,
                spriteUp,
                spriteDown
            );
        }
    }
}
