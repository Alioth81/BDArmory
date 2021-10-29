using UnityEngine;

using System.Collections.Generic;

namespace BDArmory.Core
{
    public class BDArmorySettings
    {
        public static string oldSettingsConfigURL = "GameData/BDArmory/settings.cfg"; // Migrate from the old settings file to the new one in PluginData so that we don't invalidate the ModuleManager cache.
        public static string settingsConfigURL = "GameData/BDArmory/PluginData/settings.cfg";
        public static bool ready = false;

        // Settings section toggles
        [BDAPersistentSettingsField] public static bool GAMEPLAY_SETTINGS_TOGGLE = false;
        [BDAPersistentSettingsField] public static bool GRAPHICS_UI_SECTION_TOGGLE = false;
        [BDAPersistentSettingsField] public static bool GAME_MODES_SETTINGS_TOGGLE = false;
        [BDAPersistentSettingsField] public static bool SLIDER_SETTINGS_TOGGLE = false;
        [BDAPersistentSettingsField] public static bool RADAR_SETTINGS_TOGGLE = false;
        [BDAPersistentSettingsField] public static bool OTHER_SETTINGS_TOGGLE = false;
        [BDAPersistentSettingsField] public static bool DEBUG_SETTINGS_TOGGLE = false;
        [BDAPersistentSettingsField] public static bool COMPETITION_SETTINGS_TOGGLE = true;
        [BDAPersistentSettingsField] public static bool GM_SETTINGS_TOGGLE = false;
        [BDAPersistentSettingsField] public static bool SPAWN_SETTINGS_TOGGLE = false;
        [BDAPersistentSettingsField] public static bool ADVANDED_USER_SETTINGS = false;

        // Window settings
        [BDAPersistentSettingsField] public static bool STRICT_WINDOW_BOUNDARIES = true;
        [BDAPersistentSettingsField] public static float REMOTE_ORCHESTRATION_WINDOW_WIDTH = 225f;
        [BDAPersistentSettingsField] public static float VESSEL_SWITCHER_WINDOW_WIDTH = 500f;
        [BDAPersistentSettingsField] public static bool VESSEL_SWITCHER_WINDOW_SORTING = false;
        [BDAPersistentSettingsField] public static bool VESSEL_SWITCHER_WINDOW_OLD_DISPLAY_STYLE = false;
        [BDAPersistentSettingsField] public static float VESSEL_SPAWNER_WINDOW_WIDTH = 480f;
        [BDAPersistentSettingsField] public static float EVOLUTION_WINDOW_WIDTH = 350f;

        // General toggle settings
        //[BDAPersistentSettingsField] public static bool INSTAKILL = true; //Depreciated, only affects lasers; use an Instagib mutator isntead
        [BDAPersistentSettingsField] public static bool AI_TOOLBAR_BUTTON = true;                 // Show or hide the BDA AI toolbar button.
        [BDAPersistentSettingsField] public static bool INFINITE_AMMO = false;
        [BDAPersistentSettingsField] public static bool BULLET_HITS = true;
        [BDAPersistentSettingsField] public static bool EJECT_SHELLS = true;
        [BDAPersistentSettingsField] public static bool AIM_ASSIST = true;
        [BDAPersistentSettingsField] public static bool DRAW_AIMERS = true;
        [BDAPersistentSettingsField] public static bool DRAW_DEBUG_LINES = false;
        [BDAPersistentSettingsField] public static bool DRAW_DEBUG_LABELS = false;
        [BDAPersistentSettingsField] public static bool DRAW_ARMOR_LABELS = false;                 //armor only debug messages, for testing/debugging. remove/revert back to debug_labels later
        [BDAPersistentSettingsField] public static bool REMOTE_SHOOTING = false;
        [BDAPersistentSettingsField] public static bool BOMB_CLEARANCE_CHECK = false;
        [BDAPersistentSettingsField] public static bool SHOW_AMMO_GAUGES = false;
        [BDAPersistentSettingsField] public static bool SHELL_COLLISIONS = true;
        [BDAPersistentSettingsField] public static bool BULLET_DECALS = true;
        [BDAPersistentSettingsField] public static bool DISABLE_RAMMING = true;                   // Prevent craft from going into ramming mode when out of ammo.
        [BDAPersistentSettingsField] public static bool DEFAULT_FFA_TARGETING = false;            // Free-for-all combat style instead of teams (changes target selection behaviour)
        [BDAPersistentSettingsField] public static bool RUNWAY_PROJECT = false;                    // Enable/disable Runway Project specific enhancements.
        //[BDAPersistentSettingsField] public static bool DISABLE_KILL_TIMER = true;                //disables the kill timers.
        [BDAPersistentSettingsField] public static bool AUTO_ENABLE_VESSEL_SWITCHING = false;     // Automatically enables vessel switching on competition start.
        [BDAPersistentSettingsField] public static bool AUTONOMOUS_COMBAT_SEATS = false;          // Enable/disable seats without kerbals.
        [BDAPersistentSettingsField] public static bool DESTROY_UNCONTROLLED_WMS = false;         // Automatically destroy the WM if there's no kerbal or drone core controlling it.
        [BDAPersistentSettingsField] public static bool RESET_HP = false;                         // Automatically reset HP of parts of vessels when they're spawned in flight mode.
        [BDAPersistentSettingsField] public static bool RESET_ARMOUR = false;                     // Automatically reset Armor/ hull material of parts of vessels when they're spawned in flight mode.
        [BDAPersistentSettingsField] public static int KERBAL_SAFETY = 1;                         // Try to save kerbals by ejecting/leaving seats and deploying parachutes.
        [BDAPersistentSettingsField] public static bool TRACE_VESSELS_DURING_COMPETITIONS = false; // Trace vessel positions and rotations during competitions.
        [BDAPersistentSettingsField] public static bool DUMB_IR_SEEKERS = false;                  // IR missiles will go after hottest thing they can see
        [BDAPersistentSettingsField] public static bool AUTOCATEGORIZE_PARTS = true;
        [BDAPersistentSettingsField] public static bool SHOW_CATEGORIES = true;
        [BDAPersistentSettingsField] public static bool IGNORE_TERRAIN_CHECK = false;
        [BDAPersistentSettingsField] public static bool DISPLAY_PATHING_GRID = false;             //laggy when the grid gets large
        //[BDAPersistentSettingsField] public static bool ADVANCED_EDIT = true;                     //Used for debug fields not nomrally shown to regular users //SI - Only usage is a commented out function in BDExplosivePart
        [BDAPersistentSettingsField] public static bool DISPLAY_COMPETITION_STATUS = true;             //Display competition status
        [BDAPersistentSettingsField] public static bool DISPLAY_COMPETITION_STATUS_WITH_HIDDEN_UI = false; // Display the competition status when using the "hidden UI"
        [BDAPersistentSettingsField] public static bool BULLET_WATER_DRAG = true;
        [BDAPersistentSettingsField] public static bool PERSISTENT_FX = false;
        [BDAPersistentSettingsField] public static bool LEGACY_ARMOR = false;

        // General slider settings
        [BDAPersistentSettingsField] public static int COMPETITION_DURATION = 5;                       // Competition duration in minutes
        [BDAPersistentSettingsField] public static float COMPETITION_INITIAL_GRACE_PERIOD = 60;        // Competition initial grace period in seconds.
        [BDAPersistentSettingsField] public static float COMPETITION_FINAL_GRACE_PERIOD = 10;          // Competition final grace period in seconds.
        [BDAPersistentSettingsField] public static float COMPETITION_KILL_TIMER = 15;                  // Competition kill timer in seconds.
        [BDAPersistentSettingsField] public static float COMPETITION_KILLER_GM_FREQUENCY = 60;         // Competition killer GM timer in seconds.
        [BDAPersistentSettingsField] public static float COMPETITION_KILLER_GM_GRACE_PERIOD = 150;     // Competition killer GM grace period in seconds.
        [BDAPersistentSettingsField] public static float COMPETITION_ALTITUDE_LIMIT_HIGH = 46;         // Altitude (high) in km at which to kill off craft.
        [BDAPersistentSettingsField] public static float COMPETITION_ALTITUDE_LIMIT_LOW = -1;          // Altitude (low) in km at which to kill off craft.
        [BDAPersistentSettingsField] public static float COMPETITION_NONCOMPETITOR_REMOVAL_DELAY = 30; // Competition non-competitor removal delay in seconds.
        [BDAPersistentSettingsField] public static float COMPETITION_DISTANCE = 1000;                  // Competition distance.
        [BDAPersistentSettingsField] public static int COMPETITION_START_NOW_AFTER = 11;               // Competition auto-start now.
        [BDAPersistentSettingsField] public static float DEBRIS_CLEANUP_DELAY = 15f;                   // Clean up debris after 30s.
        [BDAPersistentSettingsField] public static int MAX_NUM_BULLET_DECALS = 200;
        [BDAPersistentSettingsField] public static int TERRAIN_ALERT_FREQUENCY = 1;                    // Controls how often terrain avoidance checks are made (gets scaled by 1+(radarAltitude/500)^2)
        [BDAPersistentSettingsField] public static int CAMERA_SWITCH_FREQUENCY = 3;                    // Controls the minimum time between automated camera switches
        [BDAPersistentSettingsField] public static int DEATH_CAMERA_SWITCH_INHIBIT_PERIOD = 0;         // Controls the delay before the next switch after the currently active vessel dies
        [BDAPersistentSettingsField] public static int KERBAL_SAFETY_INVENTORY = 2;                    // Controls how Kerbal Safety adjusts the inventory of kerbals.
        [BDAPersistentSettingsField] public static float TRIGGER_HOLD_TIME = 0.2f;
        [BDAPersistentSettingsField] public static float BDARMORY_UI_VOLUME = 0.35f;
        [BDAPersistentSettingsField] public static float BDARMORY_WEAPONS_VOLUME = 0.45f;
        [BDAPersistentSettingsField] public static float MAX_GUARD_VISUAL_RANGE = 200000f;
        [BDAPersistentSettingsField] public static float MAX_ACTIVE_RADAR_RANGE = 200000f;        //NOTE: used ONLY for display range of radar windows! Actual radar range provided by part configs!
        [BDAPersistentSettingsField] public static float MAX_ENGAGEMENT_RANGE = 200000f;          //NOTE: used ONLY for missile dlz parameters!
        [BDAPersistentSettingsField] public static float IVA_LOWPASS_FREQ = 2500f;
        [BDAPersistentSettingsField] public static float SMOKE_DEFLECTION_FACTOR = 10f;
        [BDAPersistentSettingsField] public static float BALLISTIC_TRAJECTORY_SIMULATION_MULTIPLIER = 256f;      // Multiplier of fixedDeltaTime for the large scale steps of ballistic trajectory simulations.
        [BDAPersistentSettingsField] public static float FIRE_RATE_OVERRIDE = 10f;
        [BDAPersistentSettingsField] public static float FIRE_RATE_OVERRIDE_CENTER = 20f;
        [BDAPersistentSettingsField] public static float FIRE_RATE_OVERRIDE_SPREAD = 5f;
        [BDAPersistentSettingsField] public static float FIRE_RATE_OVERRIDE_BIAS = 0.16f;
        [BDAPersistentSettingsField] public static float FIRE_RATE_OVERRIDE_HIT_MULTIPLIER = 2f;

        // Physics constants
        [BDAPersistentSettingsField] public static float GLOBAL_LIFT_MULTIPLIER = 0.25f;
        [BDAPersistentSettingsField] public static float GLOBAL_DRAG_MULTIPLIER = 6f;
        [BDAPersistentSettingsField] public static float RECOIL_FACTOR = 0.75f;
        [BDAPersistentSettingsField] public static float DMG_MULTIPLIER = 100f;
        [BDAPersistentSettingsField] public static float BALLISTIC_DMG_FACTOR = 1.55f;
        [BDAPersistentSettingsField] public static float HITPOINT_MULTIPLIER = 3.0f;
        [BDAPersistentSettingsField] public static float EXP_DMG_MOD_BALLISTIC_NEW = 0.65f;     // HE bullet explosion damage multiplier
        [BDAPersistentSettingsField] public static float EXP_DMG_MOD_MISSILE = 6.75f;           // Missile explosion damage multiplier
        [BDAPersistentSettingsField] public static float EXP_DMG_MOD_ROCKET = 1f;               // Rocket explosion damage multiplier (FIXME needs tuning; Note: rockets used Ballistic mod before, but probably ought to be more like missiles)
        [BDAPersistentSettingsField] public static float EXP_DMG_MOD_BATTLE_DAMAGE = 1f;        // Battle damage explosion damage multiplier (FIXME needs tuning; Note: CASE-0 explosions used Missile mod, while CASE-1, CASE-2 and fuel explosions used Ballistic mod)
        [BDAPersistentSettingsField] public static float EXP_IMP_MOD = 0.25f;
        [BDAPersistentSettingsField] public static bool EXTRA_DAMAGE_SLIDERS = false;
        [BDAPersistentSettingsField] public static float WEAPON_FX_DURATION = 15;               //how long do weapon secondary effects(EMP/choker/gravitic/etc) last
        [BDAPersistentSettingsField] public static float S4R2_DMG_MULT = 0.1f;

        // FX
        [BDAPersistentSettingsField] public static bool FIRE_FX_IN_FLIGHT = false;
        [BDAPersistentSettingsField] public static int MAX_FIRES_PER_VESSEL = 10;                 //controls fx for penetration only for landed or splashed //this is only for physical missile collisons into fueltanks - SI
        [BDAPersistentSettingsField] public static float FIRELIFETIME_IN_SECONDS = 90f;           //controls fx for penetration only for landed or splashed 

        // Radar settings
        [BDAPersistentSettingsField] public static float RWR_WINDOW_SCALE_MIN = 0.50f;
        [BDAPersistentSettingsField] public static float RWR_WINDOW_SCALE = 1f;
        [BDAPersistentSettingsField] public static float RWR_WINDOW_SCALE_MAX = 1.50f;
        [BDAPersistentSettingsField] public static float RADAR_WINDOW_SCALE_MIN = 0.50f;
        [BDAPersistentSettingsField] public static float RADAR_WINDOW_SCALE = 1f;
        [BDAPersistentSettingsField] public static float RADAR_WINDOW_SCALE_MAX = 1.50f;
        [BDAPersistentSettingsField] public static float TARGET_WINDOW_SCALE_MIN = 0.50f;
        [BDAPersistentSettingsField] public static float TARGET_WINDOW_SCALE = 1f;
        [BDAPersistentSettingsField] public static float TARGET_WINDOW_SCALE_MAX = 2f;
        [BDAPersistentSettingsField] public static float TARGET_CAM_RESOLUTION = 1024f;
        [BDAPersistentSettingsField] public static bool BW_TARGET_CAM = true;

        // Game modes
        [BDAPersistentSettingsField] public static bool PEACE_MODE = false;
        [BDAPersistentSettingsField] public static bool TAG_MODE = false;
        [BDAPersistentSettingsField] public static bool PAINTBALL_MODE = false;
        [BDAPersistentSettingsField] public static bool GRAVITY_HACKS = false;
        [BDAPersistentSettingsField] public static bool BATTLEDAMAGE = false;
        [BDAPersistentSettingsField] public static bool HEART_BLEED_ENABLED = false;
        [BDAPersistentSettingsField] public static bool RESOURCE_STEAL_ENABLED = false;
        [BDAPersistentSettingsField] public static bool ASTEROID_FIELD = false;
        [BDAPersistentSettingsField] public static int ASTEROID_FIELD_NUMBER = 100; // Number of asteroids
        [BDAPersistentSettingsField] public static float ASTEROID_FIELD_ALTITUDE = 2f; // Km.
        [BDAPersistentSettingsField] public static float ASTEROID_FIELD_RADIUS = 5f; // Km.
        [BDAPersistentSettingsField] public static bool ASTEROID_FIELD_ANOMALOUS_ATTRACTION = false; // Asteroids are attracted to vessels.
        [BDAPersistentSettingsField] public static float ASTEROID_FIELD_ANOMALOUS_ATTRACTION_STRENGTH = 0.2f; // Strength of the effect.
        [BDAPersistentSettingsField] public static bool ASTEROID_RAIN = false;
        [BDAPersistentSettingsField] public static int ASTEROID_RAIN_NUMBER = 100; // Number of asteroids
        [BDAPersistentSettingsField] public static float ASTEROID_RAIN_DENSITY = 0.5f; // Arbitrary density scale.
        [BDAPersistentSettingsField] public static float ASTEROID_RAIN_ALTITUDE = 2f; // Km.k
        [BDAPersistentSettingsField] public static float ASTEROID_RAIN_RADIUS = 3f; // Km.
        [BDAPersistentSettingsField] public static bool ASTEROID_RAIN_FOLLOWS_CENTROID = true;
        [BDAPersistentSettingsField] public static bool ASTEROID_RAIN_FOLLOWS_SPREAD = true;
        [BDAPersistentSettingsField] public static bool MUTATOR_MODE = false;

        //Battle Damage settings
        [BDAPersistentSettingsField] public static bool BATTLEDAMAGE_TOGGLE = false;    // Main battle damage toggle.
        [BDAPersistentSettingsField] public static float BD_DAMAGE_CHANCE = 5;          // Base chance per-hit to proc damage
        [BDAPersistentSettingsField] public static bool BD_SUBSYSTEMS = true;           // Non-critical module damage?
        [BDAPersistentSettingsField] public static bool BD_TANKS = true;                // Fuel tanks, batteries can leak/burn
        [BDAPersistentSettingsField] public static float BD_TANK_LEAK_TIME = 20;        // Leak duration
        [BDAPersistentSettingsField] public static float BD_TANK_LEAK_RATE = 1;         // Leak rate modifier
        [BDAPersistentSettingsField] public static bool BD_AMMOBINS = true;             // Can ammo bins explode?
        [BDAPersistentSettingsField] public static bool BD_VOLATILE_AMMO = false;       // Ammo bins guaranteed to explode when destroyed
        [BDAPersistentSettingsField] public static bool BD_PROPULSION = true;           // Engine thrust reduction, fires
        [BDAPersistentSettingsField] public static float BD_PROP_FLOOR = 20;            // Minimum thrust% damaged engines produce
        [BDAPersistentSettingsField] public static float BD_PROP_FLAMEOUT = 25;         // Remaining HP% engines flameout
        [BDAPersistentSettingsField] public static bool BD_BALANCED_THRUST = true;
        [BDAPersistentSettingsField] public static float BD_PROP_DAM_RATE = 1;          // Rate multiplier, 0.1-2
        [BDAPersistentSettingsField] public static bool BD_INTAKES = true;              // Can intakes be damaged?
        [BDAPersistentSettingsField] public static bool BD_GIMBALS = true;              // Can gimbals be disabled?
        [BDAPersistentSettingsField] public static bool BD_AEROPARTS = true;            // Lift loss & added drag
        [BDAPersistentSettingsField] public static float BD_LIFT_LOSS_RATE = 1;         // Rate multiplier
        [BDAPersistentSettingsField] public static bool BD_CTRL_SRF = true;             // Disable ctrl srf actuatiors?
        [BDAPersistentSettingsField] public static bool BD_COCKPITS = false;            // Control degredation
        [BDAPersistentSettingsField] public static bool BD_PILOT_KILLS = false;         // Cockpit damage can kill pilots?
        [BDAPersistentSettingsField] public static bool BD_FIRES_ENABLED = true;        // Can fires occur
        [BDAPersistentSettingsField] public static bool BD_FIRE_DOT = true;             // Do fires do DoT
        [BDAPersistentSettingsField] public static float BD_FIRE_DAMAGE = 5;            // Do fires do DoT
        [BDAPersistentSettingsField] public static bool BD_FIRE_HEATDMG = true;         // Do fires add heat to parts/are fires able to cook off fuel/ammo?
        [BDAPersistentSettingsField] public static bool BD_INTENSE_FIRES = false;       // Do fuel tank fires DoT get bigger over time?
        [BDAPersistentSettingsField] public static bool ALLOW_S4R2_BD = false;          // Allow battle damage to proc when using zombie mode?

        // Remote logging
        [BDAPersistentSettingsField] public static bool REMOTE_LOGGING_VISIBLE = false;                                   // Show/hide the remote orchestration toggle
        [BDAPersistentSettingsField] public static bool REMOTE_LOGGING_ENABLED = false;                                   // Enable/disable remote orchestration
        [BDAPersistentSettingsField] public static string REMOTE_ORCHESTRATION_BASE_URL = "bdascores.herokuapp.com";      // Base URL used for orchestration (note: we can't include the https:// as it breaks KSP's serialisation routine)
        [BDAPersistentSettingsField] public static string REMOTE_CLIENT_SECRET = "";                                      // Token used to authorize remote orchestration client
        [BDAPersistentSettingsField] public static string COMPETITION_HASH = "";                                          // Competition hash used for orchestration
        [BDAPersistentSettingsField] public static float REMOTE_INTERHEAT_DELAY = 30;                                     // Delay between heats.
        [BDAPersistentSettingsField] public static int RUNWAY_PROJECT_ROUND = 10;                                         // RWP round index.

        // Spawner settings
        [BDAPersistentSettingsField] public static bool SHOW_SPAWN_OPTIONS = true;                 // Show spawn options.
        [BDAPersistentSettingsField] public static Vector2d VESSEL_SPAWN_GEOCOORDS = new Vector2d(0.05096, -74.8016); // Spawning coordinates on a planetary body.
        [BDAPersistentSettingsField] public static float VESSEL_SPAWN_ALTITUDE = 5f;               // Spawning altitude above the surface.
        [BDAPersistentSettingsField] public static float VESSEL_SPAWN_DISTANCE_FACTOR = 20f;       // Scale factor for the size of the spawning circle.
        [BDAPersistentSettingsField] public static float VESSEL_SPAWN_DISTANCE = 10f;              // Radius of the size of the spawning circle.
        [BDAPersistentSettingsField] public static bool VESSEL_SPAWN_DISTANCE_TOGGLE = false;      // Toggle between scaling factor and absolute distance.
        [BDAPersistentSettingsField] public static bool VESSEL_SPAWN_REASSIGN_TEAMS = true;        // Reassign teams on spawn, overriding teams defined in the SPH.
        [BDAPersistentSettingsField] public static float VESSEL_SPAWN_EASE_IN_SPEED = 1f;          // Rate to limit "falling" during spawning.
        [BDAPersistentSettingsField] public static int VESSEL_SPAWN_CONCURRENT_VESSELS = 0;        // Maximum number of vessels to spawn in concurrently (continuous spawning mode).
        [BDAPersistentSettingsField] public static int VESSEL_SPAWN_LIVES_PER_VESSEL = 0;          // Maximum number of times to spawn a vessel (continuous spawning mode).
        [BDAPersistentSettingsField] public static float OUT_OF_AMMO_KILL_TIME = -1f;              // Out of ammo kill timer for continuous spawn mode.
        [BDAPersistentSettingsField] public static int VESSEL_SPAWN_FILL_SEATS = 1;                // Fill seats: 0 - minimal, 1 - all ModuleCommand and KerbalSeat parts, 2 - also cabins.
        [BDAPersistentSettingsField] public static bool VESSEL_SPAWN_CONTINUE_SINGLE_SPAWNING = false; // Spawn craft again after single spawn competition finishes.
        [BDAPersistentSettingsField] public static bool VESSEL_SPAWN_DUMP_LOG_EVERY_SPAWN = false; // Dump competition scores every time a vessel spawns.
        [BDAPersistentSettingsField] public static bool SHOW_SPAWN_LOCATIONS = false;              // Show the interesting spawn locations.
        [BDAPersistentSettingsField] public static int VESSEL_SPAWN_NUMBER_OF_TEAMS = 0;           // Number of Teams: 0 - FFA, 1 - Folders, 2-10 specified directly
        [BDAPersistentSettingsField] public static string VESSEL_SPAWN_FILES_LOCATION = "";        // Spawn files location (under AutoSpawn).
        [BDAPersistentSettingsField] public static bool VESSEL_SPAWN_RANDOM_ORDER = true;          // Shuffle vessels before spawning them.

        // Heartbleed
        [BDAPersistentSettingsField] public static float HEART_BLEED_RATE = 0.01f;
        [BDAPersistentSettingsField] public static float HEART_BLEED_INTERVAL = 10f;
        [BDAPersistentSettingsField] public static float HEART_BLEED_THRESHOLD = 10f;

        // Resource steal
        [BDAPersistentSettingsField] public static float RESOURCE_STEAL_FUEL_RATION = 0.2f;
        [BDAPersistentSettingsField] public static float RESOURCE_STEAL_AMMO_RATION = 0.2f;
        [BDAPersistentSettingsField] public static float RESOURCE_STEAL_CM_RATION = 0f;

        //Space Friction
        [BDAPersistentSettingsField] public static bool SPACE_HACKS = false;
        [BDAPersistentSettingsField] public static bool SF_FRICTION = false;
        [BDAPersistentSettingsField] public static bool SF_GRAVITY = false;
        [BDAPersistentSettingsField] public static bool SF_REPULSOR = false;
        [BDAPersistentSettingsField] public static float SF_DRAGMULT = 2f;

        //Mutator Mode
        [BDAPersistentSettingsField] public static bool MUTATOR_APPLY_GLOBAL = false;
        [BDAPersistentSettingsField] public static bool MUTATOR_APPLY_KILL = false;
        [BDAPersistentSettingsField] public static bool MUTATOR_APPLY_TIMER = false;
        [BDAPersistentSettingsField] public static float MUTATOR_DURATION = 0.5f;
        [BDAPersistentSettingsField] public static List<string> MUTATOR_LIST = new List<string>();
        [BDAPersistentSettingsField] public static int MUTATOR_APPLY_NUM = 1;
        [BDAPersistentSettingsField] public static bool MUTATOR_ICONS = false;

        // Tournament settings
        [BDAPersistentSettingsField] public static bool SHOW_TOURNAMENT_OPTIONS = false;           // Show tournament options.
        [BDAPersistentSettingsField] public static int TOURNAMENT_STYLE = 0;                       // Tournament Style (Random, N-choose-K, etc.)
        [BDAPersistentSettingsField] public static float TOURNAMENT_DELAY_BETWEEN_HEATS = 10;      // Delay between heats
        [BDAPersistentSettingsField] public static int TOURNAMENT_ROUNDS = 1;                      // Rounds
        [BDAPersistentSettingsField] public static int TOURNAMENT_VESSELS_PER_HEAT = 8;            // Vessels Per Heat
        [BDAPersistentSettingsField] public static int TOURNAMENT_TEAMS_PER_HEAT = 2;              // Teams Per Heat
        [BDAPersistentSettingsField] public static int TOURNAMENT_VESSELS_PER_TEAM = 2;            // Vessels Per Team
        [BDAPersistentSettingsField] public static bool TOURNAMENT_FULL_TEAMS = true;              // Full Teams
        [BDAPersistentSettingsField] public static float TOURNAMENT_TIMEWARP_BETWEEN_ROUNDS = 0;   // Timewarp between rounds in minutes.
        [BDAPersistentSettingsField] public static bool AUTO_RESUME_TOURNAMENT = false;            // Automatically load the game the last incomplete tournament was running in and continue the tournament.
        [BDAPersistentSettingsField] public static float QUIT_MEMORY_USAGE_THRESHOLD = float.MaxValue; // Automatically quit KSP when memory usage is beyond this. (0 = disabled)

        // Scoring categories
        [BDAPersistentSettingsField] public static float SCORING_HEADSHOT = 3;                     // Head-Shot Time Limit
        [BDAPersistentSettingsField] public static float SCORING_KILLSTEAL = 5;                   // Kill-Steal Time Limit

        // Evolution settings
        [BDAPersistentSettingsField] public static bool EVOLUTION_ENABLED = false;
        [BDAPersistentSettingsField] public static bool SHOW_EVOLUTION_OPTIONS = false;
        [BDAPersistentSettingsField] public static int EVOLUTION_ANTAGONISTS_PER_HEAT = 1;
        [BDAPersistentSettingsField] public static int EVOLUTION_MUTATIONS_PER_HEAT = 1;
        [BDAPersistentSettingsField] public static int EVOLUTION_HEATS_PER_GROUP = 1;
        [BDAPersistentSettingsField] public static bool AUTO_RESUME_EVOLUTION = false;             // Automatically load the game and start evolution with the last used settings/seeds. Note: this overrides the AUTO_RESUME_TOURNAMENT setting.

        // Countermeasure constants
        [BDAPersistentSettingsField] public static float FLARE_FACTOR = 1.6f;                       // Change this to make flares more or less effective, values close to or below 1.0 will cause flares to fail to decoy often
        [BDAPersistentSettingsField] public static float CHAFF_FACTOR = 0.6f;                       // Change this to make chaff more or less effective. Higher values will make chaff batter, lower values will make chaff worse.
    }
}
