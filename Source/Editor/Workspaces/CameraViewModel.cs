// <copyright file="CameraViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Editor.CameraViewModel class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor
{
    using Atom.Math;
    using Atom.Wpf;
    using Zelda.Entities;

    /// <summary>
    /// Defines the ViewModel over the <see cref="Zelda.Entities.ZeldaCamera"/>.
    /// </summary>
    public sealed class CameraViewModel : ViewModel<ZeldaCamera>
    {
        /// <summary>
        /// Gets or sets the (bindable) value which represents the translation
        /// of the Scene archived by the Camera.
        /// </summary>
        public Vector2 Scroll
        {
            get 
            {
                return this.Model.Scroll;
            }

            set
            {
                this.Model.Scroll = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraViewModel"/> class.
        /// </summary>
        /// <param name="model">
        /// The Model the new ViewModel wraps around.
        /// </param>
        public CameraViewModel( Zelda.Entities.ZeldaCamera model )
            : base( model )
        {
            this.Model.TransformChanged += this.OnCameraTransformChanged;
        }
        
        /// <summary>
        /// Tells the camera to center the given position.
        /// </summary>
        /// <param name="position">
        /// The input position.
        /// </param>
        public void MoveToCentered( Atom.Math.Vector2 position )
        {
            this.Scroll = position - (this.Model.ViewSize / 2);
        }

        /// <summary>
        /// Gets called when the transform of the Camera has chaned.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnCameraTransformChanged( ZeldaCamera sender )
        {
            this.OnPropertyChanged( "Scroll" );
        }
    }
}
