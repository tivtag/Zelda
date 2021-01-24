// <copyright file="QuickActionSlot.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.QuickActions.QuickActionSlot class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.QuickActions
{
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Represents a single entry in a <see cref="QuickActionSlotList"/>.
    /// </summary>
    public sealed class QuickActionSlot
    {
        /// <summary>
        /// Gets or sets the <see cref="IQuickAction"/> this QuickActionSlot is
        /// currently associated with.
        /// </summary>
        public IQuickAction Action 
        { 
            get; 
            set; 
        }

        public Keys Key
        {
            get
            {
                return key;
            }
        }

        public bool TopRow
        {
            get
            {
                return topRow;
            }
        }

        internal QuickActionSlot( Keys key, bool topRow )
        {
            this.key = key;
            this.topRow = topRow;
        }

        private readonly Keys key;
        private readonly bool topRow;
    }
}
