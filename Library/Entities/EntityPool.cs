// <copyright file="EntityPool.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Entities.EntityPool{TEntity} class.
// </summary>
// <author>
//     Paul Ennemoser (Tick)
// </author>

namespace Zelda.Entities
{
    using System.Linq;
    using Atom.Collections.Pooling;

    /// <summary>
    /// Defines a <see cref="Pool{TEntity}"/> that contains poolable <see cref="ZeldaEntity"/>
    /// objects.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of Entity the EntityPool can contain.
    /// </typeparam>
    public class EntityPool<TEntity> : WrappingPool<TEntity>
        where TEntity : ZeldaEntity
    {
        /// <summary>
        /// Initializes a new instance of the EntityPool class.
        /// </summary>
        /// <param name="initialSize">
        /// The initial size of the new EntityPool{TEntity}.
        /// </param>
        /// <param name="creator">
        /// The creation function of the new EntityPool.
        /// </param>
        public EntityPool( int initialSize, PooledObjectCreator<IPooledObjectWrapper<TEntity>> creator )
            : base( initialSize, creator )
        {
        }

        /// <summary>
        /// Called when the given PoolNode&lt;IPooledObjectWrapper&lt;TEntity&gt;&gt; has been created
        /// for this Pool{T}.
        /// </summary>
        /// <param name="node">
        /// The PoolNode{T} that has just been created.
        /// This value is never null.
        /// </param>
        protected override void OnCreated( PoolNode<IPooledObjectWrapper<TEntity>> node )
        {
            IPooledObjectWrapper<TEntity> wrapper = node.Item;
            TEntity aura = wrapper.PooledObject;

            wrapper.PoolNode = node;
            aura.Removed += this.OnEntityRemoved;
        }

        /// <summary>
        /// Called when an Entity created by this EntityPool has been disabled.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="scene">
        /// The scene from which the entity was removed from.
        /// </param>
        private void OnEntityRemoved( object sender, ZeldaScene scene )
        {
            var entity = (TEntity)sender;
            var entityNode = this.ActiveNodes.First( node => node.Item.PooledObject == entity );

            if( entityNode != null )
            {
                this.Return( entityNode );
            }
        }
    }
}
