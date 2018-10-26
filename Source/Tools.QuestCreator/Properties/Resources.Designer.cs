﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Zelda.QuestCreator.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Zelda.QuestCreator.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cancel.
        /// </summary>
        public static string ButtonText_Cancel {
            get {
                return ResourceManager.GetString("ButtonText_Cancel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to OK.
        /// </summary>
        public static string ButtonText_OK {
            get {
                return ResourceManager.GetString("ButtonText_OK", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select the Quest Completion Event to add..
        /// </summary>
        public static string DialogTitle_CompletionEventSelection {
            get {
                return ResourceManager.GetString("DialogTitle_CompletionEventSelection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select the Quest Goal to add..
        /// </summary>
        public static string DialogTitle_GoalSelection {
            get {
                return ResourceManager.GetString("DialogTitle_GoalSelection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select the Quest to open..
        /// </summary>
        public static string DialogTitle_OpenQuest {
            get {
                return ResourceManager.GetString("DialogTitle_OpenQuest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select the Quest Requirement to add..
        /// </summary>
        public static string DialogTitle_RequirementSelection {
            get {
                return ResourceManager.GetString("DialogTitle_RequirementSelection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select the Quest Reward to add..
        /// </summary>
        public static string DialogTitle_RewardSelection {
            get {
                return ResourceManager.GetString("DialogTitle_RewardSelection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The name of the Quest must be set before it can be saved..
        /// </summary>
        public static string Error_QuestNameMustBeSetForSave {
            get {
                return ResourceManager.GetString("Error_QuestNameMustBeSetForSave", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Quest Data Files (*.zq)|*.zq.
        /// </summary>
        public static string Filter_QuestFiles {
            get {
                return ResourceManager.GetString("Filter_QuestFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Quest &apos;{0}&apos; has been saved successfully..
        /// </summary>
        public static string Info_QuestXSavedSuccessfully {
            get {
                return ResourceManager.GetString("Info_QuestXSavedSuccessfully", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Quest Creator for TLoZ - Black Crown.
        /// </summary>
        public static string Title {
            get {
                return ResourceManager.GetString("Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The resource id that is used to receive the localized text
        ///that is shown when the quest has been completed.
        ///Should be in the format &apos;QC_X&apos;..
        /// </summary>
        public static string Tooltip_CompletedTextId {
            get {
                return ResourceManager.GetString("Tooltip_CompletedTextId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The location the quest must be delivered at.
        ///Interpreted differently regarding the set Deliver Type.
        ///
        ///        Instant: This field can be left empty.
        ///        NPC:     This field must contain the name that uniquely identifies the NPC.
        ///.
        /// </summary>
        public static string Tooltip_DeliverLocation {
            get {
                return ResourceManager.GetString("Tooltip_DeliverLocation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to States how the player ends the quest..
        /// </summary>
        public static string Tooltip_DeliverType {
            get {
                return ResourceManager.GetString("Tooltip_DeliverType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The resource id that is used to receive the localized description of the quest.
        ///Should be in the format &apos;QD_X&apos;..
        /// </summary>
        public static string Tooltip_DescriptionId {
            get {
                return ResourceManager.GetString("Tooltip_DescriptionId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to States whether the quest can be done over and over again.
        ///After completing a repeatable quest it becomes available to be picked-up again..
        /// </summary>
        public static string Tooltip_IsRepeatable {
            get {
                return ResourceManager.GetString("Tooltip_IsRepeatable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to States whether the quest&apos;s current state is hidden from the player..
        /// </summary>
        public static string Tooltip_IsStateHidden {
            get {
                return ResourceManager.GetString("Tooltip_IsStateHidden", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The level of the quest. Quest difficulty can be induces by
        ///looking at the level of the quest compared to the level of the player..
        /// </summary>
        public static string Tooltip_Level {
            get {
                return ResourceManager.GetString("Tooltip_Level", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The name that uniquely identifies the quest.
        ///Localization is done by looking up the resource string similiar to &apos;QN_X&apos;,
        ///where X is the name of the quest..
        /// </summary>
        public static string Tooltip_Name {
            get {
                return ResourceManager.GetString("Tooltip_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The resource id that is used to receive the localized text
        ///that is shown when the quest has not been completed yet.
        ///Should be in the format &apos;QNC_X&apos;..
        /// </summary>
        public static string Tooltip_NotCompletedTextId {
            get {
                return ResourceManager.GetString("Tooltip_NotCompletedTextId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The resource id that is used to receive the localized start text of the quest.
        ///Should be in the format &apos;QS_X&apos;..
        /// </summary>
        public static string Tooltip_StartTextId {
            get {
                return ResourceManager.GetString("Tooltip_StartTextId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type of the quest..
        /// </summary>
        public static string Tooltip_Type {
            get {
                return ResourceManager.GetString("Tooltip_Type", resourceCulture);
            }
        }
    }
}
