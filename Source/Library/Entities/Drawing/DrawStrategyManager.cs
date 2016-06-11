// <copyright file="DrawStrategyManager.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Drawing.DrawStrategyManager class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Drawing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The <see cref="DrawStrategyManager"/> manages the creation of <see cref="IDrawDataAndStrategy"/>ies.
    /// This class can't be inherited.
    /// </summary>
    /// <remarks>
    /// The <see cref="DrawStrategyManager"/> only stores uninitialized
    /// <see cref="IDrawDataAndStrategy"/>ies.
    /// </remarks>
    public sealed class DrawStrategyManager
    {
        #region [ Properties ]

        /// <summary>
        /// Gets a collection that contains the names of all known strategies.
        /// </summary>
        public ICollection<string> KnownStrategyNames
        {
            get 
            { 
                return this.dict.Keys;
            }
        }

        /// <summary>
        /// Gets a collection that contains the types of all known strategies.
        /// </summary>
        public IEnumerable<Type> KnownStrategies
        {
            get
            {
                return this.dict.Values.Select( strategy => strategy.GetType() );
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawStrategyManager"/> class.
        /// </summary>
        public DrawStrategyManager()
        {
            this.dict = new Dictionary<string, IDrawDataAndStrategy>( 16 );
        }

        /// <summary>
        /// Loads the default IDrawDataAndStrategies into this <see cref="DrawStrategyManager"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void LoadDefaults( IZeldaServiceProvider serviceProvider )
        {
            var rand = serviceProvider.Rand;

            this.Register( new OneDirDrawDataAndStrategy() );
            this.Register( new TintedOneDirDrawDataAndStrategy() );

            this.Register( new OneDirAnimDrawDataAndStrategy() );
            this.Register( new OneDirAnimMoveStandDrawDataAndStrategy() );
            this.Register( new OneDirMoveStandAttackAnimDrawDataAndStrategy() );
            this.Register( new RandomOneDirAnimDrawDataAndStrategy( rand ) );
            this.Register( new TintedOneDirAnimMoveStandDrawDataAndStrategy() );
            this.Register( new TintedOneDirAnimDrawDataAndStrategy() );
            this.Register( new TintedAndScaledOneDirAnimDrawDataAndStrategy() );

            this.Register( new FourDirDrawDataAndStrategy() );

            this.Register( new FourDirAnimDrawDataAndStrategy() );
            this.Register( new TintedFourDirAnimDrawDataAndStrategy() );

            this.Register( new LeftRightMoveAttackDDS() );
            this.Register( new TintedLeftRightMoveAttackDDS() );
            this.Register( new LeftRightMoveStandDrawDataAndStrategy() );
            this.Register( new HorizontalMoveRandomStandAttackDDS( rand ) );

            this.Register( new DualSwitchableDrawDataAndStrategy() );
            this.Register( new RangedEnemyDrawDataAndStrategy() );
        }

        #endregion

        #region [ Methods ]

        #region GetStrategyClone

        /// <summary>
        /// Gets a clone of the <see cref="IDrawDataAndStrategy"/> 
        /// with the given <paramref name="name"/> for the given
        /// <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="IDrawDataAndStrategy"/> to get.</param>
        /// <param name="newOwner">The entity to visualize with the <see cref="IDrawDataAndStrategy"/>.</param>
        /// <returns>A clone of the <see cref="IDrawDataAndStrategy"/>.</returns>
        public IDrawDataAndStrategy GetStrategyClone( string name, ZeldaEntity newOwner )
        {
            return dict[name].Clone( newOwner );
        }

        /// <summary>
        /// Gets a clone of the <see cref="IDrawDataAndStrategy"/> 
        /// of the given <paramref name="type"/> for the given
        /// <see cref="ZeldaEntity"/>.
        /// </summary>
        /// <param name="type">The type of the <see cref="IDrawDataAndStrategy"/> to get.</param>
        /// <param name="newOwner">The entity to visualize with the <see cref="IDrawDataAndStrategy"/>.</param>
        /// <returns>A clone of the <see cref="IDrawDataAndStrategy"/>.</returns>
        public IDrawDataAndStrategy GetStrategyClone( Type type, ZeldaEntity newOwner )
        {
            return dict[GetName(type)].Clone( newOwner );
        }

        #endregion

        #region GetName

        /// <summary>
        /// Utility method that returns the name that identifies the given <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="drawDataAndStrategy">The IDrawDataAndStrategy to receive the name for.</param>
        /// <returns>The name that identifies the IDrawDataAndStrategy.</returns>
        public static string GetName( IDrawDataAndStrategy drawDataAndStrategy )
        {
#if DEBUG
            if( drawDataAndStrategy == null )
                throw new ArgumentNullException( "drawDataAndStrategy" );
#endif

            return drawDataAndStrategy.GetType().FullName;
        }

        /// <summary>
        /// Utility method that returns the name that identifies the given <see cref="IDrawDataAndStrategy"/>.
        /// </summary>
        /// <param name="type">The type of the IDrawDataAndStrategy to receive the name for.</param>
        /// <returns>The name that identifies the IDrawDataAndStrategy.</returns>
        public static string GetName( Type type )
        {
#if DEBUG
            if( type == null )
                throw new ArgumentNullException( "type" );
#endif
            return type.FullName;
        }

        #endregion

        /// <summary>
        /// Utility method that registers the given IDrawDataAndStrategy.
        /// </summary>
        /// <param name="drawDataAndStrategy">
        /// The IDrawDataAndStrategy to register.
        /// </param>
        private void Register( IDrawDataAndStrategy drawDataAndStrategy )
        {
            this.dict.Add( GetName( drawDataAndStrategy ), drawDataAndStrategy );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The dictionary that stores the known <see cref="IDrawDataAndStrategy"/> implementations; sorted by name.
        /// </summary>
        private readonly Dictionary<string, IDrawDataAndStrategy> dict;

        #endregion
    }
}
