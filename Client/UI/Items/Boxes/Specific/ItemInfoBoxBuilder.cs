// <copyright file="ItemInfoBoxBuilder.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.UI.Items.Boxes.ItemInfoBoxBuilder class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.UI.Items.Boxes
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Atom.Math;
    using Atom.Xna.Fonts;
    using Zelda.Items;
    using Zelda.Status;
    using Xna = Microsoft.Xna.Framework;

    /// <summary>
    /// Implements a mechanism that breaks down the individual fields that descripe
    /// an Item into boxes that can be used to correctly draw the information.
    /// </summary>
    internal sealed class ItemInfoBoxBuilder
    {
        #region [ Constants ]

        /// <summary>
        /// The offset on the x-axis applied to the additional StatusEffects
        /// given by Equipment, Weapons and Gems.
        /// </summary>
        private const int HorizontalEffectOffset = 10;

        /// <summary>
        /// The offset on the x-axis applied to the StatusEffects gems inside
        /// an Equipment or Weapon provide.
        /// </summary>
        private const int HorizontalSocketEffectOffset = 20;

        /// <summary>
        /// The offset on the x-axis applied to the inidividual items
        /// of a set.
        /// </summary>
        private const int HorizontalOffsetSetItems = 3;

        #region > Strings <

        /// <summary>
        /// The format string smiliar to 'Requires {0} Strength'.
        /// </summary>
        private static readonly string FormatRequiresStrength = Resources.RequiresXStrength;

        /// <summary>
        /// The format string smiliar to 'Requires {0} Dexterity'.
        /// </summary>
        private static readonly string FormatRequiresDexterity = Resources.RequiresXDexterity;

        /// <summary>
        /// The format string smiliar to 'Requires {0} Agility'.
        /// </summary>
        private static readonly string FormatRequiresAgility = Resources.RequiresXAgility;

        /// <summary>
        /// The format string smiliar to 'Requires {0} Vitality'.
        /// </summary>
        private static readonly string FormatRequiresVitality = Resources.RequiresXVitality;
        
        /// <summary>
        /// The format string smiliar to 'Requires {0} Vitality'.
        /// </summary>
        private static readonly string FormatRequiresIntelligence = Resources.RequiresXIntelligence;
        
        /// <summary>
        /// The format string smiliar to 'Requires {0} Luck'.
        /// </summary>
        private static readonly string FormatRequiresLuck = Resources.RequiresXLuck;

        #endregion

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the ItemInfoBoxBuilder class.
        /// </summary>
        /// <param name="resources">
        /// The resources used by the new ItemInfoBoxBuilder.
        /// </param>
        public ItemInfoBoxBuilder( ItemInfoResources resources )
        {
            this.resources = resources;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Builds up the IBoxes that are required to visualize the ItemInstance.
        /// </summary>
        /// <param name="itemInstance">
        /// The instance of the Item to visualize.
        /// </param>
        /// <param name="statable">
        /// The ExtendedStatable component of the entity that owns the given ItemInstance.
        /// </param>
        /// <param name="equipmentStatus">
        /// The EquipmentStatus component of the entity that owns the given ItemInstance.
        /// </param>
        /// <returns>
        /// The build model.
        /// </returns>
        public ItemBoxModel Build( ItemInstance itemInstance, ExtendedStatable statable, EquipmentStatus equipmentStatus )
        {
            switch( itemInstance.Item.ItemType )
            {
                case ItemType.Equipment:
                case ItemType.AffixedEquipment:
                    return this.BuildEquipment( (EquipmentInstance)itemInstance, statable, equipmentStatus );

                case ItemType.Weapon:
                case ItemType.AffixedWeapon:
                    return this.BuildWeapon( (WeaponInstance)itemInstance, statable, equipmentStatus );
                    
                case ItemType.Gem:
                    return this.BuildGem( (GemInstance)itemInstance, statable );

                default:
                case ItemType.Item:
                    return this.BuildItem( itemInstance, statable );
            }
        }

        /// <summary>
        /// Builds up the IBoxes that are required to visualize the ItemInstance.
        /// </summary>
        /// <param name="itemInstance">
        /// The instance of the Item to visualize.
        /// </param>
        /// <param name="uncompressedBoxModel">
        /// The BoxModel that has been build normally using <see cref="Build"/>,
        /// before trying to build a compressed one.
        /// </param>
        /// <param name="statable">
        /// The ExtendedStatable component of the entity that owns the given ItemInstance.
        /// </param>
        /// <param name="equipmentStatus">
        /// The EquipmentStatus component of the entity that owns the given ItemInstance.
        /// </param>
        public ItemBoxModel BuildCompressed( ItemInstance itemInstance, ItemBoxModel uncompressedBoxModel, ExtendedStatable statable, EquipmentStatus equipmentStatus )
        {
            this.compressionEnabled = true;
            this.uncompressedBoxModel = uncompressedBoxModel;

            var model = this.Build( itemInstance, statable, equipmentStatus );

            this.compressionEnabled = false;
            this.uncompressedBoxModel = null;
            return model;
        }

        /// <summary>
        /// Builds up the IBoxes that are required to visualize the EquipmentInstance.
        /// </summary>
        /// <param name="equipmentInstance">
        /// The instance of the Equipment to visualize.
        /// </param>
        /// <param name="statable">
        /// The ExtendedStatable component of the entity that owns the given EquipmentInstance.
        /// </param>
        /// <param name="equipmentStatus">
        /// The EquipmentStatus component of the entity that owns the given WeaponInstance.
        /// </param>
        private ItemBoxModel BuildEquipment( EquipmentInstance equipmentInstance, ExtendedStatable statable, EquipmentStatus equipmentStatus )
        {
            this.PreBuild( statable );

            var item = equipmentInstance.Item;
            var equipment = equipmentInstance.Equipment;

            this.AddName( item );

            // Slot.
            this.AddBox(
                new SingleLineLeftRightTextBox(
                    LocalizedEnums.Get( equipment.Slot ),
                    equipmentStatus.HasEquipped( equipmentInstance ) ? "equipped" : string.Empty,
                    0, // additional offset.
                    0, // additional height.
                    this.resources.FontSmallText,
                    Xna.Color.White,
                    Xna.Color.Gray
                )
            );

            this.AddArmor( equipment );
            this.AddStats( equipment );
            this.AddEquipmentEffects( equipment );
            this.AddSocketProperties( equipmentInstance.SocketProperties );
            this.AddRequirements( equipment );

            this.AddUseEffect( item );
            this.AddSet( equipmentInstance );

            RubyBox rubyBox = this.CreateRubyBox( item );
            this.AddDescriptionAndRubyBox( item, rubyBox );

            return this.FinishBuild();
        }
        
        /// <summary>
        /// Builds up the IBoxes that are required to visualize the WeaponInstance.
        /// </summary>
        /// <param name="weaponInstance">
        /// The instance of the Weapon to visualize.
        /// </param>
        /// <param name="statable">
        /// The ExtendedStatable component of the entity that owns the given WeaponInstance.
        /// </param>
        /// <param name="equipmentStatus">
        /// The EquipmentStatus component of the entity that owns the given WeaponInstance.
        /// </param>
        private ItemBoxModel BuildWeapon( WeaponInstance weaponInstance, ExtendedStatable statable, EquipmentStatus equipmentStatus )
        {
            this.PreBuild( statable );

            var item = weaponInstance.Item;
            var equipment = weaponInstance.Equipment;
            var weapon = weaponInstance.Weapon;

            this.AddName( item );

            // Slot & Weapon Type:
            this.AddBox(
                new SingleLineLeftRightTextBox(
                    LocalizedEnums.Get( equipment.Slot ),
                    LocalizedEnums.Get( weapon.WeaponType ),
                    80, // minimum gap,
                    0, // add height.
                    this.resources.FontSmallText,
                    Xna.Color.White
                )
            );

            // Damage Range:
            this.AddBox(
                new SingleLineLeftRightTextBox(
                    this.GetDamageRangeString( weapon ),
                    equipmentStatus.HasEquipped( weaponInstance ) ? "equipped" : string.Empty,
                    1, // offset
                    0, // width
                    this.resources.FontSmallText,
                    Xna.Color.White,
                    Xna.Color.Gray
                )
            );

            this.AddDpsAndSpeed( weapon );

            this.AddArmor( equipment );
            this.AddStats( equipment );

            this.AddEquipmentEffects( equipment );

            this.AddSocketProperties( weaponInstance.SocketProperties );
            this.AddRequirements( equipment );

            this.AddUseEffect( item );
            this.AddSet( weaponInstance );

            RubyBox rubyBox = this.CreateRubyBox( item );
            this.AddDescriptionAndRubyBox( item, rubyBox );

            return this.FinishBuild();
        }

        /// <summary>
        /// Builds up the IBoxes that are required to visualize the GemInstance.
        /// </summary>
        /// <param name="gemInstance">
        /// The instance of the Gem to visualize.
        /// </param>
        /// <param name="statable">
        /// The ExtendedStatable component of the entity that owns the given GemInstance.
        /// </param>
        private ItemBoxModel BuildGem( GemInstance gemInstance, ExtendedStatable statable )
        {
            this.PreBuild( statable );

            var gem = gemInstance.Gem;
            var item = gemInstance.Item;

            this.AddName( item );

            // Gem Type:
            this.AddBox(
                new TextBox(
                    LocalizedEnums.GetGemType( gem.GemColor ),
                    0, // additional offset.
                    25, // additional width.
                    this.resources.FontSmallText,
                    Xna.Color.White,
                    TextAlign.Left
                )
            );

            this.AddEffects( gem.EffectAura );
            this.AddLevelRequirement( gem.RequiredLevel );

            RubyBox rubyBox = this.CreateRubyBox( item );
            this.AddDescriptionAndRubyBox( item, rubyBox );

            return this.FinishBuild();
        }

        /// <summary>
        /// Builds up the IBoxes that are required to visualize the ItemInstance.
        /// </summary>
        /// <param name="itemInstance">
        /// The instance of the Item to visualize.
        /// </param>
        /// <param name="statable">
        /// The ExtendedStatable component of the entity that owns the given ItemInstance.
        /// </param>
        private ItemBoxModel BuildItem( ItemInstance itemInstance, ExtendedStatable statable )
        {
            this.PreBuild( statable );
            var item = itemInstance.Item;

            this.AddName( item );
            this.AddUseEffect( item );

            RubyBox rubyBox = this.CreateRubyBox( item );
            this.AddDescriptionAndRubyBox( item, rubyBox );

            return this.FinishBuild();
        }

        /// <summary>
        /// Adds an IBox to the BoxModel that contains the name of the given Item.
        /// </summary>
        /// <param name="item">
        /// The item that is currently processed.
        /// </param>
        private void AddName( Item item )
        {
            this.AddBox(
                new ItemNameTextBox(
                    item.LocalizedName,
                    this.resources.FontLargeText,
                    this.resources.FontSmallBoldText,
                    UIColors.Get( item.Quality )
                )
            );
        }

        /// <summary>
        /// Adds, if required, an IBox to the BoxModel that contains the
        /// amount of armor the given equipment gives.
        /// </summary>
        /// <param name="equipment">
        /// The equipment that is currently processed.
        /// </param>
        private void AddArmor( Equipment equipment )
        {
            if( equipment.Armor > 0 )
            {
                string armorString = string.Format(
                    this.culture,
                    Resources.FormatStringArmor,
                    equipment.Armor.ToString( culture )
                );

                this.AddBox(
                    new SingleLineTextBox(
                        armorString,
                        this.resources.FontSmallText,
                        Xna.Color.White,
                        TextAlign.Left
                    )
                );
            }
        }

        /// <summary>
        /// Adds, if required, IBoxes to the BoxModel that contain
        /// the individual stats of the given Equipment.
        /// </summary>
        /// <param name="equipment">
        /// The equipment that is currently processed.
        /// </param>
        private void AddStats( Equipment equipment )
        {
            this.BeginBoxGroup();

            this.AddStats( equipment.Strength, Stat.Strength );
            this.AddStats( equipment.Dexterity, Stat.Dexterity );
            this.AddStats( equipment.Agility, Stat.Agility );
            this.AddStats( equipment.Vitality, Stat.Vitality );
            this.AddStats( equipment.Intelligence, Stat.Intelligence );
            this.AddStats( equipment.Luck, Stat.Luck );

            this.EndBoxGroup();
        }

        /// <summary>
        /// Adds, if required, an IBox to the BoxModel that contains the given Stat.
        /// </summary>
        /// <param name="amount">
        /// The amount of the stat.
        /// </param>
        /// <param name="stat">
        /// The stat to add.
        /// </param>
        private void AddStats( int amount, Stat stat )
        {
            if( amount != 0 )
            {
                this.AddBox(
                    new SingleLineTextBox(
                        this.GetStatString( amount, stat ),
                        HorizontalEffectOffset,
                        0,
                        this.resources.FontSmallText,
                        amount >= 0 ? ItemInfoColors.Effect : ItemInfoColors.BadEffect,
                        TextAlign.Left
                    )
                );
            }
        }

        /// <summary>
        /// Adds, if required, IBoxes to the BoxModel that descripe
        /// the requirements of the given Equipment.
        /// </summary>
        /// <param name="equipment">
        /// The equipment to process.
        /// </param>
        private void AddRequirements( Equipment equipment )
        {
            if( equipment.RequiredStrength > this.statable.Strength )
            {
                this.AddStatRequirement( equipment.RequiredStrength, FormatRequiresStrength );
            }

            if( equipment.RequiredDexterity > this.statable.Dexterity )
            {
                this.AddStatRequirement( equipment.RequiredDexterity, FormatRequiresDexterity );
            }

            if( equipment.RequiredVitality > this.statable.Vitality )
            {
                this.AddStatRequirement( equipment.RequiredVitality, FormatRequiresVitality );
            }

            if( equipment.RequiredAgility > this.statable.Agility )
            {
                this.AddStatRequirement( equipment.RequiredAgility, FormatRequiresAgility );
            }

            if( equipment.RequiredIntelligence > this.statable.Intelligence )
            {
                this.AddStatRequirement( equipment.RequiredIntelligence, FormatRequiresIntelligence );
            }

            if( equipment.RequiredLuck > this.statable.Luck )
            {
                this.AddStatRequirement( equipment.RequiredLuck, FormatRequiresLuck );
            }

            this.AddLevelRequirement( equipment.RequiredLevel );
        }

        /// <summary>
        /// Adds an IBox that descripes a stat requirement of the Equipment or Weapon.
        /// </summary>
        /// <param name="stat">
        /// The amount required.
        /// </param>
        /// <param name="formatString">
        /// The format string to use to create the description text.
        /// </param>
        private void AddStatRequirement( int stat, string formatString )
        {
            string requiredStatString = string.Format(
                this.culture,
                formatString,
                stat.ToString( culture )
            );

            this.AddBox(
                new SingleLineTextBox(
                    requiredStatString,
                    this.resources.FontSmallText,
                    ItemInfoColors.RequirementNotFulfilled,
                    TextAlign.Left
                )
            );
        }

        /// <summary>
        /// Adds, if required, an IBox that descripes that the item requires
        /// a specific level.
        /// </summary>
        /// <param name="requiredLevel">
        /// The level required by the Weapon, Equipment or Gem.
        /// </param>
        private void AddLevelRequirement( int requiredLevel )
        {
            if( requiredLevel > this.statable.Level )
            {
                string requiredLevelString = string.Format(
                    culture,
                    Resources.RequiresLevelX,
                    requiredLevel.ToString( culture )
                );

                this.AddBox(
                    new SingleLineTextBox(
                        requiredLevelString,
                        this.resources.FontSmallText,
                        ItemInfoColors.RequirementNotFulfilled,
                        TextAlign.Left
                   )
                );
            }
        }

        /// <summary>
        /// Adds, if required, an IBox to the BoxModel that contains a description
        /// of the UseEffect of the given Item.
        /// </summary>
        /// <param name="item">
        /// The item that is currently processed.
        /// </param>
        private void AddUseEffect( Item item )
        {
            if( item.UseEffect != null )
            {
                this.AddBox(
                    new TextBox(
                        this.GetString( item.UseEffect ),
                        this.resources.FontText,
                        Xna.Color.LightBlue,
                        TextAlign.Center
                    )
                );
            }
        }

        /// <summary>
        /// Adds, if required, IBoxes to the BoxModel that descripe the set properties of the item.
        /// </summary>
        /// <param name="equipmentInstance">
        /// The equipment to process.
        /// </param>
        private void AddSet( EquipmentInstance equipmentInstance )
        {
            var equipment = equipmentInstance.Equipment;
            var set = equipment.Set;

            if( set != null )
            {
                this.AddBox(
                    new SingleLineTextBox(
                        set.LocalizedName,
                        this.resources.FontSmallBoldText,
                        ItemInfoColors.Set,
                        TextAlign.Left
                    )
                );

                foreach( var setItem in set.Items )
                {
                    this.AddBox(
                       new SingleLineTextBox(
                           setItem.LocalizedName,
                           HorizontalOffsetSetItems,
                           0,
                           this.resources.FontSmallText,
                           statable.Equipment.HasEquipped( setItem.Name ) ? ItemInfoColors.Set : ItemInfoColors.SetItemNotEquipped,
                           TextAlign.Left
                       )
                   );
                }

                var statusSetBonus = equipment.Set.Bonus as Zelda.Items.Sets.IStatusSetBonus;

                if( statusSetBonus != null )
                {
                    foreach( var effect in statusSetBonus.Effects )
                    {
                        this.AddBox(
                            new TextBox(
                                effect.GetDescription( this.statable ),
                                HorizontalEffectOffset,
                                0, // add. width
                                -1, // add. height
                                this.resources.FontSmallText,
                                statusSetBonus.IsApplied ? ItemInfoColors.SetEffect : ItemInfoColors.SetItemNotEquipped,
                                TextAlign.Left
                            )
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Adds, if required, IBoxes to the BoxModel that descripe the gems and sockets of the item.
        /// </summary>
        /// <param name="socketProperties">
        /// The ItemSocketProperties to process.
        /// </param>
        private void AddSocketProperties( ItemSocketProperties socketProperties )
        {
            if( socketProperties.HasGems )
            {
                this.AddEffects(
                    socketProperties.MergedEffects,
                    socketProperties.MergedEffectCount,
                    ItemInfoColors.GemEffect,
                    ItemInfoColors.BadEffect,
                    HorizontalSocketEffectOffset
                );
            }

            foreach( var emptySocket in socketProperties.EmptySockets )
            {
                string desc = string.Format(
                    culture,
                    Resources.EmptySocketX,
                    LocalizedEnums.Get( emptySocket.Color )
                );

                this.AddBox(
                    new SingleLineTextBox(
                        desc,
                        HorizontalSocketEffectOffset,
                        0,
                        -1, // add. height
                        this.resources.FontSmallText,
                        ItemInfoColors.BadEffect,
                        TextAlign.Left
                    )
                );
            }
        }

        /// <summary>
        /// Adds IBoxes to the BoxModel containing the given StatusEffects.
        /// </summary>
        /// <param name="effects">
        /// The effects to process.
        /// </param>
        /// <param name="effectCount">
        /// The number of StatusEffects. Used for optimization.
        /// </param>
        /// <param name="colorEffect">
        /// The color of the effect is it is considered 'good'.
        /// </param>
        /// <param name="colorBadEffect">
        /// The color of the effect is it is considered 'bad'.
        /// </param>
        /// <param name="horizontalOffset">
        /// The offset on the x-axis before the effect text.
        /// </param>
        private void AddEffects( 
            IEnumerable<StatusEffect> effects, 
            int effectCount,
            Xna.Color colorEffect,
            Xna.Color colorBadEffect,
            int horizontalOffset )
        {
            var effectBoxes = new List<IBox>( effectCount );

            foreach( var effect in effects )
            {
                IBox box = CreateTextBox( 
                    effect.GetDescription( statable ),
                    horizontalOffset,
                    this.resources.FontSmallText,
                    effect.IsBad ? colorBadEffect : colorEffect
                );

                effectBoxes.Add( box );
            }

            if( this.compressionEnabled )
            {
                effectBoxes.Sort( CompareBoxesByWidth );
            }

            this.BeginBoxGroup();

            for( int i = 0; i < effectBoxes.Count; ++i )
            {
                this.AddBox( effectBoxes[i] );
            }

            this.EndBoxGroup();
        }
        
        /// <summary>
        /// Adds, if required, IBoxes to the BoxModel that descripe the
        /// effect of the given PermanentAura.
        /// </summary>
        /// <param name="aura">
        /// The effect aura to process.
        /// </param>
        private void AddEffects( PermanentAura aura )
        {
            if( aura != null )
            {
                this.AddEffects( 
                    aura.Effects, 
                    aura.Effects.Count,
                    ItemInfoColors.Effect,
                    ItemInfoColors.BadEffect, 
                    HorizontalEffectOffset 
                );
            }
        }

        /// <summary>
        /// Adds, if required, IBoxes to the BoxModel that descripe
        /// the additional StatusEffects the given Equipment provides.
        /// </summary>
        /// <param name="equipment">
        /// The equipment to process.
        /// </param>
        private void AddEquipmentEffects( Equipment equipment )
        {
            var aura = equipment.AdditionalEffectsAura;

            this.AddEffects( aura );
        }

        /// <summary>
        /// Adds a box containing the description of the given Item
        /// and the given RubyBox to the BoxModel.
        /// </summary>
        /// <param name="item">
        /// The item that is currently processed.
        /// </param>
        /// <param name="rubyBox">
        /// The RubyBox of the item currently processed.
        /// </param>
        private void AddDescriptionAndRubyBox( Item item, RubyBox rubyBox )
        {
            if( item.LocalizedDescriptionSplit != null )
            {
                if( !(this.compressionEnabled && item.LocalizedDescriptionSplit.Length >= 3) )
                {
                    this.AddBox(
                        new MultiLineTextBox(
                             item.LocalizedDescriptionSplit,
                             rubyBox != null ? rubyBox.Size.X : 0, // additional size.
                             this.resources.FontSmallText,
                             ItemInfoColors.Description,
                             TextAlign.Center
                        )
                    );
                }
            }
            else if( !string.IsNullOrEmpty( item.LocalizedDescription ) )
            {
                this.AddBox(
                    new TextBox(
                        item.LocalizedDescription,
                        0, // horizontal offset.
                        rubyBox != null ? rubyBox.Size.X : 0, // additional size.
                        this.resources.FontSmallText,
                        ItemInfoColors.Description,
                        TextAlign.Center
                    )
                );
            }

            if( rubyBox != null )
            {
                this.AddBox( rubyBox );
            }
        }

        /// <summary>
        /// Creates the RubyBox for the given Item.
        /// </summary>
        /// <param name="item">
        /// The item that is currently processed.
        /// </param>
        /// <returns>
        /// The newly created RubyBox.
        /// </returns>
        private RubyBox CreateRubyBox( Item item )
        {
            if( item.RubiesWorth > 0 )
            {
                return new RubyBox(
                    this.GetRubyBoxHeight( item ),
                    item.RubiesWorth.ToString( culture ),
                    this.resources.SpriteRuby,
                    this.resources.FontSmallText
                );
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// Adds an IBox to the BoxModel that descripes the dps andd speed of the given Weapon.
        /// </summary>
        /// <param name="weapon">
        /// The weapon to process.
        /// </param>
        private void AddDpsAndSpeed( Weapon weapon )
        {
            string dpsString = System.Math.Round( weapon.DamagePerSecond, 3 ).ToString( culture ) + " DPS";
            string speedString = weapon.AttackSpeed.ToString( culture ) + ' ' + Resources.Speed;

            this.AddBox(
                new SingleLineLeftRightTextBox(
                    dpsString,
                    speedString,
                    30, // horizontal gap,
                    2, // add. height.
                    this.resources.FontSmallText,
                    Xna.Color.White
                )
            );
        }

        /// <summary>
        /// Gets a string descriping the damage range of the given Weapon.
        /// </summary>
        /// <param name="weapon">
        /// The weapon to process.
        /// </param>
        /// <returns>
        /// A string descriping the damage range of the given Weapon.
        /// </returns>
        private string GetDamageRangeString( Weapon weapon )
        {
            return weapon.DamageMinimum.ToString( this.culture ) + " - " +
                   weapon.DamageMaximum.ToString( this.culture ) + ' ' + Resources.Damage;
        }

        /// <summary>
        /// Calculates and returns the size of the RubyBox.
        /// </summary>
        /// <param name="item">
        /// The item that is currently processed.
        /// </param>
        /// <returns>
        /// The height of the RubyBox in pixels.
        /// </returns>
        private int GetRubyBoxHeight( Item item )
        {
            if( item.LocalizedDescriptionSplit == null )
            {
                return this.resources.SpriteRuby.Height;
            }

            var lastSplit = item.LocalizedDescriptionSplit[item.LocalizedDescriptionSplit.Length - 1];
            return (lastSplit.Length <= 1) ? -4 : 0;
        }

        /// <summary>
        /// Gets a string that descripes the given ItemUseEffect.
        /// </summary>
        /// <param name="useEffect">
        /// The ItemUseEffect to descripe.
        /// </param>
        /// <returns>
        /// A string that descripes the given ItemUseEffect.
        /// </returns>
        private string GetString( ItemUseEffect useEffect )
        {
            string str = useEffect.GetDescription( this.statable );

            if( useEffect.Cooldown != null )
            {
                if( !useEffect.Cooldown.IsReady )
                {
                    str += "\n" + useEffect.Cooldown.ToString();
                }
            }

            return str;
        }

        /// <summary>
        /// Gets the string to show for the given Stat.
        /// </summary>
        /// <param name="amount">
        /// The amount of the stat.
        /// </param>
        /// <param name="stat">
        /// Specifies what kind of stat is meant.
        /// </param>
        /// <returns>
        /// A string descriping the input parameters.
        /// </returns>
        private string GetStatString( int amount, Stat stat )
        {
            return string.Format(
                this.culture,
                amount >= 0 ? "+{0} {1}" : "{0} {1}",
                amount.ToString( culture ),
                LocalizedEnums.Get( stat )
            );
        }

        /// <summary>
        /// Setups this ItemInfoBoxBuilder before a build operation
        /// is executed.
        /// </summary>
        /// <param name="statable">
        /// The ExtendedStatable component of the entity that owns the ItemInstance
        /// whose box model is about to be build.
        /// </param>
        private void PreBuild( ExtendedStatable statable )
        {
            this.statable = statable;
            this.boxes.Clear();
        }

        /// <summary>
        /// Marks the beginning of a group of related IBoxes.
        /// </summary>
        private void BeginBoxGroup()
        {
            if( this.compressionEnabled )
            {
                this.AddBox( BeginCompressionGroupSpacer.Instance );
            }
        }

        /// <summary>
        /// Marks the end of a group of related IBoxes.
        /// </summary>
        private void EndBoxGroup()
        {
            if( this.compressionEnabled )
            {
                this.AddBox( EndCompressionGroupSpacer.Instance );
            }
        }

        /// <summary>
        /// Finishes building the ItemBoxModel.
        /// </summary>
        /// <returns>
        /// The BoxModel that has been build.
        /// </returns>
        private ItemBoxModel FinishBuild()
        {
            if( this.compressionEnabled )
            {
                this.CompressAdjacentSingleLineBoxesWithinSameGroup();
            }

            return new ItemBoxModel( this.boxes );
        }

        /// <summary>
        /// Tries to compress adjacent SingleLineTextBox pairs that are within the 
        /// same group of elements indicated by CompressionGroupSpacers.
        /// </summary>
        private void CompressAdjacentSingleLineBoxesWithinSameGroup()
        {
            IBox current = null;
            IBox last = null;
            bool inBlock = false;

            for( int index = 0; index < this.boxes.Count; ++index )
            {
                last = current;
                current = this.boxes[index];

                if( current is EndCompressionGroupSpacer )
                {                    
                    inBlock = false;
                    continue;
                }

                if( current is BeginCompressionGroupSpacer )
                {
                    if( inBlock )
                    {
                        throw new InvalidOperationException( 
                            "Found a BeginCompressionGroupSpacer while already beeing in a CompressionBlock."
                        );
                    }

                    inBlock = true;
                }

                if( !inBlock )
                {
                    continue;
                }
               
                var currentC = current as SingleLineTextBox;
                var lastC = last as SingleLineTextBox;

                if( currentC != null && lastC != null )
                {
                    if( ShouldCompress( currentC, lastC ) )
                    {
                        current = new CompressedSingleLineTextBox( currentC, lastC );

                        // Remove last.
                        this.boxes.RemoveAt( index - 1 );

                        // Remove current.
                        this.boxes.RemoveAt( index - 1 );

                        // Add compressed.
                        this.boxes.Insert( index - 1, current );

                        --index;
                    }
                }
            }
        }

        /// <summary> 
        /// Gets a value indicating whether the specified SingleLineTextBoxes should
        /// be compressed into a single CompressedSingleLineTextBox.
        /// </summary>
        /// <param name="currentC">
        /// The currently processed box.
        /// </param>
        /// <param name="lastC">
        /// The previously processed box.
        /// </param>
        /// <returns>
        /// true if they should be compressed;
        /// otherwise false.
        /// </returns>
        private bool ShouldCompress( SingleLineTextBox currentC, SingleLineTextBox lastC )
        {            
            Point2 currentSize = currentC.Size;
            Point2 lastSize = lastC.Size;

            if( currentSize.Y == lastSize.Y )
            {
                Point2 compressedSize = new Point2(
                    currentSize.X + 15 + lastSize.X,
                    currentSize.Y
                );

                float ratio = (compressedSize.X / (float)this.uncompressedBoxModel.Size.X);
                return ratio <= 0.65f;
            }

            return false;
        }

        /// <summary>
        /// Adds the given IBox ontop of the list of boxes.
        /// </summary>
        /// <param name="box">
        /// The box to add.
        /// </param>
        private void AddBox( IBox box )
        {
            this.boxes.Add( box );
        }

        /// <summary>
        /// Utility method that based on the content of the given text creates
        /// a left-aligned TextBox or SingleLineTextBox.
        /// </summary>
        /// <param name="text">
        /// The text that should be fit into the new TextBox.
        /// </param>
        /// <param name="horizontalOffset">
        /// The additional horizontal pixel offset before the text.
        /// </param>
        /// <param name="font">
        /// The IFont that should be used to draw the specified text.
        /// </param>
        /// <param name="color">
        /// The color of the text.
        /// </param>
        /// <returns>
        /// The newly created TextBox or SingleLineTextBox.
        /// </returns>
        private static IBox CreateTextBox( string text, int horizontalOffset, IFont font, Xna.Color color )
        {
            if( text.Contains( "\n" ) )
            {
                return new TextBox(
                    text,
                    horizontalOffset,
                    0,
                    0,
                    font,
                    color,
                    TextAlign.Left
                );
            }
            else
            {
                return new SingleLineTextBox(
                    text,
                    horizontalOffset,
                    0,
                    0,
                    font,
                    color,
                    TextAlign.Left
                );
            }
        }

        /// <summary>
        /// Compares the given IBoxes by their pixel width.
        /// </summary>
        /// <param name="first">
        /// The first IBox.
        /// </param>
        /// <param name="second">
        /// The second IBox.
        /// </param>
        /// <returns>
        /// A signed value that compares the width of the given boxes relative to each other.
        /// </returns>
        private static int CompareBoxesByWidth( IBox first, IBox second )
        {
            if( first == second )
                return 0;

            return first.Size.X.CompareTo( second.Size.X );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Captures the ExtendedStatable component that wants to visualize the ItemInstance
        /// currently build up.
        /// </summary>
        private ExtendedStatable statable;

        /// <summary>
        /// The ItemBoxModel of the previously build BoxModel while compression was disabled.
        /// Is used during compression.
        /// </summary>
        private ItemBoxModel uncompressedBoxModel;
        
        /// <summary>
        /// States whether this ItemInfoBoxBuilder should try to keep the size of the output BoxModel small.
        /// </summary>
        private bool compressionEnabled;

        /// <summary>
        /// The list of IBoxes that have been build up.
        /// </summary>
        private readonly List<IBox> boxes = new List<IBox>();
        
        /// <summary>
        /// The culture used when formatting strings.
        /// </summary>
        private readonly CultureInfo culture = CultureInfo.CurrentCulture;

        /// <summary>
        /// The resources that are used to visualize item information.
        /// </summary>
        private readonly ItemInfoResources resources;

        #endregion
    }
}