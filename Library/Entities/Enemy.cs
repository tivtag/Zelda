// <copyright file="Enemy.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.Enemy class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities
{
    using System;
    using Atom.Math;
    using Zelda.Attacks;
    using Zelda.Entities.Behaviours;
    using Zelda.Entities.Components;
    using Zelda.Status;
    using Zelda.Saving;

    /// <summary>
    /// Represents an Entity that is an enemy of the Player.
    /// </summary>
    public class Enemy : ZeldaEntity, IReadOnlyLocalizedNameable,
        ICollisionWithPlayerNotifier, IAttackingEntity, IAttackableEntity, IMoveableEntity, IZeldaSetupable
    {
        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the <see cref="Enemy"/> class.
        /// </summary>
        public Enemy()
            : base( 10 )
        {
            // Create components
            this.moveable   = new Moveable();
            this.behaveable = new Behaveable();
            this.statable   = new Statable();
            this.killable   = new Killable();
            this.lootable   = new Lootable();
            this.visionable = new Visionable();
            this.attackable = new Attackable();

            // Add components
            this.Components.BeginSetup();
            {
                this.Components.Add( moveable );
                this.Components.Add( behaveable );
                this.Components.Add( statable );
                this.Components.Add( attackable );
                this.Components.Add( killable );
                this.Components.Add( visionable );
                this.Components.Add( lootable );
            }
            this.Components.EndSetup();

            this.statable.Damaged += this.OnDamaged;
            this.killable.Killed += this.OnKilled;

            this.meleeAttack = new Zelda.Attacks.MeleeAttack( this, null ) {
                Limiter = new Zelda.Attacks.Limiter.MeleeAttackSpeedBasedAttackLimiter( this.statable )
            };
        }

        /// <summary>
        /// Setups this <see cref="Enemy"/>.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            System.Diagnostics.Debug.Assert( serviceProvider != null );
            this.serviceProvider = serviceProvider;

            this.statable.Setup( serviceProvider );
            this.lootable.Setup( serviceProvider );
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the name that uniquely identifies this <see cref="Enemy"/>.
        /// </summary>
        public override string Name
        {
            get
            {                
                return base.Name;
            }

            set
            {
                if( value == null )
                    throw new ArgumentNullException( "value" );

                base.Name = value;

                this.LocalizedName = Resources.ResourceManager.GetString( "EN_" + value );
                if( this.LocalizedName == null )
                    this.LocalizedName = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the <see cref="AggressionType"/> of the Enemy controlled by this RandomEnemyMovementBehaviour.
        /// </summary>
        /// <remarks>
        /// Enemies with an aggression type of <see cref="AggressionType.Neutral"/> won't
        /// pursuit the enemy on sight.
        /// </remarks>
        public Zelda.Entities.Behaviours.AggressionType AgressionType
        {
            get
            {
                return this.aggressionType;
            }

            set 
            {
                this.aggressionType = value; 
            }
        }

        /// <summary>
        /// Gets the name of this <see cref="Enemy"/>.
        /// </summary>
        public string LocalizedName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this Enemy is currently attacking.
        /// </summary>
        public bool IsAttacking
        {
            get
            {
                if( meleeAttack == null )
                    return false;

                return meleeAttack.IsAttacking;
            }
        }

        /// <summary>
        /// Gets the default Melee Attack of this <see cref="Enemy"/>.
        /// </summary>
        public MeleeAttack MeleeAttack
        {
            get
            {
                return this.meleeAttack;
            }
        }

        #region > Components <

        /// <summary>
        /// Gets the <see cref="Moveable"/> component of this <see cref="Enemy"/> entity.
        /// </summary>
        public Moveable Moveable
        {
            get
            {
                return moveable;
            }
        }

        /// <summary>
        /// Gets the <see cref="Behaveable"/> component of this <see cref="Enemy"/> entity.
        /// </summary>
        public Behaveable Behaveable
        {
            get
            {
                return behaveable;
            }
        }

        /// <summary>
        /// Gets the <see cref="Statable"/> component of this <see cref="Enemy"/> entity.
        /// </summary>
        public Statable Statable
        {
            get
            {
                return statable;
            }
        }

        /// <summary>
        /// Gets the <see cref="Killable"/> component of this <see cref="Enemy"/> entity.
        /// </summary>
        public Killable Killable
        {
            get
            {
                return killable;
            }
        }

        /// <summary>
        /// Gets the <see cref="Lootable"/> component of this <see cref="Enemy"/> entity.
        /// </summary>
        public Lootable Lootable
        {
            get
            {
                return lootable;
            }
        }

        /// <summary>
        /// Gets the <see cref="Visionable"/> component of this <see cref="Enemy"/> entity.
        /// </summary>
        public Visionable Visionable
        {
            get
            {
                return visionable;
            }
        }

        /// <summary>
        /// Gets the <see cref="Attackable"/> component of this <see cref="Enemy"/> entity.
        /// </summary>
        public Attackable Attackable
        {
            get
            {
                return attackable;
            }
        }

        #endregion

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Notifies this <see cref="Enemy"/> entity that the <paramref name="player"/> is colliding with it.
        /// </summary>
        /// <param name="player">
        /// The corresponding <see cref="PlayerEntity"/>.
        /// </param>
        public void NotifyCollisionWithPlayer( PlayerEntity player )
        {
            if( this.meleeAttack != null && this.aggressionType != AggressionType.Neutral )
            {
                this.meleeAttack.Fire( player.Attackable );
            }
        }

        /// <summary>
        /// Updates this <see cref="Enemy"/> entity.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public override void Update( ZeldaUpdateContext updateContext )
        {
            if( this.meleeAttack != null )
            {
                this.meleeAttack.Update( updateContext );
            }

            base.Update( updateContext );
        }

        #region > Events <

        /// <summary>
        /// Gets called when this Enemy etity has been killed.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        private void OnKilled( Killable sender )
        {
            this.lootable.DropLoot( serviceProvider.Rand );
        }

        /// <summary>
        /// Gets called when this Enemy etity has been damaged.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The event data.
        /// </param>
        private void OnDamaged( Statable sender, AttackDamageResult e )
        {
            if( this.Scene == null )
                return;

            var flyingTextManager = this.Scene.FlyingTextManager;
            var center            = this.Collision.Center;

            Vector2 position;
            position.X = center.X;
            position.Y = center.Y - 10;

            switch( e.AttackReceiveType )
            {
                case Zelda.Attacks.AttackReceiveType.Hit:
                    flyingTextManager.FireAttackOnEnemyHit( position, e.Damage );
                    break;

                case Zelda.Attacks.AttackReceiveType.Crit:
                    flyingTextManager.FireAttackOnEnemyCrit( position, e.Damage );
                    break;

                case Zelda.Attacks.AttackReceiveType.PartialResisted:
                    flyingTextManager.FireAttackOnEnemyPartiallyResisted( position, e.Damage );
                    break;

                case Zelda.Attacks.AttackReceiveType.Miss:
                    flyingTextManager.FireAttackOnEnemyMissed( position );
                    break;

                case Zelda.Attacks.AttackReceiveType.Dodge:
                    flyingTextManager.FireAttackOnEnemyDodged( position );
                    break;
                case Zelda.Attacks.AttackReceiveType.Parry:
                    flyingTextManager.FireAttackOnEnemyParried( position );
                    break;

                case Zelda.Attacks.AttackReceiveType.Resisted:
                    flyingTextManager.FireAttackOnEnemyResisted( position );
                    break;

                default:
                    break;
            }
        }

        #endregion

        #region > Cloning <

        /// <summary>
        /// Setups the given <see cref="Enemy"/> entity to be a clone of this <see cref="Enemy"/> entity.
        /// </summary>
        /// <param name="clone">
        /// The Enemy entity to setup as a clone of this Enemy.
        /// </param>
        public void SetupClone( Enemy clone )
        {
            clone.aggressionType = this.aggressionType;
            clone.serviceProvider = this.serviceProvider;
            clone.MeleeAttack.DamageMethod = this.MeleeAttack.DamageMethod;

            // Clone components:
            this.moveable.SetupClone( clone.moveable );
            this.behaveable.SetupClone( clone.behaveable );
            this.statable.SetupClone( clone.statable );
            this.killable.SetupClone( clone.killable );
            this.lootable.SetupClone( clone.lootable );
            this.visionable.SetupClone( clone.visionable );

            base.SetupClone( clone );
        }

        /// <summary>
        /// Returns a clone of this <see cref="Enemy"/> entity.
        /// </summary>
        /// <returns>The cloned ZeldaEntity.</returns>
        public override ZeldaEntity Clone()
        {
            Enemy clone = new Enemy();

            this.SetupClone( clone );
            clone.Setup( this.serviceProvider );

            return clone;
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The aggression type of the enemy.
        /// </summary>
        private AggressionType aggressionType = AggressionType.AlwaysAggressive;

        /// <summary>
        /// The default Melee Attack of this <see cref="Enemy"/> entity.
        /// </summary>
        private readonly MeleeAttack meleeAttack;

        /// <summary>
        /// Identifies the <see cref="Moveable"/> component of this <see cref="Enemy"/> entity.
        /// </summary>
        private readonly Moveable moveable;

        /// <summary>
        /// Identifies the <see cref="Behaveable"/> component of this <see cref="Enemy"/> entity.
        /// </summary>
        private readonly Behaveable behaveable;
        
        /// <summary>
        /// Identifies the <see cref="Statable"/> component of this <see cref="Enemy"/> entity.
        /// </summary>
        private readonly Statable statable;

        /// <summary>
        /// Identifies the <see cref="Killable"/> component of this <see cref="Enemy"/> entity.
        /// </summary>
        private readonly Killable killable;

        /// <summary>
        /// Identifies the <see cref="Lootable"/> component of this <see cref="Enemy"/> entity.
        /// </summary>
        private readonly Lootable lootable;

        /// <summary>
        /// Identifies the <see cref="Visionable"/> component of this <see cref="Enemy"/> entity.
        /// </summary>
        private readonly Visionable visionable;

        /// <summary>
        /// Identifies the <see cref="Attackable"/> component of this <see cref="Enemy"/> entity.
        /// </summary>
        private readonly Attackable attackable;

        /// <summary>
        /// Provides fast access to game related services.
        /// </summary>
        private IZeldaServiceProvider serviceProvider;

        #endregion

        #region [ ReaderWriter ]

        /// <summary>
        /// Defines the <see cref="IEntityReaderWriter"/> for <see cref="Enemy"/> entities.
        /// </summary>
        internal sealed class ReaderWriter : EntityReaderWriter<Enemy>
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
            public override void Serialize( Enemy entity, Zelda.Saving.IZeldaSerializationContext context )
            {
                // First Write the header:
                const int Version = 10;
                context.Write( Version );

                // Write Name:
                context.Write( entity.Name );
                context.Write( (byte)entity.AgressionType );
                context.Write( (byte)entity.FloorRelativity );

                // Write Collision component
                entity.Collision.Serialize( context );

                // Write Moveable component
                entity.moveable.Serialize( context );
                
                // Write Killable component
                entity.killable.Serialize( context );
                
                // Write Statable component
                entity.statable.Serialize( context );

                // Write Visionable component
                entity.visionable.Serialize( context );

                // Write Lootable component
                entity.lootable.Serialize( context );
                
                // Write DDaS
                context.WriteDrawStrategy( entity.DrawDataAndStrategy );

                // Write Damage Method
                if( entity.MeleeAttack.DamageMethod != null )
                {
                    context.Write( true );
                    entity.MeleeAttack.DamageMethod.Serialize( context );
                }
                else
                {
                    context.Write( false );
                }

                // Write Behaveable component
                entity.behaveable.Serialize( context );
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
            public override void Deserialize( Enemy entity, Zelda.Saving.IZeldaDeserializationContext context )
            {
                entity.Setup( serviceProvider );
                
                // Header
                const int CurrentVersion = 10;
                int version = context.ReadInt32();
                Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, this.GetType() );
                
                // Properties
                entity.Name = context.ReadString();
                entity.aggressionType = (AggressionType)context.ReadByte(); // new in 8
                entity.FloorRelativity = (EntityFloorRelativity)context.ReadByte(); // new in 9

                // Read Collision component
                entity.Collision.Deserialize( context );

                // Read Moveable component
                entity.moveable.Deserialize( context );
                
                // Read Killable component
                entity.killable.Deserialize( context );

                // Read Statable component
                entity.statable.Deserialize( context );

                // Read Visionable component
                entity.visionable.Deserialize( context );

                // Read Lootable component
                entity.lootable.Deserialize( context );

                // Read draw data and strategy:
                entity.DrawDataAndStrategy = context.ReadDrawStrategy(entity);

                // Read damage method of default melee attack:
                if( context.ReadBoolean() )
                {
                    string methodTypeName = context.ReadString();
                    Type   methodType     = Type.GetType( methodTypeName );

                    var method = (AttackDamageMethod)Activator.CreateInstance( methodType );
                    method.Deserialize( context );
                    method.Setup( this.serviceProvider );

                    entity.MeleeAttack.DamageMethod = method;
                }
                else
                {
                    var method = new Zelda.Attacks.Melee.DefaultMeleeDamageMethod();
                    method.Setup( this.serviceProvider );

                    entity.MeleeAttack.DamageMethod = method;
                }

                // Read Behaveable component
                entity.behaveable.Deserialize( context ); // Moved down in V10

                // Finalize loading *hack*
                IPostLoad postLoad = entity.DrawDataAndStrategy as IPostLoad;
                if( postLoad != null )
                    postLoad.PostLoad( this.serviceProvider );
            }
        }

        #endregion
    }
}
