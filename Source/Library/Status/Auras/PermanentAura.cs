// <copyright file="PermanentAura.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.PermanentAura class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using Zelda.Status.Procs;

    /// <summary>
    /// A <see cref="PermanentAura"/> is an <see cref="Aura"/> that stays active until it's manually removed.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public class PermanentAura : Aura
    {
        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PermanentAura"/> class.
        /// </summary>
        public PermanentAura()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PermanentAura"/> class.
        /// </summary>
        /// <param name="effect"> 
        /// The <see cref="StatusEffect"/> the new <see cref="PermanentAura"/> applies.
        /// </param>
        public PermanentAura( StatusEffect effect )
            : base( new StatusEffect[1] { effect } )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PermanentAura"/> class.
        /// </summary>
        /// <param name="firstEffect"> 
        /// The first <see cref="StatusEffect"/> the new <see cref="PermanentAura"/> applies.
        /// </param>
        /// <param name="secondEffect"> 
        /// The second <see cref="StatusEffect"/> the new <see cref="PermanentAura"/> applies.
        /// </param>
        public PermanentAura( StatusEffect firstEffect, StatusEffect secondEffect )
            : base( new StatusEffect[2] { firstEffect, secondEffect } )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PermanentAura"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="effects"/> is null.
        /// </exception>
        /// <param name="effects"> The list of <see cref="StatusEffect"/>s of the new <see cref="PermanentAura"/>. </param>
        public PermanentAura( StatusEffect[] effects )
            : base( effects )
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Adds the specified StatusEffect to this PermanentAura.
        /// </summary>
        /// <param name="effect">
        /// The StatusEffect to add.
        /// </param>
        internal void AddEffect( StatusEffect effect )
        {
            Debug.Assert( effect != null );

            this.Effects.Add( effect );
        }

        /// <summary>
        /// Adds the given StatusValueEffects to this PermanentAura by simply adding the effect
        /// or if possible merging the value with an existing effect.
        /// </summary>
        /// <param name="effects">
        /// The StatusEffects to merge/add.
        /// </param>
        internal void MergeAdd( System.Collections.Generic.IEnumerable<StatusEffect> effects )
        {
            var auraList = this.AuraList;
            if( auraList != null )
                auraList.Remove( this );

            foreach( var effect in effects )
            {
                var valueEffect = effect as StatusValueEffect;

                if( valueEffect != null )
                {
                    this.MergeAdd( valueEffect );
                }
                else
                {
                    this.Effects.Add( valueEffect );
                }
            }

            if( auraList != null )
                auraList.Add( this );
        }

        /// <summary>
        /// Adds the given StatusValueEffect to this PermanentAura by simply adding the effect
        /// or if possible merging the value with an existing effect.
        /// </summary>
        /// <param name="effect">
        /// The effect to merge/add.
        /// </param>
        public void MergeAdd( StatusValueEffect effect )
        {
            if( effect == null )
                throw new ArgumentNullException( "effect" );

            var auraList = this.AuraList;
            if( auraList != null )
                auraList.Remove( this );

            StatusValueEffect existingEffect = this.GetExistingValueEffectEqual( effect );

            if( existingEffect != null )
            {
                existingEffect.Value += effect.Value;
            }
            else
            {
                this.Effects.Add( effect );
            }

            if( auraList != null )
                auraList.Add( this );
        }

        /// <summary>
        /// Sorts the StatusEffects of this PermanentAura.
        /// </summary>
        internal void SortEffects()
        {
            this.Effects.Sort( CompareEffects );
        }

        /// <summary>
        /// Used to compare two StatusEffects while sorting.
        /// </summary>
        /// <param name="x">
        /// The StatusEffect on the left side.
        /// </param>
        /// <param name="y">
        /// The StatusEffect on the right side.
        /// </param>
        /// <returns>
        /// An integer that 
        /// </returns>
        private static int CompareEffects( StatusEffect x, StatusEffect y )
        {
            if( x == y )
                return 0;
            if( x is StatEffect )
                return -1;

            // Procs should be at the end.
            {
                var procX = x as ProcEffect;
                var procY = y as ProcEffect;

                if( procX != null || procY != null )
                {
                    if( procX != null )
                    {
                        return procY != null ? 0 : -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }

            // Chance To StatusEffects should be before.
            {
                var chanceX = x as ChanceToStatusEffect;
                var chanceY = y as ChanceToStatusEffect;

                if( chanceX != null || chanceY != null )
                {
                    if( chanceX != null )
                    {
                        return chanceY != null ? 0 : -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }

            if( x.IsBad && y.IsBad )
                return 0;
            if( x.IsBad && !y.IsBad )
                return 1;
            if( !x.IsBad && y.IsBad )
                return -1;
            return 0;
        }

        /// <summary>
        /// Tries to get an existing StatusValueEffect that equals the given StatusEffect.
        /// </summary>
        /// <param name="inputEffect">
        /// The input StatusEffect.
        /// </param>
        /// <returns>
        /// An StatusValueEffect instance that is part of this Aura; or null.
        /// </returns>
        private StatusValueEffect GetExistingValueEffectEqual( StatusValueEffect inputEffect )
        {
            for( int i = 0; i < this.Effects.Count; ++i )
            {
                var effect = this.Effects[i];

                if( effect.Equals( inputEffect ) )
                {
                    return (StatusValueEffect)effect;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a clone of this <see cref="PermanentAura"/>.
        /// </summary>
        /// <returns>The cloned Aura.</returns>
        public override Aura Clone()
        {
            return new PermanentAura( this.GetClonedEffects() ) {
                Name = this.Name,
                Symbol = this.Symbol,
                IsVisible = this.IsVisible,
                DebuffFlags = this.DebuffFlags,
                DescriptionProvider = this.DescriptionProvider
            };
        }

        #endregion
    }
}
