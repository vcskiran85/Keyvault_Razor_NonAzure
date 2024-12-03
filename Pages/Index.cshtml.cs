using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Keyvault_Razor_NonAzure.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SecretClient _secretClient;
        private readonly IConfiguration _configuration;

        public IndexModel(ILogger<IndexModel> logger, SecretClient secretClient, IConfiguration configuration)
        {
            _logger = logger;
            _secretClient = secretClient;
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            var secretName = _configuration["KeyVault:SecretName"];
            var secret = await _secretClient.GetSecretAsync(secretName);
            ViewData["SecretValue"] = secret.Value.Value;
        }
    }
}
