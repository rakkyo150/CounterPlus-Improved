﻿using System.Collections.Generic;
using System.Reflection;
using CountersPlus.ConfigModels;
using CountersPlus.Custom;
using CountersPlus.Installers;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using HarmonyObj = HarmonyLib.Harmony;
using IPALogger = IPA.Logging.Logger;

namespace CountersPlus
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Logger { get; private set; }
        internal static MainConfigModel MainConfig { get; private set; }
        internal static Dictionary<Assembly, CustomCounter> LoadedCustomCounters { get; private set; } = new Dictionary<Assembly, CustomCounter>();

        private const string HARMONY_ID = "com.caeden117.countersplus";
        private HarmonyObj harmony;

        [Init]
        public Plugin(IPALogger logger,
            [Config.Name("CountersPlus")] Config conf,
            Zenjector zenjector)
        {
            Instance = this;
            Logger = logger;
            MainConfig = conf.Generated<MainConfigModel>();
            harmony = new HarmonyObj(HARMONY_ID);

            zenjector.OnApp<CoreInstaller>();
            zenjector.OnGame<CountersInstaller>()
                .Expose<CoreGameHUDController>()
                .ShortCircuitForTutorial()
                .ShortCircuitForMultiplayer(); // still dont have the time for this
            zenjector.OnMenu<MenuUIInstaller>();
        }

        [OnEnable]
        public void OnEnable()
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [OnDisable]
        public void OnDisable()
        {
            harmony.UnpatchAll(HARMONY_ID);
        }
    }
}
