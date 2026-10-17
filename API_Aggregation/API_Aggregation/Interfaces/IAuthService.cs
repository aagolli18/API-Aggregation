namespace API_Aggregation.Interfaces
{
    public interface IAuthService
    {
        string GenerateToken(string username);
    }
}
