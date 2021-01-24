// <copyright file="RetaininglyActivateEntitySpawnAction.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Actions.RetaininglyActivateEntitySpawnAction class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Actions.Scene
{
    using Atom.Storage;
    using Zelda.Entities.Data;
    using Zelda.Saving;

    /// <summary>
    /// Represents an action that when executed activates a specific entity.
    /// </summary>
    /// <seealso cref="IActivatable"/>
    public sealed class ActivateEntityAction : BaseSceneAction
    {
        /// <summary>
        /// Gets or sets the name that uniquely identifies the entity
        /// which gets activated by this ActivateEntityAction.
        /// </summary>
        public string EntityName 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets a value indicating whether the entity keeps activated
        /// even after re-loading the game.
        /// </summary>
        /// <remarks>
        /// This is done by saving the Activatable State in the save file.
        /// </remarks>
        public bool RetainsState
        {
            get;
            set;
        }

        /// <summary>
        /// Executes this ActivateEntityAction.
        /// </summary>
        public override void Execute()
        {
            this.SetState( true );
        }

        /// <summary>
        /// Deferredly undoes this IAction.
        /// </summary>
        public override void Dexecute()
        {
            this.SetState( false );
        }

        /// <summary>
        /// Sets the IsAvtive state to the given value.
        /// </summary>
        /// <param name="state">
        /// The value to set.
        /// </param>
        private void SetState( bool state )
        {
            if( string.IsNullOrEmpty( this.EntityName ) )
                return;

            var scene = this.Scene;
            var activatable = scene.GetEntity( this.EntityName ) as IActivatable;
            if( activatable == null )
                return;

            activatable.IsActive = state;

            if( this.RetainsState )
            {
                this.RetainState( state, scene.Status );
            }
        }

        /// <summary>
        /// Retains the activated state of the entity by writing it to the save file.
        /// </summary>
        /// <param name="state">
        /// The state to set.
        /// </param>
        /// <param name="sceneStatus">
        /// Stores the current status of the scene.
        /// </param>
        private void RetainState( bool state, SceneStatus sceneStatus )
        {
            if( sceneStatus == null )
                return;

            var dataStore = sceneStatus.EntityDataStore;
            var data = dataStore.GetOrCreate<ActivatableData>( this.EntityName );
            data.IsActive = state;
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Serialize( ISerializationContext context )
        {
            context.WriteDefaultHeader();

            context.Write( this.EntityName ?? string.Empty );
            context.Write( this.RetainsState );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public override void Deserialize( IDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );

            this.EntityName = context.ReadString();
            this.RetainsState = context.ReadBoolean();
        }
    }
}
