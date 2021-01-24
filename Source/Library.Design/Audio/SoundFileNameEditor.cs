// <copyright file="SoundFileNameEditor.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Audio.Design.SoundFileNameEditor class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Audio.Design
{
    using System;
    using System.Windows.Forms.Design;

    /// <summary>
    /// Defines the UITypeEditor (used in a property grid) that
    /// asks the user to select a sound asset.
    /// </summary>
    public sealed class SoundFileNameEditor : FileNameEditor
    {
        /// <summary>
        /// Initializes the open file dialog when it is created.
        /// </summary>
        /// <param name="openFileDialog">
        /// The System.Windows.Forms.OpenFileDialog to use to select a file name.
        /// </param>
        protected override void InitializeDialog( System.Windows.Forms.OpenFileDialog openFileDialog )
        {
            openFileDialog.Title            = "Please select an audio asset.";
            openFileDialog.Filter           = "Supported Sound Files (*.mp3;*.mid;*.midi;*.wav;*.ogg)|*.mp3;*.mid;*.midi;*.wav;*.ogg";
            openFileDialog.InitialDirectory = System.IO.Path.Combine( AppDomain.CurrentDomain.BaseDirectory, @"Content\Samples\" );
            openFileDialog.RestoreDirectory = true;
        }
    }
}
