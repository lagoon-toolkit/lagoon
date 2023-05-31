//TODEL
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Text;

//namespace Lagoon.Model.Models
//{
//    public class UserIdReferencer<TUser, TKey> : ILgUserIdReferencer
//        where TKey : IEquatable<TKey>
//    {

//        public string UserIdAsString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
//        public Guid UserIdAsGuid { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


//        /// <summary>
//        /// User id
//        /// </summary>
//        [ForeignKey(nameof(User))]
//        public TKey UserId { get; set; }

//        /// <summary>
//        /// User object
//        /// </summary>
//        public virtual TUser User { get; set; }
//    }

//}
