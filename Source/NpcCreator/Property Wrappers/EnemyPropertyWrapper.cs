// <copyright file="EnemyPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.NpcCreator.EnemyPropertyWrapper class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.NpcCreator
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Atom.Math;
    using Zelda.Entities;
    using Zelda.Entities.Behaviours;
    using Zelda.Entities.Components;
    using Zelda.Entities.Drawing;
    using Zelda.Items;
    using Zelda.Status;
    using Zelda.Status.Containers;
    using Zelda.Status.Damage;
    using Zelda.Status.Damage.Containers;

    /// <summary>
    /// Defines the IObjectPropertyWrapper for <see cref="Enemy"/> entities.
    /// </summary>
    internal sealed class EnemyPropertyWrapper : NpcPropertyWrapper<Enemy>
    {
        #region > Movement <

        [LocalizedDisplayName( "PropDisp_MovementSpeed" )]
        [LocalizedCategory( "PropCate_Movement" )]
        [LocalizedDescription( "PropDesc_MovementSpeed" )]
        public float MovementSpeed
        {
            get { return this.WrappedObject.Moveable.Speed; }
            set
            {
                if( value <= 0.0f )
                {
                    throw new ArgumentOutOfRangeException( nameof( value ), value, Atom.ErrorStrings.SpecifiedValueIsZeroOrNegative );
                }
                else if( value > 120.0f )
                {
                    throw new ArgumentOutOfRangeException( nameof( value ), value, Atom.ErrorStrings.SpecifiedValueIsInvalid );
                }

                this.WrappedObject.Moveable.Speed = this.WrappedObject.Moveable.BaseSpeed = value;
            }
        }

        [LocalizedDisplayName( "PropDisp_CanSlide" )]
        [LocalizedCategory( "PropCate_Movement" )]
        [LocalizedDescription( "PropDesc_CanSlide" )]
        [DefaultValue( true )]
        public bool CanSlide
        {
            get { return this.WrappedObject.Moveable.CanSlide; }
            set { this.WrappedObject.Moveable.CanSlide = value; }
        }

        [LocalizedDisplayName( "PropDisp_CanBePushed" )]
        [LocalizedCategory( "PropCate_Movement" )]
        [LocalizedDescription( "PropDesc_CanBePushed" )]
        [DefaultValue( true )]
        public bool CanBePushed
        {
            get { return this.WrappedObject.Moveable.CanBePushed; }
            set { this.WrappedObject.Moveable.CanBePushed = value; }
        }

        [LocalizedDisplayName( "PropDisp_PushingForceModifier" )]
        [LocalizedCategory( "PropCate_Movement" )]
        [LocalizedDescription( "PropDesc_PushingForceModifier" )]
        [DefaultValue( 1.0f )]
        public float PushingForceModifier
        {
            get { return this.WrappedObject.Moveable.PushingForceModifier; }
            set { this.WrappedObject.Moveable.PushingForceModifier = value; }
        }

        [LocalizedDisplayName( "PropDisp_CanChangeFloor" )]
        [LocalizedCategory( "PropCate_Movement" )]
        [LocalizedDescription( "PropDesc_CanChangeFloor" )]
        [DefaultValue( true )]
        public bool CanChangeFloor
        {
            get { return this.WrappedObject.Moveable.CanChangeFloor; }
            set { this.WrappedObject.Moveable.CanChangeFloor = value; }
        }

        [LocalizedDisplayName( "PropDisp_CanSwim" )]
        [LocalizedCategory( "PropCate_Movement" )]
        [LocalizedDescription( "PropDesc_CanSwim" )]
        [DefaultValue( false )]
        public bool CanSwim
        {
            get { return this.WrappedObject.Moveable.CanSwim; }
            set { this.WrappedObject.Moveable.CanSwim = value; }
        }

        [LocalizedDisplayName( "PropDisp_CollidesWithMap" )]
        [LocalizedCategory( "PropCate_Movement" )]
        [LocalizedDescription( "PropDesc_CollidesWithMap" )]
        [DefaultValue( true )]
        public bool CollidesWithMap
        {
            get { return this.WrappedObject.Moveable.CollidesWithMap; }
            set { this.WrappedObject.Moveable.CollidesWithMap = value; }
        }

        #endregion

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

        [LocalizedDisplayName( "PropDisp_IsSolid" )]
        [LocalizedCategory( "PropCate_Collision" )]
        [LocalizedDescription( "PropDesc_IsSolid" )]
        [DefaultValue( true )]
        public bool IsSolid
        {
            get { return this.WrappedObject.Collision.IsSolid; }
            set { this.WrappedObject.Collision.IsSolid = value; }
        }

        #endregion

        #region > Settings <

        [LocalizedDisplayName( "PropDisp_AggressionType" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescription( "PropDesc_AggressionType" )]
        public AggressionType AggressionType
        {
            get
            {
                return this.WrappedObject.AgressionType;
            }

            set
            {
                this.WrappedObject.AgressionType = value;
            }
        }

        [LocalizedDisplayName( "PropDisp_FloorRelativity" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescription( "PropDesc_FloorRelativity" )]
        public EntityFloorRelativity FloorRelativity
        {
            get
            {
                return this.WrappedObject.FloorRelativity;
            }

            set
            {
                this.WrappedObject.FloorRelativity = value;
            }
        }

        [DefaultValue( null )]
        [LocalizedDisplayName( "PropDisp_EntityBehaviour" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescription( "PropDesc_EntityBehaviour" )]
        [Editor( typeof( Design.EntityBehaviourSelectionEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Type EntityBehaviourType
        {
            get
            {
                IEntityBehaviour behaviour = this.WrappedObject.Behaveable.Behaviour;
                return behaviour != null ? behaviour.GetType() : null;
            }
            set
            {
                if( value != null )
                {
                    IEntityBehaviour behaviour = serviceProvider.BehaviourManager.GetBehaviourClone( value, this.WrappedObject );
                    this.WrappedObject.Behaveable.Behaviour = behaviour;
                }
                else
                {
                    this.WrappedObject.Behaveable.Behaviour = null;
                }
            }
        }

        [LocalizedDisplayName( "PropDisp_EntityBehaviourSettings" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescription( "PropDesc_EntityBehaviourSettings" )]
        public IEntityBehaviour EntityBehaviourSettings
        {
            get
            {
                return this.WrappedObject.Behaveable.Behaviour;
            }
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
                IDrawDataAndStrategy dds = this.WrappedObject.DrawDataAndStrategy;
                return dds != null ? dds.GetType() : null;
            }
            set
            {
                if( value != null )
                {
                    IDrawDataAndStrategy dds = serviceProvider.DrawStrategyManager.GetStrategyClone( value, this.WrappedObject );
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
        public IDrawDataAndStrategy DrawDataAndStrategySettings
        {
            get
            {
                return this.WrappedObject.DrawDataAndStrategy;
            }
        }

        #endregion

        #region > Statable <

        [Category( "Status" ),
        Description( "The amount of mana the Enemy has." )]
        public int Mana
        {
            get { return this.WrappedObject.Statable.MaximumMana; }
            set
            {
                if( value < 0 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, nameof( value ) );
                }

                this.WrappedObject.Statable.MaximumMana = value;
            }
        }

        [Category( "Status" ), DisplayName( "Mana Regen" ), DefaultValue( 0 ),
        Description( "The amount of mana the Enemy regenerates each tick." )]
        public int ManaRegeneration
        {
            get { return this.WrappedObject.Statable.ManaRegeneration; }
            set
            {
                if( value < 0 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, nameof( value ) );
                }

                this.WrappedObject.Statable.ManaRegeneration = value;
            }
        }

        [Category( "Status" ),
        Description( "The amount of life the Enemy has." )]
        public int Life
        {
            get { return this.WrappedObject.Statable.BaseMaximumLife; }
            set
            {
                if( value < 0 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, nameof( value ) );
                }

                this.WrappedObject.Statable.BaseMaximumLife = value;
            }
        }

        [Category( "Status" ), DisplayName( "Life Regen" ), DefaultValue( 0 ),
        Description( "The amount of life the Enemy regenerates each tick." )]
        public int LifeRegeneration
        {
            get { return this.WrappedObject.Statable.LifeRegeneration; }
            set
            {
                if( value < 0 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, nameof( value ) );
                }

                this.WrappedObject.Statable.LifeRegeneration = value;
            }
        }

        [Category( "Status" ), DefaultValue( false ), DisplayName( "Is Invincible" ),
         Description( "States whether the Enemy is invincible/ e.g. immun to all damage." )]
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
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, nameof( value ) );
                }

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

        /// <summary>
        /// Gets or sets the race of the <see cref="Enemy"/>.
        /// </summary>
        [Category( "Status" ),
        Description( "The race of the this.WrappedObject.Statable." )]
        public RaceType Race
        {
            get { return this.WrappedObject.Statable.Race; }
            set { this.WrappedObject.Statable.Race = value; }
        }

        [Category( "Status" ),
        Description( "The level of the this.WrappedObject.Statable." )]
        public int Level
        {
            get { return this.WrappedObject.Statable.Level; }
            set
            {
                if( value <= 0 || value > 100 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsInvalid, nameof( value ) );
                }

                this.WrappedObject.Statable.Level = value;
            }
        }

        /// <summary>
        ///  Gets or sets the value that states whether the <see cref="Enemy"/> can block attacks.
        /// </summary>
        [Category( "Status Block" ), DisplayName( "Can Block" ), DefaultValue( false ),
        Description( "States whether the enemy can block melee and ranged attacks." )]
        public bool CanBlock { get { return this.WrappedObject.Statable.CanBlock; } set { this.WrappedObject.Statable.CanBlock = value; } }

        /// <summary>
        /// Gets or sets the value that stores the chance for the <see cref="Enemy"/> to block an attack.
        /// Only relevant if canBlock is true.
        /// </summary>
        [Category( "Status Block" ), DisplayName( "Chance To Block" ), DefaultValue( 5.0f ),
        Description( "Specifies the chance for the enemy to block melee and ranged attacks." )]
        public float ChanceToBlock
        {
            get
            {
                Status.Containers.ChanceToContainer chanceTo = this.WrappedObject.Statable.ChanceTo;
                return chanceTo.GetBase( ChanceToStatus.Block );
            }

            set
            {
                if( value < 0 || value > 100 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsInvalid, nameof( value ) );
                }

                Status.Containers.ChanceToContainer chanceTo = this.WrappedObject.Statable.ChanceTo;
                chanceTo.SetBase( ChanceToStatus.Block, value );
            }
        }

        /// <summary>
        /// Gets or sets the value that stores the block 'power' of the <see cref="Enemy"/> - aka how much damage is blocked.
        /// Only relevant if canBlock is true.
        /// </summary>
        [Category( "Status Block" ), DisplayName( "Block Value" ), DefaultValue( 0 ),
        Description( "Specifies the 'Block Power' of the this.WrappedObject.Statable. Aka. how much damage is blocked on a successful block." )]
        public int BlockValue
        {
            get { return this.WrappedObject.Statable.BlockValue; }
            set
            {
                if( value < 0 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, nameof( value ) );
                }

                this.WrappedObject.Statable.BlockValue = value;
            }
        }


        [Category( "Status" ), DisplayName( "Melee DPS" ),
        Description( "The damage per second the Enemy does in Melee combat." )]
        public float DamagePerSecondMelee
        {
            get
            {
                if( this.AttackSpeedMelee == 0.0f )
                {
                    return 0.0f;
                }

                float average = (this.DamageMeleeMin + this.DamageMeleeMax) / 2.0f;
                return average / this.AttackSpeedMelee;
            }
        }

        [Category( "Status" ), DisplayName( "Ranged DPS" ),
        Description( "The damage per second the Enemy does in Ranged combat." )]
        public float DamagePerSecondRanged
        {
            get
            {
                if( this.AttackSpeedRanged == 0.0f )
                {
                    return 0.0f;
                }

                float average = (this.DamageRangedMin + this.DamageRangedMax) / 2.0f;
                return average / this.AttackSpeedRanged;
            }
        }

        [Category( "Status" ), DisplayName( "Melee Damage Minimum" ),
        Description( "The minimum amount of damage done in Melee Combat. (without armor modifiers)" )]
        public int DamageMeleeMin
        {
            get { return this.WrappedObject.Statable.DamageMeleeMin; }
            set
            {
                if( value < 0 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, nameof( value ) );
                }

                this.WrappedObject.Statable.DamageMeleeMin = value;
            }
        }

        [Category( "Status" ), DisplayName( "Melee Damage Maximum" ),
        Description( "The maximum amount of damage done in Melee Combat. (without armor modifiers)" )]
        public int DamageMeleeMax
        {
            get { return this.WrappedObject.Statable.DamageMeleeMax; }
            set
            {
                if( value < 0 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, nameof( value ) );
                }

                this.WrappedObject.Statable.DamageMeleeMax = value;
            }
        }

        [Category( "Status" ), DisplayName( "Ranged Damage Minimum" ),
        Description( "The minimum amount of damage done in Ranged Combat. (without armor modifiers)" )]
        public int DamageRangedMin
        {
            get { return this.WrappedObject.Statable.DamageRangedMin; }
            set
            {
                if( value < 0 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, nameof( value ) );
                }

                this.WrappedObject.Statable.DamageRangedMin = value;
            }
        }

        [Category( "Status" ), DisplayName( "Ranged Damage Maximum" ),
        Description( "The maximum amount of damage done in Ranged Combat. (without armor modifiers)" )]
        public int DamageRangedMax
        {
            get { return this.WrappedObject.Statable.DamageRangedMax; }
            set
            {
                if( value < 0 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, nameof( value ) );
                }

                this.WrappedObject.Statable.DamageRangedMax = value;
            }
        }

        [Category( "Status" ), DisplayName( "Melee Attack Speed" ),
        Description( "The attack speed in Melee Combat of the this.WrappedObject.Statable." )]
        public float AttackSpeedMelee
        {
            get { return this.WrappedObject.Statable.AttackSpeedMelee; }
            set
            {
                if( value < 0 || value > 10 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsInvalid, nameof( value ) );
                }

                this.WrappedObject.Statable.AttackSpeedMelee = value;
            }
        }

        [Category( "Status" ), DisplayName( "Ranged Attack Speed" ),
        Description( "The attack speed in Ranged Combat of the this.WrappedObject.Statable." )]
        public float AttackSpeedRanged
        {
            get { return this.WrappedObject.Statable.AttackSpeedRanged; }
            set
            {
                if( value < 0 || value > 10 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsInvalid, nameof( value ) );
                }

                this.WrappedObject.Statable.AttackSpeedRanged = value;
            }
        }

        #region DamageBonusMagic

        /// <summary>
        /// Gets or sets the magical damage bonus this <see cref="Enemy"/> has. 
        /// </summary>
        [Category( "Status" ), DisplayName( "Magical Damage Bonus Minimum" ), DefaultValue( 0 ),
        Description( "The amount of Damage Bonus the Enemy gets with Magical attacks." )]
        public IntegerRange DamageBonusMagicMin
        {
            get { return this.WrappedObject.Statable.DamageMagic; }
            set
            {
                if( value.Minimum < 0 || value.Maximum < 0 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, nameof( value ) );
                }

                this.WrappedObject.Statable.DamageMagic = value;
            }
        }

        #endregion

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
                DamageTakenContainer damageTaken = this.WrappedObject.Statable.DamageTaken;
                return damageTaken.FromSchool.GetBase( DamageSchool.Physical );
            }

            set
            {
                DamageTakenContainer damageTaken = this.WrappedObject.Statable.DamageTaken;
                damageTaken.FromSchool.SetBase( DamageSchool.Physical, value );
            }
        }

        [Category( "Damage Taken - School" ), DisplayName( "Taken Magical" ), DefaultValue( 1.0f ),
        Description( "The modifier that is applied to the Magical damage the Enemy takes." )]
        public float DamageTakenFromMagicalDamageSchool
        {
            get
            {
                DamageTakenContainer damageTaken = this.WrappedObject.Statable.DamageTaken;
                return damageTaken.FromSchool.GetBase( DamageSchool.Magical );
            }

            set
            {
                DamageTakenContainer damageTaken = this.WrappedObject.Statable.DamageTaken;
                damageTaken.FromSchool.SetBase( DamageSchool.Magical, value );
            }
        }

        [Category( "Damage Taken - Elemental" ), DisplayName( "Taken Fire" ), DefaultValue( 1.0f ),
        Description( "The modifier that is applied to the Fire damage the Enemy takes." )]
        public float DamageTakenFromFireElement
        {
            get
            {
                DamageTakenContainer damageTaken = this.WrappedObject.Statable.DamageTaken;
                return damageTaken.FromElement.GetBase( ElementalSchool.Fire );
            }

            set
            {
                DamageTakenContainer damageTaken = this.WrappedObject.Statable.DamageTaken;
                damageTaken.FromElement.SetBase( ElementalSchool.Fire, value );
            }
        }

        [Category( "Damage Taken - Elemental" ), DisplayName( "Taken Water" ), DefaultValue( 1.0f ),
        Description( "The modifier that is applied to the Water damage the Enemy takes." )]
        public float DamageTakenFromWaterElement
        {
            get
            {
                DamageTakenContainer damageTaken = this.WrappedObject.Statable.DamageTaken;
                return damageTaken.FromElement.GetBase( ElementalSchool.Water );
            }

            set
            {
                DamageTakenContainer damageTaken = this.WrappedObject.Statable.DamageTaken;
                damageTaken.FromElement.SetBase( ElementalSchool.Water, value );
            }
        }

        [Category( "Damage Taken - Elemental" ), DisplayName( "Taken Light" ), DefaultValue( 1.0f ),
        Description( "The modifier that is applied to the Light damage the Enemy takes." )]
        public float DamageTakenFromLightElement
        {
            get
            {
                DamageTakenContainer damageTaken = this.WrappedObject.Statable.DamageTaken;
                return damageTaken.FromElement.GetBase( ElementalSchool.Light );
            }

            set
            {
                DamageTakenContainer damageTaken = this.WrappedObject.Statable.DamageTaken;
                damageTaken.FromElement.SetBase( ElementalSchool.Light, value );
            }
        }

        [Category( "Damage Taken - Elemental" ), DisplayName( "Taken Shadow" ), DefaultValue( 1.0f ),
        Description( "The modifier that is applied to the Shadow damage the Enemy takes." )]
        public float DamageTakenFromShadowElement
        {
            get
            {
                DamageTakenContainer damageTaken = this.WrappedObject.Statable.DamageTaken;
                return damageTaken.FromElement.GetBase( ElementalSchool.Shadow );
            }

            set
            {
                DamageTakenContainer damageTaken = this.WrappedObject.Statable.DamageTaken;
                damageTaken.FromElement.SetBase( ElementalSchool.Shadow, value );
            }
        }

        [Category( "Damage Taken - Elemental" ), DisplayName( "Taken Nature" ), DefaultValue( 1.0f ),
        Description( "The modifier that is applied to the Nature damage the Enemy takes." )]
        public float DamageTakenFromNatureElement
        {
            get
            {
                DamageTakenContainer damageTaken = this.WrappedObject.Statable.DamageTaken;
                return damageTaken.FromElement.GetBase( ElementalSchool.Nature );
            }

            set
            {
                DamageTakenContainer damageTaken = this.WrappedObject.Statable.DamageTaken;
                damageTaken.FromElement.SetBase( ElementalSchool.Nature, value );
            }
        }

        #endregion

        #region BaseChanceTo

        /// <summary>
        /// Gets or sets the base chance to crit in %.
        /// </summary>
        [Category( "Chance To" ), DisplayName( "Crit" ), DefaultValue( 5.0f ),
         Description( "The chance the Enemy has to Crit with any attack in %." )]
        public float BaseChanceToCrit
        {
            get
            {
                ChanceToContainer chanceTo = this.WrappedObject.Statable.ChanceTo;
                return chanceTo.GetBase( ChanceToStatus.Crit );
            }

            set
            {
                if( value < 0 || value > 100 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsInvalid, nameof( value ) );
                }

                ChanceToContainer chanceTo = this.WrappedObject.Statable.ChanceTo;
                chanceTo.SetBase( ChanceToStatus.Crit, value );
            }
        }

        /// <summary>
        /// Gets or sets the base chance to dodge in %.
        /// </summary>
        [Category( "Chance To" ), DisplayName( "Dodge" ), DefaultValue( 2.5f ),
         Description( "The chance the Enemy has to Dodge melee and ranged attacks in %." )]
        public float BaseChanceToDodge
        {
            get
            {
                ChanceToContainer chanceTo = this.WrappedObject.Statable.ChanceTo;
                return chanceTo.GetBase( ChanceToStatus.Dodge );
            }

            set
            {
                if( value < 0 || value > 100 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsInvalid, nameof( value ) );
                }

                ChanceToContainer chanceTo = this.WrappedObject.Statable.ChanceTo;
                chanceTo.SetBase( ChanceToStatus.Dodge, value );
            }
        }

        /// <summary>
        /// Gets or sets the base chance to parry in %.
        /// </summary>
        [Category( "Chance To" ), DisplayName( "Parry" ), DefaultValue( 2.5f ),
         Description( "The chance the Enemy has to Parry melee attacks in %." )]
        public float BaseChanceToParry
        {
            get
            {
                ChanceToContainer chanceTo = this.WrappedObject.Statable.ChanceTo;
                return chanceTo.GetBase( ChanceToStatus.Parry );
            }

            set
            {
                if( value < 0 || value > 100 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsInvalid, nameof( value ) );
                }

                ChanceToContainer chanceTo = this.WrappedObject.Statable.ChanceTo;
                chanceTo.SetBase( ChanceToStatus.Parry, value );
            }
        }

        /// <summary>
        /// Gets or sets the base chance to miss in %.
        /// </summary>
        [Category( "Chance To" ), DisplayName( "Miss" ), DefaultValue( 5.0f ),
         Description( "The chance the Enemy has to Miss with any attack in %." )]
        public float BaseChanceToMiss
        {
            get
            {
                ChanceToContainer chanceTo = this.WrappedObject.Statable.ChanceTo;
                return chanceTo.GetBase( ChanceToStatus.Miss );
            }

            set
            {
                if( value < 0 || value > 100 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsInvalid, nameof( value ) );
                }

                ChanceToContainer chanceTo = this.WrappedObject.Statable.ChanceTo;
                chanceTo.SetBase( ChanceToStatus.Miss, value );
            }
        }

        #endregion

        #region ChanceToBe

        [Category( "Chance To Be" ), DisplayName( "Crit" ), DefaultValue( 0.0f ),
         Description( "The increased chance the player deosn't crit the Enemy with any attack." )]
        public float ChanceToBeCrit
        {
            get
            {
                return this.WrappedObject.Statable.ChanceToBe.BaseCrit;
            }

            set
            {
                if( value < -100 || value > 100 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsInvalid, nameof( value ) );
                }

                this.WrappedObject.Statable.ChanceToBe.BaseCrit = value;
            }
        }

        [Category( "Chance To Be" ), DisplayName( "Hit" ), DefaultValue( 0.0f ),
         Description( "The increased chance the player deosn't hit the Enemy with any attack." )]
        public float ChanceToBeHit
        {
            get
            {
                return this.WrappedObject.Statable.ChanceToBe.BaseHit;
            }

            set
            {
                if( value < -100 || value > 100 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsInvalid, nameof( value ) );
                }

                this.WrappedObject.Statable.ChanceToBe.BaseHit = value;
            }
        }

        #endregion

        #endregion

        #region > Killable <

        /// <summary>
        /// The experience the EnemyObject gives when killed.
        /// </summary>
        [Category( "Status" ),
         Description( "The experience the player receives when he kills the this.WrappedObject.Statable." )]
        public int Experience
        {
            get { return this.WrappedObject.Killable.Experience; }
            set
            {
                if( value < 0 )
                {
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, nameof( value ) );
                }

                this.WrappedObject.Killable.Experience = value;
            }
        }

        #endregion

        #region > Lootable <

        /// <summary>
        /// Gets or sets the value that specifies how many Items the Enemy can drop at once. 
        /// </summary>
        [Category( "Loot" ), DisplayName( "Item Drop Mode" ), DefaultValue( ItemDropMode.Normal ),
         Description( "Describes how many Items the Enemy can drop at once." )]
        public ItemDropMode ItemDropMode
        {
            get { return this.WrappedObject.Lootable.DropMode; }
            set { this.WrappedObject.Lootable.DropMode = value; }
        }

        [Category( "Loot" ), DisplayName( "Drop Range" ), DefaultValue( 5.0f ),
            Description( "The radius of the circle, centering at the enemy's collision position, in which loot may drop." )]
        public float ItemDropRange
        {
            get { return this.WrappedObject.Lootable.DropRange; }
            set { this.WrappedObject.Lootable.DropRange = value; }
        }

        #endregion

        #region > Visionable <

        [LocalizedDisplayName( "PropDisp_VisionRange" )]
        [LocalizedCategory( "PropCate_Vision" )]
        [LocalizedDescription( "PropDesc_VisionRange" )]
        public int VisionRange
        {
            get { return this.WrappedObject.Visionable.VisionRange; }
            set
            {
                this.WrappedObject.Visionable.VisionRange = value;
            }
        }

        [LocalizedDisplayName( "PropDisp_FeelingRange" )]
        [LocalizedCategory( "PropCate_Vision" )]
        [LocalizedDescription( "PropDesc_FeelingRange" )]
        public int FeelingRange
        {
            get { return this.WrappedObject.Visionable.FeelingRange; }
            set
            {
                this.WrappedObject.Visionable.FeelingRange = value;
            }
        }

        #endregion

        [Category( "Status" ), DisplayName( "Damage Method - Melee" )]
        [Editor( typeof( Design.EnemyAttackDamageMethodEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Zelda.Attacks.AttackDamageMethod DamageMeleeMethod
        {
            get { return this.WrappedObject.MeleeAttack.DamageMethod; }
            set { this.WrappedObject.MeleeAttack.DamageMethod = value; }
        }

        /// <summary>
        /// Applies any additional data for this EnemyPropertyWrapper.
        /// </summary>
        /// <param name="window">
        /// The application's MainWindow.
        /// </param>
        public override void ApplyData( MainWindow window )
        {
            // Apply Loot:
            Lootable lootable = this.WrappedObject.Lootable;
            lootable.Loot.Clear();

            DataGridView dataGridViewLoot = window.DataGridLoot;
            dataGridViewLoot.EndEdit();

            foreach( DataGridViewRow row in dataGridViewLoot.Rows )
            {
                string itemName = (string)row.Cells[0].Value;
                int dropChance = row.Cells[1].Value != null ? (int)row.Cells[1].Value : 0;
                bool magicFindWorks = (bool)row.Cells[2].FormattedValue;

                if( itemName == null )
                {
                    continue;
                }

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
            DataGridView dataGridLoot = window.DataGridLoot;
            LootTable loot = this.WrappedObject.Lootable.Loot;

            // Setup Loot:
            dataGridLoot.Rows.Clear();

            for( int i = 0; i < loot.Count; ++i )
            {
                Atom.Collections.HatEntry<string> entry = loot[i];

                dataGridLoot.Rows.Add(
                    entry.Data,
                    (int)entry.Weight,
                    entry.Id == LootTable.ItemIdAffectedByMagicFind,
                    loot.TotalWeight == 0.0f ? 0.0f : (entry.Weight / loot.TotalWeight) * 100.0f
                );
            }
        }

        /// <summary>
        /// Ensures the correctness of the current state of the Enemy.
        /// </summary>
        /// <returns>
        /// true if the data is in a correct state and can be saved;
        /// otherwise false.
        /// </returns>
        public override bool Ensure()
        {
            if( base.Ensure() )
            {
                return ValidationHelper.ValidateLoot( this.WrappedObject.Lootable.Loot );
            }

            return false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnemyPropertyWrapper"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public EnemyPropertyWrapper( IZeldaServiceProvider serviceProvider )
        {
            if( serviceProvider == null )
            {
                throw new ArgumentNullException( nameof( serviceProvider ) );
            }

            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Returns a clone of this EnemyPropertyWrapper.
        /// </summary>
        /// <returns>The cloned EnemyPropertyWrapper.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new EnemyPropertyWrapper( serviceProvider );
        }

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;
    }
}
