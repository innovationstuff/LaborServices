﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LaborServices.ViewModel.Resources.WestWindResources {
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
    public class LeadResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal LeadResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LaborServices.ViewModel.Resources.WestWindResources.LeadResources", typeof(LeadResources).Assembly);
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
        ///   Looks up a localized string similar to تقديم طلب قطاع أفراد.
        /// </summary>
        public static string IndividualLeadRequestTitle {
            get {
                return ResourceManager.GetString("IndividualLeadRequestTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to اختر الجنسية المطلوبة.
        /// </summary>
        public static string RequiredNationality_PH {
            get {
                return ResourceManager.GetString("RequiredNationality_PH", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to اختر المهنة المطلوبة.
        /// </summary>
        public static string RequiredProfession_PH {
            get {
                return ResourceManager.GetString("RequiredProfession_PH", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to تم تقديم الطلب بنجاح.
        /// </summary>
        public static string SuccessLeadRequest {
            get {
                return ResourceManager.GetString("SuccessLeadRequest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to لقد تم انشاء الطلب المقدم منكم بنجاح ، وسيتم التواصل معكم في أقرب وقت ممكن.
        /// </summary>
        public static string SuccessLeadRequestMsg {
            get {
                return ResourceManager.GetString("SuccessLeadRequestMsg", resourceCulture);
            }
        }
    }
}