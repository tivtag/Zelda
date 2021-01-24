// <copyright file="QuickActionSlotList.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Zelda.QuickActions.QuickActionSlotList class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Zelda.QuickActions
{
    using System;
    using System.Linq;
    using Atom;
    using Zelda.Entities;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Represents a list of <see cref="QuickActionSlot"/>s which can contain <see cref="IQuickAction"/>s.
    /// </summary>
    public sealed class QuickActionSlotList : IZeldaUpdateable
    {
        /// <summary>
        /// The number of <see cref="QuickActionSlot"/>s a <see cref="QuickActionSlotList"/> has. 
        /// </summary>
        public const int Size = 28;

        /// <summary>
        /// Initializes a new instance of the QuickActionSlotList class.
        /// </summary>
        /// <param name="player">
        /// The PlayerEntity that owns the new QuickActionSlotList.
        /// </param>
        /// <param name="keySettings">
        /// The settings used to trigger the action action slots.
        /// </param>
        public QuickActionSlotList( PlayerEntity player, KeySettings keySettings )
        {
            this.player = player;

            this.CreateSlots( keySettings );
            this.HookEvents();
        }

        /// <summary>
        /// 
        /// </summary>
        private void HookEvents()
        {
            this.player.Skills.Learned += ( sender, skill ) => this.AddSkill( skill );
            this.player.Skills.Unlearned += ( sender, skill ) => this.RemoveSkill( skill );
        }

        /// <summary>
        /// Gets the QuickActionSlot at the given zero-based index.
        /// </summary>
        /// <param name="index">
        /// The zero-based input index.
        /// </param>
        /// <returns>
        /// The requested QuickActionSlot.
        /// </returns>
        public QuickActionSlot GetSlotAt( int index )
        {
            return this.slots[index];
        }

        public bool Execute( Keys key, bool secondRow )
        {
            if( secondRow )
            {
                for( int i = Size / 2; i < Size; ++i )
                {
                    QuickActionSlot slot = slots[i];

                    if( slot.Key == key && slot.Action != null )
                    {
                        slot.Action.Execute( player );
                        return true;
                    }
                }
            }
            else
            {
                for( int i = 0; i < Size / 2; ++i )
                {
                    QuickActionSlot slot = slots[i];

                    if( slot.Key == key && slot.Action != null )
                    {
                        slot.Action.Execute( player );
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets an un-used <see cref="QuickActionSlot"/>.
        /// </summary>
        /// <returns>
        /// An un-used QuickActionSlot; or null.
        /// </returns>
        public QuickActionSlot GetFreeSlot()
        {
            foreach( var slot in this.slots )
            {
                if( slot.Action == null )
                {
                    return slot;
                }
            }

            return null;
        }

        /// <summary>
        /// Associates the given Skill with the next free QuickActionSlot.
        /// </summary>
        /// <param name="skill">
        /// The Skill to add.
        /// </param>
        internal void AddSkill( Zelda.Skills.Skill skill )
        {
            var slot = this.GetFreeSlot();

            if( slot != null )
            {
                slot.Action = new UseSkillAction( skill );
            }
            else
            {
                if( TryRemoveAnyUseItemAction() )
                {
                    this.AddSkill( skill );
                }
            }
        }

        /// <summary>
        /// Tries to remove an UseItemAction form this QuickActionSlotList.
        /// </summary>
        /// <returns>
        /// Whether an UseItemAction has been removed.
        /// </returns>
        private bool TryRemoveAnyUseItemAction()
        {
            foreach( var slot in this.slots )
            {
                if( slot.Action is UseItemAction )
                {
                    slot.Action = null;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to temove the given Skill from this QuickActionSlotList.
        /// </summary>
        /// <param name="skill">
        /// The Skill to remove.
        /// </param>
        /// <returns>
        /// Returns whether the Skill has been removed.
        /// </returns>
        internal bool RemoveSkill( Zelda.Skills.Skill skill )
        {
            foreach( var slot in this.slots )
            {
                var skillAction = slot.Action as UseSkillAction;

                if( skillAction != null )
                {
                    if( skillAction.Skill == skill )
                    {
                        slot.Action = null;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Creates the individual QuickActionSlots.
        /// </summary>
        /// <param name="keySettings">
        /// The settings used to trigger the action action slots.
        /// </param>
        private void CreateSlots( KeySettings keySettings )
        {
            int half = Size / 2;

            for( int index = 0; index < this.slots.Length; ++index )
            {
                bool topRow = index >= half;
                int actionIndex = topRow ? index - half : index;
                Keys key = keySettings.GetActionAt( actionIndex );

                this.slots[index] = new QuickActionSlot( key, topRow );
            }
        }

        /// <summary>
        /// Updates this QuickActionSlotList.
        /// </summary>
        /// <param name="updateContext">
        /// The current ZeldaUpdateContext.
        /// </param>
        public void Update( ZeldaUpdateContext updateContext )
        {
            for( int i = 0; i < this.slots.Length; ++i )
            {
                var action = this.slots[i].Action;

                if( action != null )
                {
                    action.Update( updateContext );
                }
            }
        }

        #region > Storage <

        /// <summary>
        /// Serializes/Writes the data of this <see cref="QuickActionSlotList"/>
        /// using the given System.IO.BinaryWriter.
        /// </summary>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        internal void Serialize( Zelda.Saving.IZeldaSerializationContext context )
        {
            const int Version = 2;
            context.Write( Version );
            context.Write( slots.Length );

            for( int i = 0; i < slots.Length; ++i )
            {
                QuickActionSlot slot = this.slots[i];
                SerializeAction( slot.Action, context );
            }
        }

        /// <summary>
        /// Serializes the given IQuickAction.
        /// </summary>
        /// <param name="action">
        /// The action to serialize.
        /// </param>
        /// <param name="context">
        /// The context under which the serialization process takes place.
        /// Provides access to required objects.
        /// </param>
        private static void SerializeAction( IQuickAction action, Zelda.Saving.IZeldaSerializationContext context )
        {
            if( action != null )
            {
                context.Write( true );

                context.Write( action.GetType().GetTypeName() );
                action.Serialize( context );
            }
            else
            {
                context.Write( false );
            }
        }

        /// <summary>
        /// Deserializes the data required to descripe this ISaveable.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        internal void Deserialize( Zelda.Saving.IZeldaDeserializationContext context )
        {
            const int Version = 2;
            int version = context.ReadInt32();
            Atom.ThrowHelper.InvalidVersion( version, 1, Version, this.GetType() );

            int actionCount = context.ReadInt32();

            if( version == 1 )
            {
                for( int i = 0; i < actionCount; ++i )
                {
                    string skillTypeName = context.ReadString();
                    if( skillTypeName.Length == 0 )
                        continue;

                    Type type = Type.GetType( skillTypeName );
                    var skill = this.player.Skills.Get( type );

                    if( !this.HasSkill( skill ) )
                    {
                        this.slots[i].Action = new UseSkillAction( skill );
                    }
                }
            }
            else
            {
                for( int i = 0; i < actionCount; ++i )
                {
                    bool hasAction = context.ReadBoolean();

                    IQuickAction action = null;

                    if( hasAction )
                    {
                        action = DeserializeAction( context );
                    }

                    this.slots[i].Action = action;
                }
            }

            this.SlotMissingSkills();
        }

        /// <summary>
        /// Deserializes an IQuickAction.
        /// </summary>
        /// <param name="context">
        /// The context under which the deserialization process takes place.
        /// Provides access to required objects.
        /// </param>
        /// <returns>
        /// The newly deserialized IQuickAction.
        /// </returns>
        private IQuickAction DeserializeAction( Zelda.Saving.IZeldaDeserializationContext context )
        {
            string typeName = context.ReadString();
            Type type = Type.GetType( typeName );

            var action = (IQuickAction)Activator.CreateInstance( type );
            action.Deserialize( this.player, context );

            return action;
        }

        /// <summary>
        /// Adds the skills of the player that are not yet slotted to this QuickActionSlotList.
        /// </summary>
        private void SlotMissingSkills()
        {
            foreach( var skill in player.Skills )
            {
                if( !this.HasSkill( skill ) )
                {
                    QuickActionSlot slot = this.GetFreeSlot();

                    if( slot != null )
                    {
                        slot.Action = new UseSkillAction( skill );
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this QuickActionSlotList has the given Skill
        /// in one of its QuickActionSlots.
        /// </summary>
        /// <param name="skill">
        /// The Skill to search for.
        /// </param>
        /// <returns>
        /// Whether the given Skill is present in this QuickActionSlotList.
        /// </returns>
        private bool HasSkill( Zelda.Skills.Skill skill )
        {
            return this.slots.Any(
                ( x ) => {
                    var skillAction = x.Action as UseSkillAction;

                    if( skillAction != null )
                    {
                        return skillAction.Skill == skill;
                    }
                    else
                    {
                        return false;
                    }
                }
            );
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The various available slots.
        /// </summary>
        private readonly QuickActionSlot[] slots = new QuickActionSlot[Size];

        /// <summary>
        /// Idenfities the PlayerEntity whose actions can be controlled by this QuickActionSlotList.
        /// </summary>
        private readonly Zelda.Entities.PlayerEntity player;

        #endregion
    }
}
