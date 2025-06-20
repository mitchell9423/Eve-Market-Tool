

using System.Collections;





namespace EveMarket.Network
{
    public class SsoScopes
    {

        public const string SCOPES = "publicData+esi-location.read_location.v1+esi-location.read_ship_type.v1+esi-skills.read_skills.v1+esi-skills.read_skillqueue.v1+esi-wallet.read_character_wallet.v1+esi-wallet.read_corporation_wallet.v1+esi-characters.read_contacts.v1+esi-corporations.read_corporation_membership.v1+esi-assets.read_assets.v1+esi-characters.write_contacts.v1+esi-markets.structure_markets.v1+esi-corporations.read_structures.v1+esi-characters.read_loyalty.v1+esi-characters.read_chat_channels.v1+esi-characters.read_medals.v1+esi-characters.read_standings.v1+esi-characters.read_agents_research.v1+esi-industry.read_character_jobs.v1+esi-markets.read_character_orders.v1+esi-characters.read_blueprints.v1+esi-characters.read_corporation_roles.v1+esi-location.read_online.v1+esi-characters.read_fatigue.v1+esi-corporations.track_members.v1+esi-wallet.read_corporation_wallets.v1+esi-characters.read_notifications.v1+esi-corporations.read_divisions.v1+esi-corporations.read_contacts.v1+esi-assets.read_corporation_assets.v1+esi-corporations.read_titles.v1+esi-corporations.read_blueprints.v1+esi-corporations.read_standings.v1+esi-corporations.read_starbases.v1+esi-industry.read_corporation_jobs.v1+esi-markets.read_corporation_orders.v1+esi-corporations.read_container_logs.v1+esi-industry.read_character_mining.v1+esi-industry.read_corporation_mining.v1+esi-corporations.read_facilities.v1+esi-corporations.read_medals.v1+esi-characters.read_titles.v1+esi-characters.read_fw_stats.v1+esi-corporations.read_fw_stats.v1";

        public const string PUBLIC_DATA = "publicData";
        public const string ESI_ALLIANCES_READ_CONTACTS_V1 = "esi-alliances.read_contacts.v1";
        public const string ESI_ASSETS_READ_ASSETS_V1 = "esi-assets.read_assets.v1";
        public const string ESI_ASSETS_READ_CORPORATION_ASSETS_V1 = "esi-assets.read_corporation_assets.v1";
        public const string ESI_CALENDAR_READ_CALENDAR_EVENTS_V1 = "esi-calendar.read_calendar_events.v1";
        public const string ESI_CALENDAR_RESPOND_CALENDAR_EVENTS_V1 = "esi-calendar.respond_calendar_events.v1";
        public const string ESI_CHARACTERS_READ_AGENTS_RESEARCH_V1 = "esi-characters.read_agents_research.v1";
        public const string ESI_CHARACTERS_READ_BLUEPRINTS_V1 = "esi-characters.read_blueprints.v1";
        public const string ESI_CHARACTERS_READ_CONTACTS_V1 = "esi-characters.read_contacts.v1";
        public const string ESI_CHARACTERS_READ_CORPORATION_ROLES_V1 = "esi-characters.read_corporation_roles.v1";
        public const string ESI_CHARACTERS_READ_FATIGUE_V1 = "esi-characters.read_fatigue.v1";
        public const string ESI_CHARACTERS_READ_FW_STATS_V1 = "esi-characters.read_fw_stats.v1";
        public const string ESI_CHARACTERS_READ_LOYALTY_V1 = "esi-characters.read_loyalty.v1";
        public const string ESI_CHARACTERS_READ_MEDALS_V1 = "esi-characters.read_medals.v1";
        public const string ESI_CHARACTERS_READ_NOTIFICATIONS_V1 = "esi-characters.read_notifications.v1";
        public const string ESI_CHARACTERS_READ_STANDINGS_V1 = "esi-characters.read_standings.v1";
        public const string ESI_CHARACTERS_READ_TITLES_V1 = "esi-characters.read_titles.v1";
        public const string ESI_CHARACTERS_WRITE_CONTACTS_V1 = "esi-characters.write_contacts.v1";
        public const string ESI_CLONES_READ_CLONES_V1 = "esi-clones.read_clones.v1";
        public const string ESI_CLONES_READ_IMPLANTS_V1 = "esi-clones.read_implants.v1";
        public const string ESI_CONTRACTS_READ_CHARACTER_CONTRACTS_V1 = "esi-contracts.read_character_contracts.v1";
        public const string ESI_CONTRACTS_READ_CORPORATION_CONTRACTS_V1 = "esi-contracts.read_corporation_contracts.v1";
        public const string ESI_CORPORATIONS_READ_BLUEPRINTS_V1 = "esi-corporations.read_blueprints.v1";
        public const string ESI_CORPORATIONS_READ_CONTACTS_V1 = "esi-corporations.read_contacts.v1";
        public const string ESI_CORPORATIONS_READ_CONTAINER_LOGS_V1 = "esi-corporations.read_container_logs.v1";
        public const string ESI_CORPORATIONS_READ_CORPORATION_MEMBERSHIP_V1 = "esi-corporations.read_corporation_membership.v1";
        public const string ESI_CORPORATIONS_READ_DIVISIONS_V1 = "esi-corporations.read_divisions.v1";
        public const string ESI_CORPORATIONS_READ_FACILITIES_V1 = "esi-corporations.read_facilities.v1";
        public const string ESI_CORPORATIONS_READ_FW_STATS_V1 = "esi-corporations.read_fw_stats.v1";
        public const string ESI_CORPORATIONS_READ_MEDALS_V1 = "esi-corporations.read_medals.v1";
        public const string ESI_CORPORATIONS_READ_STANDINGS_V1 = "esi-corporations.read_standings.v1";
        public const string ESI_CORPORATIONS_READ_STARBASES_V1 = "esi-corporations.read_starbases.v1";
        public const string ESI_CORPORATIONS_READ_STRUCTURES_V1 = "esi-corporations.read_structures.v1";
        public const string ESI_CORPORATIONS_READ_TITLES_V1 = "esi-corporations.read_titles.v1";
        public const string ESI_CORPORATIONS_TRACK_MEMBERS_V1 = "esi-corporations.track_members.v1";
        public const string ESI_FITTINGS_READ_FITTINGS_V1 = "esi-fittings.read_fittings.v1";
        public const string ESI_FITTINGS_WRITE_FITTINGS_V1 = "esi-fittings.write_fittings.v1";
        public const string ESI_FLEETS_READ_FLEET_V1 = "esi-fleets.read_fleet.v1";
        public const string ESI_FLEETS_WRITE_FLEET_V1 = "esi-fleets.write_fleet.v1";
        public const string ESI_INDUSTRY_READ_CHARACTER_JOBS_V1 = "esi-industry.read_character_jobs.v1";
        public const string ESI_INDUSTRY_READ_CHARACTER_MINING_V1 = "esi-industry.read_character_mining.v1";
        public const string ESI_INDUSTRY_READ_CORPORATION_JOBS_V1 = "esi-industry.read_corporation_jobs.v1";
        public const string ESI_INDUSTRY_READ_CORPORATION_MINING_V1 = "esi-industry.read_corporation_mining.v1";
        public const string ESI_KILLMAILS_READ_CORPORATION_KILLMAILS_V1 = "esi-killmails.read_corporation_killmails.v1";
        public const string ESI_KILLMAILS_READ_KILLMAILS_V1 = "esi-killmails.read_killmails.v1";
        public const string ESI_LOCATION_READ_LOCATION_V1 = "esi-location.read_location.v1";
        public const string ESI_LOCATION_READ_ONLINE_V1 = "esi-location.read_online.v1";
        public const string ESI_LOCATION_READ_SHIP_TYPE_V1 = "esi-location.read_ship_type.v1";
        public const string ESI_MAIL_ORGANIZE_MAIL_V1 = "esi-mail.organize_mail.v1";
        public const string ESI_MAIL_READ_MAIL_V1 = "esi-mail.read_mail.v1";
        public const string ESI_MAIL_SEND_MAIL_V1 = "esi-mail.send_mail.v1";
        public const string ESI_MARKETS_READ_CHARACTER_ORDERS_V1 = "esi-markets.read_character_orders.v1";
        public const string ESI_MARKETS_READ_CORPORATION_ORDERS_V1 = "esi-markets.read_corporation_orders.v1";
        public const string ESI_MARKETS_STRUCTURE_MARKETS_V1 = "esi-markets.structure_markets.v1";
        public const string ESI_PLANETS_MANAGE_PLANETS_V1 = "esi-planets.manage_planets.v1";
        public const string ESI_PLANETS_READ_CUSTOMS_OFFICES_V1 = "esi-planets.read_customs_offices.v1";
        public const string ESI_SEARCH_SEARCH_STRUCTURES_V1 = "esi-search.search_structures.v1";
        public const string ESI_SKILLS_READ_SKILLQUEUE_V1 = "esi-skills.read_skillqueue.v1";
        public const string ESI_SKILLS_READ_SKILLS_V1 = "esi-skills.read_skills.v1";
        public const string ESI_UI_OPEN_WINDOW_V1 = "esi-ui.open_window.v1";
        public const string ESI_UI_WRITE_WAYPOINT_V1 = "esi-ui.write_waypoint.v1";
        public const string ESI_UNIVERSE_READ_STRUCTURES_V1 = "esi-universe.read_structures.v1";
        public const string ESI_WALLET_READ_CHARACTER_WALLET_V1 = "esi-wallet.read_character_wallet.v1";
        public const string ESI_WALLET_READ_CORPORATION_WALLETS_V1 = "esi-wallet.read_corporation_wallets.v1";

        private string[] ALL_VALUES = { ESI_ALLIANCES_READ_CONTACTS_V1, ESI_ASSETS_READ_ASSETS_V1,
            ESI_ASSETS_READ_CORPORATION_ASSETS_V1, ESI_CALENDAR_READ_CALENDAR_EVENTS_V1,
            ESI_CALENDAR_RESPOND_CALENDAR_EVENTS_V1, ESI_CHARACTERS_READ_AGENTS_RESEARCH_V1,
            ESI_CHARACTERS_READ_BLUEPRINTS_V1, ESI_CHARACTERS_READ_CONTACTS_V1,
            ESI_CHARACTERS_READ_CORPORATION_ROLES_V1, ESI_CHARACTERS_READ_FATIGUE_V1, ESI_CHARACTERS_READ_FW_STATS_V1,
            ESI_CHARACTERS_READ_LOYALTY_V1, ESI_CHARACTERS_READ_MEDALS_V1, ESI_CHARACTERS_READ_NOTIFICATIONS_V1,
            ESI_CHARACTERS_READ_STANDINGS_V1, ESI_CHARACTERS_READ_TITLES_V1, ESI_CHARACTERS_WRITE_CONTACTS_V1,
            ESI_CLONES_READ_CLONES_V1, ESI_CLONES_READ_IMPLANTS_V1, ESI_CONTRACTS_READ_CHARACTER_CONTRACTS_V1,
            ESI_CONTRACTS_READ_CORPORATION_CONTRACTS_V1, ESI_CORPORATIONS_READ_BLUEPRINTS_V1,
            ESI_CORPORATIONS_READ_CONTACTS_V1, ESI_CORPORATIONS_READ_CONTAINER_LOGS_V1,
            ESI_CORPORATIONS_READ_CORPORATION_MEMBERSHIP_V1, ESI_CORPORATIONS_READ_DIVISIONS_V1,
            ESI_CORPORATIONS_READ_FACILITIES_V1, ESI_CORPORATIONS_READ_FW_STATS_V1, ESI_CORPORATIONS_READ_MEDALS_V1,
            ESI_CORPORATIONS_READ_STANDINGS_V1, ESI_CORPORATIONS_READ_STARBASES_V1,
            ESI_CORPORATIONS_READ_STRUCTURES_V1, ESI_CORPORATIONS_READ_TITLES_V1, ESI_CORPORATIONS_TRACK_MEMBERS_V1,
            ESI_FITTINGS_READ_FITTINGS_V1, ESI_FITTINGS_WRITE_FITTINGS_V1, ESI_FLEETS_READ_FLEET_V1,
            ESI_FLEETS_WRITE_FLEET_V1, ESI_INDUSTRY_READ_CHARACTER_JOBS_V1, ESI_INDUSTRY_READ_CHARACTER_MINING_V1,
            ESI_INDUSTRY_READ_CORPORATION_JOBS_V1, ESI_INDUSTRY_READ_CORPORATION_MINING_V1,
            ESI_KILLMAILS_READ_CORPORATION_KILLMAILS_V1, ESI_KILLMAILS_READ_KILLMAILS_V1,
            ESI_LOCATION_READ_LOCATION_V1, ESI_LOCATION_READ_ONLINE_V1, ESI_LOCATION_READ_SHIP_TYPE_V1,
            ESI_MAIL_ORGANIZE_MAIL_V1, ESI_MAIL_READ_MAIL_V1, ESI_MAIL_SEND_MAIL_V1,
            ESI_MARKETS_READ_CHARACTER_ORDERS_V1, ESI_MARKETS_READ_CORPORATION_ORDERS_V1,
            ESI_MARKETS_STRUCTURE_MARKETS_V1, ESI_PLANETS_MANAGE_PLANETS_V1, ESI_PLANETS_READ_CUSTOMS_OFFICES_V1,
            ESI_SEARCH_SEARCH_STRUCTURES_V1, ESI_SKILLS_READ_SKILLQUEUE_V1, ESI_SKILLS_READ_SKILLS_V1,
            ESI_UI_OPEN_WINDOW_V1, ESI_UI_WRITE_WAYPOINT_V1, ESI_UNIVERSE_READ_STRUCTURES_V1,
            ESI_WALLET_READ_CHARACTER_WALLET_V1, ESI_WALLET_READ_CORPORATION_WALLETS_V1
        };

        //public const string ALL = ALL_VALUES.list;

    }

    public static class NetworkSettings
    {
        // Eve ESI API
        private const string mARKET_PRICES_URI = "https://esi.evetech.net/latest/markets/prices/";
        private const string mARKET_GROUP_URI = "https://esi.evetech.net/latest/markets/groups/";
        private const string uNIVERSE_TYPES_URI = "https://esi.evetech.net/latest/universe/types/";
        private const string iTEM_ORDERS_URI = "https://esi.evetech.net/latest/markets/[region_id]/orders/?datasource=tranquility&order_type=all&page=1&type_id=[type_id]";
        private const string iTEM_HISTORY_URI = "https://esi.evetech.net/latest/markets/[region_id]/history/?datasource=tranquility&type_id=[type_id]";
        private const string rOUTE_URI = "https://esi.evetech.net/latest/route/[destination]/[origin]/?datasource=tranquility&flag=shortest";
        private const string cORP_ORDERS_URI = "https://esi.evetech.net/latest/corporations/[type_id]/orders/?datasource=tranquility&page=1&token=";

        // Login/Authurization
        private const string hTTP_CALLBACK_URL = "http://localhost:5555/oauth-callback/"; // This must match with the registered callback URL
        private const string cALLBACK_URL = "https://localhost:8080/oauth-callback"; // This must match with the registered callback URL
        private const string aUTHORIZATION_ENDPOINT = "https://login.eveonline.com/v2/oauth/authorize";
        private const string tOKEN_ENDPOINT = "https://login.eveonline.com/v2/oauth/token";
        private const string vERIFICATION_ENDPOINT = "https://login.eveonline.com/oauth/verify";

        public static string MARKET_PRICES_URI => mARKET_PRICES_URI;
        public static string MARKET_GROUP_URI => mARKET_GROUP_URI;
        public static string UNIVERSE_TYPES_URI => uNIVERSE_TYPES_URI;
        public static string ITEM_ORDERS_URI => iTEM_ORDERS_URI;
        public static string ITEM_HISTORY_URI => iTEM_HISTORY_URI;
        public static string ROUTE_URI => rOUTE_URI;
        public static string CORP_ORDERS_URI => cORP_ORDERS_URI;
        public static string HTTP_CALLBACK_URL => hTTP_CALLBACK_URL;
        public static string CALLBACK_URL => cALLBACK_URL;
        public static string AUTHORIZATION_ENDPOINT => aUTHORIZATION_ENDPOINT;
        public static string TOKEN_ENDPOINT => tOKEN_ENDPOINT;
        public static string VERIFICATION_ENDPOINT => vERIFICATION_ENDPOINT;
    }

}
