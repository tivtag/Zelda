// <copyright file="SpecialItemType.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.SpecialItemType enumeration.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items
{
    /// <summary>
    /// Enumerates the various special item subtypes.
    /// </summary>
    public enum SpecialItemType
    {
        /// <summary>
        /// No specific special type.
        /// </summary>
        None,

        /// <summary>
        /// The 'money' in the world of Hyrule.
        /// </summary>
        Ruby,

        /// <summary>
        /// Herbs are usually used as potion ingridients.
        /// </summary>
        Herb,

        /// <summary>
        /// Ores are usually used as armor or weapon ingridients.
        /// </summary>
        Ore,

        /// <summary>
        /// Any liquid, such as a filled potion.
        /// </summary>
        Liquid,

        /// <summary>
        /// Any heavy liquid, such as a filled potion.
        /// </summary>
        LiquidHeavy,

        /// <summary>
        /// Any 'gem'-like item.
        /// </summary>
        Gem,

        /// <summary>
        /// Any 'skull'-like item.
        /// </summary>
        Skull,

        /// <summary>
        /// Any quiver.
        /// </summary>
        Quiver,

        /// <summary>
        /// Any jewelery that is no gem.
        /// </summary>
        Jewelry,

        /// <summary>
        /// Any bone-like item.
        /// </summary>
        Bone,

        /// <summary>
        /// Any key-like item.
        /// </summary>
        Key,

        /// <summary>
        /// Any food; such as an apple.
        /// </summary>
        Food,

        /// <summary>
        /// Any item made of cloth.
        /// </summary>
        Cloth = 100,

        /// <summary>
        /// Any item made of leather.
        /// </summary>
        Leather,               

        /// <summary>
        /// Any item made of chains.
        /// </summary>
        Chains,

        /// <summary>
        /// Any item made of heavy chains.
        /// </summary>
        ChainsHeavy,

        /// <summary>
        /// Any item made of metal.
        /// </summary>
        Metal,

        /// <summary>
        /// Any item made of heavy metal.
        /// </summary>
        MetalHeavy,

        /// <summary>
        /// Any item made of rock.
        /// </summary>
        Rock,

        /// <summary>
        /// Any item made of paper.
        /// </summary>
        Parchment,

        /// <summary>
        /// Any magical item that doesn't
        /// fit into any of the other categories.
        /// </summary>
        Magical,

        /// <summary>
        /// Any organic item that doesn't
        /// fit into any of the other categories.
        /// </summary>
        Organic,

        /// <summary>
        /// Any item made of wood.
        /// </summary>
        Wood,

        /// <summary>
        /// Any light-weight metal weapon;
        /// such as a dagger or knife.
        /// </summary>
        MetalLight
    }
}
