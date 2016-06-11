// <copyright file="IPrefix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.IPrefix interface.
// </summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Items.Affixes
{
    /// <summary>
    /// An <see cref="ISuffix"/> is an <see cref="IAffix"/>, 
    /// whose name is added before the BaseItem's name.
    /// </summary>
    /// <remarks>
    /// The prefix of the <see cref="AffixedItem"/> 'Rugged Bone Shield of Sorcery' is 'Rugged'. 
    /// </remarks>
    public interface IPrefix : IAffix
    {
    }
}
