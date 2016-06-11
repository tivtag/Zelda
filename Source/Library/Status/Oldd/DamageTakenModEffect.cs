using System;

namespace Zelda.Status
{
    /// <summary>
    /// Defines a <see cref="StatusValueEffect"/> that manipulates how much damage
    /// the <see cref="Statable"/> ZeldaEntity takes from a specific type of attack.
    /// This class can't be inherited.
    /// </summary>
    public sealed class DamageTakenModEffect : StatusValueEffect
    {
        #region [ Constants ]

        /// <summary> 
        /// Identifies the unique manipulation name(s) of the DamageTakenModEffect.
        /// </summary>
        public const string ManipNameFire     = "DmgTakenFire",
                            ManipNameWater    = "DmgTakenWater",
                            ManipNameLight    = "DmgTakenLight",
                            ManipNameShadow   = "DmgTakenShadow",
                            ManipNameNature   = "DmgTakenNature",
                            ManipNamePhysical = "DmgTakenPhysical",
                            ManipNameAll      = "DmgTakenAll";

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets what <see cref="DamageTakenMod"/> the <see cref="DamageTakenModEffect"/> is modifying.
        /// </summary>
        public DamageTakenMod Modifying
        {
            get { return modifying; }
            set { modifying = value; }
        }

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="StatusEffect"/> manipulates.
        /// </summary>
        public override string Manipulates
        {
            get
            {
                switch( modifying )
                {
                    case DamageTakenMod.All:
                        return ManipNameAll;

                    case DamageTakenMod.Light:
                        return ManipNameLight;

                    case DamageTakenMod.Shadow:
                        return ManipNameShadow;

                    case DamageTakenMod.Fire:
                        return ManipNameFire;

                    case DamageTakenMod.Water:
                        return ManipNameWater;

                    case DamageTakenMod.Nature:
                        return ManipNameNature;

                    case DamageTakenMod.Physical:
                        return ManipNamePhysical;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Gets a short localised description of this <see cref="StatusEffect"/>.
        /// </summary>
        public override string Description
        {
            get
            {
                if( Value >= 0.0f )
                {
                    return string.Format( 
                        System.Globalization.CultureInfo.CurrentUICulture,
                        Resources.IncreasesDamageTakenFromXByY,
                        StatusCalc.GetLocalizedString( modifying ),
                        Value.ToString( System.Globalization.CultureInfo.CurrentUICulture )
                    );
                }
                else
                {
                    return string.Format(
                        System.Globalization.CultureInfo.CurrentUICulture,
                        Resources.DecreasesDamageTakenFromXByY,
                        StatusCalc.GetLocalizedString( modifying ),
                        (-Value).ToString( System.Globalization.CultureInfo.CurrentUICulture )
                    );
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this StatusEffect is 'bad' for the statable ZeldaEntity.
        /// </summary>
        public override bool IsBad
        {
            get { return this.Value > 0.0f; }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageTakenModEffect"/> class.
        /// </summary>
        public DamageTakenModEffect()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageTakenModEffect"/> class.
        /// </summary>
        /// <param name="modifying">
        /// Indicates what kind of modifier the new DamageTakenModEffect modifies.
        /// </param>
        /// <param name="value">
        /// The new DamageTakenModEffect's value.
        /// </param>
        public DamageTakenModEffect( DamageTakenMod modifying, float value )
            : base( value, StatusManipType.Fixed )
        {
            this.modifying = modifying;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="DamageTakenModEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="DamageTakenModEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="DamageTakenModEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="DamageTakenModEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            OnChanged( user );
        }

        /// <summary>
        /// Called when this <see cref="DamageTakenModEffect"/> gets enabled or disabled for the given Statable Entity.
        /// </summary>
        /// <param name="user">
        /// The related statable.
        /// </param>
        private void OnChanged( Statable user )
        {
            switch( modifying )
            {
                case DamageTakenMod.Physical:
                    user.Refresh_DamageTakenModifierPhysical();
                    break;
                case DamageTakenMod.Fire:
                    user.Refresh_DamageTakenModifierFire();
                    break;
                case DamageTakenMod.Water:
                    user.Refresh_DamageTakenModifierWater();
                    break;
                case DamageTakenMod.Light:
                    user.Refresh_DamageTakenModifierLight();
                    break;
                case DamageTakenMod.Shadow:
                    user.Refresh_DamageTakenModifierShadow();
                    break;
                case DamageTakenMod.Nature:
                    user.Refresh_DamageTakenModifierNature();
                    break;
                case DamageTakenMod.All:
                    user.Refresh_DamageTakenModifierFire();
                    user.Refresh_DamageTakenModifierWater();
                    user.Refresh_DamageTakenModifierLight();
                    user.Refresh_DamageTakenModifierShadow();
                    user.Refresh_DamageTakenModifierNature();
                    user.Refresh_DamageTakenModifierPhysical();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Serializes/Writes the data of this DamageTakenModEffect using the given System.IO.BinaryWriter.
        /// </summary>
        /// <param name="writer">The System.IO.BinaryWriter to use.</param>
        public override void Serialize( System.IO.BinaryWriter writer )
        {
            base.Serialize( writer );

            // Write Data:
            writer.Write( (byte)this.modifying );
        }

        /// <summary>
        /// Deserializes/Reads the data of this DamageTakenModEffect using the given System.IO.BinaryReader.
        /// </summary>
        /// <param name="reader">The System.IO.BinaryReader to use.</param>
        public override void Deserialize( System.IO.BinaryReader reader )
        {
            base.Deserialize( reader );

            // Read Data:
            this.modifying = (DamageTakenMod)reader.ReadByte();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// States what kind of modifier this DamageTakenModEffect modifies.
        /// </summary>
        private DamageTakenMod modifying;

        #endregion
    }
}
