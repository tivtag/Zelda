// <copyright file="ProcEffect.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Procs.ProcEffect class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Procs
{
    using System;
    using System.ComponentModel;
    using Atom;
    using Zelda.Status.Hooks;
    
    /// <summary>
    /// Represents a StatusEffect that procs some other effect.
    /// </summary>
    /// <seealso cref="IProcChance"/>
    /// <seealso cref="IStatusHook"/>
    public abstract class ProcEffect : StatusEffect
    {
        #region [ Properties ]

        /// <summary>
        /// Gets or sets the <see cref="IProcChance"/> of this TimedProcEffect.
        /// </summary>
        [Editor( typeof( Zelda.Status.Procs.Design.ProcChanceEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public IProcChance ProcChance
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="IStatusHook"/> that hooks up this TimedProcEffect
        /// with the statable entity that owns the effect.
        /// </summary>
        [Editor( typeof( Zelda.Status.Hooks.Design.StatusHookEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public IStatusHook Hook
        {
            get
            {
                return this._hook;
            }

            set
            {
                if( this._hook != null )
                {
                    this._hook.Invoked -= this.OnHookInvoked;
                }

                this._hook = value;
            }
        }

        /// <summary> 
        /// Gets an unique string that represents what this <see cref="StatusEffect"/> manipulates.
        /// </summary>
        public override string Identifier
        {
            get
            {
                return string.Empty;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Called when this <see cref="StatusEffect"/> gets enabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that enabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnEnable( Statable user )
        {
            if( this._hook != null )
            {
                this._hook.Hook( user );
                this._hook.Invoked += this.OnHookInvoked;
            }
        }

        /// <summary>
        /// Called when this <see cref="StatusEffect"/> gets disabled for the given statable Entity.
        /// </summary>
        /// <param name="user">
        /// The statable that disabled this <see cref="StatusEffect"/>.
        /// </param>
        public override void OnDisable( Statable user )
        {
            if( this._hook != null )
            {
                this._hook.Unhook( user );
                this._hook.Invoked -= this.OnHookInvoked;
            }
        }

        /// <summary>
        /// Called when the <see cref="Hook"/> has been invoked.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="invoker">
        /// The Statable component of the entity that has invoked this TimedProcEffect.
        /// </param>
        private void OnHookInvoked( object sender, Statable invoker )
        {
            if( this.HasProcced( invoker ) )
            {
                this.OnProccedPrivate( invoker );
            }
        }

        /// <summary>
        /// Gets a value indicating whether this TimedProcEffect has procced;
        /// </summary>
        /// <param name="invoker">
        /// The Statable component of the entity that has invoked this TimedProcEffect.
        /// </param>
        /// <returns>
        /// Returns true if a proc has occurred;
        /// otherwise false.
        /// </returns>
        protected virtual bool HasProcced( Statable invoker )
        {
            if( this.ProcChance != null )
            {
                var rand = invoker.Scene.Rand; // Might want to re-factor this.
                return this.ProcChance.TryProc( invoker, rand );
            }

            return true;
        }

        /// <summary>
        /// Called when this ProcEffect has actually procced.
        /// </summary>
        /// <param name="invoker">
        /// The Statable component of the entity that has invoked this TimedProcEffect.
        /// </param>
        private void OnProccedPrivate( Statable invoker )
        {
            this.OnProcced( invoker );
        }

        /// <summary>
        /// Called when this ProcEffect has actually procced.
        /// </summary>
        /// <param name="invoker">
        /// The Statable component of the entity that has invoked this TimedProcEffect.
        /// </param>
        protected abstract void OnProcced( Statable invoker );

        /// <summary>
        /// Gets a value indicating whether the given StatusEffect is 'equal' to this StatusEffect.
        /// </summary>
        /// <param name="effect">
        /// The StatusEffect to compare with this.
        /// </param>
        /// <returns>
        /// Returns true if they capture the same 'concept';
        /// otherwise false.
        /// </returns>
        public override bool Equals( StatusEffect effect )
        {
            if( effect == null )
                return false;
            return effect.GetType() == this.GetType();
        }

        /// <summary>
        /// Gets a human-readable description on how this TimedStatusProcEffect is proccing.
        /// </summary>
        /// <param name="statable">
        /// The statable component of the entity that wants to receive the description.
        /// </param>
        /// <returns>
        /// A short localized string.
        /// </returns>
        protected string GetProcChanceDescription( Statable statable )
        {
            if( this.Hook == null )
                return string.Empty;

            return this.Hook.GetShortDescription( statable );
        }

        #region > Cloning <

        /// <summary>
        /// Setups the given ProcEffect to be a clone of this ProcEffect.
        /// </summary>
        /// <param name="clone">
        /// The StatusEffect to setup as a clone of this StatusEffect.
        /// </param>
        protected void SetupClone( ProcEffect clone )
        {
            // Might need to add actual cloning
            // here.
            clone.Hook = this.Hook != null ? this.Hook.Clone() : null;
            clone.ProcChance = this.ProcChance;

            base.SetupClone( clone );
        }

        #endregion

        #region > Storage <
            
        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <remarks>
        /// This method should be called first when overriding this method
        /// in a sub-class. It writes the global header of the StatusEffect.
        /// </remarks>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            base.Serialize( context );

            const int CurrentVersion = 1;
            context.Write( CurrentVersion );

            // Proc Chance
            if( this.ProcChance != null )
            {
                context.Write( this.ProcChance.GetType().GetTypeName() );
                this.ProcChance.Serialize( context );
            }
            else
            {
                context.Write( string.Empty );
            }

            // Hook
            if( this.Hook != null )
            {
                context.Write( this.Hook.GetType().GetTypeName() );
                this.Hook.Serialize( context );
            }
            else
            {
                context.Write( string.Empty );
            }
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <remarks>
        /// This method should be called first when overriding this method
        /// in a sub-class. It reads the global header/data of the StatusEffect.
        /// </remarks>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            base.Deserialize( context );

            const int CurrentVersion = 1;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, CurrentVersion, "ProcEffect" );

            // Proc Chance
            string procChanceTypeName = context.ReadString();

            if( procChanceTypeName.Length > 0 )
            {
                Type type = Type.GetType( procChanceTypeName );

                this.ProcChance = (IProcChance)Activator.CreateInstance( type );
                this.ProcChance.Deserialize( context );
            }

            // Hook
            string hookTypeName = context.ReadString();

            if( hookTypeName.Length > 0 )
            {
                Type type = Type.GetType( hookTypeName );

                this.Hook = (IStatusHook)Activator.CreateInstance( type );
                this.Hook.Deserialize( context );
            }
        }

        #endregion

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The storage field of the <see cref="Hook"/> property.
        /// </summary>
        private IStatusHook _hook;

        #endregion
    }
}
