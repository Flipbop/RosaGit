using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using Nickel.Common;
using Shockah.Kokoro;
using System;
using System.Collections.Generic;
using System.Linq;
using Shockah.Johnson;
using Shockah.Soggins;

namespace Flipbop.Cleo;

public sealed class ModEntry : SimpleMod
{
	internal static ModEntry Instance { get; private set; } = null!;
	internal readonly ICleoApi Api = new ApiImplementation();

	internal IHarmony Harmony { get; }
	internal IKokoroApi.IV2 KokoroApi { get; }
	internal IDuoArtifactsApi? DuoArtifactsApi { get; }
	internal IJohnsonApi? IJohnsonApi { get; private set; }

	internal ISogginsApi? ISogginsApi { get; private set; }

	internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
	internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }

	internal IDeckEntry CleoDeck { get; }
	internal IPlayableCharacterEntryV2 CleoCharacter { get; }
	internal INonPlayableCharacterEntryV2 KiwiCharacter { get; }
	internal ISpriteEntry ImproveAIcon { get; }
	internal ISpriteEntry ImproveBIcon { get; }
	internal ISpriteEntry ImpairedIcon { get; }
	internal ISpriteEntry ImproveASelfIcon { get; }
	internal ISpriteEntry ImproveBSelfIcon { get; }
	internal ISpriteEntry ImpairSelfIcon { get; }
	internal ISpriteEntry ImproveAHandIcon { get; }
	internal ISpriteEntry ImproveBHandIcon { get; }
	internal ISpriteEntry ImpairHandIcon { get; }
	internal ISpriteEntry ImprovedIcon { get; }
	internal ISpriteEntry DiscountHandIcon { get; }
	internal ISpriteEntry UpgradesInHandIcon { get; }
	internal ISpriteEntry UpgradesInDrawIcon { get; }

	internal ISpriteEntry UpgradesInDiscardIcon { get; }
	internal ISpriteEntry UpgradesInExhaustIcon { get; }
	internal ISpriteEntry ImpairCostIcon { get; }


	internal ICardTraitEntry ImprovedATrait { get; }
	internal ICardTraitEntry ImprovedBTrait { get; }
	internal ICardTraitEntry ImpairedTrait { get; }
	public IModHelper helper { get; }
	
	public ISpriteEntry CleanSlateSprite { get; }
	
	public ISpriteEntry TurtleShotSprite { get; }

	internal static IReadOnlyList<Type> CommonCardTypes { get; } = [
		typeof(QuickBoostCard),
		typeof(TurtleShotCard),
		typeof(ChoicesCard),
		typeof(MemoryRecoveryCard),
		typeof(ShuffleUpgradeCard),
		typeof(ResourceSwapCard),
		typeof(ReroutePowerCard),
		typeof(RewriteCard),
		typeof(SlipShotCard),
	];

	internal static IReadOnlyList<Type> UncommonCardTypes { get; } = [
		typeof(PowerSurgeCard),
		typeof(ImprovedCannonCard),
		typeof(DoItYourselfCard),
		typeof(RepairedGlassesCard),
		typeof(ScalpedPartsCard),
		typeof(MaximumEffortCard), 
		typeof(NecessarySacrificeCard),
	];

	internal static IReadOnlyList<Type> RareCardTypes { get; } = [
		typeof(SeekerBarrageCard),
		typeof(PermaFixCard),
		typeof(CleanSlateCard),
		typeof(ApologizeNextLoopCard),
		typeof(DefensivePositionsCard),
	];

	internal static IReadOnlyList<Type> SpecialCardTypes { get; } = [
		typeof(SmallRepairsCard),
	];

	internal static IEnumerable<Type> AllCardTypes { get; }
		= [..CommonCardTypes, ..UncommonCardTypes, ..RareCardTypes, typeof(CleoExeCard), ..SpecialCardTypes];

	internal static IReadOnlyList<Type> CommonArtifacts { get; } = [
		typeof(EnhancedToolsArtifact),
		typeof(ReusableMaterialsArtifact),
		typeof(KickstartArtifact),
		typeof(MagnifiedLasersArtifact),
		typeof(UpgradedTerminalArtifact), 
	];

	internal static IReadOnlyList<Type> BossArtifacts { get; } = [
		typeof(RetainerArtifact),
		typeof(ExpensiveEquipmentArtifact),
		typeof(PowerEchoArtifact), 
	];

	internal static IReadOnlyList<Type> DuoArtifacts { get; } = [
		typeof(CleoBooksArtifact),
		typeof(CleoCatArtifact),
		typeof(CleoDizzyArtifact),
		typeof(CleoDrakeArtifact),
		typeof(CleoIsaacArtifact),
		typeof(CleoMaxArtifact),
		typeof(CleoPeriArtifact),
		typeof(CleoRiggsArtifact), 
		typeof(CleoJohnsonArtifact),
		typeof(CleoSogginsArtifact),
	];

	internal static IEnumerable<Type> AllArtifactTypes
		=> [..CommonArtifacts, ..BossArtifacts];

	internal static readonly IEnumerable<Type> RegisterableTypes
		= [..AllCardTypes, ..AllArtifactTypes];

	internal static readonly IEnumerable<Type> LateRegisterableTypes
		= DuoArtifacts;

	public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
	{
		ISpriteEntry improvedSpr = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/Improved.png")); 
		ISpriteEntry impairedSpr = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/Impaired.png"));
		CleanSlateSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/CleanSlate.png"));
		TurtleShotSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/TurtleShot.png"));
		this.helper = helper;

		Instance = this;
		Harmony = helper.Utilities.Harmony;
		KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!.V2;
		DuoArtifactsApi = helper.ModRegistry.GetApi<IDuoArtifactsApi>("Shockah.DuoArtifacts");

		helper.Events.OnModLoadPhaseFinished += (_, phase) =>
		{
			if (phase != ModLoadPhase.AfterDbInit)
				return;
			
			IJohnsonApi = helper.ModRegistry.GetApi<IJohnsonApi>("Shockah.Johnson");
			ISogginsApi = helper.ModRegistry.GetApi<ISogginsApi>("Shockah.Soggins");

			foreach (var registerableType in LateRegisterableTypes)
				AccessTools.DeclaredMethod(registerableType, nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);
		};

		this.AnyLocalizations = new JsonLocalizationProvider(
			tokenExtractor: new SimpleLocalizationTokenExtractor(),
			localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"i18n/main-{locale}.json").OpenRead()
		);
		this.Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
			new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(this.AnyLocalizations)
		);
		
		ImprovedATrait = helper.Content.Cards.RegisterTrait("Improved A", new()
		{
			Name = this.AnyLocalizations.Bind(["cardtrait", "ImprovedA", "name"]).Localize,
			Icon = (state, card) => improvedSpr.Sprite,
			Tooltips = (state, card) => [
				new GlossaryTooltip($"action.{Instance.Package.Manifest.UniqueName}::Improved A")
				{
					Icon = improvedSpr.Sprite,
					TitleColor = Colors.cardtrait,
					Title = Localizations.Localize(["cardTrait", "ImprovedA", "name"]),
					Description = Localizations.Localize(["cardTrait", "ImprovedA", "description"])
				}
			]
		});
		ImprovedBTrait = helper.Content.Cards.RegisterTrait("Improved B", new()
		{
			Name = this.AnyLocalizations.Bind(["cardtrait", "ImprovedB", "name"]).Localize,
			Icon = (state, card) => improvedSpr.Sprite,
			Tooltips = (state, card) => [
				new GlossaryTooltip($"action.{Instance.Package.Manifest.UniqueName}::Improved B")
				{
					Icon = improvedSpr.Sprite,
					TitleColor = Colors.cardtrait,
					Title = Localizations.Localize(["cardTrait", "ImprovedB", "name"]),
					Description = Localizations.Localize(["cardTrait", "ImprovedB", "description"])
				}
			]
		});
		ImpairedTrait = helper.Content.Cards.RegisterTrait("ImpairedTrait", new()
		{
			Name = this.AnyLocalizations.Bind(["cardtrait", "Impaired", "name"]).Localize,
			Icon = (state, card) => impairedSpr.Sprite,
			Tooltips = (state, card) => [
				new GlossaryTooltip($"action.{Instance.Package.Manifest.UniqueName}::Impaired")
				{
					Icon = impairedSpr.Sprite,
					TitleColor = Colors.cardtrait,
					Title = Localizations.Localize(["cardTrait", "Impaired", "name"]),
					Description = Localizations.Localize(["cardTrait", "Impaired", "description"])
				}
			]
		});
		

		DynamicWidthCardAction.ApplyPatches(Harmony, logger);
		DontLetCleoBecomeAnNPC.Apply(Harmony);
		
		CleoDeck = helper.Content.Decks.RegisterDeck("Cleo", new()
		{
			Definition = new() { color = new("8A3388"), titleColor = Colors.white },
			DefaultCardArt = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/Default.png")).Sprite,
			BorderSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/CardFrame.png")).Sprite,
			Name = this.AnyLocalizations.Bind(["character", "name"]).Localize
		});

		foreach (var registerableType in RegisterableTypes)
			AccessTools.DeclaredMethod(registerableType, nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);

		KiwiCharacter = helper.Content.Characters.V2.RegisterNonPlayableCharacter("Kiwi", new NonPlayableCharacterConfigurationV2()
		{
			CharacterType = "kiwi",
			Name = AnyLocalizations.Bind(["character", "nameKiwi"]).Localize,
			
		});
		
		CleoCharacter = helper.Content.Characters.V2.RegisterPlayableCharacter("Cleo", new()
		{
			Deck = CleoDeck.Deck,
			Description = this.AnyLocalizations.Bind(["character", "description"]).Localize,
			BorderSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/CharacterFrame.png")).Sprite,
			NeutralAnimation = new()
			{
				CharacterType = CleoDeck.UniqueName,
				LoopTag = "neutral",
				Frames = Enumerable.Range(0, 5)
					.Select(i => helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"assets/Character/Neutral/{i}.png")).Sprite)
					.ToList()
			},
			MiniAnimation = new()
			{
				CharacterType = CleoDeck.UniqueName,
				LoopTag = "mini",
				Frames = [
					helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Character/mini.png")).Sprite
				]
			},
			Starters = new()
			{
				cards = [
					new QuickBoostCard(),
					new TurtleShotCard()
				]
			},
			SoloStarters = new StarterDeck()
			{
				cards = [
					new QuickBoostCard(),
					new TurtleShotCard(),
					new ReroutePowerCard(),
					new ShuffleUpgradeCard(),
					new CannonColorless(),
					new DodgeColorless()
					]
			},
			ExeCardType = typeof(CleoExeCard)
		});
		
		helper.Content.Characters.V2.RegisterCharacterAnimation(new CharacterAnimationConfigurationV2()
		{
			CharacterType = KiwiCharacter.CharacterType,
			LoopTag = "neutral",
			Frames = Enumerable.Range(0, 1)
				.Select(i =>
					helper.Content.Sprites
						.RegisterSprite(package.PackageRoot.GetRelativeFile($"assets/Character/Kiwi/{i}.png")).Sprite)
				.ToList()
		});
		helper.Content.Characters.V2.RegisterCharacterAnimation(new()
		{
			CharacterType = CleoDeck.UniqueName,
			LoopTag = "gameover",
			Frames = Enumerable.Range(0, 1)
				.Select(i => helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"assets/Character/GameOver/{i}.png")).Sprite)
				.ToList()
		});
		helper.Content.Characters.V2.RegisterCharacterAnimation(new()
		{
			CharacterType = CleoDeck.UniqueName,
			LoopTag = "squint",
			Frames = Enumerable.Range(0, 3)
				.Select(i => helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"assets/Character/Squint/{i}.png")).Sprite)
				.ToList()
		});
		helper.Content.Characters.V2.RegisterCharacterAnimation(new()
		{
			CharacterType = CleoDeck.UniqueName,
			LoopTag = "explain",
			Frames = Enumerable.Range(0, 5)
				.Select(i => helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"assets/Character/Explain/{i}.png")).Sprite)
				.ToList()
		});
		helper.Content.Characters.V2.RegisterCharacterAnimation(new()
		{
			CharacterType = CleoDeck.UniqueName,
			LoopTag = "nervous",
			Frames = Enumerable.Range(0, 5)
				.Select(i => helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"assets/Character/Nervous/{i}.png")).Sprite)
				.ToList()
		});

		ImproveAIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/ImproveA.png"));
		ImproveBIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/ImproveB.png"));
		ImpairedIcon = impairedSpr;
		ImproveASelfIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/ImproveASelf.png"));
		ImproveBSelfIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/ImproveBSelf.png"));
		ImpairSelfIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/ImpairSelf.png"));
		ImproveAHandIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/ImproveAHand.png"));
		ImproveBHandIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/ImproveBHand.png"));
		ImpairHandIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/ImpairHand.png"));
		ImprovedIcon = improvedSpr;
		DiscountHandIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/DiscountHand.png"));
		UpgradesInHandIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/UpgradesInHand.png"));
		UpgradesInDrawIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/UpgradesInDraw.png"));
		UpgradesInDiscardIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/UpgradesInDiscard.png"));
		UpgradesInExhaustIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/UpgradesInExhaust.png"));
		ImpairCostIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/ImpairedCost.png"));

		helper.ModRegistry.AwaitApi<IMoreDifficultiesApi>(
			"TheJazMaster.MoreDifficulties",
			new SemanticVersion(1, 3, 0),
			api => api.RegisterAltStarters(
				deck: CleoDeck.Deck,
				starterDeck: new StarterDeck
				{
					cards = [
						new ShuffleUpgradeCard(),
						new SlipShotCard()
					]
				}
			)
		);
		helper.Events.RegisterBeforeArtifactsHook(nameof(Artifact.OnCombatStart), (State state, Combat combat) =>
		{
				foreach (Character crew in state.characters)
				{
					if (crew.type == CleoCharacter.CharacterType)
					{
						if (combat.otherShip.ai is Shopkeep)
						{ 
							state.rewardsQueue.Add(new BRINGITBACK());
						}
					}
				}
			});
		_ = new ImprovedAManager();
		_ = new ImprovedBManager();
		_ = new ImpairedManager();
		_ = new ImpairedCostManager();
		_ = new DialogueExtensions();
		_ = new CombatDialogue();
		_ = new EventDialogue();
		_ = new CardDialogue();
	}

	public override object? GetApi(IModManifest requestingMod)
		=> new ApiImplementation();

	internal static Rarity GetCardRarity(Type type)
	{
		if (RareCardTypes.Contains(type))
			return Rarity.rare;
		if (UncommonCardTypes.Contains(type))
			return Rarity.uncommon;
		return Rarity.common;
	}

	internal static ArtifactPool[] GetArtifactPools(Type type)
	{
		if (BossArtifacts.Contains(type))
			return [ArtifactPool.Boss];
		if (CommonArtifacts.Contains(type))
			return [ArtifactPool.Common];
		return [];
	}
}
