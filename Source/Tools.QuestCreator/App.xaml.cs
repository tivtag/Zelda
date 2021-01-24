// <copyright file="App.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.QuestCreator.App class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.QuestCreator
{
    using System.Windows;
    using Atom;

    /// <summary>
    /// The quest creator allows the creation and modification of Quests.
    /// </summary>
    public sealed partial class App : ToolApplication, Zelda.IZeldaServiceProvider
    {
        /// <summary>
        /// Gets or sets the Xna application object.
        /// </summary>
        internal XnaApp XnaApp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Xna application object.
        /// </summary>
        public override Microsoft.Xna.Framework.Game Game
        {
            get
            {
                return this.XnaApp;
            }
        }

        /// <summary>
        /// Gets the currently running application object.
        /// </summary>
        public static new App Current
        {
            get
            {
                return (App)Application.Current; 
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
            : base( "QuestCreator" )
        {
            GlobalServices.Container.AddService<Atom.Design.IItemSelectionDialogFactory>( new Atom.Wpf.Design.ItemSelectionDialogFactory() );
            GlobalServices.Container.AddService<Atom.Design.IExistingItemCollectionEditorFormFactory>( new Atom.Wpf.Design.ExistingItemCollectionEditorFormFactory() );
        }
    }
}
