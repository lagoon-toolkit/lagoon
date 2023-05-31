namespace TemplateLagoonWeb.Model.Models;

/// <summary>
/// Application user
/// </summary>
public class ApplicationUser : LgIdentityUserGuid
{

    /* #UserNameAsId: Uncomment this region if the "UserName" property isn't used to identify a userbut the "Email" property is used.

    #region We don't use the "UserName" property to identify a user.

    /// <summary>
    /// Gets or sets the user name for this user.
    /// </summary>
    /// <remarks>This override keep the field value to null in database without validation error.</remarks>
    public override string UserName { get => Email; set => Email = value; }

    /// <summary>
    /// Gets or sets the normalized user name for this user.
    /// </summary>
    /// <remarks>This override keep the field value to null in database without validation error.</remarks>
    public override string NormalizedUserName { get => NormalizedEmail; set => NormalizedEmail = value; }

    #endregion

    */

    /// <summary>
    /// First name of the user.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }

    /// <summary>
    /// Last name of the user.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }

    /* Example of a 1:N relation

    /// <summary>
    /// User's foo identifier.
    /// </summary>
    [Required]
    public Guid FooId { get; set; }

    /// <summary>
    /// User's foo.
    /// </summary>
    public virtual Foo Foo { get; set; }

    */

}