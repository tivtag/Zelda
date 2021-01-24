// <copyright file="XnaApp.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.QuestCreator.XnaApp class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.QuestCreator
{
    using Atom.Xna.Wpf;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Defines the class that handles all Xna-related logic.
    /// </summary>
    internal sealed class XnaApp : HiddenXnaGame
    {
        public XnaApp()
        {
            this.Graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }
    }
}
