// <copyright file="RazorEntitySpawnPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines Zelda.Editor.Object.PropertyWrappers.RazorEntitySpawnPropertyWrapper class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.Editor.Object.PropertyWrappers
{
    using Atom.Math;
    using Zelda.Entities.Spawning;

    /// <summary>
    /// Defines the IObjectPropertyWrapper for <see cref="RazorEntitySpawn"/> objectss.
    /// </summary>
    internal sealed class RazorEntitySpawnPropertyWrapper : EntityPropertyWrapper<RazorEntitySpawn>
    {
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

        [LocalizedDisplayName( "PropDisp_MaximumRazorBounceCount" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDisp_MaximumRazorBounceCount" )]
        public int MaximumBounceCount
        {
            get { return this.WrappedObject.MaximumBounceCount; }
            set { this.WrappedObject.MaximumBounceCount = value; }
        }

        [LocalizedDisplayName( "PropDisp_RazorBehaviourType" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescriptionAttribute( "PropDisp_RazorBehaviourType" )]
        public Zelda.Entities.Behaviours.RazorMovementBehaviour.RazorBehaviourType RazorBehaviourType
        {
            get { return this.WrappedObject.RazorBehaviourType; }
            set { this.WrappedObject.RazorBehaviourType = value; }
        }

        /// <summary>
        /// Returns a clone of this RazorEntitySpawnPropertyWrapper.
        /// </summary>
        /// <returns>
        /// The cloned IObjectPropertyWrapper.
        /// </returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new RazorEntitySpawnPropertyWrapper();
        }
    }
}
