﻿using GearsAPI.Settings;
using GearsAPI.Settings.Global;
using GearsAPI.Settings.World;
using HarmonyLib;
using KFCommonUtilityLib;
using KFCommonUtilityLib.Scripts.StaticManagers;
using KFCommonUtilityLib.Scripts.Utilities;
using System.Reflection;
using System.Runtime.InteropServices;

public class CommonUtilityLibInit : IModApi
{
    private static bool inited = false;
    internal static Harmony HarmonyInstance { get; private set; }
    public void InitMod(Mod _modInstance)
    {
        if (inited)
            return;
        inited = true;
        Log.Out(" Loading Patch: " + GetType());
        unsafe
        {
            Log.Out($"size of MultiActionIndice: {sizeof(MultiActionIndice)} marshal size: {Marshal.SizeOf<MultiActionIndice>()}");
            Log.Out($"{AccessTools.Method(typeof(ItemActionRanged), nameof(ItemActionRanged.StartHolding)).FullDescription()}");
        }
        //QualitySettings.streamingMipmapsMemoryBudget = 4096;
        DelayLoadModuleManager.RegisterDelayloadDll("FullautoLauncher", "FullautoLauncherAnimationRiggingCompatibilityPatch");
        DelayLoadModuleManager.RegisterDelayloadDll("Quartz", "QuartzUIPatch");
        //DelayLoadModuleManager.RegisterDelayloadDll("SMXcore", "SMXMultiActionCompatibilityPatch");
        //DelayLoadModuleManager.RegisterDelayloadDll("SCore", "SCoreEntityHitCompatibilityPatch");
        CustomEffectEnumManager.RegisterEnumType<MinEventTypes>();
        CustomEffectEnumManager.RegisterEnumType<PassiveEffects>();

        ModuleManagers.ClearOutputFolder();
        ItemClassModuleManager.Init();
        ItemActionModuleManager.Init();

        ModEvents.GameAwake.RegisterHandler(CommonUtilityPatch.InitShotStates);
        ModEvents.GameAwake.RegisterHandler(CustomEffectEnumManager.InitDefault);
        ModEvents.GameAwake.RegisterHandler(DelayLoadModuleManager.DelayLoad);
        //ModEvents.GameAwake.RegisterHandler(AssemblyLocator.Init);
        ModEvents.GameAwake.RegisterHandler(MultiActionUtils.SetMinEventArrays);
        ModEvents.GameStartDone.RegisterHandler(RegisterKFEnums);
        //DOES NOT WORK ON MULTIPLAYER? Patch to ItemClass.LateInitAll
        //ModEvents.GameStartDone.RegisterHandler(AnimationRiggingManager.ParseItemIDs);
        ModEvents.GameStartDone.RegisterHandler(MultiActionManager.PostloadCleanup);
        //ModEvents.GameStartDone.RegisterHandler(CustomEffectEnumManager.PrintResults);
        //ModEvents.GameUpdate.RegisterHandler(CommonUtilityPatch.ForceUpdateGC);
        HarmonyInstance = new Harmony(GetType().ToString());
        HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
    }

    private static void RegisterKFEnums(ref ModEvents.SGameStartDoneData _)
    {
        CustomEnums.onSelfMagzineDeplete = CustomEffectEnumManager.RegisterOrGetEnum<MinEventTypes>("onSelfMagzineDeplete");
        CustomEnums.onReloadAboutToStart = CustomEffectEnumManager.RegisterOrGetEnum<MinEventTypes>("onReloadAboutToStart");
        CustomEnums.onRechargeValueUpdate = CustomEffectEnumManager.RegisterOrGetEnum<MinEventTypes>("onRechargeValueUpdate");
        CustomEnums.onSelfItemSwitchMode = CustomEffectEnumManager.RegisterOrGetEnum<MinEventTypes>("onSelfItemSwitchMode");
        CustomEnums.onSelfBurstModeChanged = CustomEffectEnumManager.RegisterOrGetEnum<MinEventTypes>("onSelfBurstModeChanged");
        CustomEnums.onSelfFirstCVarSync = CustomEffectEnumManager.RegisterOrGetEnum<MinEventTypes>("onSelfFirstCVarSync");
        CustomEnums.onSelfHoldingItemAssemble = CustomEffectEnumManager.RegisterOrGetEnum<MinEventTypes>("onSelfHoldingItemAssemble");

        CustomEnums.ReloadSpeedRatioFPV2TPV = CustomEffectEnumManager.RegisterOrGetEnum<PassiveEffects>("ReloadSpeedRatioFPV2TPV");
        CustomEnums.RecoilSnappiness = CustomEffectEnumManager.RegisterOrGetEnum<PassiveEffects>("RecoilSnappiness");
        CustomEnums.RecoilReturnSpeed = CustomEffectEnumManager.RegisterOrGetEnum<PassiveEffects>("RecoilReturnSpeed");
        //CustomEnums.ProjectileImpactDamagePercentBlock = CustomEffectEnumManager.RegisterOrGetEnum<PassiveEffects>("ProjectileImpactDamagePercentBlock");
        //CustomEnums.ProjectileImpactDamagePercentEntity = CustomEffectEnumManager.RegisterOrGetEnum<PassiveEffects>("ProjectileImpactDamagePercentEntity");
        CustomEnums.PartialReloadCount = CustomEffectEnumManager.RegisterOrGetEnum<PassiveEffects>("PartialReloadCount");

        CustomEnums.CustomTaggedEffect = CustomEffectEnumManager.RegisterOrGetEnum<PassiveEffects>("CustomTaggedEffect");
        CustomEnums.KickDegreeHorizontalModifier = CustomEffectEnumManager.RegisterOrGetEnum<PassiveEffects>("KickDegreeHorizontalModifier");
        CustomEnums.KickDegreeVerticalModifier = CustomEffectEnumManager.RegisterOrGetEnum<PassiveEffects>("KickDegreeVerticalModifier");
        CustomEnums.WeaponErgonomics = CustomEffectEnumManager.RegisterOrGetEnum<PassiveEffects>("WeaponErgonomics");
        CustomEnums.RecoilCameraShakeStrength = CustomEffectEnumManager.RegisterOrGetEnum<PassiveEffects>("RecoilCameraShakeStrength");
        CustomEnums.BurstShotInterval = CustomEffectEnumManager.RegisterOrGetEnum<PassiveEffects>("BurstShotInterval");
        CustomEnums.MaxWeaponSpread = CustomEffectEnumManager.RegisterOrGetEnum<PassiveEffects>("MaxWeaponSpread");
    }
}

public class GearsImpl : IGearsModApi
{
    public void InitMod(IGearsMod modInstance)
    {

    }

    public void OnGlobalSettingsLoaded(IModGlobalSettings modSettings)
    {
        RecoilManager.InitRecoilSettings(modSettings);
    }

    public void OnWorldSettingsLoaded(IModWorldSettings worldSettings)
    {

    }
}

