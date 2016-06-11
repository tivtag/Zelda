using System;
using System.Collections.Generic;

namespace Zelda.Status
{
    /// <summary>
    /// Implements a <see cref="System.ComponentModel.Design.CollectionEditor"/> for <see cref="List&lt;StatusEffect&gt;"/> objects.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class StatusEffectListEditor
        : System.ComponentModel.Design.CollectionEditor
    {
        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusEffectListEditor"/> class.
        /// </summary>
        public StatusEffectListEditor()
            : base( typeof( System.Collections.Generic.List<StatusEffect> ) )
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Receives the list of types this CollectionEditor can create.
        /// </summary>
        /// <returns>
        /// The list of types the CollectionEditor can create.
        /// </returns>
        protected override Type[] CreateNewItemTypes()
        {
            return types;
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// The list of types supported by the editor.
        /// </summary>
        private static readonly
            Type[] types = new Type[15] 
            {
                typeof( StatEffect           ),                
                typeof( ChanceToStatusEffect ),
                typeof( ArmorEffect          ),
                typeof( LifeManaEffect       ),  
                typeof( LifeManaRegenEffect  ),       
                typeof( AttackSpeedEffect    ),
                typeof( MovementSpeedEffect  ),
                typeof( PushingForceEffect   ),
                typeof( DamageTakenModEffect ),
                typeof( DamageDoneModEffect  ),
                typeof( DamageDoneRaceModEffect ),
                typeof( RangedPiercingEffect ),
                typeof( BlockValueEffect     ),
                typeof( MagicFindEffect      ),
                typeof( LifeManaPotionEffectivenessEffect )
            };

        #endregion
    }
}
