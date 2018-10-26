// <copyright file="Aura.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Auras.Aura class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using Atom.Diagnostics.Contracts;
    using Atom;
    using Atom.Xna;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Represents a composition of StatusEffects.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public abstract class Aura : IAura
    {
        #region [ Events ]

        /// <summary>
        /// Fired when this Aura has been enabled.
        /// </summary>
        public event RelaxedEventHandler<Statable> Enabled;

        /// <summary>
        /// Fired when this Aura has been disabled.
        /// </summary>
        public event RelaxedEventHandler<Statable> Disabled;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name that identifies this <see cref="Aura"/>.
        /// </summary>
        /// <remarks>
        /// This value may or may not be supposed to be unique.
        /// </remarks>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an object which provides access to a description.
        /// </summary>
        [Browsable( false )]
        public IDescriptionProvider DescriptionProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the (localized) description of this <see cref="Aura"/>.
        /// </summary>
        [Browsable( false )]
        public string Description
        {
            get
            {
                if( this.DescriptionProvider == null )
                    return string.Empty;

                return this.DescriptionProvider.Description;
            }
        }

        /// <summary>
        /// Gets or sets the symbol sprite of this <see cref="Aura"/>. May be null.
        /// </summary>
        [Editor( typeof( Zelda.Graphics.Design.SpriteEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Atom.Xna.Sprite Symbol
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color the <see cref="Symbol"/> of this Aura is tinted in.
        /// </summary>
        /// <value>The default value is Color.White.</value>
        public Microsoft.Xna.Framework.Color SymbolColor
        {
            get
            {
                return this.symbolColor;
            }

            set
            {
                this.symbolColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Aura"/> is visible.
        /// </summary>
        /// <remarks>
        /// Visible auras are shown (in the case of the player) in the buff/debuff bar.
        /// </remarks>
        public bool IsVisible
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Aura"/> has been enabled.
        /// </summary>
        [Browsable( false )]
        public bool IsEnabled
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the <see cref="DebuffFlags"/> of this <see cref="Aura"/>.
        /// </summary>
        public DebuffFlags DebuffFlags
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Aura"/> is a debuff.
        /// </summary>
        [Browsable( false )]
        public bool IsDebuff
        {
            get
            {
                return this.DebuffFlags != DebuffFlags.None;
            }
        }

        /// <summary>
        /// Gets the list of <see cref="Effects"/> of this <see cref="Aura"/>.
        /// </summary>
        [Editor( "Zelda.Status.Design.StatusEffectListEditor, Library.Design", typeof( System.Drawing.Design.UITypeEditor ) )]
        public List<StatusEffect> Effects
        {
            get
            {
                return this.effects;
            }
        }

        /// <summary>
        /// Gets the <see cref="AuraList"/> that owns this <see cref="Aura"/>.
        /// </summary>
        [Browsable( false )]
        public AuraList AuraList
        {
            get
            {
                return this.list;
            }

            internal set
            {
                this.list = value;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="Aura"/> class.
        /// </summary>
        protected Aura()
        {
            this.effects = new List<StatusEffect>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Aura"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="effects"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If any of the elements of the given <paramref name="effects"/> array is null.
        /// </exception>
        /// <param name="effects">
        /// The list of <see cref="StatusEffect"/>s of the new <see cref="Aura"/>.
        /// </param>
        protected Aura( StatusEffect[] effects )
        {
            Contract.Requires<ArgumentNullException>( effects != null );
            Contract.Requires<ArgumentException>( Contract.ForAll( effects, effect => effect != null ) );

            this.effects = new List<StatusEffect>( effects );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Modifies the power of each effect of this Aura by the given factor.
        /// </summary>
        /// <param name="factor">
        /// The factor to change by.
        /// </param>
        public void ModifyPowerBy( float factor )
        {
            for( int i = 0; i < effects.Count; ++i )
            {
                effects[i].ModifyPowerBy( factor );
            }

            this.effects.RemoveAll( x => x.IsUseless );
        }

        /// <summary>
        /// Enables this <see cref="Aura"/>.
        /// </summary>
        internal void Enable()
        {
            #region - Validate State -

            Debug.Assert( list != null );

            if( this.IsEnabled )
            {
                throw new InvalidOperationException(
                    string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Zelda.Resources.Error_AuraXAlreadyEnabledForY,
                        this.Name,
                        list.Owner.Owner.Name
                    )
                );
            }

            #endregion

            var owner = list.Owner;

            for( int i = 0; i < effects.Count; ++i )
                effects[i].OnEnable( owner );

            this.IsEnabled = true;
            this.OnEnabledPrivate( owner );
        }

        /// <summary>
        /// Called when this <see cref="Aura"/> has got enabled.
        /// </summary>
        /// <param name="owner">
        /// The Statable that now owns this Aura.
        /// </param>
        private void OnEnabledPrivate( Statable owner )
        {
            this.OnEnabled( owner );
            this.Enabled.Raise( this, owner );
        }

        /// <summary>
        /// Called when this <see cref="Aura"/> has got enabled.
        /// </summary>
        /// <param name="owner">
        /// The Statable that now owns this Aura.
        /// </param>
        protected virtual void OnEnabled( Statable owner )
        {
        }

        /// <summary>
        /// Disables the <see cref="Aura"/>.
        /// </summary>
        internal void Disable()
        {
            #region - Validate State -

            if( !this.IsEnabled )
            {
                throw new InvalidOperationException(
                    string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Zelda.Resources.Error_AuraXAlreadyDisabled,
                        this.Name
                    )
                );
            }

            Debug.Assert( list != null );

            #endregion

            var owner = list.Owner;

            for( int i = 0; i < effects.Count; ++i )
                effects[i].OnDisable( owner );

            this.IsEnabled = false;
            this.list = null;

            this.OnDisabledPrivate( owner );
        }

        /// <summary>
        /// Called when this <see cref="Aura"/> got disabled.
        /// </summary>
        /// <param name="owner">
        /// The Statable that previously owned this Aura.
        /// </param>
        private void OnDisabledPrivate( Statable owner )
        {
            this.OnDisabled( owner );
            this.Disabled.Raise( this, owner );
        }

        /// <summary>
        /// Called when this <see cref="Aura"/> got disabled.
        /// </summary>
        /// <param name="owner">
        /// The Statable that previously owned this Aura.
        /// </param>
        protected virtual void OnDisabled( Statable owner )
        {
        }

        /// <summary>        
        /// Updates this <see cref="Aura"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public virtual void Update( ZeldaUpdateContext updateContext )
        {
        }

        /// <summary>
        /// Removes all StatusEffects from this Aura.
        /// </summary>
        public void ClearEffects()
        {
            this.effects.Clear();
        }

        #region > GetEffect <

        /// <summary>
        /// Tries to get the StatusValueEffect of the specified <typeparamref name="TEffect"/>
        /// and <see cref="StatusManipType"/>.
        /// </summary>
        /// <typeparam name="TEffect">
        /// The exact type of StatusValueEffect to get.
        /// </typeparam>
        /// <returns>
        /// The requested StatusValueEffect; or null if no such effect could be found.
        /// </returns>
        public TEffect GetEffect<TEffect>()
            where TEffect : StatusValueEffect
        {
            for( int i = 0; i < effects.Count; ++i )
            {
                var effect = effects[i];
                if( effect.GetType() == typeof( TEffect ) )
                {
                    var valueEffect = effect as StatusValueEffect;

                    if( valueEffect != null )
                    {
                        return (TEffect)valueEffect;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to get the StatusValueEffect of the specified <typeparamref name="TEffect"/>
        /// and <see cref="StatusManipType"/>.
        /// </summary>
        /// <typeparam name="TEffect">
        /// The exact type of StatusValueEffect to get.
        /// </typeparam>
        /// <param name="manipulationType">
        /// The manipulation type the StatusValueEffect must have.
        /// </param>
        /// <returns>
        /// The requested StatusValueEffect; or null if no such effect could be found.
        /// </returns>
        public TEffect GetEffect<TEffect>( StatusManipType manipulationType )
            where TEffect : StatusValueEffect
        {
            for( int i = 0; i < effects.Count; ++i )
            {
                var effect = effects[i];
                if( effect.GetType() == typeof( TEffect ) )
                {
                    var valueEffect = effect as StatusValueEffect;

                    if( valueEffect != null && valueEffect.ManipulationType == manipulationType )
                    {
                        return (TEffect)valueEffect;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to get the StatusEffect of the specified <typeparamref name="TEffect"/>
        /// that matches the given Predicate.
        /// </summary>
        /// <typeparam name="TEffect">
        /// The exact type of StatusEffect to get.
        /// </typeparam>
        /// <param name="predicate">
        /// The predicate that a StatusEffect must fulfill.
        /// </param>
        /// <returns>
        /// The requested StatusEffect; or null if no such effect could be found.
        /// </returns>
        public TEffect GetEffect<TEffect>( Predicate<TEffect> predicate )
            where TEffect : StatusEffect
        {
            Debug.Assert( predicate != null );

            for( int i = 0; i < effects.Count; ++i )
            {
                var effect = effects[i];             
                if( effect.GetType() == typeof( TEffect ) )
                {
                    var convertedEffect = (TEffect)effect;
                    if( predicate( convertedEffect ) )
                    {
                        return convertedEffect;
                    }
                }
            }

            return null;
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Clones this <see cref="Aura"/>.
        /// </summary>
        /// <returns>
        /// The cloned <see cref="Aura"/>.
        /// </returns>
        public abstract Aura Clone();

        /// <summary>
        /// Setups the given Aura to be a clone of this Aura.
        /// </summary>
        /// <param name="clone">
        /// The Aura to setup as a clone of this Aura.
        /// </param>
        public void SetupClone( Aura clone )
        {
            clone.Name = this.Name;
            clone.Symbol = this.Symbol;
            clone.symbolColor = this.symbolColor;
            clone.IsVisible = this.IsVisible;
            clone.DebuffFlags = this.DebuffFlags;
            clone.DescriptionProvider = this.DescriptionProvider;

            // clone.StatusEffects.Clear(); // Not required atm.
            clone.effects.AddRange( this.GetClonedEffects() );
        }

        /// <summary>
        /// Clones the StatusEffects of this <see cref="Aura"/>.
        /// </summary>
        /// <returns>The cloned StatusEffects, or null.</returns>
        protected StatusEffect[] GetClonedEffects()
        {
            if( effects == null )
                return null;

            StatusEffect[] clonedEffects = new StatusEffect[this.effects.Count];

            for( int i = 0; i < this.effects.Count; ++i )
            {
                clonedEffects[i] = this.effects[i].Clone();
            }

            return clonedEffects;
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
        public virtual void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int Version = 1;
            context.Write( Version );

            context.Write( this.Name ?? string.Empty );
            context.Write( this.IsVisible );
            context.Write( (int)this.DebuffFlags );

            context.Write( effects.Count );
            for( int i = 0; i < effects.Count; ++i )
            {
                effects[i].Serialize( context );
            }

            context.Write( this.Symbol != null ? this.Symbol.Name : string.Empty );
            context.Write( symbolColor );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public virtual void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            var serviceProvider = context.ServiceProvider;

            // Header.
            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

            // Properties.
            this.Name = context.ReadString();
            this.IsVisible = context.ReadBoolean();
            this.DebuffFlags = (DebuffFlags)context.ReadInt32();

            // Effects.
            int effectCount = context.ReadInt32();

            this.effects.Clear();
            this.effects.Capacity = effectCount;

            for( int i = 0; i < effectCount; ++i )
            {
                string effectTypeName = context.ReadString();
                Type effectType = Type.GetType( effectTypeName );
                StatusEffect effect = (StatusEffect)Activator.CreateInstance( effectType );

                effect.Deserialize( context );
                this.effects.Add( effect );
            }

            // Symbol.
            string spriteName = context.ReadString();

            if( spriteName.Length > 0 && serviceProvider != null )
            {
                this.Symbol = serviceProvider.SpriteLoader.LoadSprite( spriteName );
            }
            else
            {
                this.Symbol = null;
            }

            // Symbol Color.
            this.symbolColor = context.ReadColor();
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The color the <see cref="Symbol"/> of this Aura is tinted in.
        /// </summary>
        private Microsoft.Xna.Framework.Color symbolColor = Microsoft.Xna.Framework.Color.White;

        /// <summary>
        /// The list that owns the <see cref="Aura"/>.
        /// </summary>
        private AuraList list;

        /// <summary>
        /// The list of StatusEffects this <see cref="Aura"/> has.
        /// </summary>
        private readonly List<StatusEffect> effects;

        #endregion
    }
}