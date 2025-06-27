using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EveMarket.Network.OAuth
{
    public static class LoginConfig
    {
        // Login/Authurization
        private const string logFile = "./esi_auth.log";
        private const string clientId = "c1b9bcd8e791468a9ad4cd8bade13545";
        private const string clients_secret = "opIC0Mh4CCmMlIhHCIeWuO5h6YrAeMyutZyiu2Qi";
        private const string callback_url = "https://localhost:8080/oauth-callback"; // This must match with the registered callback URL
        private const string scope = "publicData+esi-location.read_location.v1+esi-location.read_ship_type.v1+esi-skills.read_skills.v1+esi-skills.read_skillqueue.v1+esi-wallet.read_character_wallet.v1+esi-wallet.read_corporation_wallet.v1+esi-characters.read_contacts.v1+esi-corporations.read_corporation_membership.v1+esi-assets.read_assets.v1+esi-characters.write_contacts.v1+esi-markets.structure_markets.v1+esi-corporations.read_structures.v1+esi-characters.read_loyalty.v1+esi-characters.read_chat_channels.v1+esi-characters.read_medals.v1+esi-characters.read_standings.v1+esi-characters.read_agents_research.v1+esi-industry.read_character_jobs.v1+esi-markets.read_character_orders.v1+esi-characters.read_blueprints.v1+esi-characters.read_corporation_roles.v1+esi-location.read_online.v1+esi-characters.read_fatigue.v1+esi-corporations.track_members.v1+esi-wallet.read_corporation_wallets.v1+esi-characters.read_notifications.v1+esi-corporations.read_divisions.v1+esi-corporations.read_contacts.v1+esi-assets.read_corporation_assets.v1+esi-corporations.read_titles.v1+esi-corporations.read_blueprints.v1+esi-corporations.read_standings.v1+esi-corporations.read_starbases.v1+esi-industry.read_corporation_jobs.v1+esi-markets.read_corporation_orders.v1+esi-corporations.read_container_logs.v1+esi-industry.read_character_mining.v1+esi-industry.read_corporation_mining.v1+esi-corporations.read_facilities.v1+esi-corporations.read_medals.v1+esi-characters.read_titles.v1+esi-characters.read_fw_stats.v1+esi-corporations.read_fw_stats.v1";
        private const string aUTHORIZATION_ENDPOINT = "https://login.eveonline.com/v2/oauth/authorize";
        private const string tOKEN_ENDPOINT = "https://login.eveonline.com/v2/oauth/token";
        private const string certFile = "./localhost.pem";
        private const string keyFile = "./localhost-key.pem";
        private static string refreshToken = "";
        private const string vERIFICATION_ENDPOINT = "https://login.eveonline.com/oauth/verify";
        private const string codeFile = "./auth_code.tmp";
        private const string hTTP_CALLBACK_URL = "http://localhost:5555/oauth-callback/"; // This must match with the registered callback URL

        public static string LogFile  => logFile;
        public static string ClientId => clientId;
        public static string ClientSecret => clients_secret;
        public static string CALLBACK_URL => callback_url;
        public static string Scope => scope;
        public static string AUTHORIZATION_ENDPOINT => aUTHORIZATION_ENDPOINT;
        public static string TOKEN_ENDPOINT => tOKEN_ENDPOINT;
        public static string CertFile => certFile;
        public static string KeyFile => keyFile;
        public static string RefreshToken { get => refreshToken; set => refreshToken = value; }
        public static string VERIFICATION_ENDPOINT => vERIFICATION_ENDPOINT;
        public static string CodeFile => codeFile;
        public static string HTTP_CALLBACK_URL => hTTP_CALLBACK_URL;
    }
}
