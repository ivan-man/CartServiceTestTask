using IdentityService.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4;

namespace IdentityService
{
    public class AppSettings
    {
        public readonly List<AllowedClientViewModel> AllowedClients = new List<AllowedClientViewModel>();
        public readonly List<ApiResourceViewModel> ApiResources = new List<ApiResourceViewModel>();

        public IConfiguration Configuration { get; }

        public AppSettings()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

            var configuration = builder.Build();

            configuration.GetSection("AllowedClients").Bind(AllowedClients);
            configuration.GetSection("ApiResources").Bind(ApiResources);
        }

        public IEnumerable<Client> GetIdentityClients() =>
            AllowedClients.Select(q =>
            new Client
            {
                ClientId = q.ClientId,
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                ClientSecrets =
                {
                    new Secret(q.Secret.Sha256())
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    IdentityServerConstants.StandardScopes.Address,
                    "x_scope"
                }
            });

        public IEnumerable<ApiResource> GetIdentityApiResources() =>
            ApiResources.Select(q =>
            new ApiResource
            {
                Name = q.Name,
                DisplayName = q.DisplayName,
            });

        public IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
                new IdentityResource()
                {
                    Name = "x_scope",
                    DisplayName = "x_scope",
                    Description = "A Custom Scope",
                    UserClaims = new[]
                    {
                        "x_scope"
                    }
                }
            };
        }
    }
}
