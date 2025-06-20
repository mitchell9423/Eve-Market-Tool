using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EveMarket.Network.OAuth
{
    public static class LoginSettings
    {
        private static string clientId = "c1b9bcd8e791468a9ad4cd8bade13545";
        private static string clientsSecret = "opIC0Mh4CCmMlIhHCIeWuO5h6YrAeMyutZyiu2Qi";
        private static string scope = "publicData+esi-location.read_location.v1+esi-location.read_ship_type.v1+esi-skills.read_skills.v1+esi-skills.read_skillqueue.v1+esi-wallet.read_character_wallet.v1+esi-wallet.read_corporation_wallet.v1+esi-characters.read_contacts.v1+esi-corporations.read_corporation_membership.v1+esi-assets.read_assets.v1+esi-characters.write_contacts.v1+esi-markets.structure_markets.v1+esi-corporations.read_structures.v1+esi-characters.read_loyalty.v1+esi-characters.read_chat_channels.v1+esi-characters.read_medals.v1+esi-characters.read_standings.v1+esi-characters.read_agents_research.v1+esi-industry.read_character_jobs.v1+esi-markets.read_character_orders.v1+esi-characters.read_blueprints.v1+esi-characters.read_corporation_roles.v1+esi-location.read_online.v1+esi-characters.read_fatigue.v1+esi-corporations.track_members.v1+esi-wallet.read_corporation_wallets.v1+esi-characters.read_notifications.v1+esi-corporations.read_divisions.v1+esi-corporations.read_contacts.v1+esi-assets.read_corporation_assets.v1+esi-corporations.read_titles.v1+esi-corporations.read_blueprints.v1+esi-corporations.read_standings.v1+esi-corporations.read_starbases.v1+esi-industry.read_corporation_jobs.v1+esi-markets.read_corporation_orders.v1+esi-corporations.read_container_logs.v1+esi-industry.read_character_mining.v1+esi-industry.read_corporation_mining.v1+esi-corporations.read_facilities.v1+esi-corporations.read_medals.v1+esi-characters.read_titles.v1+esi-characters.read_fw_stats.v1+esi-corporations.read_fw_stats.v1";
        private static string certFile = "./localhost.pem";
        private static string keyFile = "./localhost-key.pem";
        private static string codeFile = "./auth_code.tmp";
        private static string tokenFile = "./token.json";
        private static string logFile = "./esi_auth.log";

        public static string ClientId { get => clientId; set => clientId = value; }
        public static string ClientSecret { get => clientsSecret; set => clientsSecret = value; }
        public static string Scope { get => scope; set => scope = value; }
        public static string CertFile { get => certFile; set => certFile = value; }
        public static string KeyFile { get => keyFile; set => keyFile = value; }
        public static string CodeFile { get => codeFile; set => codeFile = value; }
        public static string TokenFile { get => tokenFile; set => tokenFile = value; }
        public static string LogFile { get => logFile; set => logFile = value; }
    }
}
