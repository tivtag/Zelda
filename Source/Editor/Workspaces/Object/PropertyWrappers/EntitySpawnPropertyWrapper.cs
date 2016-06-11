// <copyright file="EntitySpawnPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines Zelda.Editor.Object.PropertyWrappers.EntitySpawnPropertyWrapper class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Editor.Object.PropertyWrappers
{
    using System.ComponentModel;
    using Atom.Math;
    using Zelda.Entities.Spawning;

    /// <summary>
    /// Defines the IObjectPropertyWrapper for <see cref="EntitySpawn"/> objectss.
    /// </summary>
    internal sealed class EntitySpawnPropertyWrapper : EntityPropertyWrapper<EntitySpawn>
    {
        [LocalizedDisplayName( "PropDisp_IsActive" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_IsEntitySpawnActive" )]
        [DefaultValue( true )]
        public bool IsActive
        {
            get
            {
                return this.WrappedObject.IsActive;
            }

            set
            {
                this.WrappedObject.IsActive = value;
            }
        }
        
        [LocalizedDisplayName( "PropDisp_EntityModifier" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_EntityModifier" ) ]
        [System.ComponentModel.Editor( typeof( Zelda.Entities.Modifiers.Design.EntityModifierEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Zelda.Entities.Modifiers.IEntityModifier EntityModifier
        {
            get
            {
                return this.WrappedObject.EntityModifier;
            }

            set
            {
                this.WrappedObject.EntityModifier = value;
            }
        }

        [LocalizedDisplayName( "PropDisp_TemplateName" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_TemplateName" )]
        [System.ComponentModel.Editor( typeof( Zelda.Editor.Design.EntityTemplateNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string TemplateName
        {
            get 
            {
                return this.WrappedObject.TemplateName;
            }

            set
            {
                string templateName = System.IO.Path.GetFileNameWithoutExtension( value );
                this.WrappedObject.TemplateName = templateName; 
            }
        }

        [LocalizedDisplayName( "PropDisp_UseTemplate" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_UseTemplate" )]
        public bool UseTemplate
        {
            get { return this.WrappedObject.UseTemplate; }
            set { this.WrappedObject.UseTemplate = value; }
        }

        [LocalizedDisplayName( "PropDisp_Position" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_Position" )]
        public Vector2 Position
        {
            get { return this.WrappedObject.Transform.Position; }
            set { this.WrappedObject.Transform.Position = value; }
        }

        [LocalizedDisplayName( "PropDisp_FloorNumber" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDesc_FloorNumber" )]
        public int FloorNumber
        {
            get { return this.WrappedObject.FloorNumber; }
            set { this.WrappedObject.FloorNumber = value; }
        }

        [LocalizedDisplayName( "PropDisp_Direction" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDisp_Direction" )]
        public Direction4 Direction
        {
            get { return this.WrappedObject.Transform.Direction; }
            set { this.WrappedObject.Transform.Direction = value; }
        }

        /// <summary>
        /// Returns a clone of this EntitySpawnPropertyWrapper.
        /// </summary>
        /// <returns>
        /// The cloned IObjectPropertyWrapper.
        /// </returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new EntitySpawnPropertyWrapper();
        }
    }
}
