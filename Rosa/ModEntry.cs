using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using Nickel.Common;
using Shockah.Kokoro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Flipbop.Rosa;

public sealed class ModEntry : SimpleMod
{
	internal static ModEntry Instance { get; private set; } = null!;
	internal readonly IRosaApi Api = new ApiImplementation();

	internal IHarmony Harmony { get; }
	internal IKokoroApi.IV2 KokoroApi { get; }
	//internal IDuoArtifactsApi? DuoArtifactsApi { get; }

	internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
	internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }

	internal IDeckEntry RosaDeck { get; }
	internal IPlayableCharacterEntryV2 RosaCharacter { get; }
	internal ISpriteEntry PatientIcon { get; }

	internal ISpriteEntry FrazzleIcon { get; }
	internal ISpriteEntry RebuttalIcon { get; }
	internal ISpriteEntry SuperBoostIcon { get; }
	internal ISpriteEntry SympathyIcon { get; }
	internal ISpriteEntry SilenceIcon { get; }
	internal ICardTraitEntry PatientTrait { get; }

	internal IStatusEntry FrazzleStatus { get; }
	internal IStatusEntry RebuttalStatus { get; }
	internal IStatusEntry SympathyStatus { get; }
	internal IStatusEntry SuperBoostStatus { get; }
	internal IStatusEntry SilenceStatus { get; }


	public IModHelper helper { get; }
	

	internal static IReadOnlyList<Type> CommonCardTypes { get; } = [
		typeof(TalkingPointsCard),
		typeof(HurtfulWordsCard),
		typeof(FeedEgoCard),
		typeof(PonderCard),
		typeof(CalmDownCard),
		typeof(DiplomacyCard),
		typeof(CensorCard),
		typeof(MuteCard),
		typeof(SquabbleCard),
	];

	internal static IReadOnlyList<Type> UncommonCardTypes { get; } = [
		typeof(PepTalkCard),
		typeof(IdleBanterCard),
		typeof(SetEmUpCard),
		typeof(GossipCard),
		typeof(BitingRemarksCard),
		typeof(SoftenUpCard), 
		typeof(RapportCard),
	];

	internal static IReadOnlyList<Type> RareCardTypes { get; } = [
		typeof(ToxicMentalityCard),
		typeof(RebuttalCard),
		typeof(BassBoosterCard),
		typeof(CallSignCard),
		typeof(TranquilityCard),
	];

	internal static IReadOnlyList<Type> SpecialCardTypes { get; } = [
		typeof(KnockEmDownCard),
	];

	internal static IEnumerable<Type> AllCardTypes { get; }
		= [..CommonCardTypes, ..UncommonCardTypes, ..RareCardTypes, typeof(RosaExeCard), ..SpecialCardTypes];

	internal static IReadOnlyList<Type> CommonArtifacts { get; } = [
		typeof(DiplomaArtifact),
		//typeof(EgoArtifact),
		typeof(NumberTicketArtifact),
		
	];

	internal static IReadOnlyList<Type> BossArtifacts { get; } = [
		typeof(PhilosophyArtifact),
		typeof(ThinkingCapArtifact),
		typeof(PeaceArtifact), 
		typeof(BrainFoodArtifact),
	];

	/*internal static IReadOnlyList<Type> DuoArtifacts { get; } = [
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
	];*/

	internal static IEnumerable<Type> AllArtifactTypes
		=> [..CommonArtifacts, ..BossArtifacts];

	internal static readonly IEnumerable<Type> RegisterableTypes
		= [..AllCardTypes, ..AllArtifactTypes];

	/*internal static readonly IEnumerable<Type> LateRegisterableTypes
		= DuoArtifacts;*/

	public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
	{
		PatientIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/Patient.png")); 
		FrazzleIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/Frazzle.png"));
		RebuttalIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/Rebuttal.png"));
		SuperBoostIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/SuperBoost.png"));
		SympathyIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/Sympathy.png"));
		SilenceIcon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Icons/Silence.png"));
		this.helper = helper;
		

		Instance = this;
		Harmony = helper.Utilities.Harmony;
		KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro")!.V2;
		//DuoArtifactsApi = helper.ModRegistry.GetApi<IDuoArtifactsApi>("Shockah.DuoArtifacts");

		helper.Events.OnModLoadPhaseFinished += (_, phase) =>
		{
			if (phase != ModLoadPhase.AfterDbInit)
				return;

			/*foreach (var registerableType in LateRegisterableTypes)
				AccessTools.DeclaredMethod(registerableType, nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);*/
		};

		this.AnyLocalizations = new JsonLocalizationProvider(
			tokenExtractor: new SimpleLocalizationTokenExtractor(),
			localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"i18n/main-{locale}.json").OpenRead()
		);
		this.Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
			new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(this.AnyLocalizations)
		);
		
		PatientTrait = helper.Content.Cards.RegisterTrait("Patient", new()
		{
			Name = this.AnyLocalizations.Bind(["cardtrait", "Patient", "name"]).Localize,
			Icon = (state, card) => PatientIcon.Sprite,
			Tooltips = (state, card) => [
				new GlossaryTooltip($"action.{Instance.Package.Manifest.UniqueName}::Patient")
				{
					Icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"assets/Icons/Patient.png")).Sprite,
					TitleColor = Colors.cardtrait,
					Title = Localizations.Localize(["cardTrait", "Patient", "name"]),
					Description = Localizations.Localize(["cardTrait", "Patient", "description"])
				}
			]
		});
		
		RosaDeck = helper.Content.Decks.RegisterDeck("Rosa", new()
		{
			Definition = new() { color = new("5b48ff"), titleColor = Colors.black },
			DefaultCardArt = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/Default.png")).Sprite,
			BorderSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/CardFrame.png")).Sprite,
			Name = this.AnyLocalizations.Bind(["character", "name"]).Localize
		});

		foreach (var registerableType in RegisterableTypes)
			AccessTools.DeclaredMethod(registerableType, nameof(IRegisterable.Register))?.Invoke(null, [package, helper]);
		
		
		RosaCharacter = helper.Content.Characters.V2.RegisterPlayableCharacter("Rosa", new()
		{
			Deck = RosaDeck.Deck,
			Description = this.AnyLocalizations.Bind(["character", "description"]).Localize,
			BorderSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/CharacterFrame.png")).Sprite,
			NeutralAnimation = new()
			{
				CharacterType = RosaDeck.UniqueName,
				LoopTag = "neutral",
				Frames = Enumerable.Range(0, 1)
					.Select(i => helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"assets/Character/Neutral/{i}.png")).Sprite)
					.ToList()
			},
			MiniAnimation = new()
			{
				CharacterType = RosaDeck.UniqueName,
				LoopTag = "mini",
				Frames = [
					helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Character/mini.png")).Sprite
				]
			},
			Starters = new()
			{
				cards = [
					new TalkingPointsCard(),
					new HurtfulWordsCard()
				]
			},
			SoloStarters = new StarterDeck()
			{
				cards = [
					new TalkingPointsCard(),
					new HurtfulWordsCard(),
					new SquabbleCard(),
					new CalmDownCard(),
					new BasicShieldColorless(),
					new DodgeColorless()
					]
			},
			ExeCardType = typeof(RosaExeCard)
		});
		
		helper.Content.Characters.V2.RegisterCharacterAnimation(new()
		{
			CharacterType = RosaDeck.UniqueName,
			LoopTag = "gameover",
			Frames = Enumerable.Range(0, 1)
				.Select(i => helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"assets/Character/GameOver/{i}.png")).Sprite)
				.ToList()
		});
		/*helper.Content.Characters.V2.RegisterCharacterAnimation(new()
		{
			CharacterType = RosaDeck.UniqueName,
			LoopTag = "squint",
			Frames = Enumerable.Range(0, 3)
				.Select(i => helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"assets/Character/Squint/{i}.png")).Sprite)
				.ToList()
		});
		helper.Content.Characters.V2.RegisterCharacterAnimation(new()
		{
			CharacterType = RosaDeck.UniqueName,
			LoopTag = "nervous",
			Frames = Enumerable.Range(0, 5)
				.Select(i => helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile($"assets/Character/Nervous/{i}.png")).Sprite)
				.ToList()
		});*/

		FrazzleStatus = ModEntry.Instance.Helper.Content.Statuses.RegisterStatus("Frazzle", new()
		{
			Definition = new()
			{
				icon = FrazzleIcon.Sprite,
				color = new("312351"),
				isGood = false,
			},
			Name = AnyLocalizations.Bind([ "status", "Frazzle", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "Frazzle", "description"])
				.Localize
		});
		RebuttalStatus = ModEntry.Instance.Helper.Content.Statuses.RegisterStatus("Rebuttal", new()
		{
			Definition = new()
			{
				icon = RebuttalIcon.Sprite,
				color = new("312351"),
				isGood = true,
			},
			Name = AnyLocalizations.Bind([ "status", "Rebuttal", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "Rebuttal", "description"])
				.Localize
		});
		SuperBoostStatus = ModEntry.Instance.Helper.Content.Statuses.RegisterStatus("SuperBoost", new()
		{
			Definition = new()
			{
				icon = SuperBoostIcon.Sprite,
				color = new("312351"),
				isGood = true,
			},
			Name = AnyLocalizations.Bind([ "status", "SuperBoost", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "SuperBoost", "description"])
				.Localize
		});
		SympathyStatus = ModEntry.Instance.Helper.Content.Statuses.RegisterStatus("Sympathy", new()
		{
			Definition = new()
			{
				icon = SympathyIcon.Sprite,
				color = new("312351"),
				isGood = false,
			},
			Name = AnyLocalizations.Bind([ "status", "Sympathy", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "Sympathy", "description"])
				.Localize
		});
		SilenceStatus = ModEntry.Instance.Helper.Content.Statuses.RegisterStatus("Silence", new()
		{
			Definition = new()
			{
				icon = SilenceIcon.Sprite,
				color = new("312351"),
				isGood = false,
			},
			Name = AnyLocalizations.Bind([ "status", "Silence", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "Silence", "description"])
				.Localize
		});

		helper.ModRegistry.AwaitApi<IMoreDifficultiesApi>(
			"TheJazMaster.MoreDifficulties",
			new SemanticVersion(1, 3, 0),
			api => api.RegisterAltStarters(
				deck: RosaDeck.Deck,
				starterDeck: new StarterDeck
				{
					cards = [
						new CalmDownCard(),
						new SquabbleCard()
					]
				}
			)
		);
		
		_ = new PatientManager();
		_ = new FrazzleManager();
		_ = new RebuttalManager();
		_ = new SuperBoostManager();
		_ = new SympathyManager();
		
		/*_ = new DialogueExtensions();
		_ = new CombatDialogue();
		_ = new EventDialogue();
		_ = new CardDialogue();*/
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
