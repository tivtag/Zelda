// <copyright file="Cooldown.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Cooldown class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    /// <summary>
    /// Encapsulates 
    /// This class can't be inherited.
    /// </summary>
    /// <remarks>
    /// The same Cooldown can be shared by multiple objects
    /// These shared cooldowns are identified by using an unique id.
    /// </remarks>
    [TypeConverter( typeof( Cooldown.TypeConverter ) )]
    public sealed class Cooldown
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the time in seconds the <see cref="Cooldown"/> lasts.
        /// </summary>
        [DisplayName( "Time" ), Description( "The time in seconds the Cooldown lasts." )]
        public float TotalTime
        {
            get 
            {
                return this.totalTime;
            }
            
            set 
            {
                this.totalTime = value;

                if( this.timeLeft > this.totalTime )
                {
                    this.timeLeft = this.totalTime;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Cooldown"/> is shared by multiple objects.
        /// </summary>
        [Description( "States whether the the Cooldown is shared by multiple objects." ),
         DefaultValue( false )]
        public bool IsShared
        {
            get 
            {
                return this.isShared; 
            }

            set
            {
                this.isShared = value;
            }
        }

        /// <summary>
        /// Gets or sets the unique Id of this <see cref="Cooldown"/>.
        /// </summary>
        [Description( "The unique Id of the Cooldown. Only relevant is the cooldown is shared by multiple objects." )]
        public int Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// Gets or sets the time the that is left until this <see cref="Cooldown"/>
        /// is considered to be ready/over.
        /// </summary>
        [Browsable( false )]
        public float TimeLeft
        {
            get 
            {
                return this.timeLeft;
            }

            set
            {
                this.timeLeft = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Cooldown"/> is ready/over.
        /// </summary>
        [Browsable( false )]
        public bool IsReady
        {
            get 
            {
                return this.timeLeft <= 0.0f;
            }
        }
        
        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="Cooldown"/> class,
        /// which is not shared.
        /// </summary>
        public Cooldown()
            : this( ++idMaker, 0.0f, false )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cooldown"/> class,
        /// which is not shared.
        /// </summary>
        /// <param name="totalTime">
        /// The time in seconds the new Cooldown lasts.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="totalTime"/> is less than zero.
        /// </exception>
        public Cooldown( float totalTime )
            : this( ++idMaker, totalTime, false )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cooldown"/> class.
        /// </summary>
        /// <param name="id">
        /// The id that indentifies the new Cooldown.
        /// </param>
        /// <param name="totalTime">
        /// The time in seconds the new Cooldown lasts.
        /// </param>
        /// <param name="isShared">
        /// States whether the new Cooldown is a shared cooldown;
        /// shared cooldowns must have an (unique) <paramref name="id"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="totalTime"/> is less than zero.
        /// </exception>
        internal Cooldown( int id, float totalTime, bool isShared )
        {
            Contract.Requires<ArgumentException>( totalTime >= 0.0f );

            this.id        = id;
            this.isShared  = isShared;
            this.totalTime = totalTime;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this <see cref="Cooldown"/>.
        /// </summary>
        /// <param name="frameTime">
        /// The time in seconds the last frame took.
        /// </param>
        public void Update( float frameTime )
        {
            if( this.timeLeft >= 0.0f )
            {
                this.timeLeft -= frameTime;
            }
        }

        /// <summary>
        /// Resets this <see cref="Cooldown"/>.
        /// </summary>
        public void Reset()
        {
            this.timeLeft = this.totalTime;
        }

        /// <summary>
        /// Resets this <see cref="Cooldown"/> using the given <paramref name="time"/>.
        /// </summary>
        /// <param name="time">
        /// The time in seconds that the cooldown such last.
        /// </param>
        public void Reset( float time )
        {
            this.timeLeft = time;
        }

        /// <summary>
        /// Overwritten to return a localized string descriping how much time is left on the <see cref="Cooldown"/>.
        /// </summary>
        /// <returns>
        /// A human-readable string.
        /// </returns>
        public override string ToString()
        {
            var sb = new System.Text.StringBuilder( 15 );

            sb.Append( Resources.Cooldown );
            sb.Append( ": " );

            if( timeLeft < 60.0f )
            {
                sb.Append( System.Math.Round( this.timeLeft ).ToString( CultureInfo.CurrentCulture ) );
                sb.Append( ' ' );
                sb.Append( Resources.Seconds );
            }
            else
            {
                sb.Append( System.Math.Round( this.timeLeft / 60.0f, 2 ).ToString( CultureInfo.CurrentCulture ) );
                sb.Append( ' ' );
                sb.Append( Resources.Minutes );
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns a short localized string descriping how much time is left on the <see cref="Cooldown"/>.
        /// </summary>
        /// <returns>
        /// A human-readable string.
        /// </returns>
        public string ToShortString()
        {
            return Math.Ceiling( this.timeLeft ).ToString( CultureInfo.CurrentCulture );
        }

        /// <summary>
        /// Returns a clone of this Cooldown.
        /// </summary>
        /// <returns>
        /// The cloned Cooldown; or this if the cooldown <see cref="IsShared"/>.
        /// </returns>
        public Cooldown Clone()
        {
            if( this.isShared )
            {
                return this;
            }
            else
            {
                return new Cooldown( this.id, this.totalTime, false );
            }
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The time the cooldown lasts.
        /// </summary>
        private float totalTime;

        /// <summary>
        /// The time that is left until the cooldown is considered to be over.
        /// </summary>
        private float timeLeft;

        /// <summary>
        /// States whether the <see cref="Cooldown"/> is a shared cooldown.
        /// </summary>
        private bool isShared;

        /// <summary>
        /// The unique id of the <see cref="Cooldown"/>.
        /// </summary>
        private int id;

        /// <summary>
        /// Used to create ids of Cooldowns that are .
        /// </summary>
        private static int idMaker = int.MinValue;

        #endregion

        #region [ class TypeConverter ]

        /// <summary>
        /// Defines the TypeConverter that allows the conversion of Cooldowns into other types.
        /// </summary>
        internal sealed class TypeConverter : ExpandableObjectConverter
        {
            /// <summary>
            /// Returns whether this converter can convert the object to the specified type, 
            /// using the specified context.
            /// </summary>
            /// <param name="context">
            /// An System.ComponentModel.ITypeDescriptorContext that provides a format context.
            /// </param>
            /// <param name="destinationType">
            /// A System.Type that represents the type you want to convert to.
            /// </param>
            /// <returns>
            /// Returns true if this converter can perform the conversion; 
            /// otherwise, false.
            /// </returns>
            public override bool CanConvertTo( ITypeDescriptorContext context, Type destinationType )
            {
                if( destinationType == typeof( string ) )
                    return true;

                return base.CanConvertTo( context, destinationType );
            }

            /// <summary>
            /// Converts the given value object to the specified type,
            /// using the specified context and culture information.
            /// </summary>
            /// <param name="context">
            /// An System.ComponentModel.ITypeDescriptorContext that provides a format context.
            /// </param>
            /// <param name="culture">
            /// A System.Globalization.CultureInfo.
            /// </param>
            /// <param name="value">
            /// The System.Object to convert.
            /// </param>
            /// <param name="destinationType">
            /// The System.Type to convert the value parameter to.
            /// </param>
            /// <returns>
            /// An System.Object that represents the converted value.
            /// </returns>
            public override object ConvertTo( 
                ITypeDescriptorContext context,
                System.Globalization.CultureInfo culture,
                object value,
                Type destinationType )
            {
                Cooldown cooldown = value as Cooldown;

                if( cooldown != null )
                {
                    if( destinationType == typeof( string ) )
                    {
                        return cooldown.totalTime.ToString( culture ) + ", " +
                               cooldown.isShared.ToString( culture )  + ", " +
                               cooldown.id.ToString( culture );
                    }
                }

                return base.ConvertTo( context, culture, value, destinationType );
            }
        }

        #endregion
    }
}
