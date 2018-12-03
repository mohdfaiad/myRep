//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace myRep_app
{
    using System;
    using System.Collections.Generic;
    
    public partial class HCP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HCP()
        {
            this.HCO = new HashSet<HCO>();
            this.MeetingSet1 = new HashSet<Meeting>();
        }
    
        public int hcpID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string AcademicTitle { get; set; }
        public string Specialty { get; set; }
        public Nullable<System.DateTime> Birthdate { get; set; }
        public Nullable<int> PhoneNumber { get; set; }
        public string Email { get; set; }
        public Nullable<bool> KOL { get; set; }
        public string LanguageSpoken { get; set; }
        public int AddressID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HCO> HCO { get; set; }
        public virtual Address Address { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Meeting> MeetingSet1 { get; set; }
    }
}
