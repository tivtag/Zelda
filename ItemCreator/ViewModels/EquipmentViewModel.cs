// <copyright file="EquipmentViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.ItemCreator.EquipmentViewModel class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.ItemCreator
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using Zelda.Items;

    /// <summary>
    /// Defines the view-model/property wrapper around the <see cref="Equipment"/> class.
    /// </summary>
    internal class EquipmentViewModel : ItemViewModel
    {
        #region [ Wrapped Properties ]
        
        [LocalizedCategory( "PropCate_Equipment" )]
        [LocalizedDisplayName( "PropDisp_Slot" )]
        [LocalizedDescription( "PropDesc_Slot" )]
        public EquipmentSlot Slot
        {
            get { return this.WrappedEquipment.Slot; }
            set { this.WrappedEquipment.Slot = value; }
        }

        [LocalizedCategory( "PropCate_Equipment" )]
        [LocalizedDisplayName( "PropDisp_Armor" )]
        [LocalizedDescription( "PropDesc_Armor" )]
        [DefaultValue( 0 )]
        public int Armor
        {
            get { return this.WrappedEquipment.Armor; }
            set { this.WrappedEquipment.Armor = value; }
        }

        [LocalizedCategory( "PropCate_Equipment" )]
        [LocalizedDisplayName( "PropDisp_ArmorMitigation" )]
        [LocalizedDescription( "PropDesc_ArmorMitigation" )]
        [DefaultValue( 0.0f )]
        public float ArmorMitigation
        {
            get
            {
                return Zelda.Status.StatusCalc.GetMitigationFromArmor( this.WrappedEquipment.Armor, this.WrappedEquipment.Level );
            }
        }

        [LocalizedCategory( "PropCate_Equipment" )]
        [LocalizedDisplayName( "PropDisp_ItemPoints" )]
        [LocalizedDescription( "PropDesc_ItemPoints" )]
        public string ItemPoints
        {
            get
            {
                int aviablePoints = Status.StatusCalc.GetAvailableItemBudget( this.WrappedEquipment.Level, this.WrappedEquipment.Quality, this.WrappedEquipment.Slot );
                int usedPoints    = this.WrappedEquipment.UsedItemBudget;

                return usedPoints.ToString( CultureInfo.CurrentCulture ) + '/' + aviablePoints.ToString( CultureInfo.CurrentCulture );
            }
        }

        [LocalizedCategory( "PropCate_Equipment" )]
        [LocalizedDisplayName( "PropDisp_AdditionalEffectsToggle" )]
        [LocalizedDescription( "PropDesc_AdditionalEffectsToggle" )]
        [DefaultValue( false )]
        public bool AdditionalEffectsToggle
        {
            get { return this.AdditionalEffects != null; }
            set
            {
                if( value == this.AdditionalEffectsToggle )
                    return;

                if( value == true )
                {
                    if( cachedAdditionalEffects == null )
                        cachedAdditionalEffects = new Zelda.Status.PermanentAura();

                    this.WrappedEquipment.AdditionalEffectsAura = cachedAdditionalEffects;
                }
                else
                {
                    cachedAdditionalEffects = this.WrappedEquipment.AdditionalEffectsAura;
                    this.WrappedEquipment.AdditionalEffectsAura = null;
                }
            }
        }

        /// <summary>
        /// Caches the last used Additional Effects of the Equipment.
        /// </summary>
        private Zelda.Status.PermanentAura cachedAdditionalEffects;
        
        /// <summary>
        /// Gets the additional StatusEffects applied when an instance of <see cref="Equipment"/> gets equipped.
        /// </summary>
        /// <value>Is null by default.</value>
        [LocalizedCategory( "PropCate_Equipment" )]
        [LocalizedDisplayName( "PropDisp_AdditionalEffects" )]
        [LocalizedDescription( "PropDesc_AdditionalEffects" )]
        [Editor( "Zelda.Status.Design.StatusEffectListEditor, Library.Design", typeof( System.Drawing.Design.UITypeEditor ) )]
        public System.Collections.Generic.List<Zelda.Status.StatusEffect> AdditionalEffects
        {
            get
            {
                return this.WrappedEquipment.AdditionalEffects;
            }
        }
        
        [LocalizedCategory( "PropCate_EquipmentStats" )]
        [LocalizedDisplayName( "PropDisp_Strength" )]
        [LocalizedDescription( "PropDesc_Strength" )]
        [DefaultValue( 0 )]
        public int Strength
        {
            get { return this.WrappedEquipment.Strength; }
            set { this.WrappedEquipment.Strength = value; }
        }
        
        [LocalizedCategory( "PropCate_EquipmentStats" )]
        [LocalizedDisplayName( "PropDisp_Dexterity" )]
        [LocalizedDescription( "PropDesc_Dexterity" )]
        [DefaultValue( 0 )]
        public int Dexterity
        {
            get { return this.WrappedEquipment.Dexterity; }
            set { this.WrappedEquipment.Dexterity = value; }
        }

        [LocalizedCategory( "PropCate_EquipmentStats" )]
        [LocalizedDisplayName( "PropDisp_Agility" )]
        [LocalizedDescription( "PropDesc_Agility" )]
        [DefaultValue( 0 )]
        public int Agility
        {
            get { return this.WrappedEquipment.Agility; }
            set { this.WrappedEquipment.Agility = value; }
        }

        [LocalizedCategory( "PropCate_EquipmentStats" )]
        [LocalizedDisplayName( "PropDisp_Vitality" )]
        [LocalizedDescription( "PropDesc_Vitality" )]
        [DefaultValue( 0 )]
        public int Vitality
        {
            get { return this.WrappedEquipment.Vitality; }
            set { this.WrappedEquipment.Vitality = value; }
        }

        [LocalizedCategory( "PropCate_EquipmentStats" )]
        [LocalizedDisplayName( "PropDisp_Intelligence" )]
        [LocalizedDescription( "PropDesc_Intelligence" )]
        [DefaultValue( 0 )]
        public int Intelligence
        {
            get { return this.WrappedEquipment.Intelligence; }
            set { this.WrappedEquipment.Intelligence = value; }
        }

        [LocalizedCategory( "PropCate_EquipmentStats" )]
        [LocalizedDisplayName( "PropDisp_Luck" )]
        [LocalizedDescription( "PropDesc_Luck" )]
        [DefaultValue( 0 )]
        public int Luck
        {
            get { return this.WrappedEquipment.Luck; }
            set { this.WrappedEquipment.Luck = value; }
        }

        [LocalizedCategory( "PropCate_Requirements" )]
        [LocalizedDisplayName( "PropDisp_RequiredLevel" )]
        [LocalizedDescription( "PropDesc_RequiredLevel" )]
        [DefaultValue( 0 )]
        public int RequiredLevel
        {
            get { return this.WrappedEquipment.RequiredLevel; }
            set { this.WrappedEquipment.RequiredLevel = value; }
        }

        [LocalizedCategory( "PropCate_Requirements" )]
        [LocalizedDisplayName( "PropDisp_RequiredStrength" )]
        [LocalizedDescription( "PropDesc_RequiredStrength" )]
        [DefaultValue( 0 )]
        public int RequiredStrength
        {
            get { return this.WrappedEquipment.RequiredStrength; }
            set { this.WrappedEquipment.RequiredStrength = value; }
        }

        [LocalizedCategory( "PropCate_Requirements" )]
        [LocalizedDisplayName( "PropDisp_RequiredDexterity" )]
        [LocalizedDescription( "PropDesc_RequiredDexterity" )]
        [DefaultValue( 0 )]
        public int RequiredDexterity
        {
            get { return this.WrappedEquipment.RequiredDexterity; }
            set { this.WrappedEquipment.RequiredDexterity = value; }
        }

        [LocalizedCategory( "PropCate_Requirements" )]
        [LocalizedDisplayName( "PropDisp_RequiredAgility" )]
        [LocalizedDescription( "PropDesc_RequiredAgility" )]
        [DefaultValue( 0 )]
        public int RequiredAgility
        {
            get { return this.WrappedEquipment.RequiredAgility; }
            set { this.WrappedEquipment.RequiredAgility = value; }
        }

        [LocalizedCategory( "PropCate_Requirements" )]
        [LocalizedDisplayName( "PropDisp_RequiredVitality" )]
        [LocalizedDescription( "PropDesc_RequiredVitality" )]
        [DefaultValue( 0 )]
        public int RequiredVitality
        {
            get { return this.WrappedEquipment.RequiredVitality; }
            set { this.WrappedEquipment.RequiredVitality = value; }
        }

        [LocalizedCategory( "PropCate_Requirements" )]
        [LocalizedDisplayName( "PropDisp_RequiredIntelligence" )]
        [LocalizedDescription( "PropDesc_RequiredIntelligence" )]
        [DefaultValue( 0 )]
        public int RequiredIntelligence
        {
            get { return this.WrappedEquipment.RequiredIntelligence; }
            set { this.WrappedEquipment.RequiredIntelligence = value; }
        }

        [LocalizedCategory( "PropCate_Requirements" )]
        [LocalizedDisplayName( "PropDisp_RequiredLuck" )]
        [LocalizedDescription( "PropDesc_RequiredLuck" )]
        [DefaultValue( 0 )]
        public int RequiredLuck
        {
            get { return this.WrappedEquipment.RequiredLuck; }
            set { this.WrappedEquipment.RequiredLuck = value; }
        }

        [LocalizedCategory( "PropCate_EquipmentSockets" )]
        [LocalizedDisplayName( "PropDisp_Sockets" )]
        [LocalizedDescription( "PropDesc_Sockets" )]
        public Socket[] Sockets
        {
            get { return this.WrappedEquipment.SocketProperties.Sockets; }
            set
            {
                if( value != null )
                    this.HasSockets = true;

                this.WrappedEquipment.SocketProperties.Sockets = value;
            }
        }

        [LocalizedCategory( "PropCate_EquipmentSockets" )]
        [LocalizedDisplayName( "PropDisp_SocketBonus" )]
        [LocalizedDescription( "PropDesc_SocketBonus" )]
        public Zelda.Status.PermanentAura SocketBonus
        {
            get { return this.WrappedEquipment.SocketProperties.Bonus; }
        }

        [LocalizedCategory( "PropCate_EquipmentSockets" )]
        [LocalizedDisplayName( "PropDisp_HasSockets" )]
        [LocalizedDescription( "PropDesc_HasSockets" )]
        [DefaultValue(false)]
        public bool HasSockets
        {
            get 
            {
                return this.WrappedEquipment.SocketProperties.Sockets != null; 
            }

            set
            {
                if( value == this.HasSockets )
                    return;

                if( value )
                {
                    if( this.WrappedEquipment.SocketProperties.Sockets == null )
                        this.WrappedEquipment.SocketProperties.Sockets = new Socket[0];

                    if( this.WrappedEquipment.SocketProperties.Bonus == null )
                        this.WrappedEquipment.SocketProperties.Bonus = new Zelda.Status.PermanentAura();
                }
                else
                {
                    if( this.WrappedEquipment.SocketProperties != null )
                    {
                        if( MessageBox.Show(
                                Properties.Resources.Question_ReallyRemoveAllSocketsFromTheEquipment,
                                Zelda.Resources.Question,
                                MessageBoxButton.YesNo 
                            ) == MessageBoxResult.No )
                        {
                            return;
                        }
                    }

                    this.WrappedEquipment.SocketProperties.Sockets     = null;
                    this.WrappedEquipment.SocketProperties.Bonus = null;
                }
            }
        }

        [LocalizedCategory( "PropCate_Equipment" )]
        [LocalizedDisplayName( "PropDisp_AnimationSpriteGroup" )]
        [LocalizedDescription( "PropDesc_AnimationSpriteGroup" )]
        public string AnimationSpriteGroup
        {
            get { return this.WrappedEquipment.AnimationSpriteGroup; }
            set { this.WrappedEquipment.AnimationSpriteGroup = value; }
        }

        [LocalizedCategory( "PropCate_Equipment" )]
        [LocalizedDisplayName( "PropDisp_IngameColor" )]
        [LocalizedDescription( "PropDesc_IngameColor" )]
        public Microsoft.Xna.Framework.Color IngameColor
        {
            get { return this.WrappedEquipment.IngameColor; }
            set { this.WrappedEquipment.IngameColor = value; }
        }

        [LocalizedCategory( "PropCate_Equipment" )]
        [LocalizedDisplayName( "PropDisp_AllowedAffixes" )]
        [LocalizedDescription( "PropDesc_AllowedAffixest" )]
        public Zelda.Items.Affixes.AffixType AllowedAffixes
        {
            get { return this.WrappedEquipment.AllowedAffixes; }
            set { this.WrappedEquipment.AllowedAffixes = value; }
        }

        [LocalizedCategory( "PropCate_Equipment" )]
        [LocalizedDisplayName( "PropDisp_Set" )]
        [LocalizedDescription( "PropDesc_Set" )]
        [Editor( typeof( Zelda.Items.Sets.Design.SetEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Zelda.Items.Sets.ISet Set
        {
            get { return this.WrappedEquipment.Set; }
            set { this.WrappedEquipment.Set = value; }
        }

        #endregion

        #region [ Wrapper ]

        /// <summary>
        /// Gets the Equipment this <see cref="EquipmentViewModel"/> wraps around.
        /// </summary>
        [Browsable( false )]
        public Equipment WrappedEquipment
        {
            get
            {
                return (Equipment)this.WrappedObject;
            }
        }

        /// <summary>
        /// Gets the <see cref="Type"/> this <see cref="IObjectPropertyWrapper"/> wraps around.
        /// </summary>
        public override Type WrappedType
        {
            get
            {
                return typeof( Equipment );
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentViewModel"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game related services.
        /// </param>
        internal EquipmentViewModel( IZeldaServiceProvider serviceProvider )
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
            return new EquipmentViewModel( this.ServiceProvider );
        }

        #endregion
    }
}
