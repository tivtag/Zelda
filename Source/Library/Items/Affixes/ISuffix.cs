// <copyright file="ISuffix.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Items.Affixes.ISuffix interface.
// </summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Items.Affixes
{
    /// <summary>
    /// An <see cref="ISuffix"/> is an <see cref="IAffix"/>, 
    /// whose name is added behind the BaseItem's name.
    /// </summary>
    /// <remarks>
    /// The suffix of the <see cref="AffixedItem"/> 'Rugged Bone Shield of Sorcery' is 'of Sorcery'. 
    /// </remarks>
    public interface ISuffix : IAffix
    {
    }
}
