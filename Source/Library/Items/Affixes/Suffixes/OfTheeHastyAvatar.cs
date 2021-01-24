// <copyright file="OfHasteSuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.Suffixes.OfHasteSuffix class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Items.Affixes.Suffixes
{
    using System;
    using Atom.Xna;
    using Zelda.Attacks;
    using Zelda.Status;
    using Zelda.Status.Hooks;
    using Zelda.Status.Procs;

    /// <summary>
    /// The 'of Haste' suffix adds a chance of 2% to increase attack speed by '4 * item-level' for 10 seconds to an item.
    /// </summary>
    internal sealed class OfTheeHastyAvatarSuffix : ISuffix
    {
        /// <summary>
        /// The name of the sprite that is used for the proc smybol.
        /// </summary>
        private const string ProcSymbolName = "Chicken_Baby_Left_1";

        /// <summary>
        /// Gets the localized name of this IAffix.
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return AffixResources.OfTheeHastyAvatar; 
            }
        }

        /// <summary>
        /// Initializes a new instance of the OfTheeHastyAvatarSuffix class.
        /// </summary>
        /// <param name="spriteLoader">
        /// Provides a mechanism for loading sprite resources.
        /// </param>
        public OfTheeHastyAvatarSuffix( ISpriteLoader spriteLoader )
        {
            this.sprite = new Lazy<Sprite>( () => this.spriteLoader.LoadSprite( ProcSymbolName ), isThreadSafe: false );
            this.spriteLoader = spriteLoader;
        }
        
        /// <summary>
        /// Applies this IAffix to an Item.
        /// </summary>
        /// <param name="item">
        /// The Item that gets directly modified by this IAffix.
        /// </param>
        /// <param name="baseItem">
        /// The base non-modified Item.
        /// </param>
        public void Apply( Item item, Item baseItem )
        {
            var equipment = (Equipment)item;

            // Calculate:
            int attackSpeedRating = (int)(4 * item.Level);

            // Apply:
            var proc = new TimedStatusProcEffect() {
                Duration = 12.0f,
                Hook = new OnAttackHook() {
                    AttackType = AttackType.All,
                    HookMode = HitCritHookMode.HitAndCrit
                },
                ProcChance = new FixedProcChance() {
                    Chance = 2.5f
                },
                Effect = new AttackSpeedEffect( AttackType.All, attackSpeedRating, StatusManipType.Rating ),
                Symbol = this.sprite.Value
            };

            equipment.AdditionalEffects.Add( proc );
            equipment.MultiplyColor( 1.0f, 0.843f, 0.0f );
        }

        /// <summary>
        /// Gets a value indicating whether this IAffix could
        /// possibly applied to the given base <see cref="Item"/>.
        /// </summary>
        /// <param name="baseItem">
        /// The item this IAffix is supposed to be applied to.
        /// </param>
        /// <returns>
        /// True if this IAffix could possible applied to the given <paramref name="baseItem"/>;
        /// otherwise false.
        /// </returns>
        public bool IsApplyable( Item baseItem )
        {
            var equipment = baseItem as Equipment;
            return equipment != null && equipment.Slot == EquipmentSlot.Ring;
        }

        /// <summary>
        /// The lazy loaded sprite shown when the effect procs.
        /// </summary>
        private readonly Lazy<Sprite> sprite;

        /// <summary>
        /// Provides a mechanism for loading sprite resources.
        /// </summary>
        private readonly ISpriteLoader spriteLoader;
    }
}
