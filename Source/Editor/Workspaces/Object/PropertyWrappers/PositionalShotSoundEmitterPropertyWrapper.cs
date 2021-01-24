// <copyright file="PositionalSoundEmitterPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines Zelda.Editor.Object.PropertyWrappers.PositionalSoundEmitterPropertyWrapper class.
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
    /// Defines the IObjectPropertyWrapper for the <see cref="PositionalSoundEmitter"/> type.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    internal sealed class PositionalShotSoundEmitterPropertyWrapper : EntityPropertyWrapper<PositionalShotSoundEmitter>
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

                this.WrappedObject.Sound = serviceProvider.AudioSystem.Get( assetName, directory, PositionalSoundEmitter.SoundTag );
            }
        }

        [LocalizedDisplayName( "PropDisp_Volumne" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_Sound_Volumne" )]
        public FloatRange Volumne
        {
            get { return this.WrappedObject.Volume; }
            set
            {
                this.WrappedObject.Volume = new FloatRange( Atom.Math.MathUtilities.Clamp( value.Minimum, 0.0f, 1.0f ),  Atom.Math.MathUtilities.Clamp( value.Maximum, 0.0f, 1.0f ) );
            }
        }

        [LocalizedDisplayName( "PropDisp_TriggerPeriod" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_Sound_TriggerPeriod" )]
        public FloatRange TriggerPeriod
        {
            get { return this.WrappedObject.TriggerPeriod; }
            set
            {
                this.WrappedObject.TriggerPeriod = value;
            }
        }
        
        [LocalizedDisplayName( "PropDisp_MinimumDistance" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_Sound_MinimumDistance" )]
        public float MinimumDistance
        {
            get { return this.WrappedObject.MinimumDistance; }
            set { this.WrappedObject.MinimumDistance = value; }
        }

        [LocalizedDisplayName( "PropDisp_MaximumDistance" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_Sound_MaximumDistance" )]
        public float MaximumDistance
        {
            get { return this.WrappedObject.MaximumDistance; }
            set { this.WrappedObject.MaximumDistance = value; }
        }

        [LocalizedDisplayName( "PropDisp_PanLevel3D" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_Sound_PanLevel3D" )]
        public float PanLevel3D
        {
            get { return this.WrappedObject.PanLevel3D; }
            set { this.WrappedObject.PanLevel3D = value; }
        }

        #endregion

        /// <summary>
        /// Returns a clone of this <see cref="IObjectPropertyWrapper"/>.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new PositionalShotSoundEmitterPropertyWrapper( serviceProvider );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionalSoundPropertyWrapper"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public PositionalShotSoundEmitterPropertyWrapper( IZeldaServiceProvider serviceProvider )
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
