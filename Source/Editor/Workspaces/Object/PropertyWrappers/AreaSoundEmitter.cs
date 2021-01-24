// <copyright file="AreaSoundEmitterPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines Zelda.Editor.Object.PropertyWrappers.AreaSoundEmitterPropertyWrapper class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Editor.Object.PropertyWrappers
{
    using Atom.Math;
    using Zelda.Entities;
    using System.ComponentModel;

    /// <summary>
    /// Defines the IObjectPropertyWrapper for the <see cref="AreaSoundEmitter"/> type.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    internal sealed class AreaSoundEmitterPropertyWrapper : EntityPropertyWrapper<AreaSoundEmitter>
    {
        #region > Transform <

        [LocalizedDisplayName( "PropDisp_Position" )]
        [LocalizedCategory( "PropCate_Transform" )]
        [LocalizedDescriptionAttribute( "PropDesc_Position" )]
        public Vector2 Position
        {
            get { return this.WrappedObject.Transform.Position; }
            set { this.WrappedObject.Transform.Position = value; }
        }

        #endregion

        #region > Settings <

        [LocalizedDisplayName( "PropDisp_Sound" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_Sound" )]
        [System.ComponentModel.Editor( typeof( Zelda.Audio.Design.SoundFileNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string Sound
        {
            get
            {
                return this.WrappedObject.Sound == null ? string.Empty : this.WrappedObject.Sound.Name;
            }
            set
            {
                var assetName = System.IO.Path.GetFileName( value );
                var directory = System.IO.Path.GetDirectoryName( value );

                this.WrappedObject.Sound = serviceProvider.AudioSystem.Get( assetName, directory );
            }
        }

        [LocalizedDisplayName( "PropDisp_Volumne" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_Sound_Volumne" )]
        public float Volumne
        {
            get { return this.WrappedObject.Volume; }
            set
            {
                this.WrappedObject.Volume = Atom.Math.MathUtilities.Clamp( value, 0.0f, 1.0f );
            }
        }

        [LocalizedDisplayName( "PropDisp_Audibility" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_Sound_Audibility" )]
        public float Audibility
        {
            get
            {
                return this.WrappedObject.Audibility;
            }
        }

        [LocalizedDisplayName( "PropDisp_MinimumDistance" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_Sound_MinimumDistance" )]
        public Vector2 MinimumDistance
        {
            get { return this.WrappedObject.MinimumDistance; }
            set { this.WrappedObject.MinimumDistance = value; }
        }

        [LocalizedDisplayName( "PropDisp_MaximumDistance" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_Sound_MaximumDistance" )]
        public Vector2 MaximumDistance
        {
            get { return this.WrappedObject.MaximumDistance; }
            set { this.WrappedObject.MaximumDistance = value; }
        }

        [LocalizedDisplayName( "PropDisp_MaximumAreaOffset" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_Sound_MaximumAreaOffset" )]
        public Point2 MaximumAreaOffset
        {
            get { return this.WrappedObject.MaximumAreaOffset; }
            set { this.WrappedObject.MaximumAreaOffset = value; }
        }        

        [LocalizedDisplayName( "PropDisp_MinimumArea" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_Sound_MinimumArea" )]
        public Rectangle MinimumArea
        {
            get { return this.WrappedObject.MinimumArea; }
        }

        [LocalizedDisplayName( "PropDisp_MaximumArea" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_Sound_MaximumArea" )]
        public Rectangle MaximumArea
        {
            get { return this.WrappedObject.MaximumArea; }
        } 

        #endregion

        /// <summary>
        /// Returns a clone of this <see cref="IObjectPropertyWrapper"/>.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new AreaSoundEmitterPropertyWrapper( serviceProvider );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaSoundPropertyWrapper"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public AreaSoundEmitterPropertyWrapper( IZeldaServiceProvider serviceProvider )
        {
            System.Diagnostics.Debug.Assert( serviceProvider != null );
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;
    }
}
