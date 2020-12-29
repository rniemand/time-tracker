namespace TimeTracker.Core.Models.Responses
{
  public class AuthenticationResponse
  {
    public int UserId { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Token { get; set; }

    public AuthenticationResponse()
    {
      // TODO: [TESTS] (AuthenticationResponse) Add tests
      UserId = 0;
      Username = string.Empty;
      FirstName = string.Empty;
      LastName = string.Empty;
      Token = string.Empty;
    }
  }
}
