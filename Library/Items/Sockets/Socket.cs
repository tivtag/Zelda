// <copyright file="Socket.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Socket class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items
{
    using System;
    using System.ComponentModel;
    using Zelda.Status;

    /// <summary>
    /// A socket is a special slot on an <see cref="Equipment"/> into
    /// which <see cref="Gem"/>s can be inserted to provide bonuses.
    /// </summary>
    /// <remarks>
    /// Each socket has a special 'color'; which fit to corresponding gems.
    /// The player receives a bonus if all sockets of an <see cref="Equipment"/>
    /// are socketed with the correct colored Gems.
    /// </remarks>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public sealed class Socket
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the color of this Socket.
        /// </summary>
        public ElementalSchool Color
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the GemInstance that has been inserted into this Socket, if any.
        /// </summary>
        public GemInstance Gem
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this Socket is empty (and as such contains no <see cref="Gem"/>).
        /// </summary>
        public bool IsEmpty
        {
            get 
            { 
                return this.Gem == null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this Socket 
        /// has been socketted correctly.
        /// </summary>
        /// <remarks>
        /// If all sockets of an item have been socketted 'well'
        /// then an additional Socket Bonus is added to the item.
        /// </remarks>
        public bool IsWellSocketted
        {
            get
            {
                if( this.IsEmpty )
                    return false;

                return 
                    this.Color == this.Gem.Color || 
                    this.Color == ElementalSchool.All || 
                    this.Gem.Color == ElementalSchool.All;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="Socket"/> class.
        /// </summary>
        public Socket()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Socket"/> class.
        /// </summary>
        /// <param name="socketColor">The color of the socket.</param>
        public Socket( ElementalSchool socketColor )
        {
            this.Color = socketColor;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Tries to socket the given Gem into this <see cref="Socket"/>.
        /// </summary>
        /// <param name="gem">The gem to socket.</param>
        /// <returns>true if the gem has been socketed, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="gem"/> is null.</exception>
        internal bool Insert( GemInstance gem )
        {
            if( gem == null )
                throw new ArgumentNullException( "gem" );

            if( this.Gem == null )
            {
                this.Gem = gem;
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region [ Fields ]
        #endregion
    }
}
