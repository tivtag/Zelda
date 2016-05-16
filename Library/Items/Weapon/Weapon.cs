// <copyright file="Weapon.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Weapon class.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items
{
    using Atom.Xna;

    /// <summary>
    /// A <see cref="Weapon"/> is a special kind of <see cref="Equipment"/>
    /// that provides extra Damage stats.
    /// This class can't be inherited.
    /// </summary>
    public sealed class Weapon : Equipment
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="ItemType"/> this <see cref="Weapon"/> represents.
        /// </summary>
        public override ItemType ItemType
        {
            get { return ItemType.Weapon; }
        }

        /// <summary>
        /// Gets or sets the type of this <see cref="Weapon"/>.
        /// </summary>
        public WeaponType WeaponType
        {
            get { return weaponType; }
            set { weaponType = value; }
        }

        /// <summary>
        /// Gets the attack type of this <see cref="Weapon"/>.
        /// </summary>
        public WeaponAttackType AttackType
        {
            get
            {
                if( this.Slot == EquipmentSlot.Ranged )
                    return WeaponAttackType.Ranged;
                else
                    return WeaponAttackType.Melee;
            }
        }

        /// <summary>
        /// Gets or sets the attack speed (or better said delay) of this <see cref="Weapon"/>.
        /// </summary>
        /// <value>The default value is 2.5f.</value>
        public float AttackSpeed
        {
            get { return attackSpeed; }
            set { attackSpeed = value; }
        }

        /// <summary>
        /// Gets or sets the minimum damage of this <see cref="Weapon"/>.
        /// </summary>
        public int DamageMinimum
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum damage of this <see cref="Weapon"/>.
        /// </summary>
        public int DamageMaximum
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the average damage this <see cref="Weapon"/> provides.
        /// </summary>
        public int AverageDamage
        {
            get { return (DamageMinimum + DamageMaximum) / 2; }
        }

        /// <summary>
        /// Gets the average damage per second this <see cref="Weapon"/> provides.
        /// </summary>
        public float DamagePerSecond
        {
            get { return ((DamageMinimum + DamageMaximum) / 2.0f) / this.AttackSpeed; }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Modifies the power of this Weapon by the given factor.
        /// </summary>
        /// <param name="factor">
        /// The factor to modify the item by.
        /// </param>
        /// <returns>
        /// true if modifications were allowed; -or- otherwise false.
        /// </returns>
        public override bool ModifyPowerBy( float factor )
        {
            if( base.ModifyPowerBy( factor ) )
            {
                this.DamageMinimum = (int)(this.DamageMinimum * factor);
                this.DamageMaximum = (int)(this.DamageMaximum * factor);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a new instance of this <see cref="Weapon"/>.
        /// </summary>
        /// <param name="powerFactor">
        /// The factor by which the power of the new ItemInstance varies compared to this base Weapon.
        /// </param>
        /// <returns>A new <see cref="WeaponInstance"/>.</returns>
        public override ItemInstance CreateInstance( float powerFactor )
        {
            return new WeaponInstance( this, powerFactor );
        }

        #region > Cloning <

        /// <summary>
        /// Creates a clone of this <see cref="Weapon"/>.
        /// </summary>
        /// <returns>
        /// The cloned item.
        /// </returns>
        public override Item Clone()
        {
            var clone = new Weapon();

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the given Weapon to be a clone of this Weapon.
        /// </summary>
        /// <param name="clone">
        /// The Weapon to setup as a clone of this Weapon.
        /// </param>
        private void SetupClone( Weapon clone )
        {
            base.SetupClone( clone );

            clone.weaponType = this.weaponType;
            clone.attackSpeed = this.attackSpeed;
            clone.DamageMinimum = this.DamageMinimum;
            clone.DamageMaximum = this.DamageMaximum;
        }

        #endregion

        #region > Storage <

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            // Write Equipment & Item Data:
            base.Serialize( context );

            // Write Weapon Header:
            const int Version = 2;
            context.Write( Version );

            // Write Weapon Data:
            context.Write( this.DamageMinimum );
            context.Write( this.DamageMaximum );
            context.Write( this.AttackSpeed );
            context.Write( (int)this.WeaponType );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            // Read Equipment & Item Header/Data:
            base.Deserialize( context );

            // Read Weapon Header:
            const int CurrentVersion = 2;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, "Zelda.Items.Weapon" );

            // Read Weapon Data:
            this.DamageMinimum = context.ReadInt32();
            this.DamageMaximum = context.ReadInt32();
            this.AttackSpeed   = context.ReadSingle();
            this.WeaponType    = (WeaponType)context.ReadInt32();

            if( version == 1 )
            {
                // Color has been moved to Equipment.
                this.IngameColor = context.ReadColor();
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The storage field for the <see cref="WeaponType"/> field.
        /// </summary>
        private WeaponType weaponType = WeaponType.OneHandedSword;

        /// <summary>
        /// The storage field for the <see cref="AttackSpeed"/> field.
        /// </summary>
        private float attackSpeed = 2.5f;

        #endregion
    }
}