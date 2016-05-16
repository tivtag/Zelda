
namespace Zelda.Editor.Design
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using Atom.Design;
    using Zelda.Design;
    using Zelda.Entities;

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
                    var strategyManager = DesignTime.Services.DrawStrategyManager;
                    this.Entity.DrawDataAndStrategy = strategyManager.GetStrategyClone( value, this.Entity );
                }
            }
        }

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

        [LocalizedCategory( "PropCate_Visuals" )]
        [LocalizedDescriptionAttribute( "PropDesc_DrawDataAndStrategy" )]
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
