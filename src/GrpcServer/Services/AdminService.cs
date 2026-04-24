using Grpc.Core;

namespace GrpcServer.Services;

public class AdminService(
    ILogger<AdminService> logger) : Admin.AdminBase
{
    public override Task<BuscarTokenReply> BuscarToken(BuscarTokenRequest request, ServerCallContext context)
    {
        logger.LogInformation("Gerando token para usuário: {Username}", request.Username);

        if (string.IsNullOrEmpty(request.Password))
        {
            throw new Exception("Testando erro");
        }

        var token = Guid.NewGuid().ToString();
        return Task.FromResult(new BuscarTokenReply
        {
            Token = token
        });
    }
}
