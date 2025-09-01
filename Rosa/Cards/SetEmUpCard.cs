using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace Flipbop.Rosa;

internal sealed class SetEmUpCard : Card, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.RosaDeck.Deck,
				rarity = ModEntry.GetCardRarity(MethodBase.GetCurrentMethod()!.DeclaringType!),
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/SetEmUp.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "SetEmUp", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "FFFFFF",
			cost = 1,
			singleUse = true,
			description = ModEntry.Instance.Localizations.Localize(["card", "SetEmUp", "description", upgrade.ToString()])
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.A =>
			[
				new AStatus { targetPlayer = false, status = Status.boost, statusAmount = 1 },
				new AAddCard { amount = 1, card = new KnockEmDownCard { upgrade = Upgrade.A }, destination = CardDestination.Discard},
			],
			Upgrade.B => [
				new AStatus { targetPlayer = false, status = Status.boost, statusAmount = 2 },
				new AAddCard { amount = 1, card = new KnockEmDownCard { upgrade = Upgrade.B }, destination = CardDestination.Discard},
			],
			_ =>
			[
				new AStatus { targetPlayer = false, status = Status.boost, statusAmount = 1 },
				new AAddCard() {amount = 1, destination = CardDestination.Discard, card = new KnockEmDownCard()}
			]
		};
	
		
}
