// <copyright file="SpriteSheetViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.Tile.ViewModels.SpriteSheetViewModel class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Tile.ViewModels
{
    using Atom.Wpf;
    using Atom.Xna;

    /// <summary>
    /// Defines the ViewModel that defines the bindable properties and commands 
    /// of a <see cref="SpriteSheet"/> to provide them to the View (WPF).
    /// </summary>
    public sealed class SpriteSheetViewModel : ViewModel<SpriteSheet>
    {
        /// <summary>
        /// Receives the name of the SpriteSheet.
        /// </summary>
        public string Name
        {
            get { return this.Model.Name; }
        }

        /// <summary>
        /// Gets or sets an container object which indentifies
        /// what sprite the user has selected in the SpriteSheet.
        /// </summary>
        public SelectedSpriteContainer SelectedSprite
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteSheetViewModel"/> class.
        /// </summary>
        /// <param name="model">
        /// The model the new ViewModel wraps around.
        /// </param>
        public SpriteSheetViewModel( ISpriteSheet model )
            : base( (SpriteSheet)model )
        {
        }
    }
}
