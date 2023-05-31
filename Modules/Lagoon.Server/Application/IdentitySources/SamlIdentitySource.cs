//TODEL
//using Lagoon.Core.Exceptions;
//using Lagoon.Server.Authentication.IdentitySources;
//using Microsoft.AspNetCore.Identity;
//using System;
//using System.Threading.Tasks;

//namespace Lagoon.Server.Application.IdentitySources
//{

//    /// <summary>
//    /// Describe a form authentication in the authentication workflow
//    /// </summary>
//    public class SamlIdentitySource : IdentitySource
//    {

//        /// <summary>
//        /// Gets the user password.
//        /// </summary>
//        /// <value>
//        /// The user password.
//        /// </value>
//        public string UserPassword { get; }

//        /// <summary>
//        /// Indicate if the password is mandatory
//        /// </summary>
//        /// <value>
//        /// True if required, false otherwise
//        /// </value>
//        public bool PasswordRequired { get; private set; } = true;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="FormsIdentitySource" /> class.
//        /// </summary>
//        /// <param name="login">User login</param>
//        /// <param name="password">User password</param>
//        /// <param name="rememberMe">IdentitySource remember me</param>
//        public SamlIdentitySource(string login, string password, bool rememberMe = false)
//            : base(AuthenticationMode.Forms, login, null, "Forms", rememberMe)
//        {
//            UserPassword = password;
//        }

//        /// <summary>
//        /// Checks the user access.
//        /// </summary>
//        /// <param name="user">The identity user.</param>
//        /// <param name="userManager">UserManager used to manipulate application user</param>
//        internal protected override async Task CheckUserAccessAsync<TUser>(TUser user, UserManager<TUser> userManager)
//        {
//            if (PasswordRequired && !await userManager.CheckPasswordAsync(user, UserPassword))
//            {
//                throw new UserException("InvalidLoginAttempt".Translate());
//            }
//        }

//        /// <summary>
//        /// User impersonation: User the identity supplied and don't check user password
//        /// </summary>
//        /// <param name="login">Identity to impersonate</param>
//        public override void Impersonate(string login)
//        {
//            base.Impersonate(login);
//            PasswordRequired = false;
//        }

//    }
//}
