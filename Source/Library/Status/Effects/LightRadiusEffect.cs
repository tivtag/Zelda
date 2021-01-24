// <copyright file="LightRadiusEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.LightRadiusEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status
{
    /// <summary>
    /// Defines a StatusValueEffect that increases the chance of an extended-statable ZeldaEntity
    /// to find rare items.
    /// </summary>
    internal sealed class LightRadiusEffect : StatusValueEffect
    {
        #region [ Identification ]

        /// <summary>
        /// The string that uniquely identifies this LightRadiusEffect.
        /// </summary>
        public const string IdentifierString = "LightRadius";

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="LightRadiusEffect"/> manipulates.
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
            if( this.Value >= 0.0f )
            {
                return string.Format(
                    System.Globalization.CultureInfo.CurrentCulture,
                    Resources.IncXByY,
                    Resources.LightRadius,
                    this.Value
                );
            }
            else
            {
                return string.Format(
                    System.Globalization.CultureInfo.CurrentCulture,
                    Resources.DecXByY,
                    Resources.LightRadius,
                    -this.Value
                );
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="LightRadiusEffect"/> class.
        /// </summary>
        public LightRadiusEffect()
            : this( 0.0f )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightRadiusEffect"/> class.
        /// </summary>
        /// <param name="value">
        /// The manipulation value.
        /// </param>
        public LightRadiusEffect( float value )
            : base( value, StatusManipType.Fixed )
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this LightRadiusEffect gets enabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this LightRadiusEffect.
        /// </param>
        public override void OnEnable( Statable user )
        {
            OnChanged( user );
        }

        /// <summary>
        /// Called when this LightRadiusEffect gets disabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this LightRadiusEffect.
        /// </param>
        public override void OnDisable( Statable user )
        {
            OnChanged( user );
        }

        /// <summary>
        /// Called when this LightRadiusEffect gets enabled/disabled for the given extended-statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that has enabled/disabled this LightRadiusEffect.
        /// </param>
        private static void OnChanged( Statable user )
        {
            var laternOwner = user.Owner as Zelda.Entities.ILaternOwner;

            if( laternOwner != null )
            {
                laternOwner.Latern.Refresh();
            }
        }

        /// <summary>
        /// Returns a clone of this LightRadiusEffect.
        /// </summary>
        /// <returns>
        /// The cloned StatusEffect.
        /// </returns>
        public override StatusEffect Clone()
        {
            var clone = new LightRadiusEffect();
            this.SetupClone( clone );
            return clone;
        }

        #endregion
    }
}
