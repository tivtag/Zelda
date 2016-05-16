// <copyright file="PlantPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.NpcCreator.PlantPropertyWrapper class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.NpcCreator
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Atom.Math;
    using Zelda.Entities;
    using Zelda.Items;
    using Zelda.Status;
    using Zelda.Status.Damage;

    /// <summary>
    /// Defines the IObjectPropertyWrapper for <see cref="Plant"/> entities.
    /// </summary>
    internal sealed class PlantPropertyWrapper : NpcPropertyWrapper<Plant>
    {
        #region > Collision <

        [LocalizedDisplayName( "PropDisp_CollisionSize" )]
        [LocalizedCategory( "PropCate_Collision" )]
        [LocalizedDescription( "PropDesc_CollisionSize" )]
        public Vector2 CollisionSize
        {
            get { return this.WrappedObject.Collision.Size; }
            set { this.WrappedObject.Collision.Size = value; }
        }

        [LocalizedDisplayName( "PropDisp_CollisionOffset" )]
        [LocalizedCategory( "PropCate_Collision" )]
        [LocalizedDescription( "PropDesc_CollisionOffset" )]
        public Vector2 CollisionOffset
        {
            get { return this.WrappedObject.Collision.Offset; }
            set { this.WrappedObject.Collision.Offset = value; }
        }

        #endregion

        #region > Settings <

        [LocalizedDisplayName( "PropDisp_RespawnTime" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescription( "PropDesc_RespawnTime" )]
        public float RespawnTime
        {
            get { return this.WrappedObject.Respawnable.RespawnTime; }
            set { this.WrappedObject.Respawnable.RespawnTime = value; }
        }

        [DefaultValue( null )]
        [LocalizedDisplayName( "PropDisp_DrawDataAndStrategyType" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescription( "PropDesc_DrawDataAndStrategyType" )]
        [Editor( typeof( Design.DrawDataAndStrategySelectionEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Type DrawDataAndStrategyType
        {
            get
            {
                var dds = this.WrappedObject.DrawDataAndStrategy;
                return dds != null ? dds.GetType() : null;
            }
            set
            {
                if( value != null )
                {
                    var dds = serviceProvider.DrawStrategyManager.GetStrategyClone( value, this.WrappedObject );
                    this.WrappedObject.DrawDataAndStrategy = dds;
                }
                else
                {
                    this.WrappedObject.DrawDataAndStrategy = null;
                }
            }
        }

        [LocalizedDisplayName( "PropDisp_DrawDataAndStrategySettings" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescription( "PropDesc_DrawDataAndStrategySettings" )]
        public Zelda.Entities.Drawing.IDrawDataAndStrategy DrawDataAndStrategySettings
        {
            get
            {
                return this.WrappedObject.DrawDataAndStrategy;
            }
        }

        [LocalizedDisplayName( "PropDisp_PlantCutEffectAnimationName" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescription( "PropDisp_PlantCutEffectAnimationName" )]
        public string CutEffectAnimationName
        {
            get { return this.WrappedObject.CutEffectAnimationName; }
            set { this.WrappedObject.CutEffectAnimationName = value; }
        }        

        [LocalizedDisplayName( "PropDisp_CutEffectAnimationColor" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescription( "PropDisp_CutEffectAnimationColor" )]
        public Microsoft.Xna.Framework.Color CutEffectAnimationColor
        {
            get
            {
                return this.WrappedObject.CutEffectAnimationColor;
            }

            set
            {
                this.WrappedObject.CutEffectAnimationColor = value;
            }
        }

        #endregion

        #region > Statable <

        [Category( "Status" ),
        Description( "The amount of life the Plant has." )]
        public int Life
        {
            get { return this.WrappedObject.Statable.BaseMaximumLife; }
            set
            {
                if( value < 0 )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "value" );

                this.WrappedObject.Statable.BaseMaximumLife = value;
            }
        }

        [Category( "Status" ), DisplayName( "Life Regen" ), DefaultValue( 0 ),
        Description( "The amount of life the Plant regenerates each tick." )]
        public int LifeRegeneration
        {
            get { return this.WrappedObject.Statable.LifeRegeneration; }
            set { this.WrappedObject.Statable.LifeRegeneration = value; }
        }

        [Category( "Status" ), DefaultValue( false ), DisplayName( "Is Invincible" ),
         Description( "States whether the Plant is invincible/ e.g. immun to all damage." )]
        public bool IsInvincible
        {
            get { return this.WrappedObject.Statable.IsInvincible; }
            set { this.WrappedObject.Statable.IsInvincible = value; }
        }

        [LocalizedDisplayName( "PropDisp_Armor" )]
        [LocalizedDescription( "PropDesc_Armor" )]
        [LocalizedCategory( "PropCate_Status" )]
        [DefaultValue( 0 )]
        public int Armor
        {
            get { return this.WrappedObject.Statable.BaseArmor; }
            set
            {
                if( value < 0 )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "value" );

                this.WrappedObject.Statable.BaseArmor = value;
            }
        }

        [LocalizedDisplayName( "PropDisp_ArmorMitigation" )]
        [LocalizedDescription( "PropDesc_ArmorMitigation" )]
        [LocalizedCategory( "PropCate_Status" )]
        [DefaultValue( 0.0f )]
        public float ArmorMitigation
        {
            get { return Status.StatusCalc.GetMitigationFromArmor( this.WrappedObject.Statable.Armor, this.WrappedObject.Statable.Level ); }
        }

        [Category( "Status" ),
        Description( "The level of the this.WrappedObject.Statable." )]
        public int Level
        {
            get { return this.WrappedObject.Statable.Level; }
            set
            {
                if( value <= 0 || value > 100 )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsInvalid, "value" );

                this.WrappedObject.Statable.Level = value;
            }
        }

        [Category( "Chance To Be" ), DisplayName( "Crit" ), DefaultValue( 0.0f ),
         Description( "The increased chance the player deosn't crit the Plant with any attack." )]
        public float ChanceToBeCrit
        {
            get 
            {
                return this.WrappedObject.Statable.ChanceToBe.BaseCrit;
            }

            set
            {
                if( value < -100 || value > 100 )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsInvalid, "value" );

                this.WrappedObject.Statable.ChanceToBe.BaseCrit = value;
            }
        }

        [Category( "Chance To Be" ), DisplayName( "Hit" ), DefaultValue( 5.0f ),
         Description( "The increased chance the player deosn't hit the Plant with any attack." )]
        public float ChanceToBeHit
        {
            get 
            {
                return this.WrappedObject.Statable.ChanceToBe.BaseHit;
            }

            set
            {
                if( value < -100 || value > 100 )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsInvalid, "value" );

                this.WrappedObject.Statable.ChanceToBe.BaseHit = value;
            }
        }

        #region ChanceToResist

        /// <summary>
        /// Gets or sets the base chance to resist attacks of the fire schol.
        /// </summary>
        [Category( "Chance To Resist" ), DisplayName( "Resist Fire" ), DefaultValue( 5.0f ),
       Description( "The chance to resist fire damage in %." )]
        public float ChanceToResistFire
        {
            get 
            { 
                return this.WrappedObject.Statable.Resistances.GetBase( ElementalSchool.Fire );
            }

            set
            {
                this.WrappedObject.Statable.Resistances.SetBase( ElementalSchool.Fire, value );
            }
        }

        /// <summary>
        /// Gets or sets the base chance to resist attacks of the water school.
        /// </summary>
        [Category( "Chance To Resist" ), DisplayName( "Resist Water" ), DefaultValue( 5.0f ),
       Description( "The chance to resist water damage in %." )]
        public float ChanceToResistWater
        {
            get
            {
                return this.WrappedObject.Statable.Resistances.GetBase( ElementalSchool.Water );
            }

            set
            {
                this.WrappedObject.Statable.Resistances.SetBase( ElementalSchool.Water, value );
            }
        }

        /// <summary>
        /// Gets or sets the base chance to resist attacks of the holy attacks.
        /// </summary>
        [Category( "Chance To Resist" ), DisplayName( "Resist Light" ), DefaultValue( 5.0f ),
        Description( "The chance to resist light damage in %." )]
        public float ChanceToResistHoly
        {
            get
            {
                return this.WrappedObject.Statable.Resistances.GetBase( ElementalSchool.Light );
            }

            set
            {
                this.WrappedObject.Statable.Resistances.SetBase( ElementalSchool.Light, value );
            }
        }

        /// <summary>
        /// Gets or sets the base chance to resist attacks of the shadow school.
        /// </summary>
        [Category( "Chance To Resist" ), DisplayName( "Resist Shadow" ), DefaultValue( 5.0f ),
       Description( "The chance to resist shadow damage in %." )]
        public float ChanceToResistShadow
        {

            get
            {
                return this.WrappedObject.Statable.Resistances.GetBase( ElementalSchool.Shadow );
            }

            set
            {
                this.WrappedObject.Statable.Resistances.SetBase( ElementalSchool.Shadow, value );
            }
        }

        /// <summary>
        /// Gets or sets the base chance to resist attacks of the nature school.
        /// </summary>
        [Category( "Chance To Resist" ), DisplayName( "Resist Nature" ), DefaultValue( 5.0f ),
        Description( "The chance to resist nature damage in %." )]
        public float ChanceToResistNature
        {
            get
            {
                return this.WrappedObject.Statable.Resistances.GetBase( ElementalSchool.Nature );
            }

            set
            {
                this.WrappedObject.Statable.Resistances.SetBase( ElementalSchool.Nature, value );
            }
        }

        #endregion

        #region DamageTakenModifiers

        [Category( "Damage Taken - School" ), DisplayName( "Taken Physical" ), DefaultValue( 1.0f ),
        Description( "The modifier that is applied to the Physical damage the Enemy takes." )]
        public float DamageTakenFromPhysicalDamageSchool
        {
            get
            {
                var damageTaken = this.WrappedObject.Statable.DamageTaken;
                return damageTaken.FromSchool.GetBase( DamageSchool.Physical );
            }

            set
            {
                var damageTaken = this.WrappedObject.Statable.DamageTaken;
                damageTaken.FromSchool.SetBase( DamageSchool.Physical, value );
            }
        }

        [Category( "Damage Taken - School" ), DisplayName( "Taken Magical" ), DefaultValue( 1.0f ),
        Description( "The modifier that is applied to the Magical damage the Enemy takes." )]
        public float DamageTakenFromMagicalDamageSchool
        {
            get
            {
                var damageTaken = this.WrappedObject.Statable.DamageTaken;
                return damageTaken.FromSchool.GetBase( DamageSchool.Magical );
            }

            set
            {
                var damageTaken = this.WrappedObject.Statable.DamageTaken;
                damageTaken.FromSchool.SetBase( DamageSchool.Magical, value );
            }
        }

        [Category( "Damage Taken - Elemental" ), DisplayName( "Taken Fire" ), DefaultValue( 1.0f ),
        Description( "The modifier that is applied to the Fire damage the Enemy takes." )]
        public float DamageTakenFromFireElement
        {
            get
            {
                var damageTaken = this.WrappedObject.Statable.DamageTaken;
                return damageTaken.FromElement.GetBase( ElementalSchool.Fire );
            }

            set
            {
                var damageTaken = this.WrappedObject.Statable.DamageTaken;
                damageTaken.FromElement.SetBase( ElementalSchool.Fire, value );
            }
        }

        [Category( "Damage Taken - Elemental" ), DisplayName( "Taken Water" ), DefaultValue( 1.0f ),
        Description( "The modifier that is applied to the Water damage the Enemy takes." )]
        public float DamageTakenFromWaterElement
        {
            get
            {
                var damageTaken = this.WrappedObject.Statable.DamageTaken;
                return damageTaken.FromElement.GetBase( ElementalSchool.Water );
            }

            set
            {
                var damageTaken = this.WrappedObject.Statable.DamageTaken;
                damageTaken.FromElement.SetBase( ElementalSchool.Water, value );
            }
        }

        [Category( "Damage Taken - Elemental" ), DisplayName( "Taken Light" ), DefaultValue( 1.0f ),
        Description( "The modifier that is applied to the Light damage the Enemy takes." )]
        public float DamageTakenFromLightElement
        {
            get
            {
                var damageTaken = this.WrappedObject.Statable.DamageTaken;
                return damageTaken.FromElement.GetBase( ElementalSchool.Light );
            }

            set
            {
                var damageTaken = this.WrappedObject.Statable.DamageTaken;
                damageTaken.FromElement.SetBase( ElementalSchool.Light, value );
            }
        }

        [Category( "Damage Taken - Elemental" ), DisplayName( "Taken Shadow" ), DefaultValue( 1.0f ),
        Description( "The modifier that is applied to the Shadow damage the Enemy takes." )]
        public float DamageTakenFromShadowElement
        {
            get
            {
                var damageTaken = this.WrappedObject.Statable.DamageTaken;
                return damageTaken.FromElement.GetBase( ElementalSchool.Shadow );
            }

            set
            {
                var damageTaken = this.WrappedObject.Statable.DamageTaken;
                damageTaken.FromElement.SetBase( ElementalSchool.Shadow, value );
            }
        }

        [Category( "Damage Taken - Elemental" ), DisplayName( "Taken Nature" ), DefaultValue( 1.0f ),
        Description( "The modifier that is applied to the Nature damage the Enemy takes." )]
        public float DamageTakenFromNatureElement
        {
            get
            {
                var damageTaken = this.WrappedObject.Statable.DamageTaken;
                return damageTaken.FromElement.GetBase( ElementalSchool.Nature );
            }

            set
            {
                var damageTaken = this.WrappedObject.Statable.DamageTaken;
                damageTaken.FromElement.SetBase( ElementalSchool.Nature, value );
            }
        }

        #endregion

        #endregion

        #region > Killable <

        /// <summary>
        /// The experience the PlantObject gives when killed.
        /// </summary>
        [Category( "Status" ),
         Description( "The experience the player receives when he kills the this.WrappedObject.Statable." )]
        public int Experience
        {
            get 
            {
                return this.WrappedObject.Killable.Experience;
            }

            set
            {
                if( value < 0 )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "value" );

                this.WrappedObject.Killable.Experience = value;
            }
        }

        #endregion

        #region > Lootable <

        [Category( "Loot" ), DisplayName( "Drop Range" ), DefaultValue( 5.0f ),
            Description( "The radius of the circle, centering at the plant's collision position, in which loot may drop." )]
        public float ItemDropRange
        {
            get { return this.WrappedObject.Lootable.DropRange; }
            set { this.WrappedObject.Lootable.DropRange = value; }
        }

        [Category( "Loot" ), DisplayName( "Drop Mode" ), DefaultValue( ItemDropMode.Normal ),
         Description( "Describes how many Items the Plant can drop at once." )]
        public ItemDropMode ItemDropMode
        {
            get { return this.WrappedObject.Lootable.DropMode; }
            set { this.WrappedObject.Lootable.DropMode = value; }
        }

        #endregion

         /// <summary>
        /// Applies any additional data for this PlantPropertyWrapper.
        /// </summary>
        /// <param name="window">
        /// The application's MainWindow.
        /// </param>
        public override void ApplyData( MainWindow window )
        {
            // Apply Loot:
            var lootable = this.WrappedObject.Lootable;
            lootable.Loot.Clear();

            var dataGridViewLoot = window.DataGridLoot;
            dataGridViewLoot.EndEdit();

            foreach( DataGridViewRow row in dataGridViewLoot.Rows )
            {
                string itemName     = (string)row.Cells[0].Value;
                int dropChance      = row.Cells[1].Value != null ? (int)row.Cells[1].Value : 0;
                bool magicFindWorks = (bool)row.Cells[2].FormattedValue;

                if( itemName == null )
                    continue;

                int id = magicFindWorks ? LootTable.ItemIdAffectedByMagicFind : 0;
                lootable.Loot.Insert( itemName, dropChance, id );
            }
        }

        /// <summary>
        /// Applies any additional data of this INpcPropertyWrapper to the View.
        /// </summary>
        /// <param name="window">
        /// The application's MainWindow.
        /// </param>
        public override void SetupView( MainWindow window )
        {
            var dataGridLoot = window.DataGridLoot;
            var loot         = this.WrappedObject.Lootable.Loot;

            // Setup Loot:
            dataGridLoot.Rows.Clear();

            for( int i = 0; i < loot.Count; ++i )
            {
                var entry = loot[i];

                dataGridLoot.Rows.Add(
                    entry.Data,
                    (int)entry.Weight,
                    entry.Id == LootTable.ItemIdAffectedByMagicFind,
                    loot.TotalWeight == 0.0f ? 0.0f : (entry.Weight/loot.TotalWeight)*100.0f
                );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlantPropertyWrapper"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public PlantPropertyWrapper( IZeldaServiceProvider serviceProvider )
        {
            if( serviceProvider == null )
                throw new ArgumentNullException( "serviceProvider" );

            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Returns a clone of this PlantPropertyWrapper.
        /// </summary>
        /// <returns>The cloned PlantPropertyWrapper.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new PlantPropertyWrapper( serviceProvider );
        }

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;
    }
}
