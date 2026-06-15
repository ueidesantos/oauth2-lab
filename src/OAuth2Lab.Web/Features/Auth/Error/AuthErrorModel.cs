using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OAuth2Lab.Web.Features.Auth.Error;

public class AuthErrorModel : PageModel
{
    public string ErrorMessage { get; private set; } = "Erro desconhecido.";
    public string? ErrorCode { get; private set; }
    public string? ErrorDescription { get; private set; }

    public void OnGet(string? message, string? error, string? error_description)
    {
        ErrorMessage = message ?? error ?? "Falha na autenticação.";
        ErrorCode = error;
        ErrorDescription = error_description;
    }
}
