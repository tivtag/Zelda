// <copyright file="WantedSignPostPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines Zelda.Editor.Object.PropertyWrappers.WantedSignPostPropertyWrapper class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.Editor.Object.PropertyWrappers
{
    using Atom.Math;
    using Zelda.Entities;
    using System.ComponentModel;

    /// <summary>
    /// Defines the IObjectPropertyWrapper for the <see cref="WantedSignPost"/> type.
    /// </summary>
    internal sealed class WantedSignPostPropertyWrapper : EntityPropertyWrapper<WantedSignPost>
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

        [LocalizedDisplayName( "PropDisp_PositionTile" )]
        [LocalizedCategory( "PropCate_Transform" )]
        [LocalizedDescriptionAttribute( "PropDesc_PositionTile" )]
        public Point2 PositionTile
        {
            get { return this.WrappedObject.Transform.PositionTile; }
        }              

        [LocalizedDisplayName( "PropDisp_FloorNumber" )]
        [LocalizedCategory( "PropCate_Transform" )]
        [LocalizedDescriptionAttribute( "PropDesc_FloorNumber" )]
        public int FloorNumber
        {
            get { return this.WrappedObject.FloorNumber; }
            set { this.WrappedObject.FloorNumber = value; }
        }

        [LocalizedDisplayName( "PropDisp_Direction" )]
        [LocalizedCategory( "PropCate_Transform" )]
        [LocalizedDescriptionAttribute( "PropDesc_Direction" )]
        public Direction4 Direction
        {
            get { return this.WrappedObject.Transform.Direction; }
            set { this.WrappedObject.Transform.Direction = value; }
        }

        [LocalizedDisplayName( "PropDisp_Size" )]
        [LocalizedCategory( "PropCate_Collision" )]
        [LocalizedDescriptionAttribute( "PropDesc_Size" )]
        public Vector2 Size
        {
            get { return this.WrappedObject.Collision.Size; }
            set { this.WrappedObject.Collision.Size = value; }
        }

        [LocalizedDisplayName( "PropDisp_Offset" )]
        [LocalizedCategory( "PropCate_Collision" )]
        [LocalizedDescriptionAttribute( "PropDesc_Offset" )]
        public Vector2 Offset
        {
            get { return this.WrappedObject.Collision.Offset; }
            set { this.WrappedObject.Collision.Offset = value; }
        }

        #endregion

        #region > Settings <

        [LocalizedDisplayName( "PropDisp_Sprite" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_Sprite" )]
        [System.ComponentModel.Editor( typeof( Zelda.Graphics.Design.SpriteNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string Sprite
        {
            get
            {
                return this.WrappedObject.DrawDataAndStrategy.SpriteGroup;
            }

            set
            {
                var assetName = System.IO.Path.GetFileNameWithoutExtension( value );

                this.WrappedObject.DrawDataAndStrategy.SpriteGroup = assetName;
                this.WrappedObject.DrawDataAndStrategy.Load( serviceProvider );
            }
        }

        [LocalizedDisplayName( "PropDisp_WantedSignPost_QuestName" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_WantedSignPost_QuestName" )]
        [System.ComponentModel.Editor( typeof( Zelda.Editor.Design.QuestNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string QuestName
        {
            get { return this.WrappedObject.QuestName; }
            set 
            {       
                this.WrappedObject.QuestName = System.IO.Path.GetFileNameWithoutExtension( value );
            }
        }

        [LocalizedDisplayName( "PropDisp_TextResourceId" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_TextResourceId" )]
        public string TextResourceId
        {
            get { return this.WrappedObject.Text.Id; }
            set { this.WrappedObject.Text.Id = value; }
        }

        [LocalizedDisplayName( "PropDisp_LocalizedText" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_WantedSignPost_LocalizedText" )]
        public string LocalizedText
        {
            get { return this.WrappedObject.Text.LocalizedText; }
        }

        #endregion        

        /// <summary>
        /// Returns a clone of this <see cref="IObjectPropertyWrapper"/>.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new WantedSignPostPropertyWrapper( serviceProvider );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WantedSignPostPropertyWrapper"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public WantedSignPostPropertyWrapper( IZeldaServiceProvider serviceProvider )
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