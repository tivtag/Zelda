// <copyright file="RazorEntitySpawn.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Spawning.RazorEntitySpawn class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities.Spawning
{
    using System;
    using Atom.Math;
    using Zelda.Entities.Behaviours;
    using Zelda.Entities.Components;
    
    /// <summary>
    /// Represents an <see cref="EntitySpawn"/> that manages the spawning
    /// of a <see cref="Behaveable"/> <see cref="ZeldaEntity"/> that uses the
    /// <see cref="RazorMovementBehaviour"/>.
    /// This class can't be inherited.
    /// </summary>
    public sealed class RazorEntitySpawn : EntitySpawn
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the maximum number of bounces
        /// that may occur before the razor entity stops to bounce.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Set: If the given value is negative.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// If no entity template has been set.
        /// </exception>
        public int MaximumBounceCount
        {
            get
            {
                if( this.razorBehaviour == null )
                    throw new InvalidOperationException( Resources.Error_EntityTemplateMustBeSet );

                return razorBehaviour.MaximumBounceCount;
            }

            set
            {
                if( this.razorBehaviour == null )
                    throw new InvalidOperationException( Resources.Error_EntityTemplateMustBeSet );

                razorBehaviour.MaximumBounceCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="RazorBehaviourType"/> of this RazorMovementBehaviour,
        /// which controls how the razor entity moves or gets triggered.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If no entity template has been set.
        /// </exception>
        public RazorMovementBehaviour.RazorBehaviourType RazorBehaviourType
        {
            get
            {
                if( this.razorBehaviour == null )
                    throw new InvalidOperationException( Resources.Error_EntityTemplateMustBeSet );

                return razorBehaviour.BehaviourType;
            }

            set
            {
                if( this.razorBehaviour == null )
                    throw new InvalidOperationException( Resources.Error_EntityTemplateMustBeSet );

                razorBehaviour.BehaviourType = value;
            }
        }

        /// <summary>
        /// Gets the ZeldaEntity that gets spawned by this <see cref="RazorEntitySpawn"/>.
        /// </summary>
        public override ZeldaEntity Entity
        {
            get 
            {
                return base.Entity;
            }

            protected set
            {
                if( value != null )
                {
                    Behaveable behaveable = value.Components.Get<Behaveable>();
                    if( behaveable == null )
                        throw new Atom.Components.ComponentNotFoundException( typeof( Behaveable ) );

                    this.razorBehaviour = behaveable.GetBehaviour( typeof( RazorMovementBehaviour ) ) as RazorMovementBehaviour;                    
                    if( razorBehaviour == null )
                    {
                        throw new Atom.NotFoundException(
                            string.Format( 
                                System.Globalization.CultureInfo.CurrentCulture,
                                Resources.Error_BehaviourXNotFoundInBehaveableOfY,
                                "RazorMovementBehaviour",
                                value.ToString()
                            )
                        );
                    }
                }
                else
                {
                    this.razorBehaviour = null;
                }

                base.Entity = value;
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the RazorEntitySpawn class.
        /// </summary>
        public RazorEntitySpawn()
        {
            this.UseTemplate = true;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Identifies the RazorMovementBehaviour of the razor <see cref="Entity"/>.
        /// </summary>
        private RazorMovementBehaviour razorBehaviour;

        #endregion

        #region [ ReaderWriter ]

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="RazorEntitySpawn"/> entities.
        /// </summary>
        internal new sealed class ReaderWriter : EntityReaderWriter<RazorEntitySpawn>
        {
            /// <summary>
            /// Initializes a new instance of the ReaderWriter class.
            /// </summary>   
            /// <param name="serviceProvider">
            /// Provides fast access to game-related services.
            /// </param>
            public ReaderWriter( IZeldaServiceProvider serviceProvider )
                : base( serviceProvider )
            {
            }

            /// <summary>
            /// Serializes the given object using the given <see cref="System.IO.BinaryWriter"/>.
            /// </summary>
            /// <param name="entity">
            /// The entity to serialize.
            /// </param>
            /// <param name="context">
            /// The context under which the serialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public override void Serialize( RazorEntitySpawn entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                // Header:
                const int CurrentVersion = 2;
                context.Write( CurrentVersion );

                // Data:
                context.Write( entity.Name );
                context.Write( entity.TemplateName ?? string.Empty );

                context.Write( entity.Transform.X );
                context.Write( entity.Transform.Y );
                context.Write( (int)entity.Transform.Direction );
                context.Write( entity.FloorNumber );

                if( entity.razorBehaviour != null )
                {
                    context.Write( true );
                    entity.razorBehaviour.Serialize( context );
                }
                else
                {
                    context.Write( false );
                }
            }

            /// <summary>
            /// Deserializes the data in the given <see cref="System.IO.BinaryWriter"/> to initialize
            /// the given ZeldaEntity.
            /// </summary>
            /// <param name="entity">
            /// The ZeldaEntity to initialize.
            /// </param>
            /// <param name="context">
            /// The context under which the deserialization process takes place.
            /// Provides access to required objects.
            /// </param>
            public override void Deserialize( RazorEntitySpawn entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                entity.Setup( serviceProvider );

                // Header
                const int CurrentVersion = 2;
                int version = context.ReadInt32();
                Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );

                // Read
                entity.Name = context.ReadString();
                entity.TemplateName = context.ReadString();

                entity.Transform.Position = context.ReadVector2();
                entity.Transform.Direction = (Direction4)context.ReadInt32();
                entity.FloorNumber = context.ReadInt32();

                // Read Razor Behaviour
                if( context.ReadBoolean() )
                {
                   entity.razorBehaviour.Deserialize( context );
                }
            }
        }

        #endregion
    }
}