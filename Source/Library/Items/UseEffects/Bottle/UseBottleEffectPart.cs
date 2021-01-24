// <copyright file="UseBottleEffectPart.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines Zelda.Items.UseEffects.UseBottleEffectPart class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.Items.UseEffects
{
    using System.ComponentModel;
    using Atom.Components;
    using Zelda.Core.Predicates;
    using Zelda.Entities;
    using Zelda.Saving;

    /// <summary>
    /// Encapsulates a possibility that is checked when using the <see cref="UseBottleEffect"/>.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public sealed class UseBottleEffectPart : ISaveable, IZeldaSetupable
    {
        /// <summary>
        /// Gets or sets the name of the item that is spawned
        /// when this UseBottleEffectPart takes effect.
        /// </summary>
        [Editor( typeof( Zelda.Items.Design.ItemNameEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public string ItemName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the audio resource that is played
        /// when this UseBottleEffectPart has been triggered.
        /// </summary>
        [Editor( "Zelda.Audio.Design.SoundFileNameEditor, Library.Design", typeof( System.Drawing.Design.UITypeEditor ) )]
        public string SampleName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the IPredicate{IEntity} that must hold true
        /// for the UseBottleEffectPart to take effect. 
        /// </summary>
        [Editor( typeof( Zelda.Core.Predicates.Design.PredicateEditor<IEntity> ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public IPredicate<IEntity> Predicate
        {
            get;
            set;
        }
        
        /// <summary>
        /// Tries to trigger this UseBottleEffectPart.
        /// </summary>
        /// <param name="user">
        /// The PlayerEntity that wishes to trigger this UseBottleEffectPart.
        /// </param>
        /// <returns>
        /// true if this UseBottleEffectPart was triggered;
        /// -or- otherwise false.
        /// </returns>
        public bool Use( PlayerEntity user )
        {
            if( this.Predicate == null || this.Predicate.Holds( user ) )
            {
                this.SpawnItem( user );
                this.PlayAudioSample();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Spawns the item with the set ItemName.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that has triggered this UseBottleEffectPart.
        /// </param>
        private void SpawnItem( PlayerEntity player )
        {
            if( string.IsNullOrEmpty( this.ItemName ) )
                return;

            var item = this.serviceProvider.ItemManager.Get( this.ItemName );
            if( item == null )
                return;

            var itemInstance = item.CreateInstance();

            if( player.PickedupItemContainer.IsEmpty )
            {
                player.PickedupItemContainer.Pick( itemInstance );
            }
            else
            {
                player.Inventory.FailSafeInsert( itemInstance );
            }
        }

        /// <summary>
        /// Plays the audio sample that has been associated with this UseBottleEffectPart.
        /// </summary>
        private void PlayAudioSample()
        {
            if( string.IsNullOrEmpty( this.SampleName ) )
                return;

            var sample = this.serviceProvider.AudioSystem.GetSample( this.SampleName );
            if( sample == null )
                return;

            sample.LoadAsSample();
            sample.Play();
        }

        /// <summary>
        /// Setups this UseBottleEffectPart.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public void Setup( IZeldaServiceProvider serviceProvider )
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Serializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Serialize( IZeldaSerializationContext context )
        {
            context.WriteDefaultHeader();

            context.Write( this.ItemName ?? string.Empty );
            context.Write( this.SampleName ?? string.Empty );
            context.WriteObject( this.Predicate );
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        public void Deserialize( IZeldaDeserializationContext context )
        {
            context.ReadDefaultHeader( this.GetType() );

            this.ItemName = context.ReadString();
            this.SampleName = context.ReadString();
            this.Predicate = context.ReadObject<IPredicate<IEntity>>();
        }

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private IZeldaServiceProvider serviceProvider;
    }
}
