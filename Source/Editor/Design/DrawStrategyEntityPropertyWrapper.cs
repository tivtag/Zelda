
namespace Zelda.Editor.Design
{
    using System;
    using System.ComponentModel;
    using Atom.Diagnostics.Contracts;
    using Atom.Design;
    using Zelda.Design;
    using Zelda.Entities;
    using Zelda.Entities.Drawing;

    /// <summary>
    /// Represents a property wrapper, used in objects that are attached to a PropertyGrid, around the DrawDataAndStrategy of the ZeldaEntity class.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public sealed class DrawStrategyEntityPropertyWrapper
    {
        /// <summary>
        /// Initializes a new instance of the DrawStrategyEntityPropertyWrapper class.
        /// </summary>
        /// <param name="propertyWrapper"></param>
        public DrawStrategyEntityPropertyWrapper( IObjectPropertyWrapper propertyWrapper )
        {
            Contract.Requires<ArgumentNullException>( propertyWrapper != null );

            this.propertyWrapper = propertyWrapper;
        }

        /// <summary>
        /// Gets or sets the type of the IDrawDataAndStrategy that is to be used.
        /// </summary>
        [LocalizedCategory( "PropCate_Visuals" )]
        [LocalizedDescriptionAttribute( "PropDesc_DrawDataAndStrategyType" )]
        [Editor( typeof( Zelda.Entities.Drawing.Design.DrawDataAndStrategyTypeEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Type Type
        {
            get
            {
                return this.Entity.DrawDataAndStrategy != null ? this.Entity.DrawDataAndStrategy.GetType() : null;
            }

            set
            {
                if( value == null )
                {
                    this.Entity.DrawDataAndStrategy = null;
                }
                else
                {
                    DrawStrategyManager strategyManager = DesignTime.Services.DrawStrategyManager;
                    this.Entity.DrawDataAndStrategy = strategyManager.GetStrategyClone( value, this.Entity );
                }
            }
        }

        /// <summary>
        /// Allows the user to reload the IDrawDataAndStrategy.
        /// </summary>
        [LocalizedDescriptionAttribute( "PropDesc_LoadDrawDataAndStrategyToggle" )]
        [LocalizedCategory( "PropCate_Visuals" )]
        [Editor( typeof( Atom.Design.ToggleEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public object Reload
        {
            get
            {
                return true;
            }

            set
            {
                if( this.Entity.DrawDataAndStrategy != null )
                {
                    this.Entity.DrawDataAndStrategy.Load( DesignTime.Services );
                }
            }
        }

        /// <summary>
        /// Gets the IDrawDataAndStrategy of the entity.
        /// </summary>
        [LocalizedCategory( "PropCate_Visuals" )]
        [LocalizedDescription( "PropDesc_DrawDataAndStrategy" )]
        public Zelda.Entities.Drawing.IDrawDataAndStrategy Settings
        {
            get
            {
                return this.Entity.DrawDataAndStrategy;
            }
        }

        /// <summary>
        /// Gets the entity this DrawStrategyEntityPropertyWrapper wraps around.
        /// </summary>
        private ZeldaEntity Entity
        {
            get
            {
                return (ZeldaEntity)this.propertyWrapper.WrappedObject;
            }
        }

        private readonly IObjectPropertyWrapper propertyWrapper;
    }
}
