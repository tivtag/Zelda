// <copyright file="Equipment.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Equipment class.
// </summary>
// <author>Paul Ennemoser</author>

// TODO: Move ingame animation and ingame color into own class.

namespace Zelda.Items
{
    using System;
    using System.ComponentModel;
    using Atom;
    using Atom.Diagnostics;
    using Atom.Xna;
    using Zelda.Status;

    /// <summary>
    /// An <see cref="Equipment"/> is an <see cref="Item"/> that can be equipped.
    /// </summary>
    public class Equipment : Item
    {
        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="ItemType"/> this <see cref="Equipment"/> represents.
        /// </summary>
        public override ItemType ItemType
        {
            get
            { 
                return ItemType.Equipment; 
            }
        }

        /// <summary>
        /// Gets or sets the slot this <see cref="Equipment"/> fits in.
        /// </summary>
        public EquipmentSlot Slot
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that represents the amount of armor this <see cref="Equipment"/>
        /// provides when equipped.
        /// </summary>
        public int Armor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the PermanentAura that holds the additional StatusEffects
        /// applied when an instance of this <see cref="Equipment"/> gets equipped.
        /// </summary>
        /// <value>Is null by default.</value>
        [Browsable(false)]
        public PermanentAura AdditionalEffectsAura
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the additional StatusEffects applied when an instance of <see cref="Equipment"/> gets equipped.
        /// </summary>
        /// <value>Is null by default.</value>
        [Editor( "Zelda.Status.Design.StatusEffectListEditor, Library.Design", typeof( System.Drawing.Design.UITypeEditor ) )]
        public System.Collections.Generic.List<StatusEffect> AdditionalEffects
        {
            get
            {
                if( this.AdditionalEffectsAura == null )
                    return null;

                return this.AdditionalEffectsAura.Effects;
            }
        }

        /// <summary>
        /// Gets the <see cref="ItemSocketProperties"/> associated with this Equipment.
        /// </summary>
        public ItemSocketProperties SocketProperties
        {
            get
            {
                return this.sockets;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Zelda.Items.Sets.ISet"/> this <see cref="Equipment"/> is part of.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        public Zelda.Items.Sets.ISet Set
        {
            get;
            set;
        }

        #region > Stats <

        /// <summary>
        /// Gets or sets the strength this <see cref="Equipment"/>
        /// provides when equipped.
        /// </summary>
        public int Strength
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the dexterity this <see cref="Equipment"/>
        /// provides when equipped.
        /// </summary>
        public int Dexterity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the agility this <see cref="Equipment"/>
        /// provides when equipped.
        /// </summary>
        public int Agility
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the vitality this <see cref="Equipment"/>
        /// provides when equipped.
        /// </summary>
        public int Vitality
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the intelligence this <see cref="Equipment"/>
        /// provides when equipped.
        /// </summary>
        public int Intelligence
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the luck this <see cref="Equipment"/>
        /// provides when equipped.
        /// </summary>
        public int Luck
        {
            get;
            set;
        }

        #endregion

        #region > Requirements <

        /// <summary>
        /// Gets or sets the level required by this <see cref="Equipment"/>.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Set: If the given value is negative.
        /// </exception>
        public int RequiredLevel
        {
            get 
            { 
                return this.requiredLevel;
            }

            set
            {
                if( value < 0 )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "value" );

                this.requiredLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets the strength required by this <see cref="Equipment"/>.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Set: If the given value is negative.
        /// </exception>
        public int RequiredStrength
        {
            get
            {
                return this.requiredStrength;
            }

            set
            {
                if( value < 0 )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "value" );

                this.requiredStrength = value;
            }
        }

        /// <summary>
        /// Gets or sets the dexterity required by this <see cref="Equipment"/>.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Set: If the given value is negative.
        /// </exception>
        public int RequiredDexterity
        {
            get 
            { 
                return this.requiredDexterity;
            }

            set
            {
                if( value < 0 )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "value" );

                this.requiredDexterity = value;
            }
        }

        /// <summary>
        /// Gets or sets the agility required by this <see cref="Equipment"/>.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Set: If the given value is negative.
        /// </exception>
        public int RequiredAgility
        {
            get
            {
                return this.requiredAgility;
            }

            set
            {
                if( value < 0 )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "value" );

                this.requiredAgility = value;
            }
        }

        /// <summary>
        /// Gets or sets the vitality required by this <see cref="Equipment"/>.
        /// </summary>
        public int RequiredVitality
        {
            get
            {
                return this.requiredVitality; 
            }

            set
            {
                if( value < 0 )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "value" );

                this.requiredVitality = value;
            }
        }

        /// <summary>
        /// Gets or sets the intelligence required by this <see cref="Equipment"/>.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Set: If the given value is negative.
        /// </exception>
        public int RequiredIntelligence
        {
            get 
            {
                return this.requiredIntelligence; 
            }

            set
            {
                if( value < 0 )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "value" );

                this.requiredIntelligence = value;
            }
        }

        /// <summary>
        /// Gets or sets the luck required by this <see cref="Equipment"/>.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Set: If the given value is negative.
        /// </exception>
        public int RequiredLuck
        {
            get 
            { 
                return this.requiredLuck; 
            }
            
            set
            {
                if( value < 0 )
                    throw new ArgumentException( Atom.ErrorStrings.SpecifiedValueIsNegative, "value" );

                this.requiredLuck = value;
            }
        }

        #endregion

        #region > Animations <

        /// <summary>
        /// Gets or sets the color the weapon is tinted in ingame - on the character.
        /// </summary>
        /// <value>The default color is White.</value>
        public Microsoft.Xna.Framework.Color IngameColor
        {
            get
            {
                return this.ingameColor;
            }

            set
            {
                this.ingameColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the sprite group used to display the <see cref="Equipment"/>
        /// in game on the character.
        /// </summary>
        /// <value> Is null by default.</value>
        public string AnimationSpriteGroup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Equipment"/> 
        /// has any animations displayed ingame on the PlayerEntity.
        /// </summary>
        public bool HasIngameAnimations
        {
            get 
            { 
                return this.AnimationSpriteGroup != null;
            }
        }

        /// <summary>
        /// Gets the animation that gets shown ingame on the character.
        /// </summary>
        /// <value>The default value is null.</value>
        public AnimatedSprite AnimationDown
        {
            get 
            {
                return this.animDown;
            }
        }

        /// <summary>
        /// Gets the animation that gets shown ingame on the character.
        /// </summary>
        /// <value>The default value is null.</value>
        public AnimatedSprite AnimationUp
        {
            get
            { 
                return this.animUp; 
            }
        }

        /// <summary>
        /// Gets the animation that gets shown ingame on the character.
        /// </summary>
        /// <value>The default value is null.</value>
        public AnimatedSprite AnimationLeft
        {
            get
            {
                return this.animLeft;
            }
        }

        /// <summary>
        /// Gets the animation that gets shown ingame on the character.
        /// </summary>
        /// <value>The default value is null.</value>
        public AnimatedSprite AnimationRight
        {
            get 
            {
                return this.animRight;
            }
        }

        #endregion

        #region > ItemBudget <

        /// <summary>
        /// Gets a value that represents how many 'Item Points' this <see cref="Equipment"/> uses.
        /// </summary>
        public int UsedItemBudget
        {
            get
            {
                double budget = StatusCalc.GetItemBudgetUsed( 
                    this.Strength, 
                    this.Dexterity, 
                    this.Agility,
                    this.Vitality, 
                    this.Intelligence,
                    this.Luck 
                );

                if( UseEffect != null )
                    budget += UseEffect.ItemBudgetUsed;

                return (int)budget;
            }
        }

        #endregion

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the Equipment class.
        /// </summary>
        public Equipment()
        {
            this.sockets = new ItemSocketProperties( this );
            this.AllowedAffixes = Zelda.Items.Affixes.AffixType.Both;
        }

        /// <summary>
        /// Creates a new instance of this <see cref="Equipment"/>.
        /// </summary>
        /// <param name="powerFactor">
        /// The factor by which the power of the new ItemInstance varies compared to this base Item.
        /// </param>
        /// <returns>A new <see cref="EquipmentInstance"/>.</returns>
        public override ItemInstance CreateInstance( float powerFactor )
        {
            return new EquipmentInstance( this, powerFactor );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Modifies the power of this Equipment by the given factor.
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
                this.ModifyStatPowerBy( factor );
                this.Armor = (int)(this.Armor * factor);
                
                if( this.AdditionalEffectsAura != null )
                {
                    this.AdditionalEffectsAura.ModifyPowerBy( factor );
                }

                return true;
            }

            return false;
        }

        private void ModifyStatPowerBy( float powerFactor )
        {
            this.Strength     = (int)(this.Strength * powerFactor);
            this.Dexterity    = (int)(this.Dexterity * powerFactor);
            this.Vitality     = (int)(this.Vitality * powerFactor);
            this.Agility      = (int)(this.Agility * powerFactor);
            this.Intelligence = (int)(this.Intelligence * powerFactor);
            this.Luck         = (int)(this.Luck * powerFactor);
        }
        
        /// <summary>
        /// Multiplies the <see cref="Item.SpriteColor"/> and <see cref="IngameColor"/>
        /// of this Equipment by the specified multipliers.
        /// </summary>
        /// <param name="redMultiplier">The multiplier value for the red color component.</param>
        /// <param name="greenMultiplier">The multiplier value for the green color component.</param>
        /// <param name="blueMultiplier">The multiplier value for the blue color component.</param>
        /// <param name="alphaMultiplier">The multiplier value for the alpha color component.</param>
        internal override void MultiplyColor( float redMultiplier, float greenMultiplier, float blueMultiplier, float alphaMultiplier )
        {
            this.SpriteColor = this.SpriteColor.MultiplyBy( redMultiplier, greenMultiplier, blueMultiplier, alphaMultiplier );
            this.IngameColor = this.IngameColor.MultiplyBy( redMultiplier, greenMultiplier, blueMultiplier, alphaMultiplier );
        }

        /// <summary>
        /// Gets how much of the specified <see cref="Stat"/> this Equipment gives.
        /// </summary>
        /// <param name="stat">
        /// The Stat to get.
        /// </param>
        /// <returns>
        /// The Stat's value.
        /// </returns>
        public int GetStat( Stat stat )
        {
            switch( stat )
            {
                case Stat.Strength:
                    return this.Strength;

                case Stat.Dexterity:
                    return this.Dexterity;

                case Stat.Agility:
                    return this.Agility;

                case Stat.Vitality:
                    return this.Vitality;

                case Stat.Intelligence:
                    return this.Intelligence;

                case Stat.Luck:
                    return this.Luck;

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Sets how much of the specified <see cref="Stat"/> this Equipment gives.
        /// </summary>
        /// <param name="value">
        /// The value to set the Stat to.
        /// </param>
        /// <param name="stat">
        /// The Stat to set.
        /// </param>
        public void SetStat( int value, Stat stat )
        {
            switch( stat )
            {
                case Stat.Strength:
                    this.Strength = value;
                    break;

                case Stat.Dexterity:
                    this.Dexterity = value;
                    break;

                case Stat.Agility:
                    this.Agility = value;
                    break;

                case Stat.Vitality:
                    this.Vitality = value;
                    break;

                case Stat.Intelligence:
                    this.Intelligence = value;
                    break;

                case Stat.Luck:
                    this.Luck = value;
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        #region > Requirements <

        /// <summary>
        /// Gets whether the given <see cref="ExtendedStatable"/> fulfills all requirements 
        /// to wear this <see cref="Equipment"/>.
        /// </summary>
        /// <param name="statable">
        /// The <see cref="ExtendedStatable"/> component of the ZeldaEntity
        /// that wishes to wear an instance of this <see cref="Equipment"/>.
        /// </param>
        /// <returns>
        /// true if the ExtendedStatable would be able to wear an instance of this <see cref="Equipment"/>;
        /// otherwise false.
        /// </returns>
        public bool FulfillsRequirements( ExtendedStatable statable )
        {
            if( statable == null )
                return false;

            if( requiredLevel > statable.Level )
                return false;

            if( requiredStrength > statable.Strength )
                return false;

            if( requiredDexterity > statable.Dexterity )
                return false;

            if( requiredVitality > statable.Vitality )
                return false;

            if( requiredAgility > statable.Agility )
                return false;

            if( requiredIntelligence > statable.Intelligence )
                return false;

            if( requiredLuck > statable.Luck )
                return false;

            return true;
        }

        #endregion

        #region > Animations <

        /// <summary>
        /// Receives the ingame animation for the given <see cref="Atom.Math.Direction4"/>.
        /// </summary>
        /// <param name="direction">The direction to get the ingame animation for.</param>
        /// <returns>The AnimatedSprite object, or null if no such animation exists. </returns>
        public AnimatedSprite GetIngameAnimation( Atom.Math.Direction4 direction )
        {
            switch( direction )
            {
                case Atom.Math.Direction4.Left:
                    return animLeft;

                case Atom.Math.Direction4.Right:
                    return animRight;

                case Atom.Math.Direction4.Up:
                    return animUp;

                case Atom.Math.Direction4.Down:
                    return animDown;

                default:
                    return null;
            }
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Creates a clone of this <see cref="Equipment"/>.
        /// </summary>
        /// <returns>
        /// The cloned item.
        /// </returns>
        public override Item Clone()
        {
            var clone = new Equipment();

            this.SetupClone( clone );

            return clone;
        }

        /// <summary>
        /// Setups the given Equipment to be a clone of this Equipment.
        /// </summary>
        /// <param name="clone">
        /// The Equipment to setup as a clone of this Equipment.
        /// </param>
        protected void SetupClone( Equipment clone )
        {
            base.SetupClone( clone );

            clone.Armor = this.Armor;
            clone.Strength = this.Strength;
            clone.Dexterity = this.Dexterity;
            clone.Vitality = this.Vitality;
            clone.Agility = this.Agility;
            clone.Intelligence = this.Intelligence;
            clone.Luck = this.Luck;

            clone.requiredLevel = this.requiredLevel;
            clone.requiredStrength = this.requiredStrength;
            clone.requiredDexterity = this.requiredDexterity;
            clone.requiredVitality = this.requiredVitality;
            clone.requiredAgility = this.requiredAgility;
            clone.requiredIntelligence = this.requiredIntelligence;
            clone.requiredLuck = this.requiredLuck;

            clone.Slot = this.Slot;
            clone.ingameColor = this.ingameColor;
            this.SocketProperties.SetupClone( clone.SocketProperties );

            if( this.AdditionalEffectsAura != null )
                clone.AdditionalEffectsAura = (PermanentAura)this.AdditionalEffectsAura.Clone();
            else
                clone.AdditionalEffectsAura = null;
           
            clone.AnimationSpriteGroup = this.AnimationSpriteGroup;
            clone.animUp = this.animUp;
            clone.animDown = this.animDown;
            clone.animLeft = this.animLeft;
            clone.animRight = this.animRight;

            clone.Set = this.Set;
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
            base.Serialize( context );

            const int CurrentVersion = 5;
            context.Write( CurrentVersion );

            context.Write( (int)this.Slot );
            context.Write( this.Armor );

            // Stats:
            context.Write( this.Strength );
            context.Write( this.Dexterity );
            context.Write( this.Vitality );
            context.Write( this.Agility );
            context.Write( this.Intelligence );
            context.Write( this.Luck );

            // Requirements:
            context.Write( this.RequiredLevel );
            context.Write( this.RequiredStrength );
            context.Write( this.RequiredDexterity );
            context.Write( this.RequiredVitality );
            context.Write( this.RequiredAgility );
            context.Write( this.RequiredIntelligence );
            context.Write( this.RequiredLuck );

            // Animation:
            context.Write( this.AnimationSpriteGroup ?? string.Empty );

            context.Write( ingameColor );

            // Additional Effects:
            if( this.AdditionalEffectsAura != null )
            {
                context.Write( true );

                this.AdditionalEffectsAura.Serialize( context );
            }
            else
            {
                context.Write( false );
            }

            // Sockets:
            if( this.sockets.SocketCount != 0 )
            {
                context.Write( true );

                this.sockets.Serialize( context );
            }
            else
            {
                context.Write( false );
            }

            if( this.Set != null )
            {
                context.Write( this.Set.Name );
            }
            else
            {
                context.Write( string.Empty );
            }
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
            base.Deserialize( context );
            
            const int CurrentVersion = 5;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, CurrentVersion, "Zelda.Items.Equipment" );

            var serviceProvider = context.ServiceProvider;
            this.Slot  = (EquipmentSlot)context.ReadInt32();
            this.Armor = context.ReadInt32();

            // Stats:
            this.Strength     = context.ReadInt32();
            this.Dexterity    = context.ReadInt32();
            this.Vitality     = context.ReadInt32();
            this.Agility      = context.ReadInt32();
            this.Intelligence = context.ReadInt32();
            this.Luck         = context.ReadInt32();

            // Requirements:
            this.RequiredLevel        = context.ReadInt32();
            this.RequiredStrength     = context.ReadInt32();
            this.RequiredDexterity    = context.ReadInt32();
            this.RequiredVitality     = context.ReadInt32();
            this.RequiredAgility      = context.ReadInt32();
            this.RequiredIntelligence = context.ReadInt32();
            this.RequiredLuck         = context.ReadInt32();

            #region > Animations <

            this.AnimationSpriteGroup = context.ReadString();

            if( version >= 2 )
            {
                // Color has been moved to Equipment in Version 2.
                this.IngameColor = context.ReadColor();
            }

            if( this.AnimationSpriteGroup.Length > 0 && serviceProvider != null )
            {
                try 
                {
                    var spriteLoader = serviceProvider.SpriteLoader;

                    this.animLeft  = spriteLoader.LoadAnimatedSprite( this.AnimationSpriteGroup + "_Left" );
                    this.animRight = spriteLoader.LoadAnimatedSprite( this.AnimationSpriteGroup + "_Right" );
                    this.animUp    = spriteLoader.LoadAnimatedSprite( this.AnimationSpriteGroup + "_Up" );
                    this.animDown  = spriteLoader.LoadAnimatedSprite( this.AnimationSpriteGroup + "_Down" );
                }
                catch( System.IO.FileNotFoundException exc )
                {
                    serviceProvider.Log.WriteLine();
                    serviceProvider.Log.WriteLine(
                        LogSeverities.Error,
                        string.Format( 
                            System.Globalization.CultureInfo.CurrentCulture,
                            Zelda.Resources.Error_LoadingSpriteAssetXExceptionMessageY,
                            AnimationSpriteGroup,
                            exc.Message
                        )
                    );
                }
            }

            #endregion

            #region > AdditionalEffects <

            bool hasAdditionalEffects = context.ReadBoolean();

            if( hasAdditionalEffects )
            {
                this.AdditionalEffectsAura = new PermanentAura();
                this.AdditionalEffectsAura.Deserialize( context );
            }
            else
            {
                this.AdditionalEffectsAura = null;
            }

            #endregion

            #region > Sockets <

            bool hasSockets = context.ReadBoolean();

            if( hasSockets )
            {
                this.sockets.Deserialize( context );
            }

            #endregion

            if( version >= 5 )
            {
                string setName = context.ReadString();

                if( setName.Length > 0 )
                {
                    if( serviceProvider != null )
                    {
                        var setDatabase = serviceProvider.GetService<Zelda.Items.Sets.ISetDatabase>();
                        this.Set = setDatabase.Get( setName );
                    }
                }
            }
        }

        #endregion

        #endregion

        #region [ Fields ]
        
        /// <summary>
        /// Encapsulates the sockets properties of this <see cref="Equipment"/> instance.
        /// </summary>
        private readonly ItemSocketProperties sockets;

        #region > Requirements <

        /// <summary>
        /// The storage field for the <see cref="RequiredLevel"/> field.
        /// </summary>
        private int requiredLevel;

        /// <summary>
        /// The storage field for the <see cref="RequiredStrength"/> field.
        /// </summary>
        private int requiredStrength;

        /// <summary>
        /// The storage field for the <see cref="RequiredDexterity"/> field.
        /// </summary>
        private int requiredDexterity;

        /// <summary>
        /// The storage field for the <see cref="RequiredVitality"/> field.
        /// </summary>
        private int requiredVitality;

        /// <summary>
        /// The storage field for the <see cref="RequiredAgility"/> field.
        /// </summary>
        private int requiredAgility;

        /// <summary>
        /// The storage field for the <see cref="RequiredIntelligence"/> field.
        /// </summary>
        private int requiredIntelligence;

        /// <summary>
        /// The storage field for the <see cref="RequiredLuck"/> field.
        /// </summary>
        private int requiredLuck;

        #endregion

        #region > Animations <

        /// <summary>
        /// The animations shown ingame on the character.
        /// </summary>
        private Atom.Xna.AnimatedSprite animDown, animUp, animLeft, animRight;

        /// <summary>
        /// The storage field for the <see cref="IngameColor"/> field.
        /// </summary>
        private Microsoft.Xna.Framework.Color ingameColor = Microsoft.Xna.Framework.Color.White;

        #endregion

        #endregion
    }
}