// <copyright file="UseEffectSelectionDialog.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.ItemCreator.Dialogs.UseEffectSelectionDialog class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.ItemCreator.Dialogs
{
    using System;
    using Zelda.Items.UseEffects;

    /// <summary>
    /// Defines a dialog that asks the user to select a ItemUseEffect type.
    /// </summary>
    internal sealed class UseEffectSelectionDialog : TypeSelectionDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnUseEffectSelectionDialog"/> class.
        /// </summary>
        public UseEffectSelectionDialog()
            : base( Effects )
        {
        }
        
        /// <summary>
        /// The StatusEffects available to be selected in the <see cref="OnUseEffectSelectionDialog"/>.
        /// </summary>
        private static readonly Type[] Effects = new Type[] {
            typeof( ExecuteActionEffect ),     
            typeof( RestoreEffect ),        
            typeof( RestoreLifeManaEffect ),
            typeof( TemporaryStatusEffect ),
            typeof( AwardExperienceEffect ),
            typeof( AwardStatusPointEffect ),
            typeof( SpawnRandomItemEffect ),
            typeof( SchottlanderEffect ),
            typeof( NewQuestEffect ),
            typeof( LearnSongEffect ),
            typeof( TeleportationEffect ),
            typeof( UseBottleEffect ),
            typeof( SaveTeleportPositionEffect )
        };
    }
}
