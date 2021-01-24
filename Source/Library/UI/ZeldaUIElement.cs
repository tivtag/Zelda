// <copyright file="ZeldaUIElement.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.UI.ZeldaUIElement class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.UI
{
    using Atom.Xna.UI;

    /// <summary>
    /// Represents an <see cref="UIElement"/> that is part of a <see cref="ZeldaUserInterface"/>.
    /// </summary>
    /// <remarks>
    /// There is no ground-breaking difference in inheriting
    /// from <see cref="UIElement"/> or <see cref="ZeldaUIElement"/>.
    /// Both will work just fine.
    /// </remarks>
    public abstract class ZeldaUIElement : UIElement
    {
        /// <summary>
        /// Gets the <see cref="ZeldaUserInterface"/> that owns this <see cref="ZeldaUIElement"/>.
        /// </summary>
        public new ZeldaUserInterface Owner
        {
            get
            { 
                return base.Owner as ZeldaUserInterface;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeldaUIElement"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the new <see cref="ZeldaUIElement"/>.
        /// </param> 
        protected ZeldaUIElement( string name )
            : base( name )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZeldaUIElement"/> class.
        /// </summary>
        protected ZeldaUIElement()
            : base()
        {
        }
    }
}
