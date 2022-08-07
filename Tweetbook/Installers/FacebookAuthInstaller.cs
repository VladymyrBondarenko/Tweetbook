using Tweetbook.Options;
using Tweetbook.Services;

namespace Tweetbook.Installers
{
    public class FacebookAuthInstaller : IInstaller
    {
        public void InstallServices(IConfiguration configuration, IServiceCollection services)
        {
            var facebookAuthSettings = new FacebookAuthSettingsOptions();
            configuration.GetSection(nameof(FacebookAuthSettingsOptions)).Bind(facebookAuthSettings);
            services.AddSingleton(facebookAuthSettings);

            var facebookGraphSettings = new FacebookGraphSettingsOptions();
            configuration.GetSection(nameof(FacebookGraphSettingsOptions)).Bind(facebookGraphSettings);
            services.AddSingleton(facebookGraphSettings);

            services.AddHttpClient("GraphFacebook");
            services.AddScoped<IFacebookAuthService, FacebookAuthService>();
        }
    }
}
