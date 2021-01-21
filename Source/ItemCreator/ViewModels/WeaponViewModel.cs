
namespace Zelda.ItemCreator
{
    using System;
    using System.ComponentModel;
    using Zelda.Items;

    /// <summary>
    /// Defines the view-model/property wrapper around the <see cref="Weapon"/> class.
    /// </summary>
    internal sealed class WeaponViewModel : EquipmentViewModel
    {
        [LocalizedCategory( "PropCate_Weapon" )]
        [LocalizedDisplayName( "PropDisp_WeaponType" )]
        [LocalizedDescription( "PropDesc_WeaponType" )]
        public WeaponType WeaponType
        {
            get { return this.WrappedWeapon.WeaponType; }
            set { this.WrappedWeapon.WeaponType = value; }
        }

        [LocalizedCategory( "PropCate_Weapon" )]
        [LocalizedDisplayName( "PropDisp_AttackSpeed" )]
        [LocalizedDescription( "PropDesc_AttackSpeed" )]
        public float AttackSpeed
        {
            get { return this.WrappedWeapon.AttackSpeed; }
            set { this.WrappedWeapon.AttackSpeed = value; }
        }

        [LocalizedCategory( "PropCate_Weapon" )]
        [LocalizedDisplayName( "PropDisp_DamageMinimum" )]
        [LocalizedDescription( "PropDesc_DamageMinimum" )]
        public int DamageMinimum
        {
            get { return this.WrappedWeapon.DamageMinimum; }
            set { this.WrappedWeapon.DamageMinimum = value; }
        }

        [LocalizedCategory( "PropCate_Weapon" )]
        [LocalizedDisplayName( "PropDisp_DamageMaximum" )]
        [LocalizedDescription( "PropDesc_DamageMaximum" )]
        public int DamageMaximum
        {
            get { return this.WrappedWeapon.DamageMaximum; }
            set { this.WrappedWeapon.DamageMaximum = value; }
        }

        [LocalizedCategory( "PropCate_Weapon" )]
        [LocalizedDisplayName( "PropDisp_AverageDamage" )]
        [LocalizedDescription( "PropDesc_AverageDamage" )]
        public int AverageDamage
        {
            get { return this.WrappedWeapon.AverageDamage; }
        }

        [LocalizedCategory( "PropCate_Weapon" )]
        [LocalizedDisplayName( "PropDisp_DamagePerSecond" )]
        [LocalizedDescription( "PropDesc_DamagePerSecond" )]
        public float DamagePerSecond
        {
            get { return this.WrappedWeapon.DamagePerSecond; }
        }

        /// <summary>
        /// Gets the Weapon this <see cref="WeaponViewModel"/> wraps around.
        /// </summary>
        [Browsable( false )]
        public Weapon WrappedWeapon
        {
            get
            {
                return (Weapon)this.WrappedObject;
            }
        }

        /// <summary>
        /// Gets the <see cref="Type"/> this <see cref="WeaponViewModel"/> wraps around.
        /// </summary>
        public override Type WrappedType
        {
            get
            {
                return typeof( Weapon );
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="WeaponViewModel"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game related services.
        /// </param>
        internal WeaponViewModel( IZeldaServiceProvider serviceProvider )
            : base( serviceProvider )
        {
        }

        /// <summary>
        /// Creates a clone of this EquipmentViewModel.
        /// </summary>
        /// <returns>
        /// The cloned IObjectPropertyWrapper.
        /// </returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new WeaponViewModel( this.ServiceProvider );
        }
    }
}
