// <copyright file="MagicFindEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.MagicFindEffect class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// Defines a StatusValueEffect that increases the chance of an extended-statable ZeldaEntity
    /// to find rare items.
    /// </summary>
    internal sealed class MagicFindEffect : StatusValueEffect
    {
        #region [ Identification ]

        /// <summary>
        /// The string that uniquely identifies this MagicFindEffect.
        /// </summary>
        public const string IdentifierString = "MF";
        
        /// <summary> 
        /// Gets an unique string that represents what this <see cref="MagicFindEffect"/> manipulates.
        /// </summary>
        public override string Identifier
        {
            get 
            { 
                return IdentifierString;
            }
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets a short localised description of this <see cref="StatusEffect"/>.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description about this StatusEffect.
        /// </param>
        /// <returns>
        /// The localized description string.
        /// </returns>
        public override string GetDescription( Statable statable )
        {
            if( this.ManipulationType == StatusManipType.Fixed )
            {
                if( this.Value >= 0.0f )
                {
                    return string.Format( 
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.IncXByY,
                        Resources.MagicFind,
                        this.Value
                    );
                }
                else
                {
                    return string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.DecXByY,
                        Resources.MagicFind,
                        -this.Value
                    );
                }
            }
            else
            {
                if( this.Value >= 0.0f )
                {
                    return string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.IncXByYPercent, 
                        Resources.MagicFind,
                        this.Value
                    );
                }
                else
                {
                    return string.Format( 
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.DecXByYPercent,
                        Resources.MagicFind,
                        -this.Value
                    );
                }
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="MagicFindEffect"/> class.
        /// </summary>
        public MagicFindEffect()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MagicFindEffect"/> class.
        /// </summary>
        /// <param name="value">
        /// The manipulation value.
        /// </param>
        /// <param name="manipulationType">
        /// Indicates how the Value of the new <see cref="MagicFindEffect"/> should be interpreted.
        /// </param>
        public MagicFindEffect( float value, StatusManipType manipulationType )
            : base( value, manipulationType )
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this MagicFindEffect gets enabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this MagicFindEffect.
        /// </param>
        public override void OnEnable( Statable user )
        {
            OnChanged( user );
        }

        /// <summary>
        /// Called when this MagicFindEffect gets disabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this MagicFindEffect.
        /// </param>
        public override void OnDisable( Statable user )
        {
            OnChanged( user );
        }

        /// <summary>
        /// Called when this MagicFindEffect gets enabled/disabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that has enabled/disabled this MagicFindEffect.
        /// </param>
        private static void OnChanged( Statable user )
        {
            ((ExtendedStatable)user).Refresh_MagicFind();
        }

        /// <summary>
        /// Returns a clone of this MagicFindEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new MagicFindEffect();
            this.SetupClone( clone );
            return clone;
        }

        #endregion
    }
}
