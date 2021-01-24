// <copyright file="IZeldaServiceProvider.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.IZeldaServiceProvider interface.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda
{
    using System;
    using Atom.Math;
    using Atom.Xna;
    using Atom.Xna.Fonts;

    /// <summary>
    /// Provides fast access to game related services.
    /// </summary>
    /// <remarks>
    /// The implementor of this interface usually provides access (but slower) to 
    /// additional objects by implementing <see cref="System.IServiceProvider.GetService"/>.
    /// </remarks>
    public interface IZeldaServiceProvider : IServiceProvider, 
        IContentManagerProvider, 
        Atom.Diagnostics.ILogProvider,
        Atom.IObjectProviderContainer
    {
        /// <summary>
        /// Gets the <see cref="ITexture2DLoader"/> object that provides a mechanism for loading Texture2D assets.
        /// </summary>
        ITexture2DLoader TextureLoader { get; }

        /// <summary>
        /// Gets the <see cref="ISpriteLoader"/> object that provides a mechanism for loading Sprite and AnimatedSprite assets.
        /// </summary>
        ISpriteLoader SpriteLoader { get; }

        /// <summary>
        /// Gets the <see cref="ISpriteSheetLoader"/> object that provides a mechanism for loading ISpriteSheet assets.
        /// </summary>
        ISpriteSheetLoader SpriteSheetLoader { get; }

        /// <summary>
        /// Gets a random number generator.
        /// </summary>
        RandMT Rand { get; }

        /// <summary>
        /// Gets the size of the (unscaled) game window.
        /// </summary>
        Point2 ViewSize { get; }

        /// <summary>
        /// Gets the Xna game object of the current application.
        /// </summary>
        Microsoft.Xna.Framework.Game Game { get; }

        /// <summary>
        /// Gets the ObjectReaderWriterManager object.
        /// </summary>
        Zelda.Entities.EntityReaderWriterManager EntityReaderWriterManager { get; }

        /// <summary>
        /// Gets the Zelda.Entities.EntityTemplateManager object.
        /// </summary>
        Zelda.Entities.EntityTemplateManager EntityTemplateManager { get; }    

        /// <summary>
        /// Gets the <see cref="Zelda.Entities.Behaviours.BehaviourManager"/> object.
        /// </summary>
        Zelda.Entities.Behaviours.BehaviourManager BehaviourManager { get; }
        
        /// <summary>
        /// Gets the <see cref="Zelda.Entities.Drawing.DrawStrategyManager"/> object.
        /// </summary>
        Zelda.Entities.Drawing.DrawStrategyManager DrawStrategyManager { get; }

        /// <summary>
        /// Gets the <see cref="Zelda.Items.ItemManager"/> object.
        /// </summary>
        Zelda.Items.ItemManager ItemManager { get; }

        /// <summary>
        /// Gets the <see cref="Zelda.Audio.ZeldaAudioSystem"/> object.
        /// </summary>
        Zelda.Audio.ZeldaAudioSystem AudioSystem { get; }
    }
}
