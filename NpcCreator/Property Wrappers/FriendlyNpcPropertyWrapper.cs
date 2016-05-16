// <copyright file="FriendlyNpcPropertyWrapper.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Zelda.NpcCreator.FriendlyNpcPropertyWrapper class.</summary>
// <author>Paul Ennemoser (Tick)</author>

namespace Zelda.NpcCreator
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Atom.Math;
    using Zelda.Entities;
    using Zelda.Factions;

    /// <summary>
    /// Defines the IObjectPropertyWrapper for <see cref="FriendlyNpc"/> entities.
    /// </summary>
    internal sealed class FriendlyNpcPropertyWrapper : NpcPropertyWrapper<FriendlyNpc>
    {
        #region > Settings <

        [DefaultValue( null )]
        [LocalizedDisplayName( "PropDisp_Faction" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescription( "PropDesc_Faction" )]
        [Editor( typeof( Design.FactionSelectionEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Faction Faction
        {
            get
            {
                return this.WrappedObject.Faction;
            }
            set
            {
                this.WrappedObject.Faction = value;
            }
        }

        [DefaultValue( null )]
        [LocalizedDisplayName( "PropDisp_EntityBehaviour" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescription( "PropDesc_EntityBehaviour" )]
        [Editor( typeof( Design.EntityBehaviourSelectionEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Type EntityBehaviourType
        {
            get
            {
                var behaviour = this.WrappedObject.Behaveable.Behaviour;
                return behaviour != null ? behaviour.GetType() : null;
            }
            set
            {
                if( value != null )
                {
                    var behaviour = serviceProvider.BehaviourManager.GetBehaviourClone( value, this.WrappedObject );
                    this.WrappedObject.Behaveable.Behaviour = behaviour;
                }
                else
                {
                    this.WrappedObject.Behaveable.Behaviour = null;
                }
            }
        }

        [LocalizedDisplayName( "PropDisp_EntityBehaviourSettings" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescription( "PropDesc_EntityBehaviourSettings" )]
        public Zelda.Entities.Behaviours.IEntityBehaviour EntityBehaviourSettings
        {
            get
            {
                return this.WrappedObject.Behaveable.Behaviour;
            }
        }

        [DefaultValue( null )]
        [LocalizedDisplayName( "PropDisp_DrawDataAndStrategyType" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescription( "PropDesc_DrawDataAndStrategyType" )]
        [Editor( typeof( Design.DrawDataAndStrategySelectionEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
        public Type DrawDataAndStrategyType
        {
            get
            {
                var dds = this.WrappedObject.DrawDataAndStrategy;
                return dds != null ? dds.GetType() : null;
            }
            set
            {
                if( value != null )
                {
                    var dds = serviceProvider.DrawStrategyManager.GetStrategyClone( value, this.WrappedObject );
                    this.WrappedObject.DrawDataAndStrategy = dds;
                }
                else
                {
                    this.WrappedObject.DrawDataAndStrategy = null;
                }
            }
        }

        [LocalizedDisplayName( "PropDisp_DrawDataAndStrategySettings" )]
        [LocalizedCategory( "PropCate_Settings" )]
        [LocalizedDescription( "PropDesc_DrawDataAndStrategySettings" )]
        public Zelda.Entities.Drawing.IDrawDataAndStrategy DrawDataAndStrategySettings
        {
            get
            {
                return this.WrappedObject.DrawDataAndStrategy;
            }
        }

        #endregion

        #region > Visionable <

        [LocalizedDisplayName( "PropDisp_VisionRange" )]
        [LocalizedCategory( "PropCate_Vision" )]
        [LocalizedDescription( "PropDesc_VisionRange" )]
        public int VisionRange
        {
            get { return this.WrappedObject.Visionable.VisionRange; }
            set
            {
                this.WrappedObject.Visionable.VisionRange = value;
            }
        }

        [LocalizedDisplayName( "PropDisp_FeelingRange" )]
        [LocalizedCategory( "PropCate_Vision" )]
        [LocalizedDescription( "PropDesc_FeelingRange" )]
        public int FeelingRange
        {
            get { return this.WrappedObject.Visionable.FeelingRange; }
            set
            {
                this.WrappedObject.Visionable.FeelingRange = value;
            }
        }

        #endregion

        #region > Talkable <

        [LocalizedDisplayName( "PropDisp_HatedTextToggle" )]
        [LocalizedCategory( "PropCate_Talkable" )]
        [LocalizedDescription( "PropDesc_HatedTextToggle" )]
        public bool HatedTextToggle
        {
            get { return this.WrappedObject.Talkable.GetTextList( ReputationLevel.Hated ) != null; }
            set
            {
                if( value == this.HatedTextToggle )
                    return;

                if( value )
                {
                    this.WrappedObject.Talkable.SetTextList( ReputationLevel.Hated, new List<LocalizableText>() );
                }
                else
                {
                    this.WrappedObject.Talkable.SetTextList( ReputationLevel.Hated, null );
                }
            }
        }

        [LocalizedDisplayName( "PropDisp_HatedText" )]
        [LocalizedCategory( "PropCate_Talkable" )]
        [LocalizedDescription( "PropDesc_HatedText" )]
        public List<LocalizableText> HatedText
        {
            get { return this.WrappedObject.Talkable.GetTextList( ReputationLevel.Hated ); }
        }

        [LocalizedDisplayName( "PropDisp_HostileTextToggle" )]
        [LocalizedCategory( "PropCate_Talkable" )]
        [LocalizedDescription( "PropDesc_HostileTextToggle" )]
        public bool HostileTextToggle
        {
            get { return this.WrappedObject.Talkable.GetTextList( ReputationLevel.Hostile ) != null; }
            set
            {
                if( value == this.HostileTextToggle )
                    return;

                if( value )
                {
                    this.WrappedObject.Talkable.SetTextList( ReputationLevel.Hostile, new List<LocalizableText>() );
                }
                else
                {
                    this.WrappedObject.Talkable.SetTextList( ReputationLevel.Hostile, null );
                }
            }
        }

        [LocalizedDisplayName( "PropDisp_HostileText" )]
        [LocalizedCategory( "PropCate_Talkable" )]
        [LocalizedDescription( "PropDesc_HostileText" )]
        public List<LocalizableText> HostileText
        {
            get { return this.WrappedObject.Talkable.GetTextList( ReputationLevel.Hostile ); }
        }


        [LocalizedDisplayName( "PropDisp_HonoredText" )]
        [LocalizedCategory( "PropCate_Talkable" )]
        [LocalizedDescription( "PropDesc_HonoredText" )]
        public List<LocalizableText> HonoredText
        {
            get { return this.WrappedObject.Talkable.GetTextList( ReputationLevel.Honored ); }
        }

        [LocalizedDisplayName( "PropDisp_HonoredTextToggle" )]
        [LocalizedCategory( "PropCate_Talkable" )]
        [LocalizedDescription( "PropDesc_HonoredTextToggle" )]
        public bool HonoredTextToggle
        {
            get { return this.WrappedObject.Talkable.GetTextList( ReputationLevel.Honored ) != null; }
            set
            {
                if( value == this.HonoredTextToggle )
                    return;

                if( value )
                {
                    this.WrappedObject.Talkable.SetTextList( ReputationLevel.Honored, new List<LocalizableText>() );
                }
                else
                {
                    this.WrappedObject.Talkable.SetTextList( ReputationLevel.Honored, null );
                }
            }
        }

        [LocalizedDisplayName( "PropDisp_UnfriendlyTextToggle" )]
        [LocalizedCategory( "PropCate_Talkable" )]
        [LocalizedDescription( "PropDesc_UnfriendlyTextToggle" )]
        public bool UnfriendlyTextToggle
        {
            get { return this.WrappedObject.Talkable.GetTextList( ReputationLevel.Unfriendly ) != null; }
            set
            {
                if( value == this.UnfriendlyTextToggle )
                    return;

                if( value )
                {
                    this.WrappedObject.Talkable.SetTextList( ReputationLevel.Unfriendly, new List<LocalizableText>() );
                }
                else
                {
                    this.WrappedObject.Talkable.SetTextList( ReputationLevel.Unfriendly, null );
                }
            }
        }

        [LocalizedDisplayName( "PropDisp_UnfriendlyText" )]
        [LocalizedCategory( "PropCate_Talkable" )]
        [LocalizedDescription( "PropDesc_UnfriendlyText" )]
        public List<LocalizableText> UnfriendlyText
        {
            get { return this.WrappedObject.Talkable.GetTextList( ReputationLevel.Unfriendly ); }
        }

        [LocalizedDisplayName( "PropDisp_NeutralTextToggle" )]
        [LocalizedCategory( "PropCate_Talkable" )]
        [LocalizedDescription( "PropDesc_NeutralTextToggle" )]
        public bool NeutralTextToggle
        {
            get { return this.WrappedObject.Talkable.GetTextList( ReputationLevel.Neutral ) != null; }
            set
            {
                if( value == this.NeutralTextToggle )
                    return;

                if( value )
                {
                    this.WrappedObject.Talkable.SetTextList( ReputationLevel.Neutral, new List<LocalizableText>() );
                }
                else
                {
                    this.WrappedObject.Talkable.SetTextList( ReputationLevel.Neutral, null );
                }
            }
        }

        [LocalizedDisplayName( "PropDisp_NeutralText" )]
        [LocalizedCategory( "PropCate_Talkable" )]
        [LocalizedDescription( "PropDesc_NeutralText" )]
        public List<LocalizableText> NeutralText
        {
            get { return this.WrappedObject.Talkable.GetTextList( ReputationLevel.Neutral ); }
        }
        
        [LocalizedDisplayName( "PropDisp_FriendlyTextToggle" )]
        [LocalizedCategory( "PropCate_Talkable" )]
        [LocalizedDescription( "PropDesc_FriendlyTextToggle" )]
        public bool FriendlyTextToggle
        {
            get { return this.WrappedObject.Talkable.GetTextList( ReputationLevel.Friendly ) != null; }
            set
            {
                if( value == this.FriendlyTextToggle )
                    return;

                if( value )
                {
                    this.WrappedObject.Talkable.SetTextList( ReputationLevel.Friendly, new List<LocalizableText>() );
                }
                else
                {
                    this.WrappedObject.Talkable.SetTextList( ReputationLevel.Friendly, null );
                }
            }
        }

        [LocalizedDisplayName( "PropDisp_FriendlyText" )]
        [LocalizedCategory( "PropCate_Talkable" )]
        [LocalizedDescription( "PropDesc_FriendlyText" )]
        public List<LocalizableText> FriendlyText
        {
            get { return this.WrappedObject.Talkable.GetTextList( ReputationLevel.Friendly ); }
        }
        
        [LocalizedDisplayName( "PropDisp_ReveredTextToggle" )]
        [LocalizedCategory( "PropCate_Talkable" )]
        [LocalizedDescription( "PropDesc_ReveredTextToggle" )]
        public bool ReveredTextToggle
        {
            get { return this.WrappedObject.Talkable.GetTextList( ReputationLevel.Revered ) != null; }
            set
            {
                if( value == this.ReveredTextToggle )
                    return;

                if( value )
                {
                    this.WrappedObject.Talkable.SetTextList( ReputationLevel.Revered, new List<LocalizableText>() );
                }
                else
                {
                    this.WrappedObject.Talkable.SetTextList( ReputationLevel.Revered, null );
                }
            }
        }

        [LocalizedDisplayName( "PropDisp_ReveredText" )]
        [LocalizedCategory( "PropCate_Talkable" )]
        [LocalizedDescription( "PropDesc_ReveredText" )]
        public List<LocalizableText> ReveredText
        {
            get { return this.WrappedObject.Talkable.GetTextList( ReputationLevel.Revered ); }
        }

        [LocalizedDisplayName( "PropDisp_ExaltedTextToggle" )]
        [LocalizedCategory( "PropCate_Talkable" )]
        [LocalizedDescription( "PropDesc_ExaltedTextToggle" )]
        public bool ExaltedTextToggle
        {
            get { return this.WrappedObject.Talkable.GetTextList( ReputationLevel.Exalted ) != null; }
            set
            {
                if( value == this.ExaltedTextToggle )
                    return;

                if( value )
                {
                    this.WrappedObject.Talkable.SetTextList( ReputationLevel.Exalted, new List<LocalizableText>() );
                }
                else
                {
                    this.WrappedObject.Talkable.SetTextList( ReputationLevel.Exalted, null );
                }
            }
        }

        [LocalizedDisplayName( "PropDisp_ExaltedText" )]
        [LocalizedCategory( "PropCate_Talkable" )]
        [LocalizedDescription( "PropDesc_ExaltedText" )]
        public List<LocalizableText> ExaltedText
        {
            get { return this.WrappedObject.Talkable.GetTextList( ReputationLevel.Exalted ); }
        }

        #endregion

        #region > Quests <

        [LocalizedDisplayName( "PropDisp_QuestNames" )]
        [LocalizedCategory( "PropCate_Quests" )]
        [LocalizedDescription( "PropDesc_QuestNames" )]
        [Editor( "System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", 
                 typeof( System.Drawing.Design.UITypeEditor ) )]
        public List<string> QuestNames
        {
            get { return this.WrappedObject.QuestsGiveable.QuestNames; }
        }

        #endregion

        #region > Collision <

        [LocalizedDisplayName( "PropDisp_CollisionSize" )]
        [LocalizedCategory( "PropCate_Collision" )]
        [LocalizedDescription( "PropDesc_CollisionSize" )]
        public Vector2 CollisionSize
        {
            get { return this.WrappedObject.Collision.Size; }
            set { this.WrappedObject.Collision.Size = value; }
        }

        [LocalizedDisplayName( "PropDisp_CollisionOffset" )]
        [LocalizedCategory( "PropCate_Collision" )]
        [LocalizedDescription( "PropDesc_CollisionOffset" )]
        public Vector2 CollisionOffset
        {
            get { return this.WrappedObject.Collision.Offset; }
            set { this.WrappedObject.Collision.Offset = value; }
        }

        #endregion
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FriendlyNpcPropertyWrapper"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// Provides fast access to game-related services.
        /// </param>
        public FriendlyNpcPropertyWrapper( IZeldaServiceProvider serviceProvider )
        {
            if( serviceProvider == null )
                throw new ArgumentNullException( "serviceProvider" );

            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Ensures that the state of the FriendlyNpc is valid.
        /// </summary>
        /// <returns></returns>
        public override bool Ensure()
        {
            return true;
        }

        /// <summary>
        /// Returns a clone of this FriendlyNpcPropertyWrapper.
        /// </summary>
        /// <returns>The cloned FriendlyNpcPropertyWrapper.</returns>
        public override Atom.Design.IObjectPropertyWrapper Clone()
        {
            return new FriendlyNpcPropertyWrapper( serviceProvider );
        }

        /// <summary>
        /// Provides fast access to game-related services.
        /// </summary>
        private readonly IZeldaServiceProvider serviceProvider;
    }
}


// That's one of the lead developers at valve playing WoW. 