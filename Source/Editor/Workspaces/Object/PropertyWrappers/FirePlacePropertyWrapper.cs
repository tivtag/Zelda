// <copyright file="FirePlacePropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines Zelda.Editor.Object.PropertyWrappers.FirePlacePropertyWrapper class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Editor.Object.PropertyWrappers
{
    using Atom.Math;
    using Zelda.Entities;
    using System.ComponentModel;

    /// <summary>
    /// Defines the IObjectPropertyWrapper for the <see cref="FirePlace"/> type.
    /// </summary>
    internal sealed class FirePlacePropertyWrapper : EntityPropertyWrapper<FirePlace>
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

        #region > Collision <

        [LocalizedDisplayName( "PropDisp_CollisionRectangle" )]
        [LocalizedCategory( "PropCate_Collision" )]
        [LocalizedDescriptionAttribute( "PropDesc_CollisionRectangle" )]
        public RectangleF CollisionRectangle
        {
            get { return this.WrappedObject.Collision.Rectangle; }
        }

        #endregion

        #region > Settings <

        [LocalizedDisplayName( "PropDisp_FirePlace_IsSwitchable" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_FirePlace_IsSwitchable" )]
        public bool IsSwitchable
        {
            get { return this.WrappedObject.IsSwitchable; }
            set { this.WrappedObject.IsSwitchable = value; }
        }

        [LocalizedDisplayName( "PropDisp_FirePlace_IsSwitched" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_FirePlace_IsSwitched" )]
        public bool IsSwitched
        {
            get { return this.WrappedObject.IsSwitched; }
            set
            {
                bool oldSwitchable = this.WrappedObject.IsSwitchable;

                this.WrappedObject.IsSwitchable = true;
                this.WrappedObject.IsSwitched   = value;
                this.WrappedObject.IsSwitchable = oldSwitchable;
            }
        }

        [LocalizedDisplayName( "PropDisp_FloorNumber" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_FloorNumber" )]
        public int FloorNumber
        {
            get { return this.WrappedObject.FloorNumber; }
            set { this.WrappedObject.FloorNumber = value; }
        }

        [LocalizedDisplayName( "PropDisp_SpriteOn" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_FirePlace_SpriteOn" )]
        [System.ComponentModel.Editor( typeof( Zelda.Graphics.Design.SpriteNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string SpriteOn
        {
            get
            {
                return this.WrappedObject.DrawDataAndStrategy.SpriteOnName;
            }

            set
            {
                var assetName = System.IO.Path.GetFileNameWithoutExtension( value );
                var dds       = this.WrappedObject.DrawDataAndStrategy;

                dds.SpriteOnName = assetName;
                dds.Load( serviceProvider );
            }
        }

        [LocalizedDisplayName( "PropDisp_SpriteOff" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_FirePlace_SpriteOff" )]
        [System.ComponentModel.Editor( typeof( Zelda.Graphics.Design.SpriteNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string SpriteOff
        {
            get
            {
                return this.WrappedObject.DrawDataAndStrategy.SpriteOffName;
            }

            set
            {
                var assetName = System.IO.Path.GetFileNameWithoutExtension( value );
                var dds       = this.WrappedObject.DrawDataAndStrategy;

                dds.SpriteOffName = assetName;
                dds.Load( serviceProvider );
            }
        }

        [LocalizedDisplayName( "PropDisp_HasLight" )]
        [LocalizedCategory( "PropCate_LightSettings" )]
        [LocalizedDescriptionAttribute( "PropDesc_FirePlace_HasLight" )]
        public bool HasLight
        {
            get { return this.WrappedObject.HasLight; }
            set { this.WrappedObject.HasLight = value; }
        }

        [LocalizedDisplayName( "PropDisp_Light" )]
        [LocalizedCategory( "PropCate_LightSettings" )]
        [LocalizedDescriptionAttribute( "PropDesc_FirePlace_Light" )]
        public LightPropertyWrapper Light
        {
            get
            {
                var light = this.WrappedObject.Light;
                if( light == null )
                    return null;

                // Rework following: ?!
                var scene = Zelda.Editor.EditorApp.Current.AppObject.Scene;
                return scene.GetExistingWrapperFor( light ) as LightPropertyWrapper;
            }
        }

        [LocalizedDisplayName( "PropDisp_HasSoundEmitter" )]
        [LocalizedCategory( "PropCate_SoundSettings" )]
        [LocalizedDescriptionAttribute( "PropDesc_FirePlace_HasSoundEmitter" )]
        public bool HasSoundEmitter
        {
            get { return this.WrappedObject.HasSoundEmitter; }
            set { this.WrappedObject.HasSoundEmitter = value; }
        }

        [LocalizedDisplayName( "PropDisp_SoundEmitter" )]
        [LocalizedCategory( "PropCate_SoundSettings" )]
        [LocalizedDescriptionAttribute( "PropDesc_FirePlace_SoundEmitter" )]
        public PositionalSoundEmitterPropertyWrapper SoundEmitter
        {
            get
            {
                var soundEmitter = this.WrappedObject.SoundEmitter;
                if( soundEmitter == null )
                    return null;

                // Rework following: ?!
                var scene = Zelda.Editor.EditorApp.Current.AppObject.Scene;
                return scene.GetExistingWrapperFor( soundEmitter ) as PositionalSoundEmitterPropertyWrapper;
            }
        }

        #endregion

        /// <summary>
        /// Returns a clone of this <see cref="IObjectPropertyWrapper"/>.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new FirePlacePropertyWrapper( serviceProvider );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirePlacePropertyWrapper"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public FirePlacePropertyWrapper( IZeldaServiceProvider serviceProvider )
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
