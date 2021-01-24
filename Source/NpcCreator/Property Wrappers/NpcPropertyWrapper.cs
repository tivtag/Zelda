// <copyright file="NpcPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.NpcCreator.NpcPropertyWrapper class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.NpcCreator
{
    using Atom.Design;

    /// <summary>
    /// Defines a basic implemention of an <see cref="IObjectPropertyWrapper"/>
    /// that wraps around the properties of a <see cref="Zelda.Entities.ZeldaEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of the entity beeing wrapped by this IObjectPropertyWrapper.
    /// </typeparam>
    internal abstract class NpcPropertyWrapper<TEntity> : BaseObjectPropertyWrapper<TEntity>, INpcPropertyWrapper
        where TEntity : Zelda.Entities.ZeldaEntity
    {
        /// <summary>
        /// Gets or sets the name of the ZeldaEntity this NpcPropertyWrapper{TEntity} wraps around.
        /// </summary>
        [LocalizedDisplayName( "PropDisp_Name" )]
        [LocalizedCategory( "PropCate_Identification" )]
        [LocalizedDescriptionAttribute( "PropDesc_Name" )]
        public string Name
        {
            get
            {
                return this.WrappedObject.Name;
            }

            set
            {
                if( value == this.Name )
                    return;

                this.WrappedObject.Name = value;
                this.OnPropertyChanged( "Name" );
            }
        }

        /// <summary>
        /// Gets a value indicating whether this NpcPropertyWrapper
        /// requires the Loot tab of the NPC Creator to be accessable.
        /// </summary>
        /// <value>The default value is true.</value>
        [System.ComponentModel.Browsable(false)]
        public virtual bool HasLoot
        {
            get { return true; }
        }

        /// <summary>
        /// Applies any additional data for this NpcPropertyWrapper.
        /// </summary>
        /// <param name="window">
        /// The application's MainWindow.
        /// </param>
        public virtual void ApplyData( MainWindow window )
        {
        }

        /// <summary>
        /// Ensures the correctness of the current state of the NPC.
        /// </summary>
        /// <returns>
        /// true if the data is in a correct state and can be saved;
        /// otherwise false.
        /// </returns>
        public virtual bool Ensure()
        {
            if( this.Name == null || this.Name.Length == 0 )
            {
                System.Windows.MessageBox.Show(
                    Properties.Resources.Info_NameMustBeSet, 
                    string.Empty, 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Information
                );

                return false;
            }

            return true;
        }

        /// <summary>
        /// Applies any additional data of this INpcPropertyWrapper to the View.
        /// </summary>
        /// <param name="window">
        /// The application's MainWindow.
        /// </param>
        public virtual void SetupView( MainWindow window )
        {
        }
    }
}
