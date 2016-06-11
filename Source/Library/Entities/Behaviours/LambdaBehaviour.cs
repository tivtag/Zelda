namespace Zelda.Entities.Behaviours
{
    using System;
    using System.Diagnostics.Contracts;
    using Zelda.Saving.Storage;

    public sealed class LambdaBehaviour<TEntity> : IEntityBehaviour
        where TEntity : ZeldaEntity
    {
        public bool IsActive
        {
            get;
            private set;
        }

        public DataStore Data
        {
            get
            {
                return this.data;
            }
        }

        public TEntity Owner
        {
            get
            {
                return this.owner;
            }
        }

        public LambdaBehaviour( TEntity owner, Action<LambdaBehaviour<TEntity>, ZeldaUpdateContext> action )
        {
            Contract.Requires( owner != null );
            Contract.Requires( action != null );

            this.owner = owner;
            this.action = action;
        }

        public void Update( ZeldaUpdateContext updateContext )
        {
            action( this, updateContext );
        }

        public void Enter()
        {
            this.IsActive = true;
        }

        public void Leave()
        {
            this.IsActive = false;
        }

        public void Reset()
        {
        }

        public IEntityBehaviour Clone( ZeldaEntity newOwner )
        {
            throw new NotSupportedException();
        }
        
        public void Serialize( Saving.IZeldaSerializationContext context )
        {
            throw new NotImplementedException();
        }

        public void Deserialize( Saving.IZeldaDeserializationContext context )
        {
            throw new NotImplementedException();
        }

        private readonly Action<LambdaBehaviour<TEntity>, ZeldaUpdateContext> action;
        private readonly TEntity owner;
        private readonly DataStore data = new DataStore();
    }
}
