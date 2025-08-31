using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using daisyowl.text;
using Shockah.Kokoro;

namespace Flipbop.Cleo;

internal sealed class CleoJohnsonArtifact : Artifact, IRegisterable
{
	internal static Spr RPYDShares;
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		
		if (ModEntry.Instance.DuoArtifactsApi is not { } api)
			return;
		if (ModEntry.Instance.IJohnsonApi is not { } johnsonApi)
			return;
		RPYDShares = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/Shares.png")).Sprite;
		helper.Content.Artifacts.RegisterArtifact("CleoJohnson", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = api.DuoArtifactVanillaDeck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/Duo/CleoJohnson.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Duo", "CleoJohnson", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Duo", "CleoJohnson", "description"]).Localize
		});

		api.RegisterDuoArtifact(MethodBase.GetCurrentMethod()!.DeclaringType!, [ModEntry.Instance.CleoDeck.Deck, johnsonApi.JohnsonDeck.Deck]);

		Hold0Card.Register(package, helper);
		Hold1Card.Register(package, helper);
		Hold2Card.Register(package, helper);
		Hold3Card.Register(package, helper);
		ReturnOnInvestmentCard.Register(package, helper);
	}

	public override List<Tooltip>? GetExtraTooltips()
		=> [new TTCard { card = new Hold0Card() },
			new TTCard { card = new Hold1Card() },
			new TTCard { card = new Hold2Card() },
			new TTCard { card = new Hold3Card() },
			new TTCard { card = new ReturnOnInvestmentCard() }];

	public override void OnReceiveArtifact(State state)
	{
		base.OnReceiveArtifact(state);
		state.deck.Add(new Hold0Card());
	}
}

internal sealed class Hold0Card : Card
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.DuoArtifactsApi!.DuoArtifactVanillaDeck,
				rarity = ModEntry.GetCardRarity(MethodBase.GetCurrentMethod()!.DeclaringType!),
				upgradesTo = [Upgrade.A, Upgrade.B],
				dontOffer = true,
			},
			Art = CleoJohnsonArtifact.RPYDShares,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Duo", "Hold0", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "ED2938",
			cost = 0,
			singleUse = true,
			unplayable = upgrade == Upgrade.None,
			description = ModEntry.Instance.Localizations.Localize(["card", "Duo", "Hold0", "description", upgrade.ToString()])
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=>
		[
			new AAddCard() {amount = 0, card = new Hold1Card(), destination = CardDestination.Exhaust}
		];
	
	
}internal sealed class Hold1Card : Card
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.DuoArtifactsApi!.DuoArtifactVanillaDeck,
				rarity = ModEntry.GetCardRarity(MethodBase.GetCurrentMethod()!.DeclaringType!),
				upgradesTo = [Upgrade.A, Upgrade.B],
				dontOffer = true,
			},
			Art = CleoJohnsonArtifact.RPYDShares,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Duo", "Hold1", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "B25F4A",
			cost = 0,
			singleUse = true,
			unplayable = upgrade == Upgrade.None,
			description = ModEntry.Instance.Localizations.Localize(["card", "Duo", "Hold1", "description", upgrade.ToString()])
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=>
		[
			new AAddCard() {amount = 1, card = new Hold2Card(), destination = CardDestination.Exhaust}
		];
}internal sealed class Hold2Card : Card
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.DuoArtifactsApi!.DuoArtifactVanillaDeck,
				rarity = ModEntry.GetCardRarity(MethodBase.GetCurrentMethod()!.DeclaringType!),
				upgradesTo = [Upgrade.A, Upgrade.B],
				dontOffer = true,
			},
			Art = CleoJohnsonArtifact.RPYDShares,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Duo", "Hold2", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "77945C",
			cost = 0,
			singleUse = true,
			unplayable = upgrade == Upgrade.None,
			description = ModEntry.Instance.Localizations.Localize(["card", "Duo", "Hold2", "description", upgrade.ToString()])
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=>
		[
			new AAddCard() {amount = 1, card = new Hold3Card(), destination = CardDestination.Exhaust}
		];
}internal sealed class Hold3Card : Card
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.DuoArtifactsApi!.DuoArtifactVanillaDeck,
				rarity = ModEntry.GetCardRarity(MethodBase.GetCurrentMethod()!.DeclaringType!),
				upgradesTo = [Upgrade.A, Upgrade.B],
				dontOffer = true,
			},
			Art = CleoJohnsonArtifact.RPYDShares,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Duo", "Hold3", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "3BCA6D",
			cost = 0,
			singleUse = true,
			unplayable = upgrade == Upgrade.None,
			description = ModEntry.Instance.Localizations.Localize(["card", "Duo", "Hold3", "description", upgrade.ToString()])
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=>
		[
			new AAddCard() {amount = 1, card = new ReturnOnInvestmentCard(), destination = CardDestination.Exhaust}
		];
}internal sealed class ReturnOnInvestmentCard : Card
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.DuoArtifactsApi!.DuoArtifactVanillaDeck,
				rarity = ModEntry.GetCardRarity(MethodBase.GetCurrentMethod()!.DeclaringType!),
				upgradesTo = [Upgrade.A, Upgrade.B],
				dontOffer = true,
			},
			Art = CleoJohnsonArtifact.RPYDShares,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Duo", "ReturnOnInvestment", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "00FF7F",
			cost = 1,
			exhaust = true,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.A =>
			[
				new AAttack { damage = GetDmg(s, 9) },
			],
			Upgrade.B => [
				new AStatus { targetPlayer = true, status = Status.shield, statusAmount = 2 },
				new AAttack { damage = GetDmg(s, 7) },
			],
			_ => [
				new AAttack { damage = GetDmg(s, 7) },
			]
		};
	
}