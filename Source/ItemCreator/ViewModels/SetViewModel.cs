// <copyright file="SetViewModel.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.ItemCreator.SetViewModel class.</summary>
// <author>Paul Ennemoser</author>

namespace Zelda.ItemCreator
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using Atom.Design;
    using Zelda.Items.Sets;
    using Zelda.Saving;

    /// <summary>
    /// Defines the view-model/property wrapper around the <see cref="Set"/> class.
    /// </summary>
    internal sealed class SetViewModel : BaseObjectPropertyWrapper, IObjectViewModel
    {
        #region [ Wrapped Properties ]

        [LocalizedCategory( "PropCate_Set" )]
        [LocalizedDisplayName( "PropDisp_Name" )]
        [LocalizedDescription( "PropDesc_Name" )]
        public string Name
        {
            get
            {
                return this.WrappedSet.Name;
            }

            set
            { 
                this.WrappedSet.Name = value;
            }
        }

        [LocalizedCategory( "PropCate_Set" )]
        [LocalizedDisplayName( "PropDisp_LocalizedName" )]
        [LocalizedDescription( "PropDesc_LocalizedName" )]
        public string LocalizedName
        {
            get
            {
                return this.WrappedSet.LocalizedName;
            }
        }

        [LocalizedCategory( "PropCate_Set" )]
        [LocalizedDisplayName( "PropDisp_SetBonus" )]
        [LocalizedDescription( "PropDesc_SetBonus" )]
        public ISetBonus Bonus
        {
            get
            {
                return this.WrappedSet.Bonus;
            }
        }
        
        [LocalizedCategory( "PropCate_Set" )]
        [LocalizedDisplayName( "PropDisp_SetItems" )]
        [LocalizedDescription( "PropDesc_SetItems" )]
        [Editor( typeof( Design.SetItemListEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public IList<ISetItem> Items
        {
            get
            {
                return this.WrappedSet.Items;
            }
        }

        #endregion

        #region [ Wrapper ]

        /// <summary>
        /// Gets the Set this <see cref="SetViewModel"/> wraps around.
        /// </summary>
        [Browsable( false )]
        public Set WrappedSet
        {
            get
            {
                return (Set)this.WrappedObject;
            }
        }

        /// <summary>
        /// Gets the <see cref="Type"/> this <see cref="SetViewModel"/> wraps around.
        /// </summary>
        [Browsable( false )]
        public override Type WrappedType
        {
            get
            {
                return typeof( Set );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetViewModel"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game related services.
        /// </param>
        internal SetViewModel( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Returns a clone of this <see cref="SetViewModel"/>.
        /// </summary>
        /// <returns>The cloned IObjectPropertyWrapper.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new ItemViewModel( this.serviceProvider );
        }

        /// <summary>
        /// Gets the <see cref="IZeldaServiceProvider"/> which provides
        /// fast access to game-related services.
        /// </summary>
        private IZeldaServiceProvider ServiceProvider
        {
            get
            {
                return this.serviceProvider;
            }
        }

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;

        #endregion

        #region [ ViewModel ]

        /// <summary>
        /// Saves this SetViewModel.
        /// </summary>
        public void Save()
        {
            string directory = this.GetDirectory();
            string fullPath = directory + this.GetFileName();
            Directory.CreateDirectory( directory );

            using( var stream = File.Create( fullPath ) )
            {
                var context = new Zelda.Saving.SerializationContext( new BinaryWriter( stream ), this.serviceProvider );
                context.WriteObject( this.WrappedSet );
            }
        }

        /// <summary>
        /// Gets the full path of the directory in which
        /// the object is saved in.
        /// </summary>
        /// <returns></returns>
        private string GetDirectory()
        {
            return Path.Combine( AppDomain.CurrentDomain.BaseDirectory, this.GetRelativeDirectory() );
        }

        /// <summary>
        /// Gets the directory, relative to the game folder,
        /// in which the object is saved in.
        /// </summary>
        /// <returns></returns>
        private string GetRelativeDirectory()
        {
            return @"Content\Items\Sets\";
        }

        /// <summary>
        /// Gets the filename under which the item is saved.
        /// </summary>
        /// <returns></returns>
        private string GetFileName()
        {
            return this.Name + ".zset";
        }

        #endregion
    }
}
