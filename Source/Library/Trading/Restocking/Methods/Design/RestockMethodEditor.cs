// <copyright file="RestockMethodEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.Trading.Restocking.Design.RestockMethodEditor class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Trading.Restocking.Design
{
    using System;

    /// <summary>
    /// Implements an <see cref="Zelda.Design.BaseZeldaObjectCreationEditor"/> that provides a mechanism that
    /// allows the user to create instances of the <see cref="IRestockMethod"/> interface.
    /// </summary>
    internal sealed class RestockMethodEditor : Zelda.Design.BaseZeldaObjectCreationEditor
    {
        /// <summary>
        /// Gets the types of the objects that can be created by this StatusEffectEditor.
        /// </summary>
        /// <returns>
        /// The list of types.
        /// </returns>
        protected override System.Collections.Generic.IEnumerable<Type> GetTypes()
        {
            return RestockMethodEditor.types;
        }

        /// <summary>
        /// The list of Types a RestockModeEditor can create.
        /// </summary>
        private static readonly Type[] types = new Type[3] {
            typeof( OnMerchantCreatedRestockMethod ),
            typeof( AfterWorldTimeRestockMethod ),
            typeof( AfterWorldTimeAndOnCreationRestockMethod )
        };
    }
}
