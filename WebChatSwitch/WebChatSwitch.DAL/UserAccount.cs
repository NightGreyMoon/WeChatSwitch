//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebChatSwitch.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserAccount
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserAccount()
        {
            this.Items = new HashSet<Item>();
        }
    
        public int Id { get; set; }
        public string OpenId { get; set; }
        public string Name { get; set; }
        public string WeChatNickName { get; set; }
        public string WeChatNumber { get; set; }
        public string Remark { get; set; }
        public short Balance { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Item> Items { get; set; }
    }
}
