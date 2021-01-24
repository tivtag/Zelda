// <copyright file="IAnimatedDrawDataAndStrategy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.IAnimatedDrawDataAndStrategy interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Entities.Drawing
{
    using Atom.Xna;

    /// <summary>
    /// Represents an IDrawDataAndStrategy that uses SpriteAnimation(s) for
    /// drawing.
    /// </summary>
    public interface IAnimatedDrawDataAndStrategy : IDrawDataAndStrategy
    {
        /// <summary>
        /// Gets the currently shown SpriteAnimation.
        /// </summary>
        SpriteAnimation CurrentAnimation
        {
            get;
        }
    }
}
