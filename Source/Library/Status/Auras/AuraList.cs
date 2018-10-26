// <copyright file="AuraList.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Auras.AuraList class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Status
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Atom.Collections;
    using Atom.Diagnostics.Contracts;

    /// <summary>
    /// Manages the life-cycle of all <see cref="Aura"/>s
    /// that are applied to a <see cref="Statable"/> ZeldaEntity.
    /// This class can't be inherited.
    /// </summary>
    public sealed class AuraList
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="AuraList"/> class.
        /// </summary>
        /// <param name="owner">
        /// The object that owns the new <see cref="AuraList"/>.
        /// </param>
        internal AuraList( Statable owner )
        {
            if( owner == null )
                throw new ArgumentNullException( "owner" );

            this.owner = owner;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the object that owns this <see cref="AuraList"/>.
        /// </summary>
        public Statable Owner
        {
            get
            {
                return this.owner; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AuraList"/> is capturing
        /// what <see cref="Aura"/>s are currently visible.
        /// </summary>
        public bool CaptureVisibleAuras
        {
            get
            {
                return this.visibleAuras != null;
            }

            set
            {
                if( value == this.CaptureVisibleAuras )
                    return;

                if( value )
                {
                    this.visibleAuras = new List<Aura>();
                    this.RefreshVisibleAuras();
                }
                else
                {
                    this.visibleAuras = null;
                }                
            }
        }

        /// <summary>
        /// Gets the enumeration of currently visible auras. 
        /// Might be null. Warning: Don't modify this list directly.
        /// </summary>
        public List<Aura> VisibleAuras
        {
            get
            {
                return this.visibleAuras;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates this <see cref="AuraList"/>.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            for( int i = 0; i < this.permanentAuras.Count; ++i )
            {
                this.permanentAuras[i].Update( updateContext );
            }

            for( int i = 0; i < this.timedAuras.Count; ++i )
            {
                TimedAura aura = this.timedAuras[i];
                aura.Update( updateContext );

                if( aura.IsActive == false )
                {
                    this.RemoveTimedAura( i, aura );
                    --i;
                }
            }
        }

        #region > Has <
        
        /// <summary>
        /// Gets a value indicating whether this AuraList contains
        /// a TimedAura with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the TimedAura to find.
        /// </param>
        /// <returns>
        /// true if this AuraList contains a TimedAura that has the specified <paramref name="name"/>;
        /// otherwise false.
        /// </returns>
        public bool HasTimedAura( string name )
        {
            if( name == null )
                return false;

            for( int i = 0; i < this.timedAuras.Count; ++i )
            {
                var aura = this.timedAuras[i];

                if( name.Equals( aura.Name, StringComparison.Ordinal ) )
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region > Find <
        
        /// <summary>
        /// Tries to find the TimedAura with the specified <paramref name="name"/> in this AuraList.
        /// </summary>
        /// <param name="name">
        /// The name of the TimedAura to find.
        /// </param>
        /// <returns>
        /// The TimedAura that has been found; or null if none was found.
        /// </returns>
        public TimedAura FindTimedAura( string name )
        {
            if( name == null )
                return null;

            for( int i = 0; i < this.timedAuras.Count; ++i )
            {
                TimedAura timedAura = this.timedAuras[i];

                if( name.Equals( timedAura.Name, StringComparison.Ordinal ) )
                {
                    return timedAura;
                }
            }

            return null;
        }

        #endregion

        #region > Get <

        /// <summary>
        /// Gets the active <see cref="StatusValueEffect"/>s that match
        /// the specified manipulation criterica.
        /// </summary>
        /// <param name="identifier">
        /// The string that represents what the StatusEffects to receive are manibulating.
        /// </param>
        /// <param name="matchingEffects">
        /// Will contain the matching effects.
        /// </param>
        internal bool GetEffects( string identifier, out List<StatusValueEffect> matchingEffects )
        {
            return this.effects.TryGet( identifier, out matchingEffects );
        }

        #region - Fixed & Percental & Rating -

        /// <summary>
        /// Gets the value for the active <see cref="StatusValueEffect"/>s that match
        /// the specified manipulation criterica.
        /// </summary>
        /// <param name="identifier">
        /// The string that represents what the StatusEffects to receive are manibulating.
        /// </param>
        /// <param name="fixedValue">
        /// Will contain the additive effect value.
        /// </param>
        /// <param name="percentalValue">
        /// Will contain the multipicative effect value.
        /// </param>
        internal void GetEffectValues( string identifier, out float fixedValue, out float percentalValue )
        {
            fixedValue     = 0.0f;
            percentalValue = 100.0f;

            List<StatusValueEffect> matchingEffects;
            if( this.effects.TryGet( identifier, out matchingEffects ) )
            {
                for( int i = 0; i < matchingEffects.Count; ++i )
                {
                    var effect = matchingEffects[i];

                    switch( effect.ManipulationType )
                    {
                        case StatusManipType.Fixed:
                            fixedValue += effect.Value;
                            break;

                        default:
                        case StatusManipType.Rating:
                        case StatusManipType.Percental:
                            percentalValue += effect.Value;
                            break;
                    }
                }
            }

            percentalValue = (percentalValue / 100.0f);
        }

        /// <summary>
        /// Gets the value for the active <see cref="StatusValueEffect"/>s that match
        /// the specified manipulation criterica.
        /// </summary>
        /// <param name="identifier">
        /// The string that represents what the StatusEffects to receive are manibulating.
        /// </param>
        /// <param name="fixedValue">
        /// Will contain the additive effect value.
        /// </param>
        /// <param name="percentalValue">
        /// Will contain the multipicative effect value.
        /// </param>
        /// <param name="ratingValue">
        /// Will contain the rating effect value.
        /// </param>
        internal void GetEffectValues( string identifier, out float fixedValue, out float percentalValue, out float ratingValue )
        {
            fixedValue     = 0.0f;
            percentalValue = 100.0f;
            ratingValue    = 0.0f;
            List<StatusValueEffect> matchingEffects;

            if( this.effects.TryGet( identifier, out matchingEffects ) )
            {
                for( int i = 0; i < matchingEffects.Count; ++i )
                {
                    var effect = matchingEffects[i];

                    switch( effect.ManipulationType )
                    {
                        default:
                        case StatusManipType.Fixed:
                            fixedValue += effect.Value;
                            break;

                        case StatusManipType.Percental:
                            percentalValue += effect.Value;
                            break;

                        case StatusManipType.Rating:
                            ratingValue += effect.Value;
                            break;
                    }
                }
            }

            percentalValue = (percentalValue / 100.0f);
        }

        /// <summary>
        /// Gets the value for the active <see cref="StatusValueEffect"/>s that match
        /// atleast one of the two the specified manipulation critericas.
        /// </summary>
        /// <param name="identifierA">
        /// The first string that represents what the StatusEffects to receive are manibulating.
        /// </param>
        /// <param name="identifierB">
        /// The second string that represents what the StatusEffects to receive are manibulating.
        /// </param>
        /// <param name="fixedValue">
        /// Will contain the additive effect value.
        /// </param>
        /// <param name="percentalValue">
        /// Will contain the multipicative effect value.
        /// </param>
        internal void GetEffectValues( string identifierA, string identifierB, out float fixedValue, out float percentalValue )
        {
            fixedValue     = 0.0f;
            percentalValue = 100.0f;
            List<StatusValueEffect> matchingEffects;

            if( this.effects.TryGet( identifierA, out matchingEffects ) )
            {
                for( int i = 0; i < matchingEffects.Count; ++i )
                {
                    var effect = matchingEffects[i];

                    switch( effect.ManipulationType )
                    {
                        case StatusManipType.Fixed:
                            fixedValue += effect.Value;
                            break;

                        default:
                        case StatusManipType.Rating:
                        case StatusManipType.Percental:
                            percentalValue += effect.Value;
                            break;
                    }
                }
            }

            if( this.effects.TryGet( identifierB, out matchingEffects ) )
            {
                for( int i = 0; i < matchingEffects.Count; ++i )
                {
                    var effect = matchingEffects[i];

                    switch( effect.ManipulationType )
                    {
                        case StatusManipType.Fixed:
                            fixedValue += effect.Value;
                            break;

                        default:
                        case StatusManipType.Rating:
                        case StatusManipType.Percental:
                            percentalValue += effect.Value;
                            break;
                    }
                }
            }

            percentalValue = (percentalValue / 100.0f);
        }

        /// <summary>
        /// Gets the value for the active <see cref="StatusValueEffect"/>s that match
        /// atleast one of the two the specified manipulation critericas.
        /// </summary>
        /// <param name="identifierA">
        /// The first string that represents what the StatusEffects to receive are manibulating.
        /// </param>
        /// <param name="identifierB">
        /// The second string that represents what the StatusEffects to receive are manibulating.
        /// </param>
        /// <param name="fixedValue">
        /// Will contain the additive effect value.
        /// </param>
        /// <param name="percentalValue">
        /// Will contain the multipicative effect value.
        /// </param>
        /// <param name="ratingValue">
        /// Will contain the rating effect value.
        /// </param>
        internal void GetEffectValues( 
            string identifierA, string identifierB, 
            out float fixedValue, out float percentalValue, out float ratingValue )
        {
            fixedValue     = 0.0f;
            percentalValue = 100.0f;
            ratingValue    = 0.0f;
            List<StatusValueEffect> matchingEffects;

            if( this.effects.TryGet( identifierA, out matchingEffects ) )
            {
                for( int i = 0; i < matchingEffects.Count; ++i )
                {
                    var effect = matchingEffects[i];

                    switch( effect.ManipulationType )
                    {
                        default:
                        case StatusManipType.Fixed:
                            fixedValue += effect.Value;
                            break;

                        case StatusManipType.Percental:
                            percentalValue += effect.Value;
                            break;

                        case StatusManipType.Rating:
                            ratingValue += effect.Value;
                            break;
                    }
                }
            }

            if( this.effects.TryGet( identifierB, out matchingEffects ) )
            {
                for( int i = 0; i < matchingEffects.Count; ++i )
                {
                    var effect = matchingEffects[i];

                    switch( effect.ManipulationType )
                    {
                        default:
                        case StatusManipType.Fixed:
                            fixedValue += effect.Value;
                            break;

                        case StatusManipType.Percental:
                            percentalValue += effect.Value;
                            break;

                        case StatusManipType.Rating:
                            ratingValue += effect.Value;
                            break;
                    }
                }
            }

            percentalValue = (percentalValue / 100.0f);
        }

        #endregion

        #region - Percental & Rating -

        /// <summary>
        /// Gets the value for the active <see cref="StatusValueEffect"/>s that match
        /// atleast one of the two the specified manipulation critericas.
        /// </summary>
        /// <param name="identifier">
        /// The first string that represents what the StatusEffects to receive are manibulating.
        /// </param>
        /// <param name="percentalValue">
        /// Will contain the multipicative effect value.
        /// </param>
        /// <param name="ratingValue">
        /// Will contain the rating effect value.
        /// </param>
        internal void GetPercentalAndRatingEffectValues( string identifier, out float percentalValue, out float ratingValue )
        {
            percentalValue = 100.0f;
            ratingValue    = 0.0f;
            List<StatusValueEffect> matchingEffects;

            if( this.effects.TryGet( identifier, out matchingEffects ) )
            {
                for( int i = 0; i < matchingEffects.Count; ++i )
                {
                    var effect = matchingEffects[i];

                    switch( effect.ManipulationType )
                    {
                        case StatusManipType.Rating:
                            ratingValue += effect.Value;
                            break;

                        default:
                        case StatusManipType.Percental:
                            percentalValue += effect.Value;
                            break;
                    }
                }
            }

            percentalValue = (percentalValue / 100.0f);
        }

        /// <summary>
        /// Gets the value for the active <see cref="StatusValueEffect"/>s that match
        /// atleast one of the two the specified manipulation critericas.
        /// </summary>
        /// <param name="identifierA">
        /// The first string that represents what the StatusEffects to receive are manibulating.
        /// </param>
        /// <param name="identifierB">
        /// The second string that represents what the StatusEffects to receive are manibulating.
        /// </param>
        /// <param name="percentalValue">
        /// Will contain the multipicative effect value.
        /// </param>
        /// <param name="ratingValue">
        /// Will contain the rating effect value.
        /// </param>
        internal void GetPercentalAndRatingEffectValues( string identifierA, string identifierB, out float percentalValue, out float ratingValue )
        {
            percentalValue = 100.0f;
            ratingValue    = 0.0f;
            List<StatusValueEffect> matchingEffects;

            if( this.effects.TryGet( identifierA, out matchingEffects ) )
            {
                for( int i = 0; i < matchingEffects.Count; ++i )
                {
                    var effect = matchingEffects[i];

                    switch( effect.ManipulationType )
                    {
                        case StatusManipType.Rating:
                            ratingValue += effect.Value;
                            break;

                        default:
                        case StatusManipType.Percental:
                            percentalValue += effect.Value;
                            break;
                    }
                }
            }

            if( this.effects.TryGet( identifierB, out matchingEffects ) )
            {
                for( int i = 0; i < matchingEffects.Count; ++i )
                {
                    var effect = matchingEffects[i];

                    switch( effect.ManipulationType )
                    {
                        case StatusManipType.Rating:
                            ratingValue += effect.Value;
                            break;

                        default:
                        case StatusManipType.Percental:
                            percentalValue += effect.Value;
                            break;
                    }
                }
            }

            percentalValue = (percentalValue / 100.0f);
        }

        #endregion

        #region - Fixed -

        /// <summary>
        /// Gets the fixed value for the active <see cref="StatusValueEffect"/>s that match
        /// the specified manipulation criterica.
        /// </summary>
        /// <param name="identifier">
        /// The string that represents what the StatusEffects to receive are manibulating.
        /// </param>
        /// <returns>
        /// The fixed value.
        /// </returns>
        internal float GetFixedEffectValue( string identifier )
        {
            float value = 0.0f;

            List<StatusValueEffect> matchingEffects;
            if( this.effects.TryGet( identifier, out matchingEffects ) )
            {
                for( int i = 0; i < matchingEffects.Count; ++i )
                {
                    value += matchingEffects[i].Value;
                }
            }

            return value;
        }

        #endregion

        #region - Percental -

        /// <summary>
        /// Gets the percental value for the active <see cref="StatusValueEffect"/>s
        /// of the specified <paramref name="identifier"/>.
        /// </summary>
        /// <param name="identifier">
        /// The string that represents what the StatusEffects to receive are manibulating.
        /// </param>
        /// <returns>
        /// The percental value.
        /// </returns>
        internal float GetPercentalEffectValue( string identifier )
        {
            float value = 1.0f;

            List<StatusValueEffect> matchingEffects;
            if( this.effects.TryGet( identifier, out matchingEffects ) )
            {
                for( int i = 0; i < matchingEffects.Count; ++i )
                {
                    value += matchingEffects[i].Value / 100.0f;
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the percental value for the active <see cref="StatusValueEffect"/>s that matches
        /// atleast one of the specified manipulation critericas.
        /// </summary>    
        /// <param name="identifierA">
        /// The first string that represents what the StatusEffects to receive are manibulating.
        /// </param>
        /// <param name="identifierB">
        /// The second string that represents what the StatusEffects to receive are manibulating.
        /// </param>
        /// <returns>
        /// The percental value.
        /// </returns>
        internal float GetPercentalEffectValue( string identifierA, string identifierB )
        {
            float value = 1.0f;

            List<StatusValueEffect> matchingEffects;
            if( this.effects.TryGet( identifierA, out matchingEffects ) )
            {
                for( int i = 0; i < matchingEffects.Count; ++i )
                {
                    value += matchingEffects[i].Value / 100.0f;
                }
            }

            if( this.effects.TryGet( identifierB, out matchingEffects ) )
            {
                for( int i = 0; i < matchingEffects.Count; ++i )
                {
                    value += matchingEffects[i].Value / 100.0f;
                }
            }

            return value;
        }

        #endregion

        #endregion

        #region > Organization <

        #region - PermanentAura -

        /// <summary>
        /// Adds the specified <see cref="PermanentAura"/> to this <see cref="AuraList"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="aura"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If the <paramref name="aura"/> already has been added to a <see cref="AuraList"/>.
        /// </exception>
        /// <param name="aura"> The aura to add, must be disabled. </param>
        public void Add( PermanentAura aura )
        {
            #region - Verify -

            Debug.Assert( aura != null );
            if( aura.AuraList == this )
                return;

            if( aura.AuraList != null )
            {
                throw new InvalidOperationException(
                    string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.Error_AuraXAlreadyIsAttachedToTheAuraListOfEntityY,
                        aura.Name,
                        aura.AuraList.Owner.Owner.Name
                    )
                );
            }

            #endregion

            this.permanentAuras.Add( aura );
            this.AddEffects( aura.Effects );
            this.AddToVisibleListIfRequired( aura );

            // Enable aura:
            aura.AuraList = this;
            aura.Enable();
        }

        /// <summary>
        /// Tries to remove the specified <see cref="PermanentAura"/> from this <see cref="AuraList"/>.
        /// </summary>
        /// <param name="aura">
        /// The aura to remove.
        /// </param>
        /// <returns>
        /// true if the PermanentAura has been removed; 
        /// otherwise false.
        /// </returns>
        public bool Remove( PermanentAura aura )
        {
            if( aura == null || aura.AuraList != this )
                return false;

            if( !this.permanentAuras.Remove( aura ) )
                return false;

            this.RemoveFromVisibleAuras( aura );
            this.RemoveEffects( aura.Effects );

            aura.Disable();
            Debug.Assert( aura.AuraList == null );
            return true;
        }

        #endregion

        #region - TimedAura -

        /// <summary>
        /// Adds the specified <see cref="TimedAura"/> to this <see cref="AuraList"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="aura"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If the <paramref name="aura"/> already has been added to a different <see cref="AuraList"/>.
        /// </exception>
        /// <param name="aura"> The aura to add, must be disabled. </param>
        public void Add( TimedAura aura )
        {
            #region - Verify -

            Debug.Assert( aura != null );

            if( aura.AuraList == this )
                return;

            if( aura.AuraList != null )
            {
                throw new InvalidOperationException(
                    string.Format(
                        System.Globalization.CultureInfo.CurrentCulture,
                        Resources.Error_AuraXAlreadyIsAttachedToTheAuraListOfEntityY,
                        aura.Name,
                        aura.AuraList.Owner.Owner.Name
                    )
                );
            }

            #endregion

            this.timedAuras.Add( aura );
            this.AddEffects( aura.Effects );
            this.AddToVisibleListIfRequired( aura );

            // Enable aura:
            aura.AuraList = this;
            aura.Enable();
        }

        /// <summary>
        /// Tries to remove the specified <see cref="TimedAura"/> from this <see cref="AuraList"/>.
        /// </summary>
        /// <param name="aura">
        /// The aura to remove.
        /// </param>
        /// <returns>
        /// true if the TimedAura has been removed; 
        /// otherwise false.
        /// </returns>
        public bool Remove( TimedAura aura )
        {
            if( aura == null || aura.AuraList != this )
                return false;

            if( !this.timedAuras.Remove( aura ) )
                return false;
       
            this.RemoveFromVisibleAuras( aura );
            this.RemoveEffects( aura.Effects );

            aura.Disable();
            Debug.Assert( aura.AuraList == null );
            return true;
        }

        /// <summary>
        /// Removes the TimedAura at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index of the aura to remove.
        /// </param>
        /// <param name="aura">
        /// The TimedAura to remove.
        /// </param>
        private void RemoveTimedAura( int index, TimedAura aura )
        {
            Debug.Assert( aura != null );
            Debug.Assert( index >= 0 && index < timedAuras.Count );
            
            this.timedAuras.RemoveAt( index );
            this.RemoveFromVisibleAuras( aura );
            this.RemoveEffects( aura.Effects );

            // Disable aura
            aura.Disable();
            Debug.Assert( aura.AuraList == null );
        }

        #endregion

        /// <summary>
        /// Adds and enables the specified StatusValueEffect.
        /// </summary>
        /// <param name="effect">
        /// The effect to add.
        /// </param>
        public void AddEffect( StatusValueEffect effect )
        {
            Contract.Requires<ArgumentNullException>( effect != null );

            this.effects.Add( effect.Identifier, effect );
            effect.OnEnable( this.owner );
        }

         /// <summary>
        /// Adds the specified StatusEffects to this AuraList;
        /// without enableding them.
        /// </summary>
        /// <param name="effects">
        /// The list of StatusEffects to add.
        /// </param>
        private void AddEffects( List<StatusEffect> effects )
        {
            for( int i = 0; i < effects.Count; ++i )
            {
                var effect = effects[i] as StatusValueEffect;

                if( effect != null )
                {
                    this.effects.Add( effect.Identifier, effect );
                }
            }
        }

        /// <summary>
        /// Removes the specified StatusEffects from this AuraList.
        /// </summary>
        /// <param name="effects">
        /// The list of StatusEffects to remove.
        /// </param>
        private void RemoveEffects( List<StatusEffect> effects )
        {
            for( int i = 0; i < effects.Count; ++i )
            {
                var effect = effects[i] as StatusValueEffect;

                if( effect != null )
                {
                    this.effects.Remove( effect.Identifier, effect );
                }
            }
        }

        #region - Visible Auras -

        /// <summary>
        /// Adds the specified Aura to the list of visibleAuras
        /// if required.
        /// </summary>
        /// <param name="aura">
        /// The aura to investigate.
        /// </param>
        private void AddToVisibleListIfRequired( Aura aura )
        {
            if( aura.IsVisible && this.CaptureVisibleAuras )
            {
                this.visibleAuras.Add( aura );
            }
        }

        /// <summary>
        /// Removes the specified Aura from the list of visibleAuras
        /// if required.
        /// </summary>
        /// <param name="aura">
        /// The aura to investigate.
        /// </param>
        private void RemoveFromVisibleAuras( Aura aura )
        {
            if( aura.IsVisible && this.CaptureVisibleAuras )
            {
                this.visibleAuras.Remove( aura );
            }
        }

        /// <summary>
        /// Rebuilds the visibleAuras list.
        /// </summary>
        private void RefreshVisibleAuras()
        {
            Debug.Assert( this.CaptureVisibleAuras );
            this.visibleAuras.Clear();

            for( int i = 0; i < this.permanentAuras.Count; ++i )              
            {
                var aura = this.permanentAuras[i];

                if( aura.IsVisible )
                {
                    this.visibleAuras.Add( aura );
                }
            }

            for( int i = 0; i < this.timedAuras.Count; ++i )
            {
                var aura = this.timedAuras[i];

                if( aura.IsVisible )
                {
                    this.visibleAuras.Add( aura );
                }
            }
        }

        #endregion

        /// <summary>
        /// Removes all permanent and all timed auras from this AuraList.
        /// </summary>
        internal void Clear()
        {
            while( this.permanentAuras.Count > 0 )
            {
                this.Remove( this.permanentAuras[0] );
            }

            while( this.timedAuras.Count > 0 )
            {
                this.RemoveTimedAura( 0, this.timedAuras[0] );
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the Statable component that owns this <see cref="AuraList"/>.
        /// </summary>
        private readonly Statable owner;

        /// <summary>
        /// Lists the currently active visible <see cref="Aura"/>s.
        /// </summary>
        private List<Aura> visibleAuras;

        /// <summary>
        /// Lists the currently active <see cref="PermanentAura"/>s.
        /// </summary>
        private readonly List<PermanentAura> permanentAuras = new List<PermanentAura>();

        /// <summary>
        /// Lists the currently active <see cref="TimedAura"/>s.
        /// </summary>
        private readonly List<TimedAura> timedAuras = new List<TimedAura>();

        /// <summary>
        /// The list of active <see cref="StatusValueEffect"/>s.
        /// </summary>
        private readonly FastMultiMap<string, StatusValueEffect> effects = new FastMultiMap<string, StatusValueEffect>();
        
        #endregion
    }
}