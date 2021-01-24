// <copyright file="AuraPool.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.Status.Pooling.AuraPool{TAura} class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Status.Pooling
{
    using System.Linq;
    using Atom.Collections.Pooling;

    /// <summary>
    /// Defines a <see cref="Pool{TAura}"/> that contains poolable <see cref="Aura"/>
    /// objects.
    /// </summary>
    /// <typeparam name="TAura">
    /// The type of Aura the AuraPool can contain.
    /// </typeparam>
    public sealed class AuraPool<TAura> : WrappingPool<TAura>
        where TAura : Aura
    {
        /// <summary>
        /// Initializes a new instance of the AuraPool class.
        /// </summary>
        /// <param name="initialCapacity">
        /// The initial capacity of the new AuraPool{TAura}.
        /// </param>
        /// <param name="creator">
        /// The creation function of the new AuraPool.
        /// </param>
        private AuraPool( int initialCapacity, PooledObjectCreator<IPooledObjectWrapper<TAura>> creator )
            : base( initialCapacity, creator )
        {
        }

        /// <summary>
        /// Creates a new instance of the AuraPool{TAura} class that has the specified number of pre-allocated PoolNode{TAura}s.
        /// </summary>
        /// <param name="initialSize">
        /// The initial size of the new AuraPool{TAura}.
        /// </param>
        /// <param name="creator">
        /// The creation function of the new AuraPool.
        /// </param>
        /// <returns>
        /// The newly created AuraPool{TAura}.
        /// </returns>
        public static new AuraPool<TAura> Create( int initialSize, PooledObjectCreator<IPooledObjectWrapper<TAura>> creator )
        {
            var pool = new AuraPool<TAura>( initialSize, creator );
            pool.AddNodes( initialSize );

            return pool;
        }

        /// <summary>
        /// Called when the given PoolNode&lt;IPooledObjectWrapper&lt;TAura&gt;&gt; has been created
        /// for this Pool{T}.
        /// </summary>
        /// <param name="node">
        /// The PoolNode{T} that has just been created.
        /// This value is never null.
        /// </param>
        protected override void OnCreated( PoolNode<IPooledObjectWrapper<TAura>> node )
        {
            IPooledObjectWrapper<TAura> wrapper = node.Item;
            TAura aura = wrapper.PooledObject;

            wrapper.PoolNode = node;
            aura.Disabled += this.OnAuraDisabled;
        }

        /// <summary>
        /// Called when an Aura created by this AuraPool has been disabled.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="oldOwner">
        /// The Statable component of the Entity that has owned the Aura before it got disabled.
        /// </param>
        private void OnAuraDisabled( object sender, Statable oldOwner )
        {
            var auraNode = this.ActiveNodes.First( node => node.Item.PooledObject == sender );
            
            if( auraNode != null )
            {
                this.Return( auraNode );
            }
        }
    }
}
